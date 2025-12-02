using AdventOfCode.Abstractions;
using AdventOfCode.Application;

namespace AdventOfCode._2025.Solutions;

public class Day2(
    IDataFetcher fetcher,
    ISolutionConfigurator configurator
) : BaseSolution(configurator, fetcher)
{
    /// <inheritdoc />
    public override int Year => 2025;

    /// <inheritdoc />
    public override int Day => 2;

    public override long SolveA()
    {
        return InputProcessor.Process(Data);
    }

    public override long SolveB()
    {
        return InputProcessor.Process(Data, true);
    }
}

static class InputProcessor
{
    public static long Process(string input, bool any = false)
    {
        var ranges = input.Split(",");
        var doubleCount = 0L;
        var anyCount = 0L;
        foreach (var range in ranges)
        {
            var parts = range.Split("-");
            var min = long.Parse(parts[0]);
            var max = long.Parse(parts[1]);

            doubleCount += SequencesInRange(min, max, OnlyRepeatedSequenceTwice);
            anyCount += SequencesInRange(min, max, OnlyRepeatedSequenceAny);
        }

        var ret = any ? anyCount : doubleCount;
        return ret;
    }

    private static bool OnlyRepeatedSequenceTwice(string input)
    {
        var len = input.Length;
        if (len % 2 != 0) return false;
        var half = len / 2;
        var firstHalf = input[..half];
        var secondHalf = input.Substring(half, half);
        return firstHalf == secondHalf;
    }

    private static bool OnlyRepeatedSequenceAny(string input)
    {
        var len = input.Length;
        for (var subLen = 1; subLen <= len / 2; subLen++)
        {
            if (len % subLen != 0) continue;
            var subStr = input[..subLen];
            var repeated = true;
            for (var i = 0; i < len; i += subLen)
            {
                var part = input.Substring(i, subLen);
                if (part == subStr)
                    continue;
                repeated = false;
                break;
            }

            if (repeated) return true;
        }

        return false;
    }

    private static long SequencesInRange(long min, long max, Func<string, bool> predicate)
    {
        var count = 0L;
        for (var i = min; i <= max; i++)
        {
            var str = i.ToString();
            if (predicate(str))
            {
                count += i;
            }
        }

        return count;
    }
}