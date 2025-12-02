using AdventOfCode._2025.Solutions;
using AdventOfCode.Application;

namespace AdventOfCode.Tests._2025;

/// <summary>
/// Verifies the 2025 Day 1 dial simulation using sample inputs and real puzzle data.
/// </summary>
public class Day1Tests : TestBase
{
    /// <summary>
    /// Validates the sample input for part 1 produces the documented result.
    /// </summary>
    [Fact]
    public async Task Part1_SmokeTest()
    {
        // Arrange
        const string input = """
                             L68
                             L30
                             R48
                             L5
                             R60
                             L55
                             L1
                             L99
                             R14
                             L82
                             """;
        var inputFetcher = new TestDataFetcher(input);
        var solver = new Day1(inputFetcher, new SolutionContext());
        await solver.InitializeAsync();
        
        // Act
        var result = solver.SolveA();
        
        // Assert
        Assert.Equal(3, result);
    }

    /// <summary>
    /// Executes part 1 against the actual puzzle input to guard against regressions.
    /// </summary>
    [Fact]
    public void Part1_RealData()
    {
        // Act
        var result = Solution.SolveA();

        // Assert
        Assert.Equal(964, result);
    }

    /// <summary>
    /// Validates the sample input for part 2 using method 0x434C49434B rules.
    /// </summary>
    [Fact]
    public async Task Part2_SmokeTest()
    {
        // Arrange
        const string input = """
                             L68
                             L30
                             R48
                             L5
                             R60
                             L55
                             L1
                             L99
                             R14
                             L82
                             """;
        var inputFetcher = new TestDataFetcher(input);
        var solver = new Day1(inputFetcher, new SolutionContext());
        await solver.InitializeAsync();
        
        // Act
        var result = solver.SolveB();
        
        // Assert
        Assert.Equal(6, result);
    }

    /// <summary>
    /// Executes part 2 against the actual puzzle input to guard against regressions.
    /// </summary>
    [Fact]
    public void Part2_RealData()
    {
        // Act
        var result = Solution.SolveB();
        
        // Assert
        Assert.Equal(5872, result);
    }

    /// <inheritdoc />
    public override int Year => 2025;
    /// <inheritdoc />
    public override int Day => 1;
    /// <inheritdoc />
    public override bool SkipGetData => false;
}