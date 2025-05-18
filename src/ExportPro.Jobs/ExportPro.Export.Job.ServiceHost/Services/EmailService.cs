using System.Net;
using System.Net.Mail;
using ExportPro.Export.Job.ServiceHost.DTOs;
using ExportPro.Export.Job.ServiceHost.Interfaces;

namespace ExportPro.Export.Job.ServiceHost.Services;

public sealed class EmailService(SmtpSettings settings) : IEmailService
{
    private readonly SmtpSettings _settings = settings;

    public async Task SendAsync(EmailSendDto dto)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_settings.From),
            Subject = dto.Subject,
            Body = dto.Body,
            IsBodyHtml = false,
        };

        message.To.Add(dto.To);

        if (dto.Attachment != null && dto.FileName != null)
        {
            var stream = new MemoryStream(dto.Attachment);
            var att = new Attachment(stream, dto.FileName, dto.ContentType ?? "application/octet-stream");
            message.Attachments.Add(att);
        }

        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
        };

        await smtp.SendMailAsync(message);
    }
}
