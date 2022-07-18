namespace AoC2021Runner;

internal partial class Day_2019_15 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_15(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        var explorer = new Explorer(inputData);
        var map = await explorer.Run();
        var start = map.Nodes.Single(n => n.Data.NodeData.x == 0 && n.Data.NodeData.y == 0);
        var end = map.Nodes.Single(n => n.Data.NodeData.LocationType == LocationType.Oxygen);

        DijkstraAlgorithm.FindShortestPathsFrom(map, start, _ => true);

        return end.Data.Cost!.Value.ToString();
    }

    public async Task<string> Part2()
    {
        var explorer = new Explorer(inputData);
        var map = await explorer.Run();
        var start = map.Nodes.Single(n => n.Data.NodeData.LocationType == LocationType.Oxygen);

        DijkstraAlgorithm.FindShortestPathsFrom(map, start, _ => true);

        return map.Nodes.Select(n => n.Data.Cost!.Value).Max().ToString();
    }

    private enum LocationType
    {
        Unknown,
        Wall,
        Empty,
        Oxygen,
    }

    private record Exploration((int x, int y) From, (int x, int y) To, long Explore, long Backtrack);

    private class Explorer
    {
        private readonly long[] program;

        public Explorer(long[] program)
        {
            this.program = program;
        }
        public long Score { get; private set; }

        public async Task<DijkstraAlgorithm.DijkstraGraph<(int x, int y, LocationType LocationType)>> Run()
        {
            (int x, int y) currentLocation = (0, 0);
            (int x, int y) destination = (0, 0);

            Dictionary<(int x, int y), LocationType> map = new()
            {
                { (0, 0), LocationType.Empty },
            };


            Stack<Exploration> pending = new();
            Stack<Exploration> backtracks = new();

            foreach (var adj in Adjacent((0, 0)))
            {
                pending.Push(adj);
            }

            var computer = IntCodeComputer.New();

            computer
                .PipeInputFrom(Explore)
                .PipeOutputTo(HandleOutput);

            try
            {
                await computer.Run(this.program);
            }
            catch (OperationCanceledException)
            {
            }

            return ToGraph(map);

            Task<long> Explore()
            {
                if (pending.TryPeek(out var pendingItem))
                {
                    if (pendingItem.From == currentLocation)
                    {
                        pendingItem = pending.Pop();
                        destination = pendingItem.To;
                        backtracks.Push(pendingItem);
                        return Task.FromResult(pendingItem.Explore);
                    }
                    else
                    {
                        pendingItem = backtracks.Pop();
                        destination = pendingItem.From;
                        return Task.FromResult(pendingItem.Backtrack);
                    }
                }

                return Task.FromException<long>(new OperationCanceledException());
            }

            void HandleOutput(long output)
            {
                switch (output)
                {
                    case 0:
                        map[destination] = LocationType.Wall;
                        _ = backtracks.Pop();
                        return;
                    case 1:
                        map[destination] = LocationType.Empty;
                        break;
                    default:
                        map[destination] = LocationType.Oxygen;
                        break;
                }

                currentLocation = destination;

                foreach (var adjacent in Adjacent(currentLocation))
                {
                    if (map.TryAdd(adjacent.To, LocationType.Unknown))
                    {
                        pending.Push(adjacent);
                    }
                }
            }

            static IEnumerable<Exploration> Adjacent((int x, int y) location)
            {
                yield return new Exploration(location, (location.x, location.y - 1), 1, 2);
                yield return new Exploration(location, (location.x, location.y + 1), 2, 1);
                yield return new Exploration(location, (location.x - 1, location.y), 3, 4);
                yield return new Exploration(location, (location.x + 1, location.y), 4, 3);
            }

            static DijkstraAlgorithm.DijkstraGraph<(int x, int y, LocationType LocationType)> ToGraph(Dictionary<(int x, int y), LocationType> map)
            {
                var graph = new DijkstraAlgorithm.DijkstraGraph<(int x, int y, LocationType LocationType)>();

                Dictionary<(int x, int y), Graph<DijkstraAlgorithm.IData<(int x, int y, LocationType LocationType)>>.Node> nodes = map
                    .Where(kvp => kvp.Value != LocationType.Wall)
                    .Select(kvp => graph.AddNode((kvp.Key.x, kvp.Key.y, kvp.Value)))
                    .ToDictionary(k => (k.Data.NodeData.x, k.Data.NodeData.y), v => v);

                foreach ((var location, var node) in nodes)
                {
                    foreach (var adj in Adjacent(location))
                    {
                        if (nodes.TryGetValue(adj.To, out var adjacentNode))
                        {
                            node.AddEdgeTo(adjacentNode, 1);
                        }
                    }
                }

                return graph;
            }
        }
    }
}
