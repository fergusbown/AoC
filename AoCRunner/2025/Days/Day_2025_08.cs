using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using AoCRunner;

internal class Day_2025_08 : IDayChallenge
{
    private readonly ImmutableArray<Point3D> inputData;

    public Day_2025_08(string inputData)
    {
        this.inputData = Parse(inputData);
    }

    public string Part1()
    {
        (IReadOnlyCollection<Distance> distances, Dictionary<Point3D, int> circuits) = Initialise();

        foreach (var d in distances.Take(1000))
        {
            _ = TryProcessDistance(d, circuits, out _);
        }

        return circuits
            .Select(c => c.Value)
            .GroupBy(v => v)
            .Select(g => g.Count())
            .OrderByDescending(c => c)
            .Take(3)
            .Aggregate(1, (a, b) => a * b)
            .ToString();
    }

    public string Part2()
    {
        (IReadOnlyCollection<Distance> distances, Dictionary<Point3D, int> circuits) = Initialise();
        HashSet<int> remainingCircuits = [.. circuits.Values];

        foreach (var d in distances)
        {
            if (TryProcessDistance(d, circuits, out int? removedCircuit))
            {
                remainingCircuits.Remove(removedCircuit.Value);

                if (remainingCircuits.Count == 1)
                {
                    return $"{(long)d.A.X * d.B.X}";
                }
            }
        }

        return "Booooo";
    }

    private static ImmutableArray<Point3D> Parse(string inputData)
    {
        return inputData.StringsForDay()
            .Select(inputData =>
            {
                var parts = inputData.Split(',').Select(int.Parse).ToArray();
                return new Point3D(parts[0], parts[1], parts[2]);
            })
            .ToImmutableArray();
    }

    private (IReadOnlyCollection<Distance> Distances, Dictionary<Point3D, int> Circuits) Initialise()
    {
        List<Distance> distances = new(inputData.Length * inputData.Length / 2);

        for (int i = 0; i < inputData.Length; i++)
        {
            Point3D a = inputData[i];

            for (int j = i + 1; j < inputData.Length; j++)
            {
                Point3D b = inputData[j];
                distances.Add(new Distance(a, b, a.DistanceSquared(b)));
            }
        }

        distances.Sort((a, b) => a.DistanceSquared.CompareTo(b.DistanceSquared));

        int circuitId = 0;
        Dictionary<Point3D, int> circuits = inputData.ToDictionary(p => p, _ => circuitId++);

        return (distances, circuits);
    }

    private static bool TryProcessDistance(Distance d, Dictionary<Point3D, int> circuits, [NotNullWhen(true)] out int? removed)
    {
        (Point3D a, Point3D b, _) = d;

        (int ca, int cb) = (circuits[a], circuits[b]);
        if (ca == cb)
        {
            // already in the same circuit
            removed = null;
            return false;
        }

        // merge "b" circuit into "a" circuit
        foreach ((Point3D p, int c) in circuits)
        {
            if (c == cb)
            {
                circuits[p] = ca;
            }
        }

        removed = cb;
        return true;
    }

    private readonly record struct Point3D(int X, int Y, int Z)
    {
        public long DistanceSquared(Point3D other)
        {
            long x = X - other.X;
            long y = Y - other.Y;
            long z = Z - other.Z;

            return x * x + y * y + z * z;
        }
    }

    private readonly record struct Distance(Point3D A, Point3D B, long DistanceSquared);
}
