using System.Net;

namespace ExportPro.Common.Shared.Library;

public class ValidationFailedResponse : BaseResponse
{
    public ValidationFailedResponse()
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationFailedResponse(Dictionary<string, string[]> errors)
    {
        ApiState = HttpStatusCode.UnprocessableEntity;
        IsSuccess = false;
        Errors = errors;
    }

    public Dictionary<string, string[]> Errors { get; set; }
}
