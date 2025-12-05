using AdventOfCode.Abstractions;
using AdventOfCode.Application;

namespace AdventOfCode._2025.Solutions;

public class Day4(
    IDataFetcher fetcher,
    ISolutionConfigurator configurator
) : BaseSolution(configurator, fetcher)
{
    public override int Year => 2025;
    public override int Day => 4;
    
    private string[][] Grid 
        => Data.Split("\n", StringSplitOptions.TrimEntries)
        .Select(s => s.Trim())
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Select(line => line.ToCharArray().Select(c => c.ToString()).ToArray())
        .ToArray();

    private bool AdjacentCellsHaveLessThan4Tps(string[][] g, int x, int y)
    {
        if (!IsTp(g[y][x])) return false;

        var count = 0;
        if (x > 0) count += IsTp(g[y][x - 1]) ? 1 : 0;
        if (x < g[y].Length - 1) count += IsTp(g[y][x + 1]) ? 1 : 0;
        if (y > 0) count += IsTp(g[y - 1][x]) ? 1 : 0;
        if (y < g.Length - 1) count += IsTp(g[y + 1][x]) ? 1 : 0;
        if (x > 0 && y > 0) count += IsTp(g[y - 1][x - 1]) ? 1 : 0;
        if (x < g[y].Length - 1 && y > 0) count += IsTp(g[y - 1][x + 1]) ? 1 : 0;
        if (x > 0 && y < g.Length - 1) count += IsTp(g[y + 1][x - 1]) ? 1 : 0;
        if (x < g[y].Length - 1 && y < g.Length - 1) count += IsTp(g[y + 1][x + 1]) ? 1 : 0;
        return count < 4;
    }

    private bool IsTp(string cell) => cell.Equals("@");
    
    public override long SolveA() =>
        Grid
            .SelectMany((row, y) => row.Select((cell, x) => (cell, x, y)))
            .Where(c => AdjacentCellsHaveLessThan4Tps(Grid, c.x, c.y))
            .ToArray()
            .Length;

    public override long SolveB() => CountMarked(MarkAccessible(Grid));

    private string[][] MarkAccessible(string[][] grid)
    {
        var clone = grid.ToArray();
        int marked, markedBefore;
        do
        {
            markedBefore = CountMarked(clone);
            foreach (var (x, y) in grid.SelectMany((row, y) => row.Select((cell, x) => (x, y)))) 
                clone = MarkAccessible(clone, x, y);
            
            marked = CountMarked(clone);
        } while (marked != markedBefore);

        return clone;
    }
    
    private static int CountMarked(string[][] grid) 
        => grid.SelectMany(row => row).Count(c => c == "x");

    private string[][] MarkAccessible(string[][] grid, int x, int y)
    {
        var clone = grid.ToArray();
        if (!AdjacentCellsHaveLessThan4Tps(grid, x, y)) return clone;
        clone[y][x] = "x";
        return clone;
    }
}