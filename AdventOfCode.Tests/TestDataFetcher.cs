using AdventOfCode.Abstractions;

namespace AdventOfCode.Tests;

/// <summary>
/// Lightweight test double that returns deterministic puzzle input.
/// </summary>
/// <param name="data">The data to return when <see cref="GetData"/> is called.</param>
class TestDataFetcher(string data) : IDataFetcher
{
    /// <inheritdoc />
    public ValueTask<string> GetData() => new(data.ReplaceLineEndings("\n"));
}