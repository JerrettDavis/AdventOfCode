namespace AdventOfCode.Abstractions;
/// <summary>
/// Represents the (year, day) tuple a solution operates on.
/// </summary>
public interface ISolutionContext
{
    /// <summary>
    /// Gets the Advent of Code year for the current solution.
    /// </summary>
    public int Year { get; }
    /// <summary>
    /// Gets the day within the configured year for the current solution.
    /// </summary>
    public int Day { get; }
}