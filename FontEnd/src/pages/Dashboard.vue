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
            <tr v-for="(o, i) in mockOrders" :key="o.id" class="border-b border-[#EAE3D9]/60 hover:bg-[#F5F2ED] transition-colors group">
              <td class="px-8 py-5 font-bold text-[#2A231E]">{{ o.id }}</td>
              <td class="px-6 py-5 font-semibold text-[#5C544E]">{{ o.table }}</td>
              <td class="px-6 py-5 text-[#8A8178] font-medium">{{ o.items.reduce((s, item) => s + item.qty, 0) }} món</td>
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
import { TrendingUp, ShoppingBag, Users, Coffee, ArrowUpRight, ChevronRight } from 'lucide-vue-next'
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
import { mockOrders, statusMeta } from '../data/orders'
import { formatVND } from '../data/menu'

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

const currentRevenue = "26.330.000₫"

const stats = [
  { label: "Doanh thu hôm nay", value: "4.280.000₫", delta: "+12.4%", icon: TrendingUp },
  { label: "Đơn hàng", value: "87", delta: "+8 đơn", icon: ShoppingBag },
  { label: "Khách phục vụ", value: "142", delta: "+18%", icon: Users },
  { label: "Món bán chạy nhất", value: "Cappuccino", delta: "32 ly", icon: Coffee },
]

const revenueData = [
  { day: "T2", revenue: 2800000 },
  { day: "T3", revenue: 3200000 },
  { day: "T4", revenue: 2950000 },
  { day: "T5", revenue: 3800000 },
  { day: "T6", revenue: 4200000 },
  { day: "T7", revenue: 5100000 },
  { day: "CN", revenue: 4280000 },
]

const topItems = [
  { name: "Cappuccino", qty: 32 },
  { name: "Cà phê sữa đá", qty: 28 },
  { name: "Trà sữa trân châu", qty: 24 },
  { name: "Matcha đá xay", qty: 19 },
  { name: "Tiramisu", qty: 14 },
]

const lineChartData = {
  labels: revenueData.map(d => d.day),
  datasets: [
    {
      label: 'Doanh thu',
      data: revenueData.map(d => d.revenue),
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
}

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
        callback: (value: any) => `${value / 1000000}M`
      }
    }
  }
}

const barChartData = {
  labels: topItems.map(d => d.name),
  datasets: [
    {
      label: 'Số lượng',
      data: topItems.map(d => d.qty),
      backgroundColor: '#CC8033',
      borderRadius: 6,
      barThickness: 24
    }
  ]
}

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
