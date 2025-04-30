using MediatR;

namespace ExportPro.Export.CQRS.Queries;

public record DownloadLogByDateRangeQuery(DateOnly startDate, DateOnly endDate) : IRequest<byte[]?>;

public class DowloadLogByDateRangeQueryHandler : IRequestHandler<DownloadLogByDateRangeQuery, byte[]?>
{
    public async Task<byte[]?> Handle(DownloadLogByDateRangeQuery request, CancellationToken cancellationToken)
    {
        var combinedLogs = new List<string>();
        for (var i = request.startDate; i <= request.endDate; i.AddDays(1))
        {
            var format = i.ToString("yyyyMMdd");
            var fileName = $"log-{format}.txt";
            var logPath = Path.Combine("C:/Logs", fileName);
            if (File.Exists(logPath))
            {
                var content = await File.ReadAllTextAsync(logPath, cancellationToken);
                combinedLogs.Add(format);
                combinedLogs.Add(content);
            }
        }
        var combinedText = string.Join(Environment.NewLine + "-----" + Environment.NewLine, combinedLogs);
        return System.Text.Encoding.UTF8.GetBytes(combinedText);
    }
}
