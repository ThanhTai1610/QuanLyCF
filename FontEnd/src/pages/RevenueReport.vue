<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h2 class="text-2xl font-display font-semibold text-espresso">Báo cáo doanh thu</h2>
        <p class="text-sm text-muted-foreground mt-1">Gom dữ liệu từ hoá đơn để thống kê doanh thu</p>
      </div>
      <button class="px-4 py-2 bg-espresso hover:bg-brown text-cream rounded-lg text-sm font-semibold transition-colors shadow-sm">
        Xuất báo cáo
      </button>
    </div>

    <!-- Filter / Date Picker Placeholder -->
    <div class="bg-white p-4 rounded-2xl border border-cream-deep shadow-sm flex items-center gap-4">
      <div class="flex items-center gap-2">
        <span class="text-sm font-medium text-espresso">Thời gian:</span>
        <select class="border border-cream-deep rounded-lg px-3 py-1.5 text-sm bg-background text-espresso focus:outline-none focus:border-caramel">
          <option>Hôm nay</option>
          <option>Tuần này</option>
          <option>Tháng này</option>
          <option>Tùy chỉnh...</option>
        </select>
      </div>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white p-6 rounded-2xl border border-cream-deep shadow-sm">
        <h3 class="text-sm font-semibold text-muted-foreground">Tổng Doanh Thu</h3>
        <p class="text-2xl font-bold text-success mt-2">0đ</p>
      </div>
      <div class="bg-white p-6 rounded-2xl border border-cream-deep shadow-sm">
        <h3 class="text-sm font-semibold text-muted-foreground">Số Đơn Hàng</h3>
        <p class="text-2xl font-bold text-espresso mt-2">0</p>
      </div>
      <div class="bg-white p-6 rounded-2xl border border-cream-deep shadow-sm">
        <h3 class="text-sm font-semibold text-muted-foreground">Giá trị TB/Đơn</h3>
        <p class="text-2xl font-bold text-espresso mt-2">0đ</p>
      </div>
    </div>

    <!-- Chart Placeholder -->
    <div class="bg-white rounded-2xl border border-cream-deep shadow-sm p-8 flex flex-col items-center justify-center min-h-[300px]">
      <p class="text-muted-foreground">Biểu đồ doanh thu đang được phát triển.</p>
    </div>
  </div>
</template>
