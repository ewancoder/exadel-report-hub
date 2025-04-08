using ExportPro.Common.Shared.Exceptions;
using ExportPro.Common.Shared.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ExportPro.Common.Shared.Middlewares;
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        switch (ex)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                var res = new ValidationFailedResponse();
                foreach (var item in validationException.Failures)
                {
                    res.Messages.Add($"{item.Key} {item.Value}");
                }
                return context.Response.WriteAsync(JsonSerializer.Serialize(res));
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new InternalServiceFailedResponse(ex)));
        }

    }
}

    

