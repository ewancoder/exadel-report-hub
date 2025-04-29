using ExportPro.Export.SDK.Enums;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.Csv.Services;
using ExportPro.Export.Excel.Services;

namespace ExportPro.Export.ServiceHost.Infrastructure;

/// <summary>Factory that chooses CSV or XLSX generator.</summary>
public sealed class ReportGeneratorResolver(
    CsvReportGenerator csv,
    ExcelReportGenerator xlsx)
    : IReportGeneratorResolver
{
    public IReportGenerator Resolve(ReportFormat format) => format switch
    {
        ReportFormat.Csv => csv,
        ReportFormat.Xlsx => xlsx,
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
    };
}
