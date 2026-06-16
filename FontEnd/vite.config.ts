import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  server: {
    // Cho phep host cua cloudflared/ngrok (de public web)
    allowedHosts: true,
    // Proxy /api -> backend: chi can 1 link public, cung origin nen khong dinh CORS
    proxy: {
      '/api': {
        target: 'http://localhost:5124',
        changeOrigin: true,
      },
    },
  },
})
