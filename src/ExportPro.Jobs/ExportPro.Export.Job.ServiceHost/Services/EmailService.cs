using ExportPro.Export.Job.ServiceHost.DTOs;
using ExportPro.Export.Job.ServiceHost.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ExportPro.Export.Job.ServiceHost.Services;

public sealed class EmailService(SmtpSettings settings) : IEmailService
{
    private readonly SmtpSettings _settings = settings;

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        byte[]? attachment = null,
        string? fileName = null,
        string? contentType = null)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.From),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        message.To.Add(to);

        if (attachment != null && fileName != null)
        {
            var stream = new MemoryStream(attachment);
            var att = new Attachment(stream, fileName, contentType ?? "application/octet-stream");
            message.Attachments.Add(att);
        }

        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        await smtp.SendMailAsync(message);
    }
}