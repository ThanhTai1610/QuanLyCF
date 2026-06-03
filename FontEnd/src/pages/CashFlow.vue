<template>
  <div class="space-y-6 font-premium-sans text-[#2A231E]">
    <!-- Header -->
    <div class="flex flex-col md:flex-row md:items-center justify-between gap-4">
      <div>
        <h2 class="text-3xl font-premium-serif font-bold text-[#2A231E]">Dòng tiền & Chi phí</h2>
        <p class="text-[11px] uppercase tracking-[0.15em] text-[#8A8178] font-bold mt-2">Tổng hợp và theo dõi chi phí phát sinh trong tháng này.</p>
      </div>
      <div class="flex items-center gap-3">
        <select class="border border-[#EAE3D9] rounded-lg px-4 py-2 text-sm bg-[#FDFDFD] text-[#2A231E] font-bold focus:outline-none focus:border-[#CC8033] shadow-sm">
          <option>Tháng 06 / 2026</option>
          <option>Tháng 05 / 2026</option>
        </select>
        <button @click="isCreateModalOpen = true" class="px-5 py-2.5 bg-[#CC8033] hover:bg-[#B3702C] text-white rounded-lg text-sm font-bold transition-colors shadow-md flex items-center gap-2">
          <Plus class="w-4 h-4" stroke-width="2.5" /> Tạo phiếu chi
        </button>
      </div>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-[#FDFDFD] p-6 rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)]">
        <div class="flex justify-between items-start">
          <h3 class="text-sm font-bold text-[#2A231E]">Tổng Thu Dòng Tiền</h3>
          <span class="text-[10px] font-bold px-2 py-1 bg-emerald-50 text-emerald-600 rounded flex items-center gap-1 border border-emerald-100">
            <TrendingUp class="w-3 h-3" stroke-width="2.5" /> Trend
          </span>
        </div>
        <p class="text-[28px] font-premium-serif font-bold text-emerald-600 mt-4">+ 1,200,000đ</p>
      </div>
      
      <div class="bg-[#FDFDFD] p-6 rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)]">
        <div class="flex justify-between items-start">
          <h3 class="text-sm font-bold text-[#2A231E]">Tổng Chi Vận Hành</h3>
          <span class="text-[10px] font-bold px-2 py-1 bg-red-50 text-red-600 rounded flex items-center gap-1 border border-red-100">
            <TrendingDown class="w-3 h-3" stroke-width="2.5" /> Trend
          </span>
        </div>
        <p class="text-[28px] font-premium-serif font-bold text-red-600 mt-4">- 12,200,000đ</p>
        <p class="text-xs text-[#8A8178] font-semibold mt-2 tracking-wide">Lương: 15.2tr • Kho: 12.8tr • Khác: 7.4tr</p>
      </div>

      <div class="bg-[#FDFDFD] p-6 rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)]">
        <div class="flex justify-between items-start">
          <h3 class="text-sm font-bold text-[#2A231E]">Dòng Tiền Thuần</h3>
          <span class="text-[10px] font-bold px-2 py-1 bg-[#F5F2ED] text-[#8A6D53] rounded flex items-center gap-1 border border-[#EAE3D9]">
            <TrendingUp class="w-3 h-3" stroke-width="2.5" /> Trend
          </span>
        </div>
        <p class="text-[28px] font-premium-serif font-bold text-[#8A6D53] mt-4">+ 3,400,000đ</p>
      </div>
    </div>

    <!-- Charts -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <div class="lg:col-span-2 bg-[#FDFDFD] rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-6 md:p-8">
        <h3 class="font-premium-serif text-2xl font-bold text-[#2A231E] mb-6">Xu hướng Thu - Chi 2026</h3>
        <div class="w-full h-[300px]">
          <!-- @ts-ignore -->
          <Bar :data="barChartData" :options="barChartOptions" />
        </div>
      </div>
      <div class="bg-[#FDFDFD] rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] p-6 md:p-8 flex flex-col">
        <h3 class="font-premium-serif text-2xl font-bold text-[#2A231E] mb-6">Cơ cấu Chi phí tháng này</h3>
        <div class="flex-1 min-h-[220px] flex items-center justify-center">
          <!-- @ts-ignore -->
          <Doughnut :data="doughnutChartData" :options="doughnutOptions" />
        </div>
        <div class="grid grid-cols-2 gap-y-3 gap-x-2 mt-8 text-[11px] font-bold text-[#5C544E] uppercase tracking-wider">
          <div class="flex items-center gap-2"><span class="w-2.5 h-2.5 rounded-full bg-[#5C4533]"></span> Lương nv <span class="text-[#2A231E] ml-auto">43%</span></div>
          <div class="flex items-center gap-2"><span class="w-2.5 h-2.5 rounded-full bg-[#8A6D53]"></span> Nguyên liệu <span class="text-[#2A231E] ml-auto">36%</span></div>
          <div class="flex items-center gap-2"><span class="w-2.5 h-2.5 rounded-full bg-[#C1A081]"></span> Mặt bằng <span class="text-[#2A231E] ml-auto">15%</span></div>
          <div class="flex items-center gap-2"><span class="w-2.5 h-2.5 rounded-full bg-[#E5D5C5]"></span> Điện nước <span class="text-[#2A231E] ml-auto">6%</span></div>
        </div>
      </div>
    </div>

    <!-- Table -->
    <div class="bg-[#FDFDFD] rounded-2xl border border-[#EAE3D9] shadow-[0_8px_30px_rgb(0,0,0,0.04)] overflow-hidden">
      <div class="p-6 md:p-8 border-b border-[#EAE3D9]">
        <h3 class="font-premium-serif text-2xl font-bold text-[#2A231E]">Sổ nhật ký chi tiêu chi tiết</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full text-sm text-left">
          <thead>
            <tr class="bg-[#F9F8F6] text-[#8A8178] text-[10px] uppercase tracking-[0.15em] border-b border-[#EAE3D9]">
              <th class="px-8 py-5 font-bold">ID</th>
              <th class="px-6 py-5 font-bold">TAG</th>
              <th class="px-6 py-5 font-bold">CHI PHÍ TRONG</th>
              <th class="px-6 py-5 font-bold">TRẠNG THÁI</th>
              <th class="px-6 py-5 font-bold">CHỜ DUYỆT</th>
              <th class="px-8 py-5 font-bold text-center">THAO TÁC</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, i) in tableData" :key="i" class="border-b border-[#EAE3D9]/60 hover:bg-[#F5F2ED] transition-colors group">
              <td class="px-8 py-5 font-bold text-[#2A231E]">{{ item.id }}</td>
              <td class="px-6 py-5 flex gap-2">
                <span v-for="(tag, tIdx) in item.tags" :key="tIdx" class="px-2.5 py-1.5 bg-[#F5F2ED] text-[#8A6D53] border border-[#EAE3D9] rounded-md text-[10px] font-bold uppercase tracking-wider">
                  {{ tag }}
                </span>
              </td>
              <td class="px-6 py-5 text-[#5C544E] font-bold">{{ item.month }}</td>
              <td class="px-6 py-5">
                <span :class="item.status === 'Đã chi' ? 'bg-emerald-50 text-emerald-600 border-emerald-100' : 'bg-red-50 text-red-600 border-red-100'" class="px-3 py-1.5 border rounded-full text-[10px] uppercase tracking-wider font-bold">
                  {{ item.status }}
                </span>
              </td>
              <td class="px-6 py-5">
                <span class="px-3 py-1.5 bg-amber-50 text-amber-600 border border-amber-100 rounded-full text-[10px] uppercase tracking-wider font-bold">
                  {{ item.approval }}
                </span>
              </td>
              <td class="px-8 py-5 text-center">
                <button class="p-2 text-[#8A8178] hover:text-[#2A231E] hover:bg-black/5 rounded-lg transition-colors inline-flex">
                  <Eye class="w-4 h-4" />
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Slide-over Modal: Tạo phiếu chi -->
    <div v-if="isCreateModalOpen" class="fixed inset-0 z-[100] flex justify-end">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-[#2A231E]/40 backdrop-blur-sm transition-opacity" @click="isCreateModalOpen = false"></div>
      
      <!-- Panel -->
      <div class="relative w-full max-w-lg bg-[#FDFBF7] h-full shadow-2xl flex flex-col animate-in slide-in-from-right duration-300">
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-5 border-b border-[#EAE3D9] bg-white">
          <h2 class="text-xl font-premium-serif font-bold text-[#2A231E]">Tạo Phiếu Chi Mới</h2>
          <button @click="isCreateModalOpen = false" class="p-2 hover:bg-[#F5F2ED] rounded-full transition-colors text-[#8A8178]">
            <X class="w-5 h-5" />
          </button>
        </div>

        <!-- Body -->
        <div class="flex-1 overflow-y-auto p-6 space-y-8">
          <!-- 1. Metadata -->
          <div class="bg-white p-4 rounded-xl border border-[#EAE3D9] grid grid-cols-2 gap-4">
            <div>
              <label class="block text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mb-1.5">Mã phiếu chi</label>
              <div class="font-bold text-[#2A231E] tracking-wider">PC-20260603-001</div>
            </div>
            <div>
              <label class="block text-[10px] uppercase tracking-widest text-[#8A8178] font-bold mb-1.5">Thời gian tạo</label>
              <div class="font-bold text-[#2A231E] tracking-wider">{{ new Date().toLocaleString('vi-VN') }}</div>
            </div>
          </div>

          <!-- 2. Phân loại -->
          <div class="space-y-4">
            <h3 class="text-sm font-bold text-[#8A6D53] border-b border-[#EAE3D9] pb-2 uppercase tracking-widest">Phân loại & Phương thức</h3>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Danh mục chi *</label>
                <select class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold text-[#2A231E] focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] outline-none">
                  <option>Nhập kho nguyên liệu</option>
                  <option>Lương nhân sự</option>
                  <option>Chi phí mặt bằng</option>
                  <option>Điện nước</option>
                  <option>Internet</option>
                  <option>Chi phí phát sinh ngoài</option>
                </select>
              </div>
              <div>
                <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Hình thức thanh toán *</label>
                <select class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold text-[#2A231E] focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] outline-none">
                  <option>Tiền mặt</option>
                  <option>Chuyển khoản / Ví ĐT</option>
                </select>
              </div>
            </div>
          </div>

          <!-- 3. Financials -->
          <div class="space-y-4">
            <h3 class="text-sm font-bold text-[#8A6D53] border-b border-[#EAE3D9] pb-2 uppercase tracking-widest">Thông tin tài chính</h3>
            
            <div>
              <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Số tiền chi (VNĐ) *</label>
              <input type="number" placeholder="Ví dụ: 500000" class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2.5 text-base font-bold bg-white text-[#CC8033] focus:border-[#CC8033] focus:ring-1 focus:ring-[#CC8033] outline-none" />
            </div>

            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Người thực hiện</label>
                <input type="text" value="Quản lý" readonly class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-[#F5F2ED] text-[#8A8178] font-semibold outline-none cursor-not-allowed" />
              </div>
              <div>
                <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Người nhận tiền *</label>
                <input type="text" placeholder="Tên cá nhân/NCC" class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white font-semibold focus:border-[#CC8033] outline-none" />
              </div>
            </div>

            <div>
              <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Lý do chi chi tiết *</label>
              <textarea rows="3" placeholder="Mô tả ngắn gọn lý do (VD: Chi mua 5 bao đá bi ca sáng)..." class="w-full border border-[#EAE3D9] rounded-lg px-3 py-2 text-sm bg-white focus:border-[#CC8033] font-medium outline-none resize-none"></textarea>
            </div>
          </div>

          <!-- 4. Workflow -->
          <div class="space-y-4 pb-4">
            <h3 class="text-sm font-bold text-[#8A6D53] border-b border-[#EAE3D9] pb-2 uppercase tracking-widest">Luồng phê duyệt & Chứng từ</h3>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Trạng thái phiếu</label>
                <select class="w-full border border-amber-200 rounded-lg px-3 py-2 text-sm bg-amber-50 text-amber-700 font-bold focus:outline-none">
                  <option value="Chờ duyệt">Chờ duyệt</option>
                  <option value="Đã chi">Đã chi</option>
                  <option value="Đã hủy">Đã hủy</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-[11px] font-bold text-[#5C544E] mb-1.5">Minh chứng đính kèm</label>
              <div class="border-2 border-dashed border-[#EAE3D9] rounded-xl p-6 flex flex-col items-center justify-center text-center hover:bg-[#FDFBF7] hover:border-[#CC8033] transition-colors cursor-pointer group bg-white">
                <UploadCloud class="w-8 h-8 text-[#C5BEB8] group-hover:text-[#CC8033] mb-2 transition-colors" stroke-width="1.5" />
                <span class="text-sm font-bold text-[#8A8178] group-hover:text-[#5C544E]">Nhấn để tải ảnh hóa đơn/biên nhận</span>
                <span class="text-[10px] text-[#C5BEB8] font-medium mt-1">Hỗ trợ JPG, PNG, PDF (Max 5MB)</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer -->
        <div class="p-5 border-t border-[#EAE3D9] bg-white flex justify-end gap-3 shadow-[0_-10px_20px_rgba(0,0,0,0.02)]">
          <button @click="isCreateModalOpen = false" class="px-5 py-2.5 rounded-lg text-sm font-bold text-[#5C544E] hover:bg-[#F5F2ED] transition-colors">
            Hủy bỏ
          </button>
          <button @click="isCreateModalOpen = false" class="px-6 py-2.5 rounded-lg text-sm font-bold bg-[#CC8033] hover:bg-[#B3702C] text-white shadow-md transition-colors flex items-center gap-2">
            <Check class="w-4 h-4" stroke-width="2.5" /> Lưu phiếu chi
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { TrendingUp, TrendingDown, Eye, Plus, X, UploadCloud, Check } from 'lucide-vue-next'
import { Bar, Doughnut } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  Title,
  Tooltip,
  Legend
)

const isCreateModalOpen = ref(false)

const barChartData = {
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  datasets: [
    {
      label: 'Tổng Thu',
      data: [6.8, 7.3, 8.5, 9.1, 10, 11.2, 11, 11.8, 11.5, 13.2, 16.5, 14],
      backgroundColor: '#5C4533',
      borderRadius: 4,
      barPercentage: 0.6,
      categoryPercentage: 0.8
    },
    {
      label: 'Tổng Chi',
      data: [4.4, 4, 5.5, 7.2, 6.5, 5, 6.8, 7.8, 7.5, 9, 10.5, 9.5],
      backgroundColor: '#C1A081',
      borderRadius: 4,
      barPercentage: 0.6,
      categoryPercentage: 0.8
    }
  ]
}

const barChartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false },
    tooltip: {
      mode: 'index',
      intersect: false,
      backgroundColor: '#2A231E',
      padding: 12,
      titleFont: { size: 13, family: 'Montserrat' },
      bodyFont: { size: 12, family: 'Montserrat' },
      cornerRadius: 8
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: { color: '#EAE3D9', drawBorder: false },
      ticks: { color: '#8A8178', font: { family: 'Montserrat', size: 10, weight: 'bold' } }
    },
    x: {
      grid: { display: false },
      ticks: { color: '#8A8178', font: { family: 'Montserrat', size: 10, weight: 'bold' } }
    }
  }
}

const doughnutChartData = {
  labels: ['Lương nhân viên', 'Nguyên liệu', 'Mặt bằng', 'Điện nước'],
  datasets: [{
    data: [43, 36, 15, 6],
    backgroundColor: ['#5C4533', '#8A6D53', '#C1A081', '#E5D5C5'],
    borderWidth: 2,
    borderColor: '#ffffff',
    hoverOffset: 4
  }]
}

const doughnutOptions = {
  responsive: true,
  maintainAspectRatio: false,
  cutout: '65%',
  plugins: {
    legend: { display: false },
    tooltip: {
      backgroundColor: '#2A231E',
      padding: 12,
      bodyFont: { size: 13, family: 'Montserrat', weight: 'bold' },
      cornerRadius: 8,
      displayColors: true
    }
  }
}

const tableData = [
  { id: '07153000', tags: ['Lương nhân viên', 'Nguyên liệu'], month: 'Tháng 06 / 2026', status: 'Đã chi', approval: 'Chờ duyệt' },
  { id: '07153001', tags: ['Mặt bằng'], month: 'Tháng 06 / 2026', status: 'Đã chi', approval: 'Chờ duyệt' },
  { id: '07153002', tags: ['Điện nước', 'Mặt bằng'], month: 'Tháng 06 / 2026', status: 'Chưa chi', approval: 'Chờ duyệt' },
  { id: '07153003', tags: ['Lương nhân viên'], month: 'Tháng 06 / 2026', status: 'Đã chi', approval: 'Chờ duyệt' },
  { id: '07153004', tags: ['Nguyên liệu'], month: 'Tháng 06 / 2026', status: 'Đã chi', approval: 'Chờ duyệt' },
]
</script>
