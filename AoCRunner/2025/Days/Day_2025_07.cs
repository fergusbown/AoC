using System.Collections.Immutable;
using AoCRunner;
using CommunityToolkit.HighPerformance;

internal class Day_2025_07 : IDayChallenge
{
    private readonly string inputData;

    public Day_2025_07(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        Span2D<char> grid = this.inputData.GridForDay(c => c);

        ReadOnlySpan<char> s = ['S'];
        int firstBeem = grid.GetRowSpan(0).IndexOf(s, StringComparison.Ordinal);
        grid[1, firstBeem] = '|';

        int splits = 0;

        for (int row = 1; row < grid.Height - 1; row++)
        {
            for (int column = 0; column < grid.Width; column++)
            {
                if (grid[row, column] == '|')
                {
                    switch (grid[row + 1, column])
                    {
                        case '.':
                            grid[row + 1, column] = '|';
                            break;
                        case '^':
                            if (grid[row + 1, column - 1] == '.' || grid[row + 1, column + 1] == '.')
                            {
                                grid[row + 1, column - 1] = '|';
                                grid[row + 1, column + 1] = '|';
                                splits++;
                            }

                            break;
                        default:
                            break;
                    }

                }
            }
        }

        return splits.ToString();
    }

    public string Part2()
    {
        Span2D<char> grid = this.inputData.GridForDay(c => c);

        ReadOnlySpan<char> s = ['S'];
        int firstBeem = grid.GetRowSpan(0).IndexOf(s, StringComparison.Ordinal);

        Dictionary<int, long> routes = new() { [firstBeem] = 1 };

        for (int row = 2; row < grid.Height - 1; row++)
        {
            Dictionary<int, long> nextRoutes = [];

            void AddOrUpdate(int column, long count)
            {
                if (!nextRoutes.TryAdd(column, count))
                {
                    nextRoutes[column] += count;
                }
            }

            foreach ((var column, var count) in routes)
            {
                char next = grid[row, column];

                if (next == '.')
                {
                    AddOrUpdate(column, count);
                }
                else if (next == '^')
                {
                    AddOrUpdate(column - 1, count);
                    AddOrUpdate(column + 1, count);
                }
            }

            routes = nextRoutes;
        }

        return routes.Select(r => r.Value).Sum().ToString();
    }
}
