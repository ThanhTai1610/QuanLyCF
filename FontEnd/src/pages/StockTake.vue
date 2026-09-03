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

    <!-- Premium Tabs -->
    <div class="flex justify-center sm:justify-start">
      <div class="inline-flex bg-[#EAE3D9]/50 p-1 rounded-xl shadow-inner border border-[#EAE3D9]/50">
        <button @click="activeTab = 'create'" :class="['px-6 py-2.5 rounded-lg text-xs font-bold uppercase tracking-wider transition-all duration-300 flex items-center gap-2', activeTab === 'create' ? 'bg-white text-[#CC8033] shadow-md border border-white' : 'text-[#8A8178] hover:text-[#2A231E]']">
          <ClipboardCheck class="w-4 h-4" /> Tạo phiếu kiểm kê
        </button>
        <button @click="activeTab = 'history'" :class="['px-6 py-2.5 rounded-lg text-xs font-bold uppercase tracking-wider transition-all duration-300 flex items-center gap-2', activeTab === 'history' ? 'bg-white text-[#CC8033] shadow-md border border-white' : 'text-[#8A8178] hover:text-[#2A231E]']">
          <History class="w-4 h-4" /> Chờ duyệt & Lịch sử
          <span v-if="pendingCount > 0" class="px-1.5 py-0.5 rounded text-[10px] shadow-sm" :class="activeTab === 'history' ? 'bg-[#CC8033] text-white' : 'bg-red-500 text-white animate-pulse'">{{ pendingCount }}</span>
        </button>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- TAB 1: Tạo phiếu kiểm kê -->
    <!-- ===================================================================== -->
    <div v-show="activeTab === 'create'" class="animate-in fade-in slide-in-from-bottom-4 duration-500 space-y-5">

      <!-- Draft Banner -->
      <div v-if="draftData" class="bg-[#FDFBF7] border border-[#CC8033]/30 rounded-xl p-4 flex flex-wrap items-center justify-between gap-3 shadow-sm transition-all duration-300">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-full bg-[#CC8033]/10 flex items-center justify-center text-[#CC8033]"><Clock class="w-4 h-4" /></div>
          <div>
            <p class="text-xs font-bold text-[#2A231E]">Phát hiện bản nháp chưa gửi</p>
            <p class="text-[10px] text-[#8A8178]">Được lưu lúc: {{ draftData.time }} • Đã nhập dữ liệu của {{ draftData.items.length }} mặt hàng</p>
          </div>
        </div>
        <div class="flex gap-2">
          <button @click="loadDraft" class="px-3 py-1.5 rounded-lg bg-[#CC8033] hover:bg-[#B87029] text-white text-[10px] font-bold uppercase tracking-wider shadow-sm transition-colors">Sử dụng bản nháp</button>
          <button @click="clearDraft" class="px-3 py-1.5 rounded-lg bg-white border border-[#EAE3D9] text-[#8A8178] hover:text-red-500 hover:border-red-200 text-[10px] font-bold uppercase tracking-wider transition-colors">Xóa nháp</button>
        </div>
      </div>

      <!-- Header form -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-5 grid grid-cols-1 md:grid-cols-2 gap-4">
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
      </div>

      <!-- Count table with Search & Filters -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden flex flex-col relative z-0">
        
        <!-- Toolbar -->
        <div class="p-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex flex-wrap items-center justify-between gap-4">
          <div class="flex items-center gap-3 flex-1 min-w-[300px]">
            <div class="relative flex-1 max-w-sm">
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none"><Search class="w-4 h-4 text-[#8A8178]" /></div>
              <input v-model="searchQuery" placeholder="Tìm tên hoặc SKU..." class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg pl-10 pr-3 text-sm font-medium focus:outline-none focus:border-[#CC8033] shadow-sm transition-colors" />
            </div>
            <select v-model="filterStatus" class="bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033] shadow-sm">
              <option value="all">Tất cả mặt hàng</option>
              <option value="uncounted">Chưa kiểm đếm</option>
              <option value="counted">Đã kiểm đếm</option>
              <option value="diff">Có chênh lệch</option>
            </select>
          </div>
          
          <!-- Quick Stats Progress -->
          <div class="flex items-center gap-4 text-xs font-medium">
            <div class="flex items-center gap-2">
              <span class="w-2 h-2 rounded-full bg-[#8A8178]"></span>
              <span class="text-[#5C544E]">Chưa đếm: <b>{{ rows.length - countedRows }}</b></span>
            </div>
            <div class="flex items-center gap-2">
              <span class="w-2 h-2 rounded-full bg-[#4A7C59]"></span>
              <span class="text-[#5C544E]">Khớp: <b>{{ countedRows - diffRows }}</b></span>
            </div>
            <div class="flex items-center gap-2">
              <span class="w-2 h-2 rounded-full bg-[#CC8033] animate-pulse"></span>
              <span class="text-[#CC8033]">Lệch: <b>{{ diffRows }}</b></span>
            </div>
          </div>
        </div>

        <div class="overflow-x-auto overflow-y-auto max-h-[60vh] custom-scrollbar relative">
          <table class="w-full text-sm text-left">
            <thead class="sticky top-0 z-10 shadow-sm">
              <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
                <th class="px-5 py-4 font-bold">Tên nguyên liệu</th>
                <th class="px-5 py-4 font-bold text-center w-36">Hạn sử dụng</th>
                <th class="px-5 py-4 font-bold text-center w-24">Đơn vị</th>
                <th class="px-5 py-4 font-bold text-right w-36">Tồn hệ thống</th>
                <th class="px-5 py-4 font-bold text-right w-44">Tồn thực tế</th>
                <th class="px-5 py-4 font-bold text-right w-36">Chênh lệch</th>
                <th class="px-5 py-4 font-bold w-56">Ghi chú</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#EAE3D9]/60">
              <tr v-if="filteredRows.length === 0">
                <td colspan="7" class="px-5 py-12 text-center text-[#8A8178]">
                  <PackageOpen class="w-8 h-8 mx-auto opacity-50 mb-2" />
                  Không tìm thấy nguyên liệu nào phù hợp với bộ lọc.
                </td>
              </tr>
              <tr v-for="row in filteredRows" :key="row.id" class="transition-colors group" :class="row.actual !== undefined ? 'bg-white' : 'bg-[#FDFBF7]/50 hover:bg-white'">
                <td class="px-5 py-3">
                  <div class="flex items-center gap-2">
                    <div class="w-1.5 h-6 rounded-full transition-colors" :class="row.actual === undefined ? 'bg-[#EAE3D9]' : (diff(row) !== 0 ? 'bg-[#CC8033]' : 'bg-[#4A7C59]')"></div>
                    <div>
                      <p class="font-bold text-[#2A231E] group-hover:text-[#CC8033] transition-colors">{{ row.name }}</p>
                      <p class="text-[10px] text-[#8A8178] font-mono mt-0.5">SKU: {{ row.sku || 'N/A' }}</p>
                    </div>
                  </div>
                </td>
                <td class="px-5 py-3 text-center">
                  <div class="flex flex-col items-center justify-center">
                    <span class="text-xs font-semibold" :class="isExpired(row.expiryDate) && row.system > 0 ? 'text-red-500 font-bold' : 'text-[#5C544E]'">
                      {{ formatExpiry(row.expiryDate) }}
                    </span>
                    <span v-if="isExpired(row.expiryDate) && row.system > 0" class="text-[9px] font-black text-red-500 uppercase bg-red-50 border border-red-200 px-1.5 py-0.5 rounded mt-1 animate-pulse">
                      Hết Hạn!
                    </span>
                  </div>
                </td>
                <td class="px-5 py-3 text-center text-[11px] font-bold text-[#8A8178] uppercase tracking-wider bg-[#FDFBF7]/30 border-x border-[#EAE3D9]/30">{{ row.unit }}</td>
                <td class="px-5 py-3 text-right font-mono font-medium text-[#5C544E]">{{ formatNumber(row.system) }}</td>
                <td class="px-5 py-3 bg-[#FDFBF7]/30">
                  <input :disabled="isExpired(row.expiryDate) && row.system > 0" type="number" min="0" v-model.number="row.actual" @input="row.actual = (row.actual && row.actual < 0) ? 0 : row.actual" placeholder="Nhập SL..." class="w-full text-right bg-white border border-[#EAE3D9] hover:border-[#CC8033] h-10 rounded-lg px-3 text-sm font-bold text-[#2A231E] focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033] transition-all shadow-inner disabled:bg-red-50/10 disabled:opacity-75 disabled:cursor-not-allowed" />
                </td>
                <td class="px-5 py-3 text-right">
                  <div v-if="row.actual !== undefined" class="inline-flex items-center justify-center font-bold px-3 py-1.5 rounded-lg text-xs font-mono min-w-[4rem]" :class="diffClass(row)">
                    {{ diff(row) > 0 ? '+' : '' }}{{ formatNumber(diff(row)) }}
                  </div>
                  <div v-else class="text-[#D5C9B3] text-xs font-medium">—</div>
                </td>
                <td class="px-5 py-3">
                  <div class="relative flex items-center">
                    <input :disabled="isExpired(row.expiryDate) && row.system > 0" list="reason-options" v-model="row.note" placeholder="Lý do..." class="w-full bg-transparent border-b border-transparent hover:border-[#EAE3D9] focus:border-[#CC8033] h-9 px-2 pr-14 text-xs font-medium focus:outline-none transition-all disabled:opacity-75 disabled:cursor-not-allowed" :class="row.actual !== undefined && diff(row) !== 0 && !row.note.trim() ? 'border-red-300 hover:border-red-400 focus:border-red-500 bg-red-50/20 placeholder-red-400' : ''" />
                    <span v-if="row.actual !== undefined && diff(row) !== 0 && !row.note.trim()" class="absolute right-2 text-[9px] font-black text-red-500 uppercase tracking-wide">Bắt buộc</span>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <datalist id="reason-options">
            <option value="Hao hụt tự nhiên"></option>
            <option value="Hàng hỏng / Đổ vỡ"></option>
            <option value="Hết hạn sử dụng"></option>
            <option value="Sai sót kiểm đếm đợt trước"></option>
            <option value="Sử dụng nội bộ / Test món"></option>
          </datalist>
        </div>
        
        <!-- Sticky Bottom Action Bar -->
        <div class="px-6 py-4 border-t border-[#EAE3D9] bg-white flex flex-wrap items-center justify-between gap-3 sticky bottom-0 z-20 shadow-[0_-4px_10px_rgba(0,0,0,0.02)]">
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-full bg-[#FDFBF7] border border-[#EAE3D9] flex items-center justify-center">
              <PieChart class="w-5 h-5 text-[#CC8033]" />
            </div>
            <div>
              <p class="text-xs font-bold text-[#8A8178] uppercase tracking-wider mb-0.5">Tiến độ kiểm kê</p>
              <div class="flex items-center gap-2">
                <div class="w-32 h-2 bg-[#EAE3D9] rounded-full overflow-hidden">
                  <div class="h-full bg-gradient-to-r from-[#CC8033] to-[#B87029]" :style="`width: ${rows.length ? Math.round((countedRows / rows.length) * 100) : 0}%`"></div>
                </div>
                <span class="text-sm font-black text-[#2A231E]">{{ rows.length ? Math.round((countedRows / rows.length) * 100) : 0 }}%</span>
              </div>
            </div>
          </div>
          <div class="flex gap-3">
            <button @click="saveDraft" class="px-6 py-2.5 rounded-xl border border-[#EAE3D9] text-[#5C544E] text-xs font-bold uppercase tracking-wider hover:bg-[#FDFBF7] transition-colors bg-white shadow-sm flex items-center gap-2">
              <Save class="w-4 h-4" /> Lưu bản nháp
            </button>
            <button @click="submitRequest" :disabled="diffRows === 0" class="px-6 py-2.5 rounded-xl bg-gradient-to-r from-[#2A231E] to-[#3D332A] hover:from-black hover:to-[#2A231E] text-[#CC8033] text-xs font-bold uppercase tracking-wider shadow-lg transition-all flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed">
              <Send class="w-4 h-4" /> Gửi duyệt chênh lệch ({{ diffRows }})
            </button>
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
            <span v-if="req.ghiChu" class="text-xs text-[#8A8178] italic hidden sm:inline-block">📝 {{ req.ghiChu }}</span>
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
        <div class="p-4 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-2.5">
          <div v-for="(it, i) in req.chiTiets" :key="i" class="flex flex-col justify-between bg-[#FDFBF7] border border-[#EAE3D9] rounded-xl p-3 gap-1.5 shadow-sm">
            <div class="flex items-center justify-between">
              <span class="text-xs font-bold text-[#2A231E] truncate" :title="it.tenNguyenLieu">{{ it.tenNguyenLieu }}</span>
              <span class="text-xs font-mono font-bold shrink-0 ml-2" :class="it.chenhLech > 0 ? 'text-[#4A7C59]' : 'text-red-500'">{{ it.chenhLech > 0 ? '+' : '' }}{{ formatNumber(it.chenhLech) }}</span>
            </div>
            <div v-if="it.lyDoLech" class="text-[11px] text-[#CC8033] font-medium flex items-center gap-1.5 bg-[#FFF9F2] px-2.5 py-1 rounded-lg border border-[#CC8033]/25 mt-0.5">
              <span class="font-bold shrink-0">📌 Lý do:</span>
              <span class="italic truncate" :title="it.lyDoLech">{{ it.lyDoLech }}</span>
            </div>
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
import { ref, computed, onMounted } from 'vue'
import {
  Truck, ClipboardList, ChevronRight, Package, ClipboardCheck, History,
  CheckCircle2, Clock, XCircle, Search, PieChart, Save, Send, PackageOpen
} from 'lucide-vue-next'

// ── Types ───────────────────────────────────────────────
interface Row { id: number; name: string; sku: string | null; unit: string; system: number; actual: number | undefined; note: string; expiryDate: string | null }

import { materialsApi } from '@/services/materials'
import { stockTakeApi, type StockTakeListItem, type StockTakeDetail } from '@/services/stockTakes'
import { useAuthStore } from '@/stores/auth'

const loading = ref(false)
const rows = ref<Row[]>([])
const authStore = useAuthStore()
const staff = computed(() => [authStore.user?.hoTen || 'Quản trị viên'])
const activeTab = ref<'create' | 'history'>('create')
const takeDate = ref(new Date().toISOString().slice(0, 10))
const takeBy = ref(staff.value[0]!)

// Filters & Search
const searchQuery = ref('')
const filterStatus = ref('all')

const filteredRows = computed(() => {
  return rows.value.filter(r => {
    // Search
    if (searchQuery.value) {
      const q = searchQuery.value.toLowerCase()
      const matchName = r.name.toLowerCase().includes(q)
      const matchSku = r.sku?.toLowerCase().includes(q)
      if (!matchName && !matchSku) return false
    }
    // Filter
    if (filterStatus.value === 'uncounted') return r.actual === undefined
    if (filterStatus.value === 'counted') return r.actual !== undefined
    if (filterStatus.value === 'diff') return r.actual !== undefined && diff(r) !== 0
    return true
  })
})

const fetchMaterials = async () => {
  try {
    const list = await materialsApi.list()
    rows.value = list.map(m => {
      const expired = isExpired(m.ngayHetHan) && m.soLuongTon > 0
      return {
        id: m.maNguyenLieu,
        name: m.tenNguyenLieu,
        sku: m.maVach_SKU,
        unit: m.donViTinh,
        system: m.soLuongTon,
        actual: expired ? 0 : undefined,
        note: expired ? 'Hết hạn sử dụng' : '',
        expiryDate: m.ngayHetHan
      }
    })
  } catch (err) {
    toast('Lỗi khi tải danh sách nguyên liệu')
  }
}

onMounted(async () => {
  await fetchMaterials()
  fetchRequests()
  checkDraft()
})

interface DraftItem { maNguyenLieu: number; actual: number; note: string }
interface DraftData { time: string; items: DraftItem[] }
const draftData = ref<DraftData | null>(null)

const checkDraft = () => {
  const stored = localStorage.getItem('stocktake_draft')
  if (stored) {
    try {
      draftData.value = JSON.parse(stored)
    } catch {
      draftData.value = null
    }
  } else {
    draftData.value = null
  }
}

// ── Diff helpers ────────────────────────────────────────
const formatNumber = (n: number) => (n || 0).toLocaleString('vi-VN')
const isExpired = (expiryDate: string | null) => {
  if (!expiryDate) return false
  const today = new Date()
  today.setHours(0,0,0,0)
  const exp = new Date(expiryDate)
  exp.setHours(0,0,0,0)
  return exp < today
}
const formatExpiry = (dStr: string | null) => {
  if (!dStr) return 'Không có hạn'
  return new Date(dStr).toLocaleDateString('vi-VN')
}
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
const saveDraft = () => {
  const counted = rows.value.filter(r => r.actual !== undefined)
  if (counted.length === 0) {
    toast('Chưa có mặt hàng nào được kiểm đếm để lưu nháp')
    return
  }
  const draft: DraftData = {
    time: new Date().toLocaleString('vi-VN'),
    items: counted.map(r => ({
      maNguyenLieu: r.id,
      actual: r.actual!,
      note: r.note
    }))
  }
  localStorage.setItem('stocktake_draft', JSON.stringify(draft))
  toast('Đã lưu bản nháp thành công')
  checkDraft()
}

const loadDraft = async () => {
  if (!draftData.value) return
  if (rows.value.length === 0) {
    await fetchMaterials()
  }
  let loadedCount = 0
  draftData.value.items.forEach(dItem => {
    const row = rows.value.find(r => String(r.id) === String(dItem.maNguyenLieu))
    if (row) {
      row.actual = dItem.actual
      row.note = dItem.note || ''
      loadedCount++
    }
  })
  if (loadedCount > 0) {
    toast(`✨ Đã tải thành công ${loadedCount} mặt hàng từ bản nháp!`)
  } else {
    toast('Không tìm thấy dữ liệu khớp trong bản nháp')
  }
  checkDraft()
}

const clearDraft = () => {
  localStorage.removeItem('stocktake_draft')
  draftData.value = null
  toast('Đã xóa bản nháp')
}

const submitRequest = async () => {
  const hasNegative = rows.value.some(r => r.actual !== undefined && r.actual < 0)
  if (hasNegative) {
    toast('Số lượng thực tế không được phép là số âm')
    return
  }

  const missingReason = rows.value.some(r => r.actual !== undefined && diff(r) !== 0 && !r.note.trim())
  if (missingReason) {
    toast('Bắt buộc nhập Lý do cho tất cả các mặt hàng lệch tồn kho')
    return
  }

  const changed = rows.value.filter(r => r.actual !== undefined && diff(r) !== 0)
  if (changed.length === 0) {
    toast('Chưa có chênh lệch nào để gửi')
    return
  }
  
  loading.value = true
  try {
    await stockTakeApi.create({
      ghiChu: `Kiểm kê định kỳ - Thực hiện bởi ${takeBy.value}`,
      chiTiets: changed.map(r => ({
        maNguyenLieu: r.id,
        soLuongThucTe: r.actual!,
        lyDoLech: r.note || null
      }))
    })
    
    // reset form
    rows.value.forEach(r => {
      const expired = isExpired(r.expiryDate) && r.system > 0
      r.actual = expired ? 0 : undefined
      r.note = expired ? 'Hết hạn sử dụng' : ''
    })
    activeTab.value = 'history'
    
    // Clear draft on successful submit
    localStorage.removeItem('stocktake_draft')
    draftData.value = null

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
    await fetchMaterials() // Update stock numbers
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
