using AdventOfCode._2025.Solutions;
using AdventOfCode.Application;

namespace AdventOfCode.Tests._2025;

public class Day4Tests : TestBase
{
    public override int Year => 2025;
    public override int Day => 4;

    public override bool SkipGetData => true;

    [Fact]
    public async Task Part1_SmokeTest()
    {
        // Arrange
        const string input = """
                             ..@@.@@@@.
                             @@@.@.@.@@
                             @@@@@.@.@@
                             @.@@@@..@.
                             @@.@@@@.@@
                             .@@@@@@@.@
                             .@.@.@.@@@
                             @.@@@.@@@@
                             .@@@@@@@@.
                             @.@.@@@.@.
                             """;
        
        var inputFetcher = new TestDataFetcher(input);
        var solver = new Day4(inputFetcher, new SolutionContext());
        await solver.InitializeAsync();
        
        // Act
        var result = solver.SolveA();
        
        // Assert
        Assert.Equal(13, result);
    }
    
    [Fact]
    public async Task Part2_SmokeTest()
    {
        // Arrange
        const string input = """
                             ..@@.@@@@.
                             @@@.@.@.@@
                             @@@@@.@.@@
                             @.@@@@..@.
                             @@.@@@@.@@
                             .@@@@@@@.@
                             .@.@.@.@@@
                             @.@@@.@@@@
                             .@@@@@@@@.
                             @.@.@@@.@.
                             """;
        
        var inputFetcher = new TestDataFetcher(input);
        var solver = new Day4(inputFetcher, new SolutionContext());
        await solver.InitializeAsync();
        
        // Act
        var result = solver.SolveB();
        
        // Assert
        Assert.Equal(43, result);
    }
}