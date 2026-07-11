<template>
  <div class="space-y-6 font-premium-sans text-[#2A231E]">

    <!-- Header -->
    <div class="flex items-start justify-between gap-4">
      <div>
        <h2 class="text-3xl font-premium-serif font-bold text-[#2A231E]">Nhật ký Hệ thống & Lưu vết</h2>
        <p class="text-[10px] uppercase tracking-[0.2em] text-[#8A8178] font-bold mt-2">Theo dõi mọi hoạt động và thay đổi trong hệ thống</p>
      </div>
      <div class="flex items-center gap-2">
        <button
          @click="fetchLogs(); fetchStats();"
          class="w-10 h-10 rounded-full bg-white hover:bg-[#FAF6F0] border border-[#EAE3D9] flex items-center justify-center transition-colors text-espresso hover:text-caramel shadow-sm active:scale-95"
          title="Tải lại nhật ký"
        >
          <RefreshCw :class="['w-4 h-4', isLoading ? 'animate-spin' : '']" stroke-width="2.5" />
        </button>
        <button
          @click="showConfirmClear = true"
          class="shrink-0 w-10 h-10 rounded-full bg-red-50 hover:bg-red-100 border border-red-100 flex items-center justify-center transition-colors text-red-500 hover:text-red-600 shadow-sm active:scale-95"
          title="Xóa nhật ký (Chỉ Admin)"
        >
          <Trash2 class="w-4 h-4" stroke-width="2.5" />
        </button>
      </div>
    </div>

    <!-- Stats Row -->
    <div class="grid grid-cols-2 gap-4">
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-5 flex items-center gap-4">
        <div class="w-10 h-10 shrink-0 rounded-full bg-[#F9F8F6] border border-[#EAE3D9] text-[#5C544E] flex items-center justify-center">
          <Activity class="w-4 h-4" stroke-width="2.5" />
        </div>
        <div>
          <p class="text-[10px] uppercase tracking-[0.15em] text-[#8A8178] font-bold">Tổng bản ghi</p>
          <p class="text-2xl font-premium-serif font-bold text-[#2A231E] mt-0.5">{{ totalCount }}</p>
        </div>
      </div>
      <div class="bg-white rounded-xl border border-amber-100 shadow-sm p-5 flex items-center gap-4">
        <div class="w-10 h-10 shrink-0 rounded-full bg-amber-50 border border-amber-100 text-amber-500 flex items-center justify-center">
          <AlertTriangle class="w-4 h-4" stroke-width="2.5" />
        </div>
        <div>
          <p class="text-[10px] uppercase tracking-[0.15em] text-[#CC8033] font-bold">Cảnh báo</p>
          <p class="text-2xl font-premium-serif font-bold text-[#CC8033] mt-0.5">{{ warningCount }}</p>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-6">
      <h3 class="font-premium-serif text-lg font-bold text-[#2A231E] mb-5">Bộ lọc tìm kiếm</h3>
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div class="space-y-2">
          <label class="block text-[10px] font-bold text-[#8A8178] uppercase tracking-[0.15em]">Tìm kiếm nhanh</label>
          <div class="relative">
            <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-[#C5BEB8]" stroke-width="2.5" />
            <input
              type="text"
              v-model="searchQuery"
              placeholder="Tên tài khoản, IP, hành động..."
              class="w-full pl-9 pr-3 py-2.5 bg-white border border-[#EAE3D9] rounded-lg text-sm text-[#2A231E] font-medium focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033]/20 transition-colors"
            />
          </div>
        </div>
        <div class="space-y-2">
          <label class="block text-[10px] font-bold text-[#8A8178] uppercase tracking-[0.15em]">Phân hệ</label>
          <div class="relative">
            <select v-model="selectedModule" class="w-full pl-3 pr-10 py-2.5 bg-white border border-[#EAE3D9] rounded-lg text-sm text-[#2A231E] font-semibold focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033]/20 transition-colors appearance-none">
              <option>Tất cả</option>
              <option v-for="m in modules" :key="m" :value="m">{{ m }}</option>
            </select>
            <ChevronDown class="w-4 h-4 absolute right-3 top-1/2 -translate-y-1/2 text-[#8A8178] pointer-events-none" stroke-width="2.5" />
          </div>
        </div>
        <div class="space-y-2">
          <label class="block text-[10px] font-bold text-[#8A8178] uppercase tracking-[0.15em]">Mức độ</label>
          <div class="relative">
            <select v-model="selectedLevel" class="w-full pl-3 pr-10 py-2.5 bg-white border border-[#EAE3D9] rounded-lg text-sm text-[#2A231E] font-semibold focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033]/20 transition-colors appearance-none">
              <option>Tất cả</option>
              <option>Bình thường</option>
              <option>Cảnh báo</option>
              <option>Nghiêm trọng</option>
            </select>
            <ChevronDown class="w-4 h-4 absolute right-3 top-1/2 -translate-y-1/2 text-[#8A8178] pointer-events-none" stroke-width="2.5" />
          </div>
        </div>
      </div>
    </div>

    <!-- Table -->
    <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-[#F9F8F6] text-[#8A8178] text-[10px] uppercase tracking-[0.15em] border-b border-[#EAE3D9]">
              <th class="px-6 py-5 font-bold whitespace-nowrap">Thời gian</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Tài khoản</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Phân hệ</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Hành động</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Thiết bị/IP</th>
              <th class="px-6 py-5 font-bold">Chi tiết thay đổi</th>
              <th class="px-6 py-5 font-bold text-center">Xem</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="isLoading" class="border-b border-[#EAE3D9]/60">
              <td colspan="7" class="text-center py-16 text-muted-foreground font-medium">Đang tải nhật ký...</td>
            </tr>
            <tr v-else-if="filteredLogs.length === 0" class="border-b border-[#EAE3D9]/60">
              <td colspan="7" class="text-center py-16 text-muted-foreground font-medium">Không tìm thấy nhật ký nào hợp lệ.</td>
            </tr>
            <tr
              v-else
              v-for="log in filteredLogs"
              :key="log.maNhatKy"
              class="border-b border-[#EAE3D9]/60 hover:bg-[#F5F2ED] transition-colors"
            >
              <td class="px-6 py-5 text-[#8A8178] font-bold text-xs font-mono whitespace-nowrap">{{ formatDate(log.thoiGianTao) }}</td>
              <td class="px-6 py-5 font-bold text-[#2A231E] whitespace-nowrap">
                <span class="block">{{ log.tenNhanVien || 'Khách / Hệ thống' }}</span>
                <span v-if="log.maNhanVien" class="text-[9px] text-[#8A8178] font-semibold">Mã NV: {{ log.maNhanVien }}</span>
              </td>
              <td class="px-6 py-5">
                <span class="inline-block px-2.5 py-1.5 bg-[#F5F2ED] text-[#8A6D53] border border-[#EAE3D9] rounded-md text-[10px] font-bold uppercase tracking-widest whitespace-nowrap">
                  {{ log.module }}
                </span>
              </td>
              <td class="px-6 py-5">
                <span :class="['inline-block px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider rounded-md', getLogLevel(log).class]">
                  {{ log.hanhDong }}
                </span>
              </td>
              <td class="px-6 py-5 text-[#8A8178] font-mono text-[11px] font-semibold whitespace-nowrap">
                <span class="block">{{ log.diaChiIP || 'Localhost' }}</span>
                <span v-if="log.thietBi" class="block text-[9px] text-[#A09890] mt-0.5 max-w-[150px] truncate" :title="log.thietBi">{{ log.thietBi }}</span>
              </td>
              <td class="px-6 py-5 text-[#5C544E] font-medium leading-relaxed max-w-sm truncate" :title="formatDetails(log)">
                <div class="text-xs">{{ formatDetails(log) }}</div>
              </td>
              <td class="px-6 py-5 text-center whitespace-nowrap">
                <button
                  @click="openDetails(log)"
                  class="w-8 h-8 rounded-lg bg-[#FAF6F0] hover:bg-cream border border-[#EAE3D9] flex items-center justify-center transition-colors text-[#5C544E] hover:text-[#CC8033] shadow-xs active:scale-95 mx-auto"
                  title="Xem chi tiết nhật ký"
                >
                  <Eye class="w-4 h-4" stroke-width="2.5" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="totalPages > 1" class="flex items-center justify-between p-4 border border-[#EAE3D9] bg-white rounded-xl shadow-sm">
      <div class="text-xs text-[#8A8178] font-semibold">
        Hiển thị <span class="text-[#2A231E]">{{ (currentPage - 1) * pageSize + 1 }}</span> - 
        <span class="text-[#2A231E]">{{ Math.min(currentPage * pageSize, totalCount) }}</span> trong số 
        <span class="text-[#2A231E]">{{ totalCount }}</span> nhật ký
      </div>
      <div class="flex items-center gap-2">
        <button 
          @click="currentPage--" 
          :disabled="currentPage === 1"
          class="p-1.5 rounded-lg border border-[#EAE3D9] text-[#2A231E] disabled:opacity-50 disabled:cursor-not-allowed hover:bg-[#FAF6F0]"
        >
          <ChevronLeft class="w-4 h-4" />
        </button>
        <span class="text-xs font-bold text-[#2A231E]">Trang {{ currentPage }} / {{ totalPages }}</span>
        <button 
          @click="currentPage++" 
          :disabled="currentPage === totalPages"
          class="p-1.5 rounded-lg border border-[#EAE3D9] text-[#2A231E] disabled:opacity-50 disabled:cursor-not-allowed hover:bg-[#FAF6F0]"
        >
          <ChevronRight class="w-4 h-4" />
        </button>
      </div>
    </div>

    <!-- Custom Clear Confirmation Modal -->
    <div v-if="showConfirmClear" class="fixed inset-0 bg-black/40 backdrop-blur-xs flex items-center justify-center z-[100] p-4">
      <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-2xl p-6 max-w-sm w-full space-y-4 text-center">
        <div class="w-12 h-12 bg-red-50 text-red-500 rounded-full flex items-center justify-center mx-auto border border-red-100 animate-pulse">
          <Trash2 class="w-5 h-5" stroke-width="2.5" />
        </div>
        
        <div class="space-y-1">
          <h3 class="font-display text-lg text-[#2A231E] font-bold">Xóa nhật ký hệ thống?</h3>
          <p class="text-xs text-[#8A8178] leading-relaxed font-semibold">
            Bạn có chắc chắn muốn xóa toàn bộ lịch sử lưu vết? Hành động này **không thể hoàn tác**.
          </p>
        </div>

        <div class="flex items-center gap-2.5 pt-2">
          <button 
            @click="showConfirmClear = false" 
            class="flex-1 h-10 text-xs font-bold rounded-lg text-espresso border border-[#EAE3D9] hover:bg-[#FAF6F0] transition-colors"
          >
            Hủy bỏ
          </button>
          <button 
            @click="confirmClearLogs" 
            class="flex-1 h-10 text-xs font-bold rounded-lg bg-red-600 hover:bg-red-700 text-white transition-colors"
          >
            Xác nhận xóa
          </button>
        </div>
      </div>
    </div>
    <!-- Custom Details Modal -->
    <div v-if="showDetailsModal && selectedLog" class="fixed inset-0 bg-black/40 backdrop-blur-xs flex items-center justify-center z-[100] p-4">
      <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-2xl p-6 max-w-lg w-full space-y-4">
        <div class="flex items-center justify-between border-b border-[#EAE3D9] pb-3">
          <h3 class="font-display text-base text-[#2A231E] font-bold">Chi tiết nhật ký hệ thống</h3>
          <button @click="closeDetails" class="text-[#8A8178] hover:text-[#CC8033] text-lg font-bold">×</button>
        </div>

        <div class="space-y-3.5 text-xs text-[#2A231E]">
          <div class="grid grid-cols-3 gap-2">
            <span class="text-[#8A8178] font-bold">Thời gian:</span>
            <span class="col-span-2 font-semibold text-[#5C544E]">{{ formatDate(selectedLog.thoiGianTao) }}</span>
          </div>
          <div class="grid grid-cols-3 gap-2">
            <span class="text-[#8A8178] font-bold">Tài khoản:</span>
            <span class="col-span-2 font-bold">{{ selectedLog.tenNhanVien || 'Hệ thống / Khách' }} <span v-if="selectedLog.maNhanVien" class="text-[10px] text-[#8A8178]">(Mã NV: {{ selectedLog.maNhanVien }})</span></span>
          </div>
          <div class="grid grid-cols-3 gap-2">
            <span class="text-[#8A8178] font-bold">Phân hệ:</span>
            <span class="col-span-2 font-semibold">
              <span class="px-2 py-0.5 bg-[#F5F2ED] text-[#8A6D53] border border-[#EAE3D9] rounded text-[10px] font-bold uppercase tracking-wider">{{ selectedLog.module }}</span>
            </span>
          </div>
          <div class="grid grid-cols-3 gap-2">
            <span class="text-[#8A8178] font-bold">Hành động:</span>
            <span class="col-span-2 font-semibold">
              <span :class="['px-2 py-0.5 text-[10px] font-bold uppercase tracking-wider rounded', getLogLevel(selectedLog).class]">{{ selectedLog.hanhDong }}</span>
            </span>
          </div>
          <div class="grid grid-cols-3 gap-2">
            <span class="text-[#8A8178] font-bold">Địa chỉ IP:</span>
            <span class="col-span-2 font-mono font-semibold text-[#5C544E]">{{ selectedLog.diaChiIP || 'Localhost' }}</span>
          </div>
          <div class="grid grid-cols-3 gap-2">
            <span class="text-[#8A8178] font-bold">Thiết bị:</span>
            <span class="col-span-2 text-[11px] text-[#5C544E] leading-relaxed break-all font-medium">{{ selectedLog.thietBi || 'N/A' }}</span>
          </div>

          <div class="border-t border-[#EAE3D9] pt-3.5 space-y-2">
            <div class="space-y-1">
              <span class="text-[#8A8178] font-bold block">Dữ liệu trước thay đổi:</span>
              <div class="bg-[#FBF9F6] p-3 rounded-lg border border-[#F0EAE1] font-semibold text-[11px] text-[#5C544E] leading-relaxed break-all max-h-32 overflow-y-auto">
                {{ selectedLog.duLieuCu || '(Không có)' }}
              </div>
            </div>
            <div class="space-y-1">
              <span class="text-[#8A8178] font-bold block">Dữ liệu sau thay đổi:</span>
              <div class="bg-[#FBF9F6] p-3 rounded-lg border border-[#F0EAE1] font-semibold text-[11px] text-[#2A231E] leading-relaxed break-all max-h-32 overflow-y-auto">
                {{ selectedLog.duLieuMoi || '(Không có)' }}
              </div>
            </div>
          </div>
        </div>

        <div class="pt-2">
          <button 
            @click="closeDetails" 
            class="w-full h-10 text-xs font-bold rounded-lg text-[#2A231E] border border-[#EAE3D9] hover:bg-[#FAF6F0] transition-colors"
          >
            Đóng cửa sổ
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { Trash2, Search, Activity, AlertTriangle, ChevronLeft, ChevronRight, RefreshCw, Eye, ChevronDown } from 'lucide-vue-next'
import { auditLogsApi, type AuditLogItem } from '@/services/auditLogs'
import { useToast } from '@/stores/toast'

const toast = useToast()
const logs = ref<AuditLogItem[]>([])
const modules = ref<string[]>([])

const selectedLog = ref<AuditLogItem | null>(null)
const showDetailsModal = ref(false)

function openDetails(log: AuditLogItem) {
  selectedLog.value = log
  showDetailsModal.value = true
}

function closeDetails() {
  showDetailsModal.value = false
  selectedLog.value = null
}

// Query parameters
const searchQuery = ref('')
const selectedModule = ref('Tất cả')
const selectedLevel = ref('Tất cả') // Tất cả, Bình thường, Cảnh báo, Nghiêm trọng

// Pagination
const currentPage = ref(1)
const pageSize = ref(15)
const totalCount = ref(0)
const totalPages = ref(1)
const isLoading = ref(false)

async function fetchModules() {
  try {
    const res = await auditLogsApi.getModules()
    modules.value = res
  } catch (e) {
    console.error('Không thể tải danh sách phân hệ:', e)
  }
}

async function fetchLogs() {
  isLoading.value = true
  try {
    const res = await auditLogsApi.getPaged({
      module: selectedModule.value === 'Tất cả' ? undefined : selectedModule.value,
      search: searchQuery.value || undefined,
      page: currentPage.value,
      pageSize: pageSize.value
    })
    logs.value = res.data
    totalCount.value = res.total
    totalPages.value = res.totalPages
  } catch (e: any) {
    toast.error(e.message || 'Lỗi khi tải nhật ký hệ thống')
  } finally {
    isLoading.value = false
  }
}

// Client-side severity classification
function getLogLevel(log: AuditLogItem) {
  const act = log.hanhDong.toLowerCase()
  if (act.includes('xóa') || act.includes('delete') || act.includes('hủy') || act.includes('clear')) {
    return { label: 'Nghiêm trọng', class: 'bg-red-50 text-red-600 border border-red-200' }
  }
  if (act.includes('sửa') || act.includes('update') || act.includes('chỉnh sửa') || act.includes('thay đổi') || act.includes('edit')) {
    return { label: 'Cảnh báo', class: 'bg-amber-50 text-amber-600 border border-amber-200' }
  }
  return { label: 'Bình thường', class: 'bg-green-50 text-green-600 border border-green-200' }
}

// Client-side filter for level
const filteredLogs = computed(() => {
  if (selectedLevel.value === 'Tất cả') return logs.value

  return logs.value.filter(log => {
    const levelObj = getLogLevel(log)
    return levelObj.label === selectedLevel.value
  })
})

// Stats
const warningCount = ref(0)
async function fetchStats() {
  try {
    const res = await auditLogsApi.getPaged({ pageSize: 100 })
    warningCount.value = res.data.filter(log => {
      const lvl = log.hanhDong.toLowerCase()
      return lvl.includes('xóa') || lvl.includes('delete') || lvl.includes('hủy') || lvl.includes('sửa') || lvl.includes('update') || lvl.includes('chỉnh sửa') || lvl.includes('clear')
    }).length
  } catch {
    warningCount.value = 0
  }
}

// Watch inputs
watch([selectedModule, selectedLevel], () => {
  currentPage.value = 1
  fetchLogs()
})

// Debounce search query to prevent excessive backend requests
let debounceTimer: any = null
watch(searchQuery, () => {
  currentPage.value = 1
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    fetchLogs()
  }, 350)
})

watch(currentPage, () => {
  fetchLogs()
})

onMounted(() => {
  fetchModules()
  fetchLogs()
  fetchStats()
})

const showConfirmClear = ref(false)

async function confirmClearLogs() {
  showConfirmClear.value = false
  try {
    await auditLogsApi.clearLogs()
    toast.success('Đã xóa toàn bộ nhật ký hệ thống')
    logs.value = []
    totalCount.value = 0
    warningCount.value = 0
    totalPages.value = 1
  } catch (e: any) {
    toast.error(e.message || 'Lỗi khi xóa nhật ký')
  }
}

function formatDate(dateStr: string) {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  return d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' }) + ' ' + d.toLocaleDateString('vi-VN')
}

function formatDetails(log: AuditLogItem): string {
  if (log.duLieuMoi) {
    return `Mới: ${log.duLieuMoi}`
  }
  if (log.duLieuCu) {
    return `Cũ: ${log.duLieuCu}`
  }
  return `${log.hanhDong} thành công trên phân hệ ${log.module}`
}
</script>

<style scoped>
select {
  appearance: none;
  -webkit-appearance: none;
  -moz-appearance: none;
  background-image: none !important;
}
</style>
