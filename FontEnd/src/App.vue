<template>
  <!-- Chế độ bảo trì hệ thống toàn màn hình cho Khách hàng -->
  <div v-if="storeInfoStore.cheDoBaoTri && !isStaffRoute" class="fixed inset-0 z-[9999] bg-[#1C130E] text-white flex flex-col items-center justify-center p-6 text-center select-none overflow-hidden">
    <!-- Hiệu ứng ánh sáng nền -->
    <div class="absolute -inset-4 bg-gradient-to-tr from-[#CC8033]/20 via-[#E89E53]/10 to-amber-600/20 blur-3xl pointer-events-none"></div>

    <div class="relative max-w-lg w-full bg-[#281C16]/90 border border-white/15 rounded-3xl p-8 sm:p-12 shadow-2xl backdrop-blur-xl space-y-6">
      
      <!-- Icon Cờ lê bảo trì -->
      <div class="w-20 h-20 mx-auto rounded-3xl bg-gradient-to-tr from-[#CC8033] via-[#E89E53] to-amber-500 p-0.5 shadow-xl shadow-[#CC8033]/30">
        <div class="w-full h-full bg-[#1C130E] rounded-[22px] flex items-center justify-center">
          <Wrench class="w-10 h-10 text-[#E89E53] animate-bounce" />
        </div>
      </div>

      <!-- Tiêu đề & Thông điệp -->
      <div class="space-y-3">
        <span class="text-[10px] font-black uppercase tracking-[0.3em] text-[#E89E53] bg-[#CC8033]/15 px-3 py-1 rounded-full border border-[#CC8033]/30">
          Thông báo bảo trì
        </span>
        <h2 class="font-display text-2xl sm:text-3xl font-black text-white">
          {{ storeInfoStore.tenQuan || 'cà phê F6' }} Đang Bảo Trì
        </h2>
        <p class="text-xs sm:text-sm text-white/75 leading-relaxed font-medium">
          {{ storeInfoStore.thongDiepBaoTri || 'Hệ thống đang tiến hành nâng cấp & bảo trì định kỳ. Quý khách vui lòng quay lại sau ít phút hoặc liên hệ trực tiếp hotline của quán!' }}
        </p>
      </div>

      <!-- Thông tin quán -->
      <div class="bg-white/5 border border-white/10 rounded-2xl p-4 text-left text-xs space-y-2 text-white/80">
        <div class="flex items-center justify-between" v-if="storeInfoStore.soDienThoai">
          <span class="text-[#D5B08D] font-bold">Hotline quán:</span>
          <span class="font-bold font-mono text-white">{{ storeInfoStore.soDienThoai }}</span>
        </div>
        <div class="flex items-center justify-between" v-if="storeInfoStore.diaChi">
          <span class="text-[#D5B08D] font-bold">Địa chỉ:</span>
          <span class="font-medium text-white/90 truncate max-w-[220px]">{{ storeInfoStore.diaChi }}</span>
        </div>
      </div>

      <!-- Thao tác -->
      <div class="pt-2 flex flex-col sm:flex-row gap-3">
        <button
          @click="checkStatus"
          class="flex-1 h-12 rounded-xl bg-gradient-to-r from-[#CC8033] to-[#B3702C] hover:from-[#B3702C] hover:to-[#965A1E] text-white font-bold text-xs uppercase tracking-wider transition-all shadow-lg active:scale-95 flex items-center justify-center gap-2 cursor-pointer"
        >
          <RefreshCw class="w-4 h-4" :class="{ 'animate-spin': checking }" />
          <span>Thử kết nối lại</span>
        </button>

        <router-link
          to="/login"
          class="h-12 px-5 rounded-xl bg-white/10 hover:bg-white/20 border border-white/20 text-white font-bold text-xs transition-all flex items-center justify-center gap-2 shrink-0 cursor-pointer"
        >
          <Lock class="w-4 h-4 text-[#E89E53]" />
          <span>Cổng Quản Trị</span>
        </router-link>
      </div>
    </div>
  </div>

  <template v-else>
    <router-view />
    <ToastContainer />
    <AlertDialog />
  </template>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { Wrench, RefreshCw, Lock } from 'lucide-vue-next'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { useAuthStore } from '@/stores/auth'
import ToastContainer from '@/components/ui/ToastContainer.vue'
import AlertDialog from '@/components/ui/AlertDialog.vue'

const storeInfoStore = useStoreInfoStore()
const authStore = useAuthStore()
const route = useRoute()
const checking = ref(false)

// Cho phép Nhân viên / Quản trị viên truy cập các trang quản trị ngay cả khi bảo trì
const isStaffRoute = computed(() => {
  const path = route.path.toLowerCase()
  return authStore.isAuthenticated || path.startsWith('/login') || path.startsWith('/tables') || path.startsWith('/settings') || path.startsWith('/kitchen') || path.startsWith('/pos') || path.startsWith('/dashboard') || path.startsWith('/finance') || path.startsWith('/employees')
})

const checkStatus = async () => {
  checking.value = true
  try {
    await storeInfoStore.fetchInfo(true)
  } finally {
    setTimeout(() => { checking.value = false }, 600)
  }
}

onMounted(() => {
  storeInfoStore.fetchInfo()
  if (authStore.isAuthenticated) {
    authStore.refreshProfile()
  }
})
</script>

<style scoped>
/* CSS của bạn */
</style>
