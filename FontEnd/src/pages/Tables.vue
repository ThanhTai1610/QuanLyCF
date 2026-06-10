<template>
  <div class="space-y-5 p-6">
    <!-- Summary cards -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-3">
      <button
        v-for="s in summaryCards"
        :key="s.key"
        @click="statusFilter = statusFilter === s.key ? 'all' : s.key"
        :class="[
          'flex items-center gap-3 p-4 rounded-lg border shadow-card text-left transition',
          statusFilter === s.key ? 'ring-2 ring-espresso/40' : '',
          s.cardClass
        ]"
      >
        <div :class="['w-10 h-10 rounded-lg flex items-center justify-center shrink-0', s.iconWrap]">
          <component :is="s.icon" class="w-5 h-5" />
        </div>
        <div>
          <p class="text-2xl font-display font-semibold leading-none">{{ s.count }}</p>
          <p class="text-xs text-muted-foreground mt-1">{{ s.label }}</p>
        </div>
      </button>
    </div>

    <!-- Toolbar -->
    <div class="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
      <div class="flex flex-col sm:flex-row gap-3 w-full md:w-auto">
        <div class="relative w-full sm:max-w-[200px]">
          <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Tìm bàn..."
            v-model="search"
            class="pl-9 bg-card border-cream-deep h-10 rounded-lg"
          />
        </div>
        <div class="flex gap-2 overflow-x-auto scrollbar-none">
          <button
            v-for="z in zoneFilters"
            :key="z"
            @click="filter = z"
            :class="[
              'px-4 py-2 rounded-lg text-sm font-semibold whitespace-nowrap border shadow-card',
              filter === z
                ? 'bg-espresso text-cream border-espresso'
                : 'bg-card text-espresso border-cream-deep'
            ]"
          >
            {{ z === "all" ? "Tất cả khu vực" : z }}
          </button>
        </div>
      </div>
      <Button @click="addTable" class="bg-caramel text-cream rounded-lg border border-caramel/30 shadow-card shrink-0">
        <Plus class="w-4 h-4 mr-1.5" /> Thêm bàn
      </Button>
    </div>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
      <article
        v-for="t in paginatedItems"
        :key="t.id"
        :class="[
          'bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden border-l-4',
          statusMeta[t.status].borderClass
        ]"
      >
        <!-- Header + status -->
        <div class="p-5 pb-3">
          <div class="flex justify-between items-start">
            <div>
              <h3 class="font-display text-lg text-espresso font-semibold">{{ t.name }}</h3>
              <p class="text-xs text-muted-foreground mt-0.5">{{ t.zone }} • {{ t.seats }} chỗ</p>
            </div>
            <div class="flex items-center gap-2">
              <span :class="['px-2.5 py-1 rounded-lg text-[11px] font-semibold whitespace-nowrap', statusMeta[t.status].badgeClass]">
                {{ statusMeta[t.status].label }}
              </span>
              <button @click="removeTable(t.id)" class="text-muted-foreground hover:text-destructive">
                <Trash2 class="w-4 h-4" />
              </button>
            </div>
          </div>

          <!-- Status / order detail -->
          <div class="mt-4 min-h-[68px]">
            <div v-if="t.order" class="rounded-lg bg-cream border border-cream-deep p-3 space-y-1.5">
              <div class="flex items-center justify-between text-xs">
                <span class="font-semibold text-espresso">{{ t.order.id }}</span>
                <span class="flex items-center gap-1 text-muted-foreground">
                  <Clock class="w-3 h-3" /> {{ t.order.createdAt }}
                </span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-xs text-muted-foreground">{{ orderQty(t.order) }} món</span>
                <span class="text-sm font-display font-semibold text-espresso">{{ formatVnd(t.order.total) }}</span>
              </div>
            </div>
            <div v-else class="rounded-lg bg-cream/60 border border-dashed border-cream-deep p-3 flex items-center gap-2 text-xs text-muted-foreground h-full">
              <CheckCircle2 class="w-4 h-4 text-success/70" />
              Bàn trống · sẵn sàng đón khách
            </div>
          </div>

          <!-- Context action -->
          <div class="mt-3">
            <Button
              v-if="t.status === 'pending'"
              @click="confirmTable(t)"
              size="sm"
              class="w-full bg-success text-white rounded-lg shadow-card text-xs h-9 hover:bg-success/90"
            >
              <Check class="w-3.5 h-3.5 mr-1" /> Xác nhận khách
            </Button>
            <Button
              v-else-if="t.status === 'serving'"
              @click="clearTable(t)"
              size="sm"
              variant="outline"
              class="w-full border border-cream-deep rounded-lg shadow-card text-xs h-9 text-espresso"
            >
              <RotateCcw class="w-3.5 h-3.5 mr-1" /> Trả bàn / Dọn bàn
            </Button>
            <Button
              v-else
              @click="occupyTable(t)"
              size="sm"
              variant="outline"
              class="w-full border border-cream-deep rounded-lg shadow-card text-xs h-9 text-espresso"
            >
              <Users class="w-3.5 h-3.5 mr-1" /> Đánh dấu có khách
            </Button>
          </div>
        </div>

        <!-- QR row -->
        <div class="flex items-center gap-3 px-5 py-3 border-t border-cream-deep bg-cream/40">
          <button
            @click="openQR(t)"
            class="bg-white p-1.5 rounded-lg shadow-inner border border-cream-deep shrink-0"
            title="Xem mã QR"
          >
            <QrcodeVue
              :id="`qr-${t.id}`"
              :value="`${baseUrl}/menu/${t.id}`"
              :size="56"
              foreground="#2C1A0E"
              background="#ffffff"
              level="M"
            />
          </button>
          <div class="flex-1 grid grid-cols-3 gap-2">
            <Button
              @click="printQR(t)"
              size="sm"
              variant="outline"
              class="border border-cream-deep rounded-lg shadow-card text-xs h-9"
              title="In mã QR"
            >
              <Printer class="w-3.5 h-3.5" />
            </Button>
            <Button
              @click="downloadQR(t)"
              size="sm"
              variant="outline"
              class="border border-cream-deep rounded-lg shadow-card text-xs h-9"
              title="Tải mã QR"
            >
              <Download class="w-3.5 h-3.5" />
            </Button>
            <router-link :to="`/menu/${t.id}`" target="_blank">
              <Button
                size="sm"
                variant="outline"
                class="border border-cream-deep rounded-lg shadow-card text-xs h-9 w-full"
                title="Mở trang gọi món"
              >
                <ExternalLink class="w-3.5 h-3.5" />
              </Button>
            </router-link>
          </div>
        </div>
      </article>

      <!-- Empty state -->
      <div v-if="paginatedItems.length === 0" class="col-span-full py-12 text-center text-muted-foreground">
        Không tìm thấy bàn nào phù hợp.
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="filteredItems.length > 0" class="flex items-center justify-between py-2 border-t border-cream-deep/50 mt-4">
      <div class="text-sm text-muted-foreground">
        Hiển thị <span class="font-medium text-espresso">{{ (currentPage - 1) * itemsPerPage + 1 }}</span> - <span class="font-medium text-espresso">{{ Math.min(currentPage * itemsPerPage, filteredItems.length) }}</span> / <span class="font-medium text-espresso">{{ filteredItems.length }}</span> bàn
      </div>
      <div class="flex items-center gap-2">
        <Button
          variant="outline"
          size="icon"
          @click="currentPage--"
          :disabled="currentPage === 1"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50"
        >
          <ChevronLeft class="w-4 h-4" />
        </Button>
        <span class="text-sm font-semibold text-espresso px-3">
          Trang {{ currentPage }} / {{ totalPages }}
        </span>
        <Button
          variant="outline"
          size="icon"
          @click="currentPage++"
          :disabled="currentPage === totalPages"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50"
        >
          <ChevronRight class="w-4 h-4" />
        </Button>
      </div>
    </div>

    <!-- QR Modal -->
    <Modal v-model="qrModalOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-semibold">Mã QR · {{ activeTable?.name }}</h2>
        <p class="text-sm text-muted-foreground">Quét để gọi món · {{ activeTable?.zone }}</p>
      </template>

      <div v-if="activeTable" class="flex flex-col items-center gap-4">
        <div class="bg-white p-4 rounded-lg shadow-inner border border-cream-deep">
          <QrcodeVue
            :id="`qr-modal-${activeTable.id}`"
            :value="`${baseUrl}/menu/${activeTable.id}`"
            :size="240"
            foreground="#2C1A0E"
            background="#ffffff"
            level="M"
          />
        </div>
        <code class="text-xs text-muted-foreground break-all text-center">{{ baseUrl }}/menu/{{ activeTable.id }}</code>
      </div>

      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-lg" @click="downloadQR(activeTable!)">
          <Download class="w-4 h-4 mr-1.5" /> Tải xuống
        </Button>
        <Button class="bg-espresso text-cream rounded-lg" @click="printQR(activeTable!)">
          <Printer class="w-4 h-4 mr-1.5" /> In mã QR
        </Button>
      </template>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import QrcodeVue from 'qrcode.vue'
import {
  Printer, Download, Plus, Trash2, ExternalLink, Search,
  ChevronLeft, ChevronRight, Check, RotateCcw, Users, Clock,
  CheckCircle2, Coffee, Bell, Grid3x3
} from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import { mockOrders } from '@/data/orders'

type TableStatus = "empty" | "pending" | "serving"

interface TableOrder {
  id: string
  total: number
  createdAt: string
  qty: number
}

interface Table {
  id: string
  name: string
  zone: string
  seats: number
  status: TableStatus
  order?: TableOrder
}

const toast = { success: (msg: string) => alert('Thành công: ' + msg) }

const baseTables: Omit<Table, "status" | "order">[] = [
  { id: "1", name: "Bàn 1", zone: "Trong nhà", seats: 2 },
  { id: "2", name: "Bàn 2", zone: "Trong nhà", seats: 4 },
  { id: "3", name: "Bàn 3", zone: "Trong nhà", seats: 4 },
  { id: "5", name: "Bàn 5", zone: "Cửa sổ", seats: 2 },
  { id: "7", name: "Bàn 7", zone: "Sân vườn", seats: 6 },
  { id: "8", name: "Bàn 8", zone: "Sân vườn", seats: 4 },
  { id: "12", name: "Bàn 12", zone: "Tầng 2", seats: 4 },
]

// Derive table status from active orders (customer scanned QR & ordered)
function seedTable(base: Omit<Table, "status" | "order">): Table {
  const active = mockOrders.find(
    (o) => o.table === base.name && (o.status === "pending" || o.status === "preparing")
  )
  if (!active) return { ...base, status: "empty" }
  const order: TableOrder = {
    id: active.id,
    total: active.total,
    createdAt: active.createdAt,
    qty: active.items.reduce((s, i) => s + i.qty, 0),
  }
  // pending order = customer just ordered, chờ nhân viên xác nhận
  return { ...base, status: active.status === "pending" ? "pending" : "serving", order }
}

const tables = ref<Table[]>(baseTables.map(seedTable))
const baseUrl = typeof window !== "undefined" ? window.location.origin : ""

const statusMeta: Record<TableStatus, { label: string; badgeClass: string; borderClass: string }> = {
  empty: {
    label: "Trống",
    badgeClass: "bg-muted text-muted-foreground border border-cream-deep",
    borderClass: "border-l-cream-deep",
  },
  pending: {
    label: "Chờ xác nhận",
    badgeClass: "bg-warning/15 text-warning border border-warning/30",
    borderClass: "border-l-warning",
  },
  serving: {
    label: "Đang phục vụ",
    badgeClass: "bg-caramel/15 text-caramel border border-caramel/30",
    borderClass: "border-l-caramel",
  },
}

const zoneFilters = ["all", "Trong nhà", "Cửa sổ", "Sân vườn", "Tầng 2"]
const filter = ref("all")
const statusFilter = ref<TableStatus | "all">("all")
const search = ref("")
const currentPage = ref(1)
const itemsPerPage = ref(8)

const summaryCards = computed(() => [
  {
    key: "all" as const, label: "Tổng số bàn", icon: Grid3x3,
    count: tables.value.length,
    cardClass: "bg-card border-cream-deep text-espresso",
    iconWrap: "bg-cream text-espresso",
  },
  {
    key: "serving" as const, label: "Đang phục vụ", icon: Coffee,
    count: tables.value.filter(t => t.status === "serving").length,
    cardClass: "bg-card border-cream-deep text-caramel",
    iconWrap: "bg-caramel/15 text-caramel",
  },
  {
    key: "pending" as const, label: "Chờ xác nhận", icon: Bell,
    count: tables.value.filter(t => t.status === "pending").length,
    cardClass: "bg-card border-cream-deep text-warning",
    iconWrap: "bg-warning/15 text-warning",
  },
  {
    key: "empty" as const, label: "Bàn trống", icon: CheckCircle2,
    count: tables.value.filter(t => t.status === "empty").length,
    cardClass: "bg-card border-cream-deep text-success",
    iconWrap: "bg-success/15 text-success",
  },
])

const filteredItems = computed(() => {
  return tables.value.filter((t) => {
    const matchZone = filter.value === "all" || t.zone === filter.value
    const matchStatus = statusFilter.value === "all" || t.status === statusFilter.value
    const matchSearch = t.name.toLowerCase().includes(search.value.toLowerCase())
    return matchZone && matchStatus && matchSearch
  })
})

const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage.value) || 1)

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredItems.value.slice(start, start + itemsPerPage.value)
})

watch([search, filter, statusFilter], () => {
  currentPage.value = 1
})

const formatVnd = (n: number) => n.toLocaleString("vi-VN") + "đ"
const orderQty = (o: TableOrder) => o.qty

// --- Status actions ---
const confirmTable = (table: Table) => {
  table.status = "serving"
  toast.success(`Đã xác nhận khách tại ${table.name}`)
}

const clearTable = (table: Table) => {
  table.status = "empty"
  table.order = undefined
  toast.success(`Đã dọn ${table.name}, sẵn sàng đón khách mới`)
}

const occupyTable = (table: Table) => {
  table.status = "serving"
  toast.success(`Đã đánh dấu ${table.name} có khách`)
}

// --- Table CRUD ---
const addTable = () => {
  const ids = tables.value.map(t => Number(t.id)).filter(n => !isNaN(n))
  const id = String((ids.length > 0 ? Math.max(...ids) : 0) + 1)
  tables.value.push({ id, name: `Bàn ${id}`, zone: "Trong nhà", seats: 2, status: "empty" })
  toast.success(`Đã thêm Bàn ${id}`)
}

const removeTable = (id: string) => {
  tables.value = tables.value.filter(t => t.id !== id)
  toast.success("Đã xóa bàn")
}

// --- QR ---
const qrModalOpen = ref(false)
const activeTable = ref<Table | null>(null)

const openQR = (table: Table) => {
  activeTable.value = table
  qrModalOpen.value = true
}

const downloadQR = (table: Table) => {
  const canvas =
    document.querySelector<HTMLCanvasElement>(`#qr-modal-${table.id}`) ||
    document.querySelector<HTMLCanvasElement>(`#qr-${table.id}`)
  if (!canvas) return
  const link = document.createElement("a")
  link.download = `QR-${table.name}.png`
  link.href = canvas.toDataURL("image/png")
  link.click()
  toast.success(`Đã tải mã QR ${table.name}`)
}

const printQR = (table: Table) => {
  const canvas =
    document.querySelector<HTMLCanvasElement>(`#qr-modal-${table.id}`) ||
    document.querySelector<HTMLCanvasElement>(`#qr-${table.id}`)
  if (!canvas) return
  const w = window.open("", "_blank")
  if (!w) return
  w.document.write(`
    <html><head><title>In QR ${table.name}</title>
    <style>body{font-family:sans-serif;text-align:center;padding:40px;color:#2C1A0E}
    h1{font-family:Georgia;margin:0 0 8px}p{color:#6B3F1F;margin:0 0 24px}
    img{width:280px;height:280px}small{display:block;margin-top:16px;color:#888}</style>
    </head><body>
    <h1>BrewManager</h1><p>Quét mã để gọi món · ${table.name}</p>
    <img src="${canvas.toDataURL()}" />
    <small>${baseUrl}/menu/${table.id}</small>
    </body></html>
  `)
  w.document.close()
  setTimeout(() => w.print(), 200)
}
</script>
