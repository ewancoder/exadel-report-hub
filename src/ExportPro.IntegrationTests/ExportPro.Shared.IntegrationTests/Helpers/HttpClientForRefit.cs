namespace ExportPro.Shared.IntegrationTests.Helpers;

public static class HttpClientForRefit
{
    public static HttpClient GetHttpClient(string jwtToken, int port)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{port}") };
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", jwtToken);
        return httpClient;
    }
}
