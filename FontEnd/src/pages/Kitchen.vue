<template>
  <div class="min-h-screen text-[#FDFBF7] font-premium-sans bg-[#0F0A07] flex flex-col" @click="openAssign = null">

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
          <button @click="muted = !muted" class="w-9 h-9 rounded-lg border border-white/10 flex items-center justify-center">
            <VolumeX v-if="muted" class="w-4 h-4 text-[#8A8178]" />
            <Volume2 v-else class="w-4 h-4 text-[#CC8033]" />
          </button>
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
        <!-- Bộ lọc Trạm pha chế -->
        <div class="flex items-center bg-black/40 rounded-lg border border-white/10 p-1">
          <button @click="stationFilter = 'all'" :class="stationFilter === 'all' ? 'bg-[#CC8033] text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">Tất cả trạm</button>
          <button @click="stationFilter = 'bar'" :class="stationFilter === 'bar' ? 'bg-[#CC8033] text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">Quầy Pha Chế</button>
          <button @click="stationFilter = 'kitchen'" :class="stationFilter === 'kitchen' ? 'bg-[#CC8033] text-white shadow-sm' : 'text-[#8A8178] hover:text-white'" class="px-3 py-1.5 rounded-md text-[10px] font-bold uppercase tracking-wider transition-all">Quầy Bánh</button>
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
              v-show="stationFilter === 'all' || getStation(it.name) === stationFilter"
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

              <!-- Người làm + Báo hết nguyên liệu -->
              <div class="flex items-center gap-2 mt-2 pl-8">
                <!-- Assignee chip + dropdown -->
                <div class="relative">
                  <button
                    @click.stop="toggleAssign(o.id, i)"
                    :class="['flex items-center gap-1.5 px-2 py-1 rounded-md border text-[10px] font-bold uppercase tracking-wide transition-colors',
                      it.assignee ? 'bg-[#CC8033]/10 border-[#CC8033]/30 text-[#CC8033]' : 'bg-white/5 border-white/10 text-[#8A8178] hover:text-white']"
                  >
                    <User class="w-3 h-3" stroke-width="2.5" />
                    {{ it.assignee || 'Chưa gán' }}
                    <ChevronDown class="w-3 h-3 opacity-60" />
                  </button>
                  <!-- Dropdown menu -->
                  <div v-if="openAssign === o.id + '-' + i"
                    class="absolute z-20 left-0 mt-1 w-32 rounded-lg border border-white/10 bg-[#1A1512] shadow-card py-1">
                    <button v-for="s in staffList" :key="s"
                      @click.stop="assign(o.id, i, s)"
                      :class="['w-full text-left px-3 py-1.5 text-xs font-medium transition-colors hover:bg-white/5',
                        it.assignee === s ? 'text-[#CC8033]' : 'text-white/80']">
                      {{ s }}
                    </button>
                    <div class="h-px bg-white/10 my-1"></div>
                    <button @click.stop="assign(o.id, i, '')"
                      class="w-full text-left px-3 py-1.5 text-xs font-medium text-[#8A8178] hover:bg-white/5">
                      Bỏ gán
                    </button>
                  </div>
                </div>

                <!-- Báo hết nguyên liệu -->
                <button
                  @click.stop="reportOutOfStock(o.id, i)"
                  :class="['ml-auto flex items-center gap-1 px-2 py-1 rounded-md border text-[10px] font-bold uppercase tracking-wide transition-colors',
                    it.outOfStock ? 'bg-red-500/15 border-red-500/40 text-red-400' : 'bg-white/5 border-white/10 text-[#8A8178] hover:text-red-400 hover:border-red-500/30']"
                  :title="it.outOfStock ? 'Bỏ báo hết nguyên liệu' : 'Báo hết nguyên liệu'"
                >
                  <AlertTriangle class="w-3 h-3" stroke-width="2.5" />
                  {{ it.outOfStock ? 'Hết NL' : 'Báo hết' }}
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
        <button @click="clearHistory" v-if="completedOrders.length > 0"
          class="ml-auto flex items-center gap-1.5 px-3 h-9 rounded-lg border border-red-500/20 bg-red-500/5 text-red-400 text-[10px] font-bold uppercase tracking-wide hover:bg-red-500/10 transition-colors">
          <Trash2 class="w-3 h-3" stroke-width="2" />
          Xoá lịch sử
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
              class="flex items-center gap-2 px-3 py-2 rounded-lg bg-black/20 border border-white/5"
            >
              <CheckCircle2 class="w-3.5 h-3.5 text-emerald-400 shrink-0" stroke-width="2" />
              <span class="text-xs font-medium text-white/70 truncate">{{ it.name }}</span>
              <span class="text-[#CC8033] text-xs font-bold ml-auto shrink-0">×{{ it.qty }}</span>
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import {
  Volume2, VolumeX, Coffee, ChevronLeft, ChevronRight, ChevronDown,
  Check, CheckCircle2, Search, Clock, Trash2, History, ClipboardList,
  User, X, AlertTriangle, LayoutGrid, List, Bell, Zap
} from 'lucide-vue-next'
import { useOrderStore } from '@/stores/orders'
import { useStoreInfoStore } from '@/stores/storeInfo'
import type { Order } from '@/data/orders'

// ── Types ──────────────────────────────────────────────────────
interface KItem    { name: string; qty: number; done: boolean }
interface KDone    { id: string; table: string; items: KItem[]; duration: number; completedAt: string }

// ── Danh sách nhân viên bếp ─────────────────────────────────────
const staffList = ['Minh', 'Lan', 'Huy', 'Trang', 'Phúc']

// ── Store đơn hàng (nguồn dữ liệu chung) ────────────────────────
const orderStore     = useOrderStore()
const storeInfoStore = useStoreInfoStore()

// Đơn đang làm = các đơn ở trạng thái chờ xác nhận / đang pha chế / chờ lấy (ready)
const activeOrders = computed(() =>
  orderStore.orders.filter(o => o.status === 'pending' || o.status === 'preparing' || o.status === 'ready')
)

// ── State ──────────────────────────────────────────────────────
const completedOrders  = ref<KDone[]>([])
const now              = ref(Date.now())
const muted            = ref(false)
const activeTab        = ref<'active' | 'history'>('active')
const currentPage      = ref(1)
const itemsPerPage     = 8
const historySearch    = ref('')
const expandedHistory  = ref<Set<string>>(new Set())
const openAssign       = ref<string | null>(null)
const viewMode         = ref<'table' | 'item'>('table')
const stationFilter    = ref<'all' | 'bar' | 'kitchen'>('all')

const getStation = (name: string) => {
  const n = name.toLowerCase()
  if (n.includes('bánh') || n.includes('cheesecake') || n.includes('croissant') || n.includes('tiramisu')) return 'kitchen'
  return 'bar'
}

const filteredActiveOrders = computed(() => {
  return activeOrders.value.filter(o => 
    stationFilter.value === 'all' || o.items.some(it => getStation(it.name) === stationFilter.value)
  ).sort((a, b) => {
    if (a.isPriority && !b.isPriority) return -1;
    if (!a.isPriority && b.isPriority) return 1;
    return 0;
  })
})

const aggregatedItems = computed(() => {
  const map = new Map<string, { qty: number; done: number; outOfStock: number; tables: string[] }>()
  filteredActiveOrders.value.forEach(o => {
    const validItems = o.items.filter(it => stationFilter.value === 'all' || getStation(it.name) === stationFilter.value)
    validItems.forEach(it => {
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

// Pre-seed a few history entries for demo
completedOrders.value = [
  { id: '1038', table: 'Bàn 2',  duration: 7  * 60 * 1000, completedAt: '09:14', items: [{ name: 'Espresso đôi', qty: 2, done: true }, { name: 'Bánh mì bơ tỏi', qty: 1, done: true }] },
  { id: '1039', table: 'Bàn 9',  duration: 12 * 60 * 1000, completedAt: '09:32', items: [{ name: 'Trà sữa trân châu', qty: 3, done: true }] },
  { id: '1040', table: 'Bàn 4',  duration: 5  * 60 * 1000, completedAt: '09:58', items: [{ name: 'Cappuccino', qty: 1, done: true }, { name: 'Cheesecake dâu', qty: 2, done: true }] },
  { id: '1041', table: 'Bàn 11', duration: 18 * 60 * 1000, completedAt: '10:25', items: [{ name: 'Matcha đá xay', qty: 2, done: true }, { name: 'Croissant bơ', qty: 2, done: true }, { name: 'Nước cam ép', qty: 1, done: true }] },
]

// ── Timer & Audio ──────────────────────────────────────────────
let timer: ReturnType<typeof setInterval> | null = null

const playSound = (type: 'new' | 'alarm' | 'vip') => {
  if (muted.value) return
  try {
     const ctx = new (window.AudioContext || (window as any).webkitAudioContext)()
     const osc = ctx.createOscillator()
     const gain = ctx.createGain()
     osc.connect(gain)
     gain.connect(ctx.destination)
     if (type === 'new') { osc.frequency.value = 800; gain.gain.setValueAtTime(0.1, ctx.currentTime); osc.start(); osc.stop(ctx.currentTime + 0.2) }
     else if (type === 'vip') { osc.frequency.value = 1200; gain.gain.setValueAtTime(0.1, ctx.currentTime); osc.type='square'; osc.start(); osc.stop(ctx.currentTime + 0.3) }
     else if (type === 'alarm') { osc.frequency.value = 400; gain.gain.setValueAtTime(0.2, ctx.currentTime); osc.type='sawtooth'; osc.start(); osc.stop(ctx.currentTime + 0.5) }
  } catch(e) {}
}

watch(() => activeOrders.value.length, (newVal, oldVal) => {
  if (newVal > oldVal) {
     const latest = activeOrders.value[0]
     if (latest?.isPriority) playSound('vip')
     else playSound('new')
  }
})

onMounted (() => { 
  timer = setInterval(() => { 
    now.value = Date.now() 
    if (!muted.value && Math.floor(now.value / 1000) % 10 === 0) {
       // Báo động mỗi 10s cho đơn đỏ
       if (activeOrders.value.some(o => (now.value - o.createdTs) > 20*60000 && !o.isPriority && o.status !== 'ready')) {
          playSound('alarm')
       }
    }
  }, 1000) 
})
onUnmounted(() => { if (timer) clearInterval(timer) })

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
  completedOrders.value.reduce((s, o) => s + o.items.reduce((ss, i) => ss + i.qty, 0), 0)
)

const avgDuration = computed(() => {
  if (!completedOrders.value.length) return '—'
  const avg = completedOrders.value.reduce((s, o) => s + o.duration, 0) / completedOrders.value.length
  return fmtDuration(avg)
})

// ── Helpers ────────────────────────────────────────────────────
const isAllDone = (o: Order) => o.items.every(i => i.done || i.outOfStock)

const fmtElapsed = (started: number) => {
  const s = Math.floor((now.value - started) / 1000)
  return `${String(Math.floor(s / 60)).padStart(2, '0')}:${String(s % 60).padStart(2, '0')}`
}

const fmtDuration = (ms: number) => {
  const m = Math.floor(ms / 60000)
  const s = Math.floor((ms % 60000) / 1000)
  return `${m}p ${String(s).padStart(2, '0')}s`
}

const colorByMin = (started: number) => {
  const m = (now.value - started) / 60000
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
const toggle = (oid: string, idx: number) => orderStore.toggleItemDone(oid, idx)

const toggleAssign = (oid: string, idx: number) => {
  const key = oid + '-' + idx
  openAssign.value = openAssign.value === key ? null : key
}

const assign = (oid: string, idx: number, name: string) => {
  orderStore.setAssignee(oid, idx, name)
  openAssign.value = null
}

const reportOutOfStock = (oid: string, idx: number) => orderStore.toggleOutOfStock(oid, idx)

const markReady = (o: Order) => {
  if (!isAllDone(o)) return
  orderStore.updateStatus(o.id, 'ready')
  orderStore.notifyPos(o.table)
}

const complete = (o: Order) => {
  if (o.status !== 'ready') return
  const duration = Date.now() - o.createdTs
  const completedAt = new Date().toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })
  completedOrders.value.unshift({
    id: o.id,
    table: o.table,
    items: o.items.map(i => ({ name: i.name, qty: i.qty, done: true })),
    duration,
    completedAt,
  })
  orderStore.updateStatus(o.id, 'done')
  if (currentPage.value > totalPages.value && currentPage.value > 1) currentPage.value--
}

const toggleHistory = (id: string) => {
  if (expandedHistory.value.has(id)) expandedHistory.value.delete(id)
  else expandedHistory.value.add(id)
  expandedHistory.value = new Set(expandedHistory.value) // trigger reactivity
}

const clearHistory = () => {
  if (confirm('Xoá toàn bộ lịch sử hôm nay?')) completedOrders.value = []
}
</script>

<style scoped>
.font-premium-serif,
.font-premium-sans { font-family: 'Be Vietnam Pro', system-ui, sans-serif; }

::-webkit-scrollbar       { width: 4px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background-color: rgba(255,255,255,0.05); border-radius: 4px; }
</style>
