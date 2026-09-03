import { api } from './api'

export interface AuditLogItem {
  maNhatKy: number
  maNhanVien: number | null
  tenNhanVien: string | null
  hanhDong: string
  module: string
  duLieuCu: string | null
  duLieuMoi: string | null
  diaChiIP: string | null
  thietBi: string | null
  thoiGianTao: string
}

export interface AuditLogResponse {
  data: AuditLogItem[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

export const auditLogsApi = {
  getPaged(params: {
    module?: string
    hanhDong?: string
    maNhanVien?: number
    page?: number
    pageSize?: number
  }) {
    const query = new URLSearchParams()
    if (params.module && params.module !== 'Tất cả') query.append('module', params.module)
    if (params.hanhDong) query.append('hanhDong', params.hanhDong)
    if (params.maNhanVien) query.append('maNhanVien', String(params.maNhanVien))
    if (params.page) query.append('page', String(params.page))
    if (params.pageSize) query.append('pageSize', String(params.pageSize))

    return api.get<AuditLogResponse>(`/api/audit-logs?${query.toString()}`)
  },

  getModules() {
    return api.get<string[]>('/api/audit-logs/modules')
  },

  clearLogs() {
    return api.del<void>('/api/audit-logs')
  },

  createLog(payload: {
    maNhanVien?: number | null
    hanhDong: string
    module: string
    duLieuCu?: string | null
    duLieuMoi?: string | null
    thietBi?: string | null
  }) {
    return api.post<AuditLogItem>('/api/audit-logs', payload)
  }
}
