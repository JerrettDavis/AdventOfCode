namespace AdventOfCode.Abstractions;
/// <summary>
/// Provides puzzle input data for a solution
/// </summary>
public interface IDataFetcher
{
    /// <summary>
    /// Retrieves the raw puzzle input for the currently configured solution context.
    /// </summary>
    /// <returns>A task that completes with the normalized puzzle input text.</returns>
    public ValueTask<string> GetData();
}