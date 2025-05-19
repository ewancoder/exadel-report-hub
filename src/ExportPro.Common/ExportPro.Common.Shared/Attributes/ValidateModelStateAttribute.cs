using ExportPro.Common.Shared.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExportPro.Common.Shared.Attributes;

public class ValidateModelStateAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var dict = context
                .ModelState.Where(ms => ms.Value?.Errors.Count > 0)
                .GroupBy(ms => ms.Key.Split('.').Last()!)
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(ms => ms.Value!.Errors.Select(e => e.ErrorMessage)).ToArray()
                );
            var validationFailedResponse = new ValidationFailedResponse(dict) { Messages = ["Validation Failed"] };
            context.Result = new JsonResult(validationFailedResponse)
            {
                StatusCode = StatusCodes.Status422UnprocessableEntity,
            };
        }
    }
}
