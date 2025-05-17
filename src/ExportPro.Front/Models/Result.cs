namespace ExportPro.Front.Models;

public class Result<T>
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public int ApiState { get; set; }
    public bool IsSuccess { get; set; }

}
