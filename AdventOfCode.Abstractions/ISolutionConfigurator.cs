namespace AdventOfCode.Abstractions;
/// <summary>
/// Exposes the ability to bind solutions to an execution context.
/// </summary>
public interface ISolutionConfigurator
{
    /// <summary>
    /// Copies the supplied context data onto the configurator implementation.
    /// </summary>
    /// <param name="context">The context that describes the current year/day.</param>
    public void SetContext(ISolutionContext context);
}