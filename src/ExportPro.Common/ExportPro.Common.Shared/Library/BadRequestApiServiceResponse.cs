using System.Net;


namespace ExportPro.Common.Shared.Library
{
    public class BadRequestApiServiceResponse : ApiServiceResponse
    {
        public BadRequestApiServiceResponse(string message = null, string errorCode = null, List<string> validationErrors = null)
        {
            ApiState = HttpStatusCode.BadRequest;
            Message = message;
            ValidationErrors = validationErrors;
            ErrorCode = errorCode ?? HttpStatusCode.BadRequest.ToString();
        }
    }
}
