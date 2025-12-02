namespace AdventOfCode.Abstractions;
/// <summary>
/// Defines a puzzle solution that can compute answers for parts A and B.
/// </summary>
public interface ISolution : ISolutionContext
{
    /// <summary>
    /// Executes the logic for part A using the prepared input data.
    /// </summary>
    /// <returns>The computed answer for part A.</returns>
    public int SolveA();
    /// <summary>
    /// Executes the logic for part B using the prepared input data.
    /// </summary>
    /// <returns>The computed answer for part B.</returns>
    public int SolveB();
}