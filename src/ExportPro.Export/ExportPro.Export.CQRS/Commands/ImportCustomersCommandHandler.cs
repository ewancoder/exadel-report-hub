// namespace ExportPro.Export.CQRS.Commands;
// namespace ExportPro.Export.CQRS.CommandHandlers;


using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Refit;

namespace ExportPro.Export.CQRS.Commands;

public sealed record ImportCustomersCommand(IFormFile ExcelFile) : ICommand<int>;

public sealed class ImportCustomersCommandHandler(
    IStorageServiceApi storageApi,
    ICustomerExcelParser parser)
    : ICommandHandler<ImportCustomersCommand, int>
{
    public async Task<BaseResponse<int>> Handle(
        ImportCustomersCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ExcelFile.Length == 0)
            return new BadRequestResponse<int>("Empty file.");

        // ── 1. parse Excel ───────────────────────────────
        List<CreateUpdateCustomerDto> customers;
        try
        {
            using var stream = request.ExcelFile.OpenReadStream();
            customers = parser.Parse(stream);
        }
        catch (Exception ex)
        {
            return new BadRequestResponse<int>($"Excel parsing failed: {ex.Message}");
        }

        // ── 2. forward to Storage‑service ────────────────
        try
        {
            return await storageApi.CreateCustomersBulkAsync(customers, cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                                      !string.IsNullOrWhiteSpace(ex.Content))
        {
            // Bubble the validation errors exactly as Storage‑service sent them
            var resp = JsonConvert.DeserializeObject<BaseResponse<int>>(ex.Content);
            return resp ?? new BadRequestResponse<int>("Import failed.");
        }
    }
}