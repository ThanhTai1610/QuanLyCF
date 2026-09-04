using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BackEnd.Shared;

public class EmailSettings
{
    public string Provider { get; set; } = "Auto"; // Auto, Brevo, Resend, Smtp
    public string ApiKey { get; set; } = "";        // Brevo or Resend API key for HTTPS Port 443
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = "";
    public string SenderPassword { get; set; } = ""; // App password
    public string SenderName { get; set; } = "F6 Coffee";
}

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        var (success, _) = await SendEmailExAsync(toEmail, subject, body);
        return success;
    }

    public async Task<(bool Success, string? ErrorMessage)> SendEmailExAsync(string toEmail, string subject, string body)
    {
        var settings = _config.GetSection("Email").Get<EmailSettings>() ?? new EmailSettings();

        // 1. Ưu tiên gửi qua Brevo / Resend HTTP API (Port 443) nếu có ApiKey (Bảo đảm 100% không bị chặn cổng 587/465 trên Cloud)
        if (!string.IsNullOrWhiteSpace(settings.ApiKey) || settings.Provider.Equals("Brevo", StringComparison.OrdinalIgnoreCase) || settings.Provider.Equals("Resend", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                if (settings.Provider.Equals("Resend", StringComparison.OrdinalIgnoreCase))
                {
                    return await SendViaResendApiAsync(settings, toEmail, subject, body);
                }
                return await SendViaBrevoApiAsync(settings, toEmail, subject, body);
            }
        }

        // 2. Gửi qua SMTP truyền thống (MailKit)
        if (string.IsNullOrWhiteSpace(settings.SenderEmail) || string.IsNullOrWhiteSpace(settings.SenderPassword))
        {
            var msg = "Cấu hình gửi email (SenderEmail hoặc SenderPassword) chưa được thiết lập trong appsettings.json.";
            _logger.LogWarning($"[EMAIL WARNING] {msg}");
            return (false, msg);
        }

        var cleanPassword = settings.SenderPassword.Replace(" ", "");

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            try
            {
                using var cts1 = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, SecureSocketOptions.StartTls, cts1.Token);
            }
            catch (Exception exStartTls)
            {
                _logger.LogWarning($"[SMTP STARTTLS FAIL] Port 587 failed: {exStartTls.Message}. Retrying port 465 SSL...");
                using var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                await client.ConnectAsync(settings.SmtpServer, 465, SecureSocketOptions.SslOnConnect, cts2.Token);
            }

            using var ctsAuth = new CancellationTokenSource(TimeSpan.FromSeconds(6));
            await client.AuthenticateAsync(settings.SenderEmail, cleanPassword, ctsAuth.Token);
            
            using var ctsSend = new CancellationTokenSource(TimeSpan.FromSeconds(6));
            await client.SendAsync(message, ctsSend.Token);
            
            using var ctsDisc = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await client.DisconnectAsync(true, ctsDisc.Token);

            _logger.LogInformation($"[EMAIL SENT REAL] Đã gửi thành công email OTP thực tế tới {toEmail} qua Gmail SMTP");
            return (true, null);
        }
        catch (OperationCanceledException)
        {
            var err = "Hạ tầng Cloud (Render/VPS) chặn cổng SMTP 587/465. Vui lòng cấu hình Brevo ApiKey (gửi qua HTTPS Port 443) để gửi mail tức thì.";
            _logger.LogError($"[EMAIL TIMEOUT] {err}");
            return (false, err);
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
            _logger.LogError(ex, $"[EMAIL ERROR] Không thể gửi email tới {toEmail}: {err}");
            return (false, err);
        }
    }

    private async Task<(bool Success, string? ErrorMessage)> SendViaBrevoApiAsync(EmailSettings settings, string toEmail, string subject, string body)
    {
        try
        {
            using var http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(10);
            http.DefaultRequestHeaders.Add("api-key", settings.ApiKey);
            http.DefaultRequestHeaders.Add("accept", "application/json");

            var payload = new
            {
                sender = new { name = settings.SenderName, email = settings.SenderEmail },
                to = new[] { new { email = toEmail } },
                subject = subject,
                htmlContent = body
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await http.PostAsync("https://api.brevo.com/v3/smtp/email", content);
            if (resp.IsSuccessStatusCode)
            {
                _logger.LogInformation($"[EMAIL BREVO SENT] Đã gửi email OTP thành công tới {toEmail} qua Brevo HTTP API (Port 443)");
                return (true, null);
            }

            var errBody = await resp.Content.ReadAsStringAsync();
            _logger.LogError($"[EMAIL BREVO ERROR] HTTP {(int)resp.StatusCode}: {errBody}");
            return (false, $"Brevo API HTTP {(int)resp.StatusCode}: {errBody}");
        }
        catch (Exception ex)
        {
            return (false, $"Brevo HTTP Error: {ex.Message}");
        }
    }

    private async Task<(bool Success, string? ErrorMessage)> SendViaResendApiAsync(EmailSettings settings, string toEmail, string subject, string body)
    {
        try
        {
            using var http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(10);
            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.ApiKey);

            var payload = new
            {
                from = $"{settings.SenderName} <onboarding@resend.dev>",
                to = new[] { toEmail },
                subject = subject,
                html = body
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await http.PostAsync("https://api.resend.com/emails", content);
            if (resp.IsSuccessStatusCode)
            {
                _logger.LogInformation($"[EMAIL RESEND SENT] Đã gửi email OTP thành công tới {toEmail} qua Resend HTTP API (Port 443)");
                return (true, null);
            }

            var errBody = await resp.Content.ReadAsStringAsync();
            _logger.LogError($"[EMAIL RESEND ERROR] HTTP {(int)resp.StatusCode}: {errBody}");
            return (false, $"Resend API HTTP {(int)resp.StatusCode}: {errBody}");
        }
        catch (Exception ex)
        {
            return (false, $"Resend HTTP Error: {ex.Message}");
        }
    }
}
