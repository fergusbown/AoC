using System.Drawing;
using System.Text;

namespace AoC2021Runner;

internal class Day_2021_25 : IDayChallenge
{
    private readonly SeaCucumbers inputData;

    public Day_2021_25(string inputData)
    {
        this.inputData = ParseInput(inputData);
    }

    public string Part1()
    {
        SeaCucumbers state = inputData;
        int steps = 0;

        bool stillMoving;

        do
        {
            stillMoving = state.TryStep(out state);
            steps++;
        }
        while (stillMoving);

        return steps.ToString();
    }
    public string Part2()
        => "Happy Christmas!";

    private class SeaCucumbers
    {
        public int Width { get; }
        public int Height { get; }
        public HashSet<Point> EastFacing { get; }
        public HashSet<Point> SouthFacing { get; }
        public HashSet<Point> Empty { get; }

        public SeaCucumbers(int width, int height, HashSet<Point> eastFacing, HashSet<Point> southFacing, HashSet<Point> empty)
        {
            Width = width;
            Height = height;
            EastFacing = eastFacing;
            SouthFacing = southFacing;
            Empty = empty;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    var point = new Point(column, row);

                    if (SouthFacing.Contains(point) && !EastFacing.Contains(point) && !Empty.Contains(point))
                        sb.Append('v');
                    else if (EastFacing.Contains(point) && !SouthFacing.Contains(point) && !Empty.Contains(point))
                        sb.Append('>');
                    else if (Empty.Contains(point) && !EastFacing.Contains(point) && !SouthFacing.Contains(point))
                        sb.Append('.');
                    else
                        sb.Append('?');
                }
                sb.AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine();

            return sb.ToString();
        }

        public bool TryStep(out SeaCucumbers newState)
        {
            HashSet<Point> newEmpty = new();
            HashSet<Point> newEast = new();
            HashSet<Point> newSouth = new();

            foreach (var check in Empty)
            {
                Point west = new(check.X == 0 ? Width - 1 : check.X - 1, check.Y);

                if (EastFacing.Contains(west))
                {
                    newEmpty.Add(west);
                    newEast.Add(check);
                }
            }

            foreach (var check in Empty.Concat(newEmpty).Except(newEast).ToList())
            {
                Point north = new(check.X, check.Y == 0 ? Height - 1 : check.Y - 1);

                if (SouthFacing.Contains(north))
                {
                    newEmpty.Add(north);
                    newSouth.Add(check);
                    newEmpty.Remove(check);
                }
            }

            if (newEast.Count > 0 || newSouth.Count > 0)
            {
                var east = new HashSet<Point>(this.EastFacing);
                east.UnionWith(newEast);
                east.ExceptWith(newEmpty);
                east.ExceptWith(newSouth);

                var south = new HashSet<Point>(this.SouthFacing);
                south.UnionWith(newSouth);
                south.ExceptWith(newEmpty);
                south.ExceptWith(newEast);

                var empty = new HashSet<Point>(this.Empty);
                empty.UnionWith(newEmpty);
                empty.ExceptWith(newEast);
                empty.ExceptWith(newSouth);

                newState = new SeaCucumbers(
                    this.Width,
                    this.Height,
                    east,
                    south,
                    empty);

                return true;
            }
            else
            {
                newState = this;
                return false;
            }
        }
    }

    private static SeaCucumbers ParseInput(string input)
    {
        var rows = input.StringsForDay();
        int width = rows[0].Length;
        int height = rows.Length;
        HashSet<Point> eastFacing = new();
        HashSet<Point> southFacing = new();
        HashSet<Point> empty = new();

        for (var row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                Point point = new(column, row);
                switch (rows[row][column])
                {
                    case '>':
                        eastFacing.Add(point);
                        break;
                    case 'v':
                        southFacing.Add(point);
                        break;
                    default:
                        empty.Add(point);
                        break;
                }
            }
        }

        return new SeaCucumbers(width, height, eastFacing, southFacing, empty);
    }
}
