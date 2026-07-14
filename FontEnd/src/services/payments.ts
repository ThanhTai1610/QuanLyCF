import { api } from './api'

export interface CashPaymentBody {
  maDonHang: number
  soTienKhachTra: number | null
  maKhuyenMai: number | null
}

export interface MomoPaymentBody {
  maDonHang: number
  maKhuyenMai: number | null
}

export interface PaymentResultDto {
  success: boolean
  message: string
  maDonHang: number
  maHoaDon: number | null
  tongThanhTien: number
  tienGiam: number
  soTienPhaiThanhToan: number
  tienKhachTra: number
  tienThoiLai: number
  payUrl: string | null
  qrCodeUrl: string | null      // URL ảnh QR (VietQR image URL)
  qrRawString: string | null    // Chuỗi EMVCo raw (MoMo Sandbox) để render bằng qrcode.vue
}

export interface PaymentStatusDto {
  maDonHang: number
  maHoaDon: number | null
  daThanhToan: boolean
  trangThaiHoaDon: string
  tongThanhTien: number
  phuongThuc: string | null
  thoiGianThanhToan: string | null
}

export const paymentsApi = {
  payCash: (body: CashPaymentBody) => api.post<PaymentResultDto>('/api/payments/cash', body),
  payMomo: (body: MomoPaymentBody) => api.post<PaymentResultDto>('/api/payments/momo', body),
  payVietQr: (body: MomoPaymentBody) => api.post<PaymentResultDto>('/api/payments/vietqr', body),
  getStatus: (maDonHang: number) => api.get<PaymentStatusDto>(`/api/payments/status/${maDonHang}`),
  confirmTransfer: (maDonHang: number, soTienThucNhan: number) =>
    api.post<PaymentResultDto>(`/api/payments/confirm-transfer/${maDonHang}`, { maDonHang, soTienThucNhan }),
  queryMomo: (maDonHang: number, body?: { orderId?: string; requestId?: string }) =>
    api.post<PaymentStatusDto>(`/api/payments/momo-query/${maDonHang}`, body ?? {}),
}
