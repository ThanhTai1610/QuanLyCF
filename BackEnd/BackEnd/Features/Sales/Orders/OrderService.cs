using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Orders;

public class OrderService
{
    // Đơn còn hoạt động (chưa hoàn thành/huỷ)
    private static readonly string[] TrangThaiActive = { "ChoXacNhan", "DangPha" };

    private readonly QuanLyCFDbContext _db;
    public OrderService(QuanLyCFDbContext db) => _db = db;

    /// <summary>Danh sách món đang bán (kèm size) cho màn hình đặt món.</summary>
    public async Task<List<MenuItemDto>> LayMenuAsync() =>
        await _db.SanPhams.Where(x => x.TrangThaiBan)
            .Include(x => x.DanhMuc).Include(x => x.KichCos)
            .OrderBy(x => x.TenSanPham)
            .Select(x => new MenuItemDto(
                x.MaSanPham, x.TenSanPham, x.DanhMuc != null ? x.DanhMuc.TenDanhMuc : null,
                x.GiaBan, x.HinhAnh,
                x.KichCos.Where(s => s.TrangThaiHoatDong)
                    .Select(s => new MenuSizeDto(s.MaKichCo, s.TenKichCo, s.GiaCongThem)).ToList()))
            .ToListAsync();

    /// <summary>Tất cả đơn đang hoạt động (để hiển thị trên bàn).</summary>
    public async Task<List<OrderDto>> LayDonActiveAsync()
    {
        var dons = await _db.DonHangs
            .Where(d => TrangThaiActive.Contains(d.TrangThaiDon))
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Include(d => d.ChiTiets).ThenInclude(c => c.KichCo)
            .OrderBy(d => d.ThoiGianTao)
            .ToListAsync();
        return dons.Select(Map).ToList();
    }

    public async Task<(OrderDto? Data, string? Error)> TaoDonAsync(CreateOrderRequest req, int? maNhanVien)
    {
        if (req.Items is null || req.Items.Count == 0) return (null, "Đơn phải có ít nhất 1 món.");
        var ban = await _db.Bans.FindAsync(req.MaBan);
        if (ban is null) return (null, "Bàn không tồn tại.");

        var spIds = req.Items.Select(i => i.MaSanPham).Distinct().ToList();
        var spMap = await _db.SanPhams.Include(s => s.KichCos)
            .Where(s => spIds.Contains(s.MaSanPham))
            .ToDictionaryAsync(s => s.MaSanPham);

        var don = new DonHang
        {
            MaBan = req.MaBan,
            MaNhanVien = maNhanVien,
            LoaiDonHang = "DineIn",
            TrangThaiDon = "ChoXacNhan",
            GhiChuDonHang = req.GhiChuDonHang,
            ThoiGianTao = DateTime.UtcNow,
            ThoiGianCapNhat = DateTime.UtcNow,
        };

        decimal tong = 0;
        foreach (var it in req.Items)
        {
            if (it.SoLuong <= 0) continue;
            if (!spMap.TryGetValue(it.MaSanPham, out var sp)) return (null, "Có sản phẩm không tồn tại.");
            var donGia = sp.GiaBan;
            KichCoSanPham? kc = null;
            if (it.MaKichCo is { } kcId)
            {
                kc = sp.KichCos.FirstOrDefault(k => k.MaKichCo == kcId);
                if (kc is not null) donGia += kc.GiaCongThem;
            }
            var thanhTien = donGia * it.SoLuong;
            tong += thanhTien;
            don.ChiTiets.Add(new ChiTietDonHang
            {
                MaSanPham = it.MaSanPham,
                MaKichCo = kc?.MaKichCo,
                SoLuong = it.SoLuong,
                DonGia = donGia,
                ThanhTien = thanhTien,
                GhiChuMon = it.GhiChuMon,
                TrangThaiBep = "ChoLam",
            });
        }
        if (don.ChiTiets.Count == 0) return (null, "Đơn phải có ít nhất 1 món.");

        don.TongTienHang = tong;
        don.ThanhTien = tong;
        _db.DonHangs.Add(don);
        ban.TrangThai = "CoKhach";
        await _db.SaveChangesAsync();

        await _db.Entry(don).Reference(d => d.Ban).LoadAsync();
        foreach (var c in don.ChiTiets)
        {
            await _db.Entry(c).Reference(x => x.SanPham).LoadAsync();
            if (c.MaKichCo is not null) await _db.Entry(c).Reference(x => x.KichCo).LoadAsync();
        }
        return (Map(don), null);
    }

    /// <summary>Đổi bàn: bàn trống thì chuyển đơn; bàn đã có đơn thì ghép bàn (giữ đơn riêng).</summary>
    public async Task<(MoveOrderResult? Data, string? Error)> DoiBanAsync(int maDon, int maBanMoi)
    {
        var don = await _db.DonHangs.Include(d => d.Ban).FirstOrDefaultAsync(d => d.MaDonHang == maDon);
        if (don is null) return (null, "Đơn không tồn tại.");
        if (!TrangThaiActive.Contains(don.TrangThaiDon)) return (null, "Đơn không còn hoạt động.");
        if (don.MaBan == maBanMoi) return (null, "Đơn đã ở bàn này.");

        var banCu = don.Ban;
        var banMoi = await _db.Bans.FindAsync(maBanMoi);
        if (banMoi is null) return (null, "Bàn mới không tồn tại.");

        var coDonOBanMoi = await _db.DonHangs
            .AnyAsync(d => d.MaBan == maBanMoi && TrangThaiActive.Contains(d.TrangThaiDon));

        if (coDonOBanMoi)
        {
            // Ghép 2 bàn thành nhóm, giữ đơn riêng. Bàn mới (đã có khách trước) làm bàn chính.
            banMoi.MaBanChinh = null;
            if (banCu is not null)
            {
                banCu.MaBanChinh = maBanMoi;
                var con = await _db.Bans.Where(b => b.MaBanChinh == banCu.MaBan).ToListAsync();
                foreach (var c in con) c.MaBanChinh = maBanMoi;
            }
            await _db.SaveChangesAsync();
            return (new MoveOrderResult("merged", banCu?.TenBan, banMoi.TenBan), null);
        }

        // Bàn mới trống → chuyển đơn sang
        don.MaBan = maBanMoi;
        don.ThoiGianCapNhat = DateTime.UtcNow;
        banMoi.TrangThai = "CoKhach";
        if (banCu is not null)
        {
            var conDon = await _db.DonHangs.AnyAsync(d =>
                d.MaBan == banCu.MaBan && d.MaDonHang != maDon && TrangThaiActive.Contains(d.TrangThaiDon));
            if (!conDon) banCu.TrangThai = "Trong";
        }
        await _db.SaveChangesAsync();
        return (new MoveOrderResult("moved", banCu?.TenBan, banMoi.TenBan), null);
    }

    public async Task<(bool Ok, string? Error)> HuyDonAsync(int maDon, string? lyDo)
    {
        var don = await _db.DonHangs.Include(d => d.Ban).FirstOrDefaultAsync(d => d.MaDonHang == maDon);
        if (don is null) return (false, "Đơn không tồn tại.");

        don.TrangThaiDon = "Huy";
        don.LyDoHuy = lyDo;
        don.ThoiGianCapNhat = DateTime.UtcNow;
        if (don.Ban is not null)
        {
            var conDon = await _db.DonHangs.AnyAsync(d =>
                d.MaBan == don.MaBan && d.MaDonHang != maDon && TrangThaiActive.Contains(d.TrangThaiDon));
            if (!conDon) don.Ban.TrangThai = "Trong";
        }
        await _db.SaveChangesAsync();
        return (true, null);
    }

    private static OrderDto Map(DonHang d) => new(
        d.MaDonHang, d.MaBan, d.Ban?.TenBan, d.LoaiDonHang, d.TrangThaiDon, d.ThanhTien,
        d.ChiTiets.Sum(c => c.SoLuong), d.ThoiGianTao,
        d.ChiTiets.Select(c => new OrderItemDto(
            c.MaChiTiet, c.MaSanPham, c.SanPham?.TenSanPham ?? "(món)", c.KichCo?.TenKichCo,
            c.SoLuong, c.DonGia, c.ThanhTien, c.GhiChuMon)).ToList());
}
