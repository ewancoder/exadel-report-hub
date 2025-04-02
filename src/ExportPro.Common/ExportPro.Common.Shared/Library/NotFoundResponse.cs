using System.Net;


namespace ExportPro.Common.Shared.Library;

public class NotFoundResponse : BaseResponse
{
    public NotFoundResponse(bool isSuccess = false, string message = null)
     {
         ApiState = HttpStatusCode.NotFound;
         Messages = [message];
         IsSuccess = isSuccess;
    }
}

