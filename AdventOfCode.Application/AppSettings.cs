namespace AdventOfCode.Application;

/// <summary>
/// Provides configuration values required to fetch Advent of Code input data.
/// </summary>
public record AppSettings
{
    /// <summary>
    /// Gets the Advent of Code session token used to authenticate HTTP requests.
    /// </summary>
    public required string Token { get; init; }
    /// <summary>
    /// Gets the optional directory where puzzle input files are cached between runs.
    /// </summary>
    public string? AocDataDirectory { get; init; }

}