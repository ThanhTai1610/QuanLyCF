<template>
  <div class="space-y-6 font-premium-sans text-[#2A231E]">

    <!-- Header -->
    <div class="flex items-start justify-between gap-4">
      <div>
        <h2 class="text-3xl font-premium-serif font-bold text-[#2A231E]">Nhật ký Hệ thống & Lưu vết</h2>
        <p class="text-[10px] uppercase tracking-[0.2em] text-[#8A8178] font-bold mt-2">Theo dõi mọi hoạt động và thay đổi trong hệ thống</p>
      </div>
      <button
        class="shrink-0 w-10 h-10 rounded-full bg-red-50 hover:bg-red-100 border border-red-100 flex items-center justify-center transition-colors text-red-500 hover:text-red-600 shadow-sm"
        title="Xóa nhật ký (Chỉ Admin)"
      >
        <Trash2 class="w-4 h-4" stroke-width="2.5" />
      </button>
    </div>

    <!-- Stats Row -->
    <div class="grid grid-cols-2 gap-4">
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-5 flex items-center gap-4">
        <div class="w-10 h-10 shrink-0 rounded-full bg-[#F9F8F6] border border-[#EAE3D9] text-[#5C544E] flex items-center justify-center">
          <Activity class="w-4 h-4" stroke-width="2.5" />
        </div>
        <div>
          <p class="text-[10px] uppercase tracking-[0.15em] text-[#8A8178] font-bold">Tổng bản ghi</p>
          <p class="text-2xl font-premium-serif font-bold text-[#2A231E] mt-0.5">3</p>
        </div>
      </div>
      <div class="bg-white rounded-xl border border-amber-100 shadow-sm p-5 flex items-center gap-4">
        <div class="w-10 h-10 shrink-0 rounded-full bg-amber-50 border border-amber-100 text-amber-500 flex items-center justify-center">
          <AlertTriangle class="w-4 h-4" stroke-width="2.5" />
        </div>
        <div>
          <p class="text-[10px] uppercase tracking-[0.15em] text-[#CC8033] font-bold">Cảnh báo</p>
          <p class="text-2xl font-premium-serif font-bold text-[#CC8033] mt-0.5">2</p>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm p-6">
      <h3 class="font-premium-serif text-lg font-bold text-[#2A231E] mb-5">Bộ lọc tìm kiếm</h3>
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div class="space-y-2">
          <label class="block text-[10px] font-bold text-[#8A8178] uppercase tracking-[0.15em]">Tìm kiếm nhanh</label>
          <div class="relative">
            <Search class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-[#C5BEB8]" stroke-width="2.5" />
            <input
              type="text"
              placeholder="Tên tài khoản, IP..."
              class="w-full pl-9 pr-3 py-2.5 bg-white border border-[#EAE3D9] rounded-lg text-sm text-[#2A231E] font-medium focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033]/20 transition-colors"
            />
          </div>
        </div>
        <div class="space-y-2">
          <label class="block text-[10px] font-bold text-[#8A8178] uppercase tracking-[0.15em]">Phân hệ</label>
          <select class="w-full px-3 py-2.5 bg-white border border-[#EAE3D9] rounded-lg text-sm text-[#2A231E] font-semibold focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033]/20 transition-colors">
            <option>Tất cả</option>
            <option>Hóa đơn</option>
            <option>Kho & Kiểm kê</option>
            <option>Nhân sự</option>
            <option>Hệ thống</option>
          </select>
        </div>
        <div class="space-y-2">
          <label class="block text-[10px] font-bold text-[#8A8178] uppercase tracking-[0.15em]">Mức độ</label>
          <select class="w-full px-3 py-2.5 bg-white border border-[#EAE3D9] rounded-lg text-sm text-[#2A231E] font-semibold focus:outline-none focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033]/20 transition-colors">
            <option>Tất cả</option>
            <option>Bình thường</option>
            <option>Cảnh báo</option>
            <option>Nghiêm trọng</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Table -->
    <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-[#F9F8F6] text-[#8A8178] text-[10px] uppercase tracking-[0.15em] border-b border-[#EAE3D9]">
              <th class="px-6 py-5 font-bold whitespace-nowrap">Thời gian</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Tài khoản</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Phân hệ</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Hành động</th>
              <th class="px-6 py-5 font-bold whitespace-nowrap">Thiết bị/IP</th>
              <th class="px-6 py-5 font-bold">Chi tiết</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(log, idx) in logs"
              :key="idx"
              class="border-b border-[#EAE3D9]/60 hover:bg-[#F5F2ED] transition-colors"
            >
              <td class="px-6 py-5 text-[#8A8178] font-bold text-xs font-mono whitespace-nowrap">{{ log.time }}</td>
              <td class="px-6 py-5 font-bold text-[#2A231E] whitespace-nowrap">{{ log.account }}</td>
              <td class="px-6 py-5">
                <span class="inline-block px-2.5 py-1.5 bg-[#F5F2ED] text-[#8A6D53] border border-[#EAE3D9] rounded-md text-[10px] font-bold uppercase tracking-widest whitespace-nowrap">
                  {{ log.module }}
                </span>
              </td>
              <td class="px-6 py-5 font-bold text-[#5C544E] whitespace-nowrap">{{ log.action }}</td>
              <td class="px-6 py-5 text-[#8A8178] font-mono text-[11px] font-semibold whitespace-nowrap">{{ log.ip }}</td>
              <td class="px-6 py-5 text-[#5C544E] font-medium leading-relaxed">{{ log.details }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { Trash2, Search, Activity, AlertTriangle } from 'lucide-vue-next'

const logs = [
  {
    time: '11:42:15',
    account: 'Lan Trần (Thu ngân)',
    module: 'Hóa đơn',
    action: 'Xóa Hóa Đơn',
    ip: '192.168.1.25',
    details: 'Xóa hóa đơn #HD-9082 bàn số 5 sau khi in tạm tính (Xem đối soát)'
  },
  {
    time: '09:15:34',
    account: 'Khoa Phạm (Kho)',
    module: 'Kho & Kiểm kê',
    action: 'Sửa Kiểm Kê',
    ip: '192.168.1.12',
    details: 'Điều chỉnh giảm 30kg Cà phê hạt dòng Robusta (Xem đối soát)'
  },
  {
    time: '07:30:00',
    account: 'Minh Nguyễn (Quản lý)',
    module: 'Hệ thống',
    action: 'Đăng nhập',
    ip: '115.79.42.8',
    details: 'Đăng nhập thành công thiết bị Manager-PC'
  }
]
</script>

<style scoped>
select {
  background-image: url("data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='none' stroke='%238A8178' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3e%3cpolyline points='6 9 12 15 18 9'%3e%3c/polyline%3e%3c/svg%3e");
  background-repeat: no-repeat;
  background-position: right 0.75rem center;
  background-size: 1em;
}
</style>
