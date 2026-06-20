<template>
  <div class="p-6 space-y-5">

    <!-- ── Header ── -->
    <div class="flex items-center justify-between gap-4">
      <div>
        <h1 class="font-display text-2xl text-espresso font-bold">Vai trò &amp; quyền</h1>
        <p class="text-sm text-muted-foreground mt-0.5">Cấu hình mỗi vị trí được vào những trang/chức năng nào</p>
      </div>
      <RouterLink to="/staff" class="text-sm font-semibold text-espresso hover:text-caramel inline-flex items-center gap-1.5">
        <ArrowLeft class="w-4 h-4" /> Về Quản lý tài khoản
      </RouterLink>
    </div>

    <p v-if="errorMsg" class="text-sm font-semibold text-red-600 bg-red-50 border border-red-200 rounded-xl px-4 py-3">{{ errorMsg }}</p>
    <p v-if="noticeMsg" class="text-sm font-semibold text-green-700 bg-green-50 border border-green-200 rounded-xl px-4 py-3">{{ noticeMsg }}</p>

    <div class="grid grid-cols-1 lg:grid-cols-[300px_1fr] gap-5">

      <!-- ── Danh sách vai trò ── -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Vị trí ({{ roles.length }})</p>
          <button @click="newRole" class="inline-flex items-center gap-1 text-xs font-bold text-caramel hover:text-espresso">
            <Plus class="w-3.5 h-3.5" /> Thêm
          </button>
        </div>
        <button v-for="r in roles" :key="r.maVaiTro" @click="selectRole(r)"
          :class="['w-full text-left rounded-xl border px-3 py-2.5 transition',
            sel?.maVaiTro === r.maVaiTro && mode === 'edit'
              ? 'border-caramel bg-caramel/5'
              : 'border-cream-deep bg-card hover:bg-cream/50']">
          <div class="flex items-center justify-between">
            <span class="font-semibold text-espresso">{{ r.tenVaiTro }}</span>
            <span v-if="r.laVaiTroHeThong" class="text-[9px] uppercase font-bold text-caramel bg-caramel/10 px-1.5 py-0.5 rounded">hệ thống</span>
          </div>
          <p class="text-[11px] text-muted-foreground mt-0.5">{{ r.soTaiKhoan }} tài khoản · {{ r.quyens.length }} quyền</p>
        </button>
        <button @click="newRole"
          :class="['w-full rounded-xl border border-dashed py-2.5 text-sm font-semibold transition',
            mode === 'new' ? 'border-caramel text-caramel bg-caramel/5' : 'border-cream-deep text-muted-foreground hover:text-espresso']">
          + Vai trò mới
        </button>
      </div>

      <!-- ── Trình chỉnh sửa ── -->
      <div class="rounded-2xl border border-cream-deep bg-card p-5">
        <template v-if="mode">

          <!-- Tên + Mô tả -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 mb-3">
            <div>
              <div class="flex items-center justify-between mb-1">
                <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">
                  Tên vị trí <span class="text-red-500">*</span>
                </label>
                <span class="text-[10px] text-muted-foreground">{{ form.tenVaiTro.length }}/50</span>
              </div>
              <Input v-model="form.tenVaiTro" :disabled="isNameLock" placeholder="VD: Thu ngân" maxlength="50"
                :class="['bg-cream/40 border-cream-deep rounded-lg h-10', fieldErrors.tenVaiTro ? 'border-red-400' : '']"
                @input="fieldErrors.tenVaiTro = ''" />
              <p v-if="fieldErrors.tenVaiTro" class="text-xs text-red-500 mt-1 flex items-center gap-1">
                <AlertCircle class="w-3 h-3 shrink-0" /> {{ fieldErrors.tenVaiTro }}
              </p>
            </div>
            <div>
              <div class="flex items-center justify-between mb-1">
                <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Mô tả</label>
                <span class="text-[10px] text-muted-foreground">{{ form.moTa.length }}/200</span>
              </div>
              <Input v-model="form.moTa" :disabled="isNameLock" placeholder="Mô tả ngắn" maxlength="200"
                class="bg-cream/40 border-cream-deep rounded-lg h-10" />
            </div>
          </div>

          <!-- Notice bar -->
          <p v-if="isFullLock" class="text-sm text-muted-foreground bg-cream/50 border border-cream-deep rounded-lg px-3 py-2 mb-3">
            Vai trò <b>Quản lý</b> luôn có <b>toàn quyền</b> và không thể chỉnh để tránh tự khoá hệ thống.
          </p>
          <p v-else-if="isNameLock" class="text-sm text-muted-foreground bg-cream/50 border border-cream-deep rounded-lg px-3 py-2 mb-3">
            Đây là vai trò hệ thống — tên không thể đổi, nhưng bạn có thể điều chỉnh quyền truy cập.
          </p>

          <!-- Quyền header -->
          <div class="flex items-center justify-between mb-2">
            <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Quyền truy cập</p>
            <span class="text-[11px] font-semibold" :class="selectedCount === 0 && !isFullLock ? 'text-amber-500' : 'text-muted-foreground'">
              {{ isFullLock ? permissions.length : selectedCount }} / {{ permissions.length }} quyền
            </span>
          </div>

          <!-- Warning no permissions -->
          <p v-if="fieldErrors.quyens" class="text-xs text-amber-700 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2 mb-2 flex items-center gap-1.5">
            <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ fieldErrors.quyens }}
          </p>

          <!-- Quyền list -->
          <div class="space-y-3 max-h-[360px] overflow-y-auto pr-1">
            <div v-for="g in groups" :key="g.nhom" class="rounded-xl border border-cream-deep overflow-hidden">
              <div class="flex items-center justify-between px-3 py-2 bg-cream/40">
                <div class="flex items-center gap-2">
                  <span class="text-sm font-bold text-espresso">{{ nhomLabel[g.nhom] || g.nhom }}</span>
                  <span class="text-[10px] text-muted-foreground bg-cream-deep px-1.5 rounded-full">
                    {{ isFullLock ? g.items.length : g.items.filter(p => has(p.maCode)).length }}/{{ g.items.length }}
                  </span>
                </div>
                <button v-if="!isFullLock" @click="toggleGroup(g); fieldErrors.quyens = ''"
                  class="text-[11px] font-semibold text-caramel hover:text-espresso">
                  {{ g.items.every(p => has(p.maCode)) ? 'Bỏ chọn' : 'Chọn hết' }}
                </button>
              </div>
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-1 p-2">
                <label v-for="p in g.items" :key="p.maCode"
                  :class="['flex items-center gap-2 rounded-lg px-2 py-1.5 text-sm', isFullLock ? 'opacity-60' : 'cursor-pointer hover:bg-cream/50']">
                  <input type="checkbox" :checked="isFullLock || has(p.maCode)" :disabled="isFullLock"
                    @change="togglePerm(p.maCode); fieldErrors.quyens = ''" class="w-4 h-4 accent-espresso" />
                  <span class="text-espresso">{{ p.tenQuyen }}</span>
                </label>
              </div>
            </div>
          </div>

          <!-- Bottom actions -->
          <div class="flex items-center justify-between mt-5 pt-4 border-t border-cream-deep">
            <div>
              <!-- Xoá: vai trò tự tạo (không hệ thống) -->
              <button v-if="mode === 'edit' && sel && !sel.laVaiTroHeThong"
                @click="openConfirmDelete"
                class="inline-flex items-center gap-1.5 text-sm font-semibold text-red-500 hover:text-red-700 hover:bg-red-50 px-3 py-1.5 rounded-lg transition-colors">
                <Trash2 class="w-4 h-4" /> Xoá vai trò
              </button>
              <!-- Xoá: vai trò hệ thống (disable) -->
              <span v-else-if="mode === 'edit' && sel && sel.laVaiTroHeThong && !isFullLock"
                class="inline-flex items-center gap-1.5 text-sm font-semibold text-muted-foreground/40 px-3 py-1.5 cursor-not-allowed select-none"
                title="Không thể xoá vai trò hệ thống">
                <Trash2 class="w-4 h-4" /> Xoá vai trò
              </span>
            </div>
            <div class="flex items-center gap-2">
              <button @click="cancel"
                class="text-sm font-semibold text-muted-foreground hover:text-espresso px-3 py-1.5 rounded-lg hover:bg-cream transition-colors">
                Huỷ
              </button>
              <Button v-if="!isFullLock" @click="save" :disabled="saving" class="bg-espresso text-cream rounded-xl h-10 px-6">
                {{ saving ? 'Đang lưu...' : (mode === 'new' ? 'Tạo vai trò' : 'Lưu thay đổi') }}
              </Button>
            </div>
          </div>
        </template>

        <!-- Empty state -->
        <div v-else class="flex flex-col items-center justify-center py-16 gap-3 text-center">
          <ShieldCheck class="w-10 h-10 text-cream-deep" />
          <p class="text-sm text-muted-foreground">Chọn một vai trò bên trái để xem/sửa quyền, hoặc tạo vai trò mới.</p>
        </div>
      </div>
    </div>

    <!-- ── Modal xác nhận xoá ── -->
    <Modal v-model="confirmDeleteOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold">Xoá vai trò</h2>
      </template>
      <div class="space-y-3">
        <p class="text-sm text-espresso/80">
          Bạn chắc chắn muốn xoá vai trò <b>"{{ sel?.tenVaiTro }}"</b>?
        </p>
        <p v-if="sel && sel.soTaiKhoan > 0"
          class="text-sm text-amber-700 bg-amber-50 border border-amber-200 rounded-xl px-3 py-2.5 flex items-start gap-2">
          <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
          Vai trò này đang có <b>{{ sel.soTaiKhoan }} tài khoản</b> sử dụng.
          Hãy chuyển tất cả tài khoản sang vai trò khác trước khi xoá.
        </p>
        <p v-else class="text-sm text-muted-foreground">Hành động này không thể hoàn tác.</p>
        <p v-if="deleteError" class="text-sm text-red-600 bg-red-50 border border-red-200 rounded-xl px-3 py-2">{{ deleteError }}</p>
      </div>
      <template #footer>
        <Button variant="outline" @click="confirmDeleteOpen = false" class="border-cream-deep rounded-xl text-espresso">Huỷ</Button>
        <Button @click="doDelete" :disabled="saving || (sel?.soTaiKhoan ?? 0) > 0"
          class="bg-red-600 text-cream rounded-xl disabled:opacity-40">
          {{ saving ? 'Đang xoá...' : 'Xoá vai trò' }}
        </Button>
      </template>
    </Modal>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { Plus, Trash2, ArrowLeft, AlertCircle, ShieldCheck } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import { rolesApi, type RoleDetail, type PermissionItem } from '@/services/accounts'

const roles = ref<RoleDetail[]>([])
const permissions = ref<PermissionItem[]>([])
const errorMsg = ref('')
const noticeMsg = ref('')
const saving = ref(false)

const nhomLabel: Record<string, string> = {
  TaiKhoan: 'Tài khoản', SanPham: 'Sản phẩm & Menu', Kho: 'Kho', DonHang: 'Đơn hàng',
  Bep: 'Bếp', ThanhToan: 'Thanh toán', KhachHang: 'Khách hàng', NhanSu: 'Nhân sự',
  BaoCao: 'Báo cáo', CaiDat: 'Cài đặt', Ban: 'Bàn & QR',
}

const groups = computed(() => {
  const m = new Map<string, PermissionItem[]>()
  for (const p of permissions.value) { const a = m.get(p.nhom) ?? []; a.push(p); m.set(p.nhom, a) }
  return Array.from(m, ([nhom, items]) => ({ nhom, items }))
})

async function loadAll() {
  try {
    const [r, p] = await Promise.all([rolesApi.list(), rolesApi.permissions()])
    roles.value = r
    permissions.value = p
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không tải được dữ liệu.'
  }
}
onMounted(loadAll)

// ── Editor ──
const mode = ref<'edit' | 'new' | null>(null)
const sel = ref<RoleDetail | null>(null)
const form = ref<{ tenVaiTro: string; moTa: string; quyens: Set<string> }>({ tenVaiTro: '', moTa: '', quyens: new Set() })
const fieldErrors = ref({ tenVaiTro: '', quyens: '' })

const isFullLock = computed(() => mode.value === 'edit' && sel.value?.maVaiTro === 1)
const isNameLock = computed(() => mode.value === 'edit' && (sel.value?.laVaiTroHeThong ?? false))
const selectedCount = computed(() => form.value.quyens.size)

function selectRole(r: RoleDetail) {
  mode.value = 'edit'; sel.value = r
  form.value = { tenVaiTro: r.tenVaiTro, moTa: r.moTa || '', quyens: new Set(r.quyens) }
  fieldErrors.value = { tenVaiTro: '', quyens: '' }
  noticeMsg.value = ''; errorMsg.value = ''
}
function newRole() {
  mode.value = 'new'; sel.value = null
  form.value = { tenVaiTro: '', moTa: '', quyens: new Set() }
  fieldErrors.value = { tenVaiTro: '', quyens: '' }
  noticeMsg.value = ''; errorMsg.value = ''
}
function cancel() {
  mode.value = null; sel.value = null
  fieldErrors.value = { tenVaiTro: '', quyens: '' }
  errorMsg.value = ''; noticeMsg.value = ''
}

const has = (code: string) => form.value.quyens.has(code)
function togglePerm(code: string) {
  if (form.value.quyens.has(code)) form.value.quyens.delete(code)
  else form.value.quyens.add(code)
  form.value = { ...form.value, quyens: new Set(form.value.quyens) }
}
function toggleGroup(g: { items: PermissionItem[] }) {
  const all = g.items.every(p => form.value.quyens.has(p.maCode))
  for (const p of g.items) { if (all) form.value.quyens.delete(p.maCode); else form.value.quyens.add(p.maCode) }
  form.value = { ...form.value, quyens: new Set(form.value.quyens) }
}

function validate(): boolean {
  fieldErrors.value = { tenVaiTro: '', quyens: '' }
  const ten = form.value.tenVaiTro.trim()
  if (!ten) { fieldErrors.value.tenVaiTro = 'Vui lòng nhập tên vai trò.'; return false }
  if (ten.length < 2) { fieldErrors.value.tenVaiTro = 'Tên vai trò tối thiểu 2 ký tự.'; return false }
  if (form.value.quyens.size === 0 && !isNameLock.value)
    fieldErrors.value.quyens = 'Chưa chọn quyền nào — vai trò này sẽ không truy cập được trang nào.'
  return true
}

async function save() {
  errorMsg.value = ''
  if (!validate()) return
  saving.value = true
  try {
    const body = { tenVaiTro: form.value.tenVaiTro.trim(), moTa: form.value.moTa.trim() || null, quyens: Array.from(form.value.quyens) }
    if (mode.value === 'new') await rolesApi.create(body)
    else await rolesApi.update(sel.value!.maVaiTro, body)
    noticeMsg.value = mode.value === 'new' ? 'Đã tạo vai trò mới.' : 'Đã lưu thay đổi.'
    mode.value = null; sel.value = null
    await loadAll()
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không lưu được vai trò.'
  } finally {
    saving.value = false
  }
}

// ── Xoá ──
const confirmDeleteOpen = ref(false)
const deleteError = ref('')

function openConfirmDelete() {
  deleteError.value = ''
  confirmDeleteOpen.value = true
}

async function doDelete() {
  if (!sel.value) return
  deleteError.value = ''
  saving.value = true
  try {
    await rolesApi.remove(sel.value.maVaiTro)
    const tenDaXoa = sel.value.tenVaiTro
    confirmDeleteOpen.value = false
    mode.value = null; sel.value = null
    noticeMsg.value = `Đã xoá vai trò "${tenDaXoa}".`
    await loadAll()
  } catch (e) {
    deleteError.value = e instanceof Error ? e.message : 'Không xoá được vai trò.'
  } finally {
    saving.value = false
  }
}
</script>
