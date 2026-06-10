<template>
  <div class="space-y-5 p-6">
    <!-- Header: Search + Button -->
    <div class="flex flex-wrap items-center gap-3 justify-between">
      <div class="relative flex-1 min-w-[220px] max-w-md">
        <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Tìm theo tên món..."
          v-model="search"
          class="pl-9 bg-card border border-cream-deep shadow-inner h-10 rounded-lg"
        />
      </div>
      <Button @click="openNew" class="bg-caramel text-cream rounded-lg shadow-xl border border-cream-deep/60 px-6 py-2.5 font-semibold flex items-center gap-2">
        <Plus class="w-4 h-4" /> Thêm món mới
      </Button>
    </div>

    <!-- Filter categories -->
    <div class="flex gap-2 overflow-x-auto scrollbar-none pb-1">
      <button
        v-for="c in categories"
        :key="c.id"
        @click="filter = c.id as Category | 'all'"
        :class="[
          'px-5 py-2.5 rounded-lg text-sm font-semibold whitespace-nowrap border shadow-sm',
          filter === c.id
            ? 'bg-espresso text-cream border-espresso shadow-xl'
            : 'bg-card text-espresso border-cream-deep'
        ]"
      >
        {{ c.label }}
      </button>
    </div>

    <!-- Menu grid -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
      <article
        v-for="m in paginatedItems"
        :key="m.id"
        class="bg-card rounded-lg border border-cream-deep shadow-card overflow-hidden flex flex-col"
      >
        <!-- Image area with depth -->
        <div class="relative aspect-[4/3] bg-cream-deep">
          <img
            :src="m.image"
            :alt="m.name"
            loading="lazy"
            class="w-full h-full object-cover"
          />
          <!-- Subtle gradient overlay for depth -->
          <div class="absolute inset-0 bg-gradient-to-b from-transparent via-transparent to-black/20"></div>

          <!-- Popular badge -->
          <span
            v-if="m.popular"
            class="absolute top-4 left-4 px-4 py-1 rounded-lg bg-espresso/95 text-cream text-xs font-bold flex items-center gap-1 shadow-xl"
          >
            <Flame class="w-3.5 h-3.5" /> Bán chạy
          </span>

          <!-- Action buttons - luôn hiển thị, không hover -->
          <div class="absolute top-4 right-4 flex gap-2">
            <button
              @click="openEdit(m)"
              class="w-9 h-9 rounded-lg bg-cream/95 border border-cream-deep shadow-xl flex items-center justify-center text-espresso hover:bg-caramel hover:text-cream"
            >
              <Edit3 class="w-4 h-4" />
            </button>
            <button
              @click="remove(m.id)"
              class="w-9 h-9 rounded-lg bg-cream/95 border border-cream-deep shadow-xl flex items-center justify-center text-destructive"
            >
              <Trash2 class="w-4 h-4" />
            </button>
          </div>
        </div>

        <!-- Content -->
        <div class="p-5 flex-1 flex flex-col">
          <h3 class="font-display text-espresso text-xl font-semibold leading-tight">{{ m.name }}</h3>
          <p class="text-sm text-muted-foreground mt-2 line-clamp-2 flex-1">{{ m.description }}</p>

          <div class="flex justify-between items-end mt-4">
            <span class="text-2xl font-semibold text-caramel">{{ formatVND(m.price) }}</span>
            <span
              class="text-xs uppercase tracking-widest font-medium px-4 py-1 rounded-lg bg-cream border border-cream-deep text-espresso/70"
            >
              {{ categories.find((c) => c.id === m.category)?.label }}
            </span>
          </div>
        </div>
      </article>

      <!-- Empty state -->
      <div
        v-if="paginatedItems.length === 0"
        class="col-span-full text-center py-20 text-muted-foreground flex flex-col items-center"
      >
        <Coffee class="w-16 h-16 mx-auto mb-4 opacity-30" />
        <p class="text-lg font-medium">Không tìm thấy món nào</p>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="filteredItems.length > 0" class="flex items-center justify-between py-2 border-t border-cream-deep/50 mt-4">
      <div class="text-sm text-muted-foreground">
        Hiển thị <span class="font-medium text-espresso">{{ (currentPage - 1) * itemsPerPage + 1 }}</span> - <span class="font-medium text-espresso">{{ Math.min(currentPage * itemsPerPage, filteredItems.length) }}</span> / <span class="font-medium text-espresso">{{ filteredItems.length }}</span> món
      </div>
      <div class="flex items-center gap-2">
        <Button 
          variant="outline"
          size="icon"
          @click="currentPage--" 
          :disabled="currentPage === 1"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50"
        >
          <ChevronLeft class="w-4 h-4" />
        </Button>
        <span class="text-sm font-semibold text-espresso px-3">
          Trang {{ currentPage }} / {{ totalPages }}
        </span>
        <Button 
          variant="outline"
          size="icon"
          @click="currentPage++" 
          :disabled="currentPage === totalPages"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50"
        >
          <ChevronRight class="w-4 h-4" />
        </Button>
      </div>
    </div>

    <!-- Edit/Add Modal (giữ nguyên, chỉ tăng shadow cho modal nếu component hỗ trợ) -->
    <Modal v-model="isOpen">
      <template #header>
        <h2 class="font-display text-3xl text-espresso">
          {{ editing && items.some(i => i.id === editing!.id) ? "Sửa món" : "Thêm món mới" }}
        </h2>
      </template>
      <div v-if="editing" class="space-y-6">
        <!-- Hình ảnh -->
        <div class="space-y-1.5">
          <Label class="text-espresso">Hình ảnh</Label>
          <div class="flex gap-4">
            <!-- Preview -->
            <div class="w-28 h-28 rounded-lg border border-cream-deep bg-cream overflow-hidden shrink-0 flex items-center justify-center">
              <img v-if="editing.image" :src="editing.image" alt="preview" class="w-full h-full object-cover" />
              <ImageIcon v-else class="w-8 h-8 text-muted-foreground/40" />
            </div>
            <!-- Controls -->
            <div class="flex-1 space-y-2 min-w-0">
              <Input v-model="editing.image" placeholder="Dán link ảnh (https://...)" class="bg-background border border-cream-deep rounded-lg shadow-inner" />
              <div class="flex items-center gap-3">
                <Button type="button" variant="outline" @click="triggerUpload" class="border border-cream-deep rounded-lg text-sm">
                  <Upload class="w-4 h-4 mr-1.5" /> Tải ảnh lên
                </Button>
                <button v-if="editing.image" type="button" @click="editing.image = ''" class="text-xs text-destructive font-medium hover:underline">
                  Xóa ảnh
                </button>
              </div>
              <input ref="fileInput" type="file" accept="image/*" class="hidden" @change="onFileChange" />
              <p class="text-[11px] text-muted-foreground">Dán link ảnh hoặc tải lên từ máy (PNG, JPG).</p>
            </div>
          </div>
        </div>

        <div class="space-y-1.5">
          <Label class="text-espresso">Tên món</Label>
          <Input v-model="editing.name" class="bg-background border border-cream-deep rounded-lg shadow-inner" placeholder="Vd: Cappuccino" />
        </div>
        <div class="space-y-1.5">
          <Label class="text-espresso">Mô tả</Label>
          <Textarea v-model="editing.description" class="bg-background border border-cream-deep rounded-lg shadow-inner min-h-[80px]" />
        </div>
        <div class="grid grid-cols-2 gap-4">
          <div class="space-y-1.5">
            <Label class="text-espresso">Giá (VND)</Label>
            <Input type="number" v-model.number="editing.price" class="bg-background border border-cream-deep rounded-lg shadow-inner" />
          </div>
          <div class="space-y-1.5">
            <Label class="text-espresso">Danh mục</Label>
            <select v-model="editing.category" class="flex h-11 w-full rounded-lg border border-cream-deep bg-background px-4 text-base shadow-inner focus-visible:outline-none">
              <option v-for="c in categories.filter(c => c.id !== 'all')" :key="c.id" :value="c.id">{{ c.label }}</option>
            </select>
          </div>
        </div>
        <div class="flex items-center justify-between p-4 rounded-lg bg-background border border-cream-deep shadow-inner">
          <div>
            <Label class="text-espresso">Đánh dấu Bán chạy</Label>
            <p class="text-xs text-muted-foreground mt-0.5">Hiển thị badge nổi bật cho khách</p>
          </div>
          <Switch v-model="editing.popular" />
        </div>
      </div>
      <template #footer>
        <Button variant="outline" @click="isOpen = false" class="border border-cream-deep rounded-lg">Hủy</Button>
        <Button @click="save" class="bg-caramel hover:bg-brown text-cream rounded-lg shadow-xl">Lưu</Button>
      </template>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Plus, Search, Edit3, Trash2, Flame, Coffee, ChevronLeft, ChevronRight, Upload, Image as ImageIcon } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Label from '@/components/ui/Label.vue'
import Switch from '@/components/ui/Switch.vue'
import Textarea from '@/components/ui/Textarea.vue'
import Modal from '@/components/ui/Modal.vue'
import { menuItems as initial, formatVND, categories, type MenuItem, type Category } from '@/data/menu'

const toast = {
  success: (msg: string) => alert('Thành công: ' + msg),
  error: (msg: string) => alert('Lỗi: ' + msg)
}

const items = ref<MenuItem[]>([...initial])
const search = ref("")
const filter = ref<Category | "all">("all")
const editing = ref<MenuItem | null>(null)
const isOpen = ref(false)
const fileInput = ref<HTMLInputElement | null>(null)

const triggerUpload = () => fileInput.value?.click()

const onFileChange = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file || !editing.value) return
  if (!file.type.startsWith('image/')) {
    toast.error('Vui lòng chọn một tệp ảnh hợp lệ')
    return
  }
  const reader = new FileReader()
  reader.onload = () => {
    if (editing.value) editing.value.image = reader.result as string
  }
  reader.readAsDataURL(file)
  // reset để chọn lại cùng 1 file vẫn kích hoạt change
  ;(e.target as HTMLInputElement).value = ''
}

const currentPage = ref(1)
const itemsPerPage = ref(8)

const filteredItems = computed(() => {
  return items.value.filter(m =>
    (filter.value === "all" || m.category === filter.value) &&
    m.name.toLowerCase().includes(search.value.toLowerCase())
  )
})

const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage.value) || 1)

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredItems.value.slice(start, start + itemsPerPage.value)
})

watch([search, filter], () => {
  currentPage.value = 1
})

const openNew = () => {
  editing.value = {
    id: `new-${Date.now()}`,
    name: "",
    description: "",
    price: 0,
    category: "coffee",
    image: "",
    popular: false,
  }
  isOpen.value = true
}

const openEdit = (m: MenuItem) => {
  editing.value = { ...m }
  isOpen.value = true
}

const save = () => {
  if (!editing.value || !editing.value.name.trim()) {
    toast.error("Vui lòng nhập tên món")
    return
  }
 
  const index = items.value.findIndex(p => p.id === editing.value!.id)
  if (index !== -1) {
    items.value[index] = { ...editing.value }
  } else {
    items.value.unshift({ ...editing.value })
  }
 
  toast.success("Đã lưu món")
  isOpen.value = false
}

const remove = (id: string) => {
  items.value = items.value.filter((p) => p.id !== id)
  toast.success("Đã xóa món")
}
</script>
