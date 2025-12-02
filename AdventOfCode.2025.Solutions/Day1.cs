using AdventOfCode.Abstractions;
using AdventOfCode.Application;

namespace AdventOfCode._2025.Solutions;

/// <summary>
/// Simulates the dial instructions from the Day 1 puzzle, calculating zero-rest and zero-touch counts.
/// </summary>
/// <param name="fetcher">Provides the puzzle input.</param>
/// <param name="configurator">Captures the solution context for dependent services.</param>
public class Day1(
    IDataFetcher fetcher,
    ISolutionConfigurator configurator
) : BaseSolution(configurator, fetcher)
{
    /// <summary>
    /// Counts how many instructions end with the dial resting on zero.
    /// </summary>
    public override long SolveA() => RunSimulation().ZeroRests;

    /// <summary>
    /// Counts how many individual clicks land on zero while processing the instructions.
    /// </summary>
    public override long SolveB() => RunSimulation().ZeroTouches;

    /// <inheritdoc />
    public override int Year => 2025;
    /// <inheritdoc />
    public override int Day => 1;
    
    private Dial RunSimulation()
    {
        var lines = Data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var dial = new Dial();

        foreach (var line in lines)
        {
            var dir = line[0].ToDirection();
            var step = int.Parse(line[1..]);
            
            switch (dir)
            {
                case Dial.Direction.Left:
                    dial.SpinLeft(step);
                    break;
                case Dial.Direction.Right:
                    dial.SpinRight(step);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        return dial;
    }
}

/// <summary>
/// Models the circular safe dial and tracks zero hits during rotations.
/// </summary>
class Dial
{
    private const int Min = 0;
    private const int Max = 99;

    private int _current = 50;
    private int _zeroRests;
    /// <summary>
    /// How many times the dial has rested on zero.
    /// </summary>
    public int ZeroRests => _zeroRests;
    private int _zeroTouches;
    /// <summary>
    /// How many times the dial has been clicked to zero.
    /// </summary>
    public int ZeroTouches => _zeroTouches;
    public int Current => _current;

    private void Back()
    {
        if (_current == Min)
            _current = Max;
        else
            _current--;

        LogZeroTouch(ref _zeroTouches);
    }

    private void Forward()
    {
        if (_current == Max)
            _current = Min;
        else
            _current++;

        LogZeroTouch(ref _zeroTouches);
    }


    private void LogZeroTouch(ref int counter)
    {
        if (_current == 0)
            counter++;
    }

    /// <summary>
    /// Rotates the dial to the left the specified number of steps.
    /// </summary>
    /// <param name="steps">How many clicks to rotate.</param>
    public void SpinLeft(int steps)
    {
        while (steps-- > 0) Back();

        LogZeroTouch(ref _zeroRests);
    }

    /// <summary>
    /// Rotates the dial to the right the specified number of steps.
    /// </summary>
    /// <param name="steps">How many clicks to rotate.</param>
    public void SpinRight(int steps)
    {
        while (steps-- > 0) Forward();

        LogZeroTouch(ref _zeroRests);
    }

    internal enum Direction { Left, Right}
}

static class DialExtensions
{
    /// <summary>
    /// Converts the puzzle instruction character into a dial direction.
    /// </summary>
    /// <param name="input">The character token from the instruction line.</param>
    public static Dial.Direction ToDirection(this char input)
        => input switch
        {
            'L' => Dial.Direction.Left,
            'R' => Dial.Direction.Right,
            _ => throw new ArgumentException("Invalid direction")
        };
}
