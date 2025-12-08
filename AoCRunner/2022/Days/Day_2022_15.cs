using System.Collections.Concurrent;
using System.Collections.Immutable;
using MoreLinq;

namespace AoCRunner;

internal class Day_2022_15 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_15(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        IReadOnlyList<Reading> readings = Parse(inputData);

        return readings
            .Select(r => r.DefinitelyEmptyForRow(2_000_000))
            .Aggregate(RangesSet.Empty, (seed, reading) => seed.Union(reading))
            .Count
            .ToString();
    }

    public string Part2()
    {
        const int maxIndex = 4_000_000;

        IReadOnlyList<Reading> readings = Parse(inputData);

        IReadOnlyList<Reading> readingsFlipped = readings
            .Select(r => new Reading(new Position(r.Sensor.Y, r.Sensor.X), new Position(r.NearestBeacon.Y, r.NearestBeacon.X)))
            .ToArray();

        ConcurrentDictionary<int, IImmutableSet<int>> possibilitiesForColumn = new();

        long result = 0;

        Parallel.For(0, maxIndex + 1, row =>
        {
            if (result > 0)
            {
                return;
            }

            IImmutableSet<int> possiblyOccupied = GetPossiblyOccupied(readings, row, 0, maxIndex);

            if (possiblyOccupied.Count == 0)
            {
                return;
            }

            foreach (var column in possiblyOccupied)
            {
                var possible = possibilitiesForColumn.GetOrAdd(column, (c) => GetPossiblyOccupied(readingsFlipped, c, row, maxIndex));

                if (possible.Contains(row))
                {
                    result = ((long)column * maxIndex) + row;
                    return;
                }
            }
        });

        return result.ToString();

        static IImmutableSet<int> GetPossiblyOccupied(IReadOnlyList<Reading> readings, int rowIndex, int min, int max)
        {
            IImmutableSet<int> seed = new RangesSet(min, max + 1);

            return readings
                .Select(r => r.PossiblyOccupiedForRow(rowIndex, min, max))
                .Aggregate(seed, (s, reading) => s.Intersect(reading));
        }
    }


    private static IReadOnlyList<Reading> Parse(string inputData)
    {
        return inputData
            .Replace("Sensor at x=", "")
            .Replace(": closest beacon is at x=", " ")
            .Replace(", y=", " ")
            .StringsForDay()
            .Select(s =>
            {
                var parts = s
                    .Split(' ');

                Position sensor = new(int.Parse(parts[0]), int.Parse(parts[1]));
                Position beacon = new(int.Parse(parts[2]), int.Parse(parts[3]));

                return new Reading(sensor, beacon);
            })
            .ToArray();
    }

    private record Position(int X, int Y)
    {
    }

    private class Reading
    {
        private readonly int manhattanDistance;

        public Reading(Position sensor, Position nearestBeacon)
        {
            Sensor = sensor;
            NearestBeacon = nearestBeacon;
            manhattanDistance = Math.Abs(Sensor.X - NearestBeacon.X) + Math.Abs(Sensor.Y - NearestBeacon.Y);
        }

        public Position Sensor { get; }

        public Position NearestBeacon { get; }

        public IImmutableSet<int> DefinitelyEmptyForRow(int rowIndex)
        {
            int distanceToRow = Math.Abs(Sensor.Y - rowIndex);

            if (distanceToRow > manhattanDistance)
            {
                return RangesSet.Empty;
            }

            int contributionFromRow = manhattanDistance - distanceToRow;

            int from = Sensor.X - contributionFromRow;
            int to = Sensor.X + contributionFromRow;

            if (rowIndex == NearestBeacon.Y)
            {
                if (from == NearestBeacon.X)
                {
                    from++;
                }

                if (to == NearestBeacon.X)
                {
                    to--;
                }
            }

            if (to < from)
            {
                return RangesSet.Empty;
            }

            return new RangesSet(new SimpleRange<int>(from, to + 1));
        }

        public IImmutableSet<int> PossiblyOccupiedForRow(int rowIndex, int minimum, int maximum)
        {
            int distanceToRow = Math.Abs(Sensor.Y - rowIndex);

            if (distanceToRow > manhattanDistance)
            {
                return new RangesSet(minimum, maximum + 1);
            }

            int contributionFromRow = manhattanDistance - distanceToRow;

            int from = Sensor.X - contributionFromRow;
            int to = Sensor.X + contributionFromRow;

            return new RangesSet(new SimpleRange<int>(minimum, from), new SimpleRange<int>(to + 1, maximum + 1));
        }
    }
}
