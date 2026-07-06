import { defineStore } from 'pinia'
import { ref, watch } from 'vue'
import { mockOrders, type Order, type OrderItem, type OrderStatus } from '@/data/orders'

const LOCAL_STORAGE_KEY = 'brew_orders_store_data'

/**
 * Store đơn hàng trung tâm — nguồn dữ liệu DUY NHẤT cho toàn bộ luồng:
 * Khách gọi món (CustomerMenu) → Bếp (Kitchen) → Đơn hàng (Orders) → Thanh toán (Payment)
 * và bán tại quầy (POSSale). Mọi trang đọc/ghi qua store này thay vì giữ mock riêng.
 */
export const useOrderStore = defineStore('orders', () => {
  // Load from localStorage if present, otherwise use mockOrders
  const saved = localStorage.getItem(LOCAL_STORAGE_KEY)
  const orders = ref<Order[]>(
    saved ? JSON.parse(saved) : mockOrders.map(o => ({ ...o, items: o.items.map(i => ({ ...i })) }))
  )

  // Watch for changes and save to localStorage
  watch(orders, (newVal) => {
    localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(newVal))
  }, { deep: true })

  // Bộ sinh mã đơn nối tiếp seed động dựa trên mã cao nhất hiện có
  const getNextSeq = () => {
    if (orders.value.length === 0) return 2042
    const ids = orders.value
      .map(o => {
        const match = o.id.match(/DH-(\d+)/)
        return match ? parseInt(match[1]) : 0
      })
      .filter(id => !isNaN(id))
    const maxId = Math.max(...ids, 2041)
    return maxId + 1
  }
  let seq = getNextSeq()
  const nextId = () => `DH-${seq++}`

  // Cờ báo hiệu gọi POS (Bếp pha xong)
  const posNotification = ref<{ table: string; time: number } | null>(null)
  
  // Danh sách các món đang báo hết trên toàn hệ thống
  const globalOutOfStock = ref<Set<string>>(new Set())

  const getById = (id: string) => orders.value.find(o => o.id === id)

  /** Tạo đơn mới (mặc định: chờ xác nhận, chưa thanh toán) */
  function createOrder(payload: {
    table: string
    items: OrderItem[]
    customer?: string
    status?: OrderStatus
    paid?: boolean
    paymentMethod?: string
    isPriority?: boolean
  }): Order {
    const now = new Date()
    const items = payload.items.map(i => ({ ...i }))
    const newId = nextId()
    
    // Xử lý đơn mang về không hiện bàn mà hiện số riêng (lấy theo mã đơn hoặc id)
    let displayTable = payload.table
    if (!displayTable || displayTable.toLowerCase() === 'mang về' || displayTable.toLowerCase() === 'takeaway') {
       displayTable = `Mang về - #${newId.replace('DH-', '')}`
    }

    const order: Order = {
      id: newId,
      table: displayTable,
      items,
      total: items.reduce((s, i) => s + i.price * i.qty, 0),
      status: payload.status ?? 'pending',
      createdAt: now.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
      createdTs: now.getTime(),
      customer: payload.customer,
      paid: payload.paid ?? false,
      paymentMethod: payload.paymentMethod,
      isPriority: payload.isPriority ?? false,
    }
    orders.value.unshift(order)
    return order
  }

  function updateStatus(id: string, status: OrderStatus, cancelReason?: string) {
    const o = getById(id)
    if (o) {
      o.status = status
      if (status === 'cancelled' && cancelReason) {
        o.cancelReason = cancelReason
      } else if (status !== 'cancelled') {
        o.cancelReason = undefined
      }
    }
  }

  /** Đánh dấu đã thanh toán (đồng thời coi như hoàn thành nếu còn đang xử lý) */
  function markPaid(id: string, method: string) {
    const o = getById(id)
    if (!o) return
    o.paid = true
    o.paymentMethod = method
    if (o.status === 'pending' || o.status === 'preparing') o.status = 'done'
  }

  // ── Thao tác tại bếp (theo mã đơn + vị trí món) ──────────────────
  function toggleItemDone(id: string, idx: number) {
    const it = getById(id)?.items[idx]
    if (it && !it.outOfStock) it.done = !it.done
  }

  function setAssignee(id: string, idx: number, name: string) {
    const it = getById(id)?.items[idx]
    if (it) it.assignee = name || undefined
  }

  function toggleOutOfStock(id: string, idx: number) {
    const it = getById(id)?.items[idx]
    if (!it) return
    it.outOfStock = !it.outOfStock
    if (it.outOfStock) {
       it.done = false
       globalOutOfStock.value.add(it.name)
    } else {
       globalOutOfStock.value.delete(it.name)
    }
    // Kích hoạt reactivity cho Set
    globalOutOfStock.value = new Set(globalOutOfStock.value)
  }

  function notifyPos(table: string) {
    posNotification.value = { table, time: Date.now() }
  }

  return {
    orders,
    getById,
    createOrder,
    updateStatus,
    markPaid,
    toggleItemDone,
    setAssignee,
    toggleOutOfStock,
    notifyPos,
    globalOutOfStock,
    posNotification,
  }
})
