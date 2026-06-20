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
  kichCos: MenuSize[]
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
}
export interface CheckoutResult {
  maDonHang: number
  maHoaDon: number
  tienGiam: number
  thanhTien: number
  tienThoiLai: number
  phuongThuc: string
}

export const ordersApi = {
  menu: () => api.get<MenuItem[]>('/api/orders/menu'),
  checkout: (body: CheckoutBody) => api.post<CheckoutResult>('/api/orders/checkout', body),
  active: () => api.get<OrderDto[]>('/api/orders/active'),
  create: (body: CreateOrderBody) => api.post<OrderDto>('/api/orders', body),
  move: (maDon: number, maBanMoi: number) =>
    api.put<MoveOrderResult>(`/api/orders/${maDon}/move`, { maBanMoi }),
  cancel: (maDon: number, lyDo?: string) =>
    api.put<void>(`/api/orders/${maDon}/cancel`, { lyDo: lyDo ?? null }),

  // Đóng bàn / hoàn tác / lịch sử
  closeTable: (maBan: number) => api.post<void>(`/api/orders/close-table/${maBan}`),
  reopenTable: (maBan: number) => api.post<void>(`/api/orders/reopen-table/${maBan}`),
  history: (maBan: number) => api.get<OrderDto[]>(`/api/orders/history/${maBan}`),
  restore: (maDon: number) => api.post<void>(`/api/orders/${maDon}/restore`),
}
