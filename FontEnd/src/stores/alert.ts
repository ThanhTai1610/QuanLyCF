/**
 * Alert Dialog — thông báo trung tâm màn hình.
 * Dùng như: const alert = useAlert()
 *           await alert.success('Lưu thành công!', 'Tất cả thay đổi đã được lưu.')
 *           await alert.confirm('Bạn có chắc?', 'Thao tác này không thể hoàn tác.')
 */
import { defineStore } from 'pinia'
import { ref } from 'vue'

export type AlertType = 'success' | 'error' | 'warning' | 'info' | 'confirm'

export interface AlertOptions {
  type: AlertType
  title: string
  message?: string
  confirmText?: string
  cancelText?: string
}

export const useAlertStore = defineStore('alert', () => {
  const visible  = ref(false)
  const options  = ref<AlertOptions>({ type: 'info', title: '' })
  let _resolve: ((confirmed: boolean) => void) | null = null

  function show(opts: AlertOptions): Promise<boolean> {
    options.value = opts
    visible.value = true
    return new Promise(resolve => { _resolve = resolve })
  }

  function confirm() {
    visible.value = false
    _resolve?.(true)
    _resolve = null
  }

  function cancel() {
    visible.value = false
    _resolve?.(false)
    _resolve = null
  }

  return { visible, options, show, confirm, cancel }
})

export function useAlert() {
  const store = useAlertStore()

  return {
    success: (title: string, message?: string) =>
      store.show({ type: 'success', title, message, confirmText: 'Hoàn tất' }),

    error: (title: string, message?: string) =>
      store.show({ type: 'error', title, message, confirmText: 'Đã hiểu' }),

    warning: (title: string, message?: string) =>
      store.show({ type: 'warning', title, message, confirmText: 'Xác nhận' }),

    info: (title: string, message?: string) =>
      store.show({ type: 'info', title, message, confirmText: 'OK' }),

    confirm: (title: string, message?: string, confirmText = 'Xác nhận', cancelText = 'Hủy') =>
      store.show({ type: 'confirm', title, message, confirmText, cancelText }),
  }
}
