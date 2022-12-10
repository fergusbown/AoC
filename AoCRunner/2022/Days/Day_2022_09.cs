using MoreLinq.Extensions;

namespace AoCRunner;

internal class Day_2022_09 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_09(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
        => Solve(inputData, 2);

    public string Part2()
        => Solve(inputData, 10);

    private enum Direction
    {
        Up = 'U',
        Down = 'D',
        Left = 'L',
        Right = 'R',
    }

    private static string Solve(string inputData, int ropeLength)
    {
        var instructions = Parse(inputData);

        List<Point> rope = Enumerable.Repeat(new Point(0, 0), ropeLength).ToList();
        HashSet<Point> tailPositions = new() { rope.Last() };

        foreach ((var direction, var distance) in instructions)
        {
            Move(direction, distance, tailPositions, rope);
        }

        return tailPositions.Count().ToString();
    }

    private static void Move(
        Direction direction,
        int distance,
        HashSet<Point> tailPositions,
        List<Point> rope)
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
            rope[0] = rope[0].MoveBy(movement);
            Point priorKnot = rope[0];

            for (int j = 1; j < rope.Count; j++)
            {
                priorKnot = rope[j] = rope[j].DragTowards(priorKnot);
            }

            tailPositions.Add(rope.Last());
        }
    }

    private static IReadOnlyCollection<(Direction Direction, int Distance)> Parse(string inputData)
        => inputData.StringsForDay().Select(d => ((Direction)d[0], int.Parse(d[2..]))).ToArray();

    private record Point(int X, int Y)
    {
        public Point MoveBy(Point point)
            => new Point(this.X + point.X, this.Y + point.Y);

        public Point DragTowards(Point point)
        {
            int xDistance = this.X - point.X;
            int yDistance = this.Y - point.Y;

            if (Math.Abs(xDistance) <= 1 && Math.Abs(yDistance) <= 1)
            {
                return this;
            }

            if (xDistance < -1)
            {
                point = point with { X = point.X - 1 };
            }
            else if (xDistance > 1)
            {
                point = point with { X = point.X + 1 };
            }

            if (yDistance < -1)
            {
                point = point with { Y = point.Y - 1 };
            }
            else if (yDistance > 1)
            {
                point = point with { Y = point.Y + 1 };
            }

            return point;
        }
    }
}
