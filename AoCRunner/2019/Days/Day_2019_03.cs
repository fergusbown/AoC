using Generator.Equals;

namespace AoCRunner;

internal partial class Day_2019_03 : IDayChallenge
{
    private readonly IEnumerable<Instruction> wire1;
    private readonly IEnumerable<Instruction> wire2;

    public Day_2019_03(string inputData)
    {
        (wire1, wire2) = GetWires(inputData);
    }

    public string Part1()
    {

        var intersections = GetPoints(wire1);
        intersections.IntersectWith(GetPoints(wire2));

        return intersections
            .Select(p => Math.Abs(p.X) + Math.Abs(p.Y))
            .Min()
            .ToString();
    }

    public string Part2()
    {
        var wire1Points = GetPoints(wire1);
        var wire2Points = GetPoints(wire2);

        int fewestSteps = int.MaxValue;

        foreach (var wire1Point in wire1Points)
        {
            if (wire2Points.TryGetValue(wire1Point, out Point? wire2Point))
            {
                fewestSteps = Math.Min(wire1Point.Steps + wire2Point.Steps, fewestSteps);
            }
        }

        return fewestSteps.ToString();
    }

    private enum Direction
    {
        Up,
        Down,
        Right,
        Left,
    }

    private static (IEnumerable<Instruction> Wire1, IEnumerable<Instruction> Wire2) GetWires(string inputData)
    {
        Instruction[][] wires = inputData.StringsForDay().Select(w => w.Split(',').Select(i => Instruction.FromString(i)).ToArray()).ToArray();
        return (wires[0], wires[1]);
    }

    private static HashSet<Point> GetPoints(IEnumerable<Instruction> wire)
    {
        int steps = 0;
        Point point = new(0, 0, steps++);
        HashSet<Point> result = new();

        foreach (var instruction in wire)
        {
            (int xDelta, int yDelta) = instruction.Direction switch
            {
                Direction.Up => (0, 1),
                Direction.Down => (0, -1),
                Direction.Right => (1, 0),
                _ => (-1, 0),
            };

            for (int i = 0; i < instruction.Distance; i++)
            {
                point = new Point(point.X + xDelta, point.Y + yDelta, steps++);
                result.Add(point);
            }
        }

        return result;
    }

    [Equatable]
    private partial record Point(int X, int Y, [property: IgnoreEquality] int Steps);

    private record Instruction(Direction Direction, int Distance)
    {
        public static Instruction FromString(string instruction)
        {
            var direction = instruction[0] switch
            {
                'U' => Direction.Up,
                'D' => Direction.Down,
                'R' => Direction.Right,
                _ => Direction.Left,
            };

            var distance = int.Parse(instruction[1..]);

            return new Instruction(direction, distance);
        }
    }
}