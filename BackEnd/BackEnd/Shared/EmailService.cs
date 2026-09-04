using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BackEnd.Shared;

public class EmailSettings
{
    public string Provider { get; set; } = "Smtp";
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
            
            // Bypass SSL certificate errors if any
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            // Try primary port 587 with STARTTLS, fallback to port 465 with SSL
            try
            {
                using var cts1 = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await client.ConnectAsync(settings.SmtpServer, settings.SmtpPort, SecureSocketOptions.StartTls, cts1.Token);
            }
            catch (Exception exStartTls)
            {
                _logger.LogWarning($"[SMTP STARTTLS FAIL] Port 587 failed: {exStartTls.Message}. Retrying port 465 SSL...");
                using var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await client.ConnectAsync(settings.SmtpServer, 465, SecureSocketOptions.SslOnConnect, cts2.Token);
            }

            using var ctsAuth = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await client.AuthenticateAsync(settings.SenderEmail, cleanPassword, ctsAuth.Token);
            
            using var ctsSend = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await client.SendAsync(message, ctsSend.Token);
            
            using var ctsDisc = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await client.DisconnectAsync(true, ctsDisc.Token);

            _logger.LogInformation($"[EMAIL SENT REAL] Đã gửi thành công email OTP thực tế tới {toEmail} qua Gmail MailKit");
            return (true, null);
        }
        catch (Exception ex)
        {
            var err = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
            _logger.LogError(ex, $"[EMAIL ERROR] Không thể gửi email tới {toEmail}: {err}");
            return (false, err);
        }
    }
}
