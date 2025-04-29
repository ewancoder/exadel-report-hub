using ExportPro.Export.SDK.Enums;

namespace ExportPro.Export.SDK.Interfaces;

public interface IReportGeneratorResolver
{
    IReportGenerator Resolve(ReportFormat format);
}