using ExportPro.Export.SDK.DTOs;

namespace ExportPro.Export.SDK.Interfaces;

public interface IReportGenerator
{
    string ContentType { get; }
    string Extension { get; }
    byte[] Generate(ReportContentDto data);
}
