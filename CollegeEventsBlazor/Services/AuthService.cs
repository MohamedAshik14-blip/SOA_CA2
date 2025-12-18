using System.Net.Http.Json;
using CollegeEventsBlazor.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace CollegeEventsBlazor.Services;

public class AuthService
{
    private readonly IHttpClientFactory _factory;
    private readonly TokenStore _store;
    private readonly CustomAuthStateProvider _authState;

    public AuthService(IHttpClientFactory factory, TokenStore store, AuthenticationStateProvider authState)
    {
        _factory = factory;
        _store = store;
        _authState = (CustomAuthStateProvider)authState;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req)
    {
        var http = _factory.CreateClient("Api");
        var res = await http.PostAsJsonAsync("api/Auth/login", req);
        if (!res.IsSuccessStatusCode)
            throw new Exception(await res.Content.ReadAsStringAsync());

        var data = await res.Content.ReadFromJsonAsync<LoginResponse>()
                   ?? throw new Exception("Invalid login response");

        await _store.SetTokenAsync(data.Token);
        _authState.Notify();
        return data;
    }

    public async Task RegisterAsync(RegisterRequest req)
    {
        var http = _factory.CreateClient("Api");
        var res = await http.PostAsJsonAsync("api/Auth/register", req);
        if (!res.IsSuccessStatusCode)
            throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task LogoutAsync()
    {
        await _store.ClearAsync();
        _authState.Notify();
    }
}
