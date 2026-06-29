// Client goi API dung chung: tu gan JWT, tu lam moi token khi het han, boc message loi cua BE.
// Mac dinh goi tuong doi ('' -> cung origin) de Vite proxy /api ve backend.
// Dat VITE_API_URL neu muon goi thang toi 1 backend URL cu the.
const BASE_URL = import.meta.env.VITE_API_URL || ''

const TOKEN_KEY = 'accessToken'
const REFRESH_KEY = 'refreshToken'
const USER_KEY = 'user'

function xoaPhien() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(REFRESH_KEY)
  localStorage.removeItem(USER_KEY)
}

// Tranh goi refresh nhieu lan dong thoi (nhieu request 401 cung luc)
let refreshPromise: Promise<boolean> | null = null

async function lamMoiToken(): Promise<boolean> {
  const rt = localStorage.getItem(REFRESH_KEY)
  if (!rt) return false
  try {
    const res = await fetch(`${BASE_URL}/api/auth/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: rt }),
    })
    if (!res.ok) return false
    const data = await res.json()
    localStorage.setItem(TOKEN_KEY, data.accessToken)
    localStorage.setItem(REFRESH_KEY, data.refreshToken)
    localStorage.setItem(USER_KEY, JSON.stringify(data.nguoiDung))
    return true
  } catch {
    return false
  }
}

async function request<T>(method: string, endpoint: string, body?: unknown, daThuLai = false): Promise<T> {
  const token = localStorage.getItem(TOKEN_KEY)
  const res = await fetch(`${BASE_URL}${endpoint}`, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })

  // Token het han → thu lam moi 1 lan roi goi lai (bo qua chinh cac endpoint auth)
  if (res.status === 401 && !daThuLai && !endpoint.startsWith('/api/auth/')) {
    refreshPromise ??= lamMoiToken().finally(() => { refreshPromise = null })
    const ok = await refreshPromise
    if (ok) return request<T>(method, endpoint, body, true)
    // Refresh that bai → het phien, ve trang dang nhap
    xoaPhien()
    if (typeof window !== 'undefined' && !window.location.pathname.startsWith('/login')) {
      window.location.href = '/login'
    }
    throw new Error('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.')
  }

  // Hệ thống đang bảo trì → chuyển hướng về trang bảo trì
  if (res.status === 503 && !endpoint.includes('/api/settings/maintenance')) {
    if (typeof window !== 'undefined' && !window.location.pathname.startsWith('/maintenance')) {
      window.location.href = '/maintenance'
    }
    throw new Error('Hệ thống đang bảo trì. Vui lòng quay lại sau.')
  }

  // 204 No Content (PUT/DELETE) — khong co body
  if (res.status === 204) return undefined as T

  const text = await res.text()
  const data = text ? JSON.parse(text) : null

  if (!res.ok) {
    const message = (data && (data.message || data.error)) || `Lỗi ${res.status}`
    throw new Error(message)
  }

  return data as T
}

export const api = {
  get: <T>(endpoint: string) => request<T>('GET', endpoint),
  post: <T>(endpoint: string, body?: unknown) => request<T>('POST', endpoint, body),
  put: <T>(endpoint: string, body?: unknown) => request<T>('PUT', endpoint, body),
  del: <T>(endpoint: string) => request<T>('DELETE', endpoint),
}

// Giu tuong thich voi code cu (neu co noi dang dung apiFetch)
export const apiFetch = async (endpoint: string, options?: RequestInit) => {
  const token = localStorage.getItem(TOKEN_KEY)
  const response = await fetch(`${BASE_URL}${endpoint}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options?.headers,
    },
  })
  if (!response.ok) {
    throw new Error('Network response was not ok')
  }
  return response.json()
}
