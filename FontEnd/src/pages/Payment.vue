<template>
  <div class="min-h-screen bg-[#FAF6F0] flex items-center justify-center px-4 py-10 font-premium-sans">
    <div class="w-full max-w-md bg-white rounded-lg border border-[#EAE3D9] shadow-card p-8 text-center" v-if="paid">
      <!-- Success State -->
      <div class="w-16 h-16 mx-auto mb-6 bg-[#F5F2ED] rounded-lg flex items-center justify-center border border-[#EAE3D9]">
        <CheckCircle class="w-8 h-8 text-[#4A7C59]" stroke-width="1.5" />
      </div>

      <h1 class="font-display text-3xl text-espresso font-semibold">Cảm ơn bạn đã đến! ☕</h1>
      <p class="text-muted-foreground mt-2">Đơn #{{ orderId }} đã được thanh toán thành công</p>

      <div class="mt-8 p-5 rounded-lg bg-[#F5F2ED] border border-[#EAE3D9] text-left space-y-3">
        <div v-for="(it, i) in items" :key="i" class="flex justify-between text-sm">
          <span class="text-espresso">{{ it.name }} × {{ it.qty }}</span>
          <span class="text-muted-foreground">{{ formatVND(it.qty * it.price) }}</span>
        </div>
        <div class="border-t-2 border-[#EAE3D9] pt-3 flex justify-between font-display text-lg text-espresso font-semibold">
          <span>Tổng cộng</span>
          <span class="text-caramel">{{ formatVND(total) }}</span>
        </div>
      </div>

      <div class="grid grid-cols-2 gap-3 mt-8">
        <Button variant="outline" class="border border-[#EAE3D9] rounded-lg shadow-card">
          <Download class="w-4 h-4 mr-1.5" /> Tải PDF
        </Button>
        <Button @click="$router.push('/')" class="bg-caramel text-cream rounded-lg border border-caramel/30 shadow-card">
          Về trang chủ
        </Button>
      </div>
    </div>

    <!-- Checkout Form -->
    <div v-else class="w-full max-w-xl bg-white rounded-lg border border-[#EAE3D9] shadow-card">
      <header class="h-16 flex items-center px-6 border-b-2 border-[#EAE3D9]">
        <button @click="$router.back()" class="text-espresso">
          <ArrowLeft class="w-5 h-5" />
        </button>
        <div class="flex items-center gap-2 flex-1 justify-center">
          <Coffee class="w-5 h-5 text-caramel" />
          <span class="font-display text-lg text-espresso font-semibold">Thanh toán</span>
        </div>
      </header>

      <div class="p-6 space-y-6">
        <!-- Order Summary -->
        <div class="bg-[#F5F2ED] rounded-lg border border-[#EAE3D9] p-5">
          <h2 class="font-display text-lg text-espresso font-semibold">Đơn #{{ orderId }} — {{ tableLabel }}</h2>
          <div class="mt-4 space-y-3">
            <div v-for="(it, i) in items" :key="i" class="flex justify-between text-sm">
              <span class="text-espresso">{{ it.name }} × {{ it.qty }}</span>
              <span class="text-espresso/80">{{ formatVND(it.qty * it.price) }}</span>
            </div>
          </div>
          <div class="border-t-2 border-[#EAE3D9] mt-4 pt-4 space-y-1.5 text-sm">
            <div class="flex justify-between text-muted-foreground"><span>Tạm tính</span><span>{{ formatVND(subtotal) }}</span></div>
            <div v-if="appliedPromo" class="flex justify-between text-emerald-700 font-semibold">
              <span>Giảm giá ({{ appliedPromo.tenChuongTrinh }})</span>
              <span>− {{ formatVND(promoDiscount) }}</span>
            </div>
            <div v-if="usePoints" class="flex justify-between text-emerald-700 font-semibold">
              <span>Đổi điểm thưởng</span>
              <span>− {{ formatVND(pointsDiscount) }}</span>
            </div>
            <div class="flex justify-between text-muted-foreground"><span>VAT 8%</span><span>{{ formatVND(vat) }}</span></div>
            <div class="flex justify-between text-muted-foreground"><span>Phí phục vụ</span><span>{{ formatVND(service) }}</span></div>
            <div class="flex justify-between font-display text-2xl text-caramel font-semibold pt-2 border-t-2 border-[#EAE3D9]">
              <span>Tổng cộng</span>
              <span>{{ formatVND(total) }}</span>
            </div>
          </div>
        </div>

        <!-- Promotion Section -->
        <div class="bg-white rounded-lg border border-[#EAE3D9] p-5 space-y-3 shadow-card text-left">
          <h3 class="font-display text-base text-espresso font-semibold flex items-center gap-1.5">
            <Ticket class="w-4 h-4 text-caramel" />
            Khuyến mãi &amp; Voucher
          </h3>
          <!-- Trường hợp 1: Đã chọn voucher ở trang chọn món -->
          <div v-if="order?.promoDiscount" class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-emerald-50 text-emerald-600 flex items-center justify-center shrink-0">
              <CheckCircle class="w-4 h-4" />
            </div>
            <span class="flex-1">
              <span class="block text-sm font-bold text-espresso">Đã áp dụng Voucher thành công</span>
              <span class="block text-[11px] text-muted-foreground font-medium">Đơn hàng đã áp dụng voucher giảm <span class="text-[#CC8033] font-bold">{{ formatVND(order.promoDiscount) }}</span> khi đặt món.</span>
            </span>
          </div>
          <!-- Trường hợp 2: Chưa chọn voucher, cho phép chọn tại đây -->
          <template v-else>
            <div class="flex items-center gap-2">
              <input
                v-model="voucherCode"
                @keyup.enter="applyVoucher({ code: voucherCode })"
                placeholder="Nhập mã giảm giá..."
                class="flex-1 px-3 h-10 border border-[#EAE3D9] rounded-lg text-sm focus:border-caramel outline-none uppercase bg-cream/10 text-espresso"
              />
              <button
                @click="applyVoucher({ code: voucherCode })"
                :disabled="!voucherCode.trim() || promoBusy"
                class="px-4 h-10 rounded-lg bg-espresso text-cream text-sm font-semibold disabled:opacity-40 hover:bg-espresso/90 transition-colors"
              >
                Áp dụng
              </button>
            </div>
            <div v-if="activePromos.length" class="space-y-1.5">
              <div class="text-[11px] text-muted-foreground font-semibold uppercase tracking-wider">Chương trình hiện có:</div>
              <div class="flex gap-1.5 flex-wrap">
                <button
                  v-for="p in activePromos"
                  :key="p.maKhuyenMai"
                  @click="applyVoucher({ maKhuyenMai: p.maKhuyenMai })"
                  :class="appliedPromo?.maKhuyenMai === p.maKhuyenMai ? 'border-caramel bg-[#FDF7EF] text-caramel' : 'border-[#EAE3D9] text-[#5C544E] hover:border-caramel hover:bg-cream/10'"
                  class="px-2.5 py-1 rounded-lg border text-xs font-semibold transition-colors"
                >
                  {{ p.tenChuongTrinh }}
                </button>
              </div>
            </div>
            <div v-if="savedVouchers.length" class="space-y-1.5 pt-2 border-t border-dashed border-[#EAE3D9]">
              <div class="text-[11px] text-[#CC8033] font-bold uppercase tracking-wider flex items-center gap-1">
                <Ticket class="w-3.5 h-3.5" /> Voucher đã lưu của bạn:
              </div>
              <div class="flex gap-1.5 flex-wrap">
                <button
                  v-for="code in savedVouchers"
                  :key="code"
                  @click="applySavedVoucher(code)"
                  :class="appliedPromo?.maGiamGia === code || voucherCode === code ? 'border-caramel bg-[#FDF7EF] text-caramel' : 'border-[#EAE3D9] text-[#5C544E] hover:border-caramel hover:bg-cream/10'"
                  class="px-2.5 py-1 rounded-lg border text-xs font-semibold transition-colors"
                >
                  {{ code }}
                </button>
              </div>
            </div>
            <p v-if="voucherError" class="text-xs font-semibold text-red-600">{{ voucherError }}</p>
            <div v-if="appliedPromo" class="flex items-center justify-between text-xs bg-emerald-50 border border-emerald-200 text-emerald-800 rounded-lg px-3 py-2">
              <span class="inline-flex items-center gap-1 text-emerald-700 font-semibold">
                <CheckCircle class="w-3.5 h-3.5 shrink-0" />
                Đã áp dụng: {{ appliedPromo.tenChuongTrinh }}
              </span>
              <button @click="clearPromo" class="text-espresso underline hover:text-[#2A231E] font-bold">Bỏ</button>
            </div>
          </template>
        </div>

        <!-- Dùng điểm tích luỹ (OTP) -->
        <div v-if="customerProfile" class="bg-white rounded-lg border border-[#EAE3D9] p-5 space-y-3 shadow-card text-left">
          <!-- Trường hợp 1: Đã đổi điểm ở trang chọn món -->
          <div v-if="order?.pointsDiscount" class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-emerald-50 text-emerald-600 flex items-center justify-center shrink-0">
              <CheckCircle class="w-4 h-4" />
            </div>
            <span class="flex-1">
              <span class="block text-sm font-bold text-espresso">Đã áp dụng đổi điểm thành công</span>
              <span class="block text-[11px] text-muted-foreground font-medium">Đơn hàng này đã được trừ 50 điểm thưởng (giảm 20.000đ) khi đặt món.</span>
            </span>
            <Gift class="w-5 h-5 text-[#CC8033]" />
          </div>
          <!-- Trường hợp 2: Chưa đổi điểm, cho phép đổi tại đây -->
          <label v-else for="usePoints" class="flex items-center gap-3 cursor-pointer">
            <input type="checkbox" v-model="usePoints" id="usePoints" class="w-5 h-5 rounded-md border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033]" />
            <span class="flex-1">
              <span class="block text-sm font-bold text-espresso">Dùng 50 điểm thưởng</span>
              <span class="block text-[11px] text-muted-foreground font-medium">Giảm ngay <span class="text-[#CC8033] font-bold">20.000đ</span> cho đơn này (Hội viên: {{ customerProfile.name }} - {{ customerProfile.points }} điểm)</span>
            </span>
            <Gift class="w-5 h-5 text-[#CC8033]" />
          </label>
        </div>

        <!-- Payment Methods -->
        <div class="bg-white rounded-lg border border-[#EAE3D9] p-5">
          <h3 class="font-display text-lg text-espresso font-semibold mb-4">Phương thức thanh toán</h3>
          <div class="space-y-2">
            <button
              v-for="m in methods"
              :key="m.id"
              @click="method = m.id"
              class="w-full flex items-center gap-3 p-4 rounded-lg border shadow-card"
              :class="method === m.id ? 'border-caramel bg-[#F5F2ED]' : 'border-[#EAE3D9] bg-white'"
            >
              <div class="w-10 h-10 rounded-lg flex items-center justify-center border border-[#EAE3D9]" 
                   :class="method === m.id ? 'bg-caramel text-white' : 'bg-[#F5F2ED] text-espresso'">
                <Banknote v-if="m.id === 'cash'" class="w-5 h-5" />
                <QrCode v-else class="w-5 h-5" />
              </div>
              <div class="flex-1 text-left">
                <div class="font-medium text-espresso">{{ m.label }}</div>
                <div class="text-xs text-muted-foreground">{{ m.sub }}</div>
              </div>
              <div class="w-5 h-5 rounded-full border flex items-center justify-center"
                   :class="method === m.id ? 'border-caramel bg-caramel' : 'border-[#EAE3D9]'">
                <CheckCircle v-if="method === m.id" class="w-3 h-3 text-white" />
              </div>
            </button>
          </div>

          <!-- QR Code Area cho VietQR & MoMo -->
          <div v-if="method === 'qr' || method === 'momo'" class="mt-6 p-5 rounded-lg bg-[#F5F2ED] border border-[#EAE3D9] text-center">
            <div v-if="loading" class="py-10 text-espresso text-sm bold">Đang tải mã QR thanh toán...</div>
            <div v-else-if="errorMessage" class="py-6 text-red-600 text-sm">{{ errorMessage }}</div>
            <div v-else class="space-y-4">
              <div class="w-48 h-48 mx-auto bg-white rounded-lg flex items-center justify-center border border-[#EAE3D9] overflow-hidden p-2">
                <!-- Mã QR cho MoMo: render EMVCo raw string bằng QrcodeVue -->
                <div v-if="qrRawString" class="w-full h-full flex items-center justify-center">
                  <QrcodeVue :value="qrRawString" :size="160" level="H" render-as="svg" />
                </div>
                <!-- Mã QR cho VietQR: dùng ảnh URL từ img.vietqr.io -->
                <img v-else-if="qrCodeUrl" :src="qrCodeUrl" class="w-full h-full object-contain" alt="QR Code Thanh Toán" />
                <!-- Fallback hiển thị spinner khi đang tải mã QR -->
                <div v-else class="flex flex-col items-center justify-center h-full text-xs text-muted-foreground p-4">
                  <div class="animate-spin w-6 h-6 border-2 border-caramel border-t-transparent rounded-full mb-2"></div>
                  Đang tải mã QR thật...
                </div>
              </div>
              
              <div v-if="payUrl" class="text-center">
                <a :href="payUrl" target="_blank" class="inline-flex items-center gap-1 text-sm font-semibold text-caramel hover:underline">
                  Nhấn vào đây để mở liên kết thanh toán
                </a>
              </div>

              <p class="text-xs text-muted-foreground">
                {{ method === 'qr' ? 'Quét mã bằng App Ngân hàng (VietQR)' : 'Quét mã bằng ứng dụng Ví MoMo' }}
              </p>
              <p class="text-sm font-semibold text-caramel mt-1">Trạng thái: Chờ thanh toán...</p>
            </div>
          </div>
        </div>

        <!-- Pay Button -->
        <Button @click="handlePay" :disabled="loading" class="w-full h-12 bg-caramel text-cream font-semibold rounded-lg border border-caramel/30 shadow-card">
          {{ loading ? 'Đang xử lý...' : (method === 'cash' ? `Xác nhận thanh toán ${formatVND(total)}` : 'Mở liên kết thanh toán') }}
        </Button>
      </div>
    </div>
  </div>

  <!-- OTP Verification Modal for Loyalty Points -->
  <Transition name="login-modal">
    <div
      v-if="otpModalOpen"
      class="fixed inset-0 z-[70] flex items-center justify-center p-4 animate-in fade-in duration-200"
    >
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-[#1A1512]/50 backdrop-blur-sm" @click="cancelOtp"></div>

      <!-- Card -->
      <div class="relative w-full max-w-sm bg-white rounded-3xl shadow-[0_30px_80px_rgba(42,35,30,0.3)] border border-[#EAE3D9] overflow-hidden p-6 text-center space-y-5 animate-in zoom-in-95 duration-200">
        <div class="w-14 h-14 mx-auto bg-[#FDF7EF] rounded-2xl flex items-center justify-center text-[#CC8033]">
          <Gift class="w-7 h-7" />
        </div>
        <div>
          <h3 class="font-premium-serif text-lg font-bold text-espresso">Xác thực OTP đổi điểm</h3>
          <p class="text-xs text-muted-foreground mt-1.5 leading-relaxed">
            Mã xác thực 6 số đã được gửi tới email thành viên của bạn. Vui lòng nhập để xác nhận đổi <strong>50 điểm thưởng</strong>.
          </p>
        </div>

        <div class="space-y-3">
          <input
            type="text"
            v-model="otpCode"
            placeholder="MÃ OTP..."
            maxlength="6"
            class="w-full h-12 text-center text-lg font-bold tracking-[8px] rounded-xl border-2 border-[#EAE3D9] focus:border-[#CC8033] focus:outline-none bg-[#FAF6F0] text-espresso"
          />
          <p v-if="otpError" class="text-xs font-semibold text-red-600">{{ otpError }}</p>
        </div>

        <div class="flex gap-2">
          <button
            @click="cancelOtp"
            class="flex-1 h-11 rounded-xl border-2 border-[#EAE3D9] text-[#5C544E] text-xs font-bold hover:bg-[#FAF6F0] transition-colors"
          >
            Hủy bỏ
          </button>
          <button
            @click="verifyAndRedeem"
            :disabled="otpBusy"
            class="flex-1 h-11 rounded-xl bg-[#CC8033] hover:bg-[#B8722D] text-white text-xs font-bold uppercase transition-colors disabled:opacity-50"
          >
            {{ otpBusy ? 'Đang xử lý...' : 'Xác nhận' }}
          </button>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { ArrowLeft, Banknote, QrCode, CheckCircle, Download, Coffee, Ticket, Sparkles, Gift } from 'lucide-vue-next'
import QrcodeVue from 'qrcode.vue'
import Button from '@/components/ui/Button.vue'
import { formatVND } from '@/data/menu'
import { useOrderStore } from '@/stores/orders'
import { paymentsApi } from '@/services/payments'
import { promotionsApi, type Promotion, type ApplyResult } from '@/services/promotions'
import { loyaltyApi } from '@/services/loyalty'
import { ordersApi } from '@/services/orders'
import { tablesApi } from '@/services/tables'

const route = useRoute()
const orderStore = useOrderStore()
const orderId = String(route.params.orderId || '')

// Kiểm tra xem ID đơn hàng có phải ID thật (kiểu số tự tăng từ API backend)
const isRealOrder = computed(() => {
  const num = parseInt(orderId, 10)
  return !isNaN(num) && String(num) === orderId
})

const orderIdNum = computed(() => {
  return isRealOrder.value ? parseInt(orderId, 10) : 0
})

const realBackendOrderId = ref<number | null>(null)

// Lấy mã đơn hàng thực tế
const effectiveOrderId = computed(() => {
  return isRealOrder.value ? orderIdNum.value : realBackendOrderId.value
})

// Lấy đúng đơn từ store
const order = computed(() => orderStore.getById(orderId))
const items = computed(() => order.value?.items ?? [])
const tableLabel = computed(() => order.value?.table ?? '—')

const methods = [
  { id: "cash", label: "Tiền mặt", sub: "Thanh toán tại quầy" },
  { id: "qr", label: "Chuyển khoản QR", sub: "Quét mã QR ngân hàng" },
  { id: "momo", label: "MoMo", sub: "Ví điện tử MoMo" }
]

const method = ref("qr")
const paid = ref(false)

// Cấu trúc trạng thái tích hợp API
const payUrl = ref<string | null>(null)
const qrCodeUrl = ref<string | null>(null)
const qrRawString = ref<string | null>(null)
const loading = ref(false)
const errorMessage = ref<string | null>(null)
let statusInterval: number | null = null

const subtotal = computed(() => items.value.reduce((s, i) => s + i.qty * i.price, 0))
const voucherCode = ref('')
const voucherError = ref('')
const activePromos = ref<Promotion[]>([])
const appliedPromo = ref<ApplyResult | null>(null)
const promoBusy = ref(false)

const customerProfile = ref<{ id: number; name: string; phone: string; email?: string; tier: string; points: number } | null>(null)
const usePoints = ref(false)
const otpModalOpen = ref(false)
const otpCode = ref('')
const otpError = ref('')
const otpBusy = ref(false)

const loadCustomerProfile = () => {
  const saved = localStorage.getItem('brewCustomerProfile')
  if (saved) {
    try {
      customerProfile.value = JSON.parse(saved)
    } catch (e) {}
  }
}

watch(usePoints, async (newVal) => {
  if (newVal) {
    if (!customerProfile.value || customerProfile.value.points < 50) {
      errorMessage.value = 'Bạn cần tối thiểu 50 điểm thưởng để đổi ưu đãi này.'
      usePoints.value = false
      return
    }
    otpCode.value = ''
    otpError.value = ''
    otpModalOpen.value = true
    await triggerSendOtp()
  } else {
    initPayment()
  }
})

const triggerSendOtp = async () => {
  if (!customerProfile.value) return
  otpBusy.value = true
  try {
    await loyaltyApi.sendPublicOtp(customerProfile.value.id)
  } catch (e: any) {
    errorMessage.value = e.message || 'Không thể gửi mã OTP.'
    otpModalOpen.value = false
    usePoints.value = false
  } finally {
    otpBusy.value = false
  }
}

const verifyAndRedeem = async () => {
  if (!otpCode.value.trim() || !customerProfile.value) return
  otpBusy.value = true
  otpError.value = ''
  try {
    const res = await loyaltyApi.redeemPublicPoints(
      customerProfile.value.id, 
      50, 
      otpCode.value.trim(), 
      effectiveOrderId.value ? effectiveOrderId.value : undefined
    )
    customerProfile.value.points = res.points
    localStorage.setItem('brewCustomerProfile', JSON.stringify(customerProfile.value))
    otpModalOpen.value = false
    
    if (effectiveOrderId.value) {
      await initPayment()
    }
  } catch (e: any) {
    otpError.value = e.message || 'Mã OTP không chính xác.'
    usePoints.value = false
  } finally {
    otpBusy.value = false
  }
}

const cancelOtp = () => {
  otpModalOpen.value = false
  usePoints.value = false
}

const pointsDiscount = computed(() => {
  if (order.value?.pointsDiscount) {
    return order.value.pointsDiscount
  }
  return usePoints.value ? 20000 : 0
})
const promoDiscount = computed(() => {
  if (order.value?.promoDiscount) {
    return order.value.promoDiscount
  }
  return appliedPromo.value?.tienGiam || 0
})
const discount = computed(() => promoDiscount.value + pointsDiscount.value)
const vat = computed(() => Math.round(Math.max(0, subtotal.value - discount.value) * 0.08))
const service = computed(() => (items.value.length ? 10000 : 0))
const total = computed(() => Math.max(0, subtotal.value - discount.value + vat.value + service.value))

const savedVouchers = ref<string[]>([])

const loadSavedVouchers = () => {
  try {
    const key = 'savedVouchers'
    savedVouchers.value = JSON.parse(localStorage.getItem(key) || '[]')
  } catch (e) {
    savedVouchers.value = []
  }
}

function applySavedVoucher(code: string) {
  voucherCode.value = code
  applyVoucher({ code })
}

async function loadActivePromotions() {
  try {
    activePromos.value = await promotionsApi.active()
  } catch (e) {
    console.error('Không tải được danh sách khuyến mãi:', e)
  }
}

async function applyVoucher(opts: { maKhuyenMai?: number; code?: string }) {
  voucherError.value = ''
  promoBusy.value = true
  try {
    const res = await promotionsApi.preview(subtotal.value, opts)
    appliedPromo.value = res
    voucherError.value = ''
    initPayment()
  } catch (e: any) {
    voucherError.value = e.message || 'Mã giảm giá không hợp lệ hoặc không đủ điều kiện.'
    appliedPromo.value = null
  } finally {
    promoBusy.value = false
  }
}

function clearPromo() {
  appliedPromo.value = null
  voucherCode.value = ''
  voucherError.value = ''
  initPayment()
}

// Đồng bộ đơn hàng local sang backend để có ID thực tế
const syncMockOrderToBackend = async () => {
  if (isRealOrder.value) return

  const saved = localStorage.getItem(`backend_order_${orderId}`)
  if (saved) {
    realBackendOrderId.value = parseInt(saved, 10)
    return
  }

  try {
    const realMenu = await ordersApi.menu()
    const apiItems = items.value.map(it => {
      const match = realMenu.find(m => m.tenSanPham.toLowerCase() === it.name.toLowerCase())
      return {
        maSanPham: match ? match.maSanPham : (realMenu[0]?.maSanPham ?? 1),
        maKichCo: null,
        soLuong: it.qty,
        ghiChuMon: it.note || null
      }
    })

    const tablesList = await tablesApi.list()
    const foundTable = tablesList.find(t => t.tenBan.toLowerCase() === tableLabel.value.toLowerCase() || t.tenBan.replace(/\s+/g, '').toLowerCase() === tableLabel.value.replace(/\s+/g, '').toLowerCase())
    let maBan: number | null = foundTable ? foundTable.maBan : null

    // Nếu không tìm thấy bàn nào khớp, lấy bàn đầu tiên có trạng thái "Trong" hoặc bất kỳ bàn nào
    if (!maBan && tablesList.length > 0) {
      maBan = tablesList[0].maBan
    }

    const orderRes = await ordersApi.create({
      maBan,
      items: apiItems,
      ghiChuDonHang: `Khách đặt - Đơn gốc ${orderId}`
    })

    if (orderRes && orderRes.maDonHang) {
      realBackendOrderId.value = orderRes.maDonHang
      localStorage.setItem(`backend_order_${orderId}`, String(orderRes.maDonHang))
    }
  } catch (err) {
    console.error('Lỗi đồng bộ đơn hàng lên Backend:', err)
  }
}

const initPayment = async () => {
  if (!effectiveOrderId.value) {
    // Chưa đồng bộ xong hoặc không có ID
    qrCodeUrl.value = null
    qrRawString.value = null
    payUrl.value = null
    return
  }

  loading.value = true
  errorMessage.value = null
  qrCodeUrl.value = null
  qrRawString.value = null
  payUrl.value = null
  stopStatusPolling()

  try {
    if (method.value === 'momo') {
      const res = await paymentsApi.payMomo({ maDonHang: effectiveOrderId.value, maKhuyenMai: appliedPromo.value?.maKhuyenMai ?? null })
      if (res.success) {
        payUrl.value = res.payUrl
        qrCodeUrl.value = res.qrCodeUrl
        qrRawString.value = res.qrRawString ?? null
        startStatusPolling()
      } else {
        errorMessage.value = res.message
      }
    } else if (method.value === 'qr') {
      const res = await paymentsApi.payVietQr({ maDonHang: effectiveOrderId.value, maKhuyenMai: appliedPromo.value?.maKhuyenMai ?? null })
      if (res.success) {
        payUrl.value = res.payUrl
        qrCodeUrl.value = res.qrCodeUrl
        qrRawString.value = res.qrRawString ?? null
        startStatusPolling()
      } else {
        errorMessage.value = res.message
      }
    } else {
      // Tiền mặt
      stopStatusPolling()
    }
  } catch (err: any) {
    errorMessage.value = err.message || 'Lỗi kết nối API thanh toán.'
  } finally {
    loading.value = false
  }
}

const startStatusPolling = () => {
  stopStatusPolling()
  if (!effectiveOrderId.value) return

  statusInterval = window.setInterval(async () => {
    try {
      const res = await paymentsApi.getStatus(effectiveOrderId.value!)
      if (res.daThanhToan) {
        stopStatusPolling()
        orderStore.markPaid(orderId, method.value)
        paid.value = true
      }
    } catch (err) {
      console.error('Lỗi check status:', err)
    }
  }, 3000)
}

const stopStatusPolling = () => {
  if (statusInterval) {
    clearInterval(statusInterval)
    statusInterval = null
  }
}

const handlePay = async () => {
  if (effectiveOrderId.value) {
    loading.value = true
    errorMessage.value = null
    try {
      if (method.value === 'cash') {
        const res = await paymentsApi.payCash({
          maDonHang: effectiveOrderId.value,
          soTienKhachTra: total.value,
          maKhuyenMai: appliedPromo.value?.maKhuyenMai ?? null
        })
        if (res.success) {
          orderStore.markPaid(orderId, method.value)
          paid.value = true
        } else {
          errorMessage.value = res.message
        }
      } else if (method.value === 'momo' || method.value === 'qr') {
        if (payUrl.value) {
          window.open(payUrl.value, '_blank')
        }
      }
    } catch (err: any) {
      errorMessage.value = err.message || 'Lỗi xử lý thanh toán.'
    } finally {
      loading.value = false
    }
  } else {
    // Fallback nếu không có ID backend (đơn mock bị lỗi mạng)
    orderStore.markPaid(orderId, method.value)
    paid.value = true
  }
}

watch(method, () => {
  initPayment()
})

onMounted(async () => {
  await syncMockOrderToBackend()
  initPayment()
  loadActivePromotions()
  loadCustomerProfile()
  loadSavedVouchers()
})

onUnmounted(() => {
  stopStatusPolling()
})
</script>
