<template>
  <div class="min-h-screen bg-[#120F0D] text-white flex flex-col justify-between p-6 font-sans">
    
    <!-- Top Header -->
    <header class="text-center py-4">
      <div class="inline-flex items-center gap-2 mb-2">
        <div class="w-8 h-8 rounded-lg bg-[#CC8033] flex items-center justify-center text-white shadow-lg">
          <Zap class="w-4 h-4 fill-current" />
        </div>
        <span class="font-display font-bold tracking-wider text-sm uppercase">{{ storeInfoStore.tenQuan }}</span>
      </div>
      <h1 class="font-display text-xl font-bold text-white mt-1">Chấm công di động</h1>
      <p class="text-[10px] text-[#8A8178] uppercase tracking-widest mt-1">Xác thực vị trí & khuôn mặt</p>
    </header>

    <!-- Main Card -->
    <main class="flex-1 flex flex-col justify-center max-w-sm mx-auto w-full my-6">
      
      <!-- Lỗi chưa liên kết session -->
      <div v-if="!tokenLoaded" class="bg-red-500/10 border border-red-500/20 p-6 rounded-2xl text-center space-y-4">
        <AlertTriangle class="w-12 h-12 text-red-500 mx-auto animate-bounce" />
        <h3 class="font-bold text-lg text-white">Thiếu phiên làm việc</h3>
        <p class="text-xs text-[#8A8178]">Vui lòng quét lại mã QR trên màn hình chấm công tại quầy để bắt đầu.</p>
      </div>

      <!-- Đã hoàn thành -->
      <div v-else-if="checkinSuccess" class="bg-emerald-500/10 border border-emerald-500/20 p-6 rounded-2xl text-center space-y-4 animate-in zoom-in-95 duration-300">
        <div class="w-16 h-16 rounded-full bg-emerald-500/20 flex items-center justify-center mx-auto text-emerald-400">
          <Check class="w-8 h-8" stroke-width="3" />
        </div>
        <h3 class="font-bold text-xl text-white">Chấm công thành công!</h3>
        <p class="text-xs text-[#8A8178]">Thời gian ghi nhận: <span class="text-emerald-400 font-bold font-mono">{{ successTime }}</span></p>
        <p class="text-[11px] text-[#8A8178] font-medium pt-2">Bạn có thể đóng tab trình duyệt này ngay bây giờ.</p>
      </div>

      <!-- Form chụp ảnh check-in -->
      <div v-else class="bg-[#1A1512] border border-white/10 rounded-3xl p-6 shadow-2xl space-y-5">
        
        <!-- Greeting user -->
        <div class="text-center bg-white/5 border border-white/5 p-3 rounded-xl">
          <div class="text-[9px] uppercase tracking-widest text-[#8A8178] font-bold">Nhân viên xác nhận</div>
          <div class="text-sm font-bold text-[#CC8033] mt-0.5">{{ employeeName || 'Đang tải danh tính...' }}</div>
          <div class="text-[10px] text-white/50 mt-1 uppercase font-semibold">
            Hoạt động: 
            <span :class="checkInType === 'in' ? 'text-emerald-400' : 'text-red-400'" class="font-bold">
              {{ checkInType === 'in' ? 'Vào Ca' : 'Kết Ca' }}
            </span>
          </div>
        </div>

        <!-- Camera box -->
        <div class="relative w-full aspect-[4/3] rounded-2xl overflow-hidden bg-black border border-white/15 shadow-inner">
          <video ref="videoElement" class="w-full h-full object-cover" autoplay playsinline></video>
          <div v-if="!cameraActive" class="absolute inset-0 flex items-center justify-center bg-black/80">
            <Camera class="w-8 h-8 text-white/20 animate-pulse" />
          </div>
          <div v-if="photoCaptured" class="absolute inset-0">
            <img :src="photoUrl" class="w-full h-full object-cover" />
          </div>
          <div class="absolute inset-6 border-2 border-dashed border-white/40 rounded-[30%] animate-pulse pointer-events-none"></div>
        </div>

        <!-- Ghi chú -->
        <div class="space-y-1.5" v-if="photoCaptured">
          <label class="text-[10px] uppercase tracking-widest text-[#8A8178] font-bold">Ghi chú (Tùy chọn)</label>
          <input v-model="checkInNotes" placeholder="Lý do đi trễ/về sớm..." class="w-full bg-[#120F0D] border border-white/10 rounded-xl px-4 py-2.5 text-xs text-white placeholder:text-[#8A8178] focus:outline-none focus:border-[#CC8033]" />
        </div>

        <!-- Action Buttons -->
        <div class="space-y-3 pt-2">
          <div v-if="!photoCaptured" class="flex justify-center">
            <button @click="takePhoto" class="w-16 h-16 rounded-full bg-[#CC8033] hover:bg-[#B8722D] flex items-center justify-center text-white shadow-lg active:scale-95 transition-all border-4 border-white">
              <Camera class="w-6 h-6" />
            </button>
          </div>
          <div v-else class="grid grid-cols-2 gap-3">
            <button @click="retakePhoto" class="py-2.5 rounded-xl border border-white/10 bg-white/5 hover:bg-white/10 text-white text-xs font-bold transition-colors">
              Chụp lại
            </button>
            <button @click="submitCheckIn" :disabled="submitting" class="py-2.5 rounded-xl bg-[#CC8033] hover:bg-[#B8722D] text-white text-xs font-bold transition-colors shadow-lg flex items-center justify-center gap-1.5">
              <span v-if="submitting">Đang gửi...</span>
              <span v-else>Xác nhận</span>
            </button>
          </div>
        </div>

      </div>
    </main>

    <!-- Bottom Footer -->
    <footer class="text-center py-4 border-t border-white/5 text-[9px] text-[#8A8178] uppercase tracking-widest">
      {{ storeInfoStore.tenQuan }} © 2026 · Hệ thống chấm công FaceID
    </footer>

  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { Zap, Camera, Check, AlertTriangle } from 'lucide-vue-next'
import { api } from '@/services/api'
import { hrApi } from '@/services/hr'
import { useStoreInfoStore } from '@/stores/storeInfo'

const storeInfoStore = useStoreInfoStore()

const route = useRoute()

const tokenLoaded = ref(false)
const employeeName = ref('')
const checkInType = ref<'in' | 'out'>('in')
const checkinSuccess = ref(false)
const successTime = ref('')
const submitting = ref(false)

// Camera State
const videoElement = ref<HTMLVideoElement | null>(null)
const cameraActive = ref(false)
const photoCaptured = ref(false)
const photoUrl = ref('')
const checkInNotes = ref('')
let mediaStream: MediaStream | null = null

const startCamera = async () => {
  cameraActive.value = false
  photoCaptured.value = false
  photoUrl.value = ''
  try {
    mediaStream = await navigator.mediaDevices.getUserMedia({ 
      video: { facingMode: 'user', width: 640, height: 480 } 
    })
    if (videoElement.value) {
      videoElement.value.srcObject = mediaStream
      videoElement.value.onloadedmetadata = () => {
        cameraActive.value = true
      }
    }
  } catch (err) {
    console.error("Failed to access camera", err)
  }
}

const takePhoto = () => {
  if (videoElement.value && cameraActive.value && mediaStream) {
    try {
      const canvas = document.createElement('canvas')
      canvas.width = videoElement.value.videoWidth || 640
      canvas.height = videoElement.value.videoHeight || 480
      const ctx = canvas.getContext('2d')
      if (ctx) {
        ctx.drawImage(videoElement.value, 0, 0, canvas.width, canvas.height)
        photoUrl.value = canvas.toDataURL('image/jpeg')
        photoCaptured.value = true
        stopCamera()
      }
    } catch (e) {
      console.error(e)
    }
  }
}

const stopCamera = () => {
  if (mediaStream) {
    mediaStream.getTracks().forEach(track => track.stop())
    mediaStream = null
  }
  cameraActive.value = false
}

const retakePhoto = async () => {
  photoCaptured.value = false
  photoUrl.value = ''
  await startCamera()
}

const submitCheckIn = async () => {
  if (!photoUrl.value) return
  submitting.value = true
  try {
    const res = await hrApi.checkIn({
      type: checkInType.value,
      photoUrl: photoUrl.value,
      ghiChu: checkInNotes.value
    })
    successTime.value = new Date().toLocaleTimeString('vi-VN')
    checkinSuccess.value = true
  } catch (err: any) {
    alert(err.message || 'Lỗi khi gửi chấm công')
  } finally {
    submitting.value = false
  }
}

const loadSession = async () => {
  const token = route.query.token as string
  const typeParam = route.query.type as string

  if (token) {
    localStorage.setItem('accessToken', token)
    tokenLoaded.value = true
  } else {
    // Thử đọc token cũ xem có sẵn không
    const existingToken = localStorage.getItem('accessToken')
    if (existingToken) {
      tokenLoaded.value = true
    }
  }

  if (typeParam === 'in' || typeParam === 'out') {
    checkInType.value = typeParam
  }

  if (tokenLoaded.value) {
    try {
      // Gọi thử api get me để lấy thông tin nhân viên
      const me: any = await api.get('/api/auth/me')
      if (me && me.hoTen) {
        employeeName.value = me.hoTen
      }
    } catch (e) {
      console.error("Failed to load employee session info", e)
      employeeName.value = 'Mã nhân viên #' + (route.query.userId || '')
    }
    await startCamera()
  }
}

onMounted(() => {
  loadSession()
})

onUnmounted(() => {
  stopCamera()
})
</script>
