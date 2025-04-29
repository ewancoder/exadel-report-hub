using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;

namespace ExportPro.Export.Csv.Services;

public sealed class CsvReportGenerator : IReportGenerator
{
    public string ContentType => "text/csv";
    public string Extension => "csv";

    public byte[] Generate(StatisticsReportDto data)
    {
        using var ms = new MemoryStream();
        using var writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            NewLine = Environment.NewLine
        });

        writer.WriteLine($"Generated at,{DateTime.UtcNow:u}");
        writer.WriteLine($"Filters,Start:{data.Filters.StartDate:yyyy-MM-dd},End:{data.Filters.EndDate:yyyy-MM-dd},ClientId:{data.Filters.ClientId}");
        writer.WriteLine();

        writer.WriteLine("Invoices");
        csv.WriteRecords(data.Invoices);
        writer.WriteLine();

        writer.WriteLine("Items");
        csv.WriteRecords(data.Items);
        writer.WriteLine();

        writer.WriteLine("Plans");
        csv.WriteRecords(data.Plans);

        writer.Flush();
        return ms.ToArray();
    }
}
