using Microsoft.JSInterop;

namespace ExportPro.Front.Services;

public static class IJSRuntimeExtension
{
    public static async Task ToastrSucess(this IJSRuntime js, string message)
    {
        
        await js.InvokeVoidAsync("ShowToastr","success", message);
    }

    public static async Task ToastrError(this IJSRuntime js, string message)
    {
        await js.InvokeVoidAsync("ShowToastr","error", message);
    }

}

