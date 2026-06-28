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

    <!-- TIER BENEFITS -->
    <div class="bg-white rounded-xl border border-cream-deep shadow-sm overflow-hidden">
      <button
        @click="showBenefits = !showBenefits"
        class="w-full flex items-center justify-between px-5 py-3.5 hover:bg-background/50 transition-colors"
      >
        <span class="flex items-center gap-2 text-sm font-bold text-espresso">
          <Gift class="w-4 h-4 text-[#CC8033]" /> Quyền lợi theo hạng thành viên
        </span>
        <ChevronDown class="w-4 h-4 text-muted-foreground transition-transform" :class="{ 'rotate-180': showBenefits }" />
      </button>
      <div v-if="showBenefits" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 p-4 pt-0">
        <div
          v-for="tier in tiers"
          :key="tier.name"
          class="rounded-xl border p-4"
          :style="{ borderColor: tier.color + '40', backgroundColor: tier.color + '0A' }"
        >
          <div class="flex items-center gap-2 mb-3">
            <div class="w-8 h-8 rounded-lg flex items-center justify-center" :style="{ backgroundColor: tier.color + '22' }">
              <component :is="tier.icon" class="w-4 h-4" :style="{ color: tier.color }" />
            </div>
            <div>
              <div class="text-sm font-bold text-espresso">{{ tier.name }}</div>
              <div class="text-[10px] text-muted-foreground">Từ {{ tier.min.toLocaleString() }} điểm</div>
            </div>
          </div>
          <ul class="space-y-1.5">
            <li v-for="b in tier.benefits" :key="b" class="flex items-start gap-1.5 text-xs text-espresso/80">
              <Check class="w-3 h-3 mt-0.5 flex-shrink-0" :style="{ color: tier.color }" />
              {{ b }}
            </li>
          </ul>
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
        @click="openAdd"
        class="ml-auto flex items-center gap-2 px-4 h-9 rounded-lg bg-[#CC8033] text-white text-xs font-bold hover:bg-brown transition-colors shadow-md shadow-[#CC8033]/20"
      >
        <UserPlus class="w-4 h-4" />
        Thêm khách hàng
      </button>
    </div>

    <!-- CUSTOMER CARD GRID -->
    <div v-if="filteredCustomers.length > 0" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
      <article
        v-for="customer in filteredCustomers"
        :key="customer.id"
        class="bg-white rounded-2xl border border-cream-deep shadow-sm overflow-hidden flex flex-col group hover:shadow-md transition-shadow"
      >
        <!-- Tier strip -->
        <div class="h-1.5" :style="{ backgroundColor: tierColor(customer.tier) }" />

        <div class="p-5 flex-1 flex flex-col">
          <!-- Identity -->
          <div class="flex items-start justify-between">
            <div class="flex items-center gap-3">
              <div
                class="w-12 h-12 rounded-full flex items-center justify-center text-white font-bold text-base flex-shrink-0"
                :style="{ backgroundColor: avatarColor(customer.name) }"
              >
                {{ customer.name.charAt(0) }}
              </div>
              <div>
                <div class="font-bold text-espresso leading-tight">{{ customer.name }}</div>
                <div class="text-xs text-muted-foreground">{{ customer.phone }}</div>
              </div>
            </div>
            <span
              class="inline-flex items-center gap-1 px-2 py-1 rounded-full text-[10px] font-bold flex-shrink-0"
              :class="tierClass(customer.tier)"
            >
              <component :is="tierIcon(customer.tier)" class="w-3 h-3" />
              {{ customer.tier }}
            </span>
          </div>

          <!-- Points progress -->
          <div class="mt-4">
            <div class="flex justify-between text-[11px] mb-1.5">
              <span class="font-bold text-espresso">{{ customer.points.toLocaleString() }} điểm</span>
              <span class="text-muted-foreground">
                {{ customer.tier === 'Kim cương' ? 'Hạng cao nhất' : `/ ${nextTierPoints(customer.tier).toLocaleString()}` }}
              </span>
            </div>
            <div class="h-2 rounded-full bg-cream-deep overflow-hidden">
              <div
                class="h-full rounded-full transition-all duration-500"
                :style="{
                  width: `${Math.min(100, (customer.points / nextTierPoints(customer.tier)) * 100)}%`,
                  backgroundColor: tierColor(customer.tier)
                }"
              />
            </div>
          </div>

          <!-- Mini stats -->
          <div class="grid grid-cols-2 gap-2 mt-4">
            <div class="bg-background rounded-lg p-2.5 text-center">
              <div class="text-sm font-bold text-espresso">{{ formatVnd(customer.totalSpend) }}</div>
              <div class="text-[10px] text-muted-foreground mt-0.5">Tổng chi tiêu</div>
            </div>
            <div class="bg-background rounded-lg p-2.5 text-center">
              <div class="text-sm font-bold text-[#CC8033]">{{ customer.visits }}</div>
              <div class="text-[10px] text-muted-foreground mt-0.5">Lần ghé thăm</div>
            </div>
          </div>

          <div class="text-[10px] text-muted-foreground mt-3 flex items-center gap-1">
            <Clock class="w-3 h-3" /> Ghé gần nhất: {{ customer.lastVisit }}
          </div>

          <!-- Actions -->
          <div class="grid grid-cols-3 gap-1.5 mt-4 pt-4 border-t border-cream-deep">
            <button
              @click="openDetail(customer)"
              class="flex items-center justify-center h-8 rounded-lg bg-background hover:bg-caramel/10 text-muted-foreground hover:text-[#CC8033] transition-colors"
              title="Xem chi tiết"
            >
              <Eye class="w-4 h-4" />
            </button>
            <button
              @click="openEdit(customer)"
              class="flex items-center justify-center h-8 rounded-lg bg-background hover:bg-blue-50 text-muted-foreground hover:text-blue-600 transition-colors"
              title="Chỉnh sửa"
            >
              <Pencil class="w-4 h-4" />
            </button>
            <button
              @click="deleteCustomer(customer)"
              class="flex items-center justify-center h-8 rounded-lg bg-background hover:bg-red-50 text-muted-foreground hover:text-destructive transition-colors"
              title="Xóa khách hàng"
            >
              <Trash2 class="w-4 h-4" />
            </button>
          </div>
        </div>
      </article>
    </div>

    <!-- Empty state -->
    <div v-else class="bg-white rounded-xl border border-cream-deep shadow-sm py-16 text-center text-muted-foreground text-sm">
      <Users class="w-10 h-10 mx-auto mb-2 opacity-30" />
      Không tìm thấy khách hàng nào.
    </div>

    <!-- DETAIL MODAL -->
    <div
      v-if="selectedCustomer"
      class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm p-4"
      @click.self="selectedCustomer = null"
    >
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md max-h-[90vh] overflow-y-auto custom-scrollbar">
        <!-- Modal header -->
        <div class="bg-gradient-to-r from-[#CC8033] to-[#E09048] p-6 text-white relative sticky top-0">
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
              <span class="inline-flex items-center gap-1 mt-1 px-2 py-0.5 rounded-full text-[10px] font-bold bg-white/25">
                <component :is="tierIcon(selectedCustomer.tier)" class="w-3 h-3" />
                {{ selectedCustomer.tier }}
              </span>
            </div>
          </div>
        </div>

        <!-- Modal body -->
        <div class="p-6 space-y-5">
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

          <!-- Redeem rewards -->
          <div>
            <div class="text-sm font-semibold text-espresso mb-2 flex items-center gap-1.5">
              <Ticket class="w-4 h-4 text-[#CC8033]" /> Đổi điểm lấy ưu đãi
            </div>
            <div class="space-y-2">
              <div
                v-for="reward in rewards"
                :key="reward.id"
                class="flex items-center justify-between gap-3 p-3 rounded-xl border border-cream-deep"
              >
                <div class="flex items-center gap-2.5 min-w-0">
                  <div class="w-8 h-8 rounded-lg bg-caramel/10 flex items-center justify-center flex-shrink-0">
                    <component :is="reward.icon" class="w-4 h-4 text-[#CC8033]" />
                  </div>
                  <div class="min-w-0">
                    <div class="text-xs font-semibold text-espresso truncate">{{ reward.name }}</div>
                    <div class="text-[10px] text-muted-foreground">{{ reward.cost.toLocaleString() }} điểm</div>
                  </div>
                </div>
                <button
                  @click="redeemReward(selectedCustomer, reward)"
                  :disabled="selectedCustomer.points < reward.cost"
                  class="px-3 h-8 rounded-lg text-xs font-bold transition-colors flex-shrink-0"
                  :class="selectedCustomer.points >= reward.cost
                    ? 'bg-[#CC8033] text-white hover:bg-brown'
                    : 'bg-cream-deep text-muted-foreground cursor-not-allowed'"
                >
                  Đổi
                </button>
              </div>
            </div>
          </div>

          <!-- History -->
          <div>
            <div class="text-sm font-semibold text-espresso mb-2">Lịch sử giao dịch gần đây</div>
            <div v-if="selectedCustomer.history.length === 0" class="text-xs text-muted-foreground py-3 text-center">
              Chưa có giao dịch nào.
            </div>
            <div
              v-for="(tx, i) in selectedCustomer.history"
              :key="i"
              class="flex items-center justify-between py-2 border-b border-cream-deep last:border-0"
            >
              <div>
                <div class="text-xs font-medium text-espresso">{{ tx.note }}</div>
                <div class="text-[10px] text-muted-foreground">{{ tx.date }}</div>
              </div>
              <span class="text-xs font-bold" :class="tx.points > 0 ? 'text-green-600' : 'text-destructive'">
                {{ tx.points > 0 ? '+' : '' }}{{ tx.points }} điểm
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ADD / EDIT CUSTOMER MODAL -->
    <div
      v-if="showFormModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm p-4"
      @click.self="showFormModal = false"
    >
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-sm p-6">
        <div class="flex items-center justify-between mb-5">
          <h3 class="text-base font-bold text-espresso">
            {{ formMode === 'edit' ? 'Chỉnh sửa khách hàng' : 'Thêm khách hàng mới' }}
          </h3>
          <button @click="showFormModal = false" class="p-1.5 rounded-lg hover:bg-background transition-colors">
            <X class="w-4 h-4 text-muted-foreground" />
          </button>
        </div>

        <div class="space-y-3">
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Họ và tên</label>
            <input
              v-model="form.name"
              placeholder="Nguyễn Văn A"
              class="w-full h-10 px-3 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Số điện thoại</label>
            <input
              v-model="form.phone"
              placeholder="0912345678"
              class="w-full h-10 px-3 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Email (Tùy chọn)</label>
            <input
              v-model="form.email"
              placeholder="example@gmail.com"
              class="w-full h-10 px-3 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Ghi chú</label>
            <input
              v-model="form.note"
              placeholder="VD: Khách quen, thích đồ ngọt nhẹ..."
              class="w-full h-10 px-3 bg-background border border-cream-deep rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
        </div>

        <div class="flex gap-2 mt-5">
          <button
            @click="showFormModal = false"
            class="flex-1 h-10 rounded-lg border border-cream-deep text-sm font-semibold text-espresso hover:bg-background transition-colors"
          >
            Hủy
          </button>
          <button
            @click="handleSaveCustomer"
            class="flex-1 h-10 rounded-lg bg-[#CC8033] text-white text-sm font-bold hover:bg-brown transition-colors shadow-md shadow-[#CC8033]/20"
          >
            {{ formMode === 'edit' ? 'Lưu thay đổi' : 'Thêm mới' }}
          </button>
        </div>
      </div>
    </div>

    <!-- OTP VERIFICATION MODAL -->
    <div
      v-if="showOtpModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm p-4"
      @click.self="showOtpModal = false"
    >
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-base font-bold text-espresso flex items-center gap-2">
            <Lock class="w-5 h-5 text-[#CC8033]" /> Xác thực mã OTP
          </h3>
          <button @click="showOtpModal = false" class="p-1.5 rounded-lg hover:bg-background transition-colors">
            <X class="w-4 h-4 text-muted-foreground" />
          </button>
        </div>

        <p class="text-xs text-muted-foreground mb-4">
          <template v-if="simulatedOtpCode">
            Hệ thống đã gửi một mã OTP giả lập tới số điện thoại của <strong>{{ currentRedeemCustomer?.name }}</strong>.
          </template>
          <template v-else>
            Hệ thống đã gửi một mã OTP xác thực tới số điện thoại <strong>{{ currentRedeemCustomer?.phone }}</strong> của <strong>{{ currentRedeemCustomer?.name }}</strong>. Vui lòng nhập mã OTP từ điện thoại của bạn.
          </template>
        </p>

        <!-- Simulated OTP Widget -->
        <div v-if="simulatedOtpCode" class="bg-orange-50/70 border border-orange-200/50 rounded-xl p-4 mb-4 text-center">
          <div class="text-[10px] uppercase font-bold tracking-wider text-orange-600/80 mb-1">Mã OTP Giả lập</div>
          <div class="text-3xl font-extrabold tracking-[0.25em] text-[#CC8033]">{{ simulatedOtpCode }}</div>
        </div>

        <div class="space-y-3">
          <div>
            <label class="text-xs font-semibold text-espresso mb-1.5 block">Nhập mã OTP</label>
            <input
              v-model="otpInput"
              type="text"
              maxlength="6"
              placeholder="Nhập 6 chữ số"
              class="w-full h-12 text-center text-xl font-bold tracking-[0.5em] bg-background border border-cream-deep rounded-xl focus:outline-none focus:ring-2 focus:ring-caramel/20 focus:border-caramel transition-all"
            />
          </div>
        </div>

        <div class="flex gap-2 mt-5">
          <button
            @click="showOtpModal = false"
            class="flex-1 h-10 rounded-lg border border-cream-deep text-sm font-semibold text-espresso hover:bg-background transition-colors"
          >
            Hủy
          </button>
          <button
            @click="handleRedeemWithOtp"
            :disabled="otpInput.length < 6"
            class="flex-1 h-10 rounded-lg bg-[#CC8033] text-white text-sm font-bold hover:bg-brown transition-colors shadow-md shadow-[#CC8033]/20 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Xác nhận
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  Search, Users, UserPlus, Eye, X, Clock,
  Star, Award, Gem, Crown, Pencil, Trash2,
  Gift, Ticket, Percent, Coffee, Cookie, ChevronDown, Check, Lock
} from 'lucide-vue-next'
import { loyaltyApi, type Customer, type Reward } from '@/services/loyalty'
import { useToast } from '@/stores/toast'
import { useAlert } from '@/stores/alert'

// ─── Composable Notifications ────────────────────────────────────────────────
const toast = useToast()
const alertStore = useAlert()

// ─── State ───────────────────────────────────────────────────────────────────
const searchQuery = ref('')
const selectedTier = ref('Tất cả')
const selectedCustomer = ref<any | null>(null)
const showBenefits = ref(true)
const showFormModal = ref(false)
const formMode = ref<'add' | 'edit'>('add')
const editingId = ref<number | null>(null)
const form = ref({ name: '', phone: '', email: '', note: '' })
const customers = ref<Customer[]>([])
const backendRewards = ref<Reward[]>([])
const loading = ref(false)

// OTP Modal State
const showOtpModal = ref(false)
const otpInput = ref('')
const currentRedeemCustomer = ref<Customer | null>(null)
const currentRedeemReward = ref<any>(null)
const simulatedOtpCode = ref<string | null>(null)

// ─── Tier benefits ─────────────────────────────────────────────────────────────
const tiers = [
  {
    name: 'Đồng', icon: Award, color: '#fb923c', min: 0,
    benefits: ['Tích 1 điểm / 1.000đ', 'Ưu đãi ngày sinh nhật'],
  },
  {
    name: 'Bạc', icon: Star, color: '#94a3b8', min: 500,
    benefits: ['Tích 1.2 điểm / 1.000đ', 'Giảm 5% đồ uống', 'Ưu đãi sinh nhật'],
  },
  {
    name: 'Vàng', icon: Crown, color: '#fbbf24', min: 1500,
    benefits: ['Tích 1.5 điểm / 1.000đ', 'Giảm 10% hóa đơn', 'Tặng 1 topping', 'Ưu tiên đặt bàn'],
  },
  {
    name: 'Kim cương', icon: Gem, color: '#60a5fa', min: 3000,
    benefits: ['Tích 2 điểm / 1.000đ', 'Giảm 15% hóa đơn', 'Đồ uống tặng hàng tháng', 'Quà tặng đặc biệt'],
  },
]

// ─── Rewards catalog ───────────────────────────────────────────────────────────
const getRewardIcon = (name: string) => {
  const nameLower = name.toLowerCase()
  if (nameLower.includes('topping') || nameLower.includes('cookie') || nameLower.includes('bánh')) return Cookie
  if (nameLower.includes('%') || nameLower.includes('giảm')) return Percent
  if (nameLower.includes('cà phê') || nameLower.includes('nước') || nameLower.includes('coffee') || nameLower.includes('ly')) return Coffee
  return Ticket
}

const rewards = computed(() => {
  return backendRewards.value.map(r => ({
    id: r.id,
    name: r.name,
    cost: r.cost,
    icon: getRewardIcon(r.name)
  }))
})

// ─── API Integration ──────────────────────────────────────────────────────────
const loadCustomers = async () => {
  loading.value = true
  try {
    customers.value = await loyaltyApi.list()
  } catch (err: any) {
    toast.error(err.message || 'Không thể tải danh sách khách hàng.')
  } finally {
    loading.value = false
  }
}

const loadRewards = async () => {
  try {
    backendRewards.value = await loyaltyApi.getRewards()
  } catch (err: any) {
    console.error('Không thể tải danh sách phần thưởng', err)
  }
}

onMounted(() => {
  loadCustomers()
  loadRewards()
})

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
  { label: 'Tổng khách hàng', value: customers.value.length, icon: Users, color: '#CC8033', bg: '#CC803320' },
  { label: 'Kim cương', value: customers.value.filter(c => c.tier === 'Kim cương').length, icon: Gem, color: '#60a5fa', bg: '#60a5fa20' },
  { label: 'Vàng', value: customers.value.filter(c => c.tier === 'Vàng').length, icon: Crown, color: '#fbbf24', bg: '#fbbf2420' },
  { label: 'Bạc & Đồng', value: customers.value.filter(c => ['Bạc', 'Đồng'].includes(c.tier)).length, icon: Award, color: '#94a3b8', bg: '#94a3b820' },
])

// ─── Helpers ──────────────────────────────────────────────────────────────────
const avatarColors = ['#CC8033', '#60a5fa', '#34d399', '#a78bfa', '#f87171', '#fb923c']
const avatarColor = (name: string) => avatarColors[name.charCodeAt(0) % avatarColors.length]

const formatVnd = (n: number) =>
  n >= 1000000 ? (n / 1000000).toFixed(n % 1000000 === 0 ? 0 : 1) + 'tr' : n.toLocaleString() + 'đ'

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
const openDetail = async (customer: Customer) => {
  try {
    const detail = await loyaltyApi.get(customer.id)
    selectedCustomer.value = detail
  } catch (err: any) {
    toast.error(err.message || 'Không thể tải chi tiết khách hàng.')
  }
}

const redeemReward = async (customer: Customer, reward: any) => {
  if (customer.points < reward.cost) return
  const confirmed = await alertStore.confirm(
    'Xác nhận đổi điểm',
    `Bạn có chắc muốn đổi ${reward.cost} điểm lấy "${reward.name}" cho ${customer.name}?`
  )
  if (!confirmed) return
  try {
    const otpRes = await loyaltyApi.sendOtp(customer.id)
    currentRedeemCustomer.value = customer
    currentRedeemReward.value = reward
    simulatedOtpCode.value = otpRes.otp
    otpInput.value = ''
    showOtpModal.value = true
  } catch (err: any) {
    toast.error(err.message || 'Không thể gửi mã OTP xác nhận.')
  }
}

const handleRedeemWithOtp = async () => {
  if (!otpInput.value || !currentRedeemCustomer.value || !currentRedeemReward.value) return
  try {
    await loyaltyApi.redeem(currentRedeemCustomer.value.id, currentRedeemReward.value.id, otpInput.value.trim())
    toast.success('Đổi phần thưởng thành công!')
    showOtpModal.value = false
    await loadCustomers()
    if (selectedCustomer.value && selectedCustomer.value.id === currentRedeemCustomer.value.id) {
      await openDetail(currentRedeemCustomer.value)
    }
  } catch (err: any) {
    toast.error(err.message || 'Mã OTP không chính xác hoặc đã hết hạn.')
  }
}

const openAdd = () => {
  formMode.value = 'add'
  editingId.value = null
  form.value = { name: '', phone: '', email: '', note: '' }
  showFormModal.value = true
}

const openEdit = (customer: Customer) => {
  formMode.value = 'edit'
  editingId.value = customer.id
  form.value = { 
    name: customer.name, 
    phone: customer.phone, 
    email: customer.email || '',
    note: customer.note ?? '' 
  }
  showFormModal.value = true
}

const handleSaveCustomer = async () => {
  const name = form.value.name.trim()
  const phone = form.value.phone.trim()
  const email = form.value.email.trim()
  const note = form.value.note.trim()
  
  if (!name || !phone) {
    toast.warning('Vui lòng điền họ tên và số điện thoại.')
    return
  }

  try {
    if (formMode.value === 'edit' && editingId.value !== null) {
      await loyaltyApi.update(editingId.value, { name, phone, email: email || undefined, note })
      toast.success('Cập nhật khách hàng thành công!')
    } else {
      await loyaltyApi.create({ name, phone, email: email || undefined, note })
      toast.success('Thêm khách hàng thành công!')
    }
    showFormModal.value = false
    form.value = { name: '', phone: '', email: '', note: '' }
    await loadCustomers()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi lưu thông tin khách hàng.')
  }
}

const deleteCustomer = async (customer: Customer) => {
  const confirmed = await alertStore.confirm(
    'Xác nhận xóa',
    `Xóa khách hàng "${customer.name}"? Hành động này không thể hoàn tác.`
  )
  if (!confirmed) return
  try {
    await loyaltyApi.remove(customer.id)
    toast.success('Xóa khách hàng thành công!')
    if (selectedCustomer.value?.id === customer.id) selectedCustomer.value = null
    await loadCustomers()
  } catch (err: any) {
    toast.error(err.message || 'Có lỗi xảy ra khi xóa khách hàng.')
  }
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #E0D5C7; border-radius: 3px; }
</style>
