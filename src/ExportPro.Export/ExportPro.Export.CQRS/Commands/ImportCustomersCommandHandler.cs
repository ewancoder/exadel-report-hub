using System.Net;
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
        var parseResult = await ParseExcelAsync(request.ExcelFile);

        if (!parseResult.IsSuccess)
            return new BadRequestResponse<int>(parseResult.ErrorMessage!);

        return await SaveCustomersAsync(parseResult.Customers!, cancellationToken);
    }

    private async Task<(bool IsSuccess, List<CreateUpdateCustomerDto>? Customers, string? ErrorMessage)>
        ParseExcelAsync(IFormFile excelFile)
    {
        if (excelFile.Length == 0)
            return (false, null, "Empty file.");

        try
        {
            await using var stream = excelFile.OpenReadStream();
            var customers = await Task.Run(() => parser.Parse(stream));
            return (true, customers, null);
        }
        catch (Exception ex)
        {
            return (false, null, $"Excel parsing failed: {ex.Message}");
        }
    }

    private async Task<BaseResponse<int>> SaveCustomersAsync(
        List<CreateUpdateCustomerDto> customers,
        CancellationToken cancellationToken)
    {
        if (customers.Count == 0)
            return new BadRequestResponse<int>("No customers to import.");

        try
        {
            return await storageApi.CreateCustomersBulkAsync(customers, cancellationToken);
        }
        catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.BadRequest &&
                                      !string.IsNullOrWhiteSpace(ex.Content))
        {
            var resp = JsonConvert.DeserializeObject<BaseResponse<int>>(ex.Content);
            return resp ?? new BadRequestResponse<int>("Import failed.");
        }
    }
}