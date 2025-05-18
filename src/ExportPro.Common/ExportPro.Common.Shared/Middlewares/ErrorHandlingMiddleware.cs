using System.Net;
using System.Text.Json;
using ExportPro.Common.Shared.Exceptions;
using ExportPro.Common.Shared.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExportPro.Common.Shared.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
                var validationResponse = new ValidationFailedResponse();
                foreach (var item in validationException.Failures)
                    validationResponse.Messages?.Add($"{item.Key} {item.Value}");
                return context.Response.WriteAsync(JsonSerializer.Serialize(validationResponse));
            case FluentValidation.ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                var dict = validationException
                    .Errors.GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key.Split('.').Last(), g => g.Select(x => x.ErrorMessage).ToArray());
                ValidationFailedResponse validationFailedResponse = new(dict);
                validationFailedResponse.Messages = ["Validation Failed"];
                return context.Response.WriteAsync(JsonSerializer.Serialize(validationFailedResponse));
            case EmailAlreadyExistsException _:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(JsonSerializer.Serialize(new InternalServiceFailedResponse(ex)));
        }
    }
}
