using ExportPro.Front.Helper;
using ExportPro.Front.Models;
using Microsoft.AspNetCore.Components;

namespace ExportPro.Front.Pages;

public partial class Register : ComponentBase
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private HttpClient httpClient { get; set; } = default!;

    private ApiHelper? apiHelper;

    private RegisterModel registerModel = new();

    protected override void OnInitialized()
    {
        apiHelper = new ApiHelper(httpClient);
    }

    private async Task HandleRegister()
    {
        if (apiHelper is null)
            return;

        try
        {
            var result = await apiHelper.PostAsync<RegisterModel, object>("api/Auth/register", registerModel);
            Console.Write(httpClient.BaseAddress);

            if (result.IsSuccess)
            {
                Nav.NavigateTo("/login");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
    }

    private void NavigateToLogin() => Nav.NavigateTo("/login");
}
