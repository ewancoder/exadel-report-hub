using System.Net;

namespace ExportPro.Common.Shared.Library;

internal class InternalServiceFailedResponse : BaseResponse
{
    public InternalServiceFailedResponse(Exception ex)
    {
        ApiState = HttpStatusCode.InternalServerError;
        Messages = [ex.Message];
        IsSuccess = false;
    }
}
