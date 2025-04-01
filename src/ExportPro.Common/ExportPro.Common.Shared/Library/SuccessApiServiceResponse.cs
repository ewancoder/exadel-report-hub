
using System.Net;


namespace ExportPro.Common.Shared.Library
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
