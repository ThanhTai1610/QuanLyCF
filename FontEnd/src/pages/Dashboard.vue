<template>
  <div class="space-y-8 font-premium-sans text-[#2A231E]">
    <!-- Stats -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
      <div v-for="s in stats" :key="s.label" class="bg-white rounded-lg p-6 border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] group hover:-translate-y-1 transition-all duration-500 relative overflow-hidden">
        <!-- Highlight top border -->
        <div class="absolute top-0 left-0 w-full h-1 bg-[#CC8033] opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
        
        <div class="flex items-start justify-between">
          <div class="w-12 h-12 rounded-full bg-[#F5F2ED] flex items-center justify-center text-[#CC8033] group-hover:bg-[#CC8033] group-hover:text-white transition-colors duration-500 shadow-sm">
            <component :is="s.icon" class="w-5 h-5" stroke-width="1.5" />
          </div>
          <span class="text-[11px] font-bold flex items-center gap-1 px-2.5 py-1 rounded-full bg-green-50 text-green-700 border border-green-100">
            <ArrowUpRight class="w-3 h-3" stroke-width="2.5" /> {{ s.delta }}
          </span>
        </div>
        <div class="mt-6">
          <div class="text-[10px] uppercase tracking-[0.15em] text-[#8A8178] font-bold">{{ s.label }}</div>
          <div class="font-premium-sans text-4xl text-[#2A231E] font-bold mt-2 tracking-tight">{{ s.value }}</div>
        </div>
      </div>
    </div>

    <!-- Charts -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Line Chart -->
      <div class="lg:col-span-2 bg-white rounded-lg p-6 md:p-8 border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)]">
        <div class="flex flex-col sm:flex-row justify-between sm:items-end mb-8 gap-4">
          <div>
            <h3 class="font-premium-serif text-3xl font-bold text-[#2A231E]">Doanh thu 7 ngày qua</h3>
            <p class="text-[10px] uppercase tracking-[0.2em] text-[#8A8178] font-bold mt-2">Cập nhật liên tục theo từng đơn hàng</p>
          </div>
          <span class="text-3xl font-premium-sans font-bold text-[#CC8033]">{{ currentRevenue }}</span>
        </div>
        <div class="w-full h-[300px]">
          <!-- @ts-ignore -->
          <Line :data="lineChartData" :options="lineChartOptions" />
        </div>
      </div>

      <!-- Bar Chart -->
      <div class="bg-white rounded-lg p-6 md:p-8 border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] flex flex-col">
        <h3 class="font-premium-serif text-2xl font-bold text-[#2A231E] mb-1">Top 5 món bán chạy</h3>
        <p class="text-[10px] uppercase tracking-[0.2em] text-[#8A8178] font-bold mb-8">Thống kê hôm nay</p>
        <div class="w-full flex-1 min-h-[260px]">
          <!-- @ts-ignore -->
          <Bar :data="barChartData" :options="barChartOptions" />
        </div>
      </div>
    </div>

    <!-- Recent orders -->
    <div class="bg-white rounded-lg border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] overflow-hidden">
      <div class="p-6 md:p-8 flex justify-between items-end border-b border-[#EAE3D9]">
        <div>
          <h3 class="font-premium-serif text-2xl font-bold text-[#2A231E]">Đơn hàng gần đây</h3>
          <p class="text-[10px] uppercase tracking-[0.2em] text-[#8A8178] font-bold mt-2">7 đơn mới nhất hôm nay</p>
        </div>
        <router-link to="/orders" class="text-xs uppercase tracking-widest text-[#CC8033] font-bold hover:text-[#2A231E] transition-colors flex items-center gap-1">
          Xem tất cả <ChevronRight class="w-3 h-3" />
        </router-link>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-[0.15em] border-b border-[#EAE3D9]">
              <th class="px-8 py-4 font-bold">Mã đơn</th>
              <th class="px-6 py-4 font-bold">Bàn</th>
              <th class="px-6 py-4 font-bold">Số món</th>
              <th class="px-6 py-4 font-bold">Tổng tiền</th>
              <th class="px-6 py-4 font-bold">Trạng thái</th>
              <th class="px-8 py-4 font-bold text-right">Giờ đặt</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading" class="border-b border-[#EAE3D9]/60">
              <td colspan="6" class="px-8 py-8 text-center text-gray-500 font-medium">Đang tải dữ liệu...</td>
            </tr>
            <tr v-else-if="realOrders.length === 0" class="border-b border-[#EAE3D9]/60">
              <td colspan="6" class="px-8 py-8 text-center text-gray-500 font-medium">Hôm nay chưa có đơn hàng nào</td>
            </tr>
            <tr v-else v-for="o in realOrders" :key="o.id" class="border-b border-[#EAE3D9]/60 hover:bg-[#F5F2ED] transition-colors group">
              <td class="px-8 py-5 font-bold text-[#2A231E]">#{{ o.id.toString().padStart(3, '0') }}</td>
              <td class="px-6 py-5 font-semibold text-[#5C544E]">{{ o.table }}</td>
              <td class="px-6 py-5 text-[#8A8178] font-medium">{{ o.itemsCount }} món</td>
              <td class="px-6 py-5 font-bold text-[#CC8033]">{{ formatVND(o.total) }}</td>
              <td class="px-6 py-5">
                <span :class="['px-3 py-1.5 rounded-full text-[11px] font-bold tracking-wide border', statusMeta[o.status]?.className || 'bg-gray-50 text-gray-600 border-gray-200']">
                  {{ statusMeta[o.status]?.label || o.status }}
                </span>
              </td>
              <td class="px-8 py-5 text-[#8A8178] text-right font-medium group-hover:text-[#2A231E] transition-colors">{{ o.createdAt }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { TrendingUp, ShoppingBag, Users, Coffee, ArrowUpRight, ChevronRight, ArrowDownRight, Minus } from 'lucide-vue-next'
import { Line, Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'
import { formatVND } from '../data/menu'
import { dashboardApi, type DashboardDataDto } from '@/services/dashboard'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend
)

const statusMeta: Record<string, { label: string, className: string }> = {
  ChoXacNhan: { label: 'Chờ nhận', className: 'bg-yellow-50 text-yellow-700 border-yellow-200' },
  DangPha: { label: 'Đang pha', className: 'bg-blue-50 text-blue-700 border-blue-200' },
  HoanThanh: { label: 'Hoàn thành', className: 'bg-green-50 text-green-700 border-green-200' },
  Huy: { label: 'Đã hủy', className: 'bg-red-50 text-red-700 border-red-200' }
}

const loading = ref(true)
const dashboardData = ref<DashboardDataDto | null>(null)

onMounted(async () => {
  try {
    loading.value = true
    dashboardData.value = await dashboardApi.getDashboardData()
  } catch (error) {
    console.error('Lỗi khi tải dữ liệu dashboard:', error)
  } finally {
    loading.value = false
  }
})

const currentRevenue = computed(() => formatVND(dashboardData.value?.stats.todayRevenue || 0))

const stats = computed(() => {
  const d = dashboardData.value?.stats
  return [
    { 
      label: "Doanh thu hôm nay", 
      value: formatVND(d?.todayRevenue || 0), 
      delta: d?.revenueDelta ? (d.revenueDelta > 0 ? `+${d.revenueDelta}%` : `${d.revenueDelta}%`) : "0%", 
      icon: TrendingUp 
    },
    { 
      label: "Đơn hàng", 
      value: (d?.todayOrders || 0).toString(), 
      delta: d?.ordersDelta ? (d.ordersDelta > 0 ? `+${d.ordersDelta} đơn` : `${d.ordersDelta} đơn`) : "0 đơn", 
      icon: ShoppingBag 
    },
    { 
      label: "Khách phục vụ", 
      value: (d?.customers || 0).toString(), 
      delta: d?.customersDelta ? (d.customersDelta > 0 ? `+${d.customersDelta}%` : `${d.customersDelta}%`) : "0%", 
      icon: Users 
    },
    { 
      label: "Món bán chạy nhất", 
      value: d?.bestItemName || "Chưa có", 
      delta: d?.bestItemQty ? `${d.bestItemQty} phần` : "0 phần", 
      icon: Coffee 
    },
  ]
})

const revenueData = computed(() => dashboardData.value?.revenueData || [])
const topItems = computed(() => dashboardData.value?.topItems || [])
const realOrders = computed(() => dashboardData.value?.recentOrders || [])

const lineChartData = computed(() => ({
  labels: revenueData.value.map(d => d.day),
  datasets: [
    {
      label: 'Doanh thu',
      data: revenueData.value.map(d => d.revenue),
      borderColor: '#CC8033',
      backgroundColor: 'rgba(204, 128, 51, 0.1)',
      borderWidth: 3,
      pointBackgroundColor: '#CC8033',
      pointBorderColor: '#FFFFFF',
      pointBorderWidth: 2,
      pointRadius: 5,
      pointHoverRadius: 7,
      tension: 0.4,
      fill: true
    }
  ]
}))

const lineChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#2A231E',
      titleColor: '#FDFBF7',
      bodyColor: '#FDFBF7',
      titleFont: { family: 'Be Vietnam Pro', size: 13, weight: 'bold' },
      bodyFont: { family: 'Be Vietnam Pro', size: 14, weight: 'bold' },
      padding: 12,
      cornerRadius: 8,
      displayColors: false,
      callbacks: {
        label: (context: any) => formatVND(context.raw)
      }
    }
  },
  scales: {
    x: { 
      grid: { display: false },
      ticks: { font: { family: 'Be Vietnam Pro', weight: 'bold' }, color: '#8A8178' }
    },
    y: { 
      grid: { color: '#EAE3D9', borderDash: [4, 4] },
      border: { display: false },
      ticks: {
        font: { family: 'Be Vietnam Pro', weight: 'bold' },
        color: '#8A8178',
        callback: (value: any) => {
            if (value === 0) return '0'
            if (value >= 1000000) return `${value / 1000000}M`
            if (value >= 1000) return `${value / 1000}k`
            return value
        }
      }
    }
  }
}

const barChartData = computed(() => ({
  labels: topItems.value.map(d => d.name),
  datasets: [
    {
      label: 'Số lượng',
      data: topItems.value.map(d => d.qty),
      backgroundColor: '#CC8033',
      borderRadius: 6,
      barThickness: 24
    }
  ]
}))

const barChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  indexAxis: 'y' as const,
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#2A231E',
      titleColor: '#FDFBF7',
      bodyColor: '#FDFBF7',
      titleFont: { family: 'Be Vietnam Pro', size: 13, weight: 'bold' },
      bodyFont: { family: 'Be Vietnam Pro', size: 14, weight: 'bold' },
      padding: 12,
      cornerRadius: 8,
      displayColors: false,
    }
  },
  scales: {
    x: { 
      display: false,
      grid: { display: false } 
    },
    y: { 
      grid: { display: false },
      border: { display: false },
      ticks: { font: { family: 'Be Vietnam Pro', weight: 'bold' }, color: '#5C544E' }
    }
  }
}
</script>

<style scoped>
/* Nhúng font chữ sang trọng chuẩn nhà hàng 5 sao */
/* Phông thống nhất toàn web */
.font-premium-serif,
.font-premium-sans {
  font-family: 'Be Vietnam Pro', system-ui, sans-serif;
}
</style>
