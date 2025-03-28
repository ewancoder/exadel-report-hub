
using ExportPro.Common.Core.Messages;

namespace ExportPro.Common.Core.Library
{
    public class ApiServiceResponse
    {
        public string? Message { get; set; }
        public string DetailsMessage { get; set; }
        public ApiStatus ApiState { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> ValidationErrors { get; set; }
    }

    public class ApiServiceResponse<T> : ApiServiceResponse
    {
        public ApiServiceResponse() { }
        public ApiServiceResponse(T data, ApiServiceResponse response)
        {
            base.ApiState = response.ApiState;
            base.Message = response.Message;
            base.DetailsMessage = response.DetailsMessage;
            base.ErrorCode = response.ErrorCode;
            base.ValidationErrors = response.ValidationErrors;
            Data = data;
        }

        public T Data { get; set; }
    }
    public class SuccessApiResponse : ApiServiceResponse
    {
        public SuccessApiResponse(string message = null)
        {
            ApiState = ApiStatus.Ok;
            Message = message;
        }
    }
    public class SuccessApiServiceResponse<T> : ApiServiceResponse<T>
    {
        public SuccessApiServiceResponse(T data, string message=null)
        {
            Data = data;
            ApiState = ApiStatus.Ok;
            Message = message;
        }
    }
    public class BadRequestApiServiceResponse : ApiServiceResponse
    {
        public BadRequestApiServiceResponse(string message = null, string errorCode = ResponseErrorCodes.BadRequest, List<string> validationErrors = null)
        {
            ApiState = ApiStatus.BadRequest;
            Message = message;
            ValidationErrors = validationErrors;
            ErrorCode = errorCode;
        }
    }

    public class NotFoundApiServiceResponse<T> : ApiServiceResponse<T>
    {
        public NotFoundApiServiceResponse(string message = null, string errorCode = ResponseErrorCodes.NotFound)
        {
            ApiState = ApiStatus.NotFount;
            Message = message;
            ErrorCode = errorCode;
        }
    }
}
