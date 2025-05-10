namespace ExportPro.Export.SDK.DTOs;

public sealed record ReportFileDto(string FileName, byte[] Content, string ContentType);