using System.Net;


namespace ExportPro.Common.Shared.Library;

public class BadRequestResponse : BaseResponse
{
   public BadRequestResponse()
   {
        ApiState = HttpStatusCode.BadRequest;
        IsSuccess = false;
   }
}

