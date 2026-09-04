using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Orders;

public class OrderService
{
    // Đơn còn hoạt động (chưa đóng bàn/huỷ)
    private static readonly string[] TrangThaiActive = { "ChoThanhToan", "ChoXacNhan", "DangPha", "DaPhaXong", "HoanThanh" };

    private readonly QuanLyCFDbContext _db;
    private readonly Promotions.PromotionService _promo;
    public OrderService(QuanLyCFDbContext db, Promotions.PromotionService promo)
    {
        _db = db; _promo = promo;
    }

    public async Task<List<MenuItemDto>> LayMenuAsync(bool isPos = false) 
    {
        var rawMenu = await _db.SanPhams.Where(x => x.TrangThaiBan)
            .Include(x => x.DanhMuc).Include(x => x.KichCos)
            .OrderBy(x => x.TenSanPham)
            .ToListAsync();

        // Lấy giờ hiện tại (múi giờ VN UTC+7)
        var currentTime = DateTime.UtcNow.AddHours(7).TimeOfDay;

        var filtered = rawMenu.Where(x => 
        {
            if (x.DanhMuc == null) return true;
            if (!x.DanhMuc.TrangThaiHoatDong) return false;
            return true;
        });

        var menuItems = filtered.Select(x => new MenuItemDto(
                x.MaSanPham, x.TenSanPham, x.DanhMuc?.TenDanhMuc,
                x.GiaBan, x.HinhAnh, x.KieuMon, x.MoTa, x.LaMonNoiBat,
                x.KichCos.Where(s => s.TrangThaiHoatDong)
                    .Select(s => new MenuSizeDto(s.MaKichCo, s.TenKichCo, s.GiaCongThem)).ToList(),
                x.DanhMuc?.ApDungKhungGio ?? false,
                x.DanhMuc?.GioBatDau?.ToString(@"hh\:mm"),
                x.DanhMuc?.GioKetThuc?.ToString(@"hh\:mm"),
                x.DiemTichLuy ?? 0))
            .ToList();

        // ── Thêm combo đang hoạt động vào menu (kieuMon = "Combo", maSanPham = -maCombo) ──
        var combos = await _db.Combos.Where(c => c.TrangThaiHoatDong)
            .Include(c => c.ChiTiets).ThenInclude(ct => ct.SanPham)
            .OrderBy(c => c.TenCombo)
            .ToListAsync();

        foreach (var cb in combos)
        {
            if (cb.ApDungKhungGio && cb.GioBatDau.HasValue && cb.GioKetThuc.HasValue)
            {
                var start = cb.GioBatDau.Value;
                var end = cb.GioKetThuc.Value;
                if (start <= end)
                {
                    if (currentTime < start || currentTime > end) continue; // Tự động ẩn ngoài khung giờ
                }
                else
                {
                    if (currentTime < start && currentTime > end) continue; // Khung giờ qua đêm
                }
            }

            var moTaItems = string.Join(", ", cb.ChiTiets.Select(ct =>
                ct.SoLuong > 1 ? $"{ct.SanPham.TenSanPham} x{ct.SoLuong}" : ct.SanPham.TenSanPham));
            menuItems.Add(new MenuItemDto(
                -cb.MaCombo,          // ID âm để frontend phân biệt combo
                cb.TenCombo,
                "Combo",              // Danh mục ảo
                cb.GiaCombo,
                cb.HinhAnh,
                "Combo",              // kieuMon đặc biệt
                cb.MoTa ?? moTaItems, // Mô tả gồm danh sách món
                false,
                new List<MenuSizeDto>(),
                cb.ApDungKhungGio,
                cb.GioBatDau?.ToString(@"hh\:mm"),
                cb.GioKetThuc?.ToString(@"hh\:mm")));
        }

        return menuItems;
    }

    /// <summary>Tất cả đơn đang phục vụ của lượt khách hiện tại trên bàn (chưa bị đóng bàn DaDongBan).</summary>
    public async Task<List<OrderDto>> LayDonActiveAsync()
    {
        var dons = await _db.DonHangs
            .Where(d => d.Ban != null && d.Ban.TrangThai == "CoKhach" && d.TrangThaiDon != "Huy" && d.TrangThaiDon != "DaDongBan" &&
                (
                    d.TrangThaiDon == "ChoThanhToan" || 
                    d.TrangThaiDon == "ChoXacNhan" || 
                    d.TrangThaiDon == "DangPha" || 
                    d.TrangThaiDon == "DaPhaXong" ||
                    d.TrangThaiDon == "HoanThanh"
                ))
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Include(d => d.ChiTiets).ThenInclude(c => c.KichCo)
            .OrderBy(d => d.ThoiGianTao)
            .ToListAsync();
        return dons.Select(Map).ToList();
    }

    /// <summary>Đơn hàng chờ pha chế cho màn hình Bếp KDS (LOẠI BỎ tuyệt đối đơn ChoThanhToan).</summary>
    public async Task<List<OrderDto>> LayDonBepActiveAsync()
    {
        var kitchenStatuses = new[] { "ChoXacNhan", "DangPha", "DaPhaXong" };
        var dons = await _db.DonHangs
            .Where(d => kitchenStatuses.Contains(d.TrangThaiDon))
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Include(d => d.ChiTiets).ThenInclude(c => c.KichCo)
            .OrderBy(d => d.ThoiGianTao)
            .ToListAsync();
        return dons.Select(Map).ToList();
    }

    /// <summary>Lấy tất cả đơn hàng cho màn hình Quản lý đơn hàng (POS / Admin).</summary>
    public async Task<List<OrderDto>> LayTatCaDonHangAsync()
    {
        var dons = await _db.DonHangs
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Include(d => d.ChiTiets).ThenInclude(c => c.KichCo)
            .OrderByDescending(d => d.ThoiGianTao)
            .Take(100)
            .ToListAsync();
        return dons.Select(Map).ToList();
    }

    /// <summary>Tạo + lưu đơn hàng (entity). Dùng chung cho tạo đơn và thanh toán.</summary>
    private async Task<(DonHang? Don, string? Error)> TaoDonHangAsync(
        int? maBan, List<OrderLineRequest>? items, string? ghiChu, int? maNhanVien, int? maKhachHang = null)
    {
        if (items is null || items.Count == 0) return (null, "Đơn phải có ít nhất 1 món.");
        Ban? ban = null;
        if (maBan is { } mb)
        {
            ban = await _db.Bans.FindAsync(mb);
            if (ban is null) return (null, "Bàn không tồn tại.");
        }

        // Tách combo (MaSanPham < 0) và sản phẩm thường (MaSanPham > 0)
        var comboLines = items.Where(i => i.MaSanPham < 0).ToList();
        var normalLines = items.Where(i => i.MaSanPham > 0).ToList();

        var spIds = normalLines.Select(i => i.MaSanPham).Distinct().ToList();
        var spMap = await _db.SanPhams.Include(s => s.KichCos)
            .Where(s => spIds.Contains(s.MaSanPham))
            .ToDictionaryAsync(s => s.MaSanPham);

        var isGuestQR = (maNhanVien == null);
        var don = new DonHang
        {
            MaBan = maBan,
            MaNhanVien = maNhanVien,
            MaKhachHang = maKhachHang,
            LoaiDonHang = maBan is null ? "TakeAway" : "DineIn",
            TrangThaiDon = isGuestQR ? "ChoThanhToan" : "ChoXacNhan",
            GhiChuDonHang = ghiChu,
            ThoiGianTao = DateTime.UtcNow,
            ThoiGianCapNhat = DateTime.UtcNow,
        };

        decimal tong = 0;

        // ── Xử lý sản phẩm thường ──
        foreach (var it in normalLines)
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
                TrangThaiBep = isGuestQR ? "ChoThanhToan" : "ChoLam",
            });
        }

        // ── Xử lý combo (MaSanPham âm = -MaCombo) ──
        foreach (var cLine in comboLines)
        {
            if (cLine.SoLuong <= 0) continue;
            var maCombo = -cLine.MaSanPham; // chuyển lại ID dương
            var combo = await _db.Combos
                .Include(c => c.ChiTiets).ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(c => c.MaCombo == maCombo && c.TrangThaiHoatDong);
            if (combo is null) return (null, $"Combo #{maCombo} không tồn tại hoặc đã ngừng kinh doanh.");

            // Thêm từng món trong combo vào đơn, giá = giaCombo chia theo tỉ lệ
            var tongGiaGoc = combo.ChiTiets.Sum(ct => ct.SanPham.GiaBan * ct.SoLuong);
            foreach (var ct in combo.ChiTiets)
            {
                // Chia đơn giá theo tỉ lệ giá gốc → giá combo (tránh mất tiền do làm tròn)
                var donGia = tongGiaGoc > 0
                    ? Math.Round(ct.SanPham.GiaBan * combo.GiaCombo / tongGiaGoc, 0)
                    : 0;
                var soLuong = ct.SoLuong * cLine.SoLuong;
                var thanhTien = donGia * soLuong;
                tong += thanhTien;
                don.ChiTiets.Add(new ChiTietDonHang
                {
                    MaSanPham = ct.MaSanPham,
                    MaKichCo = null,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    ThanhTien = thanhTien,
                    GhiChuMon = $"[Combo] {combo.TenCombo}",
                    TrangThaiBep = isGuestQR ? "ChoThanhToan" : "ChoLam",
                });
            }
        }
        if (don.ChiTiets.Count == 0) return (null, "Đơn phải có ít nhất 1 món.");

        don.TongTienHang = tong;
        don.ThanhTien = tong;
        _db.DonHangs.Add(don);
        if (ban is not null)
        {
            ban.TrangThai = "CoKhach";
            if (string.IsNullOrEmpty(ban.MaPinSession))
            {
                ban.MaPinSession = Random.Shared.Next(1000, 9999).ToString();
            }
            ban.ThoiGianKhoaHetHan = DateTime.UtcNow.AddHours(2);

            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                var match = global::System.Text.RegularExpressions.Regex.Match(ghiChu, @"0\d{8,10}");
                if (match.Success) ban.SoDienThoaiDatBan = match.Value;
            }
            if (maKhachHang.HasValue && string.IsNullOrEmpty(ban.SoDienThoaiDatBan))
            {
                var kh = await _db.KhachHangs.FindAsync(maKhachHang.Value);
                if (kh != null && !string.IsNullOrEmpty(kh.SoDienThoai)) ban.SoDienThoaiDatBan = kh.SoDienThoai;
            }
        }
        await _db.SaveChangesAsync();
        return (don, null);
    }

    public async Task<(OrderDto? Data, string? Error)> TaoDonAsync(CreateOrderRequest req, int? maNhanVien)
    {
        var (don, err) = await TaoDonHangAsync(req.MaBan, req.Items, req.GhiChuDonHang, maNhanVien, req.MaKhachHang);
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
        var (don, err) = await TaoDonHangAsync(req.MaBan, req.Items, req.GhiChuDonHang, maNhanVien, req.MaKhachHang);
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

        // ── Tích điểm cho Khách hàng (Tích theo thiết lập của món hoặc 10.000đ = 1 điểm) ──
        int diemTich = 0;
        if (don.MaKhachHang is { } khId && khId > 0)
        {
            var kh = await _db.KhachHangs.FindAsync(khId);
            bool daTichDiem = await _db.Set<LichSuDiem>()
                .AnyAsync(ls => ls.MaDonHang == don.MaDonHang && (ls.LoaiBienDong == "Cong" || ls.LoaiBienDong == "Tich"));
            if (kh != null && !daTichDiem)
            {
                foreach (var ct in don.ChiTiets)
                {
                    if (ct.MaSanPham.HasValue && ct.MaSanPham.Value > 0)
                    {
                        var sp = await _db.SanPhams.FindAsync(ct.MaSanPham.Value);
                        if (sp != null && sp.DiemTichLuy.HasValue && sp.DiemTichLuy.Value > 0)
                        {
                            diemTich += sp.DiemTichLuy.Value * ct.SoLuong;
                        }
                        else
                        {
                            diemTich += (int)(ct.ThanhTien / 10000m);
                        }
                    }
                }
                if (diemTich <= 0) diemTich = (int)(don.ThanhTien / 10000m);

                kh.DiemTichLuy += diemTich;
                kh.TongDiemTichLuy += diemTich;
                kh.TongTienDaTieu += don.ThanhTien;
                kh.LanGheThamCuoi = DateTime.UtcNow;

                int maxPts = Math.Max(kh.TongDiemTichLuy, kh.DiemTichLuy);
                if (maxPts >= 3000) kh.HangThanhVien = "Diamond";
                else if (maxPts >= 1500) kh.HangThanhVien = "Gold";
                else if (maxPts >= 500) kh.HangThanhVien = "Silver";

                if (diemTich > 0)
                {
                    _db.Set<LichSuDiem>().Add(new LichSuDiem
                    {
                        MaKhachHang = khId,
                        LoaiBienDong = "Tich",
                        SoDiem = diemTich,
                        GhiChu = $"Tích điểm thanh toán đơn hàng #{don.MaDonHang}",
                        ThoiGianTao = DateTime.UtcNow
                    });
                }
            }
        }

        await _db.SaveChangesAsync();

        string? pinSession = don.Ban?.MaPinSession;
        return (new CheckoutResult(don.MaDonHang, hd.MaHoaDon, tienGiam, don.ThanhTien, thoiLai, req.PhuongThuc, diemTich, pinSession), null);
    }

    /// <summary>Đổi bàn: chuyển đơn sang bàn mới, bàn mới trở thành Có khách, bàn cũ được giải phóng nếu hết đơn.</summary>
    public async Task<(MoveOrderResult? Data, string? Error)> DoiBanAsync(int maDon, int maBanMoi)
    {
        var don = await _db.DonHangs.Include(d => d.Ban).FirstOrDefaultAsync(d => d.MaDonHang == maDon);
        if (don is null) return (null, "Đơn không tồn tại.");
        if (don.TrangThaiDon == "Huy" || don.TrangThaiDon == "DaDongBan") return (null, "Đơn không còn hoạt động.");
        if (don.MaBan == maBanMoi) return (null, "Đơn đã ở bàn này.");

        var banCu = don.Ban;
        var banMoi = await _db.Bans.FindAsync(maBanMoi);
        if (banMoi is null) return (null, "Bàn mới không tồn tại.");

        var now = DateTime.UtcNow;

        // Nếu bàn mới đang Trống => Dọn dẹp toàn bộ đơn lịch sử cũ của bàn mới để nhận đơn chuyển sang
        if (banMoi.TrangThai == "Trong")
        {
            var oldUnclosed = await _db.DonHangs
                .Where(d => d.MaBan == maBanMoi && d.TrangThaiDon != "Huy" && d.TrangThaiDon != "DaDongBan")
                .ToListAsync();
            foreach (var o in oldUnclosed)
            {
                o.TrangThaiDon = "DaDongBan";
                o.ThoiGianCapNhat = now;
            }
        }

        // Tách bất kỳ liên kết ghép bàn cũ (nếu có)
        banMoi.MaBanChinh = null;
        if (banCu is not null) banCu.MaBanChinh = null;

        // Chuyển đơn sang bàn mới
        don.MaBan = maBanMoi;
        don.ThoiGianCapNhat = now;
        banMoi.TrangThai = "CoKhach";

        if (banCu is not null)
        {
            if (!string.IsNullOrEmpty(banCu.MaPinSession) && string.IsNullOrEmpty(banMoi.MaPinSession))
            {
                banMoi.MaPinSession = banCu.MaPinSession;
                banMoi.ThoiGianKhoaHetHan = banCu.ThoiGianKhoaHetHan;
            }

            // Kiểm tra xem bàn cũ còn đơn nào khác đang hoạt động không
            var conDon = await _db.DonHangs.AnyAsync(d =>
                d.MaBan == banCu.MaBan && d.MaDonHang != maDon && d.TrangThaiDon != "Huy" && d.TrangThaiDon != "DaDongBan");

            if (!conDon)
            {
                banCu.TrangThai = "Trong";
                banCu.MaPinSession = null;
                banCu.ThoiGianKhoaHetHan = null;
                banCu.SoDienThoaiDatBan = null;
            }
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

    /// <summary>Cập nhật trạng thái đơn hàng (ví dụ: DangPha, HoanThanh).</summary>
    public async Task<(bool Ok, string? Error)> CapNhatTrangThaiAsync(int maDon, string trangThai)
    {
        var don = await _db.DonHangs.FindAsync(maDon);
        if (don is null) return (false, "Đơn không tồn tại.");

        don.TrangThaiDon = trangThai;
        don.ThoiGianCapNhat = DateTime.UtcNow;

        if (trangThai == "DaPhaXong" || trangThai == "HoanThanh")
        {
            var chiTiets = await _db.ChiTietDonHangs.Where(c => c.MaDonHang == maDon).ToListAsync();
            foreach (var ct in chiTiets)
            {
                ct.TrangThaiBep = "HoanThanh";
                ct.ThoiGianLamXong = DateTime.UtcNow;
            }
        }

        if (trangThai == "Huy")
        {
            if (don.MaBan is { } mb)
            {
                var conDon = await _db.DonHangs.AnyAsync(d =>
                    d.MaBan == mb && d.MaDonHang != maDon && TrangThaiActive.Contains(d.TrangThaiDon));
                if (!conDon)
                {
                    var ban = await _db.Bans.FindAsync(mb);
                    if (ban is not null) ban.TrangThai = "Trong";
                }
            }
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
            .Where(d => d.MaBan == maBan && d.TrangThaiDon != "Huy" && d.TrangThaiDon != "DaDongBan").ToListAsync();
        foreach (var d in active) { d.TrangThaiDon = "DaDongBan"; d.ThoiGianCapNhat = now; }
        ban.TrangThai = "Trong";
        ban.MaPinSession = null;
        ban.ThoiGianKhoaHetHan = null;
        ban.SoDienThoaiDatBan = null;
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

    /// <summary>Lấy thông tin chi tiết 1 đơn hàng theo ID.</summary>
    public async Task<OrderDto?> LayDonTheoIdAsync(int maDon)
    {
        var don = await _db.DonHangs
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Include(d => d.ChiTiets).ThenInclude(c => c.KichCo)
            .FirstOrDefaultAsync(d => d.MaDonHang == maDon);
        return don != null ? Map(don) : null;
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
        d.MaDonHang, d.MaBan, d.LoaiDonHang == "TakeAway" ? $"Mang về - #{d.MaDonHang:D3}" : d.Ban?.TenBan, d.LoaiDonHang, d.TrangThaiDon, d.ThanhTien,
        d.ChiTiets.Sum(c => c.SoLuong), d.ThoiGianTao,
        d.ChiTiets.Select(c => new OrderItemDto(
            c.MaChiTiet, c.MaSanPham, c.SanPham?.TenSanPham ?? "(món)", c.KichCo?.TenKichCo,
            c.SoLuong, c.DonGia, c.ThanhTien, c.GhiChuMon, c.TrangThaiBep)).ToList());
}     