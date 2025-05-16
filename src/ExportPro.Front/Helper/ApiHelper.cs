using System.Net.Http.Json;

namespace ExportPro.Front.Helper
{
    public class ApiHelper(HttpClient httpClient)
    {

        public async Task<T> GetAsync<T>(string url)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>()
                   ?? throw new InvalidOperationException("Response was null");
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await httpClient.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>()
                   ?? throw new InvalidOperationException("Response was null");
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest data)
        {
            var response = await httpClient.PutAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>()
                   ?? throw new InvalidOperationException("Response was null");
        }

        public async Task DeleteAsync(string url)
        {
            var response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}