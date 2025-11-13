using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using Portfolio.UI.DTOs.Auth;

namespace Portfolio.UI.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";
    private string? _cachedToken;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>()
            ?? throw new InvalidOperationException("Failed to deserialize authentication response");

        await SetTokenAsync(authResponse.Token);
        return authResponse;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
        response.EnsureSuccessStatusCode();

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>()
            ?? throw new InvalidOperationException("Failed to deserialize authentication response");

        await SetTokenAsync(authResponse.Token);
        return authResponse;
    }

    public async Task LogoutAsync()
    {
        await RemoveTokenAsync();
    }

    public async Task<string?> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken))
            return _cachedToken;

        try
        {
            _cachedToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);

            return _cachedToken;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    private async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
    }

    private async Task RemoveTokenAsync()
    {
        _cachedToken = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }
}
