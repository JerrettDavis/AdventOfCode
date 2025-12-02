# Advent of Code Solver

Automated Advent of Code runner written in .NET 10.0. The solution is split into focused projects so puzzle logic stays clean while shared plumbing (input fetching, dependency injection, CLI hosting, and testing utilities) lives in reusable libraries.

## Repository Layout

| Project | Purpose |
| --- | --- |
| `AdventOfCode.Abstractions` | Interfaces (`ISolution`, `ISolutionContext`, `IDataFetcher`, etc.) that define the contract every puzzle implementation follows. |
| `AdventOfCode.Application` | Infrastructure: base solution class, input fetching, session token injectors, and DI helpers that discover/register solutions automatically. |
| `AdventOfCode.Console` | CLI entry point that wires services, parses `year`/`day`, and logs both part answers. Includes optional browser cookie support to auto-populate the AoC session token. |
| `AdventOfCode.2025.Solutions` | Real puzzle implementations. Add future days/years here following the same pattern. |
| `AdventOfCode.Tests` | xUnit test suite with reusable host setup plus sample/real-data regression tests. |

## Requirements

- [.NET SDK 10.0-preview](https://dotnet.microsoft.com/download)
- Access to your Advent of Code session token (for authenticated input downloads)
- Windows/macOS/Linux with Chrome or Edge installed if you want automatic cookie extraction

## First-Time Setup

1. Restore packages and build:
   ```powershell
   dotnet build AdventOfCode.slnx
   ```
2. Configure your Advent of Code session token so HTTP requests can be authenticated. You have two options:
   - **Browser auto-detection** (default): Chrome/Edge cookies are read via `BrowserTokenInjector`. Just make sure you are logged in to adventofcode.com in at least one of those browsers.
   - **Explicit configuration**: set the token via user-secrets or environment variables consumed by `AppSettings.Token`.

   ```powershell
   cd AdventOfCode.Console
   dotnet user-secrets init
   dotnet user-secrets set "Token" "<your-session-token>"
   ```

   Optionally set a persistent cache directory so inputs are written someplace specific:
   ```powershell
   dotnet user-secrets set "AocDataDirectory" "G:\\AoC\\Inputs"
   ```

## Running a Puzzle

Use the console project and pass the Advent of Code year/day to solve:

```powershell
dotnet run --project AdventOfCode.Console -- 2025 1
```

- Inputs are fetched once and cached under `AocDataDirectory` (or a temp folder if unspecified).
- Results for part A and B are logged at `Information` level.

## Writing a New Solution

1. Create a class in `AdventOfCode.{YEAR}.Solutions` that inherits `BaseSolution` and implements `Year`, `Day`, `SolveA`, and `SolveB`.
2. Inject `IDataFetcher` and `ISolutionConfigurator` into the constructor and pass them to `BaseSolution`.
3. Call `InitializeAsync()` before solving so `Data` is populated.
4. Add regression tests under `AdventOfCode.Tests/{YEAR}` using `TestBase` for consistent DI + configuration.
5. Run the full test suite to confirm everything still passes.

Because `DependencyInjection.AddSolutions` scans assemblies automatically, no manual registration is needed as long as the class lives in a referenced `*.Solutions` project.

## Testing

Run all tests (sample data + real puzzle guards) with:

```powershell
dotnet test AdventOfCode.slnx
```

## Troubleshooting

- **Missing session token**: either log into Advent of Code in Chrome/Edge or set `AppSettings.Token` via user secrets/environment variables.
- **Cached input mismatch**: delete the file under the configured `AocDataDirectory` (named `{year}_{day}.txt`) and rerun.
- **Solution discovery issues**: ensure the new solution class is `public`, non-abstract, derives from `BaseSolution`, and exposes constant `Year`/`Day` property getters so the scanner can read them.

