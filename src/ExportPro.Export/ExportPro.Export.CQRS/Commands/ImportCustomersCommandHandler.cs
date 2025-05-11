using ExportPro.Common.Shared.Mediator;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using ExportPro.Common.Shared.Library;
using ExportPro.Common.Shared.Mediator;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.IO;
using Refit;
using Newtonsoft.Json;
using Refit;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace ExportPro.Export.CQRS.Commands;

public sealed record ImportCustomersCommand(IFormFile ExcelFile) : ICommand<int>;

public sealed class ImportCustomersCommandHandler(
    IStorageServiceApi storageApi)
    : ICommandHandler<ImportCustomersCommand, int>
{
    public async Task<BaseResponse<int>> Handle(
        ImportCustomersCommand request,
        CancellationToken cancellationToken)
    {
        if (request.ExcelFile.Length == 0)
            return new BadRequestResponse<int>("Empty file.");

        List<CreateUpdateCustomerDto> customers;
        try
        {
            customers = ParseExcel(request.ExcelFile);
        }
        catch (Exception ex)
        {
            return new BadRequestResponse<int>($"Excel parsing failed: {ex.Message}");
        }

        try
        {
            return await storageApi.CreateCustomersBulkAsync(customers, cancellationToken);
        }
        catch (ApiException ex) when (ex.HasContent)
        {
            var resp = JsonConvert.DeserializeObject<BaseResponse<int>>(ex.Content);
            if (resp is not null) return resp;

            return new BaseResponse<int>
            {
                ApiState = ex.StatusCode,
                Messages = [$"Upstream error: {ex.StatusCode}"],
                IsSuccess = false
            };
        }
    }


    private static List<CreateUpdateCustomerDto> ParseExcel(IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        ms.Position = 0;

        using var wb = new XLWorkbook(ms);
        var ws = wb.Worksheets.First();
        var header = ws.FirstRowUsed();

        // map column names → index
        var required = new[] { "Name", "Email", "Address", "CountryId" };
        var col = header.Cells()
            .Where(c => required.Contains(c.GetString().Trim(), StringComparer.OrdinalIgnoreCase))
            .ToDictionary(c => c.GetString().Trim(), c => c.Address.ColumnNumber,
                StringComparer.OrdinalIgnoreCase);

        var missing = required.Where(r => !col.ContainsKey(r)).ToArray();
        if (missing.Length > 0)
            throw new Exception($"Missing columns: {string.Join(", ", missing)}");

        List<CreateUpdateCustomerDto> list = [];
        
        foreach (var row in header.Worksheet.RowsUsed().Skip(1)) // skip header
        {
            var name = row.Cell(col["Name"]).GetString().Trim();
            var email = row.Cell(col["Email"]).GetString().Trim();
            var addr = row.Cell(col["Address"]).GetString().Trim();
            var cid = row.Cell(col["CountryId"]).GetString().Trim();

            if (string.IsNullOrWhiteSpace(name) &&
                string.IsNullOrWhiteSpace(email) &&
                string.IsNullOrWhiteSpace(addr) &&
                string.IsNullOrWhiteSpace(cid))
                continue;

            if (!Guid.TryParse(cid, out var countryId))
                throw new Exception($"Invalid CountryId '{cid}' (row {row.RowNumber()}).");

            list.Add(new CreateUpdateCustomerDto
            {
                Name = name,
                Email = email,
                Address = addr,
                CountryId = countryId
            });
        }

        if (list.Count == 0)
            throw new Exception("No valid rows found.");

        return list;
    }
}