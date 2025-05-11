using ExportPro.Export.SDK.DTOs;

namespace ExportPro.Export.Job.ServiceHost.Interfaces;

public interface IInvoiceReportService
{
    Task<ReportContentDto> BuildReportContentAsync();
}