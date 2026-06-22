<template>
  <Teleport to="body">
    <Transition name="alert-fade">
      <div
        v-if="store.visible"
        class="fixed inset-0 z-[9999] flex items-center justify-center p-4"
        @click.self="onBackdropClick"
      >
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" />

        <!-- Dialog -->
        <Transition name="alert-scale">
          <div
            v-if="store.visible"
            class="relative w-full max-w-md rounded-3xl shadow-2xl overflow-hidden"
            :class="cardClass"
          >
            <!-- Top glow strip -->
            <div class="absolute top-0 left-0 right-0 h-1 rounded-t-3xl" :class="stripClass" />

            <!-- Body -->
            <div class="px-8 pt-10 pb-8 flex flex-col items-center text-center">

              <!-- Icon circle -->
              <div
                class="w-20 h-20 rounded-full flex items-center justify-center mb-6 shadow-lg"
                :class="iconBgClass"
              >
                <component :is="iconComponent" class="w-10 h-10" :class="iconColorClass" stroke-width="1.8" />
              </div>

              <!-- Title -->
              <h2 class="text-2xl font-bold mb-2 leading-tight" :class="titleColorClass">
                {{ store.options.title }}
              </h2>

              <!-- Message -->
              <p v-if="store.options.message" class="text-base leading-relaxed opacity-75" :class="messageColorClass">
                {{ store.options.message }}
              </p>

              <!-- Buttons -->
              <div class="flex gap-3 mt-8 w-full" :class="store.options.type === 'confirm' ? 'flex-row' : 'flex-col'">

                <!-- Cancel (only confirm) -->
                <button
                  v-if="store.options.type === 'confirm'"
                  class="flex-1 h-12 rounded-2xl text-sm font-semibold transition-all duration-200 border"
                  :class="cancelBtnClass"
                  @click="store.cancel()"
                >
                  {{ store.options.cancelText ?? 'Hủy' }}
                </button>

                <!-- Confirm -->
                <button
                  class="flex-1 h-12 rounded-2xl text-sm font-bold tracking-wide transition-all duration-200 shadow-lg active:scale-95"
                  :class="confirmBtnClass"
                  @click="store.confirm()"
                >
                  {{ store.options.confirmText ?? 'OK' }}
                </button>

              </div>
            </div>

            <!-- Decorative coffee beans (subtle) -->
            <div class="absolute -bottom-6 -right-6 w-24 h-24 rounded-full opacity-[0.04]" :class="decorClass" />
            <div class="absolute -top-4 -left-4 w-16 h-16 rounded-full opacity-[0.04]" :class="decorClass" />
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

// Đóng khi click backdrop (chỉ với info/success/warning/error, không phải confirm)
function onBackdropClick() {
  if (type.value !== 'confirm') store.confirm()
}

const cardClass = computed(() => ({
  success: 'bg-[#F5FAF4] border border-[#BBE3B0]',
  error:   'bg-[#FDF4F4] border border-[#F0B8B8]',
  warning: 'bg-[#FDFAF0] border border-[#EDD98A]',
  info:    'bg-[#F4F7FD] border border-[#B0C8F0]',
  confirm: 'bg-[#1E2128] border border-white/10',
}[type.value]))

const stripClass = computed(() => ({
  success: 'bg-gradient-to-r from-[#22C55E] to-[#16A34A]',
  error:   'bg-gradient-to-r from-[#EF4444] to-[#DC2626]',
  warning: 'bg-gradient-to-r from-[#F59E0B] to-[#D97706]',
  info:    'bg-gradient-to-r from-[#3B82F6] to-[#2563EB]',
  confirm: 'bg-gradient-to-r from-[#CC8033] to-[#B8722D]',
}[type.value]))

const iconBgClass = computed(() => ({
  success: 'bg-[#DCFCE7]',
  error:   'bg-[#FEE2E2]',
  warning: 'bg-[#FEF3C7]',
  info:    'bg-[#DBEAFE]',
  confirm: 'bg-[#CC8033]/20',
}[type.value]))

const iconColorClass = computed(() => ({
  success: 'text-[#16A34A]',
  error:   'text-[#DC2626]',
  warning: 'text-[#D97706]',
  info:    'text-[#2563EB]',
  confirm: 'text-[#CC8033]',
}[type.value]))

const iconComponent = computed(() => ({
  success: CheckCircle2,
  error:   XCircle,
  warning: AlertTriangle,
  info:    Info,
  confirm: HelpCircle,
}[type.value]))

const titleColorClass = computed(() => ({
  success: 'text-[#14532D]',
  error:   'text-[#7F1D1D]',
  warning: 'text-[#78350F]',
  info:    'text-[#1E3A8A]',
  confirm: 'text-white',
}[type.value]))

const messageColorClass = computed(() => ({
  success: 'text-[#14532D]',
  error:   'text-[#7F1D1D]',
  warning: 'text-[#78350F]',
  info:    'text-[#1E3A8A]',
  confirm: 'text-white',
}[type.value]))

const confirmBtnClass = computed(() => ({
  success: 'bg-[#16A34A] hover:bg-[#15803D] text-white shadow-[#16A34A]/30',
  error:   'bg-[#DC2626] hover:bg-[#B91C1C] text-white shadow-[#DC2626]/30',
  warning: 'bg-[#D97706] hover:bg-[#B45309] text-white shadow-[#D97706]/30',
  info:    'bg-[#2563EB] hover:bg-[#1D4ED8] text-white shadow-[#2563EB]/30',
  confirm: 'bg-[#CC8033] hover:bg-[#B8722D] text-white shadow-[#CC8033]/30',
}[type.value]))

const cancelBtnClass = computed(() => 'bg-white/5 hover:bg-white/10 text-white/70 border-white/10')

const decorClass = computed(() => ({
  success: 'bg-[#16A34A]',
  error:   'bg-[#DC2626]',
  warning: 'bg-[#D97706]',
  info:    'bg-[#2563EB]',
  confirm: 'bg-[#CC8033]',
}[type.value]))
</script>

<style scoped>
/* Backdrop fade */
.alert-fade-enter-active,
.alert-fade-leave-active {
  transition: opacity 0.25s ease;
}
.alert-fade-enter-from,
.alert-fade-leave-to {
  opacity: 0;
}

/* Card scale bounce */
.alert-scale-enter-active {
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.alert-scale-leave-active {
  transition: all 0.2s cubic-bezier(0.4, 0, 1, 1);
}
.alert-scale-enter-from {
  opacity: 0;
  transform: scale(0.75) translateY(20px);
}
.alert-scale-leave-to {
  opacity: 0;
  transform: scale(0.9) translateY(-10px);
}
</style>
