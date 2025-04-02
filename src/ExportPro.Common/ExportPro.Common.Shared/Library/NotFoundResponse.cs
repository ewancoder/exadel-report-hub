using System.Net;


namespace ExportPro.Common.Shared.Library;

public class NotFoundResponse : BaseResponse
{
    public NotFoundResponse()
     {
         ApiState = HttpStatusCode.NotFound;
    }
}

