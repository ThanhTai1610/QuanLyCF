import { api } from './api'

export interface ChamCongItem {
  maChamCong: number
  maCa: number | null
  tenCa: string
  date: string
  timeIn: string
  timeOut: string
  imgIn: string | null
  imgOut: string | null
  timeInExact: string
  timeOutExact: string
  total: string
  ghiChu?: string
}

export interface DonTuItem {
  maDon: number
  loaiDon: string
  thoiGianLienQuan: string
  lyDo: string
  trangThai: string // ChoDuyet, DaDuyet, TuChoi
  thoiGianTao: string
  tenNhanVien?: string
}

export const hrApi = {
  getMyCheckins: async (employeeId?: number): Promise<ChamCongItem[]> => {
    const url = employeeId ? `/api/hr/my-checkins?employeeId=${employeeId}` : '/api/hr/my-checkins'
    return await api.get<ChamCongItem[]>(url)
  },
  checkIn: async (payload: { type: 'in' | 'out'; maCa?: number | null; photoUrl?: string | null; ghiChu?: string | null; maNhanVien?: number | null }) => {
    return await api.post<{ message: string; id: number }>('/api/hr/check-in', payload)
  },
  getMyRequests: async (): Promise<DonTuItem[]> => {
    return await api.get<DonTuItem[]>('/api/hr/my-requests')
  },
  createRequest: async (payload: { loaiDon: string; thoiGianLienQuan: string; lyDo: string; maNhanVien?: number | null }) => {
    return await api.post<{ message: string; id: number }>('/api/hr/create-request', payload)
  },
  getActiveCheckins: async (): Promise<ChamCongItem[]> => {
    return await api.get<ChamCongItem[]>('/api/hr/active-checkins')
  },
  forceCheckout: async (id: number, reason?: string) => {
    return await api.post<{ message: string }>(`/api/hr/force-checkout/${id}`, { reason })
  },
  getAllRequests: async (): Promise<DonTuItem[]> => {
    return await api.get<DonTuItem[]>('/api/hr/all-requests')
  },
  reviewRequest: async (id: number, status: 'DaDuyet' | 'TuChoi') => {
    return await api.post<{ message: string }>(`/api/hr/review-request/${id}`, { status })
  },
  getShifts: async (): Promise<{ maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }[]> => {
    return await api.get<{ maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }[]>('/api/hr/shifts')
  },
  getEmployees: async (): Promise<{ maNhanVien: number; hoTen: string }[]> => {
    return await api.get<{ maNhanVien: number; hoTen: string }[]>('/api/hr/employees')
  }
}
