using ExportPro.Front.Models;
using System.Net.Http.Json;

namespace ExportPro.Front.Helper
{
    public class ApiHelper(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<Result<T>> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Result<T>>()
                   ?? throw new InvalidOperationException("Response was null");
        }

        public async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Result<TResponse>>()
                   ?? throw new InvalidOperationException("Response was null");
        }

        public async Task<Result<TResponse>> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Result<TResponse>>()
                   ?? throw new InvalidOperationException("Response was null");
        }

        public async Task DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}
