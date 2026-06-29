<template>
  <Teleport to="body">
    <div
      class="fixed bottom-6 right-6 z-[9999] flex flex-col gap-3 pointer-events-none"
      aria-live="polite"
    >
      <TransitionGroup name="toast" tag="div" class="flex flex-col gap-3">
        <div
          v-for="toast in toastStore.toasts"
          :key="toast.id"
          class="toast-item pointer-events-auto flex items-start gap-4 min-w-[420px] max-w-[520px] px-5 py-4 rounded-2xl shadow-2xl border backdrop-blur-xl cursor-pointer select-none"
          :class="variantClass(toast.type)"
          @click="toastStore.remove(toast.id)"
        >
          <!-- Icon -->
          <div class="mt-0.5 shrink-0 w-11 h-11 rounded-full flex items-center justify-center" :class="iconBgClass(toast.type)">
            <component :is="iconComponent(toast.type)" class="w-6 h-6" :class="iconColorClass(toast.type)" stroke-width="2.5" />
          </div>

          <!-- Content -->
          <div class="flex-1 min-w-0">
            <p v-if="toast.title" class="text-base font-bold leading-snug" :class="titleColorClass(toast.type)">
              {{ toast.title }}
            </p>
            <p class="text-sm leading-snug mt-1" :class="toast.title ? 'text-white/75' : titleColorClass(toast.type)">
              {{ toast.message }}
            </p>
          </div>

          <!-- Progress bar -->
          <div class="absolute bottom-0 left-0 right-0 h-[3px] rounded-b-2xl overflow-hidden">
            <div
              class="h-full rounded-full opacity-60"
              :class="progressClass(toast.type)"
              :style="{ animationDuration: toast.duration + 'ms' }"
              style="animation: toastProgress linear forwards;"
            />
          </div>

          <!-- Close button -->
          <button
            class="shrink-0 mt-0.5 text-white/40 hover:text-white/80 transition-colors"
            @click.stop="toastStore.remove(toast.id)"
          >
            <X class="w-4 h-4" stroke-width="2" />
          </button>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { useToastStore, type ToastType } from '@/stores/toast'
import { CheckCircle2, XCircle, AlertTriangle, Info, X } from 'lucide-vue-next'

const toastStore = useToastStore()

function variantClass(type: ToastType) {
  return {
    success: 'bg-[#1A2218]/95 border-[#2D5A27]/60 relative overflow-hidden',
    error:   'bg-[#221818]/95 border-[#5A2727]/60 relative overflow-hidden',
    warning: 'bg-[#221D10]/95 border-[#5A4A15]/60 relative overflow-hidden',
    info:    'bg-[#1A1C22]/95 border-[#2D3A5A]/60 relative overflow-hidden',
  }[type]
}

function iconBgClass(type: ToastType) {
  return {
    success: 'bg-[#22C55E]/15',
    error:   'bg-[#EF4444]/15',
    warning: 'bg-[#F59E0B]/15',
    info:    'bg-[#3B82F6]/15',
  }[type]
}

function iconColorClass(type: ToastType) {
  return {
    success: 'text-[#22C55E]',
    error:   'text-[#EF4444]',
    warning: 'text-[#F59E0B]',
    info:    'text-[#3B82F6]',
  }[type]
}

function titleColorClass(type: ToastType) {
  return {
    success: 'text-[#86EFAC]',
    error:   'text-[#FCA5A5]',
    warning: 'text-[#FDE68A]',
    info:    'text-[#93C5FD]',
  }[type]
}

function progressClass(type: ToastType) {
  return {
    success: 'bg-[#22C55E]',
    error:   'bg-[#EF4444]',
    warning: 'bg-[#F59E0B]',
    info:    'bg-[#3B82F6]',
  }[type]
}

function iconComponent(type: ToastType) {
  return {
    success: CheckCircle2,
    error:   XCircle,
    warning: AlertTriangle,
    info:    Info,
  }[type]
}
</script>

<style scoped>
/* Slide in từ phải */
.toast-enter-active {
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.toast-leave-active {
  transition: all 0.25s cubic-bezier(0.4, 0, 1, 1);
}
.toast-enter-from {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}
.toast-leave-to {
  opacity: 0;
  transform: translateX(100%) scale(0.9);
}
.toast-move {
  transition: transform 0.3s ease;
}

/* Progress bar animation */
@keyframes toastProgress {
  from { width: 100%; }
  to   { width: 0%; }
}
</style>
