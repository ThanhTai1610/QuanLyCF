import { api } from './api'

export interface InvoiceItemDto {
  tenMon: string
  soLuong: number
  donGia: number
  thanhTien: number
}

export interface InvoiceListItemDto {
  maHoaDon: number
  maDonHang: number
  tenBan: string | null
  thoiGianThanhToan: string
  tongThanhTien: number
  phuongThuc: string | null
  tenThuNgan: string | null
  trangThai: string
}

export interface InvoiceDetailDto extends InvoiceListItemDto {
  tongTienHang: number
  tienGiamGia: number
  phiDichVu: number
  thueVAT: number
  soTienKhachTra: number
  tienThoiLai: number
  maSoThueXuatHD: string | null
  items: InvoiceItemDto[]
}

export interface InvoiceQueryParams {
  search?: string
  trangThai?: string
  tuNgay?: string
  denNgay?: string
}

export const invoicesApi = {
  list: (params?: InvoiceQueryParams) => api.get<InvoiceListItemDto[]>('/api/invoices', { params }),
  get: (id: number) => api.get<InvoiceDetailDto>(`/api/invoices/${id}`)
}