using AdventOfCode.Abstractions;

namespace AdventOfCode.Application;

/// <summary>
/// Maintains the (year, day) pair shared between solutions and infrastructure components.
/// </summary>
public class SolutionContext : ISolutionConfigurator, ISolutionContext
{
    /// <inheritdoc />
    public int Year { get; private set; }
    /// <inheritdoc />
    public int Day { get; private set; }
    
    /// <summary>
    /// Copies the supplied context values into this instance so consumers can read them later.
    /// </summary>
    /// <param name="context">The context whose year/day values should be reused.</param>
    public void SetContext(ISolutionContext context) => (Year, Day) = (context.Year, context.Day);
}