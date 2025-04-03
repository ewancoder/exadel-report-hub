

using ExportPro.Common.Shared.Library;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ExportPro.Common.Shared.Behaviors;

public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest: IRequest<TResponse>
    where TResponse: BaseResponse
{

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    //TODO gonna need this when we have built auth
    //private readonly IHttpContextAccessor _contextAccessor;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation($"Starting request: {requestName} {JsonSerializer.Serialize(request)} at {DateTime.UtcNow}");

        var result = await next();

        if (!result.IsSuccess)
            _logger.LogError($"Starting request: {requestName} {JsonSerializer.Serialize(result.Messages)} at {DateTime.UtcNow}");
        else
            _logger.LogInformation($"Finish request: {requestName} {JsonSerializer.Serialize(result)} at {DateTime.UtcNow}");
        return result;
    }
}

