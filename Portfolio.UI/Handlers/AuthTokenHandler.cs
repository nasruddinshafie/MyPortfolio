using Microsoft.JSInterop;

namespace Portfolio.UI.Handlers;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;
    private const string TokenKey = "authToken";

    public AuthTokenHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get the token from localStorage
            // This may fail during prerendering when JSInterop is not available
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);

            // If token exists, add it to the Authorization header
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (InvalidOperationException)
        {
            // JSInterop is not available during prerendering
            // This is expected and we can safely ignore it
            // The request will proceed without the token
        }
        catch (JSException)
        {
            // JSInterop failed for some reason
            // Proceed without the token
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
