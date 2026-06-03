<template>
  <div class="space-y-8 font-premium-sans text-[#2A231E]">

    <!-- Page Header -->
    <div class="flex items-center justify-between">
      <div>
        <p class="text-[10px] uppercase tracking-[0.3em] text-[#8A8178] font-bold mb-2">Quản lý sản phẩm</p>
        <h2 class="text-3xl font-premium-serif font-bold text-[#2A231E]">Combo & Ưu đãi</h2>
      </div>
      <button @click="openCreate"
        class="flex items-center gap-2 px-5 py-3 bg-[#CC8033] hover:bg-[#B3702C] text-white rounded-xl text-sm font-bold transition-all duration-200 shadow-md hover:shadow-lg hover:-translate-y-0.5">
        <Plus class="w-4 h-4" stroke-width="2.5" /> Tạo combo mới
      </button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-3 gap-5">
      <div v-for="stat in stats" :key="stat.label"
        class="bg-white rounded-2xl border border-[#EAE3D9] p-5 shadow-sm hover:-translate-y-0.5 transition-all duration-300">
        <div class="flex items-center justify-between mb-3">
          <span class="text-[10px] uppercase tracking-[0.2em] text-[#8A8178] font-bold">{{ stat.label }}</span>
          <div class="w-9 h-9 rounded-xl flex items-center justify-center" :style="`background: ${stat.bg}`">
            <component :is="stat.icon" class="w-4 h-4" :style="`color: ${stat.color}`" stroke-width="2" />
          </div>
        </div>
        <p class="text-3xl font-premium-serif font-bold" :style="`color: ${stat.color}`">{{ stat.value }}</p>
      </div>
    </div>

    <!-- Combo Cards -->
    <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
      <div v-for="combo in combos" :key="combo.id"
        class="bg-white rounded-2xl border border-[#EAE3D9] shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 overflow-hidden flex flex-col">

        <!-- Card image: single fixed image, consistent across all cards -->
        <div class="relative h-32 overflow-hidden bg-[#F0EDE9]">
          <img
            :src="combo.coverImage || getMenuImg(combo.items[0]?.menuId)"
            :alt="combo.name"
            class="w-full h-full object-cover"
          />
          <div class="absolute inset-0 bg-gradient-to-t from-black/70 via-black/10 to-transparent"></div>
          <!-- Overlay text -->
          <div class="absolute inset-0 flex items-end p-4">
            <div class="flex items-end justify-between w-full">
              <div>
                <h3 class="text-white font-premium-serif font-bold text-base leading-tight drop-shadow-md">{{ combo.name }}</h3>
                <p class="text-white/70 text-[10px] font-medium mt-0.5">{{ combo.items.length }} món · {{ combo.items.reduce((s,i)=>s+i.qty,0) }} phần</p>
              </div>
              <span
                :style="combo.active
                  ? 'background: rgba(34,197,94,0.9);'
                  : 'background: rgba(239,68,68,0.9);'"
                class="shrink-0 text-white px-2.5 py-1 rounded-full text-[9px] font-bold uppercase tracking-widest backdrop-blur-sm">
                {{ combo.active ? '● Hoạt động' : '● Đã ẩn' }}
              </span>
            </div>
          </div>
          <!-- Discount badge -->
          <div class="absolute top-3 left-3 bg-[#CC8033] text-white text-xs font-bold rounded-lg px-2.5 py-1 shadow-lg">
            −{{ Math.round((1 - combo.comboPrice / combo.originalPrice) * 100) }}%
          </div>
        </div>

        <!-- Body -->
        <div class="p-5 space-y-4 flex-1 flex flex-col">
          <p class="text-xs text-[#8A8178] font-medium leading-relaxed">{{ combo.description }}</p>

          <!-- Items list with mini images -->
          <div class="space-y-2 flex-1">
            <div v-for="(item, idx) in combo.items" :key="idx"
              class="flex items-center gap-3 py-2 px-3 bg-[#F9F8F6] rounded-xl">
              <img :src="getMenuImg(item.menuId)" :alt="item.name"
                class="w-10 h-10 rounded-xl object-cover shrink-0 border border-[#EAE3D9]" />
              <span class="text-sm text-[#5C544E] font-semibold flex-1 truncate">{{ item.name }}</span>
              <span class="bg-[#EAE3D9] text-[#8A6D53] text-[10px] font-bold px-2 py-0.5 rounded-full shrink-0">×{{ item.qty }}</span>
            </div>
          </div>

          <!-- Price -->
          <div class="flex items-center justify-between py-3 px-4 bg-[#F9F8F6] rounded-xl">
            <div>
              <p class="text-[9px] uppercase tracking-widest text-[#C5BEB8] font-bold">Giá gốc</p>
              <p class="text-sm font-semibold text-[#C5BEB8] line-through">{{ formatVND(combo.originalPrice) }}</p>
            </div>
            <div class="text-right">
              <p class="text-[9px] uppercase tracking-widest text-[#CC8033] font-bold">Giá combo</p>
              <p class="text-lg font-premium-serif font-bold text-[#CC8033]">{{ formatVND(combo.comboPrice) }}</p>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex gap-2 pt-1">
            <button @click="editCombo(combo)"
              class="flex-1 flex items-center justify-center gap-1.5 py-2.5 rounded-xl border border-[#EAE3D9] bg-white hover:bg-[#F5F2ED] text-[#5C544E] text-xs font-bold uppercase tracking-wider transition-colors">
              <Pencil class="w-3.5 h-3.5" /> Sửa
            </button>
            <button @click="toggleActive(combo)"
              class="flex-1 flex items-center justify-center gap-1.5 py-2.5 rounded-xl text-xs font-bold uppercase tracking-wider transition-colors border"
              :class="combo.active ? 'bg-amber-50 border-amber-100 text-amber-600 hover:bg-amber-100' : 'bg-emerald-50 border-emerald-100 text-emerald-600 hover:bg-emerald-100'">
              <Power class="w-3.5 h-3.5" /> {{ combo.active ? 'Tắt' : 'Bật' }}
            </button>
            <button @click="deleteCombo(combo.id)"
              class="w-10 flex items-center justify-center rounded-xl border border-red-100 bg-red-50 text-red-400 hover:bg-red-100 transition-colors">
              <Trash2 class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      <!-- Empty state add card -->
      <button @click="openCreate"
        class="border-2 border-dashed border-[#EAE3D9] rounded-2xl p-8 flex flex-col items-center justify-center gap-3 text-[#C5BEB8] hover:text-[#CC8033] hover:border-[#CC8033] hover:bg-[#FDF9F5] transition-all duration-300 group min-h-[320px]">
        <div class="w-12 h-12 rounded-xl border-2 border-dashed border-current flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
          <Plus class="w-5 h-5" stroke-width="2.5" />
        </div>
        <div class="text-center">
          <p class="text-sm font-bold">Thêm combo mới</p>
          <p class="text-[10px] mt-1 font-medium opacity-70">Nhấn để tạo gói ưu đãi</p>
        </div>
      </button>
    </div>

    <!-- =================== MODAL =================== -->
    <Transition name="modal-fade">
      <div v-if="isModalOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4">
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm" @click="isModalOpen = false"></div>
        <div class="relative w-full max-w-2xl bg-white rounded-2xl shadow-2xl flex flex-col max-h-[90vh] overflow-hidden">

          <!-- Modal Header -->
          <div class="flex items-center justify-between px-6 py-5 border-b border-[#EAE3D9]">
            <h3 class="text-xl font-premium-serif font-bold text-[#2A231E]">
              {{ editingId ? 'Chỉnh sửa Combo' : 'Tạo Combo Mới' }}
            </h3>
            <button @click="isModalOpen = false" class="p-2 hover:bg-[#F5F2ED] rounded-full text-[#8A8178] transition-colors">
              <X class="w-5 h-5" />
            </button>
          </div>

          <div class="flex overflow-hidden flex-1 min-h-0">
            <!-- Left: Basic info -->
            <div class="w-1/2 border-r border-[#EAE3D9] overflow-y-auto p-6 space-y-4">
              <div>
                <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1.5">Tên combo *</label>
                <input v-model="form.name" placeholder="VD: Combo Sáng Năng Lượng"
                  class="w-full px-4 py-3 border border-[#EAE3D9] rounded-xl text-sm font-semibold focus:border-[#CC8033] focus:ring-2 focus:ring-[#CC8033]/10 outline-none" />
              </div>
              <div>
                <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1.5">Mô tả</label>
                <textarea v-model="form.description" rows="2" placeholder="Mô tả ngắn gọn..."
                  class="w-full px-4 py-3 border border-[#EAE3D9] rounded-xl text-sm font-medium resize-none focus:border-[#CC8033] outline-none"></textarea>
              </div>
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1.5">Giá gốc (₫)</label>
                  <input v-model.number="form.originalPrice" type="number" placeholder="0"
                    class="w-full px-3 py-3 border border-[#EAE3D9] rounded-xl text-sm font-bold text-[#8A8178] focus:border-[#CC8033] outline-none" />
                </div>
                <div>
                  <label class="block text-[10px] uppercase tracking-widest font-bold text-[#CC8033] mb-1.5">Giá combo *</label>
                  <input v-model.number="form.comboPrice" type="number" placeholder="0"
                    class="w-full px-3 py-3 border border-[#CC8033]/40 rounded-xl text-sm font-bold text-[#CC8033] focus:border-[#CC8033] outline-none" />
                </div>
              </div>

              <!-- Cover image upload -->
              <div>
                <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1.5">Ảnh bìa combo</label>
                <input ref="fileInputRef" type="file" accept="image/*" class="hidden" @change="handleImageUpload" />
                <div v-if="form.coverImage" class="relative rounded-xl overflow-hidden border border-[#EAE3D9] mb-2">
                  <img :src="form.coverImage" alt="preview" class="w-full h-28 object-cover" />
                  <button @click="form.coverImage = ''"
                    class="absolute top-2 right-2 w-7 h-7 rounded-full bg-black/50 text-white flex items-center justify-center hover:bg-red-500 transition-colors">
                    <X class="w-3.5 h-3.5" />
                  </button>
                  <span class="absolute bottom-2 left-2 text-[9px] uppercase tracking-widest text-white font-bold bg-black/40 px-2 py-1 rounded-full backdrop-blur-sm">Ảnh bìa</span>
                </div>
                <button @click="fileInputRef?.click()"
                  class="w-full flex items-center justify-center gap-2 py-2.5 border-2 border-dashed border-[#EAE3D9] rounded-xl text-[#8A8178] hover:border-[#CC8033] hover:text-[#CC8033] hover:bg-[#FDF9F5] transition-all text-xs font-bold uppercase tracking-wider">
                  <UploadCloud class="w-4 h-4" stroke-width="2" />
                  {{ form.coverImage ? 'Đổi ảnh bìa' : 'Tải ảnh bìa lên' }}
                </button>
              </div>

              <!-- Savings preview -->
              <div v-if="form.originalPrice > 0 && form.comboPrice > 0 && form.comboPrice < form.originalPrice"
                class="flex items-center gap-3 px-4 py-3 bg-[#FDF7EF] border border-[#F0DFC4] rounded-xl">
                <Tag class="w-4 h-4 text-[#CC8033] shrink-0" stroke-width="2.5" />
                <p class="text-sm text-[#2A231E]">
                  Tiết kiệm <strong class="text-[#CC8033] font-premium-serif">{{ formatVND(form.originalPrice - form.comboPrice) }}</strong>
                  (<strong class="text-[#CC8033]">{{ Math.round((1 - form.comboPrice / form.originalPrice) * 100) }}%</strong>)
                </p>
              </div>

              <!-- Selected items preview -->
              <div v-if="form.items.length > 0">
                <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-2">Đã chọn ({{ form.items.length }})</label>
                <div class="space-y-2">
                  <div v-for="(item, idx) in form.items" :key="idx"
                    class="flex items-center gap-2.5 py-2 px-3 bg-[#F9F8F6] rounded-xl">
                    <img :src="getMenuImg(item.menuId)" :alt="item.name" class="w-10 h-10 rounded-xl object-cover shrink-0 border border-[#EAE3D9]" />
                    <span class="text-sm text-[#5C544E] font-semibold flex-1 truncate">{{ item.name }}</span>
                    <div class="flex items-center border border-[#EAE3D9] rounded-lg bg-white overflow-hidden">
                      <button @click="item.qty > 1 && item.qty--" class="px-2 py-1 text-[#8A8178] hover:bg-[#F5F2ED] text-sm font-bold">−</button>
                      <span class="px-2 text-sm font-bold text-[#2A231E] min-w-[1.5rem] text-center">{{ item.qty }}</span>
                      <button @click="item.qty++" class="px-2 py-1 text-[#8A8178] hover:bg-[#F5F2ED] text-sm font-bold">+</button>
                    </div>
                    <button @click="removeItem(idx)" class="w-7 h-7 flex items-center justify-center rounded-lg text-red-400 hover:bg-red-50 transition-colors">
                      <X class="w-3.5 h-3.5" />
                    </button>
                  </div>
                </div>
              </div>
              <div v-else class="text-center py-3 text-[11px] text-[#C5BEB8] font-medium">
                Chọn món từ thực đơn bên phải →
              </div>
            </div>

            <!-- Right: Menu picker -->
            <div class="w-1/2 overflow-y-auto p-6 bg-[#FDFBF7]">
              <div class="mb-4">
                <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-2">Chọn từ thực đơn</label>
                <!-- Category filter -->
                <div class="flex flex-wrap gap-1.5 mb-3">
                  <button v-for="cat in catFilters" :key="cat.id"
                    @click="activeCat = cat.id"
                    :class="activeCat === cat.id
                      ? 'bg-[#CC8033] text-white border-[#CC8033]'
                      : 'bg-white text-[#8A8178] border-[#EAE3D9] hover:border-[#CC8033] hover:text-[#CC8033]'"
                    class="px-2.5 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider border transition-colors">
                    {{ cat.label }}
                  </button>
                </div>
                <!-- Search -->
                <div class="relative">
                  <Search class="w-3.5 h-3.5 absolute left-3 top-1/2 -translate-y-1/2 text-[#C5BEB8]" />
                  <input v-model="menuSearch" placeholder="Tìm món..." class="w-full pl-8 pr-3 py-2 border border-[#EAE3D9] rounded-xl text-sm bg-white focus:border-[#CC8033] outline-none" />
                </div>
              </div>

              <div class="space-y-2">
                <button v-for="item in filteredMenu" :key="item.id"
                  @click="addMenuItem(item)"
                  :class="isSelected(item.id) ? 'border-[#CC8033] bg-[#FDF7EF]' : 'border-[#EAE3D9] bg-white hover:border-[#CC8033] hover:bg-[#FDF9F5]'"
                  class="w-full flex items-center gap-3 p-3 rounded-xl border transition-all duration-150 text-left group">
                  <img :src="item.image" :alt="item.name" class="w-10 h-10 rounded-lg object-cover shrink-0 border border-[#EAE3D9]" />
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-bold text-[#2A231E] truncate">{{ item.name }}</p>
                    <p class="text-[10px] text-[#8A8178] font-medium truncate">{{ item.description }}</p>
                  </div>
                  <div class="flex items-center gap-2 shrink-0">
                    <span class="text-sm font-bold text-[#CC8033]">{{ formatVND(item.price) }}</span>
                    <div :class="isSelected(item.id) ? 'bg-[#CC8033] text-white' : 'bg-[#F5F2ED] text-[#8A8178] group-hover:bg-[#CC8033] group-hover:text-white'"
                      class="w-6 h-6 rounded-full flex items-center justify-center transition-colors">
                      <Check v-if="isSelected(item.id)" class="w-3.5 h-3.5" stroke-width="3" />
                      <Plus v-else class="w-3.5 h-3.5" stroke-width="3" />
                    </div>
                  </div>
                </button>
              </div>
            </div>
          </div>

          <!-- Modal Footer -->
          <div class="px-6 py-4 border-t border-[#EAE3D9] flex justify-end gap-3 bg-white">
            <button @click="isModalOpen = false" class="px-5 py-2.5 rounded-xl text-sm font-bold text-[#5C544E] hover:bg-[#F5F2ED] transition-colors">Hủy</button>
            <button @click="saveCombo"
              class="px-6 py-2.5 rounded-xl text-sm font-bold bg-[#CC8033] hover:bg-[#B3702C] text-white shadow-md transition-all flex items-center gap-2 hover:-translate-y-0.5 disabled:opacity-50"
              :disabled="!form.name.trim() || form.items.length === 0">
              <Check class="w-4 h-4" stroke-width="2.5" />
              {{ editingId ? 'Lưu thay đổi' : 'Tạo combo' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Plus, CheckCircle, Tag, Pencil, Power, Trash2, X, Check, Layers, Search, UploadCloud } from 'lucide-vue-next'
import { menuItems, categories, formatVND, type MenuItem } from '../data/menu'

interface ComboItem { menuId: string; name: string; image: string; qty: number }
interface Combo {
  id: number; name: string; description: string
  originalPrice: number; comboPrice: number
  active: boolean; items: ComboItem[]; coverImage?: string
}

const getMenuImg = (menuId: string) => {
  const found = menuItems.find(m => m.id === menuId)
  return found?.image ?? ''
}

const combos = ref<Combo[]>([
  {
    id: 1, name: 'Combo Sáng Năng Lượng',
    description: 'Cà phê + Bánh sừng bò toàn bộ trong một suất tiết kiệm, lý tưởng để bắt đầu ngày mới.',
    originalPrice: 77000, comboPrice: 62000, active: true,
    items: [
      { menuId: 'c3', name: 'Cà phê sữa đá', image: menuItems.find(m=>m.id==='c3')?.image ?? '', qty: 1 },
      { menuId: 'p1', name: 'Croissant bơ', image: menuItems.find(m=>m.id==='p1')?.image ?? '', qty: 1 },
    ]
  },
  {
    id: 2, name: 'Combo Trà Sữa Đôi',
    description: 'Mua 2 trà sữa bất kỳ với giá ưu đãi đặc biệt, lý tưởng cho cặp đôi.',
    originalPrice: 84000, comboPrice: 70000, active: true,
    items: [
      { menuId: 't1', name: 'Trà sữa trân châu', image: menuItems.find(m=>m.id==='t1')?.image ?? '', qty: 2 },
    ]
  },
  {
    id: 3, name: 'Combo Gia Đình',
    description: 'Bộ đồ uống đa dạng cho cả gia đình, đầy đủ hương vị.',
    originalPrice: 184000, comboPrice: 150000, active: false,
    items: [
      { menuId: 'c3', name: 'Cà phê sữa đá', image: menuItems.find(m=>m.id==='c3')?.image ?? '', qty: 2 },
      { menuId: 't2', name: 'Trà đào cam sả', image: menuItems.find(m=>m.id==='t2')?.image ?? '', qty: 1 },
      { menuId: 'f1', name: 'Matcha đá xay', image: menuItems.find(m=>m.id==='f1')?.image ?? '', qty: 1 },
    ]
  },
])

const stats = computed(() => [
  { label: 'Tổng combo', value: combos.value.length, icon: Layers, color: '#CC8033', bg: '#FDF4E8' },
  { label: 'Đang kích hoạt', value: combos.value.filter(c => c.active).length, icon: CheckCircle, color: '#10B981', bg: '#ECFDF5' },
  { label: 'Tiết kiệm trung bình', value: '15%', icon: Tag, color: '#F59E0B', bg: '#FFFBEB' },
])

// Modal state
const isModalOpen = ref(false)
const editingId = ref<number | null>(null)
const form = ref({ name: '', description: '', originalPrice: 0, comboPrice: 0, coverImage: '', items: [] as ComboItem[] })
const fileInputRef = ref<HTMLInputElement | null>(null)

const handleImageUpload = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return
  const reader = new FileReader()
  reader.onload = (ev) => { form.value.coverImage = ev.target?.result as string }
  reader.readAsDataURL(file)
}

// Menu picker state
const activeCat = ref<string>('all')
const menuSearch = ref('')

const catFilters = [{ id: 'all', label: 'Tất cả' }, ...categories.filter(c => c.id !== 'all')]
const filteredMenu = computed(() =>
  menuItems.filter(m =>
    (activeCat.value === 'all' || m.category === activeCat.value) &&
    m.name.toLowerCase().includes(menuSearch.value.toLowerCase())
  )
)
const isSelected = (menuId: string) => form.value.items.some(i => i.menuId === menuId)

const addMenuItem = (item: MenuItem) => {
  const existing = form.value.items.find(i => i.menuId === item.id)
  if (existing) { existing.qty++ }
  else { form.value.items.push({ menuId: item.id, name: item.name, image: item.image, qty: 1 }) }
}
const removeItem = (idx: number) => form.value.items.splice(idx, 1)

const openCreate = () => {
  editingId.value = null
  form.value = { name: '', description: '', originalPrice: 0, comboPrice: 0, coverImage: '', items: [] }
  activeCat.value = 'all'; menuSearch.value = ''
  isModalOpen.value = true
}
const editCombo = (c: Combo) => {
  editingId.value = c.id
  form.value = { name: c.name, description: c.description, originalPrice: c.originalPrice, comboPrice: c.comboPrice, coverImage: c.coverImage ?? '', items: c.items.map(i => ({ ...i })) }
  activeCat.value = 'all'; menuSearch.value = ''
  isModalOpen.value = true
}
const saveCombo = () => {
  if (!form.value.name.trim() || form.value.items.length === 0) return
  if (editingId.value) {
    const idx = combos.value.findIndex(c => c.id === editingId.value)
    if (idx !== -1) combos.value[idx] = { ...combos.value[idx], ...form.value }
  } else {
    combos.value.push({ id: Date.now(), active: true, ...form.value })
  }
  isModalOpen.value = false
}
const toggleActive = (c: Combo) => { c.active = !c.active }
const deleteCombo = (id: number) => { combos.value = combos.value.filter(c => c.id !== id) }
</script>

<style scoped>
.modal-fade-enter-active, .modal-fade-leave-active { transition: opacity 0.2s ease; }
.modal-fade-enter-from, .modal-fade-leave-to { opacity: 0; }
</style>
