using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BackEnd.Features.Sales.Invoices;

public class InvoiceService
{
    private readonly QuanLyCFDbContext _db;

    public InvoiceService(QuanLyCFDbContext db)
    {
        _db = db;
    }

    /// <summary>Lấy danh sách hoá đơn phân trang và lọc.</summary>
    public async Task<object> LayDanhSachHoaDonAsync(InvoiceSearchQuery q)
    {
        var query = _db.HoaDons
            .Include(h => h.DonHang).ThenInclude(d => d.Ban)
            .Include(h => h.NhanVienThuNgan)
            .Include(h => h.ChiTietThanhToans)
            .AsNoTracking();

        // ── Lọc theo thời gian ──
        if (q.TuNgay.HasValue)
        {
            query = query.Where(h => h.ThoiGianThanhToan >= q.TuNgay.Value);
        }
        if (q.DenNgay.HasValue)
        {
            query = query.Where(h => h.ThoiGianThanhToan <= q.DenNgay.Value);
        }

        // ── Lọc theo trạng thái hoá đơn ──
        if (!string.IsNullOrEmpty(q.TrangThai))
        {
            query = query.Where(h => h.TrangThai == q.TrangThai);
        }

        // ── Lọc theo mã đơn hàng ──
        if (q.MaDonHang.HasValue)
        {
            query = query.Where(h => h.MaDonHang == q.MaDonHang.Value);
        }

        // ── Lọc theo tên bàn ──
        if (!string.IsNullOrEmpty(q.TenBan))
        {
            query = query.Where(h => h.DonHang.Ban != null && h.DonHang.Ban.TenBan.Contains(q.TenBan));
        }

        // ── Lọc theo phương thức thanh toán ──
        if (!string.IsNullOrEmpty(q.PhuongThuc))
        {
            query = query.Where(h => h.ChiTietThanhToans.Any(c => c.PhuongThuc == q.PhuongThuc));
        }

        var totalCount = await query.CountAsync();

        var rawItems = await query
            .OrderByDescending(h => h.ThoiGianThanhToan)
            .ThenByDescending(h => h.MaHoaDon)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync();

        var items = rawItems.Select(h => {
            var rawPt = h.ChiTietThanhToans.FirstOrDefault()?.PhuongThuc ?? "TienMat";
            var ptDisplay = rawPt switch
            {
                "TienMat" => "Tiền mặt",
                "Momo" => "MoMo",
                "NganHang" => "VietQR",
                "ChuyenKhoan" => "VietQR",
                _ => rawPt
            };
            string? loaiDon = h.DonHang?.LoaiDonHang;
            string? tenBan = h.DonHang?.Ban?.TenBan;
            if (loaiDon == "TakeAway" || string.IsNullOrWhiteSpace(tenBan))
            {
                tenBan = "Mang về";
            }

            return new InvoiceListItemDto(
                MaHoaDon: h.MaHoaDon,
                MaDonHang: h.MaDonHang,
                TenBan: tenBan,
                LoaiDonHang: loaiDon ?? (tenBan == "Mang về" ? "TakeAway" : "DineIn"),
                TongThanhTien: h.TongThanhTien,
                TrangThai: h.TrangThai,
                ThoiGianThanhToan: h.ThoiGianThanhToan,
                TenThuNgan: h.NhanVienThuNgan?.HoTen ?? "Hệ thống",
                PhuongThuc: ptDisplay
            );
        }).ToList();

        return new
        {
            items,
            totalCount,
            page = q.Page,
            pageSize = q.PageSize
        };
    }

    /// <summary>Lấy chi tiết hoá đơn.</summary>
    public async Task<(InvoiceDetailDto? Data, string? Error)> LayChiTietHoaDonAsync(int id)
    {
        var h = await _db.HoaDons
            .Include(h => h.NhanVienThuNgan)
            .Include(h => h.ChiTietThanhToans)
            .Include(h => h.DonHang).ThenInclude(d => d.Ban)
            .Include(h => h.DonHang).ThenInclude(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Include(h => h.DonHang).ThenInclude(d => d.ChiTiets).ThenInclude(c => c.KichCo)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaHoaDon == id);

        if (h is null) return (null, "Hoá đơn không tồn tại.");

        var items = h.DonHang.ChiTiets.Select(c => new InvoiceItemDto(
            TenSanPham: c.SanPham?.TenSanPham ?? "Sản phẩm",
            TenKichCo: c.KichCo?.TenKichCo,
            SoLuong: c.SoLuong,
            DonGia: c.DonGia,
            ThanhTien: c.ThanhTien,
            GhiChuMon: c.GhiChuMon
        )).ToList();

        var payments = h.ChiTietThanhToans.Select(p => new InvoicePaymentDto(
            PhuongThuc: p.PhuongThuc,
            SoTien: p.SoTien,
            MaGiaoDichCong: p.MaGiaoDichCong,
            ThoiGianThanhToan: p.ThoiGianThanhToan
        )).ToList();

        var detail = new InvoiceDetailDto(
            MaHoaDon: h.MaHoaDon,
            MaDonHang: h.MaDonHang,
            TenBan: h.DonHang?.Ban?.TenBan,
            LoaiDonHang: h.DonHang?.LoaiDonHang ?? "DineIn",
            TongTienHang: h.DonHang?.TongTienHang ?? h.TongThanhTien,
            TienGiam: h.DonHang?.TienGiamGia ?? 0,
            ThanhTien: h.TongThanhTien,
            SoTienKhachTra: h.SoTienKhachTra,
            TienThoiLai: h.TienThoiLai,
            TrangThai: h.TrangThai,
            ThoiGianThanhToan: h.ThoiGianThanhToan,
            TenNhanVienThuNgan: h.NhanVienThuNgan?.HoTen,
            MaSoThueXuatHD: h.MaSoThueXuatHD,
            Items: items,
            Payments: payments
        );

        return (detail, null);
    }

    /// <summary>Sinh HTML in hoá đơn K80 nhiệt.</summary>
    public async Task<(string? Html, string? Error)> TaoTemplateInHoaDonAsync(int id)
    {
        var (h, err) = await LayChiTietHoaDonAsync(id);
        if (err is not null) return (null, err);

        // Đọc thông tin cài đặt quán
        var tenQuan = await LayGiaTriCaiDatAsync("TEN_QUAN") ?? "BrewManager Coffee";
        var diaChi = await LayGiaTriCaiDatAsync("DIA_CHI") ?? "123 Nguyễn Huệ, Quận 1, TP.HCM";
        var hotline = await LayGiaTriCaiDatAsync("SO_DIEN_THOAI") ?? "0909 123 456";

        var sb = new StringBuilder();
        sb.Append(@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>In Hoá Đơn</title>
    <style>
        @page { size: 80mm auto; margin: 0; }
        body { 
            font-family: 'Arial', sans-serif; 
            font-size: 12px; 
            line-height: 1.4; 
            width: 76mm; 
            margin: 0 auto; 
            padding: 5mm 0;
            color: #000;
        }
        .text-center { text-align: center; }
        .text-right { text-align: right; }
        .bold { font-weight: bold; }
        .header { margin-bottom: 5mm; }
        .title { font-size: 16px; font-weight: bold; margin: 2mm 0; }
        .info-table { width: 100%; border-collapse: collapse; margin-bottom: 3mm; }
        .info-table td { padding: 1px 0; vertical-align: top; }
        .item-table { width: 100%; border-collapse: collapse; margin-top: 3mm; margin-bottom: 3mm; }
        .item-table th { border-bottom: 1px dashed #000; padding: 3px 0; text-align: left; font-weight: bold; }
        .item-table td { padding: 4px 0; border-bottom: 1px dotted #ccc; }
        .totals { width: 100%; margin-top: 3mm; border-top: 1px dashed #000; padding-top: 2mm; }
        .totals td { padding: 2px 0; }
        .footer { margin-top: 6mm; border-top: 1px dashed #000; padding-top: 4mm; font-size: 11px; }
        .divider { border-top: 1px dashed #000; margin: 3mm 0; }
    </style>
</head>
<body onload='window.print()'>");

        // Header
        sb.AppendFormat("<div class='text-center header'>");
        sb.AppendFormat("   <div class='bold' style='font-size: 15px;'>{0}</div>", tenQuan);
        sb.AppendFormat("   <div>Đ/c: {0}</div>", diaChi);
        sb.AppendFormat("   <div>SĐT: {0}</div>", hotline);
        sb.AppendFormat("   <div class='title'>PHIẾU THANH TOÁN</div>");
        sb.AppendFormat("   <div>Số HĐ: HD{0:D6} | Đơn: #{1}</div>", h!.MaHoaDon, h.MaDonHang);
        sb.AppendFormat("</div>");

        // Thông tin đơn
        sb.AppendFormat("<table class='info-table'>");
        sb.AppendFormat("   <tr><td class='bold'>Bàn:</td><td class='text-right bold'>{0}</td></tr>", h.TenBan ?? "Mang về");
        sb.AppendFormat("   <tr><td>Thời gian:</td><td class='text-right'>{0:dd/MM/yyyy HH:mm:ss}</td></tr>", h.ThoiGianThanhToan.AddHours(7)); // Giả định UTC sang GMT+7
        sb.AppendFormat("   <tr><td>Thu ngân:</td><td class='text-right'>{0}</td></tr>", h.TenNhanVienThuNgan ?? "Hệ thống");
        sb.AppendFormat("   <tr><td>Hình thức:</td><td class='text-right'>{0}</td></tr>", h.LoaiDonHang == "TakeAway" ? "Mang đi" : "Tại bàn");
        sb.AppendFormat("</table>");

        // Bảng món ăn
        sb.AppendFormat("<table class='item-table'>");
        sb.AppendFormat("   <thead>");
        sb.AppendFormat("       <tr>");
        sb.AppendFormat("           <th style='width: 45%;'>Tên món</th>");
        sb.AppendFormat("           <th class='text-center' style='width: 15%;'>SL</th>");
        sb.AppendFormat("           <th class='text-right' style='width: 20%;'>Đ.Giá</th>");
        sb.AppendFormat("           <th class='text-right' style='width: 20%;'>T.Tiền</th>");
        sb.AppendFormat("       </tr>");
        sb.AppendFormat("   </thead>");
        sb.AppendFormat("   <tbody>");
        
        foreach (var item in h.Items)
        {
            var tenMon = item.TenSanPham;
            if (!string.IsNullOrEmpty(item.TenKichCo))
            {
                tenMon += $" ({item.TenKichCo})";
            }
            sb.AppendFormat("       <tr>");
            sb.AppendFormat("           <td>{0}</td>", tenMon);
            sb.AppendFormat("           <td class='text-center'>{0}</td>", item.SoLuong);
            sb.AppendFormat("           <td class='text-right'>{0:N0}</td>", item.DonGia);
            sb.AppendFormat("           <td class='text-right'>{0:N0}</td>", item.ThanhTien);
            sb.AppendFormat("       </tr>");
            
            if (!string.IsNullOrEmpty(item.GhiChuMon))
            {
                sb.AppendFormat("       <tr>");
                sb.AppendFormat("           <td colspan='4' style='font-size: 10px; color: #555; padding-left: 2mm; font-style: italic;'>* Ghi chú: {0}</td>", item.GhiChuMon);
                sb.AppendFormat("       </tr>");
            }
        }
        
        sb.AppendFormat("   </tbody>");
        sb.AppendFormat("</table>");

        // Tổng tiền
        sb.AppendFormat("<table class='totals'>");
        sb.AppendFormat("   <tr><td>Tổng cộng:</td><td class='text-right'>{0:N0}đ</td></tr>", h.TongTienHang);
        if (h.TienGiam > 0)
        {
            sb.AppendFormat("   <tr><td>Giảm giá:</td><td class='text-right'>-{0:N0}đ</td></tr>", h.TienGiam);
        }
        sb.AppendFormat("   <tr class='bold' style='font-size: 13px;'><td>THÀNH TIỀN:</td><td class='text-right'>{0:N0}đ</td></tr>", h.ThanhTien);
        
        // Phương thức thanh toán chi tiết
        foreach (var p in h.Payments)
        {
            var pName = p.PhuongThuc switch
            {
                "TienMat" => "Tiền mặt",
                "Momo" => "Ví MoMo",
                "ChuyenKhoan" => "Chuyển khoản",
                _ => p.PhuongThuc
            };
            sb.AppendFormat("   <tr style='font-size: 11px; color: #333;'><td>- {0}:</td><td class='text-right'>{1:N0}đ</td></tr>", pName, p.SoTien);
        }

        if (h.SoTienKhachTra > h.ThanhTien)
        {
            sb.AppendFormat("   <tr><td>Khách đưa:</td><td class='text-right'>{0:N0}đ</td></tr>", h.SoTienKhachTra);
            sb.AppendFormat("   <tr class='bold'><td>Tiền thối lại:</td><td class='text-right'>{0:N0}đ</td></tr>", h.TienThoiLai);
        }
        sb.AppendFormat("</table>");

        // Footer
        sb.AppendFormat("<div class='text-center footer'>");
        sb.AppendFormat("   <div class='bold'>CẢM ƠN QUÝ KHÁCH & HẸN GẶP LẠI</div>");
        sb.AppendFormat("   <div style='margin-top: 1mm;'>Powered by BrewManager</div>");
        sb.AppendFormat("</div>");

        sb.Append(@"</body>
</html>");

        return (sb.ToString(), null);
    }

    private async Task<string?> LayGiaTriCaiDatAsync(string khoa)
    {
        var item = await _db.CaiDatHeThongs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.KhoaCaiDat == khoa);
        return item?.GiaTriCaiDat;
    }

    /// <summary>Xoá tất cả hoá đơn mẫu và đơn hàng khỏi hệ thống.</summary>
    public async Task<int> XoaTatCaHoaDonAsync()
    {
        _db.ThanhToanChiTiets.RemoveRange(_db.ThanhToanChiTiets);

        var diems = await _db.LichSuDiems.Where(x => x.MaDonHang != null).ToListAsync();
        foreach (var d in diems) d.MaDonHang = null;

        _db.HoaDons.RemoveRange(_db.HoaDons);
        _db.ChiTietDonHangs.RemoveRange(_db.ChiTietDonHangs);
        _db.DonHangs.RemoveRange(_db.DonHangs);

        int count = await _db.SaveChangesAsync();

        _db.NhatKyHeThongs.Add(new NhatKyHeThong
        {
            HanhDong = "XOÁ TẤT CẢ HÓA ĐƠN",
            Module = "HÓA ĐƠN",
            DuLieuMoi = "Quản trị viên đã xóa sạch toàn bộ danh sách hóa đơn và đơn hàng mẫu khỏi hệ thống.",
            ThietBi = "Màn hình Quản lý Hóa đơn",
            ThoiGianTao = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return count;
    }
}
