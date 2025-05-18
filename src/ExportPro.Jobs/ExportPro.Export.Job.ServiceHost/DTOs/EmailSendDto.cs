namespace ExportPro.Export.Job.ServiceHost.DTOs;

public class EmailSendDto
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public byte[]? Attachment { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
}