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
      
      <div v-if="isManager" class="bg-caramel p-5 rounded-xl shadow-warm flex flex-col justify-center relative overflow-hidden">
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
          v-if="isManager"
          @click="activeTab = 'payroll'" 
          class="pb-3 text-sm font-bold transition-colors relative whitespace-nowrap"
          :class="activeTab === 'payroll' ? 'text-caramel' : 'text-muted-foreground hover:text-espresso'"
        >
          Xét lương & Tính toán
          <div v-if="activeTab === 'payroll'" class="absolute bottom-[-2px] left-0 w-full h-0.5 bg-caramel rounded-t-full"></div>
        </button>
      </div>
      
      <!-- Actions -->
      <div v-if="activeTab === 'shifts'" class="pb-2 flex gap-2.5 items-center flex-wrap">
        <Button @click="openShiftModal()" size="sm" class="bg-espresso text-cream rounded-lg border border-espresso/30 shadow-card hover:bg-brown cursor-pointer">
          <Plus class="w-3.5 h-3.5 mr-1" /> {{ isManager ? 'Xếp ca' : 'Đăng ký ca làm' }}
        </Button>
        <button 
          v-if="isManager"
          @click="showShiftDefModal = true" 
          class="px-3.5 py-1.5 rounded-lg border border-cream-deep bg-card hover:bg-cream text-espresso text-xs font-bold transition-all shadow-sm flex items-center gap-1.5 cursor-pointer"
        >
          <Clock class="w-3.5 h-3.5 text-caramel" />
          <span>⏰ Quản lý Ca (Sáng, Trưa, Chiều, Tối)</span>
        </button>
        <button 
          v-if="isManager"
          @click="showLimitModal = true" 
          class="px-3.5 py-1.5 rounded-lg border border-cream-deep bg-card hover:bg-cream text-espresso text-xs font-bold transition-all shadow-sm flex items-center gap-1.5 cursor-pointer"
        >
          <Settings class="w-3.5 h-3.5 text-caramel" />
          <span>⚙️ Cấu hình số người / Ca</span>
        </button>
      </div>
    </div>

    <!-- Tab 1: Lịch ca -->
    <div v-if="activeTab === 'shifts'" class="animate-in fade-in duration-300">
      <div class="grid grid-cols-1 xl:grid-cols-[1fr_280px] gap-5">
        <div class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden">
          <div class="grid grid-cols-7 gap-px bg-cream-deep">
            <!-- Day headers -->
            <div v-for="w in weekDates" :key="w.isoDate" class="bg-cream/50 p-2.5 sm:p-3 text-center relative group/head">
              <div class="flex items-center justify-center gap-1">
                <span class="text-xs text-muted-foreground font-bold">{{ w.day }}</span>
                <button 
                  v-if="isManager"
                  @click="openDayLimitModal(w)" 
                  class="opacity-0 group-hover/head:opacity-100 transition-opacity text-caramel hover:text-brown p-0.5 cursor-pointer" 
                  title="⚙️ Chỉnh giới hạn ca cho ngày này"
                >
                  <Settings class="w-3 h-3" />
                </button>
              </div>
              <div class="font-display text-base text-espresso font-semibold">{{ w.dateStr }}</div>
            </div>

            <!-- Cells -->
            <div v-for="(wObj, dIdx) in weekDates" :key="`cell-${wObj.isoDate}`" class="bg-card min-h-[240px] p-2.5 flex flex-col gap-2 group relative">
              <!-- Sĩ số các ca làm trong ngày -->
              <div class="space-y-1 pb-1.5 border-b border-cream-deep/40">
                <div 
                  v-for="c in shiftsList" 
                  :key="`cap-status-${wObj.isoDate}-${c.maCa}`"
                  class="text-[10px] font-bold px-1.5 py-0.5 rounded flex items-center justify-between transition-colors"
                  :class="isShiftFull(c.maCa, wObj.isoDate) ? 'bg-amber-100 text-amber-800 border border-amber-300/60' : 'bg-cream/60 text-muted-foreground'"
                >
                  <span class="truncate">{{ c.tenCa }}</span>
                  <span :class="isShiftFull(c.maCa, wObj.isoDate) ? 'font-extrabold text-red-600' : 'text-espresso font-semibold'">
                    {{ getShiftCount(c.maCa, wObj.isoDate) }}/{{ getShiftLimit(c.maCa, wObj.isoDate) }}
                  </span>
                </div>
              </div>

              <!-- Danh sách Nhân viên đăng ký/được xếp -->
              <div v-for="(s, i) in schedule[wObj.day] || []" :key="i" class="p-2 sm:p-2.5 rounded-xl border text-xs shadow-card relative group/card transition-all hover:shadow-warm" :class="(shiftColors as any)[s.shift]">
                <!-- Nút thao tác Sửa/Hủy ca (Quản lý hoặc Chính chủ nhân viên đó) -->
                <div 
                  v-if="isManager || (authStore.user && s.maNhanVien === authStore.user.maNhanVien)" 
                  class="absolute top-1.5 right-1.5 z-20 opacity-0 group-hover/card:opacity-100 transition-opacity bg-white/95 backdrop-blur-sm border border-cream-deep/80 rounded-lg p-0.5 shadow-sm flex items-center gap-0.5"
                >
                  <button v-if="isManager" @click.stop="openShiftModal(wObj.dateStr)" class="text-muted-foreground hover:text-caramel p-1 rounded hover:bg-cream transition-colors cursor-pointer" title="Sửa ca làm">
                    <Edit3 class="w-3 h-3" />
                  </button>
                  <button @click.stop="deleteShiftById(s.id, s.staff)" class="text-muted-foreground hover:text-destructive p-1 rounded hover:bg-red-50 transition-colors cursor-pointer" title="Hủy / Xóa ca">
                    <Trash2 class="w-3 h-3" />
                  </button>
                </div>

                <div class="flex items-center gap-1.5 min-w-0 pr-1">
                  <div :class="['w-5 h-5 rounded-full text-cream text-[9px] font-extrabold flex items-center justify-center shrink-0 shadow-xs', s.color]">
                    {{ s.initials }}
                  </div>
                  <span class="font-bold text-espresso text-[11px] leading-tight truncate" :title="s.staff">{{ s.staff }}</span>
                </div>

                <div class="text-muted-foreground mt-1 text-[10px] font-medium flex items-center justify-between">
                  <span>{{ s.time }}</span>
                  <span v-if="s.note" class="px-1 py-0.2 rounded text-[8px] font-bold uppercase" :class="s.note === 'OT' ? 'bg-purple-200 text-purple-700' : 'bg-blue-200 text-blue-700'">
                    {{ s.note === 'OT' ? '+ OT' : 'Nghỉ' }}
                  </span>
                </div>
              </div>

              <!-- Vùng bấm đăng ký ca inline -->
              <button 
                @click="openShiftModal(wObj.dateStr)"
                class="mt-auto w-full py-1.5 rounded-lg border border-dashed border-cream-deep/80 hover:border-caramel hover:bg-cream/40 transition-all flex items-center justify-center gap-1 text-[10px] font-bold text-caramel opacity-80 group-hover:opacity-100 cursor-pointer"
              >
                <Plus class="w-3.5 h-3.5" />
                <span>{{ isManager ? 'Xếp ca' : 'Đăng ký ca' }}</span>
              </button>
            </div>
          </div>
        </div>

        <aside class="space-y-4">
          <!-- Tổng giờ tuần -->
          <div class="bg-card rounded-lg border border-cream-deep shadow-card p-5">
            <h4 class="font-display text-base text-espresso font-semibold mb-4">Phân bổ giờ làm</h4>
            <div class="space-y-3">
              <div v-for="emp in staffObjects" :key="emp.maNhanVien" class="group/rate flex justify-between items-center text-sm border-b border-cream/35 pb-2 last:border-0 last:pb-0">
                <div>
                  <span class="text-espresso font-medium block">{{ emp.hoTen }}</span>
                  <div class="flex items-center gap-1.5 text-[10px] text-muted-foreground">
                    <span>
                      Nhân viên <template v-if="isManager">· {{ (emp.luongCoBan || 25000).toLocaleString() }}đ/h</template>
                    </span>
                    <button v-if="isManager" @click="editRate(emp.hoTen)" class="opacity-0 group-hover/rate:opacity-100 transition-opacity text-caramel hover:text-brown p-0.5" title="Sửa mức lương">
                      <Edit3 class="w-3.5 h-3.5" />
                    </button>
                  </div>
                </div>
                <div class="text-right">
                  <span class="font-semibold text-caramel block">{{ totalHours.find(t => t[0] === emp.hoTen)?.[1] || '0h' }}</span>
                  <span v-if="isManager" class="text-[11px] font-bold text-espresso">
                    {{ ((parseInt(totalHours.find(t => t[0] === emp.hoTen)?.[1] || '0') || 0) * (emp.luongCoBan || 25000)).toLocaleString() }}đ
                  </span>
                </div>
              </div>
            </div>
          </div>
        </aside>
      </div>
    </div>
    <!-- Tab 2: Bảng lương -->
    <div v-if="activeTab === 'payroll'" class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden min-h-[300px] animate-in fade-in duration-300">
      <!-- Header điều khiển chọn Kỳ Lương (Tháng/Năm) -->
      <div class="p-4 border-b border-cream-deep bg-cream/30 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div class="flex items-center gap-2">
          <span class="text-xs font-bold text-espresso uppercase tracking-wider">📅 Chọn kỳ tính lương:</span>
          <select 
            v-model="selectedPayrollKy" 
            @change="onPayrollKyChange"
            class="bg-cream border border-cream-deep rounded-xl px-4 py-2 text-xs font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20 cursor-pointer shadow-sm"
          >
            <option v-for="opt in payrollKyOptions" :key="opt.value" :value="opt.value">
              {{ opt.label }}
            </option>
          </select>
        </div>

        <div class="flex items-center gap-4 text-xs font-bold text-muted-foreground bg-background px-4 py-2 rounded-xl border border-cream-deep">
          <div>Tổng quỹ lương {{ selectedPayrollKy }}: <span class="text-caramel text-sm font-extrabold">{{ totalPayrollAmount.toLocaleString() }}đ</span></div>
        </div>
      </div>

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
              <th class="px-5 py-4 font-medium">Trạng thái thanh toán</th>
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
              <td class="px-5 py-4">
                <div v-if="row.trangThaiThanhToan === 'DaThanhToan'">
                  <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-emerald-500/10 text-emerald-600 text-[11px] font-bold border border-emerald-500/20 whitespace-nowrap shadow-xs">
                    <div class="w-1.5 h-1.5 rounded-full bg-emerald-500"></div> Đã thanh toán
                  </span>
                  <span v-if="row.thoiGianThanhToan" class="block text-[10px] text-muted-foreground mt-0.5 font-medium">Lúc {{ row.thoiGianThanhToan }}</span>
                </div>
                <div v-else>
                  <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-amber-500/10 text-amber-600 text-[11px] font-bold border border-amber-500/20 whitespace-nowrap shadow-xs">
                    <div class="w-1.5 h-1.5 rounded-full bg-amber-500 animate-ping"></div> Chưa thanh toán
                  </span>
                </div>
              </td>
              <td class="px-5 py-4 text-right">
                <div class="flex justify-end items-center gap-2">
                  <button 
                    v-if="isManager && row.trangThaiThanhToan !== 'DaThanhToan'" 
                    @click="openPaySalaryModal(row)" 
                    class="px-3 py-1.5 bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-bold rounded-lg shadow-sm transition-all flex items-center gap-1 uppercase cursor-pointer whitespace-nowrap"
                  >
                    💵 Chi lương
                  </button>
                  <span v-else-if="row.trangThaiThanhToan === 'DaThanhToan'" class="text-xs text-emerald-600 font-bold flex items-center gap-1 bg-emerald-50 px-2 py-1 rounded border border-emerald-200">
                    ✓ Đã trả lương
                  </span>
                </div>
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
              <select 
                v-if="isManager"
                v-model="shiftForm.maNhanVien" 
                class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20"
              >
                <option v-for="emp in staffObjects" :key="emp.maNhanVien" :value="emp.maNhanVien">
                  {{ emp.hoTen }}
                </option>
              </select>
              <div v-else class="w-full bg-cream/70 border border-cream-deep rounded-xl px-4 py-3 text-sm font-bold text-espresso">
                👤 {{ authStore.user?.hoTen || 'Tài khoản của bạn' }}
              </div>
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ngày làm việc</label>
              <select v-model="shiftForm.isoDate" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option v-for="w in weekDates" :key="w.isoDate" :value="w.isoDate">{{ w.day }} - {{ w.dateStr }}</option>
              </select>
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ca làm</label>
              <select v-model="shiftForm.maCa" class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20">
                <option 
                  v-for="c in shiftsList" 
                  :key="c.maCa" 
                  :value="c.maCa"
                  :disabled="isShiftFull(c.maCa, shiftForm.isoDate)"
                >
                  {{ c.tenCa }} ({{ c.gioBatDau }} - {{ c.gioKetThuc }})
                  {{ isShiftFull(c.maCa, shiftForm.isoDate) ? ` - ❌ Đã đầy (${getShiftCount(c.maCa, shiftForm.isoDate)}/${getShiftLimit(c.maCa, shiftForm.isoDate)} người)` : ` - (${getShiftCount(c.maCa, shiftForm.isoDate)}/${getShiftLimit(c.maCa, shiftForm.isoDate)} người)` }}
                </option>
              </select>
            </div>

            <div class="space-y-1.5">
              <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ghi chú (Tùy chọn)</label>
              <input v-model="shiftForm.ghiChu" type="text" placeholder="Nhập ghi chú..." class="w-full bg-cream border border-cream-deep rounded-xl px-4 py-3 text-sm text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-caramel/20" />
            </div>
          </div>

          <div class="flex gap-3 mt-8">
            <button @click="showShiftModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy
            </button>
            <button @click="saveShift" class="flex-1 py-3 rounded-xl bg-caramel hover:bg-brown text-white font-bold text-sm transition-colors uppercase tracking-wider">
              Lưu Ca
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Cấu Hình Giới Hạn Ca -->
    <div v-if="showLimitModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[440px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="showLimitModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6">
          <div class="flex items-center gap-2 mb-1 justify-center">
            <Settings class="w-5 h-5 text-caramel" />
            <h2 class="font-display text-xl font-bold text-espresso">Giới hạn Số người / Ca</h2>
          </div>
          <p class="text-xs text-muted-foreground text-center mb-5">
            Thiết lập số nhân sự tối đa cho từng ca làm việc theo ngày.
          </p>

          <!-- Day Selector -->
          <div class="mb-4 text-left">
            <label class="block text-[11px] font-bold text-muted-foreground uppercase tracking-widest mb-1.5">Chọn ngày cài đặt</label>
            <select v-model="selectedLimitTarget" class="w-full bg-cream border border-cream-deep rounded-xl px-3.5 py-2.5 text-xs font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20">
              <option value="all">📅 Mặc định (Áp dụng cho tất cả các ngày)</option>
              <option v-for="w in weekDates" :key="w.isoDate" :value="w.isoDate">
                🗓️ {{ w.day }} - Ngày {{ w.dateStr }}
              </option>
            </select>
          </div>

          <div class="space-y-3">
            <div v-for="c in shiftsList" :key="c.maCa" class="flex items-center justify-between p-3.5 rounded-xl border border-cream-deep bg-cream/30">
              <div class="text-left">
                <span class="font-bold text-xs text-espresso block">{{ c.tenCa }}</span>
                <span class="text-[10px] text-muted-foreground">{{ c.gioBatDau }} - {{ c.gioKetThuc }}</span>
              </div>
              <div class="flex items-center gap-2">
                <input 
                  type="number" 
                  min="1" 
                  max="20" 
                  :value="getLimitFormValue(c.maCa)"
                  @input="e => setLimitFormValue(c.maCa, parseInt((e.target as HTMLInputElement).value) || 1)"
                  class="w-16 bg-white border border-cream-deep rounded-lg px-2.5 py-1.5 text-xs text-center font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20"
                />
                <span class="text-xs font-semibold text-muted-foreground">người</span>
              </div>
            </div>
          </div>

          <div class="flex gap-3 mt-8">
            <button @click="showLimitModal = false" class="flex-1 py-3 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-sm transition-colors">
              Hủy
            </button>
            <button @click="saveShiftLimits" class="flex-1 py-3 rounded-xl bg-caramel hover:bg-brown text-white font-bold text-sm transition-colors shadow-warm uppercase tracking-wider">
              Lưu Cấu Hình
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

    <!-- Modal Quản Lý Các Ca Làm Việc (Thêm/Sửa Ca Sáng, Trưa, Chiều, Tối) -->
    <div v-if="showShiftDefModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[480px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep">
        <button @click="showShiftDefModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors">
          <X class="w-5 h-5" />
        </button>

        <div class="p-6">
          <div class="flex items-center gap-2 mb-1 justify-center">
            <Clock class="w-5 h-5 text-caramel" />
            <h2 class="font-display text-xl font-bold text-espresso">Quản lý Ca làm việc (Sáng, Trưa, Chiều, Tối...)</h2>
          </div>
          <p class="text-xs text-muted-foreground text-center mb-5">
            Cấu hình danh sách và khung giờ cho các ca làm việc của quán.
          </p>

          <!-- Danh sách Ca hiện có -->
          <div class="space-y-2 mb-6 max-h-[180px] overflow-y-auto pr-1">
            <div 
              v-for="c in shiftsList" 
              :key="c.maCa" 
              class="flex items-center justify-between p-3 rounded-xl border border-cream-deep bg-cream/30 hover:border-caramel/40 transition-colors"
            >
              <div class="text-left">
                <span class="font-bold text-xs text-espresso block">{{ c.tenCa }}</span>
                <span class="text-[11px] text-muted-foreground font-medium">Khung giờ: {{ c.gioBatDau }} - {{ c.gioKetThuc }}</span>
              </div>
              <div class="flex items-center gap-1.5">
                <button @click="openEditShiftDef(c)" class="p-1.5 rounded-lg bg-cream hover:bg-cream-deep text-caramel transition-colors cursor-pointer" title="Sửa ca">
                  <Edit3 class="w-3.5 h-3.5" />
                </button>
                <button @click="deleteShiftDef(c.maCa, c.tenCa)" class="p-1.5 rounded-lg bg-cream hover:bg-red-50 text-red-500 transition-colors cursor-pointer" title="Xóa ca">
                  <Trash2 class="w-3.5 h-3.5" />
                </button>
              </div>
            </div>
          </div>

          <!-- Form Thêm/Sửa Ca -->
          <div class="p-4 rounded-xl border border-caramel/30 bg-cream/40 space-y-3 text-left">
            <div class="flex items-center justify-between">
              <span class="text-xs font-bold text-espresso">
                {{ isEditingShiftDef ? '✏️ Chỉnh sửa ca làm' : '➕ Thêm ca làm mới' }}
              </span>
              <button v-if="isEditingShiftDef" @click="openCreateShiftDef" class="text-[11px] text-caramel font-semibold hover:underline">
                Hủy sửa
              </button>
            </div>

            <div>
              <label class="block text-[11px] font-bold text-muted-foreground uppercase tracking-widest mb-1">Tên ca làm</label>
              <input 
                type="text" 
                v-model="shiftDefForm.tenCa" 
                placeholder="Ví dụ: Ca Sáng, Ca Trưa, Ca Chiều, Ca Tối" 
                class="w-full bg-white border border-cream-deep rounded-xl px-3.5 py-2 text-xs font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20"
              />
            </div>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="block text-[11px] font-bold text-muted-foreground uppercase tracking-widest mb-1">Giờ bắt đầu</label>
                <input 
                  type="time" 
                  v-model="shiftDefForm.gioBatDau" 
                  class="w-full bg-white border border-cream-deep rounded-xl px-3 py-2 text-xs font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20"
                />
              </div>
              <div>
                <label class="block text-[11px] font-bold text-muted-foreground uppercase tracking-widest mb-1">Giờ kết thúc</label>
                <input 
                  type="time" 
                  v-model="shiftDefForm.gioKetThuc" 
                  class="w-full bg-white border border-cream-deep rounded-xl px-3 py-2 text-xs font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20"
                />
              </div>
            </div>

            <button 
              @click="saveShiftDef" 
              class="w-full py-2.5 rounded-xl bg-caramel hover:bg-brown text-white font-bold text-xs uppercase tracking-wider transition-colors shadow-sm cursor-pointer mt-1"
            >
              {{ isEditingShiftDef ? 'Lưu thay đổi ca' : 'Lưu ca làm mới' }}
            </button>
          </div>

          <div class="mt-4 text-center">
            <button @click="showShiftDefModal = false" class="px-6 py-2 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-xs transition-colors cursor-pointer">
              Đóng
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Confirm Tùy Chỉnh Đẹp Theo Theme Quán -->
    <div v-if="showConfirmModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[400px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep p-6 text-center">
        <button @click="showConfirmModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors cursor-pointer">
          <X class="w-5 h-5" />
        </button>

        <div class="w-12 h-12 rounded-full bg-amber-500/10 border border-amber-500/20 text-amber-600 flex items-center justify-center mx-auto mb-4">
          <Trash2 class="w-6 h-6" />
        </div>

        <h3 class="font-display text-xl font-bold text-espresso mb-2">
          {{ confirmData.title }}
        </h3>
        
        <p class="text-xs text-muted-foreground leading-relaxed mb-6 px-2">
          {{ confirmData.message }}
        </p>

        <div class="flex gap-3">
          <button 
            @click="showConfirmModal = false" 
            class="flex-1 py-2.5 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-xs font-semibold transition-colors cursor-pointer"
          >
            {{ confirmData.cancelText }}
          </button>
          <button 
            @click="handleConfirmAction" 
            class="flex-1 py-2.5 rounded-xl bg-red-600 hover:bg-red-700 text-white font-bold text-xs shadow-sm transition-colors cursor-pointer uppercase tracking-wider"
          >
            {{ confirmData.confirmText }}
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Xác Nhận Thanh Toán Lương -->
    <div v-if="showPayModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/60 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card rounded-2xl shadow-warm w-[420px] relative animate-in zoom-in-95 duration-300 overflow-hidden border border-cream-deep p-6">
        <button @click="showPayModal = false" class="absolute top-4 right-4 text-muted-foreground hover:text-espresso z-10 bg-cream/50 rounded-full p-1 transition-colors cursor-pointer">
          <X class="w-5 h-5" />
        </button>

        <div class="text-center mb-5 space-y-1">
          <div class="w-12 h-12 rounded-full bg-emerald-500/10 border border-emerald-500/20 text-emerald-600 flex items-center justify-center mx-auto mb-2">
            <DollarSign class="w-6 h-6" />
          </div>
          <h3 class="font-display text-xl font-bold text-espresso">Thanh Toán Lương</h3>
          <p class="text-xs text-muted-foreground">Xác nhận chi trả lương cho nhân viên</p>
        </div>

        <div class="space-y-4 text-left bg-cream/30 p-4 rounded-xl border border-cream-deep mb-5">
          <div class="flex justify-between items-center text-xs">
            <span class="text-muted-foreground font-medium">Nhân viên:</span>
            <span class="font-bold text-espresso text-sm">{{ payForm.name }}</span>
          </div>
          <div class="flex justify-between items-center text-xs">
            <span class="text-muted-foreground font-medium">Tổng tiền lương:</span>
            <span class="font-extrabold text-caramel text-base">{{ payForm.amount.toLocaleString() }}đ</span>
          </div>
          <div class="space-y-1.5 pt-2 border-t border-cream-deep/60">
            <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Hình thức thanh toán</label>
            <select v-model="payForm.phuongThuc" class="w-full bg-cream border border-cream-deep rounded-xl px-3.5 py-2.5 text-xs font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-emerald-500/20">
              <option value="Chuyển khoản">🏦 Chuyển khoản ngân hàng (VNPAY/STK)</option>
              <option value="Tiền mặt">💵 Tiền mặt trực tiếp tại quán</option>
            </select>
          </div>
          <div class="space-y-1.5">
            <label class="text-[11px] uppercase tracking-widest text-muted-foreground font-bold">Ghi chú chi lương (Tùy chọn)</label>
            <input v-model="payForm.ghiChu" type="text" placeholder="Nhập mã GD ngân hàng hoặc ghi chú..." class="w-full bg-cream border border-cream-deep rounded-xl px-3.5 py-2 text-xs text-espresso font-medium focus:outline-none focus:ring-2 focus:ring-emerald-500/20" />
          </div>
        </div>

        <div class="flex gap-3">
          <button @click="showPayModal = false" class="flex-1 py-2.5 rounded-xl border border-cream-deep bg-background hover:bg-cream text-espresso font-bold text-xs transition-colors cursor-pointer">
            Hủy bỏ
          </button>
          <button @click="confirmPaySalary" :disabled="submittingPay" class="flex-1 py-2.5 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white font-bold text-xs shadow-sm transition-colors cursor-pointer uppercase tracking-wider">
            {{ submittingPay ? 'Đang lưu...' : 'Xác Nhận Đã Trả' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { Plus, Edit3, Eye, X, Trash2, DollarSign, Settings, Clock } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import { hrApi, type DonTuItem, type PhanCaItem, type EmployeePayrollItem } from '@/services/hr'
import { useToast } from '@/stores/toast'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
const activeTab = ref<'shifts' | 'payroll'>('shifts')
const toast = useToast()
const isEditing = ref(false)

// Kiểm tra quyền Quản lý / Admin
const isManager = computed(() => {
  const u = authStore.user
  if (!u) return false
  const role = (u.vaiTro || '').toLowerCase()
  const quyens = u.quyens || []
  return role.includes('quản lý') || role.includes('admin') || quyens.includes('NhanSu.QuanLy') || quyens.includes('NhanSu.Xoa') || quyens.includes('System.Admin')
})

// --- Shift Definition State (Quản lý các ca sáng, trưa, chiều, tối...) ---
const showShiftDefModal = ref(false)
const isEditingShiftDef = ref(false)
const editingShiftDefId = ref<number | null>(null)
const shiftDefForm = ref({ tenCa: '', gioBatDau: '07:00', gioKetThuc: '14:00' })

const openCreateShiftDef = () => {
  isEditingShiftDef.value = false
  editingShiftDefId.value = null
  shiftDefForm.value = { tenCa: 'Ca Trưa', gioBatDau: '11:00', gioKetThuc: '15:00' }
}

const openEditShiftDef = (c: { maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }) => {
  isEditingShiftDef.value = true
  editingShiftDefId.value = c.maCa
  shiftDefForm.value = { tenCa: c.tenCa, gioBatDau: c.gioBatDau, gioKetThuc: c.gioKetThuc }
}

const saveShiftDef = async () => {
  const { tenCa, gioBatDau, gioKetThuc } = shiftDefForm.value
  if (!tenCa.trim()) {
    toast.warning('Vui lòng nhập tên ca làm việc.', 'Thiếu thông tin')
    return
  }
  if (!gioBatDau || !gioKetThuc) {
    toast.warning('Vui lòng chọn giờ bắt đầu và kết thúc.', 'Thiếu thông tin')
    return
  }

  try {
    if (isEditingShiftDef.value && editingShiftDefId.value) {
      await hrApi.updateShiftDefinition(editingShiftDefId.value, { tenCa: tenCa.trim(), gioBatDau, gioKetThuc })
      toast.success(`Đã cập nhật ${tenCa}`, 'Thành công')
    } else {
      await hrApi.createShiftDefinition({ tenCa: tenCa.trim(), gioBatDau, gioKetThuc })
      toast.success(`Đã thêm ${tenCa} mới thành công!`, 'Thành công')
    }
    openCreateShiftDef()
    loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi lưu ca làm việc.')
  }
}

const deleteShiftDef = (id: number, name: string) => {
  openConfirmModal({
    title: 'Xóa Ca Làm Việc',
    message: `Bạn có chắc chắn muốn xóa ca làm "${name}"? Các phân ca đã đăng ký trước đó sẽ không bị ảnh hưởng.`,
    confirmText: 'Xóa ca',
    cancelText: 'Hủy',
    onConfirm: async () => {
      try {
        await hrApi.deleteShiftDefinition(id)
        toast.success(`Đã xóa ca "${name}"`, 'Thành công')
        loadData()
      } catch (err: any) {
        toast.error(err.message || 'Lỗi khi xóa ca làm.')
      }
    }
  })
}

// --- Real State from DB ---
const staffObjects = ref<{ maNhanVien: number; hoTen: string; luongCoBan?: number }[]>([])
const shiftsList = ref<{ maCa: number; tenCa: string; gioBatDau: string; gioKetThuc: string }[]>([])
const rawSchedules = ref<PhanCaItem[]>([])
const payrollList = ref<EmployeePayrollItem[]>([])
const loading = ref(false)

// Dynamic Week Dates (Current Week Monday -> Sunday)
const weekDates = computed(() => {
  const now = new Date()
  const currentDay = now.getDay() // 0 = Sun, 1 = Mon
  const distanceToMon = currentDay === 0 ? -6 : 1 - currentDay
  const monday = new Date(now)
  monday.setDate(now.getDate() + distanceToMon)

  const dayNames = ["T2", "T3", "T4", "T5", "T6", "T7", "CN"]
  return dayNames.map((d, i) => {
    const dt = new Date(monday)
    dt.setDate(monday.getDate() + i)
    const day = dt.getDate().toString().padStart(2, '0')
    const month = (dt.getMonth() + 1).toString().padStart(2, '0')
    const year = dt.getFullYear()
    return {
      day: d,
      dateStr: `${day}/${month}`,
      isoDate: `${year}-${month}-${day}`
    }
  })
})

const days = computed(() => weekDates.value.map(w => w.day))
const dates = computed(() => weekDates.value.map(w => w.dateStr))

const shiftColors = {
  morning: "bg-warning/15 border-warning/30",
  afternoon: "bg-caramel-light border-caramel/30",
  evening: "bg-espresso/10 border-espresso/30",
}

// Group rawSchedules by Thu (T2, T3, T4, T5, T6, T7, CN) & sort by start time
const schedule = computed(() => {
  const map: Record<string, any[]> = { T2: [], T3: [], T4: [], T5: [], T6: [], T7: [], CN: [] }

  for (const item of rawSchedules.value) {
    const thu = item.thuTrongTuan || 'T2'
    if (!map[thu]) map[thu] = []

    let shiftType: 'morning' | 'afternoon' | 'evening' = 'morning'
    const caInfo = shiftsList.value.find(c => c.maCa === item.maCa)
    const startTimeStr = caInfo ? caInfo.gioBatDau : (item.gio || '00:00')

    const nameLower = (item.tenCa || '').toLowerCase()
    if (nameLower.includes('chiều') || nameLower.includes('trưa')) shiftType = 'afternoon'
    else if (nameLower.includes('tối') || nameLower.includes('đêm')) shiftType = 'evening'

    const nameParts = (item.tenNhanVien || '').split(' ')
    const initials = nameParts.length >= 2
      ? (nameParts[0].charAt(0) + nameParts[nameParts.length - 1].charAt(0)).toUpperCase()
      : (item.tenNhanVien || 'NV').slice(0, 2).toUpperCase()

    map[thu].push({
      id: item.maPhanCa,
      maNhanVien: item.maNhanVien,
      maCa: item.maCa,
      staff: item.tenNhanVien,
      initials,
      color: "bg-brown",
      shift: shiftType,
      time: item.gio,
      startTimeStr,
      ngayLamViec: item.ngayLamViec,
      note: item.ghiChu
    })
  }

  // Tự động sắp xếp các ca trong ngày theo thứ tự giờ bắt đầu (Sáng ➔ Trưa ➔ Chiều ➔ Tối)
  for (const thu in map) {
    map[thu].sort((a, b) => a.startTimeStr.localeCompare(b.startTimeStr))
  }

  return map
})

const getShiftDurationHours = (maCa?: number) => {
  if (!maCa) return 6
  const ca = shiftsList.value.find(c => c.maCa === maCa)
  if (!ca || !ca.gioBatDau || !ca.gioKetThuc) return 6
  const [h1, m1] = ca.gioBatDau.split(':').map(Number)
  const [h2, m2] = ca.gioKetThuc.split(':').map(Number)
  let diffMin = (h2 * 60 + m2) - (h1 * 60 + m1)
  if (diffMin <= 0) diffMin += 24 * 60
  return Math.round((diffMin / 60) * 10) / 10
}

// Calculate total hours per staff for left side summary
const totalHours = computed(() => {
  const res: [string, string][] = []
  const hoursMap: Record<string, number> = {}

  for (const item of rawSchedules.value) {
    const name = item.tenNhanVien
    if (!name || name === 'Quản trị viên') continue
    if (!hoursMap[name]) hoursMap[name] = 0
    hoursMap[name] += getShiftDurationHours(item.maCa)
  }

  for (const emp of staffObjects.value) {
    if (emp.hoTen === 'Quản trị viên') continue
    const h = hoursMap[emp.hoTen] || 0
    res.push([emp.hoTen, `${Math.round(h * 10) / 10}h`])
  }
  return res
})

const totalWeeklyHours = computed(() => {
  let total = 0
  for (const item of rawSchedules.value) {
    if (item.tenNhanVien === 'Quản trị viên') continue
    total += getShiftDurationHours(item.maCa)
  }
  return Math.round(total * 10) / 10
})

const totalWeeklyWages = computed(() => {
  let total = 0
  for (const item of rawSchedules.value) {
    if (item.tenNhanVien === 'Quản trị viên') continue
    const emp = staffObjects.value.find(e => e.maNhanVien === item.maNhanVien)
    const rate = emp?.luongCoBan || 25000
    total += getShiftDurationHours(item.maCa) * rate
  }
  return total
})

// --- Shift Modal State ---
const showShiftModal = ref(false)
const shiftForm = ref<{ maNhanVien: number; maCa: number; isoDate: string; ghiChu: string }>({
  maNhanVien: 0,
  maCa: 0,
  isoDate: '',
  ghiChu: ''
})

const openShiftModal = (prefillDateStr = '') => {
  let targetIso = weekDates.value[0]?.isoDate || ''
  if (prefillDateStr) {
    const found = weekDates.value.find(w => w.dateStr === prefillDateStr)
    if (found) targetIso = found.isoDate
  }

  const loggedId = authStore.user?.maNhanVien || staffObjects.value[0]?.maNhanVien || 0

  shiftForm.value = {
    maNhanVien: isManager.value ? (staffObjects.value[0]?.maNhanVien || 0) : loggedId,
    maCa: shiftsList.value[0]?.maCa || 0,
    isoDate: targetIso,
    ghiChu: ''
  }
  showShiftModal.value = true
}

// Limits state per shift ID & daily limits
const showLimitModal = ref(false)
const selectedLimitTarget = ref<string>('all') // 'all' or isoDate (e.g. '2026-07-27')
const shiftLimits = ref<Record<number, number>>({ 1: 2, 2: 2, 3: 2 })
const dailyShiftLimits = ref<Record<string, number>>({})

const loadShiftLimits = async () => {
  try {
    const res = await hrApi.getShiftLimits()
    if (res.generalLimitsJson && res.generalLimitsJson !== '{}') {
      shiftLimits.value = JSON.parse(res.generalLimitsJson)
    } else {
      const savedGeneral = localStorage.getItem('quanlycf_shift_limits')
      if (savedGeneral) shiftLimits.value = JSON.parse(savedGeneral)
    }

    if (res.dailyLimitsJson && res.dailyLimitsJson !== '{}') {
      dailyShiftLimits.value = JSON.parse(res.dailyLimitsJson)
    } else {
      const savedDaily = localStorage.getItem('quanlycf_daily_shift_limits')
      if (savedDaily) dailyShiftLimits.value = JSON.parse(savedDaily)
    }
  } catch (e) {
    console.error('Failed to load shift limits:', e)
  }
}

const openDayLimitModal = (wObj: { day: string; dateStr: string; isoDate: string }) => {
  selectedLimitTarget.value = wObj.isoDate
  showLimitModal.value = true
}

const matchDate = (d1?: string, d2?: string) => {
  if (!d1 || !d2) return false
  const str1 = d1.includes('T') ? d1.split('T')[0] : d1
  const str2 = d2.includes('T') ? d2.split('T')[0] : d2
  return str1 === str2
}

const getShiftLimit = (maCa: number, isoDate: string) => {
  if (isoDate) {
    const cleanDate = isoDate.includes('T') ? isoDate.split('T')[0] : isoDate
    const key = `${cleanDate}_${maCa}`
    if (dailyShiftLimits.value[key] !== undefined) {
      return dailyShiftLimits.value[key]
    }
  }
  return shiftLimits.value[maCa] || 2
}

const getLimitFormValue = (maCa: number) => {
  if (selectedLimitTarget.value === 'all') {
    return shiftLimits.value[maCa] || 2
  }
  const cleanDate = selectedLimitTarget.value.includes('T') ? selectedLimitTarget.value.split('T')[0] : selectedLimitTarget.value
  const key = `${cleanDate}_${maCa}`
  return dailyShiftLimits.value[key] !== undefined ? dailyShiftLimits.value[key] : (shiftLimits.value[maCa] || 2)
}

const setLimitFormValue = (maCa: number, val: number) => {
  if (selectedLimitTarget.value === 'all') {
    shiftLimits.value[maCa] = val
  } else {
    const cleanDate = selectedLimitTarget.value.includes('T') ? selectedLimitTarget.value.split('T')[0] : selectedLimitTarget.value
    const key = `${cleanDate}_${maCa}`
    dailyShiftLimits.value[key] = val
  }
}

const saveShiftLimits = async () => {
  try {
    const genJson = JSON.stringify(shiftLimits.value)
    const dailyJson = JSON.stringify(dailyShiftLimits.value)

    localStorage.setItem('quanlycf_shift_limits', genJson)
    localStorage.setItem('quanlycf_daily_shift_limits', dailyJson)

    await hrApi.saveShiftLimits({
      generalLimitsJson: genJson,
      dailyLimitsJson: dailyJson
    })

    // Ép Vue 3 cập nhật giao diện lập tức (Cập nhật tham chiếu object)
    shiftLimits.value = JSON.parse(genJson)
    dailyShiftLimits.value = JSON.parse(dailyJson)

    toast.success('Đã lưu cấu hình giới hạn số người!', 'Thành công')
    showLimitModal.value = false
    await loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi lưu cấu hình giới hạn ca.')
  }
}

const getShiftCount = (maCa: number, isoDate: string) => {
  if (!maCa || !isoDate) return 0
  return rawSchedules.value.filter(s => s.maCa === maCa && matchDate(s.ngayLamViec, isoDate)).length
}

const isShiftFull = (maCa: number, isoDate: string) => {
  const maxCap = getShiftLimit(maCa, isoDate)
  return getShiftCount(maCa, isoDate) >= maxCap
}

const saveShift = async () => {
  if (!shiftForm.value.maNhanVien) {
    toast.warning('Vui lòng chọn nhân viên.', 'Thiếu thông tin')
    return
  }
  if (!shiftForm.value.maCa) {
    toast.warning('Vui lòng chọn ca làm.', 'Thiếu thông tin')
    return
  }

  const maxCap = getShiftLimit(shiftForm.value.maCa, shiftForm.value.isoDate)
  if (isShiftFull(shiftForm.value.maCa, shiftForm.value.isoDate)) {
    const caInfo = shiftsList.value.find(c => c.maCa === shiftForm.value.maCa)
    toast.warning(`Ca ${caInfo?.tenCa || ''} vào ngày này đã đủ giới hạn tối đa (${maxCap} người).`, 'Ca đã đầy')
    return
  }

  try {
    const created = await hrApi.createSchedule({
      maNhanVien: shiftForm.value.maNhanVien,
      maCa: shiftForm.value.maCa,
      ngayLamViec: shiftForm.value.isoDate,
      ghiChu: shiftForm.value.ghiChu
    })
    rawSchedules.value.push(created)
    toast.success('Xếp ca làm mới thành công!', 'Thành công')
    showShiftModal.value = false
    loadData()
  } catch (err: any) {
    toast.error(err.message || 'Không thể lưu ca làm mới.', 'Lỗi hệ thống')
  }
}

// --- Custom Confirm Modal State ---
const showConfirmModal = ref(false)
const confirmData = ref<{
  title: string
  message: string
  confirmText: string
  cancelText: string
  onConfirm: () => void | Promise<void>
}>({
  title: 'Xác nhận xóa',
  message: '',
  confirmText: 'Xóa',
  cancelText: 'Hủy',
  onConfirm: () => {}
})

const openConfirmModal = (options: {
  title?: string
  message: string
  confirmText?: string
  cancelText?: string
  onConfirm: () => void | Promise<void>
}) => {
  confirmData.value = {
    title: options.title || 'Xác nhận thao tác',
    message: options.message,
    confirmText: options.confirmText || 'Đồng ý',
    cancelText: options.cancelText || 'Hủy',
    onConfirm: options.onConfirm
  }
  showConfirmModal.value = true
}

const handleConfirmAction = async () => {
  showConfirmModal.value = false
  if (confirmData.value.onConfirm) {
    await confirmData.value.onConfirm()
  }
}

const deleteShiftById = (id: number, staffName: string) => {
  if (!id) return

  const isSelf = authStore.user && staffObjects.value.find(e => e.maNhanVien === authStore.user?.maNhanVien)?.hoTen === staffName

  openConfirmModal({
    title: isSelf ? 'Hủy Đăng Ký Ca Làm' : 'Xác Nhận Xóa Ca Làm',
    message: isSelf 
      ? `Bạn có chắc chắn muốn hủy đăng ký ca làm ngày này không?` 
      : `Bạn có chắc chắn muốn xoá ca làm của nhân viên ${staffName}? Ca làm sẽ bị gỡ khỏi lịch phân ca.`,
    confirmText: isSelf ? 'Hủy đăng ký' : 'Xóa ca',
    cancelText: 'Quay lại',
    onConfirm: async () => {
      try {
        await hrApi.deleteSchedule(id)
        rawSchedules.value = rawSchedules.value.filter(s => s.maPhanCa !== id)
        toast.success(isSelf ? 'Đã hủy đăng ký ca làm thành công!' : 'Đã xoá ca làm thành công!', 'Thành công')
        loadData()
      } catch (err: any) {
        toast.error(err.message || 'Lỗi khi hủy ca làm.')
      }
    }
  })
}

const deleteShift = async (day: string, index: number) => {
  const item = schedule.value[day]?.[index]
  if (!item || !item.id) return
  await deleteShiftById(item.id, item.staff)
}

// --- Payroll Rate Edit Modal ---
const showRateModal = ref(false)
const rateForm = ref<{ employeeId: number; name: string; rate: number }>({
  employeeId: 0,
  name: '',
  rate: 25000
})

const editRate = (name: string) => {
  const emp = staffObjects.value.find(e => e.hoTen === name)
  if (!emp) return
  rateForm.value = {
    employeeId: emp.maNhanVien,
    name: emp.hoTen,
    rate: emp.luongCoBan || 25000
  }
  showRateModal.value = true
}

const saveRate = async () => {
  const { employeeId, name, rate } = rateForm.value
  const num = parseInt(rate as any)
  if (isNaN(num) || num <= 0) {
    toast.warning('Mức lương không hợp lệ.', 'Lỗi nhập liệu')
    return
  }

  try {
    await hrApi.updateEmployeeRate(employeeId, num)
    const emp = staffObjects.value.find(e => e.maNhanVien === employeeId)
    if (emp) emp.luongCoBan = num
    toast.success(`Đã cập nhật mức lương của ${name} thành ${num.toLocaleString()}đ/h`, 'Thành công')
    showRateModal.value = false
    loadData()
  } catch (err: any) {
    toast.error(err.message || 'Lỗi cập nhật mức lương.')
  }
}

// --- Period (Kỳ lương Tháng/Năm) State ---
const selectedPayrollKy = ref(new Date().toISOString().slice(0, 7)) // e.g. "2026-09"

const payrollKyOptions = computed(() => {
  const options = []
  const now = new Date()
  for (let i = -3; i <= 12; i++) {
    const d = new Date(now.getFullYear(), now.getMonth() - i, 1)
    const yStr = d.getFullYear()
    const mStr = String(d.getMonth() + 1).padStart(2, '0')
    const val = `${yStr}-${mStr}`
    const isCurrent = val === now.toISOString().slice(0, 7)
    options.push({
      value: val,
      label: `Tháng ${mStr} / ${yStr}${isCurrent ? ' (Hiện tại)' : ''}`
    })
  }
  return options
})

const totalPayrollAmount = computed(() => {
  return payroll.value.reduce((sum, item) => sum + (item.total || 0), 0)
})

const onPayrollKyChange = async () => {
  loading.value = true
  try {
    payrollList.value = await hrApi.getPayrollSummary(selectedPayrollKy.value)
  } catch (err) {
    console.error("Failed to load payroll for ky", err)
  } finally {
    loading.value = false
  }
}

// --- Pay Salary Modal ---
const showPayModal = ref(false)
const payForm = ref<{ employeeId: number; name: string; amount: number; phuongThuc: string; ghiChu: string }>({
  employeeId: 0,
  name: '',
  amount: 0,
  phuongThuc: 'Chuyển khoản',
  ghiChu: ''
})
const submittingPay = ref(false)

const openPaySalaryModal = (row: any) => {
  payForm.value = {
    employeeId: row.id,
    name: row.name,
    amount: row.total,
    phuongThuc: 'Chuyển khoản',
    ghiChu: ''
  }
  showPayModal.value = true
}

const confirmPaySalary = async () => {
  if (!payForm.value.employeeId) return
  submittingPay.value = true
  try {
    const res = await hrApi.paySalary(payForm.value.employeeId, {
      ky: selectedPayrollKy.value,
      phuongThuc: payForm.value.phuongThuc,
      ghiChu: payForm.value.ghiChu
    })
    toast.success(res.message, 'Xác nhận thanh toán lương')
    showPayModal.value = false
    await onPayrollKyChange()
  } catch (err: any) {
    toast.error(err?.message || 'Không thể xác nhận thanh toán lương.')
  } finally {
    submittingPay.value = false
  }
}

const payroll = computed(() => {
  return payrollList.value.map(p => ({
    id: p.maNhanVien,
    name: p.hoTen,
    role: p.chucVu,
    avatar: `https://api.dicebear.com/7.x/adventurer/svg?seed=${encodeURIComponent(p.hoTen)}`,
    rate: p.luongCoBan,
    normalHours: p.tongGioLam,
    otHours: 0,
    leaveDays: 0,
    total: p.tongLuong,
    trangThaiThanhToan: p.trangThaiThanhToan || 'ChuaThanhToan',
    thoiGianThanhToan: p.thoiGianThanhToan || '',
    ghiChuThanhToan: p.ghiChuThanhToan || ''
  }))
})

const loadData = async () => {
  loading.value = true
  try {
    await loadShiftLimits()
    const promises: Promise<any>[] = [
      hrApi.getEmployees(),
      hrApi.getShifts(),
      hrApi.getSchedules()
    ]
    if (isManager.value) {
      promises.push(hrApi.getPayrollSummary(selectedPayrollKy.value).catch(() => []))
    } else {
      promises.push(Promise.resolve([]))
    }

    const [employees, shifts, schedules, payrolls] = await Promise.all(promises)
    staffObjects.value = (employees || []).filter(e => e.hoTen !== 'Quản trị viên')
    shiftsList.value = shifts || []
    rawSchedules.value = schedules || []
    payrollList.value = payrolls || []
  } catch (err: any) {
    console.error("Failed to load HR data:", err)
  } finally {
    loading.value = false
  }
}

const loadDataSilently = async () => {
  try {
    await loadShiftLimits()
    const promises: Promise<any>[] = [
      hrApi.getEmployees(),
      hrApi.getShifts(),
      hrApi.getSchedules()
    ]
    if (isManager.value) {
      promises.push(hrApi.getPayrollSummary(selectedPayrollKy.value).catch(() => []))
    } else {
      promises.push(Promise.resolve([]))
    }

    const [employees, shifts, schedules, payrolls] = await Promise.all(promises)
    staffObjects.value = (employees || []).filter(e => e.hoTen !== 'Quản trị viên')
    shiftsList.value = shifts || []
    rawSchedules.value = schedules || []
    payrollList.value = payrolls || []
  } catch (e) {
    // Tự động đồng bộ ngầm không hiện báo lỗi
  }
}

let autoSyncTimer: any = null

onMounted(() => {
  loadData()
  autoSyncTimer = setInterval(() => {
    if (!document.hidden) {
      loadDataSilently()
    }
  }, 3000)
  window.addEventListener('focus', loadDataSilently)
})

onUnmounted(() => {
  if (autoSyncTimer) clearInterval(autoSyncTimer)
  window.removeEventListener('focus', loadDataSilently)
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
