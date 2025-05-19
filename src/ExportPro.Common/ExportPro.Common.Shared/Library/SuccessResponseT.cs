using System.Net;

namespace ExportPro.Common.Shared.Library;

public class SuccessResponse<T> : BaseResponse<T>
{
    public SuccessResponse(T data, string? message = null)
    {
        Data = data;
        ApiState = HttpStatusCode.OK;
        Messages = [message];
        IsSuccess = true;
    }
}
