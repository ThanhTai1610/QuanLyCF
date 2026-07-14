using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using BackEnd.Domain.Entities;
using BackEnd.Features.Sales.Promotions;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BackEnd.Features.Sales.Payments;

public class PaymentService
{
    private readonly QuanLyCFDbContext _db;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;
    private readonly PromotionService _promo;
    private readonly IMemoryCache _cache;

    public PaymentService(
        QuanLyCFDbContext db,
        IConfiguration config,
        HttpClient http,
        PromotionService promo,
        IMemoryCache cache)
    {
        _db = db;
        _config = config;
        _http = http;
        _promo = promo;
        _cache = cache;
    }

    /// <summary>Thanh toán bằng tiền mặt.</summary>
    public async Task<(PaymentResultDto? Data, string? Error)> ThanhToanTienMatAsync(
        int maDonHang, decimal? soTienKhachTra, int? maNhanVien, int? maKhuyenMai)
    {
        var don = await _db.DonHangs
            .Include(d => d.ChiTiets)
            .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang);

        if (don is null) return (null, "Đơn hàng không tồn tại.");
        if (don.TrangThaiDon == "Huy") return (null, "Đơn hàng đã bị huỷ, không thể thanh toán.");

        // Áp dụng khuyến mãi nếu truyền lên và đơn chưa có khuyến mãi
        decimal tienGiam = don.TienGiamGia;
        if (maKhuyenMai is { } kmId && don.MaKhuyenMai is null)
        {
            var (km, giam, kmErr) = await _promo.ApDungChoDonAsync(kmId, don.TongTienHang);
            if (kmErr != null) return (null, kmErr);
            tienGiam = giam;
            don.MaKhuyenMai = km!.MaKhuyenMai;
            don.TienGiamGia = giam;
            don.ThanhTien = don.TongTienHang - giam;
        }

        var phaiThanhToan = don.ThanhTien;
        var khachTra = soTienKhachTra ?? phaiThanhToan;

        if (khachTra < phaiThanhToan)
            return (null, $"Số tiền khách trả ({khachTra:N0}đ) chưa đủ. Cần thanh toán: {phaiThanhToan:N0}đ.");

        var thoiLai = khachTra - phaiThanhToan;

        // Tìm hoặc tạo hoá đơn
        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
        if (hd is null)
        {
            hd = new HoaDon
            {
                MaDonHang = don.MaDonHang,
                MaNhanVienThuNgan = maNhanVien,
                TongThanhTien = phaiThanhToan,
                SoTienKhachTra = khachTra,
                TienThoiLai = thoiLai,
                TrangThai = "DaThanhToan",
                ThoiGianThanhToan = DateTime.UtcNow
            };
            _db.HoaDons.Add(hd);
        }
        else
        {
            if (hd.TrangThai == "DaThanhToan")
                return (null, "Đơn hàng này đã được thanh toán trước đó.");

            hd.MaNhanVienThuNgan = maNhanVien;
            hd.TongThanhTien = phaiThanhToan;
            hd.SoTienKhachTra = khachTra;
            hd.TienThoiLai = thoiLai;
            hd.TrangThai = "DaThanhToan";
            hd.ThoiGianThanhToan = DateTime.UtcNow;
        }

        // Tạo chi tiết thanh toán
        var ct = new ThanhToanChiTiet
        {
            HoaDon = hd,
            PhuongThuc = "TienMat",
            SoTien = phaiThanhToan,
            ThoiGianThanhToan = DateTime.UtcNow
        };
        _db.ThanhToanChiTiets.Add(ct);

        // Cập nhật trạng thái đơn hàng (Xác nhận đơn và chuyển sang chế biến nếu đang chờ)
        if (don.TrangThaiDon == "ChoXacNhan")
        {
            don.TrangThaiDon = "DangPha";
        }
        don.ThoiGianCapNhat = DateTime.UtcNow;
        await TichDiemChoKhachHangAsync(don);

        await _db.SaveChangesAsync();

        return (new PaymentResultDto(
            Success: true,
            Message: "Thanh toán bằng tiền mặt thành công.",
            MaDonHang: don.MaDonHang,
            MaHoaDon: hd.MaHoaDon,
            TongThanhTien: don.TongTienHang,
            TienGiam: tienGiam,
            SoTienPhaiThanhToan: phaiThanhToan,
            TienKhachTra: khachTra,
            TienThoiLai: thoiLai,
            PayUrl: null,
            QrCodeUrl: null,
            QrRawString: null
        ), null);
    }

    /// <summary>Tạo link thanh toán MoMo.</summary>
    public async Task<(PaymentResultDto? Data, string? Error)> TaoThanhToanMomoAsync(
        int maDonHang, int? maNhanVien, int? maKhuyenMai, string requestHost)
    {
        var don = await _db.DonHangs
            .Include(d => d.ChiTiets)
            .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang);

        if (don is null) return (null, "Đơn hàng không tồn tại.");
        if (don.TrangThaiDon == "Huy") return (null, "Đơn hàng đã bị huỷ, không thể thanh toán.");

        // Áp dụng khuyến mãi nếu truyền lên và đơn chưa có
        decimal tienGiam = don.TienGiamGia;
        if (maKhuyenMai is { } kmId && don.MaKhuyenMai is null)
        {
            var (km, giam, kmErr) = await _promo.ApDungChoDonAsync(kmId, don.TongTienHang);
            if (kmErr != null) return (null, kmErr);
            tienGiam = giam;
            don.MaKhuyenMai = km!.MaKhuyenMai;
            don.TienGiamGia = giam;
            don.ThanhTien = don.TongTienHang - giam;
            await _db.SaveChangesAsync();
        }

        var phaiThanhToan = don.ThanhTien;

        // Đọc cấu hình MoMo
        var partnerCode = _config["Momo:PartnerCode"] ?? "";
        var accessKey = _config["Momo:AccessKey"] ?? "";
        var secretKey = _config["Momo:SecretKey"] ?? "";
        var endpoint = _config["Momo:Endpoint"] ?? "";
        var redirectUrl = _config["Momo:RedirectUrl"] ?? "";
        var ipnUrl = _config["Momo:IpnUrl"] ?? "";

        // Nếu IPN Url đang để tương đối hoặc local, ta có thể linh hoạt thay thế bằng host của request (nếu cần)
        if (ipnUrl.StartsWith("/"))
        {
            ipnUrl = $"{requestHost}{ipnUrl}";
        }

        // Tạo HoaDon tạm tính ở trạng thái ChuaTT nếu chưa có
        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
        if (hd is null)
        {
            hd = new HoaDon
            {
                MaDonHang = don.MaDonHang,
                MaNhanVienThuNgan = maNhanVien,
                TongThanhTien = phaiThanhToan,
                SoTienKhachTra = 0,
                TienThoiLai = 0,
                TrangThai = "ChuaTT",
                ThoiGianThanhToan = DateTime.UtcNow
            };
            _db.HoaDons.Add(hd);
            await _db.SaveChangesAsync();
        }
        else if (hd.TrangThai == "DaThanhToan")
        {
            return (null, "Đơn hàng này đã được thanh toán trước đó.");
        }

        // Sinh mã MoMo OrderId và RequestId độc nhất
        var momoOrderId = $"DH_{maDonHang}_{DateTime.UtcNow.Ticks}";
        var requestId = Guid.NewGuid().ToString();

        // Lưu thông tin giao dịch MoMo vào cache để đối soát chủ động sau này (hạn 30 phút)
        var cacheKey = $"momo_txn_{maDonHang}";
        _cache.Set(cacheKey, new MomoTxnCache(momoOrderId, requestId), TimeSpan.FromMinutes(30));

        // Chuẩn bị dữ liệu gửi MoMo
        var orderInfo = $"Thanh toan don hang #{maDonHang} tai BrewManager";
        var amountStr = ((long)phaiThanhToan).ToString();
        var extraData = maDonHang.ToString();
        var requestType = "captureWallet";

        // Tạo signature
        // accessKey=$accessKey&amount=$amount&extraData=$extraData&ipnUrl=$ipnUrl&orderId=$orderId&orderInfo=$orderInfo&partnerCode=$partnerCode&redirectUrl=$redirectUrl&requestId=$requestId&requestType=$requestType
        var rawSignature = $"accessKey={accessKey}&amount={amountStr}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={momoOrderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
        var signature = ComputeHmacSha256(rawSignature, secretKey);

        var requestBody = new
        {
            partnerCode,
            partnerName = "BrewManager Coffee",
            storeId = "QuanLyCF_Store",
            requestId,
            amount = (long)phaiThanhToan,
            orderId = momoOrderId,
            orderInfo,
            redirectUrl,
            ipnUrl,
            extraData,
            requestType,
            signature,
            lang = "vi"
        };

        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (null, $"Lỗi gọi API MoMo: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<MomoResponse>();
            if (result is null || result.resultCode != 0)
            {
                return (null, $"MoMo từ chối yêu cầu: {result?.message ?? "Không có phản hồi"} (Code: {result?.resultCode})");
            }

            // MoMo Sandbox trả về chuỗi EMVCo raw QR (bắt đầu bằng 000201...)
            // Client sẽ dùng thư viện qrcode.vue để render. ᨰng thời sị sinh URL ảnh dự phòng.
            string? qrRawString = null;
            string? qrImageUrl = null;
            if (!string.IsNullOrEmpty(result.qrCodeUrl))
            {
                if (result.qrCodeUrl.StartsWith("000201"))
                {
                    // EMVCo raw string → render bằng client
                    qrRawString = result.qrCodeUrl;
                    // Sinh ảnh QR từ API public (dự phòng)
                    qrImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(result.qrCodeUrl)}";
                }
                else
                {
                    qrImageUrl = result.qrCodeUrl;
                }
            }
            else if (!string.IsNullOrEmpty(result.payUrl))
            {
                qrImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(result.payUrl)}";
            }

            return (new PaymentResultDto(
                Success: true,
                Message: "Tạo liên kết thanh toán MoMo thành công.",
                MaDonHang: don.MaDonHang,
                MaHoaDon: hd.MaHoaDon,
                TongThanhTien: don.TongTienHang,
                TienGiam: tienGiam,
                SoTienPhaiThanhToan: phaiThanhToan,
                TienKhachTra: 0,
                TienThoiLai: 0,
                PayUrl: result.payUrl,
                QrCodeUrl: qrImageUrl,
                QrRawString: qrRawString
            ), null);
        }
        catch (Exception ex)
        {
            return (null, $"Có lỗi xảy ra khi kết nối tới MoMo: {ex.Message}");
        }
    }

    /// <summary>Xác nhận thanh toán từ Webhook IPN của MoMo.</summary>
    public async Task<(bool Success, string Message)> ProcessMomoIpnAsync(MomoIpnRequest req)
    {
        var secretKey = _config["Momo:SecretKey"] ?? "";
        var accessKey = _config["Momo:AccessKey"] ?? "";

        // Kiểm tra chữ ký đối chiếu
        // accessKey=$accessKey&amount=$amount&extraData=$extraData&message=$message&orderId=$orderId&orderInfo=$orderInfo&partnerCode=$partnerCode&requestId=$requestId&responseTime=$responseTime&resultCode=$resultCode&transId=$transId
        var rawSignature = $"accessKey={accessKey}&amount={req.amount}&extraData={req.extraData}&message={req.message}&orderId={req.orderId}&orderInfo={req.orderInfo}&partnerCode={req.partnerCode}&requestId={req.requestId}&responseTime={req.responseTime}&resultCode={req.resultCode}&transId={req.transId}";
        var computedSignature = ComputeHmacSha256(rawSignature, secretKey);

        if (!computedSignature.Equals(req.signature, StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Chữ ký MoMo IPN không hợp lệ.");
        }

        // Parse mã đơn hàng từ extraData (hoặc parse từ orderId)
        if (!int.TryParse(req.extraData, out var maDonHang))
        {
            // Dự phòng: parse từ orderId dạng DH_maDonHang_ticks
            var parts = req.orderId.Split('_');
            if (parts.Length < 2 || !int.TryParse(parts[1], out maDonHang))
            {
                return (false, "Không thể xác định mã đơn hàng từ dữ liệu MoMo.");
            }
        }

        if (req.resultCode != 0)
        {
            return (false, $"Giao dịch MoMo thất bại với mã lỗi: {req.resultCode} ({req.message}).");
        }

        return await CompleteMomoPaymentAsync(maDonHang, req.amount, req.transId.ToString());
    }

    /// <summary>Chủ động đối soát trạng thái giao dịch MoMo từ API của MoMo.</summary>
    public async Task<(PaymentStatusDto? Data, string? Error)> DoiSoatMomoTransactionAsync(int maDonHang, string? clientOrderId, string? clientRequestId)
    {
        var don = await _db.DonHangs.FindAsync(maDonHang);
        if (don is null) return (null, "Đơn hàng không tồn tại.");

        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
        if (hd?.TrangThai == "DaThanhToan")
        {
            var ct = await _db.ThanhToanChiTiets
                .Where(c => c.MaHoaDon == hd.MaHoaDon)
                .OrderByDescending(c => c.ThoiGianThanhToan)
                .FirstOrDefaultAsync();

            return (new PaymentStatusDto(
                MaDonHang: don.MaDonHang,
                MaHoaDon: hd.MaHoaDon,
                DaThanhToan: true,
                TrangThaiHoaDon: hd.TrangThai,
                TongThanhTien: hd.TongThanhTien,
                PhuongThuc: ct?.PhuongThuc ?? "Momo",
                ThoiGianThanhToan: hd.ThoiGianThanhToan
            ), null);
        }

        // Lấy thông tin orderId, requestId từ cache hoặc client truyền lên
        string? orderId = clientOrderId;
        string? requestId = clientRequestId;

        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(requestId))
        {
            var cacheKey = $"momo_txn_{maDonHang}";
            if (_cache.TryGetValue<MomoTxnCache>(cacheKey, out var cachedTxn) && cachedTxn != null)
            {
                orderId = cachedTxn.OrderId;
                requestId = cachedTxn.RequestId;
            }
        }

        if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(requestId))
        {
            return (null, "Không tìm thấy thông tin phiên giao dịch MoMo để đối soát. Vui lòng truyền orderId và requestId từ Client.");
        }

        var partnerCode = _config["Momo:PartnerCode"] ?? "";
        var accessKey = _config["Momo:AccessKey"] ?? "";
        var secretKey = _config["Momo:SecretKey"] ?? "";
        var queryEndpoint = _config["Momo:QueryEndpoint"] ?? "https://test-payment.momo.vn/v2/gateway/api/query";

        // Tạo signature cho query API
        // accessKey=$accessKey&orderId=$orderId&partnerCode=$partnerCode&requestId=$requestId
        var rawSignature = $"accessKey={accessKey}&orderId={orderId}&partnerCode={partnerCode}&requestId={requestId}";
        var signature = ComputeHmacSha256(rawSignature, secretKey);

        var requestBody = new { partnerCode, requestId, orderId, signature };

        try
        {
            var response = await _http.PostAsJsonAsync(queryEndpoint, requestBody);
            if (!response.IsSuccessStatusCode)
            {
                return (null, $"Lỗi gọi API đối soát MoMo: {response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<MomoQueryResponse>();
            if (result is null)
            {
                return (null, "Không nhận được phản hồi hợp lệ từ cổng MoMo.");
            }

            if (result.resultCode == 0) // Thanh toán thành công trên MoMo
            {
                var (ok, err) = await CompleteMomoPaymentAsync(maDonHang, result.amount, result.transId.ToString());
                if (!ok) return (null, err);

                var dbHd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
                return (new PaymentStatusDto(
                    MaDonHang: don.MaDonHang,
                    MaHoaDon: dbHd?.MaHoaDon,
                    DaThanhToan: true,
                    TrangThaiHoaDon: "DaThanhToan",
                    TongThanhTien: don.ThanhTien,
                    PhuongThuc: "Momo",
                    ThoiGianThanhToan: dbHd?.ThoiGianThanhToan
                ), null);
            }
            else
            {
                return (new PaymentStatusDto(
                    MaDonHang: don.MaDonHang,
                    MaHoaDon: hd?.MaHoaDon,
                    DaThanhToan: false,
                    TrangThaiHoaDon: hd?.TrangThai ?? "ChuaTT",
                    TongThanhTien: don.ThanhTien,
                    PhuongThuc: "Momo",
                    ThoiGianThanhToan: null
                ), $"Giao dịch MoMo chưa thành công: {result.message} (Code: {result.resultCode})");
            }
        }
        catch (Exception ex)
        {
            return (null, $"Lỗi xảy ra khi đối soát giao dịch MoMo: {ex.Message}");
        }
    }

    /// <summary>Lấy trạng thái thanh toán của đơn hàng trong Database.</summary>
    public async Task<PaymentStatusDto> LayTrangThaiThanhToanAsync(int maDonHang)
    {
        var don = await _db.DonHangs.FindAsync(maDonHang);
        if (don is null)
        {
            return new PaymentStatusDto(maDonHang, null, false, "KhongTonTai", 0, null, null);
        }

        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
        if (hd is null)
        {
            return new PaymentStatusDto(don.MaDonHang, null, false, "ChuaTT", don.ThanhTien, null, null);
        }

        var ct = await _db.ThanhToanChiTiets
            .Where(c => c.MaHoaDon == hd.MaHoaDon)
            .OrderByDescending(c => c.ThoiGianThanhToan)
            .FirstOrDefaultAsync();

        return new PaymentStatusDto(
            MaDonHang: don.MaDonHang,
            MaHoaDon: hd.MaHoaDon,
            DaThanhToan: hd.TrangThai == "DaThanhToan",
            TrangThaiHoaDon: hd.TrangThai,
            TongThanhTien: hd.TongThanhTien,
            PhuongThuc: ct?.PhuongThuc,
            ThoiGianThanhToan: hd.ThoiGianThanhToan
        );
    }

    /// <summary>Tạo link thanh toán VietQR động.</summary>
    public async Task<(PaymentResultDto? Data, string? Error)> TaoThanhToanVietQrAsync(
        int maDonHang, int? maNhanVien, int? maKhuyenMai)
    {
        var don = await _db.DonHangs
            .Include(d => d.ChiTiets)
            .FirstOrDefaultAsync(d => d.MaDonHang == maDonHang);

        if (don is null) return (null, "Đơn hàng không tồn tại.");
        if (don.TrangThaiDon == "Huy") return (null, "Đơn hàng đã bị huỷ, không thể thanh toán.");

        // Áp dụng khuyến mãi nếu truyền lên và đơn chưa có
        decimal tienGiam = don.TienGiamGia;
        if (maKhuyenMai is { } kmId && don.MaKhuyenMai is null)
        {
            var (km, giam, kmErr) = await _promo.ApDungChoDonAsync(kmId, don.TongTienHang);
            if (kmErr != null) return (null, kmErr);
            tienGiam = giam;
            don.MaKhuyenMai = km!.MaKhuyenMai;
            don.TienGiamGia = giam;
            don.ThanhTien = don.TongTienHang - giam;
            await _db.SaveChangesAsync();
        }

        var phaiThanhToan = don.ThanhTien;

        // Đọc cấu hình ngân hàng từ DB cài đặt hệ thống
        var bankId = await LayGiaTriCaiDatAsync("NGAN_HANG_ID") ?? "MB";
        var accountNo = await LayGiaTriCaiDatAsync("NGAN_HANG_STK") ?? "19035282928014";
        var accountName = await LayGiaTriCaiDatAsync("NGAN_HANG_TEN") ?? "CONG TY BREWMANAGER";
        
        // Nội dung chuyển khoản định danh đơn hàng
        var addInfo = $"BrewManager DH{maDonHang}";

        // Sinh link QR VietQR động (sử dụng template compact)
        var vietQrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-compact.png?amount={((long)phaiThanhToan)}&addInfo={Uri.EscapeDataString(addInfo)}&accountName={Uri.EscapeDataString(accountName)}";

        // Tạo HoaDon tạm tính ở trạng thái ChuaTT nếu chưa có
        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
        if (hd is null)
        {
            hd = new HoaDon
            {
                MaDonHang = don.MaDonHang,
                MaNhanVienThuNgan = maNhanVien,
                TongThanhTien = phaiThanhToan,
                SoTienKhachTra = 0,
                TienThoiLai = 0,
                TrangThai = "ChuaTT",
                ThoiGianThanhToan = DateTime.UtcNow
            };
            _db.HoaDons.Add(hd);
            await _db.SaveChangesAsync();
        }
        else if (hd.TrangThai == "DaThanhToan")
        {
            return (null, "Đơn hàng này đã được thanh toán trước đó.");
        }

        return (new PaymentResultDto(
            Success: true,
            Message: "Tạo mã VietQR chuyển khoản thành công.",
            MaDonHang: don.MaDonHang,
            MaHoaDon: hd.MaHoaDon,
            TongThanhTien: don.TongTienHang,
            TienGiam: tienGiam,
            SoTienPhaiThanhToan: phaiThanhToan,
            TienKhachTra: 0,
            TienThoiLai: 0,
            PayUrl: vietQrUrl,
            QrCodeUrl: vietQrUrl,
            QrRawString: null
        ), null);
    }

    /// <summary>Xác nhận chuyển khoản ngân hàng thủ công (do Thu ngân duyệt).</summary>
    public async Task<(PaymentResultDto? Data, string? Error)> ConfirmChuyenKhoanThuCongAsync(
        int maDonHang, decimal? soTienThucNhan, int? maNhanVien)
    {
        var don = await _db.DonHangs.FindAsync(maDonHang);
        if (don is null) return (null, "Đơn hàng không tồn tại.");
        if (don.TrangThaiDon == "Huy") return (null, "Đơn hàng đã bị huỷ, không thể thanh toán.");

        var phaiThanhToan = don.ThanhTien;
        var thucNhan = soTienThucNhan ?? phaiThanhToan;

        var (ok, err) = await CompleteChuyenKhoanPaymentAsync(maDonHang, thucNhan, "MANUAL_" + DateTime.UtcNow.Ticks, maNhanVien);
        if (!ok) return (null, err);

        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);

        return (new PaymentResultDto(
            Success: true,
            Message: "Thu ngân xác nhận chuyển khoản thành công.",
            MaDonHang: don.MaDonHang,
            MaHoaDon: hd?.MaHoaDon,
            TongThanhTien: don.TongTienHang,
            TienGiam: don.TienGiamGia,
            SoTienPhaiThanhToan: phaiThanhToan,
            TienKhachTra: thucNhan,
            TienThoiLai: 0,
            PayUrl: null,
            QrCodeUrl: null,
            QrRawString: null
        ), null);
    }

    /// <summary>Xử lý webhook tự động nhận tiền từ Casso.</summary>
    public async Task<(bool Success, string Message)> ProcessCassoWebhookAsync(CassoWebhookRequest req)
    {
        if (req.error != 0 || req.data is null)
        {
            return (false, "Dữ liệu webhook Casso bị lỗi hoặc không có giao dịch.");
        }

        int countSuccess = 0;
        foreach (var txn in req.data)
        {
            // Phân tích description để tìm mã đơn hàng
            // Định dạng: "BrewManager DH123" hoặc tương tự
            var desc = txn.description ?? "";
            var match = global::System.Text.RegularExpressions.Regex.Match(desc, @"DH(\d+)", global::System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (!match.Success) continue;

            if (int.TryParse(match.Groups[1].Value, out var maDonHang))
            {
                var result = await CompleteChuyenKhoanPaymentAsync(maDonHang, txn.amount, txn.tid, null);
                if (result.Success) countSuccess++;
            }
        }

        return (true, $"Xử lý thành công {countSuccess} giao dịch từ Casso.");
    }

    /// <summary>Đọc cấu hình hệ thống.</summary>
    public async Task<string?> LayGiaTriCaiDatAsync(string khoa)
    {
        var item = await _db.CaiDatHeThongs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.KhoaCaiDat == khoa);
        return item?.GiaTriCaiDat;
    }

    private async Task<(bool Success, string? Error)> CompleteChuyenKhoanPaymentAsync(
        int maDonHang, decimal soTien, string transId, int? maNhanVienThuNgan)
    {
        var don = await _db.DonHangs.FindAsync(maDonHang);
        if (don is null) return (false, "Đơn hàng không tồn tại.");

        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
        if (hd is null)
        {
            hd = new HoaDon
            {
                MaDonHang = don.MaDonHang,
                MaNhanVienThuNgan = maNhanVienThuNgan,
                TongThanhTien = don.ThanhTien,
                SoTienKhachTra = soTien,
                TienThoiLai = 0,
                TrangThai = "DaThanhToan",
                ThoiGianThanhToan = DateTime.UtcNow
            };
            _db.HoaDons.Add(hd);
        }
        else
        {
            if (hd.TrangThai == "DaThanhToan")
            {
                return (true, null); // Đã thanh toán xong từ trước
            }

            hd.MaNhanVienThuNgan = maNhanVienThuNgan ?? hd.MaNhanVienThuNgan;
            hd.TongThanhTien = don.ThanhTien;
            hd.SoTienKhachTra = soTien;
            hd.TienThoiLai = 0;
            hd.TrangThai = "DaThanhToan";
            hd.ThoiGianThanhToan = DateTime.UtcNow;
        }

        // Tạo chi tiết thanh toán chuyển khoản
        var existsCt = await _db.ThanhToanChiTiets.AnyAsync(c => c.MaGiaoDichCong == transId);
        if (!existsCt)
        {
            var ct = new ThanhToanChiTiet
            {
                HoaDon = hd,
                PhuongThuc = "ChuyenKhoan",
                SoTien = soTien,
                MaGiaoDichCong = transId,
                ThoiGianThanhToan = DateTime.UtcNow
            };
            _db.ThanhToanChiTiets.Add(ct);
        }

        // Cập nhật trạng thái đơn hàng (xác nhận đơn hàng)
        if (don.TrangThaiDon == "ChoXacNhan")
        {
            don.TrangThaiDon = "DangPha";
        }
        don.ThoiGianCapNhat = DateTime.UtcNow;
        await TichDiemChoKhachHangAsync(don);

        await _db.SaveChangesAsync();
        return (true, null);
    }

    #region Helper Methods

    private async Task<(bool Success, string Message)> CompleteMomoPaymentAsync(int maDonHang, decimal soTien, string transId)
    {
        var don = await _db.DonHangs.FindAsync(maDonHang);
        if (don is null) return (false, "Đơn hàng liên kết MoMo không tồn tại.");

        var hd = await _db.HoaDons.FirstOrDefaultAsync(h => h.MaDonHang == maDonHang);
        if (hd is null)
        {
            hd = new HoaDon
            {
                MaDonHang = don.MaDonHang,
                TongThanhTien = don.ThanhTien,
                SoTienKhachTra = soTien,
                TienThoiLai = 0,
                TrangThai = "DaThanhToan",
                ThoiGianThanhToan = DateTime.UtcNow
            };
            _db.HoaDons.Add(hd);
        }
        else
        {
            if (hd.TrangThai == "DaThanhToan")
            {
                return (true, "Đơn hàng đã được ghi nhận thanh toán trước đó.");
            }

            hd.TongThanhTien = don.ThanhTien;
            hd.SoTienKhachTra = soTien;
            hd.TienThoiLai = 0;
            hd.TrangThai = "DaThanhToan";
            hd.ThoiGianThanhToan = DateTime.UtcNow;
        }

        // Lưu thông tin chi tiết giao dịch MoMo
        var existsCt = await _db.ThanhToanChiTiets.AnyAsync(c => c.MaGiaoDichCong == transId);
        if (!existsCt)
        {
            var ct = new ThanhToanChiTiet
            {
                HoaDon = hd,
                PhuongThuc = "Momo",
                SoTien = soTien,
                MaGiaoDichCong = transId,
                ThoiGianThanhToan = DateTime.UtcNow
            };
            _db.ThanhToanChiTiets.Add(ct);
        }

        // Cập nhật trạng thái đơn hàng (xác nhận đơn hàng)
        if (don.TrangThaiDon == "ChoXacNhan")
        {
            don.TrangThaiDon = "DangPha";
        }
        don.ThoiGianCapNhat = DateTime.UtcNow;
        await TichDiemChoKhachHangAsync(don);

        await _db.SaveChangesAsync();
        return (true, "Ghi nhận thanh toán MoMo thành công.");
    }

    private string ComputeHmacSha256(string message, string secretKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(messageBytes);
        return Convert.ToHexString(hashBytes).ToLower();
    }

    private async Task TichDiemChoKhachHangAsync(DonHang don)
    {
        if (don.MaKhachHang == null) return;
        
        var kh = await _db.KhachHangs.FindAsync(don.MaKhachHang.Value);
        if (kh == null) return;

        // Tích luỹ 1 điểm cho mỗi 10.000đ giá trị thanh toán của đơn
        int diemCong = (int)(don.ThanhTien / 10000);
        if (diemCong <= 0) return;

        kh.DiemTichLuy += diemCong;
        kh.HangThanhVien = GetTierByPoints(kh.DiemTichLuy);

        var ls = new LichSuDiem
        {
            MaKhachHang = kh.MaKhachHang,
            LoaiBienDong = "Cong",
            SoDiem = diemCong,
            GhiChu = $"Tích điểm từ đơn hàng #{don.MaDonHang} (Thanh toán: {don.ThanhTien:N0}đ)",
            MaDonHang = don.MaDonHang,
            ThoiGianTao = DateTime.UtcNow
        };
        _db.Set<LichSuDiem>().Add(ls);
    }

    private static string GetTierByPoints(int points)
    {
        if (points >= 3000) return "Diamond";
        if (points >= 1500) return "Gold";
        if (points >= 500) return "Silver";
        return "Bronze";
    }

    #endregion
}

// ── Lớp phụ hỗ trợ mapping và cache ────────────────────────────

public record MomoTxnCache(string OrderId, string RequestId);

public class MomoResponse
{
    public string partnerCode { get; set; } = null!;
    public string orderId { get; set; } = null!;
    public string requestId { get; set; } = null!;
    public long amount { get; set; }
    public long responseTime { get; set; }
    public string message { get; set; } = null!;
    public int resultCode { get; set; }
    public string payUrl { get; set; } = null!;
    public string deeplink { get; set; } = null!;
    public string qrCodeUrl { get; set; } = null!;
}

public class MomoQueryResponse
{
    public string partnerCode { get; set; } = null!;
    public string orderId { get; set; } = null!;
    public string requestId { get; set; } = null!;
    public decimal amount { get; set; }
    public string message { get; set; } = null!;
    public int resultCode { get; set; }
    public long transId { get; set; }
    public string payType { get; set; } = null!;
    public long responseTime { get; set; }
    public string extraData { get; set; } = null!;
    public string signature { get; set; } = null!;
}
