using Blazored.LocalStorage;
using ExportPro.Front.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ExportPro.Front.Services;
public class UserStateService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public UserDto? CurrentUser { get; private set; }

    public event Action? OnChange;

    public UserStateService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    public async Task InitializeAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("accessToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetFromJsonAsync<Result<UserDto>>("api/user/current");
                if (response?.IsSuccess == true)
                {
                    CurrentUser = response.Data;
                    OnChange?.Invoke();
                }
            }
            catch
            {
                await ClearUser();
            }
        }
    }

    public async Task SetUser(UserDto? user)
    {
        CurrentUser = user;
        OnChange?.Invoke();
        await Task.CompletedTask;
    }

    public async Task ClearUser()
    {
        CurrentUser = null;
        await _localStorage.RemoveItemAsync("accessToken");
        await _localStorage.RemoveItemAsync("expiresAt");
        _httpClient.DefaultRequestHeaders.Authorization = null;
        OnChange?.Invoke();
    }
}