<template>
  <Teleport to="body">
    <Transition name="alert-fade">
      <div
        v-if="store.visible"
        class="fixed inset-0 z-[9999] flex items-center justify-center p-4 bg-black/60 backdrop-blur-md"
        @click.self="onBackdropClick"
      >
        <!-- Card -->
        <Transition name="alert-scale">
          <div
            v-if="store.visible"
            class="relative w-full max-w-[380px] bg-white rounded-3xl p-6 shadow-[0_25px_60px_-15px_rgba(42,35,30,0.3)] border border-[#EAE3D9] text-center space-y-5 overflow-hidden"
          >
            <!-- Top decorative accent line -->
            <div class="absolute top-0 left-0 right-0 h-1.5 rounded-t-3xl" :class="accentBarClass" />

            <!-- Icon -->
            <div
              class="w-14 h-14 rounded-2xl flex items-center justify-center mx-auto shadow-sm border mt-1"
              :class="iconBgClass"
            >
              <component :is="iconComponent" class="w-7 h-7" :class="iconColorClass" stroke-width="2" />
            </div>

            <!-- Content -->
            <div class="space-y-1.5 px-2">
              <h3 class="text-lg font-bold text-[#2A231E] leading-snug font-premium-serif">
                {{ store.options.title }}
              </h3>
              <p v-if="store.options.message" class="text-xs text-[#786E65] font-medium leading-relaxed">
                {{ store.options.message }}
              </p>
            </div>

            <!-- Action Buttons -->
            <div class="flex items-center gap-3 pt-2">
              <button
                v-if="store.options.type === 'confirm'"
                @click="store.cancel()"
                class="flex-1 h-11 rounded-2xl bg-[#F5F2ED] hover:bg-[#EAE3D9] text-[#5C544E] font-bold text-xs transition-all active:scale-95 border border-[#EAE3D9]"
              >
                {{ store.options.cancelText ?? 'Hủy' }}
              </button>

              <button
                @click="store.confirm()"
                class="flex-1 h-11 rounded-2xl text-white font-bold text-xs shadow-md transition-all active:scale-95 uppercase tracking-wider"
                :class="confirmBtnClass"
              >
                {{ getConfirmText() }}
              </button>
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAlertStore } from '@/stores/alert'
import { CheckCircle2, XCircle, AlertTriangle, Info, HelpCircle } from 'lucide-vue-next'

const store = useAlertStore()
const type = computed(() => store.options.type)

function onBackdropClick() {
  if (type.value !== 'confirm') store.confirm()
}

const getConfirmText = () => {
  if (store.options.confirmText) {
    if (store.options.confirmText === 'HOÀN TẤT' || store.options.confirmText === 'OK') return 'Hoàn tất'
    return store.options.confirmText
  }
  return type.value === 'confirm' ? 'Xác nhận' : 'Hoàn tất'
}

const accentBarClass = computed(() => ({
  success: 'bg-gradient-to-r from-[#CC8033] to-[#D97724]',
  error:   'bg-gradient-to-r from-red-500 to-rose-600',
  warning: 'bg-gradient-to-r from-amber-400 to-orange-500',
  info:    'bg-gradient-to-r from-blue-400 to-indigo-500',
  confirm: 'bg-gradient-to-r from-[#4A3224] to-[#CC8033]',
}[type.value]))

const iconBgClass = computed(() => ({
  success: 'bg-[#FFF9F2] border-[#E8C5A5]/60',
  error:   'bg-red-50 border-red-100',
  warning: 'bg-amber-50 border-amber-100',
  info:    'bg-blue-50 border-blue-100',
  confirm: 'bg-[#F5F2ED] border-[#EAE3D9]',
}[type.value]))

const iconColorClass = computed(() => ({
  success: 'text-[#CC8033]',
  error:   'text-red-500',
  warning: 'text-amber-500',
  info:    'text-blue-500',
  confirm: 'text-[#4A3224]',
}[type.value]))

const iconComponent = computed(() => ({
  success: CheckCircle2,
  error:   XCircle,
  warning: AlertTriangle,
  info:    Info,
  confirm: HelpCircle,
}[type.value]))

const confirmBtnClass = computed(() => ({
  success: 'bg-[#CC8033] hover:bg-[#B8722D] text-white shadow-[#CC8033]/20',
  error:   'bg-red-600 hover:bg-red-700 text-white shadow-red-600/20',
  warning: 'bg-amber-600 hover:bg-amber-700 text-white shadow-amber-600/20',
  info:    'bg-[#4A3224] hover:bg-[#382418] text-white shadow-[#4A3224]/20',
  confirm: 'bg-[#CC8033] hover:bg-[#B8722D] text-white shadow-[#CC8033]/20',
}[type.value]))
</script>

<style scoped>
.alert-fade-enter-active,
.alert-fade-leave-active {
  transition: opacity 0.2s ease;
}
.alert-fade-enter-from,
.alert-fade-leave-to {
  opacity: 0;
}

.alert-scale-enter-active {
  transition: all 0.25s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.alert-scale-leave-active {
  transition: all 0.15s ease-in;
}
.alert-scale-enter-from {
  opacity: 0;
  transform: scale(0.9) translateY(10px);
}
.alert-scale-leave-to {
  opacity: 0;
  transform: scale(0.95);
}
</style>
