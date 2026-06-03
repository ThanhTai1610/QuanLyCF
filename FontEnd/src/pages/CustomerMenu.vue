<template>
  <div class="min-h-screen bg-[#FDFBF7] pb-40 font-premium-sans text-[#2A231E]">
   
    <!-- Top bar -->
    <header class="sticky top-0 z-30 bg-[#FDFBF7]/80 backdrop-blur-xl border-b border-[#EAE3D9] shadow-card">
      <div class="max-w-[1200px] mx-auto px-4 sm:px-6 h-16 flex items-center justify-between">
       
        <!-- Logo -->
        <router-link to="/" class="flex items-center gap-3">
          <div class="w-10 h-10 flex items-center justify-center bg-[#2A231E] text-[#FDFBF7] rounded-lg shadow-xl">
            <Coffee class="w-5 h-5" stroke-width="1.5" />
          </div>
          <span class="font-premium-serif text-2xl font-bold tracking-wide text-[#2A231E]">BrewManager</span>
        </router-link>
        
        <!-- Right actions -->
        <div class="flex items-center gap-5">
          <div class="hidden sm:flex items-center gap-2 bg-white px-3 py-1.5 rounded-lg border border-[#EAE3D9] shadow-xl">
            <span class="w-2 h-2 rounded-full bg-[#CC8033]"></span>
            <span class="text-[10px] uppercase tracking-[0.2em] font-semibold text-[#5C544E]">Bàn số {{ tableId }}</span>
          </div>
         
          <button @click="open = true" class="relative w-11 h-11 bg-white rounded-lg border border-[#EAE3D9] shadow-xl flex items-center justify-center text-[#2A231E]">
            <ShoppingBag class="w-5 h-5" stroke-width="1.5" />
            <span v-if="cart.count() > 0" class="absolute -top-1 -right-1 w-5 h-5 rounded-lg bg-[#CC8033] text-white text-[10px] font-bold flex items-center justify-center shadow-xl border border-[#FDFBF7]">
              {{ cart.count() }}
            </span>
          </button>
        </div>
      </div>
    </header>

    <!-- Hero / Tiêu đề -->
    <section class="max-w-[1200px] mx-auto px-4 sm:px-6 pt-12 pb-8 text-center sm:text-left">
      <div class="flex flex-col sm:flex-row sm:items-end justify-between gap-6">
        <div>
          <h2 class="font-premium-serif text-5xl sm:text-6xl font-semibold text-[#1A1512] mb-3">Thực đơn</h2>
          <div class="w-16 h-1.5 rounded-lg bg-[#CC8033] mx-auto sm:mx-0 mb-4"></div>
          <p class="text-xs text-[#5C544E] uppercase tracking-[0.2em] font-medium">Khám phá hương vị tinh hoa tại bàn {{ tableId }}</p>
        </div>
      </div>
    </section>

    <!-- Loyalty Banner -->
    <div class="max-w-[1200px] mx-auto px-4 sm:px-6 mb-6">
      <button
        @click="openLoginSheet = true"
        class="w-full flex items-center justify-between gap-4 px-5 py-4 rounded-xl bg-gradient-to-r from-[#CC8033] to-[#E8973D] text-white shadow-[0_8px_24px_rgba(204,128,51,0.25)] border border-[#CC8033]/30 hover:shadow-[0_12px_32px_rgba(204,128,51,0.35)] transition-all duration-300 active:scale-[0.99] group"
      >
        <div class="flex items-center gap-3">
          <div class="w-9 h-9 rounded-lg bg-white/20 flex items-center justify-center border border-white/30 shrink-0">
            <Gift class="w-4.5 h-4.5 text-white" stroke-width="2" />
          </div>
          <div class="text-left">
            <p class="text-xs font-bold uppercase tracking-[0.1em]">🎉 Đăng nhập để tích điểm &amp; nhận voucher 10%</p>
            <p class="text-[10px] text-white/75 font-medium mt-0.5">Áp dụng ngay cho đơn hàng hôm nay</p>
          </div>
        </div>
        <ChevronRight class="w-4 h-4 text-white/80 group-hover:translate-x-0.5 transition-transform duration-300 shrink-0" stroke-width="2.5" />
      </button>
    </div>

    <!-- Categories Tabs -->
    <nav class="sticky top-16 z-20 bg-[#FDFBF7]/95 backdrop-blur-md border-b border-[#EAE3D9] py-2">
      <div class="max-w-[1200px] mx-auto px-4 sm:px-6 flex gap-3 overflow-x-auto custom-scrollbar pb-2">
        <button
          v-for="c in categories"
          :key="c.id"
          @click="activeCat = c.id as any"
          :class="[
            'px-5 py-2.5 rounded-lg whitespace-nowrap text-[11px] uppercase tracking-[0.1em] font-bold border shadow-xl',
            activeCat === c.id
              ? 'bg-[#CC8033] text-white border-[#CC8033]'
              : 'bg-white border-[#EAE3D9] text-[#8A8178]'
          ]"
        >
          {{ c.label }}
        </button>
      </div>
    </nav>

    <!-- Menu grid -->
    <main class="max-w-[1200px] mx-auto px-4 sm:px-6 mt-8">
      <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-6">
        <article
          v-for="m in paginatedItems"
          :key="m.id"
          class="bg-white rounded-lg border border-[#EAE3D9] shadow-card flex flex-col relative overflow-hidden"
        >
          <!-- Image Container (Clickable) -->
          <div @click="openItemOptions(m)" class="relative aspect-[4/3] overflow-hidden bg-[#F5F2ED] cursor-pointer group">
            <img
              :src="m.image"
              :alt="m.name"
              loading="lazy"
              class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
            />
            <!-- Gradient Overlay -->
            <div class="absolute inset-0 bg-gradient-to-t from-black/10 to-transparent"></div>
           
            <div v-if="m.popular" class="absolute top-3 left-3 px-3 py-1.5 rounded-lg bg-white/95 backdrop-blur-md text-[#CC8033] text-[9px] uppercase tracking-[0.2em] font-bold shadow-xl">
              Nổi bật
            </div>
          </div>
         
          <!-- Content -->
          <div class="p-4 sm:p-5 flex flex-col flex-1 bg-white relative">
            <h3 class="font-premium-serif text-xl font-bold leading-snug text-[#2A231E] cursor-pointer hover:text-[#CC8033] transition-colors" @click="openItemOptions(m)">{{ m.name }}</h3>
            <p class="text-[11px] text-[#8A8178] mt-2 line-clamp-2 leading-relaxed flex-1 font-medium">{{ m.description }}</p>
           
            <div class="flex flex-wrap items-center justify-between gap-y-2 mt-5 pt-4 border-t border-[#EAE3D9]/60">
              <span class="text-[#CC8033] font-bold text-base tracking-wide">{{ formatVND(m.price) }}</span>
              <div class="flex items-center gap-1.5 sm:gap-2">
                <button 
                  @click="openItemOptions(m)" 
                  class="h-9 px-3 rounded-xl bg-[#FDFBF7] border border-[#EAE3D9] text-[#8A8178] text-[10px] font-bold uppercase tracking-widest hover:bg-[#F5F2ED] hover:text-[#CC8033] hover:border-[#CC8033]/30 transition-all shadow-sm"
                >
                  Tùy chỉnh
                </button>
                <div v-if="(cart.lines.find(l => l.item.id === m.id)?.qty || 0) > 0" class="flex items-center bg-[#FDFBF7] rounded-xl border border-[#EAE3D9] p-0.5 shadow-sm h-9">
                  <button @click="cart.setQty(m.id, cart.lines.find(l => l.item.id === m.id)!.qty - 1)" class="w-8 h-full rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-all">
                    <Minus class="w-3.5 h-3.5" stroke-width="2.5" />
                  </button>
                  <span class="w-8 text-center text-sm font-bold text-[#2A231E]">
                    {{ cart.lines.find(l => l.item.id === m.id)?.qty }}
                  </span>
                  <button @click="cart.setQty(m.id, cart.lines.find(l => l.item.id === m.id)!.qty + 1)" class="w-8 h-full rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-all">
                    <Plus class="w-3.5 h-3.5" stroke-width="2.5" />
                  </button>
                </div>
                <button
                  v-else
                  @click="addToCart(m)"
                  class="w-9 h-9 rounded-xl bg-[#F5F2ED] border border-[#EAE3D9] flex items-center justify-center text-[#2A231E] shadow-sm hover:bg-[#CC8033] hover:text-white hover:border-[#CC8033] transition-colors"
                  title="Thêm vào giỏ"
                >
                  <Plus class="w-4 h-4" stroke-width="2" />
                </button>
              </div>
            </div>
          </div>
        </article>
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
    <div v-if="cart.count() > 0" class="fixed bottom-6 left-0 right-0 z-30 px-4 sm:px-6">
      <div class="max-w-[1200px] mx-auto flex justify-center sm:justify-end">
        <button
          @click="open = true"
          class="bg-[#2A231E]/95 backdrop-blur-xl text-[#FDFBF7] rounded-lg p-3 sm:px-5 sm:py-3 flex items-center gap-6 sm:gap-10 shadow-card border border-white/10"
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

    <!-- Slide-Over Giỏ Hàng (合一 DESIGN) -->
    <Transition name="slide-right">
      <div v-if="open" class="fixed inset-0 z-50 flex justify-end" @click.self="open = false">
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-[#1A1512]/60 backdrop-blur-md" @click="open = false"></div>
        
        <!-- Slide-Over Content -->
        <div class="relative w-full max-w-md bg-[#F5F2ED] h-full shadow-[-20px_0_60px_rgba(42,35,30,0.15)] flex flex-col border-l border-[#EAE3D9] z-10 animate-in slide-in-from-right duration-300">
          
          <!-- Slide-Over Header -->
          <div class="px-6 py-5 border-b border-[#EAE3D9] bg-white flex flex-col gap-4">
            <div class="flex justify-between items-start">
              <h2 class="font-premium-serif text-2xl font-bold text-[#2A231E]">Giỏ hàng & Thanh toán <br/> <span class="text-sm font-sans text-[#8A8178] uppercase tracking-widest font-bold">Bàn {{ tableId }}</span></h2>
              <button @click="open = false" class="w-9 h-9 rounded-full bg-[#F5F2ED] border border-[#EAE3D9] flex items-center justify-center text-[#8A8178] shadow-sm hover:text-[#2A231E]">
                <X class="w-4 h-4" stroke-width="2.5" />
              </button>
            </div>
            
            <!-- Hạt Badge Thông tin Khách hàng & Loyalty (Hover to expand) -->
            <div class="group relative bg-[#FDFBF7] border border-[#EAE3D9] rounded-xl p-3 shadow-sm cursor-pointer hover:border-[#CC8033]/30 transition-colors">
              <div class="flex justify-between items-center">
                <div class="flex items-center gap-2">
                  <div class="w-8 h-8 rounded-full bg-[#CC8033]/15 text-[#CC8033] flex items-center justify-center">
                    <User class="w-4 h-4" />
                  </div>
                  <div>
                    <div class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider">Thành viên thân thiết</div>
                    <div class="text-sm font-bold text-[#2A231E]">Nguyễn Văn A 👋</div>
                  </div>
                </div>
                <div class="flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-[#FFF9F2] border border-[#E8C5A5] text-[#CC8033] text-xs font-bold shadow-inner">
                  <Coffee class="w-3 h-3" /> 150 điểm
                </div>
              </div>
              
              <!-- Collapsible Info Card -->
              <div class="mt-3 pt-3 border-t border-dashed border-[#EAE3D9] text-[11px] font-medium text-[#5C544E] hidden group-hover:block animate-in fade-in slide-in-from-top-2">
                <div class="flex items-center justify-between mb-1.5">
                  <span class="flex items-center gap-1.5"><Gift class="w-3.5 h-3.5 text-[#CC8033]" /> Hạng thẻ hiện tại</span>
                  <span class="text-xs font-bold text-[#CC8033]">Vàng (Gold)</span>
                </div>
                <div class="flex items-center justify-between">
                  <span class="text-[#8A8178]">Ưu đãi tháng sinh nhật:</span>
                  <span class="font-bold text-success">Giảm 20% hóa đơn</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Slide-Over Body -->
          <div class="flex-1 overflow-y-auto p-6 space-y-4 bg-[#F5F2ED] custom-scrollbar">
            <!-- Items -->
            <div v-if="cart.lines.length === 0" class="text-center py-20 flex flex-col items-center">
              <div class="w-20 h-20 rounded-lg border border-dashed border-[#EAE3D9] flex items-center justify-center mb-6 bg-white shadow-xl">
                <ShoppingBag class="w-8 h-8 text-[#D5CEC4]" stroke-width="1.5" />
              </div>
              <p class="font-premium-serif text-2xl font-bold text-[#2A231E]">Chưa có món nào</p>
              <p class="text-xs text-[#8A8178] mt-2 font-medium">Vui lòng chọn món từ thực đơn để tiếp tục.</p>
            </div>
            
            <div v-for="l in cart.lines" :key="l.cartLineId" class="flex gap-4 p-3 rounded-2xl bg-white shadow-sm border border-[#EAE3D9] relative">
              <!-- Product Image -->
              <div class="w-20 h-20 rounded-xl overflow-hidden flex-shrink-0 shadow-inner">
                <img :src="l.item.image" :alt="l.item.name" class="w-full h-full object-cover" />
              </div>
              <!-- Content -->
              <div class="flex-1 min-w-0 flex flex-col justify-between">
                <div class="flex justify-between items-start gap-2 pr-6">
                  <div>
                    <h4 class="font-bold text-sm text-[#2A231E] leading-tight">{{ l.item.name }}</h4>
                    <!-- Display Options -->
                    <div v-if="l.options" class="mt-1.5 space-y-0.5">
                      <div class="text-[10px] text-[#8A8178] leading-tight" v-if="l.options.size !== 'M' || l.options.sugar !== '100%' || l.options.ice !== '100%'">
                        <span class="font-bold text-[#5C544E]">Size:</span> {{ l.options.size }} 
                        <span class="mx-1 text-[#EAE3D9]">|</span>
                        <span class="font-bold text-[#5C544E]">Đá/Đường:</span> {{ l.options.ice }}, {{ l.options.sugar }}
                      </div>
                      <div v-for="t in l.options.toppings" :key="t.name" class="text-[10px] text-[#8A8178] leading-tight">
                        <span class="font-bold text-[#CC8033]">+ {{ t.name }}</span> (x{{ t.qty }})
                      </div>
                      <div v-if="l.options.note" class="text-[10px] text-[#D97724] italic mt-1 break-words bg-[#FFF9F2] p-1.5 rounded-lg border border-[#CC8033]/20">
                        "{{ l.options.note }}"
                      </div>
                    </div>
                  </div>
                  <button @click="cart.remove(l.cartLineId)" class="absolute top-2 right-2 text-[#D5CEC4] hover:text-red-400 p-1.5 transition-colors">
                    <Trash2 class="w-4 h-4" stroke-width="2" />
                  </button>
                </div>
                <p class="text-[#CC8033] font-bold text-xs mt-1">{{ formatVND(l.item.price + (l.options?.extraPrice || 0)) }}</p>
                
                <!-- Qty controls -->
                <div class="flex items-center justify-between mt-2">
                  <div class="flex items-center bg-[#FDFBF7] rounded-xl border border-[#EAE3D9] p-0.5">
                    <button @click="cart.setQty(l.cartLineId, l.qty - 1)" class="w-7 h-7 rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm">
                      <Minus class="w-3 h-3" stroke-width="2.5" />
                    </button>
                    <span class="w-8 text-center text-xs font-bold text-[#2A231E]">{{ l.qty }}</span>
                    <button @click="cart.setQty(l.cartLineId, l.qty + 1)" class="w-7 h-7 rounded-lg flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm">
                      <Plus class="w-3 h-3" stroke-width="2.5" />
                    </button>
                  </div>
                  <span class="text-sm text-[#2A231E] font-bold">{{ formatVND((l.item.price + (l.options?.extraPrice || 0)) * l.qty) }}</span>
                </div>
              </div>
            </div>

            <!-- Tích điểm & Dùng điểm -->
            <div v-if="cart.lines.length > 0" class="mt-6 bg-white p-4 rounded-2xl border border-[#EAE3D9] shadow-sm">
              <div v-if="!customerPhone" class="space-y-3">
                <label class="text-xs font-bold uppercase tracking-wider text-[#8A8178] flex items-center gap-2">
                  <Phone class="w-3.5 h-3.5 text-[#CC8033]" /> Tích điểm thành viên
                </label>
                <div class="relative flex gap-2">
                  <input type="tel" v-model="tempPhone" @keyup.enter="applyCustomerPhone" placeholder="📱 Nhập SĐT để tích điểm" class="w-full bg-[#F5F2ED] border border-[#EAE3D9] rounded-xl h-11 px-4 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033] placeholder:font-medium placeholder:text-[#8A8178]" />
                  <button @click="applyCustomerPhone" class="h-11 px-4 bg-[#CC8033] text-white rounded-xl text-xs font-bold uppercase tracking-wider hover:bg-[#B87029] shrink-0">
                    Gửi
                  </button>
                </div>
              </div>
              <div v-else class="space-y-3">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-2">
                    <div class="w-8 h-8 rounded-full bg-[#CC8033]/15 text-[#CC8033] flex items-center justify-center">
                      <Phone class="w-4 h-4" />
                    </div>
                    <div>
                      <div class="text-[10px] font-bold text-[#8A8178] uppercase tracking-wider">Khách hàng</div>
                      <div class="text-sm font-bold text-[#2A231E]">{{ customerPhone }}</div>
                    </div>
                  </div>
                  <button @click="customerPhone = ''; usePoints = false" class="text-[10px] text-red-400 font-bold uppercase hover:underline">Hủy</button>
                </div>
                <div class="border-t border-dashed border-[#EAE3D9] pt-3 flex items-center gap-3">
                  <input type="checkbox" v-model="usePoints" id="usePoints" class="w-5 h-5 rounded border-[#EAE3D9] text-[#CC8033] focus:ring-[#CC8033]" />
                  <label for="usePoints" class="text-xs font-bold text-[#5C544E] cursor-pointer flex-1">
                    Dùng 50 điểm <span class="text-[#CC8033]">(giảm 20.000đ)</span>
                  </label>
                </div>
              </div>
            </div>
          </div>

          <!-- Slide-Over Footer -->
          <div v-if="cart.lines.length > 0" class="p-6 bg-[#FDFBF7] border-t border-[#EAE3D9] shadow-[0_-10px_30px_rgba(42,35,30,0.05)] rounded-bl-xl">
            <div class="space-y-2 mb-5">
              <div class="flex justify-between items-center text-sm">
                <span class="text-[#8A8178] font-bold">Tổng tạm tính</span>
                <span class="font-bold text-[#5C544E]">{{ formatVND(cart.total()) }}</span>
              </div>
              <div v-if="usePoints" class="flex justify-between items-center text-sm">
                <span class="text-[#8A8178] font-bold flex items-center gap-1.5"><Gift class="w-3.5 h-3.5" /> Điểm thưởng</span>
                <span class="font-bold text-[#E85D04]">- 20.000 ₫</span>
              </div>
              <div class="border-t border-dashed border-[#EAE3D9] pt-2 mt-2 flex justify-between items-end">
                <span class="text-xs font-bold uppercase tracking-widest text-[#2A231E]">Tổng thanh toán</span>
                <span class="font-sans text-3xl font-bold text-[#2A231E] leading-none">{{ formatVND(cart.total() - (usePoints ? 20000 : 0)) }}</span>
              </div>
            </div>
            <button
              @click="handleOrder"
              class="w-full h-14 bg-[#D97724] hover:bg-[#C2661B] text-white text-sm uppercase tracking-widest font-bold rounded-2xl shadow-xl transition-all"
            >
              THANH TOÁN (HOẶC GỬI YÊU CẦU)
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Chatbot Widget -->
    <ChatbotWidget />

    <!-- Customer Login Bottom Sheet -->
    <Transition name="sheet-backdrop">
      <div
        v-if="openLoginSheet"
        class="fixed inset-0 z-[60] bg-[#1A1512]/50 backdrop-blur-sm"
        @click.self="openLoginSheet = false"
      ></div>
    </Transition>

    <Transition name="sheet-slide">
      <div
        v-if="openLoginSheet"
        class="fixed bottom-0 left-0 right-0 z-[61] bg-white rounded-t-[2rem] shadow-[0_-20px_60px_rgba(42,35,30,0.15)] border-t border-[#EAE3D9]"
      >
        <!-- Handle bar -->
        <div class="flex justify-center pt-4 pb-2">
          <div class="w-10 h-1 rounded-full bg-[#EAE3D9]"></div>
        </div>

        <div class="px-6 pb-10 pt-4 max-w-md mx-auto">
          <!-- Header -->
          <div class="flex items-start justify-between mb-6">
            <div>
              <h3 class="font-premium-serif text-2xl font-bold text-[#1A1512]">Đăng nhập nhanh</h3>
              <p class="text-[11px] text-[#8A8178] mt-1 font-medium">Tích điểm · Voucher · Lịch sử đơn hàng</p>
            </div>
            <button @click="openLoginSheet = false" class="w-9 h-9 rounded-lg bg-[#F5F2ED] flex items-center justify-center text-[#8A8178] border border-[#EAE3D9]">
              <X class="w-4 h-4" stroke-width="2" />
            </button>
          </div>

          <!-- Benefits strip -->
          <div class="flex gap-3 mb-6 overflow-x-auto pb-1">
            <div v-for="b in loginBenefits" :key="b.label" class="flex items-center gap-2 px-3 py-2 rounded-lg bg-[#FAF6F0] border border-[#EAE3D9] whitespace-nowrap shrink-0">
              <span class="text-base">{{ b.emoji }}</span>
              <span class="text-[10px] font-bold text-[#5C544E] uppercase tracking-tight">{{ b.label }}</span>
            </div>
          </div>

          <!-- Quick login buttons -->
          <div class="space-y-3 mb-5">
            <!-- Zalo -->
            <button
              @click="loginWith('zalo')"
              class="w-full h-14 flex items-center justify-center gap-3 rounded-xl font-bold text-sm text-white transition-all duration-200 active:scale-[0.98] shadow-md"
              style="background: linear-gradient(135deg, #0068ff 0%, #0085ff 100%);"
            >
              <!-- Zalo icon -->
              <svg class="w-6 h-6" viewBox="0 0 40 40" fill="none" xmlns="http://www.w3.org/2000/svg">
                <rect width="40" height="40" rx="8" fill="white" fill-opacity="0.15"/>
                <text x="5" y="28" font-size="20" font-weight="900" fill="white" font-family="Arial">Z</text>
              </svg>
              <span class="tracking-wide">Đăng nhập nhanh qua Zalo</span>
            </button>

            <!-- Google -->
            <button
              @click="loginWith('google')"
              class="w-full h-14 flex items-center justify-center gap-3 rounded-xl font-bold text-sm bg-white border-2 border-[#EAE3D9] text-[#2A231E] hover:border-[#D5CEC4] hover:bg-[#FDFBF7] transition-all duration-200 active:scale-[0.98] shadow-sm"
            >
              <svg class="w-5 h-5" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
                <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
              </svg>
              <span class="tracking-wide">Tiếp tục với Google</span>
            </button>
          </div>

          <!-- Divider -->
          <div class="flex items-center gap-3 mb-5">
            <div class="flex-1 h-px bg-[#EAE3D9]"></div>
            <span class="text-[9px] uppercase tracking-widest text-[#C5BEB8] font-bold">hoặc nhập SĐT</span>
            <div class="flex-1 h-px bg-[#EAE3D9]"></div>
          </div>

          <!-- Phone OTP -->
          <div v-if="!otpSent" class="flex gap-2">
            <div class="relative flex-1">
              <span class="absolute left-3 top-1/2 -translate-y-1/2 text-sm font-bold text-[#8A8178]">🇻🇳 +84</span>
              <input
                v-model="phoneNumber"
                type="tel"
                placeholder="9xx xxx xxx"
                maxlength="10"
                class="w-full h-12 pl-16 pr-4 rounded-xl border-2 border-[#EAE3D9] focus:border-[#CC8033] focus:outline-none text-sm font-medium bg-[#FAF6F0] transition-colors duration-200"
              />
            </div>
            <button
              @click="sendOtp"
              :disabled="phoneNumber.length < 9"
              class="h-12 px-4 rounded-xl bg-[#CC8033] text-white text-xs font-bold uppercase tracking-wide disabled:opacity-40 disabled:cursor-not-allowed hover:bg-[#B8722D] transition-colors duration-200 shrink-0 whitespace-nowrap"
            >
              Gửi mã
            </button>
          </div>

          <!-- OTP input after sending -->
          <div v-else class="space-y-3">
            <p class="text-[11px] text-[#5C544E] font-medium text-center">Nhập mã OTP đã gửi đến <span class="font-bold text-[#CC8033]">+84 {{ phoneNumber }}</span></p>
            <div class="flex gap-2 justify-center">
              <input
                v-for="i in 6"
                :key="i"
                type="text"
                maxlength="1"
                class="w-10 h-12 text-center text-lg font-bold border-2 border-[#EAE3D9] focus:border-[#CC8033] rounded-lg focus:outline-none bg-[#FAF6F0] transition-colors duration-200"
              />
            </div>
            <button
              @click="otpSent = false"
              class="w-full text-[10px] text-[#8A8178] font-bold uppercase tracking-widest text-center"
            >
              Đổi số điện thoại
            </button>
          </div>

          <p class="text-center text-[9px] text-[#C5BEB8] font-medium mt-5">Bỏ qua để tiếp tục xem thực đơn mà không cần đăng nhập</p>
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
              <label class="flex items-center gap-3 p-4 rounded-2xl border-2 cursor-pointer shadow-sm relative overflow-hidden group transition-all" :class="selectedSize === 'M' ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white hover:border-[#CC8033]/50'">
                <input type="radio" v-model="selectedSize" value="M" class="sr-only" />
                <div class="w-5 h-5 rounded-full border-2 flex items-center justify-center" :class="selectedSize === 'M' ? 'border-[#CC8033]' : 'border-[#EAE3D9]'">
                  <div class="w-2.5 h-2.5 rounded-full bg-[#CC8033] transition-transform" :class="selectedSize === 'M' ? 'scale-100' : 'scale-0'"></div>
                </div>
                <span class="text-sm font-bold text-[#2A231E]">Vừa (M) <span class="block text-[#8A8178] text-xs font-medium mt-0.5">+ 0đ</span></span>
                <div v-if="selectedSize === 'M'" class="absolute right-0 bottom-0 w-12 h-12 bg-[#CC8033]/5 rounded-tl-full transition-transform group-hover:scale-110"></div>
              </label>
              
              <label class="flex items-center gap-3 p-4 rounded-2xl border-2 cursor-pointer shadow-sm relative overflow-hidden group transition-all" :class="selectedSize === 'L' ? 'border-[#CC8033] bg-[#FFF9F2]' : 'border-[#EAE3D9] bg-white hover:border-[#CC8033]/50'">
                <input type="radio" v-model="selectedSize" value="L" class="sr-only" />
                <div class="w-5 h-5 rounded-full border-2 flex items-center justify-center" :class="selectedSize === 'L' ? 'border-[#CC8033]' : 'border-[#EAE3D9]'">
                  <div class="w-2.5 h-2.5 rounded-full bg-[#CC8033] transition-transform" :class="selectedSize === 'L' ? 'scale-100' : 'scale-0'"></div>
                </div>
                <span class="text-sm font-bold text-[#2A231E]">Lớn (L) <span class="block text-[#CC8033] text-xs font-bold mt-0.5">+ 10.000đ</span></span>
                <div v-if="selectedSize === 'L'" class="absolute right-0 bottom-0 w-12 h-12 bg-[#CC8033]/5 rounded-tl-full transition-transform group-hover:scale-110"></div>
              </label>
            </div>
          </div>

          <!-- Topping -->
          <div class="space-y-4">
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

          <div class="grid grid-cols-2 gap-6">
            <!-- Tùy chỉnh khác -->
            <div class="space-y-4">
              <h3 class="text-[11px] uppercase tracking-[0.15em] font-bold text-[#8A8178]">Lượng đường</h3>
              <div class="flex flex-wrap gap-2">
                <label v-for="level in ['0%', '50%', '100%']" :key="level" class="cursor-pointer">
                  <input type="radio" v-model="selectedSugar" :value="level" class="sr-only" />
                  <span class="px-4 py-2.5 rounded-xl border text-xs font-bold shadow-sm transition-all inline-block" :class="selectedSugar === level ? 'bg-[#CC8033] border-[#CC8033] text-white' : 'bg-white border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033]'">{{ level }}</span>
                </label>
              </div>
            </div>
            
            <div class="space-y-4">
              <h3 class="text-[11px] uppercase tracking-[0.15em] font-bold text-[#8A8178]">Lượng đá</h3>
              <div class="flex flex-wrap gap-2">
                <label v-for="level in ['0%', '50%', '100%']" :key="level" class="cursor-pointer">
                  <input type="radio" v-model="selectedIce" :value="level" class="sr-only" />
                  <span class="px-4 py-2.5 rounded-xl border text-xs font-bold shadow-sm transition-all inline-block" :class="selectedIce === level ? 'bg-[#CC8033] border-[#CC8033] text-white' : 'bg-white border-[#EAE3D9] text-[#5C544E] hover:border-[#CC8033]'">{{ level }}</span>
                </label>
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
              <button class="w-11 h-full rounded-xl flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-colors">
                <Minus class="w-4 h-4" stroke-width="2.5" />
              </button>
              <span class="w-12 text-center text-lg font-bold text-[#2A231E]">1</span>
              <button class="w-11 h-full rounded-xl flex items-center justify-center text-[#5C544E] hover:bg-white hover:shadow-sm transition-colors">
                <Plus class="w-4 h-4" stroke-width="2.5" />
              </button>
            </div>
            <button @click="submitOptions" class="flex-1 h-[60px] rounded-2xl bg-[#D97724] hover:bg-[#C2661B] text-white flex items-center justify-center gap-3 shadow-xl transition-colors">
              <span class="text-sm font-bold uppercase tracking-widest">Thêm vào giỏ</span>
              <span class="text-sm font-bold opacity-90">• {{ formatVND((selectedItem?.price || 0) + currentOptionsTotalExtra) }}</span>
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
          class="flex items-start gap-4 p-5 rounded-3xl shadow-[0_20px_60px_rgba(42,35,30,0.25)] backdrop-blur-xl border pointer-events-auto relative overflow-hidden"
          :class="t.type === 'success' ? 'bg-[#2A231E]/95 border-[#4A3F35] text-[#FDFBF7]' : 'bg-[#EF4444]/95 border-[#EF4444]/50 text-white'"
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
import { ShoppingBag, Plus, Minus, Trash2, Coffee, X, ChevronLeft, ChevronRight, Gift, CheckCircle2, User, Phone, Check, AlertCircle } from 'lucide-vue-next'
import { menuItems, categories, formatVND, type Category } from '@/data/menu'
import { useCartStore } from '@/stores/cart'
import Button from '@/components/ui/Button.vue'
import ChatbotWidget from '@/components/ChatbotWidget.vue'

const route = useRoute()
const router = useRouter()
const tableId = route.params.tableId || "5"
const activeCat = ref<Category | "all">("all")
const open = ref(false)
const openLoginSheet = ref(false)
const phoneNumber = ref('')
const otpSent = ref(false)
const cart = useCartStore()

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
const tempPhone = ref('')
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

const currentOptionsTotalExtra = computed(() => {
  let extra = selectedSize.value === 'L' ? 10000 : 0
  for (const t of availableToppings) {
    if (selectedToppings.value[t.id]) {
      extra += selectedToppings.value[t.id] * t.price
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
  
  const customNames = options.toppings.map(t => t.name).join(', ')
  const details = `Size ${options.size}${customNames ? ' • Thêm ' + customNames : ''}`
  toast.success('Đã thêm vào giỏ', `${selectedItem.value.name} (${details})`)
  
  itemOptionsOpen.value = false
}

const applyCustomerPhone = () => {
  if (tempPhone.value.trim().length >= 9) {
    customerPhone.value = tempPhone.value
    tempPhone.value = ''
  } else {
    alert('Vui lòng nhập SĐT hợp lệ')
  }
}

const loginBenefits = [
  { emoji: '🎁', label: 'Voucher 10%' },
  { emoji: '⭐', label: 'Tích điểm' },
  { emoji: '📋', label: 'Lịch sử đơn' },
  { emoji: '🎂', label: 'Ưu đãi sinh nhật' },
]

const loginWith = (provider: 'zalo' | 'google') => {
  alert(`Đăng nhập qua ${provider === 'zalo' ? 'Zalo' : 'Google'} — Tính năng sẽ sớm ra mắt!`)
  openLoginSheet.value = false
}

const sendOtp = () => {
  if (phoneNumber.value.length < 9) return
  otpSent.value = true
}

// --- LOGIC LỌC VÀ PHÂN TRANG ---
const itemsPerPage = 8
const currentPage = ref(1)

const filtered = computed(() => {
  return activeCat.value === "all" 
    ? menuItems 
    : menuItems.filter((m) => m.category === activeCat.value)
})

const totalPages = computed(() => Math.ceil(filtered.value.length / itemsPerPage))

const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filtered.value.slice(start, end)
})

// Reset về trang 1 khi đổi danh mục
watch(activeCat, () => {
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
  toast.success('Đã thêm vào giỏ', m.name)
}

const handleOrder = () => {
  if (cart.lines.length === 0) return
  const orderId = Math.floor(1000 + Math.random() * 9000)
  toast.success('Gửi đơn thành công', `Món đang được pha chế cho Bàn ${tableId}`)
  cart.clear()
  open.value = false
  setTimeout(() => router.push(`/payment/${orderId}`), 1000)
}
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Cormorant+Garamond:ital,wght@0,400;0,500;0,600;0,700;1,400;1,500&family=Montserrat:wght@400;500;600;700&display=swap');

.font-premium-serif {
  font-family: 'Cormorant Garamond', Georgia, serif;
}
.font-premium-sans {
  font-family: 'Inter', sans-serif;
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
