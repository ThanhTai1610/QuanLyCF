// Goi API module Don hang (Sales/Orders). Token JWT da duoc api.ts tu gan.
import { api } from './api'

export interface MenuSize {
  maKichCo: number
  tenKichCo: string
  giaCongThem: number
}
export interface MenuItem {
  maSanPham: number
  tenSanPham: string
  tenDanhMuc: string | null
  giaBan: number
  hinhAnh: string | null
  kieuMon: string
  moTa: string | null
  laMonNoiBat: boolean
  kichCos: MenuSize[]
  apDungKhungGio?: boolean
  gioBatDau?: string | null
  gioKetThuc?: string | null
}

export interface OrderItem {
  maChiTiet: number
  maSanPham: number | null
  tenMon: string
  tenKichCo: string | null
  soLuong: number
  donGia: number
  thanhTien: number
  ghiChuMon: string | null
  trangThaiBep?: string | null
}
export interface OrderDto {
  maDonHang: number
  maBan: number | null
  tenBan: string | null
  loaiDonHang: string
  trangThaiDon: string
  thanhTien: number
  soMon: number
  thoiGianTao: string
  items: OrderItem[]
}

export interface OrderLineBody {
  maSanPham: number
  maKichCo: number | null
  soLuong: number
  ghiChuMon: string | null
}
export interface CreateOrderBody {
  maBan: number | null   // null = mang về
  items: OrderLineBody[]
  ghiChuDonHang: string | null
  maKhachHang?: number | null
  tienGiamGia?: number | null
}
export interface MoveOrderResult {
  ketQua: 'moved' | 'merged'
  tenBanCu: string | null
  tenBanMoi: string | null
}

export interface CheckoutBody {
  maBan: number | null
  items: OrderLineBody[]
  ghiChuDonHang: string | null
  phuongThuc: string          // TienMat, ChuyenKhoan, Momo
  soTienKhachTra: number | null
  maKhuyenMai: number | null  // khuyến mãi áp dụng (tuỳ chọn)
  maKhachHang?: number | null
  tienGiamGia?: number | null
}
export interface CheckoutResult {
  maDonHang: number
  maHoaDon: number
  tienGiam: number
  thanhTien: number
  tienThoiLai: number
  phuongThuc: string
  diemTichLuy?: number
  maPinSession?: string | null
}

export const ordersApi = {
  menu: (isPos = false) => api.get<MenuItem[]>(`/api/orders/menu?isPos=${isPos}`),
  getAll: () => api.get<OrderDto[]>('/api/orders'),
  getById: (id: number) => api.get<OrderDto>(`/api/orders/${id}`),
  checkout: (body: CheckoutBody) => api.post<CheckoutResult>('/api/orders/checkout', body),
  active: () => api.get<OrderDto[]>('/api/orders/active'),
  kitchenActive: () => api.get<OrderDto[]>('/api/orders/kitchen-active'),
  create: (body: CreateOrderBody) => api.post<OrderDto>('/api/orders', body),
  move: (maDon: number, maBanMoi: number) =>
    api.put<MoveOrderResult>(`/api/orders/${maDon}/move`, { maBanMoi }),
  cancel: (maDon: number, lyDo?: string) =>
    api.put<void>(`/api/orders/${maDon}/cancel`, { lyDo: lyDo ?? null }),
  updateStatus: (maDon: number, status: string) =>
    api.put<void>(`/api/orders/${maDon}/status`, { status }),
  updateItemKitchenStatus: (maChiTiet: number, trangThaiBep: string) =>
    api.put<{ success: boolean; trangThaiBep: string }>(`/api/orders/items/${maChiTiet}/kitchen-status`, { trangThaiBep }),

  // Đóng bàn / hoàn tác / lịch sử
  closeTable: (maBan: number) => api.post<void>(`/api/orders/close-table/${maBan}`),
  reopenTable: (maBan: number) => api.post<void>(`/api/orders/reopen-table/${maBan}`),
  history: (maBan: number) => api.get<OrderDto[]>(`/api/orders/history/${maBan}`),
  restore: (maDon: number) => api.post<void>(`/api/orders/${maDon}/restore`),

  // Gửi Email Mã PIN Bàn & Hóa Đơn
  sendEmailReceipt: (payload: {
    email: string
    maDonHang?: number
    tenBan?: string
    maPinSession?: string | null
  }) => api.post<{ message: string }>('/api/orders/send-email-receipt', payload),

  // Guest order history and service request calls
  guestHistory: (maBan: number) => api.get<OrderDto[]>(`/api/orders/guest/history/${maBan}`),
  getCustomerHistory: (email: string) => api.get<OrderDto[]>(`/api/orders/customer-history?email=${encodeURIComponent(email)}`),
  createServiceRequest: (body: { maBan: number; loaiYeuCau: string; ghiChu: string | null }) =>
    api.post<any>('/api/service-requests', body),
  getActiveServiceRequests: () => api.get<any[]>('/api/service-requests/active'),
  resolveServiceRequest: (id: string) => api.post<void>(`/api/service-requests/${id}/resolve`),
  getById: (maDon: number) => api.get<OrderDto>(`/api/orders/${maDon}`),
  guestCreate: (body: CreateOrderBody) => api.post<OrderDto>('/api/orders/guest', body),
}
