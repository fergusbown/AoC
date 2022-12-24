using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AoCRunner;

internal class Day_2022_19 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_19(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        return Solve(Parse(inputData), 24)
            .Select(kvp => kvp.Key * kvp.Value)
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        return Solve(Parse(inputData).Take(3), 32)
            .Values
            .Aggregate(1, (s, v) => s * v)
            .ToString();
    }

    private static ImmutableDictionary<int, int> Solve(IEnumerable<Blueprint> blueprints, int minutes)
    {
        ConcurrentDictionary<int, int> results = new();

        Parallel.ForEach(blueprints, blueprint =>
        {
            HashSet<Snapshot> snapshots = new()
            {
                new Snapshot(
                    blueprint,
                    new Inventry(1, 0, 0, 0),
                    new Inventry(0, 0, 0, 0),
                    0,
                    0)
            };

            HashSet<Snapshot> next = new();

            Snapshot best = snapshots.Single();
            for (int i = 1; i <= minutes; i++)
            {
                int valueMinutes = minutes - i;

                int optimisticFutureAdditionalValue = 0;

                if (valueMinutes > 1)
                {
                    optimisticFutureAdditionalValue = GeodeValue(valueMinutes - 1);
                }

                foreach (var snapshot in snapshots.SelectMany(s => s.Build(valueMinutes, optimisticFutureAdditionalValue)))
                {
                    if (snapshot.Value > best.Value)
                    {
                        best = snapshot;
                    }
                    
                    if (snapshot.OptimisticValue > best.Value)
                    {
                        next.Add(snapshot);
                    }
                }

                (snapshots, next) = (next, snapshots);
                next.Clear();

                if (snapshots.Count == 0)
                {
                    break;
                }
            }

            _ = results.TryAdd(blueprint.Id, best.Value);
        });

        return results.ToImmutableDictionary();

        static int GeodeValue(int minutes) => (minutes * (minutes + 1)) / 2;
    }

    private IReadOnlyCollection<Blueprint> Parse(string inputData)
    {
        var result = inputData
            .StringsForDay()
            .Select(s => Regex.Replace(s, "[^0-9 ]", ""))
            .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i)).ToArray())
            .Select(p =>
            {
                return new Blueprint(
                    p[0],
                    new RobotBlueprint(p[1], 0, 0),
                    new RobotBlueprint(p[2], 0, 0),
                    new RobotBlueprint(p[3], p[4], 0),
                    new RobotBlueprint(p[5], 0, p[6]));
            })
            .ToList();

        return result;
    }

    private readonly record struct RobotBlueprint(int OreCost, int ClayCost, int ObsidianCost)
    {
        public bool CanBuild(Inventry materials, out Inventry newMaterials)
        {
            if (materials.Ore >= OreCost && materials.Clay >= ClayCost && materials.Obsidian >= ObsidianCost)
            {
                newMaterials = materials with
                {
                    Ore = materials.Ore - OreCost,
                    Clay = materials.Clay - ClayCost,
                    Obsidian = materials.Obsidian - ObsidianCost,
                };

                return true;
            }

            newMaterials = default;
            return false;
        }
    }

    private readonly record struct Blueprint(int Id, RobotBlueprint OreRobot, RobotBlueprint ClayRobot, RobotBlueprint ObsidianRobot, RobotBlueprint GeodeRobot);

    private readonly record struct Inventry(int Ore, int Clay, int Obsidian, int Geodes)
    {
        public Inventry Add(Inventry other)
        {
            return new Inventry(
                Ore + other.Ore,
                Clay + other.Clay,
                Obsidian + other.Obsidian,
                Geodes + other.Geodes);
        }
    }

    private readonly record struct Snapshot(
        Blueprint Blueprint,
        Inventry Robots,
        Inventry Materials,
        int Value,
        int OptimisticValue)
    {

        public IEnumerable<Snapshot> Build(int purchasedGeodeValue, int optimisticFutureAdditionalValue)
        {
            if (Blueprint.GeodeRobot.CanBuild(Materials, out var newMaterials))
            {
                yield return this with
                {
                    Robots = Robots with { Geodes = Robots.Geodes + 1 },
                    Materials = newMaterials.Add(Robots),
                    Value = Value + purchasedGeodeValue,
                    OptimisticValue = Value + purchasedGeodeValue + optimisticFutureAdditionalValue,
                };
            }

            if (Blueprint.OreRobot.CanBuild(Materials, out newMaterials))
            {
                yield return this with
                {
                    Robots = Robots with { Ore = Robots.Ore + 1 },
                    Materials = newMaterials.Add(Robots),
                    OptimisticValue = Value + optimisticFutureAdditionalValue,
                };
            }

            if (Blueprint.ClayRobot.CanBuild(Materials, out newMaterials))
            {
                yield return this with
                {
                    Robots = Robots with { Clay = Robots.Clay + 1 },
                    Materials = newMaterials.Add(Robots),
                    OptimisticValue = Value + optimisticFutureAdditionalValue,
                };
            }

            if (Blueprint.ObsidianRobot.CanBuild(Materials, out newMaterials))
            {
                yield return this with
                {
                    Robots = Robots with { Obsidian = Robots.Obsidian + 1 },
                    Materials = newMaterials.Add(Robots),
                    OptimisticValue = Value + optimisticFutureAdditionalValue,
                };
            }

            yield return this with
            {
                Materials = Materials.Add(Robots),
                OptimisticValue = Value + optimisticFutureAdditionalValue,
            };
        }
    }
}
