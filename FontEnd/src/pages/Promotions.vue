<template>
  <div class="p-6 space-y-5">
    <div class="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
      <div>
        <h1 class="font-display text-2xl text-espresso font-bold">Khuyến mãi &amp; Voucher</h1>
        <p class="text-sm text-muted-foreground mt-0.5">Tạo chương trình giảm giá, mã voucher áp dụng khi thanh toán</p>
      </div>
      <Button @click="openAdd" class="bg-caramel text-cream rounded-lg border border-caramel/30 shadow-card">
        <Plus class="w-4 h-4 mr-1.5" /> Thêm khuyến mãi
      </Button>
    </div>

    <p v-if="errorMsg" class="text-sm font-semibold text-red-600 bg-red-50 border border-red-200 rounded-lg px-4 py-3">{{ errorMsg }}</p>

    <div class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b-2 border-cream-deep text-left text-xs uppercase tracking-wider text-muted-foreground">
              <th class="px-5 py-3 font-semibold">Chương trình</th>
              <th class="px-5 py-3 font-semibold">Giảm</th>
              <th class="px-5 py-3 font-semibold">Điều kiện</th>
              <th class="px-5 py-3 font-semibold">Hạn / Lượt</th>
              <th class="px-5 py-3 font-semibold">Trạng thái</th>
              <th class="px-5 py-3 font-semibold text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-cream-deep/60">
            <tr v-for="p in promotions" :key="p.maKhuyenMai" class="hover:bg-cream/40">
              <td class="px-5 py-4">
                <div class="font-semibold text-espresso">{{ p.tenChuongTrinh }}</div>
                <div v-if="p.maGiamGia" class="text-xs mt-0.5">
                  <span class="font-mono font-bold text-caramel bg-caramel/10 px-1.5 py-0.5 rounded">{{ p.maGiamGia }}</span>
                </div>
                <div v-else class="text-[11px] text-muted-foreground mt-0.5">Chương trình (không cần mã)</div>
              </td>
              <td class="px-5 py-4 font-semibold text-espresso">
                {{ p.loaiGiamGia === 'PhanTram' ? p.giaTriGiam + '%' : formatVND(p.giaTriGiam) }}
                <div v-if="p.loaiGiamGia==='PhanTram' && p.giamToiDa" class="text-[11px] text-muted-foreground">tối đa {{ formatVND(p.giamToiDa) }}</div>
              </td>
              <td class="px-5 py-4 text-muted-foreground">
                <span v-if="p.donToiThieu">Đơn ≥ {{ formatVND(p.donToiThieu) }}</span>
                <span v-else>—</span>
              </td>
              <td class="px-5 py-4 text-muted-foreground text-xs">
                <div>{{ p.ngayBatDau || p.ngayKetThuc ? (fmtD(p.ngayBatDau) + ' → ' + fmtD(p.ngayKetThuc)) : 'Không giới hạn hạn' }}</div>
                <div>{{ p.soLuongGioiHan ? (p.soLuongDaDung + '/' + p.soLuongGioiHan + ' lượt') : (p.soLuongDaDung + ' lượt đã dùng') }}</div>
              </td>
              <td class="px-5 py-4">
                <span :class="['inline-flex items-center gap-1.5 px-3 py-1 rounded-lg text-xs font-medium border', badge(p)]">
                  <span class="w-1.5 h-1.5 rounded-full" :class="dot(p)"></span>{{ statusText(p) }}
                </span>
              </td>
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-1.5">
                  <button @click="openEdit(p)" title="Sửa" class="w-8 h-8 rounded-lg border border-cream-deep flex items-center justify-center text-espresso hover:bg-cream-deep/50"><Pencil class="w-3.5 h-3.5" /></button>
                  <button @click="removePromo(p)" title="Xoá" class="w-8 h-8 rounded-lg border border-destructive/30 text-destructive flex items-center justify-center hover:bg-destructive/10"><Trash2 class="w-3.5 h-3.5" /></button>
                </div>
              </td>
            </tr>
            <tr v-if="!loading && promotions.length === 0"><td colspan="6" class="py-12 text-center text-muted-foreground">Chưa có khuyến mãi nào. Bấm "Thêm khuyến mãi".</td></tr>
            <tr v-if="loading"><td colspan="6" class="py-12 text-center text-muted-foreground">Đang tải...</td></tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal thêm/sửa -->
    <Modal v-model="modalOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold">{{ editing ? 'Sửa khuyến mãi' : 'Thêm khuyến mãi' }}</h2>
      </template>
      <div class="space-y-4">
        <div>
          <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Tên chương trình *</label>
          <Input v-model="form.tenChuongTrinh" placeholder="VD: Mừng khai trương" class="mt-1.5 bg-cream/40 border-cream-deep h-10 rounded-lg" />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Mã voucher</label>
            <Input v-model="form.maGiamGia" placeholder="VD: KHAITRUONG (để trống = chương trình)" class="mt-1.5 bg-cream/40 border-cream-deep h-10 rounded-lg" />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Loại giảm</label>
            <select v-model="form.loaiGiamGia" class="mt-1.5 w-full h-10 px-3 rounded-lg bg-cream/40 border border-cream-deep text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/30">
              <option value="PhanTram">Phần trăm (%)</option>
              <option value="TienMat">Số tiền (đ)</option>
            </select>
          </div>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Giá trị giảm *</label>
            <Input :model-value="form.giaTriGiam ?? ''" @update:model-value="(v: string|number) => numUpdate('giaTriGiam', v)" type="number" min="0" :placeholder="form.loaiGiamGia==='PhanTram' ? 'VD: 20' : 'VD: 15000'" class="mt-1.5 bg-cream/40 border-cream-deep h-10 rounded-lg" />
          </div>
          <div v-if="form.loaiGiamGia==='PhanTram'">
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Giảm tối đa (đ)</label>
            <Input :model-value="form.giamToiDa ?? ''" @update:model-value="(v: string|number) => numUpdate('giamToiDa', v)" type="number" min="0" placeholder="(tuỳ chọn)" class="mt-1.5 bg-cream/40 border-cream-deep h-10 rounded-lg" />
          </div>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Đơn tối thiểu (đ)</label>
            <Input :model-value="form.donToiThieu ?? ''" @update:model-value="(v: string|number) => numUpdate('donToiThieu', v)" type="number" min="0" placeholder="(tuỳ chọn)" class="mt-1.5 bg-cream/40 border-cream-deep h-10 rounded-lg" />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Giới hạn lượt</label>
            <Input :model-value="form.soLuongGioiHan ?? ''" @update:model-value="(v: string|number) => numUpdate('soLuongGioiHan', v)" type="number" min="0" placeholder="(tuỳ chọn)" class="mt-1.5 bg-cream/40 border-cream-deep h-10 rounded-lg" />
          </div>
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Bắt đầu</label>
            <input v-model="form.ngayBatDau" type="date" class="mt-1.5 w-full h-10 px-3 rounded-lg bg-cream/40 border border-cream-deep text-sm text-espresso" />
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Kết thúc</label>
            <input v-model="form.ngayKetThuc" type="date" class="mt-1.5 w-full h-10 px-3 rounded-lg bg-cream/40 border border-cream-deep text-sm text-espresso" />
          </div>
        </div>
        <label class="flex items-center gap-2 text-sm font-semibold text-espresso cursor-pointer">
          <input type="checkbox" v-model="form.trangThaiHoatDong" class="w-4 h-4 accent-espresso" /> Đang áp dụng
        </label>
        <p v-if="formError" class="text-xs text-destructive font-medium">{{ formError }}</p>
      </div>
      <template #footer>
        <Button variant="outline" @click="modalOpen=false" class="border-cream-deep rounded-lg text-espresso">Huỷ</Button>
        <Button @click="save" :disabled="saving" class="bg-caramel text-cream rounded-lg border border-caramel/30">{{ saving ? 'Đang lưu...' : (editing ? 'Lưu' : 'Tạo') }}</Button>
      </template>
    </Modal>

    <!-- Confirm xoá -->
    <Modal v-model="confirmOpen">
      <template #header><h2 class="font-display text-xl text-espresso font-bold">Xoá khuyến mãi</h2></template>
      <p class="text-sm text-espresso/80">Xoá chương trình <b>{{ delTarget?.tenChuongTrinh }}</b>?</p>
      <template #footer>
        <Button variant="outline" @click="confirmOpen=false" class="border-cream-deep rounded-lg text-espresso">Huỷ</Button>
        <Button @click="doRemove" :disabled="saving" class="bg-red-600 text-cream rounded-lg">Xoá</Button>
      </template>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Plus, Pencil, Trash2 } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import { promotionsApi, type Promotion, type LoaiGiamGia } from '@/services/promotions'

const promotions = ref<Promotion[]>([])
const loading = ref(false)
const errorMsg = ref('')
const formatVND = (n: number) => (n || 0).toLocaleString('vi-VN') + 'đ'
const fmtD = (iso: string | null) => iso ? new Date(iso).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: '2-digit' }) : '∞'

async function load() {
  loading.value = true; errorMsg.value = ''
  try { promotions.value = await promotionsApi.list() }
  catch (e) { errorMsg.value = e instanceof Error ? e.message : 'Không tải được khuyến mãi.' }
  finally { loading.value = false }
}
onMounted(load)

function hetHan(p: Promotion) { return p.ngayKetThuc != null && new Date(p.ngayKetThuc) < new Date() }
function hetLuot(p: Promotion) { return p.soLuongGioiHan != null && p.soLuongDaDung >= p.soLuongGioiHan }
function statusText(p: Promotion) {
  if (!p.trangThaiHoatDong) return 'Tắt'
  if (hetHan(p)) return 'Hết hạn'
  if (hetLuot(p)) return 'Hết lượt'
  return 'Đang áp dụng'
}
function badge(p: Promotion) {
  return statusText(p) === 'Đang áp dụng' ? 'bg-success/15 text-success border-success/30' : 'bg-muted text-muted-foreground border-cream-deep'
}
function dot(p: Promotion) { return statusText(p) === 'Đang áp dụng' ? 'bg-success' : 'bg-muted-foreground' }

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
function openAdd() { editing.value = null; form.value = blank(); formError.value = ''; modalOpen.value = true }
function openEdit(p: Promotion) {
  editing.value = p
  form.value = {
    maGiamGia: p.maGiamGia || '', tenChuongTrinh: p.tenChuongTrinh, loaiGiamGia: p.loaiGiamGia,
    giaTriGiam: p.giaTriGiam, giamToiDa: p.giamToiDa, donToiThieu: p.donToiThieu, soLuongGioiHan: p.soLuongGioiHan,
    ngayBatDau: p.ngayBatDau ? p.ngayBatDau.slice(0, 10) : '', ngayKetThuc: p.ngayKetThuc ? p.ngayKetThuc.slice(0, 10) : '',
    trangThaiHoatDong: p.trangThaiHoatDong,
  }
  formError.value = ''; modalOpen.value = true
}
async function save() {
  formError.value = ''
  if (!form.value.tenChuongTrinh.trim()) { formError.value = 'Vui lòng nhập tên chương trình.'; return }
  if (!form.value.giaTriGiam || form.value.giaTriGiam <= 0) { formError.value = 'Giá trị giảm phải lớn hơn 0.'; return }
  if (form.value.loaiGiamGia === 'PhanTram' && form.value.giaTriGiam > 100) { formError.value = 'Phần trăm tối đa 100%.'; return }
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
    if (editing.value) await promotionsApi.update(editing.value.maKhuyenMai, body)
    else await promotionsApi.create(body)
    modalOpen.value = false
    await load()
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Không lưu được khuyến mãi.'
  } finally { saving.value = false }
}

// ── Xoá ──
const confirmOpen = ref(false)
const delTarget = ref<Promotion | null>(null)
function removePromo(p: Promotion) { delTarget.value = p; confirmOpen.value = true }
async function doRemove() {
  saving.value = true
  try { await promotionsApi.remove(delTarget.value!.maKhuyenMai); confirmOpen.value = false; await load() }
  catch (e) { errorMsg.value = e instanceof Error ? e.message : 'Không xoá được.'; confirmOpen.value = false }
  finally { saving.value = false }
}
</script>
