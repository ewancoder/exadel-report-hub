using ExportPro.Export.SDK.DTOs;

namespace ExportPro.Export.SDK.Interfaces;

public interface ICsvExcelReportGenerator
{
    byte[] Generate(StatisticsReportDto data);
    string ContentType { get; }
    string Extension { get; }
}
