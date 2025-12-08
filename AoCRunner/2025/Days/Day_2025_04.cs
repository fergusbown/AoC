using AoCRunner;
using CommunityToolkit.HighPerformance;

internal class Day_2025_04 : IDayChallenge
{
    private bool[,] inputData;

    public Day_2025_04(string inputData)
    {
        this.inputData = inputData.GridForDay(c => c == '@').ToArray();
    }

    public string Part1()
    {
        Span2D<bool> grid = inputData.AsSpan2D();
        int result = 0;

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[y, x] && grid.Adjacencies(y, x).Where(b => b).Count() < 4)
                {
                    result++;
                }
            }
        }

        return result.ToString();
    }

    public string Part2()
    {
        Span2D<bool> grid = inputData.AsSpan2D();
        int result = 0;
        int total;
        do
        {
            total = 0;
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    if (grid[y, x] && grid.Adjacencies(y, x).Where(b => b).Count() < 4)
                    {
                        total++;
                        grid[y, x] = false;
                    }
                }
            }

            result += total;
        }
        while (total > 0);

        return result.ToString();
    }
}
