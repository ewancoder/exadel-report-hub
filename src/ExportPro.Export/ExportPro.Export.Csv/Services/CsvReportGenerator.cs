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

    public byte[] Generate(ReportContentDto data)
    {
        SetupCsvStream(out var ms, out var writer, out var csv);

        GenerateReportMetaData(data, writer);
        GenerateInvoiceSection(data, writer, csv);
        GenerateItemSection(data, writer, csv);
        GeneratePlanSection(data, writer, csv);

        writer.Flush();
        return ms.ToArray();
    }

    private static void GenerateReportMetaData(ReportContentDto data, StreamWriter w)
    {
        w.WriteLine($"Generated at,{DateTime.UtcNow:u}");
        w.WriteLine($"ClientIds,{string.Join('|',
            data.ClientNames.Select(kv => $"{kv.Value}({kv.Key})"))}");
        if (data.Filters.IssueDateFrom is { } dt)
            w.WriteLine($"IssueDateFrom,{dt:yyyy-MM-dd}");
        w.WriteLine();
    }

    private static void GenerateInvoiceSection(
        ReportContentDto data, StreamWriter w, CsvWriter csv)
    {
        w.WriteLine("Invoices");

        foreach (var grp in data.Invoices
                     .Where(i => i.ClientId.HasValue)
                     .GroupBy(i => i.ClientId!.Value))
        {
            data.ClientNames.TryGetValue(grp.Key, out var cName);
            w.WriteLine($"Client,{cName ?? "—"},{grp.Key}");
            csv.WriteRecords(ProjectInvoices(grp));
            w.WriteLine();
        }
    }

    private static void GenerateItemSection(ReportContentDto data, StreamWriter w, CsvWriter csv)
    {
        w.WriteLine("Items");
        foreach (var (clientId, items) in data.ItemsByClient)
        {
            data.ClientNames.TryGetValue(clientId, out var cName);
            w.WriteLine($"Client,{cName ?? "—"},{clientId}");
            csv.WriteRecords(items);
            w.WriteLine();
        }
    }

    private static void GeneratePlanSection(ReportContentDto data, StreamWriter w, CsvWriter csv)
    {
        w.WriteLine("Plans");
        foreach (var (clientId, plans) in data.PlansByClient)
        {
            data.ClientNames.TryGetValue(clientId, out var cName);
            w.WriteLine($"Client,{cName ?? "—"},{clientId}");
            csv.WriteRecords(plans);
            w.WriteLine();
        }
    }

    private static void SetupCsvStream(
        out MemoryStream ms, out StreamWriter writer, out CsvWriter csv)
    {
        ms = new MemoryStream();
        writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true);
        csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            NewLine = Environment.NewLine
        });
    }

    private static IEnumerable<object> ProjectInvoices(IEnumerable<InvoiceDto> src)
        => src.Select(i => new
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