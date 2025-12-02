using AdventOfCode.Abstractions;
using AdventOfCode.Application;
using AdventOfCode.Tests._2025;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Testing;

namespace AdventOfCode.Tests;

/// <summary>
/// Provides common host wiring and solution initialization helpers for Advent of Code tests.
/// </summary>
public abstract class TestBase : ISolutionContext, IAsyncLifetime
{
    private IHost _host = null!;
    public IHost Host => _host;
    public string Data { get; private set; } = null!;
    public ISolution Solution { get; private set; } = null!;

    /// <summary>
    /// Sets up the fake host used to resolve solutions for tests.
    /// </summary>
    public Task RunHost()
    {
        var builder = FakeHost.CreateBuilder();
        var context = new SolutionContext();
        
        builder
            .ConfigureAppConfiguration((_, config) => config.AddUserSecrets(typeof(Day1Tests).Assembly))
            .ConfigureServices((host, services) =>
                services
                    .Configure<AppSettings>(host.Configuration)
                    .AddTransient<TokenInjector>()
                    .AddHttpClient<IDataFetcher, DataFetcher>()
                    .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://adventofcode.com/"))
                    .AddHttpMessageHandler<TokenInjector>().Services
                    .AddSingleton<ISolutionContext>(context)
                    .AddSingleton<ISolutionConfigurator>(context)
                    .AddSolutions()
            );

        _host = builder.Build();

        return Host.StartAsync();
    }

    /// <summary>
    /// Initializes the host and retrieves the keyed solution instance for the concrete test.
    /// </summary>
    public async Task InitializeAsync()
    {
        await RunHost();
        using var scope = Host.Services.CreateScope();
        var solution = scope.ServiceProvider.GetRequiredKeyedService<BaseSolution>($"{Year}{Day:D2}");
        Solution = solution;
        if (!SkipGetData)
            await solution.InitializeAsync();
    }

    /// <summary>
    /// Stops the host and releases resources once the test suite completes.
    /// </summary>
    public Task DisposeAsync()
        => _host.StopAsync();

    /// <inheritdoc />
    public abstract int Year { get; }
    /// <inheritdoc />
    public abstract int Day { get; }
    /// <summary>
    /// Determines whether the solution should skip fetching input data during <see cref="InitializeAsync"/>.
    /// </summary>
    public virtual bool SkipGetData => true;
}