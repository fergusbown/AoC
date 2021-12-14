using System.Drawing;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner;

internal class Day11 : IDayChallenge
{
    private readonly int[] initialLevels;

    public Day11(string inputData)
    {
        this.initialLevels = inputData
            .Replace(Environment.NewLine, string.Empty)
            .Select(i => i - '0')
            .ToArray();
    }

    public string Part1()
    {
        var octopi = GetInput(initialLevels);

        int flashes = 0;
        Point topLeft = new(0, 0);
        Point bottomRight = new(octopi.Width - 1, octopi.Height - 1);
        for (int i = 0; i < 100; i++)
        {
            Reset(octopi);
            flashes += Step(octopi, topLeft, bottomRight);
        }

        return flashes.ToString();
    }

    public string Part2()
    {
        var octopi = GetInput(initialLevels);

        int steps = 1;
        Point topLeft = new(0, 0);
        Point bottomRight = new(octopi.Width - 1, octopi.Height - 1);

        while (Step(octopi, topLeft, bottomRight) != octopi.Length)
        {
            steps++;
            Reset(octopi);
        }

        return steps.ToString();
    }

    private static Span2D<Octopus> GetInput(int[] input)
    {
        var octopi = input
            .Select(i => new Octopus(i))
            .ToArray();

        int size = (int)Math.Sqrt(octopi.Length);

        return new Span2D<Octopus>(octopi, size, size);
    }

    private static void Reset(Span2D<Octopus> octopi)
    {
        for (int row = 0; row < octopi.Height; row++)
        {
            for (int column = 0; column < octopi.Width; column++)
            {
                octopi[row, column].Reset();
            }
        }
    }

    private int Step(Span2D<Octopus> octopi, Point topLeft, Point bottomRight)
    {
        int flashes = 0;
        for (int row = topLeft.Y; row <= bottomRight.Y; row++)
        {
            for (int column = topLeft.X; column <= bottomRight.X; column++)
            {
                var octopus = octopi[row, column];

                if (octopus.Energize())
                {
                    flashes++;

                    (int startRow, int endRow) = GetRange(row, octopi.Height);
                    (int startCol, int endCol) = GetRange(column, octopi.Width);

                    flashes += Step(octopi, new Point(startCol, startRow), new Point(endCol, endRow));
                }
            }
        }

        return flashes;

        static (int Start, int End) GetRange(int current, int size)
        {
            int start = Math.Max(0, current - 1);
            int end = Math.Min(current + 1, size - 1);

            return (start, end);
        }

    }

    private class Octopus
    {
        public int Energy { get; private set; }

        public Octopus(int energy)
        {
            Energy = energy;
        }

        public bool Energize()
            => Energy++ == 9;

        public void Reset()
        {
            if (Energy > 9)
            {
                Energy = 0;
            }
        }
    }
}
