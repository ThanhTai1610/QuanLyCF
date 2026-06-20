<template>
  <div class="min-h-screen bg-[#FAF6F0] font-premium-sans text-[#2A231E] relative overflow-hidden flex flex-col">

    <!-- Ambient background -->
    <div class="absolute inset-0 pointer-events-none">
      <div class="absolute top-[-15%] right-[-10%] w-[50%] h-[50%] rounded-full bg-[#CC8033]/6 blur-[120px]"></div>
      <div class="absolute bottom-[-20%] left-[-10%] w-[50%] h-[50%] rounded-full bg-[#2A231E]/4 blur-[120px]"></div>
    </div>

    <!-- Top bar -->
    <header class="relative z-10 flex items-center justify-between px-6 py-5">
      <div class="flex items-center gap-3">
        <div class="w-9 h-9 rounded-lg bg-[#2A231E] flex items-center justify-center shadow-lg">
          <Coffee class="w-4.5 h-4.5 text-[#CC8033]" stroke-width="1.5" />
        </div>
        <span class="font-premium-serif text-xl font-bold text-[#2A231E] tracking-wide">{{ storeInfoStore.tenQuan }}</span>
      </div>
      <div class="flex items-center gap-2 px-3 py-1.5 bg-white rounded-lg border border-[#EAE3D9] shadow-sm">
        <div class="w-1.5 h-1.5 rounded-full bg-emerald-400 animate-pulse"></div>
        <span class="text-[9px] uppercase tracking-[0.2em] text-[#5C544E] font-bold">Máy POS #1</span>
      </div>
    </header>

    <!-- Main Content -->
    <main class="relative z-10 flex-1 flex flex-col items-center justify-center px-4 py-6 gap-8">

      <!-- Staff Avatar Grid -->
      <section class="w-full max-w-md">
        <p class="text-center text-[9px] uppercase tracking-[0.25em] text-[#8A8178] font-bold mb-4">Nhân viên trong ca hôm nay</p>
        <div class="grid grid-cols-4 gap-3">
          <button
            v-for="s in staffOnShift"
            :key="s.id"
            @click="selectStaff(s)"
            :class="[
              'flex flex-col items-center gap-2 p-3 rounded-xl border-2 transition-all duration-300',
              selectedStaff?.id === s.id
                ? 'border-[#CC8033] bg-[#CC8033]/8 scale-[1.03] shadow-md'
                : 'border-transparent bg-white hover:border-[#EAE3D9] hover:shadow-sm'
            ]"
          >
            <div
              :class="[
                'w-12 h-12 rounded-full flex items-center justify-center text-lg font-bold shadow-sm border-2 transition-all duration-300',
                selectedStaff?.id === s.id
                  ? 'border-[#CC8033] text-white bg-[#CC8033]'
                  : 'border-[#EAE3D9] text-[#2A231E] bg-[#F5F2ED]'
              ]"
            >
              {{ s.initials }}
            </div>
            <span :class="['text-[9px] font-bold text-center leading-tight transition-colors duration-300 uppercase tracking-tight', selectedStaff?.id === s.id ? 'text-[#CC8033]' : 'text-[#5C544E]']">
              {{ s.name.split(' ').slice(-1)[0] }}
            </span>
          </button>
        </div>
      </section>

      <!-- PIN Entry Panel -->
      <section class="w-full max-w-xs">
        <!-- Prompt text -->
        <div class="text-center mb-5">
          <p v-if="!selectedStaff" class="text-sm text-[#8A8178] font-medium">Chọn nhân viên để đăng nhập</p>
          <div v-else class="space-y-1">
            <p class="text-xs text-[#8A8178] font-medium uppercase tracking-widest">Xin chào,</p>
            <p class="font-premium-serif text-2xl font-bold text-[#2A231E]">{{ selectedStaff.name }}</p>
            <p class="text-[10px] text-[#CC8033] font-bold uppercase tracking-[0.15em]">Nhập mã PIN định danh</p>
          </div>
        </div>

        <!-- PIN Dots -->
        <div class="flex items-center justify-center gap-3 mb-6 h-8">
          <template v-if="selectedStaff">
            <div
              v-for="i in pinLength"
              :key="i"
              :class="[
                'rounded-full transition-all duration-300',
                i <= pin.length
                  ? 'w-3.5 h-3.5 bg-[#CC8033] shadow-[0_0_8px_rgba(204,128,51,0.4)]'
                  : 'w-3 h-3 border-2 border-[#D5CEC4] bg-transparent'
              ]"
            ></div>
          </template>
          <template v-else>
            <div v-for="i in pinLength" :key="i" class="w-3 h-3 rounded-full border-2 border-[#EAE3D9]"></div>
          </template>
        </div>

        <!-- Error message -->
        <div v-if="errorMsg" class="mb-4 text-center">
          <p class="text-[11px] text-red-500 font-bold animate-in fade-in-0 zoom-in-95 duration-300">{{ errorMsg }}</p>
        </div>

        <!-- Numpad -->
        <div class="bg-white rounded-2xl border border-[#EAE3D9] shadow-[0_15px_40px_-10px_rgba(42,35,30,0.1)] p-4">
          <div class="grid grid-cols-3 gap-3">
            <!-- Numbers 1-9 -->
            <button
              v-for="n in [1,2,3,4,5,6,7,8,9]"
              :key="n"
              @click="pressKey(String(n))"
              :disabled="!selectedStaff"
              class="numpad-btn"
            >
              {{ n }}
            </button>

            <!-- Bottom row: clear, 0, backspace -->
            <button
              @click="clearPin"
              :disabled="!selectedStaff"
              class="numpad-btn text-sm font-bold text-[#8A8178] tracking-wide"
            >
              XÓA
            </button>
            <button
              @click="pressKey('0')"
              :disabled="!selectedStaff"
              class="numpad-btn"
            >
              0
            </button>
            <button
              @click="backspace"
              :disabled="!selectedStaff"
              class="numpad-btn flex items-center justify-center"
            >
              <Delete class="w-5 h-5 text-[#8A8178]" stroke-width="1.5" />
            </button>
          </div>
        </div>

        <!-- Actions -->
        <div class="mt-6 flex flex-col items-center justify-center gap-4">
          <router-link to="/" class="group flex items-center gap-1.5 text-[10px] font-bold text-[#8A8178] hover:text-[#CC8033] transition-all uppercase tracking-widest">
            <ArrowLeft class="w-3.5 h-3.5 group-hover:-translate-x-1 transition-transform" />
            Quay lại trang chủ
          </router-link>
        </div>
      </section>
    </main>

    <!-- Footer time -->
    <footer class="relative z-10 text-center pb-6">
      <p class="text-[9px] text-[#C5BEB8] uppercase tracking-[0.3em] font-medium">{{ currentTime }}</p>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { Coffee, Delete, ArrowLeft } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { useStoreInfoStore } from '@/stores/storeInfo'

const router         = useRouter()
const authStore      = useAuthStore()
const storeInfoStore = useStoreInfoStore()

interface Staff {
  id: number
  name: string
  initials: string
  pin: string
  role: string
  redirect: string
}

const staffOnShift: Staff[] = [
  { id: 1, name: 'Nguyễn Hà', initials: 'NH', pin: '1234', role: 'Barista', redirect: '/orders' },
  { id: 2, name: 'Trần Minh', initials: 'TM', pin: '5678', role: 'Thu Ngân', redirect: '/orders' },
  { id: 3, name: 'Lê Thu', initials: 'LT', pin: '9012', role: 'Phục vụ', redirect: '/orders' },
  { id: 4, name: 'Phạm An', initials: 'PA', pin: '3456', role: 'Bếp', redirect: '/kitchen' },
  { id: 5, name: 'Võ Lan', initials: 'VL', pin: '7890', role: 'Barista', redirect: '/orders' },
  { id: 6, name: 'Đỗ Hùng', initials: 'DH', pin: '2468', role: 'Bếp', redirect: '/kitchen' },
  { id: 7, name: 'Bùi Mai', initials: 'BM', pin: '1357', role: 'Phục vụ', redirect: '/orders' },
  { id: 8, name: 'Hoàng Bảo', initials: 'HB', pin: '8642', role: 'Thu Ngân', redirect: '/orders' },
]

const pinLength = 4
const selectedStaff = ref<Staff | null>(null)
const pin = ref('')
const errorMsg = ref('')
const currentTime = ref('')

let timer: ReturnType<typeof setInterval>

const updateTime = () => {
  const now = new Date()
  currentTime.value = now.toLocaleString('vi-VN', {
    weekday: 'long', hour: '2-digit', minute: '2-digit', second: '2-digit'
  })
}

onMounted(() => {
  updateTime()
  timer = setInterval(updateTime, 1000)
})

onUnmounted(() => clearInterval(timer))

const selectStaff = (s: Staff) => {
  selectedStaff.value = s
  pin.value = ''
  errorMsg.value = ''
}

const pressKey = (key: string) => {
  if (!selectedStaff.value) return
  if (pin.value.length >= pinLength) return
  errorMsg.value = ''
  pin.value += key

  if (pin.value.length === pinLength) {
    verifyPin()
  }
}

const backspace = () => {
  if (pin.value.length > 0) {
    pin.value = pin.value.slice(0, -1)
    errorMsg.value = ''
  }
}

const clearPin = () => {
  pin.value = ''
  errorMsg.value = ''
}

const verifyPin = async () => {
  const staff = selectedStaff.value!
  try {
    await authStore.loginPin(staff.id, pin.value)
    router.push(staff.redirect)
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Mã PIN không đúng. Vui lòng thử lại!'
    pin.value = ''
  }
}
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans { font-family: 'Be Vietnam Pro', system-ui, sans-serif; }

.numpad-btn {
  @apply w-full aspect-square flex items-center justify-center
         text-xl font-bold text-[#2A231E] rounded-xl
         bg-[#FAF6F0] border border-[#EAE3D9]
         hover:bg-[#CC8033] hover:text-white hover:border-[#CC8033]
         active:scale-[0.93] transition-all duration-150
         disabled:opacity-30 disabled:cursor-not-allowed disabled:hover:bg-[#FAF6F0] disabled:hover:text-[#2A231E];
}
</style>
