import { api } from './api'

export interface CashFlowListItem {
  maDongTien: number
  loaiGiaoDich: string
  nhomGiaoDich: string
  soTien: number
  phuongThucThanhToan: string
  nguoiNopNhan: string | null
  ghiChu: string | null
  thoiGianTao: string
  nguoiGhiNhan: string
}

export interface CreateCashOutRequest {
  nhomGiaoDich: string
  phuongThucThanhToan: string
  soTien: number
  nguoiNopNhan?: string | null
  ghiChu: string
}

export interface CashFlowSummary {
  tongThu: number
  tongChi: number
  dongTienThuan: number
  chiLuong: number
  chiKho: number
  chiKhac: number
}

export interface SalaryListItem {
  maBangLuong: number
  hoTen: string
  tenVaiTro: string
  luongTheoGio: number
  soGioThuong: number
  soGioOT: number
  phuCap: number
  thuong: number
  phat: number
  thucLanh: number
  trangThai: string
}

export const cashFlowService = {
  list(year: number, month: number) {
    return api.get<CashFlowListItem[]>(`/api/cash-flows?year=${year}&month=${month}`)
  },
  summary(year: number, month: number) {
    return api.get<CashFlowSummary>(`/api/cash-flows/summary?year=${year}&month=${month}`)
  },
  salaries(year: number, month: number) {
    return api.get<SalaryListItem[]>(`/api/cash-flows/salaries?year=${year}&month=${month}`)
  },
  create(body: CreateCashOutRequest) {
    return api.post<{ maDongTien: number; message: string }>('/api/cash-flows/out', body)
  }
}
