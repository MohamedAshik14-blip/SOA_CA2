using Microsoft.JSInterop;

namespace CollegeEventsBlazor.Services;

public class TokenStore
{
    private const string Key = "dkit_token";
    private readonly IJSRuntime _js;

    public TokenStore(IJSRuntime js) => _js = js;

    public async Task SetTokenAsync(string token) =>
        await _js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public async Task<string?> GetTokenAsync() =>
        await _js.InvokeAsync<string?>("localStorage.getItem", Key);

    public async Task ClearAsync() =>
        await _js.InvokeVoidAsync("localStorage.removeItem", Key);
}
