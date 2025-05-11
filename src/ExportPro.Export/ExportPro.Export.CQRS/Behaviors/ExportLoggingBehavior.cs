using System.Security.Claims;
using ExportPro.Export.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ExportPro.Export.CQRS.Behaviors
{
    public class ExportLoggingBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor, ILogger logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : GenerateInvoicePdfQuery
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            var userId =
                httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
            var contextLogger = logger.ForContext("SourceContext", "AuditLogs");
            try
            {
                contextLogger.Information(
                    "Start Generating Invoice Pdf: User {UserId}, InvoiceId {InvoiceId}, Timestamp {Timestamp}",
                    userId,
                    request.InvoiceId,
                    DateTime.UtcNow
                );
                var result = await next();
                contextLogger.Information(
                    "Finished Generating Invoice Pdf: User {UserId}, InvoiceId {InvoiceId}, Timestamp {Timestamp} Status: Success",
                    userId,
                    request.InvoiceId,
                    DateTime.UtcNow
                );
                return result;
            }
            catch (Exception ex)
            {
                contextLogger.Error(
                    ex,
                    "Error in Generating Invoice Pdf: User {UserId}, InvoiceId {InvoiceId}, Timestamp {Timestamp}, Status: Failed",
                    userId,
                    request.InvoiceId,
                    DateTime.UtcNow
                );

                throw;
            }
        }
    }
}
