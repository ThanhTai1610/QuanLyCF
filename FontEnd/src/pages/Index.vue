<template>
  <div class="min-h-screen bg-background">
    <header class="max-w-6xl mx-auto px-6 h-20 flex items-center justify-between">
      <div class="flex items-center gap-2">
        <div class="w-10 h-10 rounded-md bg-espresso flex items-center justify-center shadow-soft">
          <Coffee class="w-5 h-5 text-cream" />
        </div>
        <span class="font-display text-xl text-espresso font-semibold">BrewManager</span>
      </div>
      <button
        @click="isPhoneModalOpen = true"
        class="px-4 py-2 rounded-lg bg-caramel hover:bg-brown text-cream text-sm font-semibold transition-colors shadow-warm"
      >
        Đăng nhập tích điểm
      </button>
    </header>

    <main class="max-w-6xl mx-auto px-6 pt-8 pb-20">
      <section class="grid lg:grid-cols-2 gap-10 items-center">
        <div>
          <span class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-caramel-light border border-caramel/20 text-xs font-medium text-brown">
            ☕ Hệ thống dành cho quán cà phê hiện đại
          </span>
          <h1 class="font-display text-5xl md:text-6xl text-espresso mt-5 leading-[1.1] font-semibold">
            Quản lý quán cà phê.<br />
            <span class="text-caramel italic">Đơn giản, ấm áp.</span>
          </h1>
          <p class="text-muted-foreground mt-5 text-lg leading-relaxed max-w-lg">
            BrewManager giúp khách gọi món qua QR, nhân viên xử lý đơn nhanh chóng,
            và chủ quán theo dõi doanh thu mọi lúc — tất cả trong một giao diện được pha chế tỉ mỉ.
          </p>

          <div class="flex flex-wrap gap-3 mt-8">
            <router-link
              to="/menu/5"
              class="inline-flex items-center gap-2 px-5 py-3 rounded-md bg-espresso hover:bg-brown text-cream font-semibold transition-colors shadow-warm"
            >
              <QrCode class="w-4 h-4" /> Thử Menu QR (Bàn 5)
              <ArrowRight class="w-4 h-4" />
            </router-link>
            <button
              @click="isPinModalOpen = true"
              class="inline-flex items-center gap-2 px-5 py-3 rounded-md bg-card hover:bg-caramel-light text-espresso font-semibold border border-cream-deep transition-colors"
            >
              <LayoutDashboard class="w-4 h-4" /> Vào Dashboard
            </button>
            <router-link
              to="/orders"
              class="inline-flex items-center gap-2 px-5 py-3 rounded-md bg-card hover:bg-caramel-light text-espresso font-semibold border border-cream-deep transition-colors"
            >
              <ShoppingBag class="w-4 h-4" /> Quản lý đơn
            </router-link>
          </div>

          <div class="grid grid-cols-3 gap-4 mt-10 pt-8 border-t border-cream-deep">
            <div>
              <div class="font-sans text-3xl text-espresso font-semibold">87</div>
              <div class="text-xs text-muted-foreground mt-0.5">Đơn hôm nay</div>
            </div>
            <div>
              <div class="font-sans text-3xl text-caramel font-semibold">4.2tr</div>
              <div class="text-xs text-muted-foreground mt-0.5">Doanh thu / ngày</div>
            </div>
            <div>
              <div class="font-sans text-3xl text-sage font-semibold">142</div>
              <div class="text-xs text-muted-foreground mt-0.5">Lượt khách</div>
            </div>
          </div>
        </div>

        <div class="relative">
          <div class="absolute -inset-4 bg-caramel/20 rounded-lg blur-2xl" />
          <img
            :src="heroImg"
            alt="Cà phê và bánh ngọt trên bàn ấm cúng"
            width="1280"
            height="800"
            class="relative rounded-lg shadow-warm w-full h-auto object-cover aspect-[4/3]"
          />
          <div class="absolute -bottom-4 -left-4 bg-card rounded-lg p-4 border border-cream-deep shadow-warm hidden md:block">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-success/15 flex items-center justify-center">
                <Coffee class="w-5 h-5 text-success" />
              </div>
              <div>
                <div class="text-xs text-muted-foreground">Đơn vừa hoàn thành</div>
                <div class="font-semibold text-espresso text-sm">Bàn 5 • Cappuccino ×2</div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>

    <!-- PIN Modal Overlay -->
    <div v-if="isPinModalOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card p-6 rounded-xl shadow-warm w-[320px] relative animate-in zoom-in-95 duration-300">
        <button @click="isPinModalOpen = false; enteredPin = ''; pinError = ''" class="absolute top-3 right-3 text-muted-foreground hover:text-espresso transition-colors">
          <X class="w-5 h-5" />
        </button>
        
        <h3 class="text-lg font-display font-bold text-espresso mb-1 text-center">Xác minh truy cập</h3>
        <p class="text-xs text-muted-foreground text-center mb-6">Nhập mã PIN để vào Dashboard</p>
        
        <div class="flex justify-center gap-3 mb-6 h-4">
          <div v-for="i in 4" :key="i" 
               class="w-3.5 h-3.5 rounded-full transition-all duration-300"
               :class="i <= enteredPin.length ? 'bg-caramel shadow-[0_0_8px_rgba(200,133,58,0.5)] scale-110' : 'border-2 border-cream-deep bg-transparent'">
          </div>
        </div>
        
        <div class="h-5 mb-2">
          <p v-if="pinError" class="text-[11px] text-destructive text-center font-bold animate-in slide-in-from-bottom-1">
            {{ pinError }}
          </p>
        </div>
        
        <div class="grid grid-cols-3 gap-2.5">
          <button v-for="n in [1,2,3,4,5,6,7,8,9]" :key="n" @click="handlePinInput(n)" class="aspect-square rounded-xl bg-background border border-cream-deep shadow-sm text-espresso font-bold text-lg hover:bg-caramel hover:border-caramel hover:text-white active:scale-95 transition-all">
            {{ n }}
          </button>
          <button @click="enteredPin = ''; pinError = ''" class="aspect-square rounded-xl text-muted-foreground text-[10px] font-bold uppercase tracking-widest hover:text-destructive active:scale-95 transition-all">
            Xóa
          </button>
          <button @click="handlePinInput(0)" class="aspect-square rounded-xl bg-background border border-cream-deep shadow-sm text-espresso font-bold text-lg hover:bg-caramel hover:border-caramel hover:text-white active:scale-95 transition-all">
            0
          </button>
          <button @click="handlePinBackspace" class="aspect-square flex items-center justify-center rounded-xl text-muted-foreground hover:text-espresso active:scale-95 transition-all">
            <Delete class="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>
    <!-- Phone Modal Overlay -->
    <div v-if="isPhoneModalOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card p-6 rounded-xl shadow-warm w-[320px] relative animate-in zoom-in-95 duration-300">
        <button @click="isPhoneModalOpen = false; phoneNumber = ''; phoneError = ''" class="absolute top-3 right-3 text-muted-foreground hover:text-espresso transition-colors">
          <X class="w-5 h-5" />
        </button>
        
        <h3 class="text-lg font-display font-bold text-espresso mb-1 text-center">Đăng Nhập Tích Điểm</h3>
        <p class="text-xs text-muted-foreground text-center mb-6">Nhập số điện thoại để tiếp tục</p>
        
        <div class="mb-4">
          <input 
            type="tel" 
            v-model="phoneNumber" 
            placeholder="Ví dụ: 0912345678"
            class="w-full h-11 px-4 bg-background border border-cream-deep rounded-lg text-sm font-bold text-center tracking-widest focus:outline-none focus:border-caramel focus:ring-2 focus:ring-caramel/20 text-espresso transition-all"
            @keyup.enter="handlePhoneSubmit"
          />
          <p v-if="phoneError" class="text-[11px] text-destructive font-bold mt-2 animate-in slide-in-from-bottom-1 text-center">
            {{ phoneError }}
          </p>
        </div>
        
        <button 
          @click="handlePhoneSubmit" 
          class="w-full h-11 rounded-lg bg-caramel hover:bg-brown text-cream text-sm font-bold transition-colors uppercase tracking-wider shadow-sm"
        >
          Tiếp tục
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { Coffee, ArrowRight, QrCode, LayoutDashboard, ShoppingBag, X, Delete } from 'lucide-vue-next';
import heroImg from '@/assets/cafe-hero.jpg';

const router = useRouter()
const authStore = useAuthStore()

const isPinModalOpen = ref(false)
const enteredPin = ref('')
const pinError = ref('')

const isPhoneModalOpen = ref(false)
const phoneNumber = ref('')
const phoneError = ref('')

const handlePhoneSubmit = () => {
  if (phoneNumber.value.trim().length >= 9) {
    alert(`Đăng nhập thành công với SĐT: ${phoneNumber.value}`)
    isPhoneModalOpen.value = false
    phoneNumber.value = ''
    phoneError.value = ''
    router.push('/menu/5')
  } else {
    phoneError.value = 'Vui lòng nhập số điện thoại hợp lệ!'
  }
}

const handlePinInput = (num: number) => {
  if (enteredPin.value.length < 4) {
    enteredPin.value += num.toString()
    pinError.value = ''
    if (enteredPin.value.length === 4) {
      setTimeout(() => verifyPin(), 150)
    }
  }
}

const handlePinBackspace = () => {
  if (enteredPin.value.length > 0) {
    enteredPin.value = enteredPin.value.slice(0, -1)
    pinError.value = ''
  }
}

const verifyPin = () => {
  if (enteredPin.value === '2006') {
    isPinModalOpen.value = false
    enteredPin.value = ''
    router.push('/login')
  } else {
    pinError.value = 'Mã PIN không đúng. Vui lòng thử lại!'
    enteredPin.value = ''
  }
}
</script>
