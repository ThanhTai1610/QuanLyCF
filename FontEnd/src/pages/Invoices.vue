<template>
  <div class="space-y-4 md:space-y-5 font-premium-sans text-[#2A231E] p-4 sm:p-6 max-w-[1400px] mx-auto flex flex-col">
   
    <!-- Thanh công cụ -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 flex-shrink-0">
      <div class="flex items-center gap-3 w-full md:w-auto">
        <div class="relative flex-1 md:w-96">
          <Search class="w-5 h-5 absolute left-4 top-1/2 -translate-y-1/2 text-[#8A8178]" stroke-width="2" />
          <Input
            v-model="search"
            placeholder="Tìm mã hoá đơn, số bàn..."
            class="pl-12 w-full bg-white border border-[#EAE3D9] h-12 rounded-md shadow-card text-base font-medium"
          />
        </div>
        
        <Button variant="outline" class="border border-[#EAE3D9] text-[#5C544E] h-12 rounded-md bg-white shadow-card text-xs font-bold uppercase tracking-wider px-4 whitespace-nowrap">
          <Calendar class="w-4 h-4 mr-2" stroke-width="2" /> Hôm nay
        </Button>
        <Button variant="outline" class="border border-[#EAE3D9] text-[#5C544E] h-12 w-12 rounded-md bg-white shadow-card p-0 flex items-center justify-center flex-shrink-0">
          <Filter class="w-4 h-4" stroke-width="2" />
        </Button>
      </div>
     
      <Button class="w-full md:w-auto bg-[#CC8033] text-white h-12 rounded-md shadow-card border border-[#CC8033]/30 text-xs font-bold uppercase tracking-wider">
        <FileText class="w-4 h-4 mr-2" stroke-width="2.5" /> Xuất báo cáo
      </Button>
    </div>

    <!-- Thanh Bulk Actions -->
    <div v-if="selected.length > 0" class="bg-[#2A231E] text-[#FDFBF7] rounded-md px-4 py-2.5 flex items-center justify-between shadow-card flex-shrink-0">
      <span class="text-xs font-bold uppercase tracking-widest text-[#CC8033]">Đã chọn {{ selected.length }} hoá đơn</span>
      <div class="flex gap-2">
        <Button variant="outline" class="h-8 text-[10px] font-bold uppercase tracking-wider bg-transparent border-white/20 text-white rounded-md px-3">
          <Printer class="w-3 h-3 mr-1.5" stroke-width="2" /> In tất cả
        </Button>
        <Button class="h-8 text-[10px] font-bold uppercase tracking-wider bg-[#CC8033] text-white border-none rounded-md shadow-xl px-3">
          <Download class="w-3 h-3 mr-1.5" stroke-width="2" /> Xuất Excel
        </Button>
      </div>
    </div>

    <!-- Bảng dữ liệu -->
    <div class="bg-white rounded-md border border-[#EAE3D9] shadow-card flex flex-col overflow-hidden">
      <div class="overflow-x-auto custom-scrollbar">
        <table class="w-full text-left">
          <thead>
            <tr class="bg-[#FDFBF7] text-[#8A8178] text-[9px] uppercase tracking-[0.1em] border-b-2 border-[#EAE3D9]">
              <th class="px-4 py-3 w-10 text-center">
                <input
                  type="checkbox"
                  @change="toggleAll"
                  :checked="selected.length === filteredInvoices.length && filteredInvoices.length > 0"
                  class="rounded-md border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033] w-3.5 h-3.5 cursor-pointer"
                />
              </th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Mã HĐ</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap text-center">Bàn</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Ngày & Giờ</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap text-right">Tổng tiền</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Thanh toán</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Thu ngân</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap text-center">Trạng thái</th>
              <th class="pl-4 pr-8 py-3 font-bold whitespace-nowrap text-right w-32">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="inv in paginatedItems" :key="inv.id" :class="['border-b-2 border-[#EAE3D9]/60', selected.includes(inv.id) ? 'bg-[#CC8033]/5' : '']">
              <td class="px-4 py-2.5 text-center">
                <input
                  type="checkbox"
                  :checked="selected.includes(inv.id)"
                  @change="toggle(inv.id)"
                  class="rounded-md border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033] w-3.5 h-3.5 cursor-pointer"
                />
              </td>
              <td class="px-4 py-2.5 font-bold text-[#2A231E] text-xs whitespace-nowrap">{{ inv.id }}</td>
              <td class="px-4 py-2.5 text-center">
                <span class="px-2 py-1 rounded-md bg-white border border-[#EAE3D9] text-[10px] font-bold text-[#5C544E] uppercase tracking-wider shadow-xl">{{ inv.table }}</span>
              </td>
              <td class="px-4 py-2.5 text-[#8A8178] text-[11px] font-medium whitespace-nowrap">{{ inv.time }}</td>
              <td class="px-4 py-2.5 font-bold text-[#CC8033] text-xs text-right whitespace-nowrap">{{ formatVND(inv.total) }}</td>
              <td class="px-4 py-2.5 text-[#5C544E] text-[11px] font-medium whitespace-nowrap">{{ inv.method }}</td>
              <td class="px-4 py-2.5 text-[#5C544E] text-[11px] font-medium whitespace-nowrap">{{ inv.staff }}</td>
              <td class="px-4 py-2.5 text-center whitespace-nowrap">
                <div :class="['inline-flex items-center gap-1.5 px-3 py-1.5 rounded-md border shadow-xl', statusBadge[inv.status].cls]">
                  <div :class="['w-1.5 h-1.5 rounded-full', statusBadge[inv.status].dot]"></div>
                  <span class="text-[9px] font-bold uppercase tracking-[0.1em]">{{ statusBadge[inv.status].label }}</span>
                </div>
              </td>
              <!-- NÚT LUÔN HIỂN THỊ -->
              <td class="pl-4 pr-8 py-2.5">
                <div class="flex justify-end gap-1.5">
                  <button @click="preview = inv" class="p-2 text-[#8A8178] border border-[#EAE3D9] rounded-md shadow-xl" title="Xem chi tiết">
                    <Eye class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button @click="printInv" class="p-2 text-[#8A8178] border border-[#EAE3D9] rounded-md shadow-xl" title="In lại">
                    <Printer class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button @click="downloadInv" class="p-2 text-[#8A8178] border border-[#EAE3D9] rounded-md shadow-xl" title="Tải PDF">
                    <Download class="w-4 h-4" stroke-width="2" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="paginatedItems.length === 0">
              <td colspan="9" class="px-4 py-10 text-center text-[#8A8178] text-xs font-medium">
                Không tìm thấy hóa đơn nào phù hợp.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="filteredInvoices.length > 0" class="flex items-center justify-between px-6 py-4 border-t-2 border-[#EAE3D9] bg-[#FDFBF7]">
        <div class="text-xs font-medium text-[#8A8178]">
          Hiển thị <span class="text-[#2A231E]">{{ (currentPage - 1) * itemsPerPage + 1 }}</span> - <span class="text-[#2A231E]">{{ Math.min(currentPage * itemsPerPage, filteredInvoices.length) }}</span> / <span class="text-[#2A231E]">{{ filteredInvoices.length }}</span> hóa đơn
        </div>
        <div class="flex items-center gap-2">
          <button 
            @click="currentPage--" 
            :disabled="currentPage === 1"
            class="p-2 border border-[#EAE3D9] bg-white rounded-md hover:bg-[#FDFBF7] disabled:opacity-50 transition-colors shadow-sm text-[#2A231E]"
          >
            <ChevronLeft class="w-4 h-4" />
          </button>
          <span class="text-xs font-bold text-[#2A231E] px-2">
            Trang {{ currentPage }} / {{ totalPages }}
          </span>
          <button 
            @click="currentPage++" 
            :disabled="currentPage === totalPages"
            class="p-2 border border-[#EAE3D9] bg-white rounded-md hover:bg-[#FDFBF7] disabled:opacity-50 transition-colors shadow-sm text-[#2A231E]"
          >
            <ChevronRight class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Chi tiết Hóa Đơn -->
    <Modal v-model="isPreviewOpen">
      <div v-if="preview" class="bg-white rounded-md border border-[#EAE3D9] shadow-card overflow-hidden sm:max-w-md w-full mx-auto">
        <div class="p-8 pb-4">
          <div class="text-center border-b border-dashed border-[#EAE3D9] pb-5 mb-5">
            <div class="w-10 h-10 mx-auto bg-[#2A231E] rounded-md flex items-center justify-center text-white mb-3">
              <Coffee class="w-5 h-5" />
            </div>
            <h2 class="font-premium-serif text-3xl font-bold text-[#2A231E]">BrewManager</h2>
            <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mt-2">123 Nguyễn Huệ, Q1, TP.HCM</p>
            <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mt-0.5">Hotline: 0909 123 456</p>
          </div>
         
          <div class="py-2 text-[11px] font-medium text-[#5C544E] space-y-2">
            <div class="flex justify-between"><span class="text-[#8A8178] uppercase tracking-wider">Mã HĐ:</span><span class="font-bold text-[#2A231E]">{{ preview.id }}</span></div>
            <div class="flex justify-between"><span class="text-[#8A8178] uppercase tracking-wider">{{ preview.table }}:</span><span class="font-bold text-[#2A231E]">{{ preview.time }}</span></div>
            <div class="flex justify-between"><span class="text-[#8A8178] uppercase tracking-wider">Thu ngân:</span><span class="font-bold text-[#2A231E]">{{ preview.staff }}</span></div>
          </div>
         
          <div class="border-t border-dashed border-[#EAE3D9] my-4 pt-4 pb-2 space-y-3 text-[11px] font-medium text-[#2A231E]">
            <div class="flex justify-between items-start">
              <div>
                <span class="font-bold">Cappuccino</span>
                <div class="text-[#8A8178] text-[10px] mt-0.5">45.000₫ × 2</div>
              </div>
              <span class="font-bold">90.000₫</span>
            </div>
            <div class="flex justify-between items-start">
              <div>
                <span class="font-bold">Bánh sừng bò (Croissant)</span>
                <div class="text-[#8A8178] text-[10px] mt-0.5">32.000₫ × 2</div>
              </div>
              <span class="font-bold">64.000₫</span>
            </div>
            <div class="flex justify-between items-start">
              <div>
                <span class="font-bold">Tiramisu Ý</span>
                <div class="text-[#8A8178] text-[10px] mt-0.5">48.000₫ × 1</div>
              </div>
              <span class="font-bold">48.000₫</span>
            </div>
          </div>
         
          <div class="border-t border-black/10 pt-4 mt-2">
            <div class="flex justify-between items-end font-premium-sans">
              <span class="text-xl font-bold text-[#2A231E]">Tổng cộng</span>
              <span class="text-3xl font-bold text-[#CC8033]">{{ formatVND(preview.total) }}</span>
            </div>
            <p class="text-right text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mt-2">Thanh toán qua: <span class="text-[#CC8033]">{{ preview.method }}</span></p>
          </div>
         
          <div class="text-center mt-8">
            <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold">Cảm ơn quý khách & Hẹn gặp lại</p>
            <p class="text-lg mt-1">☕</p>
          </div>
        </div>
        
        <div class="p-4 bg-[#FDFBF7] border-t-2 border-[#EAE3D9] flex gap-2 rounded-b-xl">
          <Button variant="outline" class="flex-1 border border-[#EAE3D9] text-[#5C544E] h-10 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-xl" @click="isPreviewOpen = false">
            Đóng
          </Button>
          <Button class="flex-1 bg-white border border-[#EAE3D9] text-[#5C544E] h-10 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-xl" @click="printInv">
            <Printer class="w-3.5 h-3.5 mr-1.5" stroke-width="2" /> In HĐ
          </Button>
          <Button class="flex-1 bg-[#CC8033] text-white border-none h-10 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-xl" @click="downloadInv">
            <Download class="w-3.5 h-3.5 mr-1.5" stroke-width="2.5" /> Lưu PDF
          </Button>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Search, Calendar, Download, Eye, Printer, FileText, Filter, Coffee, ChevronLeft, ChevronRight } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'

const formatVND = (val: number) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)

interface Invoice {
  id: string
  table: string
  time: string
  total: number
  method: string
  staff: string
  status: "paid" | "unprinted" | "printed"
}

const invoices = ref<Invoice[]>([
  { id: "HD-1045", table: "Bàn 2", time: "21/04/2026 11:30", total: 450000, method: "Tiền mặt", staff: "Lan Trần", status: "paid" },
  { id: "HD-1044", table: "Bàn 9", time: "21/04/2026 11:15", total: 120000, method: "MoMo", staff: "Vy Hoàng", status: "paid" },
  { id: "HD-1043", table: "Bàn 5", time: "21/04/2026 10:45", total: 85000, method: "ZaloPay", staff: "Lan Trần", status: "unprinted" },
  { id: "HD-1042", table: "Bàn 5", time: "21/04/2026 10:24", total: 178000, method: "Chuyển khoản", staff: "Lan Trần", status: "paid" },
  { id: "HD-1041", table: "Bàn 12", time: "21/04/2026 10:18", total: 235000, method: "MoMo", staff: "Vy Hoàng", status: "printed" },
  { id: "HD-1040", table: "Bàn 3", time: "21/04/2026 10:12", total: 105000, method: "Tiền mặt", staff: "Vy Hoàng", status: "unprinted" },
  { id: "HD-1039", table: "Bàn 8", time: "21/04/2026 10:05", total: 320000, method: "VNPay", staff: "Lan Trần", status: "paid" },
  { id: "HD-1038", table: "Bàn 1", time: "21/04/2026 09:58", total: 65000, method: "Tiền mặt", staff: "Vy Hoàng", status: "printed" },
  { id: "HD-1037", table: "Bàn 7", time: "21/04/2026 09:42", total: 145000, method: "ZaloPay", staff: "Lan Trần", status: "printed" },
  { id: "HD-1036", table: "Bàn 4", time: "21/04/2026 09:30", total: 192000, method: "Chuyển khoản", staff: "Lan Trần", status: "unprinted" },
  { id: "HD-1035", table: "Bàn 6", time: "21/04/2026 09:15", total: 80000, method: "Tiền mặt", staff: "Vy Hoàng", status: "paid" },
  { id: "HD-1034", table: "Bàn 10", time: "21/04/2026 08:50", total: 540000, method: "Thẻ VISA", staff: "Lan Trần", status: "paid" },
])

const statusBadge = {
  paid: { label: "Đã thanh toán", cls: "bg-[#F0FDF4] text-[#166534] border-[#BBF7D0]", dot: "bg-[#166534]" },
  unprinted: { label: "Chưa in", cls: "bg-[#FFF9F2] text-[#CC8033] border-[#E8C5A5]", dot: "bg-[#CC8033]" },
  printed: { label: "Đã in HĐ", cls: "bg-[#F5F2ED] text-[#5C544E] border-[#EAE3D9]", dot: "bg-[#8A8178]" },
}

const selected = ref<string[]>([])
const preview = ref<Invoice | null>(null)
const search = ref("")
const currentPage = ref(1)
const itemsPerPage = ref(8)

const isPreviewOpen = computed({
  get: () => preview.value !== null,
  set: (val) => { if (!val) preview.value = null }
})

const filteredInvoices = computed(() => {
  const query = search.value.toLowerCase()
  return invoices.value.filter((i) => 
    i.id.toLowerCase().includes(query) || 
    i.table.toLowerCase().includes(query)
  )
})

const totalPages = computed(() => Math.ceil(filteredInvoices.value.length / itemsPerPage.value) || 1)

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredInvoices.value.slice(start, start + itemsPerPage.value)
})

watch(search, () => {
  currentPage.value = 1
})

// Xử lý chọn checkbox
const toggle = (id: string) => {
  if (selected.value.includes(id)) {
    selected.value = selected.value.filter(x => x !== id)
  } else {
    selected.value.push(id)
  }
}

const toggleAll = (e: Event) => {
  const isChecked = (e.target as HTMLInputElement).checked
  if (isChecked) {
    selected.value = filteredInvoices.value.map(i => i.id)
  } else {
    selected.value = []
  }
}

const printInv = () => alert('Đang tiến hành in hóa đơn...')
const downloadInv = () => alert('Đang xuất dữ liệu PDF...')
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,400;0,500;0,600;0,700;1,400;1,500&family=Montserrat:wght@400;500;600;700&display=swap');

.font-premium-serif {
  font-family: 'Cormorant Garamond', Georgia, serif;
}
.font-premium-sans {
  font-family: 'Inter', sans-serif;
}

.custom-scrollbar::-webkit-scrollbar {
  height: 4px;
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: rgba(42, 35, 30, 0.1);
  border-radius: 4px;
}
</style>
