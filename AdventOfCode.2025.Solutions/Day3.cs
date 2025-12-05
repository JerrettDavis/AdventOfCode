using AdventOfCode.Abstractions;
using AdventOfCode.Application;

namespace AdventOfCode._2025.Solutions;

public class Day3(
    IDataFetcher fetcher,
    ISolutionConfigurator configurator) :
    BaseSolution(configurator, fetcher)
{
    public override int Year => 2025;
    public override int Day => 3;
    public override long SolveA() =>
        Data.Split("\n", StringSplitOptions.TrimEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Sum(s => LargestNDigitComposite(s, 2));

    private long LargestNDigitComposite(string digits, int n)
    {
        if (digits.Length < n) 
            throw new ArgumentException("Digits must be at least n digits long");
        if (digits.Length == n) 
            return long.Parse(digits);
        if (n == 1) 
            return digits.Select(DigitToInt).Max();
        
        var numbers = digits.Select(DigitToInt).ToList();
        var numberWindow = numbers.Take(numbers.Count - n + 1).ToList();
        var max = numberWindow.Max();
        var maxIndex = numberWindow.IndexOf(max);
        var expandedToN = (long)Math.Pow(10, n-1) * max;
        
        return expandedToN + LargestNDigitComposite(digits[(maxIndex + 1)..], n - 1);
    }
    
    public int DigitToInt(char digit) => digit - '0';

    public override long SolveB()
        => Data.Split("\n", StringSplitOptions.TrimEntries)
        .Select(s => s.Trim())
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Sum(s => LargestNDigitComposite(s, 12));
}