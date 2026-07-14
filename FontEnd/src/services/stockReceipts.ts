import { api } from './api'

export interface ReceiptLineRequest {
  maNguyenLieu: number
  soLuong: number
  donGia: number
}

export interface CreateReceiptRequest {
  maNhaCungCap: number | null
  tienDaThanhToan: number
  phuongThucThanhToan: string
  ghiChu: string | null
  chiTiets: ReceiptLineRequest[]
}

export interface ReceiptListItem {
  maPhieu: number
  thoiGianTao: string
  tenNhaCungCap: string | null
  tongTienHang: number
  tienDaThanhToan: number
  trangThaiThanhToan: string
}

export const stockReceiptsApi = {
  list: (trangThaiThanhToan?: string) => api.get<ReceiptListItem[]>('/api/stock-receipts', { params: { trangThaiThanhToan } }),
  get: (id: number) => api.get<any>(`/api/stock-receipts/${id}`),
  create: (req: CreateReceiptRequest) => api.post<{ maPhieu: number }>('/api/stock-receipts', req)
}
