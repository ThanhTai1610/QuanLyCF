import { api } from './api'

export interface CustomerLoginBody {
  soDienThoai: string
  hoTen: string
}

export interface Customer {
  maKhachHang: number
  hoTen: string | null
  soDienThoai: string | null
  hangThanhVien: string
  diemTichLuy: number
  tongTienDaTieu: number
  lanGheThamCuoi: string | null
  thoiGianTao: string
  isNew: boolean
}

export const customerApi = {
  /** Đăng nhập / đăng ký khách hàng bằng SĐT + tên */
  login: (body: CustomerLoginBody) =>
    api.post<Customer>('/api/storefront/customer/login', body),

  /** Lấy danh sách tất cả khách hàng (admin), có thể tìm kiếm */
  getAll: (search?: string) => {
    const qs = search ? `?search=${encodeURIComponent(search)}` : ''
    return api.get<Customer[]>(`/api/storefront/customer${qs}`)
  },

  /** Tìm khách hàng theo số điện thoại */
  getByPhone: (phone: string) =>
    api.get<Customer>(`/api/storefront/customer/phone/${encodeURIComponent(phone)}`),
}
