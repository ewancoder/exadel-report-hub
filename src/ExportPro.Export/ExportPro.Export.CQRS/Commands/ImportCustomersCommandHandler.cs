using System.Net;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using ExportPro.StorageService.SDK.Refit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Refit;

namespace ExportPro.Export.CQRS.Commands;

public sealed record ImportCustomersCommand(IFormFile ExcelFile) : ICommand<int>;

public sealed class ImportCustomersCommandHandler(
    IStorageServiceApi storageApi,
    ICustomerExcelParser parser,
    ILogger<ImportCustomersCommandHandler> logger
) : ICommandHandler<ImportCustomersCommand, int>
{
    public async Task<BaseResponse<int>> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
    {
        var parseResult = await ParseExcelAsync(request.ExcelFile);

        if (!parseResult.IsSuccess)
            return new BadRequestResponse<int>(parseResult.ErrorMessage!);

        return await SaveCustomersAsync(parseResult.Customers!, cancellationToken);
    }

    private async Task<(
        bool IsSuccess,
        List<CreateUpdateCustomerDto>? Customers,
        string? ErrorMessage
    )> ParseExcelAsync(IFormFile excelFile)
    {
        if (excelFile.Length == 0)
            return (false, null, "Empty file.");

        logger.LogInformation(
            "Starting to parse Excel file: {FileName}, size: {FileSize} bytes",
            excelFile.FileName,
            excelFile.Length
        );

        try
        {
            await using var stream = excelFile.OpenReadStream();
            var customers = await Task.Run(() => parser.Parse(stream));
            logger.LogInformation("Successfully parsed Excel file with {CustomerCount} customers", customers.Count);

            return (true, customers, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excel parsing failed for file: {FileName}", excelFile.FileName);
            return (false, null, $"Excel parsing failed: {ex.Message}");
        }
    }

    private async Task<BaseResponse<int>> SaveCustomersAsync(
        List<CreateUpdateCustomerDto> customers,
        CancellationToken cancellationToken
    )
    {
        if (customers.Count == 0)
            return new BadRequestResponse<int>("No customers to import.");

        logger.LogInformation("Starting to save {CustomerCount} customers to database", customers.Count);

        try
        {
            var result = await storageApi.Customer.CreateBulk(customers, cancellationToken);
            logger.LogInformation("Successfully imported {ImportedCount} customers", result.Data);
            return result;
        }
        catch (ApiException ex)
            when (ex.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace(ex.Content))
        {
            logger.LogWarning(ex, "Bad request error while importing customers: {ErrorContent}", ex.Content);

            var resp = JsonConvert.DeserializeObject<BaseResponse<int>>(ex.Content);
            return resp ?? new BadRequestResponse<int>("Import failed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred while importing customers");
            throw;
        }
    }
}
