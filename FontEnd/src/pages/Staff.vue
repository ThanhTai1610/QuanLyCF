<template>
  <div class="space-y-5 p-6">

    <!-- ===== Header + Stats ===== -->
    <div class="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
      <div>
        <h1 class="font-display text-2xl text-espresso font-bold">Quản lý tài khoản</h1>
        <p class="text-sm text-muted-foreground mt-0.5">Quản lý tài khoản đăng nhập, vai trò và quyền truy cập hệ thống</p>
      </div>
      <div class="flex items-center gap-3">
        <div class="px-4 py-2 rounded-lg bg-card border border-cream-deep shadow-card text-center">
          <div class="font-display text-xl text-espresso font-bold">{{ accounts.length }}</div>
          <div class="text-[10px] uppercase tracking-wider text-muted-foreground font-semibold">Tài khoản</div>
        </div>
        <div class="px-4 py-2 rounded-lg bg-card border border-cream-deep shadow-card text-center">
          <div class="font-display text-xl text-success font-bold">{{ activeCount }}</div>
          <div class="text-[10px] uppercase tracking-wider text-muted-foreground font-semibold">Hoạt động</div>
        </div>
        <div class="px-4 py-2 rounded-lg bg-card border border-cream-deep shadow-card text-center">
          <div class="font-display text-xl text-destructive font-bold">{{ lockedCount }}</div>
          <div class="text-[10px] uppercase tracking-wider text-muted-foreground font-semibold">Đã khoá</div>
        </div>
      </div>
    </div>

    <!-- ===== Filter + Add ===== -->
    <div class="flex flex-col sm:flex-row items-center justify-between gap-4">
      <div class="flex flex-col sm:flex-row gap-3 w-full sm:w-auto">
        <div class="relative w-full sm:max-w-[220px]">
          <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Tìm tên, email..."
            v-model="search"
            class="pl-9 bg-card border-cream-deep h-10 rounded-lg"
          />
        </div>
        <div class="flex gap-2 overflow-x-auto scrollbar-none">
          <button
            v-for="r in roleFilters"
            :key="r"
            @click="filter = r"
            :class="[
              'px-4 py-2 rounded-lg text-sm font-semibold whitespace-nowrap border shadow-card',
              filter === r
                ? 'bg-espresso text-cream border-espresso'
                : 'bg-card text-espresso border-cream-deep'
            ]"
          >
            {{ r === "all" ? "Tất cả" : roleLabel[r as Account['role']] }}
          </button>
        </div>
      </div>
      <Button @click="openAdd" class="bg-caramel text-cream rounded-lg border border-caramel/30 shadow-card w-full sm:w-auto">
        <Plus class="w-4 h-4 mr-1.5" /> Thêm tài khoản
      </Button>
    </div>

    <!-- ===== Accounts table ===== -->
    <div class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-sm">
          <thead>
            <tr class="border-b-2 border-cream-deep text-left text-xs uppercase tracking-wider text-muted-foreground">
              <th class="px-5 py-3 font-semibold">Tài khoản</th>
              <th class="px-5 py-3 font-semibold">Vai trò &amp; quyền</th>
              <th class="px-5 py-3 font-semibold">Trạng thái</th>
              <th class="px-5 py-3 font-semibold">Đăng nhập cuối</th>
              <th class="px-5 py-3 font-semibold text-right">Thao tác</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-cream-deep/60">
            <tr v-for="a in paginatedItems" :key="a.id" class="hover:bg-cream/40 transition-colors">
              <!-- Account -->
              <td class="px-5 py-4">
                <div class="flex items-center gap-3">
                  <div :class="['w-10 h-10 rounded-lg text-cream font-semibold flex items-center justify-center border border-cream-deep shadow-md shrink-0', a.color]">
                    {{ a.initials }}
                  </div>
                  <div class="min-w-0">
                    <div class="font-semibold text-espresso leading-tight truncate">{{ a.name }}</div>
                    <div class="text-xs text-muted-foreground flex items-center gap-1 truncate">
                      <Mail class="w-3 h-3 shrink-0" /> {{ a.email }}
                    </div>
                  </div>
                </div>
              </td>
              <!-- Role -->
              <td class="px-5 py-4">
                <span :class="['inline-block px-3 py-1 rounded-lg text-xs font-medium border', roleBadge[a.role]]">
                  {{ roleLabel[a.role] }}
                </span>
                <div class="text-xs text-muted-foreground mt-1">{{ rolePermission[a.role] }}</div>
              </td>
              <!-- Status -->
              <td class="px-5 py-4">
                <span :class="[
                  'inline-flex items-center gap-1.5 px-3 py-1 rounded-lg text-xs font-medium border',
                  a.status === 'active'
                    ? 'bg-success/15 text-success border-success/30'
                    : 'bg-destructive/10 text-destructive border-destructive/30'
                ]">
                  <span class="w-1.5 h-1.5 rounded-full" :class="a.status === 'active' ? 'bg-success' : 'bg-destructive'"></span>
                  {{ a.status === "active" ? "Hoạt động" : "Đã khoá" }}
                </span>
              </td>
              <!-- Last login -->
              <td class="px-5 py-4 text-muted-foreground">
                <div class="flex items-center gap-1.5">
                  <Clock class="w-3.5 h-3.5" /> {{ a.lastLogin || "Chưa đăng nhập" }}
                </div>
              </td>
              <!-- Actions -->
              <td class="px-5 py-4">
                <div class="flex items-center justify-end gap-1.5">
                  <button @click="openEdit(a)" title="Chỉnh sửa" class="w-8 h-8 rounded-lg border border-cream-deep flex items-center justify-center text-espresso hover:bg-cream-deep/50 transition-colors">
                    <Pencil class="w-3.5 h-3.5" />
                  </button>
                  <button @click="resetPassword(a)" title="Đặt lại mật khẩu" class="w-8 h-8 rounded-lg border border-cream-deep flex items-center justify-center text-espresso hover:bg-cream-deep/50 transition-colors">
                    <KeyRound class="w-3.5 h-3.5" />
                  </button>
                  <button
                    @click="toggleLock(a)"
                    :title="a.status === 'active' ? 'Khoá tài khoản' : 'Mở khoá'"
                    :class="['w-8 h-8 rounded-lg border flex items-center justify-center transition-colors',
                      a.status === 'active'
                        ? 'border-cream-deep text-espresso hover:bg-cream-deep/50'
                        : 'border-success/30 text-success hover:bg-success/10']"
                  >
                    <Lock v-if="a.status === 'active'" class="w-3.5 h-3.5" />
                    <Unlock v-else class="w-3.5 h-3.5" />
                  </button>
                  <button @click="removeAccount(a)" title="Xoá tài khoản" class="w-8 h-8 rounded-lg border border-destructive/30 text-destructive flex items-center justify-center hover:bg-destructive/10 transition-colors">
                    <Trash2 class="w-3.5 h-3.5" />
                  </button>
                </div>
              </td>
            </tr>

            <!-- Empty state -->
            <tr v-if="paginatedItems.length === 0">
              <td colspan="5" class="py-12 text-center text-muted-foreground">
                Không tìm thấy tài khoản nào phù hợp.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- ===== Pagination ===== -->
    <div v-if="filteredItems.length > 0" class="flex items-center justify-between py-2">
      <div class="text-sm text-muted-foreground">
        Hiển thị <span class="font-medium text-espresso">{{ (currentPage - 1) * itemsPerPage + 1 }}</span> - <span class="font-medium text-espresso">{{ Math.min(currentPage * itemsPerPage, filteredItems.length) }}</span> / <span class="font-medium text-espresso">{{ filteredItems.length }}</span> tài khoản
      </div>
      <div class="flex items-center gap-2">
        <Button variant="outline" size="icon" @click="currentPage--" :disabled="currentPage === 1"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50">
          <ChevronLeft class="w-4 h-4" />
        </Button>
        <span class="text-sm font-semibold text-espresso px-3">Trang {{ currentPage }} / {{ totalPages }}</span>
        <Button variant="outline" size="icon" @click="currentPage++" :disabled="currentPage === totalPages"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50">
          <ChevronRight class="w-4 h-4" />
        </Button>
      </div>
    </div>

    <!-- ===== Add / Edit Modal ===== -->
    <Modal v-model="modalOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold">
          {{ editingId ? "Chỉnh sửa tài khoản" : "Thêm tài khoản mới" }}
        </h2>
        <p class="text-sm text-muted-foreground">
          {{ editingId ? "Cập nhật thông tin và vai trò của tài khoản." : "Tạo tài khoản đăng nhập cho nhân viên." }}
        </p>
      </template>

      <div class="space-y-4">
        <div>
          <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Họ và tên</label>
          <Input v-model="form.name" placeholder="VD: Minh Nguyễn" class="mt-1.5 bg-card border-cream-deep h-10 rounded-lg" />
        </div>
        <div>
          <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Email đăng nhập</label>
          <Input v-model="form.email" type="email" placeholder="vd: minh@brew.vn" class="mt-1.5 bg-card border-cream-deep h-10 rounded-lg" />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Vai trò</label>
            <select v-model="form.role" class="mt-1.5 w-full h-10 px-3 rounded-lg bg-card border border-cream-deep text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/30">
              <option v-for="r in (roleFilters.filter(x => x !== 'all') as Account['role'][])" :key="r" :value="r">{{ roleLabel[r] }}</option>
            </select>
          </div>
          <div>
            <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Trạng thái</label>
            <select v-model="form.status" class="mt-1.5 w-full h-10 px-3 rounded-lg bg-card border border-cream-deep text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/30">
              <option value="active">Hoạt động</option>
              <option value="locked">Đã khoá</option>
            </select>
          </div>
        </div>
        <div v-if="!editingId">
          <label class="text-xs font-semibold text-espresso uppercase tracking-wide">Mật khẩu khởi tạo</label>
          <Input v-model="form.password" type="text" placeholder="Tối thiểu 6 ký tự" class="mt-1.5 bg-card border-cream-deep h-10 rounded-lg" />
          <p class="text-[11px] text-muted-foreground mt-1">Nhân viên sẽ đổi mật khẩu này ở lần đăng nhập đầu tiên.</p>
        </div>
        <p v-if="formError" class="text-xs text-destructive font-medium">{{ formError }}</p>
      </div>

      <template #footer>
        <Button variant="outline" @click="modalOpen = false" class="border-cream-deep rounded-lg text-espresso">Huỷ</Button>
        <Button @click="saveAccount" class="bg-caramel text-cream rounded-lg border border-caramel/30">
          {{ editingId ? "Lưu thay đổi" : "Tạo tài khoản" }}
        </Button>
      </template>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, reactive } from 'vue'
import { Plus, Mail, Search, ChevronLeft, ChevronRight, Pencil, Trash2, Lock, Unlock, KeyRound, Clock } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'

interface Account {
  id: string
  name: string
  initials: string
  role: "admin" | "barista" | "cashier" | "waiter"
  email: string
  status: "active" | "locked"
  lastLogin: string
  color: string
}

const roleLabel: Record<Account["role"], string> = {
  admin: "Quản lý",
  barista: "Pha chế",
  cashier: "Thu ngân",
  waiter: "Phục vụ",
}

// Quyền truy cập tương ứng từng vai trò (ý nghĩa "phân quyền")
const rolePermission: Record<Account["role"], string> = {
  admin: "Toàn quyền hệ thống",
  barista: "Bếp, đơn hàng, kho",
  cashier: "Bán hàng, thu ngân, hoá đơn",
  waiter: "Đơn hàng, bàn, phục vụ",
}

const roleBadge: Record<Account["role"], string> = {
  admin: "bg-caramel/15 text-caramel border-caramel/30",
  barista: "bg-sage/15 text-sage border-sage/30",
  cashier: "bg-espresso/10 text-espresso border-espresso/20",
  waiter: "bg-brown/15 text-brown border-brown/30",
}

const avatarColors = ["bg-caramel", "bg-sage", "bg-brown", "bg-espresso"]

const accounts = ref<Account[]>([
  { id: "1", name: "Minh Nguyễn", initials: "MN", role: "admin",   email: "minh@brew.vn", status: "active", lastLogin: "Hôm nay, 08:12", color: "bg-caramel" },
  { id: "2", name: "Lan Trần",    initials: "LT", role: "barista", email: "lan@brew.vn",  status: "active", lastLogin: "Hôm nay, 07:45", color: "bg-sage" },
  { id: "3", name: "Khoa Phạm",   initials: "KP", role: "barista", email: "khoa@brew.vn", status: "active", lastLogin: "Hôm qua, 14:03", color: "bg-brown" },
  { id: "4", name: "Vy Hoàng",    initials: "VH", role: "cashier", email: "vy@brew.vn",   status: "active", lastLogin: "Hôm nay, 07:58", color: "bg-espresso" },
  { id: "5", name: "Nam Lê",      initials: "NL", role: "waiter",  email: "nam@brew.vn",  status: "locked", lastLogin: "12/06/2026", color: "bg-caramel" },
  { id: "6", name: "Thảo Vũ",     initials: "TV", role: "waiter",  email: "thao@brew.vn", status: "active", lastLogin: "Hôm qua, 18:20", color: "bg-sage" },
  { id: "7", name: "Hải Anh",     initials: "HA", role: "waiter",  email: "hai@brew.vn",  status: "active", lastLogin: "Hôm nay, 11:30", color: "bg-caramel" },
])

const roleFilters = ["all", "admin", "barista", "cashier", "waiter"] as const
const filter = ref<"all" | Account["role"]>("all")
const search = ref("")
const currentPage = ref(1)
const itemsPerPage = ref(6)

const activeCount = computed(() => accounts.value.filter(a => a.status === "active").length)
const lockedCount = computed(() => accounts.value.filter(a => a.status === "locked").length)

const filteredItems = computed(() =>
  accounts.value.filter((a) => {
    const matchRole = filter.value === "all" || a.role === filter.value
    const q = search.value.toLowerCase()
    const matchSearch = a.name.toLowerCase().includes(q) || a.email.toLowerCase().includes(q)
    return matchRole && matchSearch
  })
)

const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage.value) || 1)

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredItems.value.slice(start, start + itemsPerPage.value)
})

watch([search, filter], () => { currentPage.value = 1 })

// ── Modal thêm / sửa ────────────────────────────────────────────
const modalOpen = ref(false)
const editingId = ref<string | null>(null)
const formError = ref("")
const form = reactive<{ name: string; email: string; role: Account["role"]; status: Account["status"]; password: string }>({
  name: "", email: "", role: "waiter", status: "active", password: "",
})

const resetForm = () => {
  form.name = ""; form.email = ""; form.role = "waiter"; form.status = "active"; form.password = ""
  formError.value = ""
}

const openAdd = () => {
  editingId.value = null
  resetForm()
  modalOpen.value = true
}

const openEdit = (a: Account) => {
  editingId.value = a.id
  form.name = a.name; form.email = a.email; form.role = a.role; form.status = a.status; form.password = ""
  formError.value = ""
  modalOpen.value = true
}

const initialsFrom = (name: string) =>
  name.trim().split(/\s+/).slice(-2).map(w => w[0]?.toUpperCase() ?? "").join("") || "?"

const saveAccount = () => {
  formError.value = ""
  if (!form.name.trim()) { formError.value = "Vui lòng nhập họ tên."; return }
  if (!/^\S+@\S+\.\S+$/.test(form.email)) { formError.value = "Email không hợp lệ."; return }
  const dup = accounts.value.some(a => a.email.toLowerCase() === form.email.toLowerCase() && a.id !== editingId.value)
  if (dup) { formError.value = "Email này đã được dùng cho tài khoản khác."; return }

  if (editingId.value) {
    const a = accounts.value.find(x => x.id === editingId.value)
    if (a) {
      a.name = form.name.trim()
      a.email = form.email.trim()
      a.role = form.role
      a.status = form.status
      a.initials = initialsFrom(form.name)
    }
  } else {
    if (form.password.trim().length < 6) { formError.value = "Mật khẩu khởi tạo tối thiểu 6 ký tự."; return }
    accounts.value.unshift({
      id: Date.now().toString(),
      name: form.name.trim(),
      email: form.email.trim(),
      initials: initialsFrom(form.name),
      role: form.role,
      status: form.status,
      lastLogin: "",
      color: avatarColors[accounts.value.length % avatarColors.length]!,
    })
  }
  modalOpen.value = false
}

const toggleLock = (a: Account) => {
  a.status = a.status === "active" ? "locked" : "active"
}

const resetPassword = (a: Account) => {
  if (confirm(`Đặt lại mật khẩu cho "${a.name}"? Mật khẩu tạm sẽ được gửi tới ${a.email}.`)) {
    alert(`Đã gửi mật khẩu tạm tới ${a.email}.`)
  }
}

const removeAccount = (a: Account) => {
  if (confirm(`Xoá tài khoản "${a.name}"? Hành động này không thể hoàn tác.`)) {
    accounts.value = accounts.value.filter(x => x.id !== a.id)
  }
}
</script>
