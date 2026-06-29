<template>
  <div class="p-6 space-y-5">

    <!-- ── Header ── -->
    <div class="flex items-start justify-between gap-4">
      <div>
        <h1 class="font-display text-2xl font-bold text-espresso">Quản lý tài khoản</h1>
        <p class="text-sm text-muted-foreground mt-0.5">Tài khoản đăng nhập, vai trò và phân quyền nhân viên</p>
      </div>
      <Button @click="openAdd" class="bg-espresso text-cream rounded-xl h-10 px-5 shrink-0 gap-2">
        <Plus class="w-4 h-4" /> Thêm tài khoản
      </Button>
    </div>

    <!-- ── Stats ── -->
    <div class="grid grid-cols-2 lg:grid-cols-4 gap-3">
      <div class="bg-card rounded-2xl border border-cream-deep p-4 space-y-1">
        <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Tổng tài khoản</p>
        <p class="text-3xl font-display font-bold text-espresso">{{ accounts.length }}</p>
      </div>
      <div class="bg-card rounded-2xl border border-cream-deep p-4 space-y-1">
        <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Hoạt động</p>
        <p class="text-3xl font-display font-bold text-green-600">{{ activeCount }}</p>
      </div>
      <div class="bg-card rounded-2xl border border-cream-deep p-4 space-y-1">
        <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Đã khoá</p>
        <p class="text-3xl font-display font-bold text-destructive">{{ lockedCount }}</p>
      </div>
      <RouterLink to="/roles"
        class="bg-espresso/5 border border-espresso/10 rounded-2xl p-4 flex items-center gap-3 hover:bg-espresso/10 transition-colors">
        <div class="w-10 h-10 rounded-xl bg-espresso flex items-center justify-center shrink-0 shadow-sm">
          <ShieldCheck class="w-5 h-5 text-cream" />
        </div>
        <div>
          <p class="text-sm font-bold text-espresso">Vai trò &amp; quyền</p>
          <p class="text-xs text-muted-foreground">Cấu hình phân quyền</p>
        </div>
      </RouterLink>
    </div>

    <!-- ── Toolbar ── -->
    <div class="flex flex-col sm:flex-row gap-3">
      <div class="relative w-full sm:max-w-[240px]">
        <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none" />
        <Input v-model="search" placeholder="Tìm tên, email..." class="pl-9 h-10 bg-card border-cream-deep rounded-xl" />
      </div>
      <div class="flex gap-2 flex-wrap">
        <button @click="roleFilter = 'all'"
          :class="roleFilter === 'all' ? 'bg-espresso text-cream shadow-sm' : 'bg-card text-espresso border border-cream-deep hover:bg-cream'"
          class="px-4 h-10 rounded-xl text-sm font-semibold transition-colors">Tất cả</button>
        <button v-for="r in roles" :key="r.maVaiTro" @click="roleFilter = r.maVaiTro"
          :class="roleFilter === r.maVaiTro ? 'bg-espresso text-cream shadow-sm' : 'bg-card text-espresso border border-cream-deep hover:bg-cream'"
          class="px-4 h-10 rounded-xl text-sm font-semibold transition-colors whitespace-nowrap">{{ r.tenVaiTro }}</button>
      </div>
    </div>

    <p v-if="errorMsg" class="text-sm font-semibold text-red-600 bg-red-50 border border-red-200 rounded-xl px-4 py-3">{{ errorMsg }}</p>

    <!-- ── Table ── -->
    <div class="bg-card rounded-2xl border border-cream-deep overflow-hidden">
      <table class="w-full text-sm">
        <thead>
          <tr class="border-b border-cream-deep bg-cream/50">
            <th class="px-5 py-3.5 text-left text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Tài khoản</th>
            <th class="px-5 py-3.5 text-left text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Vai trò</th>
            <th class="px-5 py-3.5 text-left text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Trạng thái</th>
            <th class="px-5 py-3.5 text-left text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Đăng nhập cuối</th>
            <th class="px-5 py-3.5 text-right text-[11px] font-bold uppercase tracking-wider text-muted-foreground">Thao tác</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-cream-deep/60">
          <tr v-if="loading">
            <td colspan="5" class="py-16 text-center text-muted-foreground">Đang tải...</td>
          </tr>
          <tr v-else-if="paginatedItems.length === 0">
            <td colspan="5" class="py-16 text-center text-muted-foreground">Không tìm thấy tài khoản nào phù hợp.</td>
          </tr>
          <tr v-for="a in paginatedItems" :key="a.maNhanVien" class="hover:bg-cream/30 transition-colors">
            <!-- Tài khoản -->
            <td class="px-5 py-4">
              <div class="flex items-center gap-3">
                <div :class="['w-10 h-10 rounded-xl text-cream font-bold text-sm flex items-center justify-center shrink-0 shadow-sm', avatarColor(a.maNhanVien)]">
                  {{ initials(a.hoTen) }}
                </div>
                <div class="min-w-0">
                  <p class="font-semibold text-espresso truncate">{{ a.hoTen }}</p>
                  <p class="text-xs text-muted-foreground flex items-center gap-1 mt-0.5 truncate">
                    <Mail class="w-3 h-3 shrink-0" /> {{ a.email }}
                  </p>
                </div>
              </div>
            </td>
            <!-- Vai trò -->
            <td class="px-5 py-4">
              <span class="inline-flex items-center px-2.5 py-1 rounded-lg text-xs font-semibold bg-caramel/10 text-caramel border border-caramel/20">
                {{ a.tenVaiTro }}
              </span>
              <p class="text-[11px] text-muted-foreground mt-1 leading-tight">{{ roleDesc(a.maVaiTro) }}</p>
            </td>
            <!-- Trạng thái -->
            <td class="px-5 py-4">
              <span :class="['inline-flex items-center gap-1.5 px-2.5 py-1 rounded-lg text-xs font-semibold border',
                a.trangThaiHoatDong
                  ? 'bg-green-50 text-green-700 border-green-200'
                  : 'bg-red-50 text-red-600 border-red-200']">
                <span :class="['w-1.5 h-1.5 rounded-full', a.trangThaiHoatDong ? 'bg-green-500' : 'bg-red-500']"></span>
                {{ a.trangThaiHoatDong ? 'Hoạt động' : 'Đã khoá' }}
              </span>
            </td>
            <!-- Đăng nhập cuối -->
            <td class="px-5 py-4">
              <div class="flex items-center gap-1.5 text-xs text-muted-foreground">
                <Clock class="w-3.5 h-3.5 shrink-0" />
                {{ a.lanDangNhapCuoi ? fmtDate(a.lanDangNhapCuoi) : 'Chưa đăng nhập' }}
              </div>
            </td>
            <!-- Thao tác -->
            <td class="px-5 py-4">
              <div class="flex items-center justify-end gap-0.5">
                <button @click="openDetail(a)" title="Xem chi tiết"
                  class="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:text-espresso hover:bg-cream-deep transition-colors">
                  <Eye class="w-3.5 h-3.5" />
                </button>
                <button @click="openEdit(a)" title="Chỉnh sửa"
                  class="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:text-espresso hover:bg-cream-deep transition-colors">
                  <Pencil class="w-3.5 h-3.5" />
                </button>
                <button @click="openReset(a)" title="Đặt lại mật khẩu"
                  class="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:text-espresso hover:bg-cream-deep transition-colors">
                  <KeyRound class="w-3.5 h-3.5" />
                </button>
                <button @click="openLock(a)" :title="a.trangThaiHoatDong ? 'Khoá tài khoản' : 'Mở khoá'"
                  :class="['w-8 h-8 rounded-lg flex items-center justify-center transition-colors',
                    a.trangThaiHoatDong
                      ? 'text-muted-foreground hover:text-orange-500 hover:bg-orange-50'
                      : 'text-green-600 hover:bg-green-50']">
                  <Lock v-if="a.trangThaiHoatDong" class="w-3.5 h-3.5" />
                  <Unlock v-else class="w-3.5 h-3.5" />
                </button>
                <button @click="removeAccount(a)" title="Xoá tài khoản"
                  class="w-8 h-8 rounded-lg flex items-center justify-center text-muted-foreground hover:text-red-600 hover:bg-red-50 transition-colors">
                  <Trash2 class="w-3.5 h-3.5" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- ── Pagination ── -->
    <div v-if="totalPages > 1" class="flex items-center justify-between py-1">
      <p class="text-sm text-muted-foreground">
        <span class="font-medium text-espresso">{{ (currentPage - 1) * itemsPerPage + 1 }}–{{ Math.min(currentPage * itemsPerPage, filteredItems.length) }}</span>
        / {{ filteredItems.length }} tài khoản
      </p>
      <div class="flex items-center gap-1.5">
        <button @click="currentPage--" :disabled="currentPage === 1"
          class="w-9 h-9 rounded-xl border border-cream-deep flex items-center justify-center text-espresso disabled:opacity-40 hover:bg-cream transition-colors">
          <ChevronLeft class="w-4 h-4" />
        </button>
        <span class="text-sm font-semibold text-espresso px-2">{{ currentPage }} / {{ totalPages }}</span>
        <button @click="currentPage++" :disabled="currentPage === totalPages"
          class="w-9 h-9 rounded-xl border border-cream-deep flex items-center justify-center text-espresso disabled:opacity-40 hover:bg-cream transition-colors">
          <ChevronRight class="w-4 h-4" />
        </button>
      </div>
    </div>

    <!-- ── Modal Chi tiết tài khoản ── -->
    <Modal v-model="detailOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div :class="['w-12 h-12 rounded-xl text-cream font-bold text-base flex items-center justify-center shrink-0 shadow-sm', detailTarget ? avatarColor(detailTarget.maNhanVien) : 'bg-espresso']">
            {{ detailTarget ? initials(detailTarget.hoTen) : '' }}
          </div>
          <div>
            <h2 class="font-display text-xl font-bold text-espresso">{{ detailTarget?.hoTen }}</h2>
            <p class="text-sm text-muted-foreground flex items-center gap-1">
              <Mail class="w-3.5 h-3.5" /> {{ detailTarget?.email }}
            </p>
          </div>
        </div>
      </template>

      <div class="space-y-4">
        <!-- Vai trò + Trạng thái -->
        <div class="grid grid-cols-2 gap-3">
          <div class="rounded-xl border border-cream-deep bg-cream/30 px-4 py-3">
            <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1">Vai trò</p>
            <span class="inline-flex items-center px-2.5 py-1 rounded-lg text-xs font-semibold bg-caramel/10 text-caramel border border-caramel/20">
              {{ detailTarget?.tenVaiTro }}
            </span>
            <p class="text-[11px] text-muted-foreground mt-1 leading-tight">{{ detailTarget ? roleDesc(detailTarget.maVaiTro) : '' }}</p>
          </div>
          <div class="rounded-xl border border-cream-deep bg-cream/30 px-4 py-3">
            <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1">Trạng thái</p>
            <span :class="['inline-flex items-center gap-1.5 px-2.5 py-1 rounded-lg text-xs font-semibold border',
              detailTarget?.trangThaiHoatDong ? 'bg-green-50 text-green-700 border-green-200' : 'bg-red-50 text-red-600 border-red-200']">
              <span :class="['w-1.5 h-1.5 rounded-full', detailTarget?.trangThaiHoatDong ? 'bg-green-500' : 'bg-red-500']"></span>
              {{ detailTarget?.trangThaiHoatDong ? 'Hoạt động' : 'Đã khoá' }}
            </span>
          </div>
        </div>

        <!-- SĐT + Đăng nhập cuối -->
        <div class="grid grid-cols-2 gap-3">
          <div class="rounded-xl border border-cream-deep bg-cream/30 px-4 py-3">
            <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1.5">Số điện thoại</p>
            <p class="text-sm font-medium text-espresso flex items-center gap-1.5">
              <Phone class="w-3.5 h-3.5 text-muted-foreground" />
              {{ detailTarget?.soDienThoai || '—' }}
            </p>
          </div>
          <div class="rounded-xl border border-cream-deep bg-cream/30 px-4 py-3">
            <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1.5">Đăng nhập cuối</p>
            <p class="text-sm font-medium text-espresso flex items-center gap-1.5">
              <Clock class="w-3.5 h-3.5 text-muted-foreground" />
              {{ detailTarget?.lanDangNhapCuoi ? fmtDate(detailTarget.lanDangNhapCuoi) : 'Chưa đăng nhập' }}
            </p>
          </div>
        </div>

        <!-- Xác thực (Mật khẩu / PIN) -->
        <div class="rounded-xl border border-cream-deep bg-cream/30 px-4 py-3">
          <p class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-2">
            {{ detailTarget?.maVaiTro === 1 ? 'Mật khẩu đăng nhập (Web)' : 'Mã PIN đăng nhập (POS)' }}
          </p>
          <div class="flex items-center justify-between gap-3">
            <p class="text-sm font-mono tracking-widest text-espresso select-none">
              {{ showPwd ? (detailTarget?.maVaiTro === 1 ? '(Mật khẩu đã mã hoá bảo mật)' : '(Mã PIN đã mã hoá bảo mật)') : '••••••••••••' }}
            </p>
            <button @click="showPwd = !showPwd"
              class="flex items-center gap-1.5 text-xs font-semibold text-caramel hover:text-espresso transition-colors shrink-0">
              <component :is="showPwd ? EyeOff : Eye" class="w-3.5 h-3.5" />
              {{ showPwd ? 'Ẩn' : 'Xem' }}
            </button>
          </div>
          <p v-if="showPwd" class="text-[11px] text-muted-foreground mt-2 bg-amber-50 border border-amber-200 rounded-lg px-3 py-2">
            Vì lý do bảo mật, {{ detailTarget?.maVaiTro === 1 ? 'mật khẩu' : 'Mã PIN' }} được mã hoá một chiều (BCrypt) và không thể xem lại.
            Nếu nhân viên quên, vui lòng dùng chức năng <b>Đặt lại {{ detailTarget?.maVaiTro === 1 ? 'mật khẩu' : 'Mã PIN' }}</b>.
          </p>
        </div>
      </div>

      <template #footer>
        <Button variant="outline" @click="detailOpen = false" class="border-cream-deep rounded-xl text-espresso">Đóng</Button>
        <Button @click="detailOpen = false; openReset(detailTarget!)" class="bg-espresso text-cream rounded-xl gap-2">
          <KeyRound class="w-3.5 h-3.5" /> 
          Đặt lại {{ detailTarget?.maVaiTro === 1 ? 'mật khẩu' : 'Mã PIN' }}
        </Button>
      </template>
    </Modal>

    <!-- ── Modal Thêm / Sửa ── -->
    <Modal v-model="modalOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold">{{ editing ? 'Chỉnh sửa tài khoản' : 'Thêm tài khoản mới' }}</h2>
        <p class="text-sm text-muted-foreground">{{ editing ? 'Cập nhật thông tin và vai trò.' : 'Tạo tài khoản đăng nhập cho nhân viên.' }}</p>
      </template>
      <div class="space-y-4">
        <!-- Họ và tên -->
        <div>
          <div class="flex items-center justify-between mb-1.5">
            <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Họ và tên <span class="text-red-500">*</span></label>
            <span class="text-[11px] text-muted-foreground">{{ form.hoTen.length }}/100</span>
          </div>
          <Input v-model="form.hoTen" maxlength="100" placeholder="VD: Minh Nguyễn"
            @input="fieldErrors.hoTen = ''"
            :class="['bg-card h-10 rounded-xl', fieldErrors.hoTen ? 'border-red-400 focus-visible:ring-red-400/30' : 'border-cream-deep']" />
          <p v-if="fieldErrors.hoTen" class="flex items-center gap-1 mt-1.5 text-xs text-red-600 font-medium">
            <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ fieldErrors.hoTen }}
          </p>
        </div>
        <!-- Email -->
        <div>
          <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1.5 block">Email đăng nhập <span class="text-red-500">*</span></label>
          <Input v-model="form.email" type="email" placeholder="vd: minh@brew.vn"
            @input="fieldErrors.email = ''"
            :class="['bg-card h-10 rounded-xl', fieldErrors.email ? 'border-red-400 focus-visible:ring-red-400/30' : 'border-cream-deep']" />
          <p v-if="fieldErrors.email" class="flex items-center gap-1 mt-1.5 text-xs text-red-600 font-medium">
            <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ fieldErrors.email }}
          </p>
        </div>
        <!-- Vai trò + Trạng thái -->
        <div class="grid grid-cols-2 gap-3">
          <div>
            <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1.5 block">Vai trò <span class="text-red-500">*</span></label>
            <select v-model="form.maVaiTro" @change="fieldErrors.maVaiTro = ''"
              :class="['mt-0 w-full h-10 px-3 rounded-xl bg-card border text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/30',
                fieldErrors.maVaiTro ? 'border-red-400' : 'border-cream-deep']">
              <option :value="0" disabled>— Chọn vai trò —</option>
              <option v-for="r in roles" :key="r.maVaiTro" :value="r.maVaiTro">{{ r.tenVaiTro }}</option>
            </select>
            <p v-if="fieldErrors.maVaiTro" class="flex items-center gap-1 mt-1.5 text-xs text-red-600 font-medium">
              <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ fieldErrors.maVaiTro }}
            </p>
          </div>
          <div>
            <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1.5 block">Trạng thái</label>
            <select v-model="form.trangThaiHoatDong" class="mt-0 w-full h-10 px-3 rounded-xl bg-card border border-cream-deep text-sm text-espresso focus:outline-none focus:ring-2 focus:ring-caramel/30">
              <option :value="true">Hoạt động</option>
              <option :value="false">Đã khoá</option>
            </select>
          </div>
        </div>
        <!-- Số điện thoại -->
        <div>
          <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground mb-1.5 block">Số điện thoại <span class="text-muted-foreground font-normal normal-case">(tuỳ chọn)</span></label>
          <Input v-model="form.soDienThoai" placeholder="VD: 0901234567"
            @input="fieldErrors.soDienThoai = ''"
            :class="['bg-card h-10 rounded-xl', fieldErrors.soDienThoai ? 'border-red-400 focus-visible:ring-red-400/30' : 'border-cream-deep']" />
          <p v-if="fieldErrors.soDienThoai" class="flex items-center gap-1 mt-1.5 text-xs text-red-600 font-medium">
            <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ fieldErrors.soDienThoai }}
          </p>
        </div>
        <!-- Mật khẩu và PIN (chỉ khi tạo mới) -->
        <div v-if="!editing" class="mt-4">
          <div v-if="form.maVaiTro === 1">
            <div class="flex items-center justify-between mb-1.5">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Mật khẩu khởi tạo (Web) <span class="text-red-500">*</span></label>
            </div>
            <div class="relative">
              <Input v-model="form.matKhau" :type="showMatKhau ? 'text' : 'password'" maxlength="50"
                placeholder="Tối thiểu 6 ký tự"
                @input="fieldErrors.matKhau = ''"
                :class="['bg-card h-10 rounded-xl pr-10', fieldErrors.matKhau ? 'border-red-400 focus-visible:ring-red-400/30' : 'border-cream-deep']" />
              <button type="button" @click="showMatKhau = !showMatKhau"
                class="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-espresso transition-colors">
                <component :is="showMatKhau ? EyeOff : Eye" class="w-4 h-4" />
              </button>
            </div>
            <p v-if="fieldErrors.matKhau" class="flex items-center gap-1 mt-1.5 text-xs text-red-600 font-medium">
              <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ fieldErrors.matKhau }}
            </p>
          </div>
          <div v-else>
            <div class="flex items-center justify-between mb-1.5">
              <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Mã PIN khởi tạo (POS) <span class="text-red-500">*</span></label>
            </div>
            <Input v-model="form.pin" type="text" maxlength="6"
              placeholder="Đúng 6 chữ số"
              @input="fieldErrors.pin = ''; form.pin = form.pin.replace(/\D/g, '')"
              :class="['bg-card h-10 rounded-xl font-mono tracking-widest', fieldErrors.pin ? 'border-red-400 focus-visible:ring-red-400/30' : 'border-cream-deep']" />
            <p v-if="fieldErrors.pin" class="flex items-center gap-1 mt-1.5 text-xs text-red-600 font-medium">
              <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ fieldErrors.pin }}
            </p>
          </div>
        </div>
      </div>
      <template #footer>
        <Button variant="outline" @click="modalOpen = false" class="border-cream-deep rounded-xl text-espresso">Huỷ</Button>
        <Button @click="saveAccount" :disabled="saving" class="bg-espresso text-cream rounded-xl">
          {{ saving ? 'Đang lưu...' : (editing ? 'Lưu thay đổi' : 'Tạo tài khoản') }}
        </Button>
      </template>
    </Modal>

    <!-- ── Modal Đặt lại mật khẩu / PIN ── -->
    <Modal v-model="resetOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold">
          Đặt lại {{ resetTarget?.maVaiTro === 1 ? 'mật khẩu' : 'Mã PIN' }}
        </h2>
        <p class="text-sm text-muted-foreground">{{ resetTarget?.hoTen }} · {{ resetTarget?.email }}</p>
      </template>
      <div class="space-y-4">
        <!-- Đặt lại Mật khẩu (Web) -->
        <div v-if="resetTarget?.maVaiTro === 1">
          <div class="flex items-center justify-between mb-1.5">
            <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Mật khẩu mới (Web) <span class="text-red-500">*</span></label>
            <span class="text-[11px] text-muted-foreground">{{ newPassword.length }}/50</span>
          </div>
          <div class="relative">
            <Input v-model="newPassword" :type="showResetPwd ? 'text' : 'password'" maxlength="50"
              placeholder="Tối thiểu 6 ký tự"
              @input="resetError = ''"
              :class="['bg-card h-10 rounded-xl pr-10', resetError ? 'border-red-400' : 'border-cream-deep']" />
            <button type="button" @click="showResetPwd = !showResetPwd"
              class="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-espresso transition-colors">
              <component :is="showResetPwd ? EyeOff : Eye" class="w-4 h-4" />
            </button>
          </div>
        </div>

        <!-- Đặt lại Mã PIN (POS) -->
        <div v-else>
          <div class="flex items-center justify-between mb-1.5">
            <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">Mã PIN mới (POS) <span class="text-red-500">*</span></label>
          </div>
          <Input v-model="newPin" type="text" maxlength="6"
            placeholder="Đúng 6 số"
            @input="resetError = ''; newPin = newPin.replace(/\D/g, '')"
            :class="['bg-card h-10 rounded-xl font-mono tracking-widest', resetError ? 'border-red-400' : 'border-cream-deep']" />
        </div>

        <p v-if="resetError" class="flex items-center gap-1 mt-1 text-xs text-red-600 font-medium">
          <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ resetError }}
        </p>
      </div>
      <template #footer>
        <Button variant="outline" @click="resetOpen = false" class="border-cream-deep rounded-xl text-espresso">Huỷ</Button>
        <Button @click="doReset" :disabled="saving" class="bg-espresso text-cream rounded-xl">Đặt lại</Button>
      </template>
    </Modal>

    <!-- ── Modal Khoá / Mở khoá ── -->
    <Modal v-model="lockOpen">
      <template #header>
        <div class="flex items-center gap-3">
          <div :class="['w-11 h-11 rounded-xl flex items-center justify-center shrink-0',
            lockTarget?.trangThaiHoatDong ? 'bg-orange-50 border border-orange-200' : 'bg-green-50 border border-green-200']">
            <component :is="lockTarget?.trangThaiHoatDong ? Lock : Unlock"
              :class="['w-5 h-5', lockTarget?.trangThaiHoatDong ? 'text-orange-500' : 'text-green-600']" />
          </div>
          <div>
            <h2 class="font-display text-xl text-espresso font-bold">
              {{ lockTarget?.trangThaiHoatDong ? 'Khoá tài khoản' : 'Mở khoá tài khoản' }}
            </h2>
            <p class="text-sm text-muted-foreground">{{ lockTarget?.hoTen }} · {{ lockTarget?.email }}</p>
          </div>
        </div>
      </template>

      <div class="space-y-4">
        <!-- Cảnh báo -->
        <div :class="['flex items-start gap-3 rounded-xl px-4 py-3 text-sm',
          lockTarget?.trangThaiHoatDong
            ? 'bg-orange-50 border border-orange-200 text-orange-800'
            : 'bg-green-50 border border-green-200 text-green-800']">
          <AlertCircle class="w-4 h-4 shrink-0 mt-0.5" />
          <span v-if="lockTarget?.trangThaiHoatDong">
            Tài khoản bị khoá sẽ <b>không thể đăng nhập</b> cho đến khi được mở lại.
          </span>
          <span v-else>
            Tài khoản sẽ được <b>khôi phục quyền đăng nhập</b> ngay sau khi xác nhận.
          </span>
        </div>

        <!-- Lý do -->
        <div>
          <div class="flex items-center justify-between mb-1.5">
            <label class="text-[11px] font-bold uppercase tracking-wide text-muted-foreground">
              Lý do {{ lockTarget?.trangThaiHoatDong ? 'khoá' : 'mở khoá' }} <span class="text-red-500">*</span>
            </label>
            <span class="text-[11px] text-muted-foreground">{{ lockReason.length }}/200</span>
          </div>
          <textarea v-model="lockReason" maxlength="200" rows="3"
            @input="lockReasonError = ''"
            :placeholder="lockTarget?.trangThaiHoatDong
              ? 'VD: Nhân viên nghỉ việc, vi phạm nội quy...'
              : 'VD: Nhân viên đã quay lại làm việc...'"
            :class="['w-full rounded-xl border px-3 py-2.5 text-sm text-espresso resize-none focus:outline-none focus:ring-2 transition-colors',
              lockReasonError
                ? 'border-red-400 focus:ring-red-400/30'
                : 'border-cream-deep bg-card focus:ring-caramel/30']" />
          <p v-if="lockReasonError" class="flex items-center gap-1 mt-1.5 text-xs text-red-600 font-medium">
            <AlertCircle class="w-3.5 h-3.5 shrink-0" /> {{ lockReasonError }}
          </p>
        </div>
      </div>

      <template #footer>
        <Button variant="outline" @click="lockOpen = false" class="border-cream-deep rounded-xl text-espresso">Huỷ</Button>
        <Button @click="doToggleLock" :disabled="lockSaving"
          :class="['rounded-xl text-cream', lockTarget?.trangThaiHoatDong ? 'bg-orange-500 hover:bg-orange-600' : 'bg-green-600 hover:bg-green-700']">
          <component :is="lockTarget?.trangThaiHoatDong ? Lock : Unlock" class="w-3.5 h-3.5" />
          {{ lockSaving ? 'Đang xử lý...' : (lockTarget?.trangThaiHoatDong ? 'Khoá tài khoản' : 'Mở khoá') }}
        </Button>
      </template>
    </Modal>

    <!-- ── Modal Xoá ── -->
    <Modal v-model="confirmOpen">
      <template #header>
        <h2 class="font-display text-xl text-espresso font-bold">Xoá tài khoản</h2>
      </template>
      <p class="text-sm text-espresso/80">
        Bạn chắc chắn muốn xoá tài khoản <b>{{ delTarget?.hoTen }}</b>?
        Hành động này không thể hoàn tác.
      </p>
      <template #footer>
        <Button variant="outline" @click="confirmOpen = false" class="border-cream-deep rounded-xl text-espresso">Huỷ</Button>
        <Button @click="doRemove" :disabled="saving" class="bg-red-600 text-cream rounded-xl">Xoá</Button>
      </template>
    </Modal>

  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { Plus, Mail, Search, ChevronLeft, ChevronRight, Pencil, Trash2, Lock, Unlock, KeyRound, Clock, ShieldCheck, Eye, EyeOff, Phone, UserCircle2, AlertCircle } from 'lucide-vue-next'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Modal from '@/components/ui/Modal.vue'
import { accountsApi, rolesApi, type Account, type RoleDetail } from '@/services/accounts'

const accounts = ref<Account[]>([])
const roles = ref<RoleDetail[]>([])
const loading = ref(false)
const errorMsg = ref('')

async function loadAll() {
  loading.value = true
  errorMsg.value = ''
  try {
    const [a, r] = await Promise.all([accountsApi.list(), rolesApi.list()])
    accounts.value = a
    roles.value = r
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không tải được dữ liệu.'
  } finally {
    loading.value = false
  }
}
onMounted(loadAll)

const search = ref('')
const roleFilter = ref<number | 'all'>('all')
const currentPage = ref(1)
const itemsPerPage = ref(8)

const activeCount = computed(() => accounts.value.filter(a => a.trangThaiHoatDong).length)
const lockedCount = computed(() => accounts.value.filter(a => !a.trangThaiHoatDong).length)
const roleDesc = (maVaiTro: number) => roles.value.find(r => r.maVaiTro === maVaiTro)?.moTa || ''

const palette = ['bg-caramel', 'bg-sage', 'bg-brown', 'bg-espresso']
const avatarColor = (id: number) => palette[id % palette.length]
const initials = (name: string) => name.trim().split(/\s+/).slice(-2).map(w => w[0]?.toUpperCase() ?? '').join('') || '?'
const fmtDate = (iso: string) => new Date(iso).toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' })

const filteredItems = computed(() => accounts.value.filter(a => {
  const matchRole = roleFilter.value === 'all' || a.maVaiTro === roleFilter.value
  const q = search.value.toLowerCase()
  const matchSearch = a.hoTen.toLowerCase().includes(q) || a.email.toLowerCase().includes(q)
  return matchRole && matchSearch
}))
const totalPages = computed(() => Math.ceil(filteredItems.value.length / itemsPerPage.value) || 1)
const paginatedItems = computed(() => {
  const s = (currentPage.value - 1) * itemsPerPage.value
  return filteredItems.value.slice(s, s + itemsPerPage.value)
})
watch([search, roleFilter], () => { currentPage.value = 1 })

// ── Thêm / sửa ──
const modalOpen = ref(false)
const editing = ref<Account | null>(null)
const saving = ref(false)
const fieldErrors = ref({ hoTen: '', email: '', maVaiTro: '', soDienThoai: '', matKhau: '', pin: '' })
const showMatKhau = ref(false)
const showResetPwd = ref(false)
const form = ref<{ hoTen: string; email: string; maVaiTro: number; trangThaiHoatDong: boolean; matKhau: string; pin: string; soDienThoai: string }>(
  { hoTen: '', email: '', maVaiTro: 0, trangThaiHoatDong: true, matKhau: '', pin: '', soDienThoai: '' })

function openAdd() {
  editing.value = null
  form.value = { hoTen: '', email: '', maVaiTro: roles.value[0]?.maVaiTro ?? 0, trangThaiHoatDong: true, matKhau: '', pin: '', soDienThoai: '' }
  fieldErrors.value = { hoTen: '', email: '', maVaiTro: '', soDienThoai: '', matKhau: '', pin: '' }
  showMatKhau.value = false
  modalOpen.value = true
}
function openEdit(a: Account) {
  editing.value = a
  form.value = { hoTen: a.hoTen, email: a.email, maVaiTro: a.maVaiTro, trangThaiHoatDong: a.trangThaiHoatDong, matKhau: '', pin: '', soDienThoai: a.soDienThoai || '' }
  fieldErrors.value = { hoTen: '', email: '', maVaiTro: '', soDienThoai: '', matKhau: '', pin: '' }
  showMatKhau.value = false
  modalOpen.value = true
}

function validate(): boolean {
  fieldErrors.value = { hoTen: '', email: '', maVaiTro: '', soDienThoai: '', matKhau: '', pin: '' }
  let ok = true
  const ten = form.value.hoTen.trim()
  if (!ten) { fieldErrors.value.hoTen = 'Vui lòng nhập họ và tên.'; ok = false }
  else if (ten.length < 2) { fieldErrors.value.hoTen = 'Họ và tên tối thiểu 2 ký tự.'; ok = false }
  else if (ten.length > 100) { fieldErrors.value.hoTen = 'Họ và tên tối đa 100 ký tự.'; ok = false }
  const email = form.value.email.trim()
  if (!email) { fieldErrors.value.email = 'Vui lòng nhập email đăng nhập.'; ok = false }
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) { fieldErrors.value.email = 'Email không đúng định dạng.'; ok = false }
  else if (email.length > 100) { fieldErrors.value.email = 'Email tối đa 100 ký tự.'; ok = false }
  if (!form.value.maVaiTro) { fieldErrors.value.maVaiTro = 'Vui lòng chọn vai trò.'; ok = false }
  const sdt = form.value.soDienThoai.trim()
  if (sdt && !/^0[35789]\d{8}$/.test(sdt)) { fieldErrors.value.soDienThoai = 'Số điện thoại không hợp lệ (10 số, bắt đầu 03/05/07/08/09).'; ok = false }
  if (!editing.value) {
    if (form.value.maVaiTro === 1) {
      const pwd = form.value.matKhau
      if (!pwd) { fieldErrors.value.matKhau = 'Vui lòng nhập mật khẩu.'; ok = false }
      else if (pwd.length < 6) { fieldErrors.value.matKhau = 'Mật khẩu tối thiểu 6 ký tự.'; ok = false }
      else if (pwd.length > 50) { fieldErrors.value.matKhau = 'Mật khẩu tối đa 50 ký tự.'; ok = false }
      form.value.pin = '' // Quản lý không dùng PIN POS
    } else {
      // Dummy web password for non-admin roles to satisfy backend requirement
      form.value.matKhau = form.value.pin || '123456'
      const pin = form.value.pin
      if (!pin) { fieldErrors.value.pin = 'Vui lòng nhập Mã PIN.'; ok = false }
      else if (!/^\d{6}$/.test(pin)) { fieldErrors.value.pin = 'Mã PIN phải đúng 6 số.'; ok = false }
    }
  }
  return ok
}

async function saveAccount() {
  if (!validate()) return
  saving.value = true
  try {
    if (editing.value) {
      await accountsApi.update(editing.value.maNhanVien, {
        hoTen: form.value.hoTen.trim(), email: form.value.email.trim(), maVaiTro: form.value.maVaiTro,
        trangThaiHoatDong: form.value.trangThaiHoatDong, soDienThoai: form.value.soDienThoai.trim() || null,
      })
    } else {
      await accountsApi.create({
        hoTen: form.value.hoTen.trim(), email: form.value.email.trim(), maVaiTro: form.value.maVaiTro,
        matKhau: form.value.matKhau, pin: form.value.pin.trim() || null, soDienThoai: form.value.soDienThoai.trim() || null,
      })
    }
    modalOpen.value = false
    await loadAll()
  } catch (e) {
    fieldErrors.value.email = e instanceof Error ? e.message : 'Không lưu được tài khoản.'
  } finally {
    saving.value = false
  }
}

// ── Khoá / Mở khoá có xác nhận ──
const lockOpen = ref(false)
const lockTarget = ref<Account | null>(null)
const lockReason = ref('')
const lockReasonError = ref('')
const lockSaving = ref(false)

function openLock(a: Account) {
  lockTarget.value = a
  lockReason.value = ''
  lockReasonError.value = ''
  lockOpen.value = true
}

async function doToggleLock() {
  lockReasonError.value = ''
  if (!lockReason.value.trim()) { lockReasonError.value = 'Vui lòng nhập lý do.'; return }
  lockSaving.value = true
  try {
    const res = await accountsApi.toggleLock(lockTarget.value!.maNhanVien)
    lockTarget.value!.trangThaiHoatDong = res.trangThaiHoatDong
    lockOpen.value = false
  } catch (e) {
    lockReasonError.value = e instanceof Error ? e.message : 'Không đổi được trạng thái.'
  } finally {
    lockSaving.value = false
  }
}

// ── Xem chi tiết ──
const detailOpen = ref(false)
const detailTarget = ref<Account | null>(null)
const showPwd = ref(false)
function openDetail(a: Account) { detailTarget.value = a; showPwd.value = false; detailOpen.value = true }

// ── Đặt lại mật khẩu ──
const resetOpen = ref(false)
const resetTarget = ref<Account | null>(null)
const newPassword = ref('')
const newPin = ref('')
const resetError = ref('')
function openReset(a: Account) { resetTarget.value = a; newPassword.value = ''; newPin.value = ''; resetError.value = ''; showResetPwd.value = false; resetOpen.value = true }
async function doReset() {
  resetError.value = ''
  const pwd = newPassword.value
  const pin = newPin.value

  if (resetTarget.value!.maVaiTro === 1) {
    if (!pwd) { resetError.value = 'Vui lòng nhập Mật khẩu mới.'; return }
    if (pwd.length < 6) { resetError.value = 'Mật khẩu phải từ 6 ký tự.'; return }
  } else {
    if (!pin) { resetError.value = 'Vui lòng nhập Mã PIN mới.'; return }
    if (pin.length !== 6) { resetError.value = 'Mã PIN phải đúng 6 số.'; return }
  }

  saving.value = true
  try {
    const p: Promise<any>[] = []
    if (resetTarget.value!.maVaiTro === 1) {
      p.push(accountsApi.resetPassword(resetTarget.value!.maNhanVien, pwd))
    } else {
      p.push(accountsApi.setPin(resetTarget.value!.maNhanVien, pin))
      p.push(accountsApi.resetPassword(resetTarget.value!.maNhanVien, pin)) // Sync web password with PIN just in case
    }
    
    await Promise.all(p)
    resetOpen.value = false
  } catch (e) {
    resetError.value = e instanceof Error ? e.message : 'Không lưu được thay đổi.'
  } finally { saving.value = false }
}

// ── Xoá ──
const confirmOpen = ref(false)
const delTarget = ref<Account | null>(null)
function removeAccount(a: Account) { delTarget.value = a; confirmOpen.value = true }
async function doRemove() {
  saving.value = true
  try {
    await accountsApi.remove(delTarget.value!.maNhanVien)
    confirmOpen.value = false
    await loadAll()
  } catch (e) {
    errorMsg.value = e instanceof Error ? e.message : 'Không xoá được tài khoản.'
    confirmOpen.value = false
  } finally { saving.value = false }
}
</script>
