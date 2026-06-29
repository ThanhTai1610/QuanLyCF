<template>
  <div class="min-h-screen bg-[#FDFBF7] font-premium-sans text-[#2A231E]">

    <!-- ── Header ── -->
    <header class="sticky top-0 z-30 bg-[#FDFBF7]/80 backdrop-blur-xl border-b border-[#EAE3D9]">
      <div class="max-w-[860px] mx-auto px-4 sm:px-6 h-16 flex items-center justify-between">
        <router-link to="/" class="flex items-center gap-2.5">
          <div class="w-9 h-9 flex items-center justify-center bg-[#2A231E] text-[#FDFBF7] rounded-lg shadow-md">
            <Coffee class="w-4 h-4" stroke-width="1.5" />
          </div>
          <span class="font-premium-serif text-xl font-bold tracking-wide text-[#2A231E]">{{ storeInfoStore.tenQuan }}</span>
        </router-link>
        <div class="flex items-center gap-3">
          <button v-if="isLoggedIn" @click="logout"
            class="text-xs font-semibold text-[#8A8178] hover:text-[#2A231E] transition-colors px-3 py-1.5 rounded-lg hover:bg-[#F5F2ED]">
            Đăng xuất
          </button>
          <router-link to="/menu/5"
            class="flex items-center gap-1.5 px-4 h-9 rounded-xl bg-[#CC8033] text-white text-sm font-bold hover:bg-[#B8722D] transition-colors shadow-sm">
            <UtensilsCrossed class="w-3.5 h-3.5" /> Gọi thêm món
          </router-link>
        </div>
      </div>
    </header>

    <!-- ── Chưa đăng nhập ── -->
    <div v-if="!isLoggedIn" class="max-w-[440px] mx-auto px-4 pt-14 pb-8">
      <div class="bg-white rounded-3xl border border-[#EAE3D9] shadow-[0_24px_64px_rgba(42,35,30,0.1)] overflow-hidden">
        <!-- Gradient top band -->
        <div class="h-2 bg-gradient-to-r from-[#CC8033] via-[#E8973D] to-[#D97724]"></div>
        <div class="p-7">
          <div class="flex flex-col items-center text-center mb-7">
            <div class="w-16 h-16 rounded-2xl bg-gradient-to-br from-[#CC8033]/15 to-[#E8973D]/10 flex items-center justify-center mb-4 border border-[#CC8033]/20">
              <History class="w-8 h-8 text-[#CC8033]" stroke-width="1.5" />
            </div>
            <h1 class="font-premium-serif text-2xl font-bold text-[#2A231E]">Lịch sử đơn hàng</h1>
            <p class="text-sm text-[#8A8178] mt-1.5 font-medium leading-relaxed max-w-[280px]">
              Đăng nhập để xem điểm tích lũy, voucher và toàn bộ lịch sử mua hàng của bạn.
            </p>
          </div>

          <!-- Benefits -->
          <div class="grid grid-cols-2 gap-2 mb-6">
            <div v-for="b in loginBenefits" :key="b.label"
              class="flex items-center gap-2 px-3 py-2.5 rounded-xl bg-[#FAF6F0] border border-[#EAE3D9]">
              <span class="text-lg shrink-0">{{ b.emoji }}</span>
              <span class="text-[10px] font-bold text-[#5C544E] uppercase tracking-tight leading-tight">{{ b.label }}</span>
            </div>
          </div>

          <!-- Form -->
          <div class="space-y-3">
            <div>
              <label class="text-[11px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block">Họ và tên</label>
              <div class="relative">
                <User class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-[#8A8178]" />
                <input v-model="loginName" type="text" placeholder="Nguyễn Văn A" @keyup.enter="login"
                  class="w-full h-12 pl-10 pr-4 rounded-xl border-2 border-[#EAE3D9] focus:border-[#CC8033] focus:outline-none text-sm font-medium bg-[#FAF6F0] text-[#2A231E] placeholder:text-[#A89F95] transition-colors" />
              </div>
            </div>
            <div>
              <label class="text-[11px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block">Số điện thoại</label>
              <div class="relative">
                <span class="absolute left-3.5 top-1/2 -translate-y-1/2 text-sm font-bold text-[#8A8178]">🇻🇳 +84</span>
                <input v-model="loginPhone" type="tel" placeholder="9xx xxx xxx" maxlength="10" @keyup.enter="login"
                  class="w-full h-12 pl-[4.75rem] pr-4 rounded-xl border-2 border-[#EAE3D9] focus:border-[#CC8033] focus:outline-none text-sm font-medium bg-[#FAF6F0] text-[#2A231E] placeholder:text-[#A89F95] transition-colors" />
              </div>
            </div>
            <button @click="login" :disabled="!loginName.trim() || loginPhone.length < 9"
              class="w-full h-12 rounded-xl bg-gradient-to-r from-[#CC8033] to-[#D97724] text-white text-sm font-bold uppercase tracking-wide shadow-md hover:shadow-[0_8px_24px_rgba(204,128,51,0.4)] disabled:opacity-40 disabled:cursor-not-allowed transition-all">
              Xem lịch sử ngay
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- ── Đã đăng nhập ── -->
    <div v-else class="max-w-[860px] mx-auto px-4 sm:px-6 pb-16">

      <!-- Hero profile -->
      <div class="mt-6 relative overflow-hidden rounded-[28px] bg-gradient-to-br from-[#2A231E] via-[#3A2D22] to-[#6B4A2E] p-7 sm:p-8 shadow-[0_24px_60px_rgba(42,35,30,0.28)]">
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

      <!-- Points progress bar -->
      <div class="mt-4 bg-white rounded-2xl border border-[#EAE3D9] p-5 shadow-sm">
        <div class="flex items-start justify-between mb-3 gap-3">
          <div>
            <p class="text-sm font-bold text-[#2A231E]">Tiến độ lên hạng Vàng</p>
            <p class="text-xs text-[#8A8178] mt-0.5">
              Cần thêm <span class="font-bold text-[#CC8033]">{{ Math.max(0, 500 - totalPoints) }} điểm</span> để đạt Thành viên Vàng 🏆
            </p>
          </div>
          <span class="text-xs font-bold text-[#8A8178] shrink-0">{{ totalPoints }}/500</span>
        </div>
        <div class="h-3 rounded-full bg-[#F5F2ED] overflow-hidden">
          <div class="h-full rounded-full bg-gradient-to-r from-[#CC8033] to-[#E8973D] transition-all duration-700 ease-out"
            :style="{ width: Math.min((totalPoints / 500) * 100, 100) + '%' }"></div>
        </div>
        <div class="flex justify-between mt-2">
          <div class="flex items-center gap-1.5">
            <div class="w-4 h-4 rounded-full bg-[#D5CEC4] flex items-center justify-center">
              <Star class="w-2.5 h-2.5 text-white fill-white" />
            </div>
            <span class="text-[10px] font-bold text-[#8A8178] uppercase tracking-widest">Bạc</span>
          </div>
          <div class="flex items-center gap-1.5">
            <span class="text-[10px] font-bold text-[#CC8033] uppercase tracking-widest">Vàng</span>
            <div class="w-4 h-4 rounded-full bg-gradient-to-br from-[#CC8033] to-[#E8973D] flex items-center justify-center">
              <Star class="w-2.5 h-2.5 text-white fill-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Vouchers -->
      <div class="mt-5">
        <h2 class="font-premium-serif text-xl font-bold text-[#2A231E] mb-3 flex items-center gap-2">
          <Gift class="w-5 h-5 text-[#CC8033]" /> Voucher của bạn
        </h2>
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <!-- Active voucher -->
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

          <!-- Locked voucher -->
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

      <!-- Points history mini -->
      <div class="mt-5 bg-white rounded-2xl border border-[#EAE3D9] overflow-hidden shadow-sm">
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

      <!-- Order history -->
      <div class="mt-7">
        <div class="flex items-center justify-between mb-4">
          <h2 class="font-premium-serif text-xl font-bold text-[#2A231E] flex items-center gap-2">
            <ClipboardList class="w-5 h-5 text-[#CC8033]" /> Lịch sử đơn hàng
          </h2>
          <span class="text-xs text-[#8A8178] font-medium">{{ orderHistory.length }} đơn</span>
        </div>

        <!-- Empty -->
        <div v-if="orderHistory.length === 0"
          class="text-center py-16 bg-white rounded-2xl border border-[#EAE3D9]">
          <div class="w-16 h-16 rounded-2xl border border-dashed border-[#EAE3D9] flex items-center justify-center mx-auto mb-4 bg-[#FAF6F0]">
            <ShoppingBag class="w-7 h-7 text-[#D5CEC4]" stroke-width="1.5" />
          </div>
          <p class="font-premium-serif text-lg font-bold text-[#2A231E]">Chưa có đơn nào</p>
          <p class="text-xs text-[#8A8178] mt-1.5 font-medium">Hãy gọi món để bắt đầu lịch sử mua hàng.</p>
          <router-link to="/menu/5"
            class="inline-flex items-center gap-2 mt-5 px-5 py-2.5 rounded-xl bg-[#CC8033] text-white text-sm font-bold hover:bg-[#B8722D] transition-colors shadow-md">
            <UtensilsCrossed class="w-4 h-4" /> Gọi món ngay
          </router-link>
        </div>

        <!-- List -->
        <div v-else class="space-y-3">
          <div v-for="order in orderHistory" :key="order.id"
            class="bg-white rounded-2xl border border-[#EAE3D9] overflow-hidden shadow-sm hover:shadow-md transition-all group">
            <!-- Order header -->
            <div class="flex items-center justify-between gap-3 px-5 py-4 border-b border-[#F5F2ED]">
              <div class="flex items-center gap-3 min-w-0">
                <div :class="['w-10 h-10 rounded-xl flex items-center justify-center shrink-0', statusBg(order.status)]">
                  <component :is="statusIcon(order.status)" class="w-5 h-5" :class="statusIconColor(order.status)" stroke-width="1.5" />
                </div>
                <div class="min-w-0">
                  <div class="flex items-center flex-wrap gap-2">
                    <h4 class="font-bold text-sm text-[#2A231E]">{{ order.id }}</h4>
                    <span :class="['text-[10px] font-bold uppercase tracking-wider px-2 py-0.5 rounded-full', statusPill(order.status)]">
                      {{ statusLabel(order.status) }}
                    </span>
                  </div>
                  <p class="text-xs text-[#8A8178] mt-0.5">{{ order.table }} · {{ order.createdAt }}</p>
                </div>
              </div>
              <div class="text-right shrink-0">
                <p class="font-bold text-base text-[#2A231E]">{{ formatVND(order.total) }}</p>
                <p class="text-[11px] font-semibold text-green-600 mt-0.5">+{{ pointsFromOrder(order) }} điểm</p>
              </div>
            </div>
            <!-- Items chips -->
            <div class="px-5 py-3 flex flex-wrap gap-1.5 bg-[#FAF6F0]">
              <span v-for="(item, idx) in order.items.slice(0, 4)" :key="idx"
                class="inline-flex items-center gap-1 text-[11px] font-semibold text-[#5C544E] bg-white border border-[#EAE3D9] px-2.5 py-1 rounded-lg">
                {{ item.name }} <span class="text-[#CC8033] font-bold">×{{ item.qty }}</span>
              </span>
              <span v-if="order.items.length > 4"
                class="text-[11px] font-semibold text-[#8A8178] bg-white border border-[#EAE3D9] px-2.5 py-1 rounded-lg">
                +{{ order.items.length - 4 }} món
              </span>
              <span v-if="order.paid"
                class="ml-auto inline-flex items-center gap-1 text-[10px] font-bold text-green-700 bg-green-50 border border-green-200 px-2 py-0.5 rounded-full">
                <CheckCircle2 class="w-3 h-3" /> Đã thanh toán
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import {
  Coffee, User, Star, Gift, Sparkles, ShoppingBag, History, ClipboardList,
  UtensilsCrossed, Copy, Lock, TrendingUp, ArrowUpRight, ArrowDownLeft,
  CheckCircle2, Clock, CheckCheck, XCircle, Loader2
} from 'lucide-vue-next'
import { useOrderStore } from '@/stores/orders'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { formatVND } from '@/data/menu'
import type { Order, OrderStatus } from '@/data/orders'

const STORAGE_KEY = 'brewCustomerProfile'
const orderStore     = useOrderStore()
const storeInfoStore = useStoreInfoStore()

// ── Auth ──
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
}

function logout() {
  localStorage.removeItem(STORAGE_KEY)
  isLoggedIn.value = false
  customerName.value = ''
  customerPhone.value = ''
  loginName.value = ''
  loginPhone.value = ''
}

// ── Orders & points ──
const orderHistory = computed(() => orderStore.orders)
const totalSpent = computed(() => orderHistory.value.reduce((s, o) => s + o.total, 0))
const pointsFromOrder = (o: Order) => Math.floor(o.total / 10000)
const earnedPoints = computed(() => orderHistory.value.reduce((s, o) => s + pointsFromOrder(o), 0))
const totalPoints = computed(() => 150 + earnedPoints.value)

function formatTotal(n: number) {
  if (n >= 1_000_000) return (n / 1_000_000).toFixed(1).replace('.0', '') + 'tr'
  if (n >= 1_000) return Math.round(n / 1_000) + 'k'
  return formatVND(n)
}

// ── Points log (mock) ──
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

// ── Status helpers ──
function statusLabel(s: OrderStatus) {
  const m: Record<OrderStatus, string> = { pending: 'Đang chờ', preparing: 'Đang pha chế', done: 'Hoàn thành', cancelled: 'Đã huỷ' }
  return m[s]
}
function statusPill(s: OrderStatus) {
  const m: Record<OrderStatus, string> = {
    pending: 'bg-amber-50 text-amber-600 border border-amber-200',
    preparing: 'bg-blue-50 text-blue-600 border border-blue-200',
    done: 'bg-green-50 text-green-700 border border-green-200',
    cancelled: 'bg-red-50 text-red-500 border border-red-200',
  }
  return m[s]
}
function statusBg(s: OrderStatus) {
  const m: Record<OrderStatus, string> = { pending: 'bg-amber-50', preparing: 'bg-blue-50', done: 'bg-green-50', cancelled: 'bg-red-50' }
  return m[s]
}
function statusIcon(s: OrderStatus) {
  return { pending: Clock, preparing: Loader2, done: CheckCheck, cancelled: XCircle }[s]
}
function statusIconColor(s: OrderStatus) {
  return { pending: 'text-amber-500', preparing: 'text-blue-500', done: 'text-green-600', cancelled: 'text-red-500' }[s]
}

const loginBenefits = [
  { emoji: '⭐', label: 'Tích điểm' },
  { emoji: '🎁', label: 'Voucher 10%' },
  { emoji: '📋', label: 'Lịch sử đơn' },
  { emoji: '🎂', label: 'Ưu đãi sinh nhật' },
]
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans {
  font-family: 'Be Vietnam Pro', system-ui, sans-serif;
}
</style>
