namespace AoC2021Runner;

internal class Day12 : IDayChallenge
{
    private readonly Caves caves;

    public Day12(string inputData)
    {
        this.caves = new Caves(inputData);
    }

    public string Part1()
    {
        return caves.GetPaths((c, p) => false).ToString();
    }

    public string Part2()
    {
        return caves.GetPaths(CanRevisitSmallCave).ToString();

        bool CanRevisitSmallCave(Cave c, Path p)
        {
            if (c == caves.StartCave || c == caves.EndCave)
            {
                return false;
            }
            else
            {
                return !p.IncludesSmallCaveRevisit;
            }
        }
    }


    private class Caves
    {
        private readonly Dictionary<string, Cave> caves = new();
        public Cave StartCave { get; }
        public Cave EndCave { get; }

        public Caves(string input)
        {
            var paths = input.StringsForDay()
                .Select(p =>
                {
                    var path = p.Split('-');
                    return (path[0], path[1]);
                })
                .ToArray();

            foreach ((string from, string to) in paths)
            {
                Cave fromCave = GetCave(from);
                Cave toCave = GetCave(to);

                fromCave.AddLinkTo(toCave);
                toCave.AddLinkTo(fromCave);
            }

            StartCave = caves["start"];
            EndCave = caves["end"];

            Cave GetCave(string name)
            {
                if (!caves.TryGetValue(name, out var cave))
                {
                    cave = new Cave(name);
                    caves.Add(name, cave);
                }

                return cave;
            }
        }

        public int GetPaths(Func<Cave, Path, bool> canRevisit)
        {
            int validPaths = 0;
            Stack<Path> pendingPaths = new();

            pendingPaths.Push(new Path(this.StartCave));

            while (pendingPaths.TryPop(out Path? currentPath))
            {
                foreach (var newPath in currentPath.Travel(canRevisit))
                {
                    if (newPath.Terminus == EndCave)
                    {
                        validPaths++;
                    }
                    else
                    {
                        pendingPaths.Push(newPath);
                    }
                }
            }

            return validPaths;
        }
    }

    private class Path
    {
        public Cave Terminus { get; }

        public bool IncludesSmallCaveRevisit { get; }

        private readonly IReadOnlyList<Cave> route;

        public Path(Cave cave)
        {
            this.route = new Cave[] { cave };
            this.IncludesSmallCaveRevisit = false;
            this.Terminus = cave;
        }

        public Path(Path prior, Cave next, bool isSmallCaveRevisit)
        {
            var newRoute = new List<Cave>(prior.route);
            newRoute.Add(next);
            this.route = newRoute;
            this.IncludesSmallCaveRevisit = prior.IncludesSmallCaveRevisit || isSmallCaveRevisit;
            this.Terminus = next;
        }

        public IEnumerable<Path> Travel(Func<Cave, Path, bool> canRevisitSmallCave)
        {
            foreach (var linkedCave in Terminus.LinkedCaves)
            {
                if (linkedCave.IsLargeCave || !route.Contains(linkedCave))
                {
                    yield return new Path(this, linkedCave, isSmallCaveRevisit: false);
                }
                else if (canRevisitSmallCave(linkedCave, this))
                {
                    yield return new Path(this, linkedCave, isSmallCaveRevisit: true);
                }
            }
        }
    }

    private class Cave
    {
        private readonly List<Cave> linkedCaves = new();

        public Cave(string name)
        {
            Name = name;
            IsLargeCave = name.ToUpper() == name;
        }

        public string Name { get; }

        public bool IsLargeCave { get; }

        public void AddLinkTo(Cave cave)
        {
            linkedCaves.Add(cave);
        }

        public IEnumerable<Cave> LinkedCaves => linkedCaves;
    }
}