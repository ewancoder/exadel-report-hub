using ClosedXML.Excel;
using ExportPro.Export.Excel.Constants;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;

namespace ExportPro.Export.Excel.Services;

public sealed class CustomerExcelParser : ICustomerExcelParser
{
    private static readonly string[] RequiredColumns =
    [
        CustomerColumns.Name,
        CustomerColumns.Email,
        CustomerColumns.Address,
        CustomerColumns.CountryId,
    ];

    public List<CreateUpdateCustomerDto> Parse(Stream excelStream)
    {
        using var wb = new XLWorkbook(excelStream);
        var worksheet = GetWorksheet(wb);
        var headerRow = worksheet.FirstRowUsed();
        var columnMap = MapHeaderColumns(headerRow!);
        ValidateRequiredColumns(columnMap);
        return ExtractCustomersFromWorksheet(worksheet, columnMap);
    }

    private List<CreateUpdateCustomerDto> ExtractCustomersFromWorksheet(
        IXLWorksheet ws,
        Dictionary<string, int> columnMap
    )
    {
        var customers = ProcessRows(ws, columnMap);

        if (customers.Count == 0)
            throw new InvalidDataException("No valid rows found.");

        return customers;
    }

    private IXLWorksheet GetWorksheet(XLWorkbook workbook)
    {
        return workbook.Worksheets.First();
    }

    private Dictionary<string, int> MapHeaderColumns(IXLRow headerRow)
    {
        return headerRow
            .Cells()
            .Where(c => RequiredColumns.Contains(c.GetString().Trim(), StringComparer.OrdinalIgnoreCase))
            .ToDictionary(c => c.GetString().Trim(), c => c.Address.ColumnNumber, StringComparer.OrdinalIgnoreCase);
    }

    private void ValidateRequiredColumns(IReadOnlyDictionary<string, int> columnMap)
    {
        var missingColumns = RequiredColumns.Where(r => !columnMap.ContainsKey(r)).ToArray();
        if (missingColumns.Length > 0)
            throw new InvalidDataException($"Missing columns: {string.Join(", ", missingColumns)}");
    }

    private List<CreateUpdateCustomerDto> ProcessRows(
        IXLWorksheet worksheet,
        IReadOnlyDictionary<string, int> columnMap
    )
    {
        List<CreateUpdateCustomerDto> customers = [];

        foreach (var row in worksheet.RowsUsed().Skip(1)) // skip header
        {
            var customerDto = ParseRow(row, columnMap);
            customers.Add(customerDto!);
        }

        return customers;
    }

    private CreateUpdateCustomerDto? ParseRow(IXLRow row, IReadOnlyDictionary<string, int> columnMap)
    {
        var name = row.Cell(columnMap[CustomerColumns.Name]).GetString().Trim();
        var email = row.Cell(columnMap[CustomerColumns.Email]).GetString().Trim();
        var addr = row.Cell(columnMap[CustomerColumns.Address]).GetString().Trim();
        var cidStr = row.Cell(columnMap[CustomerColumns.CountryId]).GetString().Trim();

        if (AllBlank(name, email, addr, cidStr))
            return null; // blank row

        if (!Guid.TryParse(cidStr, out var countryId))
            throw new Exception($"Invalid CountryId '{cidStr}' (row {row.RowNumber()}).");

        return new CreateUpdateCustomerDto
        {
            Name = name,
            Email = email,
            Address = addr,
            CountryId = countryId,
        };
    }

    private static bool AllBlank(params string?[] values)
    {
        return values.All(string.IsNullOrWhiteSpace);
    }
}
