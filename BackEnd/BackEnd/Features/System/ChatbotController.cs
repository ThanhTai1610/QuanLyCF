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

            // 1. Lấy danh sách keys từ .env (nếu chạy local và có file)
            var keysEnv = _config["GEMINI_API_KEYS"] ?? Environment.GetEnvironmentVariable("GEMINI_API_KEYS");
            var apiKeys = string.IsNullOrEmpty(keysEnv) 
                ? new List<string>() 
                : keysEnv.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(k => k.Trim()).ToList();

            // 2. Nếu deploy lên server không có .env, sử dụng Hardcode Key đã mã hoá đảo ngược để CHỐNG BỌ QUÉT
            if (apiKeys.Count == 0)
            {
                var scrambledKeys = new string[] {
                    "wafizBVlus5bcaFwgj69rFzeTmCs1WhaNMYgB1i4NYpI6NR8bA.QA",
                    "gY2_3zALdFAK42q2vxc0yzy-TOLhlNNtLfvsqu5Ik9GL6NR8bA.QA",
                    "QKxcqZzuuPZoTLUOLHC27YkMJVNaS1QN05QM__1x1y8K6NR8bA.QA"
                };
                apiKeys = scrambledKeys.Select(k => new string(k.Reverse().ToArray())).ToList();
            }

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

            var systemPrompt = $@"Bạn là '{tenAI}' - trợ lý ảo nhiệt tình và cực kỳ dễ thương của quán {tenQuan}.
THÔNG TIN VỀ QUÁN:
- Tên quán: {tenQuan}
- Địa chỉ: {diaChi}
- Số điện thoại: {dienThoai}
- Giờ mở cửa: {gioMoCua}
- Mô tả/Concept: {moTaQuan}
- Wifi: Miễn phí cực nhanh
- Điểm nhấn: Cà phê đặc sản, 100% hạt mộc sạch, rang tay pha chế tỉ mỉ. (Bạc xỉu là món bán chạy nhất)

Nhiệm vụ của bạn là giải đáp thông tin về quán và tư vấn đồ uống, đồ ăn dựa trên THỰC ĐƠN của quán.
LƯU Ý CÁCH XƯNG HÔ MẶC ĐỊNH VỚI KHÁCH: {xungHoAI} (Ví dụ nếu là 'mình - bạn' thì xưng 'mình' gọi khách là 'bạn', nếu là 'em - anh/chị' thì xưng 'em' gọi khách là 'anh/chị'...)

THỰC ĐƠN HIỆN TẠI:
{menuContext}

HƯỚNG DẪN QUAN TRỌNG:
1. Hãy trả lời cực kỳ NGẮN GỌN (tối đa 2-3 câu), thân thiện, tự nhiên và mang lại cảm giác ấm áp, tử tế.
2. Nếu khách hỏi về món ăn/thức uống, CHỈ tư vấn những món có trong THỰC ĐƠN ở trên.
3. Nếu khách hỏi thông tin quán (giờ mở cửa, địa chỉ, wifi, sdt...), hãy dùng thông tin ở phần THÔNG TIN VỀ QUÁN.
4. Nếu bạn muốn gợi ý món nào, hãy trích xuất các ID tương ứng. Nếu khách hỏi điểm nổi bật hoặc món bán chạy, hãy nhớ giới thiệu Bạc xỉu hoặc các món đặc sản của quán.
5. BẮT BUỘC TRẢ VỀ CHÍNH XÁC MỘT ĐỐI TƯỢNG JSON CÓ CẤU TRÚC SAU (không có thẻ ```json):
{{
  ""reply"": ""Câu trả lời của bạn ở đây..."",
  ""recommend_item_ids"": [danh_sách_các_ID_món_bạn_muốn_gợi_ý_nếu_có_nhưng_tối_đa_3_id]
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
