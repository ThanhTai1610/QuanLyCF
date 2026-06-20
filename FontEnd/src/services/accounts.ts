// Goi API quan ly tai khoan + vai tro/quyen. Token JWT da duoc api.ts tu gan.
import { api } from './api'

export interface Account {
  maNhanVien: number
  hoTen: string
  email: string
  maVaiTro: number
  tenVaiTro: string
  trangThaiHoatDong: boolean
  soDienThoai: string | null
  lanDangNhapCuoi: string | null
}
export interface CreateAccountBody {
  hoTen: string
  email: string
  maVaiTro: number
  matKhau: string
  pin: string | null
  soDienThoai: string | null
}
export interface UpdateAccountBody {
  hoTen: string
  email: string
  maVaiTro: number
  trangThaiHoatDong: boolean
  soDienThoai: string | null
}

export interface PermissionItem {
  maQuyen: number
  maCode: string
  tenQuyen: string
  nhom: string
}
export interface RoleDetail {
  maVaiTro: number
  tenVaiTro: string
  moTa: string | null
  laVaiTroHeThong: boolean
  soTaiKhoan: number
  quyens: string[]   // danh sach maCode
}
export interface SaveRoleBody {
  tenVaiTro: string
  moTa: string | null
  quyens: string[]
}

export const accountsApi = {
  list: (q?: string, maVaiTro?: number) => {
    const p = new URLSearchParams()
    if (q) p.set('q', q)
    if (maVaiTro) p.set('maVaiTro', String(maVaiTro))
    const qs = p.toString()
    return api.get<Account[]>(`/api/accounts${qs ? '?' + qs : ''}`)
  },
  create: (b: CreateAccountBody) => api.post<{ maNhanVien: number }>('/api/accounts', b),
  update: (id: number, b: UpdateAccountBody) => api.put<void>(`/api/accounts/${id}`, b),
  toggleLock: (id: number) => api.post<{ trangThaiHoatDong: boolean }>(`/api/accounts/${id}/toggle-lock`),
  resetPassword: (id: number, matKhauMoi: string) =>
    api.post<void>(`/api/accounts/${id}/reset-password`, { matKhauMoi }),
  setPin: (id: number, pin: string) => api.post<void>(`/api/accounts/${id}/set-pin`, { pin }),
  remove: (id: number) => api.del<void>(`/api/accounts/${id}`),
}

export const rolesApi = {
  list: () => api.get<RoleDetail[]>('/api/roles'),
  permissions: () => api.get<PermissionItem[]>('/api/roles/permissions'),
  create: (b: SaveRoleBody) => api.post<{ maVaiTro: number }>('/api/roles', b),
  update: (id: number, b: SaveRoleBody) => api.put<void>(`/api/roles/${id}`, b),
  remove: (id: number) => api.del<void>(`/api/roles/${id}`),
}
