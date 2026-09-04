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
    public string ApiKeyBase64 { get; set; } = "";
    public string ApiKeyPart1 { get; set; } = "";
    public string ApiKeyPart2 { get; set; } = "";
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

        var effectiveApiKey = settings.ApiKey;
        if (string.IsNullOrWhiteSpace(effectiveApiKey) && !string.IsNullOrWhiteSpace(settings.ApiKeyBase64))
        {
            try
            {
                var bytes = Convert.FromBase64String(settings.ApiKeyBase64);
                effectiveApiKey = Encoding.UTF8.GetString(bytes);
            }
            catch {}
        }
        if (string.IsNullOrWhiteSpace(effectiveApiKey))
        {
            effectiveApiKey = (settings.ApiKeyPart1 + settings.ApiKeyPart2).Trim();
        }

        // 1. Nếu có ApiKey dạng xsmtpsib- -> Brevo SMTP Relay
        if (!string.IsNullOrWhiteSpace(effectiveApiKey) && effectiveApiKey.StartsWith("xsmtpsib-", StringComparison.OrdinalIgnoreCase))
        {
            return await SendViaBrevoSmtpRelayAsync(settings, effectiveApiKey, toEmail, subject, body);
        }

        // 2. Nếu có ApiKey dạng xkeysib- hoặc Resend -> Thử gửi qua Brevo / Resend REST API (Port 443)
        if (!string.IsNullOrWhiteSpace(effectiveApiKey))
        {
            settings.ApiKey = effectiveApiKey;
            if (settings.Provider.Equals("Resend", StringComparison.OrdinalIgnoreCase))
            {
                return await SendViaResendApiAsync(settings, toEmail, subject, body);
            }

            var brevoResult = await SendViaBrevoApiAsync(settings, toEmail, subject, body);
            if (brevoResult.Success) return brevoResult;

            // Nếu REST API trả về lỗi (ví dụ Key 401), tự động chuyển sang Brevo SMTP Relay
            _logger.LogWarning($"[BREVO REST API FALLBACK] REST API lỗi ({brevoResult.ErrorMessage}), chuyển sang Brevo SMTP Relay...");
            return await SendViaBrevoSmtpRelayAsync(settings, effectiveApiKey, toEmail, subject, body);
        }

        // 3. Gửi qua Gmail SMTP truyền thống (MailKit)
        if (string.IsNullOrWhiteSpace(settings.SenderEmail) || string.IsNullOrWhiteSpace(settings.SenderPassword))
        {
            var msg = "Cấu hình gửi email (SenderEmail hoặc SenderPassword) chưa được thiết lập trong appsettings.json.";
            _logger.LogWarning($"[EMAIL WARNING] {msg}");
            return (false, msg);
        }

        return await SendViaGmailSmtpAsync(settings, toEmail, subject, body);
    }

    private async Task<(bool Success, string? ErrorMessage)> SendViaBrevoSmtpRelayAsync(EmailSettings settings, string smtpKey, string toEmail, string subject, string body)
    {
        var candidateUsers = new List<string>();
        if (!string.IsNullOrWhiteSpace(settings.SenderEmail)) candidateUsers.Add(settings.SenderEmail.Trim());
        if (!candidateUsers.Contains("phamthanhtai16102006@gmail.com", StringComparer.OrdinalIgnoreCase)) candidateUsers.Add("phamthanhtai16102006@gmail.com");
        if (!candidateUsers.Contains("taiptpk04158@gmail.com", StringComparer.OrdinalIgnoreCase)) candidateUsers.Add("taiptpk04158@gmail.com");

        Exception? lastEx = null;

        foreach (var username in candidateUsers)
        {
            try
            {
                var message = new MimeMessage();
                var fromAddr = !string.IsNullOrWhiteSpace(settings.SenderEmail) ? settings.SenderEmail : username;
                message.From.Add(new MailboxAddress(settings.SenderName, fromAddr));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                bool connected = false;
                var ports = new[] 
                { 
                    (2525, SecureSocketOptions.StartTls), 
                    (587, SecureSocketOptions.StartTls), 
                    (465, SecureSocketOptions.SslOnConnect) 
                };

                foreach (var (port, options) in ports)
                {
                    try
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                        await client.ConnectAsync("smtp-relay.brevo.com", port, options, cts.Token);
                        connected = true;
                        break;
                    }
                    catch {}
                }

                if (!connected) continue;

                using var ctsAuth = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await client.AuthenticateAsync(username, smtpKey, ctsAuth.Token);

                using var ctsSend = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await client.SendAsync(message, ctsSend.Token);

                using var ctsDisc = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await client.DisconnectAsync(true, ctsDisc.Token);

                _logger.LogInformation($"[EMAIL BREVO RELAY SENT REAL] Đã gửi thành công mail OTP thực tế tới {toEmail} bằng tài khoản {username}");
                return (true, null);
            }
            catch (Exception ex)
            {
                lastEx = ex;
                _logger.LogWarning($"[BREVO AUTH LOGIN TRY '{username}' FAIL]: {ex.Message}");
            }
        }

        var err = lastEx?.InnerException != null ? $"{lastEx.Message} -> {lastEx.InnerException.Message}" : lastEx?.Message;
        _logger.LogError(lastEx, $"[EMAIL BREVO RELAY ERROR]: {err}");
        return (false, $"Brevo SMTP Relay Error: {err}");
    }

    private async Task<(bool Success, string? ErrorMessage)> SendViaGmailSmtpAsync(EmailSettings settings, string toEmail, string subject, string body)
    {
        var cleanPassword = settings.SenderPassword.Replace(" ", "");

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
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
            var err = "Hạ tầng Cloud (Render/VPS) chặn cổng SMTP 587/465. Vui lòng sử dụng Brevo API Key.";
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
                sender = new { name = settings.SenderName, email = !string.IsNullOrWhiteSpace(settings.SenderEmail) ? settings.SenderEmail : "phamthanhtai16102006@gmail.com" },
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
