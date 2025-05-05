using ExportPro.Export.Job.ServiceHost.Interfaces;
using ExportPro.Export.SDK.DTOs;

namespace ExportPro.Export.Job.ServiceHost.Services;

public class InvoiceReportService : IInvoiceReportService
{
    public Task<ReportContentDto> BuildReportContentAsync()
    {
        throw new NotImplementedException();
    }
}