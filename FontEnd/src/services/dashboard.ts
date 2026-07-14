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
  getDashboardData: () => api.get<DashboardDataDto>('/api/dashboard'),
  getRevenueReport: (year: number, month?: number) => {
    const q = month ? `?year=${year}&month=${month}` : `?year=${year}`
    return api.get<MonthlyReportDto>(`/api/dashboard/revenue-report${q}`)
  }
}

// --- Types cho báo cáo theo tháng/năm ---
export interface MonthlyRevenueDto {
  month: number
  monthLabel: string
  revenue: number
  orders: number
  avgOrder: number
}

export interface DailyRevenueDetailDto {
  day: number
  dayLabel: string
  revenue: number
  orders: number
}

export interface TopProductRevenueDto {
  name: string
  qty: number
  revenue: number
}

export interface MonthlyReportDto {
  year: number
  month: number | null
  totalRevenue: number
  totalOrders: number
  avgOrderValue: number
  growthPercent: number
  monthlyData: MonthlyRevenueDto[]
  dailyData: DailyRevenueDetailDto[]
  topProducts: TopProductRevenueDto[]
}
