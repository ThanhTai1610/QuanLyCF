<template>
  <div class="p-6">
    <div class="flex flex-col xl:flex-row items-start xl:items-center justify-between gap-4 mb-6">
      <div>
        <h2 class="font-display text-2xl text-espresso font-bold">Chấm công & Yêu cầu</h2>
        <p class="text-xs text-muted-foreground mt-1">Quản lý lịch sử vào/ra ca và tạo các đơn từ cá nhân.</p>
        
        <!-- Trạng thái yêu cầu cá nhân (Employee view) -->
        <div class="mt-4 flex flex-wrap items-center gap-3 bg-cream p-2.5 rounded-lg border border-cream-deep inline-flex">
          <span class="text-[10px] uppercase font-bold text-muted-foreground tracking-widest mr-1">Trạng thái yêu cầu của tôi:</span>
          <div class="flex flex-wrap gap-2">
            <span class="px-2 py-1 bg-warning/15 text-warning-dark border border-warning/20 text-[10px] font-bold rounded-md flex items-center gap-1">
              Phép năm (20/4) <div class="w-1.5 h-1.5 rounded-full bg-warning"></div>
            </span>
            <span class="px-2 py-1 bg-success/15 text-success-dark border border-success/20 text-[10px] font-bold rounded-md flex items-center gap-1">
              Tăng ca (18/4) <div class="w-1.5 h-1.5 rounded-full bg-success"></div>
            </span>
            <span class="px-2 py-1 bg-destructive/15 text-destructive border border-destructive/20 text-[10px] font-bold rounded-md flex items-center gap-1">
              Nghỉ bù (10/4) <div class="w-1.5 h-1.5 rounded-full bg-destructive"></div>
            </span>
          </div>
        </div>
      </div>
      
      <div class="flex gap-3 mt-4 xl:mt-0">
        <Button @click="showCreateModal = true" variant="outline" class="border-caramel text-caramel hover:bg-caramel/10 bg-white rounded-lg shadow-sm font-bold px-5">
          <Plus class="w-4 h-4 mr-2" /> TẠO YÊU CẦU MỚI
        </Button>
        <Button @click="showCheckInModal = true" class="bg-caramel hover:bg-brown text-cream rounded-lg shadow-warm font-bold transition-all px-5">
          <Zap class="w-4 h-4 mr-2 fill-current" /> CHẤM CÔNG NGAY
        </Button>
      </div>
    </div>

    <!-- Main Tabs -->
    <div class="flex items-center gap-6 border-b-2 border-cream-deep mb-6">
      <button 
        @click="activeTab = 'checkin'" 
        class="pb-3 text-sm font-bold transition-colors relative"
        :class="activeTab === 'checkin' ? 'text-espresso' : 'text-muted-foreground hover:text-espresso'"
      >
        Lịch sử chấm công
        <div v-if="activeTab === 'checkin'" class="absolute bottom-[-2px] left-0 w-full h-0.5 bg-espresso rounded-t-full"></div>
      </button>
      <button 
        @click="activeTab = 'requests'" 
        class="pb-3 text-sm font-bold transition-colors relative"
        :class="activeTab === 'requests' ? 'text-caramel' : 'text-muted-foreground hover:text-espresso'"
      >
        Lịch sử đơn từ 
        <div v-if="activeTab === 'requests'" class="absolute bottom-[-2px] left-0 w-full h-0.5 bg-caramel rounded-t-full"></div>
      </button>
    </div>

    <!-- Bảng Lịch sử chấm công của tôi -->
    <div v-if="activeTab === 'checkin'" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden animate-in fade-in duration-300">
      <div class="p-5 border-b-2 border-cream-deep flex justify-between items-center">
        <h3 class="font-display text-lg text-espresso font-semibold">Lịch sử chấm công của tôi (Tháng này)</h3>
        <div class="flex gap-2">
          <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-success/10 text-success text-[10px] font-bold">
            <div class="w-1.5 h-1.5 rounded-full bg-success"></div> Đang trong ca
          </span>
        </div>
      </div>
      
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="bg-cream/50 text-left text-muted-foreground text-xs uppercase border-b-2 border-cream-deep">
              <th class="px-5 py-4 font-medium">Ngày</th>
              <th class="px-5 py-4 font-medium">Ca làm</th>
              <th class="px-5 py-4 font-medium">Giờ vào</th>
              <th class="px-5 py-4 font-medium text-center">Ảnh vào</th>
              <th class="px-5 py-4 font-medium">Giờ ra</th>
              <th class="px-5 py-4 font-medium text-center">Ảnh ra</th>
              <th class="px-5 py-4 font-medium">Tổng giờ</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(row, i) in myCheckinTable" :key="i" class="border-t-2 border-cream-deep/60 hover:bg-cream/20 transition-colors">
              <td class="px-5 py-4 text-espresso font-medium">{{ row.date }}</td>
              <td class="px-5 py-4 text-espresso">
                <span class="px-2 py-1 bg-cream-deep rounded text-xs font-medium">{{ row.shift }}</span>
              </td>
              <td class="px-5 py-4">
                <span :class="row.timeIn ? 'text-espresso font-medium' : 'text-muted-foreground'">{{ row.timeIn || '--:--' }}</span>
              </td>
              <td class="px-5 py-4 text-center">
                <div class="relative group inline-block" v-if="row.imgIn">
                  <img :src="row.imgIn" class="w-8 h-8 rounded-full border-2 border-cream-deep object-cover cursor-pointer" />
                  <div class="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 w-48 bg-espresso text-cream rounded-xl p-3 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-10 shadow-warm pointer-events-none">
                    <img :src="row.imgIn" class="w-full h-auto rounded-lg mb-2 object-cover aspect-[4/3]" />
                    <div class="text-center">
                      <div class="text-xs font-bold text-caramel-light">Xác thực Vào Ca</div>
                      <div class="text-[10px] text-muted-foreground mt-1">Lúc {{ row.timeInExact }}</div>
                    </div>
                  </div>
                </div>
                <span v-else class="text-xs text-muted-foreground">-</span>
              </td>
              <td class="px-5 py-4">
                <span :class="row.timeOut ? 'text-espresso font-medium' : 'text-muted-foreground'">{{ row.timeOut || '--:--' }}</span>
              </td>
              <td class="px-5 py-4 text-center">
                <div class="relative group inline-block" v-if="row.imgOut">
                  <img :src="row.imgOut" class="w-8 h-8 rounded-full border-2 border-cream-deep object-cover cursor-pointer" />
                  <div class="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 w-48 bg-espresso text-cream rounded-xl p-3 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-10 shadow-warm pointer-events-none">
                    <img :src="row.imgOut" class="w-full h-auto rounded-lg mb-2 object-cover aspect-[4/3]" />
                    <div class="text-center">
                      <div class="text-xs font-bold text-caramel-light">Xác thực Kết Ca</div>
                      <div class="text-[10px] text-muted-foreground mt-1">Lúc {{ row.timeOutExact }}</div>
                    </div>
                  </div>
                </div>
                <span v-else class="text-xs text-muted-foreground">-</span>
              </td>
              <td class="px-5 py-4 text-espresso font-bold">{{ row.total || '-' }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Tab 2: Lịch sử đơn từ của tôi -->
    <div v-if="activeTab === 'requests'" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden min-h-[300px] animate-in fade-in duration-300">
      <div class="flex items-center gap-4 px-5 py-4 border-b-2 border-cream-deep bg-cream/30">
        <span class="text-xs font-bold text-espresso uppercase tracking-widest">Bộ lọc:</span>
        <button @click="requestFilter = 'all'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'all' ? 'bg-espresso text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Tất cả</button>
        <button @click="requestFilter = 'pending'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'pending' ? 'bg-caramel text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Chờ duyệt</button>
        <button @click="requestFilter = 'approved'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'approved' ? 'bg-success/80 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Đã duyệt</button>
        <button @click="requestFilter = 'rejected'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'rejected' ? 'bg-destructive/80 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Đã từ chối</button>
      </div>
      
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="bg-cream/50 text-left text-muted-foreground text-xs uppercase border-b-2 border-cream-deep">
              <th class="px-5 py-4 font-medium w-[15%]">Loại đơn</th>
              <th class="px-5 py-4 font-medium w-[20%]">Thời gian đăng ký</th>
              <th class="px-5 py-4 font-medium w-[40%]">Lý do</th>
              <th class="px-5 py-4 font-medium w-[25%]">Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in filteredMyRequests" :key="row.id" class="border-t border-cream-deep/60 hover:bg-cream/20 transition-colors">
              <td class="px-5 py-4">
                <span class="px-2.5 py-1 rounded text-[11px] font-bold" :class="typeColors[row.type]">
                  {{ row.type }}
                </span>
              </td>
              <td class="px-5 py-4 text-espresso font-medium text-xs">
                {{ row.time }}
              </td>
              <td class="px-5 py-4 text-muted-foreground text-xs">
                <p class="truncate max-w-[300px]" :title="row.reason">{{ row.reason }}</p>
              </td>
              <td class="px-5 py-4">
                <span v-if="row.status === 'pending'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-caramel/10 text-caramel text-[11px] font-bold border border-caramel/20">
                  <div class="w-1.5 h-1.5 rounded-full bg-caramel"></div> Chờ duyệt
                </span>
                <span v-else-if="row.status === 'approved'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-success/10 text-success text-[11px] font-bold border border-success/20">
                  <div class="w-1.5 h-1.5 rounded-full bg-success"></div> Đã duyệt
                </span>
                <span v-else-if="row.status === 'rejected'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-destructive/10 text-destructive text-[11px] font-bold border border-destructive/20">
                  <div class="w-1.5 h-1.5 rounded-full bg-destructive"></div> Đã từ chối
                </span>
              </td>
            </tr>
            <tr v-if="filteredMyRequests.length === 0">
              <td colspan="4" class="py-8 text-center text-muted-foreground text-xs">Không có đơn từ nào.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Chấm Công Camera -->
    <div v-if="showCheckInModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[400px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="closeCheckIn" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6">
          <div class="text-center mb-6">
            <h2 class="font-display text-2xl font-bold text-espresso mb-1">Chấm Công</h2>
            <p class="text-xs text-muted-foreground">Nhân viên: <span class="font-semibold text-espresso">Lan Trần</span></p>
          </div>

          <div v-if="checkInStep === 1" class="space-y-6">
            <div class="bg-cream p-4 rounded-xl border border-cream-deep text-center">
              <div class="text-xs font-bold text-caramel uppercase tracking-widest mb-1">Ca làm hiện tại</div>
              <div class="font-display text-xl text-espresso font-semibold">Ca Sáng</div>
              <div class="text-sm text-muted-foreground">07:00 - 14:00</div>
            </div>
            
            <div class="grid grid-cols-2 gap-3">
              <button @click="startCamera('in')" class="flex flex-col items-center justify-center py-6 rounded-xl border border-success/30 bg-success/10 hover:bg-success/20 transition-colors group shadow-sm">
                <LogIn class="w-8 h-8 text-success mb-2 group-hover:scale-110 transition-transform" />
                <span class="font-bold text-success text-sm tracking-wide">VÀO CA</span>
              </button>
              <button @click="startCamera('out')" class="flex flex-col items-center justify-center py-6 rounded-xl border border-destructive/30 bg-destructive/10 hover:bg-destructive/20 transition-colors group shadow-sm">
                <LogOut class="w-8 h-8 text-destructive mb-2 group-hover:scale-110 transition-transform" />
                <span class="font-bold text-destructive text-sm tracking-wide">KẾT CA</span>
              </button>
            </div>
          </div>

          <div v-else-if="checkInStep === 2" class="space-y-6 animate-in slide-in-from-right-4 duration-300">
            <div class="text-center space-y-1">
              <p class="font-bold text-espresso">{{ checkInType === 'in' ? 'Chụp ảnh Vào ca' : 'Chụp ảnh Kết ca' }}</p>
              <p class="text-[10px] text-muted-foreground uppercase tracking-widest">Đưa khuôn mặt vào khung hình</p>
            </div>

            <div class="relative w-full aspect-[4/3] rounded-2xl overflow-hidden bg-espresso border-4 border-cream shadow-inner">
              <img src="https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400&q=80" class="w-full h-full object-cover opacity-80" />
              <div class="absolute inset-6 border-2 border-dashed border-white/60 rounded-[30%] animate-pulse pointer-events-none"></div>
            </div>

            <div class="flex justify-center">
              <button @click="takePhoto" class="w-16 h-16 rounded-full bg-caramel hover:bg-brown flex items-center justify-center text-white shadow-[0_0_20px_rgba(200,133,58,0.4)] transition-all active:scale-95 group border-4 border-white">
                <Camera class="w-7 h-7 group-hover:scale-110 transition-transform" />
              </button>
            </div>
          </div>

          <div v-else-if="checkInStep === 3" class="space-y-6 animate-in slide-in-from-right-4 duration-300">
            <div class="text-center space-y-1">
              <p class="font-bold text-espresso">Kiểm tra hình ảnh</p>
              <p class="text-[10px] text-muted-foreground uppercase tracking-widest">Đảm bảo ảnh rõ nét</p>
            </div>

            <div class="relative w-full aspect-[4/3] rounded-2xl overflow-hidden border-4 border-cream-deep shadow-inner">
              <img src="https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400&q=80" class="w-full h-full object-cover" />
              <div class="absolute bottom-3 right-3 bg-espresso/80 backdrop-blur-md px-3 py-1 rounded-full text-[10px] text-cream font-medium">
                {{ new Date().toLocaleTimeString('vi-VN') }}
              </div>
            </div>

            <div class="grid grid-cols-2 gap-3">
              <button @click="checkInStep = 2" class="py-3 px-4 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso text-xs font-bold transition-colors">
                Chụp lại
              </button>
              <button @click="confirmCheckIn" class="py-3 px-4 rounded-xl border border-caramel bg-caramel hover:bg-brown text-white text-xs font-bold transition-colors shadow-warm uppercase tracking-wide">
                Xác Nhận
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Tạo Đơn -->
    <div v-if="showCreateModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[450px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <div class="p-6">
          <h2 class="font-display text-2xl font-bold text-espresso mb-6 text-center">Tạo yêu cầu mới</h2>
          
          <div class="space-y-5">
            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Loại đơn</label>
              <select class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option value="Phép năm">Xin nghỉ phép năm</option>
                <option value="Tăng ca">Xin tăng ca (OT)</option>
                <option value="Nghỉ không lương">Xin nghỉ không lương</option>
                <option value="Nghỉ bù">Xin nghỉ bù</option>
              </select>
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ngày & Ca làm</label>
              <input type="datetime-local" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20" />
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Lý do</label>
              <textarea rows="3" placeholder="Nhập lý do chi tiết..." class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20 resize-none"></textarea>
            </div>
          </div>

          <div class="flex gap-3 mt-8">
            <button @click="showCreateModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy bỏ
            </button>
            <button @click="showCreateModal = false" class="flex-1 py-3 rounded-xl bg-caramel hover:bg-brown text-white font-bold text-sm transition-colors shadow-warm uppercase tracking-wider">
              Gửi Đơn
            </button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Plus, X, Zap, Camera, LogIn, LogOut } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'

const activeTab = ref<'checkin' | 'requests'>('checkin')
const requestFilter = ref<'all' | 'pending' | 'approved' | 'rejected'>('all')

// --- Check-in State ---
const showCheckInModal = ref(false)
const showCreateModal = ref(false)

const checkInStep = ref<1 | 2 | 3>(1)
const checkInType = ref<'in' | 'out'>('in')

const closeCheckIn = () => {
  showCheckInModal.value = false
  setTimeout(() => { checkInStep.value = 1 }, 200)
}
const startCamera = (type: 'in' | 'out') => {
  checkInType.value = type; checkInStep.value = 2
}
const takePhoto = () => { checkInStep.value = 3 }
const confirmCheckIn = () => { closeCheckIn() }

const myCheckinTable = [
  { date: "Hôm nay, 21/04", shift: "Ca Sáng", timeIn: "06:58", timeOut: "", total: "", imgIn: "https://i.pravatar.cc/150?u=1", imgOut: "", timeInExact: "06:58:12", timeOutExact: "" },
  { date: "Hôm qua, 20/04", shift: "Ca Sáng", timeIn: "06:55", timeOut: "14:02", total: "7h 7m", imgIn: "https://i.pravatar.cc/150?u=1", imgOut: "https://i.pravatar.cc/150?u=1_out", timeInExact: "06:55:10", timeOutExact: "14:02:11" },
  { date: "T6, 17/04", shift: "Ca Chiều", timeIn: "13:50", timeOut: "22:15", total: "8h 25m", imgIn: "https://i.pravatar.cc/150?u=1", imgOut: "https://i.pravatar.cc/150?u=1_out", timeInExact: "13:50:20", timeOutExact: "22:15:30" },
]

// --- Requests State ---
const typeColors: Record<string, string> = {
  'Phép năm': 'bg-blue-100 text-blue-700',
  'Tăng ca': 'bg-purple-100 text-purple-700',
  'Nghỉ không lương': 'bg-gray-100 text-gray-700',
  'Nghỉ bù': 'bg-caramel/20 text-caramel',
}

const myRequests = ref([
  { id: 1, type: 'Phép năm', time: 'T5, 20/04 (Ca Sáng)', reason: 'Nhà có việc gia đình đột xuất', status: 'pending' },
  { id: 2, type: 'Tăng ca', time: 'T3, 18/04 (18h - 22h)', reason: 'Hỗ trợ sự kiện', status: 'approved' },
  { id: 3, type: 'Nghỉ bù', time: 'T6, 10/04 (Cả ngày)', reason: 'Nghỉ bù Lễ', status: 'rejected' },
  { id: 4, type: 'Phép năm', time: 'T2, 01/04', reason: 'Nghỉ phép', status: 'approved' },
])

const filteredMyRequests = computed(() => {
  if (requestFilter.value === 'all') return myRequests.value
  return myRequests.value.filter(r => r.status === requestFilter.value)
})
</script>
