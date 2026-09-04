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
    public string SmtpServer { get; set; } = "smtp-relay.brevo.com";
    public int SmtpPort { get; set; } = 2525;
    public string SmtpUser { get; set; } = "b7e30a001@smtp-brevo.com";
    public string SenderEmail { get; set; } = "taiptpk04158@gmail.com";
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
            var brevoResult = await SendViaBrevoSmtpRelayAsync(settings, effectiveApiKey, toEmail, subject, body);
            if (brevoResult.Success) return brevoResult;

            _logger.LogWarning($"[BREVO SMTP RELAY FALLBACK] Brevo SMTP Relay thất bại ({brevoResult.ErrorMessage}), tự động chuyển sang Gmail SMTP...");
        }

        // 2. Nếu có ApiKey dạng xkeysib- hoặc Resend -> Thử gửi qua Brevo / Resend REST API (Port 443)
        if (!string.IsNullOrWhiteSpace(effectiveApiKey) && !effectiveApiKey.StartsWith("xsmtpsib-", StringComparison.OrdinalIgnoreCase))
        {
            settings.ApiKey = effectiveApiKey;
            if (settings.Provider.Equals("Resend", StringComparison.OrdinalIgnoreCase))
            {
                var resendResult = await SendViaResendApiAsync(settings, toEmail, subject, body);
                if (resendResult.Success) return resendResult;
            }
            else
            {
                var brevoResult = await SendViaBrevoApiAsync(settings, toEmail, subject, body);
                if (brevoResult.Success) return brevoResult;
            }
        }

        // 3. Fallback: Gửi qua Gmail SMTP truyền thống (MailKit)
        if (!string.IsNullOrWhiteSpace(settings.SenderEmail) && !string.IsNullOrWhiteSpace(settings.SenderPassword))
        {
            var gmailResult = await SendViaGmailSmtpAsync(settings, toEmail, subject, body);
            if (gmailResult.Success) return gmailResult;

            return (false, $"Tất cả kênh gửi email đều thất bại. Gmail SMTP Lỗi: {gmailResult.ErrorMessage}");
        }

        return (false, "Cấu hình gửi email chưa đầy đủ trong appsettings.json.");
    }

    private async Task<(bool Success, string? ErrorMessage)> SendViaBrevoSmtpRelayAsync(EmailSettings settings, string smtpKey, string toEmail, string subject, string body)
    {
        var candidateUsers = new List<string>();
        if (!string.IsNullOrWhiteSpace(settings.SmtpUser)) candidateUsers.Add(settings.SmtpUser.Trim());
        if (!candidateUsers.Contains("b7e30a001@smtp-brevo.com", StringComparer.OrdinalIgnoreCase)) candidateUsers.Add("b7e30a001@smtp-brevo.com");
        if (!string.IsNullOrWhiteSpace(settings.SenderEmail) && !candidateUsers.Contains(settings.SenderEmail.Trim(), StringComparer.OrdinalIgnoreCase)) candidateUsers.Add(settings.SenderEmail.Trim());

        var ports = new[] 
        { 
            (587, SecureSocketOptions.StartTls), 
            (2525, SecureSocketOptions.StartTls), 
            (465, SecureSocketOptions.SslOnConnect) 
        };

        Exception? lastEx = null;

        foreach (var username in candidateUsers)
        {
            foreach (var (port, options) in ports)
            {
                try
                {
                    using var client = new SmtpClient();
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    using var ctsConn = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                    await client.ConnectAsync("smtp-relay.brevo.com", port, options, ctsConn.Token);

                    using var ctsAuth = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                    await client.AuthenticateAsync(username, smtpKey, ctsAuth.Token);

                    var message = new MimeMessage();
                    var fromAddr = !string.IsNullOrWhiteSpace(settings.SenderEmail) ? settings.SenderEmail : username;
                    message.From.Add(new MailboxAddress(settings.SenderName, fromAddr));
                    message.To.Add(MailboxAddress.Parse(toEmail));
                    message.Subject = subject;

                    var bodyBuilder = new BodyBuilder { HtmlBody = body };
                    message.Body = bodyBuilder.ToMessageBody();

                    using var ctsSend = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                    await client.SendAsync(message, ctsSend.Token);

                    using var ctsDisc = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                    await client.DisconnectAsync(true, ctsDisc.Token);

                    _logger.LogInformation($"[EMAIL BREVO RELAY SENT REAL] Đã gửi thành công email OTP tới {toEmail} qua Brevo SMTP (Port {port}, User: {username})");
                    return (true, null);
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    _logger.LogWarning($"[BREVO SMTP TRY User '{username}', Port {port} FAIL]: {ex.Message}");
                }
            }
        }

        var err = lastEx?.InnerException != null ? $"{lastEx.Message} -> {lastEx.InnerException.Message}" : lastEx?.Message;
        _logger.LogError(lastEx, $"[EMAIL BREVO RELAY ERROR]: {err}");
        return (false, $"Brevo SMTP Relay Error: {err}");
    }

    private async Task<(bool Success, string? ErrorMessage)> SendViaGmailSmtpAsync(EmailSettings settings, string toEmail, string subject, string body)
    {
        var cleanPassword = settings.SenderPassword.Replace(" ", "");

        var ports = new[]
        {
            (587, SecureSocketOptions.StartTls),
            (465, SecureSocketOptions.SslOnConnect)
        };

        Exception? lastEx = null;

        foreach (var (port, options) in ports)
        {
            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                var host = !string.IsNullOrWhiteSpace(settings.SmtpServer) && settings.SmtpServer.Contains("gmail", StringComparison.OrdinalIgnoreCase) 
                    ? settings.SmtpServer 
                    : "smtp.gmail.com";

                using var ctsConn = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                await client.ConnectAsync(host, port, options, ctsConn.Token);

                using var ctsAuth = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                await client.AuthenticateAsync(settings.SenderEmail, cleanPassword, ctsAuth.Token);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var ctsSend = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                await client.SendAsync(message, ctsSend.Token);

                using var ctsDisc = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await client.DisconnectAsync(true, ctsDisc.Token);

                _logger.LogInformation($"[EMAIL GMAIL SENT REAL] Đã gửi thành công email OTP tới {toEmail} qua Gmail SMTP (Port {port})");
                return (true, null);
            }
            catch (Exception ex)
            {
                lastEx = ex;
                _logger.LogWarning($"[GMAIL SMTP Port {port} FAIL]: {ex.Message}");
            }
        }

        var err = lastEx?.InnerException != null ? $"{lastEx.Message} -> {lastEx.InnerException.Message}" : lastEx?.Message;
        _logger.LogError(lastEx, $"[EMAIL GMAIL ERROR] Không thể gửi email tới {toEmail}: {err}");
        return (false, err);
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
