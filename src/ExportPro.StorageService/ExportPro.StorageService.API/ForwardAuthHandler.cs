using System.Net.Http.Headers;

namespace ExportPro.Export.ServiceHost.Infrastructure;

public class ForwardAuthHeaderHandler(IHttpContextAccessor accessor) : DelegatingHandler
{
    /// <summary>
    ///     Forwards the Authorization header from the incoming request to the outgoing request.
    /// </summary>
    /// <param name="request">The request message to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var incoming = accessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(incoming))
            request.Headers.Authorization = AuthenticationHeaderValue.Parse(incoming);

        return base.SendAsync(request, cancellationToken);
    }
}