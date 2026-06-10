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
        <button @click="openSupplierForm()" class="w-full md:w-auto flex items-center justify-center gap-2 bg-white border border-[#EAE3D9] hover:border-[#CC8033] hover:text-[#CC8033] text-[#2A231E] h-12 px-5 rounded-xl shadow-sm transition-colors text-xs font-bold uppercase tracking-wider">
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
    <!-- FULL-SCREEN MODAL: Tạo Phiếu Nhập Kho (reactive) -->
    <!-- ===================================================================== -->
    <div v-if="isCreateReceiptOpen" class="fixed inset-0 bg-white z-50 flex flex-col animate-in fade-in zoom-in-95">
      <!-- Header -->
      <div class="px-8 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center shadow-sm flex-shrink-0">
        <div>
          <h2 class="text-2xl font-bold text-[#2A231E] uppercase">Tạo Phiếu Nhập Kho Mới</h2>
          <p class="text-sm text-[#8A8178] mt-1 font-mono">Mã phiếu dự kiến: <span class="text-[#CC8033] font-bold">{{ draftCode }}</span></p>
        </div>
        <button @click="isCreateReceiptOpen = false" class="p-2 text-[#8A8178] hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-2 font-bold text-sm">
          <X class="w-5 h-5" /> Đóng
        </button>
      </div>

      <div class="flex flex-1 overflow-hidden">
        <!-- Left 30% -->
        <div class="w-[30%] min-w-[320px] bg-gray-50 border-r border-[#EAE3D9] p-6 overflow-y-auto space-y-5 custom-scrollbar">
          <h3 class="font-bold text-[#2A231E] text-sm uppercase tracking-wider flex items-center gap-2"><Box class="w-4 h-4 text-[#CC8033]" /> Thông tin chung</h3>

          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Nhà cung cấp <span class="text-red-500">*</span></label>
            <select v-model="draft.supplierCode" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-bold text-[#2A231E] focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033] focus:outline-none">
              <option value="" disabled>Chọn nhà cung cấp...</option>
              <option v-for="s in suppliers" :key="s.code" :value="s.code">{{ s.name }}</option>
            </select>
          </div>

          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Ngày nhập</label>
            <input type="datetime-local" v-model="draft.date" class="w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium text-[#5C544E] focus:outline-none" />
          </div>

          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Ghi chú phiếu nhập</label>
            <textarea v-model="draft.note" placeholder="Nhập ghi chú cho thủ kho hoặc kế toán..." rows="3" class="w-full bg-[#FDFBF7] border border-[#EAE3D9] rounded-lg p-3 text-sm font-medium text-[#5C544E] focus:outline-none focus:border-[#CC8033] resize-none"></textarea>
          </div>

          <div class="bg-[#FFF9F2] border border-[#E8C5A5] p-4 rounded-xl text-xs text-[#5C544E] leading-relaxed">
            <p class="font-bold text-[#CC8033] uppercase tracking-wider mb-1 flex items-center gap-1.5"><Info class="w-3.5 h-3.5" /> Lưu ý</p>
            Khi lưu, hàng sẽ được cộng vào kho (tạo lô mới) và phần chưa trả sẽ ghi nợ cho nhà cung cấp.
          </div>
        </div>

        <!-- Right 70% -->
        <div class="w-[70%] flex flex-col bg-white overflow-hidden relative">
          <div class="flex-1 overflow-y-auto p-6 custom-scrollbar pb-32">
            <h3 class="font-bold text-[#2A231E] text-sm uppercase tracking-wider mb-4 flex items-center gap-2"><Layers class="w-4 h-4 text-[#CC8033]" /> Chi tiết hàng hóa</h3>

            <div class="border border-[#EAE3D9] rounded-xl overflow-hidden shadow-sm">
              <table class="w-full text-sm">
                <thead class="bg-[#FDFBF7] border-b border-[#EAE3D9] text-[#8A8178] text-[10px] uppercase tracking-wider">
                  <tr>
                    <th class="px-4 py-3 text-left">Nguyên liệu</th>
                    <th class="px-4 py-3 text-left w-32">Đơn vị nhập</th>
                    <th class="px-4 py-3 text-right w-28">Số lượng</th>
                    <th class="px-4 py-3 text-right w-40">Đơn giá (₫)</th>
                    <th class="px-4 py-3 text-right w-40">Thành tiền (₫)</th>
                    <th class="px-2 py-3 text-center w-12"></th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-[#EAE3D9]/50">
                  <tr v-for="(row, idx) in draft.rows" :key="idx" class="hover:bg-gray-50 transition-colors">
                    <td class="p-3">
                      <select v-model="row.materialId" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033]">
                        <option value="" disabled>Chọn nguyên liệu...</option>
                        <option v-for="m in materials" :key="m.id" :value="m.id">{{ m.name }}</option>
                      </select>
                    </td>
                    <td class="p-3">
                      <select v-model="row.unit" class="w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-md px-2 text-sm font-medium focus:outline-none">
                        <option v-for="u in unitsFor(row.materialId)" :key="u" :value="u">{{ u }}</option>
                      </select>
                    </td>
                    <td class="p-3">
                      <input type="number" min="0" v-model.number="row.qty" class="w-full text-right bg-[#FFF9F2] border border-[#E8C5A5] h-10 rounded-md px-3 text-base font-bold text-[#CC8033] focus:outline-none" />
                    </td>
                    <td class="p-3">
                      <input type="number" min="0" v-model.number="row.price" class="w-full text-right bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
                    </td>
                    <td class="p-3 text-right font-bold text-[#2A231E] text-base">{{ formatNumber(row.qty * row.price) }}</td>
                    <td class="p-3 text-center">
                      <button @click="removeRow(idx)" class="p-2 text-red-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors"><Trash2 class="w-5 h-5" /></button>
                    </td>
                  </tr>
                  <tr v-if="draft.rows.length === 0">
                    <td colspan="6" class="px-4 py-8 text-center text-[#8A8178] text-sm">Chưa có dòng hàng nào.</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <button @click="addRow" class="mt-4 w-full border-2 border-dashed border-[#CC8033]/40 bg-[#FFF9F2] text-[#CC8033] hover:bg-[#CC8033] hover:text-white hover:border-[#CC8033] py-3 rounded-xl flex items-center justify-center gap-2 font-bold text-sm transition-colors shadow-sm">
              <Plus class="w-5 h-5" /> Thêm nguyên liệu mới
            </button>
          </div>

          <!-- Sticky footer -->
          <div class="absolute bottom-0 left-0 right-0 bg-[#1E2128] text-white p-5 border-t-4 border-[#CC8033] flex flex-wrap gap-4 justify-between items-center z-10 shadow-2xl">
            <div class="flex items-center gap-8 flex-wrap">
              <div>
                <p class="text-xs font-medium text-gray-400 uppercase tracking-wider mb-1">Tổng tiền hàng</p>
                <p class="text-2xl font-bold text-white">{{ formatVND(draftTotal) }}</p>
              </div>
              <div class="h-10 w-px bg-gray-600"></div>
              <div>
                <p class="text-xs font-medium text-gray-400 uppercase tracking-wider mb-1">Thực trả cho NCC</p>
                <div class="flex items-center gap-2">
                  <input type="number" min="0" v-model.number="draft.paid" class="w-36 text-right bg-gray-800 border border-gray-600 h-9 rounded-md px-2 text-lg font-bold text-[#4A7C59] focus:outline-none focus:border-[#4A7C59]" />
                  <span class="text-sm">₫</span>
                </div>
              </div>
              <div class="h-10 w-px bg-gray-600"></div>
              <div>
                <p class="text-xs font-bold uppercase tracking-wider mb-1 flex items-center gap-1" :class="draftDebt > 0 ? 'text-orange-400' : 'text-green-400'">
                  <AlertTriangle v-if="draftDebt > 0" class="w-3 h-3" /><CheckCircle2 v-else class="w-3 h-3" /> Nợ phát sinh
                </p>
                <p class="text-xl font-bold" :class="draftDebt > 0 ? 'text-orange-400' : 'text-green-400'">{{ formatVND(draftDebt) }}</p>
              </div>
            </div>

            <button @click="saveReceipt" class="px-10 py-3 rounded-lg bg-[#CC8033] hover:bg-[#B87029] text-white text-sm font-bold uppercase tracking-wider transition-colors shadow-lg shadow-[#CC8033]/30">
              Lưu Phiếu & Hoàn Tất
            </button>
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
        <CheckCircle2 class="w-5 h-5 text-[#4A7C59]" />
        <span class="text-sm font-medium">{{ toastMsg }}</span>
      </div>
    </Transition>

  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Plus, Search, Phone, TrendingDown, CheckCircle2, AlertTriangle, Eye, X, Trash2,
  Box, Layers, Users, ClipboardList, ChevronRight, Truck, Package, ClipboardCheck,
  Pencil, Info
} from 'lucide-vue-next'

// ── Types ───────────────────────────────────────────────
interface Supplier { code: string; name: string; phone: string; group: string; debt: number }
interface Material { id: string; name: string; units: string[] }
interface ReceiptRow { materialId: string; unit: string; qty: number; price: number }
interface Receipt { id: string; date: string; supplierCode: string; supplier: string; rows: ReceiptRow[]; total: number; paid: number; note: string }

// ── Master data (mock) ──────────────────────────────────
const materials: Material[] = [
  { id: 'RAW-CF-001', name: 'Hạt cà phê Robusta', units: ['Bao', 'Kg', 'g'] },
  { id: 'SEM-TC-012', name: 'Trân châu đen nấu sẵn', units: ['Khay', 'Kg', 'g'] },
  { id: 'RAW-MK-005', name: 'Sữa đặc Ngôi sao Phương Nam', units: ['Thùng', 'Lon'] },
  { id: 'RAW-MK-002', name: 'Sữa tươi thanh trùng 1L', units: ['Thùng', 'Lốc', 'Hộp'] },
  { id: 'SUP-CUP-01', name: 'Ly giấy Takeaway 450ml', units: ['Thùng', 'Cây', 'Chiếc'] },
]

const suppliers = ref<Supplier[]>([
  { code: 'SUP-001', name: 'Đại lý Sữa Vinamilk Quận 1', phone: '0901 234 567', group: 'Sữa & Chế phẩm', debt: 12500000 },
  { code: 'SUP-002', name: 'NPP Cafe Trung Nguyên', phone: '0988 111 222', group: 'Cà phê hạt', debt: 8000000 },
  { code: 'SUP-003', name: 'Bao Bì Xanh Sài Gòn', phone: '0912 345 678', group: 'Ly, ống hút, vật tư', debt: 0 },
])

const receipts = ref<Receipt[]>([
  { id: 'INB-2406-003', date: '03/06/2026 08:30', supplierCode: 'SUP-001', supplier: 'Đại lý Sữa Vinamilk Quận 1', rows: [{ materialId: 'RAW-MK-002', unit: 'Thùng', qty: 100, price: 45000 }], total: 4500000, paid: 4500000, note: '' },
  { id: 'INB-2406-002', date: '02/06/2026 14:15', supplierCode: 'SUP-002', supplier: 'NPP Cafe Trung Nguyên', rows: [{ materialId: 'RAW-CF-001', unit: 'Bao', qty: 16, price: 500000 }], total: 8000000, paid: 0, note: 'Hàng quý 2' },
])

// ── Tab + filters ───────────────────────────────────────
const activeTab = ref<'suppliers' | 'inbound'>('suppliers')
const supplierSearch = ref('')
const receiptFilter = ref<'all' | 'paid' | 'debt'>('all')

const totalDebt = computed(() => suppliers.value.reduce((s, x) => s + x.debt, 0))

const filteredSuppliers = computed(() => {
  const q = supplierSearch.value.toLowerCase().trim()
  if (!q) return suppliers.value
  return suppliers.value.filter(s => s.name.toLowerCase().includes(q) || s.phone.replace(/\s/g, '').includes(q.replace(/\s/g, '')))
})

const filteredReceipts = computed(() => {
  if (receiptFilter.value === 'all') return receipts.value
  if (receiptFilter.value === 'paid') return receipts.value.filter(r => r.total - r.paid <= 0)
  return receipts.value.filter(r => r.total - r.paid > 0)
})

// ── Helpers ─────────────────────────────────────────────
const formatNumber = (n: number) => (n || 0).toLocaleString('vi-VN')
const formatVND = (n: number) => formatNumber(n) + ' ₫'
const materialName = (id: string) => materials.find(m => m.id === id)?.name ?? '—'
const unitsFor = (id: string) => materials.find(m => m.id === id)?.units ?? ['Đơn vị']

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
const blankRow = (): ReceiptRow => ({ materialId: '', unit: '', qty: 1, price: 0 })
const draft = ref<{ supplierCode: string; date: string; note: string; rows: ReceiptRow[]; paid: number }>({
  supplierCode: '', date: '', note: '', rows: [], paid: 0,
})

const draftTotal = computed(() => draft.value.rows.reduce((s, r) => s + (r.qty || 0) * (r.price || 0), 0))
const draftDebt = computed(() => Math.max(0, draftTotal.value - (draft.value.paid || 0)))

const openCreateReceipt = () => {
  draftCode.value = `INB-2406-${String(receiptCounter).padStart(3, '0')}`
  const now = new Date()
  const local = new Date(now.getTime() - now.getTimezoneOffset() * 60000).toISOString().slice(0, 16)
  draft.value = { supplierCode: '', date: local, note: '', rows: [blankRow()], paid: 0 }
  isCreateReceiptOpen.value = true
}
const addRow = () => draft.value.rows.push(blankRow())
const removeRow = (idx: number) => draft.value.rows.splice(idx, 1)

const saveReceipt = () => {
  if (!draft.value.supplierCode) { toast('Vui lòng chọn nhà cung cấp'); return }
  const validRows = draft.value.rows.filter(r => r.materialId && r.qty > 0)
  if (validRows.length === 0) { toast('Phiếu chưa có dòng hàng hợp lệ'); return }

  const sup = suppliers.value.find(s => s.code === draft.value.supplierCode)!
  const total = draftTotal.value
  const debt = draftDebt.value
  const d = new Date(draft.value.date || Date.now())
  const dateStr = d.toLocaleDateString('vi-VN') + ' ' + d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })

  receipts.value.unshift({
    id: draftCode.value, date: dateStr, supplierCode: sup.code, supplier: sup.name,
    rows: validRows.map(r => ({ ...r })), total, paid: draft.value.paid || 0, note: draft.value.note,
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
