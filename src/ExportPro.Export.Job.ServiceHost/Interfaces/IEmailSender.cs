namespace ExportPro.Export.Job.ServiceHost.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, byte[]? attachment = null, string? fileName = null, string? contentType = null);
}