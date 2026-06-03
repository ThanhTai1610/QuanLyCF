<template>
  <div class="space-y-6 md:space-y-8 font-premium-sans text-[#2A231E] p-4 sm:p-6 lg:p-8 max-w-[1400px] mx-auto min-h-screen">
    
    <!-- Tiêu đề và Tabs -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div class="flex bg-white rounded-lg p-1 border border-[#EAE3D9] shadow-sm">
        <button 
          @click="activeTab = 'suppliers'"
          :class="[
            'px-6 py-2.5 rounded-md text-sm font-bold transition-all duration-300',
            activeTab === 'suppliers' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]'
          ]"
        >
          [ TAB 1: DANH SÁCH ĐỐI TÁC ]
        </button>
        <button 
          @click="activeTab = 'inbound'"
          :class="[
            'px-6 py-2.5 rounded-md text-sm font-bold transition-all duration-300',
            activeTab === 'inbound' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]'
          ]"
        >
          [ TAB 2: LỊCH SỬ NHẬP KHO ]
        </button>
      </div>
      
      <button 
        v-if="activeTab === 'inbound'" 
        @click="isCreateReceiptOpen = true"
        class="flex items-center justify-center bg-[#2A231E] hover:bg-[#3D332A] text-white h-11 px-6 rounded-lg shadow-md transition-colors text-xs font-bold uppercase tracking-wider whitespace-nowrap"
      >
        <Plus class="w-4 h-4 mr-2" /> Tạo Phiếu Nhập Mới
      </button>
    </div>

    <!-- TAB 1: Danh bạ & Công nợ Nhà cung cấp -->
    <div v-show="activeTab === 'suppliers'" class="animate-in fade-in slide-in-from-bottom-4 duration-500">
      
      <!-- Summary / Toolbar -->
      <div class="flex flex-col md:flex-row gap-4 mb-6">
        <div class="bg-white border border-[#EAE3D9] p-4 rounded-xl shadow-sm flex-1 flex items-center justify-between">
          <div>
            <p class="text-xs font-bold text-[#8A8178] uppercase tracking-wider mb-1">Tổng nợ phải trả</p>
            <h3 class="text-2xl font-sans font-bold text-red-500">24,500,000 ₫</h3>
          </div>
          <div class="w-12 h-12 rounded-full bg-red-50 flex items-center justify-center text-red-500">
            <TrendingDown class="w-6 h-6" />
          </div>
        </div>
        <div class="relative flex-1 md:max-w-md flex items-center">
          <Search class="w-5 h-5 absolute left-3.5 text-[#8A8178]" />
          <input
            placeholder="Tìm nhà cung cấp, số điện thoại..."
            class="pl-11 w-full bg-white border border-[#EAE3D9] h-14 rounded-xl shadow-sm text-sm font-medium focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033]"
          />
        </div>
      </div>

      <!-- Table Suppliers -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
        <div class="overflow-x-auto custom-scrollbar">
          <table class="w-full text-sm text-left">
            <thead>
              <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
                <th class="px-5 py-4 font-bold whitespace-nowrap">Mã NCC</th>
                <th class="px-5 py-4 font-bold whitespace-nowrap">Tên Nhà Cung Cấp</th>
                <th class="px-5 py-4 font-bold whitespace-nowrap">Liên hệ</th>
                <th class="px-5 py-4 font-bold whitespace-nowrap">Nhóm mặt hàng</th>
                <th class="px-5 py-4 font-bold whitespace-nowrap text-right">Tổng công nợ</th>
                <th class="px-5 py-4 font-bold whitespace-nowrap text-center">Thao tác</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#EAE3D9]/60">
              <tr class="hover:bg-[#FDFBF7] transition-colors group">
                <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">SUP-001</td>
                <td class="px-5 py-4 font-bold text-[#2A231E]">Đại lý Sữa Vinamilk Quận 1</td>
                <td class="px-5 py-4">
                  <div class="flex items-center gap-2 text-xs font-medium text-[#5C544E]">
                    <Phone class="w-3.5 h-3.5 text-[#8A8178]" /> 0901 234 567
                  </div>
                </td>
                <td class="px-5 py-4">
                  <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Sữa & Chế phẩm</span>
                </td>
                <td class="px-5 py-4 text-right">
                  <span class="font-bold text-red-500 text-base">12,500,000 ₫</span>
                </td>
                <td class="px-5 py-4 text-center">
                  <button @click="openPaymentModal" class="px-3 py-1.5 bg-[#FDFBF7] border border-[#EAE3D9] text-[#2A231E] hover:bg-white hover:border-[#CC8033] hover:text-[#CC8033] rounded-md text-xs font-bold transition-all shadow-sm">
                    Thanh toán nợ
                  </button>
                </td>
              </tr>
              <tr class="hover:bg-[#FDFBF7] transition-colors group">
                <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">SUP-002</td>
                <td class="px-5 py-4 font-bold text-[#2A231E]">NPP Cafe Trung Nguyên</td>
                <td class="px-5 py-4">
                  <div class="flex items-center gap-2 text-xs font-medium text-[#5C544E]">
                    <Phone class="w-3.5 h-3.5 text-[#8A8178]" /> 0988 111 222
                  </div>
                </td>
                <td class="px-5 py-4">
                  <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Cà phê hạt</span>
                </td>
                <td class="px-5 py-4 text-right">
                  <span class="font-bold text-red-500 text-base">8,000,000 ₫</span>
                </td>
                <td class="px-5 py-4 text-center">
                  <button @click="openPaymentModal" class="px-3 py-1.5 bg-[#FDFBF7] border border-[#EAE3D9] text-[#2A231E] hover:bg-white hover:border-[#CC8033] hover:text-[#CC8033] rounded-md text-xs font-bold transition-all shadow-sm">
                    Thanh toán nợ
                  </button>
                </td>
              </tr>
              <tr class="hover:bg-[#FDFBF7] transition-colors group">
                <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">SUP-003</td>
                <td class="px-5 py-4 font-bold text-[#2A231E]">Bao Bì Xanh Sài Gòn</td>
                <td class="px-5 py-4">
                  <div class="flex items-center gap-2 text-xs font-medium text-[#5C544E]">
                    <Phone class="w-3.5 h-3.5 text-[#8A8178]" /> 0912 345 678
                  </div>
                </td>
                <td class="px-5 py-4">
                  <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Ly, ống hút, vật tư</span>
                </td>
                <td class="px-5 py-4 text-right">
                  <span class="font-bold text-[#4A7C59] text-base">0 ₫</span>
                </td>
                <td class="px-5 py-4 text-center">
                  <button class="px-3 py-1.5 bg-gray-50 border border-gray-200 text-gray-400 rounded-md text-xs font-bold cursor-not-allowed">
                    Không có nợ
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- TAB 2: Quản lý Phiếu nhập kho -->
    <div v-show="activeTab === 'inbound'" class="animate-in fade-in slide-in-from-bottom-4 duration-500">
      
      <!-- Lịch sử phiếu nhập -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h3 class="font-bold text-[#2A231E]">Lịch sử nhập kho gần đây</h3>
          <div class="flex gap-2">
            <select class="bg-white border border-[#EAE3D9] rounded-md px-3 py-1.5 text-xs font-medium focus:outline-none">
              <option>Tất cả trạng thái</option>
              <option>Đã thanh toán</option>
              <option>Ghi nợ</option>
            </select>
          </div>
        </div>
        <div class="overflow-x-auto custom-scrollbar">
          <table class="w-full text-sm text-left">
            <thead>
              <tr class="text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
                <th class="px-5 py-3 font-bold whitespace-nowrap">Mã Phiếu</th>
                <th class="px-5 py-3 font-bold whitespace-nowrap">Ngày nhập</th>
                <th class="px-5 py-3 font-bold whitespace-nowrap">Nhà cung cấp</th>
                <th class="px-5 py-3 font-bold whitespace-nowrap">Tổng tiền</th>
                <th class="px-5 py-3 font-bold whitespace-nowrap">Trạng thái</th>
                <th class="px-5 py-3 font-bold whitespace-nowrap text-center">Chi tiết</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#EAE3D9]/60">
              <tr class="hover:bg-[#FDFBF7] transition-colors">
                <td class="px-5 py-4 font-mono text-xs font-bold text-[#2A231E]">INB-2406-003</td>
                <td class="px-5 py-4 text-xs font-medium text-[#5C544E]">03/06/2026 08:30</td>
                <td class="px-5 py-4 font-medium text-[#2A231E]">Đại lý Sữa Vinamilk Quận 1</td>
                <td class="px-5 py-4 font-bold text-[#2A231E]">4,500,000 ₫</td>
                <td class="px-5 py-4">
                  <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-green-50 text-green-600 border border-green-100 text-[10px] font-bold uppercase tracking-wider">
                    <CheckCircle2 class="w-3 h-3" /> Đã thanh toán
                  </span>
                </td>
                <td class="px-5 py-4 text-center">
                  <button class="p-1.5 text-[#8A8178] hover:text-[#CC8033] hover:bg-[#FDFBF7] rounded-md transition-colors">
                    <Eye class="w-4 h-4" />
                  </button>
                </td>
              </tr>
              <tr class="hover:bg-[#FDFBF7] transition-colors">
                <td class="px-5 py-4 font-mono text-xs font-bold text-[#2A231E]">INB-2406-002</td>
                <td class="px-5 py-4 text-xs font-medium text-[#5C544E]">02/06/2026 14:15</td>
                <td class="px-5 py-4 font-medium text-[#2A231E]">NPP Cafe Trung Nguyên</td>
                <td class="px-5 py-4 font-bold text-[#2A231E]">8,000,000 ₫</td>
                <td class="px-5 py-4">
                  <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-orange-50 text-orange-600 border border-orange-100 text-[10px] font-bold uppercase tracking-wider">
                    <AlertTriangle class="w-3 h-3" /> Ghi nợ
                  </span>
                </td>
                <td class="px-5 py-4 text-center">
                  <button class="p-1.5 text-[#8A8178] hover:text-[#CC8033] hover:bg-[#FDFBF7] rounded-md transition-colors">
                    <Eye class="w-4 h-4" />
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- FULL-SCREEN MODAL: Tạo Phiếu Nhập Kho -->
    <div 
      class="fixed inset-0 bg-white z-50 transition-opacity duration-300 flex flex-col animate-in fade-in zoom-in-95"
      v-if="isCreateReceiptOpen"
    >
      <!-- Header Fixed -->
      <div class="px-8 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center shadow-sm flex-shrink-0">
        <div>
          <h2 class="text-2xl font-bold text-[#2A231E] font-display uppercase">Tạo Phiếu Nhập Kho Mới</h2>
          <p class="text-sm text-[#8A8178] mt-1 font-mono">Mã phiếu dự kiến: <span class="text-[#CC8033] font-bold">INB-2406-004</span></p>
        </div>
        <button @click="isCreateReceiptOpen = false" class="p-2 text-[#8A8178] hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors flex items-center gap-2 font-bold text-sm">
          <X class="w-5 h-5" /> Đóng
        </button>
      </div>

      <!-- Layout Left (30%) & Right (70%) -->
      <div class="flex flex-1 overflow-hidden">
        
        <!-- Khu vực Left-side (Thông tin chung - chiếm 30%) -->
        <div class="w-[30%] min-w-[320px] bg-gray-50 border-r border-[#EAE3D9] p-6 overflow-y-auto space-y-5 custom-scrollbar">
          <h3 class="font-bold text-[#2A231E] text-sm uppercase tracking-wider mb-2 flex items-center gap-2">
            <Box class="w-4 h-4 text-[#CC8033]" /> Thông tin chung
          </h3>
          
          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Nhà cung cấp <span class="text-red-500">*</span></label>
            <div class="relative">
              <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-[#8A8178]" />
              <input type="text" placeholder="Tìm tên nhà cung cấp..." class="pl-9 w-full bg-white border border-[#EAE3D9] h-10 rounded-lg text-sm font-medium focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033] focus:outline-none" />
            </div>
            <!-- Giả lập kết quả search dropdown -->
            <div class="mt-2 text-xs text-[#2A231E] font-medium bg-[#FDFBF7] p-2 rounded border border-[#EAE3D9]">
              Đã chọn: <span class="font-bold text-[#CC8033]">Đại lý Sữa Vinamilk Quận 1</span>
            </div>
          </div>

          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Ngày nhập</label>
            <input type="datetime-local" value="2026-06-03T10:00" class="w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium text-[#5C544E] focus:outline-none" />
          </div>

          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Ghi chú phiếu nhập</label>
            <textarea placeholder="Nhập ghi chú cho thủ kho hoặc kế toán..." rows="3" class="w-full bg-[#FDFBF7] border border-[#EAE3D9] rounded-lg p-3 text-sm font-medium text-[#5C544E] focus:outline-none focus:border-[#CC8033] resize-none"></textarea>
          </div>
        </div>

        <!-- Khu vực Right-side (Lưới thêm nguyên liệu - chiếm 70%) -->
        <div class="w-[70%] flex flex-col bg-white overflow-hidden relative">
          
          <div class="flex-1 overflow-y-auto p-6 custom-scrollbar pb-32">
            <h3 class="font-bold text-[#2A231E] text-sm uppercase tracking-wider mb-4 flex items-center gap-2">
              <Layers class="w-4 h-4 text-[#CC8033]" /> Chi tiết hàng hóa
            </h3>
            
            <div class="border border-[#EAE3D9] rounded-xl overflow-hidden shadow-sm">
              <table class="w-full text-sm">
                <thead class="bg-[#FDFBF7] border-b border-[#EAE3D9] text-[#8A8178] text-[10px] uppercase tracking-wider">
                  <tr>
                    <th class="px-4 py-3 text-left">Nguyên liệu</th>
                    <th class="px-4 py-3 text-left w-32">Đơn vị nhập</th>
                    <th class="px-4 py-3 text-right w-32">Số lượng</th>
                    <th class="px-4 py-3 text-right w-40">Đơn giá (₫)</th>
                    <th class="px-4 py-3 text-right w-40">Thành tiền (₫)</th>
                    <th class="px-2 py-3 text-center w-12"></th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-[#EAE3D9]/50">
                  <!-- Dòng 1 -->
                  <tr class="hover:bg-gray-50 transition-colors">
                    <td class="p-3">
                      <select class="w-full bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033]">
                        <option selected>Sữa tươi thanh trùng 1L</option>
                      </select>
                    </td>
                    <td class="p-3">
                      <select class="w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-md px-2 text-sm font-medium focus:outline-none">
                        <option>Thùng</option>
                        <option selected>Lốc</option>
                      </select>
                    </td>
                    <td class="p-3">
                      <input type="number" value="10" class="w-full text-right bg-[#FFF9F2] border border-[#E8C5A5] h-10 rounded-md px-3 text-base font-bold text-[#CC8033] focus:outline-none" />
                    </td>
                    <td class="p-3">
                      <input type="text" value="35,000" class="w-full text-right bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
                    </td>
                    <td class="p-3 text-right font-bold text-[#2A231E] text-base">
                      350,000
                    </td>
                    <td class="p-3 text-center">
                      <button class="p-2 text-red-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors"><Trash2 class="w-5 h-5" /></button>
                    </td>
                  </tr>
                  
                  <!-- Dòng 2 -->
                  <tr class="hover:bg-gray-50 transition-colors">
                    <td class="p-3">
                      <select class="w-full bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033]">
                        <option selected>Sữa đặc Ngôi sao</option>
                      </select>
                    </td>
                    <td class="p-3">
                      <select class="w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-md px-2 text-sm font-medium focus:outline-none">
                        <option selected>Thùng</option>
                        <option>Lon</option>
                      </select>
                    </td>
                    <td class="p-3">
                      <input type="number" value="2" class="w-full text-right bg-[#FFF9F2] border border-[#E8C5A5] h-10 rounded-md px-3 text-base font-bold text-[#CC8033] focus:outline-none" />
                    </td>
                    <td class="p-3">
                      <input type="text" value="850,000" class="w-full text-right bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
                    </td>
                    <td class="p-3 text-right font-bold text-[#2A231E] text-base">
                      1,700,000
                    </td>
                    <td class="p-3 text-center">
                      <button class="p-2 text-red-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors"><Trash2 class="w-5 h-5" /></button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Nút Thêm Dynamic Row -->
            <button class="mt-4 w-full border-2 border-dashed border-[#CC8033]/40 bg-[#FFF9F2] text-[#CC8033] hover:bg-[#CC8033] hover:text-white hover:border-[#CC8033] py-3 rounded-xl flex items-center justify-center gap-2 font-bold text-sm transition-colors shadow-sm">
              <Plus class="w-5 h-5" /> Thêm nguyên liệu mới
            </button>
          </div>

          <!-- Khối chốt tiền (Sticky Bottom Bar) -->
          <div class="absolute bottom-0 left-0 right-0 bg-[#1E2128] text-white p-5 border-t-4 border-[#CC8033] flex justify-between items-center z-10 shadow-2xl">
            <div class="flex items-center gap-8">
              <div>
                <p class="text-xs font-medium text-gray-400 uppercase tracking-wider mb-1">Tổng tiền hàng</p>
                <p class="text-2xl font-bold text-white">2,050,000 ₫</p>
              </div>
              <div class="h-10 w-px bg-gray-600"></div>
              <div>
                <p class="text-xs font-medium text-gray-400 uppercase tracking-wider mb-1">Thực trả cho NCC</p>
                <div class="flex items-center gap-2">
                  <input type="text" value="1,000,000" class="w-36 text-right bg-gray-800 border border-gray-600 h-9 rounded-md px-2 text-lg font-bold text-[#4A7C59] focus:outline-none focus:border-[#4A7C59]" />
                  <span class="text-sm">₫</span>
                </div>
              </div>
              <div class="h-10 w-px bg-gray-600"></div>
              <div>
                <p class="text-xs font-bold text-orange-400 uppercase tracking-wider mb-1 flex items-center gap-1">
                  <AlertTriangle class="w-3 h-3" /> Nợ phát sinh
                </p>
                <p class="text-xl font-bold text-orange-400">1,050,000 ₫</p>
              </div>
            </div>

            <button class="px-10 py-3 rounded-lg bg-[#CC8033] hover:bg-[#B87029] text-white text-sm font-bold uppercase tracking-wider transition-colors shadow-lg shadow-[#CC8033]/30">
              Lưu Phiếu & Hoàn Tất
            </button>
          </div>

        </div>
      </div>
    </div>

    <!-- MODAL: Thanh toán nợ -->
    <div 
      class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-50 transition-opacity duration-300 flex justify-center items-center p-4"
      v-if="isPaymentModalOpen"
      @click.self="isPaymentModalOpen = false"
    >
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-md overflow-hidden animate-in zoom-in-95 duration-200">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h2 class="text-lg font-bold text-[#2A231E]">Lập Phiếu Chi Thanh Toán</h2>
          <button @click="isPaymentModalOpen = false" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md transition-colors">
            <X class="w-5 h-5" />
          </button>
        </div>
        <div class="p-5 space-y-4">
          <div>
            <p class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1">Nhà cung cấp</p>
            <p class="font-bold text-[#2A231E] text-base">Đại lý Sữa Vinamilk Quận 1</p>
          </div>
          <div class="bg-red-50 border border-red-100 rounded-lg p-3">
            <p class="text-[10px] font-bold uppercase tracking-wider text-red-400 mb-1">Dư nợ hiện tại</p>
            <p class="font-bold text-red-500 text-xl">12,500,000 ₫</p>
          </div>
          <div class="space-y-1.5 pt-2">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Số tiền thanh toán <span class="text-red-500">*</span></label>
            <input type="text" placeholder="Nhập số tiền..." class="w-full text-right bg-white border border-[#CC8033] h-11 rounded-lg px-3 text-lg font-bold focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 shadow-inner" />
          </div>
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Phương thức</label>
            <select class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
              <option>Chuyển khoản</option>
              <option>Tiền mặt</option>
            </select>
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="isPaymentModalOpen = false" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Đóng</button>
          <button class="px-5 py-2 rounded-lg bg-[#4A7C59] text-white text-xs font-bold uppercase shadow-md hover:bg-[#3B6347] transition-colors">Xác nhận chi</button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Plus, 
  Search, 
  Phone, 
  TrendingDown, 
  CheckCircle2, 
  AlertTriangle, 
  Eye, 
  X, 
  Trash2,
  Box,
  Layers
} from 'lucide-vue-next'

const activeTab = ref<'suppliers' | 'inbound'>('suppliers')
const isCreateReceiptOpen = ref(false)
const isPaymentModalOpen = ref(false)

const openPaymentModal = () => {
  isPaymentModalOpen.value = true
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&display=swap');

.font-premium-sans {
  font-family: 'Inter', sans-serif;
}

.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: #EAE3D9;
  border-radius: 10px;
}
.custom-scrollbar:hover::-webkit-scrollbar-thumb {
  background-color: #D5C9B3;
}
</style>
