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

    private static void GeneratePlanSection(StatisticsReportDto data, StreamWriter writer, CsvWriter csv)
    {
        writer.WriteLine("Plans");
        csv.WriteRecords(data.Plans);
        writer.WriteLine();
    }

    private static void GenerateItemSection(StatisticsReportDto data, StreamWriter writer, CsvWriter csv)
    {
        writer.WriteLine("Items");
        csv.WriteRecords(data.Items);
        writer.WriteLine();
    }

    private static void GenerateInvoiceSection(
        StatisticsReportDto data,
        StreamWriter writer,
        CsvWriter csv)
    {
        writer.WriteLine("Invoices");

        var rows = data.Invoices.Select(i => new
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

        csv.WriteRecords(rows);
        writer.WriteLine();
    }

    private static void GenerateReportMetaData(StatisticsReportDto data, StreamWriter writer)
    {
        writer.WriteLine($"Generated at,{DateTime.UtcNow:u}");
        writer.WriteLine($"Filters,ClientId:{data.Filters.ClientId}");
        writer.WriteLine();
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
