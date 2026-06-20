<template>
  <div class="min-h-screen flex items-center justify-center px-4 py-10 bg-[#FAF6F0] font-premium-sans text-[#2A231E] relative overflow-hidden">

    <!-- Ambient Background -->
    <div class="absolute inset-0 pointer-events-none">
      <div class="absolute top-[-20%] left-[-10%] w-[60%] h-[60%] rounded-full bg-[#CC8033]/5 blur-[150px]"></div>
      <div class="absolute bottom-[-20%] right-[-10%] w-[60%] h-[60%] rounded-full bg-[#2A231E]/5 blur-[150px]"></div>
    </div>

    <!-- Main Container -->
    <div class="w-full max-w-[880px] bg-white rounded-[2rem] shadow-[0_50px_100px_-20px_rgba(42,35,30,0.12)] flex flex-col md:flex-row overflow-hidden border border-[#EAE3D9]/50 relative z-10 animate-in fade-in zoom-in-95 duration-1000">

      <!-- ── Left Side: Brand ── -->
      <div class="w-full md:w-[38%] bg-[#2A231E] p-8 md:p-12 flex flex-col items-center justify-center text-center relative overflow-hidden">
        <div class="absolute inset-0 opacity-[0.03] pointer-events-none bg-[url('https://www.transparenttextures.com/patterns/cubes.png')]"></div>

        <!-- Admin Badge -->
        <div class="absolute top-6 left-0 right-0 flex justify-center">
          <div class="flex items-center gap-1.5 px-3 py-1.5 rounded-full bg-[#CC8033]/15 border border-[#CC8033]/25">
            <div class="w-1.5 h-1.5 rounded-full bg-[#CC8033] animate-pulse"></div>
            <span class="text-[9px] uppercase tracking-[0.25em] text-[#CC8033] font-black">Admin Portal</span>
          </div>
        </div>

        <div class="relative z-10 space-y-6 animate-in slide-in-from-left-8 duration-1000 delay-300">
          <div class="w-16 h-16 rounded-xl bg-[#CC8033] mx-auto flex items-center justify-center shadow-[0_15px_30px_rgba(204,128,51,0.25)] border border-white/10">
            <Coffee class="w-8 h-8 text-[#2A231E]" stroke-width="1.2" />
          </div>

          <div class="space-y-3">
            <h2 class="font-premium-serif text-3xl font-bold tracking-[0.1em] text-[#FDFBF7]">BREWMANAGER</h2>
            <div class="w-10 h-[1px] bg-[#CC8033] mx-auto opacity-60"></div>
            <p class="text-[10px] uppercase tracking-[0.3em] text-[#8A8178] font-bold leading-relaxed">
              Hệ thống quản trị và <br/> vận hành toàn diện
            </p>
          </div>

          <div class="mt-6 space-y-2.5">
            <div v-for="item in securityFeatures" :key="item" class="flex items-center gap-2 text-left">
              <Shield class="w-3 h-3 text-[#CC8033] shrink-0" stroke-width="2" />
              <span class="text-[9px] text-[#8A8178] font-medium tracking-wide">{{ item }}</span>
            </div>
          </div>
        </div>

        <div class="absolute bottom-8 left-0 right-0 opacity-10 text-[9px] uppercase tracking-[0.8em] font-black text-white">
          EST. 2026
        </div>
      </div>

      <!-- ── Right Side: Form ── -->
      <div class="flex-1 p-8 md:p-12 bg-white flex flex-col justify-center relative animate-in slide-in-from-right-8 duration-1000 delay-300">

        <!-- Back to home -->
        <router-link to="/" class="absolute top-6 right-6 flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-[#8A8178] hover:text-[#CC8033] hover:bg-[#CC8033]/10 transition-colors text-[9px] font-bold uppercase tracking-widest border border-transparent hover:border-[#CC8033]/20">
          <Home class="w-3.5 h-3.5" stroke-width="2.5" /> Trang chính
        </router-link>

        <div class="mb-8">
          <div class="inline-flex items-center gap-2 mb-4 px-2.5 py-1 rounded-md bg-[#CC8033]/8 border border-[#CC8033]/15">
            <Lock class="w-2.5 h-2.5 text-[#CC8033]" stroke-width="2.5" />
            <span class="text-[8px] uppercase tracking-[0.2em] text-[#CC8033] font-black">Truy cập hạn chế</span>
          </div>
          <h1 class="font-premium-serif text-4xl font-semibold text-[#1A1512] tracking-tight">Cổng Đăng Nhập<br/>Hệ Thống</h1>
        </div>

        <!-- ── Tabs ── -->
        <div class="flex items-center gap-6 border-b border-[#EAE3D9] mb-8">
          <button
            @click="switchTab('admin')"
            :class="[
              'pb-3 text-[10px] font-black uppercase tracking-widest transition-all duration-300 relative',
              activeTab === 'admin' ? 'text-[#CC8033]' : 'text-[#8A8178] hover:text-[#2A231E]'
            ]"
          >
            Tài khoản Admin
            <div v-if="activeTab === 'admin'" class="absolute bottom-0 left-0 w-full h-[2px] bg-[#CC8033]"></div>
          </button>
          <button
            @click="switchTab('staff')"
            :class="[
              'pb-3 text-[10px] font-black uppercase tracking-widest transition-all duration-300 relative',
              activeTab === 'staff' ? 'text-[#CC8033]' : 'text-[#8A8178] hover:text-[#2A231E]'
            ]"
          >
            Nhân viên ca trực
            <div v-if="activeTab === 'staff'" class="absolute bottom-0 left-0 w-full h-[2px] bg-[#CC8033]"></div>
          </button>
        </div>

        <!-- ── Form ── -->
        <form :key="activeTab" @submit.prevent="handleSubmit" novalidate class="space-y-5 animate-in fade-in slide-in-from-right-4 duration-500">

          <!-- Email -->
          <div class="space-y-1.5 group">
            <label for="login-email" class="text-[9px] uppercase tracking-widest font-black transition-colors duration-300"
              :class="errors.email ? 'text-red-500' : 'text-[#8A8178] group-focus-within:text-[#CC8033]'">
              {{ activeTab === 'admin' ? 'Tài khoản Email' : 'Email nhân viên' }}
            </label>
            <div class="relative">
              <input
                id="login-email"
                type="email"
                v-model="email"
                @blur="touchField('email')"
                @input="clearError('email')"
                autocomplete="email"
                :placeholder="activeTab === 'admin' ? 'admin@brew.vn' : 'staff@brew.vn'"
                class="w-full bg-transparent border-0 border-b rounded-none px-0 focus:outline-none focus:ring-0 h-10 text-sm font-medium shadow-none transition-all duration-500 placeholder:text-[#D5CEC4] placeholder:font-normal"
                :class="errors.email ? 'border-red-400 text-red-600' : 'border-[#EAE3D9] focus:border-[#CC8033]'"
              />
              <!-- focus underline -->
              <div class="absolute bottom-0 left-0 h-[1.5px] w-0 group-focus-within:w-full transition-all duration-700"
                :class="errors.email ? 'bg-red-400' : 'bg-[#CC8033]'"></div>
              <!-- valid icon -->
              <CheckCircle v-if="touched.email && !errors.email && email"
                class="absolute right-0 top-1/2 -translate-y-1/2 w-4 h-4 text-emerald-500" stroke-width="2" />
            </div>
            <p v-if="errors.email" class="text-[10px] text-red-500 font-semibold flex items-center gap-1 mt-1">
              <AlertCircle class="w-3 h-3 shrink-0" /> {{ errors.email }}
            </p>
          </div>

          <!-- Password -->
          <div class="space-y-1.5 group">
            <label for="login-password" class="text-[9px] uppercase tracking-widest font-black transition-colors duration-300"
              :class="errors.password ? 'text-red-500' : 'text-[#8A8178] group-focus-within:text-[#CC8033]'">
              Mật khẩu
            </label>
            <div class="relative">
              <input
                id="login-password"
                :type="showPwd ? 'text' : 'password'"
                v-model="password"
                @blur="touchField('password')"
                @input="clearError('password')"
                autocomplete="current-password"
                placeholder="••••••••"
                class="w-full bg-transparent border-0 border-b rounded-none px-0 focus:outline-none focus:ring-0 h-10 text-sm font-medium shadow-none transition-all duration-500 pr-16 placeholder:text-[#D5CEC4]"
                :class="errors.password ? 'border-red-400 text-red-600' : 'border-[#EAE3D9] focus:border-[#CC8033]'"
              />
              <div class="absolute bottom-0 left-0 h-[1.5px] w-0 group-focus-within:w-full transition-all duration-700"
                :class="errors.password ? 'bg-red-400' : 'bg-[#CC8033]'"></div>

              <!-- Strength bar -->
              <div v-if="password.length > 0" class="absolute bottom-[-4px] left-0 right-10 h-[2px] rounded-full bg-[#EAE3D9] overflow-hidden">
                <div class="h-full rounded-full transition-all duration-500"
                  :style="{ width: pwdStrengthPercent + '%' }"
                  :class="pwdStrengthColor"></div>
              </div>

              <button type="button" @click="showPwd = !showPwd"
                class="absolute right-0 top-1/2 -translate-y-1/2 text-[#D5CEC4] hover:text-[#CC8033] transition-all">
                <EyeOff v-if="showPwd" class="w-3.5 h-3.5" stroke-width="1.2" />
                <Eye v-else class="w-3.5 h-3.5" stroke-width="1.2" />
              </button>
            </div>

            <!-- Password strength label -->
            <div v-if="password.length > 0" class="flex items-center justify-between mt-1.5">
              <p v-if="errors.password" class="text-[10px] text-red-500 font-semibold flex items-center gap-1">
                <AlertCircle class="w-3 h-3 shrink-0" /> {{ errors.password }}
              </p>
              <span v-else class="text-[9px] font-bold" :class="pwdStrengthTextColor">{{ pwdStrengthLabel }}</span>
            </div>
          </div>

          <!-- Remember me -->
          <div class="flex items-center pt-1">
            <label class="flex items-center gap-2 cursor-pointer group/check">
              <div class="relative flex items-center justify-center">
                <input type="checkbox" v-model="rememberMe" class="sr-only peer" />
                <div class="w-3.5 h-3.5 border border-[#EAE3D9] rounded bg-white peer-checked:bg-[#CC8033] peer-checked:border-[#CC8033] transition-all duration-300"></div>
                <Check class="w-2.5 h-2.5 text-white absolute opacity-0 peer-checked:opacity-100 transition-opacity" stroke-width="4" />
              </div>
              <span class="text-[10px] font-bold text-[#8A8178] group-hover/check:text-[#2A231E] transition-colors">Ghi nhớ đăng nhập</span>
            </label>
          </div>

          <!-- API Error -->
          <div v-if="errorMsg" class="flex items-start gap-2.5 text-[11px] font-semibold text-red-600 bg-red-50 border border-red-200 rounded-xl px-4 py-3">
            <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" stroke-width="2" />
            <span>{{ errorMsg }}</span>
          </div>

          <!-- Submit -->
          <div class="pt-2">
            <button
              type="submit"
              :disabled="isLoading || !isFormValid"
              class="w-full h-12 text-[#FDFBF7] text-[10px] uppercase tracking-[0.25em] font-bold rounded-xl shadow-[0_15px_30px_-5px_rgba(42,35,30,0.25)] transition-all duration-500 active:scale-[0.98] group relative overflow-hidden"
              :class="isFormValid && !isLoading
                ? 'bg-[#2A231E] hover:bg-[#1A1512] cursor-pointer'
                : 'bg-[#2A231E]/40 cursor-not-allowed'"
            >
              <span class="relative z-10 flex items-center justify-center gap-2">
                <span v-if="isLoading" class="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                <template v-else>
                  <span>Đăng nhập</span>
                  <ArrowRight class="w-3.5 h-3.5 group-hover:translate-x-1.5 transition-transform duration-500" stroke-width="2" />
                </template>
              </span>
              <div class="absolute inset-0 bg-gradient-to-r from-transparent via-white/5 to-transparent -translate-x-full group-hover:animate-[shimmer_2s_infinite]"></div>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import {
  Coffee, Eye, EyeOff, ArrowRight, Check,
  Lock, Shield, Home, AlertCircle, CheckCircle
} from 'lucide-vue-next'
import { useAuthStore } from '@/stores/auth'

// ── State ────────────────────────────────────────────────────────
const activeTab = ref<'admin' | 'staff'>('admin')
const showPwd   = ref(false)
const email     = ref('')
const password  = ref('')
const rememberMe = ref(false)
const isLoading  = ref(false)
const errorMsg   = ref('')
const router     = useRouter()
const authStore  = useAuthStore()

// Track which fields user has blurred (for validation UX)
const touched = ref({ email: false, password: false })

// ── Switch tab ────────────────────────────────────────────────────
const switchTab = (tab: 'admin' | 'staff') => {
  activeTab.value = tab
  email.value    = ''
  password.value = ''
  errorMsg.value = ''
  touched.value  = { email: false, password: false }
  showPwd.value  = false
}

// ── Validation rules ──────────────────────────────────────────────
const EMAIL_REGEX = /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/

const errors = computed(() => {
  const e: Record<string, string> = {}

  // Email
  if (touched.value.email) {
    if (!email.value.trim()) {
      e.email = 'Email không được để trống.'
    } else if (!EMAIL_REGEX.test(email.value.trim())) {
      e.email = 'Định dạng email không hợp lệ (vd: user@domain.com).'
    } else if (!email.value.includes('.')) {
      e.email = 'Email phải có tên miền hợp lệ.'
    }
  }

  // Password
  if (touched.value.password) {
    if (!password.value) {
      e.password = 'Mật khẩu không được để trống.'
    } else if (password.value.length < 6) {
      e.password = `Mật khẩu quá ngắn (còn thiếu ${6 - password.value.length} ký tự).`
    } else if (password.value.length > 100) {
      e.password = 'Mật khẩu không được vượt quá 100 ký tự.'
    }
  }

  return e
})

const isFormValid = computed(() =>
  EMAIL_REGEX.test(email.value.trim()) &&
  password.value.length >= 6
)

const touchField = (field: 'email' | 'password') => {
  touched.value[field] = true
}

const clearError = (field: 'email' | 'password') => {
  // Re-evaluate by touching field
  if (field === 'email'    && email.value.length > 0)    touched.value.email    = true
  if (field === 'password' && password.value.length > 0) touched.value.password = true
  errorMsg.value = ''
}

// ── Password strength ─────────────────────────────────────────────
const pwdStrength = computed(() => {
  const p = password.value
  if (!p) return 0
  let score = 0
  if (p.length >= 8)  score++
  if (p.length >= 12) score++
  if (/[A-Z]/.test(p)) score++
  if (/[0-9]/.test(p)) score++
  if (/[^A-Za-z0-9]/.test(p)) score++
  return score
})

const pwdStrengthPercent = computed(() => Math.min(100, (pwdStrength.value / 5) * 100))

const pwdStrengthColor = computed(() => {
  const s = pwdStrength.value
  if (s <= 1) return 'bg-red-400'
  if (s <= 2) return 'bg-orange-400'
  if (s <= 3) return 'bg-yellow-400'
  if (s <= 4) return 'bg-emerald-400'
  return 'bg-emerald-500'
})

const pwdStrengthTextColor = computed(() => {
  const s = pwdStrength.value
  if (s <= 1) return 'text-red-500'
  if (s <= 2) return 'text-orange-500'
  if (s <= 3) return 'text-yellow-600'
  return 'text-emerald-600'
})

const pwdStrengthLabel = computed(() => {
  const s = pwdStrength.value
  if (s <= 1) return 'Rất yếu'
  if (s <= 2) return 'Yếu'
  if (s <= 3) return 'Trung bình'
  if (s <= 4) return 'Mạnh'
  return 'Rất mạnh'
})

// ── Security features sidebar ─────────────────────────────────────
const securityFeatures = [
  'Kết nối mã hóa SSL/TLS',
  'Xác thực hai yếu tố (2FA)',
  'Phiên đăng nhập tự động hết hạn',
]

// ── Submit ────────────────────────────────────────────────────────
const handleSubmit = async () => {
  // Force touch all fields to show errors
  touched.value = { email: true, password: true }

  if (!isFormValid.value) return

  isLoading.value = true
  errorMsg.value  = ''
  try {
    await authStore.login(email.value.trim(), password.value)
    router.push('/revenue-report')
  } catch (e) {
    errorMsg.value = e instanceof Error
      ? e.message
      : 'Đăng nhập thất bại. Vui lòng kiểm tra lại email và mật khẩu.'
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
  100% { transform: translateX(100%); }
}

input:focus { outline: none; }

input:-webkit-autofill,
input:-webkit-autofill:hover,
input:-webkit-autofill:focus {
  -webkit-box-shadow: 0 0 0px 1000px white inset;
  transition: background-color 5000s ease-in-out 0s;
}
</style>
