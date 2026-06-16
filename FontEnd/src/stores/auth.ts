import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '@/services/api'

// Khop voi AccountInfo cua BE (serialize camelCase)
export interface NguoiDung {
  maNhanVien: number
  hoTen: string
  email: string
  vaiTro: string
  quyens: string[]
}

// Khop voi TokenResponse cua BE
interface TokenResponse {
  accessToken: string
  refreshToken: string
  hetHan: string
  nguoiDung: NguoiDung
}

export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(localStorage.getItem('accessToken'))
  const refreshToken = ref<string | null>(localStorage.getItem('refreshToken'))
  const user = ref<NguoiDung | null>(JSON.parse(localStorage.getItem('user') || 'null'))

  const isAuthenticated = computed(() => !!accessToken.value)

  function luuPhien(res: TokenResponse) {
    accessToken.value = res.accessToken
    refreshToken.value = res.refreshToken
    user.value = res.nguoiDung
    localStorage.setItem('accessToken', res.accessToken)
    localStorage.setItem('refreshToken', res.refreshToken)
    localStorage.setItem('user', JSON.stringify(res.nguoiDung))
  }

  function xoaPhien() {
    accessToken.value = null
    refreshToken.value = null
    user.value = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('user')
  }

  /** Dang nhap bang email + mat khau (Quan ly / nhan vien). Nem Error neu sai. */
  async function login(email: string, matKhau: string) {
    const res = await api.post<TokenResponse>('/api/auth/login', { email, matKhau })
    luuPhien(res)
  }

  /** Dang nhap ca lam bang PIN (man hinh StaffLogin). Nem Error neu sai. */
  async function loginPin(maNhanVien: number, pin: string) {
    const res = await api.post<TokenResponse>('/api/auth/pin-login', { maNhanVien, pin })
    luuPhien(res)
  }

  async function logout() {
    try {
      if (refreshToken.value) {
        await api.post('/api/auth/logout', { refreshToken: refreshToken.value })
      }
    } catch {
      // Bo qua loi mang khi dang xuat — van xoa phien o client
    }
    xoaPhien()
  }

  /** Kiem tra nguoi dung hien tai co mot ma quyen (vd "BAN_QUANLY"). */
  function coQuyen(maQuyen: string) {
    return user.value?.quyens?.includes(maQuyen) ?? false
  }

  return {
    accessToken,
    refreshToken,
    user,
    isAuthenticated,
    login,
    loginPin,
    logout,
    coQuyen,
  }
})
