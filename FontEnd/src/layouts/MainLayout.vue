<template>
  <div class="h-screen flex w-full bg-background overflow-hidden">
    <!-- SIDEBAR -->
    <aside class="hidden md:flex w-[220px] flex-col bg-[#1E2128] text-[#E4E4E7] border-r border-black/10 overflow-hidden shadow-2xl z-20">
      
      <!-- LOGO -->
      <div class="h-14 flex items-center gap-2.5 px-4 border-b border-white/5">
        <div class="w-8 h-8 rounded-lg bg-[#CC8033] flex items-center justify-center shadow-lg shadow-[#CC8033]/20">
          <Coffee class="w-4.5 h-4.5 text-white" />
        </div>
        <span class="font-display text-base text-white font-bold tracking-wide">{{ storeInfoStore.tenQuan }}</span>
      </div>

      <!-- NAVIGATION -->
      <nav class="flex-1 px-3 py-4 space-y-1.5 overflow-y-auto custom-scrollbar">
        
        <template v-for="(item, idx) in visibleNav" :key="idx">
          
          <!-- SINGLE ROOT ITEM -->
          <router-link
            v-if="!item.children"
            :to="item.to!"
            class="flex items-center justify-between px-3 py-2 rounded-lg transition-colors group"
            :class="route.path === item.to ? 'bg-[#CC8033] text-white shadow-lg shadow-[#CC8033]/20' : 'hover:bg-white/5 text-[#A1A1AA] hover:text-white'"
          >
            <div class="flex items-center gap-2.5">
              <component :is="item.icon" class="w-4 h-4" />
              <span class="text-sm font-medium">{{ item.label }}</span>
            </div>
          </router-link>

          <!-- GROUP ACCORDION -->
          <div v-else class="rounded-xl overflow-hidden transition-all duration-300" :class="expanded[item.label] ? 'bg-[#13151A] pb-2 shadow-inner' : 'bg-[#272A30]'">
            
            <!-- Group Header -->
            <button
              @click="toggleGroup(item.label)"
              class="w-full flex items-center justify-between px-4 py-3 rounded-xl transition-colors relative z-10"
              :class="expanded[item.label] ? 'bg-[#CC8033] text-white shadow-lg shadow-[#CC8033]/20' : 'text-[#E4E4E7] hover:bg-white/5'"
            >
              <div class="flex items-center gap-2.5">
                <component :is="item.icon" class="w-4 h-4" />
                <span class="text-sm font-medium">{{ item.label }}</span>
              </div>
              <ChevronDown 
                class="w-4 h-4 transition-transform duration-300" 
                :class="expanded[item.label] ? 'rotate-180' : ''" 
              />
            </button>

            <!-- Group Children -->
            <div 
              class="grid transition-all duration-300"
              :class="expanded[item.label] ? 'grid-rows-[1fr] opacity-100 mt-2' : 'grid-rows-[0fr] opacity-0'"
            >
              <div class="overflow-hidden space-y-1 px-1">
                <template v-for="child in item.children" :key="child.to">
                  <!-- External Link -->
                  <a
                    v-if="child.external"
                    :href="child.to"
                    target="_blank"
                    class="flex items-center justify-between px-3 py-1.5 rounded-lg text-xs transition-colors bg-[#272A30] text-[#A1A1AA] hover:text-white hover:bg-[#31353C]"
                  >
                    <div class="flex items-center gap-2">
                      <div class="w-1 h-1 rounded-full bg-current opacity-50"></div>
                      <span class="font-medium">{{ child.label }}</span>
                    </div>
                  </a>
                  <!-- Internal Link -->
                  <router-link
                    v-else
                    :to="child.to"
                    class="flex items-center justify-between px-3 py-1.5 rounded-lg text-xs transition-colors"
                    :class="route.path.startsWith(child.to) ? 'bg-[#CC8033]/10 text-[#CC8033]' : 'bg-[#272A30] text-[#A1A1AA] hover:text-white hover:bg-[#31353C]'"
                  >
                    <div class="flex items-center gap-2">
                      <div class="w-1 h-1 rounded-full bg-current opacity-50"></div>
                      <span class="font-medium">{{ child.label }}</span>
                    </div>
                  </router-link>
                </template>
              </div>
            </div>

          </div>

        </template>
      </nav>

      <!-- USER PROFILE -->
      <div class="p-3 border-t border-white/5 bg-[#1A1D24]">
        <div class="flex items-center gap-2.5 p-1.5 rounded-lg hover:bg-white/5 transition-colors cursor-pointer group">
          <div class="w-8 h-8 rounded-full bg-[#CC8033] flex items-center justify-center text-white font-bold text-xs shadow-md">{{ userInitials }}</div>
          <div class="flex-1 min-w-0">
            <div class="text-xs text-white font-medium truncate">{{ authStore.user?.hoTen ?? 'Người dùng' }}</div>
            <div class="text-[10px] text-[#A1A1AA]">{{ authStore.user?.vaiTro ?? '' }}</div>
          </div>
          <button @click="handleLogout" class="text-[#A1A1AA] hover:text-[#EF4444] p-1.5 transition-colors">
            <LogOut class="w-3.5 h-3.5" />
          </button>
        </div>
      </div>
    </aside>

    <!-- MAIN CONTENT -->
    <div class="flex-1 flex flex-col min-w-0">
      <!-- Banner Cảnh báo bảo trì dành cho Admin -->
      <div v-if="isSystemMaintenance" class="bg-[#EF4444]/10 border-b border-[#EF4444]/20 px-6 py-2.5 flex items-center gap-2.5 text-[#EF4444] z-20">
        <Wrench class="w-4 h-4 flex-shrink-0 animate-pulse" />
        <span class="text-xs font-semibold">Chế độ bảo trì đang bật. Mọi truy cập đặt món QR của khách hàng và nhân viên thường đều bị chặn.</span>
      </div>

      <header class="h-14 bg-cream border-b border-cream-deep flex items-center justify-between px-6 shadow-sm z-10">
        <div>
          <h1 class="font-display text-lg font-bold text-espresso">{{ currentLabel }}</h1>
          <p class="text-xs text-muted-foreground mt-0.5">Hôm nay, {{ todayDate }}</p>
        </div>
        <div class="flex items-center gap-3 relative">
          <!-- Notification Bell -->
          <button @click="showNotifications = !showNotifications"
            class="relative w-8 h-8 flex items-center justify-center rounded-lg bg-white border border-cream-deep shadow-sm hover:bg-cream-deep transition-colors text-espresso">
            <Bell class="w-4 h-4" />
            <span v-if="serviceRequests.length > 0"
              class="absolute -top-1 -right-1 min-w-[16px] h-4 px-1 flex items-center justify-center bg-destructive text-white text-[8px] font-extrabold rounded-full border border-white animate-pulse">
              {{ serviceRequests.length }}
            </span>
          </button>

          <!-- Notifications Dropdown -->
          <div v-if="showNotifications"
            class="absolute right-0 top-10 w-[320px] bg-white rounded-2xl border border-cream-deep shadow-xl z-50 p-4 space-y-3">
            <div class="flex items-center justify-between border-b border-cream-deep pb-2">
              <span class="text-xs font-bold text-espresso uppercase tracking-wider">Yêu cầu hỗ trợ ({{ serviceRequests.length }})</span>
              <button @click="showNotifications = false" class="text-muted-foreground hover:text-espresso">
                <X class="w-4 h-4" />
              </button>
            </div>
            
            <div class="max-h-[240px] overflow-y-auto space-y-2 custom-scrollbar pr-1">
              <div v-if="serviceRequests.length === 0" class="text-center py-6 text-xs text-muted-foreground font-medium">
                Không có yêu cầu mới
              </div>
              <div v-else v-for="req in serviceRequests" :key="req.id"
                class="flex items-start justify-between gap-2 p-2.5 rounded-lg bg-cream/30 border border-cream-deep/60">
                <div class="min-w-0">
                  <p class="text-xs font-bold text-espresso">
                    {{ req.tenBan }} yêu cầu 
                    <span class="text-caramel font-extrabold">
                      {{
                        req.loaiYeuCau === 'GoiPhucVu'
                          ? 'gọi phục vụ'
                          : req.loaiYeuCau === 'ThanhToanTienMat'
                          ? 'thanh toán tiền mặt'
                          : 'thanh toán chuyển khoản'
                      }}
                    </span>
                  </p>
                  <p class="text-[10px] text-muted-foreground mt-0.5" v-if="req.ghiChu">Ghi chú: {{ req.ghiChu }}</p>
                </div>
                <button @click="resolveRequest(req.id)"
                  class="h-6 px-2 rounded bg-caramel hover:bg-brown text-cream text-[10px] font-bold transition-all shadow-xs active:scale-95 shrink-0">
                  Xử lý
                </button>
              </div>
            </div>
          </div>
        </div>
      </header>

      <main class="flex-1 overflow-y-auto p-6">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { useStoreInfoStore } from '../stores/storeInfo'
import { useToast } from '@/stores/toast'
import { ordersApi } from '@/services/orders'
import {
  LayoutDashboard, ShoppingBag, Coffee, Users, Settings, LogOut,
  QrCode, FileText, FolderTree, Package, ClipboardCheck, CalendarDays, BookOpen, ChefHat, Bell, ShieldCheck,
  ChevronDown, Wallet, Heart, Star, Gift, Wrench, X, Check
} from 'lucide-vue-next'

import { routePermission } from '@/router/permissions'

const route = useRoute()
const router = useRouter()
const authStore      = useAuthStore()
const storeInfoStore = useStoreInfoStore()
const toast          = useToast()

// Ẩn menu theo quyền: chỉ hiện trang user được phép vào
function canSee(to: string) {
  const p = routePermission[to]
  return p == null ? true : authStore.coQuyen(p)
}

interface NavChild { to: string; label: string; external?: boolean; badge?: string }
interface NavItem { label: string; icon?: any; to?: string; external?: boolean; badge?: string; children?: NavChild[] }
const navItems: NavItem[] = [
  {
    label: "Nhân sự & Ca làm",
    icon: Users,
    children: [
      { to: "/staff", label: "Quản lý tài khoản" },
      { to: "/roles", label: "Vai trò & quyền" },
      { to: "/check-in", label: "Chấm công & Yêu cầu" },
      { to: "/shifts", label: "Ca làm & Bảng lương" },
    ]
  },
  {
    label: "Quản lý sản phẩm",
    icon: Coffee,
    children: [
      { to: "/menu-admin", label: "Thực đơn", badge: "Cấp 1" },
      { to: "/combos", label: "Quản lý Combo", badge: "Cấp 1" },
      { to: "/categories", label: "Danh mục", badge: "Cấp 2" },
    ]
  },
  {
    label: "Quản lý đơn hàng",
    icon: ShoppingBag,
    children: [
      { to: "/orders", label: "Đơn hàng", badge: "Cấp 1" },
      { to: "/invoices", label: "Hoá đơn", badge: "Cấp 2" },
      { to: "/promotions", label: "Khuyến mãi" },
    ]
  },
  {
    label: "Quản lý Tài chính",
    icon: Wallet,
    children: [
      { to: "/cashflow", label: "Dòng tiền & Chi phí", badge: "Cấp 2" },
      { to: "/revenue-report", label: "Báo cáo doanh thu", badge: "Cấp 2" },
    ]
  },
  {
    label: "Bếp & Phục vụ",
    icon: ChefHat,
    children: [
      { to: "/kitchen", label: "Màn hình bếp", external: true, badge: "Cấp 1" },
      { to: "/tables", label: "Bàn & QR", badge: "Cấp 1" },
      { to: "/pos-sale", label: "Bán hàng tại quầy", badge: "Cấp 1" },
    ]
  },
  {
    label: "Kho & Kiểm kê",
    icon: Package,
    children: [
      { to: "/inventory", label: "Kho nguyên liệu", badge: "Cấp 1" },
      { to: "/suppliers", label: "Nhà cung cấp & Nhập", badge: "Cấp 1" },
      { to: "/stocktake", label: "Kiểm kê", badge: "Cấp 2" },
    ]
  },
  {
    label: "Khách hàng",
    icon: Heart,
    children: [
      { to: "/loyalty", label: "Khách hàng thân thiết", badge: "Cấp 1" },
    ]
  },
  {
    label: "Hệ thống",
    icon: Settings,
    children: [
      { to: "/settings", label: "Cài đặt" },
      { to: "/system-logs", label: "Nhật ký hệ thống" },
    ]
  }
]

const expanded = ref<Record<string, boolean>>({})

const isSystemMaintenance = ref(false)

const checkMaintenanceStatus = async () => {
  try {
    const res = await fetch('/api/settings/maintenance')
    if (res.ok) {
      const data = await res.json()
      isSystemMaintenance.value = data.isMaintenance
    }
  } catch (e) {}
}

function expandCurrentGroup() {
  for (const item of navItems) {
    if (item.children?.some(c => route.path === c.to || (c.to !== '/' && route.path.startsWith(c.to)))) {
      expanded.value[item.label] = true
    }
  }
}

onMounted(() => {
  expandCurrentGroup()
  checkMaintenanceStatus()
})

watch(() => route.path, () => {
  expandCurrentGroup()
  checkMaintenanceStatus()
})

const toggleGroup = (label: string) => {
  expanded.value[label] = !expanded.value[label]
}

// Menu sau khi lọc theo quyền (bỏ nhóm rỗng)
const visibleNav = computed(() =>
  navItems
    .map(g => ({ ...g, children: g.children ? g.children.filter(c => canSee(c.to)) : g.children }))
    .filter(g => !g.children || g.children.length > 0))

const flat: NavChild[] = navItems.flatMap((g) => g.children ?? [])

const currentLabel = computed(() => {
  const current = flat.find((n) => route.path === n.to || (n.to !== '/' && n.to !== '/dashboard' && route.path.startsWith(n.to)))
  return current?.label ?? "Tổng quan"
})

const todayDate = new Date().toLocaleDateString("vi-VN", { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })

const userInitials = computed(() => {
  const words = (authStore.user?.hoTen ?? '').trim().split(/\s+/).filter(Boolean)
  if (words.length === 0) return 'U'
  return words.length >= 2
    ? ((words[0]?.[0] ?? '') + (words[words.length - 1]?.[0] ?? '')).toUpperCase()
    : (words[0]?.[0] ?? 'U').toUpperCase()
})

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

// ── Service Requests Notifications ──
const showNotifications = ref(false)
const serviceRequests = ref<any[]>([])
const previousCount = ref(0)

async function loadServiceRequests() {
  try {
    if (authStore.accessToken) {
      const data = await ordersApi.getActiveServiceRequests()
      
      // Phát hiện yêu cầu mới để bắn toast thông báo nổi
      if (data.length > previousCount.value) {
        const newReqs = data.filter((x: any) => !serviceRequests.value.some((y: any) => y.id === x.id))
        newReqs.forEach((r: any) => {
          const typeStr = r.loaiYeuCau === 'GoiPhucVu'
            ? 'gọi phục vụ'
            : r.loaiYeuCau === 'ThanhToanTienMat'
            ? 'yêu cầu thanh toán tiền mặt'
            : 'yêu cầu thanh toán chuyển khoản'
          toast.info(`${r.tenBan}: ${typeStr.toUpperCase()} ${r.ghiChu && r.ghiChu !== 'Yêu cầu phục vụ tại bàn' && r.ghiChu !== 'Yêu cầu thanh toán tiền mặt' && r.ghiChu !== 'Yêu cầu thanh toán chuyển khoản (Momo/QR)' ? `(${r.ghiChu})` : ''}`)
        })
      }
      
      serviceRequests.value = data
      previousCount.value = data.length
    }
  } catch (e) {
    console.error('Lỗi tải yêu cầu phục vụ ở layout:', e)
  }
}

async function resolveRequest(id: string) {
  try {
    await ordersApi.resolveServiceRequest(id)
    serviceRequests.value = serviceRequests.value.filter(r => r.id !== id)
    previousCount.value = serviceRequests.value.length
  } catch (e) {
    console.error('Lỗi hoàn tất yêu cầu ở layout:', e)
  }
}

let serviceRequestsInterval: any = null
onMounted(() => {
  loadServiceRequests()
  serviceRequestsInterval = setInterval(loadServiceRequests, 2000)
})
onUnmounted(() => {
  if (serviceRequestsInterval) clearInterval(serviceRequestsInterval)
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 4px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
}
.custom-scrollbar:hover::-webkit-scrollbar-thumb {
  background-color: rgba(255, 255, 255, 0.2);
}
</style>
