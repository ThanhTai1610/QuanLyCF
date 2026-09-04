using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BackEnd.Shared;

public class EmailSettings
{
    public string Provider { get; set; } = "None"; // Smtp, None
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

        try
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(settings.SenderEmail, settings.SenderName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(settings.SmtpServer, settings.SmtpPort))
                {
                    client.UseDefaultCredentials = false;
                    var cleanPassword = settings.SenderPassword.Replace(" ", "");
                    client.Credentials = new NetworkCredential(settings.SenderEmail, cleanPassword);
                    client.EnableSsl = true;

                    await client.SendMailAsync(message);
                }
            }

            _logger.LogInformation($"[EMAIL SENT] Đã gửi email tới {toEmail} qua SMTP");
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
