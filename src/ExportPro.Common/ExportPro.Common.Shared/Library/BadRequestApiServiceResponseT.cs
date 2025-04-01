using System.Net;

namespace ExportPro.Common.Shared.Library
{
    public class BadRequestApiServiceResponse<T>: ApiServiceResponse<T>
    {
        public BadRequestApiServiceResponse(T data, string message = null, string errorCode = null, List<string> validationErrors = null)
        {
            Data = data;
            Message = message;
            ErrorCode = errorCode;
            ValidationErrors = validationErrors;
        }
        public BadRequestApiServiceResponse(string message = null, string errorCode = null, List<string> validationErrors = null)
        {
            ApiState = HttpStatusCode.BadRequest;
            Message = message;
            ValidationErrors = validationErrors;
            ErrorCode = errorCode ?? HttpStatusCode.BadRequest.ToString();
        }
    }
}
