<template>
  <div class="p-6">
    <div class="mb-6">
      <h2 class="font-display text-2xl text-espresso font-bold">Ca làm, Đơn từ & Bảng lương</h2>
      <p class="text-xs text-muted-foreground mt-1">Quản trị toàn bộ lịch phân ca, xét duyệt yêu cầu và tính lương nhân sự.</p>
    </div>

    <!-- Khối Chỉ số thông minh (3 Thẻ Metrics) -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-5 mb-6">
      <div class="bg-card p-5 rounded-xl border border-cream-deep shadow-card flex flex-col justify-center">
        <span class="text-xs text-muted-foreground font-bold uppercase tracking-widest mb-1">Tổng giờ làm toàn quán</span>
        <div class="flex items-baseline gap-2">
          <h3 class="font-sans text-3xl font-bold text-espresso">1,240<span class="text-lg font-normal ml-1">h</span></h3>
          <span class="text-xs font-bold text-success">+45h tăng ca</span>
        </div>
      </div>
      
      <div class="bg-caramel p-5 rounded-xl shadow-warm flex flex-col justify-center relative overflow-hidden">
        <div class="absolute -right-4 -bottom-4 opacity-10 pointer-events-none">
          <DollarSign class="w-32 h-32 text-white" />
        </div>
        <span class="text-xs text-white/80 font-bold uppercase tracking-widest mb-1 relative z-10">Quỹ lương tháng này</span>
        <div class="flex items-baseline gap-2 relative z-10">
          <h3 class="font-sans text-3xl font-bold text-white">35,400,000<span class="text-lg font-normal">đ</span></h3>
          <span class="px-2 py-0.5 bg-white/20 rounded text-[10px] text-white font-bold backdrop-blur-sm">Tạm tính</span>
        </div>
      </div>

      <div class="bg-card p-5 rounded-xl border border-cream-deep shadow-card flex flex-col justify-center">
        <span class="text-xs text-muted-foreground font-bold uppercase tracking-widest mb-1">Số ngày phép đã dùng</span>
        <div class="flex items-baseline gap-2">
          <h3 class="font-sans text-3xl font-bold text-espresso">8<span class="text-lg font-normal text-muted-foreground ml-1">ngày</span></h3>
        </div>
      </div>
    </div>

    <!-- Main Tabs -->
    <div class="flex flex-col md:flex-row md:items-center justify-between border-b-2 border-cream-deep mb-6 gap-4 md:gap-0">
      <div class="flex items-center gap-6 overflow-x-auto overflow-y-hidden pb-1">
        <button 
          @click="activeTab = 'shifts'" 
          class="pb-3 text-sm font-bold transition-colors relative whitespace-nowrap"
          :class="activeTab === 'shifts' ? 'text-espresso' : 'text-muted-foreground hover:text-espresso'"
        >
          Lịch ca tuần này
          <div v-if="activeTab === 'shifts'" class="absolute bottom-[-2px] left-0 w-full h-0.5 bg-espresso rounded-t-full"></div>
        </button>
        <button 
          @click="activeTab = 'requests'" 
          class="pb-3 text-sm font-bold transition-colors relative whitespace-nowrap"
          :class="activeTab === 'requests' ? 'text-espresso' : 'text-muted-foreground hover:text-espresso'"
        >
          Duyệt đơn từ nhân viên
          <span class="ml-1 px-1.5 py-0.5 rounded text-[10px] font-bold bg-caramel/20 text-caramel">{{ pendingRequests.length }}</span>
          <div v-if="activeTab === 'requests'" class="absolute bottom-[-2px] left-0 w-full h-0.5 bg-espresso rounded-t-full"></div>
        </button>
        <button 
          @click="activeTab = 'payroll'" 
          class="pb-3 text-sm font-bold transition-colors relative whitespace-nowrap"
          :class="activeTab === 'payroll' ? 'text-caramel' : 'text-muted-foreground hover:text-espresso'"
        >
          Xét lương & Tính toán
          <div v-if="activeTab === 'payroll'" class="absolute bottom-[-2px] left-0 w-full h-0.5 bg-caramel rounded-t-full"></div>
        </button>
      </div>
      
      <!-- Actions -->
      <div v-if="activeTab === 'shifts'" class="pb-2 flex gap-3">
        <Button variant="outline" size="sm" class="border border-cream-deep rounded-lg shadow-card text-espresso hover:bg-cream-deep">
          <Download class="w-3.5 h-3.5 mr-1" /> Excel
        </Button>
        <Button @click="openShiftModal()" size="sm" class="bg-espresso text-cream rounded-lg border border-espresso/30 shadow-card hover:bg-brown">
          <Plus class="w-3.5 h-3.5 mr-1" /> Xếp ca
        </Button>
      </div>
      <div v-if="activeTab === 'payroll'" class="pb-2">
        <Button variant="outline" size="sm" class="border border-caramel/30 text-caramel bg-white hover:bg-caramel/5 rounded-lg shadow-sm">
          <Download class="w-3.5 h-3.5 mr-1" /> Xuất Excel
        </Button>
      </div>
    </div>

    <!-- Tab 1: Lịch ca -->
    <div v-if="activeTab === 'shifts'" class="animate-in fade-in duration-300">
      <div class="grid grid-cols-1 xl:grid-cols-[1fr_280px] gap-5">
        <div class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden">
          <div class="grid grid-cols-7 gap-px bg-cream-deep">
            <!-- Day headers -->
            <div v-for="(d, i) in days" :key="d" class="bg-cream/50 p-3 text-center">
              <div class="text-xs text-muted-foreground">{{ d }}</div>
              <div class="font-display text-base text-espresso font-semibold">{{ dates[i] }}</div>
            </div>

            <!-- Cells -->
            <div v-for="(d, dIdx) in days" :key="`cell-${d}`" class="bg-card min-h-[220px] p-3 flex flex-col gap-2 group relative">
              <div v-for="(s, i) in schedule[d] || []" :key="i" class="p-3 rounded-lg border text-xs shadow-card relative z-10" :class="shiftColors[s.shift]">
                <div class="flex items-center gap-2">
                  <div :class="['w-6 h-6 rounded-lg text-cream text-[10px] font-semibold flex items-center justify-center', s.color]">
                    {{ s.initials }}
                  </div>
                  <span class="font-medium text-espresso truncate">{{ s.staff }}</span>
                </div>
                <div class="text-muted-foreground mt-1 text-xs">{{ s.time }}</div>
                <!-- Hiển thị đánh dấu Tăng ca/Nghỉ phép từ logic -->
                <div v-if="s.note" class="mt-1">
                  <span class="px-1.5 py-0.5 rounded text-[9px] font-bold" :class="s.note === 'OT' ? 'bg-purple-200 text-purple-700' : 'bg-blue-200 text-blue-700'">
                    {{ s.note === 'OT' ? '+ OT' : 'Nghỉ' }}
                  </span>
                </div>
              </div>

              <!-- Vùng bấm thêm ca inline -->
              <button 
                @click="openShiftModal(dates[dIdx])"
                class="flex-1 w-full rounded-lg border-2 border-dashed border-transparent group-hover:border-cream-deep hover:bg-cream/30 transition-all flex items-center justify-center opacity-0 group-hover:opacity-100 min-h-[60px]"
              >
                <Plus class="w-4 h-4 text-muted-foreground" />
              </button>
            </div>
          </div>
        </div>

        <aside class="space-y-4">
          <!-- Tổng giờ tuần -->
          <div class="bg-card rounded-lg border border-cream-deep shadow-card p-5">
            <h4 class="font-display text-base text-espresso font-semibold mb-4">Phân bổ giờ</h4>
            <div class="space-y-3">
              <div v-for="[n, h] in totalHours" :key="n" class="flex justify-between text-sm">
                <span class="text-espresso">{{ n }}</span>
                <span class="font-semibold text-caramel">{{ h }}</span>
              </div>
            </div>
          </div>

          <!-- Cảnh báo -->
          <div class="bg-warning/10 border border-warning/30 rounded-lg p-5">
            <div class="flex items-center gap-2 mb-3">
              <AlertCircle class="w-4 h-4 text-warning" />
              <h4 class="font-semibold text-espresso text-sm">Cần bổ sung</h4>
            </div>
            <p class="text-xs text-muted-foreground">Chủ nhật chưa có ca chiều — cần thêm 1 nhân viên.</p>
          </div>
        </aside>
      </div>
    </div>

    <!-- Tab 2: Duyệt Đơn từ -->
    <div v-if="activeTab === 'requests'" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden min-h-[300px] animate-in fade-in duration-300">
      <div class="flex items-center gap-4 px-5 py-4 border-b-2 border-cream-deep bg-cream/30">
        <span class="text-xs font-bold text-espresso uppercase tracking-widest">Bộ lọc:</span>
        <button @click="requestFilter = 'pending'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'pending' ? 'bg-caramel text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Chờ duyệt</button>
        <button @click="requestFilter = 'approved'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'approved' ? 'bg-success/80 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Đã duyệt</button>
        <button @click="requestFilter = 'rejected'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'rejected' ? 'bg-destructive/80 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Đã từ chối</button>
      </div>
      
      <table class="w-full text-sm">
        <thead>
          <tr class="bg-cream/50 text-left text-muted-foreground text-xs uppercase border-b-2 border-cream-deep">
            <th class="px-5 py-4 font-medium w-[20%]">Nhân viên</th>
            <th class="px-5 py-4 font-medium w-[15%]">Loại đơn</th>
            <th class="px-5 py-4 font-medium w-[20%]">Thời gian đăng ký</th>
            <th class="px-5 py-4 font-medium w-[25%]">Lý do</th>
            <th v-if="requestFilter === 'pending'" class="px-5 py-4 font-medium w-[20%] text-right">Thao tác</th>
          </tr>
        </thead>
        <tbody class="relative">
          <TransitionGroup name="list">
            <tr v-for="row in filteredRequests" :key="row.id" class="border-b border-cream-deep/60 hover:bg-cream/20 transition-colors">
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-full bg-cream flex items-center justify-center text-espresso font-bold text-xs border border-cream-deep">
                    {{ row.name.split(' ').slice(-1)[0].charAt(0) }}
                  </div>
                  <span class="text-espresso font-medium">{{ row.name }}</span>
                </div>
              </td>
              <td class="px-5 py-4">
                <span class="px-2.5 py-1 rounded text-[11px] font-bold" :class="typeColors[row.type]">
                  {{ row.type }}
                </span>
              </td>
              <td class="px-5 py-4 text-espresso font-medium text-xs">
                {{ row.time }}
              </td>
              <td class="px-5 py-4 text-muted-foreground text-xs">
                <p class="truncate max-w-[200px]" :title="row.reason">{{ row.reason }}</p>
              </td>
              <td v-if="requestFilter === 'pending'" class="px-5 py-4 text-right">
                <div class="flex justify-end gap-2">
                  <button @click="handleAction(row.id, 'approved')" class="flex items-center gap-1 px-3 py-1.5 rounded-lg bg-success/10 text-success hover:bg-success/20 transition-colors font-bold text-xs">
                    <Check class="w-3.5 h-3.5" /> Duyệt
                  </button>
                  <button @click="handleAction(row.id, 'rejected')" class="flex items-center gap-1 px-3 py-1.5 rounded-lg bg-destructive/10 text-destructive hover:bg-destructive/20 transition-colors font-bold text-xs">
                    <X class="w-3.5 h-3.5" /> Từ chối
                  </button>
                </div>
              </td>
            </tr>
          </TransitionGroup>
          <tr v-if="filteredRequests.length === 0">
            <td :colspan="requestFilter === 'pending' ? 5 : 4" class="py-8 text-center text-muted-foreground text-xs">Không có đơn từ nào.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Tab 3: Bảng lương -->
    <div v-if="activeTab === 'payroll'" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden min-h-[300px] animate-in fade-in duration-300">
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="bg-cream/50 text-left text-muted-foreground text-xs uppercase border-b-2 border-cream-deep">
              <th class="px-5 py-4 font-medium">Nhân viên</th>
              <th class="px-5 py-4 font-medium">Mức lương/Giờ</th>
              <th class="px-5 py-4 font-medium">Giờ thường</th>
              <th class="px-5 py-4 font-medium">Giờ tăng ca</th>
              <th class="px-5 py-4 font-medium">Ngày phép</th>
              <th class="px-5 py-4 font-medium">Thành tiền</th>
              <th class="px-5 py-4 font-medium text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in payroll" :key="row.id" class="border-b border-cream-deep/60 hover:bg-cream/20 transition-colors">
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <img :src="row.avatar" class="w-8 h-8 rounded-full border border-cream-deep object-cover" />
                  <span class="text-espresso font-medium">{{ row.name }}</span>
                </div>
              </td>
              <td class="px-5 py-4">
                <div class="flex items-center gap-2 group cursor-pointer w-fit">
                  <span class="text-espresso font-medium">{{ row.rate.toLocaleString() }}đ</span>
                  <button class="text-muted-foreground opacity-0 group-hover:opacity-100 transition-opacity hover:text-caramel">
                    <Edit3 class="w-3.5 h-3.5" />
                  </button>
                </div>
              </td>
              <td class="px-5 py-4 text-espresso">{{ row.normalHours }}h</td>
              <td class="px-5 py-4 text-espresso">
                <span v-if="row.otHours > 0" class="text-purple-700 font-bold bg-purple-100 px-2 py-0.5 rounded text-xs">{{ row.otHours }}h</span>
                <span v-else class="text-muted-foreground">-</span>
              </td>
              <td class="px-5 py-4">
                <span v-if="row.leaveDays > 0" class="px-2 py-0.5 bg-blue-100 text-blue-700 rounded text-xs font-bold">{{ row.leaveDays }}</span>
                <span v-else class="text-muted-foreground">-</span>
              </td>
              <td class="px-5 py-4">
                <span class="text-caramel font-bold text-base">{{ row.total.toLocaleString() }}đ</span>
              </td>
              <td class="px-5 py-4 text-right">
                <button class="inline-flex items-center justify-center w-8 h-8 rounded-lg bg-cream hover:bg-cream-deep text-espresso transition-colors border border-cream-deep shadow-sm group">
                  <Eye class="w-4 h-4 group-hover:text-caramel transition-colors" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Xếp Ca -->
    <div v-if="showShiftModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[400px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="showShiftModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6">
          <h2 class="font-display text-2xl font-bold text-espresso mb-6 text-center">Xếp ca mới</h2>
          
          <div class="space-y-4">
            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Nhân viên</label>
              <select v-model="shiftForm.staff" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option value="">Chọn nhân viên</option>
                <option v-for="staff in staffList" :key="staff" :value="staff">{{ staff }}</option>
              </select>
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ngày làm việc</label>
              <input v-model="shiftForm.date" type="text" placeholder="Ví dụ: 24/04" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20" />
            </div>

            <!-- Smart Info Box -->
            <div v-if="smartWarning" :class="`px-4 py-3 rounded-xl border text-xs font-bold ${smartWarning.color}`">
              <div class="flex items-start gap-2">
                <component :is="smartWarning.icon" class="w-4 h-4 shrink-0 mt-0.5" />
                <span>{{ smartWarning.message }}</span>
              </div>
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ca làm</label>
              <select v-model="shiftForm.shift" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option value="morning">Ca Sáng (07:00 - 14:00)</option>
                <option value="afternoon">Ca Chiều (14:00 - 22:00)</option>
                <option value="evening">Ca Tối (18:00 - 22:00)</option>
              </select>
            </div>
          </div>

          <div class="flex gap-3 mt-8">
            <button @click="showShiftModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy
            </button>
            <button :disabled="smartWarning?.block" :class="smartWarning?.block ? 'opacity-50 cursor-not-allowed' : 'hover:bg-brown shadow-warm'" class="flex-1 py-3 rounded-xl bg-caramel text-white font-bold text-sm transition-colors uppercase tracking-wider">
              Lưu Ca
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Plus, Download, AlertCircle, DollarSign, Edit3, Eye, Check, X, Info, AlertTriangle } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'

const activeTab = ref<'shifts' | 'requests' | 'payroll'>('shifts')

// --- Shift Modal State ---
const showShiftModal = ref(false)
const shiftForm = ref({ staff: '', date: '', shift: 'morning' })
const staffList = ['Lan Trần', 'Khoa Phạm', 'Vy Hoàng', 'Nam Lê', 'Thảo Vũ']

const openShiftModal = (prefillDate = '') => {
  shiftForm.value = { staff: '', date: prefillDate || dates[0], shift: 'morning' }
  showShiftModal.value = true
}

const smartWarning = computed(() => {
  if (!shiftForm.value.staff || !shiftForm.value.date) return null
  
  // Logic kiểm tra xin nghỉ hoặc OT
  if (shiftForm.value.staff === 'Khoa Phạm' && shiftForm.value.date === '21/04') {
    return {
      message: '⚠️ Khoa Phạm đã xin nghỉ phép năm vào ngày này.',
      color: 'bg-destructive/10 text-destructive border-destructive/20',
      icon: AlertTriangle,
      block: true
    }
  }
  if (shiftForm.value.staff === 'Thảo Vũ' && shiftForm.value.date === '24/04') {
    return {
      message: '💡 Thảo Vũ có đơn đăng ký tăng ca (OT) ngày này.',
      color: 'bg-success/10 text-success-dark border-success/20',
      icon: Info,
      block: false
    }
  }
  
  return {
    message: 'Nhân viên sẵn sàng nhận ca.',
    color: 'bg-cream-deep text-muted-foreground border-transparent',
    icon: Info,
    block: false
  }
})

// --- Shifts State ---
const days = ["T2", "T3", "T4", "T5", "T6", "T7", "CN"]
const dates = ["20/04", "21/04", "22/04", "23/04", "24/04", "25/04", "26/04"]

const shiftColors = {
  morning: "bg-warning/15 border-warning/30",
  afternoon: "bg-caramel-light border-caramel/30",
  evening: "bg-espresso/10 border-espresso/30",
}

const schedule: Record<string, any[]> = {
  T2: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "7h–14h", note: 'Leave' },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "afternoon", time: "14h–22h" },
  ],
  T3: [
    { staff: "Vy H.", initials: "VH", color: "bg-espresso", shift: "morning", time: "7h–14h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "afternoon", time: "14h–22h" },
  ],
  T4: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "7h–14h" },
    { staff: "Thảo V.", initials: "TV", color: "bg-sage", shift: "evening", time: "18h–24h" },
  ],
  T5: [
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "morning", time: "7h–14h" },
    { staff: "Vy H.", initials: "VH", color: "bg-espresso", shift: "afternoon", time: "14h–22h" },
  ],
  T6: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "7h–14h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "afternoon", time: "14h–22h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "evening", time: "18h–22h", note: 'OT' },
  ],
  T7: [
    { staff: "Vy H.", initials: "VH", color: "bg-espresso", shift: "morning", time: "7h–14h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "afternoon", time: "14h–22h" },
  ],
  CN: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "7h–14h" },
  ],
}

const totalHours = [
  ["Lan Trần", "28h"],
  ["Khoa Phạm", "24h"],
  ["Vy Hoàng", "21h"],
  ["Nam Lê", "16h"],
  ["Thảo Vũ", "12h"],
]

// --- Requests State ---
const requestFilter = ref<'pending' | 'approved' | 'rejected'>('pending')

const typeColors: Record<string, string> = {
  'Phép năm': 'bg-blue-100 text-blue-700',
  'Tăng ca': 'bg-purple-100 text-purple-700',
  'Nghỉ không lương': 'bg-gray-100 text-gray-700',
  'Nghỉ bù': 'bg-caramel/20 text-caramel',
}

const requests = ref([
  { id: 1, name: 'Lan Trần', type: 'Phép năm', time: 'T5, 04/06 (Ca Sáng)', reason: 'Nhà có việc gia đình đột xuất ở quê', status: 'pending' },
  { id: 2, name: 'Khoa Phạm', type: 'Tăng ca', time: 'T6, 05/06 (18h - 22h)', reason: 'Hỗ trợ quán vì cuối tuần đông khách', status: 'pending' },
  { id: 3, name: 'Vy Hoàng', type: 'Nghỉ không lương', time: 'T7, 06/06 (Cả ngày)', reason: 'Đi khám bệnh', status: 'pending' },
  { id: 4, name: 'Nam Lê', type: 'Nghỉ bù', time: 'CN, 07/06 (Ca Sáng)', reason: 'Nghỉ bù cho ca làm thêm Lễ 30/4', status: 'pending' },
  { id: 5, name: 'Thảo Vũ', type: 'Tăng ca', time: 'T2, 08/06 (14h - 18h)', reason: 'Hỗ trợ setup sự kiện', status: 'pending' },
  { id: 6, name: 'Lan Trần', type: 'Phép năm', time: 'T2, 01/06', reason: 'Nghỉ phép', status: 'approved' },
])

const pendingRequests = computed(() => requests.value.filter(r => r.status === 'pending'))
const filteredRequests = computed(() => requests.value.filter(r => r.status === requestFilter.value))

const handleAction = (id: number, status: 'approved' | 'rejected') => {
  const req = requests.value.find(r => r.id === id)
  if (req) {
    req.status = status
  }
}

// --- Payroll State ---
const payroll = ref([
  { id: 1, name: 'Lan Trần', avatar: 'https://i.pravatar.cc/150?u=1', rate: 25000, normalHours: 160, otHours: 12, leaveDays: 1, total: 4300000 },
  { id: 2, name: 'Khoa Phạm', avatar: 'https://i.pravatar.cc/150?u=2', rate: 22000, normalHours: 150, otHours: 0, leaveDays: 0, total: 3300000 },
  { id: 3, name: 'Vy Hoàng', avatar: 'https://i.pravatar.cc/150?u=3', rate: 25000, normalHours: 145, otHours: 5, leaveDays: 0, total: 3750000 },
  { id: 4, name: 'Nam Lê', avatar: 'https://i.pravatar.cc/150?u=4', rate: 28000, normalHours: 160, otHours: 20, leaveDays: 2, total: 5040000 },
])
</script>

<style scoped>
.list-enter-active,
.list-leave-active {
  transition: all 0.4s ease;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}
.list-leave-active {
  position: absolute;
}
</style>
