using System.Net;

namespace ExportPro.Common.Core.Library
{
    public class ApiServiceResponse
    {
        public string? Message { get; set; }
        public string DetailsMessage { get; set; }
        public HttpStatusCode ApiState { get; set; }
        public string? ErrorCode { get; set; }
        public List<string> ValidationErrors { get; set; }
    }
}
