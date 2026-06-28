using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BackEnd.Shared;

public class SmsSettings
{
    public string Provider { get; set; } = "None"; // SpeedSms, Twilio, None
    public string SpeedSmsToken { get; set; } = "";
    public string TwilioAccountSid { get; set; } = "";
    public string TwilioAuthToken { get; set; } = "";
    public string TwilioFromNumber { get; set; } = "";
}

public class SmsService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ILogger<SmsService> _logger;

    public SmsService(HttpClient http, IConfiguration config, ILogger<SmsService> logger)
    {
        _http = http;
        _config = config;
        _logger = logger;
    }

    public bool IsSimulated()
    {
        var settings = _config.GetSection("Sms").Get<SmsSettings>() ?? new SmsSettings();
        return string.Equals(settings.Provider, "None", StringComparison.OrdinalIgnoreCase) || 
               string.IsNullOrWhiteSpace(settings.Provider);
    }

    public async Task<bool> SendSmsAsync(string to, string content)
    {
        var settings = _config.GetSection("Sms").Get<SmsSettings>() ?? new SmsSettings();
        
        if (settings.Provider == "SpeedSms")
        {
            return await SendSpeedSmsAsync(to, content, settings.SpeedSmsToken);
        }
        else if (settings.Provider == "Twilio")
        {
            return await SendTwilioSmsAsync(to, content, settings.TwilioAccountSid, settings.TwilioAuthToken, settings.TwilioFromNumber);
        }

        _logger.LogInformation($"[SMS SIMULATION] Gửi tới {to}: {content}");
        return true;
    }

    private async Task<bool> SendSpeedSmsAsync(string to, string content, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogError("SpeedSmsToken is missing in configuration.");
            return false;
        }

        try
        {
            var formattedPhone = FormatPhoneVietnam(to);
            
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.speedsms.vn/index.php/sms/send");
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{token}:x"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            var payload = new
            {
                to = new[] { formattedPhone },
                content = content,
                sms_type = 2 // CSKH / Transactional SMS
            };

            var json = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var respJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"SpeedSms response: {respJson}");
                return true;
            }

            var errResp = await response.Content.ReadAsStringAsync();
            _logger.LogError($"SpeedSms failed: {response.StatusCode} - {errResp}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SpeedSMS");
            return false;
        }
    }

    private async Task<bool> SendTwilioSmsAsync(string to, string content, string sid, string authToken, string from)
    {
        if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(from))
        {
            _logger.LogError("Twilio configuration parameters are missing.");
            return false;
        }

        try
        {
            var formattedPhone = FormatPhoneE164(to);
            var url = $"https://api.twilio.com/2010-04-01/Accounts/{sid}/Messages.json";
            
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var authHeader = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sid}:{authToken}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            var formFields = new Dictionary<string, string>
            {
                { "To", formattedPhone },
                { "From", from },
                { "Body", content }
            };

            request.Content = new FormUrlEncodedContent(formFields);

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var respJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Twilio response: {respJson}");
                return true;
            }

            var errResp = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Twilio failed: {response.StatusCode} - {errResp}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Twilio SMS");
            return false;
        }
    }

    private string FormatPhoneVietnam(string phone)
    {
        phone = phone.Trim().Replace(" ", "").Replace("-", "");
        if (phone.StartsWith("+84")) return "0" + phone.Substring(3);
        if (phone.StartsWith("84")) return "0" + phone.Substring(2);
        return phone;
    }

    private string FormatPhoneE164(string phone)
    {
        phone = phone.Trim().Replace(" ", "").Replace("-", "");
        if (phone.StartsWith("+")) return phone;
        if (phone.StartsWith("0")) return "+84" + phone.Substring(1);
        if (phone.StartsWith("84")) return "+" + phone;
        return "+84" + phone;
    }
}
