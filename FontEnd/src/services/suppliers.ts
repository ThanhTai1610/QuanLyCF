import { api } from './api'

export interface SupplierItem {
  maNhaCungCap: number
  tenNhaCungCap: string
  nguoiLienHe: string | null
  soDienThoai: string | null
  email: string | null
  congNoHienTai: number
}

export interface SaveSupplierRequest {
  tenNhaCungCap: string
  maSoThue?: string | null
  nguoiLienHe?: string | null
  soDienThoai?: string | null
  email?: string | null
  diaChi?: string | null
  soTaiKhoan?: string | null
  tenNganHang?: string | null
}

export const suppliersApi = {
  list: (q?: string) => api.get<SupplierItem[]>('/api/suppliers', { params: { q } }),
  create: (req: SaveSupplierRequest) => api.post<{ maNhaCungCap: number }>('/api/suppliers', req),
  update: (id: number, req: SaveSupplierRequest) => api.put(`/api/suppliers/${id}`, req),
  pay: (id: number, soTien: number, phuongThucThanhToan: string = 'Tiền mặt') => api.post(`/api/suppliers/${id}/pay`, { soTien, phuongThucThanhToan }),
  delete: (id: number) => api.delete(`/api/suppliers/${id}`)
}
