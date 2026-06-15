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
            <div class="flex justify-between text-muted-foreground"><span>VAT 8%</span><span>{{ formatVND(vat) }}</span></div>
            <div class="flex justify-between text-muted-foreground"><span>Phí phục vụ</span><span>{{ formatVND(service) }}</span></div>
            <div class="flex justify-between font-display text-2xl text-caramel font-semibold pt-2 border-t-2 border-[#EAE3D9]">
              <span>Tổng cộng</span>
              <span>{{ formatVND(total) }}</span>
            </div>
          </div>
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

          <!-- QR Code Area -->
          <div v-if="method === 'qr'" class="mt-6 p-5 rounded-lg bg-[#F5F2ED] border border-[#EAE3D9] text-center">
            <div class="w-40 h-40 mx-auto bg-white rounded-lg flex items-center justify-center border border-[#EAE3D9]">
              <!-- Giả lập QR -->
              <div class="grid grid-cols-12 gap-px p-3">
                <div v-for="i in 144" :key="i" class="w-2 h-2" :class="Math.random() > 0.5 ? 'bg-espresso' : 'bg-white'" />
              </div>
            </div>
            <p class="text-xs text-muted-foreground mt-4">Quét mã bằng app ngân hàng</p>
            <p class="text-sm font-semibold text-caramel mt-1">Hết hạn sau {{ fmt(countdown) }}</p>
          </div>
        </div>

        <!-- Pay Button -->
        <Button @click="handlePay" class="w-full h-12 bg-caramel text-cream font-semibold rounded-lg border border-caramel/30 shadow-card">
          Hoàn tất thanh toán {{ formatVND(total) }}
        </Button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { ArrowLeft, Banknote, QrCode, CheckCircle, Download, Coffee } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import { formatVND } from '@/data/menu'
import { useOrderStore } from '@/stores/orders'

const route = useRoute()
const orderStore = useOrderStore()
const orderId = String(route.params.orderId || '')

// Lấy đúng đơn mà khách vừa gửi từ store
const order = computed(() => orderStore.getById(orderId))
const items = computed(() => order.value?.items ?? [])
const tableLabel = computed(() => order.value?.table ?? '—')

const methods = [
  { id: "cash", label: "Tiền mặt", sub: "Thanh toán tại quầy" },
  { id: "qr", label: "Chuyển khoản QR", sub: "Quét mã QR ngân hàng" },
  { id: "momo", label: "MoMo", sub: "Ví điện tử MoMo" },
  { id: "vnpay", label: "VNPay", sub: "Cổng thanh toán VNPay" },
  { id: "zalopay", label: "ZaloPay", sub: "Ví ZaloPay" },
]

const method = ref("qr")
const paid = ref(false)
const countdown = ref(298)

const subtotal = computed(() => items.value.reduce((s, i) => s + i.qty * i.price, 0))
const vat = computed(() => Math.round(subtotal.value * 0.08))
const service = computed(() => (items.value.length ? 10000 : 0))
const total = computed(() => subtotal.value + vat.value + service.value)

const fmt = (s: number) => `${Math.floor(s / 60)}:${String(s % 60).padStart(2, "0")}`

const handlePay = () => {
  orderStore.markPaid(orderId, method.value)
  paid.value = true
}
</script>
