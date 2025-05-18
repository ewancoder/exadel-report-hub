using Blazored.LocalStorage;
using ExportPro.Front.Helper;
using ExportPro.Front.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ExportPro.Front.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private ApiHelper ApiHelper { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
    [Inject] private HttpClient httpClient { get; set; }
    [Inject] private ILocalStorageService localStorageService { get; set; } = default!;

    private UserDto? CurrentUser;
    private bool collapseNavMenu = true;
    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    protected override void OnInitialized()
    {
        ApiHelper = new ApiHelper(httpClient, localStorageService);
        AuthProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
    }

    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var authState = await task;
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true && CurrentUser == null)
        {
            try
            {
                var result = await ApiHelper.GetAsync<UserDto>("api/user/current");
                if (result is not null && result.IsSuccess)
                {
                    CurrentUser = result.Data;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load current user: " + ex.Message);
            }
        }
    }

    public void Dispose()
    {
        AuthProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
    }

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }
}
