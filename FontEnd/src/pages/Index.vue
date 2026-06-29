<template>
  <div class="min-h-screen bg-background text-espresso selection:bg-caramel-light selection:text-brown">
    <!-- Sticky Glassmorphic Header -->
    <header class="sticky top-0 z-40 bg-[#4A3224]/95 text-cream backdrop-blur-md border-b border-white/10 transition-all shadow-md">
      <div class="max-w-6xl mx-auto px-6 h-20 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-caramel flex items-center justify-center shadow-soft transform hover:rotate-12 transition-transform duration-300">
            <Coffee class="w-5 h-5 text-espresso" />
          </div>
          <div class="flex flex-col text-left">
            <span class="font-display text-lg text-cream font-bold tracking-tight">{{ tenQuan || 'BrewManager Cafe' }}</span>
            <span class="text-[10px] text-caramel-light font-semibold uppercase tracking-wider">Không gian & Cà phê tử tế</span>
          </div>
        </div>
        
        <nav class="hidden md:flex items-center gap-8">
          <a href="#menu" @click.prevent="scrollToSection('menu')" class="text-sm font-bold text-cream/90 hover:text-caramel-light transition-colors cursor-pointer">Thực đơn</a>
          <a href="#loyalty" @click.prevent="scrollToSection('loyalty')" class="text-sm font-bold text-cream/90 hover:text-caramel-light transition-colors cursor-pointer">Thành viên</a>
          <a href="#promotions" @click.prevent="scrollToSection('promotions')" class="text-sm font-bold text-cream/90 hover:text-caramel-light transition-colors cursor-pointer">Ưu đãi</a>
          <a href="#about" @click.prevent="scrollToSection('about')" class="text-sm font-bold text-cream/90 hover:text-caramel-light transition-colors cursor-pointer">Câu chuyện</a>
        </nav>

        <div class="flex items-center gap-3">
          <!-- Logged in status info -->
          <div v-if="customerProfile" class="flex items-center gap-3 bg-white/10 px-3 py-1.5 rounded-lg border border-white/5">
            <div class="w-7 h-7 rounded-full bg-caramel flex items-center justify-center text-espresso text-xs font-bold">
              {{ customerProfile.name.charAt(0).toUpperCase() }}
            </div>
            <div class="hidden lg:block text-left">
              <div class="text-[9px] text-cream/70 font-bold leading-none">Xin chào,</div>
              <div class="text-[11px] font-bold text-cream leading-tight">{{ customerProfile.name }}</div>
            </div>
            <button @click="handleLogout" class="text-[10px] text-red-400 hover:text-red-300 hover:underline ml-1 font-bold">Đăng xuất</button>
          </div>
          <button
            v-else
            @click="isPhoneModalOpen = true"
            class="px-4 py-2 rounded-lg bg-white/10 border border-white/10 text-cream hover:bg-white/20 text-xs font-bold transition-all"
          >
            Đăng nhập tích điểm
          </button>
          
          <router-link
            :to="tableNumber ? `/menu/${tableNumber}` : '/menu/5'"
            class="px-4 py-2 rounded-lg bg-caramel hover:bg-caramel-light text-espresso text-xs font-bold transition-all shadow-warm"
          >
            {{ tableNumber ? `Gọi món tại Bàn ${tableNumber}` : 'Gọi món tại bàn (Mã QR)' }}
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

            <div class="flex flex-wrap gap-4 pt-2">
              <a
                v-if="!tableNumber"
                href="#menu"
                @click.prevent="scrollToSection('menu')"
                class="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-espresso hover:bg-brown text-cream font-bold transition-all hover:scale-[1.02] shadow-warm animate-bounce-subtle cursor-pointer z-10 relative"
              >
                Khám Phá Thực Đơn
                <ArrowRight class="w-4 h-4" />
              </a>
              <router-link
                v-else
                :to="`/menu/${tableNumber}`"
                class="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-espresso hover:bg-brown text-cream font-bold transition-all hover:scale-[1.02] shadow-warm animate-bounce-subtle"
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
            <h2 class="font-display text-3xl sm:text-4xl text-espresso font-extrabold">Khám Phá Hương Vị BrewManager</h2>
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
                    :to="tableNumber ? `/menu/${tableNumber}` : '/menu/5'" 
                    class="inline-flex items-center gap-1 bg-espresso hover:bg-caramel text-cream text-[10px] font-bold px-3 py-1.5 rounded-lg transition-colors"
                  >
                    Gọi món
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
              Trở thành Hội viên BrewManager để được tích điểm tự động 10% mỗi lần mua hàng. Số điểm tích lũy được sử dụng để đổi các voucher giảm giá, miễn phí topping hoặc nhận nước miễn phí vào ngày sinh nhật.
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
                <span class="text-[9px] bg-caramel text-cream px-2 py-0.5 rounded-full font-bold uppercase tracking-wider">Hạng Bạc</span>
              </div>

              <div class="pt-2">
                <div class="text-[10px] text-cream/50 uppercase tracking-widest">Chủ thẻ</div>
                <div class="text-lg font-bold tracking-wide">{{ customerProfile.name }}</div>
                <div class="text-xs font-mono text-cream/70 mt-0.5">{{ customerProfile.phone }}</div>
              </div>

              <div class="grid grid-cols-2 gap-4 pt-4 border-t border-white/10">
                <div>
                  <div class="text-[9px] text-cream/50 uppercase">Điểm tích lũy</div>
                  <div class="text-xl font-bold text-caramel">185 <span class="text-xs text-cream/70 font-normal">điểm</span></div>
                </div>
                <div>
                  <div class="text-[9px] text-cream/50 uppercase">Hạng tiếp theo</div>
                  <div class="text-xs font-semibold">Cần thêm 65 điểm để thăng hạng Vàng</div>
                </div>
              </div>

              <!-- Mini Bar Progress -->
              <div class="w-full bg-white/10 h-1.5 rounded-full overflow-hidden">
                <div class="bg-caramel h-full rounded-full" style="width: 74%" />
              </div>
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

            <!-- Vouchers Panel if logged in -->
            <div v-if="customerProfile" class="w-full max-w-sm mt-6 space-y-3">
              <div class="text-left text-xs font-bold text-espresso uppercase tracking-wider">Voucher của bạn:</div>
              <div class="grid grid-cols-1 gap-2.5">
                <!-- Voucher Card 1 -->
                <div class="bg-card rounded-xl border border-cream-deep/60 p-3 flex justify-between items-center shadow-soft">
                  <div class="text-left space-y-0.5">
                    <span class="text-[8px] bg-sage/20 text-success px-2 py-0.5 rounded-full font-extrabold uppercase">Thành Viên Mới</span>
                    <h5 class="text-xs font-bold text-espresso">Giảm 10.000đ khi gọi món</h5>
                    <p class="text-[10px] text-muted-foreground">Hạn sử dụng: 31/12/2026</p>
                  </div>
                  <button 
                    @click="copyVoucherCode('BREWNEW')"
                    class="bg-caramel-light hover:bg-caramel hover:text-cream text-brown text-[10px] font-bold px-3 py-1.5 rounded-lg transition-colors shrink-0"
                  >
                    Sao chép mã
                  </button>
                </div>
                <!-- Voucher Card 2 -->
                <div class="bg-card rounded-xl border border-cream-deep/60 p-3 flex justify-between items-center shadow-soft">
                  <div class="text-left space-y-0.5">
                    <span class="text-[8px] bg-caramel/10 text-caramel px-2 py-0.5 rounded-full font-extrabold uppercase">Hạng Bạc</span>
                    <h5 class="text-xs font-bold text-espresso">Tặng trân châu đen miễn phí</h5>
                    <p class="text-[10px] text-muted-foreground">Áp dụng đơn từ 45k</p>
                  </div>
                  <button 
                    @click="copyVoucherCode('SILVERFREE')"
                    class="bg-caramel-light hover:bg-caramel hover:text-cream text-brown text-[10px] font-bold px-3 py-1.5 rounded-lg transition-colors shrink-0"
                  >
                    Sao chép mã
                  </button>
                </div>
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
              Đừng bỏ lỡ các ưu đãi giờ vàng và quà tặng kết nối cực hời chỉ có tại BrewManager Cafe.
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
            <span class="text-xs font-bold uppercase tracking-widest text-caramel font-sans">Câu Chuyện BrewManager</span>
            <h2 class="font-display text-3xl sm:text-4xl text-espresso font-extrabold">Cà Phê Tử Tế, Gặp Gỡ Thân Tình</h2>
            <p class="text-muted-foreground text-sm sm:text-base leading-relaxed">
              BrewManager Cafe ra đời từ niềm đam mê với hương vị hạt cà phê mộc đậm đà của Tây Nguyên Việt Nam. Chúng tôi tin rằng mỗi tách cà phê được pha đúng chuẩn, phục vụ trong một không gian mộc mạc yên bình, chính là liều thuốc tinh thần tốt nhất cho một ngày bận rộn.
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
            <span class="font-display text-lg text-cream font-bold">{{ tenQuan || 'BrewManager' }}</span>
          </div>
          <p class="text-xs text-cream/50 leading-relaxed font-semibold">
            {{ moTaQuan || 'Hệ thống giải pháp vận hành tối giản, tinh tế cho các mô hình quán cafe, trà sữa, trà chanh hiện đại.' }}
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
            Email: contact@brewmanager.vn<br />
            Hotline: {{ storeInfoStore.soDienThoai || '1900 6789' }}<br />
            Địa chỉ: {{ storeInfoStore.diaChi || '123 Đường Cà Phê, Quận 1, TP. HCM' }}
          </p>
        </div>
      </div>

      <div class="max-w-6xl mx-auto px-6 mt-12 pt-8 border-t border-white/5 text-center text-[10px] text-cream/40 flex flex-col sm:flex-row justify-between gap-4">
        <span>© 2026 {{ tenQuan || 'BrewManager' }}. Mọi quyền được bảo lưu. Thiết kế pha chế tỉ mỉ.</span>
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
        
        <!-- STEP 1: Nhập SĐT -->
        <div v-if="loginStep === 1">
          <p class="text-xs text-muted-foreground text-center mb-6">Nhập số điện thoại của bạn</p>
          <div class="space-y-4 mb-6">
            <div>
              <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1.5 block text-left">Số điện thoại</label>
              <input 
                type="tel" 
                v-model="phoneNumber" 
                placeholder="Ví dụ: 0912345678"
                maxlength="10"
                @input="onPhoneInput"
                :class="[
                  'w-full h-11 px-4 bg-background border rounded-lg text-sm font-bold tracking-widest focus:outline-none focus:ring-2 transition-all text-espresso',
                  phoneError ? 'border-destructive focus:border-destructive focus:ring-destructive/20' : 'border-cream-deep focus:border-caramel focus:ring-caramel/20'
                ]"
                @keyup.enter="checkPhoneNumber"
              />
              <p v-if="phoneError" class="text-[10px] text-destructive font-bold mt-1.5 text-left animate-in fade-in">{{ phoneError }}</p>
            </div>
          </div>
          <button 
            @click="checkPhoneNumber" 
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
            <p class="text-xs font-mono text-muted-foreground mt-1">{{ phoneNumber }}</p>
          </div>
          <p class="text-sm font-bold text-espresso mb-4">Đây có phải là bạn không?</p>
          <div class="flex gap-3">
            <button @click="confirmIdentity(false)" class="flex-1 py-2.5 rounded-lg border border-cream-deep text-muted-foreground hover:text-espresso hover:bg-background text-xs font-bold transition-all">Không phải</button>
            <button @click="confirmIdentity(true)" class="flex-1 py-2.5 rounded-lg bg-espresso hover:bg-brown text-cream text-xs font-bold transition-all shadow-md">Đúng, là tôi</button>
          </div>
        </div>

        <!-- STEP 3: Nhập tên (Khách mới hoặc chọn 'Không phải') -->
        <div v-else-if="loginStep === 3" class="animate-in fade-in slide-in-from-right-4">
          <p class="text-xs text-muted-foreground text-center mb-6">Chào bạn mới! Hãy cho chúng tôi biết tên</p>
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

    <ChatbotWidget />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { 
  Coffee, ArrowRight, QrCode, LayoutDashboard, X, History, Sparkles, CheckCircle, MapPin, Phone 
} from 'lucide-vue-next';
import heroImg from '@/assets/cafe-hero.jpg';
import menuCoffee from '@/assets/menu-coffee.jpg';
import menuTea from '@/assets/menu-tea.jpg';
import menuFrappe from '@/assets/menu-frappe.jpg';
import menuPastry from '@/assets/menu-pastry.jpg';
import { useStoreInfoStore } from '@/stores/storeInfo';
import { ordersApi, type MenuItem } from '@/services/orders';
import ChatbotWidget from '@/components/ChatbotWidget.vue';

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
const loginStep = ref<1 | 2 | 3>(1)
const foundName = ref('')
const fullName = ref('')
const phoneNumber = ref('')
const nameError = ref('')
const phoneError = ref('')
const customerProfile = ref<{ name: string; phone: string } | null>(null)

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

const checkLoginStatus = () => {
  const saved = localStorage.getItem(STORAGE_KEY)
  if (saved) {
    try {
      customerProfile.value = JSON.parse(saved)
    } catch (e) {
      customerProfile.value = null
    }
  } else {
    customerProfile.value = null
  }
}

onMounted(() => {
  checkLoginStatus()
  fetchRealMenu()
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

const onPhoneInput = (e: Event) => {
  const input = e.target as HTMLInputElement;
  // Chỉ cho phép nhập số
  phoneNumber.value = input.value.replace(/\D/g, '').slice(0, 10);
  phoneError.value = '';
}

const checkPhoneNumber = () => {
  phoneError.value = '';
  const phoneVal = phoneNumber.value;
  if (!phoneVal) {
    phoneError.value = 'Vui lòng nhập số điện thoại!';
    return;
  } else if (!/^0\d{9}$/.test(phoneVal)) {
    phoneError.value = 'Số điện thoại không hợp lệ (gồm 10 số, bắt đầu bằng 0)!';
    return;
  }

  // Quét cơ sở dữ liệu giả lập (localStorage)
  const dbStr = localStorage.getItem('brewCustomersDb');
  const db = dbStr ? JSON.parse(dbStr) : {};
  
  if (db[phoneVal]) {
    foundName.value = db[phoneVal].name;
    loginStep.value = 2; // Xác nhận danh tính
  } else {
    loginStep.value = 3; // Mời nhập tên
  }
}

const confirmIdentity = (isMe: boolean) => {
  if (isMe) {
    const profile = { name: foundName.value, phone: phoneNumber.value };
    localStorage.setItem(STORAGE_KEY, JSON.stringify(profile));
    customerProfile.value = profile;
    showToast('Đăng nhập thành công', `Chào mừng ${profile.name} trở lại!`);
    resetModal();
  } else {
    loginStep.value = 3;
    fullName.value = '';
  }
}

const submitNewName = () => {
  nameError.value = '';
  const nameVal = fullName.value.trim();
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

  // Lưu khách mới vào DB giả lập
  const dbStr = localStorage.getItem('brewCustomersDb');
  const db = dbStr ? JSON.parse(dbStr) : {};
  db[phoneNumber.value] = { name: nameVal, phone: phoneNumber.value };
  localStorage.setItem('brewCustomersDb', JSON.stringify(db));

  const profile = { name: nameVal, phone: phoneNumber.value };
  localStorage.setItem(STORAGE_KEY, JSON.stringify(profile));
  customerProfile.value = profile;
  showToast('Đăng nhập thành công', `Chào mừng ${profile.name} đến với hệ thống tích điểm!`);
  resetModal();
}

const resetModal = () => {
  isPhoneModalOpen.value = false;
  loginStep.value = 1;
  phoneNumber.value = '';
  fullName.value = '';
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
  showToast('Đã sao chép', `Mã voucher ${code} đã được sao chép vào bộ nhớ tạm.`)
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
