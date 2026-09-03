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

            // Đọc key từ appsettings hoặc biến môi trường (hỗ trợ nhiều key xoay vòng)
            var rawKeys = _config["GEMINI_API_KEYS"] ?? _config["Gemini:ApiKey"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEYS") ?? "";
            var apiKeys = rawKeys.Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrEmpty(k) && !k.Contains("YOUR_GEMINI_API_KEY"))
                .Distinct()
                .ToList();

            if (apiKeys.Count == 0)
                return StatusCode(500, new { message = "Hệ thống AI đang bảo trì (Chưa cấu hình API Key)." });

            var sanPhams = await _dbContext.SanPhams
                .Where(s => s.TrangThaiBan && s.KieuMon != "Topping")
                .Select(s => new { s.MaSanPham, s.TenSanPham, s.GiaBan })
                .ToListAsync();

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
4. TUYỆT ĐỐI KHÔNG viết ID của món vào câu trả lời (phần 'reply'). Chỉ nhắc tên món.
5. Khi bạn gợi ý món (kể cả khi đố vui hoặc bói món), hãy lấy ID của món đó ở THỰC ĐƠN và chỉ điền vào mảng 'recommend_item_ids'.
6. BẮT BUỘC TRẢ VỀ CHÍNH XÁC MỘT ĐỐI TƯỢNG JSON CÓ CẤU TRÚC SAU (không có thẻ ```json):
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
            string? lastErrorDetail = null;

            for (int i = 0; i < apiKeys.Count; i++)
            {
                int attemptIndex;
                lock (_keyLock) { attemptIndex = _currentKeyIndex; }
                var currentKey = apiKeys[attemptIndex];
                var geminiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={currentKey}";

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
                        lastErrorDetail = await response.Content.ReadAsStringAsync();
                        continue;
                    }
                    else
                    {
                        var errorDetail = await response.Content.ReadAsStringAsync();
                        return StatusCode(500, new { message = "AI đang bận pha cà phê, bạn thử lại sau nha!", details = errorDetail });
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Lỗi xử lý phản hồi từ AI.", error = ex.Message });
                }
            }

            return StatusCode(500, new { message = "AI đang bận (Tất cả API Key đều hết hạn ngạch).", details = lastErrorDetail });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}
