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
          <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
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
              <th class="px-5 py-4 font-bold text-center">Hạn sử dụng</th>
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
                <div class="flex flex-col items-center">
                  <span class="text-xs font-semibold" :class="isExpired(item.expiryDate) && item.qty > 0 ? 'text-red-500 font-bold' : 'text-[#5C544E]'">
                    {{ formatExpiry(item.expiryDate) }}
                  </span>
                  <span v-if="isExpired(item.expiryDate) && item.qty > 0" class="text-[9px] font-black text-red-500 uppercase bg-red-50 border border-red-200 px-1.5 py-0.5 rounded mt-1 animate-pulse">
                    Hết Hạn!
                  </span>
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
    <div class="fixed top-0 right-0 h-full w-full max-w-[480px] bg-[#FDFBF7] shadow-2xl z-50 transform transition-transform duration-300 ease-in-out flex flex-col" :class="selected && drawerMode === 'detail' ? 'translate-x-0' : 'translate-x-full'">
      <template v-if="selected">
        <!-- HEADER CAO CẤP -->
        <div class="px-6 py-6 border-b border-[#EAE3D9] flex flex-col justify-between bg-gradient-to-br from-[#2A231E] via-[#2A231E] to-[#3D332A] text-white relative overflow-hidden">
          <div class="absolute inset-0 opacity-10 pointer-events-none">
            <svg class="w-full h-full" xmlns="http://www.w3.org/2000/svg">
              <defs><pattern id="grid-pattern-drawer" width="20" height="20" patternUnits="userSpaceOnUse"><path d="M 20 0 L 0 0 0 20" fill="none" stroke="currentColor" stroke-width="0.5"/></pattern></defs>
              <rect width="100%" height="100%" fill="url(#grid-pattern-drawer)" />
            </svg>
          </div>
          <div class="relative z-10 flex items-start justify-between">
            <div class="flex items-center gap-4">
              <div class="w-14 h-14 rounded-xl bg-white/10 border border-white/20 flex items-center justify-center backdrop-blur-md shadow-inner flex-shrink-0">
                <component :is="iconFor(selected)" class="w-7 h-7 text-yellow-400" stroke-width="1.5" />
              </div>
              <div>
                <h2 class="text-xl font-black tracking-tight text-white drop-shadow-sm">{{ selected.name }}</h2>
                <div class="flex items-center gap-2 mt-1">
                  <span class="px-2 py-0.5 rounded bg-white/10 text-white text-[10px] font-bold uppercase tracking-wider font-mono border border-white/10">SKU: {{ selected.sku }}</span>
                  <span class="px-2 py-0.5 rounded text-yellow-400 text-[10px] font-bold uppercase tracking-wider border border-yellow-400/30 bg-yellow-400/10">{{ selected.category }}</span>
                </div>
              </div>
            </div>
            <button @click="closeDrawer" class="p-2 text-white/60 hover:text-white hover:bg-white/10 rounded-lg transition-colors backdrop-blur-md"><X class="w-5 h-5" /></button>
          </div>
        </div>

        <div class="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar bg-[#FDFBF7]">
          
          <!-- THÔNG SỐ TỒN KHO -->
          <div class="grid grid-cols-3 gap-3">
            <div class="bg-white rounded-xl p-4 border border-[#EAE3D9] shadow-sm flex flex-col items-center justify-center text-center">
              <p class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider mb-1">Tồn kho</p>
              <p class="text-2xl font-black text-[#2A231E]">{{ formatNumber(selected.qty) }}<span class="text-sm font-medium text-[#8A8178] ml-1">{{ selected.unit }}</span></p>
            </div>
            <div class="bg-white rounded-xl p-4 border border-[#EAE3D9] shadow-sm flex flex-col items-center justify-center text-center">
              <p class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider mb-1">Ngưỡng tối thiểu</p>
              <p class="text-2xl font-black text-[#8A8178]">{{ formatNumber(selected.min) }}<span class="text-sm font-medium text-[#8A8178] ml-1">{{ selected.unit }}</span></p>
            </div>
            <div class="bg-white rounded-xl p-4 border border-[#EAE3D9] shadow-sm flex flex-col items-center justify-center text-center">
              <p class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider mb-1">Trạng thái</p>
              <div class="mt-1">
                <span v-if="stockState(selected) === 'low'" class="inline-flex items-center px-2 py-1 rounded bg-[#FFF9F2] text-[#CC8033] border border-[#E8C5A5]/60 text-[10px] font-bold uppercase tracking-wider shadow-sm"><AlertTriangle class="w-3 h-3 mr-1" /> Sắp hết</span>
                <span v-else-if="stockState(selected) === 'empty'" class="inline-flex items-center px-2 py-1 rounded bg-red-50 text-red-500 border border-red-200 text-[10px] font-bold uppercase tracking-wider shadow-sm"><AlertTriangle class="w-3 h-3 mr-1" /> Đã rỗng</span>
                <span v-else class="inline-flex items-center px-2 py-1 rounded bg-green-50 text-[#4A7C59] border border-green-200 text-[10px] font-bold uppercase tracking-wider shadow-sm"><CheckCircle2 class="w-3 h-3 mr-1" /> Đầy đủ</span>
              </div>
            </div>
          </div>

          <div class="h-px w-full bg-gradient-to-r from-transparent via-[#EAE3D9] to-transparent"></div>

          <!-- LÔ HÀNG -->
          <div>
            <div class="flex items-center justify-between mb-4">
              <h4 class="font-bold text-[#2A231E] flex items-center gap-2 text-sm uppercase tracking-wider"><Layers class="w-4 h-4 text-[#CC8033]" /> Chi tiết Lô hàng (FIFO)</h4>
              <span class="text-[10px] text-[#8A8178] font-bold uppercase bg-[#EAE3D9]/50 px-2 py-1 rounded-md">Vào trước, xuất trước</span>
            </div>

            <div v-if="selected.batches.length === 0" class="flex flex-col items-center justify-center py-10 bg-white border border-dashed border-[#EAE3D9] rounded-xl">
              <div class="w-12 h-12 bg-[#FDFBF7] rounded-full flex items-center justify-center mb-3"><PackageOpen class="w-6 h-6 text-[#8A8178] opacity-50" /></div>
              <p class="text-[#8A8178] text-sm font-medium">Chưa có lô hàng nào khả dụng.</p>
            </div>
            <div v-else class="space-y-3">
              <div v-for="(b, i) in selected.batches" :key="b.code" class="group border rounded-xl p-4 relative overflow-hidden transition-all hover:shadow-md"
                   :class="i === 0 ? 'border-[#E8C5A5] bg-gradient-to-br from-[#FFF9F2] to-white' : 'border-[#EAE3D9] bg-white'">
                <div v-if="i === 0" class="absolute top-0 right-0 bg-gradient-to-r from-[#CC8033] to-[#B87029] text-white text-[9px] font-bold px-3 py-1 rounded-bl-lg uppercase tracking-wider shadow-sm">Đang xuất kho</div>
                <div class="flex justify-between items-start mb-3">
                  <div>
                    <p class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider mb-0.5">Mã lô nhập</p>
                    <p class="text-sm font-bold text-[#2A231E] font-mono bg-[#EAE3D9]/30 px-2 py-0.5 rounded inline-block">{{ b.code }}</p>
                    <p class="text-[11px] text-[#8A8178] mt-2 flex items-center gap-1"><Truck class="w-3 h-3" /> Nhập: {{ b.importDate }}</p>
                  </div>
                  <div class="text-right">
                    <p class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider mb-0.5">Tồn khả dụng</p>
                    <p class="font-black text-lg" :class="i === 0 ? 'text-[#CC8033]' : 'text-[#2A231E]'">{{ formatNumber(b.qty) }}<span class="text-xs font-bold text-[#8A8178] ml-0.5">{{ selected.unit }}</span></p>
                  </div>
                </div>
                <div class="flex items-center justify-between mt-3 pt-3 border-t border-[#EAE3D9]/50">
                  <div class="flex items-center gap-1.5 text-[11px] font-bold px-2 py-1 rounded-md border"
                       :class="i === 0 ? 'text-[#C2410C] bg-orange-50 border-orange-200/50' : 'text-[#4A7C59] bg-green-50 border-green-200/50'">
                    <Clock v-if="i === 0" class="w-3 h-3" /><CheckCircle2 v-else class="w-3 h-3" />
                    HSD: {{ b.expiry }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="p-5 border-t border-[#EAE3D9] bg-white flex justify-end gap-3 shadow-[0_-4px_10px_rgba(0,0,0,0.02)] relative z-20">
          <router-link to="/suppliers" class="px-5 py-2.5 rounded-lg border-2 border-[#EAE3D9] text-[#2A231E] text-xs font-bold uppercase tracking-wider hover:bg-[#FDFBF7] transition-colors flex items-center gap-2">
            <Truck class="w-4 h-4" /> Nhập hàng thêm
          </router-link>
          <button @click="openAdjust(selected)" class="px-5 py-2.5 rounded-lg bg-gradient-to-r from-[#2A231E] to-[#3D332A] text-yellow-400 text-xs font-bold uppercase tracking-wider hover:from-black hover:to-[#2A231E] transition-colors shadow-md flex items-center gap-2">
            <ClipboardCheck class="w-4 h-4" /> Điều chỉnh
          </button>
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
              <select v-model="newItem.category" @change="handleCategoryChange" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none cursor-pointer">
                <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
                <option value="ADD_NEW" class="font-bold text-[#CC8033]">+ Thêm loại mới...</option>
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
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Hạn sử dụng</label>
            <input type="date" v-model="newItem.expiryDate" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" />
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="isAddOpen = false" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Hủy</button>
          <button @click="saveNewItem" class="px-5 py-2 rounded-lg bg-[#CC8033] text-white text-xs font-bold uppercase shadow-md hover:bg-[#B87029] transition-colors">Thêm SKU</button>
        </div>
      </div>
    </div>

    <!-- ===================================================================== -->
    <!-- MODAL: Thêm Phân Loại Nguyên Liệu -->
    <!-- ===================================================================== -->
    <div v-if="isAddCategoryOpen" class="fixed inset-0 bg-[#2A231E]/60 backdrop-blur-sm z-[70] flex justify-center items-center p-4" @click.self="cancelAddCategory">
      <div class="bg-white rounded-xl shadow-2xl w-full max-w-sm overflow-hidden animate-in zoom-in-95 duration-200">
        <div class="px-5 py-4 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <h2 class="text-base font-bold text-[#2A231E]">Thêm phân loại mới</h2>
          <button @click="cancelAddCategory" class="p-1 text-[#8A8178] hover:text-red-500 rounded-md"><X class="w-4 h-4" /></button>
        </div>
        <div class="p-5 space-y-4">
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178]">Tên phân loại <span class="text-red-500">*</span></label>
            <input v-model="newCategoryName" placeholder="VD: Bao bì, Đồ nhựa..." class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none focus:border-[#CC8033]" @keyup.enter="saveAddCategory" />
          </div>
        </div>
        <div class="p-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-2">
          <button @click="cancelAddCategory" class="px-4 py-2 rounded-lg text-[#5C544E] text-xs font-bold uppercase hover:bg-[#EAE3D9]/50 transition-colors">Hủy</button>
          <button @click="saveAddCategory" class="px-5 py-2 rounded-lg bg-[#CC8033] text-white text-xs font-bold uppercase shadow-md hover:bg-[#B87029] transition-colors">Thêm</button>
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
import { ref, computed, onMounted, watch } from 'vue'
import {
  Package, AlertTriangle, Clock, Search, Plus, Coffee, CupSoda, Milk,
  History, SlidersHorizontal, X, Layers, CheckCircle2, PackageOpen,
  Truck, ClipboardList, ChevronRight, ClipboardCheck
} from 'lucide-vue-next'
import { materialsApi, type MaterialItem } from '@/services/materials'

// ── Categories ──────────────────────────────────────────
const categories = ref<string[]>(JSON.parse(localStorage.getItem('materialCategories') || '["Nguyên liệu thô", "Bán thành phẩm / Topping", "Vật tư"]'))
watch(categories, (val) => { localStorage.setItem('materialCategories', JSON.stringify(val)) }, { deep: true })

const isAddCategoryOpen = ref(false)
const newCategoryName = ref('')

const handleCategoryChange = (e: Event) => {
  const target = e.target as HTMLSelectElement;
  if (target.value === 'ADD_NEW') {
    newCategoryName.value = '';
    isAddCategoryOpen.value = true;
  }
}

const saveAddCategory = () => {
  const name = newCategoryName.value.trim();
  if (name !== '') {
    if (!categories.value.includes(name)) {
      categories.value.push(name);
    }
    newItem.value.category = name;
  } else {
    newItem.value.category = categories.value[0];
  }
  isAddCategoryOpen.value = false;
}

const cancelAddCategory = () => {
  newItem.value.category = categories.value[0];
  isAddCategoryOpen.value = false;
}

// ── Types ───────────────────────────────────────────────
interface Batch { code: string; importDate: string; qty: number; expiry: string }
interface Move { time: string; by: string; title: string; note: string; delta: number; kind: 'in' | 'out' | 'adjust' }
interface Item {
  sku: string; name: string; category: string; unit: string; qty: number; min: number;
  icon: 'coffee' | 'cup' | 'milk'; batches: Batch[]; history: Move[]; originalId: number;
  expiryDate: string | null;
}

const items = ref<Item[]>([])
const lowStockCount = ref(0)
const emptyCount = ref(0)

// ── Filters ─────────────────────────────────────────────
const search = ref('')
const typeFilter = ref('all')
const statusFilter = ref('all')

const fetchData = async () => {
  try {
    const [listRes, summaryRes] = await Promise.all([
      materialsApi.list(search.value, typeFilter.value, statusFilter.value),
      materialsApi.summary()
    ]);
    
    items.value = listRes.map((r: MaterialItem) => ({
      originalId: r.maNguyenLieu,
      sku: r.maVach_SKU || `SKU-${r.maNguyenLieu}`,
      name: r.tenNguyenLieu,
      category: r.phanLoai,
      unit: r.donViTinh,
      qty: r.soLuongTon,
      min: r.mucTonToiThieu || 0,
      icon: r.phanLoai === 'Bán thành phẩm / Topping' ? 'cup' : (r.phanLoai === 'Vật tư' ? 'coffee' : 'milk'),
      batches: [],
      history: [],
      expiryDate: r.ngayHetHan
    }));
    
    lowStockCount.value = summaryRes.sapHet;
    emptyCount.value = summaryRes.daHet;
  } catch (err) {
    console.error('Lỗi khi lấy dữ liệu từ cơ sở dữ liệu:', err);
    toast('Lỗi kết nối máy chủ!');
  }
}

onMounted(fetchData)

let searchTimer: ReturnType<typeof setTimeout>
watch([search, typeFilter, statusFilter], () => {
  clearTimeout(searchTimer)
  searchTimer = setTimeout(fetchData, 300)
})

const stockState = (it: Item): 'ok' | 'low' | 'empty' => {
  if (it.qty <= 0) return 'empty'
  if (it.qty <= it.min) return 'low'
  return 'ok'
}

const filteredItems = computed(() => items.value)

// ── Display helpers ─────────────────────────────────────
const formatNumber = (n: number) => (n || 0).toLocaleString('vi-VN')
const iconFor = (it: Item) => (it.icon === 'cup' ? CupSoda : it.icon === 'milk' ? Milk : Coffee)
const displayQty = (it: Item) => `${formatNumber(it.qty)}${it.unit}`
const isExpired = (expiryDate: string | null) => {
  if (!expiryDate) return false
  const today = new Date()
  today.setHours(0,0,0,0)
  const exp = new Date(expiryDate)
  exp.setHours(0,0,0,0)
  return exp < today
}
const formatExpiry = (dStr: string | null) => {
  if (!dStr) return 'Không có hạn'
  return new Date(dStr).toLocaleDateString('vi-VN')
}
const stockColor = (it: Item) => {
  const s = stockState(it)
  return s === 'empty' ? 'text-red-600' : s === 'low' ? 'text-[#CC8033]' : 'text-[#4A7C59]'
}
const stockLabel = (it: Item) => {
  const s = stockState(it)
  if (s === 'empty') return 'Đã rỗng kho!'
  if (s === 'low') return 'Dưới ngưỡng tối thiểu'
  return `Đầy đủ`
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
const confirmAdjust = async () => {
  if (!adjustTarget.value || adjustActual.value === null) { toast('Nhập số lượng thực tế'); return }
  try {
    await materialsApi.adjust(adjustTarget.value.originalId, adjustActual.value, adjustReason.value)
    toast(`Đã điều chỉnh kho thành công`)
    adjustTarget.value = null
    fetchData()
  } catch (err) {
    toast('Lỗi khi điều chỉnh')
  }
}

// ── Add SKU ─────────────────────────────────────────────
const isAddOpen = ref(false)
const newItem = ref<{ name: string; category: string; unit: string; qty: number; min: number; expiryDate: string }>({ name: '', category: 'Nguyên liệu thô', unit: 'g', qty: 0, min: 0, expiryDate: '' })
const openAdd = () => { newItem.value = { name: '', category: 'Nguyên liệu thô', unit: 'g', qty: 0, min: 0, expiryDate: '' }; isAddOpen.value = true }
const saveNewItem = async () => {
  if (!newItem.value.name.trim()) { toast('Vui lòng nhập tên nguyên liệu'); return }
  try {
    const res = await materialsApi.create({
      tenNguyenLieu: newItem.value.name,
      phanLoai: newItem.value.category,
      donViTinh: newItem.value.unit || 'cái',
      mucTonToiThieu: newItem.value.min,
      ngayHetHan: newItem.value.expiryDate || null
    })
    
    // Nếu có tồn ban đầu, gọi API điều chỉnh kho ngay lập tức
    if (newItem.value.qty > 0) {
      await materialsApi.adjust(res.maNguyenLieu, newItem.value.qty, 'Tồn ban đầu hệ thống')
    }
    
    toast(`Đã thêm SKU ${newItem.value.name}`)
    isAddOpen.value = false
    fetchData()
  } catch (err: any) {
    toast(err.response?.data?.message || 'Lỗi khi tạo SKU')
  }
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
