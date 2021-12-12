using System.Drawing;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner
{
    internal class Day11 : IDayChallenge
    {
        public string Part1()
        {
            var octopi = GetInput(inputData);

            int flashes = 0;
            Point topLeft = new Point(0, 0);
            Point bottomRight = new Point(octopi.Width - 1, octopi.Height - 1);
            for (int i = 0; i < 100; i++)
            {
                Reset(octopi);
                flashes += Step(octopi, topLeft, bottomRight);
            }

            return flashes.ToString();
        }

        public string Part2()
        {
            var octopi = GetInput(inputData);

            int steps = 0;
            Point topLeft = new Point(0, 0);
            Point bottomRight = new Point(octopi.Width - 1, octopi.Height - 1);

            while (true)
            {
                steps++;
                Reset(octopi);
                if (Step(octopi, topLeft, bottomRight) == octopi.Length)
                {
                    return steps.ToString();
                }
            }
        }

        private Span2D<Octopus> GetInput(string input)
        {
            var levels = input
                .Replace(Environment.NewLine, string.Empty)
                .Select(i => new Octopus(i - '0'))
                .ToArray();

            int size = (int)Math.Sqrt(levels.Length);

            return new Span2D<Octopus>(levels, size, size);
        }

        private void Reset(Span2D<Octopus> octopi)
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

                    if (!octopus.Flashed)
                    {
                        octopus.Energy += 1;

                        if (octopus.Energy > 9)
                        {
                            octopus.Flashed = true;
                            flashes++;

                            (int startRow, int endRow) = GetRange(row, octopi.Height);
                            (int startCol, int endCol) = GetRange(column, octopi.Width);

                            flashes += Step(octopi, new Point(startCol, startRow), new Point(endCol, endRow));
                        }
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
            public int Energy { get; set; }
            public bool Flashed { get; set; }

            public Octopus(int energy)
            {
                Energy = energy;
                Flashed = false;
            }

            public void Reset()
            {
                if (Flashed)
                {
                    Flashed = false;
                    Energy = 0;
                }
            }
        }

        private const string exampleData = @"5483143223
2745854711
5264556173
6141336146
6357385478
4167524645
2176841721
6882881134
4846848554
5283751526";

        private const string inputData = @"2682551651
3223134263
5848471412
7438334862
8731321573
6415233574
5564726843
6683456445
8582346112
4617588236";
    }
}
