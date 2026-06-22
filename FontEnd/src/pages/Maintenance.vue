<template>
  <div class="min-h-screen w-full bg-[#13151A] flex items-center justify-center p-6 relative overflow-hidden">
    <!-- Hiệu ứng nền mờ màu caramel -->
    <div class="absolute w-[300px] h-[300px] rounded-full bg-[#CC8033] opacity-10 blur-[120px] top-1/4 left-1/4 animate-pulse"></div>
    <div class="absolute w-[250px] h-[250px] rounded-full bg-[#E5BA73] opacity-10 blur-[100px] bottom-1/4 right-1/4"></div>

    <!-- Hộp nội dung chính (Glassmorphism) -->
    <div class="w-full max-w-md bg-[#1E2128]/80 backdrop-blur-xl border border-white/5 rounded-2xl p-8 text-center shadow-2xl relative z-10">
      
      <!-- Icon Bảo Trì với Animation xoay nhẹ -->
      <div class="w-20 h-20 mx-auto rounded-2xl bg-[#CC8033]/15 flex items-center justify-center border border-[#CC8033]/30 mb-6 shadow-inner relative group">
        <Wrench class="w-10 h-10 text-[#CC8033] animate-spin-slow group-hover:rotate-45 transition-transform duration-500" />
      </div>

      <h1 class="font-display text-2xl font-bold text-white tracking-wide mb-3">
        Hệ thống đang bảo trì
      </h1>
      
      <!-- Thông điệp động từ Backend -->
      <p class="text-sm text-[#A1A1AA] leading-relaxed mb-8 px-2">
        {{ message || 'Chúng tôi đang tiến hành nâng cấp hệ thống để mang lại trải nghiệm tốt nhất cho bạn. Vui lòng quay lại sau.' }}
      </p>

      <div class="space-y-4">
        <!-- Nút Tải lại -->
        <button 
          @click="checkStatus" 
          :disabled="loading"
          class="w-full py-3 bg-[#CC8033] hover:bg-[#B36E2B] active:scale-[0.98] text-white rounded-xl font-medium transition-all shadow-lg shadow-[#CC8033]/20 flex items-center justify-center gap-2 disabled:opacity-50"
        >
          <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': loading }" />
          {{ loading ? 'Đang kiểm tra...' : 'Kiểm tra lại' }}
        </button>

        <!-- Nút dành cho Admin đăng nhập -->
        <router-link 
          to="/login" 
          class="block w-full py-3 bg-white/5 hover:bg-white/10 active:scale-[0.98] text-[#E4E4E7] rounded-xl font-medium border border-white/5 transition-all text-sm"
        >
          Đăng nhập Quản trị viên
        </router-link>
      </div>

      <div class="mt-8 pt-6 border-t border-white/5 flex items-center justify-center gap-2 text-xs text-[#71717A]">
        <Coffee class="w-3.5 h-3.5" />
        <span>{{ storeInfoStore.tenQuan }} &copy; {{ new Date().getFullYear() }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Wrench, RefreshCw, Coffee } from 'lucide-vue-next'
import { useStoreInfoStore } from '@/stores/storeInfo'

import { useAlert } from '@/stores/alert'

const router         = useRouter()
const storeInfoStore = useStoreInfoStore()
const alertService   = useAlert()
const message = ref('')
const loading = ref(false)

// Lấy thông điệp bảo trì từ API
const fetchStatus = async () => {
  try {
    const res = await fetch('/api/settings/maintenance')
    if (res.ok) {
      const data = await res.json()
      message.value = data.message
      // Nếu đã hết bảo trì, quay về trang chủ
      if (!data.isMaintenance) {
        router.push('/')
      }
    }
  } catch (err) {
    console.error('Không thể kiểm tra trạng thái bảo trì:', err)
  }
}

const checkStatus = async () => {
  loading.value = true
  // Trì hoãn nhẹ 500ms tạo hiệu ứng kiểm tra thực tế
  await new Promise(resolve => setTimeout(resolve, 500))
  try {
    const res = await fetch('/api/settings/maintenance')
    if (res.ok) {
      const data = await res.json()
      message.value = data.message
      if (!data.isMaintenance) {
        router.push('/')
      } else {
        // Vẫn đang bảo trì
        await alertService.warning('Thông báo', 'Hệ thống vẫn đang trong quá trình bảo trì.')
      }
    }
  } catch (err) {
    await alertService.error('Lỗi', 'Không thể kết nối tới máy chủ. Vui lòng thử lại sau.')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchStatus()
})
</script>

<script lang="ts">
// Bổ sung style animation spin-slow
</script>

<style scoped>
@keyframes spin-slow {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}
.animate-spin-slow {
  animation: spin-slow 8s linear infinite;
}
</style>
