using System.Net;


namespace ExportPro.Common.Shared.Library
{
    public class SuccessApiServiceResponse<T> : ApiServiceResponse<T>
    {
        public SuccessApiServiceResponse(T data, string message = null)
        {
            Data = data;
            ApiState = HttpStatusCode.OK;
            Message = message;
        }
    }
}
