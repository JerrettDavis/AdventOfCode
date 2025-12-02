using AdventOfCode._2025.Solutions;
using AdventOfCode.Application;

namespace AdventOfCode.Tests._2025;

public class Day2Tests : TestBase
{
    public override int Year => 2025;
    public override int Day => 2;
    
    public override bool SkipGetData => false;
    
    [Fact]
    public async Task Part1_SmokeTest()
    {
        // Arrange
        const string input = "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";
        
        var inputFetcher = new TestDataFetcher(input);
        var solver = new Day2(inputFetcher, new SolutionContext());
        await solver.InitializeAsync();
        
        // Act
        var result = solver.SolveA();
        
        // Assert
        Assert.Equal(1227775554, result);
    }
    
    [Fact]
    public void Part1_RealData()
    {
        // Act
        var result = Solution.SolveA();

        // Assert
        Assert.Equal(12850231731, result);
    }
    
    
    [Fact]
    public void Part2_RealData()
    {
        // Act
        var result = Solution.SolveB();
        
        // Assert
        Assert.Equal(24774350322, result);
    }
}