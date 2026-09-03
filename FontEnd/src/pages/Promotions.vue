<template>
  <div class="p-6 space-y-6">
    <!-- Header Page -->
    <div class="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
      <div>
        <h1 class="font-display text-2xl text-espresso font-bold flex items-center gap-2">
          <Tag class="w-6 h-6 text-caramel" />
          Khuyến mãi &amp; Voucher
        </h1>
        <p class="text-sm text-muted-foreground mt-0.5">Tạo chương trình giảm giá, quản lý mã voucher tri ân khách hàng khi thanh toán</p>
      </div>
      <Button @click="openAdd" class="bg-caramel text-cream hover:bg-brown rounded-xl border border-caramel/30 shadow-warm font-bold text-xs uppercase tracking-wider px-4 py-2.5 cursor-pointer">
        <Plus class="w-4 h-4 mr-1.5" /> Thêm khuyến mãi mới
      </Button>
    </div>

    <!-- 1. KPI Stats Cards Header -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="bg-card rounded-2xl border border-cream-deep p-4 shadow-sm flex items-center justify-between">
        <div>
          <span class="text-[11px] font-bold text-muted-foreground uppercase tracking-widest block">Đang hoạt động</span>
          <span class="text-2xl font-extrabold text-emerald-600 mt-1 block">{{ stats.activeCount }}</span>
        </div>
        <div class="w-11 h-11 rounded-xl bg-emerald-500/10 border border-emerald-500/20 text-emerald-600 flex items-center justify-center">
          <Tag class="w-5.5 h-5.5" />
        </div>
      </div>

      <div class="bg-card rounded-2xl border border-cream-deep p-4 shadow-sm flex items-center justify-between">
        <div>
          <span class="text-[11px] font-bold text-muted-foreground uppercase tracking-widest block">Tổng lượt đã dùng</span>
          <span class="text-2xl font-extrabold text-caramel mt-1 block">{{ stats.totalUsedCount.toLocaleString() }} lượt</span>
        </div>
        <div class="w-11 h-11 rounded-xl bg-caramel/10 border border-caramel/20 text-caramel flex items-center justify-center">
          <Sparkles class="w-5.5 h-5.5" />
        </div>
      </div>

      <div class="bg-card rounded-2xl border border-cream-deep p-4 shadow-sm flex items-center justify-between">
        <div>
          <span class="text-[11px] font-bold text-muted-foreground uppercase tracking-widest block">Ước tính ưu đãi</span>
          <span class="text-2xl font-extrabold text-espresso mt-1 block">{{ formatVND(stats.estimatedSavings) }}</span>
        </div>
        <div class="w-11 h-11 rounded-xl bg-amber-500/10 border border-amber-500/20 text-amber-600 flex items-center justify-center">
          <Percent class="w-5.5 h-5.5" />
        </div>
      </div>

      <div class="bg-card rounded-2xl border border-cream-deep p-4 shadow-sm flex items-center justify-between">
        <div>
          <span class="text-[11px] font-bold text-muted-foreground uppercase tracking-widest block">Hết hạn / Tạm tắt</span>
          <span class="text-2xl font-extrabold text-muted-foreground mt-1 block">{{ stats.inactiveCount }}</span>
        </div>
        <div class="w-11 h-11 rounded-xl bg-gray-100 border border-gray-200 text-gray-500 flex items-center justify-center">
          <Clock class="w-5.5 h-5.5" />
        </div>
      </div>
    </div>

    <!-- 2. Search & Filter Controls -->
    <div class="bg-card rounded-2xl border border-cream-deep p-4 shadow-sm flex flex-col md:flex-row md:items-center justify-between gap-4">
      <div class="relative flex-1 max-w-md">
        <Search class="absolute left-3.5 top-2.5 w-4 h-4 text-muted-foreground" />
        <input 
          v-model="searchQuery" 
          type="text" 
          placeholder="Tìm chương trình hoặc mã voucher..." 
          class="w-full bg-cream border border-cream-deep rounded-xl pl-9 pr-4 py-2.5 text-xs font-medium text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/20"
        />
      </div>

      <div class="flex items-center gap-1 bg-cream/50 p-1 rounded-xl border border-cream-deep overflow-x-auto">
        <button 
          @click="statusFilter = 'all'" 
          class="px-3 py-1.5 rounded-lg text-xs font-bold transition-all cursor-pointer whitespace-nowrap"
          :class="statusFilter === 'all' ? 'bg-espresso text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep/60'"
        >
          Tất cả ({{ promotions.length }})
        </button>
        <button 
          @click="statusFilter = 'active'" 
          class="px-3 py-1.5 rounded-lg text-xs font-bold transition-all cursor-pointer whitespace-nowrap"
          :class="statusFilter === 'active' ? 'bg-emerald-600 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep/60'"
        >
          🟢 Đang chạy ({{ stats.activeCount }})
        </button>
        <button 
          @click="statusFilter = 'expired'" 
          class="px-3 py-1.5 rounded-lg text-xs font-bold transition-all cursor-pointer whitespace-nowrap"
          :class="statusFilter === 'expired' ? 'bg-amber-600 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep/60'"
        >
          ⏰ Hết hạn / Hết lượt ({{ stats.expiredCount }})
        </button>
        <button 
          @click="statusFilter = 'off'" 
          class="px-3 py-1.5 rounded-lg text-xs font-bold transition-all cursor-pointer whitespace-nowrap"
          :class="statusFilter === 'off' ? 'bg-gray-600 text-white shadow-sm' : 'text-muted-foreground hover:bg-cream-deep/60'"
        >
          ⚪ Tạm tắt ({{ stats.disabledCount }})
        </button>
      </div>
    </div>

    <p v-if="errorMsg" class="text-sm font-semibold text-red-600 bg-red-50 border border-red-200 rounded-xl px-4 py-3">{{ errorMsg }}</p>

    <!-- 3. Promotions Table -->
    <div class="bg-card rounded-2xl border border-cream-deep shadow-card overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b-2 border-cream-deep text-left text-xs uppercase tracking-wider text-muted-foreground bg-cream/40">
              <th class="px-5 py-4 font-bold">Chương trình / Mã Voucher</th>
              <th class="px-5 py-4 font-bold">Mức giảm</th>
              <th class="px-5 py-4 font-bold">Điều kiện</th>
              <th class="px-5 py-4 font-bold">Thời hạn &amp; Lượt sử dụng</th>
              <th class="px-5 py-4 font-bold text-center">Bật / Tắt</th>
              <th class="px-5 py-4 font-bold text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-cream-deep/60">
            <tr v-for="p in filteredPromotions" :key="p.maKhuyenMai" class="hover:bg-cream/20 transition-colors">
              <td class="px-5 py-4">
                <div class="font-bold text-espresso text-sm">{{ p.tenChuongTrinh }}</div>
                <div v-if="p.maGiamGia" class="text-xs mt-1">
                  <span 
                    @click="copyCode(p.maGiamGia)" 
                    title="Bấm để chép mã" 
                    class="font-mono font-extrabold text-caramel bg-caramel/10 hover:bg-caramel/20 border border-caramel/30 px-2 py-0.5 rounded-lg cursor-pointer transition-colors inline-flex items-center gap-1 shadow-xs"
                  >
                    {{ p.maGiamGia }}
                    <Copy class="w-3 h-3 opacity-70" />
                  </span>
                </div>
                <div v-else class="text-[11px] text-muted-foreground mt-0.5 font-medium">Khuyến mãi tự động (không cần nhập mã)</div>
              </td>

              <td class="px-5 py-4 font-bold text-espresso">
                <span class="text-caramel font-extrabold text-base">
                  {{ p.loaiGiamGia === 'PhanTram' ? p.giaTriGiam + '%' : formatVND(p.giaTriGiam) }}
                </span>
                <div v-if="p.loaiGiamGia === 'PhanTram' && p.giamToiDa" class="text-[11px] text-muted-foreground font-medium">
                  Tối đa {{ formatVND(p.giamToiDa) }}
                </div>
              </td>

              <td class="px-5 py-4 text-muted-foreground text-xs">
                <span v-if="p.donToiThieu" class="font-bold text-espresso bg-cream/80 px-2 py-1 rounded-md border border-cream-deep">
                  Đơn ≥ {{ formatVND(p.donToiThieu) }}
                </span>
                <span v-else class="text-muted-foreground">— Tất cả đơn</span>
              </td>

              <td class="px-5 py-4 text-xs">
                <div class="text-espresso font-medium">
                  📅 {{ p.ngayBatDau || p.ngayKetThuc ? (fmtD(p.ngayBatDau) + ' → ' + fmtD(p.ngayKetThuc)) : 'Không giới hạn ngày' }}
                </div>
                <div class="text-muted-foreground mt-1 font-semibold flex items-center gap-2">
                  <span>
                    {{ p.soLuongGioiHan ? (`${p.soLuongDaDung} / ${p.soLuongGioiHan} lượt`) : (`Đã dùng ${p.soLuongDaDung} lượt`) }}
                  </span>
                </div>
                <!-- Progress bar cho lượt giới hạn -->
                <div v-if="p.soLuongGioiHan" class="w-28 bg-cream-deep/60 h-1.5 rounded-full overflow-hidden mt-1 border border-cream-deep">
                  <div 
                    class="h-full rounded-full transition-all duration-300"
                    :class="hetLuot(p) ? 'bg-red-500' : 'bg-caramel'"
                    :style="{ width: Math.min(100, Math.round((p.soLuongDaDung / p.soLuongGioiHan) * 100)) + '%' }"
                  ></div>
                </div>
              </td>

              <td class="px-5 py-4 text-center">
                <!-- Nút công tắc Quick Toggle Switch 1-Click -->
                <div class="flex flex-col items-center gap-1">
                  <button 
                    @click="toggleStatus(p)"
                    :disabled="togglingId === p.maKhuyenMai"
                    :title="isCurrentlyActive(p) ? 'Bấm để Tắt khuyến mãi' : 'Bấm để Bật / Chỉnh sửa thời hạn'"
                    class="relative inline-flex h-6 w-11 shrink-0 cursor-pointer rounded-full border-2 border-transparent transition-colors duration-200 ease-in-out focus:outline-none shadow-xs"
                    :class="isCurrentlyActive(p) ? 'bg-emerald-500' : 'bg-gray-300'"
                  >
                    <span 
                      class="pointer-events-none inline-block h-5 w-5 transform rounded-full bg-white shadow-md ring-0 transition duration-200 ease-in-out" 
                      :class="isCurrentlyActive(p) ? 'translate-x-5' : 'translate-x-0'"
                    ></span>
                  </button>
                  <span class="text-[10px] font-bold uppercase tracking-wider" :class="badgeTextClass(p)">
                    {{ statusText(p) }}
                  </span>
                </div>
              </td>

              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-1.5">
                  <button @click="openEdit(p)" title="Chỉnh sửa chương trình" class="w-8 h-8 rounded-lg border border-cream-deep flex items-center justify-center text-espresso hover:bg-cream-deep/50 transition-colors cursor-pointer">
                    <Pencil class="w-3.5 h-3.5 text-espresso" />
                  </button>
                  <button @click="removePromo(p)" title="Xoá chương trình" class="w-8 h-8 rounded-lg border border-destructive/30 text-destructive flex items-center justify-center hover:bg-destructive/10 transition-colors cursor-pointer">
                    <Trash2 class="w-3.5 h-3.5" />
                  </button>
                </div>
              </td>
            </tr>

            <tr v-if="!loading && filteredPromotions.length === 0">
              <td colspan="6" class="py-12 text-center text-muted-foreground text-xs font-semibold">
                Không tìm thấy khuyến mãi nào phù hợp. Bấm "Thêm khuyến mãi mới".
              </td>
            </tr>
            <tr v-if="loading">
              <td colspan="6" class="py-12 text-center text-muted-foreground text-xs font-semibold">
                Đang tải danh sách khuyến mãi...
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal thêm/sửa -->
    <Modal v-model="modalOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold flex items-center gap-2">
          <Tag class="w-5 h-5 text-caramel" />
          {{ editing ? 'Chỉnh sửa chương trình' : 'Thêm chương trình khuyến mãi' }}
        </h2>
      </template>

      <div class="space-y-4 text-left">
        <div>
          <label class="text-xs font-bold text-espresso uppercase tracking-wide">Tên chương trình <span class="text-red-500">*</span></label>
          <Input v-model="form.tenChuongTrinh" placeholder="VD: Giảm 20% Mừng Khai Trương, Giờ Vàng Cà Phê..." class="mt-1.5 bg-cream/40 border-cream-deep h-10 rounded-xl font-medium" />
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <div class="flex items-center justify-between mb-1">
              <label class="text-xs font-bold text-espresso uppercase tracking-wide">Mã Voucher</label>
              <button @click="generateRandomCode" type="button" class="text-[11px] font-bold text-caramel hover:text-brown flex items-center gap-1 cursor-pointer">
                <Sparkles class="w-3 h-3" /> Tạo mã tự động
              </button>
            </div>
            <Input v-model="form.maGiamGia" placeholder="VD: COFFEE2026 (Bỏ trống = Giảm tự động)" class="bg-cream/40 border-cream-deep h-10 rounded-xl font-mono font-bold uppercase" />
          </div>

          <div>
            <label class="text-xs font-bold text-espresso uppercase tracking-wide">Hình thức giảm</label>
            <select v-model="form.loaiGiamGia" class="mt-1 w-full h-10 px-3 rounded-xl bg-cream/40 border border-cream-deep text-xs font-bold text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/30 cursor-pointer">
              <option value="PhanTram">Phần trăm (%)</option>
              <option value="TienMat">Số tiền cố định (VNĐ)</option>
            </select>
          </div>
        </div>

        <!-- Preset buttons cho mức giảm -->
        <div class="space-y-1.5">
          <label class="text-[11px] font-bold text-muted-foreground uppercase tracking-widest">Chọn nhanh mức giảm</label>
          <div class="flex flex-wrap gap-2">
            <template v-if="form.loaiGiamGia === 'PhanTram'">
              <button v-for="rate in [10, 15, 20, 30, 50]" :key="rate" type="button" @click="form.giaTriGiam = rate" class="px-2.5 py-1 rounded-lg text-xs font-bold border border-cream-deep hover:bg-caramel hover:text-white transition-colors cursor-pointer" :class="form.giaTriGiam === rate ? 'bg-caramel text-white border-caramel' : 'bg-cream/60 text-espresso'">
                {{ rate }}%
              </button>
            </template>
            <template v-else>
              <button v-for="val in [10000, 20000, 30000, 50000]" :key="val" type="button" @click="form.giaTriGiam = val" class="px-2.5 py-1 rounded-lg text-xs font-bold border border-cream-deep hover:bg-caramel hover:text-white transition-colors cursor-pointer" :class="form.giaTriGiam === val ? 'bg-caramel text-white border-caramel' : 'bg-cream/60 text-espresso'">
                {{ (val/1000).toLocaleString() }}k
              </button>
            </template>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-bold text-espresso uppercase tracking-wide">Giá trị giảm <span class="text-red-500">*</span></label>
            <Input :model-value="form.giaTriGiam ?? ''" @update:model-value="(v: string|number) => numUpdate('giaTriGiam', v)" type="number" min="0" :placeholder="form.loaiGiamGia === 'PhanTram' ? 'VD: 20' : 'VD: 20000'" class="mt-1 bg-cream/40 border-cream-deep h-10 rounded-xl font-bold" />
          </div>

          <div v-if="form.loaiGiamGia === 'PhanTram'">
            <label class="text-xs font-bold text-espresso uppercase tracking-wide">Giảm tối đa (đ)</label>
            <Input :model-value="form.giamToiDa ?? ''" @update:model-value="(v: string|number) => numUpdate('giamToiDa', v)" type="number" min="0" placeholder="VD: 50000 (Tùy chọn)" class="mt-1 bg-cream/40 border-cream-deep h-10 rounded-xl font-bold" />
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-bold text-espresso uppercase tracking-wide">Đơn hàng tối thiểu (đ)</label>
            <Input :model-value="form.donToiThieu ?? ''" @update:model-value="(v: string|number) => numUpdate('donToiThieu', v)" type="number" min="0" placeholder="VD: 100000 (Tùy chọn)" class="mt-1 bg-cream/40 border-cream-deep h-10 rounded-xl font-bold" />
          </div>

          <div>
            <label class="text-xs font-bold text-espresso uppercase tracking-wide">Giới hạn số lượt dùng</label>
            <Input :model-value="form.soLuongGioiHan ?? ''" @update:model-value="(v: string|number) => numUpdate('soLuongGioiHan', v)" type="number" min="0" placeholder="VD: 100 (Bỏ trống = Vô hạn)" class="mt-1 bg-cream/40 border-cream-deep h-10 rounded-xl font-bold" />
          </div>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-bold text-espresso uppercase tracking-wide">Ngày bắt đầu</label>
            <input v-model="form.ngayBatDau" type="date" class="mt-1.5 w-full h-10 px-3 rounded-xl bg-cream/40 border border-cream-deep text-xs font-bold text-espresso" />
          </div>

          <div>
            <label class="text-xs font-bold text-espresso uppercase tracking-wide">Ngày kết thúc</label>
            <input v-model="form.ngayKetThuc" type="date" class="mt-1.5 w-full h-10 px-3 rounded-xl bg-cream/40 border border-cream-deep text-xs font-bold text-espresso" />
          </div>
        </div>

        <label class="flex items-center gap-2 text-xs font-bold text-espresso cursor-pointer pt-1">
          <input type="checkbox" v-model="form.trangThaiHoatDong" class="w-4 h-4 accent-caramel rounded" /> Cho phép áp dụng khuyến mãi ngay
        </label>

        <p v-if="formError" class="text-xs text-destructive font-bold bg-red-50 p-2.5 rounded-lg border border-red-200">{{ formError }}</p>
      </div>

      <template #footer>
        <Button variant="outline" @click="modalOpen=false" class="border-cream-deep rounded-xl text-espresso font-bold text-xs">Huỷ bỏ</Button>
        <Button @click="save" :disabled="saving" class="bg-caramel hover:bg-brown text-white rounded-xl border border-caramel/30 font-bold text-xs uppercase tracking-wider px-4">
          {{ saving ? 'Đang lưu...' : (editing ? 'Lưu cập nhật' : 'Tạo mới') }}
        </Button>
      </template>
    </Modal>

    <!-- Modal Confirm Xoá -->
    <Modal v-model="confirmOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold">Xoá khuyến mãi</h2>
      </template>
      <p class="text-xs text-espresso/80 font-medium">Bạn có chắc chắn muốn xoá chương trình <b>{{ delTarget?.tenChuongTrinh }}</b>?</p>
      <template #footer>
        <Button variant="outline" @click="confirmOpen=false" class="border-cream-deep rounded-xl text-espresso font-bold text-xs">Huỷ</Button>
        <Button @click="doRemove" :disabled="saving" class="bg-red-600 hover:bg-red-700 text-white rounded-xl font-bold text-xs uppercase tracking-wider">Xoá chương trình</Button>
      </template>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Plus, Pencil, Trash2, Tag, Copy, Sparkles, Search, Percent, Clock } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import { promotionsApi, type Promotion, type LoaiGiamGia } from '@/services/promotions'
import { useToast } from '@/stores/toast'

const toast = useToast()
const promotions = ref<Promotion[]>([])
const loading = ref(false)
const errorMsg = ref('')
const togglingId = ref<number | null>(null)

// Search & Filter State
const searchQuery = ref('')
const statusFilter = ref<'all' | 'active' | 'expired' | 'off'>('all')

const formatVND = (n: number) => (n || 0).toLocaleString('vi-VN') + 'đ'
const fmtD = (iso: string | null) => iso ? new Date(iso).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: '2-digit' }) : '∞'

async function load() {
  loading.value = true
  errorMsg.value = ''
  try { 
    promotions.value = await promotionsApi.list() 
  } catch (e) { 
    errorMsg.value = e instanceof Error ? e.message : 'Không tải được khuyến mãi.' 
  } finally { 
    loading.value = false 
  }
}
onMounted(load)

function hetHan(p: Promotion) { 
  return p.ngayKetThuc != null && new Date(p.ngayKetThuc) < new Date() 
}

function hetLuot(p: Promotion) { 
  return p.soLuongGioiHan != null && p.soLuongDaDung >= p.soLuongGioiHan 
}

function isCurrentlyActive(p: Promotion) {
  return p.trangThaiHoatDong && !hetHan(p) && !hetLuot(p)
}

function statusText(p: Promotion) {
  if (hetHan(p)) return 'Hết hạn'
  if (hetLuot(p)) return 'Hết lượt'
  if (!p.trangThaiHoatDong) return 'Đã tắt'
  return 'Đang chạy'
}

function badgeTextClass(p: Promotion) {
  const st = statusText(p)
  if (st === 'Đang chạy') return 'text-emerald-600 font-extrabold'
  if (st === 'Hết hạn' || st === 'Hết lượt') return 'text-amber-600 font-extrabold'
  return 'text-gray-400'
}

// Thống kê Dashboard KPI
const stats = computed(() => {
  const activeCount = promotions.value.filter(p => isCurrentlyActive(p)).length
  const expiredCount = promotions.value.filter(p => hetHan(p) || hetLuot(p)).length
  const disabledCount = promotions.value.filter(p => !p.trangThaiHoatDong).length
  const totalUsedCount = promotions.value.reduce((acc, p) => acc + (p.soLuongDaDung || 0), 0)

  // Ước tính số tiền tiết kiệm tri ân khách hàng
  const estimatedSavings = promotions.value.reduce((acc, p) => {
    const avgValue = p.loaiGiamGia === 'PhanTram' 
      ? (p.giamToiDa || 20000)
      : p.giaTriGiam
    return acc + ((p.soLuongDaDung || 0) * avgValue)
  }, 0)

  const inactiveCount = expiredCount + disabledCount

  return { activeCount, expiredCount, disabledCount, inactiveCount, totalUsedCount, estimatedSavings }
})

// Lọc danh sách khuyến mãi
const filteredPromotions = computed(() => {
  return promotions.value.filter(p => {
    const q = searchQuery.value.trim().toLowerCase()
    const matchSearch = !q || p.tenChuongTrinh.toLowerCase().includes(q) || (p.maGiamGia && p.maGiamGia.toLowerCase().includes(q))
    
    if (!matchSearch) return false

    const st = statusText(p)
    if (statusFilter.value === 'active') return st === 'Đang chạy'
    if (statusFilter.value === 'expired') return st === 'Hết hạn' || st === 'Hết lượt'
    if (statusFilter.value === 'off') return !p.trangThaiHoatDong
    return true
  })
})

// Copy Mã 1-click
const copyCode = (code: string | null) => {
  if (!code) return
  navigator.clipboard.writeText(code)
  toast.success(`Đã sao chép mã [${code}] vào bộ nhớ tạm!`, 'Sao chép mã')
}

// Bật/Tắt Trạng thái 1-Click (Toggle Switch)
const toggleStatus = async (p: Promotion) => {
  const isExp = hetHan(p)
  const isOut = hetLuot(p)

  // Nếu khuyến mãi đã hết hạn hoặc hết lượt -> Cảnh báo & Tự động mở Modal để Admin chỉnh sửa ngày/lượt
  if (isExp || isOut) {
    if (isExp) {
      toast.warning(`Chương trình "${p.tenChuongTrinh}" đã HẾT HẠN ngày ${fmtD(p.ngayKetThuc)}. Vui lòng gia hạn Ngày kết thúc trước khi bật!`, 'Yêu cầu gia hạn')
    } else {
      toast.warning(`Chương trình "${p.tenChuongTrinh}" đã HẾT LƯỢT dùng (${p.soLuongDaDung}/${p.soLuongGioiHan}). Vui lòng tăng giới hạn lượt dùng trước khi bật!`, 'Yêu cầu tăng lượt')
    }
    openEdit(p)
    return
  }

  togglingId.value = p.maKhuyenMai
  const newStatus = !p.trangThaiHoatDong
  try {
    const body = {
      maGiamGia: p.maGiamGia,
      tenChuongTrinh: p.tenChuongTrinh,
      loaiGiamGia: p.loaiGiamGia,
      giaTriGiam: p.giaTriGiam,
      giamToiDa: p.giamToiDa,
      donToiThieu: p.donToiThieu,
      soLuongGioiHan: p.soLuongGioiHan,
      ngayBatDau: p.ngayBatDau ? p.ngayBatDau.slice(0, 10) : null,
      ngayKetThuc: p.ngayKetThuc ? p.ngayKetThuc.slice(0, 10) : null,
      moTa: p.moTa,
      trangThaiHoatDong: newStatus
    }
    await promotionsApi.update(p.maKhuyenMai, body)
    p.trangThaiHoatDong = newStatus
    toast.success(`Đã ${newStatus ? 'bật' : 'tắt'} chương trình "${p.tenChuongTrinh}"`, 'Cập nhật trạng thái')
  } catch (e: any) {
    toast.error(e?.message || 'Không cập nhật được trạng thái khuyến mãi.')
  } finally {
    togglingId.value = null
  }
}

// Auto-generate code
const generateRandomCode = () => {
  const prefixes = ['CFG', 'CAFE', 'COFFEE', 'KHAITRUONG', 'HAPPY', 'SUMMER', 'VIP']
  const rates = [10, 15, 20, 25, 30, 50]
  const p = prefixes[Math.floor(Math.random() * prefixes.length)]
  const r = rates[Math.floor(Math.random() * rates.length)]
  form.value.maGiamGia = `${p}${r}`
}

// ── Thêm/sửa ──
interface FormState {
  maGiamGia: string; tenChuongTrinh: string; loaiGiamGia: LoaiGiamGia; giaTriGiam: number | null
  giamToiDa: number | null; donToiThieu: number | null; soLuongGioiHan: number | null
  ngayBatDau: string; ngayKetThuc: string; trangThaiHoatDong: boolean
}
const modalOpen = ref(false)
const editing = ref<Promotion | null>(null)
const saving = ref(false)
const formError = ref('')
const blank = (): FormState => ({ maGiamGia: '', tenChuongTrinh: '', loaiGiamGia: 'PhanTram', giaTriGiam: null, giamToiDa: null, donToiThieu: null, soLuongGioiHan: null, ngayBatDau: '', ngayKetThuc: '', trangThaiHoatDong: true })
const form = ref<FormState>(blank())

function numUpdate(key: 'giaTriGiam' | 'giamToiDa' | 'donToiThieu' | 'soLuongGioiHan', v: string | number) {
  form.value[key] = v === '' ? null : Number(v)
}

function openAdd() { 
  editing.value = null
  form.value = blank()
  formError.value = ''
  modalOpen.value = true 
}

function openEdit(p: Promotion) {
  editing.value = p
  form.value = {
    maGiamGia: p.maGiamGia || '', 
    tenChuongTrinh: p.tenChuongTrinh, 
    loaiGiamGia: p.loaiGiamGia,
    giaTriGiam: p.giaTriGiam, 
    giamToiDa: p.giamToiDa, 
    donToiThieu: p.donToiThieu, 
    soLuongGioiHan: p.soLuongGioiHan,
    ngayBatDau: p.ngayBatDau ? p.ngayBatDau.slice(0, 10) : '', 
    ngayKetThuc: p.ngayKetThuc ? p.ngayKetThuc.slice(0, 10) : '',
    trangThaiHoatDong: p.trangThaiHoatDong,
  }
  formError.value = ''
  modalOpen.value = true
}

async function save() {
  formError.value = ''
  if (!form.value.tenChuongTrinh.trim()) { 
    formError.value = 'Vui lòng nhập tên chương trình.'
    return 
  }
  if (!form.value.giaTriGiam || form.value.giaTriGiam <= 0) { 
    formError.value = 'Giá trị giảm phải lớn hơn 0.'
    return 
  }
  if (form.value.loaiGiamGia === 'PhanTram' && form.value.giaTriGiam > 100) { 
    formError.value = 'Phần trăm tối đa 100%.'
    return 
  }
  if (form.value.trangThaiHoatDong) {
    if (form.value.ngayKetThuc && new Date(form.value.ngayKetThuc) < new Date(new Date().setHours(0, 0, 0, 0))) {
      formError.value = 'Ngày kết thúc phải từ hôm nay trở đi để bật khuyến mãi này.'
      return
    }
    if (editing.value && form.value.soLuongGioiHan != null && editing.value.soLuongDaDung >= form.value.soLuongGioiHan) {
      formError.value = `Số lượt giới hạn (${form.value.soLuongGioiHan}) phải lớn hơn số lượt đã dùng (${editing.value.soLuongDaDung}) để bật khuyến mãi.`
      return
    }
  }
  saving.value = true
  try {
    const body = {
      maGiamGia: form.value.maGiamGia.trim() || null,
      tenChuongTrinh: form.value.tenChuongTrinh.trim(),
      loaiGiamGia: form.value.loaiGiamGia,
      giaTriGiam: form.value.giaTriGiam,
      giamToiDa: form.value.loaiGiamGia === 'PhanTram' ? (form.value.giamToiDa || null) : null,
      donToiThieu: form.value.donToiThieu || null,
      soLuongGioiHan: form.value.soLuongGioiHan || null,
      ngayBatDau: form.value.ngayBatDau || null,
      ngayKetThuc: form.value.ngayKetThuc || null,
      moTa: null,
      trangThaiHoatDong: form.value.trangThaiHoatDong,
    }
    if (editing.value) {
      await promotionsApi.update(editing.value.maKhuyenMai, body)
      toast.success('Đã cập nhật chương trình khuyến mãi thành công!', 'Thành công')
    } else {
      await promotionsApi.create(body)
      toast.success('Đã tạo chương trình khuyến mãi mới thành công!', 'Thành công')
    }
    modalOpen.value = false
    await load()
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Không lưu được khuyến mãi.'
  } finally { 
    saving.value = false 
  }
}

// ── Xoá ──
const confirmOpen = ref(false)
const delTarget = ref<Promotion | null>(null)
function removePromo(p: Promotion) { 
  delTarget.value = p
  confirmOpen.value = true 
}

async function doRemove() {
  saving.value = true
  try { 
    await promotionsApi.remove(delTarget.value!.maKhuyenMai)
    toast.success('Đã xoá khuyến mãi thành công!', 'Thành công')
    confirmOpen.value = false
    await load() 
  } catch (e) { 
    errorMsg.value = e instanceof Error ? e.message : 'Không xoá được.'
    confirmOpen.value = false 
  } finally { 
    saving.value = false 
  }
}
</script>
