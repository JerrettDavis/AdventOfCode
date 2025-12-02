using AdventOfCode.Application;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Console.Browser;

/// <summary>
/// Delegating handler that resolves an Advent of Code session cookie from the local browser before sending requests.
/// </summary>
/// <param name="settingsSnapshot">Provides fallback configuration when browser cookies are unavailable.</param>
public class BrowserTokenInjector(IOptionsSnapshot<AppSettings> settingsSnapshot) : DelegatingHandler
{
    private string? _cachedSession;

    /// <summary>
    /// Ensures outgoing requests carry a valid Advent of Code session cookie by checking cached and browser-sourced values.
    /// </summary>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(settingsSnapshot.Value.Token))
            _cachedSession = settingsSnapshot.Value.Token;
        
        if (string.IsNullOrEmpty(_cachedSession))
        {
            _cachedSession =
                await BrowserCookieReader.GetAdventOfCodeSessionAsync(
                    browser: Browser.Chrome,
                    profileName: "Default")
                ?? await BrowserCookieReader.GetAdventOfCodeSessionAsync(
                    browser: Browser.Edge,
                    profileName: "Default") 
                ?? settingsSnapshot.Value.Token; 
        }

        if (string.IsNullOrEmpty(_cachedSession))
            return await base.SendAsync(request, cancellationToken);
        
        request.Headers.Remove("Cookie");
        request.Headers.Add("Cookie", $"session={_cachedSession}");

        return await base.SendAsync(request, cancellationToken);
    }
}
