<template>
  <div class="h-full flex font-premium-sans text-[#2A231E] -m-6 overflow-hidden" style="height:calc(100vh - 3.5rem)">

    <!-- LEFT: Menu -->
    <div class="flex flex-col w-[58%] border-r border-[#EAE3D9] bg-[#FDFBF7]">

      <!-- Banner Thông báo Yêu cầu hỗ trợ tại bàn (Realtime Support Call Alert) -->
      <div v-if="activeServiceRequests.length > 0" class="mx-5 mt-4 p-3.5 rounded-2xl bg-amber-500/15 border-2 border-amber-500/50 text-amber-950 flex flex-col gap-2 shadow-lg animate-pulse shrink-0">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-2.5">
            <div class="w-9 h-9 rounded-xl bg-amber-500 text-white flex items-center justify-center font-bold text-lg shrink-0 shadow-md">
              <BellRing class="w-5 h-5 animate-bounce" />
            </div>
            <div>
              <h4 class="font-extrabold text-sm text-[#2A231E]">YÊU CẦU HỖ TRỢ TẠI BÀN ({{ activeServiceRequests.length }})</h4>
              <p class="text-xs text-[#5C544E]">Khách hàng vừa bấm gọi nhân viên hỗ trợ từ menu QR</p>
            </div>
          </div>
        </div>
        <div class="space-y-2 mt-1 max-h-48 overflow-y-auto pr-1 custom-scrollbar">
          <div v-for="req in activeServiceRequests" :key="req.id" class="p-2.5 rounded-xl bg-white border border-amber-300 flex items-center justify-between gap-3 shadow-xs">
            <div class="flex items-center gap-2">
              <span class="px-2.5 py-1 rounded-lg bg-[#CC8033] text-white text-xs font-black uppercase tracking-wider">{{ req.tenBan }}</span>
              <span class="text-xs font-bold text-[#2A231E]">{{ req.ghiChu || 'Cần hỗ trợ nhân viên phục vụ' }}</span>
              <span class="text-[10px] text-[#8A8178] font-medium">({{ formatReqTime(req.thoiGianTao) }})</span>
            </div>
            <button @click="resolveRequest(req.id)" class="px-3 py-1.5 rounded-xl bg-emerald-600 hover:bg-emerald-700 text-white text-xs font-bold transition-all shadow-sm active:scale-95 flex items-center gap-1 shrink-0 cursor-pointer">
              <CheckCircle class="w-3.5 h-3.5" /> Đã hỗ trợ
            </button>
          </div>
        </div>
      </div>

      <div class="px-5 pt-5 pb-3 space-y-3 border-b border-[#EAE3D9] bg-white">
        <div class="flex items-center gap-3">
          <div class="relative flex-1">
            <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-[#C5BEB8]" />
            <input v-model="search" placeholder="Tìm món nhanh..."
              class="w-full pl-10 pr-4 py-2.5 border border-[#EAE3D9] rounded-xl text-sm bg-white focus:border-[#CC8033] outline-none font-medium" />
          </div>
          <div class="text-right shrink-0">
            <p class="text-[10px] text-[#8A8178] font-bold uppercase tracking-widest">Tổng đơn</p>
            <p class="text-lg font-premium-serif font-bold text-[#CC8033]">{{ formatVND(cartTotal) }}</p>
          </div>
        </div>
        <div class="flex gap-2 overflow-x-auto pb-1 scrollbar-hide">
          <button v-for="cat in catFilters" :key="cat" @click="activeCat = cat"
            :class="activeCat===cat ? 'bg-gradient-to-r from-[#CC8033] to-[#A6611F] text-white shadow-md shadow-[#CC8033]/20' : 'bg-[#F5F2ED] text-[#5C544E] hover:bg-[#EAE3D9]'"
            class="shrink-0 px-4 py-1.5 rounded-full text-xs font-bold uppercase tracking-wider transition-all">
            {{ cat === 'all' ? 'Tất cả' : cat }}
          </button>
        </div>
      </div>
      <div class="flex-1 overflow-y-auto p-4">
        <p v-if="loadingMenu" class="text-sm text-[#8A8178]">Đang tải thực đơn...</p>
        <p v-else-if="menu.length===0" class="text-sm text-[#8A8178]">Chưa có món nào đang bán. Hãy thêm sản phẩm ở mục Thực đơn.</p>
        <div v-else class="grid grid-cols-2 sm:grid-cols-3 xl:grid-cols-4 gap-3">
          <button v-for="item in filteredMenu" :key="item.maSanPham" @click="!isPosItemOutOfStock(item) && handleItemClick(item)"
            class="group bg-white rounded-2xl border border-[#EAE3D9] overflow-hidden hover:shadow-xl hover:shadow-[#CC8033]/10 hover:-translate-y-1 hover:border-[#CC8033]/40 transition-all duration-200 text-left relative"
            :class="isPosItemOutOfStock(item) ? 'opacity-50 grayscale cursor-not-allowed pointer-events-none' : ''">
            
            <div class="relative h-28 overflow-hidden bg-[#F5F2ED]">
              <div v-if="isPosItemOutOfStock(item)" class="absolute inset-0 bg-black/40 backdrop-blur-[1px] z-20 flex flex-col items-center justify-center p-2 text-center">
                <span class="bg-red-600 text-white text-[10px] font-black px-2.5 py-1 rounded-md shadow-lg uppercase tracking-widest border border-red-400">TẠM HẾT</span>
                <span class="text-[9px] text-white/90 font-medium mt-1">Bếp đã báo hết</span>
              </div>
              <img v-if="item.hinhAnh" :src="item.hinhAnh" :alt="item.tenSanPham" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              <div v-else class="w-full h-full flex items-center justify-center text-[#C5BEB8]"><Coffee class="w-8 h-8" /></div>
              <div class="absolute inset-0 bg-gradient-to-t from-black/15 to-transparent"></div>
              <div v-if="cartQty(item.maSanPham)>0" class="absolute top-2 left-2 min-w-[20px] h-5 px-1.5 rounded-full bg-[#CC8033] text-white text-[10px] font-bold flex items-center justify-center shadow-md ring-2 ring-white z-10">{{ cartQty(item.maSanPham) }}</div>
              <!-- COMBO badge -->
              <span v-if="item.kieuMon === 'Combo'" class="absolute top-2 right-2 px-2 py-0.5 rounded-md bg-gradient-to-r from-purple-600 to-indigo-600 text-white text-[9px] font-bold uppercase tracking-widest shadow-lg z-10 flex items-center gap-1">
                <Layers class="w-2.5 h-2.5" stroke-width="3" /> Combo
              </span>
              <div v-else class="absolute bottom-2 right-2 w-7 h-7 rounded-full bg-white/90 backdrop-blur text-[#CC8033] flex items-center justify-center shadow-md opacity-0 group-hover:opacity-100 translate-y-1 group-hover:translate-y-0 transition-all z-10">
                <Plus class="w-4 h-4" stroke-width="2.5" />
              </div>
            </div>
            <div class="p-2.5">
              <p class="text-xs font-bold text-[#2A231E] leading-snug truncate">{{ item.tenSanPham }}</p>
              <p v-if="item.kieuMon === 'Combo' && item.moTa" class="text-[10px] text-[#8A8178] font-medium truncate mt-0.5" :title="item.moTa">{{ item.moTa }}</p>
              <p class="text-sm font-premium-serif font-bold text-[#CC8033] mt-1">{{ formatVND(item.giaBan) }}</p>
            </div>
          </button>
        </div>
      </div>
    </div>

    <!-- RIGHT: Cart -->
    <div class="flex flex-col w-[42%] bg-white">
      <div class="px-4 py-3 border-b border-[#EAE3D9] bg-[#FDFBF7] space-y-2.5">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-2">
            <div class="w-7 h-7 rounded-lg bg-gradient-to-br from-[#CC8033] to-[#8A4F1A] flex items-center justify-center shadow-sm">
              <ShoppingCart class="w-3.5 h-3.5 text-white" stroke-width="2.5" />
            </div>
            <div>
              <h3 class="text-xs font-bold text-[#2A231E] leading-none">Đơn hàng</h3>
              <p class="text-[9px] text-[#8A8178] font-medium mt-0.5">{{ cart.length }} loại · {{ cartTotalQty }} phần</p>
            </div>
          </div>
          <button v-if="cart.length>0" @click="clearCart"
            class="flex items-center gap-1 text-[10px] font-bold text-red-400 hover:bg-red-50 px-2 py-1 rounded-lg transition-colors">
            <Trash2 class="w-3 h-3" /> Xóa
          </button>
        </div>
        <!-- Order type -->
        <div class="flex gap-1 p-1 bg-[#F0EDE9] rounded-2xl">
          <button @click="orderType='dine-in'"
            :class="orderType==='dine-in' ? 'bg-white text-[#CC8033] shadow-sm ring-1 ring-[#CC8033]/15' : 'text-[#8A8178] hover:text-[#5C544E]'"
            class="flex-1 flex items-center justify-center gap-1.5 py-2.5 rounded-xl text-xs font-bold transition-all">
            <Store class="w-4 h-4" stroke-width="2.2" /> Tại quán
          </button>
          <button @click="orderType='takeaway'; selectedTableId=null"
            :class="orderType==='takeaway' ? 'bg-white text-[#CC8033] shadow-sm ring-1 ring-[#CC8033]/15' : 'text-[#8A8178] hover:text-[#5C544E]'"
            class="flex-1 flex items-center justify-center gap-1.5 py-2.5 rounded-xl text-xs font-bold transition-all">
            <ShoppingBag class="w-4 h-4" stroke-width="2.2" /> Mang về
          </button>
        </div>
        <!-- Table selector (lấy từ trang Bàn & QR) -->
        <div v-if="orderType==='dine-in'">
          <div class="flex items-center justify-between mb-1.5">
            <p class="text-[9px] uppercase tracking-widest font-bold text-[#8A8178]">Chọn bàn</p>
            <span class="text-[9px] font-bold text-emerald-600">{{ banTrong }} bàn trống</span>
          </div>
          <!-- Chip khu vực -->
          <div v-if="posZones.length > 1" class="flex gap-1.5 flex-wrap mb-2">
            <button @click="posZoneFilter = 'all'"
              :class="posZoneFilter==='all' ? 'bg-[#2A231E] text-white border-[#2A231E]' : 'bg-white text-[#5C544E] border-[#EAE3D9] hover:border-[#CC8033]'"
              class="px-2.5 py-1 rounded-full border text-[10px] font-bold transition-colors">Tất cả</button>
            <button v-for="z in posZones" :key="z.maKhuVuc" @click="posZoneFilter = z.maKhuVuc"
              :class="posZoneFilter===z.maKhuVuc ? 'bg-[#2A231E] text-white border-[#2A231E]' : 'bg-white text-[#5C544E] border-[#EAE3D9] hover:border-[#CC8033]'"
              class="px-2.5 py-1 rounded-full border text-[10px] font-bold transition-colors whitespace-nowrap">{{ z.tenKhuVuc }}</button>
          </div>
          <div class="grid grid-cols-5 gap-1.5 max-h-[120px] overflow-y-auto pr-1">
            <button v-for="t in tablesInZone" :key="t.maBan"
              :disabled="t.trangThai === 'BaoTri'"
              @click="selectedTableId = selectedTableId===t.maBan ? null : t.maBan"
              :title="tableStatusMeta[t.trangThai]?.label + (t.maPinSession ? ` - Mã PIN: ${t.maPinSession}` : '') + (t.trangThai==='CoKhach' ? ' (Có thể chọn để gọi thêm món)' : '')"
              :class="selectedTableId===t.maBan
                ? 'bg-[#CC8033] border-[#CC8033] text-white shadow-md scale-[1.02]'
                : t.trangThai==='CoKhach'
                  ? 'bg-[#FFF8EE] border-[#F5E0C3] text-[#CC8033] hover:border-[#CC8033] shadow-xs cursor-pointer'
                  : t.trangThai==='Trong'
                    ? 'bg-white border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033] hover:text-[#CC8033] cursor-pointer'
                    : 'bg-[#F5F2ED] border-[#EAE3D9] text-[#C5BEB8] cursor-not-allowed opacity-60'"
              class="relative h-[46px] rounded-xl border flex flex-col items-center justify-center gap-0.5 transition-all disabled:cursor-not-allowed p-1">
              <span class="text-xs font-bold leading-none">{{ t.tenBan }}</span>
              <span class="flex items-center gap-1 text-[8px] font-semibold leading-none">
                <span class="w-1 h-1 rounded-full" :class="selectedTableId===t.maBan ? 'bg-white' : tableStatusMeta[t.trangThai]?.dot"></span>
                {{ tableStatusMeta[t.trangThai]?.label }}
              </span>
              <span v-if="t.trangThai === 'CoKhach' && t.maPinSession" class="text-[8px] font-black text-[#CC8033] bg-white px-1 rounded border border-amber-300 leading-none shadow-2xs">
                PIN: {{ t.maPinSession }}
              </span>
            </button>
            <span v-if="tablesInZone.length===0" class="col-span-5 text-xs text-[#8A8178] py-2 text-center">Không có bàn trong khu vực này.</span>
          </div>
          <p class="text-[9px] text-[#8A8178] mt-1 font-medium">💡 Bàn "Có khách" vẫn chọn được để gọi thêm món tại quầy. Bàn bảo trì sẽ không chọn được.</p>
        </div>
      </div>

      <!-- Cart list -->
      <div class="flex-1 overflow-y-auto px-4 py-3 space-y-2">
        <div v-if="cart.length===0" class="h-full flex flex-col items-center justify-center py-12">
          <div class="w-16 h-16 rounded-2xl bg-[#F5F2ED] flex items-center justify-center mb-4">
            <ShoppingCart class="w-7 h-7 text-[#C5BEB8]" stroke-width="1.5" />
          </div>
          <p class="text-sm font-bold text-[#C5BEB8]">Giỏ hàng trống</p>
          <p class="text-xs text-[#D5CEC8] font-medium mt-1">Chọn món từ thực đơn bên trái</p>
        </div>
        <div v-for="item in cart" :key="item.cartId" class="flex items-start gap-3 p-3 bg-[#F9F8F6] border border-[#EFEAE3] rounded-2xl group hover:border-[#CC8033]/30 transition-colors">
          <div class="w-11 h-11 rounded-xl overflow-hidden bg-[#F0EDE9] border border-[#EAE3D9] shrink-0">
            <img v-if="item.image" :src="item.image" :alt="item.name" class="w-full h-full object-cover" />
            <div v-else class="w-full h-full flex items-center justify-center text-[#C5BEB8]"><Coffee class="w-5 h-5" /></div>
          </div>
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-1.5">
              <p class="text-sm font-bold text-[#2A231E] truncate">{{ item.name }}</p>
              <button v-if="item.optionText !== '[Combo]'" @click="openEditModal(item)" title="Chỉnh sửa ly (Size, Topping, Đường/Đá)"
                class="text-[#CC8033] hover:bg-[#CC8033]/15 p-1 rounded-md transition-colors shrink-0 flex items-center gap-1 text-[10px] font-bold">
                <Pencil class="w-3 h-3" />
                <span>Sửa</span>
              </button>
            </div>
            <p v-if="item.optionText" class="text-[10px] text-[#8A8178] font-medium mt-0.5 leading-tight">{{ item.optionText }}</p>
            <div v-if="item.toppings.length" class="flex items-center gap-1 mt-1">
              <img v-for="t in item.toppings" :key="t.maSanPham" :src="t.hinhAnh || ''" :alt="t.ten" :title="t.ten + ' ×' + t.qty"
                class="w-5 h-5 rounded-full object-cover border border-[#EAE3D9]" />
            </div>
            <p class="text-xs text-[#CC8033] font-semibold mt-1">{{ formatVND(item.unitPrice) }}</p>
          </div>
          <div class="flex items-center gap-1 shrink-0">
            <button @click="updateQty(item.cartId,-1)" class="w-7 h-7 rounded-lg bg-white border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] hover:border-[#CC8033] transition-colors font-bold text-sm">−</button>
            <span class="w-6 text-center text-sm font-bold">{{ item.qty }}</span>
            <button @click="updateQty(item.cartId,1)" class="w-7 h-7 rounded-lg bg-white border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] hover:border-[#CC8033] transition-colors font-bold text-sm">+</button>
          </div>
          <p class="w-20 text-right text-sm font-bold text-[#2A231E] shrink-0">{{ formatVND(item.unitPrice*item.qty) }}</p>
          <button @click="removeItem(item.cartId)" class="opacity-0 group-hover:opacity-100 text-red-400 p-1 transition-all">
            <X class="w-3.5 h-3.5" />
          </button>
        </div>
      </div>

      <!-- Footer -->
      <div class="px-4 py-3 border-t border-[#EAE3D9] space-y-3">
        <div class="relative">
          <MessageSquare class="w-3.5 h-3.5 absolute left-3 top-3 text-[#C5BEB8]" />
          <textarea v-model="note" rows="2" placeholder="Ghi chú đơn..."
            class="w-full pl-8 pr-3 py-2.5 border border-[#EAE3D9] rounded-xl text-xs font-medium resize-none focus:border-[#CC8033] outline-none"></textarea>
        </div>
        
        <label class="flex items-center gap-2 cursor-pointer bg-purple-500/5 border border-purple-500/10 p-2.5 rounded-xl transition-colors select-none hover:bg-purple-500/10" :class="isPriority ? 'bg-purple-500/15 border-purple-500/30 shadow-inner' : ''">
          <input type="checkbox" v-model="isPriority" class="w-4 h-4 accent-purple-500 rounded cursor-pointer" />
          <span class="text-xs font-bold text-purple-700 flex items-center gap-1"><Zap class="w-3.5 h-3.5" /> Đánh dấu KHẨN CẤP (Ưu tiên Bếp)</span>
        </label>

        <div class="flex justify-between items-center px-3.5 py-2.5 rounded-2xl bg-gradient-to-r from-[#FDF7EF] to-[#F9F1E6] border border-[#F0E4D2]">
          <span class="text-sm font-bold text-[#5C544E]">Tổng cộng</span>
          <span class="text-xl font-premium-serif font-bold text-[#CC8033]">{{ formatVND(cartTotal) }}</span>
        </div>
        <p v-if="posError" class="text-xs font-semibold text-red-600">{{ posError }}</p>
        <button @click="openPay" :disabled="!canCheckout"
          class="w-full py-3.5 rounded-2xl font-bold text-sm transition-all disabled:opacity-40 flex flex-col items-center justify-center gap-0.5"
          :class="canCheckout ? 'bg-gradient-to-r from-[#CC8033] to-[#8A4F1A] text-white shadow-lg shadow-[#CC8033]/30 hover:-translate-y-0.5 hover:shadow-xl' : 'bg-[#F5F2ED] text-[#C5BEB8]'">
          <span class="flex items-center gap-2">
            <CheckCircle class="w-4 h-4" stroke-width="2.5" />
            Thanh toán · {{ formatVND(cartTotal) }}
          </span>
          <span v-if="orderType==='dine-in' && !selectedTableId" class="text-[10px] opacity-60 font-medium">Vui lòng chọn bàn trước</span>
          <span v-else class="text-[10px] opacity-70 font-medium">{{ orderType==='takeaway' ? '🛍️ Mang về' : '🪑 ' + (selectedTable?.tenBan || '') }}</span>
        </button>
      </div>
    </div>

    <!-- OPTIONS MODAL -->
    <Transition name="modal-fade">
      <div v-if="optionsOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="optionsOpen=false"></div>
        <div class="relative w-full max-w-lg bg-[#FDFBF7] rounded-2xl shadow-2xl flex flex-col max-h-[88vh] overflow-hidden">
          <div class="relative h-40 shrink-0 bg-[#F5F2ED]">
            <img v-if="selectedItem?.hinhAnh" :src="selectedItem.hinhAnh" class="w-full h-full object-cover" />
            <div v-else class="w-full h-full flex items-center justify-center text-[#C5BEB8]"><Coffee class="w-12 h-12" /></div>
            <button @click="optionsOpen=false" class="absolute top-3 right-3 w-9 h-9 rounded-full bg-white/40 backdrop-blur-md border border-white/40 flex items-center justify-center text-white hover:bg-white/60 transition-colors">
              <X class="w-4 h-4" stroke-width="2.5" />
            </button>
          </div>
          <div class="flex-1 overflow-y-auto p-6 space-y-6">
            <div>
              <h2 class="font-premium-serif text-2xl font-bold text-[#2A231E]">{{ selectedItem?.tenSanPham }}</h2>
              <p class="text-xl font-bold text-[#CC8033] mt-2">{{ formatVND(unitPricePreview) }}</p>
            </div>

            <!-- Size (từ DB) -->
            <div v-if="selectedItem?.kichCos.length" class="space-y-3">
              <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Kích cỡ</h3>
              <div class="grid grid-cols-2 gap-3">
                <button type="button" @click="selSizeId = null"
                  :class="selSizeId===null ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white'"
                  class="flex items-center gap-2 p-3 rounded-xl border-2 text-left">
                  <span class="text-sm font-bold">Mặc định <span class="text-xs font-medium text-[#8A8178]">{{ formatVND(selectedItem?.giaBan || 0) }}</span></span>
                </button>
                <button v-for="s in selectedItem.kichCos" :key="s.maKichCo" type="button" @click="selSizeId = s.maKichCo"
                  :class="selSizeId===s.maKichCo ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white'"
                  class="flex items-center gap-2 p-3 rounded-xl border-2 text-left">
                  <span class="text-sm font-bold">{{ s.tenKichCo }} <span class="text-xs font-medium" :class="s.giaCongThem>0?'text-[#CC8033]':'text-[#8A8178]'">{{ s.giaCongThem>0 ? '+'+formatVND(s.giaCongThem) : '' }}</span></span>
                </button>
              </div>
            </div>

            <!-- Đường & Đá -->
            <div v-if="selectedItem?.kieuMon !== 'MonKem' && selectedItem?.tenDanhMuc !== 'Bánh'" class="grid grid-cols-2 gap-4">
              <div class="space-y-2">
                <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Lượng đường</h3>
                <div class="flex flex-wrap gap-1.5">
                  <button v-for="l in ['0%','50%','100%']" :key="l" type="button" @click="selSugar = l"
                    :class="selSugar===l ? 'bg-[#CC8033] border-[#CC8033] text-white' : 'bg-white border-[#EAE3D9] text-[#5C544E]'"
                    class="px-3 py-1.5 rounded-lg border text-xs font-bold transition-all">{{ l }}</button>
                </div>
              </div>
              <div class="space-y-2">
                <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Lượng đá</h3>
                <div class="flex flex-wrap gap-1.5">
                  <button v-for="l in ['0%','50%','100%']" :key="l" type="button" @click="selIce = l"
                    :class="selIce===l ? 'bg-[#CC8033] border-[#CC8033] text-white' : 'bg-white border-[#EAE3D9] text-[#5C544E]'"
                    class="px-3 py-1.5 rounded-lg border text-xs font-bold transition-all">{{ l }}</button>
                </div>
              </div>
            </div>

            <!-- Topping -->
            <div v-if="selectedItem?.kieuMon !== 'MonKem' && selectedItem?.tenDanhMuc !== 'Bánh' && toppingList.length" class="space-y-3">
              <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Topping</h3>
              <div class="grid grid-cols-3 gap-3">
                <div v-for="t in toppingList" :key="t.maSanPham"
                  :class="(selToppings[t.maSanPham]||0)>0 ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white'"
                  class="flex flex-col items-center p-2.5 rounded-xl border-2 transition-all">
                  <div class="w-full aspect-square rounded-lg overflow-hidden bg-[#F5F2ED] mb-2 cursor-pointer" @click="updTopping(t.maSanPham,1)">
                    <img v-if="t.hinhAnh" :src="t.hinhAnh" :alt="t.tenSanPham" class="w-full h-full object-cover" />
                    <div v-else class="w-full h-full flex items-center justify-center text-2xl">🧋</div>
                  </div>
                  <p class="text-[10px] font-bold text-[#2A231E] text-center leading-tight">{{ t.tenSanPham }}</p>
                  <p class="text-[10px] text-[#CC8033] font-bold">+{{ formatVND(t.giaBan) }}</p>
                  <div v-if="(selToppings[t.maSanPham]||0)>0" class="flex items-center gap-1 mt-1.5">
                    <button @click="updTopping(t.maSanPham,-1)" class="w-6 h-6 rounded-full bg-white border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] text-sm font-bold">−</button>
                    <span class="text-xs font-bold text-[#CC8033] w-4 text-center">{{ selToppings[t.maSanPham] }}</span>
                    <button @click="updTopping(t.maSanPham,1)" class="w-6 h-6 rounded-full bg-[#CC8033] flex items-center justify-center text-white text-sm font-bold">+</button>
                  </div>
                  <button v-else @click="updTopping(t.maSanPham,1)" class="w-full mt-1.5 py-1 rounded-lg bg-[#F5F2ED] text-[#8A8178] text-[9px] font-bold uppercase tracking-wider hover:bg-[#EAE3D9] transition-colors">Thêm</button>
                </div>
              </div>
            </div>

            <!-- Ghi chú món -->
            <div class="space-y-2">
              <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Ghi chú</h3>
              <textarea v-model="itemNote" rows="2" placeholder="Ví dụ: ít béo, không đường..."
                class="w-full p-3 rounded-xl border border-[#EAE3D9] focus:border-[#CC8033] outline-none text-sm font-medium resize-none bg-white"></textarea>
            </div>
          </div>
          <div class="p-5 bg-white border-t border-[#EAE3D9] flex items-center gap-3 shrink-0">
            <div class="flex items-center border border-[#EAE3D9] rounded-xl overflow-hidden bg-[#F9F8F6]">
              <button @click="selQty>1&&selQty--" class="px-3 py-2.5 text-[#8A8178] hover:bg-[#EAE3D9] font-bold text-base">−</button>
              <span class="px-4 text-sm font-bold text-[#2A231E]">{{ selQty }}</span>
              <button @click="selQty++" class="px-3 py-2.5 text-[#8A8178] hover:bg-[#EAE3D9] font-bold text-base">+</button>
            </div>
            <button @click="confirmAdd"
              class="flex-1 py-3 rounded-xl bg-[#CC8033] hover:bg-[#B3702C] text-white text-sm font-bold flex items-center justify-center gap-2 shadow-md transition-all hover:-translate-y-0.5">
              <Plus class="w-4 h-4" stroke-width="2.5" />
              Thêm vào đơn · {{ formatVND(unitPricePreview * selQty) }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Modal thanh toán -->
    <Transition name="modal-fade">
      <div v-if="payOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="closePayModal"></div>
        <div class="relative w-full max-w-md bg-[#FDFBF7] rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[92vh]">
          <div class="p-5 border-b border-[#EAE3D9] flex items-center justify-between shrink-0">
            <div>
              <h2 class="font-premium-serif text-xl font-bold text-[#2A231E]">Thanh toán</h2>
              <p class="text-xs text-[#8A8178] font-medium flex items-center gap-2">
                <span>{{ orderType==='takeaway' ? 'Mang về' : (selectedTable?.tenBan || '') }} · {{ cartTotalQty }} phần</span>
                <span v-if="orderType==='dine-in' && selectedTable?.maPinSession" class="px-2 py-0.5 rounded-md bg-amber-500/15 border border-amber-500/30 text-[#CC8033] font-bold text-[10px] flex items-center gap-1">
                  <KeyRound class="w-3 h-3" /> PIN Bàn: {{ selectedTable.maPinSession }}
                </span>
              </p>
            </div>
            <button @click="closePayModal" class="w-9 h-9 rounded-full bg-[#F5F2ED] flex items-center justify-center text-[#8A8178] hover:bg-[#EAE3D9]"><X class="w-4 h-4"/></button>
          </div>

          <!-- BƯỚC 1: Chọn phương thức (chưa tạo đơn) -->
          <div v-if="payStep === 'select'" class="flex-1 overflow-y-auto p-5 space-y-4">
            <!-- Tích điểm & Khách hàng hội viên -->
            <div class="rounded-xl border border-[#EAE3D9] p-3 space-y-2.5 bg-[#FAF8F5]">
              <div class="flex items-center justify-between">
                <span class="text-[11px] uppercase tracking-wider font-bold text-[#2A231E] flex items-center gap-1.5">
                  <UserCheck class="w-4 h-4 text-[#CC8033]" />
                  <span>Tích điểm hội viên (Gmail)</span>
                </span>
                <span v-if="customerProfile" class="text-[10px] bg-[#CC8033]/15 text-[#CC8033] px-2 py-0.5 rounded-md font-bold">
                  {{ customerProfile.tier }} · {{ customerProfile.points }} điểm
                </span>
              </div>

              <!-- Chưa chọn khách: Nhập Gmail tìm kiếm -->
              <div v-if="!customerProfile" class="space-y-2">
                <div class="flex gap-2">
                  <div class="relative flex-1">
                    <input
                      v-model="customerEmailInput"
                      @focus="showSuggestions = true"
                      @blur="setTimeout(() => showSuggestions = false, 200)"
                      @keyup.enter="searchCustomerByEmail"
                      placeholder="Nhập tên Gmail (ví dụ: phamthanhtai16102006)..."
                      class="w-full px-3 py-2 border border-[#EAE3D9] rounded-lg text-xs bg-white focus:border-[#CC8033] outline-none"
                    />

                    <!-- Gợi ý danh sách tài khoản phù hợp -->
                    <div
                      v-if="showSuggestions && suggestedCustomers.length > 0"
                      class="absolute left-0 right-0 top-full mt-1 bg-white border border-[#EAE3D9] rounded-xl shadow-xl z-50 overflow-hidden max-h-56 overflow-y-auto divide-y divide-[#F0EDE9]"
                    >
                      <div class="px-3 py-1.5 bg-[#FAF8F5] text-[10px] uppercase font-bold text-[#8A8178] flex items-center justify-between">
                        <span>Gợi ý tài khoản hội viên</span>
                        <span class="text-[#CC8033]">{{ suggestedCustomers.length }} kết quả</span>
                      </div>
                      <button
                        v-for="cust in suggestedCustomers"
                        :key="cust.id"
                        @mousedown.prevent="selectSuggestedCustomer(cust)"
                        class="w-full px-3 py-2 text-left hover:bg-[#FDF7EF] transition-colors flex items-center justify-between gap-2 group cursor-pointer"
                      >
                        <div class="flex items-center gap-2 min-w-0">
                          <div class="w-7 h-7 rounded-full bg-[#CC8033]/20 text-[#CC8033] font-bold text-xs flex items-center justify-center shrink-0">
                            {{ cust.name.charAt(0).toUpperCase() }}
                          </div>
                          <div class="min-w-0">
                            <p class="text-xs font-bold text-[#2A231E] truncate group-hover:text-[#CC8033] transition-colors">{{ cust.name }}</p>
                            <p class="text-[10px] text-[#8A8178] truncate">{{ cust.email }}</p>
                          </div>
                        </div>
                        <span class="text-[10px] bg-[#FAF4EB] text-[#CC8033] px-2 py-0.5 rounded font-bold shrink-0">
                          {{ cust.tier }} · {{ cust.points }}đ
                        </span>
                      </button>
                    </div>
                  </div>

                  <button @click="searchCustomerByEmail" :disabled="!customerEmailInput.trim() || customerSearchLoading"
                    class="px-3 py-2 rounded-lg bg-[#CC8033] hover:bg-[#B3702C] text-white text-xs font-bold transition-all disabled:opacity-40 flex items-center gap-1 shrink-0">
                    <Search class="w-3.5 h-3.5" /> {{ customerSearchLoading ? '...' : 'Tìm' }}
                  </button>
                </div>

                <div v-if="customerSearchError" class="flex items-center justify-between bg-red-50 p-2 rounded-lg border border-red-100">
                  <span class="text-[11px] text-red-600 font-medium">{{ customerSearchError }}</span>
                  <button @click="registerNewCustomerFast" class="text-[10px] bg-red-600 text-white px-2 py-1 rounded font-bold hover:bg-red-700">
                    + Đăng ký ngay
                  </button>
                </div>
              </div>

              <!-- Đã tìm thấy khách hàng -->
              <div v-else class="p-2.5 bg-white rounded-lg border border-[#EAE3D9] space-y-2">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-xs font-bold text-[#2A231E] flex items-center gap-1">
                      <span>{{ customerProfile.name }}</span>
                      <span class="text-[10px] text-[#8A8178] font-normal">({{ customerProfile.email }})</span>
                    </p>
                    <p class="text-[10px] text-emerald-600 font-semibold mt-0.5">
                      ✓ Đơn hàng sẽ tự động tích điểm cho khách sau khi thanh toán
                    </p>
                  </div>
                  <button @click="clearCustomerProfile" class="text-[11px] text-red-500 font-bold hover:bg-red-50 px-2 py-1 rounded-lg border border-red-200 transition-colors flex items-center gap-1 shrink-0">
                    Đổi / Hủy chọn
                  </button>
                </div>

                <!-- Thẻ điểm tích lũy dự kiến -->
                <div class="p-2 bg-[#F0FDF4] border border-emerald-200 rounded-lg flex items-center justify-between">
                  <span class="text-xs font-bold text-emerald-800 flex items-center gap-1.5">
                    <Sparkles class="w-4 h-4 text-emerald-600 shrink-0" />
                    Tích điểm dự kiến cho đơn này:
                  </span>
                  <span class="text-xs font-black text-emerald-700 bg-white px-2 py-0.5 rounded border border-emerald-200 shadow-xs">
                    +{{ estimatedEarnedPoints }} điểm
                  </span>
                </div>

                <!-- Banner Ưu đãi Hạng Thành viên (Mua N tặng 1) -->
                <div v-if="posTierRequiredDrinkCount > 0" class="pt-2 border-t border-dashed border-[#EAE3D9]">
                  <div v-if="posFreeDrinksEarned > 0" class="p-2.5 bg-emerald-50 border border-emerald-200 rounded-xl text-left flex items-center justify-between">
                    <div class="flex items-center gap-2">
                      <Crown class="w-4 h-4 text-emerald-600 shrink-0" />
                      <div class="text-xs font-bold text-emerald-900">
                        🎁 Đã tặng {{ posFreeDrinksEarned }} ly Hạng {{ customerProfile?.tier || customerProfile?.hangThanhVien }} (Miễn phí giá gốc -{{ formatVND(posTierFreeDrinkDiscount) }})
                      </div>
                    </div>
                  </div>
                  <div v-else-if="posDrinksProgressInCycle >= posTierRequiredDrinkCount" class="p-3 bg-gradient-to-r from-[#FFF3E6] to-[#FDF7EF] border-2 border-[#F2C99C] rounded-xl text-left space-y-2 shadow-xs">
                    <div class="flex items-center justify-between">
                      <span class="text-xs font-extrabold text-[#D97724] flex items-center gap-1.5">
                        <Crown class="w-4 h-4 text-[#CC8033]" />
                        🎁 ƯU ĐÃI HẠNG {{ (customerProfile?.tier || customerProfile?.hangThanhVien || 'ĐỒNG').toUpperCase() }}: MUA {{ posTierRequiredDrinkCount }} TẶNG 1 LY!
                      </span>
                      <span class="text-[10px] font-extrabold px-2 py-0.5 rounded-full bg-[#CC8033] text-white">
                        Đã đạt {{ posTotalDrinkQty }}/{{ posTierRequiredDrinkCount + 1 }} ly
                      </span>
                    </div>
                    <p class="text-xs font-bold text-[#2A231E]">
                      Khách hàng đã chọn đủ {{ posTierRequiredDrinkCount }} ly! Hãy chọn thêm 1 ly bất kỳ vào giỏ hàng để nhận <span class="text-emerald-700 font-extrabold">TẶNG MIỄN PHÍ GIÁ GỐC (0đ)</span>!
                    </p>
                    <div class="pt-1 flex justify-end">
                      <button @click="closePayModal" class="px-3.5 py-1.5 bg-[#CC8033] hover:bg-[#B3702C] text-white text-xs font-bold rounded-lg shadow-sm flex items-center gap-1.5 cursor-pointer">
                        <Plus class="w-3.5 h-3.5" /> Chọn 1 ly quà tặng ngay
                      </button>
                    </div>
                  </div>
                  <div v-else class="p-2 bg-[#FAF6F0] border border-[#EAE3D9] rounded-xl text-left text-xs text-[#8A8178] flex items-center justify-between">
                    <span class="flex items-center gap-1 font-semibold text-[#5C544E]">
                      <Crown class="w-3.5 h-3.5 text-[#CC8033]" /> Ưu đãi Hạng {{ customerProfile?.tier || customerProfile?.hangThanhVien }}: Mua {{ posTierRequiredDrinkCount }} tặng 1
                    </span>
                    <span class="text-[11px] font-bold text-[#CC8033]">
                      Còn thiếu {{ posTierRequiredDrinkCount + 1 - posDrinksProgressInCycle }} ly nữa
                    </span>
                  </div>
                </div>

                <!-- Đổi điểm thưởng (Nút gửi OTP + Nhập OTP) -->
                <div class="pt-2 border-t border-dashed border-[#EAE3D9] space-y-2">
                  <div class="flex items-center justify-between">
                    <span class="text-[11px] font-bold text-[#CC8033] flex items-center gap-1">
                      <Gift class="w-3.5 h-3.5" /> Đổi điểm thưởng tích lũy
                    </span>
                    <button v-if="!showRedeemSection && redeemedDiscount === 0" @click="openRedeemSection"
                      class="text-[10px] bg-[#CC8033] text-white px-2.5 py-1 rounded-md font-bold hover:bg-[#B3702C] cursor-pointer">
                      Đổi điểm ngay
                    </button>
                  </div>

                  <!-- Hiển thị gói quà đã đổi thành công với nút Bỏ chọn -->
                  <div v-if="redeemedDiscount > 0" class="p-2.5 bg-emerald-50 border border-emerald-200 rounded-lg flex items-center justify-between shadow-xs">
                    <div class="flex items-center gap-2">
                      <div class="w-7 h-7 rounded-full bg-emerald-100 text-emerald-700 flex items-center justify-center shrink-0">
                        <Gift class="w-4 h-4" />
                      </div>
                      <div>
                        <p class="text-xs font-bold text-emerald-900 flex items-center gap-1">
                          <span>🎁 Đã áp dụng: {{ redeemedRewardName || 'Free 1 topping' }}</span>
                        </p>
                        <p class="text-[10px] text-emerald-700 font-medium">
                          Đã giảm trừ <strong class="font-bold text-emerald-800">-{{ formatVND(redeemedDiscount) }}</strong> vào đơn hàng
                        </p>
                      </div>
                    </div>
                    <button
                      @click="cancelRedeemedReward"
                      class="px-2.5 py-1 text-[11px] font-bold text-red-600 hover:text-red-700 bg-white border border-red-200 rounded-md hover:bg-red-50 transition-colors shadow-xs cursor-pointer shrink-0"
                      title="Bỏ chọn gói quà này để dành"
                    >
                      ✕ Bỏ chọn
                    </button>
                  </div>

                  <!-- Màn hình Đổi điểm & OTP -->
                  <div v-else-if="showRedeemSection" class="p-2.5 bg-[#FAF6F0] rounded-lg border border-amber-200/80 space-y-2.5">
                    <div>
                      <div class="flex items-center justify-between mb-1">
                        <label class="text-[10px] font-bold text-[#8A8178]">Chọn gói đổi điểm:</label>
                        <span v-if="bestReward" class="text-[10px] font-bold text-[#CC8033] flex items-center gap-1">
                          <Sparkles class="w-3 h-3" /> Ưu đãi tốt nhất
                        </span>
                      </div>
                      <select v-model="selectedRewardId" class="w-full px-2.5 py-1.5 border border-[#EAE3D9] rounded-md text-xs font-semibold bg-white outline-none">
                        <option :value="null">-- Chọn gói đổi quà --</option>
                        <option v-for="r in rewardList" :key="r.id" :value="r.id" :disabled="customerProfile.points < r.points">
                          {{ bestReward?.id === r.id ? '⭐ [TỐT NHẤT] ' : '' }}{{ r.name }} (Cần {{ r.points }} điểm) {{ customerProfile.points < r.points ? '- Chưa đủ điểm' : '' }}
                        </option>
                      </select>

                      <!-- Thông báo gói tốt nhất -->
                      <div v-if="bestReward" class="mt-1.5 p-2 bg-[#FFF8EE] border border-[#F5E0C3] rounded-md text-[11px] text-[#2A231E] flex items-center justify-between">
                        <span class="flex items-center gap-1 font-bold text-[#CC8033]">
                          <Sparkles class="w-3.5 h-3.5 shrink-0" />
                          Đã tự động chọn gói ưu đãi tốt nhất cho khách!
                        </span>
                      </div>
                      <div v-else-if="customerProfile && customerProfile.points < (rewardList[0]?.points || 50)" class="mt-1.5 p-2 bg-gray-100 rounded-md text-[10px] text-gray-500 font-medium">
                        💡 Khách hàng cần tích thêm {{ (rewardList[0]?.points || 50) - customerProfile.points }} điểm để quy đổi quà đầu tiên.
                      </div>
                    </div>

                    <!-- Gửi OTP & Nhập OTP -->
                    <div v-if="selectedRewardId" class="space-y-2 pt-1 border-t border-dashed border-amber-200">
                      <div class="flex items-center justify-between">
                        <span class="text-[10px] text-[#8A8178] font-medium flex items-center gap-1">
                          <KeyRound class="w-3 h-3 text-[#CC8033]" /> Mã OTP gửi tới Gmail khách:
                        </span>
                        <button @click="sendCustomerOtp" :disabled="otpSending || otpSentCountDown > 0"
                          class="px-2 py-1 bg-[#2A231E] text-white text-[10px] font-bold rounded hover:bg-black disabled:opacity-50">
                          {{ otpSentCountDown > 0 ? `Gửi lại (${otpSentCountDown}s)` : (otpSending ? 'Đang gửi...' : '📲 Gửi mã OTP') }}
                        </button>
                      </div>

                      <div v-if="otpSent" class="flex gap-1.5">
                        <input v-model="otpInput" placeholder="Nhập 6 số OTP..." maxlength="6"
                          class="flex-1 px-3 py-1.5 border border-[#CC8033] rounded-md text-xs font-bold text-center tracking-widest bg-white outline-none" />
                        <button @click="confirmRedeemOtp" :disabled="!otpInput.trim() || redeemBusy"
                          class="px-3 py-1.5 bg-[#CC8033] text-white text-xs font-bold rounded-md hover:bg-[#B3702C] disabled:opacity-40">
                          {{ redeemBusy ? '...' : 'Xác nhận đổi' }}
                        </button>
                      </div>
                      <p v-if="otpMessage" class="text-[10px] font-semibold" :class="otpError ? 'text-red-500' : 'text-emerald-600'">
                        {{ otpMessage }}
                      </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Danh sách món trong đơn hàng -->
            <div class="rounded-xl border border-[#EAE3D9] p-3 space-y-2 bg-white shadow-xs">
              <div class="flex items-center justify-between text-xs font-bold text-[#2A231E]">
                <span class="flex items-center gap-1.5">
                  <Coffee class="w-4 h-4 text-[#CC8033]" />
                  Danh sách món trong đơn ({{ cartTotalQty }} phần)
                </span>
                <span class="text-[11px] text-[#8A8178] font-semibold">Tạm tính: {{ formatVND(cartTotal) }}</span>
              </div>

              <div class="space-y-1.5 max-h-36 overflow-y-auto pr-1 text-xs divide-y divide-[#F5F2ED]">
                <div v-for="line in cart" :key="line.id" class="pt-1.5 first:pt-0 flex items-center justify-between">
                  <div class="min-w-0 pr-2">
                    <p class="font-bold text-[#2A231E] truncate">
                      {{ line.tenSanPham || line.name }}
                      <span v-if="line.selectedSize" class="text-[10px] text-[#8A8178] font-normal">({{ line.selectedSize.tenKichCo }})</span>
                    </p>
                    <p v-if="(line.diemTichLuy || menu.find(m => m.maSanPham === line.maSanPham)?.diemTichLuy)" class="text-[9.5px] text-emerald-600 font-semibold">
                      Tích +{{ (line.diemTichLuy || menu.find(m => m.maSanPham === line.maSanPham)?.diemTichLuy || 0) * line.qty }} điểm
                    </p>
                  </div>
                  <div class="text-right shrink-0">
                    <span class="font-bold text-[#2A231E]">x{{ line.qty }}</span>
                    <span class="text-[#8A8178] ml-2 font-medium">{{ formatVND(line.unitPrice * line.qty) }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Khuyến mãi -->
            <div class="rounded-xl border border-[#EAE3D9] p-3 space-y-2">
              <div class="flex items-center gap-2">
                <input v-model="voucherCode" @keyup.enter="applyVoucher({ code: voucherCode })" placeholder="Nhập mã giảm giá..."
                  class="flex-1 px-3 py-2 border border-[#EAE3D9] rounded-lg text-sm focus:border-[#CC8033] outline-none uppercase" />
                <button @click="applyVoucher({ code: voucherCode })" :disabled="!voucherCode.trim() || promoBusy"
                  class="px-3 py-2 rounded-lg bg-[#2A231E] text-white text-xs font-bold disabled:opacity-40">Áp dụng</button>
              </div>
              <div v-if="activePromos.length" class="flex gap-1.5 flex-wrap">
                <button v-for="p in activePromos" :key="p.maKhuyenMai" @click="applyVoucher({ maKhuyenMai: p.maKhuyenMai })"
                  :class="appliedPromo?.maKhuyenMai===p.maKhuyenMai ? 'border-[#CC8033] bg-[#FDF7EF] text-[#CC8033]' : 'border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033]'"
                  class="px-2.5 py-1 rounded-lg border text-[11px] font-bold">{{ p.tenChuongTrinh }}</button>
              </div>
              <p v-if="voucherError" class="text-[11px] font-semibold text-red-600">{{ voucherError }}</p>
              <div v-if="appliedPromo" class="flex items-center justify-between text-xs">
                <span class="inline-flex items-center gap-1 text-emerald-700 font-semibold"><CheckCircle class="w-3.5 h-3.5" /> {{ appliedPromo.tenChuongTrinh }}</span>
                <button @click="clearPromo" class="text-[#8A8178] underline hover:text-[#2A231E]">Bỏ</button>
              </div>
            </div>

            <!-- Tổng tiền -->
            <div class="rounded-xl bg-gradient-to-r from-[#FDF7EF] to-[#F9F1E6] border border-[#F0E4D2] px-4 py-3 space-y-1">
              <div class="flex justify-between text-sm text-[#5C544E]"><span>Tạm tính</span><span class="font-semibold">{{ formatVND(cartTotal) }}</span></div>
              <div v-if="appliedPromo" class="flex justify-between text-sm text-emerald-700 font-semibold"><span>Mã giảm giá</span><span>− {{ formatVND(appliedPromo.tienGiam) }}</span></div>
              <div v-if="redeemedDiscount > 0" class="flex justify-between text-sm text-amber-700 font-semibold"><span>Đổi điểm thưởng</span><span>− {{ formatVND(redeemedDiscount) }}</span></div>
              <div v-if="posTierFreeDrinkDiscount > 0" class="flex justify-between text-sm text-emerald-700 font-bold"><span>Tặng ly Hạng {{ customerProfile?.tier || customerProfile?.hangThanhVien }} (Giá gốc)</span><span>− {{ formatVND(posTierFreeDrinkDiscount) }}</span></div>
              <div class="flex justify-between items-center pt-1.5 border-t border-[#F0E4D2]">
                <span class="text-sm font-bold text-[#5C544E]">Tổng cộng</span>
                <span class="text-2xl font-premium-serif font-bold text-[#CC8033]">{{ formatVND(posFinalTotal) }}</span>
              </div>
            </div>

            <!-- Chọn phương thức thanh toán -->
            <div class="grid grid-cols-3 gap-2">
              <button v-for="m in payMethods" :key="m.id" @click="payMethod = m.id; ckType = null"
                :class="payMethod===m.id ? 'border-[#CC8033] bg-[#FDF7EF] text-[#CC8033]' : 'border-[#EAE3D9] text-[#8A8178]'"
                class="flex flex-col items-center gap-1 py-3 rounded-xl border-2 transition-colors">
                <component :is="m.icon" class="w-5 h-5" />
                <span class="text-[11px] font-bold">{{ m.label }}</span>
              </button>
            </div>

            <!-- Tiền mặt: nhập số tiền -->
            <div v-if="payMethod==='TienMat'" class="space-y-2">
              <label class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Tiền khách đưa</label>
              <input v-model.number="cashReceived" type="number" placeholder="0"
                class="w-full px-3 py-2.5 border border-[#EAE3D9] rounded-xl text-base font-bold focus:border-[#CC8033] outline-none" />
              <div class="flex gap-1.5 flex-wrap">
                <button v-for="a in quickAmounts" :key="a" @click="cashReceived=a"
                  class="px-3 py-1.5 rounded-lg border border-[#EAE3D9] text-[11px] font-bold text-[#5C544E] hover:border-[#CC8033] hover:text-[#CC8033]">{{ (a/1000)+'k' }}</button>
                <button @click="cashReceived=posFinalTotal" class="px-3 py-1.5 rounded-lg border border-[#CC8033] text-[11px] font-bold text-[#CC8033]">Vừa đủ</button>
              </div>
              <div class="flex items-center justify-between px-3 py-2 rounded-xl" :class="change>0 ? 'bg-emerald-50 text-emerald-700' : 'bg-[#F5F2ED] text-[#8A8178]'">
                <span class="text-sm font-semibold">Tiền thối</span>
                <span class="text-lg font-bold">{{ formatVND(change) }}</span>
              </div>
            </div>

            <p v-if="posError" class="text-xs font-semibold text-red-600">{{ posError }}</p>

            <button @click="confirmPay" :disabled="!canPay || paying"
              class="w-full py-3.5 rounded-2xl font-bold text-sm bg-gradient-to-r from-[#CC8033] to-[#8A4F1A] text-white shadow-lg disabled:opacity-40 flex items-center justify-center gap-2">
              <CheckCircle class="w-4 h-4" />
              <span>{{ paying ? 'Đang tạo đơn...' : (payMethod === 'TienMat' ? 'Xác nhận thanh toán' : (payMethod === 'Momo' ? 'Tạo mã QR MoMo' : 'Tạo mã VietQR')) }}</span>
            </button>
          </div>

          <!-- BƯỚC 2: Hiển thị QR code MoMo / VietQR -->
          <div v-else-if="payStep === 'qr'" class="flex-1 overflow-y-auto p-5 space-y-4">
            <div class="text-center space-y-1">
              <p class="text-sm font-bold text-[#2A231E]">
                {{ payMethod === 'Momo' ? '🟣 Quét mã MoMo để thanh toán' : '🏦 Quét mã VietQR để chuyển khoản' }}
              </p>
              <p class="text-xs text-[#8A8178]">Sau khi thanh toán xong, hệ thống sẽ tự động xác nhận</p>
            </div>

            <!-- Số tiền cần thanh toán -->
            <div class="flex justify-between items-center px-4 py-2.5 rounded-xl bg-gradient-to-r from-[#FDF7EF] to-[#F9F1E6] border border-[#F0E4D2]">
              <span class="text-sm font-bold text-[#5C544E]">Số tiền</span>
              <span class="text-xl font-premium-serif font-bold text-[#CC8033]">{{ formatVND(finalTotal) }}</span>
            </div>

            <!-- QR Code -->
            <div class="flex flex-col items-center gap-3">
              <div v-if="qrLoading" class="w-52 h-52 rounded-2xl bg-[#F5F2ED] flex items-center justify-center">
                <div class="animate-spin w-8 h-8 border-2 border-[#CC8033] border-t-transparent rounded-full"></div>
              </div>
              <div v-else-if="qrError" class="w-52 h-52 rounded-2xl bg-red-50 flex flex-col items-center justify-center gap-2 p-4 text-center">
                <span class="text-2xl">⚠️</span>
                <p class="text-xs font-semibold text-red-600">{{ qrError }}</p>
                <button @click="retryQr" class="text-xs underline text-[#CC8033] font-bold">Thử lại</button>
              </div>
              <!-- QR cho MoMo: render EMVCo raw string bằng QrcodeVue -->
              <div v-else-if="qrRawString" class="w-52 h-52 rounded-2xl bg-white border-2 border-[#EAE3D9] flex items-center justify-center shadow-md p-3">
                <QrcodeVue :value="qrRawString" :size="180" level="H" render-as="svg" />
              </div>
              <!-- QR cho VietQR: dùng ảnh URL từ img.vietqr.io -->
              <div v-else-if="qrCodeUrl" class="w-52 h-52 rounded-2xl bg-white border-2 border-[#EAE3D9] overflow-hidden p-2 shadow-md">
                <img :src="qrCodeUrl" alt="QR Code VietQR" class="w-full h-full object-contain" />
              </div>

              <!-- Nút mở App MoMo -->
              <a v-if="payUrl && payMethod === 'Momo'" :href="payUrl" target="_blank"
                class="inline-flex items-center gap-2 px-4 py-2 rounded-xl bg-[#AE2070] text-white text-sm font-bold hover:opacity-90 transition-opacity shadow-md">
                <Smartphone class="w-4 h-4" /> Mở App MoMo thanh toán
              </a>
            </div>

            <!-- Trạng thái polling -->
            <div class="flex items-center justify-center gap-2 text-xs text-[#8A8178] font-medium">
              <div class="animate-pulse w-2 h-2 rounded-full bg-amber-400"></div>
              Đang chờ thanh toán... ({{ pollCount }}s)
            </div>

            <!-- Thông tin chuyển khoản (VietQR) -->
            <div v-if="payMethod === 'NganHang'" class="rounded-xl border border-[#EAE3D9] p-3 text-xs space-y-1 text-[#5C544E]">
              <p class="font-bold text-[10px] uppercase tracking-widest text-[#8A8178] mb-1">Thông tin chuyển khoản</p>
              <div class="flex justify-between"><span>Ngân hàng</span><span class="font-bold">MB Bank</span></div>
              <div class="flex justify-between"><span>Nội dung CK</span><span class="font-bold text-[#CC8033]">{{ storeInfoStore.tenQuan }} DH{{ createdOrderId }}</span></div>
            </div>

            <div class="grid grid-cols-2 gap-2 pt-1">
              <button @click="cancelQrAndBack" class="py-3 rounded-2xl font-bold text-sm border-2 border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033] transition-colors">
                ← Quay lại
              </button>
              <button @click="manualConfirm" :disabled="paying"
                class="py-3 rounded-2xl font-bold text-sm bg-gradient-to-r from-[#CC8033] to-[#8A4F1A] text-white shadow-lg disabled:opacity-40 flex items-center justify-center gap-1.5">
                <CheckCircle class="w-4 h-4" /> {{ paying ? 'Đang xử lý...' : 'Xác nhận đã nhận tiền' }}
              </button>
            </div>
          </div>

          <!-- BƯỚC 3: Thành công -->
          <div v-else-if="payStep === 'success'" class="flex-1 p-6 flex flex-col items-center justify-center gap-4 text-center">
            <div class="w-20 h-20 rounded-full bg-emerald-100 flex items-center justify-center">
              <CheckCircle class="w-10 h-10 text-emerald-500" stroke-width="2" />
            </div>
            <div>
              <h3 class="text-xl font-premium-serif font-bold text-[#2A231E]">Thanh toán thành công!</h3>
              <p class="text-sm text-[#8A8178] mt-1">{{ payMethod === 'TienMat' ? 'Tiền thối: ' + formatVND(toastChange) : 'Hệ thống đã ghi nhận giao dịch' }}</p>
            </div>

            <!-- Mã PIN bàn để người sau quét QR nhập -->
            <div v-if="paySuccessPinCode" class="w-full p-3.5 bg-gradient-to-r from-amber-50 to-orange-50 border border-amber-200/80 rounded-xl text-[#2A231E] flex flex-col items-center gap-1.5 shadow-sm">
              <span class="text-[11px] font-bold text-[#8A8178] uppercase tracking-wider flex items-center gap-1">
                <KeyRound class="w-3.5 h-3.5 text-[#CC8033]" /> Mã PIN bàn (Người sau quét QR order):
              </span>
              <span class="text-3xl font-black tracking-[0.2em] text-[#CC8033] bg-white px-4 py-1 rounded-xl border border-amber-300 shadow-sm">
                {{ paySuccessPinCode }}
              </span>
              <span class="text-[11px] text-[#8A8178] text-center font-medium mt-0.5">
                Khách ngồi cùng bàn có thể quét QR dán trên bàn &amp; nhập mã 4 số này để tiếp tục đặt món.
              </span>

              <!-- Form nhập Gmail gửi Mã PIN -->
              <div class="w-full space-y-1.5 pt-2 border-t border-amber-200/60 mt-1">
                <label class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider block text-left">
                  📧 Gửi mã PIN 4 số này qua Gmail:
                </label>
                <div class="flex gap-2">
                  <input 
                    v-model="customerReceiptEmail" 
                    type="email" 
                    placeholder="Nhập địa chỉ Gmail nhận mã..." 
                    class="flex-1 px-3 py-2 bg-white border border-[#EAE3D9] rounded-xl text-xs font-semibold text-[#2A231E] focus:outline-none focus:border-[#CC8033]"
                    @keyup.enter="sendPinToEmail"
                  />
                  <button 
                    @click="sendPinToEmail" 
                    :disabled="sendingPinEmail || !customerReceiptEmail.trim()"
                    class="px-3.5 py-2 rounded-xl bg-[#CC8033] hover:bg-[#B36B25] text-white text-xs font-bold transition-all disabled:opacity-50 flex items-center gap-1 shrink-0 cursor-pointer shadow-xs"
                  >
                    <Send class="w-3.5 h-3.5" />
                    {{ sendingPinEmail ? 'Đang gửi...' : 'Gửi Gmail' }}
                  </button>
                </div>
                <p v-if="pinEmailStatus" class="text-[11px] font-bold text-emerald-600 text-left pt-0.5">
                  {{ pinEmailStatus }}
                </p>
              </div>
            </div>

            <button @click="closePayModal" class="w-full py-3.5 rounded-2xl font-bold text-sm bg-gradient-to-r from-[#CC8033] to-[#8A4F1A] text-white shadow-lg cursor-pointer">
              Đóng
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Toast -->
    <Transition name="toast">
      <div v-if="showToast" class="fixed bottom-6 right-6 z-50 flex items-center gap-3 bg-[#2A231E] text-white px-5 py-3.5 rounded-2xl shadow-2xl">
        <CheckCircle class="w-5 h-5 text-emerald-400" stroke-width="2.5" />
        <div>
          <p class="text-sm font-bold flex items-center gap-2">
            <span>Thanh toán thành công!</span>
            <span v-if="paySuccessPinCode" class="text-amber-400 font-extrabold text-xs">🔑 PIN: {{ paySuccessPinCode }}</span>
          </p>
          <p class="text-[10px] text-white/60 font-medium">
            <span v-if="isTakeawayResponse" class="text-[#CC8033] font-bold mr-1">Mang về - #{{ String(orderIdResponse).padStart(3, '0') }}</span>
            <span v-else-if="orderIdResponse" class="mr-1">Mã đơn: #{{ orderIdResponse }}</span>
            <span v-if="toastChange > 0">· Tiền thối: {{ formatVND(toastChange) }}</span>
            <span v-else>· Đã ghi nhận</span>
          </p>
        </div>
      </div>
    </Transition>

    <!-- Ready Toast -->
    <Transition name="toast">
      <div v-if="showReadyToast" class="fixed top-6 right-6 z-[200] flex items-center gap-3 bg-emerald-500 text-white px-5 py-4 rounded-2xl shadow-[0_10px_40px_rgba(16,185,129,0.3)] border border-emerald-400">
        <Bell class="w-6 h-6 animate-[bounce_1s_ease-in-out_infinite]" stroke-width="2.5" />
        <div>
          <p class="text-base font-bold tracking-wide">Đồ đã sẵn sàng!</p>
          <p class="text-xs font-medium text-emerald-50 mt-0.5">Vui lòng phục vụ <span class="font-bold bg-white/20 px-1.5 py-0.5 rounded">{{ readyTable }}</span></p>
        </div>
        <button @click="showReadyToast = false" class="w-8 h-8 flex items-center justify-center rounded-full bg-black/10 hover:bg-black/20 ml-2 transition-colors">
          <X class="w-4 h-4" />
        </button>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, onUnmounted } from 'vue'
import { Search, ShoppingCart, Trash2, X, MessageSquare, CheckCircle, Plus, Coffee, Store, ShoppingBag, Banknote, Smartphone, Wallet, Landmark, Zap, Bell, BellRing, Layers, UserCheck, Gift, KeyRound, Send, Sparkles, Crown, Pencil } from 'lucide-vue-next'
import QrcodeVue from 'qrcode.vue'
import { ordersApi, type MenuItem } from '@/services/orders'
import { tablesApi, type TableItem } from '@/services/tables'
import { promotionsApi, type Promotion, type ApplyResult } from '@/services/promotions'
import { paymentsApi } from '@/services/payments'
import { loyaltyApi } from '@/services/loyalty'
import { useOrderStore } from '@/stores/orders'
import { useAuthStore } from '@/stores/auth'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { auditLogsApi } from '@/services/auditLogs'

const orderStore = useOrderStore()
const authStore = useAuthStore()
const storeInfoStore = useStoreInfoStore()

const formatVND = (n: number) => (n || 0).toLocaleString('vi-VN') + 'đ'

// ── Service Requests (Gọi phục vụ tại bàn) ────────────────────
const activeServiceRequests = ref<any[]>([])

const fetchActiveServiceRequests = async () => {
  try {
    const data = await ordersApi.getActiveServiceRequests()
    activeServiceRequests.value = (data || []).filter((r: any) => r.loaiYeuCau !== 'CanBungNuoc' && r.loaiYeuCau !== 'GiaoDo')
  } catch (e) {}
}

const resolveRequest = async (id: string) => {
  try {
    await ordersApi.resolveServiceRequest(id)
    activeServiceRequests.value = activeServiceRequests.value.filter(r => r.id !== id)
    toast.success('Đã xác nhận hỗ trợ bàn!', 'Hoàn tất')
  } catch (e) {
    toast.error('Lỗi xử lý yêu cầu')
  }
}

const formatReqTime = (raw: string) => {
  if (!raw) return ''
  const d = new Date(raw)
  return isNaN(d.getTime()) ? '' : d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
}

let serviceReqInterval: number | null = null
const posChannel = typeof BroadcastChannel !== 'undefined' ? new BroadcastChannel('quanlycf_orders_sync') : null

// ── Dữ liệu thật ──────────────────────────────────────────────
const menu = ref<MenuItem[]>([])
const tables = ref<TableItem[]>([])
const loadingMenu = ref(false)
const posError = ref('')

const nowTime = ref(new Date())
let timeIntervalId: number | null = null
let menuSyncIntervalId: number | null = null

const isTimeSlotActive = (item: MenuItem) => {
  if (!item.apDungKhungGio || !item.gioBatDau || !item.gioKetThuc) return true

  const now = nowTime.value
  const currentMinutes = now.getHours() * 60 + now.getMinutes()

  const [startH, startM] = item.gioBatDau.split(':').map(Number)
  const [endH, endM] = item.gioKetThuc.split(':').map(Number)

  const startMinutes = startH * 60 + startM
  const endMinutes = endH * 60 + endM

  if (startMinutes <= endMinutes) {
    return currentMinutes >= startMinutes && currentMinutes <= endMinutes
  } else {
    // Khung giờ qua đêm (VD: 22:00 -> 06:00)
    return currentMinutes >= startMinutes || currentMinutes <= endMinutes
  }
}

const fetchMenuData = async () => {
  try {
    const [m, t] = await Promise.all([ordersApi.menu(true), tablesApi.list()])
    menu.value = m
    tables.value = t
  } catch (e) {
    if (!menu.value.length) posError.value = e instanceof Error ? e.message : 'Không tải được dữ liệu.'
  }
}

onMounted(async () => {
  loadingMenu.value = true
  await fetchMenuData()
  loadingMenu.value = false

  fetchActiveServiceRequests()
  serviceReqInterval = window.setInterval(() => {
    if (!document.hidden) {
      fetchActiveServiceRequests()
    }
  }, 2500)

  if (posChannel) {
    posChannel.onmessage = (e) => {
      if (e.data?.type === 'CALL_STAFF') {
        fetchActiveServiceRequests()
        toast.warning(`🔔 ${e.data.tenBan} ĐANG CẦN HỖ TRỢ!`, 'GỌI PHỤC VỤ')
      }
    }
  }

  // Tự động kiểm tra thời gian thực mỗi 5 giây
  timeIntervalId = window.setInterval(() => {
    nowTime.value = new Date()
  }, 5000)

  // Đồng bộ thực đơn ngầm từ Server mỗi 15 giây
  menuSyncIntervalId = window.setInterval(() => {
    fetchMenuData()
  }, 15000)
})

const search = ref('')
const activeCat = ref('all')
// Tách món nước và topping (Hiển thị đầy đủ tất cả món đang kinh doanh cho Thu ngân)
const drinks = computed(() => menu.value.filter(m => m.kieuMon !== 'Topping'))
const toppingList = computed(() => menu.value.filter(m => m.kieuMon === 'Topping'))
const catFilters = computed(() => {
  const cats = Array.from(new Set(drinks.value.map(m => m.tenDanhMuc).filter(Boolean) as string[]))
  return ['all', ...cats]
})
const isPosItemOutOfStock = (item: MenuItem) => {
  if (!item) return false
  const name = item.tenSanPham || ''
  const clean = name.replace(/\s*\([^)]*\)$/, '').trim()
  return orderStore.globalOutOfStock.has(name) || orderStore.globalOutOfStock.has(clean)
}

const filteredMenu = computed(() => drinks.value.filter(m =>
  (activeCat.value === 'all' || m.tenDanhMuc === activeCat.value) &&
  m.tenSanPham.toLowerCase().includes(search.value.toLowerCase())
))

// ── Giỏ hàng ──────────────────────────────────────────────────
interface CartTopping { maSanPham: number; ten: string; gia: number; qty: number; hinhAnh: string | null }
interface CartItem {
  cartId: number
  maSanPham: number
  tenSanPham?: string
  name: string
  image: string | null
  maKichCo: number | null
  unitPrice: number
  qty: number
  optionText: string
  ghiChuMon: string | null
  toppings: CartTopping[]
  diemTichLuy?: number
}
const cart = ref<CartItem[]>([])
let cartIdSeq = 0
const cartTotal = computed(() => cart.value.reduce((s, i) => s + i.unitPrice * i.qty, 0))
const cartTotalQty = computed(() => cart.value.reduce((s, i) => s + i.qty, 0))
const cartQty = (maSanPham: number) => cart.value.filter(i => i.maSanPham === maSanPham).reduce((s, i) => s + i.qty, 0)

// ── Quy tắc ưu đãi Hạng Thành Viên: Mua N ly tặng 1 ly ────────────────
const posTierRequiredCountMap: Record<string, number> = {
  'Đồng': 10,
  'Bronze': 10,
  'Member': 10,
  'Bạc': 7,
  'Silver': 7,
  'Vàng': 5,
  'Gold': 5,
  'Kim Cương': 3,
  'Diamond': 3
}

const posTierRequiredDrinkCount = computed(() => {
  const rawTier = customerProfile.value?.tier || customerProfile.value?.hangThanhVien || ''
  const tier = rawTier.trim()
  return posTierRequiredCountMap[tier] || (customerProfile.value ? 10 : 0)
})

const currentActiveTableOrder = computed(() => {
  if (selectedTable.value) {
    const active = orderStore.orders.find(o => o.table === selectedTable.value?.tenBan && o.status !== 'done' && o.status !== 'cancelled')
    return active || null
  }
  return null
})

const posTotalDrinkQty = computed(() => {
  if (currentActiveTableOrder.value) {
    return currentActiveTableOrder.value.items.reduce((sum, i) => sum + i.qty, 0)
  }
  return cart.value.reduce((sum, i) => sum + i.qty, 0)
})

const posFreeDrinksEarned = computed(() => {
  const req = posTierRequiredDrinkCount.value
  if (req <= 0) return 0
  return Math.floor(posTotalDrinkQty.value / (req + 1))
})

const posDrinksProgressInCycle = computed(() => {
  const req = posTierRequiredDrinkCount.value
  if (req <= 0) return 0
  return posTotalDrinkQty.value % (req + 1)
})

const posTierFreeDrinkDiscount = computed(() => {
  const freeCount = posFreeDrinksEarned.value
  if (freeCount <= 0) return 0

  const itemBasePrices: number[] = []

  if (currentActiveTableOrder.value) {
    currentActiveTableOrder.value.items.forEach(i => {
      for (let k = 0; k < i.qty; k++) {
        itemBasePrices.push(i.price)
      }
    })
  } else {
    cart.value.forEach(i => {
      const toppingTotal = (i.toppings || []).reduce((ts, t) => ts + t.gia * t.qty, 0)
      const drinkDef = menu.value.find(d => d.maSanPham === i.maSanPham)
      const baseP = drinkDef ? drinkDef.giaBan : Math.max(0, i.unitPrice - toppingTotal)
      for (let k = 0; k < i.qty; k++) {
        itemBasePrices.push(baseP)
      }
    })
  }

  itemBasePrices.sort((a, b) => b - a)
  let discount = 0
  for (let k = 0; k < Math.min(freeCount, itemBasePrices.length); k++) {
    discount += itemBasePrices[k]
  }
  return discount
})

const posTableSubTotal = computed(() => {
  if (currentActiveTableOrder.value) {
    return currentActiveTableOrder.value.items.reduce((sum, i) => sum + (i.price * i.qty), 0)
  }
  return cartTotal.value
})

const posFinalTotal = computed(() => {
  const sub = posTableSubTotal.value
  const redeemed = redeemedDiscount.value || 0
  const tierDis = posTierFreeDrinkDiscount.value
  return Math.max(0, sub - redeemed - tierDis)
})

const orderType = ref<'dine-in' | 'takeaway'>('dine-in')
const selectedTableId = ref<number | null>(null)
const selectedTable = computed(() => tables.value.find(t => t.maBan === selectedTableId.value) || null)

const tableStatusMeta: Record<string, { label: string; dot: string }> = {
  Trong:   { label: 'Trống',    dot: 'bg-emerald-500' },
  CoKhach: { label: 'Có khách', dot: 'bg-[#CC8033]' },
  BaoTri:  { label: 'Bảo trì',  dot: 'bg-gray-400' },
}
// Lọc bàn theo khu vực
const posZoneFilter = ref<number | 'all'>('all')
const posZones = computed(() => {
  const seen = new Map<number, string>()
  for (const t of tables.value) if (!seen.has(t.maKhuVuc)) seen.set(t.maKhuVuc, t.tenKhuVuc)
  return Array.from(seen, ([maKhuVuc, tenKhuVuc]) => ({ maKhuVuc, tenKhuVuc }))
})
const tablesInZone = computed(() =>
  posZoneFilter.value === 'all' ? tables.value : tables.value.filter(t => t.maKhuVuc === posZoneFilter.value))
const banTrong = computed(() => tablesInZone.value.filter(t => t.trangThai === 'Trong').length)
const note = ref('')
const canCheckout = computed(() => cart.value.length > 0 && (orderType.value === 'takeaway' || !!selectedTableId.value))

// ── Modal tuỳ chọn ────────────────────────────────────────────
const optionsOpen = ref(false)
const selectedItem = ref<MenuItem | null>(null)
const selSizeId = ref<number | null>(null)
const selSugar = ref('100%')
const selIce = ref('100%')
const itemNote = ref('')
const selQty = ref(1)
const selToppings = ref<Record<number, number>>({})

const selSize = computed(() => selectedItem.value?.kichCos.find(s => s.maKichCo === selSizeId.value) || null)
const toppingExtra = computed(() =>
  toppingList.value.reduce((s, t) => s + (selToppings.value[t.maSanPham] || 0) * t.giaBan, 0))
const unitPricePreview = computed(() =>
  (selectedItem.value?.giaBan || 0) + (selSize.value?.giaCongThem || 0) + toppingExtra.value)

function updTopping(maSanPham: number, delta: number) {
  const n = (selToppings.value[maSanPham] || 0) + delta
  if (n <= 0) delete selToppings.value[maSanPham]
  else selToppings.value[maSanPham] = n
}

// Combo → thêm thẳng vào giỏ; Món thường → mở modal tuỳ chọn
function handleItemClick(item: MenuItem) {
  if (item.kieuMon === 'Combo') {
    // Thêm combo trực tiếp vào giỏ hàng (không chọn size/đá/đường/topping)
    const existing = cart.value.find(c => c.maSanPham === item.maSanPham && c.optionText === '[Combo]')
    if (existing) {
      existing.qty++
    } else {
      cart.value.push({
        cartId: cartIdSeq++,
        maSanPham: item.maSanPham,
        name: item.tenSanPham,
        image: item.hinhAnh,
        maKichCo: null,
        unitPrice: item.giaBan,
        qty: 1,
        optionText: '[Combo]',
        ghiChuMon: item.moTa ? `[Combo] ${item.moTa}` : '[Combo]',
        toppings: [],
      })
    }
    return
  }
  openOptions(item)
}

const editingCartId = ref<number | null>(null)

function openOptions(item: MenuItem) {
  editingCartId.value = null
  selectedItem.value = item
  selSizeId.value = null
  selSugar.value = '100%'
  selIce.value = '100%'
  itemNote.value = ''
  selQty.value = 1
  selToppings.value = {}
  optionsOpen.value = true
}

function openEditModal(cartItem: CartItem) {
  const menuItem = menu.value.find(m => m.maSanPham === cartItem.maSanPham)
  if (!menuItem) return

  editingCartId.value = cartItem.cartId
  selectedItem.value = menuItem
  selSizeId.value = cartItem.maKichCo

  const opts = (cartItem.optionText || '').split(' · ')
  const sugarOpt = opts.find(o => o.startsWith('Đường '))
  const iceOpt = opts.find(o => o.startsWith('Đá '))

  selSugar.value = sugarOpt ? sugarOpt.replace('Đường ', '') : '100%'
  selIce.value = iceOpt ? iceOpt.replace('Đá ', '') : '100%'

  const noteOpts = opts.filter(o => 
    !menuItem.kichCos.some(s => s.tenKichCo === o) &&
    !o.startsWith('Đường ') &&
    !o.startsWith('Đá ') &&
    !cartItem.toppings.some(t => o.startsWith(t.ten))
  )
  itemNote.value = noteOpts.join(' · ')

  selQty.value = cartItem.qty

  const topsMap: Record<number, number> = {}
  cartItem.toppings.forEach(t => {
    topsMap[t.maSanPham] = t.qty
  })
  selToppings.value = topsMap

  optionsOpen.value = true
}

function confirmAdd() {
  if (!selectedItem.value) return
  const tops: CartTopping[] = toppingList.value
    .filter(t => (selToppings.value[t.maSanPham] || 0) > 0)
    .map(t => ({ maSanPham: t.maSanPham, ten: t.tenSanPham, gia: t.giaBan, qty: selToppings.value[t.maSanPham]!, hinhAnh: t.hinhAnh }))
  const opts: string[] = []
  if (selSize.value) opts.push(selSize.value.tenKichCo)
  for (const t of tops) opts.push(t.ten + (t.qty > 1 ? ' x' + t.qty : ''))
  if (selSugar.value !== '100%') opts.push('Đường ' + selSugar.value)
  if (selIce.value !== '100%') opts.push('Đá ' + selIce.value)
  if (itemNote.value.trim()) opts.push(itemNote.value.trim())
  const optionText = opts.join(' · ')

  if (editingCartId.value !== null) {
    const existing = cart.value.find(c => c.cartId === editingCartId.value)
    if (existing) {
      existing.maKichCo = selSizeId.value
      existing.unitPrice = unitPricePreview.value
      existing.qty = selQty.value
      existing.optionText = optionText
      existing.ghiChuMon = optionText || null
      existing.toppings = tops
    }
    editingCartId.value = null
  } else {
    cart.value.push({
      cartId: cartIdSeq++,
      maSanPham: selectedItem.value.maSanPham,
      tenSanPham: selectedItem.value.tenSanPham,
      name: selectedItem.value.tenSanPham,
      image: selectedItem.value.hinhAnh,
      maKichCo: selSizeId.value,
      unitPrice: unitPricePreview.value,
      qty: selQty.value,
      optionText,
      ghiChuMon: optionText || null,
      toppings: tops,
      diemTichLuy: selectedItem.value.diemTichLuy || 0
    })
  }
  optionsOpen.value = false
}

function updateQty(cartId: number, delta: number) {
  const item = cart.value.find(i => i.cartId === cartId)
  if (!item) return
  item.qty += delta
  if (item.qty <= 0) cart.value = cart.value.filter(i => i.cartId !== cartId)
}
function removeItem(cartId: number) { cart.value = cart.value.filter(i => i.cartId !== cartId) }
function clearCart() {
  cart.value = []
  note.value = ''
  clearCustomerProfile()
}

// ── Thanh toán ────────────────────────────────────────────────
const payOpen = ref(false)
const payMethod = ref<'TienMat' | 'Momo' | 'NganHang'>('TienMat')
const ckType = ref<'Momo' | 'NganHang' | null>(null)
const cashReceived = ref<number | null>(null)
const paying = ref(false)
const showToast = ref(false)
const toastChange = ref(0)
const orderIdResponse = ref<number | null>(null)
const isTakeawayResponse = ref(false)
const isPriority = ref(false)
const showReadyToast = ref(false)
const readyTable = ref('')
const quickAmounts = [50000, 100000, 200000, 500000]

watch(() => orderStore.posNotification, (newVal) => {
  if (newVal) {
     try {
       const ctx = new (window.AudioContext || (window as any).webkitAudioContext)()
       const osc = ctx.createOscillator()
       osc.frequency.value = 1000
       osc.connect(ctx.destination)
       osc.start()
       osc.stop(ctx.currentTime + 0.1)
     } catch(e) {}
     readyTable.value = newVal.table
     showReadyToast.value = true
     setTimeout(() => { showReadyToast.value = false }, 8000)
  }
})

// Luồng thanh toán multi-step
const payStep = ref<'select' | 'qr' | 'success'>('select')
const createdOrderId = ref<number | null>(null)
const paySuccessPinCode = ref<string | null>(null)
const qrCodeUrl = ref<string | null>(null)
const qrRawString = ref<string | null>(null)  // EMVCo raw string cho MoMo
const payUrl = ref<string | null>(null)
const qrLoading = ref(false)
const qrError = ref('')
const pollCount = ref(0)
let pollInterval: number | null = null

const payMethods: { id: 'TienMat' | 'Momo' | 'NganHang'; label: string; icon: unknown }[] = [
  { id: 'TienMat', label: 'Tiền mặt', icon: Banknote },
  { id: 'Momo', label: 'MoMo', icon: Wallet },
  { id: 'NganHang', label: 'VietQR', icon: Landmark },
]
const ckOptions: { id: 'Momo' | 'NganHang'; label: string; icon: unknown }[] = [
  { id: 'Momo', label: 'MoMo', icon: Wallet },
  { id: 'NganHang', label: 'Ngân hàng', icon: Landmark },
]

// ── Khách hàng Tích điểm & Đổi điểm ──
const customerEmailInput = ref('')
const customerProfile = ref<{ id: number; name: string; phone: string; email: string; tier: string; points: number } | null>(null)
const customerSearchLoading = ref(false)
const customerSearchError = ref('')
const allCustomers = ref<{ id: number; name: string; phone: string; email: string; tier: string; points: number }[]>([])
const showSuggestions = ref(false)

const suggestedCustomers = computed(() => {
  const q = customerEmailInput.value.trim().toLowerCase()
  if (!q) return []
  return allCustomers.value.filter(c => {
    const emailName = c.email ? c.email.toLowerCase().split('@')[0] : ''
    const fullEmail = (c.email || '').toLowerCase()
    const name = c.name.toLowerCase()
    const phone = c.phone || ''
    return name.includes(q) || fullEmail.includes(q) || emailName.includes(q) || phone.includes(q)
  }).slice(0, 5)
})

function selectSuggestedCustomer(cust: { id: number; name: string; phone: string; email: string; tier: string; points: number }) {
  customerProfile.value = cust
  customerEmailInput.value = cust.email || cust.name
  customerSearchError.value = ''
  showSuggestions.value = false
}

const showRedeemSection = ref(false)
const rewardList = ref<{ id: number; name: string; points: number; description?: string }[]>([])
const selectedRewardId = ref<number | null>(null)
const otpInput = ref('')
const otpSending = ref(false)
const otpSent = ref(false)
const otpSentCountDown = ref(0)
const otpMessage = ref('')
const otpError = ref(false)
const redeemBusy = ref(false)
const redeemedDiscount = ref(0)
const redeemedRewardName = ref('')
const redeemedRewardPoints = ref(0)
let otpCountTimer: number | null = null

function cancelRedeemedReward() {
  if (customerProfile.value && redeemedRewardPoints.value > 0) {
    customerProfile.value.points += redeemedRewardPoints.value
  }
  redeemedDiscount.value = 0
  redeemedRewardName.value = ''
  redeemedRewardPoints.value = 0
  showRedeemSection.value = false
  selectedRewardId.value = null
  otpSent.value = false
  otpInput.value = ''
  otpMessage.value = 'Đã hủy áp dụng gói đổi điểm cho đơn hàng này.'
  otpError.value = false
}

function formatEmailInput(val: string): string {
  let clean = val.trim().toLowerCase()
  if (!clean) return ''
  if (!clean.includes('@')) {
    clean += '@gmail.com'
  }
  return clean
}

async function searchCustomerByEmail() {
  const email = formatEmailInput(customerEmailInput.value)
  if (!email) return
  customerEmailInput.value = email
  customerSearchLoading.value = true
  customerSearchError.value = ''
  try {
    const res = await loyaltyApi.checkPublicEmail(email)
    customerProfile.value = res
  } catch (e: any) {
    customerProfile.value = null
    customerSearchError.value = e?.message || 'Chưa có thông tin khách hàng này.'
  } finally {
    customerSearchLoading.value = false
  }
}

async function registerNewCustomerFast() {
  const email = formatEmailInput(customerEmailInput.value)
  if (!email) return
  customerEmailInput.value = email
  customerSearchLoading.value = true
  customerSearchError.value = ''
  try {
    const rawName = email.split('@')[0]
    const name = rawName.charAt(0).toUpperCase() + rawName.slice(1)
    const res = await loyaltyApi.registerPublic({
      name,
      phone: '',
      email
    })
    customerProfile.value = res
    customerSearchError.value = ''
  } catch (e: any) {
    customerSearchError.value = e?.message || 'Không tạo được khách hàng mới.'
  } finally {
    customerSearchLoading.value = false
  }
}

function clearCustomerProfile() {
  customerProfile.value = null
  customerEmailInput.value = ''
  customerSearchError.value = ''
  showRedeemSection.value = false
  selectedRewardId.value = null
  otpSent.value = false
  otpInput.value = ''
  otpMessage.value = ''
  redeemedDiscount.value = 0
  redeemedRewardName.value = ''
  redeemedRewardPoints.value = 0
}

const estimatedEarnedPoints = computed(() => {
  let points = 0
  for (const item of cart.value) {
    const menuItem = menu.value.find(m => m.maSanPham === item.maSanPham)
    const customPts = item.diemTichLuy || menuItem?.diemTichLuy || 0
    if (customPts > 0) {
      points += customPts * item.qty
    } else {
      const itemPrice = item.unitPrice || 0
      points += Math.floor((itemPrice * item.qty) / 10000)
    }
  }
  if (points <= 0 && finalTotal.value > 0) {
    points = Math.floor(finalTotal.value / 10000)
  }
  return points
})

const bestReward = computed(() => {
  if (!customerProfile.value || rewardList.value.length === 0) return null
  const pts = customerProfile.value.points || 0
  const eligible = rewardList.value.filter(r => r.points <= pts)
  if (eligible.length === 0) return null
  eligible.sort((a, b) => b.points - a.points)
  return eligible[0]
})

function autoSelectBestReward() {
  if (bestReward.value) {
    selectedRewardId.value = bestReward.value.id
  } else if (rewardList.value.length > 0) {
    selectedRewardId.value = null
  }
}

async function openRedeemSection() {
  showRedeemSection.value = true
  try {
    const rewards = await loyaltyApi.getRewards()
    rewardList.value = rewards.map(r => ({ id: r.id, name: r.name, points: r.cost, description: r.description }))
  } catch {
    rewardList.value = [
      { id: 1, name: 'Free 1 topping', points: 100 },
      { id: 2, name: 'Giảm 20.000đ vào đơn hàng', points: 50 },
      { id: 3, name: 'Giảm 10% hóa đơn', points: 200 },
      { id: 4, name: 'Voucher 50.000đ', points: 500 }
    ]
  }
  autoSelectBestReward()
}

async function sendCustomerOtp() {
  if (!customerProfile.value) return
  otpSending.value = true
  otpMessage.value = ''
  otpError.value = false
  try {
    await loyaltyApi.sendOtp(customerProfile.value.id)
    otpSent.value = true
    otpMessage.value = 'Mã OTP 6 số đã được gửi tới Gmail của khách hàng!'
    otpSentCountDown.value = 60
    if (otpCountTimer) clearInterval(otpCountTimer)
    otpCountTimer = window.setInterval(() => {
      otpSentCountDown.value--
      if (otpSentCountDown.value <= 0 && otpCountTimer) clearInterval(otpCountTimer)
    }, 1000)
  } catch (e: any) {
    otpError.value = true
    otpMessage.value = e?.message || 'Lỗi gửi mã OTP.'
  } finally {
    otpSending.value = false
  }
}

async function confirmRedeemOtp() {
  if (!customerProfile.value || !selectedRewardId.value || !otpInput.value.trim()) return
  redeemBusy.value = true
  otpMessage.value = ''
  otpError.value = false
  try {
    const res = await loyaltyApi.redeem(customerProfile.value.id, selectedRewardId.value, otpInput.value.trim())
    customerProfile.value.points = res.points

    const reward = rewardList.value.find(r => r.id === selectedRewardId.value)
    if (reward) {
      redeemedRewardName.value = reward.name
      redeemedRewardPoints.value = reward.points
      const isToppingReward = reward.name.toLowerCase().includes('topping') || reward.points === 100
      if (isToppingReward) {
        let toppingPrice = 10000
        for (const item of cart.value) {
          if (item.toppings && item.toppings.length > 0) {
            toppingPrice = item.toppings[0].gia || 10000
            break
          }
          const menuItem = menu.value.find(m => m.maSanPham === item.maSanPham)
          if (menuItem?.kieuMon === 'Topping' && item.unitPrice > 0) {
            toppingPrice = item.unitPrice
            break
          }
        }
        redeemedDiscount.value = toppingPrice
      } else if (reward.points === 50) {
        redeemedDiscount.value = 20000
      } else if (reward.points === 200) {
        redeemedDiscount.value = Math.round(cartTotal.value * 0.1)
      } else if (reward.points === 350) {
        redeemedDiscount.value = 35000
      } else if (reward.points === 500) {
        redeemedDiscount.value = 50000
      } else {
        redeemedDiscount.value = 10000
      }
    }

    otpMessage.value = `✓ Đổi quà thành công! Đã trừ điểm (Số dư mới: ${res.points} điểm).`
    showRedeemSection.value = false
    otpInput.value = ''
  } catch (e: any) {
    otpError.value = true
    otpMessage.value = e?.message || 'Mã OTP không chính xác hoặc đã hết hạn.'
  } finally {
    redeemBusy.value = false
  }
}

// ── Khuyến mãi ──
const activePromos = ref<Promotion[]>([])
const voucherCode = ref('')
const appliedPromo = ref<ApplyResult | null>(null)
const voucherError = ref('')
const promoBusy = ref(false)
const finalTotal = computed(() => Math.max(0, cartTotal.value - (appliedPromo.value?.tienGiam || 0) - redeemedDiscount.value))

async function applyVoucher(opts: { maKhuyenMai?: number; code?: string }) {
  voucherError.value = ''
  promoBusy.value = true
  try {
    appliedPromo.value = await promotionsApi.preview(cartTotal.value, opts)
  } catch (e) {
    appliedPromo.value = null
    voucherError.value = e instanceof Error ? e.message : 'Mã không hợp lệ.'
  } finally { promoBusy.value = false }
}
function clearPromo() { appliedPromo.value = null; voucherCode.value = ''; voucherError.value = '' }

const change = computed(() => Math.max(0, (cashReceived.value || 0) - finalTotal.value))
const canPay = computed(() => {
  if (payMethod.value === 'TienMat') return (cashReceived.value || 0) >= finalTotal.value
  return true  // MoMo / VietQR: tạo đơn trước, hiển thị QR sau
})

function stopPolling() {
  if (pollInterval) { clearInterval(pollInterval); pollInterval = null }
}

async function refreshCustomerProfile() {
  if (!customerProfile.value) return
  try {
    let updated: any = null
    if (customerProfile.value.email) {
      updated = await loyaltyApi.checkPublicEmail(customerProfile.value.email)
    } else if (customerProfile.value.id) {
      const detail = await loyaltyApi.get(customerProfile.value.id)
      if (detail) {
        updated = {
          id: detail.id,
          name: detail.name,
          phone: detail.phone,
          email: detail.email || '',
          tier: detail.tier,
          points: detail.points
        }
      }
    }
    if (updated) {
      customerProfile.value = updated
      localStorage.setItem('brewCustomerProfile', JSON.stringify(updated))
    }
  } catch (e) {
    console.error('Không thể tự động cập nhật điểm khách hàng sau khi thanh toán:', e)
  }
}

function startPolling(maDonHang: number) {
  stopPolling()
  pollCount.value = 0
  pollInterval = window.setInterval(async () => {
    pollCount.value += 3
    try {
      const status = await paymentsApi.getStatus(maDonHang)
      if (status.daThanhToan) {
        stopPolling()
        payStep.value = 'success'
        clearCart()
        selectedTableId.value = null
        tables.value = await tablesApi.list()
        await refreshCustomerProfile()
      }
    } catch { /* bỏ qua lỗi poll */ }
  }, 3000)
}

const customerReceiptEmail = ref('')
const sendingPinEmail = ref(false)
const pinEmailStatus = ref('')

watch(customerProfile, (newVal) => {
  if (newVal?.email) {
    customerReceiptEmail.value = newVal.email
  }
}, { immediate: true })

async function sendPinToEmail() {
  const email = customerReceiptEmail.value.trim()
  if (!email) {
    toast.warning('Vui lòng nhập địa chỉ Gmail nhận mã PIN.', 'Thiếu thông tin')
    return
  }

  sendingPinEmail.value = true
  pinEmailStatus.value = ''
  try {
    const tbName = orderType.value === 'dine-in' ? tables.value.find(t => t.maBan === selectedTableId.value)?.tenBan || 'Bàn' : 'Mang về'
    const res = await ordersApi.sendEmailReceipt({
      email,
      maDonHang: orderIdResponse.value || undefined,
      tenBan: tbName,
      maPinSession: paySuccessPinCode.value
    })
    pinEmailStatus.value = res.message || 'Đã gửi mã PIN bàn qua Gmail thành công!'
    toast.success('Đã gửi mã PIN bàn qua Gmail!', 'Thành công')
  } catch (err: any) {
    toast.error(err.message || 'Không thể gửi Gmail. Vui lòng kiểm tra lại địa chỉ Gmail.')
  } finally {
    sendingPinEmail.value = false
  }
}

async function openPay() {
  if (!canCheckout.value) return
  await fetchMenuData()
  payMethod.value = 'TienMat'
  ckType.value = null
  cashReceived.value = null
  posError.value = ''
  payStep.value = 'select'
  qrCodeUrl.value = null
  qrRawString.value = null
  payUrl.value = null
  qrError.value = ''
  createdOrderId.value = null
  paySuccessPinCode.value = orderType.value === 'dine-in' ? selectedTable.value?.maPinSession || null : null
  clearPromo()
  payOpen.value = true
  try { if (activePromos.value.length === 0) activePromos.value = await promotionsApi.active() } catch { /* bỏ qua */ }
  try { allCustomers.value = await loyaltyApi.list() } catch { allCustomers.value = [] }
}

function closePayModal() {
  stopPolling()
  payOpen.value = false
  if (payStep.value === 'success') {
    showToast.value = true
    setTimeout(() => (showToast.value = false), 3000)
  }
}

async function confirmPay() {
  if (!canPay.value) return
  paying.value = true
  posError.value = ''
  try {
    // Gom items từ cart
    const items: { maSanPham: number; maKichCo: number | null; soLuong: number; ghiChuMon: string | null }[] = []
    for (const i of cart.value) {
      items.push({ maSanPham: i.maSanPham, maKichCo: i.maKichCo, soLuong: i.qty, ghiChuMon: i.ghiChuMon })
      for (const t of i.toppings)
        items.push({ maSanPham: t.maSanPham, maKichCo: null, soLuong: t.qty * i.qty, ghiChuMon: 'Topping · ' + i.name })
    }
    // Đẩy đơn ảo vào store Bếp KDS (Dành cho bản Demo Local)
    const tbName = orderType.value === 'dine-in' ? tables.value.find(t => t.maBan === selectedTableId.value)?.tenBan || '' : 'Mang về'
    orderStore.createOrder({
      table: tbName,
      items: cart.value.map(i => ({ name: i.name, qty: i.qty, price: i.unitPrice, note: i.ghiChuMon || undefined })),
      isPriority: isPriority.value
    })

    if (payMethod.value === 'TienMat') {
      // Tiền mặt: checkout trực tiếp như cũ
      const res = await ordersApi.checkout({
        maBan: orderType.value === 'dine-in' ? selectedTableId.value : null,
        items,
        ghiChuDonHang: note.value.trim() || null,
        phuongThuc: 'TienMat',
        soTienKhachTra: cashReceived.value || finalTotal.value,
        maKhuyenMai: appliedPromo.value?.maKhuyenMai ?? null,
        maKhachHang: customerProfile.value?.id ?? null,
        tienGiamGia: (appliedPromo.value?.tienGiam || 0) + (redeemedDiscount.value || 0),
      })
      toastChange.value = res.tienThoiLai
      orderIdResponse.value = res.maDonHang
      isTakeawayResponse.value = orderType.value === 'takeaway'
      paySuccessPinCode.value = res.maPinSession || selectedTable.value?.maPinSession || null
      
      auditLogsApi.createLog({
        maNhanVien: authStore.user?.maNhanVien,
        hanhDong: 'TẠO ĐƠN HÀNG',
        module: 'ĐƠN HÀNG',
        duLieuMoi: `Mới: Thu ngân vừa thanh toán đơn #${res.maDonHang} tại [${tbName}] - Tổng tiền: ${formatVND(posFinalTotal.value)}. Phương thức: Tiền mặt.`
      }).catch(() => {})

      clearCart()
      isPriority.value = false
      selectedTableId.value = null
      tables.value = await tablesApi.list()
      payStep.value = 'success'
      showToast.value = true
      await refreshCustomerProfile()
      setTimeout(() => (showToast.value = false), 5000)
    } else {
      // MoMo / VietQR: Bước 1 - Tạo đơn hàng trước
      const order = await ordersApi.create({
        maBan: orderType.value === 'dine-in' ? selectedTableId.value : null,
        items,
        ghiChuDonHang: note.value.trim() || null,
        maKhachHang: customerProfile.value?.id ?? null,
        tienGiamGia: (appliedPromo.value?.tienGiam || 0) + (redeemedDiscount.value || 0),
      })
      createdOrderId.value = order.maDonHang
      orderIdResponse.value = order.maDonHang
      isTakeawayResponse.value = orderType.value === 'takeaway'

      // Bước 2 - Gọi API sinh QR tương ứng
      qrLoading.value = true
      payStep.value = 'qr'
      qrCodeUrl.value = null
      payUrl.value = null
      qrError.value = ''

      try {
        const qrRes = payMethod.value === 'Momo'
          ? await paymentsApi.payMomo({ maDonHang: order.maDonHang, maKhuyenMai: appliedPromo.value?.maKhuyenMai ?? null })
          : await paymentsApi.payVietQr({ maDonHang: order.maDonHang, maKhuyenMai: appliedPromo.value?.maKhuyenMai ?? null })

        if (qrRes.success) {
          qrCodeUrl.value = qrRes.qrCodeUrl
          qrRawString.value = qrRes.qrRawString ?? null
          payUrl.value = qrRes.payUrl
          // Bắt đầu polling kiểm tra trạng thái
          startPolling(order.maDonHang)
          tables.value = await tablesApi.list()  // cập nhật trạng thái bàn sang "Có khách"
        } else {
          qrError.value = qrRes.message || 'Không tạo được mã QR. Vui lòng thử lại.'
        }
      } catch (e) {
        qrError.value = e instanceof Error ? e.message : 'Lỗi kết nối khi tạo QR.'
      } finally {
        qrLoading.value = false
      }
    }
  } catch (e) {
    posError.value = e instanceof Error ? e.message : 'Thanh toán thất bại.'
  } finally {
    paying.value = false
  }
}

async function retryQr() {
  if (!createdOrderId.value) return
  qrLoading.value = true
  qrError.value = ''
  try {
    const qrRes = payMethod.value === 'Momo'
      ? await paymentsApi.payMomo({ maDonHang: createdOrderId.value, maKhuyenMai: null })
      : await paymentsApi.payVietQr({ maDonHang: createdOrderId.value, maKhuyenMai: null })
    if (qrRes.success) {
      qrCodeUrl.value = qrRes.qrCodeUrl
      qrRawString.value = qrRes.qrRawString ?? null
      payUrl.value = qrRes.payUrl
      startPolling(createdOrderId.value)
    } else {
      qrError.value = qrRes.message
    }
  } catch (e) {
    qrError.value = e instanceof Error ? e.message : 'Lỗi kết nối.'
  } finally {
    qrLoading.value = false
  }
}

async function cancelQrAndBack() {
  stopPolling()
  payStep.value = 'select'
}

async function manualConfirm() {
  if (!createdOrderId.value) return
  paying.value = true
  try {
    const res = await paymentsApi.confirmTransfer(createdOrderId.value, finalTotal.value)
    if (res.success) {
      stopPolling()
      payStep.value = 'success'
      clearCart()
      selectedTableId.value = null
      tables.value = await tablesApi.list()
      await refreshCustomerProfile()
    } else {
      posError.value = res.message
    }
  } catch (e) {
    posError.value = e instanceof Error ? e.message : 'Xác nhận thất bại.'
  } finally {
    paying.value = false
  }
}

onUnmounted(() => {
  stopPolling()
  if (serviceReqInterval) clearInterval(serviceReqInterval)
  if (timeIntervalId) clearInterval(timeIntervalId)
  if (menuSyncIntervalId) clearInterval(menuSyncIntervalId)
  if (posChannel) posChannel.close()
})
</script>

<style scoped>
.scrollbar-hide::-webkit-scrollbar { display: none; }
.scrollbar-hide { -ms-overflow-style: none; scrollbar-width: none; }
.modal-fade-enter-active, .modal-fade-leave-active { transition: opacity 0.2s ease; }
.modal-fade-enter-from, .modal-fade-leave-to { opacity: 0; }
.toast-enter-active, .toast-leave-active { transition: all 0.3s cubic-bezier(0.4,0,0.2,1); }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(12px) scale(0.95); }
</style>
