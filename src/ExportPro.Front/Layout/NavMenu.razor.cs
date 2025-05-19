using Blazored.LocalStorage;
using ExportPro.Front.Helper;
using ExportPro.Front.Models;
using ExportPro.Front.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ExportPro.Front.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private ApiHelper ApiHelper { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
    [Inject] private HttpClient httpClient { get; set; } = default!;
    [Inject] private ILocalStorageService localStorageService { get; set; } = default!;
    [Inject] private NavigationManager NavMan { get; set; } = default!;
    [Inject] private UserStateService UserState { get; set; } = default!;

    private bool collapseNavMenu = true;
    private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

    protected override async Task OnInitializedAsync()
    {
        ApiHelper = new ApiHelper(httpClient, localStorageService);
        await TrySetUserFromToken();

        AuthProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
        UserState.OnChange += StateHasChanged;

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //UserState.OnChange += StateHasChanged;

        await TrySetUserFromToken();
    }
    private async void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        var authState = await task;
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            await TrySetUserFromToken();
        }
        else
        {
            await UserState.SetUser(null);
        }
    }

    private async Task TrySetUserFromToken()
    {
        if (UserState.CurrentUser == null)
        {
            try
            {
                var result = await ApiHelper.GetAsync<UserDto>("api/user/current");
                if (result.IsSuccess)
                {
                    await UserState.SetUser(result.Data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load current user: " + ex.Message);
            }
        }
    }

    private void ToggleNavMenu()
    {
        collapseNavMenu = !collapseNavMenu;
    }

    private async Task Logout()
    {
        await localStorageService.RemoveItemAsync("accessToken");
        await localStorageService.RemoveItemAsync("refreshToken");
        await localStorageService.RemoveItemAsync("expiresAt");

        await UserState.SetUser(null);

        if (AuthProvider is AuthStateProvider auth)
        {
            auth.NotifyUserLogout();
        }

        NavMan.NavigateTo("/login", forceLoad: true);
    }

    public void Dispose()
    {
        AuthProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;
        UserState.OnChange -= StateHasChanged;
    }
}
