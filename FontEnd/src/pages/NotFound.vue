<template>
  <div class="min-h-screen bg-[#FAF6F0] flex items-center justify-center p-6 font-premium-sans">
    <div class="max-w-md w-full bg-white rounded-lg border border-[#EAE3D9] shadow-card p-12 text-center">
      
      <!-- Icon -->
      <div class="w-24 h-24 mx-auto mb-8 bg-[#F5F2ED] rounded-lg flex items-center justify-center border border-[#EAE3D9]">
        <Coffee class="w-12 h-12 text-[#CC8033]" stroke-width="1.2" />
      </div>

      <!-- 404 -->
      <h1 class="font-premium-serif text-7xl font-bold text-[#2A231E] tracking-tighter leading-none mb-2">404</h1>
      
      <!-- Tiêu đề -->
      <h2 class="text-2xl font-semibold text-espresso mb-4">Trang không tìm thấy</h2>
      
      <!-- Mô tả -->
      <p class="text-[#8A8178] text-sm leading-relaxed mb-10">
        Rất tiếc, trang bạn đang tìm kiếm không tồn tại<br>
        hoặc đang được chuyển đổi từ React sang Vue.
      </p>

      <!-- Nút quay về -->
      <Button 
        @click="$router.push('/')"
        class="w-full h-12 bg-[#CC8033] text-white rounded-lg border border-[#CC8033]/30 shadow-card text-sm font-bold uppercase tracking-widest"
      >
        Quay về Trang chủ
      </Button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Coffee } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,400;0,600;0,700;1,400&family=Montserrat:wght@300;400;500;600;700;900&display=swap');

.font-premium-serif {
  font-family: 'Cormorant Garamond', Georgia, serif;
}
.font-premium-sans {
  font-family: 'Inter', sans-serif;
}
</style>
