<template>
  <div class="min-h-screen bg-[#FDFBF7] font-premium-sans text-[#2A231E] lg:pr-[380px]">
   
    <!-- Top bar -->
    <header class="sticky top-0 z-30 bg-[#FDFBF7]/80 backdrop-blur-xl border-b border-[#EAE3D9] shadow-card">
      <div class="max-w-[1200px] mx-auto px-4 sm:px-6 h-16 flex items-center justify-between">
       
        <!-- Logo -->
        <router-link to="/" class="flex items-center gap-3">
          <div class="w-10 h-10 flex items-center justify-center bg-[#4A3224] text-[#FDFBF7] rounded-lg shadow-xl">
            <Coffee class="w-5 h-5" stroke-width="1.5" />
          </div>
          <span class="font-premium-serif text-2xl font-bold tracking-wide text-[#2A231E]">{{ storeInfoStore.tenQuan }}</span>
        </router-link>
        
        <!-- Right actions -->
        <div class="flex items-center gap-5">
          <div class="hidden sm:flex items-center gap-2 bg-white px-3 py-1.5 rounded-lg border border-[#EAE3D9] shadow-xl">
            <span class="w-2 h-2 rounded-full bg-[#CC8033]"></span>
            <span class="text-[10px] uppercase tracking-[0.2em] font-semibold text-[#5C544E]">Bàn số {{ tableId }}</span>
          </div>
         
          <button @click="open = true" class="relative w-11 h-11 bg-white rounded-lg border border-[#EAE3D9] shadow-xl flex items-center justify-center text-[#2A231E] lg:hidden">
            <ShoppingBag class="w-5 h-5" stroke-width="1.5" />
            <span v-if="cart.count() > 0" class="absolute -top-1 -right-1 w-5 h-5 rounded-lg bg-[#CC8033] text-white text-[10px] font-bold flex items-center justify-center shadow-xl border border-[#FDFBF7]">
              {{ cart.count() }}
            </span>
          </button>
        </div>
      </div>
    </header>

    <!-- Hero -->
    <section class="max-w-[1200px] mx-auto px-4 sm:px-6 pt-6">
      <div class="relative overflow-hidden rounded-[28px] bg-gradient-to-br from-[#4A3224] via-[#5C4033] to-[#7B563F] p-7 sm:p-10 shadow-[0_24px_60px_rgba(74,50,36,0.28)]">
        <!-- Decorative glows -->
        <div class="absolute -top-16 -right-8 w-56 h-56 rounded-full bg-[#CC8033]/30 blur-[80px]"></div>
        <div class="absolute -bottom-24 -left-10 w-64 h-64 rounded-full bg-[#E8973D]/20 blur-[90px]"></div>

        <div class="relative">
          <div class="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-white/10 border border-white/15 backdrop-blur-md mb-5">
            <span class="w-2 h-2 rounded-full bg-[#7CD992] animate-pulse"></span>
            <span class="text-[10px] sm:text-[11px] uppercase tracking-[0.2em] font-semibold text-white/80">Đang phục vụ · Bàn {{ tableId }}</span>
          </div>

          <h2 class="font-premium-serif text-4xl sm:text-6xl font-bold text-white leading-[1.05]">
            Thực đơn <span class="text-[#E8973D]">tinh hoa</span>
          </h2>
          <p class="text-sm text-white/55 mt-3 max-w-md font-medium leading-relaxed">
            Gọi món ngay tại bàn — pha chế thủ công, phục vụ tận nơi.
          </p>

          <!-- Search -->
          <div class="relative mt-6 max-w-md">
            <Search class="w-5 h-5 absolute left-4 top-1/2 -translate-y-1/2 text-[#8A8178]" />
            <input
              v-model="search"
              type="text"
              placeholder="Tìm món, đồ uống yêu thích..."
              class="w-full h-12 pl-12 pr-4 rounded-2xl bg-white text-sm font-medium text-[#2A231E] placeholder:text-[#8A8178] focus:outline-none focus:ring-2 focus:ring-[#CC8033]/50 shadow-xl"
            />
          </div>
        </div>
      </div>
    </section>

    <!-- Loyalty Banner -->
    <div class="max-w-[1200px] mx-auto px-4 sm:px-6 mt-4">
      <button
        @click="openLoginSheet = true"
        class="relative w-full overflow-hidden flex items-center justify-between gap-4 px-4 sm:px-5 py-4 rounded-2xl bg-gradient-to-r from-[#CC8033] via-[#D9842F] to-[#E8973D] text-white text-left shadow-[0_8px_24px_rgba(204,128,51,0.3)] border border-white/15 hover:shadow-[0_12px_32px_rgba(204,128,51,0.4)] transition-all duration-300 active:scale-[0.99] group"
      >
        <!-- Decorative soft glows -->
        <div class="absolute -right-4 -top-12 w-32 h-32 rounded-full bg-white/15 blur-2xl pointer-events-none"></div>
        <div class="absolute right-28 -bottom-14 w-28 h-28 rounded-full bg-white/10 blur-2xl pointer-events-none"></div>

        <!-- Left: icon + text -->
        <div class="relative flex items-center gap-3.5 min-w-0">
          <div class="w-11 h-11 rounded-xl bg-white/20 flex items-center justify-center border border-white/30 shrink-0 shadow-inner">
            <Gift class="w-5 h-5 text-white" stroke-width="2" />
          </div>
          <div class="min-w-0">
            <p class="flex items-center gap-1.5 text-sm font-bold leading-tight">
              <Sparkles class="w-4 h-4 shrink-0" stroke-width="2.5" /> Tích điểm &amp; nhận voucher 10%
            </p>
            <p class="text-[11px] text-white/80 font-medium mt-0.5 truncate">Đăng nhập để áp dụng ngay cho đơn hôm nay</p>
          </div>
        </div>

        <!-- Right: CTA -->
        <span class="relative shrink-0 inline-flex items-center gap-1.5 h-9 px-3.5 sm:px-4 rounded-xl bg-white text-[#CC8033] text-xs font-bold shadow-md transition-all group-hover:gap-2.5">
          <span class="hidden sm:inline">Đăng nhập</span>
          <ChevronRight class="w-4 h-4" stroke-width="2.5" />
        </span>
      </button>
    </div>

    <!-- Categories Tabs -->
    <nav class="sticky top-16 z-20 bg-[#FDFBF7]/90 backdrop-blur-md border-b border-[#EAE3D9] py-3 mt-6">
      <div class="max-w-[1200px] mx-auto px-4 sm:px-6 flex gap-2.5 overflow-x-auto custom-scrollbar pb-1">
        <button
          v-for="c in categories"
          :key="c.id"
          @click="activeCat = c.id as any"
          :class="[
            'px-5 py-2.5 rounded-full whitespace-nowrap text-xs font-bold tracking-wide transition-all duration-200 border',
            activeCat === c.id
              ? 'bg-[#4A3224] text-white border-[#4A3224] shadow-lg shadow-[#4A3224]/20'
              : 'bg-white border-[#EAE3D9] text-[#8A8178] hover:border-[#CC8033]/40 hover:text-[#CC8033]'
          ]"
        >
          {{ c.label }}
        </button>
      </div>
    </nav>

    <!-- Menu grid -->
    <main class="max-w-[1200px] mx-auto px-4 sm:px-6 mt-8 pb-40 lg:pb-12">
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6">
        <article
          v-for="m in paginatedItems"
          :key="m.id"
          class="group bg-white rounded-2xl border border-[#EAE3D9] shadow-card flex flex-col relative overflow-hidden hover:shadow-[0_16px_40px_rgba(42,35,30,0.12)] hover:-translate-y-1 transition-all duration-300"
        >
          <!-- Image Container (Clickable) -->
          <div @click="openItemOptions(m)" class="relative aspect-square overflow-hidden bg-[#F5F2ED] cursor-pointer">
            <img
              :src="m.image"
              :alt="m.name"
              loading="lazy"
              class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-700"
            />
            <!-- Gradient Overlay -->
            <div class="absolute inset-0 bg-gradient-to-t from-black/25 via-transparent to-transparent"></div>

            <div v-if="m.popular" class="absolute top-3 left-3 flex items-center gap-1 px-2.5 py-1 rounded-full bg-white/95 backdrop-blur-md text-[#CC8033] text-[9px] uppercase tracking-[0.15em] font-bold shadow-lg">
              <Star class="w-3 h-3 fill-[#CC8033]" /> Nổi bật
            </div>

            <!-- Customize button (hover) -->
            <button
              @click.stop="openItemOptions(m)"
              class="absolute bottom-3 right-3 w-9 h-9 rounded-full bg-white/90 backdrop-blur-md flex items-center justify-center text-[#2A231E] shadow-lg opacity-0 group-hover:opacity-100 translate-y-2 group-hover:translate-y-0 transition-all duration-300 hover:bg-[#CC8033] hover:text-white"
              title="Tùy chỉnh món"
            >
              <Settings2 class="w-4 h-4" stroke-width="2" />
            </button>
          </div>

          <!-- Content -->
          <div class="p-4 flex flex-col flex-1">
            <h3 class="font-bold text-base leading-snug text-[#2A231E] cursor-pointer hover:text-[#CC8033] transition-colors line-clamp-1" @click="openItemOptions(m)">{{ m.name }}</h3>
            <p class="text-[11px] text-[#8A8178] mt-1.5 line-clamp-2 leading-relaxed flex-1 font-medium">{{ m.description }}</p>

            <div class="flex items-end justify-between gap-2 mt-4">
              <div class="flex flex-col min-w-0">
                <span class="text-[9px] uppercase tracking-widest text-[#B5ADA3] font-bold">Giá</span>
                <span class="text-[#CC8033] font-bold text-lg leading-tight truncate">{{ formatVND(m.price) }}</span>
              </div>
              <div v-if="(cart.lines.find(l => l.item.id === m.id)?.qty || 0) > 0" class="flex items-center bg-[#FDFBF7] rounded-xl border border-[#EAE3D9] p-0.5 shadow-sm h-10 shrink-0">
                <button @click="cart.setQty(m.id, cart.lines.find(l => l.item.id === m.id)!.qty - 1)" class="w-8 h-full rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-all">
                  <Minus class="w-3.5 h-3.5" stroke-width="2.5" />
                </button>
                <span class="w-7 text-center text-sm font-bold text-[#2A231E]">
                  {{ cart.lines.find(l => l.item.id === m.id)?.qty }}
                </span>
                <button @click="cart.setQty(m.id, cart.lines.find(l => l.item.id === m.id)!.qty + 1)" class="w-8 h-full rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-all">
                  <Plus class="w-3.5 h-3.5" stroke-width="2.5" />
                </button>
              </div>
              <button
                v-else
                @click="addToCart(m)"
                class="w-10 h-10 rounded-xl bg-[#4A3224] flex items-center justify-center text-white shadow-md hover:bg-[#CC8033] transition-colors active:scale-90 shrink-0"
                title="Đặt ngay"
              >
                <Plus class="w-5 h-5" stroke-width="2.5" />
              </button>
            </div>
          </div>
        </article>
      </div>

      <!-- Empty state -->
      <div v-if="paginatedItems.length === 0" class="flex flex-col items-center justify-center py-20 text-center">
        <div class="w-16 h-16 rounded-2xl bg-white border border-[#EAE3D9] flex items-center justify-center shadow-card mb-5">
          <Search class="w-7 h-7 text-[#D5CEC4]" stroke-width="1.5" />
        </div>
        <p class="font-premium-serif text-xl font-bold text-[#2A231E]">Không tìm thấy món nào</p>
        <p class="text-xs text-[#8A8178] mt-1.5 font-medium">Thử từ khóa khác hoặc chọn danh mục khác nhé.</p>
      </div>

      <!-- Phân trang -->
      <div v-if="totalPages > 1" class="flex justify-center items-center gap-2 mt-12">
        <button
          @click="prevPage"
          :disabled="currentPage === 1"
          class="w-10 h-10 rounded-lg bg-white border border-[#EAE3D9] flex items-center justify-center text-[#2A231E] shadow-xl disabled:opacity-40"
        >
          <ChevronLeft class="w-5 h-5" />
        </button>
       
        <div class="flex gap-1.5 px-2">
          <button
            v-for="page in totalPages"
            :key="page"
            @click="goToPage(page)"
            :class="[
              'w-10 h-10 rounded-lg text-sm font-bold shadow-xl border',
              currentPage === page
                ? 'bg-[#CC8033] text-white border-[#CC8033]'
                : 'bg-white border-[#EAE3D9] text-[#2A231E]'
            ]"
          >
            {{ page }}
          </button>
        </div>
        
        <button
          @click="nextPage"
          :disabled="currentPage === totalPages"
          class="w-10 h-10 rounded-lg bg-white border border-[#EAE3D9] flex items-center justify-center text-[#2A231E] shadow-xl disabled:opacity-40"
        >
          <ChevronRight class="w-5 h-5" />
        </button>
      </div>
    </main>

    <!-- Sticky Bottom Bar -->
    <div v-if="cart.count() > 0" class="fixed bottom-6 left-0 right-0 z-30 px-4 sm:px-6 lg:hidden">
      <div class="max-w-[1200px] mx-auto flex justify-center sm:justify-end">
        <button
          @click="open = true"
          class="bg-[#4A3224]/95 backdrop-blur-xl text-[#FDFBF7] rounded-lg p-3 sm:px-5 sm:py-3 flex items-center gap-6 sm:gap-10 shadow-card border border-white/10"
        >
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-lg border border-white/20 flex items-center justify-center text-sm font-bold text-white bg-[#CC8033] shadow-xl">
              {{ cart.count() }}
            </div>
            <div class="text-left hidden sm:block">
              <div class="font-bold text-xs uppercase tracking-[0.1em] text-white/90">Giỏ hàng của bạn</div>
              <div class="text-[10px] text-white/50 uppercase tracking-widest mt-0.5 font-medium">Bàn {{ tableId }}</div>
            </div>
          </div>
          <div class="flex items-center gap-5">
            <span class="font-premium-sans text-2xl font-bold text-[#CC8033] tracking-wide">{{ formatVND(cart.total()) }}</span>
            <span class="w-10 h-10 bg-white/10 rounded-lg flex items-center justify-center text-white border border-white/20">
              <ShoppingBag class="w-4 h-4" />
            </span>
          </div>
        </button>
      </div>
    </div>

    <!-- Slide-Over Giỏ Hàng (mobile only) -->
    <Transition name="slide-right">
      <div v-if="open" class="fixed inset-0 z-50 flex justify-end lg:hidden" @click.self="open = false">
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-[#1A1512]/60 backdrop-blur-md" @click="open = false"></div>
        
        <!-- Slide-Over Content -->
        <div class="relative w-full max-w-md bg-[#F5F2ED] h-full shadow-[-20px_0_60px_rgba(42,35,30,0.15)] flex flex-col border-l border-[#EAE3D9] z-10 animate-in slide-in-from-right duration-300">

          <!-- Slide-Over Header -->
          <div class="px-6 pt-6 pb-5 border-b border-[#EAE3D9] bg-white flex flex-col gap-4">
            <div class="flex justify-between items-start">
              <div>
                <div class="flex items-center gap-2">
                  <h2 class="font-premium-serif text-2xl font-bold text-[#2A231E]">Giỏ hàng</h2>
                  <span v-if="cart.count() > 0" class="px-2 py-0.5 rounded-full bg-[#CC8033]/12 text-[#CC8033] text-xs font-bold">{{ cart.count() }} món</span>
                </div>
                <p class="text-[11px] text-[#8A8178] uppercase tracking-[0.15em] font-bold mt-1">Bàn {{ tableId }}</p>
              </div>
              <button @click="open = false" class="w-9 h-9 rounded-full bg-[#F5F2ED] border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] shadow-sm hover:text-[#2A231E] transition-colors">
                <X class="w-4 h-4" stroke-width="2.5" />
              </button>
            </div>

            <!-- Member badge / Login CTA -->
            <div v-if="customerPhone" class="flex items-center justify-between bg-gradient-to-r from-[#FFF9F2] to-[#FDFBF7] border border-[#E8C5A5]/60 rounded-2xl p-3 shadow-sm">
              <div class="flex items-center gap-2.5 min-w-0">
                <div class="w-9 h-9 rounded-full bg-[#CC8033] text-white flex items-center justify-center font-bold shadow-sm shrink-0">
                  {{ (customerName || 'K').charAt(0).toUpperCase() }}
                </div>
                <div class="min-w-0">
                  <div class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider">Thành viên thân thiết</div>
                  <div class="text-sm font-bold text-[#2A231E] truncate">{{ customerName || customerPhone }} 👋</div>
                </div>
              </div>
              <div class="flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-white border border-[#E8C5A5] text-[#CC8033] text-xs font-bold shadow-sm shrink-0">
                <Coffee class="w-3 h-3" /> 150 điểm
              </div>
            </div>
            <button
              v-else
              @click="openLoginSheet = true"
              class="w-full flex items-center justify-between gap-3 bg-[#FDFBF7] border border-dashed border-[#CC8033]/40 rounded-2xl p-3 hover:bg-[#FFF9F2] transition-colors"
            >
              <span class="flex items-center gap-2.5">
                <span class="w-9 h-9 rounded-full bg-[#CC8033]/12 text-[#CC8033] flex items-center justify-center shrink-0"><Gift class="w-4 h-4" /></span>
                <span class="text-left">
                  <span class="block text-xs font-bold text-[#2A231E]">Đăng nhập tích điểm</span>
                  <span class="block text-[10px] text-[#8A8178] font-medium">Nhận voucher &amp; ưu đãi thành viên</span>
                </span>
              </span>
              <ChevronRight class="w-4 h-4 text-[#CC8033] shrink-0" stroke-width="2.5" />
            </button>
          </div>

          <!-- Slide-Over Body -->
          <div class="flex-1 overflow-y-auto p-5 space-y-3 bg-[#F5F2ED] custom-scrollbar">
            <!-- Empty -->
            <div v-if="cart.lines.length === 0" class="text-center py-20 flex flex-col items-center">
              <div class="w-20 h-20 rounded-2xl border border-dashed border-[#EAE3D9] flex items-center justify-center mb-6 bg-white shadow-sm">
                <ShoppingBag class="w-8 h-8 text-[#D5CEC4]" stroke-width="1.5" />
              </div>
              <p class="font-premium-serif text-2xl font-bold text-[#2A231E]">Chưa có món nào</p>
              <p class="text-xs text-[#8A8178] mt-2 font-medium">Hãy chọn món từ thực đơn để bắt đầu.</p>
            </div>

            <!-- Items -->
            <div v-for="l in cart.lines" :key="l.cartLineId" class="flex gap-3.5 p-3 rounded-2xl bg-white shadow-[0_2px_10px_rgba(42,35,30,0.04)] border border-[#EAE3D9] relative">
              <div class="w-[68px] h-[68px] rounded-xl overflow-hidden flex-shrink-0">
                <img :src="l.item.image" :alt="l.item.name" class="w-full h-full object-cover" />
              </div>
              <div class="flex-1 min-w-0 flex flex-col justify-between">
                <div class="flex justify-between items-start gap-2 pr-5">
                  <div class="min-w-0">
                    <h4 class="font-bold text-sm text-[#2A231E] leading-tight truncate">{{ l.item.name }}</h4>
                    <!-- Options -->
                    <div v-if="l.options" class="mt-1 flex flex-wrap gap-1">
                      <span v-if="l.options.size !== 'M' || l.options.sugar !== '100%' || l.options.ice !== '100%'" class="text-[9px] font-bold text-[#5C544E] bg-[#F5F2ED] px-1.5 py-0.5 rounded-md">
                        {{ l.options.size }} · Đá {{ l.options.ice }} · Đường {{ l.options.sugar }}
                      </span>
                      <span v-for="t in l.options.toppings" :key="t.name" class="text-[9px] font-bold text-[#CC8033] bg-[#FFF9F2] px-1.5 py-0.5 rounded-md">
                        + {{ t.name }} ×{{ t.qty }}
                      </span>
                    </div>
                    <div v-if="l.options?.note" class="text-[10px] text-[#D97724] italic mt-1 break-words">"{{ l.options.note }}"</div>
                  </div>
                  <button @click="cart.remove(l.cartLineId)" class="absolute top-2.5 right-2.5 w-7 h-7 rounded-full flex items-center justify-center text-[#C5BEB8] hover:text-red-500 hover:bg-red-50 transition-colors">
                    <Trash2 class="w-3.5 h-3.5" stroke-width="2" />
                  </button>
                </div>

                <!-- Qty + line total -->
                <div class="flex items-center justify-between mt-2">
                  <div class="flex items-center bg-[#FDFBF7] rounded-xl border border-[#EAE3D9] p-0.5">
                    <button @click="cart.setQty(l.cartLineId, l.qty - 1)" class="w-7 h-7 rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-all">
                      <Minus class="w-3 h-3" stroke-width="2.5" />
                    </button>
                    <span class="w-8 text-center text-xs font-bold text-[#2A231E]">{{ l.qty }}</span>
                    <button @click="cart.setQty(l.cartLineId, l.qty + 1)" class="w-7 h-7 rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-all">
                      <Plus class="w-3 h-3" stroke-width="2.5" />
                    </button>
                  </div>
                  <span class="text-sm text-[#2A231E] font-bold">{{ formatVND((l.item.price + (l.options?.extraPrice || 0)) * l.qty) }}</span>
                </div>
              </div>
            </div>

            <!-- Dùng điểm (chỉ khi đã là thành viên) -->
            <div v-if="cart.lines.length > 0 && customerPhone" class="bg-white p-4 rounded-2xl border border-[#EAE3D9] shadow-sm">
              <label for="usePoints" class="flex items-center gap-3 cursor-pointer">
                <input type="checkbox" v-model="usePoints" id="usePoints" class="w-5 h-5 rounded-md border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033]" />
                <span class="flex-1">
                  <span class="block text-sm font-bold text-[#2A231E]">Dùng 50 điểm thưởng</span>
                  <span class="block text-[11px] text-[#8A8178] font-medium">Giảm ngay <span class="text-[#CC8033] font-bold">20.000đ</span> cho đơn này</span>
                </span>
                <Gift class="w-5 h-5 text-[#CC8033]" />
              </label>
            </div>
          </div>

          <!-- Slide-Over Footer -->
          <div v-if="cart.lines.length > 0" class="p-5 bg-white border-t border-[#EAE3D9] shadow-[0_-10px_30px_rgba(42,35,30,0.06)]">
            <div class="space-y-2 mb-4">
              <div class="flex justify-between items-center text-sm">
                <span class="text-[#8A8178] font-medium">Tạm tính</span>
                <span class="font-bold text-[#5C544E]">{{ formatVND(cart.total()) }}</span>
              </div>
              <div v-if="usePoints" class="flex justify-between items-center text-sm">
                <span class="text-[#8A8178] font-medium flex items-center gap-1.5"><Gift class="w-3.5 h-3.5 text-[#CC8033]" /> Điểm thưởng</span>
                <span class="font-bold text-[#E85D04]">- 20.000đ</span>
              </div>
              <div class="border-t border-dashed border-[#EAE3D9] pt-3 flex justify-between items-center">
                <span class="text-sm font-bold text-[#2A231E]">Tổng cộng</span>
                <span class="font-sans text-2xl font-bold text-[#2A231E] leading-none">{{ formatVND(cart.total() - (usePoints ? 20000 : 0)) }}</span>
              </div>
            </div>
            <button
              @click="handleOrder"
              class="group w-full h-14 bg-gradient-to-r from-[#CC8033] to-[#D97724] hover:shadow-[0_10px_30px_rgba(204,128,51,0.4)] text-white rounded-2xl shadow-lg transition-all active:scale-[0.99] flex items-center justify-between px-5"
            >
              <span class="flex items-center gap-2 text-sm font-bold uppercase tracking-wide">
                <ShoppingBag class="w-4 h-4" /> Gửi đơn đặt món
              </span>
              <span class="flex items-center gap-2 font-bold">
                {{ formatVND(cart.total() - (usePoints ? 20000 : 0)) }}
                <ChevronRight class="w-4 h-4 group-hover:translate-x-0.5 transition-transform" stroke-width="2.5" />
              </span>
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- ─── Desktop Cart Sidebar (always visible, lg+) ─── -->
    <div class="hidden lg:flex fixed top-0 right-0 h-screen w-[380px] bg-[#F5F2ED] border-l border-[#EAE3D9] flex-col z-20 shadow-[-16px_0_48px_rgba(42,35,30,0.07)]">
      <!-- Header -->
      <div class="px-5 pt-5 pb-4 border-b border-[#EAE3D9] bg-white flex flex-col gap-3 shrink-0">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-2">
            <h2 class="font-premium-serif text-xl font-bold text-[#2A231E]">Đơn của bạn</h2>
            <span v-if="cart.count() > 0" class="px-2 py-0.5 rounded-full bg-[#CC8033]/12 text-[#CC8033] text-xs font-bold">{{ cart.count() }} món</span>
          </div>
          <div class="flex items-center gap-1.5 bg-[#F5F2ED] px-3 py-1.5 rounded-lg border border-[#EAE3D9]">
            <span class="w-1.5 h-1.5 rounded-full bg-[#CC8033]"></span>
            <span class="text-[10px] uppercase tracking-[0.2em] font-semibold text-[#5C544E]">Bàn {{ tableId }}</span>
          </div>
        </div>
        <div v-if="customerPhone" class="flex items-center justify-between bg-[#FFF9F2] border border-[#E8C5A5]/60 rounded-xl p-2.5">
          <div class="flex items-center gap-2 min-w-0">
            <div class="w-8 h-8 rounded-full bg-[#CC8033] text-white flex items-center justify-center font-bold text-xs shrink-0">{{ (customerName || 'K').charAt(0).toUpperCase() }}</div>
            <div class="text-sm font-bold text-[#2A231E] truncate">{{ customerName || customerPhone }}</div>
          </div>
          <div class="flex items-center gap-1 px-2 py-0.5 rounded-full bg-white border border-[#E8C5A5] text-[#CC8033] text-[11px] font-bold shrink-0"><Coffee class="w-3 h-3" /> 150 điểm</div>
        </div>
        <button v-else @click="openLoginSheet = true" class="w-full flex items-center justify-between gap-2 bg-[#FDFBF7] border border-dashed border-[#CC8033]/40 rounded-xl p-2.5 hover:bg-[#FFF9F2] transition-colors">
          <span class="flex items-center gap-2">
            <span class="w-8 h-8 rounded-full bg-[#CC8033]/12 text-[#CC8033] flex items-center justify-center shrink-0"><Gift class="w-3.5 h-3.5" /></span>
            <span class="text-left">
              <span class="block text-xs font-bold text-[#2A231E]">Đăng nhập tích điểm</span>
              <span class="block text-[10px] text-[#8A8178]">Nhận voucher &amp; ưu đãi thành viên</span>
            </span>
          </span>
          <ChevronRight class="w-4 h-4 text-[#CC8033] shrink-0" stroke-width="2.5" />
        </button>
      </div>

      <!-- Body -->
      <div class="flex-1 overflow-y-auto p-4 space-y-3 custom-scrollbar">
        <div v-if="cart.lines.length === 0" class="text-center py-16 flex flex-col items-center">
          <div class="w-16 h-16 rounded-2xl border border-dashed border-[#EAE3D9] flex items-center justify-center mb-4 bg-white shadow-sm">
            <ShoppingBag class="w-7 h-7 text-[#D5CEC4]" stroke-width="1.5" />
          </div>
          <p class="font-premium-serif text-lg font-bold text-[#2A231E]">Chưa chọn món</p>
          <p class="text-xs text-[#8A8178] mt-1.5 font-medium">Chọn món từ thực đơn để đặt ngay.</p>
        </div>

        <div v-for="l in cart.lines" :key="l.cartLineId" class="flex gap-3 p-3 rounded-2xl bg-white border border-[#EAE3D9] relative">
          <div class="w-16 h-16 rounded-xl overflow-hidden shrink-0">
            <img :src="l.item.image" :alt="l.item.name" class="w-full h-full object-cover" />
          </div>
          <div class="flex-1 min-w-0 flex flex-col justify-between">
            <div class="flex justify-between items-start gap-2 pr-5">
              <div class="min-w-0">
                <h4 class="font-bold text-sm text-[#2A231E] leading-tight truncate">{{ l.item.name }}</h4>
                <div v-if="l.options" class="mt-0.5 flex flex-wrap gap-1">
                  <span v-if="l.options.size !== 'M' || l.options.sugar !== '100%' || l.options.ice !== '100%'" class="text-[9px] font-bold text-[#5C544E] bg-[#F5F2ED] px-1.5 py-0.5 rounded-md">{{ l.options.size }} · Đá {{ l.options.ice }} · Đường {{ l.options.sugar }}</span>
                  <span v-for="t in l.options.toppings" :key="t.name" class="text-[9px] font-bold text-[#CC8033] bg-[#FFF9F2] px-1.5 py-0.5 rounded-md">+{{ t.name }}</span>
                </div>
                <div v-if="l.options?.note" class="text-[10px] text-[#D97724] italic mt-0.5">"{{ l.options.note }}"</div>
              </div>
              <button @click="cart.remove(l.cartLineId)" class="absolute top-2 right-2 w-6 h-6 rounded-full flex items-center justify-center text-[#C5BEB8] hover:text-red-500 hover:bg-red-50 transition-colors">
                <Trash2 class="w-3.5 h-3.5" stroke-width="2" />
              </button>
            </div>
            <div class="flex items-center justify-between mt-2">
              <div class="flex items-center bg-[#FDFBF7] rounded-xl border border-[#EAE3D9] p-0.5">
                <button @click="cart.setQty(l.cartLineId, l.qty - 1)" class="w-6 h-6 rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white transition-all"><Minus class="w-3 h-3" stroke-width="2.5" /></button>
                <span class="w-7 text-center text-xs font-bold text-[#2A231E]">{{ l.qty }}</span>
                <button @click="cart.setQty(l.cartLineId, l.qty + 1)" class="w-6 h-6 rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white transition-all"><Plus class="w-3 h-3" stroke-width="2.5" /></button>
              </div>
              <span class="text-sm font-bold text-[#2A231E]">{{ formatVND((l.item.price + (l.options?.extraPrice || 0)) * l.qty) }}</span>
            </div>
          </div>
        </div>

        <div v-if="cart.lines.length > 0 && customerPhone" class="bg-white p-3.5 rounded-2xl border border-[#EAE3D9]">
          <label for="usePointsSidebar" class="flex items-center gap-3 cursor-pointer">
            <input type="checkbox" v-model="usePoints" id="usePointsSidebar" class="w-4 h-4 rounded border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033]" />
            <span class="flex-1">
              <span class="block text-sm font-bold text-[#2A231E]">Dùng 50 điểm thưởng</span>
              <span class="block text-[10px] text-[#8A8178]">Giảm ngay <span class="text-[#CC8033] font-bold">20.000đ</span></span>
            </span>
            <Gift class="w-4 h-4 text-[#CC8033]" />
          </label>
        </div>
      </div>

      <!-- Footer -->
      <div v-if="cart.lines.length > 0" class="p-4 bg-white border-t border-[#EAE3D9] shrink-0 shadow-[0_-8px_24px_rgba(42,35,30,0.05)]">
        <div class="space-y-2 mb-3">
          <div class="flex justify-between items-center text-sm">
            <span class="text-[#8A8178] font-medium">Tạm tính</span>
            <span class="font-bold text-[#5C544E]">{{ formatVND(cart.total()) }}</span>
          </div>
          <div v-if="usePoints" class="flex justify-between items-center text-sm">
            <span class="text-[#8A8178] font-medium flex items-center gap-1.5"><Gift class="w-3.5 h-3.5 text-[#CC8033]" /> Điểm thưởng</span>
            <span class="font-bold text-[#E85D04]">- 20.000đ</span>
          </div>
          <div class="border-t border-dashed border-[#EAE3D9] pt-2.5 flex justify-between items-center">
            <span class="text-sm font-bold text-[#2A231E]">Tổng cộng</span>
            <span class="text-xl font-bold text-[#2A231E]">{{ formatVND(cart.total() - (usePoints ? 20000 : 0)) }}</span>
          </div>
        </div>
        <button @click="handleOrder" class="group w-full h-12 bg-gradient-to-r from-[#CC8033] to-[#D97724] hover:shadow-[0_8px_24px_rgba(204,128,51,0.4)] text-white rounded-2xl shadow-lg transition-all active:scale-[0.99] flex items-center justify-between px-4">
          <span class="flex items-center gap-2 text-xs font-bold uppercase tracking-wide"><ShoppingBag class="w-4 h-4" /> Gửi đơn đặt món</span>
          <span class="flex items-center gap-1.5 font-bold text-sm">{{ formatVND(cart.total() - (usePoints ? 20000 : 0)) }}<ChevronRight class="w-4 h-4 group-hover:translate-x-0.5 transition-transform" stroke-width="2.5" /></span>
        </button>
      </div>
    </div>

    <!-- Chatbot Widget -->
    <ChatbotWidget />

    <!-- Customer Login Modal (form nhỏ căn giữa) -->
    <Transition name="login-modal">
      <div
        v-if="openLoginSheet"
        class="fixed inset-0 z-[60] flex items-center justify-center p-4"
        @click.self="openLoginSheet = false"
      >
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-[#1A1512]/50 backdrop-blur-sm" @click="openLoginSheet = false"></div>

        <!-- Card -->
        <div class="relative w-full max-w-sm bg-white rounded-3xl shadow-[0_30px_80px_rgba(42,35,30,0.3)] border border-[#EAE3D9] max-h-[90vh] overflow-y-auto custom-scrollbar">
          <div class="px-6 py-6">
            <!-- Header -->
            <div class="flex items-start justify-between mb-5">
              <div class="flex items-center gap-3">
                <div class="w-11 h-11 rounded-2xl bg-[#CC8033]/12 flex items-center justify-center text-[#CC8033] shrink-0">
                  <Gift class="w-5 h-5" stroke-width="2" />
                </div>
                <div>
                  <h3 class="font-premium-serif text-xl font-bold text-[#1A1512] leading-tight">Đăng nhập nhanh</h3>
                  <p class="text-[11px] text-[#8A8178] mt-0.5 font-medium">Tích điểm · Voucher · Lịch sử đơn</p>
                </div>
              </div>
              <button @click="openLoginSheet = false" class="w-9 h-9 rounded-xl bg-[#F5F2ED] flex items-center justify-center text-[#8A8178] border border-[#EAE3D9] hover:text-[#2A231E] transition-colors shrink-0">
                <X class="w-4 h-4" stroke-width="2" />
              </button>
            </div>

            <!-- Benefits grid (2x2, không tràn ngang) -->
            <div class="grid grid-cols-2 gap-2 mb-5">
              <div v-for="b in loginBenefits" :key="b.label" class="flex items-center gap-2 px-3 py-2 rounded-xl bg-[#FAF6F0] border border-[#EAE3D9]">
                <span class="text-base shrink-0">{{ b.emoji }}</span>
                <span class="text-[10px] font-bold text-[#5C544E] uppercase tracking-tight leading-tight">{{ b.label }}</span>
              </div>
            </div>

            <!-- Form tích điểm: chỉ tên + SĐT -->
            <div class="space-y-3">
              <div>
                <label class="text-[11px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block">Họ và tên</label>
                <div class="relative">
                  <User class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-[#8A8178]" />
                  <input
                    v-model="customerName"
                    type="text"
                    placeholder="Nguyễn Văn A"
                    class="w-full h-12 pl-10 pr-4 rounded-xl border-2 border-[#EAE3D9] focus:border-[#CC8033] focus:outline-none text-sm font-medium bg-[#FAF6F0] text-[#2A231E] placeholder:text-[#A89F95] transition-colors duration-200"
                  />
                </div>
              </div>

              <div>
                <label class="text-[11px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block">Số điện thoại</label>
                <div class="relative">
                  <span class="absolute left-3.5 top-1/2 -translate-y-1/2 text-sm font-bold text-[#8A8178]">🇻🇳 +84</span>
                  <input
                    v-model="phoneNumber"
                    type="tel"
                    placeholder="9xx xxx xxx"
                    maxlength="10"
                    @keyup.enter="submitLogin"
                    class="w-full h-12 pl-[4.75rem] pr-4 rounded-xl border-2 border-[#EAE3D9] focus:border-[#CC8033] focus:outline-none text-sm font-medium bg-[#FAF6F0] text-[#2A231E] placeholder:text-[#A89F95] transition-colors duration-200"
                  />
                </div>
              </div>

              <button
                @click="submitLogin"
                :disabled="!customerName.trim() || phoneNumber.length < 9"
                class="w-full h-12 rounded-xl bg-[#CC8033] text-white text-sm font-bold uppercase tracking-wide shadow-md hover:bg-[#B8722D] disabled:opacity-40 disabled:cursor-not-allowed transition-colors duration-200"
              >
                Tích điểm ngay
              </button>
            </div>

            <button @click="openLoginSheet = false" class="block w-full text-center text-[10px] text-[#A89F95] font-medium mt-5 hover:text-[#8A8178] transition-colors">
              Bỏ qua, tiếp tục xem thực đơn
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Bottom Sheet / Modal Tùy Chỉnh Món (Topping, Size) -->
    <Transition name="sheet-backdrop">
      <div v-if="itemOptionsOpen" class="fixed inset-0 z-[60] bg-[#1A1512]/60 backdrop-blur-sm" @click.self="itemOptionsOpen = false"></div>
    </Transition>
    <Transition name="sheet-slide">
      <div v-if="itemOptionsOpen" class="fixed inset-x-0 bottom-0 sm:inset-auto sm:top-1/2 sm:left-1/2 sm:-translate-x-1/2 sm:-translate-y-1/2 z-[61] w-full sm:max-w-[600px] bg-[#FDFBF7] rounded-t-[2rem] sm:rounded-[2rem] shadow-[0_20px_60px_rgba(42,35,30,0.2)] flex flex-col max-h-[90vh] overflow-hidden border border-[#EAE3D9]/50">
        
        <!-- Header Image -->
        <div class="relative h-48 sm:h-56 shrink-0 bg-[#F5F2ED]">
          <img :src="selectedItem?.image" class="w-full h-full object-cover" />
          <div class="absolute inset-0 bg-gradient-to-t from-[#FDFBF7] via-black/10 to-transparent"></div>
          <button @click="itemOptionsOpen = false" class="absolute top-4 right-4 w-9 h-9 rounded-full bg-white/30 backdrop-blur-md border border-white/40 flex items-center justify-center text-white shadow-lg hover:bg-white/40 transition-colors">
            <X class="w-4 h-4" stroke-width="2.5" />
          </button>
        </div>
        
        <div class="flex-1 overflow-y-auto custom-scrollbar p-6 sm:p-8 space-y-8">
          <!-- Title & Price -->
          <div class="text-center sm:text-left">
            <h2 class="font-premium-serif text-3xl font-bold text-[#2A231E] mb-2">{{ selectedItem?.name }}</h2>
            <p class="text-sm text-[#8A8178] font-medium leading-relaxed">{{ selectedItem?.description }}</p>
            <div class="mt-3 font-bold text-2xl text-[#CC8033]">{{ formatVND(selectedItem?.price || 0) }}</div>
          </div>

          <!-- Kích cỡ (Size) -->
          <div class="space-y-4">
            <h3 class="text-[11px] uppercase tracking-[0.15em] font-bold text-[#8A8178] flex items-center gap-2"><Coffee class="w-4 h-4" /> Chọn Kích Cỡ</h3>
            <div class="grid grid-cols-2 gap-3">
              <button type="button" @click="selectedSize = 'M'" class="flex items-center gap-3 p-4 rounded-2xl border-2 cursor-pointer shadow-sm relative overflow-hidden group transition-all text-left" :class="selectedSize === 'M' ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white hover:border-[#CC8033]/50'">
                <div class="w-5 h-5 rounded-full border-2 flex items-center justify-center shrink-0" :class="selectedSize === 'M' ? 'border-[#CC8033]' : 'border-[#EAE3D9]'">
                  <div class="w-2.5 h-2.5 rounded-full bg-[#CC8033] transition-transform" :class="selectedSize === 'M' ? 'scale-100' : 'scale-0'"></div>
                </div>
                <span class="text-sm font-bold text-[#2A231E]">Vừa (M) <span class="block text-[#8A8178] text-xs font-medium mt-0.5">+ 0đ</span></span>
                <div v-if="selectedSize === 'M'" class="absolute right-0 bottom-0 w-12 h-12 bg-[#CC8033]/5 rounded-tl-full transition-transform group-hover:scale-110"></div>
              </button>

              <button type="button" @click="selectedSize = 'L'" class="flex items-center gap-3 p-4 rounded-2xl border-2 cursor-pointer shadow-sm relative overflow-hidden group transition-all text-left" :class="selectedSize === 'L' ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white hover:border-[#CC8033]/50'">
                <div class="w-5 h-5 rounded-full border-2 flex items-center justify-center shrink-0" :class="selectedSize === 'L' ? 'border-[#CC8033]' : 'border-[#EAE3D9]'">
                  <div class="w-2.5 h-2.5 rounded-full bg-[#CC8033] transition-transform" :class="selectedSize === 'L' ? 'scale-100' : 'scale-0'"></div>
                </div>
                <span class="text-sm font-bold text-[#2A231E]">Lớn (L) <span class="block text-[#CC8033] text-xs font-bold mt-0.5">+ 10.000đ</span></span>
                <div v-if="selectedSize === 'L'" class="absolute right-0 bottom-0 w-12 h-12 bg-[#CC8033]/5 rounded-tl-full transition-transform group-hover:scale-110"></div>
              </button>
            </div>
          </div>

          <!-- Topping -->
          <div v-if="selectedItem?.category !== 'pastry'" class="space-y-4">
            <h3 class="text-[11px] uppercase tracking-[0.15em] font-bold text-[#8A8178] flex items-center gap-2"><Plus class="w-4 h-4" /> Thêm Topping</h3>
            
            <div class="grid grid-cols-3 sm:grid-cols-4 gap-3 sm:gap-4">
              <div v-for="topping in availableToppings" :key="topping.id" class="relative flex flex-col p-2 rounded-2xl bg-white transition-all group" :class="(selectedToppings[topping.id] || 0) > 0 ? 'border-2 border-[#CC8033] shadow-md bg-[#FFF9F2]' : 'border-2 border-[#EAE3D9] shadow-sm hover:shadow-md'">
                <div class="w-full aspect-[4/3] rounded-xl overflow-hidden bg-[#F5F2ED] mb-2.5 relative z-10 cursor-pointer" @click="updateTopping(topping.id, 1)">
                  <img :src="topping.image" class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500" />
                  <div class="absolute inset-0 bg-gradient-to-b from-black/20 to-transparent transition-opacity" :class="(selectedToppings[topping.id] || 0) > 0 ? 'opacity-100' : 'opacity-0'"></div>
                  
                  <!-- Quantity Badge -->
                  <div v-if="(selectedToppings[topping.id] || 0) > 0" class="absolute top-1.5 right-1.5 w-6 h-6 rounded-full bg-[#CC8033] border-2 border-white flex items-center justify-center shadow-md">
                    <span class="text-white text-[10px] font-bold">x{{ selectedToppings[topping.id] }}</span>
                  </div>
                </div>
                
                <div class="text-center w-full px-1 z-10 pb-1 flex flex-col items-center flex-1 justify-between">
                  <div>
                    <div class="text-[11px] sm:text-xs font-bold text-[#2A231E] leading-tight line-clamp-1">{{ topping.name }}</div>
                    <div class="text-[10px] font-bold text-[#CC8033] mt-0.5">+ {{ formatVND(topping.price) }}</div>
                  </div>
                  
                  <!-- Quantity Controls -->
                  <div v-if="(selectedToppings[topping.id] || 0) > 0" class="flex items-center justify-between w-full mt-2 px-1">
                    <button @click.stop="updateTopping(topping.id, -1)" class="w-6 h-6 rounded-full bg-white border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] hover:bg-[#F5F2ED] shadow-sm">
                      <Minus class="w-3 h-3" stroke-width="3" />
                    </button>
                    <span class="text-xs font-bold text-[#CC8033]">{{ selectedToppings[topping.id] }}</span>
                    <button @click.stop="updateTopping(topping.id, 1)" class="w-6 h-6 rounded-full bg-[#CC8033] flex items-center justify-center text-white hover:bg-[#B8722D] shadow-sm">
                      <Plus class="w-3 h-3" stroke-width="3" />
                    </button>
                  </div>
                  <button v-else @click.stop="updateTopping(topping.id, 1)" class="w-full mt-2 py-1.5 rounded-lg bg-[#F5F2ED] text-[#8A8178] text-[10px] font-bold uppercase tracking-wider hover:bg-[#EAE3D9] transition-colors border border-transparent">
                    Thêm
                  </button>
                </div>
              </div>
            </div>
          </div>

          <div v-if="selectedItem?.category !== 'pastry'" class="grid grid-cols-2 gap-6">
            <!-- Tùy chỉnh khác -->
            <div class="space-y-4">
              <h3 class="text-[11px] uppercase tracking-[0.15em] font-bold text-[#8A8178]">Lượng đường</h3>
              <div class="flex flex-wrap gap-2">
                <button
                  v-for="level in ['0%', '50%', '100%']"
                  :key="level"
                  type="button"
                  @click="selectedSugar = level"
                  class="px-4 py-2.5 rounded-xl border text-xs font-bold shadow-sm transition-all"
                  :class="selectedSugar === level ? 'bg-[#CC8033] border-[#CC8033] text-white' : 'bg-white border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033]'"
                >{{ level }}</button>
              </div>
            </div>
            
            <div class="space-y-4">
              <h3 class="text-[11px] uppercase tracking-[0.15em] font-bold text-[#8A8178]">Lượng đá</h3>
              <div class="flex flex-wrap gap-2">
                <button
                  v-for="level in ['0%', '50%', '100%']"
                  :key="level"
                  type="button"
                  @click="selectedIce = level"
                  class="px-4 py-2.5 rounded-xl border text-xs font-bold shadow-sm transition-all"
                  :class="selectedIce === level ? 'bg-[#CC8033] border-[#CC8033] text-white' : 'bg-white border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033]'"
                >{{ level }}</button>
              </div>
            </div>
          </div>

          <!-- Ghi chú thêm -->
          <div class="space-y-4 pt-2">
            <h3 class="text-[11px] uppercase tracking-[0.15em] font-bold text-[#8A8178] flex items-center gap-2">
              <svg class="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20h9"></path><path d="M16.5 3.5a2.12 2.12 0 0 1 3 3L7 19l-4 1 1-4Z"></path></svg>
              Ghi chú thêm
            </h3>
            <textarea
              v-model="itemNote"
              rows="2"
              placeholder="Ví dụ: Ít béo, không lấy thìa, thêm ống hút..."
              class="w-full p-4 rounded-2xl border border-[#EAE3D9] focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] bg-[#FAF6F0] outline-none text-sm font-medium text-[#2A231E] resize-none transition-colors shadow-inner"
            ></textarea>
          </div>
        </div>

        <!-- Footer Add to cart -->
        <div class="p-6 bg-white border-t border-[#EAE3D9] shadow-[0_-10px_30px_rgba(42,35,30,0.06)] shrink-0 flex items-center justify-center gap-4">
          <div class="flex w-full items-center gap-4">
            <div class="flex items-center bg-[#F5F2ED] rounded-2xl border border-[#EAE3D9] p-1.5 shadow-inner h-[60px]">
              <button @click="changeQuantity(-1)" :disabled="quantity <= 1" class="w-11 h-full rounded-xl flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-colors disabled:opacity-40 disabled:cursor-not-allowed">
                <Minus class="w-4 h-4" stroke-width="2.5" />
              </button>
              <span class="w-12 text-center text-lg font-bold text-[#2A231E]">{{ quantity }}</span>
              <button @click="changeQuantity(1)" class="w-11 h-full rounded-xl flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-colors">
                <Plus class="w-4 h-4" stroke-width="2.5" />
              </button>
            </div>
            <button @click="submitOptions" class="flex-1 h-[60px] rounded-2xl bg-[#D97724] hover:bg-[#C2661B] text-white flex items-center justify-center gap-3 shadow-xl transition-colors">
              <span class="text-sm font-bold uppercase tracking-widest">Đặt ngay</span>
              <span class="text-sm font-bold opacity-90">• {{ formatVND(((selectedItem?.price || 0) + currentOptionsTotalExtra) * quantity) }}</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Toast Notifications (合一 DESIGN) -->
    <div class="fixed top-6 left-1/2 -translate-x-1/2 z-[100] flex flex-col gap-4 pointer-events-none w-full max-w-[420px] px-4">
      <TransitionGroup name="toast">
        <div 
          v-for="t in toasts" 
          :key="t.id"
          class="flex items-start gap-4 p-5 rounded-3xl shadow-[0_20px_60px_rgba(74,50,36,0.25)] backdrop-blur-xl border pointer-events-auto relative overflow-hidden"
          :class="t.type === 'success' ? 'bg-[#4A3224]/95 border-[#5C4033] text-[#FDFBF7]' : 'bg-[#EF4444]/95 border-[#EF4444]/50 text-white'"
        >
          <!-- Gradient glow background -->
          <div v-if="t.type === 'success'" class="absolute -top-10 -right-10 w-32 h-32 bg-[#CC8033] rounded-full blur-[50px] opacity-20 pointer-events-none"></div>

          <!-- Icon -->
          <div class="w-12 h-12 rounded-2xl flex items-center justify-center shrink-0 shadow-inner relative z-10" :class="t.type === 'success' ? 'bg-gradient-to-br from-[#E8973D] to-[#CC8033]' : 'bg-white/20'">
            <Check v-if="t.type === 'success'" class="w-6 h-6 text-white" stroke-width="3" />
            <AlertCircle v-else class="w-6 h-6 text-white" stroke-width="3" />
          </div>

          <!-- Text -->
          <div class="flex-1 pt-0.5 relative z-10 pr-6">
            <h4 class="text-[15px] font-bold text-white tracking-wide mb-1">{{ t.title }}</h4>
            <p v-if="t.message" class="text-[13px] font-medium text-[#D5CEC4] leading-relaxed">{{ t.message }}</p>
          </div>

          <!-- Close -->
          <button @click="toasts = toasts.filter(x => x.id !== t.id)" class="absolute top-4 right-4 text-white/40 hover:text-white hover:bg-white/10 transition-colors p-1.5 rounded-full z-10">
            <X class="w-4 h-4" />
          </button>
        </div>
      </TransitionGroup>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ShoppingBag, Plus, Minus, Trash2, Coffee, X, ChevronLeft, ChevronRight, Gift, CheckCircle2, User, Check, AlertCircle, Search, Star, Settings2, Sparkles } from 'lucide-vue-next'
import { menuItems, categories, formatVND, type Category } from '@/data/menu'
import { useCartStore } from '@/stores/cart'
import { useOrderStore } from '@/stores/orders'
import { useStoreInfoStore } from '@/stores/storeInfo'
import Button from '@/components/ui/Button.vue'
import ChatbotWidget from '@/components/ChatbotWidget.vue'

const route = useRoute()
const router = useRouter()
const tableId = route.params.tableId || "5"
const activeCat = ref<Category | "all">("all")
const search = ref("")
const open = ref(false)
const openLoginSheet = ref(false)
const phoneNumber = ref('')
const customerName = ref('')
const cart         = useCartStore()
const orderStore   = useOrderStore()
const storeInfoStore = useStoreInfoStore()

const toasts = ref<{ id: number, title?: string, message: string, type: 'success' | 'error' }[]>([])
let toastId = 0
const toast = {
  success: (title: string, message: string = '') => {
    const id = toastId++
    toasts.value.push({ id, title, message, type: 'success' })
    setTimeout(() => {
      toasts.value = toasts.value.filter(t => t.id !== id)
    }, 4000)
  },
  error: (title: string, message: string = '') => {
    const id = toastId++
    toasts.value.push({ id, title, message, type: 'error' })
    setTimeout(() => {
      toasts.value = toasts.value.filter(t => t.id !== id)
    }, 4000)
  }
}

const customerPhone = ref('')
const usePoints = ref(false)

const selectedItem = ref<any>(null)
const itemOptionsOpen = ref(false)

const availableToppings = [
  { id: 'tran_chau_den', name: 'Trân châu đen', price: 10000, image: '/toppings/tran_chau_den.png' },
  { id: 'tran_chau_trang', name: 'Trân châu trắng', price: 15000, image: '/toppings/tran_chau_trang.png' },
  { id: 'thach_pho_mai', name: 'Thạch phô mai', price: 15000, image: '/toppings/thach_pho_mai.png' },
  { id: 'pudding', name: 'Pudding', price: 15000, image: '/toppings/pudding.png' },
  { id: 'thach_suong_sao', name: 'Thạch sương sáo', price: 10000, image: '/toppings/thach_suong_sao.png' },
]

const selectedToppings = ref<Record<string, number>>({})
const itemNote = ref('')
const selectedSize = ref('M')
const selectedSugar = ref('100%')
const selectedIce = ref('100%')
const quantity = ref(1)

const changeQuantity = (delta: number) => {
  quantity.value = Math.max(1, quantity.value + delta)
}

const currentOptionsTotalExtra = computed(() => {
  let extra = selectedSize.value === 'L' ? 10000 : 0
  for (const t of availableToppings) {
    if (selectedToppings.value[t.id]) {
      extra += (selectedToppings.value[t.id] || 0) * t.price
    }
  }
  return extra
})

const updateTopping = (id: string, delta: number) => {
  if (!selectedToppings.value[id]) selectedToppings.value[id] = 0
  if (selectedToppings.value[id] + delta >= 0) {
    selectedToppings.value[id] += delta
  }
}

const openItemOptions = (m: any) => {
  selectedItem.value = m
  selectedToppings.value = {} // reset toppings when opening
  itemNote.value = '' // reset note
  selectedSize.value = 'M'
  selectedSugar.value = '100%'
  selectedIce.value = '100%'
  quantity.value = 1
  itemOptionsOpen.value = true
}

const submitOptions = () => {
  const toppingsArr = []
  for (const t of availableToppings) {
    const qty = selectedToppings.value[t.id] || 0
    if (qty > 0) {
      toppingsArr.push({ name: t.name, price: t.price, qty })
    }
  }

  const options = {
    size: selectedSize.value,
    sugar: selectedSugar.value,
    ice: selectedIce.value,
    toppings: toppingsArr,
    note: itemNote.value,
    extraPrice: currentOptionsTotalExtra.value
  }

  cart.add(selectedItem.value, options)
  // Áp dụng số lượng đã chọn cho dòng vừa thêm
  const addedLine = cart.lines[cart.lines.length - 1]
  if (addedLine && quantity.value > 1) {
    cart.setQty(addedLine.cartLineId, quantity.value)
  }

  itemOptionsOpen.value = false
}

const loginBenefits = [
  { emoji: '🎁', label: 'Voucher 10%' },
  { emoji: '⭐', label: 'Tích điểm' },
  { emoji: '📋', label: 'Lịch sử đơn' },
  { emoji: '🎂', label: 'Ưu đãi sinh nhật' },
]

const submitLogin = () => {
  if (!customerName.value.trim() || phoneNumber.value.length < 9) return
  customerPhone.value = phoneNumber.value
  toast.success('Đã tích điểm thành viên', `Chào ${customerName.value.trim()}! Điểm sẽ được cộng vào đơn này.`)
  openLoginSheet.value = false
}

// --- LOGIC LỌC VÀ PHÂN TRANG ---
const itemsPerPage = 8
const currentPage = ref(1)

const filtered = computed(() => {
  let items = activeCat.value === "all"
    ? menuItems
    : menuItems.filter((m) => m.category === activeCat.value)
  const q = search.value.trim().toLowerCase()
  if (q) {
    items = items.filter((m) =>
      m.name.toLowerCase().includes(q) ||
      (m.description?.toLowerCase().includes(q) ?? false)
    )
  }
  return items
})

const totalPages = computed(() => Math.ceil(filtered.value.length / itemsPerPage))

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filtered.value.slice(start, end)
})

// Reset về trang 1 khi đổi danh mục hoặc tìm kiếm
watch([activeCat, search], () => {
  currentPage.value = 1
})

const nextPage = () => {
  if (currentPage.value < totalPages.value) currentPage.value++
}
const prevPage = () => {
  if (currentPage.value > 1) currentPage.value--
}
const goToPage = (page: number) => {
  currentPage.value = page
}
// -------------------------------

const addToCart = (m: any) => {
  cart.add(m)
}

const handleOrder = () => {
  if (cart.lines.length === 0) return

  // Chuyển giỏ hàng thành các dòng món của đơn (gộp topping/size vào ghi chú)
  const items = cart.lines.map(l => {
    const opt = l.options
    const noteParts: string[] = []
    if (opt?.size) noteParts.push(`Size ${opt.size}`)
    if (opt?.sugar) noteParts.push(`Đường ${opt.sugar}`)
    if (opt?.ice) noteParts.push(`Đá ${opt.ice}`)
    if (opt?.toppings?.length) noteParts.push(opt.toppings.map(t => `${t.name}${t.qty > 1 ? ' x' + t.qty : ''}`).join(', '))
    if (opt?.note) noteParts.push(opt.note)
    return {
      name: l.item.name,
      qty: l.qty,
      price: l.item.price + (opt?.extraPrice || 0),
      note: noteParts.join(' · ') || undefined,
    }
  })

  // Tạo đơn thật trong store → đơn này sẽ xuất hiện ở Bếp và trang Đơn hàng
  const order = orderStore.createOrder({
    table: `Bàn ${tableId}`,
    items,
    customer: customerName.value || undefined,
  })

  toast.success('Gửi đơn thành công', `Đơn ${order.id} đang được pha chế cho Bàn ${tableId}`)
  cart.clear()
  open.value = false
  setTimeout(() => router.push(`/payment/${order.id}`), 1000)
}
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans {
  font-family: 'Be Vietnam Pro', system-ui, sans-serif;
}

.custom-scrollbar::-webkit-scrollbar {
  width: 3px;
  height: 3px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: rgba(42, 35, 30, 0.1);
  border-radius: 4px;
}

/* Bottom sheet transitions */
.sheet-backdrop-enter-active,
.sheet-backdrop-leave-active {
  transition: opacity 0.3s ease;
}
.sheet-backdrop-enter-from,
.sheet-backdrop-leave-to {
  opacity: 0;
}

.sheet-slide-enter-active,
.sheet-slide-leave-active {
  transition: transform 0.35s cubic-bezier(0.32, 0.72, 0, 1);
}
.sheet-slide-enter-from,
.sheet-slide-leave-to {
  transform: translateY(100%);
}

/* Login modal (fade + pop) */
.login-modal-enter-active,
.login-modal-leave-active {
  transition: opacity 0.25s ease;
}
.login-modal-enter-from,
.login-modal-leave-to {
  opacity: 0;
}
.login-modal-enter-active .relative,
.login-modal-leave-active .relative {
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.login-modal-enter-from .relative,
.login-modal-leave-to .relative {
  transform: scale(0.94) translateY(12px);
}

/* Toast transitions */
.toast-enter-active,
.toast-leave-active {
  transition: all 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}
.toast-enter-from {
  opacity: 0;
  transform: translateY(-20px) scale(0.9);
}
.toast-leave-to {
  opacity: 0;
  transform: translateY(-20px) scale(0.9);
}
</style>
