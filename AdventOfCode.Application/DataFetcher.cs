using AdventOfCode.Abstractions;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Application;

/// <summary>
/// Retrieves puzzle input via HTTP and caches it locally for repeat executions.
/// </summary>
/// <param name="client">HTTP client configured for the Advent of Code host.</param>
/// <param name="optionsSnapshot">Provides access to the latest <see cref="AppSettings"/>.</param>
/// <param name="context">Supplies the year/day pair that determines the input file.</param>
public class DataFetcher(
    HttpClient client,
    IOptionsMonitor<AppSettings> optionsSnapshot,
    ISolutionContext context) : IDataFetcher
{
    /// <summary>
    /// Downloads the input file if needed and returns its contents with normalized line endings.
    /// </summary>
    public async ValueTask<string> GetData()
    {
        // Check the data directory for the input file first ($"{context.Year}_{context.Day}.txt")"
        var dataDirectory = optionsSnapshot.CurrentValue.AocDataDirectory;
        
        // If we don't have a data directory, create a consistent temporary directory for testing
        if (dataDirectory is null)
        {
            dataDirectory = Path.Combine(Path.GetTempPath(), $"{context.Year}_{context.Day}");
            Directory.CreateDirectory(dataDirectory);
            Environment.SetEnvironmentVariable(nameof(AppSettings.AocDataDirectory), dataDirectory);
        }
        var inputPath = Path.Combine(dataDirectory, $"{context.Year}_{context.Day}.txt");
        if (File.Exists(inputPath)) 
            return (await File.ReadAllTextAsync(inputPath)).ReplaceLineEndings("\n");

        var result = await client.GetStringAsync($"{context.Year}/day/{context.Day}/input");
        await File.WriteAllTextAsync(inputPath, result);
        
        return result.ReplaceLineEndings("\n");
    }
}