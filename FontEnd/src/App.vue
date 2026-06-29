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
import ToastContainer from '@/components/ui/ToastContainer.vue'
import AlertDialog from '@/components/ui/AlertDialog.vue'

const storeInfoStore = useStoreInfoStore()

// Fetch thông tin quán 1 lần khi app khởi động — dùng chung toàn bộ ứng dụng
onMounted(() => {
  storeInfoStore.fetchInfo()
})
</script>

<style scoped>
/* CSS của bạn */
</style>
