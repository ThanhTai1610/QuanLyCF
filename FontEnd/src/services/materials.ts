import { api } from './api'

export interface MaterialItem {
  maNguyenLieu: number
  tenNguyenLieu: string
  maVach_SKU: string | null
  phanLoai: string
  donViTinh: string
  soLuongTon: number
  mucTonToiThieu: number | null
  giaVonTrungBinh: number | null
  trangThaiTon: string
  ngayHetHan: string | null
}

export interface SaveMaterialRequest {
  tenNguyenLieu: string
  maVach_SKU?: string | null
  phanLoai: string
  donViTinh: string
  mucTonToiThieu?: number | null
  mucTonToiDa?: number | null
  hanSuDungNgay?: number | null
  hinhAnh?: string | null
  ngayHetHan?: string | null
}

export const materialsApi = {
  list: (q?: string, typeFilter?: string, statusFilter?: string) => {
    const params = new URLSearchParams()
    if (q) params.append('q', q)
    if (typeFilter) params.append('typeFilter', typeFilter)
    if (statusFilter) params.append('statusFilter', statusFilter)
    const qs = params.toString()
    return api.get<MaterialItem[]>(`/api/materials${qs ? '?' + qs : ''}`)
  },
  summary: () => api.get<{ tongSKU: number, sapHet: number, daHet: number }>('/api/materials/summary'),
  create: (req: SaveMaterialRequest) => api.post<MaterialItem>('/api/materials', req),
  update: (id: number, req: SaveMaterialRequest) => api.put(`/api/materials/${id}`, req),
  delete: (id: number) => api.del(`/api/materials/${id}`),
  adjust: (id: number, soLuongThucTe: number, lyDo: string) => api.post(`/api/materials/${id}/adjust`, { soLuongThucTe, lyDo })
}
