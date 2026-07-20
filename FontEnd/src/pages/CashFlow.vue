<template>
  <div class="space-y-6 font-premium-sans text-[#2A231E] pb-10">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
      <div>
        <h2 class="text-3xl font-premium-serif font-bold text-[#2A231E]">Dòng tiền & Chi phí</h2>
        <p class="text-[11px] uppercase tracking-[0.15em] text-[#8A8178] font-bold mt-2">Tổng hợp và theo dõi chi phí phát sinh trong hệ thống.</p>
      </div>
      
      <div class="flex items-center gap-3">
        <!-- Month Selection -->
        <select 
          v-model="selectedOptionIdx"
          @change="onPeriodChange"
          class="border border-[#EAE3D9] rounded-lg px-4 py-2.5 text-sm bg-[#FDFDFD] text-[#2A231E] font-bold focus:outline-none focus:border-[#CC8033] shadow-sm cursor-pointer"
        >
          <option v-for="(opt, idx) in monthOptions" :key="idx" :value="idx">
            {{ opt.label }}
          </option>
        </select>
        
        <!-- Refresh Button -->
        <button 
          @click="loadData" 
          :disabled="loading"
          class="p-2.5 bg-[#F5F2ED] hover:bg-[#EAE3D9] border border-[#EAE3D9] text-[#5C544E] rounded-lg text-sm font-bold transition-all flex items-center gap-1.5 shadow-sm disabled:opacity-50"
        >
          <span v-if="loading" class="w-4 h-4 border-2 border-[#8A8178] border-t-transparent rounded-full animate-spin"></span>
          <span v-else>Tải lại</span>
        </button>

        <button 
          @click="isCreateModalOpen = true" 
          class="px-5 py-2.5 bg-[#CC8033] hover:bg-[#B3702C] text-white rounded-lg text-sm font-bold transition-colors shadow-md flex items-center gap-2"
        >
          <Plus class="w-4 h-4" stroke-width="2.5" /> Tạo phiếu chi
        </button>
      </div>
    </div>

    <!-- Loading overlay or state -->
    <div v-if="loading && !journalList.length" class="flex flex-col items-center justify-center py-20 bg-[#FDFDFD] rounded-2xl border border-[#EAE3D9]">
      <div class="w-10 h-10 border-4 border-[#CC8033] border-t-transparent rounded-full animate-spin"></div>
      <p class="text-sm font-bold text-[#8A8178] mt-4">Đang đồng bộ dữ liệu từ sổ cái...</p>
    </div>

    <template v-else>
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- Inflow -->
        <div class="bg-[#FDFDFD] p-6 rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-md transition-shadow">
          <div class="flex justify-between items-start">
            <h3 class="text-sm font-bold text-[#8A8178] uppercase tracking-wider">Tổng Thu Dòng Tiền</h3>
            <span class="text-[10px] font-bold px-2 py-1 bg-emerald-50 text-emerald-600 rounded flex items-center gap-1 border border-emerald-100">
              <TrendingUp class="w-3 h-3" stroke-width="2.5" /> Ghi nhận
            </span>
          </div>
          <p class="text-[28px] font-premium-serif font-bold text-emerald-600 mt-4">
            + {{ summaryData.tongThu.toLocaleString('vi-VN') }}đ
          </p>
          <p class="text-[11px] text-[#8A8178] font-bold mt-2">Doanh thu POS và các khoản thu nhập khác</p>
        </div>
        
        <!-- Outflow -->
        <div class="bg-[#FDFDFD] p-6 rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-md transition-shadow">
          <div class="flex justify-between items-start">
            <h3 class="text-sm font-bold text-[#8A8178] uppercase tracking-wider">Tổng Chi Vận Hành</h3>
            <span class="text-[10px] font-bold px-2 py-1 bg-red-50 text-red-600 rounded flex items-center gap-1 border border-red-100">
              <TrendingDown class="w-3 h-3" stroke-width="2.5" /> Ghi nhận
            </span>
          </div>
          <p class="text-[28px] font-premium-serif font-bold text-red-600 mt-4">
            - {{ summaryData.tongChi.toLocaleString('vi-VN') }}đ
          </p>
          <div class="text-[11px] text-[#8A8178] font-bold mt-2 flex flex-wrap gap-x-2">
            <span>Lương: {{ (summaryData.chiLuong / 1000000).toFixed(1) }}tr</span>
            <span>•</span>
            <span>Kho: {{ (summaryData.chiKho / 1000000).toFixed(1) }}tr</span>
            <span>•</span>
            <span>Khác: {{ (summaryData.chiKhac / 1000000).toFixed(1) }}tr</span>
          </div>
        </div>

        <!-- Net Cashflow -->
        <div class="bg-[#FDFDFD] p-6 rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-md transition-shadow">
          <div class="flex justify-between items-start">
            <h3 class="text-sm font-bold text-[#8A8178] uppercase tracking-wider">Dòng Tiền Thuần</h3>
            <span :class="summaryData.dongTienThuan >= 0 ? 'bg-emerald-50 text-emerald-600 border-emerald-100' : 'bg-red-50 text-red-600 border-red-100'" class="text-[10px] font-bold px-2 py-1 rounded flex items-center gap-1 border">
              <TrendingUp class="w-3 h-3" stroke-width="2.5" /> Chênh lệch
            </span>
          </div>
          <p :class="summaryData.dongTienThuan >= 0 ? 'text-emerald-600' : 'text-red-600'" class="text-[28px] font-premium-serif font-bold mt-4">
            {{ summaryData.dongTienThuan >= 0 ? '+' : '' }} {{ summaryData.dongTienThuan.toLocaleString('vi-VN') }}đ
          </p>
          <p class="text-[11px] text-[#8A8178] font-bold mt-2">Dòng tiền thuần tích lũy trong tháng này</p>
        </div>
      </div>

      <!-- Charts Section -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Bar Chart (Revenue vs Expenses day-by-day) -->
        <div class="lg:col-span-2 bg-[#FDFDFD] rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-6 md:p-8">
          <div class="flex items-center justify-between mb-6">
            <h3 class="font-premium-serif text-xl font-bold text-[#2A231E]">Biểu đồ dòng tiền hằng ngày</h3>
            <div class="flex items-center gap-4 text-xs font-bold">
              <div class="flex items-center gap-1.5"><span class="w-3 h-3 rounded bg-[#5C4533]"></span> Thu nhập</div>
              <div class="flex items-center gap-1.5"><span class="w-3 h-3 rounded bg-[#C1A081]"></span> Chi phí</div>
            </div>
          </div>
          <div class="w-full h-[300px]">
            <!-- @ts-ignore -->
            <Bar :data="barChartData" :options="barChartOptions" />
          </div>
        </div>

        <!-- Doughnut Chart (Expenses breakdown) -->
        <div class="bg-[#FDFDFD] rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-6 md:p-8 flex flex-col justify-between">
          <h3 class="font-premium-serif text-xl font-bold text-[#2A231E] mb-6">Cơ cấu Chi phí vận hành</h3>
          <div class="flex-1 min-h-[200px] max-h-[220px] flex items-center justify-center">
            <!-- @ts-ignore -->
            <Doughnut :data="doughnutChartData" :options="doughnutOptions" />
          </div>
          <div class="grid grid-cols-3 gap-2 mt-6 text-[10px] font-bold text-[#5C544E] uppercase tracking-wider">
            <div class="flex flex-col items-center p-2 bg-[#F9F8F6] border border-[#EAE3D9] rounded-lg">
              <span class="w-2 h-2 rounded-full bg-[#5C4533] mb-1"></span>
              <span>Lương</span>
              <span class="text-[#2A231E] mt-1">{{ formatPercent(summaryData.chiLuong, summaryData.tongChi) }}%</span>
            </div>
            <div class="flex flex-col items-center p-2 bg-[#F9F8F6] border border-[#EAE3D9] rounded-lg">
              <span class="w-2 h-2 rounded-full bg-[#8A6D53] mb-1"></span>
              <span>Nguyên liệu</span>
              <span class="text-[#2A231E] mt-1">{{ formatPercent(summaryData.chiKho, summaryData.tongChi) }}%</span>
            </div>
            <div class="flex flex-col items-center p-2 bg-[#F9F8F6] border border-[#EAE3D9] rounded-lg">
              <span class="w-2 h-2 rounded-full bg-[#C1A081] mb-1"></span>
              <span>Mặt bằng & Khác</span>
              <span class="text-[#2A231E] mt-1">{{ formatPercent(summaryData.chiKhac, summaryData.tongChi) }}%</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Tab View (Transaction Journal vs Staff Payroll) -->
      <div class="bg-[#FDFDFD] rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] overflow-hidden">
        <!-- Tabs Header -->
        <div class="flex border-b border-[#EAE3D9] bg-[#F9F8F6] px-6">
          <button 
            @click="activeTab = 'journal'"
            :class="activeTab === 'journal' ? 'border-[#CC8033] text-[#CC8033] font-bold' : 'border-transparent text-[#8A8178] hover:text-[#2A231E]'"
            class="px-5 py-4 border-b-2 text-sm font-semibold transition-all"
          >
            Sổ Nhật Ký Giao Dịch
          </button>
          <button 
            @click="activeTab = 'salaries'"
            :class="activeTab === 'salaries' ? 'border-[#CC8033] text-[#CC8033] font-bold' : 'border-transparent text-[#8A8178] hover:text-[#2A231E]'"
            class="px-5 py-4 border-b-2 text-sm font-semibold transition-all"
          >
            Bảng Lương Nhân Sự
          </button>
        </div>

        <!-- Tab Content 1: Transaction Journal -->
        <div v-show="activeTab === 'journal'">
          <!-- Filters -->
          <div class="p-6 border-b border-[#EAE3D9] bg-white flex flex-col md:flex-row md:items-center justify-between gap-4">
            <div class="flex items-center gap-3 flex-1">
              <div class="relative flex-1 max-w-md">
                <Search class="absolute left-3 top-2.5 w-4 h-4 text-[#8A8178]" />
                <input 
                  v-model="filterSearch"
                  type="text" 
                  placeholder="Tìm theo ghi chú, người nhận..." 
                  class="pl-9 pr-4 py-2 w-full border border-[#EAE3D9] rounded-lg text-sm bg-[#FDFDFD] text-[#2A231E] focus:outline-none focus:border-[#CC8033]"
                />
              </div>

              <!-- Type Select -->
              <select 
                v-model="filterType"
                class="border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold focus:outline-none focus:border-[#CC8033]"
              >
                <option value="TatCa">Tất cả loại giao dịch</option>
                <option value="Thu">Chỉ dòng Thu (Inflow)</option>
                <option value="Chi">Chỉ dòng Chi (Outflow)</option>
              </select>

              <!-- Category Select -->
              <select 
                v-model="filterCategory"
                class="border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold focus:outline-none focus:border-[#CC8033]"
              >
                <option value="TatCa">Tất cả phân loại</option>
                <option value="DoanhThuPOS">Doanh thu POS</option>
                <option value="NhapHang">Nhập hàng / Nguyên liệu</option>
                <option value="TraLuong">Thanh toán lương</option>
                <option value="DienNuoc">Điện nước & Tiện ích</option>
                <option value="MatBang">Tiền thuê mặt bằng</option>
                <option value="Khac">Chi phí khác</option>
              </select>

              <!-- Page Limit Select -->
              <select 
                v-model="itemsPerPage"
                class="border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold focus:outline-none focus:border-[#CC8033]"
              >
                <option :value="10">Hiển thị 10 dòng</option>
                <option :value="20">Hiển thị 20 dòng</option>
                <option :value="50">Hiển thị 50 dòng</option>
                <option :value="100">Hiển thị 100 dòng</option>
              </select>
            </div>
            
            <div class="text-xs font-bold text-[#8A8178]">
              Tìm thấy {{ filteredJournal.length }} giao dịch
            </div>
          </div>

          <!-- Table -->
          <div class="overflow-x-auto">
            <table class="w-full text-sm text-left">
              <thead>
                <tr class="bg-[#F9F8F6] text-[#8A8178] text-[10px] uppercase tracking-[0.15em] border-b border-[#EAE3D9]">
                  <th class="px-8 py-4 font-bold">Mã số</th>
                  <th class="px-6 py-4 font-bold">Thời gian</th>
                  <th class="px-6 py-4 font-bold">Phân loại</th>
                  <th class="px-6 py-4 font-bold">Phương thức</th>
                  <th class="px-6 py-4 font-bold">Người thực hiện/nhận</th>
                  <th class="px-6 py-4 font-bold">Ghi chú</th>
                  <th class="px-8 py-4 font-bold text-right">Số tiền</th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="!filteredJournal.length">
                  <td colspan="7" class="px-8 py-10 text-center text-sm font-semibold text-[#8A8178]">
                    Không tìm thấy giao dịch nào khớp với bộ lọc.
                  </td>
                </tr>
                <tr 
                  v-for="item in paginatedJournal" 
                  :key="item.maDongTien" 
                  class="border-b border-[#EAE3D9]/60 hover:bg-[#F5F2ED] transition-colors group"
                >
                  <td class="px-8 py-4 font-bold text-[#8A6D53]">#{{ item.maDongTien }}</td>
                  <td class="px-6 py-4 text-[#8A8178] font-medium text-xs">
                    {{ new Date(item.thoiGianTao).toLocaleString('vi-VN') }}
                  </td>
                  <td class="px-6 py-4">
                    <span 
                      :class="item.loaiGiaoDich === 'Thu' ? 'bg-emerald-50 text-emerald-600 border-emerald-100' : 'bg-amber-50 text-amber-700 border-amber-200'"
                      class="px-2.5 py-1 border rounded-md text-[10px] font-bold uppercase tracking-wider"
                    >
                      {{ getNhomLabel(item.nhomGiaoDich) }}
                    </span>
                  </td>
                  <td class="px-6 py-4 text-xs font-semibold text-[#5C544E]">
                    {{ getPhuongThucLabel(item.phuongThucThanhToan) }}
                  </td>
                  <td class="px-6 py-4 font-bold text-[#2A231E]">
                    {{ item.nguoiNopNhan || 'Hệ thống' }}
                  </td>
                  <td class="px-6 py-4 text-xs text-[#5C544E] max-w-xs truncate" :title="item.ghiChu || ''">
                    {{ item.ghiChu || '-' }}
                  </td>
                  <td :class="item.loaiGiaoDich === 'Thu' ? 'text-emerald-600' : 'text-red-600'" class="px-8 py-4 text-right font-bold">
                    {{ item.loaiGiaoDich === 'Thu' ? '+' : '-' }}{{ item.soTien.toLocaleString('vi-VN') }}đ
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Pagination Footer -->
          <div class="p-4 border-t border-[#EAE3D9] bg-[#F9F8F6] flex flex-col sm:flex-row items-center justify-between gap-4">
            <span class="text-xs font-semibold text-[#8A8178]">
              Hiển thị {{ filteredJournal.length ? Math.min((currentPage - 1) * itemsPerPage + 1, filteredJournal.length) : 0 }}-{{ Math.min(currentPage * itemsPerPage, filteredJournal.length) }} trên tổng số {{ filteredJournal.length }} giao dịch
            </span>
            <div class="flex items-center gap-2">
              <button 
                @click="currentPage--" 
                :disabled="currentPage === 1"
                class="px-3 py-1.5 border border-[#EAE3D9] rounded-md text-xs font-bold bg-white text-[#5C544E] hover:bg-[#F5F2ED] disabled:opacity-50 transition-colors"
              >
                Trước
              </button>
              <span class="text-xs font-bold text-[#2A231E]">
                Trang {{ currentPage }} / {{ totalPages }}
              </span>
              <button 
                @click="currentPage++" 
                :disabled="currentPage === totalPages"
                class="px-3 py-1.5 border border-[#EAE3D9] rounded-md text-xs font-bold bg-white text-[#5C544E] hover:bg-[#F5F2ED] disabled:opacity-50 transition-colors"
              >
                Sau
              </button>
            </div>
          </div>
        </div>

        <!-- Tab Content 2: Staff Payroll -->
        <div v-show="activeTab === 'salaries'">
          <!-- Payroll Summary and Filter -->
          <div class="p-6 border-b border-[#EAE3D9] bg-white flex flex-col md:flex-row md:items-center justify-between gap-4">
            <div class="relative flex-1 max-w-md">
              <Search class="absolute left-3 top-2.5 w-4 h-4 text-[#8A8178]" />
              <input 
                v-model="salarySearch"
                type="text" 
                placeholder="Tìm nhân viên theo tên..." 
                class="pl-9 pr-4 py-2 w-full border border-[#EAE3D9] rounded-lg text-sm bg-[#FDFDFD] text-[#2A231E] focus:outline-none focus:border-[#CC8033]"
              />
            </div>
            
            <div class="flex items-center gap-4 text-xs font-bold text-[#8A8178] bg-[#F9F8F6] p-3 rounded-lg border border-[#EAE3D9]">
              <div>Tổng thực lĩnh: <span class="text-[#CC8033] text-sm">{{ totalSalarySum.toLocaleString('vi-VN') }}đ</span></div>
              <span>|</span>
              <div>Số nhân sự: <span class="text-[#2A231E] text-sm">{{ filteredSalaries.length }}</span></div>
            </div>
          </div>

          <!-- Payroll Table -->
          <div class="overflow-x-auto">
            <table class="w-full text-sm text-left">
              <thead>
                <tr class="bg-[#F9F8F6] text-[#8A8178] text-[10px] uppercase tracking-[0.15em] border-b border-[#EAE3D9]">
                  <th class="px-8 py-4 font-bold">Nhân viên</th>
                  <th class="px-6 py-4 font-bold">Vai trò</th>
                  <th class="px-6 py-4 font-bold text-right">Lương/Giờ</th>
                  <th class="px-6 py-4 font-bold text-center">Giờ thường</th>
                  <th class="px-6 py-4 font-bold text-center">Giờ OT</th>
                  <th class="px-6 py-4 font-bold text-right">Phụ cấp</th>
                  <th class="px-6 py-4 font-bold text-right">Thưởng</th>
                  <th class="px-6 py-4 font-bold text-right">Khấu trừ</th>
                  <th class="px-6 py-4 font-bold text-right">Thực lãnh</th>
                  <th class="px-8 py-4 font-bold text-center">Trạng thái</th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="!filteredSalaries.length">
                  <td colspan="10" class="px-8 py-10 text-center text-sm font-semibold text-[#8A8178]">
                    Không tìm thấy nhân sự nào.
                  </td>
                </tr>
                <tr 
                  v-for="salary in filteredSalaries" 
                  :key="salary.maBangLuong" 
                  class="border-b border-[#EAE3D9]/60 hover:bg-[#F5F2ED] transition-colors group"
                >
                  <td class="px-8 py-4 font-bold text-[#2A231E]">{{ salary.hoTen }}</td>
                  <td class="px-6 py-4 text-xs font-semibold text-[#8A8178]">{{ salary.tenVaiTro }}</td>
                  <td class="px-6 py-4 text-right font-medium text-xs text-[#5C544E]">
                    {{ salary.luongTheoGio.toLocaleString('vi-VN') }}đ
                  </td>
                  <td class="px-6 py-4 text-center font-bold text-xs text-[#2A231E]">{{ salary.soGioThuong }}h</td>
                  <td class="px-6 py-4 text-center font-bold text-xs text-amber-600 bg-amber-50/50">
                    {{ salary.soGioOT > 0 ? `+${salary.soGioOT}h` : '0h' }}
                  </td>
                  <td class="px-6 py-4 text-right text-xs text-[#5C544E] font-medium">
                    {{ salary.phuCap > 0 ? `+${salary.phuCap.toLocaleString('vi-VN')}đ` : '-' }}
                  </td>
                  <td class="px-6 py-4 text-right text-xs text-emerald-600 font-medium">
                    {{ salary.thuong > 0 ? `+${salary.thuong.toLocaleString('vi-VN')}đ` : '-' }}
                  </td>
                  <td class="px-6 py-4 text-right text-xs text-red-600 font-medium">
                    {{ salary.phat > 0 ? `-${salary.phat.toLocaleString('vi-VN')}đ` : '-' }}
                  </td>
                  <td class="px-6 py-4 text-right font-bold text-[#CC8033] bg-[#FDFBF7]">
                    {{ salary.thucLanh.toLocaleString('vi-VN') }}đ
                  </td>
                  <td class="px-8 py-4 text-center">
                    <span 
                      :class="salary.trangThai === 'DaTra' ? 'bg-emerald-50 text-emerald-600 border-emerald-100' : 'bg-amber-50 text-amber-700 border-amber-200'"
                      class="px-2.5 py-1 border rounded-full text-[9px] font-bold uppercase tracking-wider"
                    >
                      {{ salary.trangThai === 'DaTra' ? 'Đã chi trả' : 'Tạm tính' }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </template>

    <!-- Create Voucher Modal (Slide-over) -->
    <div v-if="isCreateModalOpen" class="fixed inset-0 z-[100] flex justify-end">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-[#2A231E]/40 backdrop-blur-sm transition-opacity" @click="isCreateModalOpen = false"></div>
      
      <!-- Panel -->
      <div class="relative w-full max-w-lg bg-[#FDFBF7] h-full shadow-2xl flex flex-col animate-in slide-in-from-right duration-300">
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-5 border-b border-[#EAE3D9] bg-white">
          <h2 class="text-xl font-premium-serif font-bold text-[#2A231E]">Tạo Phiếu Chi Mới</h2>
          <button @click="isCreateModalOpen = false" class="p-2 hover:bg-[#F5F2ED] rounded-full transition-colors text-[#8A8178]">
            <X class="w-5 h-5" />
          </button>
        </div>

        <!-- Body -->
        <div class="flex-1 overflow-y-auto p-6 space-y-6">
          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] grid grid-cols-2 gap-4">
            <div>
              <label class="block text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mb-1.5">Loại phiếu</label>
              <div class="font-bold text-red-600 tracking-wider">PHIẾU CHI TIỀN</div>
            </div>
            <div>
              <label class="block text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mb-1.5">Thời gian ghi nhận</label>
              <div class="font-bold text-[#2A231E] tracking-wider">{{ new Date().toLocaleString('vi-VN') }}</div>
            </div>
          </div>

          <!-- Classification -->
          <div class="space-y-4">
            <h3 class="text-xs font-bold text-[#8A6D53] border-b border-[#EAE3D9] pb-2 uppercase tracking-widest">Phân loại & Phương thức</h3>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Danh mục chi *</label>
                <select 
                  v-model="newVoucher.nhomGiaoDich"
                  class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold text-[#2A231E] focus:border-[#CC8033] outline-none"
                >
                  <option value="NhapHang">Nhập hàng / Nguyên liệu</option>
                  <option value="TraLuong">Chi trả lương nhân viên</option>
                  <option value="DienNuoc">Điện nước & Tiện ích</option>
                  <option value="MatBang">Tiền thuê mặt bằng</option>
                  <option value="Khac">Chi phí khác</option>
                </select>
              </div>
              <div>
                <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Hình thức thanh toán *</label>
                <select 
                  v-model="newVoucher.phuongThucThanhToan"
                  class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold text-[#2A231E] focus:border-[#CC8033] outline-none"
                >
                  <option value="ChuyenKhoan">Chuyển khoản ngân hàng</option>
                  <option value="TienMat">Tiền mặt thủ quỹ</option>
                </select>
              </div>
            </div>
          </div>

          <!-- Financials -->
          <div class="space-y-4">
            <h3 class="text-xs font-bold text-[#8A6D53] border-b border-[#EAE3D9] pb-2 uppercase tracking-widest">Thông tin tài chính</h3>
            
            <div>
              <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Số tiền chi (VNĐ) *</label>
              <input 
                v-model.number="newVoucher.soTien"
                type="number" 
                min="1000"
                placeholder="Ví dụ: 500000" 
                class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2.5 text-base font-bold bg-white text-[#CC8033] focus:border-[#CC8033] outline-none"
              />
            </div>

            <div>
              <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Người nhận tiền (Tên cá nhân/Nhà cung cấp) *</label>
              <input 
                v-model="newVoucher.nguoiNopNhan"
                type="text" 
                placeholder="Nhập tên đối tác nhận tiền..." 
                class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold focus:border-[#CC8033] outline-none"
              />
            </div>

            <div>
              <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Lý do chi chi tiết *</label>
              <textarea 
                v-model="newVoucher.ghiChu"
                rows="4" 
                placeholder="Mô tả lý do rõ ràng để đối soát (VD: Chi phí sửa bóng đèn vỡ ở tầng trệt)..." 
                class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white focus:border-[#CC8033] font-medium outline-none resize-none"
              ></textarea>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-5 border-t border-[#EAE3D9] bg-white flex justify-end gap-3 shadow-[0_-10px_20px_rgba(0,0,0,0.02)]">
          <button 
            @click="isCreateModalOpen = false" 
            :disabled="submitting"
            class="px-5 py-2.5 rounded-lg text-sm font-bold text-[#5C544E] hover:bg-[#F5F2ED] transition-colors disabled:opacity-50"
          >
            Hủy bỏ
          </button>
          <button 
            @click="handleCreateVoucher" 
            :disabled="submitting"
            class="px-6 py-2.5 rounded-lg text-sm font-bold bg-[#CC8033] hover:bg-[#B3702C] text-white shadow-md transition-all flex items-center gap-2 disabled:opacity-50"
          >
            <span v-if="submitting" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
            <Check v-else class="w-4 h-4" stroke-width="2.5" /> 
            <span>{{ submitting ? 'Đang lưu...' : 'Lưu phiếu chi' }}</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { TrendingUp, TrendingDown, Eye, Plus, X, Check, Search } from 'lucide-vue-next'
import { Bar, Doughnut } from 'vue-chartjs'
import { cashFlowService, type CashFlowListItem, type CashFlowSummary, type SalaryListItem } from '../services/cashflow'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
)

const activeTab = ref<'journal' | 'salaries'>('journal')
const isCreateModalOpen = ref(false)
const loading = ref(false)

// Select Period
const selectedOptionIdx = ref(0)
const selectedYear = ref(new Date().getFullYear())
const selectedMonth = ref(new Date().getMonth() + 1)
const monthOptions = ref<{ label: string; year: number; month: number }[]>([])

// Data references
const summaryData = ref<CashFlowSummary>({
  tongThu: 0,
  tongChi: 0,
  dongTienThuan: 0,
  chiLuong: 0,
  chiKho: 0,
  chiKhac: 0
})
const journalList = ref<CashFlowListItem[]>([])
const salariesList = ref<SalaryListItem[]>([])

// Create Voucher form
const submitting = ref(false)
const newVoucher = ref({
  nhomGiaoDich: 'NhapHang',
  phuongThucThanhToan: 'ChuyenKhoan',
  soTien: 0,
  nguoiNopNhan: '',
  ghiChu: ''
})

// Search & filters
const filterSearch = ref('')
const filterType = ref('TatCa')
const filterCategory = ref('TatCa')
const salarySearch = ref('')

// Pagination
const itemsPerPage = ref(10)
const currentPage = ref(1)

watch([filterSearch, filterType, filterCategory, itemsPerPage], () => {
  currentPage.value = 1
})

const generateMonthOptions = () => {
  const options = []
  const now = new Date()
  for (let i = 0; i < 12; i++) {
    const d = new Date(now.getFullYear(), now.getMonth() - i, 1)
    options.push({
      label: `Tháng ${String(d.getMonth() + 1).padStart(2, '0')} / ${d.getFullYear()}`,
      year: d.getFullYear(),
      month: d.getMonth() + 1
    })
  }
  monthOptions.value = options
}

const onPeriodChange = () => {
  const opt = monthOptions.value[selectedOptionIdx.value]
  if (opt) {
    selectedYear.value = opt.year
    selectedMonth.value = opt.month
    loadData()
  }
}

const loadData = async () => {
  loading.value = true
  try {
    const [summary, list, salaries] = await Promise.all([
      cashFlowService.summary(selectedYear.value, selectedMonth.value),
      cashFlowService.list(selectedYear.value, selectedMonth.value),
      cashFlowService.salaries(selectedYear.value, selectedMonth.value)
    ])
    summaryData.value = summary
    journalList.value = list
    salariesList.value = salaries
  } catch (err: any) {
    console.error(err)
    alert(err.message || 'Lỗi kết nối API dòng tiền')
  } finally {
    loading.value = false
  }
}

// Helpers
const getNhomLabel = (nhom: string) => {
  switch (nhom) {
    case 'DoanhThuPOS': return 'Doanh thu POS'
    case 'NhapHang': return 'Nhập nguyên liệu'
    case 'TraLuong': return 'Thanh toán lương'
    case 'DienNuoc': return 'Điện nước & Tiện ích'
    case 'MatBang': return 'Thuê mặt bằng'
    default: return 'Chi phí khác'
  }
}

const getPhuongThucLabel = (pt: string) => {
  return pt === 'ChuyenKhoan' ? 'Chuyển khoản' : 'Tiền mặt'
}

const formatPercent = (part: number, total: number) => {
  if (!total) return 0
  return Math.round((part / total) * 100)
}

// Computeds for Filters
const filteredJournal = computed(() => {
  return journalList.value.filter(item => {
    const matchSearch = !filterSearch.value || 
      (item.ghiChu && item.ghiChu.toLowerCase().includes(filterSearch.value.toLowerCase())) ||
      (item.nguoiNopNhan && item.nguoiNopNhan.toLowerCase().includes(filterSearch.value.toLowerCase())) ||
      (String(item.maDongTien).includes(filterSearch.value))

    const matchType = filterType.value === 'TatCa' || item.loaiGiaoDich === filterType.value
    const matchCategory = filterCategory.value === 'TatCa' || item.nhomGiaoDich === filterCategory.value

    return matchSearch && matchType && matchCategory
  })
})

const paginatedJournal = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredJournal.value.slice(start, end)
})

const totalPages = computed(() => {
  return Math.ceil(filteredJournal.value.length / itemsPerPage.value) || 1
})

const filteredSalaries = computed(() => {
  return salariesList.value.filter(item => {
    return !salarySearch.value || item.hoTen.toLowerCase().includes(salarySearch.value.toLowerCase())
  })
})

const totalSalarySum = computed(() => {
  return filteredSalaries.value.reduce((acc, curr) => acc + curr.thucLanh, 0)
})

// Computeds for Charts
const barChartData = computed(() => {
  const days = new Date(selectedYear.value, selectedMonth.value, 0).getDate()
  const labels = Array.from({ length: days }, (_, i) => `${i + 1}`)
  const rev = new Array(days).fill(0)
  const exp = new Array(days).fill(0)

  journalList.value.forEach(item => {
    const d = new Date(item.thoiGianTao)
    const day = d.getDate()
    if (day >= 1 && day <= days) {
      if (item.loaiGiaoDich === 'Thu') {
        rev[day - 1] += item.soTien
      } else {
        exp[day - 1] += item.soTien
      }
    }
  })

  return {
    labels,
    datasets: [
      {
        label: 'Tổng Thu',
        data: rev,
        backgroundColor: '#5C4533',
        borderRadius: 4,
        barPercentage: 0.6,
        categoryPercentage: 0.8
      },
      {
        label: 'Tổng Chi',
        data: exp,
        backgroundColor: '#C1A081',
        borderRadius: 4,
        barPercentage: 0.6,
        categoryPercentage: 0.8
      }
    ]
  }
})

const barChartOptions: any = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      mode: 'index',
      intersect: false,
      backgroundColor: '#2A231E',
      padding: 12,
      titleFont: { size: 13, family: 'Inter' },
      bodyFont: { size: 12, family: 'Inter' },
      cornerRadius: 8,
      callbacks: {
        label: (context: any) => {
          const val = context.raw || 0
          return ` ${context.dataset.label}: ${val.toLocaleString('vi-VN')}đ`
        }
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: { color: '#EAE3D9', drawBorder: false },
      ticks: {
        color: '#8A8178',
        font: { family: 'Inter', size: 10, weight: 'bold' },
        callback: (value: any) => (value / 1000000) + 'M'
      }
    },
    x: {
      grid: { display: false },
      ticks: { color: '#8A8178', font: { family: 'Inter', size: 10, weight: 'bold' } }
    }
  }
}

const doughnutChartData = computed(() => {
  const chiLuong = summaryData.value.chiLuong || 0
  const chiKho = summaryData.value.chiKho || 0
  const chiKhac = summaryData.value.chiKhac || 0

  return {
    labels: ['Lương nhân viên', 'Nguyên liệu', 'Chi phí khác'],
    datasets: [{
      data: [chiLuong, chiKho, chiKhac],
      backgroundColor: ['#5C4533', '#8A6D53', '#C1A081'],
      borderWidth: 2,
      borderColor: '#ffffff',
      hoverOffset: 4
    }]
  }
})

const doughnutOptions: any = {
  responsive: true,
  maintainAspectRatio: false,
  cutout: '65%',
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#2A231E',
      padding: 12,
      bodyFont: { size: 13, family: 'Inter', weight: 'bold' },
      cornerRadius: 8,
      displayColors: true,
      callbacks: {
        label: (context: any) => {
          const val = context.raw || 0
          return ` ${context.label}: ${val.toLocaleString('vi-VN')}đ`
        }
      }
    }
  }
}

// Action handlers
const handleCreateVoucher = async () => {
  if (newVoucher.value.soTien <= 0) {
    alert('Số tiền chi phải lớn hơn 0.')
    return
  }
  if (!newVoucher.value.nguoiNopNhan.trim()) {
    alert('Vui lòng nhập người nhận tiền.')
    return
  }
  if (!newVoucher.value.ghiChu.trim()) {
    alert('Vui lòng nhập lý do chi.')
    return
  }

  submitting.value = true
  try {
    await cashFlowService.create({
      nhomGiaoDich: newVoucher.value.nhomGiaoDich,
      phuongThucThanhToan: newVoucher.value.phuongThucThanhToan,
      soTien: newVoucher.value.soTien,
      nguoiNopNhan: newVoucher.value.nguoiNopNhan,
      ghiChu: newVoucher.value.ghiChu
    })
    
    newVoucher.value = {
      nhomGiaoDich: 'NhapHang',
      phuongThucThanhToan: 'ChuyenKhoan',
      soTien: 0,
      nguoiNopNhan: '',
      ghiChu: ''
    }
    isCreateModalOpen.value = false
    alert('Đã lưu phiếu chi thành công!')
    await loadData()
  } catch (err: any) {
    console.error(err)
    alert(err.message || 'Lỗi khi tạo phiếu chi')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  generateMonthOptions()
  loadData()
})
</script>
