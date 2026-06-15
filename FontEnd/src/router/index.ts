import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
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
      path: '/payment/:orderId',
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
      path: '/',
      component: () => import('../layouts/MainLayout.vue'),
      meta: { requiresAuth: true },
      children: [
        { path: 'orders', name: 'orders', component: () => import('../pages/Orders.vue') },
        { path: 'invoices', name: 'invoices', component: () => import('../pages/Invoices.vue') },
        { path: 'menu-admin', name: 'menu-admin', component: () => import('../pages/MenuAdmin.vue') },
        { path: 'combos', name: 'combos', component: () => import('../pages/Combos.vue') },
        { path: 'categories', name: 'categories', component: () => import('../pages/Categories.vue') },
        { path: 'tables', name: 'tables', component: () => import('../pages/Tables.vue') },
        { path: 'inventory', name: 'inventory', component: () => import('../pages/Inventory.vue') },
        { path: 'suppliers', name: 'suppliers', component: () => import('../pages/Suppliers.vue') },
        { path: 'stocktake', name: 'stocktake', component: () => import('../pages/StockTake.vue') },
        { path: 'shifts', name: 'shifts', component: () => import('../pages/Shifts.vue') },
        { path: 'staff', name: 'staff', component: () => import('../pages/Staff.vue') },
        {
          path: 'settings',
          name: 'settings',
          component: () => import('../pages/Settings.vue'),
        },
        {
          path: 'system-logs',
          name: 'system-logs',
          component: () => import('../pages/SystemLogs.vue'),
        },
        {
          path: 'check-in',
          name: 'check-in',
          component: () => import('../pages/CheckIn.vue'),
        },
        {
          path: 'cashflow',
          name: 'cashflow',
          component: () => import('../pages/CashFlow.vue'),
        },
        {
          path: 'revenue-report',
          name: 'revenue-report',
          component: () => import('../pages/Dashboard.vue'),
        },
        {
          path: 'pos-sale',
          name: 'pos-sale',
          component: () => import('../pages/POSSale.vue'),
        },
        {
          path: 'loyalty',
          name: 'loyalty',
          component: () => import('../pages/Loyalty.vue'),
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('../pages/NotFound.vue')
    }
  ]
})

// Global auth guard
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login' })
  } else {
    next()
  }
})

export default router