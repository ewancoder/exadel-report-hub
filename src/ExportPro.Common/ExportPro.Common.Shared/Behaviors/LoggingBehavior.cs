﻿using System.Text.Json;
using ExportPro.Common.Shared.Library;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ExportPro.Common.Shared.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : BaseResponse
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation(
            $"Starting request: {requestName} {JsonSerializer.Serialize(request)} at {DateTime.UtcNow}"
        );

        var result = await next();

        if (!result.IsSuccess)
            _logger.LogError(
                $"Request failure: {requestName} {JsonSerializer.Serialize(result.Messages)} at {DateTime.UtcNow}"
            );
        else
            _logger.LogInformation(
                $"Finish request: {requestName} {JsonSerializer.Serialize(result)} at {DateTime.UtcNow}"
            );
        return result;
    }
}
