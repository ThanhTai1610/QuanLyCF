import { api } from './api'

export interface DashboardStatsDto {
  todayRevenue: number
  revenueDelta: number
  todayOrders: number
  ordersDelta: number
  customers: number
  customersDelta: number
  bestItemName: string
  bestItemQty: number
}

export interface DailyRevenueDto {
  day: string
  revenue: number
}

export interface TopItemDto {
  name: string
  qty: number
}

export interface RecentOrderDto {
  id: number
  table: string
  itemsCount: number
  total: number
  status: string
  createdAt: string
}

export interface DashboardDataDto {
  stats: DashboardStatsDto
  revenueData: DailyRevenueDto[]
  topItems: TopItemDto[]
  recentOrders: RecentOrderDto[]
}

export const dashboardApi = {
  getDashboardData: () => api.get<DashboardDataDto>('/api/dashboard')
}
