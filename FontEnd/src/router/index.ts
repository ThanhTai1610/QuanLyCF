import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import { routePermission } from './permissions'
import { BASE_URL } from '@/services/api'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  scrollBehavior(to, from, savedPosition) {
    if (to.hash) {
      return {
        el: to.hash,
        behavior: 'smooth',
      }
    }
    return savedPosition || { top: 0 }
  },
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('../pages/Login.vue')
    },
    {
      path: '/staff-login',
      name: 'staff-login',
      component: () => import('../pages/StaffLogin.vue')
    },
    {
      path: '/menu/:tableId',
      name: 'customer-menu',
      component: () => import('../pages/CustomerMenu.vue')
    },
    {
      path: '/lich-su-don',
      name: 'customer-order-history',
      component: () => import('../pages/CustomerOrderHistory.vue')
    },
    {
      path: '/payment/:orderId?',
      name: 'payment',
      component: () => import('../pages/Payment.vue')
    },
    {
      path: '/kitchen',
      name: 'kitchen',
      component: () => import('../pages/Kitchen.vue')
    },
    {
      path: '/',
      name: 'home',
      component: () => import('../pages/Index.vue')
    },
    {
      path: '/admin-layout',
      component: () => import('../layouts/MainLayout.vue'),
      children: [
        { path: '/orders', name: 'orders', component: () => import('../pages/Orders.vue') },
        { path: '/invoices', name: 'invoices', component: () => import('../pages/Invoices.vue') },
        { path: '/promotions', name: 'promotions', component: () => import('../pages/Promotions.vue') },
        { path: '/menu-admin', name: 'menu-admin', component: () => import('../pages/MenuAdmin.vue') },
        { path: '/combos', name: 'combos', component: () => import('../pages/Combos.vue') },
        { path: '/categories', name: 'categories', component: () => import('../pages/Categories.vue') },
        { path: '/tables', name: 'tables', component: () => import('../pages/Tables.vue') },
        { path: '/roles', name: 'roles', component: () => import('../pages/Roles.vue') },
        { path: '/inventory', name: 'inventory', component: () => import('../pages/Inventory.vue') },
        { path: '/suppliers', name: 'suppliers', component: () => import('../pages/Suppliers.vue') },
        { path: '/stocktake', name: 'stocktake', component: () => import('../pages/StockTake.vue') },
        { path: '/shifts', name: 'shifts', component: () => import('../pages/Shifts.vue') },
        { path: '/staff', name: 'staff', component: () => import('../pages/Staff.vue') },
        {
          path: '/settings',
          name: 'settings',
          component: () => import('../pages/Settings.vue'),
        },
        {
          path: '/system-logs',
          name: 'system-logs',
          component: () => import('../pages/SystemLogs.vue'),
        },
        {
          path: '/check-in',
          name: 'check-in',
          component: () => import('../pages/CheckIn.vue'),
        },
        {
          path: '/cashflow',
          name: 'cashflow',
          component: () => import('../pages/CashFlow.vue'),
        },
        {
          path: '/revenue-report',
          name: 'revenue-report',
          component: () => import('../pages/RevenueReport.vue'),
        },
        {
          path: '/pos-sale',
          name: 'pos-sale',
          alias: ['/pos', '/pos sale', '/pos_sale', '/possale', '/ban-hang'],
          component: () => import('../pages/POSSale.vue'),
        },
        {
          path: '/loyalty',
          name: 'loyalty',
          component: () => import('../pages/Loyalty.vue'),
        }
      ]
    },
    {
      path: '/maintenance',
      name: 'maintenance',
      component: () => import('../pages/Maintenance.vue')
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('../pages/NotFound.vue')
    }
  ]
})

let isMaintenanceCached = false
let lastMaintCheck = 0

async function checkMaintenanceMode(): Promise<boolean> {
  const now = Date.now()
  if (now - lastMaintCheck < 60000) return isMaintenanceCached
  lastMaintCheck = now
  try {
    const res = await fetch(`${BASE_URL}/api/settings/maintenance`)
    if (res.ok) {
      const data = await res.json()
      isMaintenanceCached = !!data.isMaintenance
    } else {
      isMaintenanceCached = false
    }
  } catch {
    isMaintenanceCached = false
  }
  return isMaintenanceCached
}

// Global auth + permission guard
router.beforeEach(async (to) => {
  const authStore = useAuthStore()

  // 1. Kiểm tra chế độ bảo trì hệ thống (Cached 60s để không làm nghẽn chuyển trang)
  const isMaintPage = to.name === 'maintenance' || to.path === '/maintenance'
  const isLoginPage = to.name === 'login' || to.path === '/login' || to.name === 'staff-login' || to.path === '/staff-login'

  if (!isMaintPage && !isLoginPage) {
    const isMaint = await checkMaintenanceMode()
    if (isMaint && (!authStore.isAuthenticated || !authStore.coQuyen('CAIDAT_QUANLY'))) {
      return { name: 'maintenance' }
    }
  } else if (isMaintPage) {
    const isMaint = await checkMaintenanceMode()
    if (!isMaint) return '/'
  }

  // 2. Kiểm tra quyền truy cập thông thường
  const perm = routePermission[to.path]
  const needAuth = to.meta.requiresAuth || perm !== undefined

  if (needAuth && !authStore.isAuthenticated) return { name: 'login' }

  // Chặn theo quyền: không đủ quyền -> chuyển về trang đầu tiên được phép
  if (perm && authStore.isAuthenticated && !authStore.coQuyen(perm)) {
    const allowed = Object.entries(routePermission).find(([, p]) => p === null || authStore.coQuyen(p))
    return allowed ? allowed[0] : '/'
  }
  return true
})

export default router