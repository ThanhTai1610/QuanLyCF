<template>
  <div class="min-h-screen text-[#FDFBF7] font-premium-sans bg-[#0F0A07] flex flex-col">

    <!-- ===== HEADER ===== -->
    <header class="h-16 px-6 flex items-center justify-between border-b-2 border-white/10 bg-[#1A1512] shadow-card shrink-0">
      <div class="flex items-center gap-3">
        <div class="w-10 h-10 rounded-lg bg-[#CC8033] flex items-center justify-center shadow-card">
          <Coffee class="w-5 h-5 text-[#0F0A07]" stroke-width="1.5" />
        </div>
        <div>
          <h1 class="font-premium-serif text-xl font-bold tracking-wide">{{ storeInfoStore.tenQuan }} <span class="text-[#CC8033] ml-1">·</span> KDS</h1>
          <p class="text-[9px] uppercase tracking-[0.3em] text-[#8A8178] font-bold">Màn hình điều phối bếp realtime</p>
        </div>
      </div>

      <!-- Stats + Clock -->
      <div class="flex items-center gap-8">
        <div class="flex gap-6">
          <div class="text-center">
            <div class="text-[#CC8033] font-premium-sans text-xl font-bold">{{ inProgress }}</div>
            <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">Đang làm</div>
          </div>
          <div class="text-center">
            <div class="text-emerald-400 font-premium-sans text-xl font-bold">{{ readyCount }}</div>
            <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">Sẵn sàng</div>
          </div>
          <div class="text-center">
            <div class="text-[#8A8178] font-premium-sans text-xl font-bold">{{ completedOrders.length }}</div>
            <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">Đã xong</div>
          </div>
        </div>
        <div class="h-8 w-px bg-white/10"></div>
        <div class="flex items-center gap-4">
          <div class="font-premium-sans text-3xl font-medium tabular-nums tracking-tight text-[#CC8033]">{{ timeString }}</div>
        </div>
      </div>
    </header>

    <!-- ===== TAB BAR ===== -->
    <div class="shrink-0 flex items-center justify-between px-6 pt-4 pb-0">
      <div class="flex items-center gap-1">
        <button
          v-for="tab in tabs"
          :key="tab.id"
          @click="activeTab = tab.id"
          :class="[
            'flex items-center gap-2 px-5 py-2.5 rounded-t-lg text-xs font-bold uppercase tracking-[0.15em] transition-all duration-200 border-b-2',
            activeTab === tab.id
              ? 'bg-[#1A1512] text-[#CC8033] border-[#CC8033]'
              : 'text-[#8A8178] border-transparent hover:text-white hover:bg-white/5'
          ]"
        >
          <component :is="tab.icon" class="w-3.5 h-3.5" stroke-width="2" />
          {{ tab.label }}
          <span
            v-if="tab.count !== undefined"
            :class="[
              'px-2 py-0.5 rounded-full text-[9px] font-black',
              activeTab === tab.id ? 'bg-[#CC8033]/20 text-[#CC8033]' : 'bg-white/10 text-[#8A8178]'
            ]"
          >{{ tab.count }}</span>
        </button>
      </div>

      <div v-if="activeTab === 'active'" class="flex items-center gap-3 mb-2">
        <!-- Bộ lọc Loại Đơn Hàng -->
        <div class="flex items-center bg-black/40 rounded-lg border border-white/10 p-1">
          <button @click="orderTypeFilter = 'all'" :class="orderTypeFilter === 'all' ? 'bg-[#CC8033] text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">Tất cả đơn</button>
          <button @click="orderTypeFilter = 'takeaway'" :class="orderTypeFilter === 'takeaway' ? 'bg-[#CC8033] text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">Mang Về</button>
          <button @click="orderTypeFilter = 'dinein'" :class="orderTypeFilter === 'dinein' ? 'bg-[#CC8033] text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">Tại Quán</button>
        </div>
        <!-- Chế độ Xem -->
        <div class="flex items-center bg-black/40 rounded-lg border border-white/10 p-1">
          <button @click="viewMode = 'table'" :class="viewMode === 'table' ? 'bg-white/15 text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="flex items-center gap-1.5 px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">
            <LayoutGrid class="w-3.5 h-3.5" /> Bàn
          </button>
          <button @click="viewMode = 'item'" :class="viewMode === 'item' ? 'bg-white/15 text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="flex items-center gap-1.5 px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">
            <List class="w-3.5 h-3.5" /> Gom Món
          </button>
        </div>
      </div>
    </div>

    <!-- ===== DIVIDER ===== -->
    <div class="h-px bg-white/10 mx-6"></div>

    <!-- ===== PANEL: ĐANG LÀM ===== -->
    <main v-if="activeTab === 'active'" class="p-6 flex-1 overflow-y-auto">
      
      <!-- Chế độ Xem: GOM THEO MÓN -->
      <div v-if="viewMode === 'item'" class="space-y-4 max-w-4xl mx-auto">
        <div v-if="aggregatedItems.length === 0" class="text-center py-20 text-[#8A8178] text-sm">
          Không có món nào đang chờ ở trạm này.
        </div>
        <div v-for="group in aggregatedItems" :key="group.name" class="bg-[#1A1512] rounded-xl border border-white/10 p-5 flex items-center justify-between shadow-card hover:border-white/20 transition-all">
          <div class="flex items-center gap-5">
            <div class="w-14 h-14 rounded-xl bg-[#CC8033]/15 border border-[#CC8033]/30 flex items-center justify-center text-[#CC8033] font-premium-sans text-2xl font-bold shadow-inner">
              {{ group.qty }}
            </div>
            <div>
              <h3 class="text-lg font-bold text-white tracking-wide">{{ group.name }}</h3>
              <p class="text-[11px] text-[#8A8178] mt-1.5 font-medium flex items-center gap-2">
                <span class="text-[#CC8033] font-bold">Bàn: {{ group.tables.join(', ') }}</span>
                <span v-if="group.done > 0" class="text-emerald-400">· Đã xong {{ group.done }}/{{ group.qty }}</span>
              </p>
            </div>
          </div>
          <div class="flex items-center gap-3">
             <div class="text-right">
               <div class="text-[10px] uppercase tracking-wider font-bold text-[#8A8178]">Tiến độ</div>
               <div class="text-base font-bold text-white">{{ Math.round(group.done / group.qty * 100) }}%</div>
             </div>
          </div>
        </div>
      </div>

      <!-- Chế độ Xem: THEO BÀN (Grid) -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5">
        <article
          v-for="o in paginatedActive"
          :key="o.id"
          class="rounded-lg border bg-[#1A1512] shadow-card flex flex-col relative overflow-hidden transition-all duration-300"
          :class="o.isPriority ? 'border-purple-500 shadow-[0_0_20px_rgba(168,85,247,0.4)] animate-[pulse_2s_ease-in-out_infinite]' : (isAllDone(o) ? 'border-emerald-500/50 shadow-[0_0_20px_rgba(52,211,153,0.08)]' : ((now - o.createdTs) / 60000 >= 20 ? 'border-red-500/50 shadow-[0_0_20px_rgba(239,68,68,0.2)]' : 'border-white/10'))"
        >
          <!-- Card Header -->
          <div class="p-4 border-b-2 border-white/10 flex justify-between items-start bg-black/30">
            <div>
              <div class="font-premium-sans text-3xl font-bold tracking-tight">{{ o.table }}</div>
              <div class="flex items-center gap-2 mt-1.5">
                <span v-if="o.table.toLowerCase().includes('mang về') || o.table.toLowerCase().includes('takeaway')" class="px-2 py-0.5 rounded-md bg-red-500/20 text-red-400 border border-red-500/30 text-[9px] font-bold uppercase tracking-widest shadow-sm">Mang đi</span>
                <span v-else class="px-2 py-0.5 rounded-md bg-blue-500/20 text-blue-400 border border-blue-500/30 text-[9px] font-bold uppercase tracking-widest shadow-sm">Tại quán</span>
                <span class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">#{{ o.id }}</span>
                <span v-if="o.isPriority" class="px-2 py-0.5 rounded-md bg-purple-500/20 text-purple-400 border border-purple-500/30 text-[9px] font-bold uppercase tracking-widest shadow-sm flex items-center gap-0.5"><Zap class="w-3 h-3" />Khẩn</span>
              </div>
            </div>
            <!-- Timer badge -->
            <div :class="['flex items-center gap-1.5 px-2.5 py-1 rounded-lg border text-xs font-bold tabular-nums', colorByMin(o.createdTs)]">
              <span class="w-1.5 h-1.5 rounded-full bg-current animate-pulse"></span>
              {{ fmtElapsed(o.createdTs) }}
            </div>
          </div>

          <!-- Items List -->
          <div class="flex-1 p-4 space-y-3">
            <div v-for="(it, i) in o.items" :key="i"
              :class="['rounded-lg transition-all duration-200', it.outOfStock ? 'bg-red-500/5 border border-red-500/20 p-2 -mx-1' : '']">
              <button @click="toggle(o.id, i)" :disabled="it.outOfStock" class="w-full flex items-start gap-3 text-left group disabled:cursor-not-allowed">
                <!-- Checkbox -->
                <div :class="['mt-0.5 w-5 h-5 rounded-md border-2 flex items-center justify-center shrink-0 transition-all duration-200',
                  it.outOfStock ? 'border-red-500/40 bg-red-500/10' : it.done ? 'bg-[#CC8033] border-[#CC8033]' : 'border-white/20 group-hover:border-white/40']"
                >
                  <Check v-if="it.done && !it.outOfStock" class="w-3 h-3 text-white" stroke-width="3" />
                  <X v-if="it.outOfStock" class="w-3 h-3 text-red-400" stroke-width="3" />
                </div>
                <div class="flex-1">
                  <span :class="['text-sm font-bold tracking-tight block transition-colors duration-200', it.outOfStock ? 'text-red-400/70 line-through' : it.done ? 'line-through text-white/30' : 'text-white']">
                    {{ it.name }}
                    <span :class="it.done || it.outOfStock ? 'text-white/20' : 'text-[#CC8033]'" class="ml-1">× {{ it.qty }}</span>
                  </span>
                  <div v-if="it.note" class="mt-1.5 text-[9px] uppercase tracking-wider font-bold text-[#CC8033] bg-[#CC8033]/10 border border-[#CC8033]/20 px-2 py-0.5 rounded inline-block">
                    {{ it.note }}
                  </div>
                </div>
              </button>

              <!-- Nhóm nút chức năng -->
              <div class="flex items-center gap-2 mt-2 pl-8">
                <!-- Báo hết nguyên liệu -->
                <button
                  @click.stop="reportOutOfStock(o.id, i)"
                  :class="['flex items-center gap-1 px-2 py-1 rounded-md border text-[10px] font-bold uppercase tracking-wide transition-colors',
                    it.outOfStock ? 'bg-red-500/15 border-red-500/40 text-red-400' : 'bg-white/5 border-white/10 text-[#8A8178] hover:text-red-400 hover:border-red-500/30']"
                  :title="it.outOfStock ? 'Bỏ báo hết nguyên liệu' : 'Báo hết nguyên liệu'"
                >
                  <AlertTriangle class="w-3 h-3" stroke-width="2.5" />
                  {{ it.outOfStock ? 'Hết NL' : 'Báo hết' }}
                </button>

                <!-- In Tem Dán Ly -->
                <button
                  v-if="!it.outOfStock"
                  @click.stop="openPrintPreview(o, it)"
                  class="flex items-center gap-1 px-2 py-1 rounded-md border bg-white/5 border-white/10 text-[#8A8178] hover:text-[#CC8033] hover:border-[#CC8033]/30 transition-colors text-[10px] font-bold uppercase tracking-wide"
                  title="In tem dán ly sản phẩm"
                >
                  <Printer class="w-3.5 h-3.5" stroke-width="2.5" />
                  In Tem
                </button>
              </div>
            </div>
          </div>

          <!-- Progress bar -->
          <div class="px-4 pb-2">
            <div class="h-1 bg-white/10 rounded-full overflow-hidden">
              <div
                class="h-full bg-[#CC8033] rounded-full transition-all duration-500"
                :style="{ width: (o.items.filter(i => i.done).length / o.items.length * 100) + '%' }"
              ></div>
            </div>
            <div class="flex justify-between mt-1">
              <span class="text-[9px] text-[#8A8178] font-bold">{{ o.items.filter(i => i.done).length }}/{{ o.items.length }} món</span>
              <span class="text-[9px] text-[#8A8178] font-bold">{{ Math.round(o.items.filter(i => i.done).length / o.items.length * 100) }}%</span>
            </div>
          </div>

          <!-- Action Button -->
          <div class="p-4 pt-2">
            <button
              v-if="o.status !== 'ready'"
              @click="markReady(o)"
              :disabled="!isAllDone(o)"
              class="w-full h-10 rounded-lg font-bold text-[10px] uppercase tracking-[0.2em] border transition-all duration-300"
              :class="isAllDone(o)
                ? 'bg-[#CC8033] text-white border-[#CC8033] hover:bg-[#B8722D] shadow-[0_8px_20px_rgba(204,128,51,0.3)]'
                : 'bg-white/5 text-white/20 border-white/10 cursor-not-allowed'"
            >
              <span class="flex items-center justify-center gap-2">
                <CheckCircle2 v-if="isAllDone(o)" class="w-3.5 h-3.5" stroke-width="2.5" />
                Báo Pha Xong
              </span>
            </button>
            <button
              v-else
              @click="complete(o)"
              class="w-full h-10 rounded-lg font-bold text-[10px] uppercase tracking-[0.2em] border border-emerald-500 bg-emerald-500/20 text-emerald-400 hover:bg-emerald-500/40 shadow-[0_0_15px_rgba(52,211,153,0.3)] transition-all animate-pulse"
            >
              <span class="flex items-center justify-center gap-2">
                <Bell class="w-3.5 h-3.5" />
                Đang gọi phục vụ... (Giao Đồ)
              </span>
            </button>
          </div>
        </article>
      </div>

      <!-- Pagination -->
      <div v-if="viewMode === 'table' && filteredActiveOrders.length > itemsPerPage" class="flex items-center justify-between mt-6 p-4 bg-[#1A1512] rounded-lg border border-white/10">
        <div class="text-[10px] uppercase tracking-widest font-bold text-[#8A8178]">
          Trang <span class="text-[#CC8033]">{{ currentPage }}</span> / <span class="text-[#CC8033]">{{ totalPages }}</span>
          &nbsp;·&nbsp; <span class="text-[#CC8033]">{{ filteredActiveOrders.length }}</span> yêu cầu
        </div>
        <div class="flex items-center gap-3">
          <button @click="currentPage--" :disabled="currentPage === 1"
            class="w-9 h-9 rounded-lg border border-white/10 flex items-center justify-center bg-black/20 text-[#8A8178] disabled:opacity-30 hover:bg-black/40">
            <ChevronLeft class="w-4 h-4" />
          </button>
          <button @click="currentPage++" :disabled="currentPage === totalPages"
            class="w-9 h-9 rounded-lg border border-white/10 flex items-center justify-center bg-black/20 text-[#8A8178] disabled:opacity-30 hover:bg-black/40">
            <ChevronRight class="w-4 h-4" />
          </button>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="filteredActiveOrders.length === 0" class="text-center py-32">
        <div class="w-20 h-20 rounded-lg border border-white/10 flex items-center justify-center mx-auto mb-6 bg-[#1A1512]">
          <Coffee class="w-8 h-8 text-white/10" stroke-width="1" />
        </div>
        <h3 class="font-premium-serif text-2xl font-medium text-white/20">Mọi thứ đã sẵn sàng</h3>
        <p class="text-[9px] uppercase tracking-[0.3em] text-[#8A8178] font-bold mt-3">Hiện không có yêu cầu mới từ quầy</p>
      </div>
    </main>

    <!-- ===== PANEL: LỊCH SỬ ===== -->
    <main v-else class="p-6 flex-1 overflow-y-auto">

      <!-- Search + Filter bar -->
      <div class="flex items-center gap-3 mb-5">
        <div class="relative flex-1 max-w-xs">
          <Search class="w-3.5 h-3.5 absolute left-3 top-1/2 -translate-y-1/2 text-[#8A8178]" stroke-width="2" />
          <input
            v-model="historySearch"
            placeholder="Tìm theo bàn, order..."
            class="w-full pl-9 pr-4 h-9 bg-[#1A1512] border border-white/10 rounded-lg text-xs font-medium text-white placeholder:text-[#8A8178] focus:outline-none focus:border-[#CC8033]/50"
          />
        </div>
        <div class="flex items-center gap-2 text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">
          <Clock class="w-3 h-3" />
          Hôm nay
        </div>
        <button @click="handleClearHistoryClick" v-if="completedOrders.length > 0"
          class="ml-auto flex items-center gap-1.5 px-3 h-9 rounded-lg border border-red-500/30 bg-red-500/10 text-red-400 text-[10px] font-bold uppercase tracking-wide hover:bg-red-500/20 transition-all shadow-xs cursor-pointer">
          <Lock class="w-3 h-3 text-red-400" />
          <span>Xoá lịch sử</span>
        </button>
      </div>

      <!-- Summary strip -->
      <div class="grid grid-cols-3 gap-4 mb-6">
        <div class="bg-[#1A1512] rounded-lg border border-white/10 p-4 text-center">
          <div class="font-premium-sans text-3xl font-bold text-emerald-400">{{ completedOrders.length }}</div>
          <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mt-1">Đơn hoàn thành</div>
        </div>
        <div class="bg-[#1A1512] rounded-lg border border-white/10 p-4 text-center">
          <div class="font-premium-sans text-3xl font-bold text-[#CC8033]">{{ totalItemsDone }}</div>
          <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mt-1">Món đã phục vụ</div>
        </div>
        <div class="bg-[#1A1512] rounded-lg border border-white/10 p-4 text-center">
          <div class="font-premium-sans text-3xl font-bold text-white">{{ avgDuration }}</div>
          <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mt-1">TG xử lý TB</div>
        </div>
      </div>

      <!-- History list -->
      <div v-if="filteredHistory.length > 0" class="space-y-3">
        <div
          v-for="o in filteredHistory"
          :key="o.id"
          class="bg-[#1A1512] border border-white/10 rounded-lg overflow-hidden hover:border-white/20 transition-colors duration-200"
        >
          <!-- Row header -->
          <div class="flex items-center justify-between px-5 py-3 border-b border-white/5 bg-black/20">
            <div class="flex items-center gap-4">
              <!-- Done badge -->
              <div class="w-8 h-8 rounded-lg bg-emerald-500/15 border border-emerald-500/30 flex items-center justify-center">
                <CheckCircle2 class="w-4 h-4 text-emerald-400" stroke-width="2" />
              </div>
              <div>
                <span class="font-premium-sans text-lg font-bold">{{ o.table }}</span>
                <span class="ml-2 text-[#8A8178] text-xs font-medium">#{{ o.id }}</span>
              </div>
              <!-- Items count pill -->
              <span class="px-2.5 py-1 rounded-full bg-white/5 border border-white/10 text-[10px] font-bold text-[#8A8178] uppercase tracking-wide">
                {{ o.items.length }} món
              </span>
            </div>

            <div class="flex items-center gap-6 text-right">
              <!-- Duration -->
              <div>
                <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mb-0.5">Thời gian xử lý</div>
                <div :class="['text-sm font-bold tabular-nums', durationColor(o.duration)]">
                  {{ fmtDuration(o.duration) }}
                </div>
              </div>
              <!-- Completed at -->
              <div>
                <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold mb-0.5">Hoàn tất lúc</div>
                <div class="text-sm font-bold text-white tabular-nums">{{ o.completedAt }}</div>
              </div>
              <!-- Expand toggle -->
              <button @click="toggleHistory(o.id)" class="w-8 h-8 rounded-lg border border-white/10 flex items-center justify-center text-[#8A8178] hover:text-white hover:bg-white/5 transition-colors">
                <ChevronDown class="w-4 h-4 transition-transform duration-200" :class="expandedHistory.has(o.id) ? 'rotate-180' : ''" />
              </button>
            </div>
          </div>

          <!-- Expandable items list -->
          <div v-if="expandedHistory.has(o.id)" class="px-5 py-3 grid grid-cols-2 md:grid-cols-3 gap-2">
            <div
              v-for="(it, i) in o.items"
              :key="i"
              :class="['flex items-center gap-2 px-3 py-2 rounded-lg bg-black/20 border', it.outOfStock ? 'border-red-500/20 bg-red-500/5' : 'border-white/5']"
            >
              <XCircle v-if="it.outOfStock" class="w-3.5 h-3.5 text-red-500 shrink-0" stroke-width="2" />
              <CheckCircle2 v-else class="w-3.5 h-3.5 text-emerald-400 shrink-0" stroke-width="2" />
              <span :class="['text-xs font-medium truncate', it.outOfStock ? 'text-red-400/70 line-through' : 'text-white/70']">
                {{ it.name }}
              </span>
              <span v-if="it.outOfStock" class="text-[9px] uppercase font-black text-red-500 bg-red-500/10 border border-red-500/20 px-1 py-0.5 rounded ml-2 shrink-0">
                Hết NL
              </span>
              <span :class="it.outOfStock ? 'text-red-400/50' : 'text-[#CC8033]'" class="text-xs font-bold ml-auto shrink-0">
                ×{{ it.qty }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty history -->
      <div v-else class="text-center py-32">
        <div class="w-20 h-20 rounded-lg border border-white/10 flex items-center justify-center mx-auto mb-6 bg-[#1A1512]">
          <History class="w-8 h-8 text-white/10" stroke-width="1" />
        </div>
        <h3 class="font-premium-serif text-2xl font-medium text-white/20">Chưa có lịch sử</h3>
        <p class="text-[9px] uppercase tracking-[0.3em] text-[#8A8178] font-bold mt-3">Hoàn tất đơn hàng để xem lịch sử tại đây</p>
      </div>
    </main>

    <!-- Modal xem trước tem dán ly -->
    <div v-if="showPrintModal" class="fixed inset-0 z-50 flex items-center justify-center bg-espresso/80 backdrop-blur-sm animate-in fade-in duration-200">
      <div class="bg-white rounded-2xl shadow-2xl w-[360px] p-6 relative border border-cream-deep text-black animate-in zoom-in-95 duration-300">
        <button @click="showPrintModal = false" class="absolute top-4 right-4 text-gray-400 hover:text-black transition-colors bg-gray-100 hover:bg-gray-200 rounded-full p-1">
          <X class="w-4 h-4" />
        </button>

        <div class="text-center mb-5">
          <h3 class="font-display text-lg font-bold text-espresso">Xem trước tem dán ly</h3>
          <p class="text-xs text-gray-500">Tem nhiệt kích thước tiêu chuẩn 50x30mm</p>
        </div>

        <!-- Thermal Label Mockup -->
        <div id="print-label-content" class="bg-[#F8F9FA] border-2 border-dashed border-gray-300 p-4 rounded-xl font-mono text-xs text-black shadow-inner flex flex-col justify-between min-h-[160px]">
          <div class="border-b border-gray-400 pb-2 text-center">
            <div class="font-bold text-sm tracking-widest text-espresso">{{ storeInfoStore.tenQuan.toUpperCase() }}</div>
            <div class="text-[9px] text-gray-500">Mã đơn: {{ printLabelData?.orderId }}</div>
          </div>
          
          <div class="py-3 space-y-1.5 font-sans">
            <div class="flex justify-between items-baseline">
              <span class="text-[10px] text-gray-500 uppercase">Khu vực:</span>
              <span class="font-bold text-sm text-espresso">{{ printLabelData?.table }}</span>
            </div>
            <div class="flex justify-between items-baseline">
              <span class="text-[10px] text-gray-500 uppercase">Sản phẩm:</span>
              <span class="font-bold text-sm text-espresso">{{ printLabelData?.name }}</span>
            </div>
            <div class="flex justify-between items-baseline">
              <span class="text-[10px] text-gray-500 uppercase">Số lượng:</span>
              <span class="font-bold text-sm text-espresso">x{{ printLabelData?.qty }}</span>
            </div>
            <div v-if="printLabelData?.note" class="bg-yellow-50 border border-yellow-200 p-1.5 rounded text-[10px] mt-1 text-espresso">
              <span class="font-bold text-amber-700">Lưu ý:</span> {{ printLabelData?.note }}
            </div>
          </div>

          <div class="border-t border-gray-400 pt-2 flex justify-between text-[9px] text-gray-500">
            <span>Giờ vào: {{ printLabelData?.time }}</span>
            <span>KDS Printed</span>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-3 mt-6">
          <button @click="showPrintModal = false" class="py-2.5 rounded-xl border border-gray-300 bg-white hover:bg-gray-50 text-gray-700 text-xs font-bold transition-colors">
            Hủy bỏ
          </button>
          <button @click="triggerPrint" class="py-2.5 rounded-xl bg-espresso hover:bg-brown text-white text-xs font-bold transition-colors flex items-center justify-center gap-1.5 shadow-md">
            <Printer class="w-4 h-4" />
            In tem ngay
          </button>
        </div>
      </div>
    </div>

    <!-- Modal xác nhận Báo Hết Nguyên Liệu (Bếp) -->
    <div v-if="outOfStockModalOpen && outOfStockTarget" class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/75 backdrop-blur-md">
      <div class="relative w-full max-w-md bg-[#1A1512] rounded-3xl border border-amber-500/30 p-6 shadow-2xl space-y-5 text-white animate-in zoom-in-95 duration-200">
        <div class="flex items-center gap-3 border-b border-white/10 pb-4">
          <div class="w-12 h-12 rounded-2xl bg-amber-500/20 border border-amber-500/40 text-amber-400 flex items-center justify-center shrink-0">
            <AlertTriangle class="w-6 h-6" />
          </div>
          <div>
            <h3 class="font-premium-serif text-lg font-bold text-amber-400">Xác nhận báo hết nguyên liệu</h3>
            <p class="text-xs text-[#8A8178] mt-0.5">Xác nhận tình trạng kho thực tế tại bếp</p>
          </div>
        </div>

        <div class="space-y-3 bg-white/5 p-4 rounded-2xl border border-white/10 text-xs leading-relaxed">
          <div class="flex justify-between items-center pb-2 border-b border-white/10">
            <span class="text-[#8A8178]">Đơn hàng:</span>
            <span class="font-bold text-white">{{ outOfStockTarget.table }}</span>
          </div>
          <div class="flex justify-between items-center pb-2 border-b border-white/10">
            <span class="text-[#8A8178]">Món báo hết:</span>
            <span class="font-bold text-amber-400 text-sm">{{ outOfStockTarget.itemName }}</span>
          </div>
          <p class="text-white/80 pt-1">
            ⚠️ Nguyên liệu cho món này hiện tại <strong>chỉ còn đủ làm cho 1 ly / món đang chờ này</strong>.
          </p>
          <p class="text-white/70">
            Sau khi xác nhận, món <strong>{{ outOfStockTarget.itemName }}</strong> sẽ tự động chuyển sang trạng thái <span class="text-red-400 font-bold">TẠM HẾT</span> trên cả <strong>Bán hàng tại quầy (POS)</strong> và <strong>Thực đơn gọi món (QR Menu)</strong>. Khách &amp; Thu ngân không thể đặt thêm món này nữa cho tới khi Quản lý mở lại ở <strong>Thực đơn</strong>.
          </p>
        </div>

        <div class="flex gap-3 pt-2">
          <button
            @click="outOfStockModalOpen = false; outOfStockTarget = null"
            class="flex-1 h-11 rounded-xl border border-white/20 bg-white/5 text-white/80 hover:bg-white/10 font-bold text-xs uppercase tracking-wider transition-all"
          >
            Hủy bỏ
          </button>
          <button
            @click="confirmOutOfStock"
            class="flex-1 h-11 rounded-xl bg-gradient-to-r from-red-600 to-amber-600 hover:from-red-500 hover:to-amber-500 text-white font-bold text-xs uppercase tracking-wider shadow-lg shadow-red-900/40 transition-all flex items-center justify-center gap-2"
          >
            <AlertTriangle class="w-4 h-4" />
            Xác nhận Báo Hết
          </button>
        </div>
      </div>
    </div>

    <!-- Modal Xác thực Mã PIN Admin 0000 Xóa Lịch Sử -->
    <div v-if="showAdminAuthModal" class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/80 backdrop-blur-md animate-in fade-in duration-200" @click="showAdminAuthModal = false">
      <div class="relative w-full max-w-sm bg-[#1A1512] rounded-3xl border border-red-500/40 p-6 shadow-2xl space-y-4 text-white animate-in zoom-in-95 duration-200" @click.stop>
        <div class="flex items-center gap-3 border-b border-white/10 pb-3">
          <div class="w-10 h-10 rounded-2xl bg-red-500/20 border border-red-500/40 text-red-400 flex items-center justify-center shrink-0">
            <Lock class="w-5 h-5" />
          </div>
          <div class="text-left">
            <h3 class="font-premium-serif text-base font-bold text-red-400">Xác Nhận Mã PIN Admin</h3>
            <p class="text-[10px] text-[#8A8178] font-semibold">
              {{ isAdmin ? 'Nhập mã PIN 4 số (Mặc định: 0000)' : 'Vui lòng nhập mã PIN bảo mật 4 số của Quản trị viên' }}
            </p>
          </div>
        </div>

        <div class="space-y-3 text-center">
          <div class="space-y-1.5">
            <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block">MÃ PIN BẢO MẬT ADMIN:</label>
            <input 
              v-model="adminPinInput" 
              type="password" 
              maxlength="4"
              :placeholder="isAdmin ? '0000' : '••••'" 
              class="w-full px-4 py-2.5 bg-black/60 border border-white/20 rounded-2xl text-2xl font-black text-[#CC8033] placeholder:text-white/20 focus:outline-none focus:border-[#CC8033] text-center tracking-[0.5em]"
              @keyup.enter="verifyAdminPinAndClear" 
            />
            <p v-if="adminAuthError" class="text-xs font-bold text-red-400 pt-0.5">
              {{ adminAuthError }}
            </p>
          </div>

          <!-- Bàn phím số cảm ứng 4 số -->
          <div class="grid grid-cols-3 gap-2 pt-1 max-w-[230px] mx-auto">
            <button v-for="num in ['1','2','3','4','5','6','7','8','9']" :key="num" 
              @click="adminPinInput = (adminPinInput + num).slice(0, 4)"
              class="py-2 rounded-xl bg-white/10 hover:bg-white/20 active:bg-[#CC8033] text-white font-bold text-base border border-white/10 transition-all active:scale-95 shadow-xs cursor-pointer">
              {{ num }}
            </button>
            <button @click="adminPinInput = ''" class="py-2 rounded-xl bg-red-500/20 hover:bg-red-500/30 text-red-300 font-bold text-xs border border-red-500/30 transition-all active:scale-95 cursor-pointer">
              Xóa
            </button>
            <button @click="adminPinInput = (adminPinInput + '0').slice(0, 4)" class="py-2 rounded-xl bg-white/10 hover:bg-white/20 active:bg-[#CC8033] text-white font-bold text-base border border-white/10 transition-all active:scale-95 shadow-xs cursor-pointer">
              0
            </button>
            <button v-if="isAdmin" @click="adminPinInput = '0000'" class="py-2 rounded-xl bg-[#CC8033]/20 hover:bg-[#CC8033]/40 text-[#E89E53] font-bold text-[10px] border border-[#CC8033]/30 transition-all active:scale-95 cursor-pointer">
              0000
            </button>
            <button v-else @click="adminPinInput = adminPinInput.slice(0, -1)" class="py-2 rounded-xl bg-white/5 hover:bg-white/15 text-white/70 font-bold text-xs border border-white/10 transition-all active:scale-95 cursor-pointer">
              ⌫ Lùi
            </button>
          </div>
        </div>

        <div class="flex gap-2.5 pt-2 border-t border-white/10">
          <button
            @click="showAdminAuthModal = false"
            class="flex-1 h-10 rounded-xl border border-white/20 bg-white/5 text-white/80 hover:bg-white/10 font-bold text-xs uppercase tracking-wider transition-all cursor-pointer"
          >
            Hủy
          </button>
          <button
            @click="verifyAdminPinAndClear"
            :disabled="!adminPinInput.trim() || adminAuthBusy"
            class="flex-1 h-10 rounded-xl bg-gradient-to-r from-red-600 to-amber-600 hover:from-red-500 hover:to-amber-500 text-white font-bold text-xs uppercase tracking-wider shadow-lg shadow-red-900/40 transition-all flex items-center justify-center gap-1.5 disabled:opacity-50 cursor-pointer"
          >
            <Lock class="w-3.5 h-3.5" />
            Xóa Lịch Sử
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import {
  Volume2, VolumeX, Coffee, ChevronLeft, ChevronRight, ChevronDown,
  Check, CheckCircle2, Search, Clock, Trash2, History, ClipboardList,
  X, AlertTriangle, LayoutGrid, List, Bell, Zap, Printer, XCircle, Lock
} from 'lucide-vue-next'
import { useOrderStore } from '@/stores/orders'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { useToast } from '@/stores/toast'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/services/api'
import { auditLogsApi } from '@/services/auditLogs'
import { ordersApi } from '@/services/orders'
import type { Order } from '@/data/orders'

const syncChannel = typeof BroadcastChannel !== 'undefined' ? new BroadcastChannel('quanlycf_orders_sync') : null

const sendCanBungNuocRequest = async (o: Order) => {
  try {
    let maBan = o.maBan || 0
    if (!maBan && o.table) {
      maBan = parseInt(o.table.replace(/\D/g, ''), 10)
    }
    if (maBan && maBan > 0) {
      await ordersApi.createServiceRequest({
        maBan,
        loaiYeuCau: 'CanBungNuoc',
        ghiChu: `Bếp đã pha xong đồ uống cho ${o.table} (Đơn #${o.id.replace('DH-', '')})`
      })
      if (syncChannel) {
        syncChannel.postMessage({ type: 'SERVICE_REQUEST_CHANGED', ts: Date.now() })
      }
    }
  } catch (err) {
    console.error('Lỗi tạo yêu cầu bưng nước:', err)
  }
}

// ── Types ──────────────────────────────────────────────────────
interface KItem    { name: string; qty: number; done: boolean }
interface KDone    { id: string; table: string; items: KItem[]; duration: number; completedAt: string }



// ── Store đơn hàng (nguồn dữ liệu chung) ────────────────────────
const orderStore     = useOrderStore()
const storeInfoStore = useStoreInfoStore()
const toast          = useToast()

// Đơn đang làm = các đơn ở trạng thái chờ xác nhận / đang pha chế / chờ lấy (ready)
const activeOrders = computed(() =>
  orderStore.orders.filter(o => o.status === 'pending' || o.status === 'preparing' || o.status === 'ready')
)

// ── State ──────────────────────────────────────────────────────
const completedOrders  = ref<KDone[]>([])
const now              = ref(Date.now())

// --- Print Label State ---
const showPrintModal = ref(false)
const printLabelData = ref<{
  orderId: string
  table: string
  name: string
  qty: number
  note?: string
  time: string
} | null>(null)

const openPrintPreview = (order: Order, item: any) => {
  const timeFormatted = new Date(order.createdTs).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
  printLabelData.value = {
    orderId: order.id.slice(-6).toUpperCase(),
    table: order.table,
    name: item.name,
    qty: item.qty,
    note: item.note,
    time: timeFormatted
  }
  showPrintModal.value = true
}

const triggerPrint = () => {
  window.print()
  showPrintModal.value = false
  if (printLabelData.value) {
    toast.success(`Đã gửi lệnh in tem cho: ${printLabelData.value.name}`, 'In tem dán ly')
  }
}
const activeTab        = ref<'active' | 'history'>('active')
const currentPage      = ref(1)
const itemsPerPage     = 8
const historySearch    = ref('')
const expandedHistory  = ref<Set<string>>(new Set())
const viewMode         = ref<'table' | 'item'>('table')
const orderTypeFilter  = ref<'all' | 'takeaway' | 'dinein'>('all')

const isTakeaway = (table: string) => {
  const t = (table || '').toLowerCase()
  return t.includes('mang về') || t.includes('takeaway') || t.includes('mang đi')
}

const filteredActiveOrders = computed(() => {
  return activeOrders.value.filter(o => {
    if (orderTypeFilter.value === 'all') return true
    if (orderTypeFilter.value === 'takeaway') return isTakeaway(o.table)
    if (orderTypeFilter.value === 'dinein') return !isTakeaway(o.table)
    return true
  }).sort((a, b) => {
    if (a.isPriority && !b.isPriority) return -1
    if (!a.isPriority && b.isPriority) return 1
    return 0
  })
})

const aggregatedItems = computed(() => {
  const map = new Map<string, { qty: number; done: number; outOfStock: number; tables: string[] }>()
  filteredActiveOrders.value.forEach(o => {
    o.items.forEach(it => {
      const key = it.name
      if (!map.has(key)) map.set(key, { qty: 0, done: 0, outOfStock: 0, tables: [] })
      const group = map.get(key)!
      group.qty += it.qty
      if (it.done) group.done += it.qty
      if (it.outOfStock) group.outOfStock += it.qty
      if (!group.tables.includes(o.table)) group.tables.push(o.table)
    })
  })
  return Array.from(map.entries()).map(([name, data]) => ({ name, ...data })).sort((a, b) => b.qty - a.qty)
})

const saveCompletedOrders = () => {
  try {
    localStorage.setItem('kitchen_completed_orders', JSON.stringify(completedOrders.value))
  } catch (e) {}
}

const loadCompletedOrders = () => {
  try {
    const raw = localStorage.getItem('kitchen_completed_orders')
    if (raw) {
      completedOrders.value = JSON.parse(raw)
      return
    }
  } catch (e) {}
  completedOrders.value = []
}

loadCompletedOrders()

// ── Timer ──────────────────────────────────────────────
let timer: ReturnType<typeof setInterval> | null = null

const playSound = (_type?: 'new' | 'alarm' | 'vip') => {
  // Đã tắt hoàn toàn âm thanh màn hình bếp theo yêu cầu
  return
}

let pollTimer: ReturnType<typeof setInterval> | null = null

onMounted (() => { 
  orderStore.fetchOrders()
  pollTimer = setInterval(() => {
    if (!document.hidden) {
      orderStore.fetchOrders()
    }
  }, 2000)
  timer = setInterval(() => { 
    now.value = Date.now() 
  }, 1000) 
})
onUnmounted(() => { 
  if (timer) clearInterval(timer) 
  if (pollTimer) clearInterval(pollTimer)
})

// ── Tabs ───────────────────────────────────────────────────────
const tabs = computed(() => [
  { id: 'active'  as const, label: 'Đang làm',  icon: ClipboardList, count: activeOrders.value.length },
  { id: 'history' as const, label: 'Lịch sử',   icon: History,       count: completedOrders.value.length },
])

// ── Computed ───────────────────────────────────────────────────
const timeString = computed(() =>
  new Date(now.value).toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
)

const inProgress  = computed(() => activeOrders.value.filter(o => !isAllDone(o)).length)
const readyCount  = computed(() => activeOrders.value.filter(o => isAllDone(o)).length)

const totalPages  = computed(() => Math.ceil(filteredActiveOrders.value.length / itemsPerPage) || 1)
const paginatedActive = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  return filteredActiveOrders.value.slice(start, start + itemsPerPage)
})

const filteredHistory = computed(() => {
  const q = historySearch.value.toLowerCase().trim()
  if (!q) return completedOrders.value
  return completedOrders.value.filter(o =>
    o.table.includes(q) || o.id.includes(q) || o.items.some(i => i.name.toLowerCase().includes(q))
  )
})

const totalItemsDone = computed(() =>
  completedOrders.value.reduce((s, o) => s + o.items.reduce((ss, i) => ss + (i.outOfStock ? 0 : i.qty), 0), 0)
)

const avgDuration = computed(() => {
  if (!completedOrders.value.length) return '—'
  const avg = completedOrders.value.reduce((s, o) => s + o.duration, 0) / completedOrders.value.length
  return fmtDuration(avg)
})

// ── Helpers ────────────────────────────────────────────────────
const isAllDone = (o: Order) => o.items.every(i => i.done || i.outOfStock)

const fmtElapsed = (started: number) => {
  const s = Math.max(0, Math.floor((now.value - started) / 1000))
  return `${String(Math.floor(s / 60)).padStart(2, '0')}:${String(s % 60).padStart(2, '0')}`
}

const fmtDuration = (ms: number) => {
  const m = Math.floor(ms / 60000)
  const s = Math.floor((ms % 60000) / 1000)
  return `${m}p ${String(s).padStart(2, '0')}s`
}

const colorByMin = (started: number) => {
  const m = Math.max(0, (now.value - started) / 60000)
  if (m < 10) return 'text-emerald-400 border-emerald-500/30 bg-emerald-500/5'
  if (m < 20) return 'text-amber-400 border-amber-500/30 bg-amber-500/5'
  return 'text-red-400 border-red-500/60 bg-red-500/10 animate-pulse shadow-[0_0_15px_rgba(239,68,68,0.4)]'
}

const durationColor = (ms: number) => {
  const m = ms / 60000
  if (m < 10) return 'text-emerald-400'
  if (m < 15) return 'text-amber-400'
  return 'text-red-400'
}

// ── Actions (uỷ thác cho store đơn hàng chung) ──────────────────
const toggle = (oid: string, idx: number) => {
  const o = orderStore.getById(oid)
  const it = o?.items[idx]
  if (it) {
    const wasDone = it.done
    orderStore.toggleItemDone(oid, idx)
    if (!wasDone) {
      toast.success(`Đã làm xong món: ${it.name} (x${it.qty})`, `Đơn ${o?.table}`)
    } else {
      toast.info(`Chuyển món ${it.name} về hàng chờ`, `Đơn ${o?.table}`)
    }
  }
}

// Out of stock confirmation modal state
const outOfStockModalOpen = ref(false)
const outOfStockTarget = ref<{ oid: string; idx: number; itemName: string; table: string } | null>(null)

const reportOutOfStock = (oid: string, idx: number) => {
  const o = orderStore.getById(oid)
  const it = o?.items[idx]
  if (!it) return

  if (it.outOfStock) {
    // Nếu đang outOfStock -> khôi phục nguyên liệu trực tiếp
    orderStore.toggleOutOfStock(oid, idx)
    toast.success(`Đã khôi phục nguyên liệu cho: ${it.name}`, 'Cập nhật kho')
  } else {
    // Mở popup xác nhận báo hết
    outOfStockTarget.value = {
      oid,
      idx,
      itemName: it.name,
      table: o?.table || 'Đơn hàng'
    }
    outOfStockModalOpen.value = true
  }
}

const confirmOutOfStock = () => {
  if (!outOfStockTarget.value) return
  const { oid, idx, itemName, table } = outOfStockTarget.value
  orderStore.toggleOutOfStock(oid, idx)
  toast.warning(`Đã báo hết nguyên liệu cho: ${itemName} (Đã chuyển TẠM HẾT trên POS & Menu)`, 'Khóa món thành công')
  
  auditLogsApi.createLog({
    maNhanVien: authStore.user?.maNhanVien,
    hanhDong: 'BÁO HẾT MÓN',
    module: 'BẾP - KDS',
    duLieuMoi: `Mới: Nhân viên bếp báo hết nguyên liệu món [${itemName}] tại ${table}. Món đã được tạm khóa trên POS & QR Menu.`
  }).catch(() => {})

  outOfStockModalOpen.value = false
  outOfStockTarget.value = null
}

const markReady = (o: Order) => {
  if (!isAllDone(o)) return
  orderStore.updateStatus(o.id, 'ready')
  orderStore.notifyPos(o.table)
  sendCanBungNuocRequest(o)
  toast.success(`Đã thông báo BƯNG NƯỚC cho Phục vụ tại ${o.table}!`, 'Sẵn sàng giao')

  auditLogsApi.createLog({
    maNhanVien: authStore.user?.maNhanVien,
    hanhDong: 'PHA XONG MÓN',
    module: 'BẾP - KDS',
    duLieuMoi: `Mới: Bếp đã pha chế xong toàn bộ đồ uống cho [${o.table}] (${o.items.length} món).`
  }).catch(() => {})
}

const complete = (o: Order) => {
  const duration = Date.now() - o.createdTs
  const completedAt = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })

  const exists = completedOrders.value.some(item => item.id === o.id)
  if (!exists) {
    completedOrders.value.unshift({
      id: o.id,
      table: o.table,
      items: o.items.map(i => ({ name: i.name, qty: i.qty, done: i.done, outOfStock: i.outOfStock })),
      duration,
      completedAt,
    })
    saveCompletedOrders()
  }

  orderStore.updateStatus(o.id, 'done')
  sendCanBungNuocRequest(o)
  if (currentPage.value > totalPages.value && currentPage.value > 1) currentPage.value--
  toast.success(`Đã hoàn tất giao đồ cho ${o.table}`, 'Hoàn tất đơn')

  auditLogsApi.createLog({
    maNhanVien: authStore.user?.maNhanVien,
    hanhDong: 'GIAO ĐỒ',
    module: 'BẾP - KDS',
    duLieuMoi: `Mới: Bếp đã hoàn tất giao đồ cho [${o.table}]. Thời gian phục vụ: ${fmtDuration(duration)}.`
  }).catch(() => {})
}

const toggleHistory = (id: string) => {
  if (expandedHistory.value.has(id)) expandedHistory.value.delete(id)
  else expandedHistory.value.add(id)
  expandedHistory.value = new Set(expandedHistory.value) // trigger reactivity
}

const authStore = useAuthStore()

const isAdmin = computed(() => {
  const user = authStore.user
  if (!user) return false
  const roleId = user.maVaiTro
  const roleName = (user.tenVaiTro || user.vaiTro || '').toLowerCase()
  const quyens = user.quyens || []
  return roleId === 1 || roleName.includes('admin') || roleName.includes('quản lý') || roleName.includes('quanly') || quyens.includes('System.Admin') || quyens.includes('CAIDAT_QUANLY')
})

// --- State Xác thực Admin xóa Lịch sử ---
const showAdminAuthModal = ref(false)
const adminPinInput = ref('')
const adminAuthError = ref('')
const adminAuthBusy = ref(false)

const handleClearHistoryClick = () => {
  if (completedOrders.value.length === 0) return
  adminPinInput.value = ''
  adminAuthError.value = ''
  showAdminAuthModal.value = true
}

const verifyAdminPinAndClear = async () => {
  adminAuthError.value = ''
  const pin = adminPinInput.value.trim()
  if (!pin) {
    adminAuthError.value = 'Vui lòng nhập mã PIN Admin 4 số!'
    return
  }

  // Quy định mã PIN Admin xóa lịch sử: '0000' (hoặc 1234, admin, 8888)
  if (['0000', '1234', '8888', '123456', 'admin'].includes(pin.toLowerCase())) {
    completedOrders.value = []
    localStorage.removeItem('kitchen_completed_orders')
    showAdminAuthModal.value = false
    toast.success('Xác thực Mã PIN thành công! Đã xóa toàn bộ lịch sử pha chế.', 'Quản trị viên')

    auditLogsApi.createLog({
      maNhanVien: authStore.user?.maNhanVien,
      hanhDong: 'XOÁ LỊCH SỬ',
      module: 'BẾP - KDS',
      duLieuMoi: 'Mới: Quản trị viên đã xác thực PIN và xóa toàn bộ lịch sử pha chế trong ngày.'
    }).catch(() => {})
  } else {
    adminAuthError.value = isAdmin.value
      ? 'Mã PIN Admin không chính xác (Mặc định: 0000)!'
      : 'Mã PIN Admin không chính xác. Vui lòng thử lại!'
  }
}

const clearHistory = () => {
  handleClearHistoryClick()
}
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans { font-family: 'Be Vietnam Pro', system-ui, sans-serif; }

::-webkit-scrollbar       { width: 4px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background-color: rgba(255,255,255,0.05); border-radius: 4px; }

@media print {
  /* Hide everything except the thermal label mockup */
  :global(#app), :global(body) {
    background: white !important;
  }
  :global(body *) {
    visibility: hidden !important;
  }
  #print-label-content, #print-label-content * {
    visibility: visible !important;
  }
  #print-label-content {
    position: fixed !important;
    left: 0 !important;
    top: 0 !important;
    width: 50mm !important;
    height: 30mm !important;
    border: none !important;
    background: white !important;
    box-shadow: none !important;
    padding: 2mm !important;
    margin: 0 !important;
    display: flex !important;
    flex-direction: column !important;
    justify-content: space-between !important;
    color: black !important;
    font-family: monospace !important;
    font-size: 8pt !important;
    box-sizing: border-box !important;
  }
}
</style>
