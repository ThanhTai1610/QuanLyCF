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
  list: (q?: string, trangThai?: string) => {
    const params = new URLSearchParams()
    if (q) params.append('q', q)
    if (trangThai) params.append('trangThai', trangThai)
    const qs = params.toString()
    return api.get<MaterialItem[]>(`/api/materials${qs ? '?' + qs : ''}`)
  }
}
