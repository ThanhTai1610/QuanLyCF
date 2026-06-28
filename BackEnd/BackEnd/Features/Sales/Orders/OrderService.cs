using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Orders;

public class OrderService
{
    // Đơn còn hoạt động (chưa hoàn thành/huỷ)
    private static readonly string[] TrangThaiActive = { "ChoXacNhan", "DangPha" };

    private readonly QuanLyCFDbContext _db;
    private readonly Promotions.PromotionService _promo;
    public OrderService(QuanLyCFDbContext db, Promotions.PromotionService promo)
    {
        _db = db; _promo = promo;
    }

    /// <summary>Danh sách món đang bán (kèm size) cho màn hình đặt món.</summary>
    public async Task<List<MenuItemDto>> LayMenuAsync() =>
        await _db.SanPhams.Where(x => x.TrangThaiBan)
            .Include(x => x.DanhMuc).Include(x => x.KichCos)
            .OrderBy(x => x.TenSanPham)
            .Select(x => new MenuItemDto(
                x.MaSanPham, x.TenSanPham, x.DanhMuc != null ? x.DanhMuc.TenDanhMuc : null,
                x.GiaBan, x.HinhAnh, x.KieuMon, x.MoTa, x.LaMonNoiBat,
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

    /// <summary>Tạo + lưu đơn hàng (entity). Dùng chung cho tạo đơn và thanh toán.</summary>
    private async Task<(DonHang? Don, string? Error)> TaoDonHangAsync(
        int? maBan, List<OrderLineRequest>? items, string? ghiChu, int? maNhanVien)
    {
        if (items is null || items.Count == 0) return (null, "Đơn phải có ít nhất 1 món.");
        Ban? ban = null;
        if (maBan is { } mb)
        {
            ban = await _db.Bans.FindAsync(mb);
            if (ban is null) return (null, "Bàn không tồn tại.");
        }

        var spIds = items.Select(i => i.MaSanPham).Distinct().ToList();
        var spMap = await _db.SanPhams.Include(s => s.KichCos)
            .Where(s => spIds.Contains(s.MaSanPham))
            .ToDictionaryAsync(s => s.MaSanPham);

        var don = new DonHang
        {
            MaBan = maBan,
            MaNhanVien = maNhanVien,
            LoaiDonHang = maBan is null ? "TakeAway" : "DineIn",
            TrangThaiDon = "ChoXacNhan",
            GhiChuDonHang = ghiChu,
            ThoiGianTao = DateTime.UtcNow,
            ThoiGianCapNhat = DateTime.UtcNow,
        };

        decimal tong = 0;
        foreach (var it in items)
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
        if (ban is not null) ban.TrangThai = "CoKhach";
        await _db.SaveChangesAsync();
        return (don, null);
    }

    public async Task<(OrderDto? Data, string? Error)> TaoDonAsync(CreateOrderRequest req, int? maNhanVien)
    {
        var (don, err) = await TaoDonHangAsync(req.MaBan, req.Items, req.GhiChuDonHang, maNhanVien);
        if (err is not null) return (null, err);

        await _db.Entry(don!).Reference(d => d.Ban).LoadAsync();
        foreach (var c in don!.ChiTiets)
        {
            await _db.Entry(c).Reference(x => x.SanPham).LoadAsync();
            if (c.MaKichCo is not null) await _db.Entry(c).Reference(x => x.KichCo).LoadAsync();
        }
        return (Map(don), null);
    }

    /// <summary>Tạo đơn + thanh toán (sinh hoá đơn). Dùng cho POS bán hàng tại quầy.</summary>
    public async Task<(CheckoutResult? Data, string? Error)> ThanhToanAsync(CheckoutRequest req, int? maNhanVien)
    {
        if (string.IsNullOrWhiteSpace(req.PhuongThuc)) return (null, "Thiếu phương thức thanh toán.");
        var (don, err) = await TaoDonHangAsync(req.MaBan, req.Items, req.GhiChuDonHang, maNhanVien);
        if (err is not null) return (null, err);

        // Áp dụng khuyến mãi (nếu có) — kiểm tra + tính giảm + tăng lượt đã dùng
        decimal tienGiam = 0;
        if (req.MaKhuyenMai is { } kmId)
        {
            var (km, giam, kmErr) = await _promo.ApDungChoDonAsync(kmId, don!.TongTienHang);
            if (kmErr != null) return (null, kmErr);
            tienGiam = giam;
            don.MaKhuyenMai = km!.MaKhuyenMai;
            don.TienGiamGia = giam;
            don.ThanhTien = don.TongTienHang - giam;
        }

        var khachTra = req.SoTienKhachTra ?? don!.ThanhTien;
        if (khachTra < don!.ThanhTien && req.PhuongThuc == "TienMat")
        return (null, "Tiền khách đưa chưa đủ.");
        var thoiLai = Math.Max(0, khachTra - don.ThanhTien);

        var hd = new HoaDon
        {
            MaDonHang = don.MaDonHang,
            MaNhanVienThuNgan = maNhanVien,
            TongThanhTien = don.ThanhTien,
            SoTienKhachTra = khachTra,
            TienThoiLai = thoiLai,
            TrangThai = "DaThanhToan",
            ThoiGianThanhToan = DateTime.UtcNow,
        };
        hd.ChiTietThanhToans.Add(new ThanhToanChiTiet
        {
            PhuongThuc = req.PhuongThuc,
            SoTien = don.ThanhTien,
            ThoiGianThanhToan = DateTime.UtcNow,
        });
        _db.HoaDons.Add(hd);
        await _db.SaveChangesAsync();

        return (new CheckoutResult(don.MaDonHang, hd.MaHoaDon, tienGiam, don.ThanhTien, thoiLai, req.PhuongThuc), null);
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

    /// <summary>Đóng bàn (khách rời + đã dọn): hoàn tất mọi đơn đang hoạt động của bàn, đặt bàn Trống.</summary>
    public async Task<(bool Ok, string? Error)> DongBanAsync(int maBan)
    {
        var ban = await _db.Bans.FindAsync(maBan);
        if (ban is null) return (false, "Bàn không tồn tại.");
        var now = DateTime.UtcNow;
        var active = await _db.DonHangs
            .Where(d => d.MaBan == maBan && TrangThaiActive.Contains(d.TrangThaiDon)).ToListAsync();
        foreach (var d in active) { d.TrangThaiDon = "HoanThanh"; d.ThoiGianCapNhat = now; }
        ban.TrangThai = "Trong";
        await _db.SaveChangesAsync();
        return (true, null);
    }

    /// <summary>Hoàn tác đóng bàn: khôi phục lô đơn vừa hoàn tất gần nhất của bàn, đặt lại Có khách.</summary>
    public async Task<(bool Ok, string? Error)> MoLaiBanAsync(int maBan)
    {
        var done = await _db.DonHangs
            .Where(d => d.MaBan == maBan && d.TrangThaiDon == "HoanThanh").ToListAsync();
        if (done.Count == 0) return (false, "Không có đơn nào để khôi phục.");
        var lastTime = done.Max(d => d.ThoiGianCapNhat);
        var batch = done.Where(d => d.ThoiGianCapNhat == lastTime).ToList();
        foreach (var d in batch) { d.TrangThaiDon = "ChoXacNhan"; d.ThoiGianCapNhat = DateTime.UtcNow; }
        var ban = await _db.Bans.FindAsync(maBan);
        if (ban is not null) ban.TrangThai = "CoKhach";
        await _db.SaveChangesAsync();
        return (true, null);
    }

    /// <summary>Khôi phục 1 đơn cụ thể (đã hoàn tất/huỷ) về hoạt động, đặt lại bàn Có khách.</summary>
    public async Task<(bool Ok, string? Error)> KhoiPhucDonAsync(int maDon)
    {
        var don = await _db.DonHangs.FindAsync(maDon);
        if (don is null) return (false, "Đơn không tồn tại.");
        if (TrangThaiActive.Contains(don.TrangThaiDon)) return (false, "Đơn đang hoạt động.");
        don.TrangThaiDon = "ChoXacNhan";
        don.LyDoHuy = null;
        don.ThoiGianCapNhat = DateTime.UtcNow;
        if (don.MaBan is { } mb)
        {
            var ban = await _db.Bans.FindAsync(mb);
            if (ban is not null) ban.TrangThai = "CoKhach";
       }
        await _db.SaveChangesAsync();
        return (true, null);
    }

    /// <summary>Lịch sử tất cả đơn của 1 bàn (mới nhất trước).</summary>
    public async Task<List<OrderDto>> LichSuBanAsync(int maBan)
    {
        var dons = await _db.DonHangs.Where(d => d.MaBan == maBan)
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Include(d => d.ChiTiets).ThenInclude(c => c.KichCo)
            .OrderByDescending(d => d.ThoiGianTao)
            .ToListAsync();
        return dons.Select(Map).ToList();
    }

    private static OrderDto Map(DonHang d) => new(
        d.MaDonHang, d.MaBan, d.Ban?.TenBan, d.LoaiDonHang, d.TrangThaiDon, d.ThanhTien,
        d.ChiTiets.Sum(c => c.SoLuong), d.ThoiGianTao,
        d.ChiTiets.Select(c => new OrderItemDto(
            c.MaChiTiet, c.MaSanPham, c.SanPham?.TenSanPham ?? "(món)", c.KichCo?.TenKichCo,
            c.SoLuong, c.DonGia, c.ThanhTien, c.GhiChuMon)).ToList());
}     