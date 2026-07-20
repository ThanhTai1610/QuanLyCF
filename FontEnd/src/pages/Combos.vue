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
    <div class="grid grid-cols-1 md:grid-cols-3 gap-5">
      <div v-for="stat in stats" :key="stat.label"
        class="bg-white rounded-2xl border border-[#EAE3D9] p-5 shadow-soft hover:shadow-warm hover:-translate-y-1 transition-all duration-300 relative overflow-hidden group">
        <!-- Decorative bg blur -->
        <div class="absolute -right-6 -top-6 w-24 h-24 rounded-full blur-3xl opacity-20 transition-transform group-hover:scale-150" :style="`background: ${stat.color}`"></div>
        
        <div class="flex items-center justify-between mb-4 relative z-10">
          <span class="text-[10px] uppercase tracking-[0.2em] text-[#8A8178] font-bold">{{ stat.label }}</span>
          <div class="w-10 h-10 rounded-xl flex items-center justify-center shadow-inner" :style="`background: ${stat.bg}`">
            <component :is="stat.icon" class="w-5 h-5" :style="`color: ${stat.color}`" stroke-width="2.5" />
          </div>
        </div>
        <p class="text-4xl font-premium-serif font-black relative z-10" :style="`color: ${stat.color}`">{{ stat.value }}</p>
      </div>
    </div>

    <!-- Combo Cards -->
    <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
      <div v-for="combo in combos" :key="combo.id"
        class="bg-white rounded-2xl border border-[#EAE3D9] shadow-sm hover:shadow-xl hover:-translate-y-1 transition-all duration-300 overflow-hidden flex flex-col">

        <!-- Card image: single fixed image, consistent across all cards -->
        <div class="relative h-36 overflow-hidden bg-[#F0EDE9] flex items-center justify-center group-hover:shadow-inner">
          <div class="absolute inset-0 bg-gradient-to-br from-[#CC8033] to-[#8A6D53] opacity-90 flex items-center justify-center">
            <Coffee class="w-12 h-12 text-white opacity-20 animate-pulse" />
          </div>
            v-if="combo.coverImage || getMenuImg(combo.items[0]?.menuId)"
            :src="combo.coverImage || getMenuImg(combo.items[0]?.menuId)"
            :alt="combo.name"
            class="absolute inset-0 w-full h-full object-cover z-10 mix-blend-overlay opacity-60 group-hover:scale-105 transition-transform duration-700"
            @error="(e) => { (e.target as HTMLImageElement).remove(); }"
          />
          <!-- Glassmorphic gradient overlay -->
          <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent z-20"></div>
          
          <!-- Overlay text -->
          <div class="absolute inset-0 flex items-end p-5 z-30">
            <div class="flex items-end justify-between w-full">
              <div class="min-w-0 pr-3">
                <h3 class="text-white font-premium-serif font-bold text-xl leading-tight drop-shadow-lg truncate">{{ combo.name }}</h3>
                <p class="text-white/80 text-[11px] font-semibold mt-1 tracking-wide">{{ combo.items.length }} món · {{ combo.items.reduce((s,i)=>s+i.qty,0) }} phần</p>
              </div>
              <span
                :class="combo.active ? 'bg-emerald-500/90 shadow-emerald-500/30' : 'bg-red-500/90 shadow-red-500/30'"
                class="shrink-0 text-white px-2.5 py-1.5 rounded-lg text-[9px] font-bold uppercase tracking-widest backdrop-blur-md shadow-md border border-white/20">
                {{ combo.active ? 'Đang bật' : 'Đã ẩn' }}
              </span>
            </div>
          </div>
          <!-- Discount badge -->
          <div class="absolute top-4 left-4 bg-white/20 backdrop-blur-md border border-white/30 text-white text-xs font-black tracking-wider rounded-xl px-3 py-1.5 shadow-lg z-30">
            −{{ Math.round((1 - combo.comboPrice / combo.originalPrice) * 100) }}%
          </div>
        </div>

        <!-- Body -->
        <div class="p-5 space-y-4 flex-1 flex flex-col bg-white">
          <p v-if="combo.description" class="text-xs text-[#8A8178] font-medium leading-relaxed line-clamp-2">{{ combo.description }}</p>

          <!-- Items list -->
          <div class="space-y-2 flex-1 pt-1">
            <div v-for="(item, idx) in combo.items" :key="idx"
              class="flex items-center gap-3 py-2 px-3 bg-[#FDFBF7] border border-[#F5F2ED] rounded-xl hover:border-[#EAE3D9] hover:bg-white transition-colors">
              <div class="w-10 h-10 rounded-xl bg-[#F0EDE9] flex items-center justify-center overflow-hidden shrink-0 border border-[#EAE3D9] relative shadow-sm">
                <Coffee class="w-4 h-4 text-[#8A6D53] opacity-30" />
                <img v-if="getMenuImg(item.menuId)" :src="getMenuImg(item.menuId)" :alt="item.name"
                  class="absolute inset-0 w-full h-full object-cover z-10"
                  @error="(e) => { (e.target as HTMLImageElement).remove(); }" />
              </div>
              <span class="text-sm text-[#2A231E] font-semibold flex-1 truncate">{{ item.name }}</span>
              <span class="bg-[#CC8033]/10 text-[#CC8033] text-[11px] font-black px-2 py-1 rounded-lg shrink-0">×{{ item.qty }}</span>
            </div>
          </div>

          <!-- Price -->
          <div class="flex items-center justify-between pt-4 pb-2 border-t border-dashed border-[#EAE3D9]">
            <div>
              <p class="text-[9px] uppercase tracking-[0.2em] text-[#C5BEB8] font-bold mb-0.5">Giá lẻ</p>
              <p class="text-sm font-semibold text-[#C5BEB8] line-through decoration-[#C5BEB8]/50">{{ formatVND(combo.originalPrice) }}</p>
            </div>
            <div class="text-right bg-amber-50 px-3 py-1.5 rounded-xl border border-amber-100/50">
              <p class="text-[9px] uppercase tracking-[0.2em] text-[#CC8033] font-bold mb-0.5">Giá Combo</p>
              <p class="text-xl font-premium-serif font-black text-[#CC8033]">{{ formatVND(combo.comboPrice) }}</p>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex gap-2 pt-2">
            <button @click="editCombo(combo)"
              class="flex-1 flex items-center justify-center gap-1.5 py-2.5 rounded-xl border border-[#EAE3D9] bg-white hover:bg-[#F5F2ED] text-[#5C544E] text-[11px] font-bold uppercase tracking-wider transition-all shadow-sm active:scale-95">
              <Pencil class="w-3.5 h-3.5" /> Sửa
            </button>
            <button @click="toggleActive(combo)"
              class="flex-1 flex items-center justify-center gap-1.5 py-2.5 rounded-xl text-[11px] font-bold uppercase tracking-wider transition-all shadow-sm border active:scale-95"
              :class="combo.active ? 'bg-amber-50 border-amber-200 text-amber-700 hover:bg-amber-100' : 'bg-emerald-50 border-emerald-200 text-emerald-700 hover:bg-emerald-100'">
              <Power class="w-3.5 h-3.5" /> {{ combo.active ? 'Tắt' : 'Bật' }}
            </button>
            <button @click="deleteCombo(combo.id)"
              class="w-10 flex items-center justify-center rounded-xl border border-red-200 bg-red-50 text-red-500 hover:bg-red-100 transition-all shadow-sm active:scale-95">
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
                <label class="block text-[10px] uppercase tracking-widest font-bold mb-1.5"
                  :class="errors.name ? 'text-red-500' : 'text-[#8A8178]'">Tên combo *</label>
                <input v-model="form.name" placeholder="VD: Combo Sáng Năng Lượng"
                  :class="errors.name ? 'border-red-400 focus:border-red-400 focus:ring-red-400/10' : 'border-[#EAE3D9] focus:border-[#CC8033] focus:ring-[#CC8033]/10'"
                  class="w-full px-4 py-3 border rounded-xl text-sm font-semibold focus:ring-2 outline-none" />
                <p v-if="errors.name" class="text-[11px] text-red-500 font-semibold mt-1">{{ errors.name }}</p>
              </div>
              <div>
                <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1.5">Mô tả</label>
                <textarea v-model="form.description" rows="2" placeholder="Mô tả ngắn gọn..."
                  class="w-full px-4 py-3 border border-[#EAE3D9] rounded-xl text-sm font-medium resize-none focus:border-[#CC8033] outline-none"></textarea>
              </div>
              <div class="grid grid-cols-2 gap-3">
                <div>
                  <label class="block text-[10px] uppercase tracking-widest font-bold text-[#8A8178] mb-1.5">Giá gốc (₫)</label>
                  <input v-model.number="form.originalPrice" type="number" readonly placeholder="0"
                    class="w-full px-3 py-3 border border-[#EAE3D9] rounded-xl text-sm font-bold text-[#8A8178] bg-[#F5F2ED] outline-none cursor-not-allowed" />
                </div>
                <div>
                  <label class="block text-[10px] uppercase tracking-widest font-bold mb-1.5"
                    :class="errors.comboPrice ? 'text-red-500' : 'text-[#CC8033]'">Giá combo *</label>
                  <input v-model.number="form.comboPrice" type="number" placeholder="0"
                    :class="errors.comboPrice ? 'border-red-400 focus:border-red-400 focus:ring-red-400/10' : 'border-[#CC8033]/40 focus:border-[#CC8033]'"
                    class="w-full px-3 py-3 rounded-xl text-sm font-bold text-[#CC8033] outline-none" />
                  <p v-if="errors.comboPrice" class="text-[10px] text-red-500 font-semibold mt-1">{{ errors.comboPrice }}</p>
                </div>
              </div>

              <!-- Cover image upload -->
              <div>
                <label class="block text-[10px] uppercase tracking-widest font-bold mb-1.5"
                  :class="errors.coverImage ? 'text-red-500' : 'text-[#8A8178]'">Ảnh bìa combo *</label>
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
                  :class="errors.coverImage ? 'border-red-300 hover:border-red-400 hover:bg-red-50/20 text-red-500' : 'border-[#EAE3D9] hover:border-[#CC8033] hover:text-[#CC8033] hover:bg-[#FDF9F5]'"
                  class="w-full flex items-center justify-center gap-2 py-2.5 border-2 border-dashed rounded-xl transition-all text-xs font-bold uppercase tracking-wider">
                  <UploadCloud class="w-4 h-4" stroke-width="2" />
                  {{ form.coverImage ? 'Đổi ảnh bìa' : 'Tải ảnh bìa lên' }}
                </button>
                <p v-if="errors.coverImage" class="text-[11px] text-red-500 font-semibold mt-1">{{ errors.coverImage }}</p>
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
                    <div class="w-10 h-10 rounded-xl bg-[#F0EDE9] flex items-center justify-center overflow-hidden shrink-0 border border-[#EAE3D9] relative">
                      <Coffee class="w-4 h-4 text-[#8A6D53] opacity-45" />
                      <img v-if="getMenuImg(item.menuId)" :src="getMenuImg(item.menuId)" :alt="item.name"
                        class="absolute inset-0 w-full h-full object-cover z-10"
                        @error="(e) => { (e.target as HTMLImageElement).remove(); }" />
                    </div>
                    <span class="text-sm text-[#5C544E] font-semibold flex-1 truncate">{{ item.name }}</span>
                    <div class="flex items-center border border-[#EAE3D9] rounded-lg bg-white overflow-hidden">
                      <button @click="item.qty > 1 && item.qty--; recalcOriginalPrice()" class="px-2 py-1 text-[#8A8178] hover:bg-[#F5F2ED] text-sm font-bold">−</button>
                      <span class="px-2 text-sm font-bold text-[#2A231E] min-w-[1.5rem] text-center">{{ item.qty }}</span>
                      <button @click="item.qty++; recalcOriginalPrice()" class="px-2 py-1 text-[#8A8178] hover:bg-[#F5F2ED] text-sm font-bold">+</button>
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
              <p v-if="errors.items" class="text-[11px] text-red-500 font-semibold text-center mt-2">{{ errors.items }}</p>
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
                  <div class="w-10 h-10 rounded-lg bg-[#F0EDE9] flex items-center justify-center overflow-hidden shrink-0 border border-[#EAE3D9] relative">
                    <Coffee class="w-4 h-4 text-[#8A6D53] opacity-45" />
                    <img v-if="item.image" :src="item.image" :alt="item.name"
                      class="absolute inset-0 w-full h-full object-cover z-10"
                      @error="(e) => { (e.target as HTMLImageElement).remove(); }" />
                  </div>
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
import { ref, computed, onMounted, watch } from 'vue'
import { Plus, CheckCircle, Tag, Pencil, Power, Trash2, X, Check, Layers, Search, UploadCloud, Coffee } from 'lucide-vue-next'
import { combosApi } from '@/services/combos'
import { productsApi, type ProductListItem, type CategoryItem } from '@/services/products'
import { useToast } from '@/stores/toast'
import { useAlert } from '@/stores/alert'

const toast = useToast()
const alert = useAlert()
const formatVND = (v: number) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(v)

interface ComboItem { menuId: number; name: string; image: string; qty: number; price: number }
interface Combo {
  id: number; name: string; description: string
  originalPrice: number; comboPrice: number
  active: boolean; items: ComboItem[]; coverImage?: string
}

const combos = ref<Combo[]>([])
const allProducts = ref<ProductListItem[]>([])
const allCategories = ref<CategoryItem[]>([])

const fetchCombos = async () => {
  try {
    const res = await combosApi.list()
    const details = await Promise.all(res.map(c => combosApi.get(c.maCombo)))
    combos.value = details.map(data => {
      let origPrice = 0
      const itemsMapped = data.chiTiets.map(ct => {
        const p = allProducts.value.find(x => x.maSanPham === ct.maSanPham)
        origPrice += (p?.giaBan || 0) * ct.soLuong
        return {
          menuId: ct.maSanPham,
          name: ct.tenSanPham || p?.tenSanPham || '',
          image: p?.hinhAnh || '',
          qty: ct.soLuong,
          price: p?.giaBan || 0
        }
      })
      return {
        id: data.maCombo,
        name: data.tenCombo,
        description: data.moTa || '',
        originalPrice: origPrice,
        comboPrice: data.giaCombo,
        active: data.trangThaiHoatDong,
        coverImage: data.hinhAnh || '',
        items: itemsMapped
      }
    })
  } catch (err) {
    console.error(err)
    toast.error('Lỗi khi tải dữ liệu Combo')
  }
}

const fetchProducts = async () => {
  try {
    const [pRes, cRes] = await Promise.all([
      productsApi.list(),
      productsApi.listCategories()
    ])
    allProducts.value = pRes
    allCategories.value = cRes
  } catch (err) {
    toast.error('Lỗi khi tải danh sách sản phẩm')
  }
}

onMounted(async () => {
  await fetchProducts()
  await fetchCombos()
})

const getMenuImg = (menuId: number) => {
  const p = allProducts.value.find(x => x.maSanPham === menuId)
  return p?.hinhAnh || ''
}

const stats = computed(() => [
  { label: 'Tổng combo', value: combos.value.length, icon: Layers, color: '#CC8033', bg: '#FDF4E8' },
  { label: 'Đang kích hoạt', value: combos.value.filter(c => c.active).length, icon: CheckCircle, color: '#10B981', bg: '#ECFDF5' },
  { label: 'Tiết kiệm trung bình', value: combos.value.length ? Math.round(combos.value.reduce((s,c) => s + (1 - (c.comboPrice / (c.originalPrice || 1))), 0) / combos.value.length * 100) + '%' : '0%', icon: Tag, color: '#F59E0B', bg: '#FFFBEB' },
])

// Modal state
const isModalOpen = ref(false)
const editingId = ref<number | null>(null)
const form = ref({ name: '', description: '', originalPrice: 0, comboPrice: 0, coverImage: '', items: [] as ComboItem[] })
const fileInputRef = ref<HTMLInputElement | null>(null)

const errors = ref({
  name: '',
  comboPrice: '',
  items: '',
  coverImage: ''
})

const validateForm = () => {
  let isValid = true
  errors.value = { name: '', comboPrice: '', items: '', coverImage: '' }

  if (!form.value.name.trim()) {
    errors.value.name = 'Tên combo không được để trống'
    isValid = false
  } else if (form.value.name.length > 100) {
    errors.value.name = 'Tên combo không được dài quá 100 ký tự'
    isValid = false
  }

  if (form.value.comboPrice <= 0) {
    errors.value.comboPrice = 'Giá combo phải lớn hơn 0 ₫'
    isValid = false
  } else if (form.value.comboPrice > form.value.originalPrice) {
    errors.value.comboPrice = 'Giá combo không được lớn hơn tổng giá gốc các món lẻ'
    isValid = false
  }

  if (form.value.items.length === 0) {
    errors.value.items = 'Vui lòng chọn ít nhất 1 món từ thực đơn'
    isValid = false
  }

  if (!form.value.coverImage) {
    errors.value.coverImage = 'Vui lòng tải ảnh bìa cho combo'
    isValid = false
  }

  return isValid
}

// Watch inputs to clear validation errors reactively
watch(() => form.value.name, (newVal) => {
  if (newVal.trim() && newVal.length <= 100) errors.value.name = ''
})

watch(() => form.value.comboPrice, (newVal) => {
  if (newVal > 0 && newVal <= form.value.originalPrice) errors.value.comboPrice = ''
})

watch(() => form.value.items, (newVal) => {
  if (newVal.length > 0) errors.value.items = ''
})

watch(() => form.value.coverImage, (newVal) => {
  if (newVal) errors.value.coverImage = ''
}, { deep: true })

const handleImageUpload = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file) return
  const reader = new FileReader()
  reader.onload = (ev) => { form.value.coverImage = ev.target?.result as string }
  reader.readAsDataURL(file)
}

// Menu picker state
const activeCat = ref<number | 'all'>('all')
const menuSearch = ref('')

const catFilters = computed(() => [{ id: 'all' as const, label: 'Tất cả' }, ...allCategories.value.map(c => ({ id: c.maDanhMuc, label: c.tenDanhMuc }))])
const filteredMenu = computed(() =>
  allProducts.value.filter(m =>
    (activeCat.value === 'all' || m.maDanhMuc === activeCat.value) &&
    m.tenSanPham.toLowerCase().includes(menuSearch.value.toLowerCase())
  ).map(p => ({
    id: p.maSanPham,
    name: p.tenSanPham,
    description: p.tenDanhMuc || '',
    price: p.giaBan,
    image: p.hinhAnh || ''
  }))
)

const isSelected = (menuId: number) => form.value.items.some(i => i.menuId === menuId)

const addMenuItem = (item: any) => {
  const existing = form.value.items.find(i => i.menuId === item.id)
  if (existing) { existing.qty++ }
  else { form.value.items.push({ menuId: item.id, name: item.name, image: item.image, qty: 1, price: item.price }) }
  recalcOriginalPrice()
}

const removeItem = (idx: number) => {
  form.value.items.splice(idx, 1)
  recalcOriginalPrice()
}

const recalcOriginalPrice = () => {
  form.value.originalPrice = form.value.items.reduce((s, i) => s + i.price * i.qty, 0)
  // Re-validate price difference if originalPrice drops below comboPrice
  if (form.value.comboPrice > form.value.originalPrice) {
    errors.value.comboPrice = 'Giá combo không được lớn hơn tổng giá gốc các món lẻ'
  } else {
    errors.value.comboPrice = ''
  }
}

const openCreate = () => {
  editingId.value = null
  form.value = { name: '', description: '', originalPrice: 0, comboPrice: 0, coverImage: '', items: [] }
  errors.value = { name: '', comboPrice: '', items: '' }
  activeCat.value = 'all'; menuSearch.value = ''
  isModalOpen.value = true
}

const editCombo = (c: Combo) => {
  editingId.value = c.id
  form.value = { name: c.name, description: c.description, originalPrice: c.originalPrice, comboPrice: c.comboPrice, coverImage: c.coverImage ?? '', items: c.items.map(i => ({ ...i })) }
  errors.value = { name: '', comboPrice: '', items: '' }
  activeCat.value = 'all'; menuSearch.value = ''
  isModalOpen.value = true
}

const saveCombo = async () => {
  if (!validateForm()) return
  try {
    const payload = {
      tenCombo: form.value.name,
      giaCombo: form.value.comboPrice,
      hinhAnh: form.value.coverImage || null,
      moTa: form.value.description || null,
      trangThaiHoatDong: true,
      chiTiets: form.value.items.map(i => ({ maSanPham: i.menuId, soLuong: i.qty }))
    }
    
    if (editingId.value) {
      await combosApi.update(editingId.value, payload)
      toast.success('Cập nhật combo thành công')
    } else {
      await combosApi.create(payload)
      toast.success('Tạo combo thành công')
    }
    isModalOpen.value = false
    fetchCombos()
  } catch (err) {
    toast.error('Có lỗi xảy ra khi lưu combo')
  }
}

const toggleActive = async (c: Combo) => {
  try {
    await combosApi.update(c.id, {
      tenCombo: c.name,
      giaCombo: c.comboPrice,
      hinhAnh: c.coverImage || null,
      moTa: c.description || null,
      trangThaiHoatDong: !c.active,
      chiTiets: c.items.map(i => ({ maSanPham: i.menuId, soLuong: i.qty }))
    })
    c.active = !c.active
    toast.success(`Đã ${c.active ? 'Bật' : 'Tắt'} combo ${c.name}`)
  } catch (err) {
    toast.error('Lỗi khi đổi trạng thái')
  }
}

const deleteCombo = async (id: number) => {
  const confirmed = await alert.confirm('Xác nhận xóa', 'Bạn có chắc chắn muốn xóa combo này?')
  if (!confirmed) return
  try {
    await combosApi.delete(id)
    toast.success('Đã xóa combo')
    fetchCombos()
  } catch (err: any) {
    toast.error(err.message || 'Không thể xóa combo')
  }
}
</script>

<style scoped>
.modal-fade-enter-active, .modal-fade-leave-active { transition: opacity 0.2s ease; }
.modal-fade-enter-from, .modal-fade-leave-to { opacity: 0; }
</style>
