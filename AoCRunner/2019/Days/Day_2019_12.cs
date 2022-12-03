using Generator.Equals;

namespace AoCRunner;

internal partial class Day_2019_12 : IDayChallenge
{
    private readonly string inputData;

    public Day_2019_12(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var system = new CelestialSystem(this.inputData);
        system.TimePasses(1000);

        return system.TotalEnergy().ToString();
    }

    public string Part2()
    {
        var system = new CelestialSystem(this.inputData);
        return system.CyclesToReturnToInitialState().ToString();
    }

    private record Point3D(int x, int y, int z)
    {
        public int Energy => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);

        public Point3D Add(Point3D velocity)
            => new Point3D(x + velocity.x, y + velocity.y, z + velocity.z);
    }

    [Equatable]
    private partial record SystemDimensionState([property: OrderedEquality] int[] State);

    [Equatable]
    private partial class Moon
    {
        public Moon(Point3D position)
        {
            Position = position;
            Velocity = new(0, 0, 0);
        }

        public Point3D Position { get; set; }

        public Point3D Velocity { get; set; }
    }

    private class CelestialSystem
    {
        private Moon[] moons;

        public CelestialSystem(string inputData)
        {
            this.moons = inputData.StringsForDay()
                .Select(s => s.Split(new string[] { "<x=", ", y=", ", z=", ">" }, StringSplitOptions.RemoveEmptyEntries))
                .Select(parts => new Moon(new Point3D(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]))))
                .ToArray();
        }

        public void TimePasses(int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                TimePasses();
            }
        }

        public long CyclesToReturnToInitialState()
        {
            HashSet<SystemDimensionState> xStates = new();
            HashSet<SystemDimensionState> yStates = new();
            HashSet<SystemDimensionState> zStates = new();

            while (AddStates(moons, xStates, yStates, zStates))
            {
                TimePasses();
            }

            return Factorisation.GetLowestCommonMultiple(xStates.Count, yStates.Count, zStates.Count);

            static bool AddStates(
                Moon[] moons,
                HashSet<SystemDimensionState> xStates,
                HashSet<SystemDimensionState> yStates,
                HashSet<SystemDimensionState> zStates)
            {
                bool addedAnythingNew = false;
                (SystemDimensionState x, SystemDimensionState y, SystemDimensionState z) = GetState(moons);

                addedAnythingNew |= xStates.Add(x);
                addedAnythingNew |= yStates.Add(y);
                addedAnythingNew |= zStates.Add(z);

                return addedAnythingNew;
            }

            static (SystemDimensionState, SystemDimensionState, SystemDimensionState) GetState(Moon[] moons)
            {
                int[] x = new int[moons.Length * 2];
                int[] y = new int[moons.Length * 2];
                int[] z = new int[moons.Length * 2];

                int p = 0;
                int v = 1;
                foreach (var moon in moons)
                {
                    x[p] = moon.Position.x;
                    x[v] = moon.Velocity.x;
                    y[p] = moon.Position.y;
                    y[v] = moon.Velocity.y;
                    z[p] = moon.Position.z;
                    z[v] = moon.Velocity.z;

                    p += 2;
                    v += 2;
                }

                return (new SystemDimensionState(x), new SystemDimensionState(y), new SystemDimensionState(z));
            }
        }

        public int TotalEnergy()
        {
            return moons
                .Select(m => m.Position.Energy * m.Velocity.Energy)
                .Sum();
        }

        private void TimePasses()
        {
            foreach (var moon in moons)
            {
                var newVelocity = new Point3D(
                    NewVelocity(moon.Position, moon.Velocity, p => p.x),
                    NewVelocity(moon.Position, moon.Velocity, p => p.y),
                    NewVelocity(moon.Position, moon.Velocity, p => p.z));
                moon.Velocity = newVelocity;
            }
            foreach (var moon in moons)
            {
                moon.Position = moon.Position.Add(moon.Velocity);
            }

            int NewVelocity(Point3D position, Point3D velocity, Func<Point3D, int> retriever)
            {
                var current = retriever(position);

                return moons.Select(k =>
                {
                    var other = retriever(k.Position);
                    if (other > current)
                    {
                        return 1;
                    }
                    else if (other < current)
                    {
                        return -1;
                    }

                    return 0;
                })
                .Sum() + retriever(velocity);
            }
        }
    }
}
