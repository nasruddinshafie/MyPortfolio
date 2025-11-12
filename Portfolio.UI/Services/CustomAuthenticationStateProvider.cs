using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Portfolio.UI.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _authService.GetTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(_anonymous);
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Name, out object? name);
            if (name != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, name.ToString() ?? ""));
            }

            keyValuePairs.TryGetValue(ClaimTypes.Email, out object? email);
            if (email != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, email.ToString() ?? ""));
            }

            keyValuePairs.TryGetValue(ClaimTypes.NameIdentifier, out object? id);
            if (id != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, id.ToString() ?? ""));
            }
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
