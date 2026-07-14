<template>
  <div class="p-6">
    <div class="mb-6">
      <h2 class="font-display text-2xl text-espresso font-bold">Ca làm, Đơn từ & Bảng lương</h2>
      <p class="text-xs text-muted-foreground mt-1">Quản trị toàn bộ lịch phân ca, xét duyệt yêu cầu và tính lương nhân sự.</p>
    </div>

    <!-- Khối Chỉ số thông minh (3 Thẻ Metrics) -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-5 mb-6">
      <div class="bg-card p-5 rounded-xl border border-cream-deep shadow-card flex flex-col justify-center">
        <span class="text-xs text-muted-foreground font-bold uppercase tracking-widest mb-1">Tổng giờ làm tuần này</span>
        <div class="flex items-baseline gap-2">
          <h3 class="font-sans text-3xl font-bold text-espresso">{{ totalWeeklyHours }}<span class="text-lg font-normal ml-1">h</span></h3>
          <span class="text-xs font-bold text-success">Lịch phân ca</span>
        </div>
      </div>
      
      <div class="bg-caramel p-5 rounded-xl shadow-warm flex flex-col justify-center relative overflow-hidden">
        <div class="absolute -right-4 -bottom-4 opacity-10 pointer-events-none">
          <DollarSign class="w-32 h-32 text-white" />
        </div>
        <span class="text-xs text-white/80 font-bold uppercase tracking-widest mb-1 relative z-10">Quỹ lương tuần này</span>
        <div class="flex items-baseline gap-2 relative z-10">
          <h3 class="font-sans text-3xl font-bold text-white">{{ totalWeeklyWages.toLocaleString() }}<span class="text-lg font-normal">đ</span></h3>
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
        <Button @click="openShiftModal()" size="sm" class="bg-espresso text-cream rounded-lg border border-espresso/30 shadow-card hover:bg-brown">
          <Plus class="w-3.5 h-3.5 mr-1" /> Xếp ca
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
              <div v-for="(s, i) in schedule[d] || []" :key="i" class="p-3 rounded-lg border text-xs shadow-card relative z-10 group/card" :class="(shiftColors as any)[s.shift]">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-2">
                    <div :class="['w-6 h-6 rounded-lg text-cream text-[10px] font-semibold flex items-center justify-center', s.color]">
                      {{ s.initials }}
                    </div>
                    <span class="font-medium text-espresso truncate">{{ s.staff }}</span>
                  </div>
                  <div class="flex items-center gap-1 opacity-0 group-hover/card:opacity-100 transition-opacity">
                    <button @click="editShift(d, i, dates[dIdx])" class="text-muted-foreground hover:text-caramel p-0.5" title="Sửa ca làm">
                      <Edit3 class="w-3.5 h-3.5" />
                    </button>
                    <button @click="deleteShift(d, i)" class="text-muted-foreground hover:text-destructive p-0.5" title="Xóa ca làm">
                      <Trash2 class="w-3.5 h-3.5" />
                    </button>
                  </div>
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
            <h4 class="font-display text-base text-espresso font-semibold mb-4">Phân bổ giờ & Lương</h4>
            <div class="space-y-3">
              <div v-for="[n, h] in totalHours" :key="n" class="group/rate flex justify-between items-center text-sm border-b border-cream/35 pb-2 last:border-0 last:pb-0">
                <div>
                  <span class="text-espresso font-medium block">{{ n }}</span>
                  <div class="flex items-center gap-1.5 text-[10px] text-muted-foreground">
                    <span>
                      {{ n === 'Vy Hoàng' || n === 'Khoa Phạm' ? 'Pha chế' : 'Nhân viên' }} · {{ (employeeRates[n] || 20000).toLocaleString() }}đ/h
                    </span>
                    <button @click="editRate(n)" class="opacity-0 group-hover/rate:opacity-100 transition-opacity text-caramel hover:text-brown p-0.5" title="Sửa mức lương">
                      <Edit3 class="w-3.5 h-3.5" />
                    </button>
                  </div>
                </div>
                <div class="text-right">
                  <span class="font-semibold text-caramel block">{{ h }}</span>
                  <span class="text-[11px] font-bold text-espresso">
                    {{ (parseInt(h) * (employeeRates[n] || 20000)).toLocaleString() }}đ
                  </span>
                </div>
              </div>
            </div>
          </div>
        </aside>
      </div>
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
                  <div>
                    <span class="text-espresso font-medium block">{{ row.name }}</span>
                    <span class="text-[10px] text-muted-foreground">{{ row.role }}</span>
                  </div>
                </div>
              </td>
              <td class="px-5 py-4">
                <div @click="editRate(row.name)" class="flex items-center gap-2 group cursor-pointer w-fit" title="Click để sửa mức lương">
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
          <h2 class="font-display text-2xl font-bold text-espresso mb-6 text-center">
            {{ isEditing ? 'Chỉnh sửa ca làm' : 'Xếp ca mới' }}
          </h2>
          
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
                <option value="morning" :disabled="isShiftOptionDisabled('morning')">
                  Ca Sáng (06:00 - 12:00) {{ isShiftOptionDisabled('morning') ? ' - (Đầy)' : '' }}
                </option>
                <option value="afternoon" :disabled="isShiftOptionDisabled('afternoon')">
                  Ca Chiều (12:00 - 17:00) {{ isShiftOptionDisabled('afternoon') ? ' - (Đầy)' : '' }}
                </option>
                <option value="evening" :disabled="isShiftOptionDisabled('evening')">
                  Ca Tối (17:00 - 22:00) {{ isShiftOptionDisabled('evening') ? ' - (Đầy)' : '' }}
                </option>
              </select>
            </div>
          </div>

          <div class="flex gap-3 mt-8">
            <button @click="showShiftModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy
            </button>
            <button @click="saveShift" :disabled="smartWarning?.block" :class="smartWarning?.block ? 'opacity-50 cursor-not-allowed' : 'hover:bg-brown shadow-warm'" class="flex-1 py-3 rounded-xl bg-caramel text-white font-bold text-sm transition-colors uppercase tracking-wider">
              Lưu Ca
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Sửa Mức Lương -->
    <div v-if="showRateModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[400px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="showRateModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6">
          <h2 class="font-display text-2xl font-bold text-espresso mb-1 text-center">Cấu hình Mức lương</h2>
          <p class="text-xs text-muted-foreground text-center mb-6">
            Nhân viên: <span class="font-semibold text-espresso">{{ rateForm.name }}</span>
          </p>

          <div class="space-y-4">
            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-[#8A8178] font-bold">Mức lương mỗi giờ (đ/h)</label>
              <div class="relative">
                <input 
                  type="number" 
                  v-model.number="rateForm.rate" 
                  placeholder="Ví dụ: 25000" 
                  class="w-full bg-cream border border-cream-deep rounded-xl pl-4 pr-12 py-3 text-sm text-espresso font-semibold focus:outline-none focus:ring-2 focus:ring-caramel/20" 
                />
                <span class="absolute right-4 top-1/2 -translate-y-1/2 text-xs font-bold text-muted-foreground">đ/h</span>
              </div>
            </div>
          </div>

          <div class="flex gap-3 mt-8">
            <button @click="showRateModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy
            </button>
            <button @click="saveRate" class="flex-1 py-3 rounded-xl bg-caramel hover:bg-brown text-white font-bold text-sm transition-colors shadow-warm uppercase tracking-wider">
              Lưu cấu hình
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, reactive } from 'vue'
import { Plus, Download, AlertCircle, DollarSign, Edit3, Eye, Check, X, Info, AlertTriangle, Trash2 } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import { hrApi, type DonTuItem } from '@/services/hr'
import { useToast } from '@/stores/toast'

const activeTab = ref<'shifts' | 'requests' | 'payroll'>('shifts')
const toast = useToast()

// --- Shift Modal State ---
const showShiftModal = ref(false)
const shiftForm = ref({ staff: '', date: '', shift: 'morning' })
const staffList = ['Lan Trần', 'Khoa Phạm', 'Vy Hoàng', 'Nam Lê', 'Thảo Vũ']

const isEditing = ref(false)
const editingCell = ref<{ day: string; index: number } | null>(null)

const openShiftModal = (prefillDate = '') => {
  shiftForm.value = { staff: '', date: prefillDate || dates[0] || '', shift: 'morning' }
  isEditing.value = false
  editingCell.value = null
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

const schedule = reactive<Record<string, any[]>>({
  T2: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "6h–12h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "morning", time: "6h–12h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "afternoon", time: "12h–17h" },
    { staff: "Thảo V.", initials: "TV", color: "bg-sage", shift: "afternoon", time: "12h–17h" },
  ],
  T3: [
    { staff: "Vy H.", initials: "VH", color: "bg-espresso", shift: "morning", time: "6h–12h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "morning", time: "6h–12h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "evening", time: "17h–22h" },
    { staff: "Thảo V.", initials: "TV", color: "bg-sage", shift: "evening", time: "17h–22h" },
  ],
  T4: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "6h–12h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "morning", time: "6h–12h" },
    { staff: "Thảo V.", initials: "TV", color: "bg-sage", shift: "evening", time: "17h–22h" },
    { staff: "Vy H.", initials: "VH", color: "bg-espresso", shift: "evening", time: "17h–22h" },
  ],
  T5: [
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "morning", time: "6h–12h" },
    { staff: "Vy H.", initials: "VH", color: "bg-espresso", shift: "morning", time: "6h–12h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "afternoon", time: "12h–17h" },
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "afternoon", time: "12h–17h" },
  ],
  T6: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "6h–12h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "morning", time: "6h–12h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "evening", time: "17h–22h" },
    { staff: "Thảo V.", initials: "TV", color: "bg-sage", shift: "evening", time: "17h–22h" },
  ],
  T7: [
    { staff: "Vy H.", initials: "VH", color: "bg-espresso", shift: "morning", time: "6h–12h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "morning", time: "6h–12h" },
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "afternoon", time: "12h–17h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "afternoon", time: "12h–17h" },
  ],
  CN: [
    { staff: "Lan T.", initials: "LT", color: "bg-sage", shift: "morning", time: "6h–12h" },
    { staff: "Khoa P.", initials: "KP", color: "bg-brown", shift: "morning", time: "6h–12h" },
    { staff: "Nam L.", initials: "NL", color: "bg-caramel", shift: "evening", time: "17h–22h" },
    { staff: "Thảo V.", initials: "TV", color: "bg-sage", shift: "evening", time: "17h–22h" },
  ],
})

const getDayFromDate = (dateStr: string) => {
  const dayIdx = dates.indexOf(dateStr)
  return dayIdx !== -1 ? days[dayIdx] : null
}

const getShiftEmployeeCount = (dateStr: string, shiftVal: string) => {
  const day = getDayFromDate(dateStr)
  if (!day || !schedule[day]) return 0
  return schedule[day].filter(s => s.shift === shiftVal).length
}

const isShiftOptionDisabled = (shiftVal: string) => {
  if (!shiftForm.value.date) return false
  
  const count = getShiftEmployeeCount(shiftForm.value.date, shiftVal)
  
  if (isEditing.value && editingCell.value) {
    const { day, index } = editingCell.value
    const editingItem = schedule[day][index]
    const editingItemDay = getDayFromDate(shiftForm.value.date)
    
    if (day === editingItemDay && editingItem.shift === shiftVal) {
      return false
    }
  }
  
  return count >= 2
}

watch(() => shiftForm.value.date, (newDate) => {
  if (!newDate) return
  if (isShiftOptionDisabled(shiftForm.value.shift)) {
    const shifts: ('morning' | 'afternoon' | 'evening')[] = ['morning', 'afternoon', 'evening']
    const available = shifts.find(s => !isShiftOptionDisabled(s))
    if (available) {
      shiftForm.value.shift = available
    }
  }
})

const editShift = (day: string, index: number, dateStr: string) => {
  const item = schedule[day][index]
  const fullNameMap: Record<string, string> = {
    "Lan T.": "Lan Trần",
    "Khoa P.": "Khoa Phạm",
    "Vy H.": "Vy Hoàng",
    "Nam L.": "Nam Lê",
    "Thảo V.": "Thảo Vũ"
  }
  
  shiftForm.value = {
    staff: fullNameMap[item.staff] || item.staff,
    date: dateStr,
    shift: item.shift
  }
  
  isEditing.value = true
  editingCell.value = { day, index }
  showShiftModal.value = true
}

const saveShift = () => {
  if (!shiftForm.value.staff) {
    toast.warning('Vui lòng chọn nhân viên.', 'Thiếu thông tin')
    return
  }
  
  const shortNameMap: Record<string, { staff: string; initials: string; color: string }> = {
    "Lan Trần": { staff: "Lan T.", initials: "LT", color: "bg-sage" },
    "Khoa Phạm": { staff: "Khoa P.", initials: "KP", color: "bg-brown" },
    "Vy Hoàng": { staff: "Vy H.", initials: "VH", color: "bg-espresso" },
    "Nam Lê": { staff: "Nam L.", initials: "NL", color: "bg-caramel" },
    "Thảo Vũ": { staff: "Thảo V.", initials: "TV", color: "bg-sage" }
  }
  
  const staffInfo = shortNameMap[shiftForm.value.staff] || { staff: shiftForm.value.staff.split(' ').slice(-2).join(' '), initials: 'NV', color: 'bg-cream-deep' }
  const timeMap = {
    morning: "6h–12h",
    afternoon: "12h–17h",
    evening: "17h–22h"
  }
  const timeStr = timeMap[shiftForm.value.shift as keyof typeof timeMap] || "6h–12h"
  
  const shiftItem = {
    staff: staffInfo.staff,
    initials: staffInfo.initials,
    color: staffInfo.color,
    shift: shiftForm.value.shift,
    time: timeStr
  }
  
  const dayIdx = dates.indexOf(shiftForm.value.date)
  const targetDay = dayIdx !== -1 ? days[dayIdx] : null
  
  if (!targetDay) {
    toast.error('Ngày làm việc không hợp lệ.', 'Lỗi dữ liệu')
    return
  }

  const count = schedule[targetDay].filter(s => s.shift === shiftForm.value.shift).length
  let isSame = false
  if (isEditing.value && editingCell.value) {
    const { day, index } = editingCell.value
    const editingItem = schedule[day][index]
    if (day === targetDay && editingItem.shift === shiftForm.value.shift) {
      isSame = true
    }
  }
  
  if (!isSame && count >= 2) {
    toast.warning('Ca làm việc này đã đủ 2 nhân sự.', 'Không thể lưu')
    return
  }
  
  if (isEditing.value && editingCell.value) {
    const { day, index } = editingCell.value
    if (day !== targetDay) {
      schedule[day].splice(index, 1)
      if (!schedule[targetDay]) {
        schedule[targetDay] = []
      }
      schedule[targetDay].push(shiftItem)
    } else {
      schedule[day][index] = shiftItem
    }
    toast.success('Cập nhật ca làm thành công!', 'Thành công')
  } else {
    if (!schedule[targetDay]) {
      schedule[targetDay] = []
    }
    schedule[targetDay].push(shiftItem)
    toast.success('Xếp ca làm mới thành công!', 'Thành công')
  }
  
  recalculateTotalHours()
  localStorage.setItem('quanlycf_schedule', JSON.stringify(schedule))
  showShiftModal.value = false
}

const deleteShift = (day: string, index: number) => {
  const item = schedule[day][index]
  if (confirm(`Bạn có chắc chắn muốn xoá ca làm của nhân viên ${item.staff} vào ngày ${day}?`)) {
    schedule[day].splice(index, 1)
    recalculateTotalHours()
    localStorage.setItem('quanlycf_schedule', JSON.stringify(schedule))
    toast.success('Đã xoá ca làm thành công!', 'Thành công')
  }
}

const recalculateTotalHours = () => {
  const hoursMap: Record<string, number> = {
    "Lan Trần": 0,
    "Khoa Phạm": 0,
    "Vy Hoàng": 0,
    "Nam Lê": 0,
    "Thảo Vũ": 0
  }
  
  const fullNameMap: Record<string, string> = {
    "Lan T.": "Lan Trần",
    "Khoa P.": "Khoa Phạm",
    "Vy H.": "Vy Hoàng",
    "Nam L.": "Nam Lê",
    "Thảo V.": "Thảo Vũ"
  }
  
  for (const day of days) {
    const list = schedule[day] || []
    for (const item of list) {
      const fName = fullNameMap[item.staff] || item.staff
      const duration = item.shift === 'morning' ? 6 : 5
      if (hoursMap[fName] !== undefined) {
        hoursMap[fName] += duration
      }
    }
  }
  
  totalHours.value = [
    ["Vy Hoàng", `${hoursMap["Vy Hoàng"]}h`],
    ["Khoa Phạm", `${hoursMap["Khoa Phạm"]}h`],
    ["Nam Lê", `${hoursMap["Nam Lê"]}h`],
    ["Lan Trần", `${hoursMap["Lan Trần"]}h`],
    ["Thảo Vũ", `${hoursMap["Thảo Vũ"]}h`],
  ]
}

const totalHours = ref([
  ["Vy Hoàng", "39h"],
  ["Khoa Phạm", "39h"],
  ["Nam Lê", "32h"],
  ["Lan Trần", "39h"],
  ["Thảo Vũ", "25h"],
])

// --- Requests State ---
const requestFilter = ref<'pending' | 'approved' | 'rejected'>('pending')

const typeColors: Record<string, string> = {
  'Phép năm': 'bg-blue-100 text-blue-700',
  'Tăng ca': 'bg-purple-100 text-purple-700',
  'Nghỉ không lương': 'bg-gray-100 text-gray-700',
  'Nghỉ bù': 'bg-caramel/20 text-caramel',
}

const requestLogs = ref<DonTuItem[]>([])

const loadRequests = async () => {
  try {
    requestLogs.value = await hrApi.getAllRequests()
  } catch (err: any) {
    console.error("Failed to load requests", err)
  }
}

const pendingRequests = computed(() => requestLogs.value.filter(r => r.trangThai === 'ChoDuyet'))

const filteredRequests = computed(() => {
  const statusMap = {
    pending: 'ChoDuyet',
    approved: 'DaDuyet',
    rejected: 'TuChoi'
  }
  const targetStatus = statusMap[requestFilter.value]
  return requestLogs.value.filter(r => r.trangThai === targetStatus)
})

const handleAction = async (id: number, actionStatus: 'DaDuyet' | 'TuChoi') => {
  try {
    const res = await hrApi.reviewRequest(id, actionStatus)
    toast.success(res.message, 'Duyệt đơn từ')
    await loadRequests()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi duyệt đơn từ')
  }
}

onMounted(() => {
  loadRequests()
})

// --- Payroll State ---
const employeeRates = ref<Record<string, number>>({
  "Vy Hoàng": 25000,
  "Khoa Phạm": 25000,
  "Lan Trần": 20000,
  "Nam Lê": 20000,
  "Thảo Vũ": 20000
})

const showRateModal = ref(false)
const rateForm = ref({ name: '', rate: 20000 })

const editRate = (name: string) => {
  rateForm.value = {
    name,
    rate: employeeRates.value[name] || 20000
  }
  showRateModal.value = true
}

const saveRate = () => {
  const { name, rate } = rateForm.value
  const num = parseInt(rate as any)
  if (!isNaN(num) && num > 0) {
    employeeRates.value[name] = num
    toast.success(`Đã cập nhật mức lương của ${name} thành ${num.toLocaleString()}đ/h`, 'Thành công')
    showRateModal.value = false
  } else {
    toast.warning('Mức lương không hợp lệ.', 'Lỗi nhập liệu')
  }
}

const payroll = computed(() => {
  const hoursMap: Record<string, number> = {
    "Lan Trần": 0,
    "Khoa Phạm": 0,
    "Vy Hoàng": 0,
    "Nam Lê": 0,
    "Thảo Vũ": 0
  }
  
  const fullNameMap: Record<string, string> = {
    "Lan T.": "Lan Trần",
    "Khoa P.": "Khoa Phạm",
    "Vy H.": "Vy Hoàng",
    "Nam L.": "Nam Lê",
    "Thảo V.": "Thảo Vũ"
  }
  
  for (const day of days) {
    const list = schedule[day] || []
    for (const item of list) {
      const fName = fullNameMap[item.staff] || item.staff
      const duration = item.shift === 'morning' ? 6 : 5
      if (hoursMap[fName] !== undefined) {
        hoursMap[fName] += duration
      }
    }
  }

  const roles: Record<string, string> = {
    "Vy Hoàng": "Pha chế",
    "Khoa Phạm": "Pha chế",
    "Lan Trần": "Nhân viên",
    "Nam Lê": "Nhân viên",
    "Thảo Vũ": "Nhân viên"
  }

  const avatars: Record<string, string> = {
    "Lan Trần": 'https://api.dicebear.com/7.x/adventurer/svg?seed=LanTran',
    "Khoa Phạm": 'https://api.dicebear.com/7.x/adventurer/svg?seed=KhoaPham',
    "Vy Hoàng": 'https://api.dicebear.com/7.x/adventurer/svg?seed=VyHoang',
    "Nam Lê": 'https://api.dicebear.com/7.x/adventurer/svg?seed=NamLe',
    "Thảo Vũ": 'https://api.dicebear.com/7.x/adventurer/svg?seed=ThaoVu'
  }

  return Object.keys(hoursMap).map((name, index) => {
    const hours = hoursMap[name]
    const rate = employeeRates.value[name] || 20000
    const role = roles[name] || "Nhân viên"
    const total = hours * rate
    
    return {
      id: index + 1,
      name,
      role,
      avatar: avatars[name] || 'https://i.pravatar.cc/150',
      rate,
      normalHours: hours,
      otHours: 0,
      leaveDays: 0,
      total
    }
  })
})

const totalWeeklyHours = computed(() => {
  let sum = 0
  for (const day of days) {
    const list = schedule[day] || []
    for (const item of list) {
      sum += item.shift === 'morning' ? 6 : 5
    }
  }
  return sum
})

const totalWeeklyWages = computed(() => {
  let sum = 0
  const fullNameMap: Record<string, string> = {
    "Lan T.": "Lan Trần",
    "Khoa P.": "Khoa Phạm",
    "Vy H.": "Vy Hoàng",
    "Nam L.": "Nam Lê",
    "Thảo V.": "Thảo Vũ"
  }
  for (const day of days) {
    const list = schedule[day] || []
    for (const item of list) {
      const fName = fullNameMap[item.staff] || item.staff
      const duration = item.shift === 'morning' ? 6 : 5
      const rate = employeeRates.value[fName] || 20000
      sum += duration * rate
    }
  }
  return sum
})

onMounted(() => {
  const saved = localStorage.getItem('quanlycf_schedule')
  if (saved) {
    try {
      const parsed = JSON.parse(saved)
      for (const key of Object.keys(schedule)) {
        if (parsed[key]) {
          schedule[key] = parsed[key]
        }
      }
    } catch (e) {
      console.error(e)
    }
  } else {
    localStorage.setItem('quanlycf_schedule', JSON.stringify(schedule))
  }
  recalculateTotalHours()
})
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
