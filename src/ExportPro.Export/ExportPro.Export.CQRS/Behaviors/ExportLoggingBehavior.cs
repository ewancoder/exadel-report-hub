using System.Security.Claims;
using ExportPro.Common.Shared.Library;
using ExportPro.Export.CQRS.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExportPro.Export.CQRS.Behaviors;

public class ExportLoggingBehavior<TRequest, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    ILogger<ExportLoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : GenerateInvoicePdfQuery
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        logger.LogInformation(
            "Start Generating  Invoice Pdf: User {@UserId}, InvoiceId {@InvoiceId}, Timestamp {@Timestamp}",
            userId,
            request.InvoiceId,
            DateTime.UtcNow
        );
        try
        {
            var result = await next();
            logger.LogInformation(
                "Finished Generating  Invoice Pdf: User {@UserId}, InvoiceId {@InvoiceId},Timestamp {@Timestamp} Status : Success",
                userId,
                request.InvoiceId,
                DateTime.UtcNow
            );
            return result;
        }
        catch
        {
            logger.LogInformation(
                "Error in Generating Invoice Pdf: User {UserId}, InvoiceId {InvoiceId}, Timestamp {Timestamp}, Status : Failed",
                userId,
                request.InvoiceId,
                DateTime.UtcNow
            );
            throw;
        }
    }
}
