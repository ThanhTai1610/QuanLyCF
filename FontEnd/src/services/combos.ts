import { api } from './api'

export interface ComboLineDto {
  maSanPham: number
  soLuong: number
  tenSanPham?: string
}

export interface ComboListItem {
  maCombo: number
  tenCombo: string
  giaCombo: number
  hinhAnh: string | null
  trangThaiHoatDong: boolean
  soMon: number
}

export interface ComboDetail {
  maCombo: number
  tenCombo: string
  giaCombo: number
  hinhAnh: string | null
  moTa: string | null
  trangThaiHoatDong: boolean
  chiTiets: ComboLineDto[]
}

export interface SaveComboRequest {
  tenCombo: string
  giaCombo: number
  hinhAnh?: string | null
  moTa?: string | null
  trangThaiHoatDong: boolean
  chiTiets: ComboLineDto[]
}

export const combosApi = {
  list: () => api.get<ComboListItem[]>('/api/combos'),
  get: (id: number) => api.get<ComboDetail>(`/api/combos/${id}`),
  create: (req: SaveComboRequest) => api.post<{ maCombo: number }>('/api/combos', req),
  update: (id: number, req: SaveComboRequest) => api.put(`/api/combos/${id}`, req),
  delete: (id: number) => api.del(`/api/combos/${id}`)
}
