<template>
  <div class="p-6">
    <div class="flex flex-col xl:flex-row items-start xl:items-center justify-between gap-4 mb-6">
      <div>
        <h2 class="font-display text-2xl text-espresso font-bold">Chấm công & Yêu cầu</h2>
        <p class="text-xs text-muted-foreground mt-1">Quản lý lịch sử vào/ra ca và tạo các đơn từ cá nhân.</p>
        
        <!-- Trạng thái yêu cầu cá nhân (Employee view) -->
        <div class="mt-4 flex flex-wrap items-center gap-3 bg-cream p-2.5 rounded-lg border border-cream-deep inline-flex">
          <span class="text-[10px] uppercase font-bold text-muted-foreground tracking-widest mr-1">Tài khoản:</span>
          <span class="text-xs font-semibold text-espresso">{{ authStore.user?.hoTen }} ({{ authStore.user?.vaiTro }})</span>
          
          <template v-if="isManager">
            <span class="text-[10px] uppercase font-bold text-[#8A8178] tracking-widest ml-3 border-l border-cream-deep pl-3">Xem nhân viên:</span>
            <select v-model="selectedViewEmployeeId" @change="onViewEmployeeChange" class="bg-transparent border-none text-xs font-semibold text-caramel focus:outline-none cursor-pointer">
              <option v-for="emp in filteredEmployeeList" :key="emp.maNhanVien" :value="emp.maNhanVien">
                {{ emp.hoTen }}
              </option>
            </select>
          </template>
        </div>
      </div>
      
      <div class="flex gap-3 mt-4 xl:mt-0">
        <Button @click="openCreateRequestModal" variant="outline" class="border-caramel text-caramel hover:bg-caramel/10 bg-white rounded-lg shadow-sm font-bold px-5">
          <Plus class="w-4 h-4 mr-2" /> TẠO YÊU CẦU MỚI
        </Button>
        <Button @click="openCheckInModal" class="bg-caramel hover:bg-brown text-cream rounded-lg shadow-warm font-bold transition-all px-5">
          <Zap class="w-4 h-4 mr-2 fill-current" /> CHẤM CÔNG NGAY
        </Button>
      </div>
    </div>

    <!-- Wifi verification card removed -->

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
      <button 
        v-if="isManager"
        @click="activeTab = 'staff-active'" 
        class="pb-3 text-sm font-bold transition-colors relative whitespace-nowrap"
        :class="activeTab === 'staff-active' ? 'text-caramel' : 'text-muted-foreground hover:text-espresso'"
      >
        Nhân viên đang làm
        <span class="ml-1 px-1.5 py-0.5 rounded text-[10px] font-bold bg-[#CC8033]/20 text-[#CC8033]">{{ activeStaffLogs.length }}</span>
        <div v-if="activeTab === 'staff-active'" class="absolute bottom-[-2px] left-0 w-full h-0.5 bg-[#CC8033] rounded-t-full"></div>
      </button>
    </div>

    <!-- Bảng Lịch sử chấm công của tôi -->
    <div v-if="activeTab === 'checkin'" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden animate-in fade-in duration-300">
      <div class="p-5 border-b-2 border-cream-deep flex justify-between items-center">
        <h3 class="font-display text-lg text-espresso font-semibold">Lịch sử chấm công của tôi (Tháng này)</h3>
        <div class="flex gap-2">
          <span v-if="isInShift" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-success/10 text-success text-[10px] font-bold">
            <div class="w-1.5 h-1.5 rounded-full bg-success"></div> Đang trong ca
          </span>
          <span v-else class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-gray-100 text-gray-500 text-[10px] font-bold">
            <div class="w-1.5 h-1.5 rounded-full bg-gray-400"></div> Ngoài ca làm
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
            <tr v-for="(row, i) in checkinLogs" :key="i" class="border-t-2 border-cream-deep/60 hover:bg-cream/20 transition-colors">
              <td class="px-5 py-4 text-espresso font-medium">{{ row.date }}</td>
              <td class="px-5 py-4 text-espresso">
                <span class="px-2 py-1 bg-cream-deep rounded text-xs font-medium">{{ row.tenCa }}</span>
                <div v-if="row.ghiChu" class="text-[10px] text-muted-foreground mt-1.5 italic max-w-[150px] truncate" :title="row.ghiChu">
                  Ghi chú: {{ row.ghiChu }}
                </div>
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
                <span v-if="row.timeOut" class="text-espresso font-medium">{{ row.timeOut }}</span>
                <span v-else>
                  <button @click="startCheckOutForRow(row)" class="px-2.5 py-1 bg-destructive/10 border border-destructive/20 hover:bg-destructive/20 text-destructive text-[10px] font-bold rounded transition-colors uppercase whitespace-nowrap shadow-sm">
                    Kết ca
                  </button>
                </span>
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
            <tr v-if="checkinLogs.length === 0">
              <td colspan="7" class="py-8 text-center text-muted-foreground text-xs">Chưa có lịch sử chấm công trong tháng này.</td>
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
        <button @click="requestFilter = 'ChoDuyet'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'ChoDuyet' ? 'bg-caramel text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Chờ duyệt</button>
        <button @click="requestFilter = 'DaDuyet'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'DaDuyet' ? 'bg-success/80 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Đã duyệt</button>
        <button @click="requestFilter = 'TuChoi'" class="px-3 py-1.5 rounded-md text-xs font-bold transition-colors" :class="requestFilter === 'TuChoi' ? 'bg-destructive/80 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep'">Đã từ chối</button>
      </div>
      
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="bg-cream/50 text-left text-muted-foreground text-xs uppercase border-b-2 border-cream-deep">
              <th v-if="isManager" class="px-5 py-4 font-medium">Nhân viên</th>
              <th class="px-5 py-4 font-medium">Loại đơn</th>
              <th class="px-5 py-4 font-medium">Thời gian đăng ký</th>
              <th class="px-5 py-4 font-medium">Lý do</th>
              <th class="px-5 py-4 font-medium">Trạng thái</th>
              <th v-if="isManager" class="px-5 py-4 font-medium text-right">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in filteredMyRequests" :key="row.maDon" class="border-t border-cream-deep/60 hover:bg-cream/20 transition-colors">
              <td v-if="isManager" class="px-5 py-4 text-espresso font-bold">
                {{ row.tenNhanVien || authStore.user?.hoTen }}
              </td>
              <td class="px-5 py-4">
                <span class="px-2.5 py-1 rounded text-[11px] font-bold" :class="typeColors[row.loaiDon] || 'bg-gray-100 text-gray-700'">
                  {{ row.loaiDon }}
                </span>
              </td>
              <td class="px-5 py-4 text-espresso font-medium text-xs">
                {{ row.thoiGianLienQuan }}
              </td>
              <td class="px-5 py-4 text-muted-foreground text-xs">
                <p class="truncate max-w-[300px]" :title="row.lyDo">{{ row.lyDo }}</p>
              </td>
              <td class="px-5 py-4">
                <span v-if="row.trangThai === 'ChoDuyet'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-caramel/10 text-caramel text-[11px] font-bold border border-caramel/20">
                  <div class="w-1.5 h-1.5 rounded-full bg-caramel"></div> Chờ duyệt
                </span>
                <span v-else-if="row.trangThai === 'DaDuyet'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-success/10 text-success text-[11px] font-bold border border-success/20">
                  <div class="w-1.5 h-1.5 rounded-full bg-success"></div> Đã duyệt
                </span>
                <span v-else-if="row.trangThai === 'TuChoi'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-destructive/10 text-destructive text-[11px] font-bold border border-destructive/20">
                  <div class="w-1.5 h-1.5 rounded-full bg-destructive"></div> Đã từ chối
                </span>
              </td>
              <td v-if="isManager" class="px-5 py-4 text-right">
                <div class="flex justify-end gap-2" v-if="row.trangThai === 'ChoDuyet'">
                  <button @click="handleReviewRequest(row.maDon, 'DaDuyet')" class="px-2.5 py-1.5 bg-success text-white text-xs font-bold rounded-lg hover:bg-success/90 shadow-sm transition-colors uppercase">
                    Duyệt
                  </button>
                  <button @click="handleReviewRequest(row.maDon, 'TuChoi')" class="px-2.5 py-1.5 bg-red-500 text-white text-xs font-bold rounded-lg hover:bg-red-600 shadow-sm transition-colors uppercase">
                    Từ chối
                  </button>
                </div>
                <span v-else class="text-xs text-muted-foreground">-</span>
              </td>
            </tr>
            <tr v-if="filteredMyRequests.length === 0">
              <td :colspan="isManager ? 6 : 4" class="py-8 text-center text-muted-foreground text-xs">Không có đơn từ nào.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Tab 3: Danh sách nhân viên đang làm (Quản lý) -->
    <div v-if="activeTab === 'staff-active' && isManager" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden animate-in fade-in duration-300">
      <div class="p-5 border-b-2 border-cream-deep flex justify-between items-center bg-cream/20">
        <h3 class="font-display text-lg text-espresso font-semibold">Nhân viên đang trong ca làm việc</h3>
      </div>
      
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="bg-cream/50 text-left text-muted-foreground text-xs uppercase border-b-2 border-cream-deep">
              <th class="px-5 py-4 font-medium">Nhân viên</th>
              <th class="px-5 py-4 font-medium">Ngày vào</th>
              <th class="px-5 py-4 font-medium">Giờ vào</th>
              <th class="px-5 py-4 font-medium text-center">Ảnh vào</th>
              <th class="px-5 py-4 font-medium">Hành động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(row, i) in activeStaffLogs" :key="i" class="border-t border-cream-deep/60 hover:bg-cream/20 transition-colors">
              <td class="px-5 py-4 text-espresso font-bold">{{ row.tenCa.split(' - ')[0] }}</td>
              <td class="px-5 py-4 text-espresso font-medium">{{ row.date }}</td>
              <td class="px-5 py-4 text-espresso font-medium">{{ row.timeIn }}</td>
              <td class="px-5 py-4 text-center">
                <div class="relative group inline-block" v-if="row.imgIn">
                  <img :src="row.imgIn" class="w-8 h-8 rounded-full border-2 border-cream-deep object-cover cursor-pointer" />
                  <div class="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 w-48 bg-espresso text-cream rounded-xl p-3 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-10 shadow-warm pointer-events-none">
                    <img :src="row.imgIn" class="w-full h-auto rounded-lg mb-2 object-cover aspect-[4/3]" />
                    <div class="text-center">
                      <div class="text-xs font-bold text-caramel-light">Ảnh Check-In</div>
                      <div class="text-[10px] text-muted-foreground mt-1">Lúc {{ row.timeInExact }}</div>
                    </div>
                  </div>
                </div>
                <span v-else class="text-xs text-muted-foreground">-</span>
              </td>
              <td class="px-5 py-4">
                <button @click="openForceCheckoutModal(row)" class="px-3 py-1.5 bg-red-500 hover:bg-red-600 text-white text-xs font-bold rounded-lg shadow-sm transition-colors uppercase whitespace-nowrap">
                  Kết ca hộ
                </button>
              </td>
            </tr>
            <tr v-if="activeStaffLogs.length === 0">
              <td colspan="5" class="py-8 text-center text-muted-foreground text-xs">Hiện không có nhân viên nào đang trong ca.</td>
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
          <div class="text-center mb-6 space-y-3">
            <h2 class="font-display text-2xl font-bold text-espresso mb-1">Chấm Công</h2>
            
            <div class="space-y-1.5 text-left max-w-[280px] mx-auto">
              <label class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold">Nhân viên chấm công</label>
              <select v-model="selectedEmployeeId" @change="onCheckinEmployeeChange" class="w-full bg-cream border border-cream-deep rounded-xl px-3 py-2 text-sm text-espresso font-semibold focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option v-for="emp in filteredEmployeeList" :key="emp.maNhanVien" :value="emp.maNhanVien">
                  {{ emp.hoTen }} {{ emp.maNhanVien === authStore.user?.maNhanVien ? '(Tôi)' : '' }}
                </option>
              </select>
            </div>
          </div>

          <div v-if="checkInStep === 1" class="space-y-6">
            <div class="bg-cream p-4 rounded-xl border border-cream-deep text-center">
              <div class="text-xs font-bold text-caramel uppercase tracking-widest mb-1">Lượt hoạt động</div>
              <div v-if="isInShift" class="text-sm font-semibold text-espresso">
                Đã vào ca lúc {{ checkinLogs[0]?.timeIn }} ({{ checkinLogs[0]?.tenCa || 'Ca Tự Do' }})
              </div>
              <div v-else class="text-sm font-semibold text-muted-foreground">
                Chưa vào ca làm việc
              </div>
            </div>

            <!-- Bộ chọn ca làm việc khi vào ca -->
            <div v-if="!isInShift" class="space-y-1.5 text-left max-w-[280px] mx-auto">
              <label class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold">Chọn ca làm việc</label>
              <select v-model="selectedShiftId" class="w-full bg-cream border border-cream-deep rounded-xl px-3 py-2 text-sm text-espresso font-semibold focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option :value="null">Ca tự do (Không theo lịch)</option>
                <option v-for="shift in activeShifts" :key="shift.maCa" :value="shift.maCa">
                  {{ shift.tenCa }} ({{ shift.gioBatDau.slice(0, 5) }} - {{ shift.gioKetThuc.slice(0, 5) }})
                </option>
              </select>
            </div>

            <!-- Banner cảnh báo/thông báo logic -->
            <div v-if="!isInShift" :class="[
              'p-3.5 rounded-xl border flex items-start gap-2.5 text-xs transition-colors',
              shiftInfoMessage.type === 'warning' ? 'bg-warning/10 text-[#a26207] border-warning/20' :
              shiftInfoMessage.type === 'late' ? 'bg-destructive/10 text-destructive border-destructive/20' :
              shiftInfoMessage.type === 'success' ? 'bg-success/10 text-success border-success/20' :
              'bg-cream-deep text-muted-foreground border-transparent'
            ]">
              <Info class="w-4 h-4 shrink-0 mt-0.5 text-caramel" />
              <div>
                <span class="font-bold text-espresso block mb-0.5">Thông tin ca làm</span>
                {{ shiftInfoMessage.text }}
              </div>
            </div>
            
            <div class="grid grid-cols-2 gap-3">
              <button @click="startCamera('in')" :disabled="isInShift" class="flex flex-col items-center justify-center py-6 rounded-xl border border-success/30 bg-success/10 hover:bg-success/20 disabled:opacity-40 disabled:hover:bg-success/10 disabled:cursor-not-allowed transition-colors group shadow-sm">
                <LogIn class="w-8 h-8 text-success mb-2 group-hover:scale-110 transition-transform" />
                <span class="font-bold text-success text-sm tracking-wide">VÀO CA</span>
              </button>
              <button @click="startCamera('out')" :disabled="!isInShift" class="flex flex-col items-center justify-center py-6 rounded-xl border border-destructive/30 bg-destructive/10 hover:bg-destructive/20 disabled:opacity-40 disabled:hover:bg-destructive/10 disabled:cursor-not-allowed transition-colors group shadow-sm">
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
              <video ref="videoElement" class="w-full h-full object-cover" autoplay playsinline></video>
              <div v-if="!cameraActive" class="absolute inset-0 flex items-center justify-center bg-espresso">
                <img src="https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400&q=80" class="w-full h-full object-cover opacity-80" />
              </div>
              <div class="absolute inset-6 border-2 border-dashed border-white/60 rounded-[30%] animate-pulse pointer-events-none"></div>
            </div>

            <div class="flex justify-center">
              <button @click="takePhoto" class="w-16 h-16 rounded-full bg-caramel hover:bg-brown flex items-center justify-center text-white shadow-[0_0_20px_rgba(200,133,58,0.4)] transition-all active:scale-95 group border-4 border-white">
                <Camera class="w-7 h-7 group-hover:scale-110 transition-transform" />
              </button>
            </div>
          </div>

          <div v-else-if="checkInStep === 3" class="space-y-4 animate-in slide-in-from-right-4 duration-300">
            <div class="text-center space-y-1">
              <p class="font-bold text-espresso">Kiểm tra thông tin</p>
              <p class="text-[10px] text-muted-foreground uppercase tracking-widest">Đảm bảo ảnh rõ nét</p>
            </div>

            <div class="relative w-full aspect-[4/3] rounded-2xl overflow-hidden border-4 border-cream-deep shadow-inner">
              <img :src="photoUrl" class="w-full h-full object-cover" />
              <div class="absolute bottom-3 right-3 bg-espresso/80 backdrop-blur-md px-3 py-1 rounded-full text-[10px] text-cream font-medium">
                {{ new Date().toLocaleTimeString('vi-VN') }}
              </div>
            </div>

            <div class="space-y-1.5">
              <label class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold">Ghi chú (Không bắt buộc)</label>
              <input v-model="checkInNotes" placeholder="Ví dụ: Đi trễ do kẹt xe..." class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-2.5 text-xs text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20" />
            </div>

            <div class="grid grid-cols-2 gap-3 pt-2">
              <button @click="checkInStep = 2" class="py-3 px-4 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso text-xs font-bold transition-colors">
                Chụp lại
              </button>
              <button @click="confirmCheckIn" :disabled="submittingCheckin" class="py-3 px-4 rounded-xl border border-caramel bg-caramel hover:bg-brown text-white text-xs font-bold transition-colors shadow-warm uppercase tracking-wide flex items-center justify-center gap-1.5">
                <span v-if="submittingCheckin">Đang gửi...</span>
                <span v-else>Xác Nhận</span>
              </button>
            </div>
          </div>


        </div>
      </div>
    </div>

    <!-- Modal Tạo Đơn -->
    <div v-if="showCreateModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[450px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="showCreateModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6">
          <h2 class="font-display text-2xl font-bold text-espresso mb-6 text-center">Tạo yêu cầu mới</h2>
          
          <div class="space-y-5">
            <div class="space-y-1.5" v-if="isManager">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Nhân viên yêu cầu</label>
              <select v-model="formRequest.maNhanVien" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option v-for="emp in filteredEmployeeList" :key="emp.maNhanVien" :value="emp.maNhanVien">
                  {{ emp.hoTen }} {{ emp.maNhanVien === authStore.user?.maNhanVien ? '(Tôi)' : '' }}
                </option>
              </select>
            </div>
            
            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Loại đơn</label>
              <select v-model="formRequest.loaiDon" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option value="PhepNam">Xin nghỉ phép năm</option>
                <option value="TangCa">Xin tăng ca (OT)</option>
                <option value="NghiKhongLuong">Xin nghỉ không lương</option>
                <option value="NghiBu">Xin nghỉ bù</option>
              </select>
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div class="space-y-1.5">
                <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ngày áp dụng</label>
                <input v-model="requestDate" type="date" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20" />
              </div>

              <div class="space-y-1.5">
                <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ca làm việc</label>
                <select v-model="requestShift" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                  <option value="">Chọn ca làm</option>
                  <option v-for="shift in activeShifts" :key="shift.maCa" :value="shift.tenCa">
                    {{ shift.tenCa }} ({{ shift.gioBatDau }} - {{ shift.gioKetThuc }})
                  </option>
                  <option value="Ca Tự Do">Ca Tự Do</option>
                </select>
              </div>
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Lý do</label>
              <textarea v-model="formRequest.lyDo" rows="3" placeholder="Nhập lý do chi tiết..." class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20 resize-none"></textarea>
            </div>
          </div>

          <div class="flex gap-3 mt-8">
            <button @click="showCreateModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy bỏ
            </button>
            <button @click="submitRequest" :disabled="submittingRequest" class="flex-1 py-3 rounded-xl bg-caramel hover:bg-brown text-white font-bold text-sm transition-colors shadow-warm uppercase tracking-wider">
              {{ submittingRequest ? 'Đang gửi...' : 'Gửi Đơn' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Kết Ca Hộ (Quản lý) -->
    <div v-if="showForceCheckoutModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[400px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="showForceCheckoutModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6">
          <h2 class="font-display text-2xl font-bold text-espresso mb-1 text-center">Kết ca hộ nhân viên</h2>
          <p class="text-xs text-muted-foreground text-center mb-6">
            Nhân viên: <span class="font-semibold text-espresso">{{ selectedActiveStaffLog?.tenCa.split(' - ')[0] }}</span>
          </p>

          <div class="space-y-4">
            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Lý do kết ca hộ</label>
              <textarea v-model="forceCheckoutReason" rows="3" placeholder="Nhập lý do kết ca hộ (ví dụ: Nhân viên quên bấm kết ca, về sớm có việc đột xuất...)" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20 resize-none"></textarea>
            </div>
          </div>

          <div class="flex gap-3 mt-6">
            <button @click="showForceCheckoutModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy bỏ
            </button>
            <button @click="confirmForceCheckout" :disabled="submittingForceCheckout" class="flex-1 py-3 rounded-xl bg-destructive hover:bg-destructive/90 text-white font-bold text-sm transition-colors shadow-warm uppercase tracking-wider">
              {{ submittingForceCheckout ? 'Đang lưu...' : 'Xác nhận' }}
            </button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { Plus, X, Zap, Camera, LogIn, LogOut, Info } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import { useAuthStore } from '@/stores/auth'
import { useToast } from '@/stores/toast'
import { hrApi, type ChamCongItem, type DonTuItem } from '@/services/hr'

const authStore = useAuthStore()
const toast = useToast()

const activeTab = ref<'checkin' | 'requests' | 'staff-active'>('checkin')
const requestFilter = ref<'all' | 'ChoDuyet' | 'DaDuyet' | 'TuChoi'>('all')

const isManager = computed(() => authStore.user?.vaiTro === 'Quản lý')
const activeStaffLogs = ref<ChamCongItem[]>([])

// --- WiFi verification bypassed ---
const checkWiFiIP = () => true

const openCheckInModal = () => {
  if (!checkWiFiIP()) return
  if (authStore.user) {
    const isCurrentUserAdmin = authStore.user.hoTen === 'Quản trị viên'
    selectedEmployeeId.value = isCurrentUserAdmin 
      ? (filteredEmployeeList.value[0]?.maNhanVien || null) 
      : authStore.user.maNhanVien
    onCheckinEmployeeChange()
  }
  showCheckInModal.value = true
}

const startCheckOutForRow = (row: ChamCongItem) => {
  if (!checkWiFiIP()) return
  startCamera('out')
  showCheckInModal.value = true
}

// --- HR State ---
const checkinLogs = ref<ChamCongItem[]>([])
const requestLogs = ref<DonTuItem[]>([])
const loadingData = ref(false)

const isInShift = computed(() => {
  if (checkinLogs.value.length === 0) return false
  const latest = checkinLogs.value[0]
  // Nếu có dòng chấm công mà chưa check-out thì là đang trong ca
  return latest && !latest.timeOut
})

// --- Logical Shift and Checkin Matching State & Logic ---
const activeShifts = ref<{ maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }[]>([])
const selectedShiftId = ref<number | null>(null)
const shiftInfoMessage = ref({ text: 'Nhân viên sẵn sàng nhận ca.', type: 'info' })

const loadShifts = async () => {
  try {
    activeShifts.value = await hrApi.getShifts()
  } catch (err) {
    console.error("Failed to load shifts", err)
  }
}

const getTodayDayCode = () => {
  const day = new Date().getDay() // 0 = Sunday, 1 = Monday, ..., 6 = Saturday
  const map = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']
  return map[day]
}

const selectedEmployeeName = computed(() => {
  const emp = employeeList.value.find(x => x.maNhanVien === selectedEmployeeId.value)
  return emp ? emp.hoTen : ''
})

const updateAutoShiftSelection = () => {
  const name = selectedEmployeeName.value
  if (!name) return

  const saved = localStorage.getItem('quanlycf_schedule')
  let scheduledShift: string | null = null

  if (saved) {
    try {
      const scheduleData = JSON.parse(saved)
      const todayCode = getTodayDayCode()
      const todayList = scheduleData[todayCode] || []

      const shortNameMap: Record<string, string> = {
        "Lan Trần": "Lan T.",
        "Khoa Phạm": "Khoa P.",
        "Vy Hoàng": "Vy H.",
        "Nam Lê": "Nam L.",
        "Thảo Vũ": "Thảo V."
      }
      const targetShortName = shortNameMap[name] || name
      const scheduledItem = todayList.find((x: any) => x.staff === targetShortName)
      if (scheduledItem) {
        scheduledShift = scheduledItem.shift // 'morning', 'afternoon' or 'evening'
      }
    } catch (e) {
      console.error(e)
    }
  }

  if (scheduledShift) {
    const shiftNameKeywordMap = {
      morning: 'sáng',
      afternoon: 'chiều',
      evening: 'tối'
    }
    const keyword = shiftNameKeywordMap[scheduledShift as keyof typeof shiftNameKeywordMap]
    const matched = activeShifts.value.find(s => s.tenCa.toLowerCase().includes(keyword))

    if (matched) {
      selectedShiftId.value = matched.maCa
      
      const now = new Date()
      const currentHour = now.getHours()
      const currentMinute = now.getMinutes()
      const currentTimeVal = currentHour * 60 + currentMinute

      // morning: 06:00 - 12:00 -> 360 to 720
      // afternoon: 12:00 - 17:00 -> 720 to 1020
      // evening: 17:00 - 22:00 -> 1020 to 1320
      const shiftTimes: Record<string, { start: number; end: number; label: string }> = {
        morning: { start: 360, end: 720, label: 'Ca Sáng (06:00 - 12:00)' },
        afternoon: { start: 720, end: 1020, label: 'Ca Chiều (12:00 - 17:00)' },
        evening: { start: 1020, end: 1320, label: 'Ca Tối (17:00 - 22:00)' }
      }

      const timeRange = shiftTimes[scheduledShift]
      if (timeRange) {
        if (currentTimeVal < timeRange.start - 30) {
          const diffMin = timeRange.start - currentTimeVal
          const diffHrs = Math.floor(diffMin / 60)
          const diffMins = diffMin % 60
          const timeStr = diffHrs > 0 ? `${diffHrs}h ${diffMins}m` : `${diffMins} phút`
          shiftInfoMessage.value = {
            text: `⚠️ Chưa đến giờ làm việc chính thức cho ${timeRange.label}. Bạn đang đến sớm ${timeStr}.`,
            type: 'warning'
          }
        } else if (currentTimeVal > timeRange.start + 15) {
          const lateMin = currentTimeVal - timeRange.start
          const lateHrs = Math.floor(lateMin / 60)
          const lateMins = lateMin % 60
          const timeStr = lateHrs > 0 ? `${lateHrs}h ${lateMins}m` : `${lateMins} phút`
          shiftInfoMessage.value = {
            text: `⚠️ Bạn đang đi trễ ${timeStr} so với giờ bắt đầu ${timeRange.label}.`,
            type: 'late'
          }
        } else {
          shiftInfoMessage.value = {
            text: `💡 Bạn có lịch làm việc ${timeRange.label} hôm nay. Trạng thái: Đúng giờ.`,
            type: 'success'
          }
        }
      }
    } else {
      selectedShiftId.value = null
      shiftInfoMessage.value = { text: 'Nhân viên sẵn sàng nhận ca (Ca Tự Do).', type: 'info' }
    }
  } else {
    selectedShiftId.value = null
    shiftInfoMessage.value = {
      text: '💡 Hôm nay bạn không có lịch xếp ca. Bạn sẽ chấm công vào Ca Tự Do.',
      type: 'info'
    }
  }
}

watch(selectedShiftId, (newId) => {
  if (newId === null) {
    shiftInfoMessage.value = {
      text: '💡 Bạn đã chọn Ca Tự Do (không theo lịch xếp ca).',
      type: 'info'
    }
    return
  }
  
  const name = selectedEmployeeName.value
  const saved = localStorage.getItem('quanlycf_schedule')
  let scheduledShift: string | null = null

  if (saved && name) {
    try {
      const scheduleData = JSON.parse(saved)
      const todayCode = getTodayDayCode()
      const todayList = scheduleData[todayCode] || []

      const shortNameMap: Record<string, string> = {
        "Lan Trần": "Lan T.",
        "Khoa Phạm": "Khoa P.",
        "Vy Hoàng": "Vy H.",
        "Nam Lê": "Nam L.",
        "Thảo Vũ": "Thảo V."
      }
      const targetShortName = shortNameMap[name] || name
      const scheduledItem = todayList.find((x: any) => x.staff === targetShortName)
      if (scheduledItem) {
        scheduledShift = scheduledItem.shift
      }
    } catch (e) {
      console.error(e)
    }
  }

  const selectedShiftObj = activeShifts.value.find(s => s.maCa === newId)
  if (selectedShiftObj) {
    const shiftName = selectedShiftObj.tenCa.toLowerCase()
    
    if (scheduledShift) {
      const shiftNameKeywordMap = {
        morning: 'sáng',
        afternoon: 'chiều',
        evening: 'tối'
      }
      const keyword = shiftNameKeywordMap[scheduledShift as keyof typeof shiftNameKeywordMap]
      if (shiftName.includes(keyword)) {
        const now = new Date()
        const currentHour = now.getHours()
        const currentMinute = now.getMinutes()
        const currentTimeVal = currentHour * 60 + currentMinute

        const shiftTimes: Record<string, { start: number; end: number; label: string }> = {
          morning: { start: 360, end: 720, label: 'Ca Sáng (06:00 - 12:00)' },
          afternoon: { start: 720, end: 1020, label: 'Ca Chiều (12:00 - 17:00)' },
          evening: { start: 1020, end: 1320, label: 'Ca Tối (17:00 - 22:00)' }
        }

        const timeRange = shiftTimes[scheduledShift]
        if (currentTimeVal < timeRange.start - 30) {
          const diffMin = timeRange.start - currentTimeVal
          const diffHrs = Math.floor(diffMin / 60)
          const diffMins = diffMin % 60
          const timeStr = diffHrs > 0 ? `${diffHrs}h ${diffMins}m` : `${diffMins} phút`
          shiftInfoMessage.value = {
            text: `⚠️ Chưa đến giờ làm việc chính thức cho ${timeRange.label}. Bạn đang đến sớm ${timeStr}.`,
            type: 'warning'
          }
        } else if (currentTimeVal > timeRange.start + 15) {
          const lateMin = currentTimeVal - timeRange.start
          const lateHrs = Math.floor(lateMin / 60)
          const lateMins = lateMin % 60
          const timeStr = lateHrs > 0 ? `${lateHrs}h ${lateMins}m` : `${lateMins} phút`
          shiftInfoMessage.value = {
            text: `⚠️ Bạn đang đi trễ ${timeStr} so với giờ bắt đầu ${timeRange.label}.`,
            type: 'late'
          }
        } else {
          shiftInfoMessage.value = {
            text: `💡 Bạn có lịch làm việc ${timeRange.label} hôm nay. Trạng thái: Đúng giờ.`,
            type: 'success'
          }
        }
        return
      }
    }
    
    if (scheduledShift) {
      const shiftNameKeywordMap = {
        morning: 'Ca Sáng',
        afternoon: 'Ca Chiều',
        evening: 'Ca Tối'
      }
      const label = shiftNameKeywordMap[scheduledShift as keyof typeof shiftNameKeywordMap] || scheduledShift
      shiftInfoMessage.value = {
        text: `⚠️ Bạn đang chọn ${selectedShiftObj.tenCa} nhưng hôm nay bạn được lịch xếp ca là ${label}.`,
        type: 'warning'
      }
    } else {
      shiftInfoMessage.value = {
        text: `💡 Bạn đã chọn ${selectedShiftObj.tenCa}. Hôm nay bạn không có lịch xếp ca.`,
        type: 'info'
      }
    }
  }
})

// --- Check-in State ---
const showCheckInModal = ref(false)
const showCreateModal = ref(false)
const showForceCheckoutModal = ref(false)
const forceCheckoutReason = ref('')
const selectedActiveStaffLog = ref<ChamCongItem | null>(null)
const submittingForceCheckout = ref(false)

const employeeList = ref<{ maNhanVien: number; hoTen: string }[]>([])
const selectedEmployeeId = ref<number | null>(null)
const selectedViewEmployeeId = ref<number | null>(null)

const filteredEmployeeList = computed(() => {
  return employeeList.value.filter(emp => emp.hoTen !== 'Quản trị viên')
})

const onViewEmployeeChange = async () => {
  if (!selectedViewEmployeeId.value) return
  loadingData.value = true
  try {
    checkinLogs.value = await hrApi.getMyCheckins(selectedViewEmployeeId.value)
  } catch (err) {
    console.error("Failed to load check-ins for viewed employee", err)
  } finally {
    loadingData.value = false
  }
}

const loadEmployees = async () => {
  try {
    employeeList.value = await hrApi.getEmployees()
  } catch (err) {
    console.error("Failed to load employees", err)
  }
}

const onCheckinEmployeeChange = async () => {
  if (!selectedEmployeeId.value) return
  try {
    checkinLogs.value = await hrApi.getMyCheckins(selectedEmployeeId.value)
    updateAutoShiftSelection()
  } catch (err) {
    console.error("Failed to load check-ins for employee", err)
  }
}

const checkInStep = ref<1 | 2 | 3>(1)
const checkInType = ref<'in' | 'out'>('in')
const photoUrl = ref('')
const checkInNotes = ref('')
const submittingCheckin = ref(false)

// Camera logic
const videoElement = ref<HTMLVideoElement | null>(null)
const cameraActive = ref(false)
let mediaStream: MediaStream | null = null

const startCamera = async (type: 'in' | 'out') => {
  checkInType.value = type
  checkInStep.value = 2
  cameraActive.value = false
  photoUrl.value = ''
  try {
    mediaStream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'user' } })
    if (videoElement.value) {
      videoElement.value.srcObject = mediaStream
      videoElement.value.onloadedmetadata = () => {
        cameraActive.value = true
      }
    }
  } catch (err) {
    console.error("Camera access failed", err)
    // Tự động dùng fallback ảnh giả lập sau 1s nếu ko bật đc camera
    setTimeout(() => {
      cameraActive.value = false
    }, 500)
  }
}

const takePhoto = () => {
  if (videoElement.value && cameraActive.value && mediaStream) {
    try {
      const canvas = document.createElement('canvas')
      canvas.width = videoElement.value.videoWidth || 640
      canvas.height = videoElement.value.videoHeight || 480
      const ctx = canvas.getContext('2d')
      if (ctx) {
        ctx.drawImage(videoElement.value, 0, 0, canvas.width, canvas.height)
        photoUrl.value = canvas.toDataURL('image/jpeg')
      }
    } catch (e) {
      photoUrl.value = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400&q=80"
    }
  } else {
    // Fallback ảnh giả lập nếu không có camera thật
    photoUrl.value = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400&q=80"
  }
  stopCamera()
  checkInStep.value = 3
}

const stopCamera = () => {
  if (mediaStream) {
    mediaStream.getTracks().forEach(track => track.stop())
    mediaStream = null
  }
  cameraActive.value = false
}

const closeCheckIn = () => {
  stopCamera()
  showCheckInModal.value = false
  checkInNotes.value = ''
  setTimeout(() => { checkInStep.value = 1 }, 200)
}

const confirmCheckIn = async () => {
  if (!checkWiFiIP()) {
    closeCheckIn()
    return
  }
  submittingCheckin.value = true
  try {
    const res = await hrApi.checkIn({
      type: checkInType.value,
      maCa: selectedShiftId.value,
      photoUrl: photoUrl.value,
      ghiChu: checkInNotes.value,
      maNhanVien: selectedEmployeeId.value
    })
    toast.success(res.message, 'Chấm công')
    closeCheckIn()
    await loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi chấm công')
  } finally {
    submittingCheckin.value = false
  }
}

// --- Requests State ---
const formRequest = ref<{ loaiDon: string; thoiGianLienQuan: string; lyDo: string; maNhanVien: number | null }>({
  loaiDon: 'PhepNam',
  thoiGianLienQuan: '',
  lyDo: '',
  maNhanVien: null
})
const submittingRequest = ref(false)
const requestDate = ref('')
const requestShift = ref('')
const openCreateRequestModal = () => {
  const isCurrentUserAdmin = authStore.user?.hoTen === 'Quản trị viên'
  formRequest.value = {
    loaiDon: 'PhepNam',
    thoiGianLienQuan: '',
    lyDo: '',
    maNhanVien: isCurrentUserAdmin 
      ? (filteredEmployeeList.value[0]?.maNhanVien || null) 
      : (authStore.user?.maNhanVien || null)
  }
  requestDate.value = ''
  requestShift.value = ''
  showCreateModal.value = true
  loadShifts()
}

const submitRequest = async () => {
  if (!requestDate.value || !requestShift.value || !formRequest.value.lyDo.trim()) {
    toast.warning('Vui lòng điền đầy đủ các thông tin.', 'Thiếu thông tin')
    return
  }
  
  // Định dạng chuỗi ngày tháng tiếng Việt thân thiện
  const d = new Date(requestDate.value)
  const dayNames = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy']
  const dayName = dayNames[d.getDay()]
  const dateStr = d.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
  const formattedThoiGian = `${dayName}, ${dateStr} (${requestShift.value})`

  submittingRequest.value = true
  try {
    const res = await hrApi.createRequest({
      loaiDon: formRequest.value.loaiDon,
      thoiGianLienQuan: formattedThoiGian,
      lyDo: formRequest.value.lyDo,
      maNhanVien: formRequest.value.maNhanVien
    })
    toast.success(res.message, 'Gửi đơn từ')
    showCreateModal.value = false
    await loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi gửi đơn')
  } finally {
    submittingRequest.value = false
  }
}

const typeColors: Record<string, string> = {
  'Phép năm': 'bg-blue-100 text-blue-700',
  'Tăng ca': 'bg-purple-100 text-purple-700',
  'Nghỉ không lương': 'bg-gray-100 text-gray-700',
  'Nghỉ bù': 'bg-orange-100 text-orange-700',
}

const filteredMyRequests = computed(() => {
  if (requestFilter.value === 'all') return requestLogs.value
  return requestLogs.value.filter(r => r.trangThai === requestFilter.value)
})

// --- Init & Load ---
const loadData = async () => {
  loadingData.value = true
  try {
    const targetCheckinEmpId = (isManager.value && selectedViewEmployeeId.value)
      ? selectedViewEmployeeId.value
      : undefined

    const promises: [Promise<any>, Promise<any>, Promise<any>?] = [
      hrApi.getMyCheckins(targetCheckinEmpId),
      isManager.value ? hrApi.getAllRequests() : hrApi.getMyRequests()
    ]
    if (isManager.value) {
      promises.push(hrApi.getActiveCheckins())
    }
    const [checkins, requests, activeStaff] = await Promise.all(promises)
    checkinLogs.value = checkins
    requestLogs.value = requests
    if (activeStaff) {
      activeStaffLogs.value = activeStaff
    }
  } catch (err: any) {
    console.error("Failed to load HR data", err)
  } finally {
    loadingData.value = false
  }
}

const handleReviewRequest = async (id: number, status: 'DaDuyet' | 'TuChoi') => {
  try {
    const res = await hrApi.reviewRequest(id, status)
    toast.success(res.message, 'Duyệt đơn từ')
    await loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi duyệt đơn từ')
  }
}

const openForceCheckoutModal = (row: ChamCongItem) => {
  selectedActiveStaffLog.value = row
  forceCheckoutReason.value = ''
  showForceCheckoutModal.value = true
}

const confirmForceCheckout = async () => {
  if (!selectedActiveStaffLog.value) return
  if (!forceCheckoutReason.value.trim()) {
    toast.warning('Vui lòng nhập lý do kết ca hộ.', 'Thiếu thông tin')
    return
  }
  submittingForceCheckout.value = true
  try {
    const res = await hrApi.forceCheckout(selectedActiveStaffLog.value.maChamCong, forceCheckoutReason.value.trim())
    toast.success(res.message, 'Quản lý kết ca')
    showForceCheckoutModal.value = false
    await loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi kết ca hộ')
  } finally {
    submittingForceCheckout.value = false
  }
}

onMounted(async () => {
  await loadEmployees()
  await loadShifts()
  if (authStore.user) {
    const isCurrentUserAdmin = authStore.user.hoTen === 'Quản trị viên'
    selectedViewEmployeeId.value = isCurrentUserAdmin 
      ? (filteredEmployeeList.value[0]?.maNhanVien || null) 
      : authStore.user.maNhanVien
    selectedEmployeeId.value = isCurrentUserAdmin 
      ? (filteredEmployeeList.value[0]?.maNhanVien || null) 
      : authStore.user.maNhanVien
    updateAutoShiftSelection()
  }
  await loadData()
})

onUnmounted(() => {
  stopCamera()
})
</script>
