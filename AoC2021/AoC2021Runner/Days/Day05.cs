using System.Drawing;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner
{
    internal class Day05 : IDayChallenge
    {
        private readonly IReadOnlyCollection<(Point Start, Point End)> input;

        public Day05(string inputData)
        {
            this.input = GetInput(inputData);
        }

        public string Part1()
            => Solve(p => p.Start.X == p.End.X || p.Start.Y == p.End.Y);

        public string Part2()
            => Solve(p => true);

        public string Solve(Func<(Point Start, Point End), bool> filter)
        {
            var result = input
                .Where(p => filter(p))
                .SelectMany(x => PointsBetween(x.Start, x.End))
                .GroupBy(p => p)
                .Where(g => g.Count() > 1)
                .Count();

            return $"{result}";
        }

        private static IReadOnlyCollection<(Point Start, Point End)> GetInput(string data)
        {
            return data.StringsForDay()
                .Select(s =>
                {
                    string[] parts = s.Split(new string[] { ",", " -> " }, StringSplitOptions.None);
                    return (new Point(int.Parse(parts[0]), int.Parse(parts[1])), new Point(int.Parse(parts[2]), int.Parse(parts[3])));
                })
                .ToArray();
        }

        private static IEnumerable<Point> PointsBetween(Point start, Point end)
        {
            int xInc = GetIncrementAmount(start.X, end.X);
            int yInc = GetIncrementAmount(start.Y, end.Y);


            int x = start.X;
            int y = start.Y;

            while (x != end.X || y != end.Y)
            {
                yield return new Point(x, y);
                x += xInc;
                y += yInc;
            }

            yield return new Point(x, y);

            static int GetIncrementAmount(int start, int end)
            {
                if (start < end)
                {
                    return 1;
                }
                else if (start > end)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
