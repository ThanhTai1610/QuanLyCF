<template>
  <div class="min-h-screen bg-background text-espresso selection:bg-caramel-light selection:text-brown">
    <!-- Sticky Glassmorphic Header -->
    <header class="sticky top-0 z-40 bg-background/80 backdrop-blur-md border-b border-cream-deep/60 transition-all">
      <div class="max-w-6xl mx-auto px-6 h-20 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-espresso flex items-center justify-center shadow-soft transform hover:rotate-12 transition-transform duration-300">
            <Coffee class="w-5 h-5 text-cream" />
          </div>
          <div class="flex flex-col text-left">
            <span class="font-display text-lg text-espresso font-bold tracking-tight">{{ tenQuan || 'BrewManager Cafe' }}</span>
            <span class="text-[10px] text-caramel font-semibold uppercase tracking-wider">Không gian & Cà phê tử tế</span>
          </div>
        </div>
        
        <nav class="hidden md:flex items-center gap-8">
          <a href="#menu" class="text-sm font-semibold text-espresso/70 hover:text-caramel transition-colors">Thực đơn</a>
          <a href="#loyalty" class="text-sm font-semibold text-espresso/70 hover:text-caramel transition-colors">Thành viên</a>
          <a href="#promotions" class="text-sm font-semibold text-espresso/70 hover:text-caramel transition-colors">Ưu đãi</a>
          <a href="#about" class="text-sm font-semibold text-espresso/70 hover:text-caramel transition-colors">Câu chuyện</a>
        </nav>

        <div class="flex items-center gap-3">
          <!-- Logged in status info -->
          <div v-if="customerProfile" class="flex items-center gap-3 bg-cream-deep/40 px-3 py-1.5 rounded-lg border border-cream-deep/60">
            <div class="w-7 h-7 rounded-full bg-caramel flex items-center justify-center text-cream text-xs font-bold">
              {{ customerProfile.name.charAt(0).toUpperCase() }}
            </div>
            <div class="hidden lg:block text-left">
              <div class="text-[9px] text-muted-foreground font-bold leading-none">Xin chào,</div>
              <div class="text-[11px] font-bold text-espresso leading-tight">{{ customerProfile.name }}</div>
            </div>
            <button @click="handleLogout" class="text-[10px] text-destructive hover:underline ml-1 font-bold">Đăng xuất</button>
          </div>
          <button
            v-else
            @click="isPhoneModalOpen = true"
            class="px-4 py-2 rounded-lg bg-caramel-light border border-caramel/20 text-brown hover:bg-caramel hover:text-cream text-xs font-bold transition-all"
          >
            Đăng nhập tích điểm
          </button>
          
          <router-link
            to="/menu/5"
            class="px-4 py-2 rounded-lg bg-espresso hover:bg-brown text-cream text-xs font-bold transition-all shadow-soft"
          >
            Gọi món tại bàn (Mã QR)
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
              Chào mừng bạn đến với<br />
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
                href="#menu"
                class="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-espresso hover:bg-brown text-cream font-bold transition-all hover:scale-[1.02] shadow-warm animate-bounce-subtle"
              >
                Khám Phá Thực Đơn
                <ArrowRight class="w-4 h-4" />
              </a>
              <a
                href="#loyalty"
                class="inline-flex items-center gap-2 px-6 py-3.5 rounded-xl bg-card hover:bg-caramel-light text-espresso font-bold border border-cream-deep transition-colors shadow-soft"
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
            <div class="absolute -inset-4 bg-caramel/10 rounded-2xl blur-3xl" />
            <!-- Hero Image Frame -->
            <div class="relative rounded-2xl overflow-hidden shadow-warm border-4 border-card transform lg:rotate-1 hover:rotate-0 transition-all duration-500">
              <img
                :src="heroImg"
                alt="BrewManager Coffee Vibe"
                class="w-full h-auto object-cover aspect-[4/3] scale-105 hover:scale-100 transition-all duration-700"
              />
              <div class="absolute inset-0 bg-gradient-to-t from-espresso/45 via-transparent to-transparent" />
            </div>

            <!-- Overlays -->
            <div class="absolute -bottom-6 -left-6 bg-card rounded-xl p-4 border border-cream-deep shadow-warm hidden md:block max-w-[220px]">
              <div class="flex items-center gap-3">
                <div class="w-9 h-9 rounded-lg bg-caramel/10 flex items-center justify-center shrink-0">
                  <Coffee class="w-5 h-5 text-caramel" />
                </div>
                <div class="min-w-0 text-left">
                  <div class="text-[9px] font-bold text-muted-foreground uppercase">Signature Drink</div>
                  <div class="font-bold text-espresso text-xs truncate">Cà phê sữa đá Sài Gòn</div>
                </div>
              </div>
            </div>

            <div class="absolute -top-6 -right-6 bg-card rounded-xl p-4 border border-cream-deep shadow-warm hidden md:block">
              <div class="flex items-center gap-2">
                <div class="w-2.5 h-2.5 rounded-full bg-sage animate-pulse" />
                <div class="text-[11px] font-bold text-espresso">Mở cửa: 7:00 - 22:30</div>
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
              v-for="item in filteredMenuItems" 
              :key="item.name" 
              class="bg-card rounded-2xl overflow-hidden border border-cream-deep/60 hover:-translate-y-1 hover:shadow-warm transition-all duration-300 flex flex-col justify-between group"
            >
              <div class="relative overflow-hidden aspect-[4/3] bg-cream-deep">
                <img 
                  :src="item.image" 
                  :alt="item.name"
                  class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                />
                <span 
                  v-if="item.tag"
                  class="absolute top-3 left-3 bg-caramel text-cream text-[9px] uppercase tracking-wider font-extrabold px-2 py-0.5 rounded-full"
                >
                  {{ item.tag }}
                </span>
              </div>
              <div class="p-5 flex-1 flex flex-col justify-between text-left space-y-4">
                <div class="space-y-1">
                  <h3 class="font-display text-sm text-espresso font-bold group-hover:text-caramel transition-colors">{{ item.name }}</h3>
                  <p class="text-muted-foreground text-[11px] leading-relaxed line-clamp-2">{{ item.desc }}</p>
                </div>
                <div class="flex justify-between items-center pt-2 border-t border-cream-deep/30">
                  <span class="font-sans text-sm font-extrabold text-caramel">{{ item.price }}đ</span>
                  <router-link 
                    to="/menu/5" 
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
              <div class="absolute inset-0 bg-white/40 backdrop-blur-[1px] flex flex-col items-center justify-center p-6 text-center space-y-4">
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

              <!-- Background card elements (blurred) -->
              <div class="flex justify-between items-start opacity-40">
                <span class="font-display font-bold text-xs tracking-widest text-espresso/40">BREWMEMBER</span>
                <span class="text-[8px] bg-espresso/10 text-espresso/50 px-2 py-0.5 rounded-full font-bold uppercase">Thành viên</span>
              </div>
              <div class="pt-2 opacity-40">
                <div class="text-[8px] text-espresso/40 uppercase">Chủ thẻ</div>
                <div class="text-base font-bold">Khách Hàng Thân Thiết</div>
                <div class="text-xs font-mono text-espresso/40">09xx xxx xxx</div>
              </div>
              <div class="grid grid-cols-2 gap-4 pt-4 border-t border-espresso/10 opacity-40">
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
                <h5 class="font-bold text-espresso">7:00 - 22:30</h5>
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
    <footer class="bg-espresso text-cream/70 py-16 border-t border-espresso-soft">
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
            <li><a href="#menu" class="hover:text-caramel transition-colors">Menu Gọi Món</a></li>
            <li><a href="#loyalty" class="hover:text-caramel transition-colors">Thành Viên Tích Điểm</a></li>
            <li><a href="#promotions" class="hover:text-caramel transition-colors">Ưu Đãi</a></li>
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
        <button @click="isPhoneModalOpen = false; phoneNumber = ''; fullName = ''; phoneError = ''" class="absolute top-3 right-3 text-muted-foreground hover:text-espresso transition-colors">
          <X class="w-5 h-5" />
        </button>
        
        <h3 class="text-lg font-display font-bold text-espresso mb-1 text-center">Đăng Nhập Tích Điểm</h3>
        <p class="text-xs text-muted-foreground text-center mb-6">Nhập thông tin để tiếp tục</p>
        
        <div class="space-y-3 mb-4">
          <div>
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1 block text-left">Họ và tên</label>
            <input 
              type="text" 
              v-model="fullName" 
              placeholder="Ví dụ: Nguyễn Văn A"
              class="w-full h-11 px-4 bg-background border border-cream-deep rounded-lg text-sm font-semibold focus:outline-none focus:border-caramel focus:ring-2 focus:ring-caramel/20 text-espresso transition-all"
              @keyup.enter="handlePhoneSubmit"
            />
          </div>
          <div>
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] mb-1 block text-left">Số điện thoại</label>
            <input 
              type="tel" 
              v-model="phoneNumber" 
              placeholder="Ví dụ: 0912345678"
              class="w-full h-11 px-4 bg-background border border-cream-deep rounded-lg text-sm font-bold text-center tracking-widest focus:outline-none focus:border-caramel focus:ring-2 focus:ring-caramel/20 text-espresso transition-all"
              @keyup.enter="handlePhoneSubmit"
            />
          </div>
          <p v-if="phoneError" class="text-[11px] text-destructive font-bold mt-2 animate-in slide-in-from-bottom-1 text-center">
            {{ phoneError }}
          </p>
        </div>
        
        <button 
          @click="handlePhoneSubmit" 
          class="w-full h-11 rounded-lg bg-caramel hover:bg-brown text-cream text-sm font-bold transition-colors uppercase tracking-wider shadow-sm"
        >
          Tiếp tục
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { 
  Coffee, ArrowRight, QrCode, LayoutDashboard, X, History, Sparkles, CheckCircle, MapPin, Phone 
} from 'lucide-vue-next';
import heroImg from '@/assets/cafe-hero.jpg';
import menuCoffee from '@/assets/menu-coffee.jpg';
import menuTea from '@/assets/menu-tea.jpg';
import menuFrappe from '@/assets/menu-frappe.jpg';
import menuPastry from '@/assets/menu-pastry.jpg';
import { useStoreInfoStore } from '@/stores/storeInfo';

const router = useRouter()
const storeInfoStore = useStoreInfoStore()

// Dùng store toàn cục — App.vue đã fetch khi khởi động
const tenQuan  = computed(() => storeInfoStore.tenQuan)
const moTaQuan = computed(() => storeInfoStore.moTaQuan)

// Modal and loyalty state management
const isPhoneModalOpen = ref(false)
const fullName = ref('')
const phoneNumber = ref('')
const phoneError = ref('')
const customerProfile = ref<{ name: string; phone: string } | null>(null)

const STORAGE_KEY = 'brewCustomerProfile'

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
})

const handleLogout = () => {
  localStorage.removeItem(STORAGE_KEY)
  customerProfile.value = null
  alert('Đã đăng xuất tích điểm thành công!')
}

const handlePhoneSubmit = () => {
  if (!fullName.value.trim()) {
    phoneError.value = 'Vui lòng nhập họ và tên!'
    return
  }
  if (phoneNumber.value.trim().length < 9) {
    phoneError.value = 'Vui lòng nhập số điện thoại hợp lệ!'
    return
  }

  const profile = {
    name: fullName.value.trim(),
    phone: phoneNumber.value.trim()
  }
  
  localStorage.setItem(STORAGE_KEY, JSON.stringify(profile))
  customerProfile.value = profile

  alert(`Chào mừng ${profile.name} đã đăng nhập tích điểm thành công!`)
  isPhoneModalOpen.value = false
  fullName.value = ''
  phoneNumber.value = ''
  phoneError.value = ''
}

// Menu Categories and Items configuration
const activeCategory = ref('coffee')

const menuCategories = [
  { id: 'coffee', name: 'Cà phê mộc' },
  { id: 'tea', name: 'Trà trái cây' },
  { id: 'frappe', name: 'Đá xay (Frappe)' },
  { id: 'pastry', name: 'Bánh ngọt nướng' }
]

const menuItems = [
  { name: 'Cà phê Sữa Đá Sài Gòn', category: 'coffee', price: '29.000', desc: 'Sự kết hợp đậm đà giữa hạt Robusta rang mộc thơm lừng và sữa đặc béo ngậy truyền thống.', tag: 'Bán chạy', image: menuCoffee },
  { name: 'Bạc Xỉu Sài Gòn', category: 'coffee', price: '32.000', desc: 'Thức uống thơm ngậy mùi sữa tươi đánh bọt mịn màng với một chút nhấn nhá của hương vị cà phê phin.', tag: '', image: menuCoffee },
  { name: 'Espresso Latte', category: 'coffee', price: '39.000', desc: 'Một shot Espresso nóng nguyên chất kết hợp cùng sữa tươi béo ngậy đánh nóng mịn màng.', tag: 'Yêu thích', image: menuCoffee },
  { name: 'Trà Đào Cam Sả', category: 'tea', price: '45.000', desc: 'Vị hồng trà thanh mát hòa quyện cùng đào miếng giòn ngọt, cam tươi và sả thơm nồng ấm.', tag: 'Bán chạy', image: menuTea },
  { name: 'Trà Vải Lài Gia Nhiệt', category: 'tea', price: '42.000', desc: 'Trà lài hoa tự nhiên thanh lọc kết hợp với những quả vải tươi ngọt lịm mang lại cảm giác mát mẻ.', tag: '', image: menuTea },
  { name: 'Trà Sen Vàng Kem Cheese', category: 'tea', price: '49.000', desc: 'Trà ô long thanh mát kết hợp hạt sen bùi ngậy và lớp kem phô mai sánh mịn mặn mặn béo ngậy.', tag: 'Mới', image: menuTea },
  { name: 'Matcha Đá Xay Kem Ngọt', category: 'frappe', price: '49.000', desc: 'Bột trà xanh Uji cao cấp từ Nhật Bản xay nhuyễn cùng sữa đặc, đá bào và phủ lớp kem sữa.', tag: 'Yêu thích', image: menuFrappe },
  { name: 'Cà Phê Dừa Đá Xay', category: 'frappe', price: '45.000', desc: 'Cà phê nguyên chất đắng thơm phối cùng cốt dừa đông lạnh béo ngậy xay mịn mát lạnh.', tag: 'Bán chạy', image: menuFrappe },
  { name: 'Croissant Bơ Tỏi Nướng', category: 'pastry', price: '25.000', desc: 'Bánh sừng bò ngập lớp bơ thơm ngậy nướng giòn rụm kết hợp xốt bơ tỏi mặn ngọt.', tag: '', image: menuPastry },
  { name: 'Tiramisu Truyền Thống', category: 'pastry', price: '35.000', desc: 'Bánh ngọt tráng miệng của Ý thơm nồng hương rượu rum, cà phê espresso xen kẽ kem trứng béo.', tag: 'Signature', image: menuPastry }
]

const filteredMenuItems = computed(() => {
  return menuItems.filter(item => item.category === activeCategory.value)
})

const copyVoucherCode = (code: string) => {
  navigator.clipboard.writeText(code)
  alert(`Đã sao chép mã voucher thành công: ${code}. Hãy nhập mã này tại màn hình thanh toán gọi món QR.`)
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
</style>
