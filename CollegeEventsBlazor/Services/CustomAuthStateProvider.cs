using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CollegeEventsBlazor.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenStore _store;
    private readonly JwtHelper _jwt;

    public CustomAuthStateProvider(TokenStore store, JwtHelper jwt)
    {
        _store = store;
        _jwt = jwt;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _store.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        var (claims, _) = _jwt.ParseClaims(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void Notify() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
