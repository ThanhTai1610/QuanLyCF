<template>
  <div class="grid grid-cols-1 lg:grid-cols-3 gap-5 h-[calc(100vh-7rem)] p-6">
    <!-- List -->
    <div class="lg:col-span-2 bg-card rounded-lg border border-cream-deep shadow-card flex flex-col overflow-hidden">
      <div class="p-5 border-b-2 border-cream-deep space-y-4">
        <div class="flex items-center gap-3">
          <div class="relative flex-1">
            <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Tìm theo mã đơn hoặc số bàn..."
              v-model="search"
              class="pl-9 bg-background border border-cream-deep h-10 rounded-lg shadow-card"
            />
          </div>
          <Button @click="resetFilters" variant="outline" size="icon" class="border border-cream-deep h-10 w-10 rounded-lg shadow-card" title="Đặt lại tất cả bộ lọc">
            <Filter class="w-4 h-4 text-espresso" />
          </Button>
        </div>

        <div class="flex gap-2 overflow-x-auto scrollbar-none">
          <button
            v-for="f in filters"
            :key="f.id"
            @click="filter = f.id"
            :class="[
              'px-5 py-2.5 rounded-lg text-sm font-semibold whitespace-nowrap border shadow-card',
              filter === f.id
                ? 'bg-espresso text-cream border-espresso'
                : 'bg-background text-espresso border-cream-deep'
            ]"
          >
            {{ f.label }} <span class="opacity-70 ml-1">({{ counts[f.id] }})</span>
          </button>
        </div>
      </div>

      <div class="flex-1 overflow-y-auto">
        <div v-if="paginatedItems.length === 0" class="text-center py-16 text-muted-foreground">
          Không có đơn hàng nào.
        </div>
        <ul v-else class="divide-y divide-cream-deep/60">
          <li
            v-for="o in paginatedItems"
            :key="o.id"
            @click="selected = o"
            :class="[
              'px-5 py-4 cursor-pointer border-l-4 transition-all relative',
              selected?.id === o.id 
                ? 'bg-caramel-light/50 border-caramel' 
                : 'border-transparent',
              isLate(o)
                ? 'bg-red-50/40 border-red-500/80 ring-2 ring-red-500/10 shadow-[0_0_12px_rgba(239,68,68,0.1)]'
                : ''
            ]"
          >
            <div class="flex justify-between items-start gap-3">
              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2 flex-wrap">
                  <span class="font-semibold text-espresso">{{ o.id }}</span>
                  <span :class="['px-3 py-1 rounded-lg text-xs font-medium', statusMeta[o.status].className]">
                    {{ statusMeta[o.status].label }}
                  </span>
                  <!-- Cancel Reason Button -->
                  <button v-if="o.status === 'cancelled'"
                    @click.stop="toast.info(`Lý do: ${o.cancelReason || 'Hủy nhanh (không lưu lý do)'}`, `Đơn ${o.id}`)"
                    class="px-2 py-0.5 rounded bg-red-100 hover:bg-red-200 text-red-800 text-[10px] font-bold transition-all border border-red-200 shadow-sm active:scale-95 shrink-0"
                    title="Bấm để xem lý do hủy">
                    Lý do hủy
                  </button>
                  <!-- Waiting time or Late warning badge -->
                  <span v-if="isLate(o)" class="px-2 py-0.5 rounded-lg bg-red-100 text-red-700 text-[10px] font-extrabold flex items-center gap-1 border border-red-200 animate-pulse">
                    <Clock class="w-3 h-3 text-red-600" /> Trễ {{ getElapsedTime(o) }}p
                  </span>
                  <span v-else-if="o.status === 'pending' || o.status === 'preparing'" class="px-2 py-0.5 rounded-lg bg-[#FAF6F0] text-[#CC8033] text-[10px] font-bold flex items-center gap-1 border border-[#EAE3D9]">
                    <Clock class="w-3 h-3 text-[#CC8033]" /> {{ getElapsedTime(o) }}p
                  </span>
                </div>
                <div class="text-sm text-muted-foreground mt-1 truncate">
                  {{ o.table }} • {{ o.items.map(i => `${i.qty}× ${i.name}`).join(", ") }}
                </div>
              </div>
              <div class="text-right flex-shrink-0">
                <div class="font-semibold text-caramel">{{ formatVND(o.total) }}</div>
                <div class="text-xs text-muted-foreground flex items-center gap-1 justify-end mt-0.5">
                  <Clock class="w-3 h-3" /> {{ o.createdAt }}
                </div>
              </div>
            </div>
          </li>
        </ul>
      </div>

      <!-- Pagination -->
      <div v-if="visible.length > 0" class="flex items-center justify-between p-4 border-t border-cream-deep bg-background shrink-0">
        <div class="text-xs text-muted-foreground">
          <span class="font-medium text-espresso">{{ (currentPage - 1) * itemsPerPage + 1 }}</span> - <span class="font-medium text-espresso">{{ Math.min(currentPage * itemsPerPage, visible.length) }}</span> / <span class="font-medium text-espresso">{{ visible.length }}</span>
        </div>
        <div class="flex items-center gap-2">
          <Button 
            variant="outline"
            size="icon"
            @click="currentPage--" 
            :disabled="currentPage === 1"
            class="h-8 w-8 rounded-lg border-cream-deep disabled:opacity-50 text-espresso shadow-sm"
          >
            <ChevronLeft class="w-4 h-4" />
          </Button>
          <span class="text-xs font-semibold text-espresso px-2">
            {{ currentPage }} / {{ totalPages }}
          </span>
          <Button 
            variant="outline"
            size="icon"
            @click="currentPage++" 
            :disabled="currentPage === totalPages"
            class="h-8 w-8 rounded-lg border-cream-deep disabled:opacity-50 text-espresso shadow-sm"
          >
            <ChevronRight class="w-4 h-4" />
          </Button>
        </div>
      </div>
    </div>

    <!-- Detail panel -->
    <div class="bg-card rounded-lg border border-cream-deep shadow-card flex flex-col overflow-hidden">
      <template v-if="selected">
        <div class="p-5 border-b-2 border-cream-deep">
          <div class="flex items-center justify-between">
            <div>
              <h3 class="font-display text-xl text-espresso font-semibold">{{ selected.id }}</h3>
              <p class="text-sm text-muted-foreground">{{ selected.table }} • {{ selected.createdAt }}</p>
            </div>
            <span :class="['px-3 py-1 rounded-lg text-xs font-medium', statusMeta[selected.status].className]">
              {{ statusMeta[selected.status].label }}
            </span>
          </div>
        </div>

        <div class="flex-1 overflow-y-auto p-5 space-y-3">
          <h4 class="text-xs uppercase tracking-wide text-muted-foreground font-semibold">Chi tiết món</h4>
          <div v-for="(it, idx) in selected.items" :key="idx" class="flex items-center gap-3 p-4 rounded-lg bg-cream/50 border border-cream-deep">
            <div class="w-9 h-9 rounded-lg bg-caramel-light flex items-center justify-center flex-shrink-0 border border-cream-deep">
              <Coffee class="w-4 h-4 text-caramel" />
            </div>
            <div class="flex-1 min-w-0">
              <div class="font-medium text-espresso text-sm">{{ it.name }}</div>
              <div class="text-xs text-muted-foreground">SL: {{ it.qty }} × {{ formatVND(it.price) }}</div>
            </div>
            <div class="font-semibold text-caramel text-sm">{{ formatVND(it.qty * it.price) }}</div>
          </div>
        </div>

        <div class="border-t-2 border-cream-deep p-5 space-y-3">
          <div class="flex justify-between font-display text-lg text-espresso font-semibold">
            <span>Tổng cộng</span>
            <span class="text-caramel">{{ formatVND(selected.total) }}</span>
          </div>

          <!-- Cancel Reason Display -->
          <div v-if="selected.status === 'cancelled'" class="p-3.5 bg-red-50 border border-red-200/60 rounded-xl text-xs text-red-700 font-semibold space-y-1">
            <div class="flex items-center gap-1.5 text-red-800">
              <AlertTriangle class="w-4 h-4 shrink-0 animate-pulse" />
              <span>Lý do hủy đơn:</span>
            </div>
            <p class="italic text-red-600 font-medium pl-5.5">"{{ selected.cancelReason || 'Hủy nhanh (không có lý do)' }}"</p>
          </div>

          <div class="grid grid-cols-2 gap-3">
            <template v-if="selected.status !== 'done' && selected.status !== 'cancelled'">
              <Button
                @click="updateStatus(selected.id, selected.status === 'pending' ? 'preparing' : 'done')"
                class="bg-caramel hover:bg-brown text-cream font-semibold rounded-lg border border-caramel/30 shadow-card"
              >
                <CheckCircle class="w-4 h-4 mr-1.5" />
                {{ selected.status === 'pending' ? 'Bắt đầu pha' : 'Hoàn thành' }}
              </Button>
              <Button
                @click="openCancelDialog(selected.id)"
                variant="outline"
                class="border-destructive/40 text-destructive rounded-lg border shadow-card"
              >
                <X class="w-4 h-4 mr-1.5" /> Hủy đơn
              </Button>
            </template>

            <template v-else>
              <Button
                @click="updateStatus(selected.id, 'pending')"
                variant="outline"
                class="col-span-2 border border-cream-deep rounded-lg text-espresso shadow-card"
              >
                Mở lại đơn
              </Button>
            </template>
          </div>
        </div>
      </template>

      <div v-else class="flex-1 flex flex-col items-center justify-center text-muted-foreground p-8 text-center">
        <Coffee class="w-12 h-12 text-muted-foreground/30 mb-3" />
        <p class="font-semibold text-espresso">Chọn một đơn hàng để xem chi tiết</p>
        <p class="text-xs text-muted-foreground mt-1">Danh sách đơn hàng sẽ tự động cập nhật khi có đơn mới.</p>
      </div>
    </div>

    <!-- Cancel Reason Modal -->
    <div v-if="showCancelModal" class="fixed inset-0 bg-black/40 backdrop-blur-xs flex items-center justify-center z-[100] p-4">
      <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-2xl p-6 max-w-sm w-full space-y-4">
        <div>
          <h3 class="font-display text-lg text-espresso font-bold">Xác nhận lý do hủy đơn</h3>
          <p class="text-xs text-muted-foreground mt-1">Vui lòng chọn hoặc nhập lý do để đối soát doanh thu.</p>
        </div>

        <div class="space-y-2">
          <label v-for="r in presetReasons" :key="r" class="flex items-center gap-2.5 p-2.5 rounded-lg border border-[#EAE3D9] hover:bg-[#FAF6F0] cursor-pointer text-xs font-semibold text-espresso">
            <input type="radio" v-model="selectedReason" :value="r" class="text-[#CC8033] focus:ring-[#CC8033]" />
            <span>{{ r }}</span>
          </label>
          <label class="flex items-center gap-2.5 p-2.5 rounded-lg border border-[#EAE3D9] hover:bg-[#FAF6F0] cursor-pointer text-xs font-semibold text-espresso">
            <input type="radio" v-model="selectedReason" value="Khác" class="text-[#CC8033] focus:ring-[#CC8033]" />
            <span>Lý do khác...</span>
          </label>
        </div>

        <div v-if="selectedReason === 'Khác'" class="mt-2">
          <textarea v-model="customReason" placeholder="Nhập lý do chi tiết..." class="w-full text-xs font-semibold p-3 border border-[#EAE3D9] rounded-lg focus:border-[#CC8033] focus:outline-none bg-[#FAF6F0] text-espresso h-20 resize-none"></textarea>
        </div>

        <div class="flex justify-end gap-2.5 pt-2">
          <Button @click="closeCancelModal" variant="outline" class="h-9 text-xs rounded-lg text-espresso border-cream-deep">Hủy bỏ</Button>
          <Button @click="submitCancel" class="h-9 text-xs rounded-lg bg-red-600 hover:bg-red-700 text-white" :disabled="selectedReason === 'Khác' && !customReason.trim()">Xác nhận hủy</Button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { Search, Filter, CheckCircle, X, Coffee, Clock, ChevronLeft, ChevronRight, AlertTriangle } from 'lucide-vue-next'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { statusMeta, type Order, type OrderStatus } from '@/data/orders'
import { formatVND } from '@/data/menu'
import { useOrderStore } from '@/stores/orders'
import { useToast } from '@/stores/toast'

const orderStore = useOrderStore()
const toast = useToast()

// ── Timeouts & Waiting Time (Timer) ──
const currentTime = ref(Date.now())
let timeInterval: any = null

const refreshOrders = () => {
  if (!document.hidden) {
    orderStore.fetchAllOrders()
  }
}

onMounted(() => {
  orderStore.fetchAllOrders() // Fetch real data from BE for all statuses
  timeInterval = setInterval(() => {
    currentTime.value = Date.now()
    refreshOrders()
  }, 2500)
  window.addEventListener('focus', refreshOrders)
})

onUnmounted(() => {
  if (timeInterval) clearInterval(timeInterval)
  window.removeEventListener('focus', refreshOrders)
})

function getElapsedTime(o: Order): number {
  return Math.floor((currentTime.value - o.createdTs) / 60000)
}

function isLate(o: Order): boolean {
  if (o.status === 'done' || o.status === 'cancelled') return false
  return getElapsedTime(o) >= 15
}

// ── Cancel Reason Modal ──
const showCancelModal = ref(false)
const cancelOrderId = ref('')
const selectedReason = ref('Khách đổi món')
const customReason = ref('')
const presetReasons = [
  'Khách đổi món',
  'Hết nguyên liệu',
  'Đợi lâu khách về',
  'Khách nhập nhầm đơn'
]

function openCancelDialog(id: string) {
  cancelOrderId.value = id
  selectedReason.value = 'Khách đổi món'
  customReason.value = ''
  showCancelModal.value = true
}

function closeCancelModal() {
  showCancelModal.value = false
}

function submitCancel() {
  const finalReason = selectedReason.value === 'Khác'
    ? customReason.value.trim()
    : selectedReason.value
  
  orderStore.updateStatus(cancelOrderId.value, 'cancelled', finalReason)
  toast.success(`Đã hủy đơn ${cancelOrderId.value}: ${finalReason}`)
  showCancelModal.value = false
}

const filters: { id: OrderStatus | "all"; label: string }[] = [
  { id: "all", label: "Tất cả" },
  { id: "pending", label: "Chờ xác nhận" },
  { id: "preparing", label: "Đang pha chế" },
  { id: "ready", label: "Chờ lấy" },
  { id: "done", label: "Hoàn thành" },
  { id: "cancelled", label: "Đã hủy" },
]

const orders = computed(() => orderStore.orders)
const filter = ref<OrderStatus | "all">("all")
const search = ref("")
const selected = ref<Order | null>(null)

watch(orders, (newOrders) => {
  if (selected.value) {
    const found = newOrders.find(o => o.id === selected.value?.id)
    selected.value = found || newOrders[0] || null
  } else {
    selected.value = newOrders[0] || null
  }
}, { immediate: true })

const currentPage = ref(1)
const itemsPerPage = ref(8)

const visible = computed(() => {
  return orders.value.filter(o =>
    (filter.value === "all" || o.status === filter.value) &&
    (search.value === "" || 
     o.id.toLowerCase().includes(search.value.toLowerCase()) || 
     o.table.toLowerCase().includes(search.value.toLowerCase()))
  )
})

const totalPages = computed(() => Math.ceil(visible.value.length / itemsPerPage.value) || 1)

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return visible.value.slice(start, start + itemsPerPage.value)
})

watch([search, filter], () => {
  currentPage.value = 1
})

const counts = computed(() => ({
  all: orders.value.length,
  pending: orders.value.filter((o) => o.status === "pending").length,
  preparing: orders.value.filter((o) => o.status === "preparing").length,
  ready: orders.value.filter((o) => o.status === "ready").length,
  done: orders.value.filter((o) => o.status === "done").length,
  cancelled: orders.value.filter((o) => o.status === "cancelled").length,
}))

const resetFilters = () => {
  search.value = ""
  filter.value = "all"
  toast.info("Đã đặt lại tất cả bộ lọc")
}


const updateStatus = (id: string, status: OrderStatus) => {
  orderStore.updateStatus(id, status)
  toast.success(`Đơn ${id} → ${statusMeta[status].label}`)
}
</script>
