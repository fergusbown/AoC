using System.Reflection.Metadata.Ecma335;
using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2022_14 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_14(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (var grid, var sourceColumn) = Parse(inputData, false);

        return DropSand(new Span2D<bool>(grid), sourceColumn).ToString();
    }

    public string Part2()
    {
        (var grid, var sourceColumn) = Parse(inputData, true);

        return DropSand(new Span2D<bool>(grid), sourceColumn).ToString();
    }

    private int DropSand(Span2D<bool> space, int sourceColumn)
    {
        int grains = 0;
        int[] columnDeltas = new[] { 0, -1, 1 };

        while (true)
        {
            int grainRow = 0;
            int grainColumn = sourceColumn;

            if (space[grainRow, grainColumn])
            {
                return grains;
            }

            while (!space[grainRow, grainColumn])
            {
                (bool outOfPlay, grainRow, grainColumn) = TryMove(space, columnDeltas, grainRow, grainColumn);

                if (outOfPlay)
                {
                    return grains;
                }
            }

            grains++;
        }

        static (bool OutOfPlay, int GrainRow, int GrainColumn) TryMove(Span2D<bool> space, int[] columnDeltas, int grainRow, int grainColumn)
        {
            int testRow = grainRow + 1;

            foreach (var columnDelta in columnDeltas)
            {
                int testColumn = grainColumn + columnDelta;

                if (OutOfPlay(space, testRow, testColumn, out bool occupied))
                {
                    return (true, grainRow, grainColumn);
                }
                else if (!occupied)
                {
                    return (false, testRow, testColumn);
                }
            }

            space[grainRow, grainColumn] = true;
            return (false, grainRow, grainColumn);

            static bool OutOfPlay(Span2D<bool> space, int row, int column, out bool occupied)
            {
                occupied = false;

                if (row >= space.Height)
                {
                    return true;
                }

                if (column < 0)
                {
                    return true;
                }

                if (column >= space.Width)
                {
                    return true;
                }

                occupied = space[row, column];
                return false;
            }
        }
    }

    private static (bool[,], int sourceColumn) Parse(string input, bool addFloor)
    {
        const int sandSourceColumn = 500;

        (int X, int Y)[][] rocks = input
            .StringsForDay()
            .Select(s => s.Split(" -> ")
                .Select(c =>
                {
                    var xy = c.Split(',');
                    (int X, int Y) p = (int.Parse(xy[0]), int.Parse(xy[1]));
                    return p;
                })
                .ToArray())
            .ToArray();

        int maxY = rocks
            .SelectMany(r => r)
            .Select(r => r.Y)
            .Max() + 1;

        int minX = rocks
            .SelectMany(r => r)
            .Select(r => r.X)
            .Min();

        int maxX = rocks
            .SelectMany(r => r)
            .Select(r => r.X)
            .Max();

        if (addFloor)
        {
            maxY += 2;
            minX = Math.Min(minX, sandSourceColumn - maxY);
            maxX = Math.Max(maxX, sandSourceColumn + maxY);
        }

        int width = maxX - minX + 1;

        Span2D<bool> result = new(new bool[width * maxY], maxY, width);

        foreach (var rockPath in rocks)
        {
            var previous = rockPath[0];

            foreach (var point in rockPath.Skip(1))
            {
                int left = Math.Min(previous.X, point.X);
                int right = Math.Max(previous.X, point.X);
                int top = Math.Min(previous.Y, point.Y);
                int bottom = Math.Max(previous.Y, point.Y);

                left -= minX;
                right -= minX;

                for (int column = left; column <= right; column++)
                {
                    for (int row = top; row <= bottom; row++)
                    {
                        result[row, column] = true;
                    }
                }

                previous = point;
            }
        }

        if (addFloor)
        {
            result.GetRow(result.Height - 1).Fill(true);
        }

        return (result.ToArray(), sandSourceColumn - minX);
    }
}
