namespace ExportPro.Common.Shared.Library;

public class BaseResponseT<T> : BaseResponse
{
     public T? Data { get; set; }
}

