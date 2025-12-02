using AdventOfCode.Abstractions;

namespace AdventOfCode.Application;

/// <summary>
/// Provides shared plumbing for Advent of Code puzzle solutions, including data fetching and context wiring.
/// </summary>
public abstract class BaseSolution : ISolution
{
    private readonly IDataFetcher _fetcher;

    private string? _data;
    
    /// <summary>
    /// Gets the normalized puzzle input retrieved during <see cref="InitializeAsync"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the solution has not been initialized.</exception>
    protected string Data => _data ?? throw new InvalidOperationException("Solution not initialized");
    
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSolution"/> class and records the active context.
    /// </summary>
    /// <param name="configurator">Receives the solution context so other services can reuse it.</param>
    /// <param name="fetcher">Supplies puzzle input data on demand.</param>
    protected BaseSolution(
        ISolutionConfigurator configurator,
        IDataFetcher fetcher)
    {
        _fetcher = fetcher;
        
        configurator.SetContext(this);
    }
    
    /// <summary>
    /// Downloads and caches the puzzle input for later use.
    /// </summary>
    public async Task InitializeAsync()
    {
        _data = await _fetcher.GetData();
    }

    /// <inheritdoc />
    public abstract int Year { get; }
    /// <inheritdoc />
    public abstract int Day { get; }
    /// <inheritdoc />
    public abstract long SolveA();
    /// <inheritdoc />
    public abstract long SolveB();
}