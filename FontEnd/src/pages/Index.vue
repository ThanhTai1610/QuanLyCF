<template>
  <div class="min-h-screen bg-background text-espresso selection:bg-caramel-light selection:text-brown">
    <!-- Sticky Glassmorphic Header -->
    <header class="sticky top-0 z-40 bg-[#281C16]/95 text-white backdrop-blur-xl border-b border-white/10 transition-all shadow-[0_10px_30px_rgba(0,0,0,0.35)]">
      <!-- Top Accent Line -->
      <div class="h-0.5 w-full bg-gradient-to-r from-transparent via-[#CC8033] to-transparent opacity-80"></div>
      
      <div class="max-w-7xl mx-auto px-3 sm:px-6 h-16 sm:h-20 flex items-center justify-between gap-2 sm:gap-4 overflow-hidden">
        
        <!-- Brand Logo & Title -->
        <a href="#" class="flex items-center gap-2 sm:gap-3 group shrink-0 text-left min-w-0">
          <div class="w-9 h-9 sm:w-11 sm:h-11 rounded-xl sm:rounded-2xl bg-gradient-to-br from-[#CC8033] via-[#B3702C] to-[#8A4F1A] p-0.5 shadow-lg shadow-[#CC8033]/25 group-hover:scale-105 transition-all duration-300 shrink-0">
            <div class="w-full h-full bg-[#1C130E]/40 rounded-[10px] sm:rounded-[14px] flex items-center justify-center backdrop-blur-sm">
              <Coffee class="w-4 h-4 sm:w-5 sm:h-5 text-white group-hover:rotate-12 transition-transform duration-300" stroke-width="2.5" />
            </div>
          </div>
          <div class="flex flex-col min-w-0">
            <span class="font-premium-serif text-lg sm:text-xl text-white font-black tracking-tight leading-none group-hover:text-[#E89E53] transition-colors truncate">
              {{ tenQuan || 'f6' }}
            </span>
            <span class="hidden sm:inline-block text-[9px] text-[#D5B08D] font-bold uppercase tracking-[0.2em] mt-1 opacity-90 truncate">
              Không gian & Cà phê tử tế
            </span>
          </div>
        </a>
        
        <!-- Navigation Links -->
        <nav class="hidden lg:flex items-center gap-1.5 bg-white/5 border border-white/10 rounded-full px-3 py-1.5 backdrop-blur-md shadow-inner">
          <a href="#menu" @click.prevent="scrollToSection('menu')"
            class="text-xs font-bold text-white/80 hover:text-white hover:bg-white/10 px-4 py-2 rounded-full transition-all duration-200">
            Thực đơn
          </a>
          <a href="#loyalty" @click.prevent="scrollToSection('loyalty')"
            class="text-xs font-bold text-white/80 hover:text-white hover:bg-white/10 px-4 py-2 rounded-full transition-all duration-200">
            Thành viên
          </a>
          <a href="#promotions" @click.prevent="scrollToSection('promotions')"
            class="text-xs font-bold text-white/80 hover:text-white hover:bg-white/10 px-4 py-2 rounded-full transition-all duration-200">
            Ưu đãi
          </a>
          <a href="#about" @click.prevent="scrollToSection('about')"
            class="text-xs font-bold text-white/80 hover:text-white hover:bg-white/10 px-4 py-2 rounded-full transition-all duration-200">
            Câu chuyện
          </a>
        </nav>

        <!-- Right Side Actions & User Profile -->
        <div class="flex items-center gap-1.5 sm:gap-2.5 shrink-0">
          
          <!-- User Info (Logged in) -->
          <div v-if="customerProfile" class="flex items-center gap-1.5 sm:gap-2">
            <!-- Profile Card -->
            <div class="flex items-center gap-1.5 sm:gap-2 bg-white/10 hover:bg-white/15 border border-white/15 px-2 sm:px-3 py-1 rounded-full backdrop-blur-md transition-all shadow-sm">
              <div class="w-6 h-6 sm:w-7 sm:h-7 rounded-full bg-gradient-to-tr from-[#CC8033] to-[#F59E0B] text-white text-[10px] sm:text-xs font-black flex items-center justify-center shadow-md shrink-0">
                {{ customerProfile.name.charAt(0).toUpperCase() }}
              </div>
              <div class="hidden md:flex flex-col text-left">
                <span class="text-[9px] text-[#D5B08D] font-semibold leading-none">Xin chào,</span>
                <span class="text-xs font-bold text-white leading-tight max-w-[100px] truncate">{{ customerProfile.name }}</span>
              </div>
              <button @click="handleLogout" title="Đăng xuất"
                class="px-2 py-0.5 rounded-full bg-red-500/20 hover:bg-red-500/30 text-red-300 text-[10px] font-bold transition-colors border border-red-500/30 shrink-0">
                Thoát
              </button>
            </div>

            <!-- Order History Button -->
            <button @click="openCustomerHistoryModal" title="Lịch sử đơn hàng"
              class="w-8 h-8 sm:w-auto sm:px-3.5 sm:py-2 rounded-full bg-white/10 hover:bg-white/20 border border-white/20 text-white text-xs font-bold transition-all duration-200 flex items-center justify-center gap-1.5 shadow-md active:scale-95 cursor-pointer shrink-0">
              <History class="w-4 h-4 text-[#E89E53]" />
              <span class="hidden md:inline">Lịch sử</span>
            </button>
          </div>

          <!-- Log in button (Not logged in) -->
          <button v-else @click="isPhoneModalOpen = true"
            class="px-2.5 sm:px-4 py-1.5 sm:py-2.5 rounded-full bg-white/10 hover:bg-white/20 border border-white/20 text-white text-[11px] sm:text-xs font-bold transition-all duration-200 backdrop-blur-md shadow-md active:scale-95 cursor-pointer shrink-0">
            🔑 <span class="hidden sm:inline">Đăng nhập</span><span class="sm:hidden">Login</span>
          </button>

          <!-- Nút Gọi phục vụ hỗ trợ tại bàn -->
          <button
            @click="openCallSupportModal"
            class="px-2.5 sm:px-4 py-1.5 sm:py-2.5 rounded-full bg-gradient-to-r from-red-600 to-amber-600 hover:from-red-500 hover:to-amber-500 text-white text-[11px] sm:text-xs font-bold transition-all duration-200 shadow-md shadow-red-900/30 active:scale-95 flex items-center gap-1.5 cursor-pointer shrink-0"
            title="Gọi nhân viên phục vụ hỗ trợ tại bàn"
          >
            <BellRing class="w-3.5 h-3.5 animate-bounce" />
            <span class="hidden sm:inline">Gọi phục vụ</span>
            <span class="sm:hidden">Gọi</span>
          </button>

          <!-- Order Online / QR CTA Button -->
          <router-link :to="tableNumber ? `/menu/${tableNumber}` : '/menu/xem-menu'"
            class="px-3 sm:px-5 py-1.5 sm:py-2.5 rounded-full bg-gradient-to-r from-[#CC8033] via-[#E89E53] to-[#CC8033] hover:brightness-110 text-[#1A120C] text-[10px] sm:text-xs font-black uppercase tracking-wider transition-all duration-300 shadow-lg shadow-[#CC8033]/30 hover:shadow-xl hover:shadow-[#CC8033]/40 active:scale-95 flex items-center gap-1.5 whitespace-nowrap shrink-0">
            <QrCode class="w-3.5 h-3.5 sm:w-4 sm:h-4 shrink-0" stroke-width="2.5" />
            <span>{{ tableNumber ? `Bàn ${tableNumber}` : 'Gọi món' }}</span>
          </router-link>
        </div>
      </div>
    </header>

    <main>
      <!-- Hero Section -->
      <section class="relative overflow-hidden pt-20 md:pt-28 pb-24 border-b border-cream-deep/50">
        <!-- Ambient Glow Elements -->
        <div class="absolute -top-40 -left-40 w-96 h-96 rounded-full bg-caramel/10 blur-[100px] pointer-events-none" />
        <div class="absolute top-20 right-0 w-80 h-80 rounded-full bg-sage/10 blur-[80px] pointer-events-none" />
        
        <div class="max-w-6xl mx-auto px-6 grid lg:grid-cols-12 gap-12 items-center">
          <div class="lg:col-span-7 text-left space-y-6">
            <span class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-caramel-light border border-caramel/20 text-xs font-medium text-brown">
              ☕ Hệ thống dành cho quán cà phê hiện đại
            </span>
            <h1 class="font-display text-4xl sm:text-5xl lg:text-6xl text-espresso leading-[1.1] font-extrabold tracking-tight">
              Chào mừng bạn đến với 
              <span class="bg-gradient-to-r from-caramel via-brown to-espresso bg-clip-text text-transparent">{{ tenQuan || 'BrewManager Cafe' }}</span>
            </h1>
            <p class="text-muted-foreground text-base sm:text-lg leading-relaxed max-w-xl">
              {{ moTaQuan || 'Từ nông trại tới ly cà phê của bạn. Chúng tôi mang đến hương vị đậm đà được pha chế tỉ mỉ, kết hợp cùng dịch vụ tự động gọi món QR hiện đại giúp trải nghiệm của bạn trọn vẹn nhất.' }}
            </p>

            <!-- Store Contact Details -->
            <div class="flex flex-col gap-2 text-left">
              <div v-if="storeInfoStore.diaChi" class="flex items-center gap-2 text-sm text-muted-foreground">
                <MapPin class="w-4 h-4 text-caramel shrink-0" />
                <span>{{ storeInfoStore.diaChi }}</span>
              </div>
              <div v-if="storeInfoStore.soDienThoai" class="flex items-center gap-2 text-sm text-muted-foreground">
                <Phone class="w-4 h-4 text-caramel shrink-0" />
                <span>{{ storeInfoStore.soDienThoai }}</span>
              </div>
            </div>

            <div class="flex flex-wrap gap-3 pt-2">
              <button
                @click="openQrScannerModal"
                class="inline-flex items-center gap-2.5 px-6 py-3.5 rounded-xl bg-gradient-to-r from-[#CC8033] via-[#E89E53] to-[#CC8033] hover:brightness-110 text-[#1C130E] font-black text-sm transition-all shadow-lg shadow-[#CC8033]/30 hover:shadow-xl active:scale-95 cursor-pointer z-10 relative"
              >
                <QrCode class="w-5 h-5 shrink-0" stroke-width="2.5" />
                <span>📷 Quét QR Gọi Món tại Bàn</span>
              </button>

              <a
                v-if="!tableNumber"
                href="#menu"
                @click.prevent="scrollToSection('menu')"
                class="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-espresso hover:bg-brown text-cream font-bold transition-all hover:scale-[1.02] shadow-warm cursor-pointer z-10 relative"
              >
                Khám Phá Thực Đơn
                <ArrowRight class="w-4 h-4" />
              </a>
              <router-link
                v-else
                :to="`/menu/${tableNumber}`"
                class="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-espresso hover:bg-brown text-cream font-bold transition-all hover:scale-[1.02] shadow-warm"
              >
                Khám Phá Thực Đơn (Bàn {{ tableNumber }})
                <ArrowRight class="w-4 h-4" />
              </router-link>
              <a
                href="#loyalty"
                @click.prevent="scrollToSection('loyalty')"
                class="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-card hover:bg-caramel-light text-espresso font-bold border border-cream-deep transition-colors shadow-soft cursor-pointer z-10 relative"
              >
                Thành Viên Tích Điểm
              </a>
            </div>

            <!-- Café Tags / Values -->
            <div class="grid grid-cols-3 gap-6 pt-8 border-t border-cream-deep/60">
              <div class="space-y-1">
                <div class="text-3xl text-espresso font-extrabold font-sans">100%</div>
                <div class="text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Hạt Mộc Sạch</div>
              </div>
              <div class="space-y-1">
                <div class="text-3xl text-caramel font-extrabold font-sans">Rang Tay</div>
                <div class="text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Pha chế tỉ mỉ</div>
              </div>
              <div class="space-y-1">
                <div class="text-3xl text-sage font-extrabold font-sans">Miễn Phí</div>
                <div class="text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Wifi cực nhanh</div>
              </div>
            </div>
          </div>

          <div class="lg:col-span-5 relative">
            <div class="absolute -inset-4 bg-caramel/10 rounded-2xl blur-3xl pointer-events-none" />
            <!-- Hero Image Frame -->
            <div class="relative rounded-2xl overflow-hidden shadow-warm border-4 border-card transform lg:rotate-1 hover:rotate-0 transition-all duration-500">
              <img
                :src="storeInfoStore.anhTrangChu || heroImg"
                alt="BrewManager Coffee Vibe"
                class="w-full h-auto object-cover aspect-[4/3] scale-105 hover:scale-100 transition-all duration-700"
              />
              <div class="absolute inset-0 bg-gradient-to-t from-espresso/45 via-transparent to-transparent" />
            </div>

            <!-- Overlays -->
            <div v-if="bestSellingItem" class="absolute -bottom-6 -left-6 bg-card rounded-xl p-4 border border-cream-deep shadow-warm hidden md:block max-w-[220px]">
              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-lg bg-caramel/10 flex items-center justify-center shrink-0">
                  <Coffee class="w-5 h-5 text-caramel" />
                </div>
                <div class="min-w-0 text-left">
                  <div class="text-[9px] font-bold text-muted-foreground uppercase">Món Bán Chạy Nhất</div>
                  <div class="font-bold text-espresso text-xs truncate">{{ bestSellingItem.tenSanPham }}</div>
                </div>
              </div>
            </div>

            <div class="absolute -top-6 -right-6 bg-card rounded-xl p-4 border border-cream-deep shadow-warm hidden md:block">
              <div class="flex items-center gap-2">
                <div class="w-2.5 h-2.5 rounded-full bg-sage animate-pulse" />
                <div class="text-[11px] font-bold text-espresso">Mở cửa: {{ storeInfoStore.gioMoCua || '7:00 - 22:30' }}</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Menu Section -->
      <section id="menu" class="py-24 bg-card/45 border-b border-cream-deep/50">
        <div class="max-w-6xl mx-auto px-6 text-center space-y-10">
          <div class="space-y-4">
            <span class="text-xs font-bold uppercase tracking-widest text-caramel font-sans">Thực Đơn Tinh Tế</span>
            <h2 class="font-display text-3xl sm:text-4xl text-espresso font-extrabold">Khám Phá Hương Vị {{ tenQuan || 'BrewManager' }}</h2>
            <p class="text-muted-foreground max-w-xl mx-auto text-sm sm:text-base">
              Mỗi thức uống là một tác phẩm nghệ thuật, kết hợp hoàn hảo giữa hương vị truyền thống và phong cách hiện đại.
            </p>
          </div>

          <!-- Category Switcher -->
          <div class="flex flex-wrap justify-center gap-2 p-1.5 bg-cream-deep/40 rounded-2xl max-w-xl mx-auto">
            <button 
              v-for="cat in menuCategories" 
              :key="cat.id"
              @click="activeCategory = cat.id"
              :class="[
                'px-5 py-2.5 rounded-xl text-xs font-bold transition-all duration-300',
                activeCategory === cat.id 
                  ? 'bg-espresso text-cream shadow-soft' 
                  : 'text-espresso/70 hover:bg-cream-deep/60 hover:text-espresso'
              ]"
            >
              {{ cat.name }}
            </button>
          </div>

          <!-- Drinks Grid -->
          <div class="grid sm:grid-cols-2 lg:grid-cols-4 gap-6 pt-6">
            <div 
              v-for="(item, index) in filteredMenuItems" 
              :key="item.maSanPham" 
              class="bg-card rounded-2xl overflow-hidden border border-cream-deep/60 hover:-translate-y-1 hover:shadow-warm transition-all duration-300 flex flex-col justify-between group"
            >
              <div class="relative overflow-hidden aspect-[4/3] bg-cream-deep">
                <img 
                  v-if="item.hinhAnh"
                  :src="item.hinhAnh" 
                  :alt="item.tenSanPham"
                  class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                />
                <div v-else class="w-full h-full flex items-center justify-center text-[#C5BEB8]">
                  <Coffee class="w-8 h-8" />
                </div>
                <span 
                  class="absolute top-3 left-3 bg-caramel text-cream text-[9px] uppercase tracking-wider font-extrabold px-2 py-0.5 rounded-full"
                >
                  {{ index === 0 ? 'Bán chạy nhất' : (index === 1 ? 'Yêu thích' : 'Top ' + (index + 1)) }}
                </span>
              </div>
              <div class="p-5 flex-1 flex flex-col justify-between text-left space-y-4">
                <div class="space-y-1">
                  <h3 class="font-display text-sm text-espresso font-bold group-hover:text-caramel transition-colors">{{ item.tenSanPham }}</h3>
                  <p class="text-muted-foreground text-[11px] leading-relaxed line-clamp-2">Món ngon chuẩn vị, được phục vụ tại {{ tenQuan }}.</p>
                </div>
                <div class="flex justify-between items-center pt-2 border-t border-cream-deep/30">
                  <span class="font-sans text-sm font-extrabold text-caramel">{{ formatVND(item.giaBan) }}</span>
                  <router-link 
                    :to="tableNumber ? `/menu/${tableNumber}` : '/menu/xem-menu'" 
                    class="inline-flex items-center gap-1 bg-espresso hover:bg-caramel text-cream text-[10px] font-bold px-3 py-1.5 rounded-lg transition-colors"
                  >
                    {{ tableNumber ? 'Gọi món' : 'Xem thực đơn' }}
                    <ArrowRight class="w-3 h-3" />
                  </router-link>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Loyalty Section -->
      <section id="loyalty" class="py-24 border-b border-cream-deep/50 bg-background relative overflow-hidden">
        <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[600px] h-[600px] rounded-full bg-caramel/5 blur-[120px] pointer-events-none" />
        
        <div class="max-w-6xl mx-auto px-6 grid lg:grid-cols-12 gap-12 items-center">
          <div class="lg:col-span-6 text-left space-y-6">
            <span class="text-xs font-bold uppercase tracking-widest text-caramel font-sans">Đặc Quyền Thành Viên</span>
            <h2 class="font-display text-3xl sm:text-4xl text-espresso font-extrabold">Tích Điểm Tự Động - Đổi Quà Cực Chất</h2>
            <p class="text-muted-foreground text-sm sm:text-base leading-relaxed">
              Trở thành Hội viên {{ tenQuan || 'BrewManager' }} để được tích điểm tự động 10% mỗi lần mua hàng. Số điểm tích lũy được sử dụng để đổi các voucher giảm giá, miễn phí topping hoặc nhận nước miễn phí vào ngày sinh nhật.
            </p>

            <div class="space-y-3.5">
              <div class="flex gap-3">
                <div class="w-8 h-8 rounded-full bg-caramel-light flex items-center justify-center text-brown font-bold text-xs shrink-0 mt-0.5">1</div>
                <div>
                  <h4 class="font-bold text-xs text-espresso">Nhập số điện thoại khi gọi món</h4>
                  <p class="text-[11px] text-muted-foreground">Hệ thống tự nhận diện và cộng điểm ngay lập tức khi thanh toán đơn thành công.</p>
                </div>
              </div>
              <div class="flex gap-3">
                <div class="w-8 h-8 rounded-full bg-caramel-light flex items-center justify-center text-brown font-bold text-xs shrink-0 mt-0.5">2</div>
                <div>
                  <h4 class="font-bold text-xs text-espresso">Thăng hạng nhận đặc quyền</h4>
                  <p class="text-[11px] text-muted-foreground">Từ Hạng Bạc (giảm 5%), Hạng Vàng (giảm 10%) tới Kim Cương (giảm 15% trọn đời và phòng chờ VIP).</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Virtual Membership Card & Voucher Panel -->
          <div class="lg:col-span-6 flex flex-col items-center">
            <!-- Active Logged In Membership Card -->
            <div v-if="customerProfile" class="w-full max-w-sm bg-gradient-to-br from-espresso to-brown rounded-2xl p-6 text-cream shadow-warm text-left space-y-6 relative overflow-hidden border border-white/10">
              <div class="absolute -right-10 -bottom-10 w-40 h-40 rounded-full bg-caramel/20 blur-2xl pointer-events-none" />
              <div class="flex justify-between items-start">
                <div class="flex items-center gap-2">
                  <div class="w-8 h-8 rounded-lg bg-caramel flex items-center justify-center text-cream">
                    <Coffee class="w-4 h-4" />
                  </div>
                  <span class="font-display font-bold text-sm tracking-widest">BREWMEMBER</span>
                </div>
                <span class="text-[9px] bg-caramel text-cream px-2 py-0.5 rounded-full font-bold uppercase tracking-wider">Hạng {{ customerProfile.tier }}</span>
              </div>

              <div class="pt-2">
                <div class="text-[10px] text-cream/50 uppercase tracking-widest">Chủ thẻ</div>
                <div class="text-lg font-bold tracking-wide">{{ customerProfile.name }}</div>
                <div class="text-xs font-mono text-cream/70 mt-0.5">{{ customerProfile.phone }}</div>
              </div>

              <div class="grid grid-cols-2 gap-4 pt-4 border-t border-white/10" v-if="nextTierInfo">
                <div>
                  <div class="text-[9px] text-cream/50 uppercase">Điểm tích lũy</div>
                  <div class="text-xl font-bold text-caramel">{{ customerProfile.points }} <span class="text-xs text-cream/70 font-normal">điểm</span></div>
                </div>
                <div>
                  <div class="text-[9px] text-cream/50 uppercase">Hạng tiếp theo</div>
                  <div class="text-xs font-semibold">{{ nextTierInfo.text }}</div>
                </div>
              </div>

              <!-- Mini Bar Progress -->
              <div class="w-full bg-white/10 h-1.5 rounded-full overflow-hidden" v-if="nextTierInfo">
                <div class="bg-caramel h-full rounded-full" :style="{ width: nextTierInfo.percent + '%' }" />
              </div>

              <!-- Button xem lịch sử đơn -->
              <button
                @click="openCustomerHistoryModal"
                class="w-full mt-3 py-2.5 bg-white/10 hover:bg-white/20 border border-white/20 rounded-xl text-xs font-bold text-cream flex items-center justify-center gap-2 transition-all cursor-pointer shadow-soft active:scale-[0.99]"
              >
                <History class="w-4 h-4 text-caramel" />
                <span>Xem lịch sử đơn hàng đã mua</span>
              </button>
            </div>

            <!-- Anonymous Membership Card Preview -->
            <div v-else class="w-full max-w-sm bg-gradient-to-br from-card to-cream-deep rounded-2xl p-6 text-espresso shadow-warm text-left space-y-6 border border-cream-deep/60 relative overflow-hidden">
              <div class="absolute inset-0 bg-card/80 backdrop-blur-[3px] flex flex-col items-center justify-center p-6 text-center space-y-4 z-10">
                <div class="w-10 h-10 rounded-full bg-caramel/10 flex items-center justify-center text-caramel">
                  <Sparkles class="w-5 h-5" />
                </div>
                <div>
                  <h4 class="font-bold text-xs text-espresso">Xem Thẻ Hội Viên Của Bạn</h4>
                  <p class="text-[10px] text-muted-foreground max-w-[200px] mt-1">Đăng nhập bằng số điện thoại để tra cứu xếp hạng, điểm tích lũy và voucher hiện có.</p>
                </div>
                <button 
                  @click="isPhoneModalOpen = true"
                  class="px-5 py-2 bg-espresso hover:bg-brown text-cream text-[10px] font-bold rounded-lg uppercase tracking-wider transition-colors shadow-soft"
                >
                  Đăng nhập ngay
                </button>
              </div>

              <!-- Background card elements (blurred to prevent text overlap) -->
              <div class="flex justify-between items-start opacity-25 blur-[2.5px] select-none pointer-events-none">
                <span class="font-display font-bold text-xs tracking-widest text-espresso/40">BREWMEMBER</span>
                <span class="text-[8px] bg-espresso/10 text-espresso/50 px-2 py-0.5 rounded-full font-bold uppercase">Thành viên</span>
              </div>
              <div class="pt-2 opacity-25 blur-[2.5px] select-none pointer-events-none">
                <div class="text-[8px] text-espresso/40 uppercase">Chủ thẻ</div>
                <div class="text-base font-bold">Khách Hàng Thân Thiết</div>
                <div class="text-xs font-mono text-espresso/40">09xx xxx xxx</div>
              </div>
              <div class="grid grid-cols-2 gap-4 pt-4 border-t border-espresso/10 opacity-25 blur-[2.5px] select-none pointer-events-none">
                <div>
                  <div class="text-[8px] text-espresso/40 uppercase">Điểm</div>
                  <div class="text-lg font-bold">0</div>
                </div>
              </div>
            </div>

            <!-- Vouchers Panel (Hiển thị voucher của quán, có thể sao chép trực tiếp) -->
            <div class="w-full max-w-sm mt-6 space-y-3">
              <div class="text-left text-xs font-bold text-espresso uppercase tracking-wider">Voucher của bạn:</div>
              <div class="grid grid-cols-1 gap-2.5">
                <template v-if="activePromos.length > 0">
                  <div v-for="p in activePromos" :key="p.maKhuyenMai" class="bg-card rounded-xl border border-cream-deep/60 p-3 flex justify-between items-center shadow-soft">
                    <div class="text-left space-y-0.5">
                      <span :class="p.loaiGiamGia === 'PhanTram' ? 'bg-caramel/10 text-caramel' : 'bg-sage/20 text-success'" class="text-[8px] px-2 py-0.5 rounded-full font-extrabold uppercase">
                        {{ p.loaiGiamGia === 'PhanTram' ? 'GIẢM ' + p.giaTriGiam + '%' : 'GIẢM ' + formatVND(p.giaTriGiam) }}
                      </span>
                      <h5 class="text-xs font-bold text-espresso">{{ p.tenChuongTrinh }}</h5>
                      <p class="text-[10px] text-muted-foreground">
                        <span v-if="p.donToiThieu">Đơn tối thiểu: {{ formatVND(p.donToiThieu) }}</span>
                        <span v-else>Không giới hạn đơn</span>
                        <span v-if="p.ngayKetThuc"> · HSD: {{ fmtD(p.ngayKetThuc) }}</span>
                      </p>
                    </div>
                    <button 
                      v-if="p.maGiamGia"
                      @click="copyVoucherCode(p.maGiamGia)"
                      :class="isVoucherSaved(p.maGiamGia) ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-caramel-light hover:bg-caramel hover:text-cream text-brown'"
                      class="text-[10px] font-bold px-3 py-1.5 rounded-lg transition-colors shrink-0"
                    >
                      {{ isVoucherSaved(p.maGiamGia) ? 'Đã lưu' : 'Lưu mã' }}
                    </button>
                    <span v-else class="text-[10px] text-muted-foreground font-bold shrink-0 px-2">Áp dụng tự động</span>
                  </div>
                </template>
                <template v-else>
                  <!-- Fallback Mock Voucher Card 1 -->
                  <div class="bg-card rounded-xl border border-cream-deep/60 p-3 flex justify-between items-center shadow-soft">
                    <div class="text-left space-y-0.5">
                      <span class="text-[8px] bg-sage/20 text-success px-2 py-0.5 rounded-full font-extrabold uppercase">Thành Viên Mới</span>
                      <h5 class="text-xs font-bold text-espresso">Giảm 10.000đ khi gọi món</h5>
                      <p class="text-[10px] text-muted-foreground">Hạn sử dụng: 31/12/2026</p>
                    </div>
                    <button 
                      @click="copyVoucherCode('BREWNEW')"
                      :class="isVoucherSaved('BREWNEW') ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-caramel-light hover:bg-caramel hover:text-cream text-brown'"
                      class="text-[10px] font-bold px-3 py-1.5 rounded-lg transition-colors shrink-0"
                    >
                      {{ isVoucherSaved('BREWNEW') ? 'Đã lưu' : 'Lưu mã' }}
                    </button>
                  </div>
                  <!-- Fallback Mock Voucher Card 2 -->
                  <div class="bg-card rounded-xl border border-cream-deep/60 p-3 flex justify-between items-center shadow-soft">
                    <div class="text-left space-y-0.5">
                      <span class="text-[8px] bg-caramel/10 text-caramel px-2 py-0.5 rounded-full font-extrabold uppercase">Hạng Bạc</span>
                      <h5 class="text-xs font-bold text-espresso">Tặng trân châu đen miễn phí</h5>
                      <p class="text-[10px] text-muted-foreground">Áp dụng đơn từ 45k</p>
                    </div>
                    <button 
                      @click="copyVoucherCode('SILVERFREE')"
                      :class="isVoucherSaved('SILVERFREE') ? 'bg-emerald-600 hover:bg-emerald-700 text-white' : 'bg-caramel-light hover:bg-caramel hover:text-cream text-brown'"
                      class="text-[10px] font-bold px-3 py-1.5 rounded-lg transition-colors shrink-0"
                    >
                      {{ isVoucherSaved('SILVERFREE') ? 'Đã lưu' : 'Lưu mã' }}
                    </button>
                  </div>
                </template>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Promotions Section -->
      <section id="promotions" class="py-24 bg-card/45 border-b border-cream-deep/50">
        <div class="max-w-6xl mx-auto px-6 text-center space-y-10">
          <div class="space-y-4">
            <span class="text-xs font-bold uppercase tracking-widest text-caramel font-sans">Chương Trình Đặc Biệt</span>
            <h2 class="font-display text-3xl sm:text-4xl text-espresso font-extrabold">Ưu Đãi Đang Diễn Ra</h2>
            <p class="text-muted-foreground max-w-xl mx-auto text-sm sm:text-base">
              Đừng bỏ lỡ các ưu đãi giờ vàng và quà tặng kết nối cực hời chỉ có tại {{ tenQuan || 'BrewManager' }}.
            </p>
          </div>

          <div class="grid md:grid-cols-2 gap-8 text-left">
            <!-- Promo 1 -->
            <div class="bg-card rounded-2xl overflow-hidden border border-cream-deep/60 shadow-soft flex flex-col sm:flex-row relative">
              <div class="sm:w-1/3 bg-espresso flex items-center justify-center p-6 text-cream relative">
                <div class="absolute -right-3 top-1/2 -translate-y-1/2 w-6 h-6 rounded-full bg-background hidden sm:block" />
                <div class="text-center">
                  <span class="text-3xl font-extrabold text-caramel">20%</span>
                  <span class="text-[9px] uppercase tracking-wider block font-bold mt-1 text-cream/60">Giờ Vàng</span>
                </div>
              </div>
              <div class="p-6 flex-1 flex flex-col justify-between space-y-4">
                <div>
                  <h4 class="font-display font-bold text-base text-espresso">Happy Hour - Đón Chiều Cực Tỉnh</h4>
                  <p class="text-xs text-muted-foreground mt-1">Giảm ngay 20% cho toàn bộ thực đơn trà trái cây và cà phê sữa đá vào khung giờ từ 14h00 đến 17h00 hàng ngày từ thứ Hai đến thứ Sáu.</p>
                </div>
                <div class="text-[10px] font-bold text-caramel">Mã áp dụng tự động tại quầy/QR</div>
              </div>
            </div>

            <!-- Promo 2 -->
            <div class="bg-card rounded-2xl overflow-hidden border border-cream-deep/60 shadow-soft flex flex-col sm:flex-row relative">
              <div class="sm:w-1/3 bg-caramel flex items-center justify-center p-6 text-cream relative">
                <div class="absolute -right-3 top-1/2 -translate-y-1/2 w-6 h-6 rounded-full bg-background hidden sm:block" />
                <div class="text-center">
                  <span class="text-xl font-extrabold text-espresso">MUA 2<br>TẶNG 1</span>
                  <span class="text-[9px] uppercase tracking-wider block font-bold mt-1 text-cream/80">Cuối Tuần</span>
                </div>
              </div>
              <div class="p-6 flex-1 flex flex-col justify-between space-y-4">
                <div>
                  <h4 class="font-display font-bold text-base text-espresso">Cuối Tuần Sum Vầy - Trọn Niềm Vui</h4>
                  <p class="text-xs text-muted-foreground mt-1">Mua 2 ly size L tặng ngay 1 ly size M bất kỳ trong thực đơn. Áp dụng cho các ngày thứ Bảy và Chủ Nhật khi đi cùng nhóm từ 3 người.</p>
                </div>
                <div class="text-[10px] font-bold text-caramel">Tặng món trực tiếp khi đặt qua QR</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- About Café Section -->
      <section id="about" class="py-24 bg-background">
        <div class="max-w-6xl mx-auto px-6 grid lg:grid-cols-2 gap-12 items-center">
          <!-- Left: Image showing clean coffee making -->
          <div class="relative">
            <div class="absolute -inset-4 bg-sage/10 rounded-2xl blur-2xl pointer-events-none" />
            <div class="relative rounded-2xl overflow-hidden shadow-warm border-4 border-card">
              <img
                :src="menuCoffee"
                alt="BrewManager Brewing Coffee"
                class="w-full h-auto object-cover aspect-[4/3]"
              />
            </div>
          </div>

          <!-- Right: Cozy explanation -->
          <div class="text-left space-y-6">
            <span class="text-xs font-bold uppercase tracking-widest text-caramel font-sans">Câu Chuyện {{ tenQuan || 'BrewManager' }}</span>
            <h2 class="font-display text-3xl sm:text-4xl text-espresso font-extrabold">Cà Phê Tử Tế, Gặp Gỡ Thân Tình</h2>
            <p class="text-muted-foreground text-sm sm:text-base leading-relaxed">
              {{ tenQuan || 'BrewManager' }} ra đời từ niềm đam mê với hương vị hạt cà phê mộc đậm đà của Tây Nguyên Việt Nam. Chúng tôi tin rằng mỗi tách cà phê được pha đúng chuẩn, phục vụ trong một không gian mộc mạc yên bình, chính là liều thuốc tinh thần tốt nhất cho một ngày bận rộn.
            </p>
            <p class="text-muted-foreground text-sm sm:text-base leading-relaxed">
              Chúng tôi luôn nỗ lực ứng dụng công nghệ để nâng tầm trải nghiệm của bạn - giúp việc gọi món trở nên riêng tư, nhanh gọn qua mã QR, đồng thời vẫn giữ được sự kết nối chân thành ấm áp đặc trưng của quán.
            </p>
            <div class="flex gap-6 pt-2">
              <div>
                <h5 class="font-bold text-espresso">{{ storeInfoStore.gioMoCua || '7:00 - 22:30' }}</h5>
                <p class="text-[11px] text-muted-foreground">Giờ mở cửa</p>
              </div>
              <div class="border-l border-cream-deep/60 pl-6">
                <h5 class="font-bold text-espresso">{{ storeInfoStore.diaChi || '123 Đường Cà Phê, Q. 1' }}</h5>
                <p class="text-[11px] text-muted-foreground">Địa chỉ quán</p>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>

    <!-- Footer -->
    <footer class="bg-[#4A3224] text-cream/70 py-16 border-t border-white/10">
      <div class="max-w-6xl mx-auto px-6 grid md:grid-cols-4 gap-10 text-left">
        <div class="space-y-4">
          <div class="flex items-center gap-2">
            <div class="w-9 h-9 rounded-lg bg-caramel flex items-center justify-center text-cream">
              <Coffee class="w-5 h-5" />
            </div>
            <span class="font-display text-lg text-cream font-bold">{{ storeInfoStore.tenQuan || 'cà phê F6' }}</span>
          </div>
          <p class="text-xs text-cream/50 leading-relaxed font-semibold">
            {{ storeInfoStore.moTaQuan || 'Quán cà phê đặc sản với không gian ấm cúng. Phục vụ cà phê pha máy, trà, bánh ngọt và các loại đồ uống đá xay.' }}
          </p>
        </div>

        <div class="space-y-4">
          <h4 class="font-bold text-xs text-cream uppercase tracking-wider">Hệ Thống</h4>
          <ul class="space-y-2 text-xs">
            <li><router-link to="/menu/5" class="hover:text-caramel transition-colors">Menu QR Gọi Món</router-link></li>
            <li><router-link to="/login" class="hover:text-caramel transition-colors">Cổng Đăng Nhập POS</router-link></li>
            <li><router-link to="/lich-su-don" class="hover:text-caramel transition-colors">Lịch Sử Tích Điểm</router-link></li>
          </ul>
        </div>

        <div class="space-y-4">
          <h4 class="font-bold text-xs text-cream uppercase tracking-wider">Tính Năng</h4>
          <ul class="space-y-2 text-xs">
            <li><a href="#menu" @click.prevent="scrollToSection('menu')" class="hover:text-caramel transition-colors cursor-pointer">Menu Gọi Món</a></li>
            <li><a href="#loyalty" @click.prevent="scrollToSection('loyalty')" class="hover:text-caramel transition-colors cursor-pointer">Thành Viên Tích Điểm</a></li>
            <li><a href="#promotions" @click.prevent="scrollToSection('promotions')" class="hover:text-caramel transition-colors cursor-pointer">Ưu Đãi</a></li>
          </ul>
        </div>

        <div class="space-y-4">
          <h4 class="font-bold text-xs text-cream uppercase tracking-wider">Liên Hệ</h4>
          <p class="text-xs text-cream/50 leading-relaxed">
            Email: {{ storeInfoStore.soDienThoai ? (storeInfoStore.soDienThoai + '@caphef6.website') : 'contact@caphef6.website' }}<br />
            Hotline: {{ storeInfoStore.soDienThoai || '1111' }}<br />
            Địa chỉ: {{ storeInfoStore.diaChi || '123 Nguyễn Huệ, Quận 1, TP.HCM' }}
          </p>
        </div>
      </div>

      <div class="max-w-6xl mx-auto px-6 mt-12 pt-8 border-t border-white/5 text-center text-[10px] text-cream/40 flex flex-col sm:flex-row justify-between gap-4">
        <span>© 2026 {{ storeInfoStore.tenQuan || 'cà phê F6' }}. Mọi quyền được bảo lưu. Thiết kế pha chế tỉ mỉ.</span>
        <div class="flex gap-4 justify-center">
          <a href="#" class="hover:text-caramel transition-colors">Chính sách bảo mật</a>
          <a href="#" class="hover:text-caramel transition-colors">Điều khoản dịch vụ</a>
        </div>
      </div>
    </footer>

    <!-- Phone Modal Overlay -->
    <div v-if="isPhoneModalOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/40 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-card p-6 rounded-xl shadow-warm w-[320px] relative animate-in zoom-in-95 duration-300">
        <button @click="resetModal" class="absolute top-3 right-3 text-muted-foreground hover:text-espresso transition-colors">
          <X class="w-5 h-5" />
        </button>
        
        <h3 class="text-lg font-display font-bold text-espresso mb-1 text-center">Đăng Nhập Tích Điểm</h3>
        
        <!-- STEP 1: Nhập Email hoặc Đăng nhập bằng Google -->
        <div v-if="loginStep === 1">
          <p class="text-xs text-muted-foreground text-center mb-4">Đăng nhập nhanh qua Google hoặc Email</p>

          <!-- Nút Đăng nhập bằng Google -->
          <button 
            type="button"
            @click="handleGoogleSignIn"
            class="w-full h-11 mb-4 bg-white hover:bg-cream border-2 border-cream-deep hover:border-caramel/40 rounded-xl text-xs font-bold text-espresso transition-all duration-300 shadow-soft flex items-center justify-center gap-3 active:scale-[0.99] cursor-pointer"
          >
            <!-- Google SVG Icon -->
            <svg class="w-4 h-4" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
              <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
              <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
              <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
            </svg>
            <span>Đăng Nhập bằng Google</span>
          </button>

          <!-- Divider -->
          <div class="flex items-center gap-3 mb-4">
            <div class="flex-1 h-px bg-cream-deep"></div>
            <span class="text-[9px] font-bold text-muted-foreground uppercase tracking-widest">hoặc nhập Email</span>
            <div class="flex-1 h-px bg-cream-deep"></div>
          </div>

          <div class="space-y-4 mb-5">
            <div>
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block text-left">Địa chỉ Email</label>
              <input 
                type="email" 
                v-model="emailInput" 
                placeholder="Ví dụ: khachhang@gmail.com"
                :class="[
                  'w-full h-11 px-4 bg-background border rounded-lg text-sm font-medium focus:outline-none focus:ring-2 transition-all text-espresso',
                  emailError ? 'border-destructive focus:border-destructive focus:ring-destructive/20' : 'border-cream-deep focus:border-caramel focus:ring-caramel/20'
                ]"
                @keyup.enter="checkEmailInput"
                @input="emailError = ''"
              />
              <p v-if="emailError" class="text-[10px] text-destructive font-bold mt-1.5 text-left animate-in fade-in">{{ emailError }}</p>
            </div>
          </div>
          <button 
            @click="checkEmailInput" 
            class="w-full h-11 rounded-lg bg-caramel hover:bg-brown text-cream text-sm font-bold transition-colors uppercase tracking-wider shadow-sm"
          >
            Tiếp tục
          </button>
        </div>

        <!-- STEP 2: Xác nhận danh tính -->
        <div v-else-if="loginStep === 2" class="text-center animate-in fade-in slide-in-from-right-4">
          <p class="text-xs text-muted-foreground mb-4">Chúng tôi tìm thấy thông tin của bạn</p>
          <div class="bg-caramel-light/30 border border-caramel/20 rounded-xl p-4 mb-6">
            <p class="text-[10px] font-bold text-caramel uppercase tracking-wider mb-1">Khách hàng quen</p>
            <p class="text-lg font-bold text-espresso">{{ foundName }}</p>
            <p class="text-xs font-mono text-muted-foreground mt-1">{{ emailInput }}</p>
          </div>
          <p class="text-sm font-bold text-espresso mb-4">Đây có phải là bạn không?</p>
          <div class="flex gap-3">
            <button @click="confirmIdentity(false)" class="flex-1 py-2.5 rounded-lg border border-cream-deep text-muted-foreground hover:text-espresso hover:bg-background text-xs font-bold transition-all">Không phải</button>
            <button @click="confirmIdentity(true)" class="flex-1 py-2.5 rounded-lg bg-espresso hover:bg-brown text-cream text-xs font-bold transition-all shadow-md">Đúng, là tôi</button>
          </div>
        </div>

        <!-- STEP 3: Nhập tên & SĐT (Đăng ký khách mới) -->
        <div v-else-if="loginStep === 3" class="animate-in fade-in slide-in-from-right-4">
          <p class="text-xs text-muted-foreground text-center mb-6">Chào bạn mới! Vui lòng cung cấp thông tin để tích điểm</p>
          <div class="space-y-4 mb-6">
            <div>
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block text-left">Họ và tên</label>
              <input 
                type="text" 
                v-model="fullName" 
                placeholder="Ví dụ: Nguyễn Văn A"
                :class="[
                  'w-full h-11 px-4 bg-background border rounded-lg text-sm font-semibold focus:outline-none focus:ring-2 transition-all text-espresso',
                  nameError ? 'border-destructive focus:border-destructive focus:ring-destructive/20' : 'border-cream-deep focus:border-caramel focus:ring-caramel/20'
                ]"
                @keyup.enter="submitNewName"
                @input="nameError = ''"
              />
              <p v-if="nameError" class="text-[10px] text-destructive font-bold mt-1.5 text-left animate-in fade-in">{{ nameError }}</p>
            </div>
            <div>
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block text-left">Số điện thoại <span class="text-muted-foreground font-normal lowercase">(không bắt buộc)</span></label>
              <input 
                type="tel" 
                v-model="phoneNumber" 
                placeholder="Ví dụ: 0912345678 (Có thể bỏ qua)"
                maxlength="10"
                :class="[
                  'w-full h-11 px-4 bg-background border rounded-lg text-sm font-semibold focus:outline-none focus:ring-2 transition-all text-espresso',
                  phoneError ? 'border-destructive focus:border-destructive focus:ring-destructive/20' : 'border-cream-deep focus:border-caramel focus:ring-caramel/20'
                ]"
                @keyup.enter="submitNewName"
                @input="phoneError = ''"
              />
              <p v-if="phoneError" class="text-[10px] text-destructive font-bold mt-1.5 text-left animate-in fade-in">{{ phoneError }}</p>
            </div>
          </div>
          <div class="flex gap-3">
            <button @click="loginStep = 1" class="px-4 py-2.5 rounded-lg border border-cream-deep text-muted-foreground hover:text-espresso hover:bg-background text-xs font-bold transition-all">Quay lại</button>
            <button @click="submitNewName" class="flex-1 h-11 rounded-lg bg-caramel hover:bg-brown text-cream text-sm font-bold transition-colors uppercase tracking-wider shadow-sm">Hoàn tất</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Custom Toast Notification -->
    <Transition name="toast">
      <div v-if="toastState.show" class="fixed bottom-6 right-6 z-[100] flex items-center gap-3 bg-[#2A231E] text-white px-5 py-3.5 rounded-2xl shadow-2xl border border-white/10">
        <div :class="toastState.type === 'success' ? 'bg-emerald-500/20 text-emerald-400' : 'bg-[#CC8033]/20 text-[#CC8033]'" class="w-8 h-8 rounded-full flex items-center justify-center shrink-0">
          <CheckCircle v-if="toastState.type === 'success'" class="w-4 h-4" stroke-width="2.5" />
          <Coffee v-else class="w-4 h-4" stroke-width="2.5" />
        </div>
        <div>
          <p class="text-sm font-bold">{{ toastState.title }}</p>
          <p class="text-[10px] text-white/60 font-medium mt-0.5">{{ toastState.message }}</p>
        </div>
      </div>
    </Transition>

    <!-- Customer Order History Modal -->
    <Transition name="login-modal">
      <div v-if="isHistoryModalOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-espresso/60 backdrop-blur-sm" @click="isHistoryModalOpen = false"></div>

        <!-- Card -->
        <div class="relative w-full max-w-lg bg-card rounded-3xl shadow-warm border border-cream-deep/60 overflow-hidden flex flex-col max-h-[85vh] z-10 animate-in zoom-in-95 duration-200">
          
          <!-- Modal Header -->
          <div class="p-6 border-b border-cream-deep/60 bg-cream-light/40 flex items-center justify-between">
            <div class="flex items-center gap-3 text-left">
              <div class="w-10 h-10 rounded-2xl bg-caramel/15 text-caramel flex items-center justify-center font-bold">
                <History class="w-5 h-5" />
              </div>
              <div>
                <h3 class="font-display font-bold text-lg text-espresso leading-tight">Lịch Sử Đơn Hàng Của Tôi</h3>
                <p class="text-xs text-muted-foreground mt-0.5">{{ customerProfile?.name }} · {{ customerProfile?.email }}</p>
              </div>
            </div>
            <button @click="isHistoryModalOpen = false" class="w-8 h-8 rounded-full bg-card border border-cream-deep flex items-center justify-center text-muted-foreground hover:text-espresso transition-colors">
              <X class="w-4 h-4" />
            </button>
          </div>

          <!-- Modal Content -->
          <div class="p-6 flex-1 overflow-y-auto space-y-4 text-left custom-scrollbar">
            <!-- Loading state -->
            <div v-if="historyLoading" class="py-12 text-center text-muted-foreground space-y-3">
              <div class="w-8 h-8 border-4 border-caramel border-t-transparent rounded-full animate-spin mx-auto"></div>
              <p class="text-xs font-bold">Đang tải lịch sử mua hàng...</p>
            </div>

            <!-- Empty state -->
            <div v-else-if="historyOrders.length === 0" class="py-12 text-center space-y-3">
              <div class="w-14 h-14 bg-cream-light rounded-2xl flex items-center justify-center mx-auto text-muted-foreground border border-cream-deep">
                <Coffee class="w-7 h-7" />
              </div>
              <h4 class="font-bold text-sm text-espresso">Bạn chưa có đơn hàng nào</h4>
              <p class="text-xs text-muted-foreground max-w-xs mx-auto">Khi bạn đặt món thành công tại quán, lịch sử đơn hàng sẽ xuất hiện tại đây.</p>
              <router-link
                :to="tableNumber ? `/menu/${tableNumber}` : '/menu/xem-menu'"
                @click="isHistoryModalOpen = false"
                class="inline-block px-5 py-2.5 bg-caramel hover:bg-caramel-light text-espresso text-xs font-bold rounded-xl transition-all shadow-soft mt-2"
              >
                Khám phá thực đơn
              </router-link>
            </div>

            <!-- Order List -->
            <div v-else class="space-y-4">
              <div v-for="order in historyOrders" :key="order.maDonHang" class="bg-background rounded-2xl p-4 border border-cream-deep shadow-soft space-y-3">
                
                <!-- Order Card Header -->
                <div class="flex items-center justify-between border-b border-cream-deep/60 pb-3">
                  <div>
                    <div class="flex items-center gap-2">
                      <span class="font-display font-extrabold text-sm text-espresso">#DH-{{ order.maDonHang }}</span>
                      <span
                        class="text-[9px] px-2 py-0.5 rounded-full font-bold uppercase"
                        :class="{
                          'bg-emerald-100 text-emerald-800': order.trangThaiDon === 'HoanThanh',
                          'bg-amber-100 text-amber-800': order.trangThaiDon === 'DangPha',
                          'bg-blue-100 text-blue-800': order.trangThaiDon === 'ChoXacNhan',
                          'bg-red-100 text-red-800': order.trangThaiDon === 'Huy'
                        }"
                      >
                        {{ order.trangThaiDon === 'HoanThanh' ? 'Hoàn tất' : order.trangThaiDon === 'DangPha' ? 'Đang pha' : order.trangThaiDon === 'ChoXacNhan' ? 'Chờ xác nhận' : 'Đã hủy' }}
                      </span>
                    </div>
                    <div class="text-[10px] text-muted-foreground mt-0.5">
                      {{ new Date(order.thoiGianTao).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' }) }} · {{ new Date(order.thoiGianTao).toLocaleDateString('vi-VN') }}
                      <span v-if="order.tenBan" class="ml-1 font-bold text-caramel">({{ order.tenBan }})</span>
                    </div>
                  </div>
                  <div class="text-right">
                    <div class="text-xs font-extrabold text-caramel">{{ formatVND(order.thanhTien) }}</div>
                    <div class="text-[9px] text-muted-foreground">{{ order.soMon }} món</div>
                  </div>
                </div>

                <!-- Items list -->
                <div class="space-y-1.5 pt-1">
                  <div v-for="item in order.items" :key="item.maChiTiet" class="flex justify-between items-center text-xs">
                    <span class="font-medium text-espresso">
                      {{ item.soLuong }}x {{ item.tenMon }}
                      <span v-if="item.tenKichCo" class="text-[10px] text-muted-foreground">({{ item.tenKichCo }})</span>
                    </span>
                    <span class="font-bold text-espresso/80">{{ formatVND(item.thanhTien) }}</span>
                  </div>
                </div>

              </div>
            </div>

          </div>

          <!-- Modal Footer -->
          <div class="p-4 border-t border-cream-deep/60 bg-cream-light/40 text-center">
            <button @click="isHistoryModalOpen = false" class="w-full py-2.5 rounded-xl bg-espresso hover:bg-brown text-cream text-xs font-bold transition-all shadow-soft">
              Đóng
            </button>
          </div>

        </div>
      </div>
    </Transition>

    <!-- MODAL CHỌN TÀI KHOẢN GOOGLE -->
    <div
      v-if="showGoogleChooserModal"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm z-[200] flex items-center justify-center p-4"
      @click.self="showGoogleChooserModal = false"
    >
      <div class="bg-white rounded-3xl shadow-2xl w-full max-w-md overflow-hidden border border-cream-deep">
        <!-- Header -->
        <div class="p-6 pb-4 border-b border-cream-deep flex items-center justify-between bg-cream/30">
          <div class="flex items-center gap-3">
            <div class="w-10 h-10 rounded-2xl bg-white border border-cream-deep shadow-sm flex items-center justify-center">
              <svg class="w-5 h-5" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
                <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
              </svg>
            </div>
            <div class="text-left">
              <h3 class="text-base font-bold text-espresso">Đăng nhập bằng Google</h3>
              <p class="text-xs text-muted-foreground">Chọn hoặc nhập tài khoản Google của bạn</p>
            </div>
          </div>
          <button @click="showGoogleChooserModal = false" class="p-2 rounded-xl hover:bg-cream-deep transition-colors">
            <X class="w-4 h-4 text-muted-foreground" />
          </button>
        </div>

        <!-- Body -->
        <div class="p-6 space-y-4 max-h-[60vh] overflow-y-auto">
          <!-- Switch mode: Select existing account OR Add new custom account -->
          <div v-if="!showCustomGoogleInput">
            <p class="text-xs font-semibold text-espresso mb-3 text-left">Chọn tài khoản Google đang có:</p>

            <div class="space-y-2">
              <button
                v-for="acc in googleAccountOptions"
                :key="acc.email"
                @click="selectGoogleAccount(acc)"
                class="w-full flex items-center justify-between p-3.5 rounded-2xl border border-cream-deep hover:border-[#CC8033] hover:bg-cream/40 transition-all text-left group cursor-pointer"
              >
                <div class="flex items-center gap-3 min-w-0">
                  <div class="w-9 h-9 rounded-full bg-[#CC8033]/15 text-[#CC8033] font-bold text-sm flex items-center justify-center shrink-0">
                    {{ (acc.name || acc.email).charAt(0).toUpperCase() }}
                  </div>
                  <div class="min-w-0">
                    <p class="text-xs font-bold text-espresso group-hover:text-[#CC8033] truncate">{{ acc.name }}</p>
                    <p class="text-[11px] text-muted-foreground truncate">{{ acc.email }}</p>
                  </div>
                </div>
                <span class="text-[10px] font-bold px-3 py-1 rounded-full bg-cream-deep text-espresso group-hover:bg-[#CC8033] group-hover:text-white transition-colors shrink-0">
                  Chọn
                </span>
              </button>
            </div>

            <button
              @click="showCustomGoogleInput = true"
              class="w-full mt-4 py-3 rounded-2xl border-2 border-dashed border-cream-deep hover:border-[#CC8033] text-xs font-bold text-[#CC8033] hover:bg-cream/30 transition-all flex items-center justify-center gap-2 cursor-pointer"
            >
              <Plus class="w-4 h-4" /> Đăng nhập bằng Gmail / Google khác
            </button>
          </div>

          <!-- Custom Google Login Input -->
          <div v-else class="space-y-3 text-left">
            <div class="flex items-center justify-between mb-1">
              <span class="text-xs font-bold text-espresso">Nhập tài khoản Google mới</span>
              <button @click="showCustomGoogleInput = false" class="text-xs text-[#CC8033] font-semibold hover:underline">
                Quay lại danh sách
              </button>
            </div>

            <div>
              <label class="block text-[11px] font-semibold text-espresso mb-1">Tên tài khoản Google</label>
              <input
                type="text"
                v-model="customGoogleName"
                placeholder="Ví dụ: Phạm Thành Tài"
                class="w-full px-3.5 py-2.5 text-xs rounded-xl border border-cream-deep focus:outline-none focus:border-[#CC8033]"
              />
            </div>

            <div>
              <label class="block text-[11px] font-semibold text-espresso mb-1">Địa chỉ Email (Gmail)</label>
              <input
                type="email"
                v-model="customGoogleEmail"
                placeholder="Ví dụ: taiptpk04158@gmail.com"
                class="w-full px-3.5 py-2.5 text-xs rounded-xl border border-cream-deep focus:outline-none focus:border-[#CC8033]"
              />
            </div>

            <button
              @click="submitCustomGoogleLogin"
              :disabled="!customGoogleName.trim() || !customGoogleEmail.trim()"
              class="w-full h-11 mt-2 rounded-xl bg-[#CC8033] hover:bg-[#b5702b] text-white text-xs font-bold shadow-md transition-all disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer"
            >
              Xác nhận Đăng nhập Google
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Floating Mobile Bottom Navigation Bar -->
    <div class="md:hidden fixed bottom-0 left-0 right-0 z-40 bg-[#1C130E]/95 text-white backdrop-blur-xl border-t border-white/15 p-2 px-4 shadow-[0_-10px_30px_rgba(0,0,0,0.5)]">
      <div class="flex items-center justify-around gap-2">
        <!-- Button 1: Menu -->
        <a href="#menu" @click.prevent="scrollToSection('menu')" class="flex flex-col items-center gap-1 text-white/80 hover:text-white transition-colors">
          <Coffee class="w-5 h-5 text-[#E89E53]" />
          <span class="text-[10px] font-bold">Thực đơn</span>
        </a>

        <!-- Button 2 (Center Glowing CTA): QUÉT QR -->
        <button @click="openQrScannerModal" class="flex flex-col items-center gap-1 group relative">
          <div class="w-12 h-12 rounded-full bg-gradient-to-tr from-[#CC8033] via-[#E89E53] to-[#F59E0B] p-0.5 shadow-[0_0_20px_rgba(204,128,51,0.6)] group-active:scale-95 transition-all">
            <div class="w-full h-full bg-[#1C130E] rounded-full flex items-center justify-center">
              <QrCode class="w-6 h-6 text-[#E89E53] animate-pulse" />
            </div>
          </div>
          <span class="text-[10px] font-black text-[#E89E53] uppercase tracking-wide">Quét QR Bàn</span>
        </button>

        <!-- Button 3: Account / Login -->
        <button v-if="!customerProfile" @click="isPhoneModalOpen = true" class="flex flex-col items-center gap-1 text-white/80 hover:text-white transition-colors">
          <User class="w-5 h-5 text-[#E89E53]" />
          <span class="text-[10px] font-bold">Đăng nhập</span>
        </button>
        <button v-else @click="openCustomerHistoryModal" class="flex flex-col items-center gap-1 text-white/80 hover:text-white transition-colors">
          <History class="w-5 h-5 text-[#E89E53]" />
          <span class="text-[10px] font-bold">Lịch sử</span>
        </button>
      </div>
    </div>

    <!-- Modal Quét Mã QR & Chọn Bàn -->
    <div v-if="showQrScannerModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/80 backdrop-blur-md animate-in fade-in duration-200" @click="closeQrScannerModal">
      <div class="bg-[#1C130E] border border-white/15 rounded-3xl shadow-2xl max-w-md w-full overflow-hidden relative text-white animate-in zoom-in-95 duration-300" @click.stop>
        
        <!-- Modal Header -->
        <div class="p-4 border-b border-white/10 flex items-center justify-between bg-white/5">
          <div class="flex items-center gap-2">
            <div class="w-8 h-8 rounded-xl bg-[#CC8033]/20 flex items-center justify-center text-[#E89E53]">
              <Camera class="w-4 h-4" />
            </div>
            <span class="font-bold text-sm text-white">Quét QR Gọi Món tại Bàn</span>
          </div>
          <button @click="closeQrScannerModal" class="w-8 h-8 rounded-full bg-white/10 hover:bg-white/20 flex items-center justify-center text-white/70 transition-colors">
            <X class="w-4 h-4" />
          </button>
        </div>

        <!-- Modal Body -->
        <div class="p-5 space-y-5 text-center">
          
          <!-- Camera Preview Frame -->
          <div class="relative w-full aspect-square max-w-[240px] mx-auto rounded-2xl overflow-hidden bg-black/60 border-2 border-[#CC8033]/60 shadow-lg flex flex-col items-center justify-center">
            <video ref="qrVideoRef" autoplay playsinline class="w-full h-full object-cover" v-show="isCameraActive"></video>
            
            <!-- Target Scanner Overlay -->
            <div v-if="isCameraActive" class="absolute inset-0 pointer-events-none flex flex-col items-center justify-center p-6">
              <div class="w-full h-full border-2 border-dashed border-[#E89E53] rounded-xl animate-pulse flex items-center justify-center">
                <div class="w-2.5 h-2.5 rounded-full bg-[#CC8033] shadow-[0_0_12px_#CC8033]"></div>
              </div>
              <span class="text-[10px] text-white/80 bg-black/60 px-2 py-0.5 rounded-full mt-2 font-semibold">Hướng ống kính vào mã QR dán ở bàn</span>
            </div>

            <!-- Camera Fallback -->
            <div v-else class="p-4 flex flex-col items-center text-center space-y-3">
              <CameraOff class="w-10 h-10 text-white/30" />
              <p class="text-xs text-white/70 font-medium leading-relaxed max-w-[200px]">
                {{ cameraError || 'Vui lòng dùng Điện thoại mở camera quét mã QR dán tại bàn nhé!' }}
              </p>
              <button @click="startQrCamera" class="px-3.5 py-1.5 bg-white/10 hover:bg-white/20 text-white text-[11px] font-bold rounded-lg transition-colors border border-white/10">
                🔄 Thử mở lại camera
              </button>
            </div>
          </div>

        </div>

        <!-- Modal Footer -->
        <div class="p-3 bg-white/5 border-t border-white/10 text-center">
          <button @click="closeQrScannerModal" class="px-5 py-2 bg-white/10 hover:bg-white/20 text-white font-bold text-xs rounded-xl transition-colors">
            Đóng
          </button>
        </div>
      </div>
    </div>

    <!-- Floating Nút Gọi Phục Vụ -->
    <button
      @click="openCallSupportModal"
      class="fixed bottom-6 left-6 z-40 px-4 py-3 rounded-full bg-gradient-to-r from-red-600 via-amber-600 to-red-600 hover:scale-105 text-white shadow-xl shadow-red-900/40 flex items-center gap-2 font-bold text-xs transition-all duration-300 active:scale-95 group border border-white/20 cursor-pointer"
      title="Gọi nhân viên hỗ trợ tại bàn"
    >
      <BellRing class="w-4 h-4 group-hover:rotate-12 transition-transform animate-bounce" />
      <span class="tracking-wide">Gọi phục vụ</span>
    </button>

    <!-- MODAL BÁO CẦN QUÉT MÃ QR BÀN -->
    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <div v-if="showNeedQrScanModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-md">
        <div class="bg-[#1C130E] border border-white/15 text-white rounded-3xl max-w-sm w-full p-6 shadow-2xl space-y-5 text-center relative overflow-hidden">
          <button @click="showNeedQrScanModal = false" class="absolute top-4 right-4 w-8 h-8 rounded-full bg-white/10 hover:bg-white/20 flex items-center justify-center text-white/70 hover:text-white transition-colors cursor-pointer">
            <X class="w-4 h-4" />
          </button>

          <div class="w-16 h-16 mx-auto rounded-2xl bg-gradient-to-br from-[#CC8033] to-[#E89E53] flex items-center justify-center shadow-lg shadow-[#CC8033]/30">
            <QrCode class="w-8 h-8 text-white animate-pulse" />
          </div>

          <div class="space-y-2">
            <h3 class="font-premium-serif text-lg font-bold text-[#E89E53]">Cần Quét Mã QR Tại Bàn</h3>
            <p class="text-xs text-white/80 leading-relaxed">
              Bạn chưa ở trong phiên phục vụ của bàn nào. Vui lòng quét mã QR dán trên mặt bàn tại quán để bắt đầu gọi phục vụ &amp; đặt món nhé!
            </p>
          </div>

          <div class="space-y-2 pt-2">
            <button
              @click="showNeedQrScanModal = false; openQrScannerModal()"
              class="w-full h-12 rounded-xl bg-gradient-to-r from-[#CC8033] via-[#E89E53] to-[#F59E0B] text-white font-bold text-xs uppercase tracking-wider shadow-lg shadow-[#CC8033]/30 transition-all flex items-center justify-center gap-2 cursor-pointer active:scale-95"
            >
              <Camera class="w-4 h-4" />
              <span>Quét mã QR Bàn ngay</span>
            </button>
            <button
              @click="showNeedQrScanModal = false"
              class="w-full h-10 rounded-xl bg-white/5 border border-white/10 text-white/70 text-xs font-bold hover:bg-white/10 transition-colors cursor-pointer"
            >
              Để sau
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <!-- MODAL GỌI PHỤC VỤ HỖ TRỢ TẠI BÀN (Khi đã ở trong bàn) -->
    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0 scale-95"
      enter-to-class="opacity-100 scale-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100 scale-100"
      leave-to-class="opacity-0 scale-95"
    >
      <div v-if="showCallSupportModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-md">
        <div class="bg-[#1C130E] border border-white/15 text-white rounded-3xl max-w-md w-full p-6 shadow-2xl space-y-5 relative overflow-hidden">
          <button @click="showCallSupportModal = false" class="absolute top-4 right-4 w-8 h-8 rounded-full bg-white/10 hover:bg-white/20 flex items-center justify-center text-white/70 hover:text-white transition-colors cursor-pointer">
            <X class="w-4 h-4" />
          </button>

          <div class="flex items-center gap-3">
            <div class="w-12 h-12 rounded-2xl bg-gradient-to-br from-red-500 to-amber-500 flex items-center justify-center shadow-lg shadow-red-900/30">
              <BellRing class="w-6 h-6 text-white animate-bounce" />
            </div>
            <div class="text-left">
              <h3 class="font-premium-serif text-lg font-bold text-red-400">Gọi Nhân Viên Hỗ Trợ</h3>
              <div class="flex items-center gap-2 mt-0.5">
                <span class="text-xs text-white/70">Vị trí:</span>
                <span class="text-xs font-bold text-[#E89E53] bg-[#CC8033]/20 px-2.5 py-0.5 rounded-md border border-[#CC8033]/40">
                  📍 {{ currentTableName || 'Bàn ' + selectedTableId }}
                </span>
              </div>
            </div>
          </div>

          <!-- Gợi ý nhanh yêu cầu -->
          <div class="space-y-2 text-left">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#D5B08D] block">Chọn nhanh nhu cầu:</label>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="chip in quickChips"
                :key="chip"
                @click="callSupportNote = chip"
                :class="[
                  'px-3 py-1.5 rounded-xl text-xs font-semibold border transition-all cursor-pointer',
                  callSupportNote === chip
                    ? 'bg-[#CC8033] border-[#CC8033] text-white font-bold shadow-md'
                    : 'bg-white/5 border-white/10 text-white/80 hover:bg-white/10'
                ]"
              >
                {{ chip }}
              </button>
            </div>
          </div>

          <!-- Ghi chú tùy chọn -->
          <div class="space-y-1.5 text-left">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#D5B08D] block">Ghi chú cụ thể (Tùy chọn):</label>
            <input
              v-model="callSupportNote"
              placeholder="VD: Cho xin thêm 2 ly đá, nước lọc, nĩa..."
              class="w-full px-4 py-2.5 bg-black/50 border border-white/15 rounded-xl text-xs text-white placeholder:text-white/30 focus:outline-none focus:border-[#CC8033] transition-colors"
            />
          </div>

          <div class="flex gap-2.5 pt-2">
            <button
              @click="showCallSupportModal = false"
              class="flex-1 h-11 rounded-xl border border-white/20 bg-white/5 text-white/80 hover:bg-white/10 font-bold text-xs uppercase tracking-wider transition-all cursor-pointer"
            >
              Hủy
            </button>
            <button
              @click="sendCallSupportRequest"
              :disabled="callStaffLoading || !selectedTableId"
              class="flex-1 h-11 rounded-xl bg-gradient-to-r from-red-600 via-amber-600 to-red-600 hover:from-red-500 hover:to-amber-500 text-white font-bold text-xs uppercase tracking-wider shadow-lg shadow-red-900/40 transition-all flex items-center justify-center gap-2 disabled:opacity-50 cursor-pointer"
            >
              <BellRing class="w-4 h-4" />
              {{ callStaffLoading ? 'Đang gửi...' : 'Gửi Yêu Cầu' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>

    <ChatbotWidget />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { 
  Coffee, ArrowRight, QrCode, LayoutDashboard, X, History, Sparkles, CheckCircle, MapPin, Phone, Plus, Camera, CameraOff, User, BellRing
} from 'lucide-vue-next';
import heroImg from '@/assets/cafe-hero.jpg';
import menuCoffee from '@/assets/menu-coffee.jpg';
import menuTea from '@/assets/menu-tea.jpg';
import menuFrappe from '@/assets/menu-frappe.jpg';
import menuPastry from '@/assets/menu-pastry.jpg';
import { useStoreInfoStore } from '@/stores/storeInfo';
import { ordersApi, type MenuItem } from '@/services/orders';
import ChatbotWidget from '@/components/ChatbotWidget.vue';
import { loyaltyApi } from '@/services/loyalty';
import { promotionsApi, type Promotion } from '@/services/promotions';

const formatVND = (n: number) => (n || 0).toLocaleString('vi-VN') + 'đ';

const router = useRouter()
const route = useRoute()
const storeInfoStore = useStoreInfoStore()

// Lấy số bàn từ Query Param (nếu khách quét QR code có gắn link dạng ?ban=5 hoặc ?table=5)
const tableNumber = computed(() => route.query.ban || route.query.table || null)

// Dùng store toàn cục — App.vue đã fetch khi khởi động
const tenQuan  = computed(() => storeInfoStore.tenQuan)
const moTaQuan = computed(() => storeInfoStore.moTaQuan)

// Modal and loyalty state management
const isPhoneModalOpen = ref(false)
const isHistoryModalOpen = ref(false)
const historyOrders = ref<any[]>([])
const historyLoading = ref(false)

// Gọi phục vụ hỗ trợ từ Trang chủ
const showCallSupportModal = ref(false)
const showNeedQrScanModal = ref(false)
const callStaffLoading = ref(false)
const selectedTableId = ref<number | null>(null)
const currentTableName = ref('')
const tablesList = ref<any[]>([])
const callSupportNote = ref('')
const quickChips = ref(['🧊 Xin thêm ly đá', '🧻 Cho xin khăn lạnh', '💧 Xin nước lọc', '💳 Hỗ trợ thanh toán'])

const openCallSupportModal = async () => {
  const savedId = localStorage.getItem('user_table_id')
  const savedName = localStorage.getItem('user_table_name')

  if (savedId) {
    selectedTableId.value = parseInt(savedId, 10)
    currentTableName.value = savedName || `Bàn ${savedId}`
    showCallSupportModal.value = true
  } else {
    // Khách chưa quét QR bàn -> yêu cầu quét mã QR
    showNeedQrScanModal.value = true
  }
}

const sendCallSupportRequest = async () => {
  if (!selectedTableId.value) {
    showNeedQrScanModal.value = true
    return
  }
  const tenBanStr = currentTableName.value || `Bàn ${selectedTableId.value}`

  callStaffLoading.value = true
  try {
    await ordersApi.createServiceRequest({
      maBan: selectedTableId.value,
      loaiYeuCau: 'GoiPhucVu',
      ghiChu: `Khách gọi hỗ trợ tại ${tenBanStr}${callSupportNote.value ? ' - Yêu cầu: ' + callSupportNote.value : ''}`
    })

    if (typeof BroadcastChannel !== 'undefined') {
      const channel = new BroadcastChannel('quanlycf_orders_sync')
      channel.postMessage({ type: 'SERVICE_REQUEST_CHANGED', ts: Date.now() })
    }

    showToast('Đã gửi yêu cầu', `Đã gửi yêu cầu gọi phục vụ cho ${tenBanStr}! Nhân viên sẽ tới ngay ạ.`, 'success')
    showCallSupportModal.value = false
    callSupportNote.value = ''
  } catch (err: any) {
    showToast('Lỗi gửi yêu cầu', err.message || 'Không thể gửi yêu cầu gọi phục vụ.', 'info')
  } finally {
    callStaffLoading.value = false
  }
}

const openCustomerHistoryModal = async () => {
  if (!customerProfile.value || !customerProfile.value.email) {
    showToast('Chưa đăng nhập', 'Vui lòng đăng nhập bằng Google hoặc Email để xem lịch sử đơn hàng.', 'info')
    return
  }
  isHistoryModalOpen.value = true
  historyLoading.value = true
  try {
    const res = await ordersApi.getCustomerHistory(customerProfile.value.email)
    historyOrders.value = res || []
  } catch (e) {
    console.error('Không lấy được lịch sử đơn hàng:', e)
    historyOrders.value = []
  } finally {
    historyLoading.value = false
  }
}

const loginStep = ref<1 | 2 | 3>(1)
const foundName = ref('')
const fullName = ref('')
const phoneNumber = ref('')
const emailInput = ref('')
const nameError = ref('')
const phoneError = ref('')
const emailError = ref('')
const customerProfile = ref<{ id: number; name: string; phone: string; email?: string; tier: string; points: number } | null>(null)

const nextTierInfo = computed(() => {
  if (!customerProfile.value) return null
  const pts = customerProfile.value.points || 0
  const tier = customerProfile.value.tier
  
  if (pts >= 3000) {
    return {
      nextTier: 'Kim cương',
      pointsNeeded: 0,
      percent: 100,
      text: 'Đã đạt cấp độ tối đa!'
    }
  }
  
  let target = 500
  let nextName = 'Bạc'
  let base = 0
  
  if (pts >= 1500) {
    target = 3000
    nextName = 'Kim cương'
    base = 1500
  } else if (pts >= 500) {
    target = 1500
    nextName = 'Vàng'
    base = 500
  } else {
    target = 500
    nextName = 'Bạc'
    base = 0
  }
  
  const needed = target - pts
  const range = target - base
  const currentInRange = pts - base
  const percent = Math.min(100, Math.max(0, Math.round((currentInRange / range) * 100)))
  
  return {
    nextTier: nextName,
    pointsNeeded: needed,
    percent: percent,
    text: `Cần thêm ${needed} điểm để thăng hạng ${nextName}`
  }
})

const STORAGE_KEY = 'brewCustomerProfile'

// Toast Notification State
const toastState = ref({
  show: false,
  title: '',
  message: '',
  type: 'success' as 'success' | 'info'
})

let toastTimeout: ReturnType<typeof setTimeout> | null = null

const showToast = (title: string, message: string, type: 'success' | 'info' = 'success') => {
  toastState.value = { show: true, title, message, type }
  if (toastTimeout) clearTimeout(toastTimeout)
  toastTimeout = setTimeout(() => {
    toastState.value.show = false
  }, 3000)
}

const checkLoginStatus = async () => {
  const saved = localStorage.getItem(STORAGE_KEY)
  if (saved) {
    try {
      const basic = JSON.parse(saved)
      if (basic && basic.email) {
        try {
          const res = await loyaltyApi.checkPublicEmail(basic.email)
          customerProfile.value = res
          localStorage.setItem(STORAGE_KEY, JSON.stringify(res))
        } catch (e) {
          customerProfile.value = basic
        }
      }
    } catch (e) {
      customerProfile.value = null
    }
  } else {
    customerProfile.value = null
  }
}

const activePromos = ref<Promotion[]>([])
const fmtD = (iso: string | null) => iso ? new Date(iso).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: '2-digit' }) : '∞'

const loadActivePromotions = async () => {
  try {
    activePromos.value = await promotionsApi.active()
  } catch (e) {
    console.error('Không tải được danh sách khuyến mãi:', e)
  }
}

const savedVouchersList = ref<string[]>([])
const loadSavedVouchersList = () => {
  try {
    const key = 'savedVouchers'
    savedVouchersList.value = JSON.parse(localStorage.getItem(key) || '[]')
  } catch (e) {
    savedVouchersList.value = []
  }
}

const isVoucherSaved = (code: string) => {
  return savedVouchersList.value.includes(code)
}

const googleClientId = ref((import.meta.env as any).VITE_GOOGLE_CLIENT_ID || '')

const initGoogleGIS = () => {
  if (googleClientId.value && typeof window !== 'undefined' && (window as any).google?.accounts?.id) {
    try {
      (window as any).google.accounts.id.initialize({
        client_id: googleClientId.value,
        callback: handleGoogleCredentialResponse,
        cancel_on_tap_outside: true,
      })
    } catch (e) {
      console.warn('Google GIS initialize note:', e)
    }
  }
}

const handleGoogleSignIn = () => {
  if (googleClientId.value && typeof window !== 'undefined' && (window as any).google?.accounts?.id) {
    try {
      (window as any).google.accounts.id.initialize({
        client_id: googleClientId.value,
        callback: handleGoogleCredentialResponse,
        cancel_on_tap_outside: true,
      })
      ;(window as any).google.accounts.id.prompt((notification: any) => {
        if (notification.isNotDisplayed() || notification.isSkippedMoment()) {
          promptGmailInput()
        }
      })
    } catch {
      promptGmailInput()
    }
  } else {
    promptGmailInput()
  }
}

const showGoogleChooserModal = ref(false)
const showCustomGoogleInput = ref(false)
const customGoogleName = ref('')
const customGoogleEmail = ref('')
const googleAccountOptions = ref<{ name: string; email: string }[]>([])

const promptGmailInput = async () => {
  try {
    const list = await loyaltyApi.list()
    if (list && list.length) {
      googleAccountOptions.value = list
        .filter(c => c.name || c.email)
        .map(c => ({
          name: c.name,
          email: c.email || `${c.phone || c.id}@gmail.com`
        }))
    } else {
      googleAccountOptions.value = [
        { name: 'Tài Phạm Thành', email: 'taiptpk04158@gmail.com' },
        { name: 'Phạm Thành Tài', email: 'phamthanhtai16102006@gmail.com' }
      ]
    }
  } catch {
    googleAccountOptions.value = [
      { name: 'Tài Phạm Thành', email: 'taiptpk04158@gmail.com' },
      { name: 'Phạm Thành Tài', email: 'phamthanhtai16102006@gmail.com' }
    ]
  }
  showCustomGoogleInput.value = false
  customGoogleName.value = ''
  customGoogleEmail.value = ''
  showGoogleChooserModal.value = true
}

const selectGoogleAccount = async (acc: { name: string; email: string }) => {
  showGoogleChooserModal.value = false
  emailInput.value = acc.email
  await processGoogleLogin(acc.email, acc.name)
}

const submitCustomGoogleLogin = async () => {
  const name = customGoogleName.value.trim()
  const email = customGoogleEmail.value.trim()
  if (!name || !email) return
  showGoogleChooserModal.value = false
  emailInput.value = email
  await processGoogleLogin(email, name)
}

const handleGoogleCredentialResponse = async (response: any) => {
  if (!response?.credential) return
  try {
    const base64Url = response.credential.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    const payload = JSON.parse(jsonPayload)

    const gEmail = payload.email
    const gName = payload.name || payload.given_name || 'Khách hàng Google'

    if (!gEmail) return

    emailInput.value = gEmail
    await processGoogleLogin(gEmail, gName)
  } catch (e: any) {
    showToast('Lỗi đăng nhập Google', e.message || 'Không thể xác thực với Google.', 'info')
  }
}

const processGoogleLogin = async (email: string, name: string) => {
  try {
    // 1. Thử đăng nhập nếu đã có tài khoản
    const customer = await loyaltyApi.checkPublicEmail(email)
    localStorage.setItem(STORAGE_KEY, JSON.stringify(customer))
    customerProfile.value = customer
    showToast('Đăng nhập Google thành công', `Chào mừng ${customer.name} trở lại!`)
    resetModal()
  } catch {
    // 2. Chưa có tài khoản -> Tự động tạo mới tức thì với thông tin Google (không hỏi SĐT)
    try {
      const newCustomer = await loyaltyApi.registerPublic({
        name: name,
        email: email,
        phone: ''
      })
      localStorage.setItem(STORAGE_KEY, JSON.stringify(newCustomer))
      customerProfile.value = newCustomer
      showToast('Đăng nhập Google thành công', `Chào mừng ${newCustomer.name} đến với ${tenQuan.value || 'BrewManager'}!`)
      resetModal()
    } catch (regErr: any) {
      showToast('Lỗi đăng ký', regErr.message || 'Không thể hoàn tất đăng nhập Google.', 'info')
    }
  }
}

onMounted(() => {
  checkLoginStatus()
  fetchRealMenu()
  loadActivePromotions()
  loadSavedVouchersList()
  setTimeout(initGoogleGIS, 800)
})

const fetchRealMenu = async () => {
  try {
    const data = await ordersApi.menu()
    realMenu.value = data.filter(m => m.kieuMon !== 'Topping')
  } catch (e) {
    console.error('Failed to fetch menu', e)
  }
}

const handleLogout = () => {
  localStorage.removeItem(STORAGE_KEY)
  customerProfile.value = null
  showToast('Đã đăng xuất', 'Bạn đã đăng xuất khỏi hệ thống tích điểm.', 'info')
}

const fixEmailTypo = (email: string): string => {
  if (!email) return ''
  return email
    .trim()
    .toLowerCase()
    .replace(/@gmai\.com$/i, '@gmail.com')
    .replace(/@gamil\.com$/i, '@gmail.com')
    .replace(/@gmaill\.com$/i, '@gmail.com')
    .replace(/@gmal\.com$/i, '@gmail.com')
    .replace(/@gmial\.com$/i, '@gmail.com')
    .replace(/@hotmai\.com$/i, '@hotmail.com')
    .replace(/@yaho\.com$/i, '@yahoo.com')
}

const checkEmailInput = async () => {
  emailError.value = '';
  const emailVal = fixEmailTypo(emailInput.value);
  emailInput.value = emailVal;

  if (!emailVal) {
    emailError.value = 'Vui lòng nhập địa chỉ email!';
    return;
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailVal)) {
    emailError.value = 'Địa chỉ email không hợp lệ!';
    return;
  }

  try {
    const customer = await loyaltyApi.checkPublicEmail(emailVal)
    foundName.value = customer.name;
    loginStep.value = 2; // Xác nhận danh tính
  } catch (err: any) {
    // Không tìm thấy email trong CSDL thật → Mời nhập tên đăng ký mới
    loginStep.value = 3;
  }
}

const confirmIdentity = async (isMe: boolean) => {
  if (isMe) {
    try {
      const customer = await loyaltyApi.checkPublicEmail(emailInput.value.trim().toLowerCase())
      localStorage.setItem(STORAGE_KEY, JSON.stringify(customer))
      customerProfile.value = customer
      showToast('Đăng nhập thành công', `Chào mừng ${customer.name} trở lại!`);
      resetModal();
    } catch (e: any) {
      emailError.value = e.message || 'Có lỗi xảy ra.';
    }
  } else {
    loginStep.value = 3;
    fullName.value = '';
    phoneNumber.value = '';
  }
}

const submitNewName = async () => {
  nameError.value = '';
  phoneError.value = '';
  const nameVal = fullName.value.trim();
  const phoneVal = phoneNumber.value.trim();
  const emailVal = fixEmailTypo(emailInput.value);
  emailInput.value = emailVal;

  if (!nameVal) {
    nameError.value = 'Vui lòng nhập họ và tên!';
    return;
  } else if (nameVal.length < 2) {
    nameError.value = 'Họ và tên phải từ 2 ký tự trở lên!';
    return;
  } else if (/\d/.test(nameVal)) {
    nameError.value = 'Họ và tên không được chứa chữ số!';
    return;
  }

  if (phoneVal && !/^0\d{9}$/.test(phoneVal)) {
    phoneError.value = 'Số điện thoại không hợp lệ (gồm 10 số, bắt đầu bằng 0)!';
    return;
  }

  try {
    const customer = await loyaltyApi.registerPublic({ name: nameVal, phone: phoneVal, email: emailVal })
    localStorage.setItem(STORAGE_KEY, JSON.stringify(customer))
    customerProfile.value = customer
    showToast('Đăng nhập thành công', `Chào mừng ${customer.name} đến với hệ thống tích điểm!`);
    resetModal();
  } catch (err: any) {
    nameError.value = err.message || 'Không thể đăng ký tài khoản.';
  }
}

const resetModal = () => {
  isPhoneModalOpen.value = false;
  loginStep.value = 1;
  emailInput.value = '';
  phoneNumber.value = '';
  fullName.value = '';
  emailError.value = '';
  phoneError.value = '';
  nameError.value = '';
  foundName.value = '';
}

// Menu Categories and Items configuration
const realMenu = ref<MenuItem[]>([])
const activeCategory = ref('all')

const scrollToSection = (id: string) => {
  const el = document.getElementById(id)
  if (el) {
    const y = el.getBoundingClientRect().top + window.scrollY - 80 // Offset for sticky header
    window.scrollTo({ top: y, behavior: 'smooth' })
  }
}

const menuCategories = computed(() => {
  const cats = Array.from(new Set(realMenu.value.map(m => m.tenDanhMuc).filter(Boolean) as string[]))
  return [{ id: 'all', name: 'Tất cả' }, ...cats.map(c => ({ id: c, name: c }))]
})

const filteredMenuItems = computed(() => {
  let filtered = activeCategory.value === 'all'
    ? realMenu.value
    : realMenu.value.filter(item => item.tenDanhMuc === activeCategory.value)
    
  // Chỉ hiển thị tối đa 4 món cho gọn (mô phỏng bán chạy)
  return filtered.slice(0, 4)
})

const bestSellingItem = computed(() => {
  return realMenu.value.length > 0 ? realMenu.value[0] : null
})

const copyVoucherCode = (code: string) => {
  navigator.clipboard.writeText(code)
  try {
    const key = 'savedVouchers'
    const current = JSON.parse(localStorage.getItem(key) || '[]')
    if (!current.includes(code)) {
      current.push(code)
      localStorage.setItem(key, JSON.stringify(current))
      savedVouchersList.value = current
    }
  } catch (e) {
    console.error('Không thể lưu voucher:', e)
  }
  showToast('Đã lưu mã', `Mã voucher ${code} đã được lưu thành công vào ví của bạn.`)
}

// --- State Quét QR & Chọn Bàn ---
const showQrScannerModal = ref(false)
const manualTableInput = ref('')
const qrVideoRef = ref<HTMLVideoElement | null>(null)
let qrStream: MediaStream | null = null
const isCameraActive = ref(false)
const cameraError = ref('')

const openQrScannerModal = async () => {
  showQrScannerModal.value = true
  manualTableInput.value = ''
  cameraError.value = ''
  await startQrCamera()
}

const closeQrScannerModal = () => {
  stopQrCamera()
  showQrScannerModal.value = false
}

const startQrCamera = async () => {
  try {
    if (!navigator?.mediaDevices?.getUserMedia) {
      isCameraActive.value = false
      cameraError.value = "Trình duyệt yêu cầu HTTPS hoặc kết nối an toàn để mở Camera trực tiếp. Bạn dùng camera mặc định của điện thoại quét mã QR nhé!"
      return
    }
    qrStream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment' } })
    isCameraActive.value = true
    if (qrVideoRef.value) {
      qrVideoRef.value.srcObject = qrStream
      qrVideoRef.value.play()
    }
  } catch (err: any) {
    console.warn("Camera access denied or unavailable", err)
    isCameraActive.value = false
    cameraError.value = "Vui lòng mở quyền Camera hoặc dùng camera điện thoại quét mã QR dán tại bàn nhé!"
  }
}

const stopQrCamera = () => {
  if (qrStream) {
    qrStream.getTracks().forEach(t => t.stop())
    qrStream = null
  }
  isCameraActive.value = false
}

const selectManualTable = (tId: number | string) => {
  if (!tId) return
  closeQrScannerModal()
  showToast('Chuyển bàn thành công', `Đã chọn Bàn ${tId}!`, 'success')
  router.push(`/menu/${tId}`)
}

const confirmManualTable = () => {
  const num = parseInt(manualTableInput.value.trim())
  if (!num || isNaN(num) || num <= 0) {
    showToast('Nhập số bàn', 'Vui lòng nhập số bàn hợp lệ (Ví dụ: 1, 2, 3...)', 'info')
    return
  }
  selectManualTable(num)
}
</script>

<style scoped>
html {
  scroll-behavior: smooth;
}

@keyframes bounceSubtle {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-4px); }
}

.animate-bounce-subtle {
  animation: bounceSubtle 2s infinite ease-in-out;
}

.toast-enter-active, .toast-leave-active { transition: all 0.3s cubic-bezier(0.4,0,0.2,1); }
.toast-enter-from, .toast-leave-to { opacity: 0; transform: translateY(12px) scale(0.95); }
</style>
