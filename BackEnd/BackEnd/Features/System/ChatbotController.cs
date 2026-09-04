using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BackEnd.Features.System
{
    [ApiController]
    [Route("api/chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly QuanLyCFDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly SettingService _settingService;

        private static int _currentKeyIndex = 0;
        private static readonly object _keyLock = new object();

        public ChatbotController(IConfiguration config, QuanLyCFDbContext dbContext, HttpClient httpClient, SettingService settingService)
        {
            _config = config;
            _dbContext = dbContext;
            _httpClient = httpClient;
            _settingService = settingService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { message = "Tin nhắn không được để trống." });

            var sanPhams = await _dbContext.SanPhams
                .Where(s => s.TrangThaiBan && s.KieuMon != "Topping")
                .Select(s => new { s.MaSanPham, s.TenSanPham, s.GiaBan })
                .ToListAsync();

            var sanPhamsDynamic = sanPhams.Select(s => (dynamic)s).ToList();

            // Đọc key từ appsettings hoặc biến môi trường (hỗ trợ nhiều key xoay vòng)
            var rawKeys = _config["GEMINI_API_KEYS"] ?? _config["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEYS") ?? "";
            var apiKeys = rawKeys.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k) && !k.Contains("YOUR_GEMINI_API_KEY"))
                .Distinct()
                .ToList();

            if (apiKeys.Count == 0)
            {
                return GenerateFallbackResponse(request.Message, sanPhamsDynamic);
            }

            var menuContext = string.Join("\n", sanPhams.Select(s => $"- ID: {s.MaSanPham} | {s.TenSanPham}: {s.GiaBan:N0}đ"));

            var storeInfo = await _settingService.GetStoreInfoAsync();
            var tenQuan = storeInfo.TenQuan;
            var tenAI = storeInfo.TenAI;
            var xungHoAI = storeInfo.XungHoAI;
            var diaChi = storeInfo.DiaChi;
            var dienThoai = storeInfo.SoDienThoai;
            var gioMoCua = storeInfo.GioMoCua;
            var moTaQuan = storeInfo.MoTaQuan;

            var systemPrompt = $@"Bạn là '{tenAI}' - Barista AI sành điệu, hài hước, am hiểu đồ uống và vô cùng nhiệt tình của quán {tenQuan}.
THÔNG TIN VỀ QUÁN:
- Tên quán: {tenQuan}
- Địa chỉ: {diaChi}
- Số điện thoại: {dienThoai}
- Giờ mở cửa: {gioMoCua}
- Mô tả/Concept: {moTaQuan}
- Wifi: Miễn phí cực nhanh
- Món bán chạy nhất (Best seller): Bạc xỉu, Cà phê sữa đá, Hồng trà sữa, Ép cam.

THỰC ĐƠN HIỆN TẠI:
{menuContext}

NĂNG LỰC ĐẶC BIỆT CỦA BẠN (CỰC KỲ DỄ THƯƠNG & THÔNG MINH):
1. **ĐỐ VUI ĐỒ UỐNG & CÀ PHÊ (BEVERAGE RIDDLES & QUIZ)**:
   - Khi khách yêu cầu 'đố vui', 'cho câu đố', 'đố tôi', 'chơi game' hoặc hỏi câu đố: Hãy đố một câu đố ngắn dí dỏm về cà phê (Robusta vs Arabica, Bạc xỉu, Cappuccino, Matcha, Trà sữa...), hoặc đố mẹo tên món đồ uống trong thực đơn! Kèm 3-4 lựa chọn A, B, C, D vui nhộn.
   - Khi khách trả lời (VD: chọn A, B, C hoặc đoán tên món): Đánh giá câu trả lời (khen thưởng siêu ngọt ngào nếu đúng / dỗi vui dí dỏm nếu sai), tiết lộ bí mật pha chế thú vị và GỢI Ý MÓN NƯỚC tương ứng trong thực đơn.

2. **BÓI ĐỒ UỐNG THEO TÂM TRẠNG (BEVERAGE FORTUNE & MOOD MATCH)**:
   - Khi khách chia sẻ cảm xúc (buồn, vui, áp lực, buồn ngủ, trời mưa, giận người yêu...): Hãy bói tâm trạng dí dỏm + tư vấn món nước 'định mệnh' giúp chữa lành/tăng năng lượng từ thực đơn.

3. **TƯ VẤN KẾT HỢP (FOOD & DRINK PAIRING)**:
   - Gợi ý bánh ngọt, topping hoặc combo hoàn hảo hợp rơ với đồ uống khách chọn.

HƯỚNG DẪN QUAN TRỌNG:
1. Trả lời NGẮN GỌN (tối đa 3-4 câu), ngôn từ tự nhiên, ấm áp, hóm hỉnh và cuốn hút.
2. XƯNG HÔ MẶC ĐỊNH VỚI KHÁCH: {xungHoAI}
3. Nếu khách hỏi về món ăn/thức uống, CHỈ tư vấn những món có trong THỰC ĐƠN ở trên.
4. KHI KHÁCH HỎI CÁC CÂU HỎI NGOÀI LỀ KHÔNG LIÊN QUAN ĐẾN CÀ PHÊ, ĐỒ UỐNG, MENU HOẶC QUÁN (như lịch sử, con người, chính trị, toán học, thời tiết...): Hãy lịch sự từ chối khéo léo: ""Dạ xin lỗi bạn nha, mình là Barista AI chỉ am hiểu về đồ uống, bánh ngọt và thông tin của quán thôi nè! ☕ Mình không nắm được thông tin ngoài lề này. Bạn có muốn mình tư vấn một món nước ngon cho hôm nay không ạ?""
5. TUYỆT ĐỐI KHÔNG viết ID của món vào câu trả lời (phần 'reply'). Chỉ nhắc tên món.
6. Khi bạn gợi ý món (kể cả khi đố vui hoặc bói món), hãy lấy ID của món đó ở THỰC ĐƠN và chỉ điền vào mảng 'recommend_item_ids'.
7. BẮT BUỘC TRẢ VỀ CHÍNH XÁC MỘT ĐỐI TƯỢNG JSON CÓ CẤU TRÚC SAU (không có thẻ ```json):
{{
  ""reply"": ""Nội dung trả lời/câu đố/lời bói dí dỏm của bạn (Không chứa ID)..."",
  ""recommend_item_ids"": [danh_sách_ID_món_bạn_muốn_gợi_ý_nếu_có_nhưng_tối_đa_3_id]
}}";

            var payload = new
            {
                system_instruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = request.Message } }
                    }
                },
                generationConfig = new { responseMimeType = "application/json" }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);

            for (int i = 0; i < apiKeys.Count; i++)
            {
                int attemptIndex;
                lock (_keyLock) { attemptIndex = _currentKeyIndex; }
                var currentKey = apiKeys[attemptIndex];
                var geminiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={currentKey}";

                try
                {
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(geminiUrl, content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(responseData);
                        
                        var botText = doc.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text").GetString();

                        using var botJson = JsonDocument.Parse(botText!);
                        var replyMsg = botJson.RootElement.GetProperty("reply").GetString();
                        var recommendedIds = new List<int>();
                        
                        if (botJson.RootElement.TryGetProperty("recommend_item_ids", out var idsElement) && idsElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var id in idsElement.EnumerateArray())
                            {
                                if (id.TryGetInt32(out int idVal)) recommendedIds.Add(idVal);
                            }
                        }

                        return Ok(new { reply = replyMsg, recommendedIds = recommendedIds });
                    }
                    else if ((int)response.StatusCode == 429)
                    {
                        // Quota Exceeded / Rate Limited -> Xoay sang key kế tiếp
                        lock (_keyLock) { _currentKeyIndex = (_currentKeyIndex + 1) % apiKeys.Count; }
                        continue;
                    }
                }
                catch
                {
                    // Tiếp tục thử key khác
                }
            }

            // Nếu tất cả API Key đều bận hoặc gặp lỗi -> Phản hồi bằng bộ trả lời thông minh local
            return GenerateFallbackResponse(request.Message, sanPhamsDynamic);
        }

        private IActionResult GenerateFallbackResponse(string message, List<dynamic> sanPhams)
        {
            var msgLower = (message ?? "").ToLower().Trim();
            string reply;
            var recommendedIds = new List<int>();

            // 1. Hỏi về nước ép / sinh tố
            if (msgLower.Contains("nước ép") || msgLower.Contains("nuoc ep") || msgLower.Contains("ép") || msgLower.Contains("sinh tố") || msgLower.Contains("sinh to"))
            {
                reply = "🍹 Quán mình có món Nước Ép Tươi nguyên chất mát lạnh siêu ngon nè bạn ơi! Thử ngay ly Nước ép cam tươi thanh mát nha:";
                var items = sanPhams.Where(s => ((string)s.TenSanPham).ToLower().Contains("ép") || ((string)s.TenSanPham).ToLower().Contains("sinh tố") || ((string)s.TenSanPham).ToLower().Contains("cam")).Take(3).ToList();
                if (items.Count == 0) items = sanPhams.Take(2).ToList();
                foreach (var it in items) recommendedIds.Add((int)it.MaSanPham);
            }
            // 2. Hỏi về Cà phê
            else if (msgLower.Contains("cà phê") || msgLower.Contains("ca phe") || msgLower.Contains("cafe") || msgLower.Contains("bạc xỉu") || msgLower.Contains("espresso") || msgLower.Contains("đen đá") || msgLower.Contains("sữa đá"))
            {
                reply = "☕ Đam mê gu cà phê đượm vị, thơm nức mũi thì thử ngay các món Cà Phê đặc sản này của quán nhé:";
                var items = sanPhams.Where(s => ((string)s.TenSanPham).ToLower().Contains("cà phê") || ((string)s.TenSanPham).ToLower().Contains("cafe") || ((string)s.TenSanPham).ToLower().Contains("bạc xỉu") || ((string)s.TenSanPham).ToLower().Contains("espresso")).Take(3).ToList();
                if (items.Count == 0) items = sanPhams.Take(2).ToList();
                foreach (var it in items) recommendedIds.Add((int)it.MaSanPham);
            }
            // 3. Hỏi về Trà / Trà trái cây / Trà sữa
            else if (msgLower.Contains("trà") || msgLower.Contains("tra ") || msgLower == "tra" || msgLower.Contains("trái cây") || msgLower.Contains("trai cay") || msgLower.Contains("hạt sen") || msgLower.Contains("vải") || msgLower.Contains("đào"))
            {
                reply = "🍵 Thanh mát giải nhiệt với các món Trà hoa quả & Trà đặc sản ngon tuyệt này của quán nha:";
                var items = sanPhams.Where(s => ((string)s.TenSanPham).ToLower().Contains("trà") || ((string)s.TenSanPham).ToLower().Contains("tea")).Take(3).ToList();
                if (items.Count == 0) items = sanPhams.Take(2).ToList();
                foreach (var it in items) recommendedIds.Add((int)it.MaSanPham);
            }
            // 4. Hỏi về Đá xay / Smoothie / Macchiato
            else if (msgLower.Contains("đá xay") || msgLower.Contains("da xay") || msgLower.Contains("smoothie") || msgLower.Contains("macchiato"))
            {
                reply = "🧊 Sảng khoái mát lạnh cùng các món Đá Xay & Smoothie béo ngậy thơm ngon nè:";
                var items = sanPhams.Where(s => ((string)s.TenSanPham).ToLower().Contains("đá xay") || ((string)s.TenSanPham).ToLower().Contains("smoothie")).Take(3).ToList();
                if (items.Count == 0) items = sanPhams.Take(2).ToList();
                foreach (var it in items) recommendedIds.Add((int)it.MaSanPham);
            }
            // 5. Hỏi về Bánh ngọt / Ăn kèm
            else if (msgLower.Contains("bánh") || msgLower.Contains("banh") || msgLower.Contains("cake") || msgLower.Contains("croissant"))
            {
                reply = "🍰 Thưởng thức bánh ngọt thơm lừng ăn kèm đồ uống ngon ngây ngất nha:";
                var items = sanPhams.Where(s => ((string)s.TenSanPham).ToLower().Contains("bánh") || ((string)s.TenSanPham).ToLower().Contains("cake")).Take(3).ToList();
                if (items.Count == 0) items = sanPhams.Take(2).ToList();
                foreach (var it in items) recommendedIds.Add((int)it.MaSanPham);
            }
            // 6. Hỏi về câu đố / game
            else if (msgLower.Contains("đố") || msgLower.Contains("riddle") || msgLower.Contains("câu đố") || msgLower.Contains("chơi"))
            {
                reply = "🎯 CÂU ĐỐ: Món nước nào có vị béo thơm ngậy của sữa hòa quyện cà phê đắng nhẹ, được mệnh danh là 'cà phê dành cho người sợ đắng'?\n\nA. Bạc xỉu\nB. Espresso\nC. Cà phê đen đá";
                var item = sanPhams.FirstOrDefault(s => ((string)s.TenSanPham).ToLower().Contains("bạc xỉu") || ((string)s.TenSanPham).ToLower().Contains("bac xiu"));
                if (item != null) recommendedIds.Add((int)item.MaSanPham);
            }
            // 7. Hỏi bói / tâm trạng
            else if (msgLower.Contains("bói") || msgLower.Contains("tâm trạng") || msgLower.Contains("buồn") || msgLower.Contains("vui") || msgLower.Contains("mệt") || msgLower.Contains("áp lực"))
            {
                reply = "🔮 Bói ly nước theo tâm trạng: Hôm nay lá trà phán rằng bạn đang cần một nguồn năng lượng sảng khoái! Thử ngay một ly Trà Trái Cây hoặc Bạc Xỉu của quán nhé! 🍹";
                var item = sanPhams.FirstOrDefault(s => ((string)s.TenSanPham).ToLower().Contains("trà") || ((string)s.TenSanPham).ToLower().Contains("bạc xỉu"));
                if (item != null) recommendedIds.Add((int)item.MaSanPham);
            }
            // 8. Hỏi bán chạy / bestseller / menu / giá / món ngon
            else if (msgLower.Contains("bán chạy") || msgLower.Contains("bestseller") || msgLower.Contains("ngon") || msgLower.Contains("món gì") || msgLower.Contains("thực đơn") || msgLower.Contains("menu"))
            {
                reply = "🔥 Các món Best-Seller ngon nức tiếng tại quán hôm nay nè bạn ơi! Uống một ngụm là say đắm ngay ☕:";
                recommendedIds = sanPhams.Take(3).Select(s => (int)s.MaSanPham).ToList();
            }
            // 9. Hỏi chào hỏi (chào, hi, hello)
            else if (msgLower == "chào" || msgLower == "hi" || msgLower == "hello" || msgLower == "xin chào")
            {
                reply = "Chào bạn nha! ☕ Mình là Barista AI đây. Quán mình đang phục vụ rất nhiều loại Cà phê pha máy, Trà trái cây sảng khoái và Bánh ngọt thơm lừng. Bạn muốn uống món gì nhâm nhi hôm nay?";
                recommendedIds = sanPhams.Take(2).Select(s => (int)s.MaSanPham).ToList();
            }
            // 10. Hỏi ngoài lề (lịch sử, thời tiết, chính trị, toán, con người... không liên quan đến quán)
            else
            {
                reply = "Dạ xin lỗi bạn nha, mình là Barista AI chuyên tư vấn đồ uống và thông tin của quán thôi nè! ☕ Mình không có thông tin về các chủ đề ngoài lề này. Bạn có muốn mình gợi ý một món nước ngon tuyệt cho hôm nay không ạ?";
            }

            return Ok(new { reply, recommendedIds });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}
