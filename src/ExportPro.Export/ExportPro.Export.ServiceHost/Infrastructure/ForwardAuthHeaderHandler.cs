using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace ExportPro.Export.ServiceHost.Infrastructure;

public sealed class ForwardAuthHeaderHandler(IHttpContextAccessor accessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken ct)
    {
        var incoming = accessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(incoming))
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(incoming);

        return base.SendAsync(request, ct);
    }
}
