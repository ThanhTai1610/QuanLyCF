<template>
  <div class="space-y-4 md:space-y-5 font-premium-sans text-[#2A231E] p-4 sm:p-6 max-w-[1400px] mx-auto flex flex-col">
   
    <!-- Thanh công cụ -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4 flex-shrink-0">
      <div class="flex items-center gap-3 w-full md:w-auto">
        <div class="relative flex-1 md:w-80">
          <Search class="w-5 h-5 absolute left-4 top-1/2 -translate-y-1/2 text-[#8A8178]" stroke-width="2" />
          <Input
            v-model="search"
            placeholder="Tìm mã hoá đơn, số bàn, thu ngân..."
            class="pl-12 w-full bg-white border border-[#EAE3D9] h-12 rounded-md shadow-card text-base font-medium"
          />
        </div>
        
        <!-- Nút chọn Thời gian -->
        <button 
          @click="showDateModal = true"
          class="border border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033] h-12 rounded-md bg-white shadow-card text-xs font-bold uppercase tracking-wider px-4 whitespace-nowrap inline-flex items-center justify-center cursor-pointer transition-all"
          :class="dateRangePreset !== 'all' ? 'border-[#CC8033] text-[#CC8033] bg-amber-50/50' : ''"
        >
          <Calendar class="w-4 h-4 mr-2 text-[#CC8033]" stroke-width="2" />
          <span>{{ datePresetLabel }}</span>
        </button>

        <!-- Nút Bộ lọc nâng cao -->
        <button 
          @click="showFilterModal = true"
          class="relative border border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033] h-12 w-12 rounded-md bg-white shadow-card p-0 inline-flex items-center justify-center flex-shrink-0 cursor-pointer transition-all"
          :class="activeFilterCount > 0 ? 'border-[#CC8033] text-[#CC8033] bg-amber-50/50' : ''"
          title="Bộ lọc nâng cao"
        >
          <Filter class="w-4 h-4" stroke-width="2" />
          <span v-if="activeFilterCount > 0" class="absolute -top-1.5 -right-1.5 w-5 h-5 rounded-full bg-[#CC8033] text-white font-extrabold text-[10px] flex items-center justify-center shadow-xs">
            {{ activeFilterCount }}
          </span>
        </button>
      </div>
     
      <button class="inline-flex items-center justify-center w-full md:w-auto bg-[#CC8033] hover:bg-[#B36B25] text-white h-12 rounded-md shadow-card border border-[#CC8033]/30 text-xs font-bold uppercase tracking-wider px-4 cursor-pointer transition-colors">
        <FileText class="w-4 h-4 mr-2" stroke-width="2.5" /> Xuất báo cáo
      </button>
    </div>

    <!-- Active Filter Tags Row -->
    <div v-if="activeFilterCount > 0 || search.trim()" class="flex items-center gap-2 flex-wrap text-xs pt-1">
      <span class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Đang lọc:</span>
      <span v-if="search.trim()" class="inline-flex items-center gap-1 px-2.5 py-1 rounded-md bg-white border border-[#EAE3D9] text-[#2A231E] font-semibold">
        Từ khóa: "{{ search }}"
        <X class="w-3 h-3 text-[#8A8178] hover:text-red-500 cursor-pointer ml-1" @click="search = ''" />
      </span>
      <span v-if="dateRangePreset !== 'all'" class="inline-flex items-center gap-1 px-2.5 py-1 rounded-md bg-amber-50 border border-amber-200 text-[#CC8033] font-semibold">
        Thời gian: {{ datePresetLabel }}
        <X class="w-3 h-3 text-[#CC8033] hover:text-red-500 cursor-pointer ml-1" @click="dateRangePreset = 'all'" />
      </span>
      <span v-if="filterMethod !== 'all'" class="inline-flex items-center gap-1 px-2.5 py-1 rounded-md bg-amber-50 border border-amber-200 text-[#CC8033] font-semibold">
        Thanh toán: {{ filterMethod === 'TienMat' ? 'Tiền mặt' : filterMethod === 'VietQR' ? 'VietQR' : 'MoMo' }}
        <X class="w-3 h-3 text-[#CC8033] hover:text-red-500 cursor-pointer ml-1" @click="filterMethod = 'all'" />
      </span>
      <span v-if="filterStatus !== 'all'" class="inline-flex items-center gap-1 px-2.5 py-1 rounded-md bg-amber-50 border border-amber-200 text-[#CC8033] font-semibold">
        Trạng thái: {{ filterStatus === 'paid' ? 'Đã thanh toán' : 'Chưa in' }}
        <X class="w-3 h-3 text-[#CC8033] hover:text-red-500 cursor-pointer ml-1" @click="filterStatus = 'all'" />
      </span>
      <span v-if="filterTable !== 'all'" class="inline-flex items-center gap-1 px-2.5 py-1 rounded-md bg-amber-50 border border-amber-200 text-[#CC8033] font-semibold">
        Loại đơn: {{ filterTable === 'dinein' ? 'Tại bàn' : 'Mang về' }}
        <X class="w-3 h-3 text-[#CC8033] hover:text-red-500 cursor-pointer ml-1" @click="filterTable = 'all'" />
      </span>
      <button @click="resetAllFilters" class="text-[11px] font-bold text-red-500 hover:underline ml-2 cursor-pointer">
        Xóa tất cả bộ lọc
      </button>
    </div>

    <!-- Thanh Bulk Actions -->
    <div v-if="selected.length > 0" class="bg-[#2A231E] text-[#FDFBF7] rounded-md px-4 py-2.5 flex items-center justify-between shadow-card flex-shrink-0">
      <span class="text-xs font-bold uppercase tracking-widest text-[#CC8033]">Đã chọn {{ selected.length }} hoá đơn</span>
      <div class="flex gap-2">
        <Button variant="outline" class="h-8 text-[10px] font-bold uppercase tracking-wider bg-transparent border-white/20 text-white rounded-md px-3">
          <Printer class="w-3 h-3 mr-1.5" stroke-width="2" /> In tất cả
        </Button>
        <Button class="h-8 text-[10px] font-bold uppercase tracking-wider bg-[#CC8033] text-white border-none rounded-md shadow-xl px-3">
          <Download class="w-3 h-3 mr-1.5" stroke-width="2" /> Xuất Excel
        </Button>
      </div>
    </div>

    <!-- Bảng dữ liệu -->
    <div class="bg-white rounded-md border border-[#EAE3D9] shadow-card flex flex-col overflow-hidden">
      <div class="overflow-x-auto custom-scrollbar">
        <table class="w-full text-left">
          <thead>
            <tr class="bg-[#FDFBF7] text-[#8A8178] text-[9px] uppercase tracking-[0.1em] border-b-2 border-[#EAE3D9]">
              <th class="px-4 py-3 w-10 text-center">
                <input
                  type="checkbox"
                  @change="toggleAll"
                  :checked="selected.length === filteredInvoices.length && filteredInvoices.length > 0"
                  class="rounded-md border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033] w-3.5 h-3.5 cursor-pointer"
                />
              </th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Mã HĐ</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap text-center">LOẠI ĐƠN &amp; BÀN</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Ngày & Giờ</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap text-right">Tổng tiền</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Thanh toán</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap">Thu ngân</th>
              <th class="px-4 py-3 font-bold whitespace-nowrap text-center">Trạng thái</th>
              <th class="pl-4 pr-8 py-3 font-bold whitespace-nowrap text-right w-32">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="inv in paginatedItems" :key="inv.id" :class="['border-b-2 border-[#EAE3D9]/60', selected.includes(inv.id) ? 'bg-[#CC8033]/5' : '']">
              <td class="px-4 py-2.5 text-center">
                <input
                  type="checkbox"
                  :checked="selected.includes(inv.id)"
                  @change="toggle(inv.id)"
                  class="rounded-md border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033] w-3.5 h-3.5 cursor-pointer"
                />
              </td>
              <td class="px-4 py-2.5 font-bold text-[#2A231E] text-xs whitespace-nowrap">{{ inv.id }}</td>
              <td class="px-4 py-2.5 text-center whitespace-nowrap">
                <span v-if="inv.orderType === 'takeaway' || inv.table === 'Mang về'" class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full bg-amber-100 text-[#CC8033] border border-amber-300 text-[10px] font-black uppercase tracking-wider shadow-2xs">
                  🛍️ Mang về
                </span>
                <span v-else class="inline-flex items-center gap-1 px-2.5 py-1 rounded-full bg-emerald-100 text-emerald-800 border border-emerald-300 text-[10px] font-black uppercase tracking-wider shadow-2xs">
                  🪑 {{ inv.table.toLowerCase().includes('bàn') ? inv.table : `Tại bàn (${inv.table})` }}
                </span>
              </td>
              <td class="px-4 py-2.5 text-[#8A8178] text-[11px] font-medium whitespace-nowrap">{{ inv.time }}</td>
              <td class="px-4 py-2.5 font-bold text-[#CC8033] text-xs text-right whitespace-nowrap">{{ formatVND(inv.total) }}</td>
              <td class="px-4 py-2.5 text-[#5C544E] text-[11px] font-medium whitespace-nowrap">{{ inv.method }}</td>
              <td class="px-4 py-2.5 text-[#5C544E] text-[11px] font-medium whitespace-nowrap">{{ inv.staff }}</td>
              <td class="px-4 py-2.5 text-center whitespace-nowrap">
                <div :class="['inline-flex items-center gap-1.5 px-3 py-1.5 rounded-md border shadow-xl', statusBadge[inv.status].cls]">
                  <div :class="['w-1.5 h-1.5 rounded-full', statusBadge[inv.status].dot]"></div>
                  <span class="text-[9px] font-bold uppercase tracking-[0.1em]">{{ statusBadge[inv.status].label }}</span>
                </div>
              </td>
              <!-- NÚT LUÔN HIỂN THỊ -->
              <td class="pl-4 pr-8 py-2.5">
                <div class="flex justify-end gap-1.5">
                  <button @click="openPreview(inv)" class="p-2 text-[#8A8178] border border-[#EAE3D9] rounded-md shadow-xl" title="Xem chi tiết">
                    <Eye class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button @click="printInv" class="p-2 text-[#8A8178] border border-[#EAE3D9] rounded-md shadow-xl" title="In lại">
                    <Printer class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button @click="downloadInv" class="p-2 text-[#8A8178] border border-[#EAE3D9] rounded-md shadow-xl" title="Tải PDF">
                    <Download class="w-4 h-4" stroke-width="2" />
                  </button>
                </div>
              </td>
            </tr>
            <tr v-if="paginatedItems.length === 0">
              <td colspan="9" class="px-4 py-10 text-center text-[#8A8178] text-xs font-medium">
                Không tìm thấy hóa đơn nào phù hợp.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div v-if="filteredInvoices.length > 0" class="flex items-center justify-between px-6 py-4 border-t-2 border-[#EAE3D9] bg-[#FDFBF7]">
        <div class="text-xs font-medium text-[#8A8178]">
          Hiển thị <span class="text-[#2A231E]">{{ (currentPage - 1) * itemsPerPage + 1 }}</span> - <span class="text-[#2A231E]">{{ Math.min(currentPage * itemsPerPage, filteredInvoices.length) }}</span> / <span class="text-[#2A231E]">{{ filteredInvoices.length }}</span> hóa đơn
        </div>
        <div class="flex items-center gap-2">
          <button 
            @click="currentPage--" 
            :disabled="currentPage === 1"
            class="p-2 border border-[#EAE3D9] bg-white rounded-md hover:bg-[#FDFBF7] disabled:opacity-50 transition-colors shadow-sm text-[#2A231E]"
          >
            <ChevronLeft class="w-4 h-4" />
          </button>
          <span class="text-xs font-bold text-[#2A231E] px-2">
            Trang {{ currentPage }} / {{ totalPages }}
          </span>
          <button 
            @click="currentPage++" 
            :disabled="currentPage === totalPages"
            class="p-2 border border-[#EAE3D9] bg-white rounded-md hover:bg-[#FDFBF7] disabled:opacity-50 transition-colors shadow-sm text-[#2A231E]"
          >
            <ChevronRight class="w-4 h-4" />
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Chi tiết Hóa Đơn -->
    <Modal v-model="isPreviewOpen">
      <div v-if="preview" class="bg-white rounded-md border border-[#EAE3D9] shadow-card overflow-hidden sm:max-w-md w-full mx-auto">
        <div class="p-8 pb-4">
          <div class="text-center border-b border-dashed border-[#EAE3D9] pb-5 mb-5">
            <div class="w-10 h-10 mx-auto bg-[#2A231E] rounded-md flex items-center justify-center text-white mb-3">
              <Coffee class="w-5 h-5" />
            </div>
            <h2 class="font-premium-serif text-3xl font-bold text-[#2A231E]">{{ storeInfoStore.tenQuan }}</h2>
            <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mt-2">{{ storeInfoStore.diaChi || '123 Nguyễn Huệ, Q1, TP.HCM' }}</p>
            <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mt-0.5">Hotline: {{ storeInfoStore.soDienThoai || '0909 123 456' }}</p>
          </div>
         
          <div class="py-2 text-[11px] font-medium text-[#5C544E] space-y-2">
            <div class="flex justify-between"><span class="text-[#8A8178] uppercase tracking-wider">Mã HĐ:</span><span class="font-bold text-[#2A231E]">HD-{{ preview.maHoaDon.toString().padStart(4, '0') }}</span></div>
            <div class="flex justify-between"><span class="text-[#8A8178] uppercase tracking-wider">Bàn / Đơn:</span><span class="font-bold text-[#2A231E]">{{ preview.tenBan || 'Mang về' }} ({{ formatDateTime(preview.thoiGianThanhToan) }})</span></div>
            <div class="flex justify-between"><span class="text-[#8A8178] uppercase tracking-wider">Thu ngân:</span><span class="font-bold text-[#2A231E]">{{ preview.tenNhanVienThuNgan || preview.tenThuNgan || 'Hệ thống' }}</span></div>
          </div>
         
          <div class="border-t border-dashed border-[#EAE3D9] my-4 pt-4 pb-2 space-y-3 text-[11px] font-medium text-[#2A231E]">
            <div v-for="(it, idx) in preview.items" :key="idx" class="flex justify-between items-start">
              <div>
                <span class="font-bold text-sm text-[#2A231E]">
                  {{ it.tenSanPham || it.tenMon || 'Món đồ uống' }}
                  <span v-if="it.tenKichCo" class="text-xs font-semibold text-[#8A8178]">({{ it.tenKichCo }})</span>
                </span>
                <div v-if="it.ghiChuMon" class="text-[10px] text-amber-700 font-semibold italic mt-0.5">
                  {{ it.ghiChuMon }}
                </div>
                <div class="text-[#8A8178] text-[10px] mt-0.5 font-mono">{{ formatVND(it.donGia) }} × {{ it.soLuong }}</div>
              </div>
              <span class="font-bold text-sm text-[#CC8033]">{{ formatVND(it.thanhTien) }}</span>
            </div>
          </div>
         
          <div class="border-t border-black/10 pt-4 mt-2 space-y-1">
            <div v-if="preview.tongTienHang && preview.tienGiam > 0" class="flex justify-between text-xs text-[#8A8178]">
              <span>Tạm tính tiền hàng:</span>
              <span>{{ formatVND(preview.tongTienHang) }}</span>
            </div>
            <div v-if="preview.tienGiam > 0" class="flex justify-between text-xs text-red-500 font-semibold">
              <span>Giảm giá / Voucher:</span>
              <span>-{{ formatVND(preview.tienGiam) }}</span>
            </div>
            <div class="flex justify-between items-end font-premium-sans pt-1 border-t border-dashed border-[#EAE3D9]">
              <span class="text-xl font-bold text-[#2A231E]">Tổng cộng</span>
              <span class="text-3xl font-bold text-[#CC8033]">
                {{ formatVND(preview.thanhTien ?? preview.tongThanhTien ?? preview.tongTienHang ?? 0) }}
              </span>
            </div>
            <p class="text-right text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mt-2">
              Thanh toán qua: <span class="text-[#CC8033] font-bold">{{ preview.phuongThuc || (preview.payments && preview.payments[0]?.phuongThuc) || 'Tiền mặt' }}</span>
            </p>
          </div>
         
          <div class="text-center mt-8">
            <p class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold">Cảm ơn quý khách & Hẹn gặp lại</p>
            <p class="text-lg mt-1">☕</p>
          </div>
        </div>
        
        <div class="p-4 bg-[#FDFBF7] border-t-2 border-[#EAE3D9] flex gap-2 rounded-b-xl">
          <Button variant="outline" class="flex-1 border border-[#EAE3D9] text-[#5C544E] h-10 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-xl" @click="isPreviewOpen = false">
            Đóng
          </Button>
          <Button class="flex-1 bg-white border border-[#EAE3D9] text-[#5C544E] h-10 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-xl" @click="printInv">
            <Printer class="w-3.5 h-3.5 mr-1.5" stroke-width="2" /> In HĐ
          </Button>
          <Button class="flex-1 bg-[#CC8033] text-white border-none h-10 rounded-md text-[10px] font-bold uppercase tracking-wider shadow-xl" @click="downloadInv">
            <Download class="w-3.5 h-3.5 mr-1.5" stroke-width="2.5" /> Lưu PDF
          </Button>
        </div>
      </div>
    </Modal>

    <!-- Modal Lọc Theo Thời Gian -->
    <div v-if="showDateModal" class="fixed inset-0 bg-black/50 backdrop-blur-xs flex items-center justify-center z-[100] p-4" @click="showDateModal = false">
      <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-2xl p-6 max-w-sm w-full space-y-4 animate-in zoom-in-95 duration-200" @click.stop>
        <div class="flex items-center justify-between border-b border-[#EAE3D9] pb-3">
          <h3 class="font-premium-serif text-base font-bold text-[#2A231E] flex items-center gap-2">
            <Calendar class="w-4 h-4 text-[#CC8033]" /> Chọn Khoảng Thời Gian
          </h3>
          <button @click="showDateModal = false" class="text-[#8A8178] hover:text-[#2A231E] text-lg font-bold">×</button>
        </div>

        <div class="space-y-2">
          <button 
            v-for="p in [
              { id: 'all', label: 'Tất cả thời gian' },
              { id: 'today', label: 'Hôm nay' },
              { id: 'yesterday', label: 'Hôm qua' },
              { id: '7days', label: '7 ngày qua' },
              { id: 'month', label: 'Tháng này' }
            ]" 
            :key="p.id"
            @click="dateRangePreset = p.id as any; showDateModal = false"
            class="w-full text-left px-4 py-2.5 rounded-xl text-xs font-bold transition-all border flex items-center justify-between cursor-pointer"
            :class="dateRangePreset === p.id ? 'bg-[#CC8033] text-white border-[#CC8033] shadow-xs' : 'bg-[#FAF6F0] text-[#5C544E] border-[#EAE3D9] hover:bg-[#F5F2ED]'"
          >
            <span>{{ p.label }}</span>
            <span v-if="dateRangePreset === p.id">✓</span>
          </button>
        </div>

        <div class="border-t border-[#EAE3D9] pt-3 space-y-3 text-left">
          <span class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block">Hoặc tùy chọn ngày:</span>
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="text-[10px] font-semibold text-[#8A8178] block mb-1">Từ ngày:</label>
              <input v-model="startDate" type="date" class="w-full px-2.5 py-2 bg-[#FAF6F0] border border-[#EAE3D9] rounded-xl text-xs font-medium focus:outline-none focus:border-[#CC8033]" />
            </div>
            <div>
              <label class="text-[10px] font-semibold text-[#8A8178] block mb-1">Đến ngày:</label>
              <input v-model="endDate" type="date" class="w-full px-2.5 py-2 bg-[#FAF6F0] border border-[#EAE3D9] rounded-xl text-xs font-medium focus:outline-none focus:border-[#CC8033]" />
            </div>
          </div>
          <button 
            @click="dateRangePreset = 'custom'; showDateModal = false"
            :disabled="!startDate || !endDate"
            class="w-full py-2.5 rounded-xl bg-[#CC8033] text-white font-bold text-xs uppercase tracking-wider transition-all disabled:opacity-50 cursor-pointer shadow-xs"
          >
            Áp dụng ngày tùy chọn
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Bộ Lọc Nâng Cao -->
    <div v-if="showFilterModal" class="fixed inset-0 bg-black/50 backdrop-blur-xs flex items-center justify-center z-[100] p-4" @click="showFilterModal = false">
      <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-2xl p-6 max-w-md w-full space-y-5 animate-in zoom-in-95 duration-200" @click.stop>
        <div class="flex items-center justify-between border-b border-[#EAE3D9] pb-3">
          <h3 class="font-premium-serif text-base font-bold text-[#2A231E] flex items-center gap-2">
            <Filter class="w-4 h-4 text-[#CC8033]" /> Bộ Lọc Nâng Cao
          </h3>
          <button @click="showFilterModal = false" class="text-[#8A8178] hover:text-[#2A231E] text-lg font-bold">×</button>
        </div>

        <div class="space-y-4 text-left">
          <!-- 1. Phương thức thanh toán -->
          <div class="space-y-2">
            <label class="text-[11px] font-bold uppercase tracking-wider text-[#8A8178] block">Phương thức thanh toán:</label>
            <div class="grid grid-cols-2 gap-2">
              <button 
                v-for="m in [
                  { id: 'all', label: 'Tất cả phương thức' },
                  { id: 'TienMat', label: '💵 Tiền mặt' },
                  { id: 'VietQR', label: '🏦 VietQR / CK' },
                  { id: 'Momo', label: '📱 Ví MoMo' }
                ]"
                :key="m.id"
                @click="filterMethod = m.id"
                class="py-2.5 px-3 rounded-xl text-xs font-bold border transition-all cursor-pointer text-center"
                :class="filterMethod === m.id ? 'bg-[#CC8033] text-white border-[#CC8033] shadow-xs' : 'bg-[#FAF6F0] text-[#5C544E] border-[#EAE3D9] hover:bg-[#F5F2ED]'"
              >
                {{ m.label }}
              </button>
            </div>
          </div>

          <!-- 2. Trạng thái -->
          <div class="space-y-2">
            <label class="text-[11px] font-bold uppercase tracking-wider text-[#8A8178] block">Trạng thái hóa đơn:</label>
            <div class="grid grid-cols-3 gap-2">
              <button 
                v-for="st in [
                  { id: 'all', label: 'Tất cả' },
                  { id: 'paid', label: 'Đã thanh toán' },
                  { id: 'unprinted', label: 'Chưa in' }
                ]"
                :key="st.id"
                @click="filterStatus = st.id"
                class="py-2.5 px-2 rounded-xl text-xs font-bold border transition-all cursor-pointer text-center"
                :class="filterStatus === st.id ? 'bg-[#CC8033] text-white border-[#CC8033] shadow-xs' : 'bg-[#FAF6F0] text-[#5C544E] border-[#EAE3D9] hover:bg-[#F5F2ED]'"
              >
                {{ st.label }}
              </button>
            </div>
          </div>

          <!-- 3. Loại đơn hàng / Bàn -->
          <div class="space-y-2">
            <label class="text-[11px] font-bold uppercase tracking-wider text-[#8A8178] block">Loại đơn hàng:</label>
            <div class="grid grid-cols-3 gap-2">
              <button 
                v-for="t in [
                  { id: 'all', label: 'Tất cả loại' },
                  { id: 'dinein', label: '🪑 Tại bàn' },
                  { id: 'takeaway', label: '🛍 Mang về' }
                ]"
                :key="t.id"
                @click="filterTable = t.id"
                class="py-2.5 px-2 rounded-xl text-xs font-bold border transition-all cursor-pointer text-center"
                :class="filterTable === t.id ? 'bg-[#CC8033] text-white border-[#CC8033] shadow-xs' : 'bg-[#FAF6F0] text-[#5C544E] border-[#EAE3D9] hover:bg-[#F5F2ED]'"
              >
                {{ t.label }}
              </button>
            </div>
          </div>
        </div>

        <div class="flex gap-2.5 pt-2 border-t border-[#EAE3D9]">
          <button 
            @click="resetAllFilters"
            class="flex-1 py-2.5 rounded-xl border border-[#EAE3D9] text-[#5C544E] hover:bg-[#FAF6F0] font-bold text-xs uppercase tracking-wider transition-colors cursor-pointer"
          >
            Bỏ lọc
          </button>
          <button 
            @click="showFilterModal = false"
            class="flex-1 py-2.5 rounded-xl bg-[#CC8033] text-white font-bold text-xs uppercase tracking-wider transition-colors cursor-pointer shadow-xs"
          >
            Áp dụng ({{ activeFilterCount }})
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { Search, Calendar, Download, Eye, Printer, FileText, Filter, Coffee, ChevronLeft, ChevronRight, Trash2, X } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { invoicesApi, type InvoiceListItemDto, type InvoiceDetailDto } from '@/services/invoices'
import { useToast } from '@/stores/toast'

const storeInfoStore = useStoreInfoStore()
const toast = useToast()

const showConfirmClearAllModal = ref(false)
const clearingInvoices = ref(false)

const handleClearAllInvoices = async () => {
  clearingInvoices.value = true
  try {
    const res = await invoicesApi.clearAll()
    toast.success(res.message || 'Đã xóa sạch tất cả hóa đơn mẫu!', 'Thành công')
    showConfirmClearAllModal.value = false
    invoices.value = []
    selected.value = []
  } catch (err: any) {
    toast.error(err.message || 'Lỗi khi xóa tất cả hóa đơn')
  } finally {
    clearingInvoices.value = false
  }
}

const formatVND = (val: number) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
const formatDateTime = (val: string) => {
  const d = new Date(val)
  return d.toLocaleString('vi-VN', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit', year: 'numeric' })
}

interface Invoice {
  id: string
  originalId: number
  table: string
  orderType: 'dinein' | 'takeaway'
  time: string
  total: number
  method: string
  staff: string
  status: "paid" | "unprinted" | "printed"
}

const invoices = ref<Invoice[]>([])

const fetchInvoices = async () => {
  try {
    const res = await invoicesApi.list()
    invoices.value = res.items.map(h => {
      const isTakeaway = h.loaiDonHang === 'TakeAway' || h.tenBan === 'Mang về' || !h.tenBan
      return {
        id: `HD-${h.maHoaDon.toString().padStart(4, '0')}`,
        originalId: h.maHoaDon,
        table: isTakeaway ? 'Mang về' : h.tenBan!,
        orderType: isTakeaway ? 'takeaway' : 'dinein',
        time: formatDateTime(h.thoiGianThanhToan),
        total: h.tongThanhTien,
        method: h.phuongThuc || 'Không rõ',
        staff: h.tenThuNgan || 'Hệ thống',
        status: h.trangThai === 'DaThanhToan' ? 'paid' : 'unprinted'
      }
    })
  } catch (err) {
    toast.error('Lỗi khi tải danh sách hóa đơn')
  }
}

onMounted(() => {
  fetchInvoices()
})

const statusBadge = {
  paid: { label: "Đã thanh toán", cls: "bg-[#F0FDF4] text-[#166534] border-[#BBF7D0]", dot: "bg-[#166534]" },
  unprinted: { label: "Chưa in", cls: "bg-[#FFF9F2] text-[#CC8033] border-[#E8C5A5]", dot: "bg-[#CC8033]" },
  printed: { label: "Đã in HĐ", cls: "bg-[#F5F2ED] text-[#5C544E] border-[#EAE3D9]", dot: "bg-[#8A8178]" },
}

const selected = ref<string[]>([])
const preview = ref<InvoiceDetailDto | null>(null)
const previewBasic = ref<Invoice | null>(null)
const search = ref("")
const currentPage = ref(1)
const itemsPerPage = ref(8)

const isPreviewOpen = computed({
  get: () => preview.value !== null,
  set: (val) => { 
    if (!val) {
      preview.value = null
      previewBasic.value = null
    }
  }
})

const openPreview = async (inv: Invoice) => {
  try {
    const res = await invoicesApi.get(inv.originalId)
    preview.value = res
    previewBasic.value = inv
  } catch (err) {
    toast.error('Lỗi tải chi tiết hóa đơn')
  }
}

// --- State Bộ Lọc ---
const showDateModal = ref(false)
const showFilterModal = ref(false)

const dateRangePreset = ref<'all' | 'today' | 'yesterday' | '7days' | 'month' | 'custom'>('all')
const startDate = ref('')
const endDate = ref('')

const filterMethod = ref<string>('all') // all, TienMat, VietQR, Momo
const filterStatus = ref<string>('all') // all, paid, unprinted
const filterTable = ref<string>('all') // all, dinein, takeaway

const activeFilterCount = computed(() => {
  let c = 0
  if (dateRangePreset.value !== 'all') c++
  if (filterMethod.value !== 'all') c++
  if (filterStatus.value !== 'all') c++
  if (filterTable.value !== 'all') c++
  return c
})

const datePresetLabel = computed(() => {
  switch (dateRangePreset.value) {
    case 'today': return 'Hôm nay'
    case 'yesterday': return 'Hôm qua'
    case '7days': return '7 ngày qua'
    case 'month': return 'Tháng này'
    case 'custom': return `${startDate.value || '...'} → ${endDate.value || '...'}`
    default: return 'Tất cả thời gian'
  }
})

const resetAllFilters = () => {
  search.value = ''
  dateRangePreset.value = 'all'
  startDate.value = ''
  endDate.value = ''
  filterMethod.value = 'all'
  filterStatus.value = 'all'
  filterTable.value = 'all'
}

const filteredInvoices = computed(() => {
  const query = search.value.toLowerCase().trim()

  return invoices.value.filter((i) => {
    // 1. Tìm kiếm nhanh
    const matchSearch = !query || 
      i.id.toLowerCase().includes(query) || 
      i.table.toLowerCase().includes(query) || 
      i.staff.toLowerCase().includes(query) ||
      i.method.toLowerCase().includes(query)

    if (!matchSearch) return false

    // 2. Lọc Phương thức thanh toán
    if (filterMethod.value !== 'all') {
      const m = i.method.toLowerCase()
      if (filterMethod.value === 'TienMat' && !m.includes('tiền mặt') && !m.includes('cash')) return false
      if (filterMethod.value === 'VietQR' && !m.includes('vietqr') && !m.includes('ngân hàng') && !m.includes('chuyển khoản')) return false
      if (filterMethod.value === 'Momo' && !m.includes('momo')) return false
    }

    // 3. Lọc Trạng thái
    if (filterStatus.value !== 'all' && i.status !== filterStatus.value) return false

    // 4. Lọc theo Bàn / Mang về
    if (filterTable.value !== 'all') {
      if (filterTable.value === 'takeaway' && !i.table.toLowerCase().includes('mang về') && !i.table.toLowerCase().includes('trống')) return false
      if (filterTable.value === 'dinein' && i.table.toLowerCase().includes('mang về')) return false
    }

    return true
  })
})

const totalPages = computed(() => Math.ceil(filteredInvoices.value.length / itemsPerPage.value) || 1)

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredInvoices.value.slice(start, start + itemsPerPage.value)
})

watch(search, () => {
  currentPage.value = 1
})

// Xử lý chọn checkbox
const toggle = (id: string) => {
  if (selected.value.includes(id)) {
    selected.value = selected.value.filter(x => x !== id)
  } else {
    selected.value.push(id)
  }
}

const toggleAll = (e: Event) => {
  const isChecked = (e.target as HTMLInputElement).checked
  if (isChecked) {
    selected.value = filteredInvoices.value.map(i => i.id)
  } else {
    selected.value = []
  }
}

const printInv = () => alert('Đang tiến hành in hóa đơn...')
const downloadInv = () => alert('Đang xuất dữ liệu PDF...')
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans {
  font-family: 'Be Vietnam Pro', system-ui, sans-serif;
}

.custom-scrollbar::-webkit-scrollbar {
  height: 4px;
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: rgba(42, 35, 30, 0.1);
  border-radius: 4px;
}
</style>
