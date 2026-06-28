import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface CartOption {
  size: string
  toppings: Array<{ name: string, price: number, qty: number }>
  sugar: string
  ice: string
  note: string
  extraPrice: number
}

export interface CartItem {
  cartLineId: string
  item: any // using any for now, but should be the type of menuItems element
  qty: number
  options?: CartOption
}

export const useCartStore = defineStore('cart', () => {
  const lines = ref<CartItem[]>([])

  function add(item: any, options?: CartOption) {
    const itemId = item.maSanPham || item.id // Support both backend and mock
    if (!options) {
      const existing = lines.value.find((l) => l.cartLineId === String(itemId))
      if (existing) {
        existing.qty += 1
        return
      }
      lines.value.push({ cartLineId: String(itemId), item, qty: 1 })
    } else {
      lines.value.push({ 
        cartLineId: Math.random().toString(36).substring(2, 9), 
        item, 
        qty: 1, 
        options 
      })
    }
  }

  function setQty(cartLineId: string, qty: number) {
    if (qty <= 0) {
      remove(cartLineId)
      return
    }
    const existing = lines.value.find((l) => l.cartLineId === cartLineId)
    if (existing) {
      existing.qty = qty
    }
  }

  function remove(cartLineId: string) {
    lines.value = lines.value.filter((l) => l.cartLineId !== cartLineId)
  }

  function clear() {
    lines.value = []
  }

  function total() {
    return lines.value.reduce((s, l) => {
      const price = l.item.giaBan || l.item.price || 0
      const itemPrice = price + (l.options?.extraPrice || 0)
      return s + itemPrice * l.qty
    }, 0)
  }

  function count() {
    return lines.value.reduce((s, l) => s + l.qty, 0)
  }

  function updateOptions(cartLineId: string, options: CartOption) {
    const existing = lines.value.find((l) => l.cartLineId === cartLineId)
    if (existing) {
      existing.options = options
    }
  }

  return { lines, add, setQty, remove, clear, total, count, updateOptions }
})
