
using System.Net;


namespace ExportPro.Common.Shared.Library;

public class SuccessResponse : BaseResponse
{
    public SuccessResponse(string message = null)
    {
        ApiState = HttpStatusCode.Accepted;
        Messages = [message];
        IsSuccess = true;
    }
}

