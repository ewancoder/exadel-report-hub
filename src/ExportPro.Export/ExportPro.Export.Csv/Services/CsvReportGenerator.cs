using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ExportPro.Export.SDK.DTOs;
using ExportPro.Export.SDK.Interfaces;

namespace ExportPro.Export.Csv.Services;

public sealed class CsvReportGenerator : IReportGenerator
{
    private static readonly string Separator = new('#', 75);
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

    private static void GenerateReportMetaData(ReportContentDto data, StreamWriter writer)
    {
        writer.WriteLine($"Client,{data.ClientName}");
        writer.WriteLine($"Generated at,{DateTime.UtcNow:u}");
        writer.WriteLine();
    }

    private static void GenerateInvoiceSection(
        ReportContentDto data,
        StreamWriter writer,
        CsvWriter csv)
    {
        writer.WriteLine(Separator);
        writer.WriteLine("Invoices");

        var rows = data.Invoices.Select(i => new
        {
            i.InvoiceNumber,
            IssueDate = i.IssueDate.ToString("yyyy-MM-dd"),
            DueDate = i.DueDate.ToString("yyyy-MM-dd"),
            i.Amount,
            i.CurrencyId,
            i.PaymentStatus,
            i.BankAccountNumber
        });

        csv.WriteRecords(rows);
        writer.WriteLine();
    }

    private static void GenerateItemSection(
        ReportContentDto data,
        StreamWriter writer,
        CsvWriter csv)
    {
        writer.WriteLine(Separator);
        writer.WriteLine("Items");

        var itemCsv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            NewLine = Environment.NewLine
        });

        itemCsv.WriteRecords(data.Items.Select(i => new
        {
            i.Name,
            i.Description,
            i.Price,
            i.Status,
            i.CurrencyId,
            i.CreatedAt,
            i.UpdatedAt
        }));
        writer.WriteLine();
    }

    private static void GeneratePlanSection(
        ReportContentDto data,
        StreamWriter writer,
        CsvWriter csv)
    {
        writer.WriteLine(Separator);
        writer.WriteLine("Plans");

        var planCsv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            NewLine = Environment.NewLine
        });

        planCsv.WriteRecords(data.Plans.Select(p => new
        {
            p.StartDate,
            p.EndDate,
            p.Amount,
            p.CreatedAt,
            p.UpdatedAt
        }));
        writer.WriteLine();
    }

    private static void SetupCsvStream(
        out MemoryStream ms,
        out StreamWriter writer,
        out CsvWriter csv)
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