<template>
  <div class="space-y-5 p-6">
    <!-- Trạng thái tải dữ liệu -->
    <div v-if="loading" class="flex flex-col items-center justify-center py-12 space-y-3">
      <RefreshCw class="w-8 h-8 text-caramel animate-spin" />
      <p class="text-sm text-muted-foreground">Đang tải cấu hình hệ thống...</p>
    </div>

    <template v-else>
      <!-- Thông tin quán -->
      <section class="bg-card rounded-lg border border-cream-deep shadow-card p-6">
        <div class="flex items-start gap-3 mb-5">
          <div class="w-10 h-10 rounded-lg bg-caramel-light flex items-center justify-center flex-shrink-0 border border-cream-deep">
            <Coffee class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h3 class="font-display text-lg text-espresso font-semibold">Thông tin quán</h3>
            <p class="text-xs text-muted-foreground mt-0.5">Tên, logo, địa chỉ và mô tả</p>
          </div>
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div class="space-y-1.5">
            <Label class="text-espresso">Tên quán</Label>
            <Input v-model="storeName" class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
          <div class="space-y-1.5">
            <Label class="text-espresso">Số điện thoại</Label>
            <Input v-model="storePhone" class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
          <div class="space-y-1.5 sm:col-span-2">
            <Label class="text-espresso">Địa chỉ</Label>
            <Input v-model="storeAddress" class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
          <div class="space-y-1.5 sm:col-span-2">
            <Label class="text-espresso">Mô tả ngắn</Label>
            <Textarea v-model="storeDesc" class="bg-background border border-cream-deep rounded-lg shadow-card min-h-[80px]" />
          </div>
          <div class="space-y-1.5">
            <Label class="text-espresso">Giờ mở cửa</Label>
            <div class="flex items-center gap-2">
              <Input type="time" v-model="openTime" class="bg-background border border-cream-deep rounded-lg shadow-card flex-1" />
              <span class="text-muted-foreground font-bold">-</span>
              <Input type="time" v-model="closeTime" class="bg-background border border-cream-deep rounded-lg shadow-card flex-1" />
            </div>
          </div>
          <div class="space-y-1.5">
            <Label class="text-espresso">Ảnh trang chủ (Link/URL)</Label>
            <div class="flex items-center gap-2">
              <Input v-model="heroImage" placeholder="Dán link ảnh bìa vào đây" class="bg-background border border-cream-deep rounded-lg shadow-card flex-1" />
              <input type="file" ref="fileInput" @change="handleFileUpload" accept="image/*" class="hidden" />
              <Button variant="outline" @click="$refs.fileInput.click()" type="button" class="border border-cream-deep rounded-lg shadow-card text-caramel hover:text-brown bg-caramel-light px-3" title="Tải ảnh từ máy tính">
                <Upload class="w-4 h-4" />
              </Button>
            </div>
          </div>
        </div>
      </section>

      <!-- Trợ lý ảo AI -->
      <section class="bg-card rounded-lg border border-cream-deep shadow-card p-6">
        <div class="flex items-start gap-3 mb-5">
          <div class="w-10 h-10 rounded-lg bg-caramel-light flex items-center justify-center flex-shrink-0 border border-cream-deep">
            <Sparkles class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h3 class="font-display text-lg text-espresso font-semibold">Trợ lý ảo AI</h3>
            <p class="text-xs text-muted-foreground mt-0.5">Tên gọi và cách xưng hô của Chatbot trên trang chủ</p>
          </div>
        </div>
        <div class="grid sm:grid-cols-2 gap-4">
          <div class="space-y-1.5">
            <Label class="text-espresso">Tên hiển thị của AI</Label>
            <Input v-model="tenAI" placeholder="VD: Barista AI, Cô Tấm, Cậu Vàng..." class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
          <div class="space-y-1.5">
            <Label class="text-espresso">Cách AI xưng hô với khách</Label>
            <Input v-model="xungHoAI" placeholder="VD: mình - bạn, em - anh/chị, tôi - bạn..." class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
        </div>
      </section>

      <!-- Phí dịch vụ & Thuế -->
      <section class="bg-card rounded-lg border border-cream-deep shadow-card p-6">
        <div class="flex items-start gap-3 mb-5">
          <div class="w-10 h-10 rounded-lg bg-caramel-light flex items-center justify-center flex-shrink-0 border border-cream-deep">
            <CreditCard class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h3 class="font-display text-lg text-espresso font-semibold">Thuế & Phí dịch vụ</h3>
            <p class="text-xs text-muted-foreground mt-0.5">Thuế VAT mặc định, phí phục vụ và tỷ lệ tích điểm</p>
          </div>
        </div>
        <div class="grid sm:grid-cols-3 gap-4">
          <div class="space-y-1.5">
            <Label class="text-espresso">Thuế VAT (%)</Label>
            <Input v-model="vatRate" type="number" class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
          <div class="space-y-1.5">
            <Label class="text-espresso">Phí dịch vụ (đ)</Label>
            <Input v-model="serviceFee" type="number" class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
          <div class="space-y-1.5">
            <Label class="text-espresso">Tích điểm (1 điểm cho mỗi X đồng)</Label>
            <Input v-model="pointRate" type="number" class="bg-background border border-cream-deep rounded-lg shadow-card" />
          </div>
        </div>
      </section>

      <!-- Bảo trì hệ thống -->
      <section class="bg-card rounded-lg border border-[#EF4444]/20 shadow-card p-6 relative overflow-hidden">
        <!-- Viền màu đỏ cảnh báo ở góc trái -->
        <div class="absolute left-0 top-0 bottom-0 w-1.5 bg-[#EF4444]"></div>
        
        <div class="flex items-start gap-3 mb-5">
          <div class="w-10 h-10 rounded-lg bg-[#EF4444]/10 flex items-center justify-center flex-shrink-0 border border-[#EF4444]/20">
            <Wrench class="w-5 h-5 text-[#EF4444]" />
          </div>
          <div>
            <h3 class="font-display text-lg text-espresso font-semibold text-[#EF4444]">Bảo trì hệ thống</h3>
            <p class="text-xs text-muted-foreground mt-0.5">Chặn tạm thời mọi truy cập từ khách đặt món QR và nhân viên không có quyền quản trị</p>
          </div>
        </div>
        
        <div class="space-y-4">
          <div class="flex items-center justify-between p-4 rounded-lg bg-background border border-cream-deep shadow-card">
            <div>
              <div class="text-sm font-medium text-espresso">Kích hoạt chế độ bảo trì</div>
              <div class="text-xs text-muted-foreground mt-0.5">Khi bật, hệ thống sẽ ngừng phục vụ khách và nhân viên thường</div>
            </div>
            <Switch v-model="maintenanceMode" />
          </div>
          
          <div v-if="maintenanceMode" class="space-y-1.5 transition-all">
            <Label class="text-espresso font-medium text-[#EF4444]">Thông điệp bảo trì hiển thị với người dùng</Label>
            <Textarea v-model="maintenanceMessage" class="bg-background border border-cream-deep rounded-lg shadow-card min-h-[80px]" placeholder="Nhập nội dung thông báo bảo trì..." />
          </div>
        </div>
      </section>

      <!-- Thông báo (Mock cục bộ) -->
      <section class="bg-card rounded-lg border border-cream-deep shadow-card p-6">
        <div class="flex items-start gap-3 mb-5">
          <div class="w-10 h-10 rounded-lg bg-caramel-light flex items-center justify-center flex-shrink-0 border border-cream-deep">
            <Bell class="w-5 h-5 text-caramel" />
          </div>
          <div>
            <h3 class="font-display text-lg text-espresso font-semibold">Thông báo client</h3>
            <p class="text-xs text-muted-foreground mt-0.5">Cách nhận thông báo về đơn hàng (chỉ lưu trên máy này)</p>
          </div>
        </div>
        <div class="space-y-3">
          <div v-for="(opt, i) in notifications" :key="i" class="flex items-center justify-between p-4 rounded-lg bg-background border border-cream-deep shadow-card">
            <div>
              <div class="text-sm font-medium text-espresso">{{ opt.label }}</div>
              <div class="text-xs text-muted-foreground mt-0.5">{{ opt.sub }}</div>
            </div>
            <Switch v-model="opt.on" />
          </div>
        </div>
      </section>

      <!-- Footer buttons -->
      <div class="flex justify-end gap-3 pt-2">
        <Button variant="outline" @click="loadSettings" :disabled="saving" class="border border-cream-deep rounded-lg shadow-card">
          Hủy thay đổi
        </Button>
        <Button @click="save" :disabled="saving" class="bg-[#CC8033] hover:bg-[#B36E2B] text-white rounded-lg border border-[#CC8033]/30 shadow-card flex items-center gap-2">
          <RefreshCw v-if="saving" class="w-4 h-4 animate-spin" />
          {{ saving ? 'Đang lưu...' : 'Lưu cài đặt' }}
        </Button>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Coffee, Bell, CreditCard, Wrench, RefreshCw, Upload, Sparkles } from 'lucide-vue-next'
import { api } from '@/services/api'
import { useStoreInfoStore } from '@/stores/storeInfo'
import { useAlert } from '@/stores/alert'
import Button from '@/components/ui/Button.vue'
import Input from '@/components/ui/Input.vue'
import Label from '@/components/ui/Label.vue'
import Switch from '@/components/ui/Switch.vue'
import Textarea from '@/components/ui/Textarea.vue'

const loading        = ref(false)
const saving         = ref(false)
const storeInfoStore = useStoreInfoStore()
const alert          = useAlert()

// Thông tin quán
const storeName = ref("")
const storePhone = ref("")
const storeAddress = ref("")
const storeDesc = ref("")
const openTime = ref("07:00")
const closeTime = ref("22:30")
const heroImage = ref("")
const fileInput = ref<HTMLInputElement | null>(null)

// Trợ lý AI
const tenAI = ref("Barista AI")
const xungHoAI = ref("tôi - bạn")

// Thuế & Phí & Tích điểm
const vatRate = ref("8")
const serviceFee = ref("0")
const pointRate = ref("1")

// Bảo trì hệ thống
const maintenanceMode = ref(false)
const maintenanceMessage = ref("")

// Cài đặt client mock
const notifications = ref([
  { label: "Thông báo đơn mới", sub: "Phát âm thanh khi có đơn từ khách", on: true },
  { label: "Email báo cáo cuối ngày", sub: "Tự động gửi tổng kết doanh thu mỗi tối", on: true },
  { label: "Cảnh báo hết nguyên liệu", sub: "Báo khi tồn kho dưới mức tối thiểu", on: false },
])

// Load cấu hình từ Backend API
const loadSettings = async () => {
  loading.value = true
  try {
    const res = await api.get<any>('/api/settings')
    if (res) {
      storeName.value = res.tenQuan || ""
      storePhone.value = res.soDienThoai || ""
      storeAddress.value = res.diaChi || ""
      storeDesc.value = res.moTaQuan || ""
      
      const hours = res.gioMoCua || "07:00 - 22:30"
      if (hours.includes(' - ')) {
        const parts = hours.split(' - ')
        openTime.value = parts[0]
        closeTime.value = parts[1]
      }
      
      heroImage.value = res.anhTrangChu || ""
      vatRate.value = res.thueVatMacDinh || "8"
      serviceFee.value = res.phiDichVu || "0"
      pointRate.value = res.tyLeTichDiem || "1"
      maintenanceMode.value = res.cheDoBaoTri ?? false
      maintenanceMessage.value = res.thongDiepBaoTri || ""
      tenAI.value = res.tenAI || "Barista AI"
      xungHoAI.value = res.xungHoAI || "tôi - bạn"
    }
  } catch (err: any) {
    await alert.error('Lỗi tải dữ liệu', 'Không thể tải cài đặt: ' + err.message)
  } finally {
    loading.value = false
  }
}

// Lưu cấu hình xuống Backend API
const save = async () => {
  saving.value = true
  try {
    await api.put('/api/settings', {
      tenQuan: storeName.value,
      diaChi: storeAddress.value,
      soDienThoai: storePhone.value,
      moTaQuan: storeDesc.value,
      gioMoCua: `${openTime.value} - ${closeTime.value}`,
      anhTrangChu: heroImage.value,
      thueVatMacDinh: vatRate.value.toString(),
      phiDichVu: serviceFee.value.toString(),
      tyLeTichDiem: pointRate.value.toString(),
      cheDoBaoTri: maintenanceMode.value,
      thongDiepBaoTri: maintenanceMessage.value,
      tenAI: tenAI.value,
      xungHoAI: xungHoAI.value,
    })

    // Cập nhật store toàn cục ngay lập tức — các trang khác thay đổi không cần reload
    storeInfoStore.setInfo({
      tenQuan:     storeName.value,
      diaChi:      storeAddress.value,
      soDienThoai: storePhone.value,
      moTaQuan:    storeDesc.value,
      gioMoCua:    `${openTime.value} - ${closeTime.value}`,
      anhTrangChu: heroImage.value,
      tenAI:       tenAI.value,
      xungHoAI:    xungHoAI.value,
    })

    if (maintenanceMode.value) {
      await alert.warning('Đã lưu cài đặt!', 'Hệ thống đang ở chế độ bảo trì — khách hàng không thể truy cập.')
    } else {
      await alert.success('Lưu thành công!', 'Tất cả thông tin quán đã được cập nhật.')
    }

    // Load lại dữ liệu
    await loadSettings()
  } catch (err: any) {
    await alert.error('Không thể lưu cài đặt', 'Kiểm tra lại kết nối và thử lại: ' + err.message)
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadSettings()
})

const handleFileUpload = async (event: Event) => {
  const target = event.target as HTMLInputElement;
  const file = target.files?.[0];
  if (!file) return;

  const formData = new FormData();
  formData.append('file', file);

  try {
    const response = await fetch('/api/uploads/image', {
      method: 'POST',
      body: formData,
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
      }
    });

    if (!response.ok) {
      let errorMessage = 'Lỗi tải ảnh';
      try {
        const errText = await response.text();
        if (errText) {
          try {
            const errJson = JSON.parse(errText);
            errorMessage = errJson.message || errorMessage;
          } catch {
            errorMessage = errText;
          }
        }
      } catch (e) {
        // Fallback if reading text fails
      }
      throw new Error(`[${response.status}] ${errorMessage}`);
    }

    const data = await response.json();
    heroImage.value = data.url; // Đường dẫn tuyệt đối từ Backend trả về
    await alert.success('Thành công', 'Đã tải ảnh lên. Nhớ bấm Lưu cài đặt!');
  } catch (err: any) {
    await alert.error('Lỗi upload', err.message);
  } finally {
    if (fileInput.value) fileInput.value.value = ''; // Reset input file
  }
}
</script>
