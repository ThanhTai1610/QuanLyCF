/**
 * Toast notification system — dùng chung toàn bộ ứng dụng.
 * Sử dụng: const toast = useToast()
 *          toast.success('Lưu thành công!')
 *          toast.error('Có lỗi xảy ra!')
 *          toast.warning('Đang bảo trì!')
 *          toast.info('Thông tin')
 */
import { defineStore } from 'pinia'
import { ref } from 'vue'

export type ToastType = 'success' | 'error' | 'warning' | 'info'

export interface Toast {
  id: number
  type: ToastType
  title?: string
  message: string
  duration: number
}

let _id = 0

export const useToastStore = defineStore('toast', () => {
  const toasts = ref<Toast[]>([])

  function add(type: ToastType, message: string, title?: string, duration = 3500) {
    const id = ++_id
    toasts.value.push({ id, type, title, message, duration })
    setTimeout(() => remove(id), duration)
    return id
  }

  function remove(id: number) {
    const idx = toasts.value.findIndex(t => t.id === id)
    if (idx !== -1) toasts.value.splice(idx, 1)
  }

  return {
    toasts,
    success: (message: string, title?: string, duration?: number) => add('success', message, title, duration),
    error:   (message: string, title?: string, duration?: number) => add('error',   message, title, duration ?? 5000),
    warning: (message: string, title?: string, duration?: number) => add('warning', message, title, duration),
    info:    (message: string, title?: string, duration?: number) => add('info',    message, title, duration),
    remove,
  }
})

/** Shorthand composable — dùng như hook */
export function useToast() {
  return useToastStore()
}
