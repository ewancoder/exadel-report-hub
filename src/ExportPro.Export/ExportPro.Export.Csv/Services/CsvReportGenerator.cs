using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;
using ExportPro.Export.SDK.Utilities;

namespace ExportPro.Export.Csv.Services;

public sealed class CsvReportGenerator : IReportGenerator
{
    public string ContentType => "text/csv";
    public string Extension => ReportFileTypes.Csv;

    public byte[] Generate(ReportContentDto data)
    {
        SetupCsvStream(out MemoryStream ms, out StreamWriter writer, out CsvWriter csv);
        GenerateReportMetaData(data, writer);
        GenerateInvoiceSection(data, writer, csv);
        GenerateItemSection(data, writer, csv);
        GeneratePlanSection(data, writer, csv);
        return FinalizeCsvData(ms, writer);
    }

    private static byte[] FinalizeCsvData(MemoryStream ms, StreamWriter writer)
    {
        writer.Flush();
        return ms.ToArray();
    }

    private static void GenerateReportMetaData(ReportContentDto data, StreamWriter writer)
    {
        writer.WriteLine($"Generated at,{DateTime.UtcNow:u}");
        writer.WriteLine($"ClientIds,{string.Join('|', data.Filters.ClientIds ?? [])}");
        if (data.Filters.IssueDateFrom is { } dt)
            writer.WriteLine($"IssueDateFrom,{dt:yyyy-MM-dd}");
        writer.WriteLine();
    }

    private static void GenerateInvoiceSection(
        ReportContentDto data,
        StreamWriter writer,
        CsvWriter csv)
    {
        writer.WriteLine("Invoices");
        foreach (var grp in data.Invoices.GroupBy(i => i.ClientId))
        {
            writer.WriteLine($"ClientId,{grp.Key}");
            csv.WriteRecords(ProjectInvoices(grp));
            writer.WriteLine();
        }
    }

    private static void GenerateItemSection(ReportContentDto data, StreamWriter writer, CsvWriter csv)
    {
        writer.WriteLine("Items");
        foreach (var (clientId, items) in data.ItemsByClient)
        {
            writer.WriteLine($"ClientId,{clientId}");
            csv.WriteRecords(items);
            writer.WriteLine();
        }
    }

    private static IEnumerable<object> ProjectInvoices(IEnumerable<InvoiceDto> src)
    {
        return src.Select(i => new
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

    private static void GeneratePlanSection(ReportContentDto data, StreamWriter writer, CsvWriter csv)
    {
        writer.WriteLine("Plans");
        foreach (var (clientId, plans) in data.PlansByClient)
        {
            writer.WriteLine($"ClientId,{clientId}");
            csv.WriteRecords(plans);
            writer.WriteLine();
        }
    }

    private static void SetupCsvStream(out MemoryStream ms, out StreamWriter writer, out CsvWriter csv)
    {
        ms = new MemoryStream();
        writer = new StreamWriter(ms, Encoding.UTF8, leaveOpen: true);
        csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            NewLine = Environment.NewLine
        });
    }
}