// Goi API module Ban & Khu vuc (Sales/Tables). Token JWT da duoc api.ts tu gan.
import { api } from './api'

export type TableStatus = 'Trong' | 'CoKhach' | 'BaoTri'

// Khop ZoneItem cua BE
export interface Zone {
  maKhuVuc: number
  tenKhuVuc: string
  phuThu: number
  soBan: number
}

// Khop TableItem cua BE
export interface TableItem {
  maBan: number
  tenBan: string
  maKhuVuc: number
  tenKhuVuc: string
  sucChua: number | null
  trangThai: TableStatus
  maQRHash: string
  urlDatMon: string
  maBanChinh: number | null   // bàn chính của nhóm ghép (null = không ghép / là bàn chính)
  tenBanChinh: string | null
}

export interface SaveZoneBody {
  tenKhuVuc: string
  phuThu: number
}

export interface SaveTableBody {
  tenBan: string
  maKhuVuc: number
  sucChua: number | null
}

export const tablesApi = {
  // ── Khu vuc ──
  listZones: () => api.get<Zone[]>('/api/zones'),
  createZone: (b: SaveZoneBody) => api.post<{ maKhuVuc: number }>('/api/zones', b),
  updateZone: (id: number, b: SaveZoneBody) => api.put<void>(`/api/zones/${id}`, b),
  deleteZone: (id: number) => api.del<void>(`/api/zones/${id}`),

  // ── Ban ──
  list: (params?: { maKhuVuc?: number; trangThai?: string }) => {
    const q = new URLSearchParams()
    if (params?.maKhuVuc) q.set('maKhuVuc', String(params.maKhuVuc))
    if (params?.trangThai) q.set('trangThai', params.trangThai)
    const qs = q.toString()
    return api.get<TableItem[]>(`/api/tables${qs ? '?' + qs : ''}`)
  },
  create: (b: SaveTableBody) => api.post<TableItem>('/api/tables', b),
  update: (id: number, b: SaveTableBody) => api.put<void>(`/api/tables/${id}`, b),
  updateStatus: (id: number, trangThai: TableStatus) =>
    api.put<{ trangThai: TableStatus }>(`/api/tables/${id}/status`, { trangThai }),
  regenerateQr: (id: number) =>
    api.post<{ maQRHash: string; urlDatMon: string }>(`/api/tables/${id}/regenerate-qr`),
  remove: (id: number) => api.del<void>(`/api/tables/${id}`),

  // Ghép / tách bàn
  merge: (maBanChinh: number, maThanhVien: number[]) =>
    api.post<void>('/api/tables/merge', { maBanChinh, maThanhVien }),
  unmerge: (id: number) => api.post<void>(`/api/tables/${id}/unmerge`),
}
