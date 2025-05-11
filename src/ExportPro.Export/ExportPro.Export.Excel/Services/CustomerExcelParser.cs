using ClosedXML.Excel;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.CustomerDTO;

namespace ExportPro.Export.Excel.Services;

public sealed class CustomerExcelParser : ICustomerExcelParser
{
    private static readonly string[] Required =
        ["Name", "Email", "Address", "CountryId"];

    public List<CreateUpdateCustomerDto> Parse(Stream excelStream)
    {
        using var wb = new XLWorkbook(excelStream);
        var ws = wb.Worksheets.First();
        var header = ws.FirstRowUsed();

        // map column names → index
        var col = header.Cells()
                        .Where(c => Required.Contains(c.GetString().Trim(),
                                                      StringComparer.OrdinalIgnoreCase))
                        .ToDictionary(c => c.GetString().Trim(),
                                      c => c.Address.ColumnNumber,
                                      StringComparer.OrdinalIgnoreCase);

        var missing = Required.Where(r => !col.ContainsKey(r)).ToArray();
        if (missing.Length > 0)
            throw new Exception($"Missing columns: {string.Join(", ", missing)}");

        List<CreateUpdateCustomerDto> list = [];
        foreach (var row in ws.RowsUsed().Skip(1)) // skip header
        {
            var name = row.Cell(col["Name"]).GetString().Trim();
            var email = row.Cell(col["Email"]).GetString().Trim();
            var addr  = row.Cell(col["Address"]).GetString().Trim();
            var cid   = row.Cell(col["CountryId"]).GetString().Trim();

            if (string.IsNullOrWhiteSpace(name) &&
                string.IsNullOrWhiteSpace(email) &&
                string.IsNullOrWhiteSpace(addr) &&
                string.IsNullOrWhiteSpace(cid))
                continue; // blank row

            if (!Guid.TryParse(cid, out var countryId))
                throw new Exception($"Invalid CountryId '{cid}' (row {row.RowNumber()}).");

            list.Add(new CreateUpdateCustomerDto
            {
                Name      = name,
                Email     = email,
                Address   = addr,
                CountryId = countryId
            });
        }

        if (list.Count == 0)
            throw new Exception("No valid rows found.");

        return list;
    }
}
