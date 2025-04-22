namespace ExportPro.Export.SDK.DTOs;

/// <summary>
/// Returned by CQRS layer to carry the finished PDF.
/// </summary>
public sealed record InvoicePdfDto(string FileName, byte[] Content);