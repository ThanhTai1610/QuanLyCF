<template>
  <div class="space-y-6 font-premium-sans text-[#2A231E] p-4 sm:p-6 lg:p-8 max-w-[1400px] mx-auto min-h-screen">

    <!-- ===== FLOW STEPPER (luồng hoàn chỉnh) ===== -->
    <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-sm p-4 flex flex-wrap items-center gap-2 text-xs font-bold uppercase tracking-wider">
      <span class="text-[10px] text-[#8A8178]">Luồng nhập kho:</span>
      <span class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#CC8033] text-white"><Truck class="w-3.5 h-3.5" /> Nguồn cung</span>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <span class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#CC8033] text-white"><ClipboardList class="w-3.5 h-3.5" /> Phiếu nhập</span>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <router-link to="/inventory" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><Package class="w-3.5 h-3.5" /> Kho</router-link>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <router-link to="/stocktake" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><ClipboardCheck class="w-3.5 h-3.5" /> Kiểm kê</router-link>
    </div>

    <!-- Tiêu đề và Tabs -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div class="flex bg-white rounded-lg p-1 border border-[#EAE3D9] shadow-sm">
        <button
          @click="activeTab = 'suppliers'"
          :class="['px-5 py-2.5 rounded-md text-xs font-bold uppercase tracking-wider transition-all duration-300 flex items-center gap-2', activeTab === 'suppliers' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]']"
        >
          <Users class="w-4 h-4" /> Đối tác cung ứng
          <span class="px-1.5 py-0.5 rounded text-[10px]" :class="activeTab === 'suppliers' ? 'bg-white/20' : 'bg-[#EAE3D9]/60'">{{ suppliers.length }}</span>
        </button>
        <button
          @click="activeTab = 'inbound'"
          :class="['px-5 py-2.5 rounded-md text-xs font-bold uppercase tracking-wider transition-all duration-300 flex items-center gap-2', activeTab === 'inbound' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]']"
        >
          <ClipboardList class="w-4 h-4" /> Phiếu nhập kho
          <span class="px-1.5 py-0.5 rounded text-[10px]" :class="activeTab === 'inbound' ? 'bg-white/20' : 'bg-[#EAE3D9]/60'">{{ receipts.length }}</span>
        </button>
      </div>

      <button
        @click="openCreateReceipt"
        class="flex items-center justify-center bg-[#2A231E] hover:bg-[#3D332A] text-white h-11 px-6 rounded-lg shadow-md transition-colors text-xs font-bold uppercase tracking-wider whitespace-nowrap"
      >
        <Plus class="w-4 h-4 mr-2" /> Tạo Phiếu Nhập Mới
      </button>
    </div>

    <!-- ===================================================================== -->
    <!-- TAB 1: Danh bạ & Công nợ Nhà cung cấp -->
    <!-- ===================================================================== -->
    <div v-show="activeTab === 'suppliers'" class="animate-in fade-in slide-in-from-bottom-4 duration-500 space-y-6">

      <!-- Summary cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="bg-white border border-[#EAE3D9] p-4 rounded-xl shadow-sm flex items-center justify-between">
          <div>
            <p class="text-xs font-bold text-[#8A8178] uppercase tracking-wider mb-1">Tổng nợ phải trả</p>
            <h3 class="text-2xl font-bold text-red-500">{{ formatVND(totalDebt) }}</h3>
          </div>
          <div class="w-12 h-12 rounded-full bg-red-50 flex items-center justify-center text-red-500"><TrendingDown class="w-6 h-6" /></div>
        </div>
        <div class="bg-white border border-[#EAE3D9] p-4 rounded-xl shadow-sm flex items-center justify-between">
          <div>
            <p class="text-xs font-bold text-[#8A8178] uppercase tracking-wider mb-1">Số đối tác</p>
            <h3 class="text-2xl font-bold text-[#2A231E]">{{ suppliers.length }}</h3>
          </div>
          <div class="w-12 h-12 rounded-full bg-[#FDFBF7] border border-[#EAE3D9] flex items-center justify-center text-[#2A231E]"><Users class="w-6 h-6" /></div>
        </div>
        <div class="bg-white border border-[#EAE3D9] p-4 rounded-xl shadow-sm flex items-center justify-between">
          <div>
            <p class="text-xs font-bold text-[#8A8178] uppercase tracking-wider mb-1">Đang còn nợ</p>
            <h3 class="text-2xl font-bold text-[#CC8033]">{{ suppliers.filter(s => s.debt > 0).length }}</h3>
          </div>
          <div class="w-12 h-12 rounded-full bg-[#FFF9F2] border border-[#E8C5A5] flex items-center justify-center text-[#CC8033]"><AlertTriangle class="w-6 h-6" /></div>
        </div>
      </div>
      <!-- Debt Notification Alert -->
      <div v-if="suppliersWithDebt.length > 0" class="mb-4 bg-white border border-red-200 rounded-xl shadow-[0_4px_20px_rgba(239,68,68,0.05)] flex items-center p-3 pr-4 animate-in fade-in duration-300">
        <div class="w-10 h-10 rounded-full bg-red-50 flex items-center justify-center mr-3 text-red-500 relative">
          <AlertTriangle class="w-5 h-5" />
          <span class="absolute -top-1 -right-1 flex h-3 w-3">
            <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-red-400 opacity-75"></span>
            <span class="relative inline-flex rounded-full h-3 w-3 bg-red-500"></span>
          </span>
        </div>
        <div>
          <p class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Lưu ý công nợ</p>
          <p class="text-xs font-bold text-[#2A231E]">Bạn có <span class="text-red-500">{{ suppliersWithDebt.length }} đối tác</span> đang có dư nợ cần thanh toán.</p>
        </div>
        <button @click="supplierFilter = 'debt'" class="ml-auto px-4 py-1.5 bg-red-50 border border-red-100 hover:bg-red-100 hover:border-red-200 text-red-600 rounded-lg text-xs font-bold transition-all shadow-sm">
          Lọc xem ngay
        </button>
      </div>

      <!-- Toolbar -->
      <div class="flex flex-col md:flex-row gap-3 items-center">
        <div class="relative flex-1 w-full">
          <Search class="w-5 h-5 absolute left-3.5 top-1/2 -translate-y-1/2 text-[#8A8178]" />
          <input
            v-model="supplierSearch"
            placeholder="Tìm nhà cung cấp, số điện thoại..."
            class="pl-11 w-full bg-white border border-[#EAE3D9] h-12 rounded-xl shadow-sm text-sm font-medium focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033]"
          />
        </div>

        <!-- Bộ lọc nợ -->
        <div class="relative w-full md:w-48 flex-shrink-0">
          <select v-model="supplierFilter" class="w-full bg-white border border-[#EAE3D9] h-12 rounded-xl pl-4 pr-10 shadow-sm text-sm font-bold text-[#5C544E] focus:outline-none focus:border-[#CC8033] appearance-none cursor-pointer hover:bg-[#FDFBF7] transition-colors">
            <option value="all">Tất cả đối tác</option>
            <option value="debt">Đang còn nợ</option>
            <option value="no-debt">Không có nợ</option>
          </select>
          <ChevronDown class="w-4 h-4 absolute right-4 top-1/2 -translate-y-1/2 text-[#8A8178] pointer-events-none" />
        </div>

        <button @click="openSupplierForm()" class="w-full md:w-auto flex items-center justify-center gap-2 bg-white border border-[#EAE3D9] hover:border-[#CC8033] hover:text-[#CC8033] text-[#2A231E] h-12 px-5 rounded-xl shadow-sm transition-colors text-xs font-bold uppercase tracking-wider flex-shrink-0">
          <Plus class="w-4 h-4" /> Thêm đối tác
        </button>
      </div>

      <!-- Table -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
        <div class="overflow-x-auto custom-scrollbar">
          <table class="w-full text-sm text-left">
            <thead>
              <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
                <th class="px-5 py-4 font-bold">Mã NCC</th>
                <th class="px-5 py-4 font-bold">Tên Nhà Cung Cấp</th>
                <th class="px-5 py-4 font-bold">Liên hệ</th>
                <th class="px-5 py-4 font-bold">Nhóm mặt hàng</th>
                <th class="px-5 py-4 font-bold text-right">Tổng công nợ</th>
                <th class="px-5 py-4 font-bold text-center w-44">Thao tác</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#EAE3D9]/60">
              <tr v-for="s in filteredSuppliers" :key="s.code" class="hover:bg-[#FDFBF7] transition-colors">
                <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">{{ s.code }}</td>
                <td class="px-5 py-4 font-bold text-[#2A231E]">{{ s.name }}</td>
                <td class="px-5 py-4">
                  <div class="flex items-center gap-2 text-xs font-medium text-[#5C544E]"><Phone class="w-3.5 h-3.5 text-[#8A8178]" /> {{ s.phone }}</div>
                </td>
                <td class="px-5 py-4">
                  <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">{{ s.group }}</span>
                </td>
                <td class="px-5 py-4 text-right">
                  <span class="font-bold text-base" :class="s.debt > 0 ? 'text-red-500' : 'text-[#4A7C59]'">{{ formatVND(s.debt) }}</span>
                </td>
                <td class="px-5 py-4">
                  <div class="flex items-center justify-center gap-2">
                    <button @click="openSupplierForm(s)" class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-md transition-colors" title="Sửa"><Pencil class="w-4 h-4" /></button>
                    <button
                      v-if="s.debt > 0"
                      @click="openPaymentModal(s)"
                      class="px-3 py-1.5 bg-[#FDFBF7] border border-[#EAE3D9] text-[#2A231E] hover:bg-white hover:border-[#CC8033] hover:text-[#CC8033] rounded-md text-xs font-bold transition-all shadow-sm"
                    >Thanh toán nợ</button>
                    <span v-else class="px-3 py-1.5 bg-green-50 border border-green-100 text-green-600 rounded-md text-xs font-bold">Không nợ</span>
                  </div>
                </td>
              </tr>
              <tr v-if="filteredSuppliers.length === 0">
                <td colspan="6" class="px-5 py-12 text-center text-[#8A8178] text-sm font-medium">Không tìm thấy nhà cung cấp phù hợp.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- TAB 2: Lịch sử phiếu nhập kho -->
    <!-- ===================================================================== -->
    <div v-show="activeTab === 'inbound'" class="animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h3 class="font-bold text-[#2A231E]">Lịch sử nhập kho gần đây</h3>
          <select v-model="receiptFilter" class="bg-white border border-[#EAE3D9] rounded-md px-3 py-1.5 text-xs font-medium focus:outline-none">
            <option value="all">Tất cả trạng thái</option>
            <option value="paid">Đã thanh toán</option>
            <option value="debt">Ghi nợ</option>
          </select>
        </div>
        <div class="overflow-x-auto custom-scrollbar">
          <table class="w-full text-sm text-left">
            <thead>
              <tr class="text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
                <th class="px-5 py-3 font-bold">Mã Phiếu</th>
                <th class="px-5 py-3 font-bold">Ngày nhập</th>
                <th class="px-5 py-3 font-bold">Nhà cung cấp</th>
                <th class="px-5 py-3 font-bold text-right">Tổng tiền</th>
                <th class="px-5 py-3 font-bold text-right">Còn nợ</th>
                <th class="px-5 py-3 font-bold text-center">Trạng thái</th>
                <th class="px-5 py-3 font-bold text-center">Chi tiết</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#EAE3D9]/60">
              <tr v-for="r in filteredReceipts" :key="r.id" class="hover:bg-[#FDFBF7] transition-colors">
                <td class="px-5 py-4 font-mono text-xs font-bold text-[#2A231E]">{{ r.id }}</td>
                <td class="px-5 py-4 text-xs font-medium text-[#5C544E]">{{ r.date }}</td>
                <td class="px-5 py-4 font-medium text-[#2A231E]">{{ r.supplier }}</td>
                <td class="px-5 py-4 text-right font-bold text-[#2A231E]">{{ formatVND(r.total) }}</td>
                <td class="px-5 py-4 text-right font-bold" :class="(r.total - r.paid) > 0 ? 'text-orange-500' : 'text-[#4A7C59]'">{{ formatVND(r.total - r.paid) }}</td>
                <td class="px-5 py-4 text-center">
                  <span v-if="r.total - r.paid <= 0" class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-green-50 text-green-600 border border-green-100 text-[10px] font-bold uppercase tracking-wider"><CheckCircle2 class="w-3 h-3" /> Đã thanh toán</span>
                  <span v-else class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-orange-50 text-orange-600 border border-orange-100 text-[10px] font-bold uppercase tracking-wider"><AlertTriangle class="w-3 h-3" /> Ghi nợ</span>
                </td>
                <td class="px-5 py-4 text-center">
                  <button @click="viewReceipt = r" class="p-1.5 text-[#8A8178] hover:text-[#CC8033] hover:bg-[#FDFBF7] rounded-md transition-colors"><Eye class="w-4 h-4" /></button>
                </td>
              </tr>
              <tr v-if="filteredReceipts.length === 0">
                <td colspan="7" class="px-5 py-12 text-center text-[#8A8178] text-sm font-medium">Chưa có phiếu nhập nào. Bấm “Tạo Phiếu Nhập Mới” để bắt đầu.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- MODAL: Tạo Phiếu Nhập Kho (Centered Pop-up) -->
    <!-- ===================================================================== -->
    <div v-if="isCreateReceiptOpen" class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-50 flex justify-center items-center p-4 sm:p-6 lg:p-8" @click.self="isCreateReceiptOpen = false">
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-[1280px] flex flex-col overflow-hidden animate-in zoom-in-95 duration-200 max-h-full">
        <!-- Header -->
        <div class="h-16 px-6 bg-white border-b border-[#EAE3D9] flex justify-between items-center shadow-sm flex-shrink-0">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 bg-[#2A231E] text-white rounded-lg flex items-center justify-center shadow-md">
              <Truck class="w-5 h-5" stroke-width="2" />
            </div>
            <div>
              <h2 class="text-lg font-bold text-[#2A231E] uppercase tracking-wide leading-tight">Tạo Phiếu Nhập Kho</h2>
              <div class="text-[11px] text-[#8A8178] uppercase tracking-widest font-bold mt-0.5">
                Mã Phiếu: <span class="text-[#CC8033]">{{ draftCode }}</span>
              </div>
            </div>
          </div>
          <button @click="isCreateReceiptOpen = false" class="flex items-center justify-center w-8 h-8 bg-[#FDFBF7] border border-[#EAE3D9] text-[#5C544E] hover:bg-red-50 hover:text-red-600 hover:border-red-200 rounded-lg transition-colors" title="Đóng">
            <X class="w-5 h-5" />
          </button>
        </div>

        <div class="flex flex-col lg:flex-row flex-1 overflow-hidden bg-[#FDFBF7]">
        <!-- Left Panel: Thông tin chung -->
        <div class="w-[320px] lg:w-[340px] bg-[#FDFBF7] border-r border-[#EAE3D9] flex flex-col flex-shrink-0">
          <div class="p-5 overflow-y-auto custom-scrollbar flex-1 space-y-4">
            
            <h3 class="font-bold text-[#2A231E] text-sm uppercase tracking-wider flex items-center gap-2 border-b border-[#EAE3D9] pb-3">
              <Box class="w-5 h-5 text-[#CC8033]" /> Thông Tin Chứng Từ
            </h3>

            <div class="space-y-4">
              <!-- Người lập phiếu -->
              <div class="space-y-1">
                <label class="text-[10px] font-bold uppercase tracking-wider text-[#5C544E]">Người Lập Phiếu</label>
                <div class="relative">
                  <input type="text" value="Quản trị viên (admin@brew.vn)" readonly class="w-full bg-[#EAE3D9]/40 border border-[#EAE3D9] h-9 rounded-lg pl-8 pr-3 text-xs font-medium text-[#5C544E] shadow-sm cursor-not-allowed focus:outline-none" />
                  <Users class="w-3.5 h-3.5 absolute left-3 top-1/2 -translate-y-1/2 text-[#8A8178]" />
                </div>
              </div>

              <!-- Nhà cung cấp -->
              <div class="space-y-1">
                <label class="text-[10px] font-bold uppercase tracking-wider text-[#5C544E]">Nhà Cung Cấp <span class="text-red-500">*</span></label>
                <div class="flex items-center gap-2">
                  <div class="relative flex-1">
                    <select v-model="draft.supplierCode" class="w-full bg-white border border-[#EAE3D9] h-9 rounded-lg pl-3 pr-8 text-xs font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] shadow-sm appearance-none cursor-pointer">
                      <option value="" disabled>-- Chọn nhà cung cấp --</option>
                      <option v-for="s in suppliers" :key="s.code" :value="s.code">{{ s.name }}</option>
                    </select>
                    <ChevronDown class="w-3.5 h-3.5 absolute right-3 top-1/2 -translate-y-1/2 text-[#8A8178] pointer-events-none" />
                  </div>
                  <button @click="openSupplierForm()" class="w-9 h-9 flex-shrink-0 bg-white border border-[#EAE3D9] text-[#CC8033] hover:border-[#CC8033] hover:bg-[#FFF9F2] rounded-lg flex items-center justify-center shadow-sm transition-colors" title="Thêm nhà cung cấp mới">
                    <Plus class="w-4 h-4" />
                  </button>
                </div>
              </div>

              <!-- Ngày nhập -->
              <div class="space-y-1">
                <label class="text-[10px] font-bold uppercase tracking-wider text-[#5C544E]">Ngày Nhập Hàng</label>
                <input type="datetime-local" v-model="draft.date" class="w-full bg-white border border-[#EAE3D9] h-9 rounded-lg px-3 text-xs font-medium text-[#2A231E] focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] shadow-sm" />
              </div>

              <!-- Ghi chú -->
              <div class="space-y-1">
                <label class="text-[10px] font-bold uppercase tracking-wider text-[#5C544E]">Ghi Chú</label>
                <textarea v-model="draft.note" placeholder="Nhập diễn giải cho phiếu nhập này..." rows="2" class="w-full bg-white border border-[#EAE3D9] rounded-lg p-2.5 text-xs font-medium text-[#2A231E] focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] shadow-sm resize-none"></textarea>
              </div>

              <!-- Tùy chọn Nhập Kho -->
              <div class="flex items-center justify-between bg-white border border-[#EAE3D9] p-3 rounded-lg shadow-sm cursor-pointer hover:border-[#CC8033] transition-colors" @click="draft.updateInventory = !draft.updateInventory">
                <div class="flex items-center gap-3">
                  <div class="w-5 h-5 rounded flex items-center justify-center transition-colors border" :class="draft.updateInventory ? 'bg-[#4A7C59] border-[#4A7C59]' : 'bg-gray-50 border-[#8A8178]'">
                    <CheckCircle2 v-if="draft.updateInventory" class="w-3.5 h-3.5 text-white" />
                  </div>
                  <div>
                    <label class="text-xs font-bold text-[#2A231E] cursor-pointer">Cập nhật Tồn Kho</label>
                    <p class="text-[9px] text-[#8A8178] mt-0.5">Cộng số lượng vào kho sau khi lưu</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Lưu ý -->
            <div class="bg-[#FFF9F2] border border-[#E8C5A5] rounded-lg p-3 flex gap-2 items-start mt-2">
              <Info class="w-4 h-4 text-[#CC8033] flex-shrink-0 mt-0.5" />
              <p class="text-[10px] text-[#5C544E] leading-relaxed">
                Hàng hóa sẽ được <span v-if="draft.updateInventory">cộng thẳng vào <strong>Tồn Kho</strong></span><span v-else><strong>ghi nhận vào hóa đơn mua hàng</strong>, không cập nhật kho</span>. Phần chưa thanh toán tự động cộng vào <strong>Công Nợ</strong>.
              </p>
            </div>
          </div>
        </div>

        <!-- Right Panel: Chi tiết hàng hóa & Footer -->
        <div class="flex-1 flex flex-col bg-white overflow-hidden relative">
          
          <div class="flex-1 overflow-y-auto p-6 lg:p-8 custom-scrollbar">
            <div class="flex justify-between items-center mb-4">
              <h3 class="font-bold text-[#2A231E] text-sm uppercase tracking-wider flex items-center gap-2">
                <Layers class="w-5 h-5 text-[#CC8033]" /> Chi Tiết Nhập Kho
              </h3>
              <button @click="addRow" class="flex items-center gap-2 bg-[#FDFBF7] border border-[#EAE3D9] hover:border-[#CC8033] hover:text-[#CC8033] text-[#2A231E] h-10 px-4 rounded-lg shadow-sm transition-colors text-xs font-bold uppercase tracking-wider">
                <Plus class="w-4 h-4" /> Thêm Dòng
              </button>
            </div>

            <!-- Bảng dữ liệu -->
            <div class="border border-[#EAE3D9] rounded-xl overflow-x-auto shadow-sm">
              <table class="w-full text-sm text-left">
                <thead class="bg-[#FDFBF7] text-[#5C544E] text-[10px] font-bold uppercase tracking-widest border-b border-[#EAE3D9]">
                  <tr>
                    <th class="px-3 py-3 w-10 text-center border-r border-[#EAE3D9]">#</th>
                    <th class="px-4 py-3 min-w-[200px] border-r border-[#EAE3D9]">Mã / Tên Nguyên Liệu</th>
                    <th class="px-4 py-3 min-w-[120px] w-36 border-r border-[#EAE3D9]">Đơn Vị Nhập</th>
                    <th class="px-4 py-3 min-w-[130px] w-32 border-r border-[#EAE3D9]">Hạn Sử Dụng</th>
                    <th class="px-4 py-3 min-w-[100px] w-24 text-right border-r border-[#EAE3D9]">Số Lượng</th>
                    <th class="px-4 py-3 min-w-[120px] w-32 text-right border-r border-[#EAE3D9]">Đơn Giá (₫)</th>
                    <th class="px-4 py-3 min-w-[120px] w-32 text-right border-r border-[#EAE3D9]">Thành Tiền (₫)</th>
                    <th class="px-2 py-3 w-12 text-center"></th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-[#EAE3D9]">
                  <tr v-for="(row, idx) in draft.rows" :key="idx" class="hover:bg-[#FDFBF7] transition-colors group">
                    <td class="px-3 py-2 text-center text-[#8A8178] font-medium border-r border-[#EAE3D9] bg-gray-50">{{ idx + 1 }}</td>
                    
                    <td class="px-3 py-2 border-r border-[#EAE3D9]">
                      <!-- Smart Search Input with Datalist -->
                      <div class="flex items-center gap-1">
                        <div class="flex-1 relative">
                          <input 
                            type="text" 
                            :list="'material-list-' + idx"
                            v-model="row._materialSearchStr"
                            @change="onMaterialSearchChange(row)"
                            @input="row.materialId = ''"
                            placeholder="Gõ mã, tên (VD: Sữa đặc)..."
                            class="w-full bg-transparent border-0 h-9 px-1 text-sm font-bold text-[#2A231E] focus:ring-0 focus:outline-none placeholder:font-normal placeholder:text-[#8A8178]"
                          />
                          <datalist :id="'material-list-' + idx">
                            <option v-for="m in materials" :key="m.id" :value="m.id + ' | ' + m.name"></option>
                          </datalist>
                        </div>
                        
                        <!-- Nút gọi AI khi chưa chọn được mã hợp lệ -->
                        <button 
                          v-if="row._materialSearchStr && !row.materialId" 
                          @click="analyzeWithAI(row)"
                          :disabled="row.isAiLoading"
                          class="flex-shrink-0 flex items-center gap-1 px-2.5 h-7 rounded bg-gradient-to-r from-[#2A231E] to-[#3D332A] hover:from-[#1A1614] hover:to-[#2A231E] text-yellow-400 font-bold text-[9px] uppercase tracking-wider shadow-sm transition-all animate-in zoom-in-50"
                          title="Nhờ AI tạo mã mới"
                        >
                          <Sparkles v-if="!row.isAiLoading" class="w-3 h-3" />
                          <div v-else class="w-3 h-3 border-2 border-yellow-400 border-t-transparent rounded-full animate-spin"></div>
                          <span v-if="!row.isAiLoading">AI Tạo</span>
                        </button>
                      </div>

                      <!-- Hiển thị phân loại nếu có -->
                      <div v-if="row.materialId && materialObj(row.materialId)" class="mt-1 ml-1 relative inline-block group">
                        <select 
                          v-model="materialObj(row.materialId)!.category" 
                          class="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-10"
                        >
                          <option v-for="c in systemCategories" :key="c" :value="c">{{ c }}</option>
                        </select>
                        <span class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded bg-[#FFF9F2] text-[#CC8033] border border-[#E8C5A5]/60 text-[8px] font-bold uppercase tracking-widest shadow-sm group-hover:bg-[#E8C5A5]/20 transition-colors">
                          <Package class="w-2.5 h-2.5" stroke-width="2.5" /> 
                          {{ materialObj(row.materialId)?.category || 'CHƯA PHÂN LOẠI' }}
                        </span>
                      </div>
                    </td>
                    
                    <td class="px-3 py-2 border-r border-[#EAE3D9]">
                      <select v-model="row.unit" class="w-full bg-[#FDFBF7] border border-[#EAE3D9] h-9 rounded px-2 text-xs font-medium text-[#2A231E] focus:outline-none focus:border-[#CC8033]">
                        <option v-for="u in unitsFor(row.materialId)" :key="u.name" :value="u.name">{{ u.name }}</option>
                      </select>
                      <div v-if="row.materialId && !isBaseUnit(row.materialId, row.unit)">
                        <!-- Display Mode -->
                        <div v-if="conversionFor(row.materialId, row.unit) && !row._isEditingConversion" class="text-[9px] text-[#8A8178] mt-1 pl-1 flex items-center justify-between group">
                          <span>Quy đổi: 1 {{ row.unit }} = {{ conversionFor(row.materialId, row.unit) }}</span>
                          <button @click="editConversion(row)" class="opacity-0 group-hover:opacity-100 text-[#CC8033] hover:underline transition-opacity flex items-center gap-0.5">Sửa</button>
                        </div>
                        
                        <!-- AI Prompt / Edit Mode -->
                        <div v-else class="mt-1.5 bg-[#FFFDF5] border border-yellow-200 rounded p-1.5 shadow-sm animate-in fade-in zoom-in-95">
                          <p class="text-[9px] font-bold text-yellow-600 flex items-center gap-1 mb-1">
                            <Sparkles class="w-2.5 h-2.5"/> AI: 1 {{ row.unit }} = ?
                          </p>
                          <div class="flex gap-1">
                            <input type="text" v-model="row._aiConversionInput" @keyup.enter="saveAiConversion(row)" placeholder="VD: 12 Lốc..." class="flex-1 min-w-0 bg-white border border-yellow-300 text-[10px] font-bold text-[#2A231E] px-1.5 py-0.5 rounded focus:outline-none focus:border-yellow-500" />
                            <button @click="saveAiConversion(row)" class="bg-yellow-400 text-black text-[9px] px-2 font-bold uppercase rounded shadow-sm hover:bg-yellow-500 transition-colors">Lưu</button>
                            <button v-if="conversionFor(row.materialId, row.unit)" @click="row._isEditingConversion = false" class="bg-gray-200 text-[#5C544E] text-[9px] px-2 font-bold uppercase rounded hover:bg-gray-300 transition-colors">Hủy</button>
                          </div>
                        </div>
                      </div>
                    </td>

                    <td class="px-3 py-2 border-r border-[#EAE3D9]">
                      <input type="date" v-model="row.expiryDate" class="w-full bg-white border border-[#EAE3D9] h-9 rounded px-2 text-xs font-medium text-[#2A231E] focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] transition-all" />
                    </td>
                    
                    <td class="px-3 py-2 border-r border-[#EAE3D9]">
                      <input type="number" min="0" max="999999" v-model.number="row.qty" class="w-full text-right bg-white border border-[#EAE3D9] h-9 rounded px-2 text-sm font-bold text-[#CC8033] focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] transition-all" />
                    </td>
                    
                    <td class="px-3 py-2 border-r border-[#EAE3D9] relative">
                      <input type="number" min="0" max="999999999" v-model.number="row.price" @keyup.enter="idx === draft.rows.length - 1 ? addRow() : null" class="w-full text-right bg-white border border-[#EAE3D9] h-9 rounded px-2 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] transition-all" />
                      <div v-if="row.price >= 100000" class="absolute right-3 bottom-0 translate-y-full text-[9px] text-[#4A7C59] font-bold z-10">{{ formatCompact(row.price) }}</div>
                    </td>
                    
                    <td class="px-4 py-2 text-right bg-gray-50">
                      <span class="font-bold text-[#2A231E] text-base">{{ formatCompactVND(row.qty * row.price) }}</span>
                    </td>
                    
                    <td class="px-2 py-2 text-center bg-white">
                      <button @click="removeRow(idx)" class="p-1.5 text-[#8A8178] hover:text-red-500 hover:bg-red-50 rounded transition-all" title="Xóa dòng này">
                        <Trash2 class="w-4 h-4" />
                      </button>
                    </td>
                  </tr>
                  <tr v-if="draft.rows.length === 0">
                    <td colspan="8" class="px-4 py-16 text-center bg-gray-50/50">
                      <div class="flex flex-col items-center justify-center text-[#8A8178]">
                        <Package class="w-12 h-12 mb-3 text-[#EAE3D9]" stroke-width="1.5" />
                        <p class="text-sm font-medium text-[#5C544E]">Chưa có hàng hóa nào được thêm.</p>
                        <button @click="addRow" class="mt-4 px-5 py-2 rounded-lg bg-white border border-[#EAE3D9] text-[#2A231E] text-xs font-bold uppercase shadow-sm hover:border-[#CC8033] hover:text-[#CC8033] transition-colors">
                          + Bấm vào đây để thêm
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            
            <p class="text-xs text-[#8A8178] mt-3 italic">* Mẹo: Chọn ô <strong>Đơn giá</strong> ở dòng cuối và bấm <strong>Enter</strong> để thêm nhanh dòng mới.</p>
          </div>

          <!-- Solid Dark Footer -->
          <div class="bg-[#1F1915] border-t border-[#3D332A] text-white flex-shrink-0 z-20">
            <div class="px-6 py-4 flex flex-wrap lg:flex-nowrap items-end justify-between gap-6">
              
              <div class="flex flex-wrap lg:flex-nowrap items-end gap-6 md:gap-8 flex-1">
                <!-- Tổng Tiền -->
                <div class="min-w-[100px] lg:min-w-[120px]">
                  <p class="text-[10px] text-[#8A8178] uppercase tracking-widest font-bold mb-1">Tổng Tiền Hàng</p>
                  <p class="text-2xl lg:text-3xl font-black text-white tracking-tight truncate max-w-[200px]" :title="formatVND(draftTotal)">{{ formatCompactVND(draftTotal) }}</p>
                </div>
                
                <div class="h-12 w-px bg-[#3D332A] hidden md:block flex-shrink-0"></div>
                
                <!-- Thực Trả -->
                <div class="flex-1 min-w-[250px]">
                  <div class="flex items-center justify-between mb-1.5">
                    <p class="text-[10px] text-[#4A7C59] uppercase tracking-widest font-bold flex items-center gap-1.5"><CheckCircle2 class="w-3.5 h-3.5" /> Thực Trả NCC</p>
                    <button @click="draft.paid = draftTotal" class="text-[9px] text-white font-bold uppercase tracking-wider hover:bg-[#3B6347] transition-colors bg-[#4A7C59]/80 px-2 py-0.5 rounded shadow-sm border border-[#4A7C59]">Trả Đủ</button>
                  </div>
                  <div class="flex gap-2 items-start">
                    <!-- Input tiền -->
                    <div class="relative w-32 lg:w-36">
                      <input type="number" min="0" max="99999999999" v-model.number="draft.paid" class="w-full text-right bg-[#0F0C0A] border border-[#4A7C59]/50 h-11 rounded-lg px-3 pr-8 text-lg font-bold text-[#4A7C59] focus:outline-none focus:border-[#4A7C59] transition-all shadow-inner" />
                      <span class="absolute right-3 top-2.5 text-[#4A7C59] font-bold pointer-events-none text-sm">₫</span>
                      <div v-if="draft.paid >= 100000" class="absolute -bottom-4 right-1 text-[9px] text-[#4A7C59] font-bold">{{ formatCompact(draft.paid) }}</div>
                    </div>

                    <!-- Select phương thức -->
                    <div class="w-32 lg:w-36 flex-shrink-0">
                      <select v-model="draft.paymentMethod" class="w-full bg-[#2A231E] border border-[#3D332A] h-11 rounded-lg px-2 lg:px-3 text-xs font-bold text-white focus:outline-none focus:border-[#CC8033] appearance-none cursor-pointer hover:bg-[#332A24] transition-colors">
                        <option value="ChuyenKhoan">Chuyển khoản</option>
                        <option value="TienMat">Tiền mặt</option>
                      </select>
                    </div>

                    <!-- Upload Button (Only if ChuyenKhoan) -->
                    <label v-if="draft.paymentMethod === 'ChuyenKhoan'" class="h-11 px-3 bg-[#2A231E] border border-dashed border-[#5C544E] hover:border-[#CC8033] hover:text-[#CC8033] rounded-lg flex items-center justify-center gap-1.5 cursor-pointer transition-all group flex-shrink-0" :class="draft.billImageName ? 'border-[#4A7C59] text-[#4A7C59]' : 'text-[#8A8178]'" :title="draft.billImageName || 'Tải lên ảnh bill chuyển khoản'">
                      <CheckCircle2 v-if="draft.billImageName" class="w-4 h-4 text-[#4A7C59]" />
                      <UploadCloud v-else class="w-4 h-4 text-[#8A8178] group-hover:text-[#CC8033]" />
                      <span class="text-[10px] font-bold uppercase tracking-wider whitespace-nowrap" :class="draft.billImageName ? 'text-[#4A7C59]' : 'group-hover:text-[#CC8033]'">
                        {{ draft.billImageName ? 'Đã Tải Bill' : 'Tải Bill' }}
                      </span>
                      <input type="file" class="hidden" accept="image/*" @change="handleFileUpload" />
                    </label>
                  </div>
                </div>

                <div class="h-12 w-px bg-[#3D332A] hidden lg:block flex-shrink-0"></div>

                <!-- Nợ Phát Sinh -->
                <div class="hidden lg:block min-w-[100px]">
                  <p class="text-[10px] uppercase tracking-widest font-bold mb-1 flex items-center gap-1.5" :class="draftDebt > 0 ? 'text-orange-400' : 'text-[#8A8178]'">
                    <AlertTriangle v-if="draftDebt > 0" class="w-3.5 h-3.5" /> Nợ Phát Sinh
                  </p>
                  <p class="text-xl lg:text-2xl font-bold truncate max-w-[150px]" :title="formatVND(draftDebt)" :class="draftDebt > 0 ? 'text-orange-400' : 'text-[#8A8178]'">{{ formatCompactVND(draftDebt) }}</p>
                </div>
              </div>

              <!-- Button -->
              <button @click="saveReceipt" class="h-14 px-4 lg:px-6 rounded-xl bg-gradient-to-r from-[#4A7C59] to-[#3B6347] hover:from-[#548D65] hover:to-[#4A7C59] text-white text-sm font-black uppercase tracking-widest shadow-[0_0_20px_rgba(74,124,89,0.3)] transition-all flex items-center gap-2 flex-shrink-0 transform active:scale-95 border border-[#548D65]/50">
                <CheckCircle2 class="w-5 h-5" /> Hoàn Tất
              </button>

            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

    <!-- ===================================================================== -->
    <!-- MODAL: Xem chi tiết phiếu nhập -->
    <!-- ===================================================================== -->
    <div v-if="viewReceipt" class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-50 flex justify-center items-center p-4" @click.self="viewReceipt = null">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-lg overflow-hidden animate-in zoom-in-95 duration-200 flex flex-col max-h-[90vh]">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <div>
            <h2 class="text-lg font-bold text-[#2A231E]">Phiếu nhập {{ viewReceipt.id }}</h2>
            <p class="text-xs text-[#8A8178] mt-0.5">{{ viewReceipt.supplier }} • {{ viewReceipt.date }}</p>
          </div>
          <button @click="viewReceipt = null" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md"><X class="w-5 h-5" /></button>
        </div>
        <div class="p-5 overflow-y-auto custom-scrollbar space-y-3">
          <div v-for="(it, i) in viewReceipt.rows" :key="i" class="flex justify-between items-center bg-[#FDFBF7] border border-[#EAE3D9] rounded-lg px-3 py-2.5">
            <div>
              <p class="font-bold text-sm text-[#2A231E]">{{ materialName(it.materialId) }}</p>
              <p class="text-[11px] text-[#8A8178]">{{ it.qty }} {{ it.unit }} × {{ formatNumber(it.price) }}₫</p>
            </div>
            <span class="font-bold text-[#2A231E]">{{ formatNumber(it.qty * it.price) }}₫</span>
          </div>
          <div v-if="viewReceipt.note" class="text-xs text-[#5C544E] italic bg-[#FFF9F2] border border-[#E8C5A5] rounded-lg p-3">Ghi chú: {{ viewReceipt.note }}</div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 space-y-1.5 text-sm">
          <div class="flex justify-between text-[#5C544E]"><span>Tổng tiền hàng</span><span class="font-bold text-[#2A231E]">{{ formatVND(viewReceipt.total) }}</span></div>
          <div class="flex justify-between text-[#5C544E]"><span>Đã trả</span><span class="font-bold text-[#4A7C59]">{{ formatVND(viewReceipt.paid) }}</span></div>
          <div class="flex justify-between text-[#5C544E]"><span>Còn nợ</span><span class="font-bold" :class="(viewReceipt.total - viewReceipt.paid) > 0 ? 'text-orange-500' : 'text-[#4A7C59]'">{{ formatVND(viewReceipt.total - viewReceipt.paid) }}</span></div>
        </div>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- MODAL: Thêm / Sửa nhà cung cấp -->
    <!-- ===================================================================== -->
    <div v-if="isSupplierFormOpen" class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-50 flex justify-center items-center p-4" @click.self="isSupplierFormOpen = false">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-md overflow-hidden animate-in zoom-in-95 duration-200">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h2 class="text-lg font-bold text-[#2A231E]">{{ supplierForm.code ? 'Cập nhật đối tác' : 'Thêm đối tác mới' }}</h2>
          <button @click="isSupplierFormOpen = false" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md"><X class="w-5 h-5" /></button>
        </div>
        <div class="p-5 space-y-4">
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Tên nhà cung cấp <span class="text-red-500">*</span></label>
            <input v-model="supplierForm.name" placeholder="VD: Đại lý Cà phê Quận 1" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
          </div>
          <div class="grid grid-cols-2 gap-3">
            <div class="space-y-1.5">
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Số điện thoại</label>
              <input v-model="supplierForm.phone" placeholder="09xx xxx xxx" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
            </div>
            <div class="space-y-1.5">
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Nhóm hàng</label>
              <input v-model="supplierForm.group" placeholder="VD: Cà phê hạt" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
            </div>
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="isSupplierFormOpen = false" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Hủy</button>
          <button @click="saveSupplier" class="px-5 py-2 rounded-lg bg-[#CC8033] text-white text-xs font-bold uppercase shadow-md hover:bg-[#B87029] transition-colors">Lưu</button>
        </div>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- MODAL: Thanh toán nợ -->
    <!-- ===================================================================== -->
    <div v-if="payingSupplier" class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-50 flex justify-center items-center p-4" @click.self="payingSupplier = null">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-md overflow-hidden animate-in zoom-in-95 duration-200">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h2 class="text-lg font-bold text-[#2A231E]">Lập Phiếu Chi Thanh Toán</h2>
          <button @click="payingSupplier = null" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md"><X class="w-5 h-5" /></button>
        </div>
        <div class="p-5 space-y-4">
          <div>
            <p class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1">Nhà cung cấp</p>
            <p class="font-bold text-[#2A231E] text-base">{{ payingSupplier.name }}</p>
          </div>
          <div class="bg-red-50 border border-red-100 rounded-lg p-3">
            <p class="text-[10px] font-bold uppercase tracking-wider text-red-400 mb-1">Dư nợ hiện tại</p>
            <p class="font-bold text-red-500 text-xl">{{ formatVND(payingSupplier.debt) }}</p>
          </div>
          <div class="space-y-1.5 pt-2">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Số tiền thanh toán <span class="text-red-500">*</span></label>
            <input type="number" min="0" v-model.number="paymentAmount" placeholder="Nhập số tiền..." class="w-full text-right bg-white border border-[#CC8033] h-11 rounded-lg px-3 text-lg font-bold focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 shadow-inner" />
            <button @click="paymentAmount = payingSupplier.debt" class="text-[11px] font-bold text-[#CC8033] hover:underline">Trả hết dư nợ</button>
          </div>
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Phương thức</label>
            <select v-model="paymentMethod" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
              <option>Chuyển khoản</option>
              <option>Tiền mặt</option>
            </select>
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="payingSupplier = null" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Đóng</button>
          <button @click="confirmPayment" class="px-5 py-2 rounded-lg bg-[#4A7C59] text-white text-xs font-bold uppercase shadow-md hover:bg-[#3B6347] transition-colors">Xác nhận chi</button>
        </div>
      </div>
    </div>
    <!-- Toast -->
    <Transition name="toast">
      <div v-if="toastMsg" class="fixed bottom-6 right-6 z-[60] bg-[#2A231E] text-white px-5 py-3 rounded-xl shadow-2xl flex items-center gap-3 border border-[#CC8033]/30">
        <CheckCircle2 v-if="!toastMsg.includes('❌')" class="w-5 h-5 text-[#4A7C59]" />
        <AlertTriangle v-else class="w-5 h-5 text-red-500" />
        <span class="text-sm font-medium">{{ toastMsg.replace('❌', '').trim() }}</span>
      </div>
    </Transition>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Plus, Search, Phone, TrendingDown, CheckCircle2, AlertTriangle, Eye, X, Trash2,
  Box, Layers, Users, ClipboardList, ChevronRight, Truck, Package, ClipboardCheck,
  Pencil, Info, Sparkles
} from 'lucide-vue-next'

// ── Types ───────────────────────────────────────────────
interface Supplier { code: string; name: string; phone: string; group: string; debt: number }
interface UnitConversion { name: string; conversion?: string }
interface Material { id: string; name: string; category?: string; units: UnitConversion[] }
interface ReceiptRow { materialId: string; unit: string; qty: number; price: number; expiryDate: string; _materialSearchStr?: string; isAiLoading?: boolean; _aiConversionInput?: string; _isEditingConversion?: boolean }
interface Receipt { id: string; date: string; supplierCode: string; supplier: string; rows: ReceiptRow[]; total: number; paid: number; note: string; paymentMethod: string }

// ── Master data (mock) ──────────────────────────────────
const materials = ref<Material[]>([
  { id: 'RAW-CF-001', name: 'Hạt cà phê Robusta', category: 'Nguyên liệu thô', units: [{name: 'Bao', conversion: '50 Kg'}, {name: 'Kg', conversion: '1000g'}, {name: 'g'}] },
  { id: 'SEM-TC-012', name: 'Trân châu đen nấu sẵn', category: 'Bán thành phẩm / Topping', units: [{name: 'Khay', conversion: '10 Kg'}, {name: 'Kg', conversion: '1000g'}, {name: 'g'}] },
  { id: 'RAW-MK-005', name: 'Sữa đặc Ngôi sao Phương Nam', category: 'Nguyên liệu thô', units: [{name: 'Thùng', conversion: '24 Lon'}, {name: 'Lon', conversion: '380g'}] },
  { id: 'RAW-MK-002', name: 'Sữa tươi thanh trùng 1L', category: 'Nguyên liệu thô', units: [{name: 'Thùng', conversion: '12 Lốc'}, {name: 'Lốc', conversion: '4 Hộp'}, {name: 'Hộp', conversion: '1000ml'}] },
  { id: 'SUP-CUP-01', name: 'Ly giấy Takeaway 450ml', category: 'Vật tư', units: [{name: 'Thùng', conversion: '1000 Chiếc'}, {name: 'Cây', conversion: '50 Chiếc'}, {name: 'Chiếc'}] },
])

const suppliers = ref<Supplier[]>([
  { code: 'SUP-001', name: 'Đại lý Sữa Vinamilk Quận 1', phone: '0901 234 567', group: 'Sữa & Chế phẩm', debt: 12500000 },
  { code: 'SUP-002', name: 'NPP Cafe Trung Nguyên', phone: '0988 111 222', group: 'Cà phê hạt', debt: 8000000 },
  { code: 'SUP-003', name: 'Bao Bì Xanh Sài Gòn', phone: '0912 345 678', group: 'Ly, ống hút, vật tư', debt: 0 },
])

const receipts = ref<Receipt[]>([
  { id: 'INB-2406-003', date: '03/06/2026 08:30', supplierCode: 'SUP-001', supplier: 'Đại lý Sữa Vinamilk Quận 1', rows: [{ materialId: 'RAW-MK-002', unit: 'Thùng', qty: 100, price: 45000, expiryDate: '' }], total: 4500000, paid: 4500000, note: '', paymentMethod: 'ChuyenKhoan' },
  { id: 'INB-2406-002', date: '02/06/2026 14:15', supplierCode: 'SUP-002', supplier: 'NPP Cafe Trung Nguyên', rows: [{ materialId: 'RAW-CF-001', unit: 'Bao', qty: 16, price: 500000, expiryDate: '' }], total: 8000000, paid: 0, note: 'Hàng quý 2', paymentMethod: 'ChuyenKhoan' },
])

// ── Tab + filters ───────────────────────────────────────
const activeTab = ref<'suppliers' | 'inbound'>('suppliers')
const supplierSearch = ref('')
const supplierFilter = ref<'all' | 'debt' | 'no-debt'>('all')
const receiptFilter = ref<'all' | 'paid' | 'debt'>('all')

const totalDebt = computed(() => suppliers.value.reduce((s, x) => s + x.debt, 0))
const suppliersWithDebt = computed(() => suppliers.value.filter(s => s.debt > 0))

const filteredSuppliers = computed(() => {
  let result = suppliers.value;
  
  if (supplierFilter.value === 'debt') result = result.filter(s => s.debt > 0);
  if (supplierFilter.value === 'no-debt') result = result.filter(s => s.debt <= 0);
  
  const q = supplierSearch.value.toLowerCase().trim()
  if (q) {
    result = result.filter(s => s.name.toLowerCase().includes(q) || s.phone.replace(/\s/g, '').includes(q.replace(/\s/g, '')))
  }
  
  return result;
})

const filteredReceipts = computed(() => {
  if (receiptFilter.value === 'all') return receipts.value
  if (receiptFilter.value === 'paid') return receipts.value.filter(r => r.total - r.paid <= 0)
  return receipts.value.filter(r => r.total - r.paid > 0)
})

// ── Helpers ─────────────────────────────────────────────
const formatNumber = (n: number) => (n || 0).toLocaleString('vi-VN')
const formatCompact = (n: number) => {
  if (!n) return '0'
  if (n >= 1e9) return (n / 1e9).toLocaleString('vi-VN', { maximumFractionDigits: 2 }) + ' tỷ'
  if (n >= 1e6) return (n / 1e6).toLocaleString('vi-VN', { maximumFractionDigits: 2 }) + ' triệu'
  if (n >= 1e5) return (n / 1e3).toLocaleString('vi-VN', { maximumFractionDigits: 2 }) + ' ngàn'
  return formatNumber(n)
}
const formatVND = (n: number) => formatNumber(n) + ' ₫'
const formatCompactVND = (n: number) => {
  if (!n) return '0 ₫'
  if (n >= 1e9) return (n / 1e9).toLocaleString('vi-VN', { maximumFractionDigits: 2 }) + ' tỷ'
  if (n >= 1e6) return (n / 1e6).toLocaleString('vi-VN', { maximumFractionDigits: 2 }) + ' triệu'
  if (n >= 1e5) return (n / 1e3).toLocaleString('vi-VN', { maximumFractionDigits: 2 }) + ' ngàn'
  return formatNumber(n) + ' ₫'
}
const materialName = (id: string) => materials.value.find(m => m.id === id)?.name ?? '—'
const materialObj = (id: string) => materials.value.find(m => m.id === id) || null
const categoryFor = (id: string) => materials.value.find(m => m.id === id)?.category || null
const systemCategories = ref<string[]>(JSON.parse(localStorage.getItem('materialCategories') || '["Nguyên liệu thô", "Bán thành phẩm / Topping", "Vật tư"]'))
const unitsFor = (id: string) => materials.value.find(m => m.id === id)?.units ?? [{name: 'Đơn vị'}]
const conversionFor = (matId: string, unitName: string) => {
  const m = materials.value.find(x => x.id === matId)
  if (!m) return null
  return m.units.find(u => u.name === unitName)?.conversion || null
}

const isBaseUnit = (matId: string, unitName: string) => {
  const m = materials.value.find(x => x.id === matId)
  if (!m || m.units.length === 0) return true;
  return m.units[m.units.length - 1]?.name === unitName;
}

const saveAiConversion = (row: ReceiptRow) => {
  if (!row._aiConversionInput) return;
  const m = materials.value.find(x => x.id === row.materialId);
  if (m) {
    const u = m.units.find(x => x.name === row.unit);
    if (u) {
      u.conversion = row._aiConversionInput;
    } else {
      m.units.push({ name: row.unit, conversion: row._aiConversionInput });
    }
    row._isEditingConversion = false;
    const savedVal = row._aiConversionInput;
    row._aiConversionInput = '';
    toast(`✨ Đã lưu quy đổi: 1 ${row.unit} = ${savedVal}`);
  }
}

const editConversion = (row: ReceiptRow) => {
  row._aiConversionInput = conversionFor(row.materialId, row.unit) || '';
  row._isEditingConversion = true;
}

const onMaterialSearchChange = (row: ReceiptRow) => {
  if (!row._materialSearchStr) {
    row.materialId = '';
    return;
  }
  const str = row._materialSearchStr.trim();
  const parts = str.split(' | ');
  const codeCandidate = parts[0] ? parts[0].trim() : str;
  
  const mat = materials.value.find(m => m.id === codeCandidate || m.name.toLowerCase() === str.toLowerCase());
  
  if (mat) {
    row.materialId = mat.id;
    row._materialSearchStr = `${mat.id} | ${mat.name}`;
    if (!row.unit) row.unit = mat.units[0]?.name || '';
  } else {
    row.materialId = '';
  }
}

// ── AI Magic (Mock Gemini AI Integration) ────────────────
let aiCounter = 1;
const analyzeWithAI = (row: ReceiptRow) => {
  if (!row._materialSearchStr) return;
  row.isAiLoading = true;
  
  // Simulate AI network delay (1.5s)
  setTimeout(() => {
    const rawName = row._materialSearchStr!.trim();
    const lowerName = rawName.toLowerCase();
    
    // AI Knowledge base (simulated)
    let suggestedUnits: UnitConversion[] = [{ name: 'Cái', conversion: '' }];
    let suggestedCategory = 'Nguyên liệu thô';
    
    if (lowerName.includes('sữa') || lowerName.includes('milk')) {
      suggestedUnits = [
        { name: 'Thùng', conversion: '' },
        { name: 'Lốc', conversion: '' },
        { name: 'Hộp', conversion: '' },
        { name: 'ml', conversion: '' }
      ];
    } else if (lowerName.includes('cà phê') || lowerName.includes('cafe')) {
      suggestedUnits = [
        { name: 'Bao', conversion: '' },
        { name: 'Kg', conversion: '1000g' },
        { name: 'g', conversion: '' }
      ];
    } else if (lowerName.includes('siro') || lowerName.includes('syrup')) {
      suggestedUnits = [
        { name: 'Thùng', conversion: '' },
        { name: 'Chai', conversion: '750ml' },
        { name: 'ml', conversion: '' }
      ];
    } else if (lowerName.includes('trà') || lowerName.includes('tea')) {
      suggestedUnits = [
        { name: 'Thùng', conversion: '20 Túi' },
        { name: 'Túi', conversion: '500g' },
        { name: 'g', conversion: '' }
      ];
    } else if (lowerName.includes('đường') || lowerName.includes('sugar') || lowerName.includes('trân châu') || lowerName.includes('thạch') || lowerName.includes('topping')) {
      suggestedUnits = [
        { name: 'Bao', conversion: '50 Kg' },
        { name: 'Kg', conversion: '1000g' },
        { name: 'g', conversion: '' }
      ];
      suggestedCategory = 'Bán thành phẩm / Topping';
    } else if (lowerName.includes('mứt') || lowerName.includes('ngâm') || lowerName.includes('puree') || lowerName.includes('đào') || lowerName.includes('vải') || lowerName.includes('nhãn')) {
      suggestedUnits = [
        { name: 'Thùng', conversion: '24 Lon' },
        { name: 'Lon', conversion: '500g' },
        { name: 'Hộp', conversion: '500g' },
        { name: 'g', conversion: '' }
      ];
      suggestedCategory = 'Nguyên liệu thô';
    } else if (lowerName.includes('ly') || lowerName.includes('cốc') || lowerName.includes('ống hút') || lowerName.includes('muỗng') || lowerName.includes('bao bì') || lowerName.includes('túi')) {
      suggestedUnits = [
        { name: 'Thùng', conversion: '1000 Cái' },
        { name: 'Bịch', conversion: '50 Cái' },
        { name: 'Cái', conversion: '' }
      ];
      suggestedCategory = 'Vật tư';
    } else {
      suggestedUnits = [
        { name: 'Thùng', conversion: '10 Hộp' },
        { name: 'Hộp', conversion: '100 Cái' },
        { name: 'Cái', conversion: '' }
      ];
    }

    const newMat: Material = {
      id: `RAW-AI-${String(aiCounter++).padStart(3, '0')}`,
      name: rawName,
      category: suggestedCategory,
      units: suggestedUnits
    };

    materials.value.push(newMat);
    row.materialId = newMat.id;
    row._materialSearchStr = `${newMat.id} | ${newMat.name}`;
    row.unit = suggestedUnits[0]?.name || '';
    row.isAiLoading = false;
    
    toast(`✨ AI đã tạo "${newMat.name}" - Thuộc nhóm: ${newMat.category}`);
  }, 1500);
}

// ── Toast ───────────────────────────────────────────────
const toastMsg = ref('')
let toastTimer: ReturnType<typeof setTimeout>
const toast = (msg: string) => {
  toastMsg.value = msg
  clearTimeout(toastTimer)
  toastTimer = setTimeout(() => (toastMsg.value = ''), 3000)
}

// ── Create receipt (reactive) ───────────────────────────
const isCreateReceiptOpen = ref(false)
let receiptCounter = 4
const draftCode = ref('')
const blankRow = (): ReceiptRow => ({ materialId: '', unit: '', qty: 1, price: 0, expiryDate: '', _materialSearchStr: '' })
const draft = ref<{ supplierCode: string; date: string; note: string; rows: ReceiptRow[]; paid: number; paymentMethod: string; billImageName?: string; updateInventory: boolean }>({
  supplierCode: '', date: '', note: '', rows: [], paid: 0, paymentMethod: 'ChuyenKhoan', billImageName: '', updateInventory: true
})

const draftTotal = computed(() => draft.value.rows.reduce((s, r) => s + (r.qty || 0) * (r.price || 0), 0))
const draftDebt = computed(() => Math.max(0, draftTotal.value - (draft.value.paid || 0)))

const handleFileUpload = (e: Event) => {
  const target = e.target as HTMLInputElement;
  const file = target.files?.[0];
  if (file) {
    draft.value.billImageName = file.name;
    toast(`Đã đính kèm ảnh: ${file.name}`);
  }
}

const openCreateReceipt = () => {
  draftCode.value = `INB-2406-${String(receiptCounter).padStart(3, '0')}`
  const now = new Date()
  const local = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16)
  draft.value = { supplierCode: '', date: local, note: '', rows: [blankRow()], paid: 0, paymentMethod: 'ChuyenKhoan', billImageName: '', updateInventory: true }
  isCreateReceiptOpen.value = true
}
const addRow = () => draft.value.rows.push(blankRow())
const removeRow = (idx: number) => draft.value.rows.splice(idx, 1)

const saveReceipt = () => {
  // Validate Supplier
  if (!draft.value.supplierCode) { 
    toast('❌ Vui lòng chọn nhà cung cấp!'); 
    return; 
  }
  
  // Validate Date
  if (!draft.value.date) {
    toast('❌ Vui lòng chọn ngày nhập hàng!');
    return;
  }

  // Validate Rows
  const validRows = draft.value.rows.filter(r => r.materialId);
  if (validRows.length === 0) { 
    toast('❌ Phiếu nhập phải có ít nhất một nguyên liệu!'); 
    return; 
  }

  for (let i = 0; i < validRows.length; i++) {
    const row = validRows[i];
    if (!row.qty || row.qty <= 0) {
      toast(`❌ Dòng ${i+1}: Số lượng phải lớn hơn 0!`);
      return;
    }
    if (row.price === null || row.price === undefined || row.price < 0) {
      toast(`❌ Dòng ${i+1}: Đơn giá không hợp lệ!`);
      return;
    }
    if (!row.unit) {
      toast(`❌ Dòng ${i+1}: Vui lòng chọn đơn vị nhập!`);
      return;
    }
    if (row.expiryDate) {
      const expiryTime = new Date(row.expiryDate).getTime();
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (expiryTime < today.getTime()) {
        toast(`❌ Dòng ${i+1}: Hạn sử dụng không được là ngày trong quá khứ!`);
        return;
      }
    }
    if (row._isEditingConversion) {
      toast(`❌ Dòng ${i+1}: Vui lòng lưu quy đổi đơn vị trước khi hoàn tất!`);
      return;
    }
  }

  // Validate Payment
  if (draft.value.paid < 0) {
    toast('❌ Số tiền thực trả không hợp lệ!');
    return;
  }

  if (draft.value.paymentMethod === 'ChuyenKhoan' && draft.value.paid > 0 && !draft.value.billImageName) {
    toast('❌ Vui lòng tải lên ảnh bill chuyển khoản!');
    return;
  }

  const sup = suppliers.value.find(s => s.code === draft.value.supplierCode)!
  const total = draftTotal.value
  const debt = draftDebt.value
  const d = new Date(draft.value.date || Date.now())
  const dateStr = d.toLocaleDateString('vi-VN') + ' ' + d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })

  receipts.value.unshift({
    id: draftCode.value, date: dateStr, supplierCode: sup.code, supplier: sup.name,
    rows: validRows.map(r => ({ ...r })), total, paid: draft.value.paid || 0, note: draft.value.note, paymentMethod: draft.value.paymentMethod
  })
  if (debt > 0) sup.debt += debt
  receiptCounter++
  isCreateReceiptOpen.value = false
  activeTab.value = 'inbound'
  toast(`Đã lưu phiếu ${draftCode.value} • +${formatVND(total)} vào kho`)
}

// ── View receipt ────────────────────────────────────────
const viewReceipt = ref<Receipt | null>(null)

// ── Supplier form ───────────────────────────────────────
const isSupplierFormOpen = ref(false)
const supplierForm = ref<Supplier>({ code: '', name: '', phone: '', group: '', debt: 0 })
let supplierCounter = 4
const openSupplierForm = (s?: Supplier) => {
  supplierForm.value = s ? { ...s } : { code: '', name: '', phone: '', group: '', debt: 0 }
  isSupplierFormOpen.value = true
}
const saveSupplier = () => {
  if (!supplierForm.value.name.trim()) { toast('Vui lòng nhập tên nhà cung cấp'); return }
  if (supplierForm.value.code) {
    const i = suppliers.value.findIndex(s => s.code === supplierForm.value.code)
    if (i !== -1) suppliers.value[i] = { ...supplierForm.value }
    toast('Đã cập nhật đối tác')
  } else {
    suppliers.value.push({ ...supplierForm.value, code: `SUP-${String(supplierCounter++).padStart(3, '0')}` })
    toast('Đã thêm đối tác mới')
  }
  isSupplierFormOpen.value = false
}

// ── Debt payment ────────────────────────────────────────
const payingSupplier = ref<Supplier | null>(null)
const paymentAmount = ref<number>(0)
const paymentMethod = ref('Chuyển khoản')
const openPaymentModal = (s: Supplier) => {
  payingSupplier.value = s
  paymentAmount.value = 0
  paymentMethod.value = 'Chuyển khoản'
}
const confirmPayment = () => {
  if (!payingSupplier.value) return
  const amt = paymentAmount.value || 0
  if (amt <= 0) { toast('Nhập số tiền hợp lệ'); return }
  payingSupplier.value.debt = Math.max(0, payingSupplier.value.debt - amt)
  toast(`Đã chi ${formatVND(amt)} (${paymentMethod.value})`)
  payingSupplier.value = null
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
