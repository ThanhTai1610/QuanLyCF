<template>
  <div class="space-y-6">

    <!-- HEADER STATS -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-4">
      <div
        v-for="stat in stats"
        :key="stat.label"
        class="bg-white rounded-xl border border-cream-deep p-5 shadow-sm flex items-center gap-4"
      >
        <div class="w-11 h-11 rounded-xl flex items-center justify-center flex-shrink-0" :style="{ backgroundColor: stat.bg }">
          <component :is="stat.icon" class="w-5 h-5" :style="{ color: stat.color }" />
        </div>
        <div>
          <div class="text-2xl font-bold text-espresso">{{ stat.value }}</div>
          <div class="text-xs text-muted-foreground mt-0.5">{{ stat.label }}</div>
        </div>
      </div>
    </div>

    <!-- FILTER & SEARCH -->
    <div class="bg-white rounded-xl border border-cream-deep shadow-sm p-4 flex flex-wrap items-center gap-3">
      <div class="relative flex-1 min-w-[200px]">
        <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
        <input
          v-model="searchQuery"
          placeholder="Tìm theo tên hoặc số điện thoại..."
          class="w-full pl-9 pr-4 h-9 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
        />
      </div>

      <div class="flex gap-2 flex-wrap">
        <button
          v-for="tier in ['Tất cả', 'Đồng', 'Bạc', 'Vàng', 'Kim cương']"
          :key="tier"
          @click="selectedTier = tier"
          class="px-3 h-9 rounded-lg text-xs font-semibold border transition-all"
          :class="selectedTier === tier
            ? 'bg-[#CC8033] text-white border-[#CC8033] shadow-md shadow-[#CC8033]/20'
            : 'bg-background border-cream-deep text-espresso hover:border-[#CC8033] hover:text-[#CC8033]'"
        >
          {{ tier }}
        </button>
      </div>

      <button
        @click="showAddModal = true"
        class="ml-auto flex items-center gap-2 px-4 h-9 rounded-lg bg-[#CC8033] text-white text-xs font-bold hover:bg-brown transition-colors shadow-md shadow-[#CC8033]/20"
      >
        <UserPlus class="w-4 h-4" />
        Thêm khách hàng
      </button>
    </div>

    <!-- TABLE -->
    <div class="bg-white rounded-xl border border-cream-deep shadow-sm overflow-hidden">
      <table class="w-full text-sm">
        <thead>
          <tr class="border-b border-cream-deep bg-background">
            <th class="text-left px-5 py-3 text-xs font-bold text-muted-foreground uppercase tracking-wider">Khách hàng</th>
            <th class="text-left px-5 py-3 text-xs font-bold text-muted-foreground uppercase tracking-wider">Hạng thành viên</th>
            <th class="text-left px-5 py-3 text-xs font-bold text-muted-foreground uppercase tracking-wider">Điểm tích lũy</th>
            <th class="text-left px-5 py-3 text-xs font-bold text-muted-foreground uppercase tracking-wider">Tổng chi tiêu</th>
            <th class="text-left px-5 py-3 text-xs font-bold text-muted-foreground uppercase tracking-wider">Lần ghé gần nhất</th>
            <th class="text-left px-5 py-3 text-xs font-bold text-muted-foreground uppercase tracking-wider">Hành động</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-cream-deep">
          <tr
            v-for="customer in filteredCustomers"
            :key="customer.id"
            class="hover:bg-background/50 transition-colors group"
          >
            <td class="px-5 py-3.5">
              <div class="flex items-center gap-3">
                <div
                  class="w-9 h-9 rounded-full flex items-center justify-center text-white font-bold text-sm flex-shrink-0"
                  :style="{ backgroundColor: avatarColor(customer.name) }"
                >
                  {{ customer.name.charAt(0) }}
                </div>
                <div>
                  <div class="font-semibold text-espresso">{{ customer.name }}</div>
                  <div class="text-xs text-muted-foreground">{{ customer.phone }}</div>
                </div>
              </div>
            </td>
            <td class="px-5 py-3.5">
              <span
                class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-[11px] font-bold"
                :class="tierClass(customer.tier)"
              >
                <component :is="tierIcon(customer.tier)" class="w-3 h-3" />
                {{ customer.tier }}
              </span>
            </td>
            <td class="px-5 py-3.5">
              <div class="flex items-center gap-2">
                <div class="flex-1 max-w-[100px]">
                  <div class="flex justify-between text-[10px] mb-1">
                    <span class="font-bold text-espresso">{{ customer.points.toLocaleString() }}</span>
                    <span class="text-muted-foreground">/ {{ nextTierPoints(customer.tier).toLocaleString() }}</span>
                  </div>
                  <div class="h-1.5 rounded-full bg-cream-deep overflow-hidden">
                    <div
                      class="h-full rounded-full transition-all duration-500"
                      :style="{
                        width: `${Math.min(100, (customer.points / nextTierPoints(customer.tier)) * 100)}%`,
                        backgroundColor: tierColor(customer.tier)
                      }"
                    />
                  </div>
                </div>
              </div>
            </td>
            <td class="px-5 py-3.5">
              <span class="font-semibold text-espresso">{{ customer.totalSpend.toLocaleString() }}đ</span>
            </td>
            <td class="px-5 py-3.5">
              <span class="text-muted-foreground text-xs">{{ customer.lastVisit }}</span>
            </td>
            <td class="px-5 py-3.5">
              <div class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
                <button
                  @click="openDetail(customer)"
                  class="p-1.5 rounded-lg hover:bg-caramel/10 text-muted-foreground hover:text-[#CC8033] transition-colors"
                  title="Xem chi tiết"
                >
                  <Eye class="w-4 h-4" />
                </button>
                <button
                  @click="addPoints(customer)"
                  class="p-1.5 rounded-lg hover:bg-green-50 text-muted-foreground hover:text-green-600 transition-colors"
                  title="Cộng điểm"
                >
                  <Plus class="w-4 h-4" />
                </button>
              </div>
            </td>
          </tr>
          <tr v-if="filteredCustomers.length === 0">
            <td colspan="6" class="px-5 py-12 text-center text-muted-foreground text-sm">
              <Users class="w-10 h-10 mx-auto mb-2 opacity-30" />
              Không tìm thấy khách hàng nào.
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- DETAIL MODAL -->
    <div
      v-if="selectedCustomer"
      class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm"
      @click.self="selectedCustomer = null"
    >
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md mx-4 overflow-hidden">
        <!-- Modal header -->
        <div class="bg-gradient-to-r from-[#CC8033] to-[#E09048] p-6 text-white relative">
          <button
            @click="selectedCustomer = null"
            class="absolute top-4 right-4 p-1.5 rounded-lg bg-white/20 hover:bg-white/30 transition-colors"
          >
            <X class="w-4 h-4" />
          </button>
          <div class="flex items-center gap-4">
            <div
              class="w-14 h-14 rounded-full flex items-center justify-center text-white font-bold text-xl border-2 border-white/40"
              :style="{ backgroundColor: avatarColor(selectedCustomer.name) }"
            >
              {{ selectedCustomer.name.charAt(0) }}
            </div>
            <div>
              <h3 class="text-lg font-bold">{{ selectedCustomer.name }}</h3>
              <p class="text-sm text-white/80">{{ selectedCustomer.phone }}</p>
              <span
                class="inline-flex items-center gap-1 mt-1 px-2 py-0.5 rounded-full text-[10px] font-bold bg-white/25"
              >
                <component :is="tierIcon(selectedCustomer.tier)" class="w-3 h-3" />
                {{ selectedCustomer.tier }}
              </span>
            </div>
          </div>
        </div>

        <!-- Modal body -->
        <div class="p-6 space-y-4">
          <div class="grid grid-cols-2 gap-3">
            <div class="bg-background rounded-xl p-4 text-center">
              <div class="text-2xl font-bold text-espresso">{{ selectedCustomer.points.toLocaleString() }}</div>
              <div class="text-xs text-muted-foreground mt-1">Điểm tích lũy</div>
            </div>
            <div class="bg-background rounded-xl p-4 text-center">
              <div class="text-2xl font-bold text-[#CC8033]">{{ selectedCustomer.visits }}</div>
              <div class="text-xs text-muted-foreground mt-1">Lần ghé thăm</div>
            </div>
          </div>

          <div class="bg-background rounded-xl p-4">
            <div class="flex items-center justify-between mb-3">
              <span class="text-sm font-semibold text-espresso">Tiến độ lên hạng tiếp theo</span>
              <span class="text-xs text-muted-foreground">{{ selectedCustomer.points }} / {{ nextTierPoints(selectedCustomer.tier) }}</span>
            </div>
            <div class="h-2 rounded-full bg-cream-deep overflow-hidden">
              <div
                class="h-full rounded-full transition-all duration-700"
                :style="{
                  width: `${Math.min(100, (selectedCustomer.points / nextTierPoints(selectedCustomer.tier)) * 100)}%`,
                  backgroundColor: tierColor(selectedCustomer.tier)
                }"
              />
            </div>
          </div>

          <div class="space-y-2">
            <div class="text-sm font-semibold text-espresso mb-2">Lịch sử giao dịch gần đây</div>
            <div
              v-for="tx in selectedCustomer.history"
              :key="tx.date"
              class="flex items-center justify-between py-2 border-b border-cream-deep last:border-0"
            >
              <div>
                <div class="text-xs font-medium text-espresso">{{ tx.note }}</div>
                <div class="text-[10px] text-muted-foreground">{{ tx.date }}</div>
              </div>
              <span
                class="text-xs font-bold"
                :class="tx.points > 0 ? 'text-green-600' : 'text-destructive'"
              >
                {{ tx.points > 0 ? '+' : '' }}{{ tx.points }} điểm
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ADD CUSTOMER MODAL -->
    <div
      v-if="showAddModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm"
      @click.self="showAddModal = false"
    >
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-sm mx-4 p-6">
        <div class="flex items-center justify-between mb-5">
          <h3 class="text-base font-bold text-espresso">Thêm khách hàng mới</h3>
          <button @click="showAddModal = false" class="p-1.5 rounded-lg hover:bg-background transition-colors">
            <X class="w-4 h-4 text-muted-foreground" />
          </button>
        </div>

        <div class="space-y-3">
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Họ và tên</label>
            <input
              v-model="newCustomer.name"
              placeholder="Nguyễn Văn A"
              class="w-full h-10 px-3 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Số điện thoại</label>
            <input
              v-model="newCustomer.phone"
              placeholder="0912345678"
              class="w-full h-10 px-3 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Ghi chú</label>
            <input
              v-model="newCustomer.note"
              placeholder="VD: Khách quen, thích đồ ngọt nhẹ..."
              class="w-full h-10 px-3 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
        </div>

        <div class="flex gap-2 mt-5">
          <button
            @click="showAddModal = false"
            class="flex-1 h-10 rounded-lg border border-cream-deep text-sm font-semibold text-espresso hover:bg-background transition-colors"
          >
            Hủy
          </button>
          <button
            @click="handleAddCustomer"
            class="flex-1 h-10 rounded-lg bg-[#CC8033] text-white text-sm font-bold hover:bg-brown transition-colors shadow-md shadow-[#CC8033]/20"
          >
            Thêm mới
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Search, Users, UserPlus, Eye, Plus, X,
  Star, Award, Trophy, Gem, Crown
} from 'lucide-vue-next'

// ─── Types ───────────────────────────────────────────────────────────────────
interface Transaction {
  date: string
  note: string
  points: number
}

interface Customer {
  id: number
  name: string
  phone: string
  tier: string
  points: number
  totalSpend: number
  lastVisit: string
  visits: number
  history: Transaction[]
}

// ─── State ───────────────────────────────────────────────────────────────────
const searchQuery = ref('')
const selectedTier = ref('Tất cả')
const selectedCustomer = ref<Customer | null>(null)
const showAddModal = ref(false)
const newCustomer = ref({ name: '', phone: '', note: '' })

// ─── Mock data ────────────────────────────────────────────────────────────────
const customers = ref<Customer[]>([
  {
    id: 1, name: 'Nguyễn Minh Châu', phone: '0901234567', tier: 'Kim cương',
    points: 4850, totalSpend: 4850000, lastVisit: 'Hôm nay, 09:30', visits: 42,
    history: [
      { date: '04/06/2026', note: 'Mua Cappuccino × 2', points: 80 },
      { date: '01/06/2026', note: 'Mua Latte × 3', points: 105 },
      { date: '28/05/2026', note: 'Đổi điểm giảm giá', points: -200 },
    ]
  },
  {
    id: 2, name: 'Trần Hoàng Linh', phone: '0912345678', tier: 'Vàng',
    points: 2100, totalSpend: 2100000, lastVisit: 'Hôm qua, 14:15', visits: 28,
    history: [
      { date: '03/06/2026', note: 'Mua Trà sữa matcha', points: 55 },
      { date: '30/05/2026', note: 'Mua Bánh croissant', points: 30 },
    ]
  },
  {
    id: 3, name: 'Phạm Thị Hương', phone: '0923456789', tier: 'Bạc',
    points: 980, totalSpend: 980000, lastVisit: '02/06/2026', visits: 14,
    history: [
      { date: '02/06/2026', note: 'Mua Cold brew', points: 45 },
    ]
  },
  {
    id: 4, name: 'Lê Văn Tuấn', phone: '0934567890', tier: 'Đồng',
    points: 320, totalSpend: 320000, lastVisit: '01/06/2026', visits: 5,
    history: [
      { date: '01/06/2026', note: 'Mua Americano', points: 35 },
    ]
  },
  {
    id: 5, name: 'Võ Thị Mai', phone: '0945678901', tier: 'Vàng',
    points: 1750, totalSpend: 1750000, lastVisit: '03/06/2026', visits: 21,
    history: [
      { date: '03/06/2026', note: 'Mua Combo 2 người', points: 120 },
      { date: '29/05/2026', note: 'Đổi điểm giảm giá', points: -100 },
    ]
  },
  {
    id: 6, name: 'Đỗ Quang Huy', phone: '0956789012', tier: 'Bạc',
    points: 860, totalSpend: 860000, lastVisit: '31/05/2026', visits: 11,
    history: [
      { date: '31/05/2026', note: 'Mua Espresso × 2', points: 60 },
    ]
  },
])

// ─── Computed ─────────────────────────────────────────────────────────────────
const filteredCustomers = computed(() => {
  return customers.value.filter(c => {
    const matchSearch =
      c.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      c.phone.includes(searchQuery.value)
    const matchTier = selectedTier.value === 'Tất cả' || c.tier === selectedTier.value
    return matchSearch && matchTier
  })
})

// ─── Stats ────────────────────────────────────────────────────────────────────
const stats = computed(() => [
  {
    label: 'Tổng khách hàng',
    value: customers.value.length,
    icon: Users,
    color: '#CC8033',
    bg: '#CC803320',
  },
  {
    label: 'Kim cương',
    value: customers.value.filter(c => c.tier === 'Kim cương').length,
    icon: Gem,
    color: '#60a5fa',
    bg: '#60a5fa20',
  },
  {
    label: 'Vàng',
    value: customers.value.filter(c => c.tier === 'Vàng').length,
    icon: Crown,
    color: '#fbbf24',
    bg: '#fbbf2420',
  },
  {
    label: 'Bạc & Đồng',
    value: customers.value.filter(c => ['Bạc', 'Đồng'].includes(c.tier)).length,
    icon: Award,
    color: '#94a3b8',
    bg: '#94a3b820',
  },
])

// ─── Helpers ──────────────────────────────────────────────────────────────────
const avatarColors = ['#CC8033', '#60a5fa', '#34d399', '#a78bfa', '#f87171', '#fb923c']
const avatarColor = (name: string) => avatarColors[name.charCodeAt(0) % avatarColors.length]

const tierClass = (tier: string) => ({
  'Kim cương': 'bg-blue-50 text-blue-600',
  'Vàng': 'bg-yellow-50 text-yellow-600',
  'Bạc': 'bg-slate-100 text-slate-500',
  'Đồng': 'bg-orange-50 text-orange-600',
}[tier] ?? 'bg-gray-100 text-gray-500')

const tierColor = (tier: string) => ({
  'Kim cương': '#60a5fa',
  'Vàng': '#fbbf24',
  'Bạc': '#94a3b8',
  'Đồng': '#fb923c',
}[tier] ?? '#CC8033')

const tierIcon = (tier: string) => ({
  'Kim cương': Gem,
  'Vàng': Crown,
  'Bạc': Star,
  'Đồng': Award,
}[tier] ?? Award)

const nextTierPoints = (tier: string) => ({
  'Đồng': 500,
  'Bạc': 1500,
  'Vàng': 3000,
  'Kim cương': 9999,
}[tier] ?? 9999)

// ─── Actions ──────────────────────────────────────────────────────────────────
const openDetail = (customer: Customer) => {
  selectedCustomer.value = customer
}

const addPoints = (customer: Customer) => {
  const pts = parseInt(prompt(`Nhập số điểm muốn cộng cho ${customer.name}:`) ?? '0')
  if (!isNaN(pts) && pts > 0) {
    customer.points += pts
    customer.history.unshift({
      date: new Date().toLocaleDateString('vi-VN'),
      note: 'Cộng điểm thủ công',
      points: pts,
    })
  }
}

const handleAddCustomer = () => {
  if (!newCustomer.value.name.trim() || !newCustomer.value.phone.trim()) return
  customers.value.push({
    id: Date.now(),
    name: newCustomer.value.name.trim(),
    phone: newCustomer.value.phone.trim(),
    tier: 'Đồng',
    points: 0,
    totalSpend: 0,
    lastVisit: 'Hôm nay',
    visits: 0,
    history: [],
  })
  showAddModal.value = false
  newCustomer.value = { name: '', phone: '', note: '' }
}
</script>
