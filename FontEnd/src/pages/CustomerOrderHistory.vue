<template>
  <div class="min-h-screen bg-[#FDFBF7] font-premium-sans text-[#2A231E]">

    <!-- ── Header ── -->
    <header class="sticky top-0 z-30 bg-[#FDFBF7]/80 backdrop-blur-xl border-b border-[#EAE3D9]">
      <div class="max-w-[860px] mx-auto px-4 sm:px-6 h-16 flex items-center justify-between">
        <router-link :to="'/menu/' + tableId" class="flex items-center gap-2.5">
          <div class="w-9 h-9 flex items-center justify-center bg-[#2A231E] text-[#FDFBF7] rounded-lg shadow-md">
            <Coffee class="w-4 h-4" stroke-width="1.5" />
          </div>
          <span class="font-premium-serif text-xl font-bold tracking-wide text-[#2A231E]">{{ storeInfoStore.tenQuan }}</span>
        </router-link>
        <div class="flex items-center gap-3">
          <span class="px-3 py-1.5 rounded-lg bg-[#FAF6F0] border border-[#EAE3D9] text-xs font-bold text-[#CC8033] shadow-sm">
            Bàn số {{ tableId }}
          </span>
          <button v-if="isLoggedIn" @click="logout"
            class="text-xs font-semibold text-[#8A8178] hover:text-[#2A231E] transition-colors px-3 py-1.5 rounded-lg hover:bg-[#F5F2ED]">
            Đăng xuất
          </button>
          <router-link :to="'/menu/' + tableId"
            class="flex items-center gap-1.5 px-4 h-9 rounded-xl bg-[#CC8033] text-white text-sm font-bold hover:bg-[#B8722D] transition-colors shadow-sm">
            <UtensilsCrossed class="w-3.5 h-3.5" /> Gọi thêm món
          </router-link>
        </div>
      </div>
    </header>

    <!-- ── Main Content Area ── -->
    <div class="max-w-[860px] mx-auto px-4 sm:px-6 pb-16 pt-6">

      <!-- ── LOYALTY CARD / LOGIN PROMPT ── -->
      <div v-if="isLoggedIn" class="relative overflow-hidden rounded-[28px] bg-gradient-to-br from-[#2A231E] via-[#3A2D22] to-[#6B4A2E] p-7 sm:p-8 shadow-[0_24px_60px_rgba(42,35,30,0.28)]">
        <div class="absolute -top-16 -right-8 w-56 h-56 rounded-full bg-[#CC8033]/30 blur-[80px] pointer-events-none"></div>
        <div class="absolute -bottom-24 -left-10 w-64 h-64 rounded-full bg-[#E8973D]/20 blur-[90px] pointer-events-none"></div>

        <!-- Profile row -->
        <div class="relative flex items-center gap-4 sm:gap-5">
          <div class="w-16 h-16 rounded-2xl bg-gradient-to-br from-[#E8973D] to-[#CC8033] flex items-center justify-center text-white text-2xl font-bold shadow-xl shrink-0">
            {{ customerName.charAt(0).toUpperCase() }}
          </div>
          <div class="min-w-0">
            <div class="flex flex-wrap items-center gap-2">
              <h1 class="font-premium-serif text-2xl sm:text-3xl font-bold text-white leading-tight">{{ customerName }}</h1>
              <span class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full bg-white/15 border border-white/25 text-[#E8C58A] text-[10px] font-bold uppercase tracking-widest backdrop-blur-sm">
                <Star class="w-3 h-3 fill-[#E8C58A]" /> Thành viên Bạc
              </span>
            </div>
            <p class="text-sm text-white/50 mt-1 font-medium">{{ customerPhone }}</p>
          </div>
        </div>

        <!-- Stats row -->
        <div class="relative mt-6 grid grid-cols-3 gap-2 sm:gap-3">
          <div class="bg-white/10 backdrop-blur-sm rounded-2xl p-3 sm:p-4 border border-white/15 text-center">
            <p class="text-[10px] uppercase tracking-widest text-white/50 font-bold mb-1">Điểm tích lũy</p>
            <p class="text-2xl sm:text-3xl font-premium-serif font-bold text-[#E8973D]">{{ totalPoints }}</p>
          </div>
          <div class="bg-white/10 backdrop-blur-sm rounded-2xl p-3 sm:p-4 border border-white/15 text-center">
            <p class="text-[10px] uppercase tracking-widest text-white/50 font-bold mb-1">Đơn hàng</p>
            <p class="text-2xl sm:text-3xl font-premium-serif font-bold text-white">{{ orderHistory.length }}</p>
          </div>
          <div class="bg-white/10 backdrop-blur-sm rounded-2xl p-3 sm:p-4 border border-white/15 text-center">
            <p class="text-[10px] uppercase tracking-widest text-white/50 font-bold mb-1">Tổng chi tiêu</p>
            <p class="text-lg sm:text-2xl font-premium-serif font-bold text-white leading-tight">{{ formatTotal(totalSpent) }}</p>
          </div>
        </div>
      </div>

      <!-- Guest / Not logged in (Show Login Prompt Banner) -->
      <div v-else class="bg-white rounded-3xl border border-[#EAE3D9]/60 p-6 shadow-sm flex flex-col md:flex-row items-center justify-between gap-6">
        <div class="flex items-center gap-4 text-center md:text-left flex-col md:flex-row">
          <div class="w-14 h-14 rounded-2xl bg-[#CC8033]/15 flex items-center justify-center shrink-0 text-[#CC8033] border border-[#CC8033]/20 shadow-inner">
            <Star class="w-6 h-6 fill-[#CC8033]" />
          </div>
          <div>
            <h3 class="font-bold text-base text-[#2A231E]">Tích điểm & nhận voucher 10%</h3>
            <p class="text-xs text-[#8A8178] mt-0.5 leading-relaxed">Đăng nhập thành viên để tích lũy điểm thưởng và nhận quà ưu đãi.</p>
          </div>
        </div>
        
        <!-- Login Form inline -->
        <div class="flex flex-col sm:flex-row gap-2 w-full md:w-auto shrink-0">
          <input v-model="loginName" type="text" placeholder="Họ và tên" class="h-11 px-4 rounded-xl border-2 border-[#EAE3D9] text-xs font-semibold focus:border-[#CC8033] focus:outline-none bg-[#FAF6F0] text-[#2A231E]" />
          <input v-model="loginPhone" type="tel" placeholder="Số điện thoại" maxlength="10" class="h-11 px-4 rounded-xl border-2 border-[#EAE3D9] text-xs font-semibold focus:border-[#CC8033] focus:outline-none bg-[#FAF6F0] text-[#2A231E]" />
          <button @click="login" :disabled="!loginName.trim() || loginPhone.length < 9" class="h-11 px-5 rounded-xl bg-[#CC8033] text-white text-xs font-bold hover:bg-[#B8722D] disabled:opacity-40 disabled:cursor-not-allowed transition-colors shrink-0 shadow-md">
            Đăng nhập ngay
          </button>
        </div>
      </div>

      <!-- Vouchers (Only if logged in) -->
      <div v-if="isLoggedIn" class="mt-5">
        <h2 class="font-premium-serif text-xl font-bold text-[#2A231E] mb-3 flex items-center gap-2">
          <Gift class="w-5 h-5 text-[#CC8033]" /> Voucher của bạn
        </h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <div class="relative overflow-hidden bg-gradient-to-r from-[#CC8033] to-[#D97724] rounded-2xl p-5 text-white shadow-[0_8px_24px_rgba(204,128,51,0.35)]">
            <div class="absolute -right-4 -top-4 w-24 h-24 rounded-full bg-white/10 pointer-events-none"></div>
            <div class="absolute right-10 -bottom-8 w-20 h-20 rounded-full bg-white/10 pointer-events-none"></div>
            <div class="relative">
              <p class="text-[10px] font-bold uppercase tracking-[0.2em] text-white/65 mb-1">Voucher thành viên</p>
              <p class="text-3xl font-premium-serif font-bold">Giảm 10%</p>
              <p class="text-[11px] text-white/70 font-medium mt-1.5">Áp dụng cho đơn từ 50.000đ · HSD 31/12/2026</p>
              <div class="mt-3 flex items-center justify-between">
                <code class="text-xs font-bold bg-white/25 border border-white/30 px-3 py-1.5 rounded-lg tracking-widest">SILVER10</code>
                <button @click="copyVoucher('SILVER10')"
                  class="flex items-center gap-1 text-[11px] font-bold text-white/80 hover:text-white transition-colors">
                  <Copy class="w-3.5 h-3.5" /> {{ copied ? 'Đã sao chép!' : 'Sao chép' }}
                </button>
              </div>
            </div>
          </div>
          <div class="relative flex flex-col items-center justify-center rounded-2xl border-2 border-dashed border-[#EAE3D9] p-5 text-center bg-[#FAF6F0]">
            <div class="w-10 h-10 rounded-xl bg-[#F5F2ED] flex items-center justify-center mb-2.5 border border-[#EAE3D9]">
              <Lock class="w-5 h-5 text-[#C5BEB8]" stroke-width="1.5" />
            </div>
            <p class="text-sm font-bold text-[#5C544E]">Voucher Vàng 15%</p>
            <p class="text-[11px] text-[#8A8178] mt-1 leading-relaxed">
              Tích thêm <span class="font-bold text-[#CC8033]">{{ Math.max(0, 500 - totalPoints) }} điểm</span> để mở khóa
            </p>
            <div class="mt-3 h-1.5 w-full rounded-full bg-[#EAE3D9] overflow-hidden">
              <div class="h-full rounded-full bg-[#CC8033]/40 transition-all duration-700"
                :style="{ width: Math.min((totalPoints / 500) * 100, 100) + '%' }"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Points history mini (Only if logged in) -->
      <div v-if="isLoggedIn" class="mt-5 bg-white rounded-2xl border border-[#EAE3D9] overflow-hidden shadow-sm">
        <div class="px-5 py-4 border-b border-[#F5F2ED] flex items-center gap-2">
          <TrendingUp class="w-4 h-4 text-[#CC8033]" />
          <h3 class="text-sm font-bold text-[#2A231E]">Điểm gần đây</h3>
        </div>
        <div class="divide-y divide-[#F5F2ED]">
          <div v-for="entry in pointsLog" :key="entry.id" class="flex items-center justify-between px-5 py-3">
            <div class="flex items-center gap-3">
              <div :class="['w-8 h-8 rounded-xl flex items-center justify-center shrink-0', entry.type === 'earn' ? 'bg-green-50' : 'bg-red-50']">
                <component :is="entry.type === 'earn' ? ArrowUpRight : ArrowDownLeft" :class="['w-4 h-4', entry.type === 'earn' ? 'text-green-600' : 'text-red-500']" />
              </div>
              <div>
                <p class="text-sm font-semibold text-[#2A231E]">{{ entry.label }}</p>
                <p class="text-[11px] text-[#8A8178]">{{ entry.date }}</p>
              </div>
            </div>
            <span :class="['text-sm font-bold', entry.type === 'earn' ? 'text-green-600' : 'text-red-500']">
              {{ entry.type === 'earn' ? '+' : '-' }}{{ entry.points }} điểm
            </span>
          </div>
        </div>
      </div>

      <!-- ── ORDER STATUS SECTION (Always visible) ── -->
      <div class="mt-8">
        <div class="flex items-center justify-between mb-4">
          <h2 class="font-premium-serif text-xl font-bold text-[#2A231E] flex items-center gap-2">
            <ClipboardList class="w-5 h-5 text-[#CC8033]" /> Trạng thái món & Lịch sử đơn bàn {{ tableId }}
          </h2>
          <span class="text-xs text-[#8A8178] font-bold bg-[#FAF6F0] border border-[#EAE3D9] px-2.5 py-1 rounded-lg shadow-inner flex items-center gap-1.5">
            <span class="w-1.5 h-1.5 rounded-full bg-green-500 animate-pulse"></span>
            Đang tự động cập nhật
          </span>
        </div>

        <!-- Empty -->
        <div v-if="orderHistory.length === 0"
          class="text-center py-16 bg-white rounded-3xl border border-[#EAE3D9]">
          <div class="w-16 h-16 rounded-2xl border border-dashed border-[#EAE3D9] flex items-center justify-center mx-auto mb-4 bg-[#FAF6F0]">
            <ShoppingBag class="w-7 h-7 text-[#D5CEC4]" stroke-width="1.5" />
          </div>
          <p class="font-premium-serif text-lg font-bold text-[#2A231E]">Chưa gọi món nào</p>
          <p class="text-xs text-[#8A8178] mt-1.5 font-medium">Bàn của bạn chưa gửi đơn đặt món nào lên hệ thống.</p>
          <router-link :to="'/menu/' + tableId"
            class="inline-flex items-center gap-2 mt-5 px-5 py-2.5 rounded-xl bg-[#CC8033] text-white text-sm font-bold hover:bg-[#B8722D] transition-colors shadow-md">
            <UtensilsCrossed class="w-4 h-4" /> Xem thực đơn & Gọi món ngay
          </router-link>
        </div>

        <!-- List of active orders and items -->
        <div v-else class="space-y-4">
          <div v-for="order in orderHistory" :key="order.id"
            class="bg-white rounded-3xl border border-[#EAE3D9] overflow-hidden shadow-sm hover:shadow-md transition-all group">
            
            <!-- Order Header -->
            <div class="flex items-center justify-between gap-3 px-5 py-4 border-b border-[#F5F2ED]">
              <div class="flex items-center gap-3 min-w-0">
                <div :class="['w-10 h-10 rounded-xl flex items-center justify-center shrink-0', statusBg(order.status)]">
                  <component :is="statusIcon(order.status)" class="w-5 h-5" :class="statusIconColor(order.status)" stroke-width="1.5" />
                </div>
                <div class="min-w-0">
                  <div class="flex items-center flex-wrap gap-2">
                    <h4 class="font-bold text-sm text-[#2A231E]">Đơn hàng {{ order.id }}</h4>
                    <span :class="['text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full', statusPill(order.status)]">
                      {{ statusLabel(order.status) }}
                    </span>
                  </div>
                  <p class="text-[11px] text-[#8A8178] mt-0.5">Đặt lúc: {{ order.createdAt }}</p>
                </div>
              </div>
              <div class="text-right shrink-0">
                <p class="font-bold text-base text-[#CC8033]">{{ formatVND(order.total) }}</p>
                <p v-if="isLoggedIn" class="text-[10px] font-bold text-green-600 mt-0.5">+{{ pointsFromOrder(order) }} điểm</p>
              </div>
            </div>

            <!-- Detailed Item List showing Preparation Status -->
            <div class="divide-y divide-[#F5F2ED]/60 bg-[#FAF6F0]/40">
              <div v-for="(item, idx) in order.items" :key="idx" 
                class="flex items-center justify-between px-5 py-3 text-xs font-semibold text-[#5C544E]">
                <div class="flex items-center gap-2">
                  <span class="w-1.5 h-1.5 rounded-full bg-[#CC8033]/60"></span>
                  <span>{{ item.name }}</span>
                  <span class="text-[#CC8033] font-bold bg-[#FFF9F2] px-1.5 py-0.5 rounded border border-[#CC8033]/10 ml-1">×{{ item.qty }}</span>
                </div>
                <span :class="['text-[10px] font-bold px-2 py-0.5 rounded-md uppercase tracking-wider', getBepColor(item.trangThaiBep)]">
                  {{ getBepLabel(item.trangThaiBep) }}
                </span>
              </div>
            </div>

            <!-- Total display / Paid badge -->
            <div class="px-5 py-3 flex justify-between items-center bg-white border-t border-[#F5F2ED]">
              <span class="text-[10px] uppercase font-bold text-[#8A8178] tracking-widest">Trạng thái thanh toán</span>
              <span v-if="order.paid" class="inline-flex items-center gap-1 text-[10px] font-bold text-green-700 bg-green-50 border border-green-200 px-2.5 py-0.5 rounded-full">
                <CheckCircle2 class="w-3 h-3" /> Đã thanh toán
              </span>
              <span v-else class="inline-flex items-center gap-1 text-[10px] font-bold text-amber-700 bg-amber-50 border border-amber-200 px-2.5 py-0.5 rounded-full">
                Chờ thanh toán
              </span>
            </div>

          </div>
        </div>
      </div>

      <ServiceRequestFAB :table-id="tableId" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { RouterLink } from 'vue-router'
import {
  Coffee, User, Star, Gift, Sparkles, ShoppingBag, History, ClipboardList,
  UtensilsCrossed, Copy, Lock, TrendingUp, ArrowUpRight, ArrowDownLeft,
  CheckCircle2, Clock, CheckCheck, XCircle, Loader2
} from 'lucide-vue-next'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { ordersApi } from '@/services/orders'
import ServiceRequestFAB from '@/components/ServiceRequestFAB.vue'

const STORAGE_KEY = 'brewCustomerProfile'
const storeInfoStore = useStoreInfoStore()

// ── Table Info ──
const tableId = ref(localStorage.getItem('customerTableId') || '5')

// ── Auth / Profile ──
const isLoggedIn = ref(false)
const customerName = ref('')
const customerPhone = ref('')
const loginName = ref('')
const loginPhone = ref('')

onMounted(() => {
  const saved = localStorage.getItem(STORAGE_KEY)
  if (saved) {
    try {
      const p = JSON.parse(saved)
      customerName.value = p.name
      customerPhone.value = p.phone
      isLoggedIn.value = true
    } catch {}
  }
})

function login() {
  if (!loginName.value.trim() || loginPhone.value.length < 9) return
  customerName.value = loginName.value.trim()
  customerPhone.value = loginPhone.value
  localStorage.setItem(STORAGE_KEY, JSON.stringify({ name: customerName.value, phone: customerPhone.value }))
  isLoggedIn.value = true
  fetchHistory() // Refresh point calculation
}

function logout() {
  localStorage.removeItem(STORAGE_KEY)
  isLoggedIn.value = false
  customerName.value = ''
  customerPhone.value = ''
  loginName.value = ''
  loginPhone.value = ''
}

// ── Real Orders from Backend ──
const rawOrders = ref<any[]>([])
const loading = ref(false)

async function fetchHistory() {
  try {
    const data = await ordersApi.guestHistory(parseInt(tableId.value))
    rawOrders.value = data
  } catch (e) {
    console.error('Lỗi tải lịch sử đơn:', e)
  }
}

let pollInterval: any = null
onMounted(async () => {
  loading.value = true
  await fetchHistory()
  loading.value = false
  
  // Realtime updates: Poll every 4 seconds
  pollInterval = setInterval(fetchHistory, 4000)
})

onUnmounted(() => {
  if (pollInterval) clearInterval(pollInterval)
})

const orderHistory = computed(() => {
  return rawOrders.value.map(o => {
    let status: 'pending' | 'preparing' | 'done' | 'cancelled' = 'pending'
    if (o.trangThaiDon === 'ChoXacNhan') status = 'pending'
    else if (o.trangThaiDon === 'DangPha') status = 'preparing'
    else if (o.trangThaiDon === 'HoanThanh') status = 'done'
    else if (o.trangThaiDon === 'Huy') status = 'cancelled'

    const date = new Date(o.thoiGianTao)
    const timeStr = date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })

    return {
      id: `DH-${o.maDonHang}`,
      rawId: o.maDonHang,
      table: o.tenBan || `Bàn ${o.maBan}`,
      createdAt: timeStr,
      total: o.thanhTien,
      status: status,
      items: o.items.map((i: any) => ({
        name: i.tenMon + (i.tenKichCo ? ` (${i.tenKichCo})` : ''),
        qty: i.soLuong,
        trangThaiBep: i.trangThaiBep
      })),
      paid: o.trangThaiDon === 'HoanThanh'
    }
  })
})

const totalSpent = computed(() => orderHistory.value.reduce((s, o) => s + o.total, 0))
const pointsFromOrder = (o: any) => Math.floor(o.total / 10000)
const earnedPoints = computed(() => orderHistory.value.reduce((s, o) => s + pointsFromOrder(o), 0))
const totalPoints = computed(() => 150 + earnedPoints.value)

function formatTotal(n: number) {
  if (n >= 1_000_000) return (n / 1_000_000).toFixed(1).replace('.0', '') + 'tr'
  if (n >= 1_000) return Math.round(n / 1_000) + 'k'
  return formatVND(n)
}

const pointsLog = computed(() => [
  { id: 1, type: 'earn', label: 'Chào mừng thành viên mới', date: 'Hôm nay', points: 50 },
  { id: 2, type: 'earn', label: 'Đăng ký tích điểm lần đầu', date: 'Hôm nay', points: 100 },
  ...orderHistory.value.slice(0, 3).map((o, i) => ({
    id: 100 + i, type: 'earn' as const,
    label: `Đơn hàng ${o.id} · ${o.table}`,
    date: o.createdAt,
    points: pointsFromOrder(o),
  })),
])

// ── Voucher copy ──
const copied = ref(false)
function copyVoucher(code: string) {
  navigator.clipboard.writeText(code).catch(() => {})
  copied.value = true
  setTimeout(() => { copied.value = false }, 2000)
}

const formatVND = (n: number) => (n || 0).toLocaleString('vi-VN') + 'đ'

// ── Status helpers ──
function statusLabel(s: 'pending' | 'preparing' | 'done' | 'cancelled') {
  const m = { pending: 'Đang chờ', preparing: 'Đang pha chế', done: 'Hoàn thành', cancelled: 'Đã huỷ' }
  return m[s]
}
function statusPill(s: 'pending' | 'preparing' | 'done' | 'cancelled') {
  const m = {
    pending: 'bg-amber-50 text-amber-600 border border-amber-200',
    preparing: 'bg-blue-50 text-blue-600 border border-blue-200',
    ready: 'bg-emerald-50 text-emerald-700 border border-emerald-200',
    done: 'bg-green-50 text-green-700 border border-green-200',
    cancelled: 'bg-red-50 text-red-500 border border-red-200',
  }
  return m[s]
}
function statusBg(s: 'pending' | 'preparing' | 'done' | 'cancelled') {
  const m = { pending: 'bg-amber-50', preparing: 'bg-blue-50', done: 'bg-green-50', cancelled: 'bg-red-50' }
  return m[s]
}
function statusIcon(s: 'pending' | 'preparing' | 'done' | 'cancelled') {
  return { pending: Clock, preparing: Loader2, done: CheckCheck, cancelled: XCircle }[s]
}
function statusIconColor(s: 'pending' | 'preparing' | 'done' | 'cancelled') {
  return { pending: 'text-amber-500', preparing: 'text-blue-500', done: 'text-green-600', cancelled: 'text-red-500' }[s]
}

function getBepLabel(status: string) {
  const m: Record<string, string> = {
    ChoLam: 'Chờ pha chế',
    DangLam: 'Đang pha chế',
    HoanThanh: 'Đã xong',
    DaTraKhach: 'Đã phục vụ'
  }
  return m[status] || status
}
function getBepColor(status: string) {
  const m: Record<string, string> = {
    ChoLam: 'text-amber-600 bg-amber-50 border border-amber-100',
    DangLam: 'text-blue-600 bg-blue-50 border border-blue-100',
    HoanThanh: 'text-green-600 bg-green-50 border border-green-100',
    DaTraKhach: 'text-gray-500 bg-gray-50 border border-gray-100'
  }
  return m[status] || 'text-gray-500 bg-gray-100'
}

const loginBenefits = [
  { emoji: '⭐', label: 'Tích điểm' },
  { emoji: '🎁', label: 'Voucher 10%' },
  { emoji: '📋', label: 'Lịch sử đơn' },
  { emoji: '☕', label: 'Mua tặng ưu đãi' },
]
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans {
  font-family: 'Be Vietnam Pro', system-ui, sans-serif;
}
</style>
