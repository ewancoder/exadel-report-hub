using System.Net;


namespace ExportPro.Common.Core.Library
{
    public class SuccessApiResponse : ApiServiceResponse
    {
        public SuccessApiResponse(string message = null)
        {
            ApiState = HttpStatusCode.OK;
            Message = message;
        }
    }
}
