using MediatR;

namespace ExportPro.Export.CQRS.Queries;

public record DownloadTodaysLogQuery : IRequest<byte[]?>;

public sealed class DownloadTodaysLogQueryHandler : IRequestHandler<DownloadTodaysLogQuery, byte[]?>
{
    public async Task<byte[]?> Handle(DownloadTodaysLogQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var fileName = $"log-{today}.txt";
        var logPath = Path.Combine("Logs", fileName);
        if (!File.Exists(logPath))
            return null;
        return await File.ReadAllBytesAsync(logPath, cancellationToken);
    }
}