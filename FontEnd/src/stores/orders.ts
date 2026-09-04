import { defineStore } from 'pinia'
import { ref } from 'vue'
import { type Order, type OrderItem, type OrderStatus } from '@/data/orders'
import { ordersApi } from '@/services/orders'

/**
 * Store đơn hàng trung tâm — nguồn dữ liệu DUY NHẤT cho toàn bộ luồng:
 * Khách gọi món (CustomerMenu) → Bếp (Kitchen) → Đơn hàng (Orders) → Thanh toán (Payment)
 * và bán tại quầy (POSSale). Mọi trang đọc/ghi qua store này thay vì giữ mock riêng.
 */
function parseBeDate(raw: string | undefined): { timeStr: string; ts: number } {
  if (!raw) {
    const d = new Date()
    return { timeStr: d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }), ts: d.getTime() }
  }
  let str = String(raw).trim()
  if (!str.endsWith('Z') && !str.includes('+') && !str.includes('-', 10)) {
    str += 'Z'
  }
  let d = new Date(str)
  if (isNaN(d.getTime())) {
    d = new Date(raw)
  }
  const ts = isNaN(d.getTime()) ? Date.now() : d.getTime()
  const timeStr = isNaN(d.getTime()) ? '' : d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
  return { timeStr, ts }
}

const syncChannel = typeof BroadcastChannel !== 'undefined' ? new BroadcastChannel('quanlycf_orders_sync') : null

function notifyOrderChange() {
  if (syncChannel) {
    try {
      syncChannel.postMessage({ type: 'ORDERS_CHANGED', ts: Date.now() })
    } catch (e) {
      console.error(e)
    }
  }
}

export const useOrderStore = defineStore('orders', () => {
  // Nguồn dữ liệu thật đồng bộ từ Backend SQL Database
  const orders = ref<Order[]>([])

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
      case 'ChoThanhToan': return 'pending'
      case 'ChoXacNhan': return 'pending'
      case 'DangPha': return 'preparing'
      case 'DaPhaXong': return 'ready'
      case 'HoanThanh': return 'done'
      case 'DaDongBan': return 'done'
      case 'Huy': return 'cancelled'
      default: return 'pending'
    }
  }

  // Map FE status to BE status
  const unmapStatus = (feStatus: OrderStatus): string => {
    switch (feStatus) {
      case 'pending': return 'ChoXacNhan'
      case 'preparing': return 'DangPha'
      case 'ready': return 'DaPhaXong'
      case 'done': return 'HoanThanh'
      case 'cancelled': return 'Huy'
      default: return 'ChoXacNhan'
    }
  }

  async function fetchOrders() {
    try {
      // Gọi API chuyên biệt cho Bếp (Query trực tiếp SQL loại bỏ hoàn toàn đơn ChoThanhToan)
      const res = await ordersApi.kitchenActive()
      orders.value = res.map(o => {
        const parsed = parseBeDate(o.thoiGianTao)
        return {
          id: `DH-${o.maDonHang}`,
          originalId: o.maDonHang, // Keep reference to real DB id
          maBan: o.maBan,
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
          createdAt: parsed.timeStr,
          createdTs: parsed.ts,
          paid: o.trangThaiDon !== 'ChoThanhToan',
        }
      })
    } catch (err) {
      console.error('Failed to fetch orders from backend:', err)
    }
  }

  async function fetchAllOrders() {
    try {
      const res = await ordersApi.getAll()
      orders.value = res.map(o => {
        const parsed = parseBeDate(o.thoiGianTao)
        return {
          id: `DH-${o.maDonHang}`,
          originalId: o.maDonHang,
          maBan: o.maBan,
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
          createdAt: parsed.timeStr,
          createdTs: parsed.ts,
          paid: o.trangThaiDon !== 'ChoThanhToan',
        }
      })
    } catch (err) {
      console.error('Failed to fetch all orders from backend:', err)
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
        notifyOrderChange()
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

  async function toggleItemDone(id: string, idx: number) {
    const it = getById(id)?.items[idx]
    if (it && !it.outOfStock) {
      const newDone = !it.done
      it.done = newDone
      const maChiTiet = parseInt(it.id, 10)
      if (!isNaN(maChiTiet) && maChiTiet > 0) {
        try {
          await ordersApi.updateItemKitchenStatus(maChiTiet, newDone ? 'HoanThanh' : 'ChoLam')
        } catch (err) {
          console.error('Lỗi khi cập nhật trạng thái món bếp:', err)
        }
      }
    }
  }

  function setAssignee(id: string, idx: number, name: string) {
    const it = getById(id)?.items[idx]
    if (it) it.assignee = name || undefined
  }

  const globalOutOfStock = ref<Set<string>>(new Set())
  const posNotification = ref<{ table: string } | null>(null)

  if (syncChannel) {
    syncChannel.onmessage = (event) => {
      const data = event.data
      if (data && data.type === 'OUT_OF_STOCK_UPDATE') {
        const { productName, cleanName, isOutOfStock } = data
        if (isOutOfStock) {
          if (productName) globalOutOfStock.value.add(productName)
          if (cleanName) globalOutOfStock.value.add(cleanName)
        } else {
          if (productName) globalOutOfStock.value.delete(productName)
          if (cleanName) globalOutOfStock.value.delete(cleanName)
        }
        globalOutOfStock.value = new Set(globalOutOfStock.value)
      } else if (data && data.type === 'ORDERS_CHANGED') {
        fetchOrders()
      }
    }
  }

  function broadcastOutOfStock(productName: string, cleanName: string, isOutOfStock: boolean) {
    if (syncChannel) {
      syncChannel.postMessage({
        type: 'OUT_OF_STOCK_UPDATE',
        productName,
        cleanName,
        isOutOfStock
      })
    }
  }

  function notifyPos(table: string) {
    posNotification.value = { table }
  }

  function toggleOutOfStock(id: string, idx: number) {
    const it = getById(id)?.items[idx]
    if (!it) return
    it.outOfStock = !it.outOfStock
    const cleanName = it.name.replace(/\s*\([^)]*\)$/, '').trim()
    if (it.outOfStock) {
      it.done = false
      globalOutOfStock.value.add(it.name)
      globalOutOfStock.value.add(cleanName)
    } else {
      globalOutOfStock.value.delete(it.name)
      globalOutOfStock.value.delete(cleanName)
    }
    globalOutOfStock.value = new Set(globalOutOfStock.value)
    broadcastOutOfStock(it.name, cleanName, it.outOfStock)
  }

  function setOutOfStock(productName: string, isOutOfStock: boolean) {
    const cleanName = productName.replace(/\s*\([^)]*\)$/, '').trim()
    if (isOutOfStock) {
      globalOutOfStock.value.add(productName)
      globalOutOfStock.value.add(cleanName)
    } else {
      globalOutOfStock.value.delete(productName)
      globalOutOfStock.value.delete(cleanName)
    }
    globalOutOfStock.value = new Set(globalOutOfStock.value)
    broadcastOutOfStock(productName, cleanName, isOutOfStock)
  }

  function clearOutOfStock(productName: string) {
    setOutOfStock(productName, false)
  }

  return {
    orders,
    getById,
    createOrder,
    fetchOrders,
    fetchAllOrders,
    updateStatus,
    markPaid,
    toggleItemDone,
    setAssignee,
    toggleOutOfStock,
    setOutOfStock,
    clearOutOfStock,
    globalOutOfStock,
    posNotification,
    notifyPos,
  }
})
