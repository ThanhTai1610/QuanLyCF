<template>
  <div class="space-y-6 md:space-y-8 font-premium-sans text-[#2A231E] p-4 sm:p-6 lg:p-8 max-w-[1400px] mx-auto min-h-screen relative overflow-hidden">
    
    <!-- Tiêu đề trang (tùy chọn vì layout có thể đã có, nhưng thêm cho rõ ràng context nếu cần, ở đây Layout đã có Header nên ta vào thẳng Dashboard) -->
    
    <!-- Thanh chỉ số nhanh (Dashboard Metrics Widgets) -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-5">
      <!-- Thẻ 1: Tổng số mã nguyên liệu -->
      <div class="bg-white rounded-xl border border-[#EAE3D9] p-5 shadow-sm flex items-center justify-between hover:shadow-md transition-shadow">
        <div>
          <p class="text-xs font-bold text-[#8A8178] uppercase tracking-wider mb-1">Tổng SKU Hiện Có</p>
          <h3 class="text-3xl font-display font-bold text-[#2A231E]">142</h3>
          <p class="text-[11px] text-[#5C544E] mt-2">+5 SKU mới trong tháng này</p>
        </div>
        <div class="w-12 h-12 rounded-full bg-[#FDFBF7] border border-[#EAE3D9] flex items-center justify-center text-[#2A231E]">
          <Package class="w-6 h-6" stroke-width="1.5" />
        </div>
      </div>

      <!-- Thẻ 2: Cảnh báo sắp hết hàng -->
      <div class="bg-white rounded-xl border border-[#E8C5A5] p-5 shadow-sm flex items-center justify-between relative overflow-hidden">
        <div class="absolute inset-y-0 left-0 w-1 bg-[#CC8033]"></div>
        <div class="pl-2">
          <p class="text-xs font-bold text-[#CC8033] uppercase tracking-wider mb-1">Sắp Hết Hàng</p>
          <h3 class="text-3xl font-display font-bold text-[#2A231E]">12</h3>
          <p class="text-[11px] text-[#5C544E] mt-2">Nguyên liệu chạm ngưỡng tối thiểu</p>
        </div>
        <div class="w-12 h-12 rounded-full bg-[#FFF9F2] border border-[#E8C5A5] flex items-center justify-center text-[#CC8033]">
          <AlertTriangle class="w-6 h-6" stroke-width="1.5" />
        </div>
      </div>

      <!-- Thẻ 3: Cảnh báo hạn sử dụng -->
      <div class="bg-white rounded-xl border border-red-200 p-5 shadow-sm flex items-center justify-between relative overflow-hidden">
        <div class="absolute inset-y-0 left-0 w-1 bg-red-500"></div>
        <div class="pl-2">
          <p class="text-xs font-bold text-red-500 uppercase tracking-wider mb-1">Sắp Hết Hạn</p>
          <h3 class="text-3xl font-display font-bold text-[#2A231E]">3</h3>
          <p class="text-[11px] text-[#5C544E] mt-2">Lô hàng hết hạn trong 7 ngày tới</p>
        </div>
        <div class="w-12 h-12 rounded-full bg-red-50 border border-red-100 flex items-center justify-center text-red-500">
          <Clock class="w-6 h-6" stroke-width="1.5" />
        </div>
      </div>
    </div>

    <!-- Thanh công cụ bộ lọc (Filter & Search Bar) -->
    <div class="flex flex-col md:flex-row items-center justify-between gap-4 bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm">
      <div class="flex flex-col sm:flex-row gap-4 w-full md:w-auto flex-1">
        <!-- Ô tìm kiếm thông minh -->
        <div class="relative w-full sm:max-w-xs md:max-w-md">
          <Search class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-[#8A8178]" stroke-width="2" />
          <input
            placeholder="Tìm theo Tên hoặc Mã SKU..."
            class="pl-10 w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg text-sm font-medium focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033] transition-all"
          />
        </div>
        
        <!-- Dropdown lọc theo Loại nguyên liệu -->
        <select class="bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg px-4 text-sm font-medium text-[#2A231E] focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 w-full sm:w-48 cursor-pointer appearance-none bg-[url('data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%2216%22%20height%3D%2216%22%20viewBox%3D%220%200%2024%24%22%20fill%3D%22none%22%20stroke%3D%22%238A8178%22%20stroke-width%3D%222%22%20stroke-linecap%3D%22round%22%20stroke-linejoin%3D%22round%22%3E%3Cpolyline%20points%3D%226%209%2012%2015%2018%209%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E')] bg-no-repeat bg-[position:right_10px_center] pr-10">
          <option value="all">Tất cả loại nguyên liệu</option>
          <option value="raw">Nguyên liệu thô (Raw)</option>
          <option value="semi">Bán thành phẩm / Topping</option>
        </select>

        <!-- Dropdown lọc theo trạng thái tồn kho -->
        <select class="bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg px-4 text-sm font-medium text-[#2A231E] focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 w-full sm:w-48 cursor-pointer appearance-none bg-[url('data:image/svg+xml;charset=US-ASCII,%3Csvg%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20width%3D%2216%22%20height%3D%2216%22%20viewBox%3D%220%200%2024%24%22%20fill%3D%22none%22%20stroke%3D%22%238A8178%22%20stroke-width%3D%222%22%20stroke-linecap%3D%22round%22%20stroke-linejoin%3D%22round%22%3E%3Cpolyline%20points%3D%226%209%2012%2015%2018%209%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E')] bg-no-repeat bg-[position:right_10px_center] pr-10">
          <option value="all">Tất cả trạng thái</option>
          <option value="in_stock">Còn hàng</option>
          <option value="low_stock">Sắp hết hàng</option>
          <option value="out_of_stock">Đã rỗng</option>
        </select>
      </div>

      <div class="flex w-full md:w-auto gap-3">
        <button class="flex-1 md:flex-none flex items-center justify-center bg-[#CC8033] hover:bg-[#B87029] text-white h-10 px-5 rounded-lg shadow-md transition-colors text-xs font-bold uppercase tracking-wider">
          <Plus class="w-4 h-4 mr-2" stroke-width="2.5" /> Thêm SKU Mới
        </button>
      </div>
    </div>

    <!-- Bảng dữ liệu cốt lõi (Main Inventory Table) -->
    <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
      <div class="overflow-x-auto custom-scrollbar">
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
              <th class="px-5 py-4 font-bold whitespace-nowrap">Mã SKU</th>
              <th class="px-5 py-4 font-bold whitespace-nowrap">Tên nguyên liệu</th>
              <th class="px-5 py-4 font-bold whitespace-nowrap">Phân loại</th>
              <th class="px-5 py-4 font-bold whitespace-nowrap">Tồn kho thực tế</th>
              <th class="px-5 py-4 font-bold whitespace-nowrap text-center">Ngưỡng (Min)</th>
              <th class="px-5 py-4 font-bold whitespace-nowrap text-center">Đơn vị gốc</th>
              <th class="px-5 py-4 font-bold whitespace-nowrap text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-[#EAE3D9]/60">
            <!-- Dòng 1: Hạt cà phê (Raw) - Cảnh báo sắp hết -->
            <tr class="hover:bg-[#FDFBF7] transition-colors cursor-pointer group" @click="openDrawer">
              <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">RAW-CF-001</td>
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg border border-[#EAE3D9] bg-white flex items-center justify-center shadow-sm flex-shrink-0 group-hover:border-[#CC8033]/50 transition-colors">
                    <Coffee class="w-4 h-4 text-[#8A8178] group-hover:text-[#CC8033]" stroke-width="1.5" />
                  </div>
                  <span class="font-bold text-[#2A231E] whitespace-nowrap">Hạt cà phê Robusta</span>
                </div>
              </td>
              <td class="px-5 py-4">
                <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Nguyên liệu thô</span>
              </td>
              <td class="px-5 py-4">
                <!-- Quy đổi thông minh -->
                <div class="flex flex-col">
                  <span class="font-bold text-[#CC8033] text-sm whitespace-nowrap">2 Bao & 450g</span>
                  <span class="text-[10px] text-[#8A8178] font-medium mt-0.5">Tổng: 10,450g</span>
                </div>
              </td>
              <td class="px-5 py-4 text-center">
                <span class="text-xs font-bold text-[#8A8178] px-2.5 py-1 rounded-md bg-[#FDFBF7] border border-[#EAE3D9]">15,000g</span>
              </td>
              <td class="px-5 py-4 text-center font-medium text-xs text-[#5C544E] uppercase">g</td>
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-2">
                  <button class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors" title="Xem lịch sử thẻ kho" @click.stop="isHistoryOpen = true">
                    <History class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button class="p-2 text-[#CC8033] hover:text-white hover:bg-[#CC8033] border border-[#CC8033]/30 rounded-lg transition-colors shadow-sm" title="Điều chỉnh kho nhanh" @click.stop="isAdjustOpen = true">
                    <SlidersHorizontal class="w-4 h-4" stroke-width="2" />
                  </button>
                </div>
              </td>
            </tr>

            <!-- Dòng 2: Trân châu (Semi-finished) - OK -->
            <tr class="hover:bg-[#FDFBF7] transition-colors cursor-pointer group" @click="openDrawer">
              <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">SEM-TC-012</td>
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg border border-[#EAE3D9] bg-white flex items-center justify-center shadow-sm flex-shrink-0 group-hover:border-[#CC8033]/50 transition-colors">
                    <CupSoda class="w-4 h-4 text-[#8A8178] group-hover:text-[#CC8033]" stroke-width="1.5" />
                  </div>
                  <span class="font-bold text-[#2A231E] whitespace-nowrap">Trân châu đen nấu sẵn</span>
                </div>
              </td>
              <td class="px-5 py-4">
                <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Topping</span>
              </td>
              <td class="px-5 py-4">
                <div class="flex flex-col">
                  <span class="font-bold text-[#4A7C59] text-sm whitespace-nowrap">4 Khay & 200g</span>
                  <span class="text-[10px] text-[#8A8178] font-medium mt-0.5">Tổng: 4,200g</span>
                </div>
              </td>
              <td class="px-5 py-4 text-center">
                <span class="text-xs font-bold text-[#8A8178] px-2.5 py-1 rounded-md bg-[#FDFBF7] border border-[#EAE3D9]">2,000g</span>
              </td>
              <td class="px-5 py-4 text-center font-medium text-xs text-[#5C544E] uppercase">g</td>
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-2">
                  <button class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors" title="Xem lịch sử thẻ kho" @click.stop="isHistoryOpen = true">
                    <History class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button class="p-2 text-[#CC8033] hover:text-white hover:bg-[#CC8033] border border-[#CC8033]/30 rounded-lg transition-colors shadow-sm" title="Điều chỉnh kho nhanh" @click.stop="isAdjustOpen = true">
                    <SlidersHorizontal class="w-4 h-4" stroke-width="2" />
                  </button>
                </div>
              </td>
            </tr>

            <!-- Dòng 3: Sữa đặc (Raw) - Đang đầy đủ -->
            <tr class="hover:bg-[#FDFBF7] transition-colors cursor-pointer group" @click="openDrawer">
              <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">RAW-MK-005</td>
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg border border-[#EAE3D9] bg-white flex items-center justify-center shadow-sm flex-shrink-0 group-hover:border-[#CC8033]/50 transition-colors">
                    <Milk class="w-4 h-4 text-[#8A8178] group-hover:text-[#CC8033]" stroke-width="1.5" />
                  </div>
                  <span class="font-bold text-[#2A231E] whitespace-nowrap">Sữa đặc Ngôi sao Phương Nam</span>
                </div>
              </td>
              <td class="px-5 py-4">
                <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Nguyên liệu thô</span>
              </td>
              <td class="px-5 py-4">
                <div class="flex flex-col">
                  <span class="font-bold text-[#4A7C59] text-sm whitespace-nowrap">12 Thùng & 5 Lon</span>
                  <span class="text-[10px] text-[#8A8178] font-medium mt-0.5">Tổng: 293 Lon</span>
                </div>
              </td>
              <td class="px-5 py-4 text-center">
                <span class="text-xs font-bold text-[#8A8178] px-2.5 py-1 rounded-md bg-[#FDFBF7] border border-[#EAE3D9]">50 Lon</span>
              </td>
              <td class="px-5 py-4 text-center font-medium text-xs text-[#5C544E] uppercase">Lon</td>
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-2">
                  <button class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors" title="Xem lịch sử thẻ kho" @click.stop="isHistoryOpen = true">
                    <History class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button class="p-2 text-[#CC8033] hover:text-white hover:bg-[#CC8033] border border-[#CC8033]/30 rounded-lg transition-colors shadow-sm" title="Điều chỉnh kho nhanh" @click.stop="isAdjustOpen = true">
                    <SlidersHorizontal class="w-4 h-4" stroke-width="2" />
                  </button>
                </div>
              </td>
            </tr>

            <!-- Dòng 4: Cốc giấy (Vật tư) - Đã rỗng -->
            <tr class="hover:bg-[#FDFBF7] transition-colors cursor-pointer group bg-red-50/30" @click="openDrawer">
              <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">SUP-CUP-01</td>
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg border border-red-200 bg-white flex items-center justify-center shadow-sm flex-shrink-0">
                    <Coffee class="w-4 h-4 text-red-500" stroke-width="1.5" />
                  </div>
                  <span class="font-bold text-[#2A231E] whitespace-nowrap">Ly giấy Takeaway 450ml</span>
                </div>
              </td>
              <td class="px-5 py-4">
                <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Vật tư</span>
              </td>
              <td class="px-5 py-4">
                <div class="flex flex-col">
                  <span class="font-bold text-red-600 text-sm whitespace-nowrap">0 Chiếc</span>
                  <span class="text-[10px] text-red-500 font-medium mt-0.5">Đã rỗng kho!</span>
                </div>
              </td>
              <td class="px-5 py-4 text-center">
                <span class="text-xs font-bold text-[#8A8178] px-2.5 py-1 rounded-md bg-[#FDFBF7] border border-[#EAE3D9]">500 Chiếc</span>
              </td>
              <td class="px-5 py-4 text-center font-medium text-xs text-[#5C544E] uppercase">Chiếc</td>
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-2">
                  <button class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors" title="Xem lịch sử thẻ kho" @click.stop="isHistoryOpen = true">
                    <History class="w-4 h-4" stroke-width="2" />
                  </button>
                  <button class="p-2 text-[#CC8033] hover:text-white hover:bg-[#CC8033] border border-[#CC8033]/30 rounded-lg transition-colors shadow-sm" title="Điều chỉnh kho nhanh" @click.stop="isAdjustOpen = true">
                    <SlidersHorizontal class="w-4 h-4" stroke-width="2" />
                  </button>
                </div>
              </td>
            </tr>
            <!-- Empty State -->
            <tr class="hover:bg-transparent">
              <td colspan="7" class="px-5 py-16 text-center">
                <div class="flex flex-col items-center justify-center">
                  <div class="w-16 h-16 bg-[#FDFBF7] rounded-full flex items-center justify-center border border-[#EAE3D9] mb-3">
                    <PackageOpen class="w-8 h-8 text-[#8A8178] opacity-50" stroke-width="1.5" />
                  </div>
                  <p class="text-[#5C544E] font-bold text-base mb-1">Không tìm thấy nguyên liệu phù hợp</p>
                  <p class="text-[#8A8178] text-xs">Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm của bạn.</p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Bảng phụ (Side Drawer) - Chi tiết lô hàng FIFO -->
    <div 
      class="fixed inset-0 bg-[#2A231E]/40 backdrop-blur-sm z-40 transition-opacity duration-300"
      :class="isDrawerOpen ? 'opacity-100 pointer-events-auto' : 'opacity-0 pointer-events-none'"
      @click="closeDrawer"
    ></div>
    
    <div 
      class="fixed top-0 right-0 h-full w-full max-w-[480px] bg-white shadow-2xl z-50 transform transition-transform duration-300 ease-in-out flex flex-col"
      :class="isDrawerOpen ? 'translate-x-0' : 'translate-x-full'"
    >
      <!-- Drawer Header -->
      <div class="px-6 py-5 border-b border-[#EAE3D9] flex items-center justify-between bg-[#FDFBF7]">
        <div>
          <h2 class="text-lg font-bold text-[#2A231E]">Chi tiết nguyên liệu</h2>
          <p class="text-xs text-[#8A8178] mt-1 font-mono">SKU: RAW-CF-001</p>
        </div>
        <button @click="closeDrawer" class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors">
          <X class="w-5 h-5" />
        </button>
      </div>

      <!-- Drawer Content -->
      <div class="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar">
        <!-- Thông tin tóm tắt -->
        <div class="flex items-start gap-4">
          <div class="w-16 h-16 rounded-xl border border-[#EAE3D9] bg-[#FDFBF7] flex items-center justify-center shadow-sm flex-shrink-0">
            <Coffee class="w-8 h-8 text-[#CC8033]" stroke-width="1.5" />
          </div>
          <div>
            <h3 class="font-bold text-[#2A231E] text-lg">Hạt cà phê Robusta</h3>
            <p class="text-sm text-[#5C544E] mt-1">Nguyên liệu thô • Đơn vị: g</p>
            <div class="mt-2 flex gap-2">
              <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#CC8033] text-[10px] font-bold uppercase tracking-wider">
                <AlertTriangle class="w-3 h-3 mr-1" /> Sắp hết hàng
              </span>
            </div>
          </div>
        </div>

        <div class="h-px w-full bg-[#EAE3D9]"></div>

        <!-- Quản lý lô hàng (Batch Management) -->
        <div>
          <div class="flex items-center justify-between mb-4">
            <h4 class="font-bold text-[#2A231E] flex items-center gap-2">
              <Layers class="w-4 h-4 text-[#8A8178]" />
              Chi tiết Lô hàng (FIFO)
            </h4>
            <span class="text-xs text-[#8A8178] font-medium">Quy tắc xuất: Vào trước, xuất trước</span>
          </div>

          <div class="space-y-3">
            <!-- Lô hàng 1: Khẩn cấp/Đang sử dụng -->
            <div class="border border-[#E8C5A5] bg-[#FFF9F2] rounded-xl p-4 relative overflow-hidden shadow-sm">
              <div class="absolute top-0 right-0 bg-[#CC8033] text-white text-[9px] font-bold px-2 py-1 rounded-bl-lg uppercase tracking-wider">
                Đang xuất kho
              </div>
              <div class="flex justify-between items-start mb-3">
                <div>
                  <p class="text-xs font-bold text-[#5C544E] mb-1">Mã lô: <span class="text-[#2A231E] font-mono">BAT-2605-01</span></p>
                  <p class="text-[11px] text-[#8A8178]">Nhập: 10/05/2026</p>
                </div>
                <div class="text-right">
                  <p class="font-bold text-[#CC8033] text-base">450g</p>
                  <p class="text-[10px] text-[#8A8178]">Tồn khả dụng</p>
                </div>
              </div>
              <div class="flex items-center gap-2 text-[11px] font-medium text-[#C2410C] bg-white border border-[#E8C5A5]/50 px-2 py-1.5 rounded-md w-max">
                <Clock class="w-3 h-3" />
                HSD: 30/06/2026 (Còn 27 ngày)
              </div>
            </div>

            <!-- Lô hàng 2: Còn mới -->
            <div class="border border-[#EAE3D9] bg-white rounded-xl p-4 relative shadow-sm">
              <div class="flex justify-between items-start mb-3">
                <div>
                  <p class="text-xs font-bold text-[#5C544E] mb-1">Mã lô: <span class="text-[#2A231E] font-mono">BAT-2605-09</span></p>
                  <p class="text-[11px] text-[#8A8178]">Nhập: 25/05/2026</p>
                </div>
                <div class="text-right">
                  <p class="font-bold text-[#2A231E] text-base">5,000g <span class="text-xs font-normal text-[#8A8178]">(1 Bao)</span></p>
                  <p class="text-[10px] text-[#8A8178]">Tồn khả dụng</p>
                </div>
              </div>
              <div class="flex items-center gap-2 text-[11px] font-medium text-[#4A7C59] bg-[#FDFBF7] border border-[#EAE3D9] px-2 py-1.5 rounded-md w-max">
                <CheckCircle2 class="w-3 h-3" />
                HSD: 25/11/2026
              </div>
            </div>

             <!-- Lô hàng 3: Còn mới -->
             <div class="border border-[#EAE3D9] bg-white rounded-xl p-4 relative shadow-sm">
              <div class="flex justify-between items-start mb-3">
                <div>
                  <p class="text-xs font-bold text-[#5C544E] mb-1">Mã lô: <span class="text-[#2A231E] font-mono">BAT-2606-02</span></p>
                  <p class="text-[11px] text-[#8A8178]">Nhập: 01/06/2026</p>
                </div>
                <div class="text-right">
                  <p class="font-bold text-[#2A231E] text-base">5,000g <span class="text-xs font-normal text-[#8A8178]">(1 Bao)</span></p>
                  <p class="text-[10px] text-[#8A8178]">Tồn khả dụng</p>
                </div>
              </div>
              <div class="flex items-center gap-2 text-[11px] font-medium text-[#4A7C59] bg-[#FDFBF7] border border-[#EAE3D9] px-2 py-1.5 rounded-md w-max">
                <CheckCircle2 class="w-3 h-3" />
                HSD: 01/12/2026
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Drawer Footer -->
      <div class="p-5 border-t border-[#EAE3D9] bg-[#FDFBF7] flex justify-end gap-3">
        <button class="px-5 py-2.5 rounded-lg border border-[#EAE3D9] text-[#5C544E] text-xs font-bold uppercase tracking-wider hover:bg-[#EAE3D9]/50 transition-colors bg-white shadow-sm">
          Nhập hàng thêm
        </button>
        <button class="px-5 py-2.5 rounded-lg border border-[#CC8033] bg-[#CC8033] text-white text-xs font-bold uppercase tracking-wider hover:bg-[#B87029] transition-colors shadow-sm">
          Chỉnh sửa số lượng
        </button>
      </div>
    </div>

    <!-- SlideOverHistory - Lịch sử thẻ kho -->
    <div 
      class="fixed inset-0 bg-[#2A231E]/40 backdrop-blur-sm z-40 transition-opacity duration-300"
      :class="isHistoryOpen ? 'opacity-100 pointer-events-auto' : 'opacity-0 pointer-events-none'"
      @click="isHistoryOpen = false"
    ></div>
    
    <div 
      class="fixed top-0 right-0 h-full w-[35%] min-w-[320px] bg-white shadow-2xl z-50 transform transition-transform duration-300 ease-in-out flex flex-col border-l border-[#EAE3D9]"
      :class="isHistoryOpen ? 'translate-x-0' : 'translate-x-full'"
    >
      <div class="px-6 py-5 border-b border-[#EAE3D9] flex items-center justify-between bg-[#FDFBF7]">
        <div>
          <h2 class="text-lg font-bold text-[#2A231E]">Lịch sử thẻ kho</h2>
          <p class="text-xs text-[#8A8178] mt-1 font-mono">SKU: RAW-CF-001</p>
        </div>
        <button @click="isHistoryOpen = false" class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors">
          <X class="w-5 h-5" />
        </button>
      </div>

      <div class="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar">
        <div class="relative border-l-2 border-[#EAE3D9] ml-3 space-y-8 pb-4">
          
          <!-- Timeline Item 1 -->
          <div class="relative pl-6">
            <div class="absolute -left-[9px] top-1 w-4 h-4 rounded-full bg-red-100 border-2 border-red-500 z-10"></div>
            <div class="mb-1 flex items-center gap-2">
              <span class="text-xs font-bold text-[#8A8178]">Hôm nay, 15:30</span>
              <span class="px-2 py-0.5 rounded bg-gray-100 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">Hệ thống</span>
            </div>
            <div class="bg-[#FDFBF7] border border-[#EAE3D9] p-3 rounded-lg shadow-sm">
              <p class="text-sm font-bold text-[#2A231E] mb-1">Xuất kho bán hàng</p>
              <div class="flex justify-between items-center">
                <p class="text-xs text-[#8A8178]">Tự động trừ kho qua POS</p>
                <p class="font-bold text-red-500">- 40 g</p>
              </div>
            </div>
          </div>

          <!-- Timeline Item 2 -->
          <div class="relative pl-6">
            <div class="absolute -left-[9px] top-1 w-4 h-4 rounded-full bg-green-100 border-2 border-[#4A7C59] z-10"></div>
            <div class="mb-1 flex items-center gap-2">
              <span class="text-xs font-bold text-[#8A8178]">03/06/2026, 10:00</span>
              <span class="px-2 py-0.5 rounded bg-[#CC8033]/10 text-[#CC8033] text-[10px] font-bold uppercase tracking-wider">Nguyễn Văn A</span>
            </div>
            <div class="bg-[#FDFBF7] border border-[#EAE3D9] p-3 rounded-lg shadow-sm">
              <p class="text-sm font-bold text-[#2A231E] mb-1">Nhập kho từ phiếu INB-2406-003</p>
              <div class="flex justify-between items-center">
                <p class="text-xs text-[#8A8178]">NCC: Đại lý Cà phê Quận 1</p>
                <p class="font-bold text-[#4A7C59]">+ 12 Thùng</p>
              </div>
            </div>
          </div>

          <!-- Timeline Item 3 -->
          <div class="relative pl-6">
            <div class="absolute -left-[9px] top-1 w-4 h-4 rounded-full bg-orange-100 border-2 border-orange-500 z-10"></div>
            <div class="mb-1 flex items-center gap-2">
              <span class="text-xs font-bold text-[#8A8178]">01/06/2026, 18:00</span>
              <span class="px-2 py-0.5 rounded bg-[#CC8033]/10 text-[#CC8033] text-[10px] font-bold uppercase tracking-wider">Trần Thị B</span>
            </div>
            <div class="bg-[#FDFBF7] border border-[#EAE3D9] p-3 rounded-lg shadow-sm">
              <p class="text-sm font-bold text-[#2A231E] mb-1">Điều chỉnh kiểm kê kho</p>
              <div class="flex justify-between items-center">
                <p class="text-xs text-[#8A8178]">Hao hụt tự nhiên</p>
                <p class="font-bold text-orange-500">- 150 g</p>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>

    <!-- ModalAdjust - Điều chỉnh kho nhanh -->
    <div 
      class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-50 transition-opacity duration-300 flex justify-center items-center p-4"
      v-if="isAdjustOpen"
      @click.self="isAdjustOpen = false"
    >
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-sm overflow-hidden animate-in zoom-in-95 duration-200">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h2 class="text-lg font-bold text-[#2A231E]">Điều chỉnh kho</h2>
          <button @click="isAdjustOpen = false" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md transition-colors">
            <X class="w-5 h-5" />
          </button>
        </div>
        <div class="p-5 space-y-4">
          <div>
            <p class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1">SKU: RAW-CF-001</p>
            <p class="font-bold text-[#2A231E] text-base">Hạt cà phê Robusta</p>
            <p class="text-xs text-[#8A8178] mt-1">Tồn hiện tại trên hệ thống: <strong class="text-[#CC8033]">10,450 g</strong></p>
          </div>
          
          <div class="space-y-1.5 pt-2">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Số lượng thực tế (g) <span class="text-red-500">*</span></label>
            <input type="number" placeholder="Nhập số lượng thực tế..." class="w-full text-right bg-[#FDFBF7] border border-[#CC8033] h-11 rounded-lg px-3 text-lg font-bold focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 shadow-inner" />
          </div>
          
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Lý do điều chỉnh</label>
            <select class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
              <option disabled selected>Chọn lý do...</option>
              <option>Hao hụt tự nhiên</option>
              <option>Sai sót kiểm đếm</option>
              <option>Hàng hỏng/Hủy</option>
            </select>
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="isAdjustOpen = false" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Hủy</button>
          <button class="px-5 py-2 rounded-lg bg-[#2A231E] text-white text-xs font-bold uppercase shadow-md hover:bg-black transition-colors">Lưu điều chỉnh</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Package, 
  AlertTriangle, 
  Clock, 
  Search, 
  Plus, 
  Coffee, 
  CupSoda, 
  Milk, 
  History, 
  SlidersHorizontal,
  X,
  Layers,
  CheckCircle2,
  PackageOpen
} from 'lucide-vue-next'

const isDrawerOpen = ref(false)
const isHistoryOpen = ref(false)
const isAdjustOpen = ref(false)

const openDrawer = () => {
  isDrawerOpen.value = true
}

const closeDrawer = () => {
  isDrawerOpen.value = false
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700&display=swap');

.font-premium-sans {
  font-family: 'Inter', sans-serif;
}

/* Optional: Custom scrollbar for table and drawer */
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
