using System.Collections.Immutable;
using CommunityToolkit.HighPerformance;

using Graph = AoCRunner.DijkstraAlgorithm.DijkstraGraph<AoCRunner.Day_2019_20.Portal?>;

namespace AoCRunner;

internal class Day_2019_20 : IDayChallenge
{
    private readonly string inputData;

    public Day_2019_20(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (var graph, var entrance, var exit, _, _) = BuildGraph(this.inputData, linkPortals: true);

        DijkstraAlgorithm.FindShortestPathsFrom(graph, entrance, _ => true);

        return exit.Data.Cost!.Value.ToString();
    }

    public string Part2()
    {
        (var graph, var entrance, var exit, var recurse, var @return) = BuildGraph(
            this.inputData,
            linkPortals: false);

        PriorityQueue<Route, int> routes = new();
        routes.Enqueue(new Route(entrance, ImmutableHashSet<Recursion>.Empty, 0), 0);
        long cheapestRoute = long.MaxValue;

        Dictionary<Graph.Node, Dictionary<Graph.Node, long>> pathsFrom = new();

        while (routes.TryDequeue(out Route? route, out int level))
        {
            if (route.Cost > cheapestRoute)
            {
                continue;
            }

            // make sure we only bother calculating distances once since its always the same graph being traversed
            // only care about poratls since everything else is just the journey
            if (!pathsFrom.TryGetValue(route.Location, out var shortestPaths))
            {
                shortestPaths = new Dictionary<Graph.Node, long>();
                pathsFrom.Add(route.Location, shortestPaths);

                DijkstraAlgorithm.FindShortestPathsFrom(graph, route.Location, _ => true);

                foreach (var node in graph.Nodes.Where(n => n.Data.Cost.HasValue && n.Data.Cost.Value > 0 && n.Data.NodeData is not null))
                {
                    shortestPaths.Add(node, node.Data.Cost!.Value);
                }
            }

            if (level == 0 && shortestPaths.TryGetValue(exit, out long cost))
            {
                long newCost = route.Cost + cost;

                if (newCost < cheapestRoute)
                {
                    cheapestRoute = newCost;
                }
            }

            // we can always go deeper...
            foreach ((var shallowerExit, var deeperEntrance) in recurse)
            {
                if (shortestPaths.TryGetValue(shallowerExit, out cost))
                {
                    Recursion recursion = new(level + 1, deeperEntrance);

                    if (!route.Recursions.Contains(recursion))
                    {
                        routes.Enqueue(new Route(
                            recursion.Location,
                            route.Recursions.Add(recursion),
                            route.Cost + cost + 1),
                            recursion.Level);
                    }
                }
            }

            // we can sometimes go up
            if (level != 0)
            {
                foreach ((var deeperExit, var shallowerEntrance) in @return)
                {
                    if (shortestPaths.TryGetValue(deeperExit, out cost))
                    {
                        routes.Enqueue(new Route(
                            shallowerEntrance,
                            route.Recursions,
                            route.Cost + cost + 1),
                            level - 1);
                    }
                }
            }
        }

        return cheapestRoute == long.MaxValue ? "NOPE" : cheapestRoute.ToString();
    }

    private static Built BuildGraph(
        string inputData,
        bool linkPortals)
    {
        Span2D<char> grid = inputData.GridForDay(c => c);

        Graph dijkstraGraph = new();

        Dictionary<(int Row, int Column), Graph.Node> nodes = new();

        for (int row = 0; row < grid.Height; row++)
        {
            for (int column = 0; column < grid.Width; column++)
            {
                if (IsTraversable(row, column, grid, out Portal? portal))
                {
                    nodes.Add((row, column), dijkstraGraph.AddNode(portal));
                }
            }
        }

        // Add regular edges
        foreach (((int row, int column), Graph.Node node) in nodes)
        {
            AddEdge(row - 1, column, nodes, node);
            AddEdge(row + 1, column, nodes, node);
            AddEdge(row, column - 1, nodes, node);
            AddEdge(row, column + 1, nodes, node);
        }

        // add extra edges between portals
        var portals = nodes
            .Values
            .Where(v => v.Data.NodeData is not null)
            .GroupBy(v => v.Data.NodeData!.Name);

        var goDeeper = new Dictionary<Graph.Node, Graph.Node>();
        var goUp = new Dictionary<Graph.Node, Graph.Node>();

        foreach (var portal in portals)
        {
            var linked = portal.ToArray();

            if (linked.Length == 2)
            {
                var inner = linked.Single(l => l.Data.NodeData!.IsInner);
                var outer = linked.Single(l => !l.Data.NodeData!.IsInner);

                goDeeper.Add(inner, outer);
                goUp.Add(outer, inner);

                if (linkPortals)
                {
                    linked[0].AddEdgesBetween(linked[1], 1);
                }
            }
        }

        return new Built(
            dijkstraGraph,
            dijkstraGraph.Nodes.Single(n => n.Data.NodeData?.Name == "AA"),
            dijkstraGraph.Nodes.Single(n => n.Data.NodeData?.Name == "ZZ"),
            goDeeper,
            goUp);

        static void AddEdge(
            int row,
            int column,
            Dictionary<(int Row, int Column), Graph.Node> nodes,
            Graph.Node to)
        {
            if (nodes.TryGetValue((row, column), out var from))
            {
                from.AddEdgeTo(to, 1);
            }
        }

        static bool IsTraversable(int row, int column, Span2D<char> grid, out Portal? portal)
        {
            char cell = grid[row, column];

            if (cell != '.')
            {
                portal = null;
                return false;
            }

            // portal above
            if (row >= 2 && IsCapitalLetter(grid[row - 1, column]))
            {
                portal = new Portal(string.Empty + grid[row - 2, column] + grid[row - 1, column], row != 2);
                return true;
            }

            // portal below
            if (row < grid.Height - 2 && IsCapitalLetter(grid[row + 1, column]))
            {
                portal = new Portal(string.Empty + grid[row + 1, column] + grid[row + 2, column], row != grid.Height - 3);
                return true;
            }

            //portal to left
            if (column >= 2 && IsCapitalLetter(grid[row, column - 1]))
            {
                portal = new Portal(string.Empty + grid[row, column - 2] + grid[row, column - 1], column != 2);
                return true;
            }

            //portal to right
            if (column < grid.Width - 2 && IsCapitalLetter(grid[row, column + 1]))
            {
                portal = new Portal(string.Empty + grid[row, column + 1] + grid[row, column + 2], column != grid.Width - 3);
                return true;
            }

            portal = null;
            return true;
        }

        static bool IsCapitalLetter(char value)
            => value >= 'A' && value <= 'Z';
    }

    private record Built(
        Graph Graph,
        Graph.Node Entrance,
        Graph.Node Exit,
        IReadOnlyDictionary<Graph.Node, Graph.Node> Recurse,
        IReadOnlyDictionary<Graph.Node, Graph.Node> Return);

    public record Portal(string Name, bool IsInner);

    private record Route(
        Graph.Node Location,
        ImmutableHashSet<Recursion> Recursions,
        long Cost);

    private record Recursion(int Level, Graph.Node Location);
}
