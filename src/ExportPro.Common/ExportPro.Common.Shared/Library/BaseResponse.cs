using System.Net;

namespace ExportPro.Common.Shared.Library;

public class BaseResponse
{
    public HttpStatusCode ApiState { get; set; }
    public List<string>? Messages { get; set; }
    public bool IsSuccess { get; set; }
}
public class BaseResponse<T> : BaseResponse
{
     public T? Data { get; set; }
}

