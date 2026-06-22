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

            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
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

            var systemPrompt = $@"Bạn là '{tenAI}' - trợ lý ảo nhiệt tình và cực kỳ dễ thương của quán {tenQuan}.
Nhiệm vụ của bạn là tư vấn đồ uống, đồ ăn dựa trên THỰC ĐƠN của quán.
LƯU Ý CÁCH XƯNG HÔ MẶC ĐỊNH VỚI KHÁCH: {xungHoAI} (Ví dụ nếu là 'mình - bạn' thì xưng 'mình' gọi khách là 'bạn', nếu là 'em - anh/chị' thì xưng 'em' gọi khách là 'anh/chị'...)
THỰC ĐƠN HIỆN TẠI:
{menuContext}

HƯỚNG DẪN QUAN TRỌNG:
1. Hãy trả lời cực kỳ NGẮN GỌN (tối đa 2-3 câu), thân thiện, tự nhiên.
2. Nếu khách hỏi, CHỈ tư vấn những món có trong THỰC ĐƠN ở trên.
3. Nếu bạn muốn gợi ý món nào, hãy trích xuất các ID tương ứng.
4. BẮT BUỘC TRẢ VỀ CHÍNH XÁC MỘT ĐỐI TƯỢNG JSON CÓ CẤU TRÚC SAU (không có thẻ ```json):
{{
  ""reply"": ""Câu trả lời của bạn ở đây..."",
  ""recommend_item_ids"": [danh_sách_các_ID_món_bạn_muốn_gợi_ý_nếu_có_nhưng_tối_đa_3_id]
}}";

            var geminiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

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
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(geminiUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorDetail = await response.Content.ReadAsStringAsync();
                    return StatusCode(500, new { message = "AI đang bận pha cà phê, bạn thử lại sau nha!", details = errorDetail });
                }

                var responseData = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseData);
                
                var botText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                // parse botText as JSON since we instructed Gemini to return JSON
                using var botJson = JsonDocument.Parse(botText!);
                var replyMsg = botJson.RootElement.GetProperty("reply").GetString();
                var recommendedIds = new List<int>();
                
                if (botJson.RootElement.TryGetProperty("recommend_item_ids", out var idsElement) && idsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var id in idsElement.EnumerateArray())
                    {
                        if (id.TryGetInt32(out int idVal))
                        {
                            recommendedIds.Add(idVal);
                        }
                    }
                }

                return Ok(new { reply = replyMsg, recommendedIds = recommendedIds });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi xử lý phản hồi từ AI.", error = ex.Message });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}
