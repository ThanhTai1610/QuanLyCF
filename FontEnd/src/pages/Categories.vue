<template>
  <div class="p-6 max-w-[1600px] mx-auto bg-[#FAF6F0] min-h-screen">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-6">
      <div>
        <h1 class="text-3xl font-extrabold bg-gradient-to-r from-gray-900 via-gray-700 to-gray-600 bg-clip-text text-transparent">Danh Mục Sản Phẩm</h1>
        <p class="text-sm text-gray-500 mt-1.5 font-medium">Quản lý các nhóm thức uống và món ăn của quán</p>
      </div>
     
      <div class="flex items-center gap-3">
        <!-- Nút thêm mới -->
        <button
          @click="openAddModal"
          class="bg-gradient-to-r from-[#CC8033] to-[#e09852] hover:from-[#b56e29] hover:to-[#CC8033] text-white px-6 py-2.5 rounded-lg text-sm font-bold flex items-center gap-2 shadow-lg shadow-[#CC8033]/30 hover:-translate-y-0.5 transition-all duration-300"
        >
          <Plus class="w-4 h-4" />
          Thêm danh mục
        </button>
      </div>
    </div>

    <!-- Main Card -->
    <div class="bg-white rounded-lg border border-[#EDE4D9] shadow-card overflow-hidden">
     
      <!-- Tabs & Search Toolbar -->
      <div class="p-5 border-b border-[#EDE4D9] flex flex-col md:flex-row md:items-center justify-between gap-4">
       
        <!-- Cụm Tab (Phân loại & Trạng thái) -->
        <div class="flex flex-col sm:flex-row sm:items-center gap-6">
          <!-- Phân loại -->
          <div class="flex bg-[#F8F4ED] p-1 rounded-lg border border-[#EDE4D9]">
            <button @click="categoryType = 'MAIN'" :class="categoryType === 'MAIN' ? 'bg-white text-[#CC8033] shadow-sm' : 'text-gray-500 hover:text-gray-700'" class="px-4 py-1.5 rounded-md text-sm font-bold transition-all flex items-center gap-2">Món chính</button>
            <button @click="categoryType = 'TOPPING'" :class="categoryType === 'TOPPING' ? 'bg-white text-[#CC8033] shadow-sm' : 'text-gray-500 hover:text-gray-700'" class="px-4 py-1.5 rounded-md text-sm font-bold transition-all flex items-center gap-2">Nhóm Topping</button>
          </div>

          <!-- Trạng thái -->
          <div class="flex items-center gap-6 border-l border-[#EDE4D9] pl-6">
            <button
              @click="activeTab = 'all'"
              :class="['text-sm font-semibold flex items-center gap-1.5 transition-colors', activeTab === 'all' ? 'text-[#CC8033]' : 'text-gray-500 hover:text-gray-700']"
            >
              Tất cả <span class="bg-[#F5EDE4] text-gray-700 py-0.5 px-2 rounded-md text-xs">{{ countAll }}</span>
            </button>
            <button
              @click="activeTab = 'visible'"
              :class="['text-sm font-semibold flex items-center gap-1.5 transition-colors', activeTab === 'visible' ? 'text-[#CC8033]' : 'text-gray-500 hover:text-gray-700']"
            >
              Hiển thị <span class="bg-[#F5EDE4] text-gray-700 py-0.5 px-2 rounded-md text-xs">{{ countVisible }}</span>
            </button>
            <button
              @click="activeTab = 'hidden'"
              :class="['text-sm font-semibold flex items-center gap-1.5 transition-colors', activeTab === 'hidden' ? 'text-amber-600' : 'text-gray-500 hover:text-gray-700']"
            >
              Ẩn <span class="bg-[#F5EDE4] text-gray-700 py-0.5 px-2 rounded-md text-xs">{{ countHidden }}</span>
            </button>
          </div>
        </div>

        <!-- Search Bar -->
        <div class="relative w-full md:w-72">
          <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Tìm danh mục..."
            class="w-full pl-9 pr-4 py-2 bg-[#F8F4ED] border border-[#EDE4D9] rounded-lg text-sm focus:outline-none shadow-inner"
          />
        </div>
      </div>

      <!-- Table -->
      <div class="overflow-x-auto">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-[#F8F4ED] text-gray-600 text-sm border-b border-[#EDE4D9]">
              <th class="py-5 px-6 font-semibold w-24">STT</th>
              <th class="py-5 px-6 font-semibold">Danh mục</th>
              <th class="py-5 px-6 font-semibold text-center">Số món</th>
              <th class="py-5 px-6 font-semibold">Trạng thái</th>
              <th class="py-5 px-6 font-semibold text-center w-32">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="filteredCats.length === 0">
              <td colspan="5" class="py-16 text-center text-gray-400">
                Không tìm thấy dữ liệu phù hợp.
              </td>
            </tr>
            <tr
              v-for="(cat, idx) in filteredCats"
              :key="cat.id"
              class="border-b border-[#EDE4D9] hover:bg-orange-50/40 transition-colors duration-200 group"
              draggable="true"
              @dragstart="onDragStart(idx)"
              @dragenter="onDragEnter(idx)"
              @dragover.prevent
              @drop="onDrop(idx)"
              @dragend="onDragEnd"
            >
              <td class="py-5 px-6 font-medium text-gray-700">
                <div class="flex items-center gap-3">
                  <GripVertical class="w-4 h-4 text-gray-400 cursor-grab" />
                  {{ idx + 1 }}
                </div>
              </td>
             
              <td class="py-5 px-6">
                <div class="flex items-center gap-4">
                  <!-- Ảnh đại diện -->
                  <div class="w-12 h-12 rounded-md bg-gray-100 flex-shrink-0 overflow-hidden border border-[#EDE4D9]">
                    <img 
                      :src="cat.image" 
                      :alt="cat.name" 
                      class="w-full h-full object-cover" 
                      @error="(e) => (e.target as HTMLImageElement).src = 'https://placehold.co/100x100?text=No+Image'" 
                    />
                  </div>
                  <div>
                    <div class="font-bold text-gray-800">{{ cat.name }}</div>
                    <div class="text-xs text-gray-400 flex items-center gap-1 mt-0.5">
                      <Link class="w-3 h-3" /> {{ toSlug(cat.name) }}
                    </div>
                  </div>
                </div>
              </td>
             
              <td class="py-5 px-6 text-center">
                <span class="px-3 py-1 bg-[#F8F4ED] text-[#CC8033] rounded-full text-xs font-bold border border-[#CC8033]/20 shadow-sm inline-block min-w-[4rem]">
                  {{ cat.count }} món
                </span>
              </td>
             
              <td class="py-5 px-6">
                <label class="relative inline-flex items-center cursor-pointer">
                  <input type="checkbox" v-model="cat.visible" @change="toggleVisibility(cat)" class="sr-only peer">
                  <div class="w-11 h-6 bg-gray-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-[#CC8033]"></div>
                  <span class="ml-3 text-xs font-semibold" :class="cat.visible ? 'text-green-600' : 'text-gray-500'">{{ cat.visible ? 'Hiển thị' : 'Đang ẩn' }}</span>
                </label>
              </td>
             
              <!-- Thao tác -->
              <td class="py-5 px-6">
                <div class="flex items-center justify-center gap-3">
                  <!-- Nút Gán món -->
                  <button
                    @click="openAssignModal(cat)"
                    class="p-2.5 text-blue-500 border border-blue-200 rounded-lg hover:bg-blue-500 hover:text-white transition-all duration-300 shadow-sm hover:shadow-md"
                    title="Gán món nhanh"
                  >
                    <ListPlus class="w-4 h-4" />
                  </button>
                  <!-- Nút Sửa -->
                  <button
                    @click="openEditModal(cat)"
                    class="p-2.5 text-[#CC8033] border border-[#CC8033]/20 rounded-lg hover:bg-[#CC8033] hover:text-white transition-all duration-300 shadow-sm hover:shadow-md"
                    title="Chỉnh sửa"
                  >
                    <Edit class="w-4 h-4" />
                  </button>
                  <!-- Nút Xóa -->
                  <button
                    @click="deleteCat(cat)"
                    class="p-2.5 text-red-500 border border-red-200 rounded-lg hover:bg-red-500 hover:text-white transition-all duration-300 shadow-sm hover:shadow-md"
                    title="Xóa"
                  >
                    <Trash2 class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Footer -->
      <div class="p-5 border-t border-[#EDE4D9] flex items-center justify-between text-sm text-gray-500">
        <div>Hiển thị {{ filteredCats.length }} / {{ cats.length }} danh mục (Kéo thả để đổi vị trí)</div>
        <div class="flex gap-2">
          <button class="px-4 py-2 border border-[#EDE4D9] rounded-md">‹</button>
          <button class="px-4 py-2 border border-[#CC8033] bg-[#CC8033] text-white rounded-md">1</button>
          <button class="px-4 py-2 border border-[#EDE4D9] rounded-md">›</button>
        </div>
      </div>
    </div>

    <!-- ============================================== -->
    <!-- MODAL FORM -->
    <!-- ============================================== -->
    <div v-if="isModalOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4 transition-all duration-300">
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl overflow-hidden flex flex-col max-h-[90vh] ring-1 ring-black/5 animate-in fade-in zoom-in-95 duration-200">
       
        <!-- Modal Header -->
        <div class="px-6 py-5 border-b border-[#EDE4D9] flex items-center justify-between bg-gradient-to-r from-[#FAF6F0] to-white">
          <h3 class="text-xl font-bold text-gray-800 flex items-center gap-2">
            <Edit class="w-5 h-5 text-[#CC8033]" v-if="isEditing" />
            <Plus class="w-5 h-5 text-[#CC8033]" v-else />
            {{ isEditing ? 'Cập nhật danh mục' : 'Thêm danh mục mới' }}
          </h3>
          <button @click="closeModal" class="p-2 text-gray-400 hover:text-gray-700 rounded-md">
            <X class="w-5 h-5" />
          </button>
        </div>

        <!-- Modal Body -->
        <div class="p-6 overflow-y-auto flex-1">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <!-- Loại danh mục -->
            <div class="space-y-1.5 md:col-span-2">
              <label class="text-sm font-semibold text-gray-700">Phân loại <span class="text-red-500">*</span></label>
              <div class="flex gap-6 mt-1">
                <label class="flex items-center gap-2 cursor-pointer bg-white border border-[#EDE4D9] px-4 py-2.5 rounded-lg flex-1 shadow-sm">
                  <input type="radio" v-model="formData.type" value="MAIN" class="w-4 h-4 text-[#CC8033] accent-[#CC8033]">
                  <span class="text-sm font-medium text-gray-700">Món chính (Đồ uống, Thức ăn)</span>
                </label>
                <label class="flex items-center gap-2 cursor-pointer bg-white border border-[#EDE4D9] px-4 py-2.5 rounded-lg flex-1 shadow-sm">
                  <input type="radio" v-model="formData.type" value="TOPPING" class="w-4 h-4 text-[#CC8033] accent-[#CC8033]">
                  <span class="text-sm font-medium text-gray-700">Nhóm Topping (Trân châu, Mức đường...)</span>
                </label>
              </div>
            </div>
           
            <!-- Tên danh mục -->
            <div class="space-y-1.5">
              <label class="text-sm font-semibold text-gray-700">Tên danh mục <span class="text-red-500">*</span></label>
              <input
                v-model="formData.name"
                type="text"
                class="w-full px-4 py-3 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none"
                placeholder="VD: Cà phê, Trà sữa..."
              />
            </div>

            <!-- Thứ tự -->
            <div class="space-y-1.5">
              <label class="text-sm font-semibold text-gray-700">Thứ tự hiển thị</label>
              <input
                v-model.number="formData.order"
                type="number"
                class="w-full px-4 py-3 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none"
              />
            </div>

            <!-- Cài đặt khung giờ -->
            <div class="space-y-3 md:col-span-2 border border-[#EDE4D9] p-4 rounded-lg bg-[#FAF6F0]">
              <label class="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" v-model="formData.isTimeBased" class="w-4 h-4 text-[#CC8033] rounded border-gray-300 focus:ring-[#CC8033] accent-[#CC8033]">
                <span class="text-sm font-semibold text-gray-800">Chỉ hiển thị theo khung giờ</span>
              </label>
              
              <div v-if="formData.isTimeBased" class="flex items-center gap-4 mt-3">
                <div class="flex-1 space-y-1.5">
                  <label class="text-xs font-semibold text-gray-500">Giờ bắt đầu</label>
                  <input type="time" v-model="formData.startTime" class="w-full px-4 py-2 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none" />
                </div>
                <div class="flex-1 space-y-1.5">
                  <label class="text-xs font-semibold text-gray-500">Giờ kết thúc</label>
                  <input type="time" v-model="formData.endTime" class="w-full px-4 py-2 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none" />
                </div>
              </div>
            </div>



            <!-- Cấu hình Topping (Chỉ hiện khi type = TOPPING) -->
            <div v-if="formData.type === 'TOPPING'" class="space-y-3 md:col-span-2 border border-[#EDE4D9] p-4 rounded-lg bg-[#FAF6F0]">
              <h4 class="text-sm font-bold text-gray-800">Cấu hình Nhóm Topping</h4>
              <div class="flex items-center gap-4">
                <div class="flex-1 space-y-1.5">
                  <label class="text-xs font-semibold text-gray-500">Tối thiểu chọn</label>
                  <input type="number" v-model.number="formData.minSelect" min="0" class="w-full px-4 py-2 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none" placeholder="VD: 0 hoặc 1" />
                </div>
                <div class="flex-1 space-y-1.5">
                  <label class="text-xs font-semibold text-gray-500">Tối đa chọn</label>
                  <input type="number" v-model.number="formData.maxSelect" min="1" class="w-full px-4 py-2 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none" placeholder="VD: 3" />
                </div>
              </div>
            </div>

            <!-- Link Ảnh -->
            <div class="space-y-1.5 md:col-span-2">
              <label class="text-sm font-semibold text-gray-700">Đường dẫn ảnh (URL)</label>
              <div class="flex gap-3">
                <div class="w-12 h-12 rounded-lg border border-[#EDE4D9] overflow-hidden flex-shrink-0 bg-gray-50">
                  <img 
                    v-if="formData.image" 
                    :src="formData.image" 
                    class="w-full h-full object-cover" 
                    @error="(e) => (e.target as HTMLImageElement).src = 'https://placehold.co/100x100?text=No+Image'" 
                  />
                </div>
                <input
                  v-model="formData.image"
                  type="text"
                  class="flex-1 px-4 py-3 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none"
                  placeholder="https://..."
                />
              </div>
            </div>

            <!-- Mô tả -->
            <div class="space-y-1.5 md:col-span-2">
              <label class="text-sm font-semibold text-gray-700">Mô tả ngắn gọn</label>
              <textarea
                v-model="formData.description"
                rows="3"
                class="w-full px-4 py-3 border border-[#EDE4D9] rounded-lg bg-white text-sm shadow-inner focus:outline-none resize-none"
                placeholder="Nhập mô tả..."
              ></textarea>
            </div>

            <!-- Trạng thái hiển thị -->
            <div class="space-y-1.5 md:col-span-2 flex items-center justify-between p-5 border border-[#EDE4D9] rounded-lg bg-[#FAF6F0]">
              <div>
                <label class="text-base font-semibold text-gray-800">Hiển thị trên Menu</label>
                <p class="text-xs text-gray-500">Khách hàng có thể thấy và đặt các món trong danh mục này.</p>
              </div>
              <!-- Custom Toggle -->
              <label class="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" v-model="formData.visible" class="sr-only peer">
                <div class="w-12 h-7 bg-gray-200 peer-focus:outline-none rounded-lg peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border after:rounded-lg after:h-6 after:w-6 peer-checked:bg-[#CC8033]"></div>
              </label>
            </div>
          </div>
        </div>

        <!-- Modal Footer -->
        <div class="px-6 py-5 border-t border-[#EDE4D9] bg-[#F8F4ED] flex justify-end gap-3 rounded-b-2xl">
          <button
            @click="closeModal"
            class="px-6 py-3 text-sm font-semibold text-gray-600 bg-white border border-[#EDE4D9] rounded-lg"
          >
            Hủy bỏ
          </button>
          <button
            @click="saveCat"
            class="px-6 py-3 text-sm font-semibold text-white bg-[#CC8033] rounded-lg border border-[#CC8033]/30 shadow-xl"
          >
            Lưu danh mục
          </button>
        </div>
      </div>
    </div>
    <!-- ============================================== -->
    <!-- ASSIGN PRODUCTS MODAL -->
    <!-- ============================================== -->
    <div v-if="isAssignModalOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm p-4 transition-all duration-300">
      <div class="bg-white rounded-2xl shadow-2xl w-full max-w-3xl overflow-hidden flex flex-col max-h-[90vh] ring-1 ring-black/5 animate-in fade-in zoom-in-95 duration-200">
        <div class="px-6 py-5 border-b border-[#EDE4D9] flex items-center justify-between bg-gradient-to-r from-[#FAF6F0] to-white">
          <h3 class="text-xl font-bold text-gray-800">
            Gán món vào danh mục: <span class="text-[#CC8033]">{{ currentCatForAssign?.name }}</span>
          </h3>
          <button @click="closeAssignModal" class="p-2 text-gray-400 hover:text-gray-700 rounded-md">
            <X class="w-5 h-5" />
          </button>
        </div>
        
        <div class="p-4 border-b border-[#EDE4D9] bg-[#F8F4ED]">
          <div class="relative w-full">
            <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input v-model="searchProductQuery" type="text" placeholder="Tìm kiếm món ăn..." class="w-full pl-9 pr-4 py-2 border border-[#EDE4D9] rounded-lg text-sm focus:outline-none shadow-inner bg-white" />
          </div>
        </div>

        <div class="p-6 overflow-y-auto flex-1 bg-white">
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            <label v-for="prod in filteredProductsForAssign" :key="prod.maSanPham" 
              class="flex items-center gap-3 p-3 border border-[#EDE4D9] rounded-lg cursor-pointer hover:bg-orange-50/50 transition-colors" 
              :class="{
                'bg-orange-50 border-[#CC8033]/30': selectedProductIds.includes(prod.maSanPham),
                'opacity-60 bg-gray-50': isBelongToOtherCategory(prod) && !selectedProductIds.includes(prod.maSanPham)
              }">
              <input type="checkbox" :value="prod.maSanPham" v-model="selectedProductIds" class="w-4 h-4 text-[#CC8033] accent-[#CC8033]">
              <div class="flex-1 min-w-0">
                <div class="font-bold text-gray-800 text-sm truncate">{{ prod.tenSanPham }}</div>
                <div class="flex items-center justify-between mt-0.5">
                  <span class="text-xs text-gray-500">{{ prod.giaBan.toLocaleString() }}đ</span>
                  <span v-if="isBelongToOtherCategory(prod)" class="text-[10px] bg-gray-200/80 text-gray-600 px-1.5 py-0.5 rounded truncate max-w-[90px]" :title="'Đang thuộc: ' + getCategoryNameOfProduct(prod.maDanhMuc)">
                    {{ getCategoryNameOfProduct(prod.maDanhMuc) }}
                  </span>
                </div>
              </div>
            </label>
            <div v-if="filteredProductsForAssign.length === 0" class="col-span-full py-8 text-center text-gray-400 text-sm">
              Không tìm thấy món ăn nào.
            </div>
          </div>
        </div>

        <div class="px-6 py-5 border-t border-[#EDE4D9] bg-[#F8F4ED] flex justify-end gap-3 rounded-b-2xl">
          <button @click="closeAssignModal" class="px-6 py-3 text-sm font-semibold text-gray-600 bg-white border border-[#EDE4D9] rounded-lg">Hủy bỏ</button>
          <button @click="saveAssignedProducts" class="px-6 py-3 text-sm font-semibold text-white bg-[#CC8033] rounded-lg border border-[#CC8033]/30 shadow-xl">Lưu thay đổi</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { Plus, Trash2, Edit, Search, Eye, EyeOff, LayoutGrid, Link, X, GripVertical, Coffee, ListPlus } from 'lucide-vue-next'
import { categoriesApi, type CategoryItem } from '@/services/categories'
import { useToast } from '@/stores/toast'
import { useAlert } from '@/stores/alert'

const toast = useToast()
const alert = useAlert()

interface Cat {
  id: string
  name: string
  image: string
  description: string
  order: number
  visible: boolean
  count: number
  type: string
  isTimeBased?: boolean
  startTime?: string
  endTime?: string
  minSelect?: number | null
  maxSelect?: number | null
  isSize?: boolean
}

const mapToCat = (item: CategoryItem): Cat => ({
  id: item.maDanhMuc.toString(),
  name: item.tenDanhMuc,
  image: item.hinhAnh || '',
  description: item.moTa || '',
  order: item.thuTuHienThi,
  visible: item.trangThaiHoatDong,
  count: item.soSanPham,
  type: item.loaiDanhMuc,
  isTimeBased: item.apDungKhungGio,
  startTime: item.gioBatDau ? item.gioBatDau.substring(0, 5) : '06:00',
  endTime: item.gioKetThuc ? item.gioKetThuc.substring(0, 5) : '10:00',
  minSelect: item.toiThieuChon,
  maxSelect: item.toiDaChon,
  isSize: item.laNhomKichThuoc
})

// STATE
const cats = ref<Cat[]>([])
const categoryType = ref<'MAIN' | 'TOPPING'>('MAIN')
const searchQuery = ref('')
const activeTab = ref<'all' | 'visible' | 'hidden'>('all')

// Drag & Drop
const dragIndex = ref<number | null>(null)
const dragOverIndex = ref<number | null>(null)

// Modal
const isModalOpen = ref(false)
const isEditing = ref(false)
const formData = ref<Cat>({
  id: '', name: '', image: '', description: '', order: 1, visible: true, count: 0, type: 'MAIN', isTimeBased: false, startTime: '06:00', endTime: '10:00', minSelect: null, maxSelect: null, isSize: false
})

// Assign Modal
const isAssignModalOpen = ref(false)
const currentCatForAssign = ref<Cat | null>(null)
const allProducts = ref<any[]>([])
const originalSelectedProductIds = ref<number[]>([])
const selectedProductIds = ref<number[]>([])
const searchProductQuery = ref('')

const filteredProductsForAssign = computed(() => {
  if (!searchProductQuery.value) return allProducts.value
  const q = searchProductQuery.value.toLowerCase()
  return allProducts.value.filter(p => p.tenSanPham.toLowerCase().includes(q))
})

// INIT DATA
const loadData = async () => {
  try {
    const data = await categoriesApi.list()
    cats.value = data.map(mapToCat)
  } catch (e) {
    toast.error('Lỗi khi tải danh mục')
  }
}
onMounted(loadData)

// COMPUTED
const countAll = computed(() => cats.value.length)
const countVisible = computed(() => cats.value.filter(c => c.visible).length)
const countHidden = computed(() => cats.value.filter(c => !c.visible).length)

const filteredCats = computed(() => {
  let result = cats.value.filter(c => c.categoryType === categoryType.value || (categoryType.value === 'MAIN' && !c.categoryType) || (c as any).loaiDanhMuc === categoryType.value)
 
  if (activeTab.value === 'visible') result = result.filter(c => c.visible)
  if (activeTab.value === 'hidden') result = result.filter(c => !c.visible)
 
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase()
    result = result.filter(c => c.name.toLowerCase().includes(q))
  }
 
  return result.sort((a, b) => a.order - b.order)
})

// Helper
const toSlug = (str: string) => {
  return str.toLowerCase()
    .normalize("NFD").replace(/[\u0300-\u036f]/g, "")
    .replace(/[đĐ]/g, 'd')
    .replace(/([^0-9a-z-\s])/g, '')
    .replace(/(\s+)/g, '-')
    .replace(/-+/g, '-')
    .replace(/^-+|-+$/g, '');
}

// Drag & Drop
const onDragStart = (idx: number) => { dragIndex.value = idx }
const onDragEnter = (idx: number) => {
  if (dragIndex.value !== null && dragIndex.value !== idx) dragOverIndex.value = idx
}
const onDrop = async (dropIndex: number) => {
  if (dragIndex.value !== null && dragIndex.value !== dropIndex) {
    const currentList = [...filteredCats.value]
    const [draggedItem] = currentList.splice(dragIndex.value, 1)
    currentList.splice(dropIndex, 0, draggedItem)
    
    currentList.forEach((item, newIndex) => {
      const catRef = cats.value.find(c => c.id === item.id)
      if (catRef) catRef.order = newIndex + 1
    })
    toast.success("Đã thay đổi vị trí danh mục")
    
    // Lưu thứ tự mới lên DB
    const orderRequests = currentList.map((item, index) => ({
      maDanhMuc: Number(item.id),
      thuTuHienThi: index + 1
    }))
    try {
      await categoriesApi.updateOrder(orderRequests)
    } catch (e) {
      toast.error('Lỗi khi lưu vị trí mới')
    }
  }
  onDragEnd()
}
const onDragEnd = () => {
  dragIndex.value = null
  dragOverIndex.value = null
}

// Modal methods
const openAddModal = () => {
  isEditing.value = false
  formData.value = {
    id: '',
    name: '',
    image: '',
    description: '',
    order: cats.value.length + 1,
    visible: true,
    count: 0,
    type: categoryType.value,
    isTimeBased: false,
    startTime: '06:00',
    endTime: '10:00',
    minSelect: null,
    maxSelect: null,
    isSize: false
  }
  isModalOpen.value = true
}

const openEditModal = (cat: Cat) => {
  isEditing.value = true
  formData.value = { ...cat }
  isModalOpen.value = true
}

const closeModal = () => {
  isModalOpen.value = false
}

const toggleVisibility = async (cat: Cat) => {
  try {
    await categoriesApi.update(Number(cat.id), {
      tenDanhMuc: cat.name,
      maDanhMucCha: null,
      hinhAnh: cat.image || null,
      thuTuHienThi: cat.order,
      moTa: cat.description || null,
      trangThaiHoatDong: cat.visible, // Updated by v-model
      loaiDanhMuc: cat.type,
      apDungKhungGio: cat.isTimeBased || false,
      gioBatDau: cat.startTime ? `${cat.startTime}:00` : null,
      gioKetThuc: cat.endTime ? `${cat.endTime}:00` : null,
      toiThieuChon: cat.minSelect || null,
      toiDaChon: cat.maxSelect || null,
      laNhomKichThuoc: cat.isSize || false
    })
    toast.success(`Đã ${cat.visible ? 'hiển thị' : 'ẩn'} danh mục ${cat.name}`)
  } catch (e) {
    cat.visible = !cat.visible // Revert UI if error
    toast.error('Có lỗi xảy ra, vui lòng thử lại!')
  }
}

const saveCat = async () => {
  const name = formData.value.name.trim()
  if (!name) {
    toast.error('Vui lòng nhập tên danh mục!')
    return
  }
  if (name.length > 100) {
    toast.error('Tên danh mục quá dài (tối đa 100 ký tự)!')
    return
  }

  // Duplicate name validation
  const isDuplicate = cats.value.some(c => c.name.toLowerCase() === name.toLowerCase() && c.id !== formData.value.id)
  if (isDuplicate) {
    toast.error(`Tên danh mục "${name}" đã tồn tại! Vui lòng chọn tên khác.`)
    return
  }

  if (formData.value.order === null || formData.value.order === undefined || formData.value.order < 0) {
    toast.error('Thứ tự hiển thị phải là số lớn hơn hoặc bằng 0!')
    return
  }

  if (formData.value.image) {
    const urlPattern = /^(http|https):\/\/[^ "]+$/;
    if (!urlPattern.test(formData.value.image)) {
      toast.error('Đường dẫn ảnh không hợp lệ (Phải bắt đầu bằng http:// hoặc https://)!')
      return
    }
  }

  if (formData.value.description && formData.value.description.length > 500) {
    toast.error('Mô tả quá dài (tối đa 500 ký tự)!')
    return
  }

  if (formData.value.isTimeBased) {
    if (!formData.value.startTime || !formData.value.endTime) {
      toast.error('Vui lòng nhập đầy đủ Giờ bắt đầu và Giờ kết thúc khi bật tính năng Khung giờ bán!')
      return
    }
  }

  if (formData.value.type === 'TOPPING') {
    const min = formData.value.minSelect;
    const max = formData.value.maxSelect;
    
    if (min !== null && min !== undefined && min < 0) {
      toast.error('Tối thiểu chọn phải lớn hơn hoặc bằng 0!')
      return
    }
    
    if (max !== null && max !== undefined && max <= 0) {
      toast.error('Tối đa chọn phải lớn hơn 0!')
      return
    }
    
    if (min !== null && min !== undefined && max !== null && max !== undefined) {
      if (min > max) {
        toast.error('Tối đa chọn phải lớn hơn hoặc bằng Tối thiểu chọn!')
        return
      }
    }
  }

  const payload = {
    tenDanhMuc: name,
    maDanhMucCha: null,
    hinhAnh: formData.value.image || null,
    thuTuHienThi: formData.value.order,
    moTa: formData.value.description || null,
    trangThaiHoatDong: formData.value.visible,
    loaiDanhMuc: formData.value.type,
    apDungKhungGio: formData.value.isTimeBased || false,
    gioBatDau: formData.value.isTimeBased && formData.value.startTime ? `${formData.value.startTime}:00` : null,
    gioKetThuc: formData.value.isTimeBased && formData.value.endTime ? `${formData.value.endTime}:00` : null,
    toiThieuChon: formData.value.type === 'TOPPING' ? (formData.value.minSelect || null) : null,
    toiDaChon: formData.value.type === 'TOPPING' ? (formData.value.maxSelect || null) : null,
    laNhomKichThuoc: formData.value.type === 'TOPPING' ? (formData.value.isSize || false) : false
  }

  try {
    if (isEditing.value && formData.value.id) {
      await categoriesApi.update(Number(formData.value.id), payload)
      toast.success("Cập nhật thành công")
    } else {
      await categoriesApi.create(payload)
      toast.success("Đã thêm danh mục mới")
    }
    await loadData()
    closeModal()
  } catch (e: any) {
    toast.error(e instanceof Error ? e.message : 'Có lỗi xảy ra')
  }
}

const deleteCat = async (cat: Cat) => {
  if (cat.count > 0) {
    toast.error(`Không thể xóa danh mục "${cat.name}" vì đang chứa ${cat.count} sản phẩm! Vui lòng chuyển sản phẩm sang danh mục khác trước.`);
    return;
  }

  const isConfirmed = await alert.confirm('Xóa danh mục?', `Bạn có chắc chắn muốn xóa danh mục "${cat.name}"?`)
  if(isConfirmed) {
    try {
      await categoriesApi.delete(Number(cat.id))
      toast.success("Đã xóa danh mục")
      await loadData()
    } catch (e: any) {
      toast.error(e instanceof Error ? e.message : 'Có lỗi khi xóa (Danh mục có thể đang chứa sản phẩm)')
    }
  }
}

// Assign methods
const openAssignModal = async (cat: Cat) => {
  currentCatForAssign.value = cat
  searchProductQuery.value = ''
  try {
    const products = await categoriesApi.getAllProducts()
    allProducts.value = products
    const existingIds = products.filter((p: any) => p.maDanhMuc === Number(cat.id)).map((p: any) => p.maSanPham)
    originalSelectedProductIds.value = [...existingIds]
    selectedProductIds.value = [...existingIds]
    isAssignModalOpen.value = true
  } catch (e) {
    toast.error('Lỗi khi tải danh sách món')
  }
}

const getCategoryNameOfProduct = (maDanhMuc: number | null) => {
  if (!maDanhMuc) return 'Khác';
  const cat = cats.value.find(c => Number(c.id) === maDanhMuc);
  return cat ? cat.name : 'Khác';
}

const isBelongToOtherCategory = (prod: any) => {
  if (!currentCatForAssign.value) return false;
  return prod.maDanhMuc !== null && prod.maDanhMuc !== Number(currentCatForAssign.value.id);
}

const closeAssignModal = () => {
  isAssignModalOpen.value = false
  currentCatForAssign.value = null
}

const getCategoryGroup = (name: string) => {
  const n = name.toLowerCase();
  if (n.includes('cà phê') || n.includes('cafe') || n.includes('coffee')) return 'CAFE';
  if (n.includes('trà') || n.includes('tea')) return 'TEA';
  if (n.includes('sinh tố') || n.includes('nước ép') || n.includes('ép') || n.includes('juice')) return 'JUICE';
  if (n.includes('bánh') || n.includes('cake')) return 'CAKE';
  if (n.includes('topping') || n.includes('thêm') || n.includes('trân châu') || n.includes('thạch')) return 'TOPPING';
  return 'OTHER';
}

const getProductGroup = (name: string) => {
  const n = name.toLowerCase();
  if (n.includes('trà') || n.includes('tea') || n.includes('matcha') || n.includes('hồng trà') || n.includes('lục trà')) return 'TEA';
  if (n.includes('cà phê') || n.includes('cafe') || n.includes('coffee') || n.includes('bạc xỉu') || n.includes('espresso') || n.includes('nâu') || n.includes('đen') || n.includes('cappuccino')) return 'CAFE';
  if (n.includes('sinh tố') || n.includes('nước ép') || n.includes('ép') || n.includes('cam') || n.includes('dâu') || n.includes('thơm') || n.includes('cóc')) return 'JUICE';
  if (n.includes('bánh') || n.includes('cake') || n.includes('tiramisu')) return 'CAKE';
  if (n.includes('trân châu') || n.includes('thạch') || n.includes('pudding') || n.includes('hạt')) return 'TOPPING';
  return 'OTHER';
}

const saveAssignedProducts = async () => {
  if (!currentCatForAssign.value) return
  
  const catName = currentCatForAssign.value.name
  const catGroup = getCategoryGroup(catName)
  
  if (catGroup !== 'OTHER') {
    const newlyAddedIds = selectedProductIds.value.filter(id => !originalSelectedProductIds.value.includes(id))
    const newlyAddedProducts = allProducts.value.filter(p => newlyAddedIds.includes(p.maSanPham))
    
    const conflictingProducts = newlyAddedProducts.filter(p => {
      const pGroup = getProductGroup(p.tenSanPham)
      return pGroup !== 'OTHER' && pGroup !== catGroup
    })
    
    if (conflictingProducts.length > 0) {
      const pNames = conflictingProducts.map(p => `- ${p.tenSanPham}`).join('\n')
      const confirmMsg = `Phát hiện các món có thể không thuộc danh mục này:\n\n${pNames}\n\nBạn có chắc chắn muốn gán những món này vào danh mục "${catName}" không?`
      const isConfirmed = await alert.confirm('Cảnh báo không khớp!', confirmMsg, 'Vẫn lưu', 'Hủy bỏ')
      if (!isConfirmed) {
        return // User canceled
      }
    }
  }

  try {
    await categoriesApi.assignProducts(Number(currentCatForAssign.value.id), selectedProductIds.value)
    toast.success("Đã gán món thành công")
    await loadData()
    closeAssignModal()
  } catch (e) {
    toast.error('Lỗi khi gán món')
  }
}
</script>