using System.Net;


namespace ExportPro.Common.Shared.Library
{
    public class BadRequestResponse : BaseResponse
    {
        public BadRequestResponse(List<string> messages = null)
        {
            ApiState = HttpStatusCode.BadRequest;
            Messages = messages;
        }
    }
}
