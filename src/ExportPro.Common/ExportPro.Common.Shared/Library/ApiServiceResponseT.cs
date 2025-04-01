namespace ExportPro.Common.Shared.Library
{
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
}
