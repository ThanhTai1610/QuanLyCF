<template>
  <router-view />
  <!-- Toast notification container — hiển thị toàn bộ ứng dụng -->
  <ToastContainer />
  <!-- Alert dialog trung tâm màn hình -->
  <AlertDialog />
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { useAuthStore } from '@/stores/auth'
import ToastContainer from '@/components/ui/ToastContainer.vue'
import AlertDialog from '@/components/ui/AlertDialog.vue'

const storeInfoStore = useStoreInfoStore()
const authStore = useAuthStore()

// Fetch thông tin quán 1 lần khi app khởi động — dùng chung toàn bộ ứng dụng
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
