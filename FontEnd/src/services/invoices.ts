import { api } from './api'

export interface InvoiceItemDto {
  tenSanPham?: string
  tenMon?: string
  tenKichCo?: string | null
  soLuong: number
  donGia: number
  thanhTien: number
  ghiChuMon?: string | null
}

export interface InvoiceListItemDto {
  maHoaDon: number
  maDonHang: number
  tenBan: string | null
  loaiDonHang?: string | null
  thoiGianThanhToan: string
  tongThanhTien: number
  thanhTien?: number
  phuongThuc: string | null
  tenThuNgan: string | null
  tenNhanVienThuNgan?: string | null
  trangThai: string
}

export interface InvoiceDetailDto extends InvoiceListItemDto {
  loaiDonHang?: string
  tongTienHang: number
  tienGiam: number
  tienGiamGia?: number
  thanhTien: number
  phiDichVu?: number
  thueVAT?: number
  soTienKhachTra: number
  tienThoiLai: number
  maSoThueXuatHD: string | null
  items: InvoiceItemDto[]
  payments?: any[]
}

export interface InvoiceQueryParams {
  search?: string
  trangThai?: string
  tuNgay?: string
  denNgay?: string
}

export interface InvoiceListResponse {
  items: InvoiceListItemDto[]
  totalCount: number
  page: number
  pageSize: number
}

export const invoicesApi = {
  list: (params?: InvoiceQueryParams) => api.get<InvoiceListResponse>('/api/invoices', { params }),
  get: (id: number) => api.get<InvoiceDetailDto>(`/api/invoices/${id}`),
  clearAll: () => api.del<{ message: string; deletedCount: number }>('/api/invoices/clear-all')
}