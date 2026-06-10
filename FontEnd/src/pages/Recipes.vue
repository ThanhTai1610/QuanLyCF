<template>
  <div class="space-y-6 md:space-y-8 font-premium-sans text-[#2A231E] p-4 sm:p-6 lg:p-8 max-w-[1400px] mx-auto min-h-screen flex flex-col">
    
    <!-- Tiêu đề và Tabs -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div class="flex bg-white rounded-lg p-1 border border-[#EAE3D9] shadow-sm">
        <button 
          @click="activeTab = 'product_recipes'"
          :class="[
            'px-6 py-2.5 rounded-md text-sm font-bold transition-all duration-300',
            activeTab === 'product_recipes' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]'
          ]"
        >
          Công thức Món ăn / Đồ uống
        </button>
        <button 
          @click="activeTab = 'batch_formulas'"
          :class="[
            'px-6 py-2.5 rounded-md text-sm font-bold transition-all duration-300',
            activeTab === 'batch_formulas' ? 'bg-[#CC8033] text-white shadow-md' : 'text-[#8A8178] hover:text-[#2A231E] hover:bg-[#FDFBF7]'
          ]"
        >
          Mẻ nấu Topping / Bán thành phẩm
        </button>
      </div>
      
      <button 
        v-if="activeTab === 'batch_formulas'"
        class="flex items-center justify-center bg-[#2A231E] hover:bg-[#3D332A] text-white h-11 px-6 rounded-lg shadow-md transition-colors text-xs font-bold uppercase tracking-wider whitespace-nowrap"
      >
        <Plus class="w-4 h-4 mr-2" /> Tạo Mẻ Nấu Mới
      </button>
    </div>

    <!-- TAB 1: Công thức Món ăn / Đồ uống (Split View) -->
    <div v-show="activeTab === 'product_recipes'" class="flex-1 flex flex-col lg:flex-row gap-6 animate-in fade-in slide-in-from-bottom-4 duration-500 h-[calc(100vh-140px)]">
      
      <!-- Cột Trái: Danh sách Menu -->
      <div class="w-full lg:w-1/3 xl:w-1/4 bg-white border border-[#EAE3D9] rounded-xl shadow-sm flex flex-col overflow-hidden flex-shrink-0">
        <div class="p-4 border-b border-[#EAE3D9] bg-[#FDFBF7]">
          <div class="relative w-full">
            <Search class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-[#8A8178]" stroke-width="2" />
            <input
              placeholder="Tìm món trong thực đơn..."
              class="pl-10 w-full bg-white border border-[#EAE3D9] h-10 rounded-lg text-sm font-medium focus:outline-none focus:ring-2 focus:ring-[#CC8033]/20 focus:border-[#CC8033] transition-all shadow-inner"
            />
          </div>
          <!-- Phân loại menu -->
          <div class="flex gap-2 mt-3 overflow-x-auto custom-scrollbar pb-1">
            <button class="px-3 py-1.5 bg-[#CC8033] text-white rounded-md text-xs font-bold whitespace-nowrap">Tất cả</button>
            <button class="px-3 py-1.5 bg-[#EAE3D9]/50 text-[#5C544E] hover:bg-[#EAE3D9] rounded-md text-xs font-bold whitespace-nowrap transition-colors">Cà phê</button>
            <button class="px-3 py-1.5 bg-[#EAE3D9]/50 text-[#5C544E] hover:bg-[#EAE3D9] rounded-md text-xs font-bold whitespace-nowrap transition-colors">Trà sữa</button>
            <button class="px-3 py-1.5 bg-[#EAE3D9]/50 text-[#5C544E] hover:bg-[#EAE3D9] rounded-md text-xs font-bold whitespace-nowrap transition-colors">Bánh ngọt</button>
          </div>
        </div>

        <!-- Danh sách món -->
        <div class="flex-1 overflow-y-auto custom-scrollbar p-2 space-y-1">
          <!-- Item 1 (Selected - Active State) -->
          <button class="w-full flex items-center gap-3 p-3 rounded-lg bg-[#CC8033] border border-[#CC8033] text-left transition-all shadow-md group">
            <div class="w-10 h-10 rounded-md bg-white flex flex-shrink-0 items-center justify-center shadow-sm">
              <Coffee class="w-5 h-5 text-[#CC8033]" />
            </div>
            <div class="flex-1 min-w-0">
              <p class="font-bold text-white text-sm truncate">Cà phê Bạc xỉu</p>
              <p class="text-[10px] font-bold text-white/80 uppercase tracking-wider">Đã cài định mức</p>
            </div>
          </button>
          
          <!-- Item 2 -->
          <button class="w-full flex items-center gap-3 p-3 rounded-lg hover:bg-[#FDFBF7] border border-transparent text-left transition-colors group">
            <div class="w-10 h-10 rounded-md bg-[#FDFBF7] border border-[#EAE3D9] flex flex-shrink-0 items-center justify-center group-hover:border-[#CC8033]/50 transition-colors">
              <CupSoda class="w-5 h-5 text-[#8A8178]" />
            </div>
            <div class="flex-1 min-w-0">
              <p class="font-bold text-[#2A231E] text-sm truncate">Trà sữa Trân châu đường đen</p>
              <p class="text-[10px] font-medium text-[#8A8178] uppercase tracking-wider">Chưa hoàn thiện</p>
            </div>
          </button>

          <!-- Item 3 -->
          <button class="w-full flex items-center gap-3 p-3 rounded-lg hover:bg-[#FDFBF7] border border-transparent text-left transition-colors group">
            <div class="w-10 h-10 rounded-md bg-[#FDFBF7] border border-[#EAE3D9] flex flex-shrink-0 items-center justify-center group-hover:border-[#CC8033]/50 transition-colors">
              <Coffee class="w-5 h-5 text-[#8A8178]" />
            </div>
            <div class="flex-1 min-w-0">
              <p class="font-bold text-[#2A231E] text-sm truncate">Espresso (Nóng)</p>
              <p class="text-[10px] font-medium text-[#CC8033] uppercase tracking-wider">Đã cài định mức</p>
            </div>
          </button>
        </div>
      </div>

      <!-- Cột Phải: Giao diện cấu hình định mức chi tiết -->
      <div class="w-full lg:w-2/3 xl:w-3/4 flex flex-col bg-white border border-[#EAE3D9] rounded-xl shadow-sm overflow-hidden flex-1">
        
        <!-- Header Cấu hình -->
        <div class="px-6 py-5 border-b border-[#EAE3D9] bg-[#FDFBF7] flex justify-between items-center">
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-lg bg-white border border-[#EAE3D9] flex items-center justify-center shadow-sm">
              <Coffee class="w-6 h-6 text-[#2A231E]" />
            </div>
            <div>
              <h2 class="text-xl font-bold text-[#2A231E] font-display">Cà phê Bạc xỉu</h2>
              <p class="text-xs text-[#8A8178] mt-1">Sản phẩm sẽ tự động trừ kho nguyên liệu dựa trên các cấu hình bên dưới.</p>
            </div>
          </div>
          <button class="bg-[#2A231E] hover:bg-[#3D332A] text-white px-5 py-2 rounded-lg font-bold text-xs uppercase tracking-wider transition-colors shadow-sm">
            Lưu cấu hình
          </button>
        </div>

        <div class="flex-1 overflow-y-auto custom-scrollbar p-6 space-y-8">
          
          <!-- Phần 1: Nhóm định mức gốc theo Size -->
          <div>
            <div class="flex items-center justify-between mb-4">
              <h3 class="font-bold text-[#2A231E] text-base flex items-center gap-2">
                <Box class="w-5 h-5 text-[#CC8033]" /> Nhóm Định Mức Gốc (Base Recipe)
              </h3>
              <!-- Chọn Size -->
              <div class="flex bg-[#FDFBF7] rounded-md p-1 border border-[#EAE3D9]">
                <button class="px-4 py-1.5 bg-white border border-[#EAE3D9] shadow-sm rounded text-xs font-bold text-[#2A231E]">Size M</button>
                <button class="px-4 py-1.5 text-xs font-bold text-[#8A8178] hover:text-[#2A231E] rounded transition-colors">Size L</button>
              </div>
            </div>

            <div class="border border-[#EAE3D9] rounded-xl overflow-hidden">
              <table class="w-full text-sm text-left">
                <thead class="bg-[#FDFBF7] border-b border-[#EAE3D9] text-[#8A8178] text-[10px] uppercase tracking-wider">
                  <tr>
                    <th class="px-5 py-3 font-bold">Mã NL</th>
                    <th class="px-5 py-3 font-bold">Tên Nguyên Liệu Tồn Kho</th>
                    <th class="px-5 py-3 font-bold text-right w-32">Định mức</th>
                    <th class="px-5 py-3 font-bold w-24">Đơn vị</th>
                    <th class="px-5 py-3 font-bold text-center w-16">Thao tác</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-[#EAE3D9]/60">
                  <tr class="hover:bg-gray-50 transition-colors">
                    <td class="px-5 py-3 font-mono text-xs text-[#8A8178]">RAW-CF-001</td>
                    <td class="px-5 py-3 font-bold text-[#2A231E]">Hạt cà phê Robusta</td>
                    <td class="px-5 py-3">
                      <input type="number" value="25" class="w-full text-right bg-white border border-[#EAE3D9] h-9 rounded-md px-2 text-sm font-bold text-[#CC8033] focus:outline-none focus:border-[#CC8033]" />
                    </td>
                    <td class="px-5 py-3 font-medium text-xs text-[#5C544E] uppercase">g</td>
                    <td class="px-5 py-3 text-center">
                      <button class="text-red-400 hover:text-red-600"><Trash2 class="w-4 h-4" /></button>
                    </td>
                  </tr>
                  <tr class="hover:bg-gray-50 transition-colors">
                    <td class="px-5 py-3 font-mono text-xs text-[#8A8178]">RAW-MK-005</td>
                    <td class="px-5 py-3 font-bold text-[#2A231E]">Sữa đặc Ngôi sao Phương Nam</td>
                    <td class="px-5 py-3">
                      <input type="number" value="40" class="w-full text-right bg-white border border-[#EAE3D9] h-9 rounded-md px-2 text-sm font-bold text-[#CC8033] focus:outline-none focus:border-[#CC8033]" />
                    </td>
                    <td class="px-5 py-3 font-medium text-xs text-[#5C544E] uppercase">g</td>
                    <td class="px-5 py-3 text-center">
                      <button class="text-red-400 hover:text-red-600"><Trash2 class="w-4 h-4" /></button>
                    </td>
                  </tr>
                  <tr class="hover:bg-gray-50 transition-colors">
                    <td class="px-5 py-3 font-mono text-xs text-[#8A8178]">RAW-FM-002</td>
                    <td class="px-5 py-3 font-bold text-[#2A231E]">Sữa tươi thanh trùng</td>
                    <td class="px-5 py-3">
                      <input type="number" value="60" class="w-full text-right bg-white border border-[#EAE3D9] h-9 rounded-md px-2 text-sm font-bold text-[#CC8033] focus:outline-none focus:border-[#CC8033]" />
                    </td>
                    <td class="px-5 py-3 font-medium text-xs text-[#5C544E] uppercase">ml</td>
                    <td class="px-5 py-3 text-center">
                      <button class="text-red-400 hover:text-red-600"><Trash2 class="w-4 h-4" /></button>
                    </td>
                  </tr>
                  <tr class="hover:bg-gray-50 transition-colors">
                    <td class="px-5 py-3 font-mono text-xs text-[#8A8178]">SUP-CUP-01</td>
                    <td class="px-5 py-3 font-bold text-[#2A231E]">Ly nhựa PET 500ml</td>
                    <td class="px-5 py-3">
                      <input type="number" value="1" class="w-full text-right bg-white border border-[#EAE3D9] h-9 rounded-md px-2 text-sm font-bold text-[#CC8033] focus:outline-none focus:border-[#CC8033]" />
                    </td>
                    <td class="px-5 py-3 font-medium text-xs text-[#5C544E] uppercase">Chiếc</td>
                    <td class="px-5 py-3 text-center">
                      <button class="text-red-400 hover:text-red-600"><Trash2 class="w-4 h-4" /></button>
                    </td>
                  </tr>
                </tbody>
              </table>
              <div class="p-3 bg-[#FDFBF7] border-t border-[#EAE3D9] text-center">
                <button class="text-[#CC8033] text-xs font-bold uppercase tracking-wider hover:text-[#B87029] transition-colors flex items-center justify-center gap-1 mx-auto">
                  <Plus class="w-4 h-4" /> Thêm Nguyên Liệu Cho Size M
                </button>
              </div>
            </div>
          </div>

          <!-- Phần 2: Nhóm định mức Option / Modifier -->
          <div>
            <div class="flex items-center justify-between mb-4">
              <h3 class="font-bold text-[#2A231E] text-base flex items-center gap-2">
                <Settings2 class="w-5 h-5 text-[#8A8178]" /> Nhóm Định Mức Gọi Thêm (Modifiers)
              </h3>
              <button class="px-3 py-1.5 border border-[#EAE3D9] bg-white text-[#5C544E] rounded-md text-xs font-bold hover:bg-[#FDFBF7] transition-colors shadow-sm flex items-center gap-1">
                <Plus class="w-3.5 h-3.5" /> Tạo Option Mới
              </button>
            </div>

            <div class="space-y-4">
              <!-- Option 1: Thêm sữa (Collapsed / Xổ ra) -->
              <div class="border border-[#EAE3D9] rounded-xl overflow-hidden shadow-sm transition-all duration-300">
                <button @click="toggleOption(1)" class="w-full px-4 py-3 bg-[#FDFBF7] hover:bg-[#FDFBF7]/80 border-b border-[#EAE3D9] flex justify-between items-center transition-colors">
                  <div class="flex items-center gap-3">
                    <div class="w-6 h-6 rounded bg-[#E8C5A5]/30 text-[#CC8033] flex items-center justify-center transition-transform" :class="isOption1Open ? 'rotate-180' : ''">
                      <ChevronDown class="w-4 h-4" />
                    </div>
                    <span class="font-bold text-[#2A231E] text-sm text-left">Option: Sữa đặc thêm (Extra)</span>
                  </div>
                  <span class="text-[10px] bg-white border border-[#EAE3D9] text-[#CC8033] font-bold px-2.5 py-1 rounded-md uppercase tracking-wider shadow-sm">+ 5,000đ / Khách gọi</span>
                </button>
                
                <div v-show="isOption1Open" class="p-4 bg-white animate-in slide-in-from-top-2">
                  <div class="flex items-end gap-4">
                    <div class="flex-1">
                      <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1">Kéo nguyên liệu từ kho</label>
                      <select class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033]">
                        <option selected>Sữa đặc Ngôi sao Phương Nam (RAW-MK-005)</option>
                      </select>
                    </div>
                    <div class="w-24">
                      <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1">Định mức trừ</label>
                      <input type="number" value="20" class="w-full text-right bg-[#FFF9F2] border border-[#E8C5A5] h-10 rounded-lg px-3 text-base font-bold text-red-500 focus:outline-none" />
                    </div>
                    <!-- Disable field for Unit (Smart FE logic) -->
                    <div class="w-20">
                       <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1">Đơn vị</label>
                       <input type="text" value="G" disabled class="w-full text-center bg-gray-100 border border-[#EAE3D9] h-10 rounded-lg px-2 text-sm font-bold text-[#8A8178] uppercase cursor-not-allowed" title="Tự động khóa theo đơn vị kho" />
                    </div>
                    <div class="pb-1">
                      <button class="p-2.5 text-red-400 hover:text-red-600 rounded-lg hover:bg-red-50 transition-colors border border-transparent hover:border-red-100"><Trash2 class="w-5 h-5" /></button>
                    </div>
                  </div>
                  <p class="text-xs text-[#8A8178] mt-4 flex items-start gap-1.5 bg-[#FDFBF7] p-3 rounded-lg border border-[#EAE3D9]/50">
                    <Info class="w-4 h-4 text-[#CC8033] flex-shrink-0" />
                    <span>Hệ thống sẽ tự động trừ thêm <strong class="text-[#2A231E]">20 G</strong> Sữa đặc vào lệnh xuất kho khi Bill có chọn Option này. Đơn vị "G" bị khóa theo gốc để tránh sai lệch tồn kho.</span>
                  </p>
                </div>
              </div>
              
              <!-- Option 2: Thêm Cà phê (Collapsed) -->
              <div class="border border-[#EAE3D9] rounded-xl overflow-hidden shadow-sm transition-all duration-300">
                <button @click="toggleOption(2)" class="w-full px-4 py-3 bg-[#FDFBF7] hover:bg-[#FDFBF7]/80 border-b border-[#EAE3D9] flex justify-between items-center transition-colors">
                  <div class="flex items-center gap-3">
                    <div class="w-6 h-6 rounded bg-[#E8C5A5]/30 text-[#CC8033] flex items-center justify-center transition-transform" :class="isOption2Open ? 'rotate-180' : ''">
                      <ChevronDown class="w-4 h-4" />
                    </div>
                    <span class="font-bold text-[#2A231E] text-sm text-left">Option: Thêm Shot Espresso</span>
                  </div>
                  <span class="text-[10px] bg-white border border-[#EAE3D9] text-[#CC8033] font-bold px-2.5 py-1 rounded-md uppercase tracking-wider shadow-sm">+ 10,000đ / Khách gọi</span>
                </button>
                
                <div v-show="isOption2Open" class="p-4 bg-white animate-in slide-in-from-top-2">
                  <div class="flex items-end gap-4">
                    <div class="flex-1">
                      <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1">Kéo nguyên liệu từ kho</label>
                      <select class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#CC8033]">
                        <option selected>Hạt cà phê Robusta (RAW-CF-001)</option>
                      </select>
                    </div>
                    <div class="w-24">
                      <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1">Định mức trừ</label>
                      <input type="number" value="15" class="w-full text-right bg-[#FFF9F2] border border-[#E8C5A5] h-10 rounded-lg px-3 text-base font-bold text-red-500 focus:outline-none" />
                    </div>
                    <!-- Disable field for Unit -->
                    <div class="w-20">
                       <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1">Đơn vị</label>
                       <input type="text" value="G" disabled class="w-full text-center bg-gray-100 border border-[#EAE3D9] h-10 rounded-lg px-2 text-sm font-bold text-[#8A8178] uppercase cursor-not-allowed" />
                    </div>
                    <div class="pb-1">
                      <button class="p-2.5 text-red-400 hover:text-red-600 rounded-lg hover:bg-red-50 transition-colors border border-transparent hover:border-red-100"><Trash2 class="w-5 h-5" /></button>
                    </div>
                  </div>
                </div>
              </div>

            </div>
          </div>

        </div>
      </div>
    </div>

    <!-- TAB 2: Định mức mẻ nấu Topping / Bán thành phẩm -->
    <div v-show="activeTab === 'batch_formulas'" class="animate-in fade-in slide-in-from-bottom-4 duration-500">
      
      <div class="bg-white rounded-xl border border-[#EAE3D9] shadow-sm overflow-hidden mb-6">
        <div class="px-6 py-5 border-b border-[#EAE3D9] bg-[#FDFBF7]">
          <h2 class="text-lg font-bold text-[#2A231E]">Thiết lập công thức sản xuất mẻ (Batch Production)</h2>
          <p class="text-xs text-[#8A8178] mt-1">Quản lý định mức nguyên liệu thô để nấu ra bán thành phẩm/topping nhập lại vào kho.</p>
        </div>
        
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            
            <!-- Đầu vào & Đầu ra -->
            <div class="space-y-6">
              <div class="bg-[#FDFBF7] border border-[#EAE3D9] rounded-xl p-5 shadow-inner">
                <h3 class="font-bold text-[#2A231E] text-sm mb-4 border-b border-[#EAE3D9] pb-2 uppercase tracking-wider flex items-center gap-2">
                  <ArrowDownToLine class="w-4 h-4 text-[#4A7C59]" /> Kết quả đầu ra (Thành phẩm)
                </h3>
                <div class="space-y-4">
                  <div>
                    <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1.5">Tên bán thành phẩm (Lưu vào kho)</label>
                    <input type="text" value="Trân châu đen chín" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-bold text-[#2A231E] focus:outline-none focus:border-[#4A7C59]" />
                  </div>
                  <div class="flex gap-4">
                    <div class="flex-1">
                      <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1.5">Sản lượng sinh ra / Mẻ</label>
                      <input type="number" value="30" class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-bold text-[#4A7C59] focus:outline-none focus:border-[#4A7C59]" />
                    </div>
                    <div class="w-32">
                      <label class="text-[10px] font-bold uppercase tracking-wider text-[#8A8178] block mb-1.5">Đơn vị lưu kho</label>
                      <select class="w-full bg-white border border-[#EAE3D9] h-10 rounded-lg px-3 text-sm font-medium focus:outline-none">
                        <option selected>Phần (Portion)</option>
                        <option>Khay</option>
                        <option>g</option>
                      </select>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Nguyên liệu thô (Công thức nấu) -->
            <div>
              <h3 class="font-bold text-[#2A231E] text-sm mb-4 border-b border-[#EAE3D9] pb-2 uppercase tracking-wider flex items-center gap-2">
                <Flame class="w-4 h-4 text-[#CC8033]" /> Nguyên liệu tiêu hao để nấu
              </h3>
              
              <div class="space-y-3">
                <div class="flex items-center gap-3">
                  <select class="flex-1 bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-medium focus:outline-none">
                    <option selected>Trân châu sống (RAW-TC-01)</option>
                  </select>
                  <input type="number" value="1000" class="w-24 text-right bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-bold text-red-500 focus:outline-none" />
                  <span class="w-10 text-xs font-medium text-[#5C544E] uppercase">g</span>
                  <button class="p-2 text-red-400 hover:text-red-600"><Trash2 class="w-4 h-4" /></button>
                </div>
                
                <div class="flex items-center gap-3">
                  <select class="flex-1 bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-medium focus:outline-none">
                    <option selected>Đường nâu (RAW-SUG-02)</option>
                  </select>
                  <input type="number" value="200" class="w-24 text-right bg-white border border-[#EAE3D9] h-10 rounded-md px-3 text-sm font-bold text-red-500 focus:outline-none" />
                  <span class="w-10 text-xs font-medium text-[#5C544E] uppercase">g</span>
                  <button class="p-2 text-red-400 hover:text-red-600"><Trash2 class="w-4 h-4" /></button>
                </div>
                
                <div class="pt-2">
                  <button class="text-[#CC8033] text-xs font-bold uppercase tracking-wider hover:text-[#B87029] transition-colors flex items-center gap-1">
                    <Plus class="w-4 h-4" /> Thêm nguyên liệu thô
                  </button>
                </div>
              </div>
              
              <div class="mt-6 bg-[#FFF9F2] border border-[#E8C5A5] rounded-lg p-3 text-xs text-[#5C544E]">
                <strong class="text-[#CC8033]">Quy trình tự động:</strong> Khi nhân viên Bếp hoàn tất 1 mẻ nấu, hệ thống sẽ tự động trừ đi <strong class="text-[#2A231E]">1000g Trân châu sống</strong> & <strong class="text-[#2A231E]">200g Đường nâu</strong>, đồng thời cộng thêm <strong class="text-[#2A231E]">30 Phần</strong> "Trân châu đen chín" vào kho Bán thành phẩm.
              </div>
            </div>

          </div>
        </div>
        <div class="px-6 py-4 border-t border-[#EAE3D9] bg-gray-50 flex justify-end gap-3">
          <button class="px-6 py-2 rounded-lg border border-[#EAE3D9] text-[#5C544E] text-xs font-bold uppercase tracking-wider hover:bg-white transition-colors bg-[#FDFBF7]">
            Hủy
          </button>
          <button class="px-8 py-2 rounded-lg border border-[#2A231E] bg-[#2A231E] text-white text-xs font-bold uppercase tracking-wider hover:bg-black transition-colors shadow-md">
            Lưu Công Thức
          </button>
        </div>
      </div>

    </div>

  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Plus, 
  Search, 
  Coffee, 
  CupSoda, 
  Box, 
  Trash2, 
  Settings2,
  ArrowDownToLine,
  Flame,
  ChevronDown,
  Info
} from 'lucide-vue-next'

const activeTab = ref<'product_recipes' | 'batch_formulas'>('product_recipes')

const isOption1Open = ref(true)
const isOption2Open = ref(false)

const toggleOption = (id: number) => {
  if (id === 1) isOption1Open.value = !isOption1Open.value
  if (id === 2) isOption2Open.value = !isOption2Open.value
}
</script>

<style scoped>
.font-premium-sans {
  font-family: 'Be Vietnam Pro', system-ui, sans-serif;
}

.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background-color: #EAE3D9;
  border-radius: 10px;
}
.custom-scrollbar:hover::-webkit-scrollbar-thumb {
  background-color: #D5C9B3;
}
</style>
