namespace ExportPro.Export.Job.ServiceHost.DTOs;

public sealed class ServiceAccountSettings
{
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}