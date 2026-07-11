import { api } from './api'

export interface Transaction {
  date: string
  note: string
  points: number
}

export interface Customer {
  id: number
  name: string
  phone: string
  email?: string
  note?: string
  tier: string
  points: number
  totalSpend: number
  lastVisit: string
  visits: number
}

export interface CustomerDetail extends Customer {
  history: Transaction[]
}

export interface SaveCustomerBody {
  name: string
  phone: string
  email?: string
  note?: string
}

export interface Reward {
  id: number
  name: string
  cost: number
  description?: string
}

export const loyaltyApi = {
  list: (q?: string, tier?: string) => {
    const query = new URLSearchParams()
    if (q) query.append('q', q)
    if (tier && tier !== 'Tất cả') query.append('tier', tier)
    return api.get<Customer[]>(`/api/customers?${query.toString()}`)
  },
  get: (id: number) => api.get<CustomerDetail>(`/api/customers/${id}`),
  create: (body: SaveCustomerBody) => api.post<{ id: number }>('/api/customers', body),
  update: (id: number, body: SaveCustomerBody) => api.put<void>(`/api/customers/${id}`, body),
  remove: (id: number) => api.del<void>(`/api/customers/${id}`),
  getRewards: () => api.get<Reward[]>('/api/customers/rewards'),
  sendOtp: (customerId: number) => api.post<{ otp: string | null }>(`/api/customers/${customerId}/send-otp`),
  redeem: (customerId: number, rewardId: number, otp: string) => 
    api.post<{ points: number }>(`/api/customers/${customerId}/redeem`, { rewardId, otp }),
  checkPublicEmail: (email: string) => 
    api.get<{ id: number; name: string; phone: string; email: string; tier: string; points: number }>(`/api/customers/public/by-email?email=${email}`),
  registerPublic: (body: { name: string; phone: string; email: string }) => 
    api.post<{ id: number; name: string; phone: string; email: string; tier: string; points: number }>('/api/customers/public/register', body),
  
  // Public OTP/Loyalty endpoints
  sendPublicOtp: (customerId: number) => 
    api.post<{ message: string }>(`/api/customers/public/${customerId}/send-otp`),
  verifyPublicOtp: (customerId: number, otp: string) => 
    api.post<{ success: boolean }>(`/api/customers/public/${customerId}/verify-otp`, { otp }),
  redeemPublicPoints: (customerId: number, points: number, otp: string, maDonHang?: number) => 
    api.post<{ points: number }>(`/api/customers/public/${customerId}/redeem-points`, { points, otp, maDonHang })
}
