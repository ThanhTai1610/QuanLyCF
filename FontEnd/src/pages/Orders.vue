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
          <Button variant="outline" size="icon" class="border border-cream-deep h-10 w-10 rounded-lg shadow-card">
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
              'px-5 py-4 cursor-pointer border-l-4',
              selected?.id === o.id 
                ? 'bg-caramel-light/50 border-caramel' 
                : 'border-transparent'
            ]"
          >
            <div class="flex justify-between items-start gap-3">
              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2">
                  <span class="font-semibold text-espresso">{{ o.id }}</span>
                  <span :class="['px-3 py-1 rounded-lg text-xs font-medium', statusMeta[o.status].className]">
                    {{ statusMeta[o.status].label }}
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
                @click="updateStatus(selected.id, 'cancelled')"
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

      <div v-else class="flex-1 flex items-center justify-center text-muted-foreground p-8 text-center">
        Chọn một đơn hàng để xem chi tiết
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Search, Filter, CheckCircle, X, Coffee, Clock, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import Input from '@/components/ui/Input.vue'
import Button from '@/components/ui/Button.vue'
import { statusMeta, type Order, type OrderStatus } from '@/data/orders'
import { formatVND } from '@/data/menu'
import { useOrderStore } from '@/stores/orders'

const orderStore = useOrderStore()
const toast = { success: (msg: string) => alert('Thành công: ' + msg) }

const filters: { id: OrderStatus | "all"; label: string }[] = [
  { id: "all", label: "Tất cả" },
  { id: "pending", label: "Chờ xác nhận" },
  { id: "preparing", label: "Đang pha chế" },
  { id: "done", label: "Hoàn thành" },
  { id: "cancelled", label: "Đã hủy" },
]

const orders = computed(() => orderStore.orders)
const filter = ref<OrderStatus | "all">("all")
const search = ref("")
const selected = ref<Order | null>(orders.value[0] || null)

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
  done: orders.value.filter((o) => o.status === "done").length,
  cancelled: orders.value.filter((o) => o.status === "cancelled").length,
}))

const updateStatus = (id: string, status: OrderStatus) => {
  orderStore.updateStatus(id, status)
  toast.success(`Đơn ${id} → ${statusMeta[status].label}`)
}
</script>
