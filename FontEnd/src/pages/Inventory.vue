<template>
  <div class="space-y-6 font-premium-sans text-[#2A231E] p-4 sm:p-6 lg:p-8 max-w-[1400px] mx-auto min-h-screen relative">

    <!-- ===== FLOW STEPPER ===== -->
    <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-sm p-4 flex flex-wrap items-center gap-2 text-xs font-bold uppercase tracking-wider">
      <span class="text-[10px] text-[#8A8178]">Luồng nhập kho:</span>
      <router-link to="/suppliers" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><Truck class="w-3.5 h-3.5" /> Nguồn cung</router-link>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <router-link to="/suppliers" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><ClipboardList class="w-3.5 h-3.5" /> Phiếu nhập</router-link>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <span class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#CC8033] text-white"><Package class="w-3.5 h-3.5" /> Kho</span>
      <ChevronRight class="w-3.5 h-3.5 text-[#D5C9B3]" />
      <router-link to="/stocktake" class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/40 transition-colors"><ClipboardCheck class="w-3.5 h-3.5" /> Kiểm kê</router-link>
    </div>

    <!-- Metrics -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-5">
      <div class="bg-white rounded-xl border border-[#EAE3D9] p-5 shadow-sm flex items-center justify-between hover:shadow-md transition-shadow">
        <div>
          <p class="text-xs font-bold text-[#8A8178] uppercase tracking-wider mb-1">Tổng SKU Hiện Có</p>
          <h3 class="text-3xl font-bold text-[#2A231E]">{{ items.length }}</h3>
          <p class="text-[11px] text-[#5C544E] mt-2">Số mã nguyên liệu đang theo dõi</p>
        </div>
        <div class="w-12 h-12 rounded-full bg-[#FDFBF7] border border-[#EAE3D9] flex items-center justify-center text-[#2A231E]"><Package class="w-6 h-6" stroke-width="1.5" /></div>
      </div>
      <div class="bg-white rounded-xl border border-[#E8C5A5] p-5 shadow-sm flex items-center justify-between relative overflow-hidden">
        <div class="absolute inset-y-0 left-0 w-1 bg-[#CC8033]"></div>
        <div class="pl-2">
          <p class="text-xs font-bold text-[#CC8033] uppercase tracking-wider mb-1">Sắp Hết Hàng</p>
          <h3 class="text-3xl font-bold text-[#2A231E]">{{ lowStockCount }}</h3>
          <p class="text-[11px] text-[#5C544E] mt-2">Nguyên liệu chạm ngưỡng tối thiểu</p>
        </div>
        <div class="w-12 h-12 rounded-full bg-[#FFF9F2] border border-[#E8C5A5] flex items-center justify-center text-[#CC8033]"><AlertTriangle class="w-6 h-6" stroke-width="1.5" /></div>
      </div>
      <div class="bg-white rounded-xl border border-red-200 p-5 shadow-sm flex items-center justify-between relative overflow-hidden">
        <div class="absolute inset-y-0 left-0 w-1 bg-red-500"></div>
        <div class="pl-2">
          <p class="text-xs font-bold text-red-500 uppercase tracking-wider mb-1">Đã Rỗng Kho</p>
          <h3 class="text-3xl font-bold text-[#2A231E]">{{ emptyCount }}</h3>
          <p class="text-[11px] text-[#5C544E] mt-2">Cần nhập hàng gấp</p>
        </div>
        <div class="w-12 h-12 rounded-full bg-red-50 border border-red-100 flex items-center justify-center text-red-500"><Clock class="w-6 h-6" stroke-width="1.5" /></div>
      </div>
    </div>

    <!-- Filter bar -->
    <div class="flex flex-col md:flex-row items-center justify-between gap-4 bg-white p-4 rounded-xl border border-[#EAE3D9] shadow-sm">
      <div class="flex flex-col sm:flex-row gap-4 w-full md:w-auto flex-1">
        <div class="relative w-full sm:max-w-xs md:max-w-md">
          <Search class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-[#8A8178]" stroke-width="2" />
          <input v-model="search" placeholder="Tìm theo Tên hoặc Mã SKU..." class="pl-10 w-full bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg text-sm font-medium focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033] transition-all" />
        </div>
        <select v-model="typeFilter" class="bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg px-4 text-sm font-medium text-[#2A231E] focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 w-full sm:w-52 cursor-pointer">
          <option value="all">Tất cả loại nguyên liệu</option>
          <option value="Nguyên liệu thô">Nguyên liệu thô</option>
          <option value="Topping">Bán thành phẩm / Topping</option>
          <option value="Vật tư">Vật tư</option>
        </select>
        <select v-model="statusFilter" class="bg-[#FDFBF7] border border-[#EAE3D9] h-10 rounded-lg px-4 text-sm font-medium text-[#2A231E] focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 w-full sm:w-48 cursor-pointer">
          <option value="all">Tất cả trạng thái</option>
          <option value="ok">Còn hàng</option>
          <option value="low">Sắp hết hàng</option>
          <option value="empty">Đã rỗng</option>
        </select>
      </div>
      <button @click="openAdd" class="w-full md:w-auto flex items-center justify-center bg-[#CC8033] hover:bg-[#B87029] text-white h-10 px-5 rounded-lg shadow-md transition-colors text-xs font-bold uppercase tracking-wider">
        <Plus class="w-4 h-4 mr-2" stroke-width="2.5" /> Thêm SKU Mới
      </button>
    </div>

    <!-- Table -->
    <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
      <div class="overflow-x-auto custom-scrollbar">
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-[#FDFBF7] text-[#8A8178] text-[10px] uppercase tracking-[0.1em] border-b border-[#EAE3D9]">
              <th class="px-5 py-4 font-bold">Mã SKU</th>
              <th class="px-5 py-4 font-bold">Tên nguyên liệu</th>
              <th class="px-5 py-4 font-bold">Phân loại</th>
              <th class="px-5 py-4 font-bold">Tồn kho thực tế</th>
              <th class="px-5 py-4 font-bold text-center">Ngưỡng (Min)</th>
              <th class="px-5 py-4 font-bold text-center">Đơn vị gốc</th>
              <th class="px-5 py-4 font-bold text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-[#EAE3D9]/60">
            <tr
              v-for="item in filteredItems"
              :key="item.sku"
              class="hover:bg-[#FDFBF7] transition-colors cursor-pointer group"
              :class="stockState(item) === 'empty' ? 'bg-red-50/30' : ''"
              @click="openDrawer(item)"
            >
              <td class="px-5 py-4 font-mono text-xs font-semibold text-[#5C544E]">{{ item.sku }}</td>
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg border bg-white flex items-center justify-center shadow-sm flex-shrink-0 transition-colors"
                       :class="stockState(item) === 'empty' ? 'border-red-200' : 'border-[#EAE3D9] group-hover:border-[#CC8033]/50'">
                    <component :is="iconFor(item)" class="w-4 h-4" :class="stockState(item) === 'empty' ? 'text-red-500' : 'text-[#8A8178] group-hover:text-[#CC8033]'" stroke-width="1.5" />
                  </div>
                  <span class="font-bold text-[#2A231E] whitespace-nowrap">{{ item.name }}</span>
                </div>
              </td>
              <td class="px-5 py-4">
                <span class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#5C544E] text-[10px] font-bold uppercase tracking-wider">{{ item.category }}</span>
              </td>
              <td class="px-5 py-4">
                <div class="flex flex-col">
                  <span class="font-bold text-sm whitespace-nowrap" :class="stockColor(item)">{{ item.qty === 0 ? '0 ' + item.unit : displayQty(item) }}</span>
                  <span class="text-[10px] font-medium mt-0.5" :class="stockState(item) === 'empty' ? 'text-red-500' : 'text-[#8A8178]'">{{ stockLabel(item) }}</span>
                </div>
              </td>
              <td class="px-5 py-4 text-center">
                <span class="text-xs font-bold text-[#8A8178] px-2.5 py-1 rounded-md bg-[#FDFBF7] border border-[#EAE3D9]">{{ formatNumber(item.min) }}{{ item.unit }}</span>
              </td>
              <td class="px-5 py-4 text-center font-medium text-xs text-[#5C544E] uppercase">{{ item.unit }}</td>
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-2">
                  <button class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors" title="Xem lịch sử thẻ kho" @click.stop="openHistory(item)"><History class="w-4 h-4" stroke-width="2" /></button>
                  <button class="p-2 text-[#CC8033] hover:text-white hover:bg-[#CC8033] border border-[#CC8033]/30 rounded-lg transition-colors shadow-sm" title="Điều chỉnh kho nhanh" @click.stop="openAdjust(item)"><SlidersHorizontal class="w-4 h-4" stroke-width="2" /></button>
                </div>
              </td>
            </tr>
            <tr v-if="filteredItems.length === 0">
              <td colspan="7" class="px-5 py-16 text-center">
                <div class="flex flex-col items-center justify-center">
                  <div class="w-16 h-16 bg-[#FDFBF7] rounded-full flex items-center justify-center border border-[#EAE3D9] mb-3"><PackageOpen class="w-8 h-8 text-[#8A8178] opacity-50" stroke-width="1.5" /></div>
                  <p class="text-[#5C544E] font-bold text-base mb-1">Không tìm thấy nguyên liệu phù hợp</p>
                  <p class="text-[#8A8178] text-xs">Thử thay đổi bộ lọc hoặc từ khóa tìm kiếm của bạn.</p>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- DRAWER: Chi tiết lô hàng FIFO -->
    <!-- ===================================================================== -->
    <div class="fixed inset-0 bg-[#2A231E]/40 backdrop-blur-sm z-40 transition-opacity duration-300" :class="selected && drawerMode === 'detail' ? 'opacity-100 pointer-events-auto' : 'opacity-0 pointer-events-none'" @click="closeDrawer"></div>
    <div class="fixed top-0 right-0 h-full w-full max-w-[480px] bg-white shadow-2xl z-50 transform transition-transform duration-300 ease-in-out flex flex-col" :class="selected && drawerMode === 'detail' ? 'translate-x-0' : 'translate-x-full'">
      <template v-if="selected">
        <div class="px-6 py-5 border-b border-[#EAE3D9] flex items-center justify-between bg-[#FDFBF7]">
          <div>
            <h2 class="text-lg font-bold text-[#2A231E]">Chi tiết nguyên liệu</h2>
            <p class="text-xs text-[#8A8178] mt-1 font-mono">SKU: {{ selected.sku }}</p>
          </div>
          <button @click="closeDrawer" class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors"><X class="w-5 h-5" /></button>
        </div>

        <div class="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar">
          <div class="flex items-start gap-4">
            <div class="w-16 h-16 rounded-xl border border-[#EAE3D9] bg-[#FDFBF7] flex items-center justify-center shadow-sm flex-shrink-0">
              <component :is="iconFor(selected)" class="w-8 h-8 text-[#CC8033]" stroke-width="1.5" />
            </div>
            <div>
              <h3 class="font-bold text-[#2A231E] text-lg">{{ selected.name }}</h3>
              <p class="text-sm text-[#5C544E] mt-1">{{ selected.category }} • Đơn vị: {{ selected.unit }}</p>
              <div class="mt-2 flex gap-2">
                <span v-if="stockState(selected) === 'low'" class="inline-flex items-center px-2 py-1 rounded bg-[#EAE3D9]/50 text-[#CC8033] text-[10px] font-bold uppercase tracking-wider"><AlertTriangle class="w-3 h-3 mr-1" /> Sắp hết hàng</span>
                <span v-else-if="stockState(selected) === 'empty'" class="inline-flex items-center px-2 py-1 rounded bg-red-50 text-red-500 text-[10px] font-bold uppercase tracking-wider"><AlertTriangle class="w-3 h-3 mr-1" /> Đã rỗng</span>
                <span v-else class="inline-flex items-center px-2 py-1 rounded bg-green-50 text-[#4A7C59] text-[10px] font-bold uppercase tracking-wider"><CheckCircle2 class="w-3 h-3 mr-1" /> Đầy đủ</span>
              </div>
            </div>
          </div>

          <div class="h-px w-full bg-[#EAE3D9]"></div>

          <div>
            <div class="flex items-center justify-between mb-4">
              <h4 class="font-bold text-[#2A231E] flex items-center gap-2"><Layers class="w-4 h-4 text-[#8A8178]" /> Chi tiết Lô hàng (FIFO)</h4>
              <span class="text-xs text-[#8A8178] font-medium">Vào trước, xuất trước</span>
            </div>

            <div v-if="selected.batches.length === 0" class="text-center py-8 text-[#8A8178] text-sm">Chưa có lô hàng nào.</div>
            <div v-else class="space-y-3">
              <div v-for="(b, i) in selected.batches" :key="b.code" class="border rounded-xl p-4 relative overflow-hidden shadow-sm"
                   :class="i === 0 ? 'border-[#E8C5A5] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white'">
                <div v-if="i === 0" class="absolute top-0 right-0 bg-[#CC8033] text-white text-[9px] font-bold px-2 py-1 rounded-bl-lg uppercase tracking-wider">Đang xuất kho</div>
                <div class="flex justify-between items-start mb-3">
                  <div>
                    <p class="text-xs font-bold text-[#5C544E] mb-1">Mã lô: <span class="text-[#2A231E] font-mono">{{ b.code }}</span></p>
                    <p class="text-[11px] text-[#8A8178]">Nhập: {{ b.importDate }}</p>
                  </div>
                  <div class="text-right">
                    <p class="font-bold text-base" :class="i === 0 ? 'text-[#CC8033]' : 'text-[#2A231E]'">{{ formatNumber(b.qty) }}{{ selected.unit }}</p>
                    <p class="text-[10px] text-[#8A8178]">Tồn khả dụng</p>
                  </div>
                </div>
                <div class="flex items-center gap-2 text-[11px] font-medium px-2 py-1.5 rounded-md w-max border"
                     :class="i === 0 ? 'text-[#C2410C] bg-white border-[#E8C5A5]/50' : 'text-[#4A7C59] bg-[#FDFBF7] border-[#EAE3D9]'">
                  <Clock v-if="i === 0" class="w-3 h-3" /><CheckCircle2 v-else class="w-3 h-3" />
                  HSD: {{ b.expiry }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="p-5 border-t border-[#EAE3D9] bg-[#FDFBF7] flex justify-end gap-3">
          <router-link to="/suppliers" class="px-5 py-2.5 rounded-lg border border-[#EAE3D9] text-[#5C544E] text-xs font-bold uppercase tracking-wider hover:bg-[#EAE3D9]/50 transition-colors bg-white shadow-sm">Nhập hàng thêm</router-link>
          <button @click="openAdjust(selected)" class="px-5 py-2.5 rounded-lg border border-[#CC8033] bg-[#CC8033] text-white text-xs font-bold uppercase tracking-wider hover:bg-[#B87029] transition-colors shadow-sm">Điều chỉnh số lượng</button>
        </div>
      </template>
    </div>

    <!-- ===================================================================== -->
    <!-- DRAWER: Lịch sử thẻ kho -->
    <!-- ===================================================================== -->
    <div class="fixed inset-0 bg-[#2A231E]/40 backdrop-blur-sm z-40 transition-opacity duration-300" :class="selected && drawerMode === 'history' ? 'opacity-100 pointer-events-auto' : 'opacity-0 pointer-events-none'" @click="closeDrawer"></div>
    <div class="fixed top-0 right-0 h-full w-[40%] min-w-[340px] bg-white shadow-2xl z-50 transform transition-transform duration-300 ease-in-out flex flex-col border-l border-[#EAE3D9]" :class="selected && drawerMode === 'history' ? 'translate-x-0' : 'translate-x-full'">
      <template v-if="selected">
        <div class="px-6 py-5 border-b border-[#EAE3D9] flex items-center justify-between bg-[#FDFBF7]">
          <div>
            <h2 class="text-lg font-bold text-[#2A231E]">Lịch sử thẻ kho</h2>
            <p class="text-xs text-[#8A8178] mt-1 font-mono">SKU: {{ selected.sku }}</p>
          </div>
          <button @click="closeDrawer" class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-[#EAE3D9]/50 rounded-lg transition-colors"><X class="w-5 h-5" /></button>
        </div>
        <div class="flex-1 overflow-y-auto p-6 custom-scrollbar">
          <div v-if="selected.history.length === 0" class="text-center py-10 text-[#8A8178] text-sm">Chưa có biến động kho.</div>
          <div v-else class="relative border-l-2 border-[#EAE3D9] ml-3 space-y-8 pb-4">
            <div v-for="(h, i) in selected.history" :key="i" class="relative pl-6">
              <div class="absolute -left-[9px] top-1 w-4 h-4 rounded-full z-10 border-2"
                   :class="h.delta > 0 ? 'bg-green-100 border-[#4A7C59]' : (h.kind === 'adjust' ? 'bg-orange-100 border-orange-500' : 'bg-red-100 border-red-500')"></div>
              <div class="mb-1 flex items-center gap-2">
                <span class="text-xs font-bold text-[#8A8178]">{{ h.time }}</span>
                <span class="px-2 py-0.5 rounded text-[10px] font-bold uppercase tracking-wider" :class="h.by === 'Hệ thống' ? 'bg-gray-100 text-[#5C544E]' : 'bg-[#CC8033]/10 text-[#CC8033]'">{{ h.by }}</span>
              </div>
              <div class="bg-[#FDFBF7] border border-[#EAE3D9] p-3 rounded-lg shadow-sm">
                <p class="text-sm font-bold text-[#2A231E] mb-1">{{ h.title }}</p>
                <div class="flex justify-between items-center">
                  <p class="text-xs text-[#8A8178]">{{ h.note }}</p>
                  <p class="font-bold" :class="h.delta > 0 ? 'text-[#4A7C59]' : (h.kind === 'adjust' ? 'text-orange-500' : 'text-red-500')">{{ h.delta > 0 ? '+' : '' }}{{ formatNumber(h.delta) }} {{ selected.unit }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
    </div>

    <!-- ===================================================================== -->
    <!-- MODAL: Điều chỉnh kho nhanh (reactive) -->
    <!-- ===================================================================== -->
    <div v-if="adjustTarget" class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-[60] flex justify-center items-center p-4" @click.self="adjustTarget = null">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-sm overflow-hidden animate-in zoom-in-95 duration-200">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h2 class="text-lg font-bold text-[#2A231E]">Điều chỉnh kho</h2>
          <button @click="adjustTarget = null" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md"><X class="w-5 h-5" /></button>
        </div>
        <div class="p-5 space-y-4">
          <div>
            <p class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1">SKU: {{ adjustTarget.sku }}</p>
            <p class="font-bold text-[#2A231E] text-base">{{ adjustTarget.name }}</p>
            <p class="text-xs text-[#8A8178] mt-1">Tồn hiện tại: <strong class="text-[#CC8033]">{{ formatNumber(adjustTarget.qty) }} {{ adjustTarget.unit }}</strong></p>
          </div>
          <div class="space-y-1.5 pt-2">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Số lượng thực tế ({{ adjustTarget.unit }}) <span class="text-red-500">*</span></label>
            <input type="number" min="0" v-model.number="adjustActual" placeholder="Nhập số lượng thực tế..." class="w-full text-right bg-[#FDFBF7] border border-[#CC8033] h-11 rounded-lg px-3 text-lg font-bold focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 shadow-inner" />
            <p v-if="adjustActual !== null" class="text-xs font-bold text-right" :class="adjustDiff === 0 ? 'text-[#8A8178]' : (adjustDiff > 0 ? 'text-[#4A7C59]' : 'text-red-500')">
              Chênh lệch: {{ adjustDiff > 0 ? '+' : '' }}{{ formatNumber(adjustDiff) }} {{ adjustTarget.unit }}
            </p>
          </div>
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Lý do điều chỉnh</label>
            <select v-model="adjustReason" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
              <option>Hao hụt tự nhiên</option>
              <option>Sai sót kiểm đếm</option>
              <option>Hàng hỏng/Hủy</option>
            </select>
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="adjustTarget = null" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Hủy</button>
          <button @click="confirmAdjust" class="px-5 py-2 rounded-lg bg-[#2A231E] text-white text-xs font-bold uppercase shadow-md hover:bg-black transition-colors">Lưu điều chỉnh</button>
        </div>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- MODAL: Thêm SKU mới -->
    <!-- ===================================================================== -->
    <div v-if="isAddOpen" class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-[60] flex justify-center items-center p-4" @click.self="isAddOpen = false">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-md overflow-hidden animate-in zoom-in-95 duration-200">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h2 class="text-lg font-bold text-[#2A231E]">Thêm SKU mới</h2>
          <button @click="isAddOpen = false" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md"><X class="w-5 h-5" /></button>
        </div>
        <div class="p-5 space-y-4">
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Tên nguyên liệu <span class="text-red-500">*</span></label>
            <input v-model="newItem.name" placeholder="VD: Siro Caramel" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
          </div>
          <div class="grid grid-cols-2 gap-3">
            <div class="space-y-1.5">
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Phân loại</label>
              <select v-model="newItem.category" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
                <option>Nguyên liệu thô</option>
                <option>Topping</option>
                <option>Vật tư</option>
              </select>
            </div>
            <div class="space-y-1.5">
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Đơn vị gốc</label>
              <input v-model="newItem.unit" placeholder="g / Lon / Chiếc" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
            </div>
          </div>
          <div class="grid grid-cols-2 gap-3">
            <div class="space-y-1.5">
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Tồn ban đầu</label>
              <input type="number" min="0" v-model.number="newItem.qty" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
            </div>
            <div class="space-y-1.5">
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Ngưỡng tối thiểu</label>
              <input type="number" min="0" v-model.number="newItem.min" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
            </div>
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="isAddOpen = false" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Hủy</button>
          <button @click="saveNewItem" class="px-5 py-2 rounded-lg bg-[#CC8033] text-white text-xs font-bold uppercase shadow-md hover:bg-[#B87029] transition-colors">Thêm SKU</button>
        </div>
      </div>
    </div>

    <!-- Toast -->
    <Transition name="toast">
      <div v-if="toastMsg" class="fixed bottom-6 right-6 z-[70] bg-[#2A231E] text-white px-5 py-3 rounded-xl shadow-2xl flex items-center gap-3 border border-[#CC8033]/30">
        <CheckCircle2 class="w-5 h-5 text-[#4A7C59]" />
        <span class="text-sm font-medium">{{ toastMsg }}</span>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Package, AlertTriangle, Clock, Search, Plus, Coffee, CupSoda, Milk,
  History, SlidersHorizontal, X, Layers, CheckCircle2, PackageOpen,
  Truck, ClipboardList, ChevronRight, ClipboardCheck
} from 'lucide-vue-next'

// ── Types ───────────────────────────────────────────────
interface Batch { code: string; importDate: string; qty: number; expiry: string }
interface Move { time: string; by: string; title: string; note: string; delta: number; kind: 'in' | 'out' | 'adjust' }
interface Item {
  sku: string; name: string; category: string; unit: string; qty: number; min: number;
  icon: 'coffee' | 'cup' | 'milk'; batches: Batch[]; history: Move[]
}

const items = ref<Item[]>([
  {
    sku: 'RAW-CF-001', name: 'Hạt cà phê Robusta', category: 'Nguyên liệu thô', unit: 'g', qty: 10450, min: 15000, icon: 'coffee',
    batches: [
      { code: 'BAT-2605-01', importDate: '10/05/2026', qty: 450, expiry: '30/06/2026' },
      { code: 'BAT-2605-09', importDate: '25/05/2026', qty: 5000, expiry: '25/11/2026' },
      { code: 'BAT-2606-02', importDate: '01/06/2026', qty: 5000, expiry: '01/12/2026' },
    ],
    history: [
      { time: 'Hôm nay, 15:30', by: 'Hệ thống', title: 'Xuất kho bán hàng', note: 'Tự động trừ kho qua POS', delta: -40, kind: 'out' },
      { time: '03/06/2026, 10:00', by: 'Nguyễn Văn A', title: 'Nhập kho từ phiếu INB-2406-003', note: 'NCC: Đại lý Cà phê Quận 1', delta: 5000, kind: 'in' },
      { time: '01/06/2026, 18:00', by: 'Trần Thị B', title: 'Điều chỉnh kiểm kê kho', note: 'Hao hụt tự nhiên', delta: -150, kind: 'adjust' },
    ],
  },
  {
    sku: 'SEM-TC-012', name: 'Trân châu đen nấu sẵn', category: 'Topping', unit: 'g', qty: 4200, min: 2000, icon: 'cup',
    batches: [{ code: 'BAT-2606-05', importDate: '02/06/2026', qty: 4200, expiry: '05/06/2026' }],
    history: [{ time: 'Hôm nay, 09:00', by: 'Bếp', title: 'Mẻ nấu topping', note: 'Trân châu sống → chín', delta: 4200, kind: 'in' }],
  },
  {
    sku: 'RAW-MK-005', name: 'Sữa đặc Ngôi sao Phương Nam', category: 'Nguyên liệu thô', unit: 'Lon', qty: 293, min: 50, icon: 'milk',
    batches: [{ code: 'BAT-2605-11', importDate: '20/05/2026', qty: 293, expiry: '20/05/2027' }],
    history: [{ time: '20/05/2026, 11:00', by: 'Nguyễn Văn A', title: 'Nhập kho', note: 'NCC: Vinamilk Q1', delta: 288, kind: 'in' }],
  },
  {
    sku: 'SUP-CUP-01', name: 'Ly giấy Takeaway 450ml', category: 'Vật tư', unit: 'Chiếc', qty: 0, min: 500, icon: 'coffee',
    batches: [],
    history: [{ time: 'Hôm qua, 20:00', by: 'Hệ thống', title: 'Xuất hết tồn', note: 'Bán mang về', delta: -120, kind: 'out' }],
  },
])

// ── Filters ─────────────────────────────────────────────
const search = ref('')
const typeFilter = ref('all')
const statusFilter = ref('all')

const stockState = (it: Item): 'ok' | 'low' | 'empty' => {
  if (it.qty <= 0) return 'empty'
  if (it.qty <= it.min) return 'low'
  return 'ok'
}
const lowStockCount = computed(() => items.value.filter(i => stockState(i) === 'low').length)
const emptyCount = computed(() => items.value.filter(i => stockState(i) === 'empty').length)

const filteredItems = computed(() => {
  const q = search.value.toLowerCase().trim()
  return items.value.filter(it => {
    const matchSearch = !q || it.name.toLowerCase().includes(q) || it.sku.toLowerCase().includes(q)
    const matchType = typeFilter.value === 'all' || it.category === typeFilter.value
    const matchStatus = statusFilter.value === 'all' || stockState(it) === statusFilter.value
    return matchSearch && matchType && matchStatus
  })
})

// ── Display helpers ─────────────────────────────────────
const formatNumber = (n: number) => (n || 0).toLocaleString('vi-VN')
const iconFor = (it: Item) => (it.icon === 'cup' ? CupSoda : it.icon === 'milk' ? Milk : Coffee)
const displayQty = (it: Item) => `${formatNumber(it.qty)}${it.unit}`
const stockColor = (it: Item) => {
  const s = stockState(it)
  return s === 'empty' ? 'text-red-600' : s === 'low' ? 'text-[#CC8033]' : 'text-[#4A7C59]'
}
const stockLabel = (it: Item) => {
  const s = stockState(it)
  if (s === 'empty') return 'Đã rỗng kho!'
  if (s === 'low') return 'Dưới ngưỡng tối thiểu'
  return `${it.batches.length} lô khả dụng`
}

// ── Toast ───────────────────────────────────────────────
const toastMsg = ref('')
let toastTimer: ReturnType<typeof setTimeout>
const toast = (msg: string) => {
  toastMsg.value = msg
  clearTimeout(toastTimer)
  toastTimer = setTimeout(() => (toastMsg.value = ''), 3000)
}

// ── Drawers ─────────────────────────────────────────────
const selected = ref<Item | null>(null)
const drawerMode = ref<'detail' | 'history'>('detail')
const openDrawer = (it: Item) => { selected.value = it; drawerMode.value = 'detail' }
const openHistory = (it: Item) => { selected.value = it; drawerMode.value = 'history' }
const closeDrawer = () => { selected.value = null }

// ── Adjust ──────────────────────────────────────────────
const adjustTarget = ref<Item | null>(null)
const adjustActual = ref<number | null>(null)
const adjustReason = ref('Hao hụt tự nhiên')
const adjustDiff = computed(() => adjustTarget.value && adjustActual.value !== null ? adjustActual.value - adjustTarget.value.qty : 0)
const openAdjust = (it: Item) => { adjustTarget.value = it; adjustActual.value = it.qty; adjustReason.value = 'Hao hụt tự nhiên' }
const confirmAdjust = () => {
  if (!adjustTarget.value || adjustActual.value === null) { toast('Nhập số lượng thực tế'); return }
  const diff = adjustDiff.value
  const it = adjustTarget.value
  it.history.unshift({ time: 'Vừa xong', by: 'Bạn', title: 'Điều chỉnh kho', note: adjustReason.value, delta: diff, kind: 'adjust' })
  it.qty = adjustActual.value
  toast(`Đã điều chỉnh ${it.name}: ${diff > 0 ? '+' : ''}${formatNumber(diff)} ${it.unit}`)
  adjustTarget.value = null
}

// ── Add SKU ─────────────────────────────────────────────
const isAddOpen = ref(false)
let skuCounter = 1
const newItem = ref<{ name: string; category: string; unit: string; qty: number; min: number }>({ name: '', category: 'Nguyên liệu thô', unit: 'g', qty: 0, min: 0 })
const openAdd = () => { newItem.value = { name: '', category: 'Nguyên liệu thô', unit: 'g', qty: 0, min: 0 }; isAddOpen.value = true }
const saveNewItem = () => {
  if (!newItem.value.name.trim()) { toast('Vui lòng nhập tên nguyên liệu'); return }
  const prefix = newItem.value.category === 'Vật tư' ? 'SUP' : newItem.value.category === 'Topping' ? 'SEM' : 'RAW'
  items.value.push({
    sku: `${prefix}-NEW-${String(skuCounter++).padStart(3, '0')}`,
    name: newItem.value.name, category: newItem.value.category, unit: newItem.value.unit || 'cái',
    qty: newItem.value.qty, min: newItem.value.min, icon: 'coffee', batches: [], history: [],
  })
  toast(`Đã thêm SKU ${newItem.value.name}`)
  isAddOpen.value = false
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
