using Microsoft.VisualBasic;
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

public class BadRequestResponse<T> : BaseResponse<T>
{
    public BadRequestResponse()
    {
        ApiState = HttpStatusCode.BadRequest;
        IsSuccess = false;
    }

    public BadRequestResponse(string message)
        : this()
    {
        Messages?.Add(message);
    }
}

