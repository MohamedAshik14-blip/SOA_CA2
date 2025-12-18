using System.Security.Claims;
using System.Text.Json;

namespace CollegeEventsBlazor.Services;

public class JwtHelper
{
    public (IEnumerable<Claim> claims, string? role) ParseClaims(string jwt)
    {
        try
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3) return (Enumerable.Empty<Claim>(), null);

            var payload = parts[1];
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(payload);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)
                       ?? new Dictionary<string, object>();

            var claims = new List<Claim>();
            foreach (var kv in dict)
            {
                if (kv.Value is JsonElement je)
                {
                    if (je.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in je.EnumerateArray())
                            claims.Add(new Claim(kv.Key, item.ToString()));
                    }
                    else
                    {
                        claims.Add(new Claim(kv.Key, je.ToString()));
                    }
                }
                else
                {
                    claims.Add(new Claim(kv.Key, kv.Value?.ToString() ?? ""));
                }
            }

           
            var role = claims.FirstOrDefault(c => c.Type == "role")?.Value
                       ?? claims.FirstOrDefault(c => c.Type.EndsWith("/role"))?.Value;

            return (claims, role);
        }
        catch
        {
            return (Enumerable.Empty<Claim>(), null);
        }
    }
}
