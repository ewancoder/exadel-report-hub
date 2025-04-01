using System.Net;


namespace ExportPro.Common.Shared.Library
{
    public class NotFoundApiServiceResponse<T> : ApiServiceResponse<T>
    {
        public NotFoundApiServiceResponse(string message = null, string errorCode = null)
        {
            ApiState = HttpStatusCode.NotFound;
            Message = message;
            ErrorCode = errorCode ?? HttpStatusCode.NotFound.ToString();
        }
    }
}
