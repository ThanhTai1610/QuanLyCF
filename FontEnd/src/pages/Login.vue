<template>
  <div class="min-h-screen flex items-center justify-center px-4 py-10 bg-[#FAF6F0] font-premium-sans text-[#2A231E] relative overflow-hidden">
    
    <!-- Ambient Background Depth -->
    <div class="absolute inset-0 pointer-events-none">
      <div class="absolute top-[-20%] left-[-10%] w-[60%] h-[60%] rounded-full bg-[#CC8033]/5 blur-[150px]"></div>
      <div class="absolute bottom-[-20%] right-[-10%] w-[60%] h-[60%] rounded-full bg-[#2A231E]/5 blur-[150px]"></div>
    </div>

    <!-- Main Container -->
    <div class="w-full max-w-[880px] bg-white rounded-[2rem] shadow-[0_50px_100px_-20px_rgba(42,35,30,0.12)] flex flex-col md:flex-row overflow-hidden border border-[#EAE3D9]/50 relative z-10 animate-in fade-in zoom-in-95 duration-1000">
      
      <!-- Left Side: Brand Showcase -->
      <div class="w-full md:w-[38%] bg-[#2A231E] p-8 md:p-12 flex flex-col items-center justify-center text-center relative overflow-hidden">
        <!-- Decorative subtle pattern -->
        <div class="absolute inset-0 opacity-[0.03] pointer-events-none bg-[url('https://www.transparenttextures.com/patterns/cubes.png')]"></div>

        <!-- Admin Badge -->
        <div class="absolute top-6 left-0 right-0 flex justify-center">
          <div class="flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-[#CC8033]/15 border border-[#CC8033]/25">
            <div class="w-1.5 h-1.5 rounded-full bg-[#CC8033] animate-pulse"></div>
            <span class="text-[9px] uppercase tracking-[0.25em] text-[#CC8033] font-black">Admin Portal</span>
          </div>
        </div>
        
        <div class="relative z-10 space-y-6 animate-in slide-in-from-left-8 duration-1000 delay-300">
          <div class="w-16 h-16 rounded-lg bg-[#CC8033] mx-auto flex items-center justify-center shadow-[0_15px_30px_rgba(204,128,51,0.25)] border border-white/10">
            <Coffee class="w-8 h-8 text-[#2A231E]" stroke-width="1.2" />
          </div>
          
          <div class="space-y-3">
            <h2 class="font-premium-serif text-3xl font-bold tracking-[0.1em] text-[#FDFBF7]">{{ storeInfoStore.tenQuan.toUpperCase() }}</h2>
            <div class="w-10 h-[1px] bg-[#CC8033] mx-auto opacity-60"></div>
            <p class="text-[10px] uppercase tracking-[0.3em] text-[#8A8178] font-bold leading-relaxed">
              Hệ thống quản trị và <br/> vận hành toàn diện
            </p>
          </div>

          <!-- Security info -->
          <div class="mt-6 space-y-2">
            <div v-for="item in securityFeatures" :key="item" class="flex items-center gap-2 text-left">
              <Shield class="w-3 h-3 text-[#CC8033] shrink-0" stroke-width="2" />
              <span class="text-[9px] text-[#8A8178] font-medium tracking-wide">{{ item }}</span>
            </div>
          </div>
        </div>

        <!-- Bottom brand mark -->
        <div class="absolute bottom-8 left-0 right-0 opacity-10 text-[9px] uppercase tracking-[0.8em] font-black text-white">
          EST. 2026
        </div>
      </div>

      <!-- Right Side: Admin Login Form -->
      <div class="flex-1 p-8 md:p-12 lg:p-16 bg-white flex flex-col justify-center relative animate-in slide-in-from-right-8 duration-1000 delay-300">
        <!-- Nút Trở về Trang Chính -->
        <router-link to="/" class="absolute top-6 right-6 flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-[#8A8178] hover:text-[#CC8033] hover:bg-[#CC8033]/10 transition-colors text-[9px] font-bold uppercase tracking-widest border border-transparent hover:border-[#CC8033]/20">
          <Home class="w-3.5 h-3.5" stroke-width="2.5" /> Trang chính
        </router-link>

        <div class="mb-8">
          <div class="inline-flex items-center gap-2 mb-4 px-3 py-1.5 rounded-lg bg-[#CC8033]/10 border border-[#CC8033]/20">
            <Lock class="w-3 h-3 text-[#CC8033]" stroke-width="2.5" />
            <span class="text-[9px] uppercase tracking-[0.2em] text-[#CC8033] font-black">Truy cập hạn chế</span>
          </div>
          <h1 class="font-premium-serif text-4xl lg:text-5xl font-bold text-[#1A1512] tracking-tight leading-[1.1] mb-3">Đăng Nhập<br/>Hệ Thống</h1>
          <p class="text-sm font-medium text-[#8A8178]">Vui lòng nhập thông tin để tiếp tục.</p>
        </div>

        <!-- Tabs -->
        <div class="flex items-center gap-6 border-b border-[#EAE3D9] mb-8">
          <button 
            @click="activeTab = 'admin'"
            type="button"
            :class="[
              'pb-3 text-[10px] font-black uppercase tracking-widest transition-all duration-300 relative',
              activeTab === 'admin' ? 'text-[#CC8033]' : 'text-[#8A8178] hover:text-[#2A231E]'
            ]"
          >
            Tài khoản Admin
            <div v-if="activeTab === 'admin'" class="absolute bottom-0 left-0 w-full h-[2px] bg-[#CC8033]"></div>
          </button>
          <button 
            @click="activeTab = 'staff'"
            type="button"
            :class="[
              'pb-3 text-[10px] font-black uppercase tracking-widest transition-all duration-300 relative',
              activeTab === 'staff' ? 'text-[#CC8033]' : 'text-[#8A8178] hover:text-[#2A231E]'
            ]"
          >
            Nhân viên ca trực
            <div v-if="activeTab === 'staff'" class="absolute bottom-0 left-0 w-full h-[2px] bg-[#CC8033]"></div>
          </button>
        </div>

        <!-- Form -->
        <form :key="activeTab" @submit.prevent="handleSubmit" class="space-y-6 animate-in fade-in slide-in-from-right-4 duration-500">
          <!-- Email -->
          <div class="space-y-2 group">
            <label for="email" class="text-[10px] uppercase tracking-widest text-[#5C544E] font-black group-focus-within:text-[#CC8033] transition-colors ml-1">Tài khoản Email</label>
            <input
              id="email"
              type="email"
              v-model="email"
              class="w-full bg-[#FAF6F0] border border-[#EAE3D9] rounded-2xl px-4 h-14 focus:outline-none focus:ring-4 focus:ring-[#CC8033]/10 focus:border-[#CC8033] text-[15px] font-semibold text-[#1A1512] transition-all duration-300 placeholder:text-[#C5BEB8] placeholder:font-medium"
              placeholder="admin@brew.vn"
              required
            />
          </div>

          <!-- Password -->
          <div class="space-y-2 group">
            <label for="password" class="text-[10px] uppercase tracking-widest text-[#5C544E] font-black group-focus-within:text-[#CC8033] transition-colors ml-1">Mật khẩu</label>
            <div class="relative">
              <input
                id="password"
                :type="showPwd ? 'text' : 'password'"
                v-model="password"
                class="w-full bg-[#FAF6F0] border border-[#EAE3D9] rounded-2xl px-4 h-14 pr-12 focus:outline-none focus:ring-4 focus:ring-[#CC8033]/10 focus:border-[#CC8033] text-[15px] font-semibold text-[#1A1512] transition-all duration-300 placeholder:text-[#C5BEB8] placeholder:font-medium"
                placeholder="••••••••"
                required
              />
              <button
                type="button"
                @click="showPwd = !showPwd"
                class="absolute right-3 top-1/2 -translate-y-1/2 w-8 h-8 flex items-center justify-center text-[#A89F95] hover:text-[#CC8033] hover:bg-white rounded-xl transition-all"
              >
                <EyeOff v-if="showPwd" class="w-4 h-4" stroke-width="2" />
                <Eye v-else class="w-4 h-4" stroke-width="2" />
              </button>
            </div>
          </div>

          <div class="flex items-center justify-between pt-1">
            <label class="flex items-center gap-2.5 cursor-pointer group/check">
              <div class="relative flex items-center justify-center">
                <input type="checkbox" v-model="rememberMe" class="sr-only peer" />
                <div class="w-5 h-5 border-2 border-[#EAE3D9] rounded-md bg-white peer-checked:bg-[#CC8033] peer-checked:border-[#CC8033] transition-all duration-300 shadow-sm"></div>
                <Check class="w-3.5 h-3.5 text-white absolute opacity-0 peer-checked:opacity-100 transition-opacity" stroke-width="4" />
              </div>
              <span class="text-xs font-bold text-[#8A8178] group-hover/check:text-[#2A231E] transition-colors">Ghi nhớ tài khoản</span>
            </label>
          </div>

          <p v-if="errorMsg" class="text-xs font-bold text-red-600 bg-red-50 border border-red-200 rounded-xl px-4 py-3">
            {{ errorMsg }}
          </p>

          <div class="pt-4">
            <button
              type="submit"
              :disabled="isLoading"
              class="w-full h-14 bg-[#2A231E] hover:bg-[#1A1512] text-[#FDFBF7] text-sm uppercase tracking-[0.15em] font-bold rounded-2xl shadow-[0_20px_40px_-10px_rgba(42,35,30,0.3)] transition-all duration-300 active:scale-[0.98] group relative overflow-hidden disabled:opacity-70 flex items-center justify-center"
            >
              <span class="relative z-10 flex items-center gap-3">
                <span v-if="isLoading" class="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                <span v-else>Đăng nhập vào hệ thống</span>
                <ArrowRight v-if="!isLoading" class="w-4 h-4 group-hover:translate-x-1 transition-transform" stroke-width="2.5" />
              </span>
              <div class="absolute inset-0 bg-gradient-to-r from-transparent via-white/10 to-transparent -translate-x-full group-hover:animate-[shimmer_2s_infinite]"></div>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Coffee, Eye, EyeOff, ArrowRight, Check, Lock, Shield, Users, Home } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'
import { useStoreInfoStore } from '@/stores/storeInfo'

const activeTab = ref<'admin'|'staff'>('admin')
const showPwd = ref(false)
const email = ref('admin@brew.vn')
const password = ref('demo1234')
const rememberMe = ref(false)
const isLoading = ref(false)
const router         = useRouter()
const authStore      = useAuthStore()
const storeInfoStore = useStoreInfoStore()

watch(activeTab, (newTab) => {
  if (newTab === 'admin') {
    email.value = 'admin@brew.vn'
    password.value = 'demo1234'
  } else {
    email.value = 'staff@brew.vn'
    password.value = 'staff1234'
  }
})

onMounted(() => {
  const savedEmail = localStorage.getItem('brew_remember_email')
  const savedPwd = localStorage.getItem('brew_remember_pwd')
  if (savedEmail && savedPwd) {
    email.value = savedEmail
    password.value = atob(savedPwd)
    rememberMe.value = true
  }
})

const securityFeatures = [
  'Kết nối mã hóa SSL/TLS',
  'Xác thực hai yếu tố (2FA)',
  'Phiên đăng nhập tự động hết hạn',
]

const errorMsg = ref('')

const handleSubmit = async () => {
  isLoading.value = true
  errorMsg.value = ''
  try {
    await authStore.login(email.value, password.value)
    
    // Ghi nhớ tài khoản
    if (rememberMe.value) {
      localStorage.setItem('brew_remember_email', email.value)
      localStorage.setItem('brew_remember_pwd', btoa(password.value))
    } else {
      localStorage.removeItem('brew_remember_email')
      localStorage.removeItem('brew_remember_pwd')
    }

    router.push('/revenue-report')
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Đăng nhập thất bại, vui lòng thử lại.'
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans {
  font-family: 'Be Vietnam Pro', system-ui, sans-serif;
}

@keyframes shimmer {
  100% {
    transform: translateX(100%);
  }
}

input:focus {
  outline: none;
}

input:-webkit-autofill,
input:-webkit-autofill:hover,
input:-webkit-autofill:focus {
  -webkit-box-shadow: 0 0 0px 1000px white inset;
  transition: background-color 5000s ease-in-out 0s;
}
</style>