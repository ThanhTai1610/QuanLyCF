import { api } from './api'

export interface StockTakeLineRequest {
  maNguyenLieu: number
  soLuongThucTe: number
  lyDoLech: string | null
}

export interface CreateStockTakeRequest {
  ghiChu: string | null
  chiTiets: StockTakeLineRequest[]
}

export interface StockTakeListItem {
  maPhieu: number
  thoiGianTao: string
  trangThai: string
  soMatHang: number
}

export interface StockTakeLineItem {
  maNguyenLieu: number
  tenNguyenLieu: string
  tonHeThong: number
  tonThucTe: number | null
  chenhLech: number
  lyDoLech: string | null
}

export interface StockTakeDetail {
  maPhieu: number
  thoiGianTao: string
  trangThai: string
  ghiChu: string | null
  chiTiets: StockTakeLineItem[]
}

export const stockTakeApi = {
  list: (trangThai?: string) => api.get<StockTakeListItem[]>('/api/stock-takes', { params: { trangThai } }),
  get: (id: number) => api.get<StockTakeDetail>(`/api/stock-takes/${id}`),
  create: (data: CreateStockTakeRequest) => api.post<{ maPhieu: number }>('/api/stock-takes', data),
  approve: (id: number) => api.post(`/api/stock-takes/${id}/approve`),
  reject: (id: number) => api.post(`/api/stock-takes/${id}/reject`),
}
