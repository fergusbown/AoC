using System.Collections.Immutable;

namespace AoCRunner;

internal class Day_2022_18 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_18(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var occupied = Parse(inputData);

        return occupied
            .SelectMany(o => o.OuterFaces(occupied))
            .Count()
            .ToString();
    }

    public string Part2()
    {
        var occupied = Parse(inputData);

        Coord3D lowerLimit = new(-1, -1, -1);

        Coord3D upperLimit = new(
            occupied.Select(o => o.x).Max() + 1,
            occupied.Select(o => o.y).Max() + 1,
            occupied.Select(o => o.z).Max() + 1);

        HashSet<Coord3D> accessible = GetCuboidFace(lowerLimit, upperLimit, lowerLimit.x, null, null)
            .Concat(GetCuboidFace(lowerLimit, upperLimit, upperLimit.x, null, null))
            .Concat(GetCuboidFace(lowerLimit, upperLimit, null, lowerLimit.y, null))
            .Concat(GetCuboidFace(lowerLimit, upperLimit, null, upperLimit.y, null))
            .Concat(GetCuboidFace(lowerLimit, upperLimit, null, null, lowerLimit.z))
            .Concat(GetCuboidFace(lowerLimit, upperLimit, null, null, upperLimit.z))
            .ToHashSet();

        Stack<Coord3D> pending = new(accessible);

        while (pending.TryPop(out var test))
        {
            foreach (var face in test.OuterFaces(occupied))
            {
                if (accessible.Add(face) && face.InRange(lowerLimit, upperLimit))
                {
                    pending.Push(face);
                }
            }
        }

        return occupied
            .SelectMany(o => o.OuterFaces(occupied))
            .Where(u => accessible.Contains(u))
            .Count()
            .ToString();

        static IEnumerable<Coord3D> GetCuboidFace(Coord3D lowerLimit, Coord3D upperLimit, int? fixX, int? fixY, int? fixZ)
        {
            foreach (var x in GetCuboidFaceIndices(lowerLimit.x, upperLimit.x, fixX))
            {
                foreach (var y in GetCuboidFaceIndices(lowerLimit.y, upperLimit.y, fixY))
                {
                    foreach (var z in GetCuboidFaceIndices(lowerLimit.z, upperLimit.z, fixZ))
                    {
                        yield return new(x, y, z);
                    }
                }
            }
        }

        static IEnumerable<int> GetCuboidFaceIndices(int lower, int upper, int? fixedValue)
        {
            if (fixedValue.HasValue)
            {
                yield return fixedValue.Value;
            }
            else
            {
                for (int i = lower; i <= upper; i++)
                {
                    yield return i;
                }
            }
        }
    }

    private IImmutableSet<Coord3D> Parse(string inputData)
    {
        return inputData
            .StringsForDay()
            .Select(s =>
            {
                var parts = s.Split(',');
                return new Coord3D(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            })
            .ToImmutableHashSet();
    }

    private record Coord3D(int x, int y, int z)
    {

        public IEnumerable<Coord3D> OuterFaces(IImmutableSet<Coord3D> occupied)
            => Faces().Where(f => !occupied.Contains(f));

        public bool InRange(Coord3D lowerLimit, Coord3D upperLimit)
        {
            if (this.x < lowerLimit.x || this.x > upperLimit.x)
            {
                return false;
            }
            if (this.y < lowerLimit.y || this.y > upperLimit.y)
            {
                return false;
            }
            if (this.z < lowerLimit.z || this.z > upperLimit.z)
            {
                return false;
            }

            return true;
        }

        private IEnumerable<Coord3D> Faces()
        {
            yield return this with { x = this.x + 1 };
            yield return this with { x = this.x - 1 };

            yield return this with { y = this.y + 1 };
            yield return this with { y = this.y - 1 };

            yield return this with { z = this.z + 1 };
            yield return this with { z = this.z - 1 };
        }
    }
}
