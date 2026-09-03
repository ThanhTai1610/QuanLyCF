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

    <!-- Bộ lọc chọn Nhân viên dành cho Quản lý / Admin -->
    <div v-if="isManager && activeTab === 'checkin'" class="mb-4 bg-card p-4 rounded-xl border border-cream-deep shadow-xs">
      <div class="flex items-center justify-between mb-2.5">
        <span class="text-xs uppercase font-bold text-espresso tracking-wider flex items-center gap-1.5">
          <Users class="w-4 h-4 text-caramel" />
          <span>Danh sách nhân viên (Bấm để xem chi tiết chấm công từng ngày):</span>
        </span>
        <span class="text-xs text-muted-foreground font-medium">
          Đang xem: <strong class="text-caramel font-bold">{{ currentViewEmployeeName }}</strong>
        </span>
      </div>

      <div class="flex items-center gap-2 flex-wrap">
        <button 
          v-for="emp in filteredEmployeeList" 
          :key="emp.maNhanVien" 
          @click="selectStaffView(emp.maNhanVien)" 
          class="px-3.5 py-2 rounded-xl text-xs font-bold transition-all flex items-center gap-2 cursor-pointer border"
          :class="selectedViewEmployeeId === emp.maNhanVien ? 'bg-espresso text-cream border-espresso shadow-md scale-102' : 'bg-cream/60 text-espresso border-cream-deep hover:bg-cream hover:border-caramel/40'"
        >
          <div class="w-5 h-5 rounded-full bg-caramel text-cream text-[10px] font-extrabold flex items-center justify-center shrink-0">
            {{ emp.hoTen.charAt(0) }}
          </div>
          <span>{{ emp.hoTen }}</span>
          <span v-if="emp.role" class="text-[10px] opacity-85 font-normal">({{ emp.role }})</span>
          <span v-else class="text-[10px] opacity-85 font-normal">(#{{ emp.maNhanVien }})</span>
        </button>
      </div>
    </div>

    <!-- Bảng Lịch sử chấm công -->
    <div v-if="activeTab === 'checkin'" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden animate-in fade-in duration-300">
      <div class="p-5 border-b-2 border-cream-deep flex justify-between items-center bg-cream/20">
        <h3 class="font-display text-base sm:text-lg text-espresso font-semibold flex items-center gap-2 flex-wrap">
          <span>Lịch sử chấm công:</span>
          <span class="text-caramel font-bold underline">{{ currentViewEmployeeName }}</span>
          <span class="text-xs text-muted-foreground font-normal">(Tháng này)</span>
        </h3>
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
              <th class="px-5 py-4 font-medium">Trạng thái</th>
              <th v-if="isManager" class="px-5 py-4 font-medium text-right">Duyệt công</th>
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
                <div class="relative group inline-block" v-if="row.imgIn" @click="openImagePreview(row.imgIn, `Ảnh Xác thực Vào Ca - ${row.date} (${row.timeIn})`)">
                  <img :src="row.imgIn" class="w-8 h-8 rounded-full border-2 border-cream-deep hover:border-caramel object-cover cursor-pointer transition-all shadow-xs" />
                  <div class="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 w-48 bg-espresso text-cream rounded-xl p-3 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-10 shadow-warm pointer-events-none">
                    <img :src="row.imgIn" class="w-full h-auto rounded-lg mb-2 object-cover aspect-[4/3]" />
                    <div class="text-center">
                      <div class="text-xs font-bold text-caramel-light">Xác thực Vào Ca (Bấm xem to)</div>
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
                <div class="relative group inline-block" v-if="row.imgOut" @click="openImagePreview(row.imgOut, `Ảnh Xác thực Kết Ca - ${row.date} (${row.timeOut})`)">
                  <img :src="row.imgOut" class="w-8 h-8 rounded-full border-2 border-cream-deep hover:border-caramel object-cover cursor-pointer transition-all shadow-xs" />
                  <div class="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 w-48 bg-espresso text-cream rounded-xl p-3 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all z-10 shadow-warm pointer-events-none">
                    <img :src="row.imgOut" class="w-full h-auto rounded-lg mb-2 object-cover aspect-[4/3]" />
                    <div class="text-center">
                      <div class="text-xs font-bold text-caramel-light">Xác thực Kết Ca (Bấm xem to)</div>
                      <div class="text-[10px] text-muted-foreground mt-1">Lúc {{ row.timeOutExact }}</div>
                    </div>
                  </div>
                </div>
                <span v-else class="text-xs text-muted-foreground">-</span>
              </td>
              <td class="px-5 py-4 text-espresso font-bold">{{ row.total || '-' }}</td>
              <td class="px-5 py-4">
                <span v-if="row.trangThai === 'ChoDuyet'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-amber-500/10 text-amber-600 text-[11px] font-bold border border-amber-500/20 whitespace-nowrap">
                  <div class="w-1.5 h-1.5 rounded-full bg-amber-500 animate-ping"></div> Chờ Admin duyệt
                </span>
                <span v-else-if="row.trangThai === 'DaDuyet' || row.trangThai === 'HopLe'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-emerald-500/10 text-emerald-600 text-[11px] font-bold border border-emerald-500/20 whitespace-nowrap">
                  <div class="w-1.5 h-1.5 rounded-full bg-emerald-500"></div> Đã duyệt công
                </span>
                <span v-else-if="row.trangThai === 'TuChoi' || row.trangThai === 'KhongHopLe'" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-red-500/10 text-red-600 text-[11px] font-bold border border-red-500/20 whitespace-nowrap">
                  <div class="w-1.5 h-1.5 rounded-full bg-red-500"></div> Từ chối công
                </span>
              </td>
              <td v-if="isManager" class="px-5 py-4 text-right">
                <div class="flex justify-end gap-1.5" v-if="row.trangThai === 'ChoDuyet'">
                  <button @click="handleReviewCheckIn(row.maChamCong, 'DaDuyet')" class="px-2.5 py-1.5 bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-bold rounded-lg shadow-sm transition-all uppercase cursor-pointer">
                    Duyệt
                  </button>
                  <button @click="handleReviewCheckIn(row.maChamCong, 'TuChoi')" class="px-2.5 py-1.5 bg-red-500 hover:bg-red-600 text-white text-xs font-bold rounded-lg shadow-sm transition-all uppercase cursor-pointer">
                    Từ chối
                  </button>
                </div>
                <span v-else class="text-xs text-muted-foreground font-medium">Đã xử lý</span>
              </td>
            </tr>
            <tr v-if="checkinLogs.length === 0">
              <td :colspan="isManager ? 9 : 8" class="py-8 text-center text-muted-foreground text-xs">Chưa có lịch sử chấm công trong tháng này.</td>
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

                <!-- Lý do từ chối của Admin hiển thị cho Nhân viên -->
                <div v-if="row.trangThai === 'TuChoi' && row.ghiChuDuyet" class="mt-1.5 p-2 rounded-lg bg-red-500/10 border border-red-500/20 text-red-600 text-[11px] font-medium max-w-[280px]">
                  <span class="font-bold block text-red-700">Lý do từ chối:</span>
                  {{ row.ghiChuDuyet }}
                </div>
              </td>
              <td v-if="isManager" class="px-5 py-4 text-right">
                <div class="flex justify-end gap-2" v-if="row.trangThai === 'ChoDuyet'">
                  <button @click="handleReviewRequest(row.maDon, 'DaDuyet')" class="px-2.5 py-1.5 bg-success text-white text-xs font-bold rounded-lg hover:bg-success/90 shadow-sm transition-colors uppercase cursor-pointer">
                    Duyệt
                  </button>
                  <button @click="openRejectModal(row.maDon)" class="px-2.5 py-1.5 bg-red-500 text-white text-xs font-bold rounded-lg hover:bg-red-600 shadow-sm transition-colors uppercase cursor-pointer">
                    Từ chối
                  </button>
                </div>
                <span v-else class="text-xs text-muted-foreground font-medium">Đã xử lý</span>
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
              <div class="w-full bg-cream border border-cream-deep rounded-xl px-3.5 py-2.5 text-sm font-bold text-espresso flex items-center gap-2.5 shadow-xs">
                <div class="w-6 h-6 rounded-full bg-caramel text-cream text-[11px] font-extrabold flex items-center justify-center shrink-0">
                  {{ (authStore.user?.hoTen || 'N').charAt(0) }}
                </div>
                <span class="truncate">{{ authStore.user?.hoTen || 'Tài khoản của bạn' }}</span>
              </div>
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

          <div v-else-if="checkInStep === 2" class="space-y-5 animate-in slide-in-from-right-4 duration-300">
            <div class="text-center space-y-1">
              <p class="font-bold text-espresso">{{ checkInType === 'in' ? 'Chụp ảnh Vào ca' : 'Chụp ảnh Kết ca' }}</p>
              <p class="text-[10px] text-muted-foreground uppercase tracking-widest">Xác thực khuôn mặt nhân viên</p>
            </div>

            <div class="relative w-full aspect-[4/3] rounded-2xl overflow-hidden bg-espresso border-4 border-cream shadow-inner flex items-center justify-center">
              <video ref="videoElement" class="w-full h-full object-cover" :class="cameraActive ? 'block' : 'hidden'" autoplay playsinline></video>

              <!-- Live Badge trạng thái AI nhận diện người -->
              <div v-if="cameraActive" class="absolute top-3 left-3 right-3 z-20 flex justify-center">
                <div 
                  class="px-3.5 py-1.5 rounded-full text-[11px] font-bold backdrop-blur-md border shadow-lg flex items-center gap-1.5 transition-all duration-300"
                  :class="isPersonDetected || !requireFaceDetection
                    ? 'bg-emerald-950/85 text-emerald-300 border-emerald-500/50 shadow-emerald-900/40'
                    : 'bg-red-950/90 text-red-200 border-red-500/50 shadow-red-900/40 animate-pulse'"
                >
                  <span class="w-2 h-2 rounded-full" :class="isPersonDetected || !requireFaceDetection ? 'bg-emerald-400 animate-ping' : 'bg-red-500 animate-ping'"></span>
                  <span>{{ requireFaceDetection ? faceDetectMessage : 'Chế độ chụp tự do' }}</span>
                </div>
              </div>

              <!-- Loading State -->
              <div v-if="cameraLoading" class="absolute inset-0 flex flex-col items-center justify-center bg-espresso text-cream space-y-3 p-4">
                <div class="w-10 h-10 border-4 border-caramel border-t-transparent rounded-full animate-spin"></div>
                <p class="text-xs font-bold">Đang kết nối Camera...</p>
              </div>

              <!-- Error / Fallback State -->
              <div v-else-if="!cameraActive" class="absolute inset-0 flex flex-col items-center justify-center bg-espresso/95 text-cream space-y-3 p-6 text-center z-10">
                <Camera class="w-10 h-10 text-amber-400 animate-pulse" />
                <p class="text-xs font-bold text-amber-200 px-2">{{ cameraError || 'Chờ mở Camera...' }}</p>
                <p class="text-[11px] text-cream/70 max-w-[300px]">💡 Mẹo: Truy cập bằng <strong class="text-white underline">http://localhost:5173</strong> trên máy tính này để Camera tự động bật ngay lập tức!</p>
                <div class="flex gap-2 pt-2">
                  <button @click="startCamera(checkInType)" class="px-4 py-2.5 rounded-xl bg-caramel hover:bg-brown text-white text-xs font-bold transition-all shadow-md active:scale-95 flex items-center gap-1.5">
                    📷 Bật lại Camera
                  </button>
                </div>
              </div>

              <!-- Viền Oval canh giữa khuôn mặt phát sáng theo trạng thái nhận diện -->
              <div 
                v-if="cameraActive" 
                class="absolute inset-5 border-3 rounded-[35%] transition-all duration-300 pointer-events-none flex flex-col items-center justify-between p-3"
                :class="isPersonDetected || !requireFaceDetection
                  ? 'border-emerald-400 shadow-[0_0_30px_rgba(52,211,153,0.6)]'
                  : 'border-red-400/80 border-dashed shadow-[0_0_20px_rgba(239,68,68,0.4)]'"
              >
                <div class="text-[9px] font-extrabold uppercase tracking-widest px-2.5 py-0.5 rounded-full backdrop-blur-sm shadow-sm"
                     :class="isPersonDetected || !requireFaceDetection ? 'bg-emerald-500 text-white' : 'bg-red-500 text-white animate-pulse'">
                  {{ isPersonDetected || !requireFaceDetection ? '✓ ĐÃ PHÁT HIỆN NGƯỜI' : 'CANH GIỮA KHUÔN MẶT' }}
                </div>
              </div>
            </div>

            <!-- Nút Chụp Ảnh & Nút Chuyển Chế Độ -->
            <div class="flex flex-col items-center gap-2">
              <button 
                @click="takePhoto" 
                :disabled="requireFaceDetection && !isPersonDetected"
                class="w-16 h-16 rounded-full flex items-center justify-center text-white shadow-lg transition-all active:scale-95 group border-4 border-white cursor-pointer disabled:opacity-40 disabled:cursor-not-allowed disabled:scale-95"
                :class="isPersonDetected || !requireFaceDetection 
                  ? 'bg-gradient-to-tr from-emerald-600 to-teal-500 hover:brightness-110 shadow-emerald-600/50 ring-4 ring-emerald-400/30' 
                  : 'bg-gray-500/60 shadow-gray-900/30'"
                :title="requireFaceDetection && !isPersonDetected ? 'Cần phát hiện người trước khi chụp' : 'Bấm để chụp ảnh chấm công'"
              >
                <Camera class="w-7 h-7 group-hover:scale-110 transition-transform" />
              </button>

              <p v-if="requireFaceDetection && !isPersonDetected" class="text-[11px] font-bold text-red-500 text-center animate-pulse">
                ⚠️ Chưa phát hiện người! Vui lòng nhìn thẳng vào camera
              </p>

              <!-- Toggle chuyển chế độ phòng trường hợp camera bị mờ -->
              <button 
                @click="requireFaceDetection = !requireFaceDetection" 
                class="mt-1 text-[10px] font-semibold text-muted-foreground hover:text-espresso underline cursor-pointer"
              >
                {{ requireFaceDetection ? 'Bỏ qua kiểm tra người (Chuyển chụp thủ công)' : 'Bật AI kiểm tra khuôn mặt người' }}
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

            <div class="space-y-1.5 text-left">
              <label class="text-[10px] uppercase tracking-widest font-bold flex items-center justify-between" :class="isCheckInLateOver5Minutes ? 'text-red-600' : 'text-[#8A8178]'">
                <span>{{ isCheckInLateOver5Minutes ? 'Lý do đi trễ (Bắt buộc)' : 'Ghi chú (Không bắt buộc)' }}</span>
                <span v-if="isCheckInLateOver5Minutes" class="text-[9px] bg-red-100 text-red-600 px-1.5 py-0.5 rounded font-extrabold">
                  Trễ {{ lateMinutesNumber }} phút
                </span>
              </label>
              <input 
                v-model="checkInNotes" 
                :placeholder="isCheckInLateOver5Minutes ? 'Vui lòng nhập lý do đi trễ (kẹt xe, hỏng xe...)' : 'Ví dụ: Đi trễ do kẹt xe...'" 
                class="w-full bg-cream border rounded-xl px-4 py-2.5 text-xs text-espresso focus:outline-none focus:ring-2"
                :class="isCheckInLateOver5Minutes ? 'border-red-400 focus:ring-red-400/30 bg-red-50/40 font-semibold' : 'border-cream-deep focus:ring-caramel/20'" 
              />
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
                  <option value="Cả ngày">Cả ngày (Tất cả các ca trong ngày)</option>
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

    <!-- Modal Từ Chối Đơn Yêu Cầu -->
    <div v-if="showRejectModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[420px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="showRejectModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6 space-y-4">
          <div class="text-center space-y-1">
            <h3 class="font-display text-xl font-bold text-destructive">Từ Chối Đơn Yêu Cầu</h3>
            <p class="text-xs text-muted-foreground">Vui lòng nhập lý do từ chối để gửi phản hồi cho nhân viên</p>
          </div>

          <div class="space-y-1.5">
            <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Lý do từ chối <span class="text-red-500">*</span></label>
            <textarea 
              v-model="rejectReasonInput" 
              rows="3" 
              placeholder="Nhập lý do từ chối (ví dụ: Trùng ca làm việc, chưa đủ số ngày phép...)" 
              class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-destructive/30 resize-none"
            ></textarea>
          </div>

          <div class="flex gap-3 pt-2">
            <button @click="showRejectModal = false" class="flex-1 py-2.5 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-xs transition-colors">
              Hủy bỏ
            </button>
            <button @click="confirmRejectRequest" :disabled="submittingReject" class="flex-1 py-2.5 rounded-xl bg-destructive hover:bg-destructive/90 text-white font-bold text-xs transition-colors shadow-warm uppercase tracking-wider">
              {{ submittingReject ? 'Đang gửi...' : 'Xác nhận Từ Chối' }}
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Xem Ảnh Chấm Công Toàn Màn Hình -->
    <div v-if="showImageModal" class="fixed inset-0 z-[100] flex items-center justify-center bg-black/80 backdrop-blur-md p-4 animate-in fade-in duration-200" @click="showImageModal = false">
      <div class="bg-card rounded-2xl shadow-2xl max-w-[550px] w-full overflow-hidden border border-cream-deep relative animate-in zoom-in-95 duration-300" @click.stop>
        <div class="p-4 border-b border-cream-deep flex items-center justify-between bg-cream/40">
          <span class="font-bold text-sm text-espresso flex items-center gap-2">
            <Camera class="w-4 h-4 text-caramel" />
            {{ previewImageTitle }}
          </span>
          <button @click="showImageModal = false" class="text-muted-foreground hover:text-espresso p-1 rounded-full hover:bg-cream transition-colors cursor-pointer">
            <X class="w-5 h-5" />
          </button>
        </div>
        <div class="p-3 bg-black flex items-center justify-center min-h-[300px] max-h-[70vh]">
          <img :src="previewImageUrl" class="max-w-full max-h-[65vh] object-contain rounded-xl shadow-lg border border-white/10" />
        </div>
        <div class="p-3.5 bg-cream/30 border-t border-cream-deep/60 flex justify-end">
          <button @click="showImageModal = false" class="px-5 py-2 bg-espresso text-cream font-bold text-xs rounded-xl hover:bg-brown transition-colors cursor-pointer">
            Đóng
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { Plus, X, Zap, Camera, LogIn, LogOut, Info, Users } from 'lucide-vue-next'
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

// --- Xem Ảnh Toàn Màn Hình ---
const showImageModal = ref(false)
const previewImageUrl = ref('')
const previewImageTitle = ref('')

const openImagePreview = (url?: string, title?: string) => {
  if (!url) return
  previewImageUrl.value = url
  previewImageTitle.value = title || 'Ảnh xác thực chấm công'
  showImageModal.value = true
}

// --- WiFi verification bypassed ---
const checkWiFiIP = () => true

const openCheckInModal = () => {
  if (!checkWiFiIP()) return
  if (authStore.user) {
    selectedEmployeeId.value = authStore.user.maNhanVien
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
const checkShiftTimeStatus = (shiftObj: { tenCa: string; gioBatDau: string; gioKetThuc: string }) => {
  if (!shiftObj) return { text: 'Thông tin ca làm.', type: 'info' }

  const [startH, startM] = shiftObj.gioBatDau.split(':').map(Number)
  const startTotalMin = (startH || 0) * 60 + (startM || 0)

  const now = new Date()
  const currentTotalMin = now.getHours() * 60 + now.getMinutes()

  const shiftLabel = `${shiftObj.tenCa} (${shiftObj.gioBatDau.slice(0, 5)} - ${shiftObj.gioKetThuc.slice(0, 5)})`

  if (currentTotalMin > startTotalMin + 15) {
    const lateMin = currentTotalMin - startTotalMin
    const lateH = Math.floor(lateMin / 60)
    const lateM = lateMin % 60
    const lateStr = lateH > 0 ? `${lateH}h ${lateM}m` : `${lateM} phút`
    return {
      text: `⚠️ TRỄ CA: Bạn được xếp [${shiftLabel}] nhưng đang đến TRỄ ${lateStr} so với giờ bắt đầu (${shiftObj.gioBatDau.slice(0, 5)}).`,
      type: 'late'
    }
  } else if (currentTotalMin < startTotalMin - 30) {
    const earlyMin = startTotalMin - currentTotalMin
    const earlyH = Math.floor(earlyMin / 60)
    const earlyM = earlyMin % 60
    const earlyStr = earlyH > 0 ? `${earlyH}h ${earlyM}m` : `${earlyM} phút`
    return {
      text: `⏰ ĐẾN SỚM: Bạn có lịch [${shiftLabel}], đến sớm ${earlyStr}.`,
      type: 'warning'
    }
  } else {
    return {
      text: `💡 ĐÚNG GIỜ: Bạn có lịch [${shiftLabel}] hôm nay. Trạng thái: Đúng giờ.`,
      type: 'success'
    }
  }
}

const updateAutoShiftSelection = async () => {
  if (!selectedEmployeeId.value) return

  try {
    const schedules = await hrApi.getSchedules()
    const todayIso = new Date().toISOString().split('T')[0]
    
    // Tìm phân ca HÔM NAY của nhân viên
    const todaySchedules = schedules.filter(s => 
      s.maNhanVien === selectedEmployeeId.value && 
      (s.ngayLamViec ? s.ngayLamViec.startsWith(todayIso) : false)
    )

    if (todaySchedules.length > 0) {
      // Lấy ca gần nhất theo giờ hiện tại
      const now = new Date()
      const currentTotalMin = now.getHours() * 60 + now.getMinutes()
      
      let bestSchedule = todaySchedules[0]
      let minDiff = 999999

      for (const sItem of todaySchedules) {
        const shiftObj = activeShifts.value.find(s => s.maCa === sItem.maCa)
        if (shiftObj) {
          const [h, m] = shiftObj.gioBatDau.split(':').map(Number)
          const startMin = (h || 0) * 60 + (m || 0)
          const diff = Math.abs(currentTotalMin - startMin)
          if (diff < minDiff) {
            minDiff = diff
            bestSchedule = sItem
          }
        }
      }

      selectedShiftId.value = bestSchedule.maCa
      const shiftObj = activeShifts.value.find(s => s.maCa === bestSchedule.maCa)
      if (shiftObj) {
        shiftInfoMessage.value = checkShiftTimeStatus(shiftObj)
      }
    } else {
      selectedShiftId.value = null
      shiftInfoMessage.value = { 
        text: '💡 Hôm nay bạn không có lịch xếp ca. Bạn sẽ chấm công vào Ca Tự Do.', 
        type: 'info' 
      }
    }
  } catch (e) {
    console.error('Lỗi khi tự động kiểm tra ca hôm nay:', e)
  }
}

const loadShifts = async () => {
  try {
    activeShifts.value = await hrApi.getShifts()
  } catch (err) {
    console.error("Failed to load shifts", err)
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
  
  const selectedShiftObj = activeShifts.value.find(s => s.maCa === newId)
  if (selectedShiftObj) {
    shiftInfoMessage.value = checkShiftTimeStatus(selectedShiftObj)
  }
})

// --- Check-in State ---
const showCheckInModal = ref(false)
const showCreateModal = ref(false)
const showForceCheckoutModal = ref(false)
const forceCheckoutReason = ref('')
const selectedActiveStaffLog = ref<ChamCongItem | null>(null)
const submittingForceCheckout = ref(false)

// Modal Từ chối đơn từ
const showRejectModal = ref(false)
const selectedRejectRequestId = ref<number | null>(null)
const rejectReasonInput = ref('')
const submittingReject = ref(false)

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

const handleReviewCheckIn = async (checkInId: number, status: 'DaDuyet' | 'TuChoi') => {
  try {
    const res = await hrApi.reviewCheckIn(checkInId, status)
    toast.success(res.message)
    if (selectedViewEmployeeId.value) {
      await onViewEmployeeChange()
    } else if (selectedEmployeeId.value) {
      await onCheckinEmployeeChange()
    } else {
      checkinLogs.value = await hrApi.getMyCheckins()
    }
  } catch (err: any) {
    toast.error(err?.message || 'Không thể cập nhật trạng thái chấm công.')
  }
}

const checkInStep = ref<1 | 2 | 3>(1)
const checkInType = ref<'in' | 'out'>('in')
const photoUrl = ref('')
const checkInNotes = ref('')
// Camera & AI Face Detection Logic
const videoElement = ref<HTMLVideoElement | null>(null)
const cameraActive = ref(false)
const cameraLoading = ref(false)
const cameraError = ref('')
let mediaStream: MediaStream | null = null

const requireFaceDetection = ref(true)
const isPersonDetected = ref(false)
const faceDetectMessage = ref('Đang quét tìm khuôn mặt...')
let faceDetectionTimer: ReturnType<typeof setInterval> | null = null

const startFaceDetectionLoop = () => {
  stopFaceDetectionLoop()
  isPersonDetected.value = false
  faceDetectMessage.value = 'Đang quét tìm khuôn mặt...'

  faceDetectionTimer = setInterval(async () => {
    if (!cameraActive.value || !videoElement.value) return
    await scanVideoFrameForFaceAndPerson()
  }, 350)
}

const stopFaceDetectionLoop = () => {
  if (faceDetectionTimer) {
    clearInterval(faceDetectionTimer)
    faceDetectionTimer = null
  }
}

const scanVideoFrameForFaceAndPerson = async () => {
  const video = videoElement.value
  if (!video || video.readyState < 2) return

  try {
    const canvas = document.createElement('canvas')
    const width = 160
    const height = 120
    canvas.width = width
    canvas.height = height
    const ctx = canvas.getContext('2d', { willReadFrequently: true })
    if (!ctx) return

    ctx.drawImage(video, 0, 0, width, height)
    const imgData = ctx.getImageData(0, 0, width, height)
    const data = imgData.data

    let totalLuminance = 0
    let centerSkinPixelCount = 0
    const centerLuminances: number[] = []

    // 3 Vùng giải phẫu khuôn mặt (Anatomical Zones)
    let foreheadLumSum = 0, foreheadCount = 0
    let eyeRowLumSum = 0, eyeRowCount = 0
    let cheekLumSum = 0, cheekCount = 0

    // Vùng trung tâm oval (x: 25%->75%, y: 15%->85%)
    const minX = Math.floor(width * 0.25)
    const maxX = Math.floor(width * 0.75)
    const minY = Math.floor(height * 0.15)
    const maxY = Math.floor(height * 0.85)
    const totalCenterPixels = (maxX - minX) * (maxY - minY)

    for (let y = 0; y < height; y++) {
      for (let x = 0; x < width; x++) {
        const i = (y * width + x) * 4
        const r = data[i]
        const g = data[i + 1]
        const b = data[i + 2]

        const lum = 0.299 * r + 0.587 * g + 0.114 * b
        totalLuminance += lum

        // Thuật toán YCbCr & RGB Skin-Tone range
        const cb = 128 - 0.168736 * r - 0.331264 * g + 0.5 * b
        const cr = 128 + 0.5 * r - 0.418688 * g - 0.081312 * b

        const isSkin = (r > 40 && g > 20 && b > 15 && Math.max(r, g, b) - Math.min(r, g, b) > 10 && Math.abs(r - g) > 10 && r > g && r > b) ||
                       (cr >= 132 && cr <= 175 && cb >= 75 && cb <= 128)

        if (x >= minX && x <= maxX && y >= minY && y <= maxY) {
          centerLuminances.push(lum)
          if (isSkin) {
            centerSkinPixelCount++
          }

          // Phân vùng quét cấu trúc khuôn mặt (Mới):
          const relY = y / height
          const relX = x / width
          if (relY >= 0.15 && relY < 0.32 && relX >= 0.35 && relX <= 0.65) {
            foreheadLumSum += lum
            foreheadCount++
          } else if (relY >= 0.32 && relY < 0.55 && relX >= 0.28 && relX <= 0.72) {
            eyeRowLumSum += lum
            eyeRowCount++
          } else if (relY >= 0.55 && relY < 0.78 && relX >= 0.30 && relX <= 0.70) {
            cheekLumSum += lum
            cheekCount++
          }
        }
      }
    }

    const avgLum = totalLuminance / (width * height)
    const centerSkinRatio = centerSkinPixelCount / totalCenterPixels

    // 1. Kiểm tra camera bị che tối hoặc ánh sáng quá chói
    if (avgLum < 18) {
      isPersonDetected.value = false
      faceDetectMessage.value = '🔴 Camera bị che / Khung hình quá tối'
      return
    } else if (avgLum > 242) {
      isPersonDetected.value = false
      faceDetectMessage.value = '🔴 Ánh sáng quá chói / Phản chiếu'
      return
    }

    // 2. Kiểm tra ngón tay/bàn tay áp sát che kín camera (Skin Ratio > 65%)
    if (centerSkinRatio > 0.65) {
      isPersonDetected.value = false
      faceDetectMessage.value = '🔴 Phát hiện che camera bằng tay / Ống kính bị áp sát'
      return
    }

    // 3. Phát hiện giơ bàn tay che trước mặt (Mid-Lower Face Hand Coverage)
    let midLowerSkinCount = 0
    let totalMidLowerPixels = 0
    const startY = Math.floor(height * 0.35)
    const endY = Math.floor(height * 0.75)
    const startX = Math.floor(width * 0.28)
    const endX = Math.floor(width * 0.72)

    for (let y = startY; y < endY; y++) {
      for (let x = startX; x < endX; x++) {
        totalMidLowerPixels++
        const i = (y * width + x) * 4
        const r = data[i]
        const g = data[i + 1]
        const b = data[i + 2]
        const cb = 128 - 0.168736 * r - 0.331264 * g + 0.5 * b
        const cr = 128 + 0.5 * r - 0.418688 * g - 0.081312 * b

        const isSkin = (r > 40 && g > 20 && b > 15 && Math.max(r, g, b) - Math.min(r, g, b) > 10 && Math.abs(r - g) > 10 && r > g && r > b) ||
                       (cr >= 132 && cr <= 175 && cb >= 75 && cb <= 128)
        if (isSkin) {
          midLowerSkinCount++
        }
      }
    }

    const midLowerSkinRatio = midLowerSkinCount / (totalMidLowerPixels || 1)
    if (midLowerSkinRatio > 0.50) {
      isPersonDetected.value = false
      faceDetectMessage.value = '🟡 VUI LÒNG BỎ TAY XUỐNG VÀ NHÌN THẲNG VÀO CAMERA'
      return
    }

    // 3. Phân tích Tương phản Vùng Mắt & Trán (Phân biệt Lòng bàn tay vs Khuôn mặt người)
    const avgForehead = foreheadLumSum / (foreheadCount || 1)
    const avgEyeRow = eyeRowLumSum / (eyeRowCount || 1)
    const avgCheek = cheekLumSum / (cheekCount || 1)

    // Độ lệch chuẩn sáng (Luminance Variance)
    const meanCenterLum = centerLuminances.reduce((a, b) => a + b, 0) / (centerLuminances.length || 1)
    const variance = Math.sqrt(centerLuminances.reduce((a, b) => a + Math.pow(b - meanCenterLum, 2), 0) / (centerLuminances.length || 1))

    // Độ chênh lệch vùng mắt so với trán/má (Hốc mắt/lông mày luôn tối hơn trán & má trên khuôn mặt thật)
    const eyeContrast = Math.max(avgForehead - avgEyeRow, avgCheek - avgEyeRow)

    // Lòng bàn tay có độ chênh lệch vùng mắt < 3.0 và độ biến thiên mờ phẳng
    if (eyeContrast < 3.0 && variance < 10) {
      isPersonDetected.value = false
      faceDetectMessage.value = '🔴 Phát hiện lòng bàn tay / Không thấy mắt & lông mày'
      return
    }

    // 4. Kiểm tra tổng hợp tỉ lệ màu da & cấu trúc khuôn mặt
    if (centerSkinRatio >= 0.10 && centerSkinRatio <= 0.65 && (eyeContrast >= 2.5 || variance >= 8.5)) {
      isPersonDetected.value = true
      faceDetectMessage.value = '🟢 ĐÃ NHẬN DIỆN KHUÔN MẶT HỢP LỆ'
    } else {
      isPersonDetected.value = false
      faceDetectMessage.value = '🟡 VUI LÒNG ĐƯA KHUÔN MẶT VÀO GIỮA KHUNG HÌNH'
    }
  } catch (err) {
    isPersonDetected.value = true
    faceDetectMessage.value = '🟢 ĐÃ BẬT CAMERA'
  }
}

const startCamera = async (type: 'in' | 'out') => {
  checkInType.value = type
  checkInStep.value = 2
  cameraActive.value = false
  cameraLoading.value = true
  cameraError.value = ''
  photoUrl.value = ''
  
  await nextTick()

  try {
    const constraintsList = [
      { video: { facingMode: 'user', width: { ideal: 1280 }, height: { ideal: 720 } } },
      { video: { facingMode: 'user' } },
      { video: true }
    ]

    let stream: MediaStream | null = null
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
      for (const constraints of constraintsList) {
        try {
          stream = await navigator.mediaDevices.getUserMedia(constraints)
          if (stream) break
        } catch (e) {}
      }
    } else {
      const getUserMediaLegacy = (navigator as any).getUserMedia ||
                                (navigator as any).webkitGetUserMedia ||
                                (navigator as any).mozGetUserMedia
      if (getUserMediaLegacy) {
        stream = await new Promise((resolve, reject) => {
          getUserMediaLegacy.call(navigator, { video: true }, resolve, reject)
        })
      }
    }

    if (stream) {
      mediaStream = stream
      cameraActive.value = true
      cameraError.value = ''

      // Chờ Vue render gán ref videoElement (thử 10 lần x 60ms)
      let retryCount = 0
      while (!videoElement.value && retryCount < 10) {
        await new Promise(r => setTimeout(r, 60))
        retryCount++
      }

      if (videoElement.value) {
        videoElement.value.srcObject = stream
        await videoElement.value.play().catch(() => {})
        startFaceDetectionLoop()
      }
    } else {
      cameraError.value = 'Trình duyệt chưa kết nối được Camera. Vui lòng bấm vào nút "Bật lại Camera" bên dưới.'
    }
  } catch (err: any) {
    console.error("Camera access failed:", err)
    if (err?.name === 'NotAllowedError' || err?.name === 'PermissionDeniedError') {
      cameraError.value = 'Bạn chưa bấm "Cho Phép" (Allow) quyền Camera trên trình duyệt. Vui lòng bấm vào biểu tượng 🔒 hoặc 📷 trên thanh địa chỉ!'
    } else {
      cameraError.value = err?.message || 'Không thể kết nối Camera thiết bị.'
    }
  } finally {
    cameraLoading.value = false
  }
}

const takePhoto = () => {
  if (requireFaceDetection.value && !isPersonDetected.value) {
    toast.warning('Chưa phát hiện khuôn mặt người trong ống kính. Vui lòng đưa mặt vào giữa camera!', 'Yêu cầu khuôn mặt')
    return
  }

  if (videoElement.value && cameraActive.value && mediaStream) {
    try {
      const canvas = document.createElement('canvas')
      canvas.width = videoElement.value.videoWidth || 640
      canvas.height = videoElement.value.videoHeight || 480
      const ctx = canvas.getContext('2d')
      if (ctx) {
        ctx.drawImage(videoElement.value, 0, 0, canvas.width, canvas.height)
        photoUrl.value = canvas.toDataURL('image/jpeg')
        stopCamera()
        checkInStep.value = 3
        return
      }
    } catch (e) {
      toast.error('Không thể chụp ảnh từ Camera')
      return
    }
  }
  toast.warning('Vui lòng bật Camera trực tiếp trên trình duyệt trước khi bấm Chụp.')
}

const stopCamera = () => {
  stopFaceDetectionLoop()
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

const lateMinutesNumber = computed(() => {
  if (checkInType.value !== 'in') return 0
  if (!selectedShiftId.value) return 0
  const shiftObj = activeShifts.value.find(s => s.maCa === selectedShiftId.value)
  if (!shiftObj) return 0

  const [startH, startM] = shiftObj.gioBatDau.split(':').map(Number)
  const startTotalMin = (startH || 0) * 60 + (startM || 0)

  const now = new Date()
  const currentTotalMin = now.getHours() * 60 + now.getMinutes()

  return currentTotalMin - startTotalMin
})

const isCheckInLateOver5Minutes = computed(() => {
  return lateMinutesNumber.value >= 5
})

const confirmCheckIn = async () => {
  if (!checkWiFiIP()) {
    closeCheckIn()
    return
  }
  if (isCheckInLateOver5Minutes.value && !checkInNotes.value.trim()) {
    toast.warning(`Bạn đang vào ca trễ ${lateMinutesNumber.value} phút. Vui lòng nhập lý do đi trễ trước khi xác nhận.`, 'Bắt buộc nhập lý do')
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
    notifyHrChange()
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
    notifyHrChange()
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

const currentViewEmployeeName = computed(() => {
  if (!isManager.value) return authStore.user?.hoTen || ''
  const emp = filteredEmployeeList.value.find(e => e.maNhanVien === selectedViewEmployeeId.value)
  return emp ? emp.hoTen : (authStore.user?.hoTen || '')
})

const selectStaffView = (empId: number) => {
  selectedViewEmployeeId.value = empId
  loadData()
}

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

const hrChannel = typeof BroadcastChannel !== 'undefined' ? new BroadcastChannel('quanlycf_hr_sync') : null

function notifyHrChange() {
  if (hrChannel) {
    try {
      hrChannel.postMessage({ type: 'HR_REQUEST_CHANGED', ts: Date.now() })
    } catch (e) {}
  }
}

const handleReviewRequest = async (id: number, status: 'DaDuyet' | 'TuChoi') => {
  if (status === 'TuChoi') {
    openRejectModal(id)
    return
  }

  try {
    const res = await hrApi.reviewRequest(id, status)
    toast.success(res.message, 'Duyệt đơn từ')
    notifyHrChange()
    await loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi duyệt đơn từ')
  }
}

const openRejectModal = (requestId: number) => {
  selectedRejectRequestId.value = requestId
  rejectReasonInput.value = ''
  showRejectModal.value = true
}

const confirmRejectRequest = async () => {
  if (!selectedRejectRequestId.value) return
  if (!rejectReasonInput.value.trim()) {
    toast.warning('Vui lòng nhập lý do từ chối đơn từ này!', 'Lý do từ chối bắt buộc')
    return
  }

  submittingReject.value = true
  try {
    const res = await hrApi.reviewRequest(selectedRejectRequestId.value, 'TuChoi', rejectReasonInput.value.trim())
    toast.success(res.message, 'Từ chối đơn từ')
    showRejectModal.value = false
    notifyHrChange()
    await loadData()
  } catch (err: any) {
    toast.error(err?.message || 'Không thể từ chối đơn từ.')
  } finally {
    submittingReject.value = false
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
    notifyHrChange()
    await loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi kết ca hộ')
  } finally {
    submittingForceCheckout.value = false
  }
}

let checkInAutoSyncTimer: any = null

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

  if (hrChannel) {
    hrChannel.onmessage = (e) => {
      if (e.data?.type === 'HR_REQUEST_CHANGED') {
        loadData()
      }
    }
  }

  checkInAutoSyncTimer = setInterval(async () => {
    if (!document.hidden) {
      await Promise.all([loadShifts(), loadData()])
    }
  }, 2500)
})

onUnmounted(() => {
  if (checkInAutoSyncTimer) clearInterval(checkInAutoSyncTimer)
  stopCamera()
})
</script>
