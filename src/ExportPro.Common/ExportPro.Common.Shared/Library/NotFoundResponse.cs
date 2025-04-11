using System.Net;


namespace ExportPro.Common.Shared.Library;

public class NotFoundResponse : BaseResponse
{
    public NotFoundResponse()
    {
        ApiState = HttpStatusCode.NotFound;
        IsSuccess = false;
    }
}

public class NotFoundResponse<T> : BaseResponse<T>
{
    public NotFoundResponse()
    {
        ApiState = HttpStatusCode.NotFound;
        IsSuccess = false;
    }

    public NotFoundResponse(string message)
        : this()
    {
        Messages?.Add(message);
    }
}

