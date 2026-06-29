<template>
  <div class="space-y-6 font-premium-sans text-[#2A231E] p-4 sm:p-6 lg:p-8 max-w-[1400px] mx-auto min-h-screen">

    <!-- ===== FLOW STEPPER ===== -->
    <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-sm p-4 flex flex-wrap items-center gap-2 text-xs font-bold uppercase tracking-wider">
      <span class="text-[10px] text-[#8A8178]">Luồng nhập kho:</span>
      <router-link to="/suppliers" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><Truck class="w-3.5 h-3.5" /> Nguồn cung</router-link>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <router-link to="/suppliers" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><ClipboardList class="w-3.5 h-3.5" /> Phiếu nhập</router-link>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <router-link to="/inventory" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><Package class="w-3.5 h-3.5" /> Kho</router-link>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <span class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#CC8033] text-white"><ClipboardCheck class="w-3.5 h-3.5" /> Kiểm kê</span>
    </div>

    <!-- Tabs -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div class="flex bg-white rounded-lg p-1 border border-[#EAE3D9] shadow-sm">
        <button @click="activeTab = 'create'" :class="['px-5 py-2.5 rounded-md text-xs font-bold uppercase tracking-wider transition-all flex items-center gap-2', activeTab === 'create' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]']">
          <ClipboardCheck class="w-4 h-4" /> Tạo phiếu kiểm kê
        </button>
        <button @click="activeTab = 'history'" :class="['px-5 py-2.5 rounded-md text-xs font-bold uppercase tracking-wider transition-all flex items-center gap-2', activeTab === 'history' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]']">
          <History class="w-4 h-4" /> Chờ duyệt & Lịch sử
          <span v-if="pendingCount > 0" class="px-1.5 py-0.5 rounded text-[10px]" :class="activeTab === 'history' ? 'bg-white/20' : 'bg-red-100 text-red-500'">{{ pendingCount }}</span>
        </button>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- TAB 1: Tạo phiếu kiểm kê -->
    <!-- ===================================================================== -->
    <div v-show="activeTab === 'create'" class="animate-in fade-in slide-in-from-bottom-4 duration-500 space-y-5">

      <!-- Header form -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-5 grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="space-y-1.5">
          <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Ngày kiểm kê</label>
          <input type="date" v-model="takeDate" class="w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
        </div>
        <div class="space-y-1.5">
          <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Người kiểm kê</label>
          <select v-model="takeBy" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
            <option v-for="p in staff" :key="p">{{ p }}</option>
          </select>
        </div>
        <div class="space-y-1.5">
          <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Khu vực / Ca</label>
          <select v-model="takeZone" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
            <option>Toàn bộ kho</option>
            <option>Quầy pha chế</option>
            <option>Bếp bánh / lạnh</option>
            <option>Kho vật tư</option>
          </select>
        </div>
      </div>

      <!-- Count table -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
        <div class="overflow-x-auto custom-scrollbar">
          <table class="w-full text-sm text-left">
            <thead>
              <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
                <th class="px-5 py-4 font-bold">Tên nguyên liệu</th>
                <th class="px-5 py-4 font-bold text-center w-24">Đơn vị</th>
                <th class="px-5 py-4 font-bold text-right w-36">Tồn hệ thống</th>
                <th class="px-5 py-4 font-bold text-right w-44">Tồn thực tế</th>
                <th class="px-5 py-4 font-bold text-right w-36">Chênh lệch</th>
                <th class="px-5 py-4 font-bold w-56">Ghi chú</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#EAE3D9]/60">
              <tr v-for="row in rows" :key="row.id" class="hover:bg-[#FDFBF7] transition-colors">
                <td class="px-5 py-3 font-bold text-[#2A231E]">{{ row.name }}</td>
                <td class="px-5 py-3 text-center text-xs font-medium text-[#5C544E] uppercase">{{ row.unit }}</td>
                <td class="px-5 py-3 text-right font-medium text-[#5C544E]">{{ formatNumber(row.system) }}</td>
                <td class="px-5 py-3">
                  <input type="number" min="0" v-model.number="row.actual" placeholder="—" class="w-full text-right bg-[#FFF9F2] border border-[#E8C5A5] h-9 rounded-md px-3 text-sm font-bold text-[#CC8033] focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20" />
                </td>
                <td class="px-5 py-3 text-right">
                  <span class="font-bold px-2.5 py-1 rounded-md text-xs" :class="diffClass(row)">
                    {{ row.actual === undefined ? '—' : (diff(row) > 0 ? '+' : '') + formatNumber(diff(row)) }}
                  </span>
                </td>
                <td class="px-5 py-3">
                  <input v-model="row.note" placeholder="Lý do chênh lệch..." class="w-full bg-white border border-[#EAE3D9] h-9 rounded-md px-3 text-xs font-medium focus:outline-none focus:border-[#CC8033]" />
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div class="px-5 py-4 border-t border-[#EAE3D9] bg-[#FDFBF7] flex flex-wrap items-center justify-between gap-3">
          <div class="text-xs font-medium text-[#5C544E]">
            Đã đếm <span class="font-bold text-[#2A231E]">{{ countedRows }}</span>/{{ rows.length }} mặt hàng •
            <span class="font-bold" :class="diffRows > 0 ? 'text-[#CC8033]' : 'text-[#4A7C59]'">{{ diffRows }}</span> mặt hàng có chênh lệch
          </div>
          <div class="flex gap-2">
            <button @click="saveDraft" class="px-5 py-2.5 rounded-lg border border-[#EAE3D9] text-[#5C544E] text-xs font-bold uppercase tracking-wider hover:bg-[#EAE3D9]/50 transition-colors bg-white shadow-sm">Lưu nháp</button>
            <button @click="submitRequest" class="px-5 py-2.5 rounded-lg bg-[#CC8033] hover:bg-[#B87029] text-white text-xs font-bold uppercase tracking-wider shadow-md transition-colors">Gửi yêu cầu điều chỉnh</button>
          </div>
        </div>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- TAB 2: Chờ duyệt & Lịch sử -->
    <!-- ===================================================================== -->
    <div v-show="activeTab === 'history'" class="animate-in fade-in slide-in-from-bottom-4 duration-500 space-y-4">
      <div v-if="requests.length === 0" class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm py-16 text-center text-[#8A8178] text-sm font-medium">
        Chưa có yêu cầu kiểm kê nào.
      </div>

      <div v-for="req in requests" :key="req.maPhieu" class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
        <div class="px-5 py-4 flex flex-wrap items-center justify-between gap-3 border-b border-[#EAE3D9] bg-[#FDFBF7]">
          <div class="flex items-center gap-4">
            <span class="font-mono text-xs font-bold text-[#2A231E]">#{{ req.maPhieu }}</span>
            <span class="text-xs text-[#5C544E]">{{ new Date(req.thoiGianTao).toLocaleString('vi-VN') }}</span>
            <span class="px-2.5 py-1 rounded-full bg-[#EAE3D9]/60 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">{{ req.chiTiets.length }} mặt hàng</span>
          </div>
          <div class="flex items-center gap-2">
            <span v-if="statusBadge[req.trangThai as keyof typeof statusBadge]" :class="['inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md border text-[10px] font-bold uppercase tracking-wider', statusBadge[req.trangThai as keyof typeof statusBadge].cls]">
              <component :is="statusBadge[req.trangThai as keyof typeof statusBadge].icon" class="w-3 h-3" /> {{ statusBadge[req.trangThai as keyof typeof statusBadge].label }}
            </span>
            <template v-if="req.trangThai === 'ChoDuyet'">
              <button @click="approve(req)" class="px-3 py-1.5 rounded-md bg-[#4A7C59] text-white text-[10px] font-bold uppercase tracking-wider hover:bg-[#3B6347] transition-colors shadow-sm">Duyệt</button>
              <button @click="reject(req)" class="px-3 py-1.5 rounded-md bg-white border border-red-200 text-red-500 text-[10px] font-bold uppercase tracking-wider hover:bg-red-50 transition-colors">Từ chối</button>
            </template>
          </div>
        </div>
        <div class="p-4 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-2">
          <div v-for="(it, i) in req.chiTiets" :key="i" class="flex items-center justify-between bg-[#FDFBF7] border border-[#EAE3D9] rounded-lg px-3 py-2">
            <span class="text-xs font-bold text-[#2A231E]">{{ it.tenNguyenLieu }}</span>
            <span class="text-xs font-bold" :class="it.chenhLech > 0 ? 'text-[#4A7C59]' : 'text-red-500'">{{ it.chenhLech > 0 ? '+' : '' }}{{ formatNumber(it.chenhLech) }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Toast -->
    <Transition name="toast">
      <div v-if="toastMsg" class="fixed bottom-6 right-6 z-[60] bg-[#2A231E] text-white px-5 py-3 rounded-xl shadow-2xl flex items-center gap-3 border border-[#CC8033]/30">
        <CheckCircle2 class="w-5 h-5 text-[#4A7C59]" />
        <span class="text-sm font-medium">{{ toastMsg }}</span>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Truck, ClipboardList, ChevronRight, Package, ClipboardCheck, History,
  CheckCircle2, Clock, XCircle
} from 'lucide-vue-next'

// ── Types ───────────────────────────────────────────────
interface Row { id: number; name: string; sku: string | null; unit: string; system: number; actual: number | undefined; note: string }

import { materialsApi } from '@/services/materials'
import { stockTakeApi, type StockTakeListItem, type StockTakeDetail } from '@/services/stockTakes'
import { onMounted } from 'vue'

const loading = ref(false)
const rows = ref<Row[]>([])
const staff = ['Hệ thống'] // We could fetch users, but let's just use current user or mock string. We can leave the mock staff list for now.
const activeTab = ref<'create' | 'history'>('create')
const takeDate = ref(new Date().toISOString().slice(0, 10))
const takeBy = ref(staff[0]!)
const takeZone = ref('Toàn bộ kho')

const fetchMaterials = async () => {
  try {
    const list = await materialsApi.list()
    rows.value = list.map(m => ({
      id: m.maNguyenLieu,
      name: m.tenNguyenLieu,
      sku: m.maVach_SKU,
      unit: m.donViTinh,
      system: m.soLuongTon,
      actual: undefined,
      note: ''
    }))
  } catch (err) {
    toast('Lỗi khi tải danh sách nguyên liệu')
  }
}

onMounted(() => {
  fetchMaterials()
  fetchRequests()
})

// ── Diff helpers ────────────────────────────────────────
const formatNumber = (n: number) => (n || 0).toLocaleString('vi-VN')
const diff = (row: Row) => row.actual === undefined ? 0 : row.actual - row.system
const diffClass = (row: Row) => {
  if (row.actual === undefined) return 'text-[#8A8178] bg-[#FDFBF7] border border-[#EAE3D9]'
  const d = diff(row)
  if (d > 0) return 'text-[#4A7C59] bg-green-50 border border-green-100'
  if (d < 0) return 'text-red-500 bg-red-50 border border-red-100'
  return 'text-[#8A8178] bg-[#FDFBF7] border border-[#EAE3D9]'
}
const countedRows = computed(() => rows.value.filter(r => r.actual !== undefined).length)
const diffRows = computed(() => rows.value.filter(r => r.actual !== undefined && diff(r) !== 0).length)

// ── Requests ────────────────────────────────────────────
const requests = ref<StockTakeDetail[]>([])
const pendingCount = computed(() => requests.value.filter(r => r.trangThai === 'ChoDuyet').length)

const fetchRequests = async () => {
  try {
    const list = await stockTakeApi.list()
    // Load full details for each
    const fullList = await Promise.all(list.map(r => stockTakeApi.get(r.maPhieu)))
    requests.value = fullList
  } catch (err) {
    toast('Lỗi khi tải lịch sử kiểm kê')
  }
}

const statusBadge = {
  ChoDuyet: { label: 'Chờ duyệt', cls: 'bg-orange-50 text-orange-600 border-orange-100', icon: Clock },
  DaDuyet: { label: 'Đã duyệt', cls: 'bg-green-50 text-green-600 border-green-100', icon: CheckCircle2 },
  TuChoi: { label: 'Từ chối', cls: 'bg-red-50 text-red-500 border-red-100', icon: XCircle },
} as const

// ── Toast ───────────────────────────────────────────────
const toastMsg = ref('')
let toastTimer: ReturnType<typeof setTimeout>
const toast = (msg: string) => {
  toastMsg.value = msg
  clearTimeout(toastTimer)
  toastTimer = setTimeout(() => (toastMsg.value = ''), 3000)
}

// ── Actions ─────────────────────────────────────────────
const saveDraft = () => toast('Đã lưu nháp phiếu kiểm kê')

const submitRequest = async () => {
  const changed = rows.value.filter(r => r.actual !== undefined && diff(r) !== 0)
  if (changed.length === 0) { toast('Chưa có chênh lệch nào để gửi'); return }
  
  loading.value = true
  try {
    await stockTakeApi.create({
      ghiChu: `Kiểm kê khu vực: ${takeZone.value}`,
      chiTiets: changed.map(r => ({
        maNguyenLieu: r.id,
        soLuongThucTe: r.actual!,
        lyDoLech: r.note || null
      }))
    })
    
    // reset form
    rows.value.forEach(r => { r.actual = undefined; r.note = '' })
    activeTab.value = 'history'
    toast(`Đã gửi yêu cầu kiểm kê (${changed.length} mặt hàng)`)
    await fetchRequests()
  } catch (err) {
    toast(err instanceof Error ? err.message : 'Lỗi khi gửi yêu cầu')
  } finally {
    loading.value = false
  }
}

const approve = async (req: StockTakeDetail) => { 
  try {
    await stockTakeApi.approve(req.maPhieu)
    toast(`Đã duyệt mã phiếu #${req.maPhieu} — kho được cập nhật`) 
    await fetchRequests()
    if (activeTab.value === 'create') await fetchMaterials() // Update stock numbers
  } catch (err) {
    toast(err instanceof Error ? err.message : 'Lỗi khi duyệt phiếu')
  }
}

const reject = async (req: StockTakeDetail) => { 
  try {
    await stockTakeApi.reject(req.maPhieu)
    toast(`Đã từ chối phiếu #${req.maPhieu}`) 
    await fetchRequests()
  } catch (err) {
    toast(err instanceof Error ? err.message : 'Lỗi khi từ chối phiếu')
  }
}
</script>

<style scoped>
.font-premium-sans { font-family: 'Be Vietnam Pro', system-ui, sans-serif; }
.custom-scrollbar::-webkit-scrollbar { width: 6px; height: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background-color: #EAE3D9; border-radius: 10px; }
.custom-scrollbar:hover::-webkit-scrollbar-thumb { background-color: #D5C9B3; }
.toast-enter-active, .toast-leave-active { transition: all .3s cubic-bezier(.34,1.56,.64,1); }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(20px); }
</style>
