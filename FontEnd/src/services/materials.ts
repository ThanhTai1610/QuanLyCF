import { api } from './api'

export interface MaterialItem {
  maNguyenLieu: number
  tenNguyenLieu: string
  maVach_SKU: string | null
  donViTinh: string
  soLuongTon: number
  mucTonToiThieu: number | null
  giaVonTrungBinh: number | null
  trangThaiTon: string
}

export const materialsApi = {
  list: (q?: string, trangThai?: string) => api.get<MaterialItem[]>('/api/materials', { params: { q, trangThai } })
}
