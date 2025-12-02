using System.CommandLine;
using AdventOfCode.Abstractions;
using AdventOfCode.Application;
using AdventOfCode.Console.Browser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets(typeof(Program).Assembly);

builder.Services
    .Configure<AppSettings>(builder.Configuration)
    .AddTransient<BrowserTokenInjector>()
    .AddHttpClient<IDataFetcher, DataFetcher>(client => { client.BaseAddress = new Uri("https://adventofcode.com/"); })
    .AddHttpMessageHandler<BrowserTokenInjector>().Services
    .AddSingleton<SolutionContext>()
    .AddSingleton<ISolutionContext, SolutionContext>(sp => sp.GetRequiredService<SolutionContext>())
    .AddSingleton<ISolutionConfigurator, SolutionContext>(sp => sp.GetRequiredService<SolutionContext>())
    .AddSolutions();

var rootCommand = new RootCommand
{
    Description = "Advent of Code Solver"
};

var yearArgument = new Argument<int>("year")
{
    Description = "Year of the puzzle"
};

var dayArgument = new Argument<int>("day")
{
    Description = "Day of the puzzle"
};

rootCommand.Arguments.Add(yearArgument);
rootCommand.Arguments.Add(dayArgument);

rootCommand.SetAction(async (parseResult, cancellationToken) =>
{
    try
    {
        var y = parseResult.GetValue(yearArgument);
        var d = parseResult.GetValue(dayArgument);

        using var host = builder.Build();
        await host.StartAsync(cancellationToken);

        var solutionKey = $"{y}{d:D2}";
        using var scope = host.Services.CreateScope();
        var solution = scope.ServiceProvider.GetRequiredKeyedService<BaseSolution>(solutionKey);
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        await solution.InitializeAsync();

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Part A: {SolveA}", solution.SolveA());
            logger.LogInformation("Part B: {SolveB}", solution.SolveB());
        }

        await host.StopAsync(cancellationToken);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});

return await rootCommand.Parse(args).InvokeAsync();
