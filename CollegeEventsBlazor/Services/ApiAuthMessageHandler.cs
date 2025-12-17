using System.Net.Http.Headers;

namespace CollegeEventsBlazor.Services;

public sealed class ApiAuthMessageHandler : DelegatingHandler
{
    private readonly TokenStore _store;

    public ApiAuthMessageHandler(TokenStore store) => _store = store;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _store.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
         
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
