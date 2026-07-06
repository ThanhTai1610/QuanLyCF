<template>
  <div class="fixed bottom-6 right-[200px] z-50">
    <!-- Backdrop when open -->
    <Transition name="fade">
      <div v-if="isOpen" class="fixed inset-0 bg-black/20 backdrop-blur-xs z-40" @click="isOpen = false"></div>
    </Transition>

    <!-- Expanded Menu -->
    <Transition name="pop">
      <div v-if="isOpen" class="absolute bottom-16 right-0 w-[280px] bg-white rounded-3xl border border-[#EAE3D9] shadow-[0_20px_50px_rgba(42,35,30,0.15)] p-5 z-50 flex flex-col gap-3">
        <h3 class="font-premium-serif text-base font-bold text-[#2A231E] border-b border-[#F5F2ED] pb-2">
          Hỗ trợ tại bàn số {{ tableId }}
        </h3>
        
        <div class="flex flex-col gap-2">
          <!-- Option 1: Gọi phục vụ -->
          <button @click="sendRequest('GoiPhucVu', 'Yêu cầu phục vụ tại bàn')" :disabled="loading" 
            class="flex items-center gap-3 p-3 rounded-xl bg-[#FAF6F0] hover:bg-[#FFF9F2] text-[#2A231E] hover:text-[#CC8033] border border-[#EAE3D9]/60 hover:border-[#CC8033]/30 transition-all text-left">
            <div class="w-8 h-8 rounded-lg bg-[#CC8033]/10 flex items-center justify-center shrink-0 text-[#CC8033]">
              <Bell class="w-4 h-4" />
            </div>
            <div>
              <p class="text-xs font-bold">Gọi nhân viên</p>
              <p class="text-[10px] text-[#8A8178]">Lấy đá, khăn giấy, dọn bàn...</p>
            </div>
          </button>

          <!-- Option 2: Thanh toán tiền mặt -->
          <button @click="sendRequest('ThanhToanTienMat', 'Yêu cầu thanh toán tiền mặt')" :disabled="loading" 
            class="flex items-center gap-3 p-3 rounded-xl bg-[#FAF6F0] hover:bg-[#FFF9F2] text-[#2A231E] hover:text-[#CC8033] border border-[#EAE3D9]/60 hover:border-[#CC8033]/30 transition-all text-left">
            <div class="w-8 h-8 rounded-lg bg-[#CC8033]/10 flex items-center justify-center shrink-0 text-[#CC8033]">
              <DollarSign class="w-4 h-4" />
            </div>
            <div>
              <p class="text-xs font-bold">Thanh toán Tiền mặt</p>
              <p class="text-[10px] text-[#8A8178]">Yêu cầu nhân viên mang hóa đơn</p>
            </div>
          </button>

          <!-- Option 3: Thanh toán chuyển khoản -->
          <button @click="sendRequest('ThanhToanChuyenKhoan', 'Yêu cầu thanh toán chuyển khoản (Momo/QR)')" :disabled="loading" 
            class="flex items-center gap-3 p-3 rounded-xl bg-[#FAF6F0] hover:bg-[#FFF9F2] text-[#2A231E] hover:text-[#CC8033] border border-[#EAE3D9]/60 hover:border-[#CC8033]/30 transition-all text-left">
            <div class="w-8 h-8 rounded-lg bg-[#CC8033]/10 flex items-center justify-center shrink-0 text-[#CC8033]">
              <CreditCard class="w-4 h-4" />
            </div>
            <div>
              <p class="text-xs font-bold">Thanh toán Chuyển khoản</p>
              <p class="text-[10px] text-[#8A8178]">Momo, ZaloPay, QR ngân hàng</p>
            </div>
          </button>
        </div>

        <!-- Custom Request Text -->
        <div class="mt-1">
          <input v-model="noteText" placeholder="Ghi chú khác (ví dụ: Lấy thêm 2 cốc đá...)" @keyup.enter="submitCustomRequest"
            class="w-full h-9 px-3 rounded-lg border border-[#EAE3D9] text-[11px] font-medium focus:border-[#CC8033] focus:outline-none bg-[#FAF6F0] text-[#2A231E]" />
        </div>
      </div>
    </Transition>

    <!-- FAB Trigger Button -->
    <button @click="isOpen = !isOpen"
      class="w-14 h-14 rounded-full bg-gradient-to-r from-[#CC8033] to-[#D97724] text-white flex items-center justify-center shadow-lg shadow-[#CC8033]/30 hover:scale-105 active:scale-95 transition-all z-50 relative">
      <X v-if="isOpen" class="w-6 h-6" />
      <div v-else class="relative">
        <Bell class="w-6 h-6 animate-swing" />
        <span class="absolute -top-1 -right-1 flex h-2 w-2">
          <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-white opacity-75"></span>
          <span class="relative inline-flex rounded-full h-2 w-2 bg-white"></span>
        </span>
      </div>
    </button>

    <!-- Success message toast inside component -->
    <TransitionGroup name="toast">
      <div v-if="successMsg" class="fixed top-6 left-1/2 -translate-x-1/2 bg-[#2A231E]/95 border border-[#CC8033]/30 text-white rounded-2xl p-4 shadow-xl z-[99] flex items-center gap-3 max-w-[340px] w-[90%]">
        <div class="w-8 h-8 rounded-full bg-green-500/20 text-green-400 flex items-center justify-center shrink-0">
          <Check class="w-4 h-4" />
        </div>
        <p class="text-xs font-semibold leading-normal">{{ successMsg }}</p>
      </div>
    </TransitionGroup>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Bell, DollarSign, CreditCard, X, Check } from 'lucide-vue-next'
import { ordersApi } from '@/services/orders'

const props = defineProps({
  tableId: {
    type: [String, Number],
    default: '5'
  }
})

const isOpen = ref(false)
const loading = ref(false)
const noteText = ref('')
const successMsg = ref('')

async function sendRequest(type: string, note: string) {
  loading.value = true
  try {
    await ordersApi.createServiceRequest({
      maBan: parseInt(props.tableId.toString()),
      loaiYeuCau: type,
      ghiChu: note
    })
    
    successMsg.value = type === 'GoiPhucVu' 
      ? 'Đã gửi yêu cầu gọi phục vụ! Nhân viên đang đến.' 
      : 'Yêu cầu thanh toán đã được gửi đi.'
      
    isOpen.value = false
    noteText.value = ''
    
    setTimeout(() => {
      successMsg.value = ''
    }, 4500)
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

function submitCustomRequest() {
  if (!noteText.value.trim()) return
  sendRequest('GoiPhucVu', noteText.value.trim())
}
</script>

<style scoped>
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

.pop-enter-active, .pop-leave-active {
  transition: transform 0.25s cubic-bezier(0.34, 1.56, 0.64, 1), opacity 0.2s ease;
  transform-origin: bottom right;
}
.pop-enter-from, .pop-leave-to {
  opacity: 0;
  transform: scale(0.85) translateY(10px);
}

@keyframes swing {
  0%, 100% { transform: rotate(0); }
  20% { transform: rotate(15deg); }
  40% { transform: rotate(-15deg); }
  60% { transform: rotate(10deg); }
  80% { transform: rotate(-10deg); }
}
.animate-swing {
  animation: swing 2.5s infinite ease-in-out;
}
</style>
