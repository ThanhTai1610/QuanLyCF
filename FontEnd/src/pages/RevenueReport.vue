<template>
  <div class="space-y-6 font-sans text-[#2A231E]">

    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
      <div>
        <h2 class="text-2xl font-bold text-[#2A231E]">Báo cáo doanh thu</h2>
        <p class="text-sm text-[#8A8178] mt-1">Thống kê doanh thu theo tháng và năm</p>
      </div>
    </div>

    <!-- Bộ lọc -->
    <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-4 flex flex-wrap items-center gap-4">
      <div class="flex items-center gap-2">
        <label class="text-sm font-semibold text-[#5C544E]">Năm:</label>
        <select
          id="select-year"
          v-model="selectedYear"
          @change="onFilterChange"
          class="border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white text-[#2A231E] focus:outline-none focus:border-[#CC8033] transition-colors"
        >
          <option v-for="y in yearOptions" :key="y" :value="y">{{ y }}</option>
        </select>
      </div>

      <div class="flex items-center gap-2">
        <label class="text-sm font-semibold text-[#5C544E]">Tháng:</label>
        <select
          id="select-month"
          v-model="selectedMonth"
          @change="onFilterChange"
          class="border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white text-[#2A231E] focus:outline-none focus:border-[#CC8033] transition-colors"
        >
          <option :value="null">Cả năm</option>
          <option v-for="m in 12" :key="m" :value="m">Tháng {{ m }}</option>
        </select>
      </div>

      <div class="ml-auto flex items-center gap-2 text-xs text-[#8A8178]">
        <span v-if="loading" class="flex items-center gap-1.5">
          <span class="w-3 h-3 border-2 border-[#CC8033] border-t-transparent rounded-full animate-spin inline-block"></span>
          Đang tải...
        </span>
        <span v-else-if="report">
          Cập nhật lúc {{ new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }) }}
        </span>
      </div>
    </div>

    <!-- Thông báo lỗi -->
    <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 rounded-xl p-4 text-sm">
      {{ error }}
    </div>

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div
        v-for="kpi in kpiCards" :key="kpi.label"
        class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-5 hover:-translate-y-0.5 transition-transform duration-300 relative overflow-hidden"
      >
        <div class="absolute top-0 left-0 w-full h-1 rounded-t-xl" :class="kpi.barColor"></div>
        <div class="flex items-start justify-between">
          <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="kpi.iconBg">
            <component :is="kpi.icon" class="w-5 h-5" :class="kpi.iconColor" stroke-width="1.8" />
          </div>
          <span
            class="text-[11px] font-bold px-2 py-1 rounded-full flex items-center gap-1"
            :class="kpi.growthPositive ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'"
          >
            <component :is="kpi.growthPositive ? ArrowUpRight : ArrowDownRight" class="w-3 h-3" />
            {{ kpi.growth }}
          </span>
        </div>
        <div class="mt-4">
          <div class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold">{{ kpi.label }}</div>
          <div class="text-2xl font-bold text-[#2A231E] mt-1 tabular-nums">
            <span v-if="loading" class="inline-block w-24 h-7 bg-[#EAE3D9] rounded animate-pulse"></span>
            <span v-else>{{ kpi.value }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Biểu đồ + Top sản phẩm -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">

      <!-- Biểu đồ doanh thu -->
      <div class="lg:col-span-2 bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-6">
        <div class="flex items-end justify-between mb-6">
          <div>
            <h3 class="text-lg font-bold text-[#2A231E]">
              {{ selectedMonth ? `Doanh thu tháng ${selectedMonth}/${selectedYear}` : `Doanh thu năm ${selectedYear}` }}
            </h3>
            <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mt-1">
              {{ selectedMonth ? 'Theo từng ngày' : 'Theo từng tháng' }}
            </p>
          </div>
          <span class="text-xl font-bold text-[#CC8033] tabular-nums">{{ formatVND(report?.totalRevenue || 0) }}</span>
        </div>

        <div class="w-full h-[280px]">
          <div v-if="loading" class="h-full flex items-center justify-center">
            <span class="w-8 h-8 border-4 border-[#CC8033] border-t-transparent rounded-full animate-spin"></span>
          </div>
          <!-- @ts-ignore -->
          <Bar v-else-if="chartData.labels.length" :data="chartData" :options="chartOptions" />
          <div v-else class="h-full flex items-center justify-center text-[#8A8178] text-sm">
            Không có dữ liệu trong kỳ này
          </div>
        </div>
      </div>

      <!-- Top sản phẩm -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-6 flex flex-col">
        <h3 class="text-lg font-bold text-[#2A231E] mb-1">Top sản phẩm</h3>
        <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mb-5">Doanh thu cao nhất kỳ này</p>

        <div v-if="loading" class="space-y-3 flex-1">
          <div v-for="i in 5" :key="i" class="h-10 bg-[#EAE3D9] rounded-lg animate-pulse"></div>
        </div>

        <div v-else-if="!topProducts.length" class="flex-1 flex items-center justify-center text-[#8A8178] text-sm">
          Chưa có dữ liệu
        </div>

        <div v-else class="space-y-3 flex-1 overflow-y-auto">
          <div
            v-for="(p, i) in topProducts" :key="p.name"
            class="flex items-center gap-3 group"
          >
            <span
              class="w-6 h-6 rounded-full text-[11px] font-bold flex items-center justify-center shrink-0"
              :class="i === 0 ? 'bg-[#CC8033] text-white' : i === 1 ? 'bg-[#E5A55A] text-white' : i === 2 ? 'bg-[#F0C98A] text-[#2A231E]' : 'bg-[#F5F2ED] text-[#8A8178]'"
            >{{ i + 1 }}</span>
            <div class="flex-1 min-w-0">
              <div class="text-sm font-semibold text-[#2A231E] truncate">{{ p.name }}</div>
              <div class="flex items-center gap-2 mt-0.5">
                <div class="flex-1 h-1.5 bg-[#EAE3D9] rounded-full overflow-hidden">
                  <div
                    class="h-full bg-[#CC8033] rounded-full transition-all duration-700"
                    :style="{ width: `${maxProductRevenue > 0 ? (p.revenue / maxProductRevenue) * 100 : 0}%` }"
                  ></div>
                </div>
                <span class="text-[11px] text-[#8A8178] font-medium whitespace-nowrap">{{ p.qty }} món</span>
              </div>
            </div>
            <span class="text-sm font-bold text-[#CC8033] tabular-nums whitespace-nowrap">{{ formatVND(p.revenue) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Bảng chi tiết theo tháng (hoặc ngày khi drill-down) -->
    <div v-if="report" class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
      <div class="px-6 py-4 border-b border-[#EAE3D9] flex justify-between items-center">
        <h3 class="text-base font-bold text-[#2A231E]">
          {{ selectedMonth ? `Chi tiết từng ngày — Tháng ${selectedMonth}/${selectedYear}` : `Chi tiết từng tháng — Năm ${selectedYear}` }}
        </h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-widest border-b border-[#EAE3D9]">
              <th class="px-6 py-3 text-left font-bold">{{ selectedMonth ? 'Ngày' : 'Tháng' }}</th>
              <th class="px-6 py-3 text-right font-bold">Doanh thu</th>
              <th class="px-6 py-3 text-right font-bold">Số đơn</th>
              <th class="px-6 py-3 text-right font-bold">TB/Đơn</th>
              <th class="px-6 py-3 text-left font-bold w-36">Tỉ trọng</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="item in paginatedData" :key="selectedMonth ? (item as DailyRevenueDetailDto).day : (item as MonthlyRevenueDto).month"
              class="border-b border-[#EAE3D9]/60 hover:bg-[#FDFBF7] transition-colors cursor-pointer"
              @click="!selectedMonth ? drillDown((item as MonthlyRevenueDto).month) : null"
            >
              <td class="px-6 py-3.5 font-semibold text-[#2A231E]">
                {{ selectedMonth ? (item as DailyRevenueDetailDto).dayLabel : (item as MonthlyRevenueDto).monthLabel }}
              </td>
              <td class="px-6 py-3.5 text-right font-bold text-[#CC8033] tabular-nums">{{ formatVND(item.revenue) }}</td>
              <td class="px-6 py-3.5 text-right text-[#5C544E] tabular-nums">{{ item.orders }}</td>
              <td class="px-6 py-3.5 text-right text-[#5C544E] tabular-nums">
                {{ formatVND(selectedMonth ? (item.orders > 0 ? Math.round(item.revenue / item.orders, 0) : 0) : (item as MonthlyRevenueDto).avgOrder) }}
              </td>
              <td class="px-6 py-3.5">
                <div class="flex items-center gap-2">
                  <div class="flex-1 h-2 bg-[#EAE3D9] rounded-full overflow-hidden">
                    <div
                      class="h-full bg-[#CC8033] rounded-full"
                      :style="{ width: `${report.totalRevenue > 0 ? (item.revenue / report.totalRevenue) * 100 : 0}%` }"
                    ></div>
                  </div>
                  <span class="text-[11px] text-[#8A8178] w-9 text-right">
                    {{ report.totalRevenue > 0 ? ((item.revenue / report.totalRevenue) * 100).toFixed(1) : 0 }}%
                  </span>
                </div>
              </td>
            </tr>
            <!-- Tổng cộng -->
            <tr class="bg-[#F5F2ED] font-bold">
              <td class="px-6 py-3.5 text-[#2A231E]">Tổng cộng</td>
              <td class="px-6 py-3.5 text-right text-[#CC8033] tabular-nums">{{ formatVND(report.totalRevenue) }}</td>
              <td class="px-6 py-3.5 text-right text-[#2A231E] tabular-nums">{{ report.totalOrders }}</td>
              <td class="px-6 py-3.5 text-right text-[#2A231E] tabular-nums">{{ formatVND(report.avgOrderValue) }}</td>
              <td class="px-6 py-3.5 text-[11px] text-[#8A8178]">100%</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Điều khiển phân trang -->
      <div v-if="totalPages > 1" class="px-6 py-4 border-t border-[#EAE3D9] flex items-center justify-between bg-[#FDFBF7]">
        <div class="text-xs text-[#8A8178]">
          Hiển thị dòng {{ startRow }} - {{ endRow }} trên tổng số {{ totalRows }} dòng
        </div>
        <div class="flex items-center gap-2">
          <button
            @click="currentPage = Math.max(1, currentPage - 1)"
            :disabled="currentPage === 1"
            class="px-3 py-1.5 rounded-lg border border-[#EAE3D9] bg-white text-xs font-semibold text-[#5C544E] hover:bg-[#F5F2ED] disabled:opacity-50 disabled:hover:bg-white transition-colors"
          >
            Trước
          </button>
          <span class="text-xs font-semibold text-[#2A231E]">Trang {{ currentPage }} / {{ totalPages }}</span>
          <button
            @click="currentPage = Math.min(totalPages, currentPage + 1)"
            :disabled="currentPage === totalPages"
            class="px-3 py-1.5 rounded-lg border border-[#EAE3D9] bg-white text-xs font-semibold text-[#5C544E] hover:bg-[#F5F2ED] disabled:opacity-50 disabled:hover:bg-white transition-colors"
          >
            Sau
          </button>
        </div>
      </div>
    </div>

    <!-- Gợi ý drill-down khi xem tháng -->
    <div v-if="selectedMonth" class="text-center">
      <button
        @click="selectedMonth = null; onFilterChange()"
        class="text-sm text-[#CC8033] font-semibold hover:underline"
      >
        ← Quay lại xem cả năm {{ selectedYear }}
      </button>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { TrendingUp, ShoppingBag, ReceiptText, TrendingDown, ArrowUpRight, ArrowDownRight } from 'lucide-vue-next'
import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale, LinearScale,
  BarElement, Title, Tooltip, Legend
} from 'chart.js'
import { formatVND } from '@/data/menu'
import { dashboardApi, type MonthlyReportDto, type MonthlyRevenueDto, type DailyRevenueDetailDto } from '@/services/dashboard'

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

// --- State ---
const currentYear = new Date().getFullYear()
const selectedYear = ref<number>(currentYear)
const selectedMonth = ref<number | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)
const report = ref<MonthlyReportDto | null>(null)

// Phân trang
const currentPage = ref(1)
const pageSize = 6 // Hiển thị 6 dòng mỗi trang cho cân đối

const yearOptions = Array.from({ length: 5 }, (_, i) => currentYear - i)

// --- Fetch ---
async function fetchReport() {
  loading.value = true
  error.value = null
  try {
    report.value = await dashboardApi.getRevenueReport(
      selectedYear.value,
      selectedMonth.value ?? undefined
    )
    currentPage.value = 1 // Reset về trang 1 sau khi tải lại bộ lọc
  } catch (e: any) {
    error.value = e.message || 'Không thể tải dữ liệu báo cáo.'
  } finally {
    loading.value = false
  }
}

function onFilterChange() {
  fetchReport()
}

function drillDown(month: number) {
  selectedMonth.value = month
  fetchReport()
}

onMounted(fetchReport)

// --- Phân trang Logic ---
const rawDataList = computed(() => {
  if (!report.value) return []
  return selectedMonth.value ? report.value.dailyData : report.value.monthlyData
})

const totalRows = computed(() => rawDataList.value.length)
const totalPages = computed(() => Math.ceil(totalRows.value / pageSize))

const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * pageSize
  return rawDataList.value.slice(start, start + pageSize)
})

const startRow = computed(() => (currentPage.value - 1) * pageSize + 1)
const endRow = computed(() => Math.min(currentPage.value * pageSize, totalRows.value))

// --- KPI Cards ---
const kpiCards = computed(() => {
  const r = report.value
  const growth = r?.growthPercent ?? 0
  const growthLabel = growth > 0 ? `+${growth}%` : `${growth}%`
  return [
    {
      label: 'Tổng doanh thu',
      value: formatVND(r?.totalRevenue ?? 0),
      growth: growthLabel,
      growthPositive: growth >= 0,
      icon: TrendingUp,
      iconBg: 'bg-orange-50',
      iconColor: 'text-[#CC8033]',
      barColor: 'bg-[#CC8033]'
    },
    {
      label: 'Số đơn hàng',
      value: (r?.totalOrders ?? 0).toLocaleString('vi-VN'),
      growth: growthLabel,
      growthPositive: growth >= 0,
      icon: ShoppingBag,
      iconBg: 'bg-blue-50',
      iconColor: 'text-blue-500',
      barColor: 'bg-blue-400'
    },
    {
      label: 'Giá trị TB/Đơn',
      value: formatVND(r?.avgOrderValue ?? 0),
      growth: growthLabel,
      growthPositive: growth >= 0,
      icon: ReceiptText,
      iconBg: 'bg-green-50',
      iconColor: 'text-green-500',
      barColor: 'bg-green-400'
    },
    {
      label: 'Tăng trưởng',
      value: growthLabel,
      growth: 'So kỳ trước',
      growthPositive: growth >= 0,
      icon: growth >= 0 ? TrendingUp : TrendingDown,
      iconBg: growth >= 0 ? 'bg-green-50' : 'bg-red-50',
      iconColor: growth >= 0 ? 'text-green-500' : 'text-red-500',
      barColor: growth >= 0 ? 'bg-green-400' : 'bg-red-400'
    }
  ]
})

// --- Chart ---
const chartData = computed(() => {
  const r = report.value
  if (!r) return { labels: [], datasets: [] }

  const items = r.month ? r.dailyData : r.monthlyData
  const labels = r.month
    ? (r.dailyData.map(d => d.dayLabel))
    : (r.monthlyData.map(m => m.monthLabel))
  const revenues = r.month
    ? r.dailyData.map(d => d.revenue)
    : r.monthlyData.map(m => m.revenue)

  return {
    labels,
    datasets: [{
      label: 'Doanh thu',
      data: revenues,
      backgroundColor: revenues.map((v, i) => {
        const max = Math.max(...revenues)
        return max > 0 && v === max ? '#CC8033' : 'rgba(204,128,51,0.35)'
      }),
      borderRadius: 6,
      borderSkipped: false,
    }]
  }
})

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#2A231E',
      titleColor: '#FDFBF7',
      bodyColor: '#FDFBF7',
      padding: 12,
      cornerRadius: 8,
      displayColors: false,
      callbacks: {
        label: (ctx: any) => `Doanh thu: ${formatVND(ctx.raw)}`
      }
    }
  },
  scales: {
    x: {
      grid: { display: false },
      ticks: { font: { family: 'Be Vietnam Pro', size: 11 }, color: '#8A8178', maxRotation: 45 }
    },
    y: {
      grid: { color: '#EAE3D9', borderDash: [3, 3] },
      border: { display: false },
      ticks: {
        color: '#8A8178',
        font: { family: 'Be Vietnam Pro', size: 11 },
        callback: (v: any) => {
          if (v >= 1_000_000) return `${(v / 1_000_000).toFixed(1)}M`
          if (v >= 1_000) return `${(v / 1_000).toFixed(0)}k`
          return v
        }
      }
    }
  }
}

// --- Top products ---
const topProducts = computed(() => report.value?.topProducts ?? [])
const maxProductRevenue = computed(() => Math.max(...topProducts.value.map(p => p.revenue), 0))
</script>
