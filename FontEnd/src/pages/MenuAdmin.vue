<template>
  <div class="space-y-6 p-6 bg-[#FDFBF7] min-h-screen text-[#2A231E] font-premium-sans">
    <!-- Header Page Title -->
    <div class="flex flex-col gap-1.5 border-b border-[#EAE3D9] pb-4">
      <h1 class="text-3xl font-bold font-premium-serif text-espresso">Thực đơn của quán</h1>
      <p class="text-xs text-[#8A8178] font-medium">Quản lý danh sách sản phẩm, giá bán, và cấu hình kích cỡ size phục vụ.</p>
    </div>

    <!-- Summary Stats Row -->
    <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
      <div class="bg-white p-4.5 rounded-2xl border border-[#EAE3D9] shadow-soft flex items-center gap-4 hover:shadow-md transition-all duration-200">
        <div class="w-12 h-12 rounded-xl bg-[#FFF9F2] text-[#CC8033] flex items-center justify-center shadow-inner shrink-0">
          <Coffee class="w-6 h-6" />
        </div>
        <div>
          <p class="text-[9px] text-[#8A8178] font-bold uppercase tracking-widest">Tổng thực đơn</p>
          <p class="text-2xl font-black text-espresso mt-0.5 leading-none">{{ items.length }} <span class="text-xs font-semibold text-[#8A8178]">món</span></p>
        </div>
      </div>
      <div class="bg-white p-4.5 rounded-2xl border border-[#EAE3D9] shadow-soft flex items-center gap-4 hover:shadow-md transition-all duration-200">
        <div class="w-12 h-12 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center shadow-inner shrink-0">
          <Flame class="w-6 h-6 fill-amber-600" />
        </div>
        <div>
          <p class="text-[9px] text-[#8A8178] font-bold uppercase tracking-widest">Món bán chạy</p>
          <p class="text-2xl font-black text-espresso mt-0.5 leading-none">{{ featuredCount }} <span class="text-xs font-semibold text-[#8A8178]">món nổi bật</span></p>
        </div>
      </div>
      <div class="bg-white p-4.5 rounded-2xl border border-[#EAE3D9] shadow-soft flex items-center gap-4 hover:shadow-md transition-all duration-200">
        <div class="w-12 h-12 rounded-xl bg-red-50 text-red-500 flex items-center justify-center shadow-inner shrink-0">
          <ShieldAlert class="w-6 h-6" />
        </div>
        <div>
          <p class="text-[9px] text-[#8A8178] font-bold uppercase tracking-widest">Tạm ngưng bán</p>
          <p class="text-2xl font-black text-espresso mt-0.5 leading-none">{{ inactiveCount }} <span class="text-xs font-semibold text-[#8A8178]">món</span></p>
        </div>
      </div>
    </div>

    <!-- Header: Search + Button -->
    <div class="flex flex-wrap items-center gap-4 justify-between bg-white p-4 rounded-2xl border border-[#EAE3D9] shadow-soft">
      <div class="relative flex-1 min-w-[260px] max-w-md">
        <Search class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Tìm tên món nhanh..."
          v-model="search"
          class="pl-10 bg-card border border-[#EAE3D9] shadow-inner h-11 rounded-xl text-sm focus-visible:ring-1 focus-visible:ring-[#CC8033]"
        />
      </div>
      <Button @click="openNew" class="bg-gradient-to-r from-[#CC8033] to-[#A6611F] text-white rounded-xl shadow-md shadow-[#CC8033]/20 px-6 py-3 font-bold flex items-center gap-2 transition-all hover:-translate-y-0.5">
        <Plus class="w-4 h-4" stroke-width="2.5" /> Thêm món mới
      </Button>
    </div>

    <!-- Filter categories -->
    <div class="flex gap-2 overflow-x-auto pb-1 scrollbar-hide">
      <button
        @click="selectedCategoryId = 'all'"
        :class="[
          'px-5 py-2.5 rounded-full text-[10px] font-bold uppercase tracking-wider whitespace-nowrap border shadow-sm transition-all duration-200',
          selectedCategoryId === 'all'
            ? 'bg-espresso text-white border-espresso shadow-md'
            : 'bg-white text-[#5C544E] border-[#EAE3D9] hover:border-caramel/50 hover:bg-[#FDFBF7]'
        ]"
      >
        Tất cả
      </button>
      <button
        v-for="c in categories"
        :key="c.maDanhMuc"
        @click="selectedCategoryId = c.maDanhMuc"
        :class="[
          'px-5 py-2.5 rounded-full text-[10px] font-bold uppercase tracking-wider whitespace-nowrap border shadow-sm transition-all duration-200',
          selectedCategoryId === c.maDanhMuc
            ? 'bg-espresso text-white border-espresso shadow-md'
            : 'bg-white text-[#5C544E] border-[#EAE3D9] hover:border-caramel/50 hover:bg-[#FDFBF7]'
        ]"
      >
        {{ c.tenDanhMuc }}
      </button>
    </div>

    <!-- Loading screen -->
    <div v-if="loading" class="text-center py-24 text-muted-foreground flex flex-col items-center bg-white rounded-3xl border border-[#EAE3D9] shadow-soft">
      <Coffee class="w-16 h-16 mx-auto mb-4 animate-bounce text-[#CC8033]" />
      <p class="text-base font-bold text-espresso">Đang tải thực đơn...</p>
      <p class="text-xs text-[#8A8178] mt-1">Xin vui lòng đợi trong giây lát.</p>
    </div>

    <!-- Error screen -->
    <div v-else-if="errorMsg" class="text-center py-20 text-red-500 bg-red-50 border border-red-200 rounded-3xl p-8 max-w-xl mx-auto shadow-sm">
      <ShieldAlert class="w-16 h-16 mx-auto mb-4" />
      <p class="text-lg font-bold">Không thể tải dữ liệu thực đơn</p>
      <p class="text-sm mt-1.5 text-red-600/80">{{ errorMsg }}</p>
      <Button @click="loadData" class="mt-5 bg-espresso text-white px-6 rounded-xl">Thử lại</Button>
    </div>

    <!-- Products list -->
    <div v-else>
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <article
          v-for="m in paginatedItems"
          :key="m.maSanPham"
          :class="[!m.trangThaiBan ? 'opacity-75' : '']"
          class="group bg-white rounded-3xl border border-[#EAE3D9] shadow-soft overflow-hidden flex flex-col hover:shadow-warm hover:-translate-y-1.5 hover:border-[#CC8033]/30 transition-all duration-300"
        >
          <!-- Image area with hover effect and badges -->
          <div class="relative aspect-[4/3] bg-[#F5F2ED] overflow-hidden shrink-0">
            <img
              v-if="m.hinhAnh"
              :src="m.hinhAnh"
              :alt="m.tenSanPham"
              loading="lazy"
              class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
              @error="(e) => (e.target as HTMLImageElement).src = 'https://placehold.co/300x200?text=No+Image'"
            />
            <div v-else class="w-full h-full flex items-center justify-center text-[#C5BEB8] bg-[#F5F2ED]">
              <Coffee class="w-12 h-12" />
            </div>

            <!-- Glassmorphic overlays -->
            <div class="absolute inset-0 bg-gradient-to-t from-black/20 via-transparent to-black/10 pointer-events-none"></div>

            <!-- Badges -->
            <div class="absolute top-3.5 left-3.5 flex flex-col gap-1.5">
              <span
                v-if="m.laMonNoiBat"
                class="px-2.5 py-1 rounded-lg bg-[#CC8033]/90 backdrop-blur-sm text-white text-[9px] font-bold flex items-center gap-1 shadow-md uppercase tracking-wider"
              >
                <Flame class="w-3 h-3 fill-white" /> Bán chạy
              </span>
              <span
                v-if="!m.trangThaiBan"
                class="px-2.5 py-1 rounded-lg bg-[#8A8178]/95 backdrop-blur-sm text-white text-[9px] font-bold flex items-center gap-1 shadow-md uppercase tracking-wider"
              >
                Tạm ngưng
              </span>
            </div>

            <div class="absolute bottom-3 left-3.5">
              <span class="inline-flex items-center gap-1 text-[9px] font-bold uppercase tracking-wider px-2 py-1 rounded-lg bg-black/40 backdrop-blur-md text-white">
                <Coffee class="w-2.5 h-2.5" />
                {{ m.kieuMon === 'Topping' ? 'Topping' : (m.kieuMon === 'MonKem' ? 'Món kèm' : 'Món chính') }}
              </span>
            </div>

            <!-- Sleek actions bar (glassmorphic circles) -->
            <div class="absolute top-3.5 right-3.5 flex gap-2">
              <button
                @click="openEdit(m)"
                class="w-8.5 h-8.5 rounded-full bg-white/80 backdrop-blur-md border border-[#EAE3D9] shadow-md flex items-center justify-center text-espresso hover:bg-caramel hover:text-white transition-all duration-200"
                title="Sửa món"
              >
                <Edit3 class="w-3.5 h-3.5" />
              </button>
              <button
                @click="remove(m.maSanPham)"
                class="w-8.5 h-8.5 rounded-full bg-white/80 backdrop-blur-md border border-[#EAE3D9] shadow-md flex items-center justify-center text-red-500 hover:bg-red-500 hover:text-white transition-all duration-200"
                title="Xóa món"
              >
                <Trash2 class="w-3.5 h-3.5" />
              </button>
            </div>
          </div>

          <!-- Content Details -->
          <div class="p-5 flex-1 flex flex-col bg-white">
            <h3 class="font-bold text-espresso text-base leading-tight truncate font-premium-sans" :title="m.tenSanPham">
              {{ m.tenSanPham }}
            </h3>
            
            <p class="text-xs text-[#8A8178] mt-1.5 font-medium flex items-center gap-1">
              <span class="inline-block w-1.5 h-1.5 rounded-full bg-caramel/70"></span>
              {{ m.tenDanhMuc || 'Chưa phân danh mục' }}
            </p>

            <div class="flex items-center justify-between mt-5 pt-3.5 border-t border-[#F5F2ED]">
              <div>
                <p class="text-[8px] uppercase tracking-widest text-[#8A8178] font-bold">Giá bán tại quầy</p>
                <span class="text-lg font-bold text-[#CC8033] leading-none font-premium-serif">{{ formatVND(m.giaBan) }}</span>
              </div>
            </div>
          </div>
        </article>
      </div>

      <!-- Empty state -->
      <div
        v-if="paginatedItems.length === 0"
        class="text-center py-20 text-muted-foreground flex flex-col items-center bg-white rounded-3xl border border-[#EAE3D9] mt-6 shadow-soft"
      >
        <Coffee class="w-16 h-16 mx-auto mb-4 opacity-30 text-[#8A8178]" />
        <p class="text-base font-bold text-espresso">Không có món nào hiển thị</p>
        <p class="text-xs text-[#8A8178] mt-1">Thay đổi từ khóa tìm kiếm hoặc chọn bộ lọc danh mục khác.</p>
      </div>

      <!-- Pagination -->
      <div v-if="filteredItems.length > 0" class="flex items-center justify-between py-4 border-t border-[#EAE3D9] mt-8">
        <div class="text-[10px] text-[#8A8178] font-bold uppercase tracking-wider">
          Đang xem <span class="text-espresso font-black">{{ (currentPage - 1) * itemsPerPage + 1 }}</span> - <span class="text-espresso font-black">{{ Math.min(currentPage * itemsPerPage, filteredItems.length) }}</span> / <span class="text-espresso font-black">{{ filteredItems.length }}</span> món ăn
        </div>
        <div class="flex items-center gap-2">
          <Button 
            variant="outline"
            size="icon"
            @click="currentPage--" 
            :disabled="currentPage === 1"
            class="h-9 w-9 rounded-xl border-cream-deep disabled:opacity-50"
          >
            <ChevronLeft class="w-4 h-4" />
          </Button>
          <span class="text-xs font-bold text-espresso px-2">
            Trang {{ currentPage }} / {{ totalPages }}
          </span>
          <Button 
            variant="outline"
            size="icon"
            @click="currentPage++" 
            :disabled="currentPage === totalPages"
            class="h-9 w-9 rounded-xl border-cream-deep disabled:opacity-50"
          >
            <ChevronRight class="w-4 h-4" />
          </Button>
        </div>
      </div>
    </div>

    <!-- Edit/Add Modal (Redesigned with beautiful tabs) -->
    <Modal v-model="isModalOpen">
      <template #header>
        <h2 class="font-premium-serif text-2xl font-bold text-espresso leading-tight">
          {{ editing && editing.maSanPham > 0 ? "Chỉnh sửa món ăn" : "Thêm món mới" }}
        </h2>
      </template>

      <!-- Tabs Navigation -->
      <div class="flex border-b border-[#EAE3D9] mb-4 text-xs font-bold uppercase tracking-wider">
        <button
          type="button"
          @click="activeTab = 'basic'"
          :class="activeTab === 'basic' ? 'border-[#CC8033] text-[#CC8033] font-black' : 'border-transparent text-[#8A8178] hover:text-espresso'"
          class="flex-1 py-3.5 border-b-2 text-center transition-all focus:outline-none"
        >
          1. Thông tin cơ bản
        </button>
        <button
          type="button"
          @click="activeTab = 'advanced'"
          :class="activeTab === 'advanced' ? 'border-[#CC8033] text-[#CC8033] font-black' : 'border-transparent text-[#8A8178] hover:text-espresso'"
          class="flex-1 py-3.5 border-b-2 text-center transition-all focus:outline-none"
        >
          2. Kích cỡ & Hình ảnh
        </button>
      </div>

      <div v-if="editing" class="space-y-4 max-h-[60vh] overflow-y-auto pr-1 text-espresso">
        <!-- TAB 1: BASIC INFO -->
        <div v-if="activeTab === 'basic'" class="space-y-4">
          <!-- Row 1: Name and Type -->
          <div class="grid grid-cols-2 gap-4">
            <div class="space-y-1.5">
              <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Tên món ăn <span class="text-red-500">*</span></Label>
              <Input v-model="editing.tenSanPham" class="bg-background border border-cream-deep rounded-xl shadow-inner h-10" placeholder="Vd: Cappuccino, Trà đào..." />
            </div>
            <div class="space-y-1.5">
              <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Kiểu món</Label>
              <select v-model="editing.kieuMon" class="flex h-10 w-full rounded-xl border border-cream-deep bg-background px-3 text-xs shadow-inner focus-visible:outline-none font-medium">
                <option value="MonChinh">Món chính (Đồ uống / Món ăn)</option>
                <option value="Topping">Topping thêm (Thạch, trân châu...)</option>
                <option value="MonKem">Món kèm</option>
              </select>
            </div>
          </div>

          <!-- Row 2: Category and SKU -->
          <div class="grid grid-cols-2 gap-4">
            <div class="space-y-1.5">
              <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Danh mục thực đơn</Label>
              <select v-model="editing.maDanhMuc" class="flex h-10 w-full rounded-xl border border-cream-deep bg-background px-3 text-xs shadow-inner focus-visible:outline-none font-medium">
                <option :value="null">-- Không chọn --</option>
                <option v-for="c in categories" :key="c.maDanhMuc" :value="c.maDanhMuc">{{ c.tenDanhMuc }}</option>
              </select>
            </div>
            <div class="space-y-1.5">
              <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Mã vạch / SKU</Label>
              <Input v-model="editing.maVach_SKU" class="bg-background border border-cream-deep rounded-xl shadow-inner h-10" placeholder="Vd: CF-CAP-01" />
            </div>
          </div>

          <!-- Row 3: Prices -->
          <div class="space-y-1.5">
            <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Giá bán (VND) <span class="text-red-500">*</span></Label>
            <Input type="number" v-model.number="editing.giaBan" class="bg-background border border-cream-deep rounded-xl shadow-inner h-10 text-sm font-bold text-caramel" />
          </div>

          <!-- Row 5: Switches -->
          <div class="grid grid-cols-2 gap-4 pt-2">
            <div class="flex items-center justify-between p-3 rounded-xl bg-background border border-cream-deep shadow-inner">
              <div class="pr-2">
                <Label class="text-espresso font-bold text-xs leading-none">Món bán chạy</Label>
                <p class="text-[9px] text-[#8A8178] mt-1">Hiển thị badge HOT cho khách</p>
              </div>
              <Switch v-model="editing.laMonNoiBat" />
            </div>
            <div class="flex items-center justify-between p-3 rounded-xl bg-background border border-cream-deep shadow-inner">
              <div class="pr-2">
                <Label class="text-espresso font-bold text-xs leading-none">Đang bán</Label>
                <p class="text-[9px] text-[#8A8178] mt-1">Khách có thể đặt trên menu</p>
              </div>
              <Switch v-model="editing.trangThaiBan" />
            </div>
          </div>
        </div>

        <!-- TAB 2: SIZE & MEDIA CONFIG -->
        <div v-if="activeTab === 'advanced'" class="space-y-4">
          <!-- Image URL with preview -->
          <div class="space-y-1.5 bg-white p-3 rounded-2xl border border-cream-deep shadow-sm">
            <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Hình ảnh món ăn</Label>
            <div class="flex gap-4 mt-1">
              <div class="w-24 h-24 rounded-2xl border border-cream-deep bg-[#F9F8F6] overflow-hidden shrink-0 flex items-center justify-center shadow-inner">
                <img v-if="editing.hinhAnh" :src="editing.hinhAnh" alt="preview" class="w-full h-full object-cover" @error="(e) => (e.target as HTMLImageElement).src = 'https://placehold.co/100x100?text=No+Image'" />
                <ImageIcon v-else class="w-8 h-8 text-muted-foreground/30" />
              </div>
              <div class="flex-1 space-y-2.5 min-w-0">
                <div class="flex gap-2">
                  <Input v-model="editing.hinhAnh" placeholder="Dán link ảnh (https://...)" class="flex-1 bg-background border border-cream-deep rounded-xl shadow-inner h-9 text-xs" />
                  <Button type="button" @click="triggerUpload" size="sm" class="bg-espresso hover:bg-espresso-soft text-white text-xs px-3 rounded-xl flex items-center gap-1 h-9 shrink-0 font-bold">
                    <Upload class="w-3.5 h-3.5" /> Tải ảnh
                  </Button>
                </div>
                <input ref="fileInput" type="file" accept="image/*" class="hidden" @change="onFileChange" />
                <p class="text-[10px] text-[#8A8178] leading-tight">Dán địa chỉ URL hình ảnh hoặc nhấn nút "Tải ảnh" để chọn tệp từ máy tính của bạn.</p>
              </div>
            </div>
          </div>

          <!-- Description -->
          <div class="space-y-1.5">
            <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Mô tả ngắn về hương vị / nguyên liệu</Label>
            <Textarea v-model="editing.moTa" class="bg-background border border-[#EAE3D9] rounded-xl shadow-inner min-h-[60px] text-xs" placeholder="Vd: Chiết xuất từ hạt Arabica đậm vị hạt dẻ, sô cô la thơm nồng..." />
          </div>

          <!-- SIZES CONTAINER (Chỉ cho Món chính / Món kèm) -->
          <div v-if="editing.kieuMon !== 'Topping'" class="space-y-3 pt-3 border-t border-[#EAE3D9]">
            <div class="flex items-center justify-between">
              <div>
                <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Cấu hình kích cỡ (Sizes)</Label>
                <p class="text-[9px] text-[#8A8178] mt-0.5">Đặt mức giá chênh lệch tùy theo dung tích ly phục vụ.</p>
              </div>
              <Button type="button" @click="addSize" size="sm" class="bg-espresso hover:bg-espresso-soft text-white text-xs px-3 rounded-lg flex items-center gap-1">
                <Plus class="w-3.5 h-3.5" /> Thêm size
              </Button>
            </div>

            <div v-if="!editing.kichCos || editing.kichCos.length === 0" class="text-center py-5 bg-[#FDFBF7] border border-dashed border-[#EAE3D9] rounded-2xl text-xs text-[#8A8178]">
              Chưa có cấu hình size phụ. Món này sẽ sử dụng giá bán mặc định.
            </div>

            <div v-else class="space-y-2 max-h-[220px] overflow-y-auto pr-1">
              <!-- List items -->
              <div v-for="(size, index) in editing.kichCos" :key="index" class="flex gap-3 items-center bg-white p-2.5 rounded-xl border border-[#EAE3D9] shadow-sm">
                <div class="w-1/3">
                  <p class="text-[8px] uppercase tracking-wider text-[#8A8178] font-bold mb-1 pl-1">Tên size</p>
                  <Input v-model="size.tenKichCo" placeholder="Vd: Size M, Size L" class="bg-background border border-cream-deep rounded-lg h-9 text-xs" />
                </div>
                <div class="w-1/3">
                  <p class="text-[8px] uppercase tracking-wider text-[#8A8178] font-bold mb-1 pl-1">Giá phụ thu</p>
                  <Input type="number" v-model.number="size.giaCongThem" class="bg-background border border-cream-deep rounded-lg h-9 text-xs font-bold text-caramel" />
                </div>
                <div class="flex items-center gap-1.5 w-1/4 justify-center pt-5">
                  <Switch v-model="size.trangThaiHoatDong" />
                  <span class="text-[9px] font-bold uppercase tracking-wider text-[#8A8178]">{{ size.trangThaiHoatDong ? 'Bật' : 'Tắt' }}</span>
                </div>
                <button type="button" @click="removeSize(index)" class="w-9 h-9 rounded-xl bg-red-50 text-red-500 hover:bg-red-500 hover:text-white flex items-center justify-center transition-colors shrink-0 mt-5">
                  <X class="w-4 h-4" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <div class="flex justify-between items-center w-full mt-2">
          <!-- Button chuyển Tab nhanh bằng nút phụ -->
          <div>
            <button
              v-if="activeTab === 'basic'"
              type="button"
              @click="activeTab = 'advanced'"
              class="text-xs font-bold text-caramel hover:underline"
            >
              Tiếp tục cấu hình Size & Mô tả →
            </button>
            <button
              v-if="activeTab === 'advanced'"
              type="button"
              @click="activeTab = 'basic'"
              class="text-xs font-bold text-[#8A8178] hover:underline"
            >
              ← Quay lại thông tin cơ bản
            </button>
          </div>
          <div class="flex gap-2">
            <Button variant="outline" @click="isModalOpen = false" :disabled="isSaving" class="border border-[#EAE3D9] rounded-xl text-xs font-bold uppercase tracking-wider px-5 h-11">Hủy</Button>
            <Button @click="save" :disabled="isSaving" class="bg-gradient-to-r from-[#CC8033] to-[#A6611F] text-white rounded-xl shadow-md px-6 py-3 text-xs font-bold uppercase tracking-wider transition-all">
              {{ isSaving ? 'Đang lưu...' : 'Lưu món ăn' }}
            </Button>
          </div>
        </div>
      </template>
    </Modal>
    <!-- Beautiful Toast Notification -->
    <Transition name="toast">
      <div v-if="toastState.show" class="fixed bottom-6 right-6 z-[100] flex items-center gap-3 px-5 py-3.5 rounded-2xl shadow-warm border text-white transition-all duration-300"
        :class="{
          'bg-emerald-600 border-emerald-500': toastState.type === 'success',
          'bg-red-600 border-red-500': toastState.type === 'error',
          'bg-amber-600 border-amber-500': toastState.type === 'warning'
        }">
        <CheckCircle v-slot:default v-if="toastState.type === 'success'" class="w-5 h-5 text-white shrink-0" stroke-width="2.5" />
        <ShieldAlert v-else class="w-5 h-5 text-white shrink-0" stroke-width="2.5" />
        <div>
          <p class="text-[10px] font-bold uppercase tracking-wider leading-none mb-0.5">
            {{ toastState.type === 'success' ? 'Thành công' : (toastState.type === 'error' ? 'Lỗi hệ thống' : 'Cảnh báo') }}
          </p>
          <p class="text-xs font-semibold text-white/95">{{ toastState.message }}</p>
        </div>
        <button @click="toastState.show = false" class="text-white/70 hover:text-white ml-2">
          <X class="w-3.5 h-3.5" />
        </button>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { Plus, Search, Edit3, Trash2, Flame, Coffee, ChevronLeft, ChevronRight, Image as ImageIcon, X, ShieldAlert, CheckCircle, Upload } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Label from '@/components/ui/Label.vue'
import Switch from '@/components/ui/Switch.vue'
import Textarea from '@/components/ui/Textarea.vue'
import Modal from '@/components/ui/Modal.vue'
import { productsApi, type ProductListItem, type ProductDetail, type SizeDto, type CategoryItem } from '@/services/products'

const formatVND = (n: number) => (n || 0).toLocaleString('vi-VN') + 'đ'

// Toast Notification State
const toastState = ref<{ show: boolean; message: string; type: 'success' | 'error' | 'warning' }>({
  show: false,
  message: '',
  type: 'success'
})

const showToast = (message: string, type: 'success' | 'error' | 'warning' = 'success') => {
  toastState.value = { show: true, message, type }
  setTimeout(() => {
    toastState.value.show = false
  }, 3000)
}

// File Upload Ref & Handlers
const fileInput = ref<HTMLInputElement | null>(null)
const triggerUpload = () => fileInput.value?.click()

const onFileChange = (e: Event) => {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (!file || !editing.value) return
  if (!file.type.startsWith('image/')) {
    showToast('Vui lòng chọn một tệp ảnh hợp lệ', 'warning')
    return
  }
  const reader = new FileReader()
  reader.onload = () => {
    if (editing.value) {
      editing.value.hinhAnh = reader.result as string
    }
  }
  reader.readAsDataURL(file)
  if (e.target) {
    (e.target as HTMLInputElement).value = ''
  }
}

// State
const items = ref<ProductListItem[]>([])
const categories = ref<CategoryItem[]>([])
const search = ref("")
const selectedCategoryId = ref<number | "all">("all")
const loading = ref(false)
const errorMsg = ref("")

// Modal & Form State
const editing = ref<ProductDetail | null>(null)
const isModalOpen = ref(false)
const isSaving = ref(false)
const activeTab = ref<'basic' | 'advanced'>('basic')

const featuredCount = computed(() => items.value.filter(i => i.laMonNoiBat).length)
const inactiveCount = computed(() => items.value.filter(i => !i.trangThaiBan).length)

const loadData = async () => {
  loading.value = true
  errorMsg.value = ""
  try {
    const [pList, cList] = await Promise.all([
      productsApi.list(),
      productsApi.listCategories()
    ])
    items.value = pList
    categories.value = cList.filter(c => c.trangThaiHoatDong)
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Có lỗi xảy ra khi tải thực đơn.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})

const currentPage = ref(1)
const itemsPerPage = ref(8)

const filteredItems = computed(() => {
  return items.value.filter(m =>
    (selectedCategoryId.value === "all" || m.maDanhMuc === selectedCategoryId.value) &&
    m.tenSanPham.toLowerCase().includes(search.value.toLowerCase())
  )
})

const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage.value) || 1)

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredItems.value.slice(start, start + itemsPerPage.value)
})

watch([search, selectedCategoryId], () => {
  currentPage.value = 1
})

const openNew = () => {
  activeTab.value = 'basic'
  editing.value = {
    maSanPham: 0,
    tenSanPham: "",
    maDanhMuc: categories.value[0] ? categories.value[0].maDanhMuc : null,
    maVach_SKU: undefined,
    giaBan: 0,
    giaVonDuKien: undefined,
    hinhAnh: undefined,
    moTa: undefined,
    luongCalo: undefined,
    thoiGianChuanBiPhut: undefined,
    laMonNoiBat: false,
    kieuMon: "MonChinh",
    trangThaiBan: true,
    kichCos: []
  }
  isModalOpen.value = true
}

const openEdit = async (m: ProductListItem) => {
  activeTab.value = 'basic'
  try {
    const detail = await productsApi.get(m.maSanPham)
    editing.value = {
      ...detail,
      maVach_SKU: detail.maVach_SKU ?? undefined,
      giaVonDuKien: detail.giaVonDuKien ?? undefined,
      hinhAnh: detail.hinhAnh ?? undefined,
      moTa: detail.moTa ?? undefined,
      luongCalo: detail.luongCalo ?? undefined,
      thoiGianChuanBiPhut: detail.thoiGianChuanBiPhut ?? undefined,
      kichCos: detail.kichCos || []
    }
    isModalOpen.value = true
  } catch (e) {
    showToast(e instanceof Error ? e.message : 'Không tải được chi tiết món ăn.', 'error')
  }
}

const addSize = () => {
  if (!editing.value) return
  if (!editing.value.kichCos) {
    editing.value.kichCos = []
  }
  editing.value.kichCos.push({
    tenKichCo: "",
    giaCongThem: 0,
    trangThaiHoatDong: true
  })
}

const removeSize = (index: number) => {
  if (!editing.value) return
  editing.value.kichCos.splice(index, 1)
}

const save = async () => {
  if (!editing.value || !editing.value.tenSanPham.trim()) {
    showToast("Vui lòng nhập tên món ăn", "warning")
    return
  }
  if (editing.value.giaBan < 0) {
    showToast("Giá bán không được nhỏ hơn 0", "warning")
    return
  }

  // Validate size inputs
  if (editing.value.kichCos && editing.value.kichCos.length > 0) {
    for (const size of editing.value.kichCos) {
      if (!size.tenKichCo.trim()) {
        showToast("Tên kích cỡ không được để trống", "warning")
        return
      }
      if (size.giaCongThem < 0) {
        showToast("Giá cộng thêm của kích cỡ không được nhỏ hơn 0", "warning")
        return
      }
    }
  }

  isSaving.value = true
  const isEditMode = editing.value.maSanPham > 0
  try {
    const payload = {
      tenSanPham: editing.value.tenSanPham.trim(),
      maDanhMuc: editing.value.maDanhMuc,
      maVach_SKU: editing.value.maVach_SKU?.trim() || null,
      giaBan: editing.value.giaBan,
      giaVonDuKien: editing.value.giaVonDuKien || 0,
      hinhAnh: editing.value.hinhAnh?.trim() || null,
      moTa: editing.value.moTa?.trim() || null,
      luongCalo: editing.value.luongCalo || null,
      thoiGianChuanBiPhut: editing.value.thoiGianChuanBiPhut || null,
      laMonNoiBat: editing.value.laMonNoiBat,
      kieuMon: editing.value.kieuMon,
      trangThaiBan: editing.value.trangThaiBan,
      kichCos: editing.value.kichCos || []
    }

    if (isEditMode) {
      await productsApi.update(editing.value.maSanPham, payload)
      showToast("Cập nhật thông tin món thành công!", "success")
    } else {
      await productsApi.create(payload)
      showToast("Thêm món mới vào thực đơn thành công!", "success")
    }
    
    isModalOpen.value = false
    await loadData()
  } catch (e) {
    showToast(e instanceof Error ? e.message : 'Có lỗi xảy ra khi lưu món ăn.', "error")
  } finally {
    isSaving.value = false
  }
}

const remove = async (id: number) => {
  if (!confirm("Bạn có chắc chắn muốn xóa món này không?")) return
  try {
    await productsApi.delete(id)
    showToast("Đã xóa món ăn khỏi thực đơn!", "success")
    await loadData()
  } catch (e) {
    showToast(e instanceof Error ? e.message : 'Có lỗi xảy ra khi xóa món ăn.', "error")
  }
}
</script>

<style scoped>
.scrollbar-hide::-webkit-scrollbar { display: none; }
.scrollbar-hide { -ms-overflow-style: none; scrollbar-width: none; }
.toast-enter-active, .toast-leave-active { transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1); }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(12px) scale(0.95); }
</style>
