using ExportPro.StorageService.Models.Enums;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface IReportExportApi
{
    [Get("/api/ReportExport")]
    Task<ApiResponse<HttpResponseMessage>> GetStatisticsAsync(
        [Query] ReportFormat format,
        [Query] Guid clientId,
        [Query] Guid clientCurrencyId,
        CancellationToken cancellationToken = default
    );
}
