<template>
  <div class="h-full flex font-premium-sans text-[#2A231E] -m-6 overflow-hidden" style="height:calc(100vh - 3.5rem)">

    <!-- LEFT: Menu -->
    <div class="flex flex-col w-[58%] border-r border-[#EAE3D9] bg-[#FDFBF7]">
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
          <button v-for="item in filteredMenu" :key="item.maSanPham" @click="openOptions(item)"
            class="group bg-white rounded-2xl border border-[#EAE3D9] overflow-hidden hover:shadow-xl hover:shadow-[#CC8033]/10 hover:-translate-y-1 hover:border-[#CC8033]/40 transition-all duration-200 text-left relative">
            <div class="relative h-28 overflow-hidden bg-[#F5F2ED]">
              <img v-if="item.hinhAnh" :src="item.hinhAnh" :alt="item.tenSanPham" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
              <div v-else class="w-full h-full flex items-center justify-center text-[#C5BEB8]"><Coffee class="w-8 h-8" /></div>
              <div class="absolute inset-0 bg-gradient-to-t from-black/15 to-transparent"></div>
              <div v-if="cartQty(item.maSanPham)>0" class="absolute top-2 left-2 min-w-[20px] h-5 px-1.5 rounded-full bg-[#CC8033] text-white text-[10px] font-bold flex items-center justify-center shadow-md ring-2 ring-white">{{ cartQty(item.maSanPham) }}</div>
              <div class="absolute bottom-2 right-2 w-7 h-7 rounded-full bg-white/90 backdrop-blur text-[#CC8033] flex items-center justify-center shadow-md opacity-0 group-hover:opacity-100 translate-y-1 group-hover:translate-y-0 transition-all">
                <Plus class="w-4 h-4" stroke-width="2.5" />
              </div>
            </div>
            <div class="p-2.5">
              <p class="text-xs font-bold text-[#2A231E] leading-snug truncate">{{ item.tenSanPham }}</p>
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
              :disabled="t.trangThai !== 'Trong'"
              @click="selectedTableId = selectedTableId===t.maBan ? null : t.maBan"
              :title="tableStatusMeta[t.trangThai]?.label"
              :class="selectedTableId===t.maBan
                ? 'bg-[#CC8033] border-[#CC8033] text-white shadow-md'
                : t.trangThai==='Trong'
                  ? 'bg-white border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033] hover:text-[#CC8033]'
                  : 'bg-[#F5F2ED] border-[#EAE3D9] text-[#C5BEB8] cursor-not-allowed'"
              class="relative h-[42px] rounded-xl border flex flex-col items-center justify-center gap-0.5 transition-all disabled:cursor-not-allowed">
              <span class="text-xs font-bold leading-none">{{ t.tenBan }}</span>
              <span class="flex items-center gap-1 text-[8px] font-semibold leading-none">
                <span class="w-1 h-1 rounded-full" :class="selectedTableId===t.maBan ? 'bg-white' : tableStatusMeta[t.trangThai]?.dot"></span>
                {{ tableStatusMeta[t.trangThai]?.label }}
              </span>
            </button>
            <span v-if="tablesInZone.length===0" class="col-span-5 text-xs text-[#8A8178] py-2 text-center">Không có bàn trong khu vực này.</span>
          </div>
          <p class="text-[9px] text-[#C5BEB8] mt-1">Bàn đang có khách / bảo trì sẽ không chọn được.</p>
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
            <p class="text-sm font-bold text-[#2A231E] truncate">{{ item.name }}</p>
            <p v-if="item.optionText" class="text-[10px] text-[#8A8178] font-medium mt-0.5">{{ item.optionText }}</p>
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
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="payOpen=false"></div>
        <div class="relative w-full max-w-md bg-[#FDFBF7] rounded-2xl shadow-2xl overflow-hidden">
          <div class="p-5 border-b border-[#EAE3D9] flex items-center justify-between">
            <div>
              <h2 class="font-premium-serif text-xl font-bold text-[#2A231E]">Thanh toán</h2>
              <p class="text-xs text-[#8A8178] font-medium">{{ orderType==='takeaway' ? 'Mang về' : (selectedTable?.tenBan || '') }} · {{ cartTotalQty }} phần</p>
            </div>
            <button @click="payOpen=false" class="w-9 h-9 rounded-full bg-[#F5F2ED] flex items-center justify-center text-[#8A8178] hover:bg-[#EAE3D9]"><X class="w-4 h-4"/></button>
          </div>
          <div class="p-5 space-y-4">
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
              <div v-if="appliedPromo" class="flex justify-between text-sm text-emerald-700 font-semibold"><span>Giảm giá</span><span>− {{ formatVND(appliedPromo.tienGiam) }}</span></div>
              <div class="flex justify-between items-center pt-1.5 border-t border-[#F0E4D2]">
                <span class="text-sm font-bold text-[#5C544E]">Tổng cộng</span>
                <span class="text-2xl font-premium-serif font-bold text-[#CC8033]">{{ formatVND(finalTotal) }}</span>
              </div>
            </div>
            <div class="grid grid-cols-2 gap-2">
              <button v-for="m in payMethods" :key="m.id" @click="payMethod=m.id; ckType=null"
                :class="payMethod===m.id ? 'border-[#CC8033] bg-[#FDF7EF] text-[#CC8033]' : 'border-[#EAE3D9] text-[#8A8178]'"
                class="flex flex-col items-center gap-1 py-3 rounded-xl border-2 transition-colors">
                <component :is="m.icon" class="w-5 h-5" />
                <span class="text-[11px] font-bold">{{ m.label }}</span>
              </button>
            </div>

            <!-- Loại chuyển khoản: MoMo / Ngân hàng -->
            <div v-if="payMethod==='ChuyenKhoan'" class="space-y-2">
              <label class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Hình thức chuyển khoản</label>
              <div class="grid grid-cols-2 gap-2">
                <button v-for="c in ckOptions" :key="c.id" @click="ckType=c.id"
                  :class="ckType===c.id ? 'border-[#CC8033] bg-[#FDF7EF] text-[#CC8033]' : 'border-[#EAE3D9] text-[#8A8178]'"
                  class="flex items-center justify-center gap-2 py-2.5 rounded-xl border-2 transition-colors">
                  <component :is="c.icon" class="w-4 h-4" />
                  <span class="text-xs font-bold">{{ c.label }}</span>
                </button>
              </div>
            </div>
            <div v-if="payMethod==='TienMat'" class="space-y-2">
              <label class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Tiền khách đưa</label>
              <input v-model.number="cashReceived" type="number" placeholder="0"
                class="w-full px-3 py-2.5 border border-[#EAE3D9] rounded-xl text-base font-bold focus:border-[#CC8033] outline-none" />
              <div class="flex gap-1.5 flex-wrap">
                <button v-for="a in quickAmounts" :key="a" @click="cashReceived=a"
                  class="px-3 py-1.5 rounded-lg border border-[#EAE3D9] text-[11px] font-bold text-[#5C544E] hover:border-[#CC8033] hover:text-[#CC8033]">{{ (a/1000)+'k' }}</button>
                <button @click="cashReceived=cartTotal" class="px-3 py-1.5 rounded-lg border border-[#CC8033] text-[11px] font-bold text-[#CC8033]">Vừa đủ</button>
              </div>
              <div class="flex items-center justify-between px-3 py-2 rounded-xl" :class="change>0 ? 'bg-emerald-50 text-emerald-700' : 'bg-[#F5F2ED] text-[#8A8178]'">
                <span class="text-sm font-semibold">Tiền thối</span>
                <span class="text-lg font-bold">{{ formatVND(change) }}</span>
              </div>
            </div>
            <p v-if="posError" class="text-xs font-semibold text-red-600">{{ posError }}</p>
          </div>
          <div class="p-5 pt-0">
            <button @click="confirmPay" :disabled="!canPay || paying"
              class="w-full py-3.5 rounded-2xl font-bold text-sm bg-gradient-to-r from-[#CC8033] to-[#8A4F1A] text-white shadow-lg disabled:opacity-40 flex items-center justify-center gap-2">
              <CheckCircle class="w-4 h-4" /> {{ paying ? 'Đang xử lý...' : 'Xác nhận thanh toán' }}
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
          <p class="text-sm font-bold">Thanh toán thành công!</p>
          <p class="text-[10px] text-white/60 font-medium">{{ toastChange > 0 ? 'Tiền thối: ' + formatVND(toastChange) : 'Đơn đã ghi nhận' }}</p>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Search, ShoppingCart, Trash2, X, MessageSquare, CheckCircle, Plus, Coffee, Store, ShoppingBag, Banknote, Smartphone, Wallet, Landmark } from 'lucide-vue-next'
import { ordersApi, type MenuItem } from '@/services/orders'
import { tablesApi, type TableItem } from '@/services/tables'
import { promotionsApi, type Promotion, type ApplyResult } from '@/services/promotions'

const formatVND = (n: number) => (n || 0).toLocaleString('vi-VN') + 'đ'

// ── Dữ liệu thật ──────────────────────────────────────────────
const menu = ref<MenuItem[]>([])
const tables = ref<TableItem[]>([])
const loadingMenu = ref(false)
const posError = ref('')

onMounted(async () => {
  loadingMenu.value = true
  try {
    const [m, t] = await Promise.all([ordersApi.menu(), tablesApi.list()])
    menu.value = m
    tables.value = t
  } catch (e) {
    posError.value = e instanceof Error ? e.message : 'Không tải được dữ liệu.'
  } finally {
    loadingMenu.value = false
  }
})

const search = ref('')
const activeCat = ref('all')
// Tách món nước và topping
const drinks = computed(() => menu.value.filter(m => m.kieuMon !== 'Topping'))
const toppingList = computed(() => menu.value.filter(m => m.kieuMon === 'Topping'))
const catFilters = computed(() => {
  const cats = Array.from(new Set(drinks.value.map(m => m.tenDanhMuc).filter(Boolean) as string[]))
  return ['all', ...cats]
})
const filteredMenu = computed(() => drinks.value.filter(m =>
  (activeCat.value === 'all' || m.tenDanhMuc === activeCat.value) &&
  m.tenSanPham.toLowerCase().includes(search.value.toLowerCase())
))

// ── Giỏ hàng ──────────────────────────────────────────────────
interface CartTopping { maSanPham: number; ten: string; gia: number; qty: number; hinhAnh: string | null }
interface CartItem {
  cartId: number
  maSanPham: number
  name: string
  image: string | null
  maKichCo: number | null
  unitPrice: number
  qty: number
  optionText: string
  ghiChuMon: string | null
  toppings: CartTopping[]
}
const cart = ref<CartItem[]>([])
let cartIdSeq = 0
const cartTotal = computed(() => cart.value.reduce((s, i) => s + i.unitPrice * i.qty, 0))
const cartTotalQty = computed(() => cart.value.reduce((s, i) => s + i.qty, 0))
const cartQty = (maSanPham: number) => cart.value.filter(i => i.maSanPham === maSanPham).reduce((s, i) => s + i.qty, 0)

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

function openOptions(item: MenuItem) {
  selectedItem.value = item
  selSizeId.value = null
  selSugar.value = '100%'
  selIce.value = '100%'
  itemNote.value = ''
  selQty.value = 1
  selToppings.value = {}
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
  cart.value.push({
    cartId: cartIdSeq++,
    maSanPham: selectedItem.value.maSanPham,
    name: selectedItem.value.tenSanPham,
    image: selectedItem.value.hinhAnh,
    maKichCo: selSizeId.value,
    unitPrice: unitPricePreview.value,
    qty: selQty.value,
    optionText,
    ghiChuMon: optionText || null,
    toppings: tops,
  })
  optionsOpen.value = false
}

function updateQty(cartId: number, delta: number) {
  const item = cart.value.find(i => i.cartId === cartId)
  if (!item) return
  item.qty += delta
  if (item.qty <= 0) cart.value = cart.value.filter(i => i.cartId !== cartId)
}
function removeItem(cartId: number) { cart.value = cart.value.filter(i => i.cartId !== cartId) }
function clearCart() { cart.value = []; note.value = '' }

// ── Thanh toán ────────────────────────────────────────────────
const payOpen = ref(false)
const payMethod = ref<'TienMat' | 'ChuyenKhoan'>('TienMat')
const ckType = ref<'Momo' | 'NganHang' | null>(null)   // loại chuyển khoản
const cashReceived = ref<number | null>(null)
const paying = ref(false)
const showToast = ref(false)
const toastChange = ref(0)
const quickAmounts = [50000, 100000, 200000, 500000]

const payMethods: { id: 'TienMat' | 'ChuyenKhoan'; label: string; icon: unknown }[] = [
  { id: 'TienMat', label: 'Tiền mặt', icon: Banknote },
  { id: 'ChuyenKhoan', label: 'Chuyển khoản', icon: Smartphone },
]
const ckOptions: { id: 'Momo' | 'NganHang'; label: string; icon: unknown }[] = [
  { id: 'Momo', label: 'MoMo', icon: Wallet },
  { id: 'NganHang', label: 'Ngân hàng', icon: Landmark },
]
// ── Khuyến mãi ──
const activePromos = ref<Promotion[]>([])
const voucherCode = ref('')
const appliedPromo = ref<ApplyResult | null>(null)
const voucherError = ref('')
const promoBusy = ref(false)
const finalTotal = computed(() => Math.max(0, cartTotal.value - (appliedPromo.value?.tienGiam || 0)))

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
  if (payMethod.value === 'ChuyenKhoan') return ckType.value !== null
  return true
})

async function openPay() {
  if (!canCheckout.value) return
  payMethod.value = 'TienMat'
  ckType.value = null
  cashReceived.value = null
  posError.value = ''
  clearPromo()
  payOpen.value = true
  try { if (activePromos.value.length === 0) activePromos.value = await promotionsApi.active() } catch { /* bỏ qua */ }
}

async function confirmPay() {
  if (!canPay.value) return
  paying.value = true
  posError.value = ''
  try {
    const items = []
    for (const i of cart.value) {
      items.push({ maSanPham: i.maSanPham, maKichCo: i.maKichCo, soLuong: i.qty, ghiChuMon: i.ghiChuMon })
      for (const t of i.toppings)
        items.push({ maSanPham: t.maSanPham, maKichCo: null, soLuong: t.qty * i.qty, ghiChuMon: 'Topping · ' + i.name })
    }
    const res = await ordersApi.checkout({
      maBan: orderType.value === 'dine-in' ? selectedTableId.value : null,
      items,
      ghiChuDonHang: note.value.trim() || null,
      phuongThuc: payMethod.value === 'ChuyenKhoan'
        ? (ckType.value === 'Momo' ? 'Momo' : 'ChuyenKhoan')
        : payMethod.value,
      soTienKhachTra: payMethod.value === 'TienMat' ? (cashReceived.value || finalTotal.value) : null,
      maKhuyenMai: appliedPromo.value?.maKhuyenMai ?? null,
    })
    toastChange.value = res.tienThoiLai
    clearCart()
    selectedTableId.value = null
    payOpen.value = false
    tables.value = await tablesApi.list()
    showToast.value = true
    setTimeout(() => (showToast.value = false), 3000)
  } catch (e) {
    posError.value = e instanceof Error ? e.message : 'Thanh toán thất bại.'
  } finally {
    paying.value = false
  }
}
</script>

<style scoped>
.scrollbar-hide::-webkit-scrollbar { display: none; }
.scrollbar-hide { -ms-overflow-style: none; scrollbar-width: none; }
.modal-fade-enter-active, .modal-fade-leave-active { transition: opacity 0.2s ease; }
.modal-fade-enter-from, .modal-fade-leave-to { opacity: 0; }
.toast-enter-active, .toast-leave-active { transition: all 0.3s cubic-bezier(0.4,0,0.2,1); }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(12px) scale(0.95); }
</style>
