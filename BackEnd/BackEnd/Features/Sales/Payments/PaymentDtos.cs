namespace BackEnd.Features.Sales.Payments;

public record CashPaymentRequest(
    int MaDonHang,
    decimal? SoTienKhachTra,
    int? MaKhuyenMai
);

public record MomoPaymentRequest(
    int MaDonHang,
    int? MaKhuyenMai
);

public record MomoQueryRequest(
    string? OrderId,
    string? RequestId
);

public record PaymentResultDto(
    bool Success,
    string Message,
    int MaDonHang,
    int? MaHoaDon,
    decimal TongThanhTien,
    decimal TienGiam,
    decimal SoTienPhaiThanhToan,
    decimal TienKhachTra,
    decimal TienThoiLai,
    string? PayUrl,              // Link thanh toán MoMo (deep link mở app)
    string? QrCodeUrl,           // URL ảnh QR (cho VietQR img.vietqr.io)
    string? QrRawString          // Chuỗi EMVCo raw QR (cho MoMo Sandbox để render bằng thư viện client)
);

public record PaymentStatusDto(
    int MaDonHang,
    int? MaHoaDon,
    bool DaThanhToan,
    string TrangThaiHoaDon,
    decimal TongThanhTien,
    string? PhuongThuc,
    DateTime? ThoiGianThanhToan
);

public record MomoIpnRequest(
    string partnerCode,
    string orderId,
    string requestId,
    long amount,
    string orderInfo,
    string orderType,
    long transId,
    int resultCode,
    string message,
    string payType,
    long responseTime,
    string extraData,
    string signature
);

public record ConfirmTransferRequest(
    int MaDonHang,
    decimal? SoTienThucNhan
);

public record CassoTransactionDto(
    long id,
    string tid,
    string description,
    decimal amount,
    DateTime when,
    string bank_sub_acc_id
);

public record CassoWebhookRequest(
    int error,
    List<CassoTransactionDto> data
);
