using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.StorageService.SDK.DTOs.InvoiceDTO;

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

        WriteSection("Invoices", ProjectInvoices(data.Invoices));

        WriteSection("Items", data.Items);

        WriteSection("Plans", data.Plans);

        writer.Flush();
        return ms.ToArray();

        void WriteSection<T>(string title, IEnumerable<T> records)
        {
            writer.WriteLine(title);
            csv.WriteRecords(records);
            writer.WriteLine();
        }
    }

    private static IEnumerable<object> ProjectInvoices(IEnumerable<InvoiceDto> src) =>
        src.Select(i => new
        {
            i.Id,
            i.InvoiceNumber,
            IssueDate = i.IssueDate.ToString("yyyy-MM-dd"),
            DueDate = i.DueDate.ToString("yyyy-MM-dd"),
            i.Amount,
            i.CurrencyId,
            i.PaymentStatus,
            i.BankAccountNumber,
            i.ClientId,
            i.CustomerId
        });
}
