using MediatR;
using Microsoft.Extensions.Configuration;

namespace ExportPro.Export.CQRS.Queries;

public record DownloadLogByDateRangeQuery(DateOnly startDate, DateOnly endDate) : IRequest<byte[]?>;

public class DowloadLogByDateRangeQueryHandler(IConfiguration configuration)
    : IRequestHandler<DownloadLogByDateRangeQuery, byte[]?>
{
    public async Task<byte[]?> Handle(DownloadLogByDateRangeQuery request, CancellationToken cancellationToken)
    {
        Dictionary<string, string> combinedLogs = new Dictionary<string, string>();
        for (var i = request.startDate; i <= request.endDate; i = i.AddDays(1))
        {
            var format = i.ToString("yyyyMMdd");
            var fileName = $"log-{format}.txt";
            var filePath = configuration["logPath"];
            var logPath = Path.Combine(filePath!, fileName);
            if (File.Exists(logPath))
            {
                var content = await File.ReadAllTextAsync(logPath, cancellationToken);
                combinedLogs.Add(i.ToString(), content);
            }
        }
        var combinedText = string.Join(
            Environment.NewLine + "-----" + Environment.NewLine,
            combinedLogs.Select(kv => $"{kv.Key}{Environment.NewLine}{kv.Value}")
        );
        if (combinedText.Length == 0)
            return null;
        return System.Text.Encoding.UTF8.GetBytes(combinedText);
    }
}
