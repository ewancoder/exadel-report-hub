
using System.Net;


namespace ExportPro.Common.Shared.Library;

public class SuccessApiResponse : BaseResponse
{
    public SuccessApiResponse(string message = null)
    {
        ApiState = HttpStatusCode.Accepted;
        Messages = [message];
    }
}

