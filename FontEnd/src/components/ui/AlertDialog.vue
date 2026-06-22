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
                  class="flex-1 h-16 rounded-2xl text-lg font-semibold transition-all duration-200 border"
                  :class="cancelBtnClass"
                  @click="store.cancel()"
                >
                  {{ store.options.cancelText ?? 'Hủy' }}
                </button>

                <!-- Confirm -->
                <button
                  class="flex-1 h-16 rounded-2xl text-xl font-extrabold tracking-widest transition-all duration-200 shadow-xl active:scale-95 uppercase"
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
  success: 'bg-[#FDF6EC] border border-[#EBE0D0] shadow-warm', // bg-card
  error:   'bg-[#FDF8F7] border border-[#F2D7D7] shadow-warm',
  warning: 'bg-[#FCFAF2] border border-[#EADFBA] shadow-warm',
  info:    'bg-[#FAFBFD] border border-[#D5E1F2] shadow-warm',
  confirm: 'bg-[#2C1A0E] border border-[#3E291C]', // bg-espresso and border-espresso-soft
}[type.value]))

const stripClass = computed(() => ({
  success: 'bg-gradient-to-r from-caramel to-brown',
  error:   'bg-gradient-to-r from-[#E57373] to-[#C62828]',
  warning: 'bg-gradient-to-r from-[#F6D07A] to-caramel',
  info:    'bg-gradient-to-r from-[#90A4AE] to-[#546E7A]',
  confirm: 'bg-gradient-to-r from-caramel to-brown',
}[type.value]))

const iconBgClass = computed(() => ({
  success: 'bg-[#F6E4CF]', // bg-caramel-light
  error:   'bg-[#FCECEB]',
  warning: 'bg-[#FCF5E3]',
  info:    'bg-[#ECEFF1]',
  confirm: 'bg-caramel/20',
}[type.value]))

const iconColorClass = computed(() => ({
  success: 'text-caramel', 
  error:   'text-[#C62828]',
  warning: 'text-caramel',  
  info:    'text-[#455A64]',
  confirm: 'text-caramel',
}[type.value]))

const iconComponent = computed(() => ({
  success: CheckCircle2,
  error:   XCircle,
  warning: AlertTriangle,
  info:    Info,
  confirm: HelpCircle,
}[type.value]))

const titleColorClass = computed(() => ({
  success: 'text-espresso font-extrabold font-display',
  error:   'text-espresso font-extrabold font-display',
  warning: 'text-espresso font-extrabold font-display',
  info:    'text-espresso font-extrabold font-display',
  confirm: 'text-cream font-extrabold font-display',
}[type.value]))

const messageColorClass = computed(() => ({
  success: 'text-espresso/70',
  error:   'text-[#7A5A58]',
  warning: 'text-espresso/70',
  info:    'text-[#54626F]',
  confirm: 'text-cream/80',
}[type.value]))

const confirmBtnClass = computed(() => ({
  success: 'bg-espresso hover:bg-brown text-cream shadow-lg shadow-espresso/25',
  error:   'bg-[#C62828] hover:bg-[#A32222] text-white shadow-lg shadow-[#C62828]/25',
  warning: 'bg-caramel hover:bg-brown text-cream shadow-lg shadow-caramel/25',
  info:    'bg-espresso hover:bg-brown text-cream shadow-lg shadow-espresso/25',
  confirm: 'bg-caramel hover:bg-caramel-light hover:text-brown text-cream shadow-lg shadow-caramel/25',
}[type.value]))

const cancelBtnClass = computed(() => 'bg-white/5 hover:bg-white/10 text-cream/70 border-white/10')

const decorClass = computed(() => ({
  success: 'bg-caramel',
  error:   'bg-red-500',
  warning: 'bg-caramel',
  info:    'bg-espresso',
  confirm: 'bg-caramel',
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
