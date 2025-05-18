using Blazored.LocalStorage;
using ExportPro.Front.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace ExportPro.Front.Helper;

public class ApiHelper
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ApiHelper(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }
    private async Task AttachAuthHeaderAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("accessToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<Result<T>> GetAsync<T>(string url)
    {
        await AttachAuthHeaderAsync();

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<T>>()
               ?? throw new InvalidOperationException("Response was null");
    }

    public async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data)
    {
        await AttachAuthHeaderAsync();

        var response = await _httpClient.PostAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<TResponse>>()
               ?? throw new InvalidOperationException("Response was null");
    }

    public async Task<Result<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data)
    {
        await AttachAuthHeaderAsync();

        var response = await _httpClient.PutAsJsonAsync(url, data);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Result<TResponse>>()
               ?? throw new InvalidOperationException("Response was null");
    }

    public async Task DeleteAsync(string url)
    {
        await AttachAuthHeaderAsync();

        var response = await _httpClient.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
