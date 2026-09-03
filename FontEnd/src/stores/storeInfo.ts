/**
 * Store quản lý thông tin quán (tên, địa chỉ, mô tả...).
 * Đồng bộ ngay lập tức theo thời gian thực (Realtime & Cross-Tab BroadcastChannel).
 * Khi admin thay đổi cài đặt -> tất cả các trang/tab lập tức cập nhật không cần reload.
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '@/services/api'

const STORAGE_KEY = 'quanlycf_store_info'
const channel = typeof BroadcastChannel !== 'undefined' ? new BroadcastChannel('quanlycf_store_info_sync') : null

export const useStoreInfoStore = defineStore('storeInfo', () => {
  const tenQuan     = ref('cà phê F6')
  const diaChi      = ref('123 Nguyễn Huệ, Quận 1, TP.HCM')
  const soDienThoai = ref('1111')
  const moTaQuan    = ref('Quán cà phê đặc sản với không gian ấm cúng. Phục vụ cà phê pha máy, trà, bánh ngọt và các loại đồ uống đá xay.')
  const gioMoCua    = ref('07:00 - 22:30')
  const anhTrangChu = ref('')
  const tenAI       = ref('Barista AI')
  const xungHoAI    = ref('tôi - bạn')
  const cheDoBaoTri     = ref(false)
  const thongDiepBaoTri = ref('')
  const loaded      = ref(false)

  /** Tên quán đầy đủ */
  const tenQuanFull = computed(() => tenQuan.value || 'cà phê F6')

  // Đọc dữ liệu đã cache từ localStorage nếu có
  try {
    const cached = localStorage.getItem(STORAGE_KEY)
    if (cached) {
      const parsed = JSON.parse(cached)
      applyData(parsed)
    }
  } catch {}

  // Lắng nghe BroadcastChannel từ các tab/cửa sổ khác
  if (channel) {
    channel.onmessage = (e) => {
      if (e.data?.type === 'STORE_INFO_UPDATED' && e.data?.data) {
        applyData(e.data.data)
      }
    }
  }

  // Lắng nghe sự kiện storage của trình duyệt
  if (typeof window !== 'undefined') {
    window.addEventListener('storage', (e) => {
      if (e.key === STORAGE_KEY && e.newValue) {
        try {
          const parsed = JSON.parse(e.newValue)
          applyData(parsed)
        } catch {}
      }
    })
  }

  function applyData(data: any) {
    if (data.tenQuan         !== undefined) tenQuan.value         = data.tenQuan         || 'cà phê F6'
    if (data.diaChi          !== undefined) diaChi.value          = data.diaChi          || ''
    if (data.soDienThoai     !== undefined) soDienThoai.value     = data.soDienThoai     || ''
    if (data.moTaQuan        !== undefined) moTaQuan.value        = data.moTaQuan        || ''
    if (data.gioMoCua        !== undefined) gioMoCua.value        = data.gioMoCua        || ''
    if (data.anhTrangChu     !== undefined) anhTrangChu.value     = data.anhTrangChu     || ''
    if (data.tenAI           !== undefined) tenAI.value           = data.tenAI           || 'Barista AI'
    if (data.xungHoAI        !== undefined) xungHoAI.value        = data.xungHoAI        || 'tôi - bạn'
    if (data.cheDoBaoTri     !== undefined) cheDoBaoTri.value     = !!data.cheDoBaoTri
    if (data.thongDiepBaoTri !== undefined) thongDiepBaoTri.value = data.thongDiepBaoTri || ''
  }

  /** Fetch từ API Backend */
  async function fetchInfo(force = false) {
    if (loaded.value && !force) return
    try {
      const data = await api.get<any>('/api/settings/store-info')
      if (data) {
        setInfo(data, true)
      }
    } catch (err: any) {
      if (err?.status === 503 || err?.response?.status === 503) {
        cheDoBaoTri.value = true
      }
    } finally {
      loaded.value = true
    }
  }

  /** Cập nhật store ngay lập tức & phát sóng cho tất cả các tab khác */
  function setInfo(
    data: {
      tenQuan?: string
      diaChi?: string
      soDienThoai?: string
      moTaQuan?: string
      gioMoCua?: string
      anhTrangChu?: string
      tenAI?: string
      xungHoAI?: string
      cheDoBaoTri?: boolean
      thongDiepBaoTri?: string
    },
    broadcast = true
  ) {
    applyData(data)

    const infoObj = {
      tenQuan: tenQuan.value,
      diaChi: diaChi.value,
      soDienThoai: soDienThoai.value,
      moTaQuan: moTaQuan.value,
      gioMoCua: gioMoCua.value,
      anhTrangChu: anhTrangChu.value,
      tenAI: tenAI.value,
      xungHoAI: xungHoAI.value,
      cheDoBaoTri: cheDoBaoTri.value,
      thongDiepBaoTri: thongDiepBaoTri.value,
    }

    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(infoObj))
    } catch {}

    if (broadcast && channel) {
      channel.postMessage({ type: 'STORE_INFO_UPDATED', data: infoObj })
    }
  }

  function invalidate() {
    loaded.value = false
  }

  return {
    tenQuan,
    tenQuanFull,
    diaChi,
    soDienThoai,
    moTaQuan,
    gioMoCua,
    anhTrangChu,
    tenAI,
    xungHoAI,
    cheDoBaoTri,
    thongDiepBaoTri,
    loaded,
    fetchInfo,
    setInfo,
    invalidate,
  }
})
