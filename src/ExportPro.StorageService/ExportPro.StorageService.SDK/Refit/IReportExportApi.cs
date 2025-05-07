using ExportPro.StorageService.Models.Enums;
using Refit;

namespace ExportPro.StorageService.SDK.Refit;

public interface IReportExportApi
{
    [Get("/api/ReportExport")]
    Task<ApiResponse<HttpContent>> GetStatisticsAsync(
        [Query] ReportFormat format,
        [Query] Guid? clientId = null,
        CancellationToken cancellationToken = default);
}