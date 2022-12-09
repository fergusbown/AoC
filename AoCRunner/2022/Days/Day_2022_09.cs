using MoreLinq.Extensions;
using System.Drawing;

namespace AoCRunner;

internal class Day_2022_09 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_09(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        return "";
    }

    public string Part2()
    {
        return "";
    }

    private enum Direction
    {
        Up = 'U',
        Down = 'D',
        Left = 'L',
        Right = 'R',
    }

    private (Point Head, Point Tail) Move(Point head, Point tail, Direction direction, int distance)
    {
        Point movement = direction switch
        {
            Direction.Up => new(0, 1),
            Direction.Down => new(0, -1),
            Direction.Left => new(-1, 0),
            Direction.Right => new(1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction)),
        };

        for (int i = 0; i < distance; i++)
        {
            Point newHead = head.Add(movement);


        }
    }

    private static IReadOnlyCollection<(Direction Direction, int Distance)> Parse(string inputData)
        => inputData.StringsForDay().Select(d => ((Direction)d[0], int.Parse(d[2..]))).ToArray();

    private record Point(int X, int Y)
    {
        public Point Add(Point point)
            => new Point(this.X + point.X, this.Y + point.Y);
    }
}
