import { defineStore } from 'pinia'
import { ref } from 'vue'
import { mockOrders, type Order, type OrderItem, type OrderStatus } from '@/data/orders'
import { ordersApi } from '@/services/orders'

/**
 * Store đơn hàng trung tâm — nguồn dữ liệu DUY NHẤT cho toàn bộ luồng:
 * Khách gọi món (CustomerMenu) → Bếp (Kitchen) → Đơn hàng (Orders) → Thanh toán (Payment)
 * và bán tại quầy (POSSale). Mọi trang đọc/ghi qua store này thay vì giữ mock riêng.
 */
export const useOrderStore = defineStore('orders', () => {
  // Nhân bản sâu seed để không sửa trực tiếp dữ liệu mẫu
  const orders = ref<Order[]>(
    mockOrders.map(o => ({ ...o, items: o.items.map(i => ({ ...i })) }))
  )

  // Bộ sinh mã đơn nối tiếp seed (seed cao nhất là DH-2041)
  let seq = 2042
  const nextId = () => `DH-${seq++}`

  const getById = (id: string) => {
    const cleanId = id.replace('DH-', '')
    return orders.value.find(o => o.id === id || o.id === `DH-${id}` || (o.originalId && String(o.originalId) === cleanId))
  }

  /** Tạo đơn mới (mặc định: chờ xác nhận, chưa thanh toán) */
  function createOrder(payload: {
    table: string
    items: OrderItem[]
    customer?: string
    status?: OrderStatus
    paid?: boolean
    paymentMethod?: string
    pointsDiscount?: number
    promoDiscount?: number
    maKhuyenMai?: number
  }): Order {
    const now = new Date()
    const items = payload.items.map(i => ({ ...i }))
    const order: Order = {
      id: nextId(),
      table: payload.table,
      items,
      total: Math.max(0, items.reduce((s, i) => s + i.price * i.qty, 0) - (payload.pointsDiscount ?? 0) - (payload.promoDiscount ?? 0)),
      status: payload.status ?? 'pending',
      createdAt: now.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
      createdTs: now.getTime(),
      customer: payload.customer,
      paid: payload.paid ?? false,
      paymentMethod: payload.paymentMethod,
      pointsDiscount: payload.pointsDiscount ?? 0,
      promoDiscount: payload.promoDiscount ?? 0,
      maKhuyenMai: payload.maKhuyenMai,
    }
    orders.value.unshift(order)
    return order
  }

  // Map BE status to FE status
  const mapStatus = (beStatus: string): OrderStatus => {
    switch (beStatus) {
      case 'ChoXacNhan': return 'pending'
      case 'DangPha': return 'preparing'
      case 'HoanThanh': return 'done'
      case 'Huy': return 'cancelled'
      default: return 'pending'
    }
  }

  // Map FE status to BE status
  const unmapStatus = (feStatus: OrderStatus): string => {
    switch (feStatus) {
      case 'pending': return 'ChoXacNhan'
      case 'preparing': return 'DangPha'
      case 'done': return 'HoanThanh'
      case 'cancelled': return 'Huy'
      default: return 'ChoXacNhan'
    }
  }

  async function fetchOrders() {
    try {
      const res = await ordersApi.active()
      // Map BE to FE format
      orders.value = res.map(o => ({
        id: `DH-${o.maDonHang}`,
        originalId: o.maDonHang, // Keep reference to real DB id
        table: o.tenBan || (o.loaiDonHang === 'TakeAway' ? `Mang về - #${o.maDonHang}` : 'Bàn trống'),
        items: o.items.map(i => ({
          id: i.maChiTiet.toString(),
          name: i.tenMon + (i.tenKichCo ? ` (${i.tenKichCo})` : ''),
          qty: i.soLuong,
          price: i.donGia,
          note: i.ghiChuMon || '',
          category: 'Đồ uống',
          done: i.trangThaiBep === 'HoanThanh',
        })),
        total: o.thanhTien,
        status: mapStatus(o.trangThaiDon),
        createdAt: new Date(o.thoiGianTao).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }),
        createdTs: new Date(o.thoiGianTao).getTime(),
        paid: false, // You would need actual data for this if it's not in OrderDto
      }))
    } catch (err) {
      console.error('Failed to fetch orders:', err)
    }
  }

  async function updateStatus(id: string, status: OrderStatus, cancelReason?: string) {
    const o = getById(id)
    if (o) {
      const beId = parseInt(id.replace('DH-', ''))
      try {
        if (status === 'cancelled') {
           await ordersApi.cancel(beId, cancelReason)
        } else {
           await ordersApi.updateStatus(beId, unmapStatus(status))
        }
        // Update local state optimistic
        o.status = status
        if (status === 'cancelled' && cancelReason) {
          o.cancelReason = cancelReason
        } else if (status !== 'cancelled') {
          o.cancelReason = undefined
        }
      } catch (err) {
        console.error('Failed to update status:', err)
      }
    }
  }

  /** Đánh dấu đã thanh toán (đồng thời coi như hoàn thành nếu còn đang xử lý) */
  function markPaid(id: string, method: string) {
    const o = getById(id)
    if (!o) return
    o.paid = true
    o.paymentMethod = method
    if (o.status === 'pending' || o.status === 'preparing') updateStatus(id, 'done')
  }

  // ── Thao tác tại bếp (theo mã đơn + vị trí món) ──────────────────
  function toggleItemDone(id: string, idx: number) {
    const it = getById(id)?.items[idx]
    if (it && !it.outOfStock) it.done = !it.done
    // Ideally this should call an API to update item status
  function setAssignee(id: string, idx: number, name: string) {
    const it = getById(id)?.items[idx]
    if (it) it.assignee = name || undefined
  }

  const globalOutOfStock = ref<Set<string>>(new Set())
  const posNotification = ref<{ table: string } | null>(null)

  function notifyPos(table: string) {
    posNotification.value = { table }
  }

  function toggleOutOfStock(id: string, idx: number) {
    const it = getById(id)?.items[idx]
    if (!it) return
    it.outOfStock = !it.outOfStock
    if (it.outOfStock) {
      it.done = false
      globalOutOfStock.value.add(it.name)
      const cleanName = it.name.replace(/\s*\([^)]*\)$/, '')
      globalOutOfStock.value.add(cleanName)
    } else {
      globalOutOfStock.value.delete(it.name)
      const cleanName = it.name.replace(/\s*\([^)]*\)$/, '')
      globalOutOfStock.value.delete(cleanName)
    }
    globalOutOfStock.value = new Set(globalOutOfStock.value)
  }

  return {
    orders,
    getById,
    createOrder,
    fetchOrders,
    updateStatus,
    markPaid,
    toggleItemDone,
    setAssignee,
    toggleOutOfStock,
    globalOutOfStock,
    posNotification,
    notifyPos,
  }
})
