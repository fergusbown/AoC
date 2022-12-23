using System.Data;
using CommunityToolkit.HighPerformance;
using MoreLinq;

namespace AoCRunner;

internal class Day_2022_17 : IDayChallenge
{
    private const string shape0 = "####";
    private const string shape1 = @".#.
###
.#.";
    private const string shape2 = @"..#
..#
###";
    private const string shape3 = @"#
#
#
#";
    private const string shape4 = @"##
##";

    private readonly string inputData;

    public Day_2022_17(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
        => HeightGained(Drop(inputData, 2022)).ToString();

    public string Part2()
    {
        long totalRocks = 1_000_000_000_000;

        int[] heightDeltas = Drop(inputData, 5_000);
        (int cycleOffset, int cycleLength) = FindCycle(heightDeltas, 1_000);

        int offsetHeight = HeightGained(heightDeltas[0..cycleOffset]);
        long cycleHeight = HeightGained(heightDeltas.Slice(cycleOffset, cycleLength));

        long numberOfCycles = (totalRocks - cycleOffset) / cycleLength;
        int remainder = (int)((totalRocks - cycleOffset) % cycleLength);

        long finalHeight = offsetHeight + (numberOfCycles * cycleHeight) + HeightGained(heightDeltas.Slice(cycleOffset, remainder));

        return finalHeight.ToString();

        static (int cycleOffset, int cycleLength) FindCycle(int[] heightDeltas, int cycleLength)
        {
            List<int> cycleIndexes = new List<int>();
            Span<int> deltaSpan = new Span<int>(heightDeltas);
            var searchSequence = deltaSpan[^cycleLength..];

            for (int i = deltaSpan.Length - (2 * cycleLength); i >= 0; i--)
            {
                var testSequence = deltaSpan.Slice(i, cycleLength);

                if (searchSequence.SequenceEqual(testSequence))
                {
                    cycleIndexes.Add(i);
                }
            }

            if (cycleIndexes.Count < 2)
            {
                throw new InvalidOperationException("Cycle not found");
            }

            return (cycleIndexes[^1], cycleIndexes[^2] - cycleIndexes[^1]);
        }
    }

    private static int HeightGained(IEnumerable<int> heightDeltas)
        => heightDeltas.Where(i => i > 0).Sum();

    private static int[] Drop(string inputData, int rockCount)
    {
        string jetStream = inputData;
        int maxHeight = rockCount * 4;
        int width = 7;

        Span2D<bool> space = new(new bool[maxHeight * width], maxHeight, width);
        int[] heightDeltas = new int[rockCount];

        int currentTop = maxHeight;

        Span2D<bool> shape0 = GetShape(Day_2022_17.shape0);
        Span2D<bool> shape1 = GetShape(Day_2022_17.shape1);
        Span2D<bool> shape2 = GetShape(Day_2022_17.shape2);
        Span2D<bool> shape3 = GetShape(Day_2022_17.shape3);
        Span2D<bool> shape4 = GetShape(Day_2022_17.shape4);

        int shapeIndex = 0;
        int jetIndex = 0;
        for (int rockIndex = 0; rockIndex < rockCount; rockIndex++)
        {
            shapeIndex %= 5;

            Span2D<bool> rock = shapeIndex++ switch
            {
                0 => shape0,
                1 => shape1,
                2 => shape2,
                3 => shape3,
                4 => shape4,
                _ => throw new InvalidOperationException(),
            };

            int currentColumn = 2;
            int shapeTop = currentTop - 3 - rock.Height;

            while (true)
            {
                jetIndex %= jetStream.Length;

                int targetColumn = jetStream[jetIndex++] switch
                {
                    '<' => currentColumn - 1,
                    '>' => currentColumn + 1,
                    _ => throw new InvalidOperationException(),
                };

                if (CanMoveTo(rock, space, shapeTop, targetColumn))
                {
                    currentColumn = targetColumn;
                }

                if (CanMoveTo(rock, space, shapeTop + 1, currentColumn))
                {
                    shapeTop += 1;
                }
                else
                {
                    Place(rock, space, shapeTop, currentColumn);
                    break;
                }
            }

            int previousTop = currentTop;
            currentTop = Math.Min(shapeTop, currentTop);
            heightDeltas[rockIndex] = previousTop - currentTop;
        }

        return heightDeltas;

        static bool CanMoveTo(Span2D<bool> shape, Span2D<bool> space, int row, int column)
        {
            if (column < 0)
            {
                return false;
            }

            if (column + shape.Width > space.Width)
            {
                return false;
            }

            if (row + shape.Height > space.Height)
            {
                return false;
            }

            Span2D<bool> destination = space.Slice(row, column, shape.Height, shape.Width);

            for (int y = 0; y < destination.Height; y++)
            {
                for (int x = 0; x < destination.Width; x++)
                {
                    if (shape[y, x] && destination[y, x])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static void Place(Span2D<bool> shape, Span2D<bool> space, int row, int column)
        {
            Span2D<bool> destination = space.Slice(row, column, shape.Height, shape.Width);

            for (int y = 0; y < destination.Height; y++)
            {
                for (int x = 0; x < destination.Width; x++)
                {
                    if (shape[y, x])
                    {
                        destination[y, x] = true;
                    }
                }
            }
        }
    }

    static Span2D<bool> GetShape(string shape)
        => shape.GridForDay(c => c == '#');
}
