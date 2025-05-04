namespace ExportPro.Export.SDK.DTOs;

/// <summary>
/// Returned by CQRS layer to carry the finished PDF.
/// </summary>
public sealed record PdfFileDto(string FileName, byte[] Content);