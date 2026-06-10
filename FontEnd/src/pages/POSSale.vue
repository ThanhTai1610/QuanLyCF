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
          <button v-for="cat in catFilters" :key="cat.id" @click="activeCat = cat.id"
            :class="activeCat===cat.id ? 'bg-[#2A231E] text-white' : 'bg-[#F5F2ED] text-[#5C544E] hover:bg-[#EAE3D9]'"
            class="shrink-0 px-3.5 py-1.5 rounded-full text-xs font-bold uppercase tracking-wider transition-colors">
            {{ cat.label }}
          </button>
        </div>
      </div>
      <div class="flex-1 overflow-y-auto p-4">
        <div class="grid grid-cols-2 sm:grid-cols-3 xl:grid-cols-4 gap-3">
          <button v-for="item in filteredMenu" :key="item.id" @click="openOptions(item)"
            class="group bg-white rounded-2xl border border-[#EAE3D9] overflow-hidden hover:shadow-lg hover:-translate-y-0.5 transition-all duration-200 text-left relative">
            <div class="relative h-24 overflow-hidden">
              <img :src="item.image" :alt="item.name" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />
              <div class="absolute inset-0 bg-gradient-to-t from-black/30 to-transparent"></div>
              <span v-if="item.popular" class="absolute top-2 left-2 bg-[#CC8033] text-white text-[8px] font-bold uppercase px-1.5 py-0.5 rounded-full">🔥 Bán chạy</span>
              <div v-if="cartQty(item.id)>0" class="absolute top-2 right-2 w-5 h-5 rounded-full bg-[#CC8033] text-white text-[10px] font-bold flex items-center justify-center shadow-md">{{ cartQty(item.id) }}</div>
            </div>
            <div class="p-2.5">
              <p class="text-xs font-bold text-[#2A231E] leading-snug truncate">{{ item.name }}</p>
              <p class="text-sm font-premium-serif font-bold text-[#CC8033] mt-1">{{ formatVND(item.price) }}</p>
            </div>
          </button>
        </div>
      </div>
    </div>

    <!-- RIGHT: Cart -->
    <div class="flex flex-col w-[42%] bg-white">
      <!-- Top bar -->
      <div class="px-4 py-3 border-b border-[#EAE3D9] bg-[#FDFBF7] space-y-2.5">
        <!-- Title row -->
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
        <!-- Order type pill -->
        <div class="flex gap-1.5 p-1 bg-[#F0EDE9] rounded-xl">
          <button @click="orderType='dine-in'"
            :class="orderType==='dine-in' ? 'bg-white text-[#2A231E] shadow-sm' : 'text-[#8A8178] hover:text-[#5C544E]'"
            class="flex-1 flex items-center justify-center gap-1.5 py-2 rounded-lg text-[11px] font-bold transition-all">
            🪑 Tại quán
          </button>
          <button @click="orderType='takeaway'; selectedTable=null"
            :class="orderType==='takeaway' ? 'bg-[#CC8033] text-white shadow-sm' : 'text-[#8A8178] hover:text-[#5C544E]'"
            class="flex-1 flex items-center justify-center gap-1.5 py-2 rounded-lg text-[11px] font-bold transition-all">
            🛍️ Mang về
          </button>
        </div>
        <!-- Table selector -->
        <div v-if="orderType==='dine-in'">
          <p class="text-[9px] uppercase tracking-widest font-bold text-[#8A8178] mb-1.5">
            Chọn bàn
            <span v-if="selectedTable" class="normal-case text-[#CC8033] font-bold ml-1">· Bàn {{ selectedTable }} đang chọn</span>
          </p>
          <div class="flex gap-1.5 overflow-x-auto scrollbar-hide pb-0.5">
            <button v-for="n in 20" :key="n" @click="selectedTable = selectedTable===n ? null : n"
              :class="selectedTable===n
                ? 'bg-[#CC8033] text-white border-[#CC8033]'
                : 'bg-white text-[#5C544E] border-[#EAE3D9] hover:border-[#CC8033] hover:text-[#CC8033]'"
              class="shrink-0 w-9 h-8 rounded-lg border text-xs font-bold transition-all">
              {{ n }}
            </button>
          </div>
        </div>
      </div>
      <div class="flex-1 overflow-y-auto px-4 py-3 space-y-2">
        <div v-if="cart.length===0" class="h-full flex flex-col items-center justify-center py-12">
          <div class="w-16 h-16 rounded-2xl bg-[#F5F2ED] flex items-center justify-center mb-4">
            <ShoppingCart class="w-7 h-7 text-[#C5BEB8]" stroke-width="1.5" />
          </div>
          <p class="text-sm font-bold text-[#C5BEB8]">Giỏ hàng trống</p>
          <p class="text-xs text-[#D5CEC8] font-medium mt-1">Chọn món từ thực đơn bên trái</p>
        </div>
        <div v-for="item in cart" :key="item.cartId" class="flex items-start gap-3 p-3 bg-[#F9F8F6] rounded-xl group">
          <img :src="item.image" :alt="item.name" class="w-10 h-10 rounded-xl object-cover shrink-0 border border-[#EAE3D9]" />
          <div class="flex-1 min-w-0">
            <p class="text-sm font-bold text-[#2A231E] truncate">{{ item.name }}</p>
            <div class="text-[10px] text-[#8A8178] font-medium mt-0.5 space-y-0.5">
              <span v-if="item.size==='L'" class="mr-2">Size L (+10k)</span>
              <span v-for="t in item.toppings" :key="t.name" class="mr-2 text-[#CC8033]">+{{ t.name }}×{{ t.qty }}</span>
              <span v-if="item.sugar!=='100%'" class="mr-2">Đường {{ item.sugar }}</span>
              <span v-if="item.ice!=='100%'">Đá {{ item.ice }}</span>
              <span v-if="item.iceStyle==='riêng'" class="text-blue-500">· Đá riêng</span>
            </div>
            <p class="text-xs text-[#CC8033] font-semibold mt-1">{{ formatVND(item.unitPrice) }}</p>
          </div>
          <div class="flex items-center gap-1 shrink-0">
            <button @click="updateQty(item.cartId,-1)" class="w-7 h-7 rounded-lg bg-white border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] hover:border-[#CC8033] transition-colors font-bold text-sm">−</button>
            <span class="w-6 text-center text-sm font-bold">{{ item.qty }}</span>
            <button @click="updateQty(item.cartId,1)" class="w-7 h-7 rounded-lg bg-white border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] hover:border-[#CC8033] transition-colors font-bold text-sm">+</button>
          </div>
          <p class="w-18 text-right text-sm font-bold text-[#2A231E] shrink-0">{{ formatVND(item.unitPrice*item.qty) }}</p>
          <button @click="removeItem(item.cartId)" class="opacity-0 group-hover:opacity-100 text-red-400 p-1 transition-all">
            <X class="w-3.5 h-3.5" />
          </button>
        </div>
      </div>
      <div class="px-4 py-3 border-t border-[#EAE3D9] space-y-3">
        <div class="relative">
          <MessageSquare class="w-3.5 h-3.5 absolute left-3 top-3 text-[#C5BEB8]" />
          <textarea v-model="note" rows="2" placeholder="Ghi chú đặc biệt..."
            class="w-full pl-8 pr-3 py-2.5 border border-[#EAE3D9] rounded-xl text-xs font-medium resize-none focus:border-[#CC8033] outline-none"></textarea>
        </div>
        <div class="space-y-1.5">
          <div class="flex justify-between text-sm text-[#8A8178]"><span>Tạm tính</span><span class="font-semibold">{{ formatVND(cartTotal) }}</span></div>
          <div class="flex justify-between items-center pt-2 border-t border-[#EAE3D9]">
            <span class="text-base font-bold">Tổng cộng</span>
            <span class="text-xl font-premium-serif font-bold text-[#CC8033]">{{ formatVND(cartTotal) }}</span>
          </div>
        </div>
        <div class="grid grid-cols-3 gap-2">
          <button v-for="m in payMethods" :key="m.id" @click="payMethod=m.id"
            :class="payMethod===m.id ? 'border-[#CC8033] bg-[#FDF7EF] text-[#CC8033]' : 'border-[#EAE3D9] text-[#8A8178]'"
            class="flex flex-col items-center gap-1 py-2.5 rounded-xl border text-center transition-colors">
            <component :is="m.icon" class="w-4 h-4" stroke-width="2" />
            <span class="text-[10px] font-bold">{{ m.label }}</span>
          </button>
        </div>

        <!-- Tiền mặt: khách đưa & tiền thối -->
        <div v-if="payMethod==='cash'" class="space-y-2">
          <div class="flex items-center gap-2">
            <div class="flex-1">
              <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1">Tiền khách đưa (₫)</label>
              <input v-model.number="cashReceived" type="number" placeholder="0"
                class="w-full px-3 py-2.5 border border-[#EAE3D9] rounded-xl text-sm font-bold text-[#2A231E] focus:border-[#CC8033] focus:ring-2 focus:ring-[#CC8033]/10 outline-none" />
            </div>
            <div class="flex-1">
              <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1">Tiền thối (₫)</label>
              <div class="px-3 py-2.5 rounded-xl text-sm font-bold border"
                :class="change >= 0 ? 'bg-emerald-50 border-emerald-200 text-emerald-700' : 'bg-red-50 border-red-200 text-red-500'">
                {{ cashReceived > 0 ? formatVND(change) : '—' }}
              </div>
            </div>
          </div>
          <!-- Quick amount -->
          <div class="flex gap-1.5 overflow-x-auto scrollbar-hide">
            <button v-for="amt in quickAmounts" :key="amt" @click="cashReceived = amt"
              :class="cashReceived===amt ? 'bg-[#CC8033] text-white border-[#CC8033]' : 'bg-white text-[#5C544E] border-[#EAE3D9] hover:border-[#CC8033] hover:text-[#CC8033]'"
              class="shrink-0 px-3 py-1.5 rounded-lg border text-[10px] font-bold transition-all whitespace-nowrap">
              {{ amt >= 1000000 ? (amt/1000000)+'tr' : (amt/1000)+'k' }}
            </button>
          </div>
        </div>
        <button @click="checkout" :disabled="cart.length===0 || (orderType==='dine-in' && !selectedTable)"
          class="w-full py-3.5 rounded-xl font-bold text-sm transition-all disabled:opacity-40 flex flex-col items-center justify-center gap-0.5 shadow-lg"
          :class="cart.length>0 ? 'bg-[#2A231E] hover:bg-[#1A1512] text-white hover:-translate-y-0.5' : 'bg-[#F5F2ED] text-[#C5BEB8]'">
          <span class="flex items-center gap-2">
            <CheckCircle class="w-4 h-4" stroke-width="2.5" />
            Thanh toán · {{ formatVND(cartTotal) }}
          </span>
          <span v-if="orderType==='dine-in' && !selectedTable" class="text-[10px] opacity-60 font-medium">Vui lòng chọn bàn trước</span>
          <span v-else class="text-[10px] opacity-70 font-medium">{{ orderType==='takeaway' ? '🛍️ Mang về' : '🪑 Bàn ' + selectedTable }}</span>
        </button>
      </div>
    </div>

    <!-- ===== TOPPING MODAL ===== -->
    <Transition name="modal-fade">
      <div v-if="optionsOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="optionsOpen=false"></div>
        <div class="relative w-full max-w-lg bg-[#FDFBF7] rounded-2xl shadow-2xl flex flex-col max-h-[88vh] overflow-hidden">
          <!-- Header image -->
          <div class="relative h-44 shrink-0 bg-[#F5F2ED]">
            <img :src="selectedItem?.image" class="w-full h-full object-cover" />
            <div class="absolute inset-0 bg-gradient-to-t from-[#FDFBF7] via-black/10 to-transparent"></div>
            <button @click="optionsOpen=false" class="absolute top-3 right-3 w-9 h-9 rounded-full bg-white/30 backdrop-blur-md border border-white/40 flex items-center justify-center text-white hover:bg-white/50 transition-colors">
              <X class="w-4 h-4" stroke-width="2.5" />
            </button>
          </div>

          <!-- Scrollable body -->
          <div class="flex-1 overflow-y-auto p-6 space-y-6">
            <div>
              <h2 class="font-premium-serif text-2xl font-bold text-[#2A231E]">{{ selectedItem?.name }}</h2>
              <p class="text-xs text-[#8A8178] font-medium mt-1">{{ selectedItem?.description }}</p>
              <p class="text-xl font-bold text-[#CC8033] mt-2">{{ formatVND(selectedItem?.price || 0) }}</p>
            </div>

            <!-- Size -->
            <div class="space-y-3">
              <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Kích cỡ</h3>
              <div class="grid grid-cols-2 gap-3">
                <button v-for="s in sizes" :key="s.value" type="button" @click="selSize = s.value"
                  :class="selSize===s.value ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white'"
                  class="flex items-center gap-3 p-3.5 rounded-xl border-2 cursor-pointer transition-all text-left">
                  <div class="w-4 h-4 rounded-full border-2 flex items-center justify-center shrink-0"
                    :class="selSize===s.value ? 'border-[#CC8033]' : 'border-[#EAE3D9]'">
                    <div class="w-2 h-2 rounded-full bg-[#CC8033] transition-transform" :class="selSize===s.value ? 'scale-100' : 'scale-0'"></div>
                  </div>
                  <span class="text-sm font-bold text-[#2A231E]">{{ s.label }} <span class="text-xs font-medium" :class="s.extra>0?'text-[#CC8033]':'text-[#8A8178]'">{{ s.extra>0 ? '+'+formatVND(s.extra) : 'Tiêu chuẩn' }}</span></span>
                </button>
              </div>
            </div>

            <!-- Toppings -->
            <div class="space-y-3">
              <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Topping</h3>
              <div class="grid grid-cols-3 gap-3">
                <div v-for="t in toppings" :key="t.id"
                  :class="(selToppings[t.id]||0)>0 ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white'"
                  class="flex flex-col items-center p-2.5 rounded-xl border-2 transition-all">
                  <div class="w-full aspect-square rounded-xl overflow-hidden bg-[#F5F2ED] mb-2 cursor-pointer" @click="updTopping(t.id,1)">
                    <div class="w-full h-full flex items-center justify-center text-3xl">{{ t.emoji }}</div>
                  </div>
                  <p class="text-[10px] font-bold text-[#2A231E] text-center">{{ t.name }}</p>
                  <p class="text-[10px] text-[#CC8033] font-bold">+{{ formatVND(t.price) }}</p>
                  <div v-if="(selToppings[t.id]||0)>0" class="flex items-center gap-1 mt-1.5">
                    <button @click="updTopping(t.id,-1)" class="w-6 h-6 rounded-full bg-white border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] text-sm font-bold">−</button>
                    <span class="text-xs font-bold text-[#CC8033] w-4 text-center">{{ selToppings[t.id] }}</span>
                    <button @click="updTopping(t.id,1)" class="w-6 h-6 rounded-full bg-[#CC8033] flex items-center justify-center text-white text-sm font-bold">+</button>
                  </div>
                  <button v-else @click="updTopping(t.id,1)" class="w-full mt-1.5 py-1 rounded-lg bg-[#F5F2ED] text-[#8A8178] text-[9px] font-bold uppercase tracking-wider hover:bg-[#EAE3D9] transition-colors">Thêm</button>
                </div>
              </div>
            </div>

            <!-- Sugar & Ice -->
            <div class="grid grid-cols-2 gap-4">
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

            <!-- Ice style: riêng / chung -->
            <div v-if="selIce !== '0%'" class="space-y-2">
              <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Cách phục vụ đá</h3>
              <div class="grid grid-cols-2 gap-2">
                <button v-for="s in iceStyles" :key="s.value" @click="selIceStyle=s.value"
                  :class="selIceStyle===s.value ? 'border-[#CC8033] bg-[#FFF9F2] text-[#CC8033]' : 'border-[#EAE3D9] bg-white text-[#5C544E] hover:border-[#CC8033]/50'"
                  class="flex items-center gap-2.5 px-4 py-3 rounded-xl border-2 text-sm font-bold transition-all">
                  <span class="text-lg">{{ s.emoji }}</span>
                  <div class="text-left">
                    <p class="text-xs font-bold">{{ s.label }}</p>
                    <p class="text-[10px] font-medium opacity-70">{{ s.desc }}</p>
                  </div>
                </button>
              </div>
            </div>

            <!-- Note -->
            <div class="space-y-2">
              <h3 class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">Ghi chú</h3>
              <textarea v-model="itemNote" rows="2" placeholder="Ví dụ: ít béo, không đường..."
                class="w-full p-3 rounded-xl border border-[#EAE3D9] focus:border-[#CC8033] outline-none text-sm font-medium resize-none bg-white"></textarea>
            </div>
          </div>

          <!-- Footer -->
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

    <!-- Success toast -->
    <Transition name="toast">
      <div v-if="showToast" class="fixed bottom-6 right-6 z-50 flex items-center gap-3 bg-[#2A231E] text-white px-5 py-3.5 rounded-2xl shadow-2xl">
        <CheckCircle class="w-5 h-5 text-emerald-400" stroke-width="2.5" />
        <div>
          <p class="text-sm font-bold">Thanh toán thành công!</p>
          <p class="text-[10px] text-white/60 font-medium">Đơn hàng đã được ghi nhận</p>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Search, ShoppingCart, Trash2, X, MessageSquare, CheckCircle, Banknote, CreditCard, Smartphone, Plus } from 'lucide-vue-next'
import { menuItems, categories, formatVND } from '../data/menu'

interface Topping { name: string; price: number; qty: number }
interface CartItem { cartId: number; id: string; name: string; price: number; image: string; qty: number; unitPrice: number; size: string; sugar: string; ice: string; iceStyle: string; toppings: Topping[] }

const orderType    = ref<'dine-in'|'takeaway'>('dine-in')
const selectedTable = ref<number|null>(null)
const search    = ref('')
const activeCat = ref('all')
const note      = ref('')
const payMethod    = ref('cash')
const cashReceived = ref(0)
const showToast    = ref(false)
const cart         = ref<CartItem[]>([])
let cartIdSeq      = 0

const quickAmounts = [50000, 100000, 200000, 500000, 1000000, 2000000]
const change = computed(() => cashReceived.value - cartTotal.value)

// Modal state
const optionsOpen  = ref(false)
const selectedItem = ref<any>(null)
const selSize      = ref('M')
const selSugar     = ref('100%')
const selIce       = ref('100%')
const selIceStyle  = ref('chung')
const selToppings  = ref<Record<string,number>>({})
const itemNote     = ref('')
const selQty       = ref(1)

const iceStyles = [
  { value: 'chung', label: 'Đá chung', desc: 'Cho đá vào ly', emoji: '🥤' },
  { value: 'riêng', label: 'Đá riêng', desc: 'Đá để bên cạnh', emoji: '🧊' },
]

const sizes = [
  { value: 'M', label: 'Vừa (M)', extra: 0 },
  { value: 'L', label: 'Lớn (L)', extra: 10000 },
]

const toppings = [
  { id: 'tc_den',   name: 'Trân châu đen',   price: 10000, emoji: '🫧' },
  { id: 'tc_trang', name: 'Trân châu trắng', price: 15000, emoji: '🤍' },
  { id: 'pho_mai',  name: 'Thạch phô mai',   price: 15000, emoji: '🧀' },
  { id: 'pudding',  name: 'Pudding',          price: 15000, emoji: '🍮' },
  { id: 'suong_sao',name: 'Sương sáo',        price: 10000, emoji: '🌿' },
  { id: 'kem_tuoi', name: 'Kem tươi',         price: 12000, emoji: '🍦' },
]

const catFilters  = [{ id: 'all', label: 'Tất cả' }, ...categories.filter(c => c.id !== 'all')]
const payMethods  = [
  { id: 'cash',     label: 'Tiền mặt',    icon: Banknote },
  { id: 'card',     label: 'Thẻ',         icon: CreditCard },
  { id: 'transfer', label: 'CK ngân hàng',icon: Smartphone },
]

const filteredMenu    = computed(() => menuItems.filter(m => (activeCat.value === 'all' || m.category === activeCat.value) && m.name.toLowerCase().includes(search.value.toLowerCase())))
const cartTotal       = computed(() => cart.value.reduce((s, i) => s + i.unitPrice * i.qty, 0))
const cartTotalQty    = computed(() => cart.value.reduce((s, i) => s + i.qty, 0))
const cartQty         = (id: string) => cart.value.filter(i => i.id === id).reduce((s,i)=>s+i.qty,0)
const toppingExtra    = computed(() => toppings.reduce((s,t) => s + (selToppings.value[t.id]||0)*t.price, 0))
const sizeExtra       = computed(() => selSize.value === 'L' ? 10000 : 0)
const unitPricePreview = computed(() => (selectedItem.value?.price || 0) + sizeExtra.value + toppingExtra.value)

const openOptions = (item: any) => {
  selectedItem.value = item
  selSize.value = 'M'; selSugar.value = '100%'; selIce.value = '100%'; selIceStyle.value = 'chung'
  selToppings.value = {}; itemNote.value = ''; selQty.value = 1
  optionsOpen.value = true
}

const updTopping = (id: string, delta: number) => {
  if (!selToppings.value[id]) selToppings.value[id] = 0
  const next = selToppings.value[id] + delta
  if (next >= 0) selToppings.value[id] = next
}

const confirmAdd = () => {
  const toppingArr = toppings.filter(t => (selToppings.value[t.id]||0)>0).map(t => ({ name: t.name, price: t.price, qty: selToppings.value[t.id] }))
  cart.value.push({
    cartId: cartIdSeq++, id: selectedItem.value.id,
    name: selectedItem.value.name, price: selectedItem.value.price,
    image: selectedItem.value.image, qty: selQty.value,
    unitPrice: unitPricePreview.value,
    size: selSize.value, sugar: selSugar.value, ice: selIce.value, iceStyle: selIceStyle.value,
    toppings: toppingArr,
  })
  optionsOpen.value = false
}

const updateQty = (cartId: number, delta: number) => {
  const item = cart.value.find(i => i.cartId === cartId)
  if (!item) return
  item.qty += delta
  if (item.qty <= 0) cart.value = cart.value.filter(i => i.cartId !== cartId)
}
const removeItem = (cartId: number) => { cart.value = cart.value.filter(i => i.cartId !== cartId) }
const clearCart  = () => { cart.value = []; note.value = ''; cashReceived.value = 0 }
const checkout   = () => {
  if (!cart.value.length) return
  showToast.value = true; clearCart()
  setTimeout(() => showToast.value = false, 3000)
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
