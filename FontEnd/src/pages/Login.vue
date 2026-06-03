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
            <h2 class="font-premium-serif text-3xl font-bold tracking-[0.1em] text-[#FDFBF7]">BREWMANAGER</h2>
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
      <div class="flex-1 p-8 md:p-12 bg-white flex flex-col justify-center animate-in slide-in-from-right-8 duration-1000 delay-300">
        <div class="mb-10">
          <div class="inline-flex items-center gap-2 mb-4 px-2.5 py-1 rounded-md bg-[#CC8033]/8 border border-[#CC8033]/15">
            <Lock class="w-2.5 h-2.5 text-[#CC8033]" stroke-width="2.5" />
            <span class="text-[8px] uppercase tracking-[0.2em] text-[#CC8033] font-black">Truy cập hạn chế</span>
          </div>
          <h1 class="font-premium-serif text-4xl font-semibold text-[#1A1512] tracking-tight">Cổng Quản Trị<br/>Hệ Thống</h1>
          <p class="text-[11px] text-[#8A8178] mt-3 font-medium leading-relaxed">Vui lòng nhập thông tin để truy cập hệ thống quản lý BrewManager.</p>
        </div>

        <form @submit.prevent="handleSubmit" class="space-y-6">
          <!-- Email Input -->
          <div class="space-y-1.5 group">
            <label for="email" class="text-[9px] uppercase tracking-widest text-[#8A8178] font-black group-focus-within:text-[#CC8033] transition-colors duration-300">Tài khoản Email</label>
            <div class="relative">
              <input
                id="email"
                type="email"
                v-model="email"
                class="w-full bg-transparent border-0 border-b border-[#EAE3D9] rounded-none px-0 focus:outline-none focus:ring-0 focus:border-[#CC8033] h-10 text-sm font-medium shadow-none transition-all duration-500 placeholder:text-[#D5CEC4] placeholder:font-normal"
                placeholder="admin@brew.vn"
                required
              />
              <div class="absolute bottom-0 left-0 h-[1.5px] bg-[#CC8033] w-0 group-focus-within:w-full transition-all duration-700"></div>
            </div>
          </div>

          <!-- Password Input -->
          <div class="space-y-1.5 group">
            <div class="flex justify-between items-end">
              <label for="password" class="text-[9px] uppercase tracking-widest text-[#8A8178] font-black group-focus-within:text-[#CC8033] transition-colors duration-300">Mật khẩu</label>
              <a href="#" class="text-[9px] font-bold text-[#CC8033] hover:text-[#1A1512] transition-colors">Quên mật khẩu?</a>
            </div>
            <div class="relative">
              <input
                id="password"
                :type="showPwd ? 'text' : 'password'"
                v-model="password"
                class="w-full bg-transparent border-0 border-b border-[#EAE3D9] rounded-none px-0 focus:outline-none focus:ring-0 h-10 text-sm font-medium shadow-none transition-all duration-500 pr-10 placeholder:text-[#D5CEC4]"
                placeholder="••••••••"
                required
              />
              <div class="absolute bottom-0 left-0 h-[1.5px] bg-[#CC8033] w-0 group-focus-within:w-full transition-all duration-700"></div>
              <button
                type="button"
                @click="showPwd = !showPwd"
                class="absolute right-0 top-1/2 -translate-y-1/2 text-[#D5CEC4] hover:text-[#CC8033] transition-all"
              >
                <EyeOff v-if="showPwd" class="w-3.5 h-3.5" stroke-width="1.2" />
                <Eye v-else class="w-3.5 h-3.5" stroke-width="1.2" />
              </button>
            </div>
          </div>

          <!-- Remember me -->
          <div class="flex items-center justify-between pt-1">
            <label class="flex items-center gap-2 cursor-pointer group/check">
              <div class="relative flex items-center justify-center">
                <input type="checkbox" v-model="rememberMe" class="sr-only peer" />
                <div class="w-3.5 h-3.5 border border-[#EAE3D9] rounded bg-white peer-checked:bg-[#CC8033] peer-checked:border-[#CC8033] transition-all duration-300"></div>
                <Check class="w-2.5 h-2.5 text-white absolute opacity-0 peer-checked:opacity-100 transition-opacity" stroke-width="4" />
              </div>
              <span class="text-[10px] font-bold text-[#8A8178] group-hover/check:text-[#2A231E] transition-colors">Ghi nhớ đăng nhập</span>
            </label>
          </div>

          <!-- Submit Button -->
          <div class="pt-3 space-y-3">
            <button
              type="submit"
              :disabled="isLoading"
              class="w-full h-12 bg-[#2A231E] hover:bg-[#1A1512] text-[#FDFBF7] text-[10px] uppercase tracking-[0.25em] font-bold rounded-md shadow-[0_15px_30px_-5px_rgba(42,35,30,0.3)] transition-all duration-700 active:scale-[0.98] group relative overflow-hidden disabled:opacity-70"
            >
              <span class="relative z-10 flex items-center justify-center gap-2">
                <span v-if="isLoading" class="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                <span v-else>Đăng nhập</span>
                <ArrowRight v-if="!isLoading" class="w-3.5 h-3.5 group-hover:translate-x-1.5 transition-transform duration-500" stroke-width="2" />
              </span>
              <div class="absolute inset-0 bg-gradient-to-r from-transparent via-white/5 to-transparent -translate-x-full group-hover:animate-[shimmer_2s_infinite]"></div>
            </button>

            <!-- Divider -->
            <div class="flex items-center gap-3">
              <div class="flex-1 h-px bg-[#EAE3D9]"></div>
              <span class="text-[9px] uppercase tracking-widest text-[#C5BEB8] font-bold">hoặc</span>
              <div class="flex-1 h-px bg-[#EAE3D9]"></div>
            </div>

            <!-- Google SSO Button -->
            <button
              type="button"
              @click="handleGoogleLogin"
              class="w-full h-11 bg-white hover:bg-[#FDFBF7] border border-[#EAE3D9] hover:border-[#D5CEC4] text-[#2A231E] text-[10px] uppercase tracking-[0.15em] font-bold rounded-md transition-all duration-300 active:scale-[0.98] flex items-center justify-center gap-3 shadow-sm"
            >
              <!-- Google icon -->
              <svg class="w-4 h-4" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
                <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
              </svg>
              <span>Đăng nhập với Google</span>
            </button>
          </div>
        </form>

        <div class="mt-8 text-center">
          <p class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">
            Chưa được cấp quyền?
            <a href="#" class="ml-1 text-[#CC8033] border-b border-[#CC8033]/30 hover:border-[#CC8033] transition-all">Yêu cầu tài khoản ngay</a>
          </p>
        </div>

        <!-- Staff shortcut -->
        <div class="mt-4 pt-4 border-t border-[#F5F2ED] text-center">
          <router-link
            to="/staff-login"
            class="inline-flex items-center gap-2 text-[10px] font-bold text-[#8A8178] hover:text-[#CC8033] transition-colors duration-200 group"
          >
            <Users class="w-3 h-3 group-hover:text-[#CC8033] transition-colors" stroke-width="2" />
            Bạn là nhân viên? Đăng nhập bằng mã PIN
            <ArrowRight class="w-2.5 h-2.5 group-hover:translate-x-1 transition-transform duration-300" stroke-width="2.5" />
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { Coffee, Eye, EyeOff, ArrowRight, Check, Lock, Shield, Users } from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'

const showPwd = ref(false)
const email = ref('admin@brew.vn')
const password = ref('demo1234')
const rememberMe = ref(false)
const isLoading = ref(false)
const router = useRouter()
const authStore = useAuthStore()

const securityFeatures = [
  'Kết nối mã hóa SSL/TLS',
  'Xác thực hai yếu tố (2FA)',
  'Phiên đăng nhập tự động hết hạn',
]

const handleSubmit = async () => {
  isLoading.value = true
  // Simulate API call
  await new Promise(r => setTimeout(r, 800))
  authStore.login()
  isLoading.value = false
  router.push('/dashboard')
}

const handleGoogleLogin = () => {
  // Placeholder for Google OAuth
  alert('Chức năng Google SSO sẽ được tích hợp sau.')
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,400;0,600;0,700;1,400&family=Montserrat:wght@300;400;500;600;700;900&display=swap');

.font-premium-serif {
  font-family: 'Cormorant Garamond', Georgia, serif;
}

.font-premium-sans {
  font-family: 'Inter', sans-serif;
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
