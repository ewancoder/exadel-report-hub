﻿using Blazored.LocalStorage;
using ExportPro.Front.Helper;
using ExportPro.Front.Models;
using ExportPro.Front.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ExportPro.Front.Pages;

public partial class Login
{
    private ApiHelper? apiHelper;
    private readonly LoginModel login = new();

    [Inject]
    private NavigationManager NavMan { get; set; } = default!;

    [Inject]
    private HttpClient httpClient { get; set; } = default!;

    [Inject]
    private AuthenticationStateProvider AuthProvider { get; set; } = default!;

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    protected override void OnInitialized()
    {
        apiHelper = new ApiHelper(httpClient, LocalStorage);
    }

    private async Task HandleLogin()
    {
        if (apiHelper is null)
            return;

        try
        {
            var result = await apiHelper.PostAsync<LoginModel, AuthResponse>("api/Auth/login", login);
            if (result.IsSuccess && result.Data is not null)
            {
                await LocalStorage.SetItemAsync("accessToken", result.Data.AccessToken);
                await LocalStorage.SetItemAsync("expiresAt", result.Data.ExpiresAt);
                if (AuthProvider is AuthStateProvider authStateProvider)
                    authStateProvider.NotifyUserAuthentication(result.Data.AccessToken);
                NavigateToHome();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error: " + ex.Message);
        }
    }

    private void NavigateToHome()
    {
        NavMan.NavigateTo("/home");
    }

    private void NavigateToRegister()
    {
        NavMan.NavigateTo("/register");
    }
}
