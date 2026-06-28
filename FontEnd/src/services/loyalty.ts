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
    api.post<{ points: number }>(`/api/customers/${customerId}/redeem`, { rewardId, otp })
}
