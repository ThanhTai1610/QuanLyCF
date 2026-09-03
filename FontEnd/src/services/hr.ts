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
  trangThai?: string // ChoDuyet, DaDuyet, TuChoi, HopLe, KhongHopLe
}

export interface DonTuItem {
  maDon: number
  loaiDon: string
  thoiGianLienQuan: string
  lyDo: string
  trangThai: string // ChoDuyet, DaDuyet, TuChoi
  thoiGianTao: string
  tenNhanVien?: string
  ghiChuDuyet?: string
}

export interface PhanCaItem {
  maPhanCa: number
  maNhanVien: number
  tenNhanVien: string
  maCa: number
  tenCa: string
  gio: string
  ngayLamViec: string
  thuTrongTuan: string
  ghiChu?: string
}

export interface EmployeePayrollItem {
  maNhanVien: number
  hoTen: string
  chucVu: string
  luongCoBan: number
  tongGioLam: number
  tongLuong: number
  trangThaiThanhToan?: string // ChuaThanhToan, DaThanhToan
  thoiGianThanhToan?: string
  ghiChuThanhToan?: string
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
    return await api.get<ChamCongItem[] >('/api/hr/active-checkins')
  },
  forceCheckout: async (id: number, reason?: string) => {
    return await api.post<{ message: string }>(`/api/hr/force-checkout/${id}`, { reason })
  },
  getAllRequests: async (): Promise<DonTuItem[]> => {
    return await api.get<DonTuItem[]>('/api/hr/all-requests')
  },
  reviewRequest: async (id: number, status: 'DaDuyet' | 'TuChoi', note?: string) => {
    return await api.post<{ message: string }>(`/api/hr/review-request/${id}`, { status, note })
  },
  reviewCheckIn: async (id: number, status: 'DaDuyet' | 'TuChoi', note?: string) => {
    return await api.post<{ message: string }>(`/api/hr/review-checkin/${id}`, { status, note })
  },
  getShifts: async (): Promise<{ maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }[]> => {
    return await api.get<{ maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }[]>('/api/hr/shifts')
  },
  createShiftDefinition: async (payload: { tenCa: string; gioBatDau: string; gioKetThuc: string }) => {
    return await api.post<{ maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }>('/api/hr/shifts', payload)
  },
  updateShiftDefinition: async (id: number, payload: { tenCa: string; gioBatDau: string; gioKetThuc: string }) => {
    return await api.put<{ message: string }>(`/api/hr/shifts/${id}`, payload)
  },
  getShiftLimits: async (): Promise<{ generalLimitsJson: string; dailyLimitsJson: string }> => {
    return await api.get<{ generalLimitsJson: string; dailyLimitsJson: string }>('/api/hr/shift-limits')
  },
  saveShiftLimits: async (payload: { generalLimitsJson: string; dailyLimitsJson: string }) => {
    return await api.post<{ message: string }>('/api/hr/shift-limits', payload)
  },
  deleteShiftDefinition: async (id: number) => {
    return await api.del<{ message: string }>(`/api/hr/shifts/${id}`)
  },
  getEmployees: async (): Promise<{ maNhanVien: number; hoTen: string; luongCoBan?: number }[]> => {
    return await api.get<{ maNhanVien: number; hoTen: string; luongCoBan?: number }[]>('/api/hr/employees')
  },
  getSchedules: async (): Promise<PhanCaItem[]> => {
    return await api.get<PhanCaItem[]>('/api/hr/schedules')
  },
  createSchedule: async (payload: { maNhanVien: number; maCa: number; ngayLamViec: string; ghiChu?: string }): Promise<PhanCaItem> => {
    return await api.post<PhanCaItem>('/api/hr/schedules', payload)
  },
  deleteSchedule: async (id: number) => {
    return await api.del<{ message: string }>(`/api/hr/schedules/${id}`)
  },
  getPayrollSummary: async (ky?: string): Promise<EmployeePayrollItem[]> => {
    const url = ky ? `/api/hr/payroll?ky=${ky}` : '/api/hr/payroll'
    return await api.get<EmployeePayrollItem[]>(url)
  },
  updateEmployeeRate: async (employeeId: number, luongCoBan: number) => {
    return await api.put<{ message: string }>(`/api/hr/employee-rate/${employeeId}`, { luongCoBan })
  },
  paySalary: async (employeeId: number, payload?: { ky?: string; phuongThuc?: string; ghiChu?: string }) => {
    return await api.post<{ message: string }>(`/api/hr/pay-salary/${employeeId}`, payload || {})
  }
}
