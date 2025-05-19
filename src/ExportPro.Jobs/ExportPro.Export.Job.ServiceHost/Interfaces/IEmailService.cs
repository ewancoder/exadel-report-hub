using ExportPro.Export.Job.ServiceHost.DTOs;

namespace ExportPro.Export.Job.ServiceHost.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailSendDto dto);
}
