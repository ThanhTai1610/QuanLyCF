<template>
  <div class="min-h-screen text-[#FDFBF7] font-premium-sans bg-[#0F0A07] flex flex-col">

    <!-- ===== HEADER ===== -->
    <header class="h-16 px-6 flex items-center justify-between border-b-2 border-white/10 bg-[#1A1512] shadow-card shrink-0">
      <div class="flex items-center gap-3">
        <div class="w-10 h-10 rounded-lg bg-[#CC8033] flex items-center justify-center shadow-card">
          <Coffee class="w-5 h-5 text-[#0F0A07]" stroke-width="1.5" />
        </div>
        <div>
          <h1 class="font-premium-serif text-xl font-bold tracking-wide">BrewManager <span class="text-[#CC8033] ml-1">·</span> KDS</h1>
          <p class="text-[9px] uppercase tracking-[0.3em] text-[#8A8178] font-bold">Màn hình điều phối bếp realtime</p>
        </div>
      </div>

      <!-- Stats + Clock -->
      <div class="flex items-center gap-8">
        <div class="flex gap-6">
          <div class="text-center">
            <div class="text-[#CC8033] font-premium-sans text-xl font-bold">{{ inProgress }}</div>
            <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">Đang làm</div>
          </div>
          <div class="text-center">
            <div class="text-emerald-400 font-premium-sans text-xl font-bold">{{ readyCount }}</div>
            <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">Sẵn sàng</div>
          </div>
          <div class="text-center">
            <div class="text-[#8A8178] font-premium-sans text-xl font-bold">{{ completedOrders.length }}</div>
            <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">Đã xong</div>
          </div>
        </div>
        <div class="h-8 w-px bg-white/10"></div>
        <div class="flex items-center gap-4">
          <div class="font-premium-sans text-3xl font-medium tabular-nums tracking-tight text-[#CC8033]">{{ timeString }}</div>
          <button @click="muted = !muted" class="w-9 h-9 rounded-lg border border-white/10 flex items-center justify-center">
            <VolumeX v-if="muted" class="w-4 h-4 text-[#8A8178]" />
            <Volume2 v-else class="w-4 h-4 text-[#CC8033]" />
          </button>
        </div>
      </div>
    </header>

    <!-- ===== TAB BAR ===== -->
    <div class="shrink-0 flex items-center gap-1 px-6 pt-4 pb-0">
      <button
        v-for="tab in tabs"
        :key="tab.id"
        @click="activeTab = tab.id"
        :class="[
          'flex items-center gap-2 px-5 py-2.5 rounded-t-lg text-xs font-bold uppercase tracking-[0.15em] transition-all duration-200 border-b-2',
          activeTab === tab.id
            ? 'bg-[#1A1512] text-[#CC8033] border-[#CC8033]'
            : 'text-[#8A8178] border-transparent hover:text-white hover:bg-white/5'
        ]"
      >
        <component :is="tab.icon" class="w-3.5 h-3.5" stroke-width="2" />
        {{ tab.label }}
        <span
          v-if="tab.count !== undefined"
          :class="[
            'px-2 py-0.5 rounded-full text-[9px] font-black',
            activeTab === tab.id ? 'bg-[#CC8033]/20 text-[#CC8033]' : 'bg-white/10 text-[#8A8178]'
          ]"
        >{{ tab.count }}</span>
      </button>
    </div>

    <!-- ===== DIVIDER ===== -->
    <div class="h-px bg-white/10 mx-6"></div>

    <!-- ===== PANEL: ĐANG LÀM ===== -->
    <main v-if="activeTab === 'active'" class="p-6 flex-1 overflow-y-auto">
      <!-- Grid orders -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5">
        <article
          v-for="o in paginatedActive"
          :key="o.id"
          class="rounded-lg border bg-[#1A1512] shadow-card flex flex-col relative overflow-hidden transition-all duration-300"
          :class="isAllDone(o) ? 'border-emerald-500/50 shadow-[0_0_20px_rgba(52,211,153,0.08)]' : 'border-white/10'"
        >
          <!-- Card Header -->
          <div class="p-4 border-b-2 border-white/10 flex justify-between items-start bg-black/30">
            <div>
              <div class="font-premium-sans text-3xl font-bold tracking-tight">Bàn {{ o.table }}</div>
              <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mt-0.5">Order #{{ o.id }}</div>
            </div>
            <!-- Timer badge -->
            <div :class="['flex items-center gap-1.5 px-2.5 py-1 rounded-lg border text-xs font-bold tabular-nums', colorByMin(o.startedAt)]">
              <span class="w-1.5 h-1.5 rounded-full bg-current animate-pulse"></span>
              {{ fmtElapsed(o.startedAt) }}
            </div>
          </div>

          <!-- Items List -->
          <div class="flex-1 p-4 space-y-3">
            <div v-for="(it, i) in o.items" :key="i">
              <button @click="toggle(o.id, i)" class="w-full flex items-start gap-3 text-left group">
                <!-- Checkbox -->
                <div :class="['mt-0.5 w-5 h-5 rounded-md border-2 flex items-center justify-center shrink-0 transition-all duration-200',
                  it.done ? 'bg-[#CC8033] border-[#CC8033]' : 'border-white/20 group-hover:border-white/40']"
                >
                  <Check v-if="it.done" class="w-3 h-3 text-white" stroke-width="3" />
                </div>
                <div class="flex-1">
                  <span :class="['text-sm font-bold tracking-tight block transition-colors duration-200', it.done ? 'line-through text-white/30' : 'text-white']">
                    {{ it.name }}
                    <span :class="it.done ? 'text-white/20' : 'text-[#CC8033]'" class="ml-1">× {{ it.qty }}</span>
                  </span>
                  <div v-if="it.note" class="mt-1.5 text-[9px] uppercase tracking-wider font-bold text-[#CC8033] bg-[#CC8033]/10 border border-[#CC8033]/20 px-2 py-0.5 rounded inline-block">
                    {{ it.note }}
                  </div>
                </div>
              </button>
            </div>
          </div>

          <!-- Progress bar -->
          <div class="px-4 pb-2">
            <div class="h-1 bg-white/10 rounded-full overflow-hidden">
              <div
                class="h-full bg-[#CC8033] rounded-full transition-all duration-500"
                :style="{ width: (o.items.filter(i => i.done).length / o.items.length * 100) + '%' }"
              ></div>
            </div>
            <div class="flex justify-between mt-1">
              <span class="text-[9px] text-[#8A8178] font-bold">{{ o.items.filter(i => i.done).length }}/{{ o.items.length }} món</span>
              <span class="text-[9px] text-[#8A8178] font-bold">{{ Math.round(o.items.filter(i => i.done).length / o.items.length * 100) }}%</span>
            </div>
          </div>

          <!-- Action Button -->
          <div class="p-4 pt-2">
            <button
              @click="complete(o)"
              :disabled="!isAllDone(o)"
              class="w-full h-10 rounded-lg font-bold text-[10px] uppercase tracking-[0.2em] border transition-all duration-300"
              :class="isAllDone(o)
                ? 'bg-[#CC8033] text-white border-[#CC8033] hover:bg-[#B8722D] shadow-[0_8px_20px_rgba(204,128,51,0.3)]'
                : 'bg-white/5 text-white/20 border-white/10 cursor-not-allowed'"
            >
              <span class="flex items-center justify-center gap-2">
                <CheckCircle2 v-if="isAllDone(o)" class="w-3.5 h-3.5" stroke-width="2.5" />
                Hoàn tất phục vụ
              </span>
            </button>
          </div>
        </article>
      </div>

      <!-- Pagination -->
      <div v-if="activeOrders.length > itemsPerPage" class="flex items-center justify-between mt-6 p-4 bg-[#1A1512] rounded-lg border border-white/10">
        <div class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">
          Trang <span class="text-[#CC8033]">{{ currentPage }}</span> / <span class="text-[#CC8033]">{{ totalPages }}</span>
          &nbsp;·&nbsp; <span class="text-[#CC8033]">{{ activeOrders.length }}</span> yêu cầu
        </div>
        <div class="flex items-center gap-3">
          <button @click="currentPage--" :disabled="currentPage === 1"
            class="w-9 h-9 rounded-lg border border-white/10 flex items-center justify-center bg-black/20 text-[#8A8178] disabled:opacity-30 hover:bg-black/40">
            <ChevronLeft class="w-4 h-4" />
          </button>
          <button @click="currentPage++" :disabled="currentPage === totalPages"
            class="w-9 h-9 rounded-lg border border-white/10 flex items-center justify-center bg-black/20 text-[#8A8178] disabled:opacity-30 hover:bg-black/40">
            <ChevronRight class="w-4 h-4" />
          </button>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="activeOrders.length === 0" class="text-center py-32">
        <div class="w-20 h-20 rounded-lg border border-white/10 flex items-center justify-center mx-auto mb-6 bg-[#1A1512]">
          <Coffee class="w-8 h-8 text-white/10" stroke-width="1" />
        </div>
        <h3 class="font-premium-serif text-2xl font-medium text-white/20">Mọi thứ đã sẵn sàng</h3>
        <p class="text-[9px] uppercase tracking-[0.3em] text-[#8A8178] font-bold mt-3">Hiện không có yêu cầu mới từ quầy</p>
      </div>
    </main>

    <!-- ===== PANEL: LỊCH SỬ ===== -->
    <main v-else class="p-6 flex-1 overflow-y-auto">

      <!-- Search + Filter bar -->
      <div class="flex items-center gap-3 mb-5">
        <div class="relative flex-1 max-w-xs">
          <Search class="w-3.5 h-3.5 absolute left-3 top-1/2 -translate-y-1/2 text-[#8A8178]" stroke-width="2" />
          <input
            v-model="historySearch"
            placeholder="Tìm theo bàn, order..."
            class="w-full pl-9 pr-4 h-9 bg-[#1A1512] border border-white/10 rounded-lg text-xs font-medium text-white placeholder:text-[#8A8178] focus:outline-none focus:border-[#CC8033]/50"
          />
        </div>
        <div class="flex items-center gap-2 text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">
          <Clock class="w-3 h-3" />
          Hôm nay
        </div>
        <button @click="clearHistory" v-if="completedOrders.length > 0"
          class="ml-auto flex items-center gap-1.5 px-3 h-9 rounded-lg border border-red-500/20 bg-red-500/5 text-red-400 text-[10px] font-bold uppercase tracking-wide hover:bg-red-500/10 transition-colors">
          <Trash2 class="w-3 h-3" stroke-width="2" />
          Xoá lịch sử
        </button>
      </div>

      <!-- Summary strip -->
      <div class="grid grid-cols-3 gap-4 mb-6">
        <div class="bg-[#1A1512] rounded-lg border border-white/10 p-4 text-center">
          <div class="font-premium-sans text-3xl font-bold text-emerald-400">{{ completedOrders.length }}</div>
          <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mt-1">Đơn hoàn thành</div>
        </div>
        <div class="bg-[#1A1512] rounded-lg border border-white/10 p-4 text-center">
          <div class="font-premium-sans text-3xl font-bold text-[#CC8033]">{{ totalItemsDone }}</div>
          <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mt-1">Món đã phục vụ</div>
        </div>
        <div class="bg-[#1A1512] rounded-lg border border-white/10 p-4 text-center">
          <div class="font-premium-sans text-3xl font-bold text-white">{{ avgDuration }}</div>
          <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mt-1">TG xử lý TB</div>
        </div>
      </div>

      <!-- History list -->
      <div v-if="filteredHistory.length > 0" class="space-y-3">
        <div
          v-for="o in filteredHistory"
          :key="o.id"
          class="bg-[#1A1512] border border-white/10 rounded-lg overflow-hidden hover:border-white/20 transition-colors duration-200"
        >
          <!-- Row header -->
          <div class="flex items-center justify-between px-5 py-3 border-b border-white/5 bg-black/20">
            <div class="flex items-center gap-4">
              <!-- Done badge -->
              <div class="w-8 h-8 rounded-lg bg-emerald-500/15 border border-emerald-500/30 flex items-center justify-center">
                <CheckCircle2 class="w-4 h-4 text-emerald-400" stroke-width="2" />
              </div>
              <div>
                <span class="font-premium-sans text-lg font-bold">Bàn {{ o.table }}</span>
                <span class="ml-2 text-[#8A8178] text-xs font-medium">#{{ o.id }}</span>
              </div>
              <!-- Items count pill -->
              <span class="px-2.5 py-1 rounded-full bg-white/5 border border-white/10 text-[10px] font-bold text-[#8A8178] uppercase tracking-wide">
                {{ o.items.length }} món
              </span>
            </div>

            <div class="flex items-center gap-6 text-right">
              <!-- Duration -->
              <div>
                <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mb-0.5">Thời gian xử lý</div>
                <div :class="['text-sm font-bold tabular-nums', durationColor(o.duration)]">
                  {{ fmtDuration(o.duration) }}
                </div>
              </div>
              <!-- Completed at -->
              <div>
                <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mb-0.5">Hoàn tất lúc</div>
                <div class="text-sm font-bold text-white tabular-nums">{{ o.completedAt }}</div>
              </div>
              <!-- Expand toggle -->
              <button @click="toggleHistory(o.id)" class="w-8 h-8 rounded-lg border border-white/10 flex items-center justify-center text-[#8A8178] hover:text-white hover:bg-white/5 transition-colors">
                <ChevronDown class="w-4 h-4 transition-transform duration-200" :class="expandedHistory.has(o.id) ? 'rotate-180' : ''" />
              </button>
            </div>
          </div>

          <!-- Expandable items list -->
          <div v-if="expandedHistory.has(o.id)" class="px-5 py-3 grid grid-cols-2 md:grid-cols-3 gap-2">
            <div
              v-for="(it, i) in o.items"
              :key="i"
              class="flex items-center gap-2 px-3 py-2 rounded-lg bg-black/20 border border-white/5"
            >
              <CheckCircle2 class="w-3.5 h-3.5 text-emerald-400 shrink-0" stroke-width="2" />
              <span class="text-xs font-medium text-white/70 truncate">{{ it.name }}</span>
              <span class="text-[#CC8033] text-xs font-bold ml-auto shrink-0">×{{ it.qty }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty history -->
      <div v-else class="text-center py-32">
        <div class="w-20 h-20 rounded-lg border border-white/10 flex items-center justify-center mx-auto mb-6 bg-[#1A1512]">
          <History class="w-8 h-8 text-white/10" stroke-width="1" />
        </div>
        <h3 class="font-premium-serif text-2xl font-medium text-white/20">Chưa có lịch sử</h3>
        <p class="text-[9px] uppercase tracking-[0.3em] text-[#8A8178] font-bold mt-3">Hoàn tất đơn hàng để xem lịch sử tại đây</p>
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import {
  Volume2, VolumeX, Coffee, ChevronLeft, ChevronRight, ChevronDown,
  Check, CheckCircle2, Search, Clock, Trash2, History, ClipboardList
} from 'lucide-vue-next'

// ── Types ──────────────────────────────────────────────────────
interface KItem    { name: string; qty: number; note?: string; done: boolean }
interface KOrder   { id: string; table: string; startedAt: number; items: KItem[] }
interface KDone    { id: string; table: string; items: KItem[]; duration: number; completedAt: string }

// ── Seed data ──────────────────────────────────────────────────
const initial: KOrder[] = [
  { id: '1042', table: '5',  startedAt: Date.now() - 8  * 60 * 1000, items: [
    { name: 'Cà phê sữa đá', qty: 2, done: false },
    { name: 'Matcha latte',   qty: 1, done: false },
    { name: 'Bánh croissant', qty: 1, done: true  },
  ]},
  { id: '1043', table: '12', startedAt: Date.now() - 3  * 60 * 1000, items: [
    { name: 'Cappuccino',  qty: 1, done: false },
    { name: 'Tiramisu Ý',  qty: 1, note: 'Ít đường, không đá', done: false },
  ]},
  { id: '1044', table: '3',  startedAt: Date.now() - 14 * 60 * 1000, items: [
    { name: 'Trà đào cam sả', qty: 2, note: 'Thêm đá', done: true },
    { name: 'Espresso',        qty: 1, done: true  },
    { name: 'Croissant bơ',    qty: 2, done: false },
  ]},
  { id: '1045', table: '8',  startedAt: Date.now() - 1  * 60 * 1000, items: [
    { name: 'Cookies & Cream', qty: 2, done: false },
    { name: 'Cheesecake dâu',  qty: 1, done: false },
  ]},
]

// ── State ──────────────────────────────────────────────────────
const activeOrders     = ref<KOrder[]>(JSON.parse(JSON.stringify(initial)))
const completedOrders  = ref<KDone[]>([])
const now              = ref(Date.now())
const muted            = ref(false)
const activeTab        = ref<'active' | 'history'>('active')
const currentPage      = ref(1)
const itemsPerPage     = 8
const historySearch    = ref('')
const expandedHistory  = ref<Set<string>>(new Set())

// Pre-seed a few history entries for demo
completedOrders.value = [
  { id: '1038', table: '2',  duration: 7  * 60 * 1000, completedAt: '09:14', items: [{ name: 'Espresso đôi', qty: 2, done: true }, { name: 'Bánh mì bơ tỏi', qty: 1, done: true }] },
  { id: '1039', table: '9',  duration: 12 * 60 * 1000, completedAt: '09:32', items: [{ name: 'Trà sữa trân châu', qty: 3, done: true }] },
  { id: '1040', table: '4',  duration: 5  * 60 * 1000, completedAt: '09:58', items: [{ name: 'Cappuccino', qty: 1, done: true }, { name: 'Cheesecake dâu', qty: 2, done: true }] },
  { id: '1041', table: '11', duration: 18 * 60 * 1000, completedAt: '10:25', items: [{ name: 'Matcha đá xay', qty: 2, done: true }, { name: 'Croissant bơ', qty: 2, done: true }, { name: 'Nước cam ép', qty: 1, done: true }] },
]

// ── Timer ──────────────────────────────────────────────────────
let timer: ReturnType<typeof setInterval> | null = null
onMounted (() => { timer = setInterval(() => { now.value = Date.now() }, 1000) })
onUnmounted(() => { if (timer) clearInterval(timer) })

// ── Tabs ───────────────────────────────────────────────────────
const tabs = computed(() => [
  { id: 'active'  as const, label: 'Đang làm',  icon: ClipboardList, count: activeOrders.value.length },
  { id: 'history' as const, label: 'Lịch sử',   icon: History,       count: completedOrders.value.length },
])

// ── Computed ───────────────────────────────────────────────────
const timeString = computed(() =>
  new Date(now.value).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
)

const inProgress  = computed(() => activeOrders.value.filter(o => o.items.some(i => !i.done)).length)
const readyCount  = computed(() => activeOrders.value.filter(o => o.items.every(i => i.done)).length)

const totalPages  = computed(() => Math.ceil(activeOrders.value.length / itemsPerPage) || 1)
const paginatedActive = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  return activeOrders.value.slice(start, start + itemsPerPage)
})

const filteredHistory = computed(() => {
  const q = historySearch.value.toLowerCase().trim()
  if (!q) return completedOrders.value
  return completedOrders.value.filter(o =>
    o.table.includes(q) || o.id.includes(q) || o.items.some(i => i.name.toLowerCase().includes(q))
  )
})

const totalItemsDone = computed(() =>
  completedOrders.value.reduce((s, o) => s + o.items.reduce((ss, i) => ss + i.qty, 0), 0)
)

const avgDuration = computed(() => {
  if (!completedOrders.value.length) return '—'
  const avg = completedOrders.value.reduce((s, o) => s + o.duration, 0) / completedOrders.value.length
  return fmtDuration(avg)
})

// ── Helpers ────────────────────────────────────────────────────
const isAllDone = (o: KOrder) => o.items.every(i => i.done)

const fmtElapsed = (started: number) => {
  const s = Math.floor((now.value - started) / 1000)
  return `${String(Math.floor(s / 60)).padStart(2, '0')}:${String(s % 60).padStart(2, '0')}`
}

const fmtDuration = (ms: number) => {
  const m = Math.floor(ms / 60000)
  const s = Math.floor((ms % 60000) / 1000)
  return `${m}p ${String(s).padStart(2, '0')}s`
}

const colorByMin = (started: number) => {
  const m = (now.value - started) / 60000
  if (m < 10) return 'text-emerald-400 border-emerald-500/30 bg-emerald-500/5'
  if (m < 15) return 'text-amber-400 border-amber-500/30 bg-amber-500/5'
  return 'text-red-400 border-red-500/30 bg-red-500/5'
}

const durationColor = (ms: number) => {
  const m = ms / 60000
  if (m < 10) return 'text-emerald-400'
  if (m < 15) return 'text-amber-400'
  return 'text-red-400'
}

// ── Actions ────────────────────────────────────────────────────
const toggle = (oid: string, idx: number) => {
  const o = activeOrders.value.find(o => o.id === oid)
  if (o) o.items[idx].done = !o.items[idx].done
}

const complete = (o: KOrder) => {
  if (!isAllDone(o)) return
  const duration = Date.now() - o.startedAt
  const completedAt = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
  completedOrders.value.unshift({ id: o.id, table: o.table, items: [...o.items], duration, completedAt })
  activeOrders.value = activeOrders.value.filter(a => a.id !== o.id)
  if (currentPage.value > totalPages.value && currentPage.value > 1) currentPage.value--
}

const toggleHistory = (id: string) => {
  if (expandedHistory.value.has(id)) expandedHistory.value.delete(id)
  else expandedHistory.value.add(id)
  expandedHistory.value = new Set(expandedHistory.value) // trigger reactivity
}

const clearHistory = () => {
  if (confirm('Xoá toàn bộ lịch sử hôm nay?')) completedOrders.value = []
}
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans { font-family: 'Be Vietnam Pro', system-ui, sans-serif; }

::-webkit-scrollbar       { width: 4px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background-color: rgba(255,255,255,0.05); border-radius: 4px; }
</style>
