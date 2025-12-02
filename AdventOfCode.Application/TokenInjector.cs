using Microsoft.Extensions.Options;

namespace AdventOfCode.Application;

/// <summary>
/// Adds the Advent of Code session cookie to outgoing HTTP requests.
/// </summary>
/// <param name="settingsSnapshot">Provides the current application settings containing the session token.</param>
public class TokenInjector(IOptionsSnapshot<AppSettings> settingsSnapshot) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("Cookie", $"session={settingsSnapshot.Value.Token}");
        return base.SendAsync(request, cancellationToken);
    }
}