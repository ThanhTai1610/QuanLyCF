/**
 * Store quản lý thông tin quán (tên, địa chỉ, mô tả...).
 * Fetch 1 lần khi app khởi động, dùng chung cho toàn bộ ứng dụng.
 * Khi admin lưu cài đặt mới → gọi setInfo() để cập nhật ngay lập tức.
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useStoreInfoStore = defineStore('storeInfo', () => {
  const tenQuan     = ref('BrewManager')
  const diaChi      = ref('')
  const soDienThoai = ref('')
  const moTaQuan    = ref('')
  const gioMoCua    = ref('')
  const anhTrangChu = ref('')
  const tenAI       = ref('Barista AI')
  const xungHoAI    = ref('tôi - bạn')
  const loaded      = ref(false)

  /** Tên quán + " Coffee" nếu chưa có thêm gì */
  const tenQuanFull = computed(() => tenQuan.value || 'BrewManager')

  /** Fetch từ API công khai — không cần token */
  async function fetchInfo() {
    if (loaded.value) return          // Không fetch lại nếu đã có
    try {
      const res = await fetch('/api/settings/store-info')
      if (res.ok) {
        const data = await res.json()
        setInfo(data)
      }
    } catch {
      // Giữ giá trị mặc định nếu không kết nối được
    } finally {
      loaded.value = true
    }
  }

  /** Cập nhật store ngay lập tức (sau khi admin lưu cài đặt) */
  function setInfo(data: {
    tenQuan?: string
    diaChi?: string
    soDienThoai?: string
    moTaQuan?: string
    gioMoCua?: string
    anhTrangChu?: string
    tenAI?: string
    xungHoAI?: string
  }) {
    if (data.tenQuan     !== undefined) tenQuan.value     = data.tenQuan     || 'BrewManager'
    if (data.diaChi      !== undefined) diaChi.value      = data.diaChi      || ''
    if (data.soDienThoai !== undefined) soDienThoai.value = data.soDienThoai || ''
    if (data.moTaQuan    !== undefined) moTaQuan.value    = data.moTaQuan    || ''
    if (data.gioMoCua    !== undefined) gioMoCua.value    = data.gioMoCua    || ''
    if (data.anhTrangChu !== undefined) anhTrangChu.value = data.anhTrangChu || ''
    if (data.tenAI       !== undefined) tenAI.value       = data.tenAI       || 'Barista AI'
    if (data.xungHoAI    !== undefined) xungHoAI.value    = data.xungHoAI    || 'tôi - bạn'
  }

  /** Xoá cache để fetch lại lần tiếp theo */
  function invalidate() {
    loaded.value = false
  }

  return { tenQuan, tenQuanFull, diaChi, soDienThoai, moTaQuan, gioMoCua, anhTrangChu, tenAI, xungHoAI, loaded, fetchInfo, setInfo, invalidate }
})
