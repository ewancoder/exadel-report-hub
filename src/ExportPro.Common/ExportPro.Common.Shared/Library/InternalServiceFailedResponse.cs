

namespace ExportPro.Common.Shared.Library
{
    class InternalServiceFailedResponse: BaseResponse
    {
        public InternalServiceFailedResponse(Exception ex)
        {
            ApiState = System.Net.HttpStatusCode.InternalServerError;
            Messages = [ex.Message];

        }
    }
}
