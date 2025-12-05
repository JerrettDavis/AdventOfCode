using AdventOfCode._2025.Solutions;
using AdventOfCode.Application;

namespace AdventOfCode.Tests._2025;

public class Day3Tests : TestBase
{
    public override int Year => 2025;
    public override int Day => 3;

    [Fact]
    public async Task Part1_SmokeTest()
    {
        // Arrange
        const string input = """
                             987654321111111
                             811111111111119
                             234234234234278
                             818181911112111
                             """;

        var inputFetcher = new TestDataFetcher(input);
        var solver = new Day3(inputFetcher, new SolutionContext());
        await solver.InitializeAsync();
        
        // Act
        var result = solver.SolveA();
        
        // Assert
        Assert.Equal(357, result);
    }

    [Fact]
    public async Task Part2_SmokeTest()
    {
        // Arrange
        const string input = """
                             987654321111111
                             811111111111119
                             234234234234278
                             818181911112111
                             """;

        var inputFetcher = new TestDataFetcher(input);
        var solver = new Day3(inputFetcher, new SolutionContext());
        await solver.InitializeAsync();
        
        // Act
        var result = solver.SolveB();
        
        // Assert
    Assert.Equal(3121910778619, result);
    }
}