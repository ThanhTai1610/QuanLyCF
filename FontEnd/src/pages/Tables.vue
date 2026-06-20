<template>
  <div class="space-y-5 p-6">
    <!-- Summary cards -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-3">
      <button
        v-for="s in summaryCards"
        :key="s.key"
        @click="statusFilter = statusFilter === s.key ? 'all' : s.key"
        :class="[
          'flex items-center gap-3 p-4 rounded-lg border shadow-card text-left transition',
          statusFilter === s.key ? 'ring-2 ring-espresso/40' : '',
          s.cardClass
        ]"
      >
        <div :class="['w-10 h-10 rounded-lg flex items-center justify-center shrink-0', s.iconWrap]">
          <component :is="s.icon" class="w-5 h-5" />
        </div>
        <div>
          <p class="text-2xl font-display font-semibold leading-none">{{ s.count }}</p>
          <p class="text-xs text-muted-foreground mt-1">{{ s.label }}</p>
        </div>
      </button>
    </div>

    <!-- Toolbar -->
    <div class="flex flex-col lg:flex-row lg:items-center gap-3">
      <!-- Tìm kiếm -->
      <div class="relative w-full lg:max-w-[240px] shrink-0">
        <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
        <Input
          placeholder="Tìm bàn..."
          v-model="search"
          class="pl-9 bg-card border-cream-deep h-10 rounded-lg"
        />
      </div>

      <!-- Lọc theo khu vực (segmented pills, tự xuống dòng) -->
      <div class="flex items-center gap-1 flex-wrap p-1 rounded-xl bg-cream/70 border border-cream-deep flex-1 min-w-0">
        <button
          v-for="z in zoneFilters"
          :key="z.value"
          @click="zoneFilter = z.value"
          :class="[
            'inline-flex items-center gap-1.5 px-3.5 py-1.5 rounded-lg text-sm font-semibold whitespace-nowrap transition',
            zoneFilter === z.value
              ? 'bg-espresso text-cream shadow-card'
              : 'text-espresso/70 hover:text-espresso hover:bg-card'
          ]"
        >
          <component :is="z.value === 'all' ? Grid3x3 : MapPin" class="w-3.5 h-3.5" />
          {{ z.label }}
        </button>
      </div>

      <!-- Hành động -->
      <div class="flex gap-2 shrink-0">
        <template v-if="!selectMode">
          <Button @click="toggleMergeMode" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-espresso">
            <CheckSquare class="w-4 h-4 mr-1.5" /> Chọn
          </Button>
          <Button @click="openZones" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-espresso">
            <LayoutGrid class="w-4 h-4 mr-1.5" /> Khu vực
          </Button>
          <Button @click="openCreateTable" class="bg-caramel text-cream rounded-lg border border-caramel/30 shadow-card">
            <Plus class="w-4 h-4 mr-1.5" /> Thêm bàn
          </Button>
        </template>
        <template v-else>
          <Button @click="openMergeModal" :disabled="selectedIds.length < 2" class="bg-espresso text-cream rounded-lg shadow-card disabled:opacity-50" title="Ghép các bàn đã chọn">
            <Combine class="w-4 h-4 mr-1.5" /> Ghép
          </Button>
          <Button @click="batchPrint" :disabled="selectedIds.length < 1 || batchPrinting" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-espresso disabled:opacity-50" title="In QR các bàn đã chọn">
            <Printer class="w-4 h-4 mr-1.5" /> In
          </Button>
          <Button @click="batchRegenerate" :disabled="selectedIds.length < 1" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-espresso disabled:opacity-50" title="Tạo lại QR các bàn đã chọn">
            <RefreshCw class="w-4 h-4 mr-1.5" /> Tạo lại QR
          </Button>
          <Button @click="toggleMergeMode" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-espresso">
            <X class="w-4 h-4 mr-1.5" /> Huỷ
          </Button>
        </template>
      </div>
    </div>

    <!-- Banner chế độ chọn -->
    <div v-if="selectMode" class="flex flex-wrap items-center gap-2 rounded-xl bg-espresso/5 border border-espresso/20 px-4 py-2.5 text-sm text-espresso">
      <CheckSquare class="w-4 h-4 shrink-0" />
      Bấm vào bàn để chọn, rồi chọn hành động: <b>Ghép</b> (≥2 bàn) · <b>In</b> · <b>Tạo lại QR</b>.
      <span class="ml-auto">Đã chọn: <b>{{ selectedIds.length }}</b></span>
      <button @click="selectedIds = []" v-if="selectedIds.length" class="text-xs underline text-muted-foreground hover:text-espresso">Bỏ chọn</button>
    </div>

    <!-- QR ẩn để render khi in hàng loạt -->
    <div aria-hidden="true" style="position:absolute;left:-9999px;top:0;pointer-events:none">
      <QrcodeVue v-for="t in printQueue" :key="t.maBan" :id="`qr-batch-${t.maBan}`" :value="t.urlDatMon" :size="240" foreground="#2C1A0E" background="#ffffff" level="M" />
    </div>

    <!-- Error / Notice / Loading -->
    <p v-if="errorMsg" class="text-sm font-semibold text-red-600 bg-red-50 border border-red-200 rounded-lg px-4 py-3">
      {{ errorMsg }}
    </p>
    <p v-if="noticeMsg" class="flex items-center justify-between gap-2 text-sm font-semibold text-green-700 bg-green-50 border border-green-200 rounded-lg px-4 py-3">
      {{ noticeMsg }}
      <button @click="noticeMsg = ''" class="text-green-700/70 hover:text-green-700"><X class="w-4 h-4" /></button>
    </p>
    <!-- Hoàn tác đóng bàn (1 bước gần nhất) -->
    <div v-if="lastCleared" class="flex items-center justify-between gap-2 text-sm bg-espresso/5 border border-espresso/20 rounded-lg px-4 py-2.5">
      <span class="text-espresso">Vừa dọn <b>{{ lastCleared.label }}</b>. Nếu nhầm, có thể khôi phục.</span>
      <div class="flex items-center gap-2 shrink-0">
        <Button @click="hoanTacDongBan" variant="outline" class="border border-cream-deep rounded-lg h-8 px-3 text-espresso">
          <RotateCcw class="w-3.5 h-3.5 mr-1" /> Hoàn tác
        </Button>
        <button @click="lastCleared = null" class="text-muted-foreground hover:text-espresso"><X class="w-4 h-4" /></button>
      </div>
    </div>
    <p v-if="loading" class="text-sm text-muted-foreground">Đang tải dữ liệu...</p>

    <div v-if="!loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">

      <!-- CHẾ ĐỘ GHÉP: chọn từng bàn -->
      <template v-if="selectMode">
        <article
          v-for="t in paginatedItems" :key="t.maBan"
          @click="toggleSelect(t)"
          :class="[
            'relative bg-card rounded-xl border-2 shadow-card p-4 cursor-pointer select-none transition',
            selectedIds.includes(t.maBan) ? 'border-espresso ring-2 ring-espresso/15' : 'border-cream-deep hover:border-caramel/50'
          ]"
        >
          <div class="absolute top-2.5 right-2.5 w-6 h-6 rounded-full grid place-items-center border-2"
            :class="selectedIds.includes(t.maBan) ? 'bg-espresso border-espresso text-cream' : 'bg-white border-cream-deep text-transparent'">
            <Check class="w-3.5 h-3.5" />
          </div>
          <h3 class="font-display text-lg text-espresso font-semibold pr-7">{{ t.tenBan }}</h3>
          <p class="text-xs text-muted-foreground mt-0.5">{{ t.tenKhuVuc }}<span v-if="t.sucChua"> • {{ t.sucChua }} chỗ</span></p>
          <div class="mt-2 flex items-center gap-2">
            <span :class="['px-2 py-0.5 rounded text-[11px] font-semibold', statusMeta[t.trangThai].badgeClass]">{{ statusMeta[t.trangThai].label }}</span>
            <span v-if="t.maBanChinh || soBanGhep(t.maBan) > 0" class="inline-flex items-center gap-1 text-[11px] text-caramel">
              <Link2 class="w-3 h-3" /> đang ghép
            </span>
          </div>
        </article>
      </template>

      <!-- CHẾ ĐỘ THƯỜNG: nhóm ghép gộp thành 1 thẻ to -->
      <template v-else>
        <article
          v-for="u in paginatedUnits" :key="u.key"
          :class="[
            'bg-card rounded-lg border shadow-card overflow-hidden border-l-4',
            statusMeta[u.trangThai].borderClass,
            u.isGroup ? 'sm:col-span-2 border-caramel/60 ring-1 ring-caramel/20' : 'border-cream-deep'
          ]"
        >
          <div class="p-5 pb-3">
            <div class="flex justify-between items-start gap-2">
              <div class="min-w-0">
                <div class="flex items-center gap-2 flex-wrap">
                  <h3 class="font-display text-lg text-espresso font-semibold">{{ u.ten }}</h3>
                  <span v-if="u.isGroup" class="inline-flex items-center gap-1 text-[11px] font-bold text-caramel bg-caramel/10 px-2 py-0.5 rounded-full">
                    <Link2 class="w-3 h-3" /> Nhóm {{ u.members.length }} bàn
                  </span>
                </div>
                <p class="text-xs text-muted-foreground mt-0.5">
                  {{ u.tenKhuVuc }}<span v-if="u.tongSucChua"> • {{ u.tongSucChua }} chỗ</span>
                </p>
              </div>
              <div class="flex items-center gap-2 shrink-0">
                <span :class="['px-2.5 py-1 rounded-lg text-[11px] font-semibold whitespace-nowrap', statusMeta[u.trangThai].badgeClass]">
                  {{ statusMeta[u.trangThai].label }}
                </span>
                <button @click="openHistory(u)" class="text-muted-foreground hover:text-espresso" title="Lịch sử bàn">
                  <Clock class="w-4 h-4" />
                </button>
                <button v-if="u.isGroup" @click="unmergeTable(u.primary)" class="text-muted-foreground hover:text-caramel" title="Tách nhóm">
                  <Unlink class="w-4 h-4" />
                </button>
                <template v-else>
                  <button @click="openEditTable(u.primary)" class="text-muted-foreground hover:text-espresso" title="Sửa bàn">
                    <Pencil class="w-4 h-4" />
                  </button>
                  <button @click="removeTable(u.primary)" class="text-muted-foreground hover:text-destructive" title="Xoá bàn">
                    <Trash2 class="w-4 h-4" />
                  </button>
                </template>
              </div>
            </div>

            <!-- Các bàn thành viên (chỉ nhóm) — bấm × để tách riêng từng bàn -->
            <div v-if="u.isGroup" class="mt-3 flex flex-wrap gap-1.5">
              <span v-for="m in u.members" :key="m.maBan"
                class="inline-flex items-center gap-1.5 text-[11px] font-medium bg-cream pl-2 pr-1.5 py-1 rounded-lg text-espresso">
                <span class="w-1.5 h-1.5 rounded-full" :class="statusDotClass[m.trangThai]"></span>{{ m.tenBan }}
                <span v-if="m.maBan === u.primary.maBan" class="text-[9px] uppercase text-caramel font-bold tracking-wide">chính</span>
                <button v-else @click="unmergeTable(m)" class="grid place-items-center w-4 h-4 rounded text-muted-foreground hover:text-red-600 hover:bg-red-50" title="Tách bàn này khỏi nhóm">
                  <X class="w-3 h-3" />
                </button>
              </span>
            </div>

            <!-- Đổi trạng thái (áp cho cả nhóm). "Có khách" tự động khi có đơn, không chỉnh tay. -->
            <div class="mt-4 grid grid-cols-3 gap-2">
              <button
                v-for="st in statusOptions" :key="st.value"
                @click="changeUnitStatus(u, st.value)"
                :disabled="u.trangThai === st.value || st.value === 'CoKhach'"
                :title="st.value === 'CoKhach' ? 'Tự động khi bàn có đơn' : ''"
                :class="[
                  'px-2 py-2 rounded-lg text-[11px] font-semibold border transition',
                  u.trangThai === st.value
                    ? st.activeClass
                    : st.value === 'CoKhach'
                      ? 'bg-cream/40 text-muted-foreground border-cream-deep cursor-not-allowed'
                      : 'bg-card text-espresso border-cream-deep hover:bg-cream'
                ]"
              >{{ st.label }}</button>
            </div>

            <!-- Đơn hàng -->
            <div class="mt-3 space-y-2">
              <div v-for="o in u.orders" :key="o.maDonHang" class="rounded-lg border border-cream-deep bg-cream/40 p-2.5">
                <div class="flex items-center justify-between gap-2">
                  <span class="inline-flex items-center gap-1.5 text-xs font-semibold text-espresso flex-wrap">
                    <Receipt class="w-3.5 h-3.5" /> Đơn #{{ o.maDonHang }}
                    <span v-if="u.isGroup && o.tenBan" class="inline-flex items-center gap-1 text-[10px] font-bold text-caramel bg-caramel/10 px-1.5 py-0.5 rounded">
                      <Armchair class="w-3 h-3" /> {{ o.tenBan }}
                    </span>
                    <span class="text-muted-foreground font-medium">· {{ o.soMon }} món</span>
                  </span>
                  <span class="text-sm font-bold text-espresso">{{ formatVnd(o.thanhTien) }}</span>
                </div>
                <p class="text-[11px] text-muted-foreground mt-0.5 truncate">
                  {{ o.items.map(i => i.soLuong + '× ' + i.tenMon + (i.tenKichCo ? ' (' + i.tenKichCo + ')' : '')).join(', ') }}
                </p>
                <div class="flex gap-1.5 mt-2">
                  <button @click="openMove(o)" class="inline-flex items-center gap-1 text-[11px] font-semibold px-2 py-1 rounded-lg border border-cream-deep bg-card text-espresso hover:bg-cream">
                    <ArrowLeftRight class="w-3 h-3" /> Đổi bàn
                  </button>
                  <button @click="cancelOrder(o)" class="inline-flex items-center gap-1 text-[11px] font-semibold px-2 py-1 rounded-lg border border-cream-deep bg-card text-red-600 hover:bg-red-50">
                    <X class="w-3 h-3" /> Huỷ đơn
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- QR (đại diện = bàn chính) -->
          <div class="flex items-center gap-3 px-5 py-3 border-t border-cream-deep bg-cream/40">
            <button @click="openQR(u.primary)" class="bg-white p-1.5 rounded-lg shadow-inner border border-cream-deep shrink-0" title="Xem mã QR">
              <QrcodeVue :id="`qr-${u.primary.maBan}`" :value="u.primary.urlDatMon" :size="56" foreground="#2C1A0E" background="#ffffff" level="M" />
            </button>
            <div class="flex-1">
              <p v-if="u.isGroup" class="text-[11px] text-muted-foreground mb-1.5">Mã QR đại diện: <b>{{ u.primary.tenBan }}</b></p>
              <div class="grid grid-cols-3 gap-2">
                <Button @click="openPrint(u.primary)" size="sm" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-xs h-9" title="In mã QR"><Printer class="w-3.5 h-3.5" /></Button>
                <Button @click="downloadQR(u.primary)" size="sm" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-xs h-9" title="Tải mã QR"><Download class="w-3.5 h-3.5" /></Button>
                <Button @click="regenerateQR(u.primary)" size="sm" variant="outline" class="border border-cream-deep rounded-lg shadow-card text-xs h-9" title="Tạo lại mã QR"><RefreshCw class="w-3.5 h-3.5" /></Button>
              </div>
            </div>
          </div>
        </article>
      </template>

      <!-- Empty state -->
      <div v-if="soBanHienThi === 0" class="col-span-full py-12 text-center text-muted-foreground">
        Không có bàn nào phù hợp. Bấm "Thêm bàn" để bắt đầu.
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="soBanHienThi > 0" class="flex items-center justify-between py-2 border-t border-cream-deep/50 mt-4">
      <div class="text-sm text-muted-foreground">
        Tổng <span class="font-medium text-espresso">{{ soBanHienThi }}</span> bàn<span v-if="!selectMode && soNhomGhep > 0"> · <span class="font-medium text-espresso">{{ soNhomGhep }}</span> nhóm ghép</span>
      </div>
      <div class="flex items-center gap-2">
        <Button variant="outline" size="icon" @click="currentPage--" :disabled="currentPage === 1"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50">
          <ChevronLeft class="w-4 h-4" />
        </Button>
        <span class="text-sm font-semibold text-espresso px-3">Trang {{ currentPage }} / {{ totalPages }}</span>
        <Button variant="outline" size="icon" @click="currentPage++" :disabled="currentPage === totalPages"
          class="h-9 w-9 rounded-lg border-cream-deep disabled:opacity-50">
          <ChevronRight class="w-4 h-4" />
        </Button>
      </div>
    </div>

    <!-- QR Modal -->
    <Modal v-model="qrModalOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-semibold">Mã QR · {{ activeTable?.tenBan }}</h2>
        <p class="text-sm text-muted-foreground">Quét để gọi món · {{ activeTable?.tenKhuVuc }}</p>
      </template>
      <div v-if="activeTable" class="flex flex-col items-center gap-4">
        <div class="bg-white p-4 rounded-lg shadow-inner border border-cream-deep">
          <QrcodeVue
            :id="`qr-modal-${activeTable.maBan}`"
            :value="activeTable.urlDatMon"
            :size="240"
            foreground="#2C1A0E"
            background="#ffffff"
            level="M"
          />
        </div>
        <code class="text-xs text-muted-foreground break-all text-center">{{ activeTable.urlDatMon }}</code>
      </div>
      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-lg" @click="downloadQR(activeTable!)">
          <Download class="w-4 h-4 mr-1.5" /> Tải xuống
        </Button>
        <Button class="bg-espresso text-cream rounded-lg" @click="openPrint(activeTable!)">
          <Printer class="w-4 h-4 mr-1.5" /> In mã QR
        </Button>
      </template>
    </Modal>

    <!-- Ghép bàn: chọn bàn chính -->
    <Modal v-model="mergeModalOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-caramel/15 grid place-items-center shrink-0">
            <Combine class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">Ghép {{ selectedIds.length }} bàn</h2>
            <p class="text-sm text-muted-foreground">Chọn bàn chính đại diện cho nhóm</p>
          </div>
        </div>
      </template>
      <div class="space-y-2">
        <label
          v-for="t in selectedTables" :key="t.maBan"
          :class="[
            'flex items-center gap-3 rounded-xl border px-3 py-2.5 cursor-pointer transition',
            mergePrimaryId === t.maBan ? 'border-caramel bg-caramel/5' : 'border-cream-deep hover:bg-cream/50'
          ]">
          <input type="radio" :value="t.maBan" v-model="mergePrimaryId" class="w-4 h-4 accent-espresso" />
          <div class="min-w-0">
            <p class="text-sm font-semibold text-espresso">{{ t.tenBan }}</p>
            <p class="text-[11px] text-muted-foreground">{{ t.tenKhuVuc }}<span v-if="t.sucChua"> • {{ t.sucChua }} chỗ</span></p>
          </div>
        </label>
      </div>
      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="mergeModalOpen = false">Huỷ</Button>
        <Button class="bg-espresso text-cream rounded-xl h-11 px-6" :disabled="merging || !mergePrimaryId" @click="confirmMerge">
          {{ merging ? 'Đang ghép...' : 'Ghép bàn' }}
        </Button>
      </template>
    </Modal>

    <!-- In QR (tuỳ chỉnh) Modal -->
    <Modal v-model="printModalOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-caramel/15 grid place-items-center shrink-0">
            <Printer class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">In mã QR · {{ printTable?.tenBan }}</h2>
            <p class="text-sm text-muted-foreground">Tuỳ chỉnh nội dung rồi in</p>
          </div>
        </div>
      </template>

      <div v-if="printTable" class="space-y-5">
        <!-- Xem trước -->
        <div class="flex justify-center">
          <div class="w-[250px] border-2 border-cream-deep rounded-2xl p-5 text-center bg-white">
            <p class="font-display text-xl font-bold text-espresso leading-tight">{{ printSettings.tieuDe }}</p>
            <p class="text-[11px] text-caramel mt-1">{{ printSettings.loiMoi }}</p>
            <span class="inline-block my-3 bg-espresso text-cream font-semibold px-4 py-1 rounded-full text-sm">{{ printTable.tenBan }}</span>
            <div class="bg-white p-2 inline-block rounded-lg border border-cream-deep">
              <QrcodeVue id="qr-print" :value="printTable.urlDatMon" :size="150" foreground="#2C1A0E" background="#ffffff" level="M" />
            </div>
            <div v-if="printSettings.hienWifi" class="mt-3 pt-3 border-t border-dashed border-cream-deep text-left text-[12px] space-y-1">
              <div class="flex justify-between gap-2"><span class="text-caramel">{{ printSettings.nhanWifi }}</span><span class="font-semibold truncate">{{ printSettings.tenWifi }}</span></div>
              <div class="flex justify-between gap-2"><span class="text-caramel">{{ printSettings.nhanMatKhau }}</span><span class="font-semibold font-mono">{{ printSettings.matKhau }}</span></div>
            </div>
            <p v-if="printSettings.ghiChu.trim()" class="mt-3 text-[11px] italic text-espresso/70 whitespace-pre-wrap">{{ printSettings.ghiChu }}</p>
          </div>
        </div>

        <!-- Tuỳ chỉnh -->
        <div class="space-y-3">
          <div class="grid grid-cols-2 gap-3">
            <div class="space-y-1">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Tiêu đề</label>
              <Input v-model="printSettings.tieuDe" maxlength="40" class="bg-cream/40 border-cream-deep rounded-lg h-10" />
            </div>
            <div class="space-y-1">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Lời mời</label>
              <Input v-model="printSettings.loiMoi" maxlength="60" class="bg-cream/40 border-cream-deep rounded-lg h-10" />
            </div>
          </div>

          <label class="flex items-center gap-2 text-sm font-semibold text-espresso cursor-pointer select-none">
            <input type="checkbox" v-model="printSettings.hienWifi" class="w-4 h-4 accent-espresso" />
            Hiển thị thông tin WiFi
          </label>

          <div v-if="printSettings.hienWifi" class="grid grid-cols-2 gap-3">
            <div class="space-y-1">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Nhãn WiFi</label>
              <Input v-model="printSettings.nhanWifi" maxlength="20" class="bg-cream/40 border-cream-deep rounded-lg h-10" />
            </div>
            <div class="space-y-1">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Tên WiFi</label>
              <Input v-model="printSettings.tenWifi" maxlength="40" class="bg-cream/40 border-cream-deep rounded-lg h-10" />
            </div>
            <div class="space-y-1">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Nhãn mật khẩu</label>
              <Input v-model="printSettings.nhanMatKhau" maxlength="20" class="bg-cream/40 border-cream-deep rounded-lg h-10" />
            </div>
            <div class="space-y-1">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Mật khẩu</label>
              <Input v-model="printSettings.matKhau" maxlength="40" class="bg-cream/40 border-cream-deep rounded-lg h-10" />
            </div>
          </div>
          <div class="space-y-1">
            <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Ghi chú (tuỳ chọn)</label>
            <textarea
              v-model="printSettings.ghiChu" maxlength="120" rows="2"
              placeholder="VD: Giờ mở cửa 7:00 - 22:00 · Hotline 0900 000 000"
              class="w-full rounded-lg border border-cream-deep bg-cream/40 px-3 py-2 text-sm text-espresso resize-none focus:outline-none focus:ring-2 focus:ring-caramel/40"></textarea>
          </div>

          <p class="text-[11px] text-muted-foreground">Tuỳ chỉnh được lưu lại cho lần in sau.</p>
        </div>
      </div>

      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="printModalOpen = false">Đóng</Button>
        <Button class="bg-espresso text-cream rounded-xl h-11 px-6" @click="doPrint">
          <Printer class="w-4 h-4 mr-1.5" /> In
        </Button>
      </template>
    </Modal>

    <!-- Confirm chung (xoá / tạo lại QR) -->
    <Modal v-model="confirmOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div :class="['w-11 h-11 rounded-xl grid place-items-center shrink-0', confirmData.danger ? 'bg-red-100' : 'bg-caramel/15']">
            <AlertTriangle :class="['w-5 h-5', confirmData.danger ? 'text-red-600' : 'text-caramel']" />
          </div>
          <h2 class="font-display text-xl text-espresso font-semibold">{{ confirmData.title }}</h2>
        </div>
      </template>
      <p class="text-sm text-espresso/80 leading-relaxed">{{ confirmData.message }}</p>
      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="confirmOpen = false">Huỷ</Button>
        <Button
          :class="['rounded-xl h-11 px-6 text-cream', confirmData.danger ? 'bg-red-600 hover:bg-red-700' : 'bg-espresso']"
          :disabled="confirmBusy" @click="runConfirm">
          {{ confirmBusy ? 'Đang xử lý...' : (confirmData.confirmLabel || 'Xác nhận') }}
        </Button>
      </template>
    </Modal>

    <!-- Confirm đổi trạng thái Modal -->
    <Modal v-model="confirmStatusOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-caramel/15 grid place-items-center shrink-0">
            <RefreshCw class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">Đổi trạng thái bàn</h2>
            <p class="text-sm text-muted-foreground">Vui lòng xác nhận thay đổi</p>
          </div>
        </div>
      </template>
      <div v-if="statusTarget" class="space-y-4">
        <p class="text-sm text-espresso text-center">
          Đổi trạng thái <span class="font-semibold">{{ statusTarget.ten }}</span>
          <span v-if="statusTarget.members.length > 1" class="text-muted-foreground"> ({{ statusTarget.members.length }} bàn)</span> sang:
        </p>
        <div class="flex items-center justify-center gap-3">
          <span :class="['px-3 py-1.5 rounded-lg text-sm font-semibold', statusMeta[statusTarget.tuTrangThai].badgeClass]">
            {{ statusMeta[statusTarget.tuTrangThai].label }}
          </span>
          <ArrowRight class="w-5 h-5 text-muted-foreground" />
          <span :class="['px-3 py-1.5 rounded-lg text-sm font-semibold', statusMeta[statusTarget.status].badgeClass]">
            {{ statusMeta[statusTarget.status].label }}
          </span>
        </div>
      </div>
      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="confirmStatusOpen = false">Huỷ</Button>
        <Button class="bg-espresso text-cream rounded-xl h-11 px-6" :disabled="statusChanging" @click="confirmChangeStatus">
          {{ statusChanging ? 'Đang đổi...' : 'Xác nhận' }}
        </Button>
      </template>
    </Modal>

    <!-- Table form Modal -->
    <Modal v-model="tableModalOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-caramel/15 flex items-center justify-center shrink-0">
            <Armchair class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">{{ editingTable ? 'Sửa bàn' : 'Thêm bàn mới' }}</h2>
            <p class="text-sm text-muted-foreground">
              {{ editingTable ? 'Cập nhật thông tin bàn' : 'Tạo bàn kèm mã QR tự động' }}
            </p>
          </div>
        </div>
      </template>

      <div class="space-y-5">
        <!-- Tên bàn -->
        <div class="space-y-1.5">
          <label class="flex items-center gap-1.5 text-[11px] font-bold uppercase tracking-wide text-muted-foreground">
            <Hash class="w-3.5 h-3.5" /> Tên bàn <span class="text-red-500">*</span>
          </label>
          <Input v-model="tableForm.tenBan" maxlength="20" placeholder="Bàn 1"
            class="bg-cream/40 border-cream-deep rounded-xl h-11" />
        </div>

        <!-- Khu vực -->
        <div class="space-y-1.5">
          <label class="flex items-center gap-1.5 text-[11px] font-bold uppercase tracking-wide text-muted-foreground">
            <MapPin class="w-3.5 h-3.5" /> Khu vực <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <select v-model="tableForm.maKhuVuc"
              class="w-full h-11 rounded-xl border border-cream-deep bg-cream/40 pl-3 pr-9 text-sm font-medium text-espresso appearance-none focus:outline-none focus:ring-2 focus:ring-caramel/40">
              <option :value="0" disabled>-- Chọn khu vực --</option>
              <option v-for="z in zones" :key="z.maKhuVuc" :value="z.maKhuVuc">{{ z.tenKhuVuc }}</option>
            </select>
            <ChevronDown class="w-4 h-4 absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none" />
          </div>
          <p v-if="zones.length === 0" class="text-[11px] text-warning">
            Chưa có khu vực nào. Hãy tạo khu vực ở nút "Khu vực" trước.
          </p>
        </div>

        <!-- Sức chứa -->
        <div class="space-y-2">
          <label class="flex items-center gap-1.5 text-[11px] font-bold uppercase tracking-wide text-muted-foreground">
            <Users class="w-3.5 h-3.5" /> Sức chứa (số ghế) <span class="text-red-500">*</span>
          </label>
          <div class="flex items-center gap-2.5 flex-wrap">
            <!-- Stepper liền khối -->
            <div class="flex items-center rounded-xl border border-cream-deep bg-cream/40 overflow-hidden">
              <button type="button" @click="stepSucChua(-1)" :disabled="!tableForm.sucChua"
                class="h-11 w-11 grid place-items-center text-espresso text-xl leading-none hover:bg-cream disabled:opacity-40 border-r border-cream-deep">−</button>
              <input
                :value="tableForm.sucChua ?? ''" @input="onSucChuaInput"
                type="number" min="1" max="100" placeholder="—"
                class="h-11 w-14 bg-transparent text-center text-sm font-bold text-espresso focus:outline-none [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none" />
              <button type="button" @click="stepSucChua(1)"
                class="h-11 w-11 grid place-items-center text-espresso text-xl leading-none hover:bg-cream border-l border-cream-deep">+</button>
            </div>
            <!-- Mức nhanh -->
            <div class="flex gap-1.5">
              <button
                v-for="p in sucChuaPresets" :key="p" type="button"
                @click="tableForm.sucChua = p"
                :class="[
                  'h-11 w-11 rounded-xl text-sm font-bold border transition',
                  tableForm.sucChua === p
                    ? 'bg-espresso text-cream border-espresso shadow-card'
                    : 'bg-card text-espresso border-cream-deep hover:bg-cream'
                ]"
              >{{ p }}</button>
            </div>
          </div>
        </div>

        <!-- Lỗi -->
        <p v-if="formError" class="flex items-center gap-2 text-[12px] font-semibold text-red-600 bg-red-50 border border-red-200 rounded-lg px-3 py-2">
          <AlertCircle class="w-4 h-4 shrink-0" /> {{ formError }}
        </p>
      </div>

      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="tableModalOpen = false">Huỷ</Button>
        <Button class="bg-espresso text-cream rounded-xl h-11 px-6" :disabled="saving" @click="saveTable">
          {{ saving ? 'Đang lưu...' : (editingTable ? 'Lưu thay đổi' : 'Tạo bàn') }}
        </Button>
      </template>
    </Modal>

    <!-- Đóng bàn (dọn dẹp) Modal -->
    <Modal v-model="closeModalOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-emerald-100 grid place-items-center shrink-0"><Sparkles class="w-5 h-5 text-emerald-600" /></div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">Dọn bàn · {{ closeUnit?.ten }}</h2>
            <p class="text-sm text-muted-foreground">Xác nhận trước khi trả bàn về Trống</p>
          </div>
        </div>
      </template>
      <div class="space-y-3">
        <p v-if="closeUnit && closeUnit.orders.length" class="text-sm text-espresso/80 bg-cream/50 border border-cream-deep rounded-lg px-3 py-2">
          Bàn đang có <b>{{ closeUnit.orders.length }} đơn</b> — đóng bàn sẽ <b>hoàn tất</b> các đơn này (vào lịch sử).
        </p>
        <label class="flex items-center gap-2.5 rounded-xl border px-3 py-2.5 cursor-pointer transition" :class="chkKhachRoi ? 'bg-emerald-50 border-emerald-300' : 'border-cream-deep'">
          <input type="checkbox" v-model="chkKhachRoi" class="w-4 h-4 accent-emerald-600" />
          <span class="text-sm font-semibold text-espresso">Khách đã rời bàn</span>
        </label>
        <label class="flex items-center gap-2.5 rounded-xl border px-3 py-2.5 cursor-pointer transition" :class="chkDaDon ? 'bg-emerald-50 border-emerald-300' : 'border-cream-deep'">
          <input type="checkbox" v-model="chkDaDon" class="w-4 h-4 accent-emerald-600" />
          <span class="text-sm font-semibold text-espresso">Đã dọn dẹp bàn</span>
        </label>
      </div>
      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="closeModalOpen = false">Huỷ</Button>
        <Button class="bg-emerald-600 text-white rounded-xl h-11 px-6 disabled:opacity-50" :disabled="!chkKhachRoi || !chkDaDon || closing" @click="confirmClose">
          {{ closing ? 'Đang xử lý...' : 'Trả bàn về Trống' }}
        </Button>
      </template>
    </Modal>

    <!-- Lịch sử bàn Modal -->
    <Modal v-model="historyModalOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-caramel/15 grid place-items-center shrink-0"><Clock class="w-5 h-5 text-caramel" /></div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">Lịch sử · {{ historyUnit?.ten }}</h2>
            <p class="text-sm text-muted-foreground">Các đơn đã đặt tại bàn</p>
          </div>
        </div>
      </template>
      <div class="space-y-4">
        <p v-if="historyLoading" class="text-sm text-muted-foreground">Đang tải...</p>
        <template v-else>
          <div v-for="grp in historyData" :key="grp.tenBan" class="space-y-2">
            <p v-if="historyData.length > 1" class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">{{ grp.tenBan }}</p>
            <div v-if="grp.orders.length === 0" class="text-sm text-muted-foreground">Chưa có đơn nào.</div>
            <div v-for="o in grp.orders" :key="o.maDonHang" class="rounded-xl border border-cream-deep p-3">
              <div class="flex items-center justify-between gap-2">
                <span class="text-xs font-semibold text-espresso">Đơn #{{ o.maDonHang }} · {{ o.soMon }} món</span>
                <span class="text-sm font-bold text-espresso">{{ formatVnd(o.thanhTien) }}</span>
              </div>
              <div class="flex items-center justify-between mt-1">
                <span :class="['text-[10px] font-semibold px-1.5 py-0.5 rounded', donBadge(o.trangThaiDon)]">{{ trangThaiDonLabel[o.trangThaiDon] || o.trangThaiDon }}</span>
                <div class="flex items-center gap-2">
                  <button v-if="o.trangThaiDon==='HoanThanh' || o.trangThaiDon==='Huy'" @click="restoreOrder(o)"
                    class="inline-flex items-center gap-1 text-[10px] font-bold text-caramel hover:text-espresso">
                    <RotateCcw class="w-3 h-3" /> Khôi phục
                  </button>
                  <span class="text-[11px] text-muted-foreground">{{ fmtTime(o.thoiGianTao) }}</span>
                </div>
              </div>
              <p class="text-[11px] text-muted-foreground mt-1 truncate">{{ o.items.map(i => i.soLuong + '× ' + i.tenMon).join(', ') }}</p>
            </div>
          </div>
        </template>
      </div>
      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="historyModalOpen = false">Đóng</Button>
      </template>
    </Modal>

    <!-- Đổi bàn Modal -->
    <Modal v-model="moveModalOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-caramel/15 grid place-items-center shrink-0"><ArrowLeftRight class="w-5 h-5 text-caramel" /></div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">Đổi bàn</h2>
            <p class="text-sm text-muted-foreground">Đơn #{{ moveOrder?.maDonHang }} · đang ở {{ moveOrder?.tenBan }}</p>
          </div>
        </div>
      </template>
      <div class="space-y-2">
        <p class="text-[11px] text-muted-foreground">Chọn bàn đích. Bàn đã có khách → sẽ <b>ghép bàn</b> (giữ đơn riêng).</p>
        <div class="space-y-1.5 max-h-72 overflow-y-auto pr-1">
          <label v-for="t in moveTargets" :key="t.maBan"
            :class="['flex items-center gap-3 rounded-xl border px-3 py-2.5 cursor-pointer transition', moveTargetId === t.maBan ? 'border-caramel bg-caramel/5' : 'border-cream-deep hover:bg-cream/50']">
            <input type="radio" :value="t.maBan" v-model="moveTargetId" class="w-4 h-4 accent-espresso" />
            <div class="min-w-0 flex-1">
              <p class="text-sm font-semibold text-espresso">{{ t.tenBan }}</p>
              <p class="text-[11px] text-muted-foreground">{{ t.tenKhuVuc }}</p>
            </div>
            <span v-if="t.coDon" class="text-[11px] font-semibold text-caramel bg-caramel/10 px-2 py-0.5 rounded-full">Có khách → ghép</span>
            <span v-else class="text-[11px] font-semibold text-success">Trống</span>
          </label>
        </div>
      </div>
      <template #footer>
        <Button variant="outline" class="border border-cream-deep rounded-xl h-11 px-5" @click="moveModalOpen = false">Huỷ</Button>
        <Button class="bg-espresso text-cream rounded-xl h-11 px-6" :disabled="!moveTargetId || moving" @click="confirmMove">
          {{ moving ? 'Đang đổi...' : 'Đổi bàn' }}
        </Button>
      </template>
    </Modal>

    <!-- Zones management Modal -->
    <Modal v-model="zonesModalOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div class="w-11 h-11 rounded-xl bg-caramel/15 grid place-items-center shrink-0">
            <LayoutGrid class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h2 class="font-display text-xl text-espresso font-semibold">Quản lý khu vực</h2>
            <p class="text-sm text-muted-foreground">Nhóm bàn theo khu (Tầng 1, Sân vườn, VIP...)</p>
          </div>
        </div>
      </template>

      <div class="space-y-5">
        <!-- Form thêm/sửa khu vực -->
        <div class="rounded-2xl border border-cream-deep bg-cream/40 p-4 space-y-3">
          <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">
            {{ editingZone ? 'Sửa khu vực' : 'Thêm khu vực mới' }}
          </p>
          <div class="flex flex-col sm:flex-row gap-3">
            <div class="flex-1 space-y-1">
              <label class="flex items-center gap-1.5 text-[11px] font-semibold text-espresso">
                <MapPin class="w-3.5 h-3.5" /> Tên khu vực
              </label>
              <Input v-model="zoneForm.tenKhuVuc" maxlength="100" placeholder="Sân vườn"
                class="bg-card border-cream-deep rounded-xl h-11" @keyup.enter="saveZone" />
            </div>
            <div class="w-full sm:w-32 space-y-1">
              <label class="flex items-center gap-1.5 text-[11px] font-semibold text-espresso">
                <Coins class="w-3.5 h-3.5" /> Phụ thu (đ)
              </label>
              <Input v-model.number="zoneForm.phuThu" type="number" min="0"
                class="bg-card border-cream-deep rounded-xl h-11" @keyup.enter="saveZone" />
            </div>
          </div>
          <div class="flex gap-2">
            <Button class="bg-caramel text-cream rounded-xl h-10 px-5 flex-1 sm:flex-none" :disabled="saving" @click="saveZone">
              <component :is="editingZone ? Pencil : Plus" class="w-4 h-4 mr-1.5" />
              {{ editingZone ? 'Cập nhật' : 'Thêm khu vực' }}
            </Button>
            <Button v-if="editingZone" variant="outline" class="border border-cream-deep rounded-xl h-10 px-5" @click="resetZoneForm">
              Huỷ
            </Button>
          </div>
          <p v-if="zoneError" class="flex items-center gap-2 text-[12px] font-semibold text-red-600">
            <AlertCircle class="w-4 h-4 shrink-0" /> {{ zoneError }}
          </p>
        </div>

        <!-- Danh sách khu vực -->
        <div class="space-y-2">
          <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">
            Danh sách khu vực ({{ zones.length }})
          </p>
          <div class="space-y-2 max-h-60 overflow-y-auto pr-1">
            <div v-for="z in zones" :key="z.maKhuVuc"
              :class="[
                'flex items-center gap-3 rounded-xl border px-3 py-2.5 transition',
                editingZone?.maKhuVuc === z.maKhuVuc ? 'border-caramel bg-caramel/5' : 'border-cream-deep bg-card hover:bg-cream/50'
              ]">
              <div class="w-9 h-9 rounded-lg bg-cream grid place-items-center shrink-0">
                <MapPin class="w-4 h-4 text-espresso" />
              </div>
              <div class="min-w-0 flex-1">
                <p class="text-sm font-semibold text-espresso truncate">{{ z.tenKhuVuc }}</p>
                <div class="flex items-center gap-1.5 mt-0.5">
                  <span class="inline-flex items-center gap-1 text-[11px] text-muted-foreground bg-cream px-1.5 py-0.5 rounded">
                    <Grid3x3 class="w-3 h-3" /> {{ z.soBan }} bàn
                  </span>
                  <span v-if="z.phuThu" class="text-[11px] text-caramel bg-caramel/10 px-1.5 py-0.5 rounded font-semibold">
                    +{{ formatVnd(z.phuThu) }}
                  </span>
                </div>
              </div>
              <div class="flex gap-1 shrink-0">
                <button @click="editZone(z)" class="w-8 h-8 grid place-items-center rounded-lg text-muted-foreground hover:text-espresso hover:bg-cream" title="Sửa">
                  <Pencil class="w-4 h-4" />
                </button>
                <button @click="removeZone(z)" class="w-8 h-8 grid place-items-center rounded-lg text-muted-foreground hover:text-red-600 hover:bg-red-50" title="Xoá">
                  <Trash2 class="w-4 h-4" />
                </button>
              </div>
            </div>
            <div v-if="zones.length === 0" class="rounded-xl border border-dashed border-cream-deep py-8 text-center text-sm text-muted-foreground">
              Chưa có khu vực nào. Thêm khu vực đầu tiên ở trên.
            </div>
          </div>
        </div>
      </div>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from 'vue'
import QrcodeVue from 'qrcode.vue'
import {
  Printer, Download, Plus, Trash2, Pencil, Search, RefreshCw, LayoutGrid,
  ChevronLeft, ChevronRight, Coffee, Wrench, CheckCircle2, Grid3x3, MapPin,
  Armchair, Hash, Users, ChevronDown, AlertCircle, ArrowRight, AlertTriangle, Coins,
  Combine, Link2, Unlink, Check, X, CheckSquare,
  Receipt, ArrowLeftRight, Clock, RotateCcw, Sparkles
} from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import { tablesApi, type TableItem, type Zone, type TableStatus } from '@/services/tables'
import { ordersApi, type OrderDto } from '@/services/orders'

// ── State ─────────────────────────────────────────────────────
const tables = ref<TableItem[]>([])
const zones = ref<Zone[]>([])
const orders = ref<OrderDto[]>([])
const loading = ref(false)
const errorMsg = ref('')

const search = ref('')
const zoneFilter = ref<number | 'all'>('all')
const statusFilter = ref<TableStatus | 'all'>('all')
const currentPage = ref(1)
const itemsPerPage = ref(8)

// Chế độ ghép bàn (khai báo sớm vì các computed bên dưới tham chiếu)
const selectMode = ref(false)
const selectedIds = ref<number[]>([])

// ── Meta trạng thái ───────────────────────────────────────────
const statusMeta: Record<TableStatus, { label: string; badgeClass: string; borderClass: string }> = {
  Trong:   { label: 'Trống',        badgeClass: 'bg-success/15 text-success border border-success/30', borderClass: 'border-l-success' },
  CoKhach: { label: 'Có khách',     badgeClass: 'bg-caramel/15 text-caramel border border-caramel/30', borderClass: 'border-l-caramel' },
  BaoTri:  { label: 'Bảo trì',      badgeClass: 'bg-warning/15 text-warning border border-warning/30', borderClass: 'border-l-warning' },
}
const statusDotClass: Record<TableStatus, string> = {
  Trong: 'bg-success', CoKhach: 'bg-caramel', BaoTri: 'bg-warning',
}
const statusOptions: { value: TableStatus; label: string; activeClass: string }[] = [
  { value: 'Trong',   label: 'Trống',    activeClass: 'bg-success text-white border-success' },
  { value: 'CoKhach', label: 'Có khách', activeClass: 'bg-caramel text-cream border-caramel' },
  { value: 'BaoTri',  label: 'Bảo trì',  activeClass: 'bg-warning text-white border-warning' },
]

// ── Load dữ liệu ──────────────────────────────────────────────
async function loadAll() {
  loading.value = true
  errorMsg.value = ''
  try {
    const [t, z, o] = await Promise.all([tablesApi.list(), tablesApi.listZones(), ordersApi.active()])
    tables.value = t
    zones.value = z
    orders.value = o
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không tải được dữ liệu bàn.'
  } finally {
    loading.value = false
  }
}
onMounted(loadAll)

// ── Filter & paginate ─────────────────────────────────────────
const zoneFilters = computed(() => [
  { value: 'all' as const, label: 'Tất cả khu vực' },
  ...zones.value.map(z => ({ value: z.maKhuVuc, label: z.tenKhuVuc })),
])

const summaryCards = computed(() => [
  { key: 'all' as const,     label: 'Tổng số bàn', icon: Grid3x3,      count: tables.value.length,
    cardClass: 'bg-card border-cream-deep text-espresso', iconWrap: 'bg-cream text-espresso' },
  { key: 'CoKhach' as const, label: 'Có khách',    icon: Coffee,       count: tables.value.filter(t => t.trangThai === 'CoKhach').length,
    cardClass: 'bg-card border-cream-deep text-caramel', iconWrap: 'bg-caramel/15 text-caramel' },
  { key: 'BaoTri' as const,  label: 'Bảo trì',     icon: Wrench,       count: tables.value.filter(t => t.trangThai === 'BaoTri').length,
    cardClass: 'bg-card border-cream-deep text-warning', iconWrap: 'bg-warning/15 text-warning' },
  { key: 'Trong' as const,   label: 'Bàn trống',   icon: CheckCircle2, count: tables.value.filter(t => t.trangThai === 'Trong').length,
    cardClass: 'bg-card border-cream-deep text-success', iconWrap: 'bg-success/15 text-success' },
])

const filteredItems = computed(() =>
  tables.value.filter(t => {
    const matchZone = zoneFilter.value === 'all' || t.maKhuVuc === zoneFilter.value
    const matchStatus = statusFilter.value === 'all' || t.trangThai === statusFilter.value
    const matchSearch = t.tenBan.toLowerCase().includes(search.value.toLowerCase())
    return matchZone && matchStatus && matchSearch
  })
)
const paginatedItems = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  return filteredItems.value.slice(start, start + itemsPerPage.value)
})

// ── Gộp nhóm bàn ghép thành 1 "đơn vị" hiển thị ──────────────
interface TableUnit {
  key: string
  primary: TableItem
  members: TableItem[]     // 1 phần tử = bàn đơn; >1 = nhóm ghép
  isGroup: boolean
  ten: string              // "Bàn 1" hoặc "Bàn 1 + Bàn 2"
  tenKhuVuc: string
  tongSucChua: number
  trangThai: TableStatus
  orders: OrderDto[]       // đơn đang hoạt động của các bàn trong đơn vị
}
const ordersByTable = computed(() => {
  const m = new Map<number, OrderDto[]>()
  for (const o of orders.value) {
    if (o.maBan == null) continue
    const arr = m.get(o.maBan) ?? []
    arr.push(o)
    m.set(o.maBan, arr)
  }
  return m
})
const allUnits = computed<TableUnit[]>(() => {
  const byPrimary = new Map<number, TableItem[]>()
  for (const t of tables.value) if (t.maBanChinh === null) byPrimary.set(t.maBan, [t])
  for (const t of tables.value) {
    if (t.maBanChinh !== null) {
      const arr = byPrimary.get(t.maBanChinh)
      if (arr) arr.push(t)
      else byPrimary.set(t.maBan, [t])   // phòng trường hợp thiếu bàn chính
    }
  }
  const units: TableUnit[] = []
  for (const members of byPrimary.values()) {
    const primary = members[0]
    if (!primary) continue
    const zones = new Set(members.map(m => m.tenKhuVuc))
    units.push({
      key: 'u' + primary.maBan,
      primary,
      members,
      isGroup: members.length > 1,
      ten: members.map(m => m.tenBan).join(' + '),
      tenKhuVuc: zones.size === 1 ? primary.tenKhuVuc : 'Nhiều khu vực',
      tongSucChua: members.reduce((s, m) => s + (m.sucChua ?? 0), 0),
      trangThai: primary.trangThai,
      orders: members.flatMap(m => ordersByTable.value.get(m.maBan) ?? []),
    })
  }
  return units.sort((a, b) => a.primary.tenBan.localeCompare(b.primary.tenBan, 'vi', { numeric: true }))
})
const filteredUnits = computed(() => allUnits.value.filter(u => {
  const matchZone = zoneFilter.value === 'all' || u.members.some(m => m.maKhuVuc === zoneFilter.value)
  const matchStatus = statusFilter.value === 'all' || u.members.some(m => m.trangThai === statusFilter.value)
  const matchSearch = u.members.some(m => m.tenBan.toLowerCase().includes(search.value.toLowerCase()))
  return matchZone && matchStatus && matchSearch
}))
// Phân trang theo "ô": bàn đơn = 1 ô, nhóm ghép = 2 ô (vì thẻ chiếm 2 cột)
function soO(u: TableUnit) { return u.isGroup ? 2 : 1 }
const unitPages = computed<TableUnit[][]>(() => {
  const pages: TableUnit[][] = []
  let cur: TableUnit[] = []
  let used = 0
  for (const u of filteredUnits.value) {
    const o = soO(u)
    if (used + o > itemsPerPage.value && cur.length) { pages.push(cur); cur = []; used = 0 }
    cur.push(u); used += o
  }
  if (cur.length) pages.push(cur)
  return pages
})
const paginatedUnits = computed(() => unitPages.value[currentPage.value - 1] ?? [])

const totalPages = computed(() =>
  selectMode.value
    ? (Math.ceil(filteredItems.value.length / itemsPerPage.value) || 1)
    : (unitPages.value.length || 1)
)
// Tổng số BÀN đang hiển thị (nhóm ghép tính theo số bàn thành viên)
const soBanHienThi = computed(() =>
  selectMode.value
    ? filteredItems.value.length
    : filteredUnits.value.reduce((s, u) => s + u.members.length, 0)
)
const soNhomGhep = computed(() => filteredUnits.value.filter(u => u.isGroup).length)

watch([search, zoneFilter, statusFilter, selectMode], () => { currentPage.value = 1 })
watch(totalPages, () => { if (currentPage.value > totalPages.value) currentPage.value = totalPages.value })

const formatVnd = (n: number) => n.toLocaleString('vi-VN') + 'đ'

// ── Ghép / Tách bàn ───────────────────────────────────────────
const mergeModalOpen = ref(false)
const mergePrimaryId = ref<number | null>(null)
const merging = ref(false)

function soBanGhep(maBan: number) {
  return tables.value.filter(t => t.maBanChinh === maBan).length
}
function laBanChinh(t: TableItem) {
  return t.maBanChinh === null && soBanGhep(t.maBan) > 0
}
function toggleMergeMode() {
  selectMode.value = !selectMode.value
  selectedIds.value = []
}
function toggleSelect(t: TableItem) {
  if (!selectMode.value) return
  const i = selectedIds.value.indexOf(t.maBan)
  if (i >= 0) selectedIds.value.splice(i, 1)
  else selectedIds.value.push(t.maBan)
}
const selectedTables = computed(() => tables.value.filter(t => selectedIds.value.includes(t.maBan)))

function openMergeModal() {
  if (selectedIds.value.length < 2) return
  mergePrimaryId.value = selectedIds.value[0] ?? null
  mergeModalOpen.value = true
}
async function confirmMerge() {
  if (!mergePrimaryId.value) return
  merging.value = true
  try {
    await tablesApi.merge(mergePrimaryId.value, selectedIds.value)
    mergeModalOpen.value = false
    selectMode.value = false
    selectedIds.value = []
    await loadAll()
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không ghép được bàn.'
  } finally {
    merging.value = false
  }
}
function unmergeTable(t: TableItem) {
  askConfirm({
    title: 'Tách bàn',
    message: laBanChinh(t)
      ? `Giải tán nhóm ghép (bàn chính "${t.tenBan}")? Các bàn sẽ trở lại độc lập.`
      : `Tách "${t.tenBan}" ra khỏi nhóm ghép?`,
    confirmLabel: 'Tách',
    onConfirm: async () => {
      try { await tablesApi.unmerge(t.maBan); await loadAll() }
      catch (e) { errorMsg.value = e instanceof Error ? e.message : 'Không tách được bàn.' }
    },
  })
}

// ── Hộp thoại xác nhận chung ──────────────────────────────────
interface ConfirmOptions {
  title: string
  message: string
  confirmLabel?: string
  danger?: boolean
  onConfirm: () => void | Promise<void>
}
const confirmOpen = ref(false)
const confirmBusy = ref(false)
const confirmData = ref<ConfirmOptions>({ title: '', message: '', onConfirm: () => {} })

function askConfirm(opts: ConfirmOptions) {
  confirmData.value = opts
  confirmOpen.value = true
}
async function runConfirm() {
  confirmBusy.value = true
  try {
    await confirmData.value.onConfirm()
    confirmOpen.value = false
  } finally {
    confirmBusy.value = false
  }
}

// ── Đổi trạng thái (có xác nhận, áp cho cả nhóm) ──────────────
const confirmStatusOpen = ref(false)
const statusTarget = ref<{ members: TableItem[]; ten: string; tuTrangThai: TableStatus; status: TableStatus } | null>(null)
const statusChanging = ref(false)

function changeUnitStatus(u: TableUnit, st: TableStatus) {
  if (u.trangThai === st) return
  if (st === 'Trong') { openCloseModal(u); return }   // đổi sang Trống = đóng bàn (có xác nhận dọn dẹp)
  if (st === 'BaoTri' && u.orders.length > 0) {
    errorMsg.value = `${u.ten} đang có đơn — hãy bấm "Đổi bàn" chuyển đơn sang bàn khác trước khi chuyển sang Bảo trì.`
    return
  }
  statusTarget.value = { members: u.members, ten: u.ten, tuTrangThai: u.trangThai, status: st }
  confirmStatusOpen.value = true
}

async function confirmChangeStatus() {
  if (!statusTarget.value) return
  const { members, status } = statusTarget.value
  statusChanging.value = true
  try {
    for (const m of members) {
      if (m.trangThai !== status) {
        await tablesApi.updateStatus(m.maBan, status)
        m.trangThai = status
      }
    }
    confirmStatusOpen.value = false
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không đổi được trạng thái.'
  } finally {
    statusChanging.value = false
  }
}

// ── Đóng bàn (xác nhận dọn dẹp) + Hoàn tác ────────────────────
const closeModalOpen = ref(false)
const closeUnit = ref<TableUnit | null>(null)
const chkKhachRoi = ref(false)
const chkDaDon = ref(false)
const closing = ref(false)
const lastCleared = ref<{ maBans: number[]; label: string } | null>(null)

function openCloseModal(u: TableUnit) {
  closeUnit.value = u
  chkKhachRoi.value = false
  chkDaDon.value = false
  closeModalOpen.value = true
}
async function confirmClose() {
  if (!closeUnit.value || !chkKhachRoi.value || !chkDaDon.value) return
  const u = closeUnit.value
  closing.value = true
  try {
    for (const m of u.members) await ordersApi.closeTable(m.maBan)
    lastCleared.value = { maBans: u.members.map(m => m.maBan), label: u.ten }
    closeModalOpen.value = false
    noticeMsg.value = `Đã dọn ${u.ten}. Bàn trở về Trống.`
    await loadAll()
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không đóng được bàn.'
  } finally {
    closing.value = false
  }
}
async function hoanTacDongBan() {
  if (!lastCleared.value) return
  try {
    for (const id of lastCleared.value.maBans) await ordersApi.reopenTable(id)
    noticeMsg.value = `Đã khôi phục ${lastCleared.value.label}.`
    lastCleared.value = null
    await loadAll()
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không khôi phục được.'
  }
}

// ── Lịch sử bàn ───────────────────────────────────────────────
const historyModalOpen = ref(false)
const historyUnit = ref<TableUnit | null>(null)
const historyLoading = ref(false)
const historyData = ref<{ tenBan: string; orders: OrderDto[] }[]>([])
const trangThaiDonLabel: Record<string, string> = {
  ChoXacNhan: 'Chờ xác nhận', DangPha: 'Đang pha', HoanThanh: 'Hoàn thành', Huy: 'Đã huỷ',
}
function donBadge(s: string) {
  return s === 'HoanThanh' ? 'bg-emerald-100 text-emerald-700'
    : s === 'Huy' ? 'bg-red-100 text-red-600'
    : 'bg-caramel/15 text-caramel'
}
function fmtTime(iso: string) {
  return new Date(iso).toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' })
}
async function openHistory(u: TableUnit) {
  historyUnit.value = u
  historyData.value = []
  historyModalOpen.value = true
  historyLoading.value = true
  try {
    historyData.value = await Promise.all(
      u.members.map(async m => ({ tenBan: m.tenBan, orders: await ordersApi.history(m.maBan) })))
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không tải được lịch sử.'
  } finally {
    historyLoading.value = false
  }
}
async function restoreOrder(o: OrderDto) {
  try {
    await ordersApi.restore(o.maDonHang)
    historyModalOpen.value = false
    noticeMsg.value = `Đã khôi phục đơn #${o.maDonHang}${o.tenBan ? ' (' + o.tenBan + ')' : ''} về hoạt động.`
    await loadAll()
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không khôi phục được đơn.'
  }
}

// ── CRUD bàn ──────────────────────────────────────────────────
const tableModalOpen = ref(false)
const editingTable = ref<TableItem | null>(null)
const tableForm = ref<{ tenBan: string; maKhuVuc: number; sucChua: number | null }>({ tenBan: '', maKhuVuc: 0, sucChua: null })
const formError = ref('')
const saving = ref(false)

/** Goi y ten ban trong ke tiep: "Ban 1", neu da co thi "Ban 2"... (lay so nho nhat chua dung). */
function goiYTenBan(): string {
  const daDung = new Set<number>()
  for (const t of tables.value) {
    const m = /^Bàn\s+(\d+)$/.exec(t.tenBan.trim())
    if (m) daDung.add(Number(m[1]))
  }
  let n = 1
  while (daDung.has(n)) n++
  return `Bàn ${n}`
}

const sucChuaPresets = [2, 4, 6, 8, 10]
function stepSucChua(delta: number) {
  const next = (tableForm.value.sucChua ?? 0) + delta
  tableForm.value.sucChua = next < 1 ? null : Math.min(next, 100)
}
function onSucChuaInput(e: Event) {
  const v = (e.target as HTMLInputElement).value
  tableForm.value.sucChua = v === '' ? null : Number(v)
}

/** Validate 3 truong; tra ve thong bao loi dau tien, hoac null neu hop le. */
function validateTable(): string | null {
  const ten = tableForm.value.tenBan.trim()
  if (!ten) return 'Vui lòng nhập tên bàn.'
  if (ten.length > 20) return 'Tên bàn tối đa 20 ký tự.'
  const trungTen = tables.value.some(t =>
    t.tenBan.trim().toLowerCase() === ten.toLowerCase() && t.maBan !== editingTable.value?.maBan)
  if (trungTen) return 'Tên bàn đã tồn tại.'

  if (!tableForm.value.maKhuVuc) return 'Vui lòng chọn khu vực.'

  const sc = tableForm.value.sucChua
  if (sc === null) return 'Vui lòng nhập sức chứa (số ghế).'
  if (!Number.isInteger(sc) || sc < 1) return 'Sức chứa phải là số nguyên ≥ 1.'
  if (sc > 100) return 'Sức chứa tối đa 100 chỗ.'
  return null
}

function openCreateTable() {
  editingTable.value = null
  tableForm.value = { tenBan: goiYTenBan(), maKhuVuc: zones.value[0]?.maKhuVuc ?? 0, sucChua: 4 }
  formError.value = ''
  tableModalOpen.value = true
}
function openEditTable(t: TableItem) {
  editingTable.value = t
  tableForm.value = { tenBan: t.tenBan, maKhuVuc: t.maKhuVuc, sucChua: t.sucChua }
  formError.value = ''
  tableModalOpen.value = true
}
async function saveTable() {
  const loi = validateTable()
  if (loi) { formError.value = loi; return }
  formError.value = ''
  saving.value = true
  try {
    const body = {
      tenBan: tableForm.value.tenBan.trim(),
      maKhuVuc: Number(tableForm.value.maKhuVuc),
      sucChua: tableForm.value.sucChua ? Number(tableForm.value.sucChua) : null,
    }
    if (editingTable.value) await tablesApi.update(editingTable.value.maBan, body)
    else await tablesApi.create(body)
    tableModalOpen.value = false
    await loadAll()
  } catch (e) {
    formError.value = e instanceof Error ? e.message : 'Không lưu được bàn.'
  } finally {
    saving.value = false
  }
}
function removeTable(t: TableItem) {
  askConfirm({
    title: 'Xoá bàn',
    message: `Bạn chắc chắn muốn xoá "${t.tenBan}"? Hành động này không thể hoàn tác.`,
    confirmLabel: 'Xoá bàn',
    danger: true,
    onConfirm: async () => {
      try {
        await tablesApi.remove(t.maBan)
        await loadAll()
      } catch (e) {
        errorMsg.value = e instanceof Error ? e.message : 'Không xoá được bàn.'
      }
    },
  })
}

// ── QR ────────────────────────────────────────────────────────
const qrModalOpen = ref(false)
const activeTable = ref<TableItem | null>(null)
function openQR(t: TableItem) { activeTable.value = t; qrModalOpen.value = true }

function getCanvas(t: TableItem): HTMLCanvasElement | null {
  return document.querySelector<HTMLCanvasElement>(`#qr-modal-${t.maBan} canvas, #qr-modal-${t.maBan}`)
    || document.querySelector<HTMLCanvasElement>(`#qr-${t.maBan} canvas, #qr-${t.maBan}`)
}
function downloadQR(t: TableItem) {
  const canvas = getCanvas(t)
  if (!canvas) return
  const link = document.createElement('a')
  link.download = `QR-${t.tenBan}.png`
  link.href = canvas.toDataURL('image/png')
  link.click()
}
// ── In QR (có modal tuỳ chỉnh) ────────────────────────────────
interface PrintSettings {
  tieuDe: string
  loiMoi: string
  hienWifi: boolean
  nhanWifi: string
  tenWifi: string
  nhanMatKhau: string
  matKhau: string
  ghiChu: string
}
const PRINT_KEY = 'tablePrintSettings'
const printDefault: PrintSettings = {
  tieuDe: 'BrewManager',
  loiMoi: 'Quét mã để gọi món',
  hienWifi: true,
  nhanWifi: 'WiFi',
  tenWifi: 'BrewManager_Cafe',
  nhanMatKhau: 'Mật khẩu',
  matKhau: '12345678',
  ghiChu: '',
}
function loadPrintSettings(): PrintSettings {
  try { return { ...printDefault, ...JSON.parse(localStorage.getItem(PRINT_KEY) || '{}') } }
  catch { return { ...printDefault } }
}
const printSettings = ref<PrintSettings>(loadPrintSettings())
const printModalOpen = ref(false)
const printTable = ref<TableItem | null>(null)
const printQueue = ref<TableItem[]>([])   // QR ẩn để render khi in hàng loạt
const batchPrinting = ref(false)

function openPrint(t: TableItem) {
  printTable.value = t
  printModalOpen.value = true
}

function escapeHtml(s: string) {
  return s.replace(/[&<>"]/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;' }[c]!))
}

const PRINT_CSS = `
  @page { size: 80mm 115mm; margin: 0; }
  * { box-sizing: border-box; }
  html, body { margin: 0; padding: 0; }
  body { font-family: 'Segoe UI', Roboto, sans-serif; color: #2C1A0E; }
  .page { width: 80mm; padding: 4mm; page-break-after: always; }
  .page:last-child { page-break-after: auto; }
  .card { width: 100%; border: 0.4mm solid #E7DECF; border-radius: 4mm; padding: 5mm 4mm; text-align: center; }
  .title { font-family: Georgia, 'Times New Roman', serif; font-size: 15pt; font-weight: 700; margin: 0; letter-spacing: .3px; }
  .prompt { color: #9A6B43; font-size: 8pt; margin: 1.2mm 0 0; }
  .table { display: inline-block; margin: 3mm 0 2mm; background: #2C1A0E; color: #fff; font-weight: 700; padding: 1.4mm 5mm; border-radius: 99mm; font-size: 10pt; }
  .qr { display: block; margin: 1mm auto; width: 46mm; height: 46mm; }
  .wifi { margin-top: 3.5mm; border-top: 0.3mm dashed #D9CDB8; padding-top: 2.5mm; text-align: left; }
  .row { display: flex; justify-content: space-between; gap: 3mm; font-size: 8.5pt; padding: 0.6mm 0; }
  .lbl { color: #9A6B43; }
  .val { font-weight: 700; font-family: 'Courier New', monospace; }
  .note { margin: 3mm 0 0; font-size: 8pt; font-style: italic; color: #6B5847; text-align: center; white-space: pre-wrap; }
`

/** Dựng HTML 1 thẻ in cho 1 bàn (tên + ảnh QR dataURL). */
function cardHtml(tenBan: string, qrDataUrl: string): string {
  const s = printSettings.value
  const wifiBlock = s.hienWifi ? `
    <div class="wifi">
      <div class="row"><span class="lbl">${escapeHtml(s.nhanWifi)}</span><span class="val">${escapeHtml(s.tenWifi)}</span></div>
      <div class="row"><span class="lbl">${escapeHtml(s.nhanMatKhau)}</span><span class="val">${escapeHtml(s.matKhau)}</span></div>
    </div>` : ''
  const ghiChuBlock = s.ghiChu.trim() ? `<p class="note">${escapeHtml(s.ghiChu)}</p>` : ''
  return `<div class="page"><div class="card">
    <p class="title">${escapeHtml(s.tieuDe)}</p>
    <p class="prompt">${escapeHtml(s.loiMoi)}</p>
    <div class="table">${escapeHtml(tenBan)}</div>
    <img class="qr" src="${qrDataUrl}" />
    ${wifiBlock}${ghiChuBlock}
  </div></div>`
}

function openPrintWindow(title: string, bodyHtml: string) {
  const w = window.open('', '_blank')
  if (!w) return
  w.document.write(`<html><head><title>${escapeHtml(title)}</title><style>${PRINT_CSS}</style></head><body>${bodyHtml}</body></html>`)
  w.document.close()
  setTimeout(() => w.print(), 300)
}

function doPrint() {
  const t = printTable.value
  if (!t) return
  const canvas = document.querySelector<HTMLCanvasElement>('#qr-print canvas, #qr-print')
  if (!canvas) return
  localStorage.setItem(PRINT_KEY, JSON.stringify(printSettings.value))
  openPrintWindow(`QR ${t.tenBan}`, cardHtml(t.tenBan, canvas.toDataURL()))
}

/** In hàng loạt các bàn đang chọn (mỗi bàn 1 thẻ, mỗi thẻ 1 trang). */
async function batchPrint() {
  const list = [...selectedTables.value]
  if (list.length === 0) return
  batchPrinting.value = true
  try {
    printQueue.value = list
    await nextTick()
    await new Promise<void>(r => setTimeout(r, 150))   // chờ canvas QR vẽ xong
    localStorage.setItem(PRINT_KEY, JSON.stringify(printSettings.value))
    const body = list.map(t => {
      const c = document.querySelector<HTMLCanvasElement>(`#qr-batch-${t.maBan} canvas, #qr-batch-${t.maBan}`)
      return c ? cardHtml(t.tenBan, c.toDataURL()) : ''
    }).join('')
    openPrintWindow(`QR ${list.length} bàn`, body)
  } finally {
    printQueue.value = []
    batchPrinting.value = false
  }
}

/** Tạo lại QR hàng loạt cho các bàn đang chọn. */
function batchRegenerate() {
  const list = [...selectedTables.value]
  if (list.length === 0) return
  askConfirm({
    title: 'Tạo lại mã QR hàng loạt',
    message: `Tạo lại mã QR cho ${list.length} bàn đã chọn? Mã QR cũ của các bàn này sẽ ngừng hoạt động.`,
    confirmLabel: 'Tạo lại',
    onConfirm: async () => {
      try {
        for (const t of list) await tablesApi.regenerateQr(t.maBan)
        selectedIds.value = []
        selectMode.value = false
        await loadAll()
      } catch (e) {
        errorMsg.value = e instanceof Error ? e.message : 'Không tạo lại được mã QR.'
      }
    },
  })
}
function regenerateQR(t: TableItem) {
  askConfirm({
    title: 'Tạo lại mã QR',
    message: `Mã QR hiện tại của "${t.tenBan}" sẽ ngừng hoạt động và không dùng được nữa. Bạn có chắc muốn tạo mã mới?`,
    confirmLabel: 'Tạo lại mã',
    onConfirm: async () => {
      try {
        const res = await tablesApi.regenerateQr(t.maBan)
        t.maQRHash = res.maQRHash
        t.urlDatMon = res.urlDatMon
      } catch (e) {
        errorMsg.value = e instanceof Error ? e.message : 'Không tạo lại được mã QR.'
      }
    },
  })
}

// ── Đơn hàng: đổi bàn / huỷ ───────────────────────────────────
const moveModalOpen = ref(false)
const moveOrder = ref<OrderDto | null>(null)
const moveTargetId = ref<number | null>(null)
const moving = ref(false)
const noticeMsg = ref('')

const moveTargets = computed(() => {
  const cur = moveOrder.value?.maBan
  return tables.value
    .filter(t => t.maBan !== cur)
    .map(t => ({ ...t, coDon: (ordersByTable.value.get(t.maBan)?.length ?? 0) > 0 }))
})
function openMove(o: OrderDto) {
  moveOrder.value = o
  moveTargetId.value = null
  moveModalOpen.value = true
}
async function confirmMove() {
  if (!moveOrder.value || !moveTargetId.value) return
  moving.value = true
  try {
    const res = await ordersApi.move(moveOrder.value.maDonHang, moveTargetId.value)
    moveModalOpen.value = false
    noticeMsg.value = res.ketQua === 'merged'
      ? `Đã ghép ${res.tenBanCu} với ${res.tenBanMoi} (giữ đơn riêng).`
      : `Đã chuyển đơn sang ${res.tenBanMoi}.`
    await loadAll()
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không đổi được bàn.'
  } finally {
    moving.value = false
  }
}
function cancelOrder(o: OrderDto) {
  askConfirm({
    title: 'Huỷ đơn',
    message: `Huỷ đơn #${o.maDonHang} (${o.tenBan})? Bàn sẽ được giải phóng nếu không còn đơn nào.`,
    confirmLabel: 'Huỷ đơn',
    danger: true,
    onConfirm: async () => {
      try { await ordersApi.cancel(o.maDonHang); await loadAll() }
      catch (e) { errorMsg.value = e instanceof Error ? e.message : 'Không huỷ được đơn.' }
    },
  })
}

// ── Quản lý khu vực ───────────────────────────────────────────
const zonesModalOpen = ref(false)
const editingZone = ref<Zone | null>(null)
const zoneForm = ref<{ tenKhuVuc: string; phuThu: number }>({ tenKhuVuc: '', phuThu: 0 })
const zoneError = ref('')

function openZones() { resetZoneForm(); zonesModalOpen.value = true }
function resetZoneForm() { editingZone.value = null; zoneForm.value = { tenKhuVuc: '', phuThu: 0 }; zoneError.value = '' }
function editZone(z: Zone) { editingZone.value = z; zoneForm.value = { tenKhuVuc: z.tenKhuVuc, phuThu: z.phuThu }; zoneError.value = '' }

async function saveZone() {
  zoneError.value = ''
  if (!zoneForm.value.tenKhuVuc.trim()) { zoneError.value = 'Vui lòng nhập tên khu vực.'; return }
  saving.value = true
  try {
    const body = { tenKhuVuc: zoneForm.value.tenKhuVuc.trim(), phuThu: zoneForm.value.phuThu || 0 }
    if (editingZone.value) await tablesApi.updateZone(editingZone.value.maKhuVuc, body)
    else await tablesApi.createZone(body)
    resetZoneForm()
    await loadAll()
  } catch (e) {
    zoneError.value = e instanceof Error ? e.message : 'Không lưu được khu vực.'
  } finally {
    saving.value = false
  }
}
function removeZone(z: Zone) {
  askConfirm({
    title: 'Xoá khu vực',
    message: `Bạn chắc chắn muốn xoá khu vực "${z.tenKhuVuc}"?`,
    confirmLabel: 'Xoá khu vực',
    danger: true,
    onConfirm: async () => {
      try {
        await tablesApi.deleteZone(z.maKhuVuc)
        await loadAll()
      } catch (e) {
        zoneError.value = e instanceof Error ? e.message : 'Không xoá được khu vực.'
      }
    },
  })
}
</script>
