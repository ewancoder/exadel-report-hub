namespace ExportPro.Export.Job.ServiceHost.DTOs;

public sealed class SmtpSettings
{
    public required string Host { get; set; }
    public int Port { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string From { get; set; }
    public bool EnableSsl { get; set; }
}
