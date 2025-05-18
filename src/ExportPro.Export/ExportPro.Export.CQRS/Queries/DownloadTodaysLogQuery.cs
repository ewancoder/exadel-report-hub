using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ExportPro.Export.CQRS.Queries;

public record DownloadTodaysLogQuery : IRequest<byte[]?>;

public sealed class DownloadTodaysLogQueryHandler(IHttpContextAccessor iContextAccessor)
    : IRequestHandler<DownloadTodaysLogQuery, byte[]?>
{
    public Task<byte[]?> Handle(DownloadTodaysLogQuery request, CancellationToken cancellationToken)
    {
        var userId = iContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Log.Information("{userId} is downloading today's log", userId);
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var fileName = $"log-{today}.txt";
        var logPath = Path.Combine("Logs", fileName);
        Log.Debug("Log path: {LogPath} User: {userId}", logPath, userId);
        if (!File.Exists(logPath))
        {
            Log.Information("Log file not found User: {userId}", userId);
            return null;
        }

        Log.Information("Log file found, reading content User: {userId}", userId);
        return File.ReadAllBytesAsync(logPath, cancellationToken)!;
    }
}
