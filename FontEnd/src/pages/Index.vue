<template>
  <div class="min-h-screen bg-background">
    <header class="max-w-6xl mx-auto px-6 h-20 flex items-center justify-between">
      <div class="flex items-center gap-2">
        <div class="w-10 h-10 rounded-md bg-espresso flex items-center justify-center shadow-soft">
          <Coffee class="w-5 h-5 text-cream" />
        </div>
        <span class="font-display text-xl text-espresso font-semibold">BrewManager</span>
      </div>
      <router-link
        to="/staff-login"
        class="px-4 py-2 rounded-lg bg-caramel hover:bg-brown text-cream text-sm font-semibold transition-colors"
      >
        Đăng nhập
      </router-link>
    </header>

    <main class="max-w-6xl mx-auto px-6 pt-8 pb-20">
      <section class="grid lg:grid-cols-2 gap-10 items-center">
        <div>
          <span class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-caramel-light border border-caramel/20 text-xs font-medium text-brown">
            ☕ Hệ thống dành cho quán cà phê hiện đại
          </span>
          <h1 class="font-display text-5xl md:text-6xl text-espresso mt-5 leading-[1.1] font-semibold">
            Quản lý quán cà phê.<br />
            <span class="text-caramel italic">Đơn giản, ấm áp.</span>
          </h1>
          <p class="text-muted-foreground mt-5 text-lg leading-relaxed max-w-lg">
            BrewManager giúp khách gọi món qua QR, nhân viên xử lý đơn nhanh chóng,
            và chủ quán theo dõi doanh thu mọi lúc — tất cả trong một giao diện được pha chế tỉ mỉ.
          </p>

          <div class="flex flex-wrap gap-3 mt-8">
            <router-link
              to="/menu/5"
              class="inline-flex items-center gap-2 px-5 py-3 rounded-md bg-espresso hover:bg-brown text-cream font-semibold transition-colors shadow-warm"
            >
              <QrCode class="w-4 h-4" /> Thử Menu QR (Bàn 5)
              <ArrowRight class="w-4 h-4" />
            </router-link>
            <router-link
              to="/dashboard"
              class="inline-flex items-center gap-2 px-5 py-3 rounded-md bg-card hover:bg-caramel-light text-espresso font-semibold border border-cream-deep transition-colors"
            >
              <LayoutDashboard class="w-4 h-4" /> Vào Dashboard
            </router-link>
            <router-link
              to="/orders"
              class="inline-flex items-center gap-2 px-5 py-3 rounded-md bg-card hover:bg-caramel-light text-espresso font-semibold border border-cream-deep transition-colors"
            >
              <ShoppingBag class="w-4 h-4" /> Quản lý đơn
            </router-link>
          </div>

          <div class="grid grid-cols-3 gap-4 mt-10 pt-8 border-t border-cream-deep">
            <div>
              <div class="font-display text-3xl text-espresso font-semibold">87</div>
              <div class="text-xs text-muted-foreground mt-0.5">Đơn hôm nay</div>
            </div>
            <div>
              <div class="font-display text-3xl text-caramel font-semibold">4.2tr</div>
              <div class="text-xs text-muted-foreground mt-0.5">Doanh thu / ngày</div>
            </div>
            <div>
              <div class="font-display text-3xl text-sage font-semibold">142</div>
              <div class="text-xs text-muted-foreground mt-0.5">Lượt khách</div>
            </div>
          </div>
        </div>

        <div class="relative">
          <div class="absolute -inset-4 bg-caramel/20 rounded-lg blur-2xl" />
          <img
            :src="heroImg"
            alt="Cà phê và bánh ngọt trên bàn ấm cúng"
            width="1280"
            height="800"
            class="relative rounded-lg shadow-warm w-full h-auto object-cover aspect-[4/3]"
          />
          <div class="absolute -bottom-4 -left-4 bg-card rounded-lg p-4 border border-cream-deep shadow-warm hidden md:block">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-success/15 flex items-center justify-center">
                <Coffee class="w-5 h-5 text-success" />
              </div>
              <div>
                <div class="text-xs text-muted-foreground">Đơn vừa hoàn thành</div>
                <div class="font-semibold text-espresso text-sm">Bàn 5 • Cappuccino ×2</div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { Coffee, ArrowRight, QrCode, LayoutDashboard, ShoppingBag } from 'lucide-vue-next';
import heroImg from '@/assets/cafe-hero.jpg';
</script>
