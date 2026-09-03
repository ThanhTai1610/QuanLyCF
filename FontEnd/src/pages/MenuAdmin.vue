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
      <div class="flex items-center gap-2">
        <Button @click="openBulkPointsModal" class="bg-[#CC8033]/10 text-[#CC8033] hover:bg-[#CC8033] hover:text-white border border-[#CC8033]/30 rounded-xl px-4 py-3 font-bold flex items-center gap-2 transition-all">
          <Gift class="w-4 h-4" /> Cài điểm tích lũy hàng loạt
        </Button>
        <Button @click="openNew" class="bg-gradient-to-r from-[#CC8033] to-[#A6611F] text-white rounded-xl shadow-md shadow-[#CC8033]/20 px-6 py-3 font-bold flex items-center gap-2 transition-all hover:-translate-y-0.5">
          <Plus class="w-4 h-4" stroke-width="2.5" /> Thêm món mới
        </Button>
      </div>
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
            <div class="absolute top-3.5 left-3.5 flex flex-col gap-1.5 z-10">
              <span
                v-if="m.laMonNoiBat"
                class="px-2.5 py-1 rounded-lg bg-[#CC8033]/90 backdrop-blur-sm text-white text-[9px] font-bold flex items-center gap-1 shadow-md uppercase tracking-wider"
              >
                <Flame class="w-3 h-3 fill-white" /> Bán chạy
              </span>
              <span
                v-if="isKitchenOutOfStock(m.tenSanPham)"
                class="px-2.5 py-1 rounded-lg bg-red-600/90 backdrop-blur-sm text-white text-[9px] font-bold flex items-center gap-1 shadow-md uppercase tracking-wider border border-red-500 animate-pulse"
              >
                <ShieldAlert class="w-3 h-3" /> Tạm hết (Bếp báo)
              </span>
              <span
                v-else-if="!m.trangThaiBan"
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
              <div v-if="m.diemTichLuy && m.diemTichLuy > 0" class="text-right">
                <p class="text-[8px] uppercase tracking-widest text-[#8A8178] font-bold">Tích điểm</p>
                <span class="text-xs font-bold text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-md border border-emerald-200">+{{ m.diemTichLuy }}đ</span>
              </div>
            </div>

            <!-- Nút Mở Bán Lại Nhanh nếu đang Tạm Hết -->
            <div v-if="isKitchenOutOfStock(m.tenSanPham) || !m.trangThaiBan" class="mt-3 pt-2.5 border-t border-[#F5F2ED]">
              <button
                @click="reopenProduct(m)"
                class="w-full py-2 px-3 rounded-xl bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-500 hover:to-teal-500 text-white text-[10px] font-bold uppercase tracking-wider flex items-center justify-center gap-1.5 shadow-sm transition-all cursor-pointer"
              >
                <Sparkles class="w-3.5 h-3.5" /> Mở bán lại (Bổ sung NL)
              </button>
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

          <!-- Row 3: Prices & Loyalty Points -->
          <div class="grid grid-cols-2 gap-4">
            <div class="space-y-1.5">
              <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Giá bán (VND) <span class="text-red-500">*</span></Label>
              <Input type="number" v-model.number="editing.giaBan" class="bg-background border border-cream-deep rounded-xl shadow-inner h-10 text-sm font-bold text-caramel" />
            </div>
            <div class="space-y-1.5">
              <Label class="text-espresso font-bold text-[10px] uppercase tracking-wider">Điểm tích lũy khi mua (Ví dụ: 5 điểm)</Label>
              <Input type="number" v-model.number="editing.diemTichLuy" class="bg-background border border-cream-deep rounded-xl shadow-inner h-10 text-sm font-bold text-[#CC8033]" placeholder="Vd: 5 (Nhập 0 để dùng mặc định)" />
            </div>
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

    <!-- Bulk Points Modal -->
    <Modal v-model="isBulkPointsModalOpen">
      <template #header>
        <div class="flex items-center gap-2">
          <div class="w-9 h-9 rounded-xl bg-[#CC8033]/15 text-[#CC8033] flex items-center justify-center font-bold">
            <Gift class="w-5 h-5" />
          </div>
          <div>
            <h2 class="font-premium-serif text-xl font-bold text-espresso leading-tight">
              Cài đặt điểm tích lũy hàng loạt
            </h2>
            <p class="text-[11px] text-[#8A8178]">Gán điểm thưởng tích lũy cho nhiều món cùng lúc hoặc điều chỉnh theo bảng.</p>
          </div>
        </div>
      </template>

      <!-- Tabs Navigation -->
      <div class="flex border-b border-[#EAE3D9] mb-4 text-xs font-bold uppercase tracking-wider">
        <button
          type="button"
          @click="bulkMode = 'batch'"
          :class="bulkMode === 'batch' ? 'border-[#CC8033] text-[#CC8033] font-black' : 'border-transparent text-[#8A8178] hover:text-espresso'"
          class="flex-1 py-3 border-b-2 text-center transition-all focus:outline-none flex items-center justify-center gap-1.5"
        >
          <Sparkles class="w-4 h-4" /> 1. Chọn món & Áp dụng nhanh
        </button>
        <button
          type="button"
          @click="bulkMode = 'table'"
          :class="bulkMode === 'table' ? 'border-[#CC8033] text-[#CC8033] font-black' : 'border-transparent text-[#8A8178] hover:text-espresso'"
          class="flex-1 py-3 border-b-2 text-center transition-all focus:outline-none flex items-center justify-center gap-1.5"
        >
          <Layers class="w-4 h-4" /> 2. Bảng tổng hợp điểm tất cả món
        </button>
      </div>

      <!-- MODE 1: BATCH SELECTION -->
      <div v-if="bulkMode === 'batch'" class="space-y-4 max-h-[60vh] overflow-y-auto pr-1">
        <!-- Input điểm & Preset buttons -->
        <div class="p-4 bg-[#FAF6F0] rounded-2xl border border-amber-200/80 space-y-3">
          <div class="flex items-center justify-between gap-4">
            <div>
              <label class="text-xs font-bold text-espresso block">Nhập số điểm tích lũy muốn cài đặt:</label>
              <p class="text-[10px] text-[#8A8178]">Mỗi món đã chọn sẽ nhận số điểm này khi mua (0 = mặc định 10k/1đ).</p>
            </div>
            <div class="flex items-center gap-1.5 shrink-0">
              <Input
                type="number"
                v-model.number="batchPointsInput"
                class="w-24 h-10 text-center font-bold text-lg text-[#CC8033] bg-white border border-[#CC8033] rounded-xl outline-none"
              />
              <span class="text-xs font-bold text-[#CC8033]">điểm</span>
            </div>
          </div>

          <!-- Quick Presets -->
          <div class="flex items-center gap-2 pt-2 border-t border-amber-200/60">
            <span class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider">Chọn nhanh:</span>
            <button
              v-for="pts in [0, 5, 10, 15, 20, 50]"
              :key="pts"
              @click="batchPointsInput = pts"
              class="px-2.5 py-1 rounded-lg text-xs font-bold transition-all border cursor-pointer"
              :class="batchPointsInput === pts ? 'bg-[#CC8033] text-white border-[#CC8033]' : 'bg-white text-espresso border-[#EAE3D9] hover:border-[#CC8033]'"
            >
              {{ pts === 0 ? '0đ (Mặc định)' : `+${pts} điểm` }}
            </button>
          </div>
        </div>

        <!-- Filter + Select All -->
        <div class="flex items-center justify-between gap-3">
          <div class="relative flex-1">
            <Search class="w-3.5 h-3.5 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
            <Input v-model="bulkSearch" placeholder="Lọc tìm món..." class="pl-9 h-9 text-xs bg-white border border-[#EAE3D9] rounded-xl" />
          </div>
          <button
            @click="toggleSelectAllBulk"
            class="px-3 h-9 rounded-xl border border-[#EAE3D9] bg-white text-xs font-bold text-espresso hover:border-[#CC8033] transition-colors flex items-center gap-1.5 shrink-0 cursor-pointer"
          >
            <component :is="isAllBulkSelected ? CheckSquare : Square" class="w-4 h-4 text-[#CC8033]" />
            {{ isAllBulkSelected ? 'Bỏ chọn tất cả' : 'Chọn tất cả món' }}
          </button>
        </div>

        <!-- Selection Items Grid -->
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-2 max-h-64 overflow-y-auto pr-1">
          <div
            v-for="m in bulkFilteredItems"
            :key="m.maSanPham"
            @click="selectedItemIds.includes(m.maSanPham) ? selectedItemIds = selectedItemIds.filter(id => id !== m.maSanPham) : selectedItemIds.push(m.maSanPham)"
            class="p-2.5 rounded-xl border transition-all cursor-pointer flex items-center justify-between gap-2"
            :class="selectedItemIds.includes(m.maSanPham) ? 'border-[#CC8033] bg-[#FDF7EF] shadow-xs' : 'border-[#EAE3D9] bg-white hover:border-[#CC8033]/40'"
          >
            <div class="flex items-center gap-2.5 min-w-0">
              <div
                class="w-5 h-5 rounded flex items-center justify-center shrink-0 border"
                :class="selectedItemIds.includes(m.maSanPham) ? 'bg-[#CC8033] border-[#CC8033] text-white' : 'border-[#D0C8BF] bg-white'"
              >
                <Check v-if="selectedItemIds.includes(m.maSanPham)" class="w-3.5 h-3.5 stroke-[3]" />
              </div>
              <div class="min-w-0">
                <p class="text-xs font-bold text-espresso truncate">{{ m.tenSanPham }}</p>
                <p class="text-[10px] text-[#8A8178] truncate">{{ m.tenDanhMuc || 'Khác' }} · {{ formatVND(m.giaBan) }}</p>
              </div>
            </div>
            <span class="text-[10px] font-bold px-2 py-0.5 rounded shrink-0" :class="m.diemTichLuy ? 'bg-emerald-50 text-emerald-600 border border-emerald-200' : 'bg-gray-100 text-gray-500'">
              {{ m.diemTichLuy ? `+${m.diemTichLuy}đ` : 'Mặc định' }}
            </span>
          </div>
        </div>
      </div>

      <!-- MODE 2: TABLE OVERVIEW -->
      <div v-if="bulkMode === 'table'" class="space-y-3 max-h-[60vh] overflow-y-auto pr-1">
        <div class="relative">
          <Search class="w-3.5 h-3.5 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input v-model="bulkSearch" placeholder="Lọc danh sách món..." class="pl-9 h-9 text-xs bg-white border border-[#EAE3D9] rounded-xl" />
        </div>

        <div class="border border-[#EAE3D9] rounded-xl overflow-hidden bg-white">
          <table class="w-full text-left text-xs">
            <thead class="bg-[#FAF8F5] text-[10px] font-bold uppercase tracking-wider text-[#8A8178] border-b border-[#EAE3D9]">
              <tr>
                <th class="py-2.5 px-3">Tên món</th>
                <th class="py-2.5 px-3">Danh mục</th>
                <th class="py-2.5 px-3">Giá bán</th>
                <th class="py-2.5 px-3 text-right">Số điểm tích lũy</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-[#F5F2ED]">
              <tr v-for="item in bulkTableItems.filter(i => !bulkSearch || i.tenSanPham.toLowerCase().includes(bulkSearch.toLowerCase()))" :key="item.maSanPham" class="hover:bg-[#FDF7EF]/50">
                <td class="py-2 px-3 font-bold text-espresso">{{ item.tenSanPham }}</td>
                <td class="py-2 px-3 text-muted-foreground text-[11px]">{{ item.tenDanhMuc || '-' }}</td>
                <td class="py-2 px-3 font-semibold text-[#CC8033]">{{ formatVND(item.giaBan) }}</td>
                <td class="py-2 px-3 text-right">
                  <div class="inline-flex items-center gap-1 justify-end">
                    <Input
                      type="number"
                      v-model.number="item.diemTichLuy"
                      class="w-20 h-8 text-center text-xs font-bold text-emerald-700 bg-white border border-[#EAE3D9] rounded-lg focus:border-[#CC8033] outline-none"
                    />
                    <span class="text-[10px] font-bold text-emerald-600">điểm</span>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <template #footer>
        <div class="flex justify-between items-center w-full mt-2">
          <Button variant="outline" @click="isBulkPointsModalOpen = false" :disabled="isBulkSaving" class="border border-[#EAE3D9] rounded-xl text-xs font-bold uppercase tracking-wider px-5 h-10">Hủy</Button>
          
          <Button
            v-if="bulkMode === 'batch'"
            @click="applyBatchPoints"
            :disabled="isBulkSaving || selectedItemIds.length === 0"
            class="bg-gradient-to-r from-[#CC8033] to-[#A6611F] text-white rounded-xl shadow-md px-6 py-2.5 text-xs font-bold transition-all disabled:opacity-50 flex items-center gap-1.5 cursor-pointer"
          >
            ⚡ Áp dụng +{{ batchPointsInput }} điểm cho {{ selectedItemIds.length }} món đã chọn
          </Button>

          <Button
            v-if="bulkMode === 'table'"
            @click="saveBulkTablePoints"
            :disabled="isBulkSaving"
            class="bg-gradient-to-r from-[#CC8033] to-[#A6611F] text-white rounded-xl shadow-md px-6 py-2.5 text-xs font-bold transition-all disabled:opacity-50 flex items-center gap-1.5 cursor-pointer"
          >
            💾 Lưu tất cả điểm đã chỉnh sửa
          </Button>
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
import { Plus, Search, Edit3, Trash2, Flame, Coffee, ChevronLeft, ChevronRight, Image as ImageIcon, X, ShieldAlert, CheckCircle, Upload, Gift, CheckSquare, Square, Sparkles, Layers, Check } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Label from '@/components/ui/Label.vue'
import Switch from '@/components/ui/Switch.vue'
import Textarea from '@/components/ui/Textarea.vue'
import Modal from '@/components/ui/Modal.vue'
import { productsApi, type ProductListItem, type ProductDetail, type SizeDto, type CategoryItem } from '@/services/products'
import { combosApi } from '@/services/combos'
import { useOrderStore } from '@/stores/orders'

const orderStore = useOrderStore()

const isKitchenOutOfStock = (tenSanPham: string) => {
  if (!tenSanPham) return false
  const clean = tenSanPham.replace(/\s*\([^)]*\)$/, '').trim()
  return orderStore.globalOutOfStock.has(tenSanPham) || orderStore.globalOutOfStock.has(clean)
}

const reopenProduct = async (m: ProductListItem) => {
  try {
    m.trangThaiBan = true
    if (m.maSanPham > 0) {
      try {
        await productsApi.updateStatus(m.maSanPham, true)
      } catch (err) {
        console.warn('Lỗi gọi API updateStatus:', err)
      }
    }
    orderStore.clearOutOfStock(m.tenSanPham)
    const clean = m.tenSanPham.replace(/\s*\([^)]*\)$/, '').trim()
    orderStore.clearOutOfStock(clean)
    showToast(`Đã mở bán lại món "${m.tenSanPham}"! Món đã hoạt động bình thường trên POS & Menu.`, 'success')
  } catch (e: any) {
    showToast(e?.message || 'Không thể mở bán lại món.', 'error')
  }
}

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

// ── Bulk Points Setting Modal State ──
const isBulkPointsModalOpen = ref(false)
const bulkMode = ref<'batch' | 'table'>('batch')
const selectedItemIds = ref<number[]>([])
const batchPointsInput = ref<number>(5)
const bulkTableItems = ref<{ maSanPham: number; tenSanPham: string; giaBan: number; tenDanhMuc: string | null; diemTichLuy: number }[]>([])
const bulkSearch = ref('')
const isBulkSaving = ref(false)

const bulkFilteredItems = computed(() => {
  const q = bulkSearch.value.trim().toLowerCase()
  return items.value.filter(i => i.maSanPham > 0 && (!q || i.tenSanPham.toLowerCase().includes(q) || (i.tenDanhMuc && i.tenDanhMuc.toLowerCase().includes(q))))
})

const isAllBulkSelected = computed(() => {
  const validIds = bulkFilteredItems.value.map(i => i.maSanPham)
  return validIds.length > 0 && validIds.every(id => selectedItemIds.value.includes(id))
})

function toggleSelectAllBulk() {
  const validIds = bulkFilteredItems.value.map(i => i.maSanPham)
  if (isAllBulkSelected.value) {
    selectedItemIds.value = selectedItemIds.value.filter(id => !validIds.includes(id))
  } else {
    selectedItemIds.value = Array.from(new Set([...selectedItemIds.value, ...validIds]))
  }
}

function openBulkPointsModal() {
  bulkMode.value = 'batch'
  selectedItemIds.value = []
  batchPointsInput.value = 5
  bulkSearch.value = ''
  bulkTableItems.value = items.value.filter(i => i.maSanPham > 0).map(i => ({
    maSanPham: i.maSanPham,
    tenSanPham: i.tenSanPham,
    giaBan: i.giaBan,
    tenDanhMuc: i.tenDanhMuc,
    diemTichLuy: i.diemTichLuy || 0
  }))
  isBulkPointsModalOpen.value = true
}

async function applyBatchPoints() {
  if (selectedItemIds.value.length === 0) {
    showToast('Vui lòng chọn ít nhất 1 món ăn để cài điểm tích lũy', 'warning')
    return
  }
  const pts = Math.max(0, batchPointsInput.value || 0)
  isBulkSaving.value = true
  try {
    const payload = selectedItemIds.value.map(id => ({ maSanPham: id, diemTichLuy: pts }))
    await productsApi.bulkUpdatePoints(payload)
    showToast(`Đã cài đặt +${pts} điểm thành công cho ${selectedItemIds.value.length} món ăn!`, 'success')
    isBulkPointsModalOpen.value = false
    await loadData()
  } catch (e: any) {
    showToast(e?.message || 'Không thể cập nhật điểm tích lũy hàng loạt', 'error')
  } finally {
    isBulkSaving.value = false
  }
}

async function saveBulkTablePoints() {
  isBulkSaving.value = true
  try {
    const payload = bulkTableItems.value.map(i => ({ maSanPham: i.maSanPham, diemTichLuy: Math.max(0, i.diemTichLuy || 0) }))
    await productsApi.bulkUpdatePoints(payload)
    showToast('Đã lưu bảng điểm tích lũy tất cả món ăn thành công!', 'success')
    isBulkPointsModalOpen.value = false
    await loadData()
  } catch (e: any) {
    showToast(e?.message || 'Không thể cập nhật điểm tích lũy', 'error')
  } finally {
    isBulkSaving.value = false
  }
}

const featuredCount = computed(() => items.value.filter(i => i.laMonNoiBat).length)
const inactiveCount = computed(() => items.value.filter(i => !i.trangThaiBan).length)

const loadData = async () => {
  loading.value = true
  errorMsg.value = ""
  try {
    const [pList, cList, cbList] = await Promise.all([
      productsApi.list(),
      productsApi.listCategories(),
      combosApi.list()
    ])
    
    const comboItems: ProductListItem[] = cbList.map(c => ({
      maSanPham: -c.maCombo,
      tenSanPham: c.tenCombo,
      maDanhMuc: -1,
      tenDanhMuc: 'Combo',
      giaBan: c.giaCombo,
      giaVonDuKien: null,
      hinhAnh: c.hinhAnh,
      kieuMon: 'Combo',
      laMonNoiBat: false,
      trangThaiBan: c.trangThaiHoatDong
    }))

    items.value = [...pList, ...comboItems]
    categories.value = [
      ...cList.filter(c => c.trangThaiHoatDong),
      { maDanhMuc: -1, tenDanhMuc: 'Combo', maDanhMucCha: null, hinhAnh: null, thuTuHienThi: 99, moTa: null, trangThaiHoatDong: true, soLuongSanPham: cbList.length } as CategoryItem
    ]
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
    kichCos: [],
    diemTichLuy: 0
  }
  isModalOpen.value = true
}

const openEdit = async (m: ProductListItem) => {
  if (m.maSanPham < 0) {
    showToast('Vui lòng chuyển sang trang "Quản lý Combo" để chỉnh sửa ưu đãi này.', 'warning')
    return
  }
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
      kichCos: detail.kichCos || [],
      diemTichLuy: detail.diemTichLuy ?? 0
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
      kichCos: editing.value.kichCos || [],
      diemTichLuy: editing.value.diemTichLuy || 0
    }

    if (isEditMode) {
      await productsApi.update(editing.value.maSanPham, payload)
      if (editing.value.trangThaiBan) {
        orderStore.clearOutOfStock(editing.value.tenSanPham)
      }
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
  if (id < 0) {
    showToast('Vui lòng chuyển sang trang "Quản lý Combo" để xóa ưu đãi này.', 'warning')
    return
  }
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
