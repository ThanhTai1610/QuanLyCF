<template>
  <div>
    <!-- Nút mở Chatbot -->
    <button
      @click="open = !open"
      class="fixed bottom-6 right-6 z-50 h-14 w-auto px-5 rounded-full bg-gradient-to-r from-[#CC8033] to-[#B36B22] hover:from-[#B36B22] hover:to-[#995919] text-white shadow-[0_10px_25px_-5px_rgba(204,128,51,0.5)] flex items-center gap-2.5 font-bold transition-all duration-300 hover:scale-105 group"
    >
      <X v-if="open" class="w-6 h-6" />
      <template v-else>
        <MessageCircle class="w-6 h-6 group-hover:rotate-12 transition-transform" /> 
        <span class="hidden sm:inline text-sm tracking-wide">Hỏi AI tư vấn</span>
      </template>
    </button>

    <!-- Khung Chat -->
    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="transform scale-95 opacity-0 translate-y-4"
      enter-to-class="transform scale-100 opacity-100 translate-y-0"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="transform scale-100 opacity-100 translate-y-0"
      leave-to-class="transform scale-95 opacity-0 translate-y-4"
    >
      <div v-if="open" class="fixed bottom-24 right-6 z-50 w-[380px] max-w-[calc(100vw-3rem)] h-[600px] max-h-[calc(100vh-8rem)] bg-[#FAF6F0] rounded-2xl shadow-2xl border border-[#EAE3D9]/60 flex flex-col overflow-hidden ring-1 ring-black/5">
        
        <!-- Header -->
        <header class="bg-gradient-to-br from-[#4A3224] to-[#2A231E] px-5 py-4 flex items-center gap-3 relative overflow-hidden">
          <div class="absolute inset-0 bg-[url('https://www.transparenttextures.com/patterns/cubes.png')] opacity-10 pointer-events-none"></div>
          <div class="relative w-10 h-10 rounded-full bg-gradient-to-b from-[#CC8033] to-[#995919] flex items-center justify-center shadow-inner border border-white/20">
            <Sparkles class="w-5 h-5 text-white" />
            <div class="absolute -bottom-0.5 -right-0.5 w-3 h-3 bg-green-500 border-2 border-[#2A231E] rounded-full"></div>
          </div>
          <div class="flex-1 relative z-10">
            <div class="font-premium-serif font-bold text-white tracking-wide text-lg leading-tight">{{ storeInfo.tenAI }}</div>
            <div class="text-xs text-[#EAE3D9] font-medium flex items-center gap-1.5 mt-0.5">
              <span class="w-1.5 h-1.5 rounded-full bg-green-400 animate-pulse"></span>
              Luôn sẵn sàng hỗ trợ
            </div>
          </div>
          <button @click="open = false" class="relative z-10 w-8 h-8 rounded-full bg-white/10 hover:bg-white/20 flex items-center justify-center text-white/80 hover:text-white transition-colors">
            <X class="w-4 h-4" stroke-width="2.5" />
          </button>
        </header>

        <!-- Body -->
        <div class="flex-1 overflow-y-auto p-4 space-y-5 bg-[#FAF6F0] scroll-smooth">
          <div v-for="(m, i) in msgs" :key="i" :class="['flex', m.role === 'user' ? 'justify-end' : 'justify-start']">
            
            <div :class="[
              'max-w-[88%] px-4 py-3 text-[14px] leading-relaxed shadow-sm relative',
              m.role === 'user' 
                ? 'bg-gradient-to-r from-[#CC8033] to-[#B36B22] text-white rounded-2xl rounded-tr-sm' 
                : 'bg-white border border-[#EAE3D9] text-[#2A231E] rounded-2xl rounded-tl-sm'
            ]">
              <p class="font-medium">{{ m.text }}</p>
              
              <!-- Recommend Items (Bot only) -->
              <div v-if="m.recommendItems" class="mt-3 space-y-2.5">
                <div v-for="it in m.recommendItems" :key="it.id" class="flex items-center gap-3 bg-[#FAF6F0] rounded-xl p-2 border border-[#EAE3D9] hover:border-[#CC8033]/30 transition-colors group">
                  <img :src="it.image" :alt="it.name" class="w-12 h-12 rounded-lg object-cover shadow-sm group-hover:scale-105 transition-transform" />
                  <div class="flex-1 min-w-0">
                    <div class="text-xs font-bold text-[#1A1512] truncate">{{ it.name }}</div>
                    <div class="text-xs font-bold text-[#CC8033] mt-0.5">{{ it.price.toLocaleString("vi-VN") }}₫</div>
                  </div>
                  <button
                    @click="addToCart(it)"
                    class="w-8 h-8 rounded-full bg-[#2A231E] text-white flex items-center justify-center hover:bg-[#CC8033] transition-colors shadow-sm shrink-0"
                    title="Thêm vào giỏ"
                  >
                    <Plus class="w-4 h-4" stroke-width="2.5" />
                  </button>
                </div>
              </div>

              <!-- Suggestions (Bot only) -->
              <div v-if="m.suggestions" class="mt-3 flex flex-wrap gap-2">
                <button v-for="s in m.suggestions" :key="s" @click="send(s)" class="text-[11px] font-bold px-3 py-1.5 rounded-full bg-[#FDFBF7] border border-[#CC8033]/30 text-[#CC8033] hover:bg-[#CC8033] hover:text-white transition-all shadow-sm active:scale-95">
                  {{ s }}
                </button>
              </div>
            </div>
          </div>

          <!-- Typing Indicator -->
          <div v-if="typing" class="flex justify-start">
            <div class="bg-white border border-[#EAE3D9] px-4 py-3.5 rounded-2xl rounded-tl-sm shadow-sm flex items-center gap-1.5">
              <span v-for="d in [0, 0.15, 0.3]" :key="d" class="w-1.5 h-1.5 rounded-full bg-[#CC8033]/60 animate-bounce" :style="{ animationDelay: `${d}s` }" />
            </div>
          </div>
        </div>

        <!-- Input Area -->
        <div class="bg-white border-t border-[#EAE3D9] p-3.5">
          <form
            @submit.prevent="send(input)"
            class="flex items-center gap-2"
          >
            <input
              v-model="input"
              placeholder="Nhập tin nhắn..."
              class="flex-1 h-11 px-4 rounded-full bg-[#FAF6F0] border border-[#EAE3D9] text-sm text-[#2A231E] font-medium outline-none focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033] transition-all placeholder:text-[#C5BEB8]"
            />
            <button 
              type="submit" 
              :disabled="!input.trim()"
              class="w-11 h-11 rounded-full bg-[#2A231E] hover:bg-[#CC8033] disabled:bg-[#D5CEC4] disabled:cursor-not-allowed text-white flex items-center justify-center transition-all shadow-sm shrink-0"
            >
              <Send class="w-4 h-4 -ml-0.5" stroke-width="2.5" />
            </button>
          </form>
          <div class="text-center text-[9px] font-bold uppercase tracking-widest text-[#C5BEB8] mt-2">
            Powered by AI <Sparkles class="inline w-2.5 h-2.5 text-[#CC8033] mb-0.5" />
          </div>
        </div>

      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { MessageCircle, X, Send, Sparkles, Plus } from 'lucide-vue-next'
import { useCartStore } from '@/stores/cart'
import { menuItems } from '@/data/menu'
import { ordersApi, type MenuItem } from '@/services/orders'
import { useStoreInfoStore } from '@/stores/storeInfo'

interface Msg {
  role: "bot" | "user"
  text: string
  suggestions?: string[]
  recommendItems?: any[]
}

const toast = { success: (msg: string) => alert('Thành công: ' + msg) }
const storeInfo = useStoreInfoStore()

const initialMsgs = computed<Msg[]>(() => [
  {
    role: "bot",
    text: `Xin chào! Tôi là ${storeInfo.tenAI} ☕ Tôi có thể giúp bạn chọn món hợp gu hôm nay. Bạn muốn gì?`,
    suggestions: ["Món phổ biến hôm nay", "Gợi ý cho người ít ngọt", "Combo cà phê + bánh", "Tôi muốn gì đó mát lạnh"],
  },
])

const realMenu = ref<MenuItem[]>([])

onMounted(async () => {
  try {
    const data = await ordersApi.menu()
    realMenu.value = data.filter((m: any) => m.kieuMon !== 'Topping')
  } catch {}
})

const respond = async (q: string): Promise<Msg> => {
  const lower = q.toLowerCase()

  const getMappedRealMenu = (filterFn: (m: MenuItem) => boolean) => {
      return realMenu.value.filter(filterFn).map(m => ({
          id: String(m.maSanPham),
          name: m.tenSanPham,
          price: m.giaBan,
          image: m.hinhAnh || "https://via.placeholder.com/150"
      }))
  }

  // Giữ lại các thẻ gợi ý trực quan (có nút Add to Cart) cho các luồng cơ bản
  if (lower.includes("phổ biến") || lower.includes("bán chạy")) {
    return {
      role: "bot",
      text: "Hôm nay các món này được order nhiều nhất:",
      recommendItems: getMappedRealMenu(m => true).slice(0, 3),
    }
  }
  if (lower.includes("ít ngọt")) {
    return {
      role: "bot",
      text: "Với người ít ngọt, mình gợi ý vài món đậm vị cà phê nhé:",
      recommendItems: getMappedRealMenu(m => (m.tenDanhMuc || '').toLowerCase().includes("cà phê")).slice(0, 2),
    }
  }
  if (lower.includes("mát") || lower.includes("lạnh") || lower.includes("đá xay")) {
    return {
      role: "bot",
      text: "Hôm nay nóng quá! Mời bạn thử các món đá xay refresh nè:",
      recommendItems: getMappedRealMenu(m => (m.tenDanhMuc || '').toLowerCase().includes("đá xay") || (m.tenDanhMuc || '').toLowerCase().includes("trà")).slice(0, 3),
    }
  }
  if (lower.includes("combo") || lower.includes("bánh")) {
    return {
      role: "bot",
      text: "Bạn thử dùng nước kết hợp với một chút bánh xem sao nhé 😋",
      recommendItems: getMappedRealMenu(m => (m.tenDanhMuc || '').toLowerCase().includes("bánh")).slice(0, 2),
    }
  }

  // Nếu không khớp các lệnh đặc biệt -> Gọi AI Gemini ở Backend xử lý!
  try {
    const res = await fetch('/api/chatbot/ask', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message: q })
    })
    
    if (!res.ok) {
        let errStr = 'Lỗi hệ thống AI'
        try {
            const errJson = await res.json()
            errStr = errJson.message || errStr
        } catch {
            errStr = 'Không thể kết nối đến máy chủ AI (Có thể cần khởi động lại Backend)'
        }
        throw new Error(errStr)
    }
    
    const data = await res.json()
    const botReply: Msg = { role: "bot", text: data.reply }
    
    // Nếu AI có gợi ý ID món ăn, ánh xạ sang dạng thẻ giao diện
    if (data.recommendedIds && Array.isArray(data.recommendedIds) && data.recommendedIds.length > 0) {
        const mappedItems = realMenu.value
            .filter(m => data.recommendedIds.includes(m.maSanPham))
            .map(m => ({
                id: String(m.maSanPham),
                name: m.tenSanPham,
                price: m.giaBan,
                image: m.hinhAnh || "https://via.placeholder.com/150"
            }))
        if (mappedItems.length > 0) {
            botReply.recommendItems = mappedItems
        }
    }
    return botReply
  } catch (error: any) {
      return {
          role: "bot",
          text: error.message || "Xin lỗi, não bộ AI của mình đang tải hơi chậm 😅 Bạn thử bấm các gợi ý bên trên nhé!",
          suggestions: ["Món phổ biến hôm nay", "Tôi muốn gì đó mát lạnh"]
      }
  }
}

const open = ref(false)
const msgs = ref<Msg[]>([...initialMsgs.value])
const input = ref("")
const typing = ref(false)
const cart = useCartStore()

const send = async (text: string) => {
  if (!text.trim()) return
  msgs.value.push({ role: "user", text })
  input.value = ""
  typing.value = true
  
  const botReply = await respond(text)
  msgs.value.push(botReply)
  typing.value = false
}

const addToCart = (it: any) => {
  cart.add(it)
  toast.success(`Đã thêm ${it.name}`)
}
</script>
