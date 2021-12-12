namespace AoC2021Runner
{
    internal class Day12 : IDayChallenge
    {
        public string Part1()
        {
            var caves = new Caves(inputData);
            return caves.GetPaths((c, p) => false).ToString();
        }

        public string Part2()
        {
            var caves = new Caves(inputData);
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
            private readonly Dictionary<string, Cave> caves = new Dictionary<string, Cave>();
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
                HashSet<string> seenPaths = new();
                HashSet<string> validPaths = new();
                Stack<Path> pendingPaths = new Stack<Path>();

                pendingPaths.Push(new Path(this.StartCave));

                while (pendingPaths.TryPop(out Path? currentPath))
                {
                    if (seenPaths.Add(currentPath.Name))
                    {
                        foreach (var path in currentPath.Travel(canRevisit))
                        {
                            if (path.Terminus == EndCave)
                            {
                                validPaths.Add(path.Name);
                            }
                            else
                            {
                                pendingPaths.Push(path);
                            }
                        }
                    }
                }

                return validPaths.Count;
            }
        }

        private class Path
        {
            public string Name { get; }

            public Cave Terminus => route.Last();

            public bool IncludesSmallCaveRevisit { get; }

            private IReadOnlyCollection<Cave> route;

            public Path(Cave cave)
            {
                this.Name = cave.Name;
                this.route = new Cave[] { cave };
                this.IncludesSmallCaveRevisit = false;
            }

            public Path(Path prior, Cave next, bool isSmallCaveRevisit)
            {
                this.Name = string.Join(',', prior.Name, next.Name);
                var newRoute = prior.route.ToList();
                newRoute.Add(next);
                this.route = newRoute;
                this.IncludesSmallCaveRevisit = prior.IncludesSmallCaveRevisit || isSmallCaveRevisit;
            }

            public IEnumerable<Path> Travel(Func<Cave, Path, bool> canRevisitSmallCave)
            {
                foreach (var linkedCave in route.Last().LinkedCaves)
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
            private HashSet<Cave> linkedCaves = new HashSet<Cave>();

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


        private const string exampleData = @"start-A
start-b
A-c
A-b
b-d
A-end
b-end";

        private const string inputData = @"start-YY
av-rz
rz-VH
fh-av
end-fh
sk-gp
ae-av
YY-gp
end-VH
CF-qz
qz-end
qz-VG
start-gp
VG-sk
rz-YY
VH-sk
rz-gp
VH-av
VH-fh
sk-rz
YY-sk
av-gp
rz-qz
VG-start
sk-fh
VG-av";
    }
}
