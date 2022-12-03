using Generator.Equals;
using System.Diagnostics.CodeAnalysis;

namespace AoCRunner;

internal partial class Day_2021_23 : IDayChallenge
{
    private readonly string inputData;

    public Day_2021_23(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var startBurrow = ParseInput(inputData);
        var endBurrow = startBurrow.BuildEndBurrow();

        return Solve(startBurrow, endBurrow);
    }
    public string Part2()
    {
        var originalInput = inputData.StringsForDay();
        var newInput = string.Join(Environment.NewLine,
            originalInput[0],
            originalInput[1],
            originalInput[2],
            "  #D#C#B#A#",
            "  #D#B#A#C#",
            originalInput[3],
            originalInput[4]);

        var startBurrow = ParseInput(newInput);
        var endBurrow = startBurrow.BuildEndBurrow();
        return Solve(startBurrow, endBurrow);

    }

    private static string Solve(Burrow startBurrow, Burrow endBurrow)
    {
        Graph<DijkstraAlgorithm.IData<Burrow>> journeys = new();
        Dictionary<Burrow, Graph<DijkstraAlgorithm.IData<Burrow>>.Node> burrowToNodes = new();
        Graph<DijkstraAlgorithm.IData<AmphipodType>> burrowGraph = BuildEmptyBurrow(startBurrow.RoomSize, out var rooms, out var routes);

        var startNode = burrowToNodes[startBurrow] = journeys.AddNode(new DijkstraAlgorithm.Data<Burrow>(startBurrow));
        var endNode = burrowToNodes[endBurrow] = journeys.AddNode(new DijkstraAlgorithm.Data<Burrow>(endBurrow));

        Stack<Burrow> pendingBurrows = new();
        pendingBurrows.Push(startBurrow);

        while (pendingBurrows.TryPop(out var currentBurrow))
        {
            currentBurrow.UpdateGraph(burrowGraph);
            var burrowNode = burrowToNodes[currentBurrow];

            foreach ((var start, var end) in ValidJourneys(rooms))
            {
                var route = routes[(start, end)];
                if (route.Skip(1).Any(n => n.Data.NodeData != AmphipodType.None))
                {
                    continue;
                }

                AmphipodType moving = start.Data.NodeData;
                start.Data.NodeData = AmphipodType.None;
                end.Data.NodeData = moving;

                var newBurrow = new Burrow(burrowGraph);

                start.Data.NodeData = moving;
                end.Data.NodeData = AmphipodType.None;

                if (!burrowToNodes.TryGetValue(newBurrow, out var node))
                {
                    node = burrowToNodes[newBurrow] = journeys.AddNode(new DijkstraAlgorithm.Data<Burrow>(newBurrow));
                    pendingBurrows.Push(newBurrow);
                }

                burrowNode.AddEdgeTo(node, (int)moving * (route.Count - 1));
            }
        }

        (var finalCost, var finalJourney) = DijkstraAlgorithm.FindShortestPath(journeys, startNode, endNode, e => true);

        return finalCost!.Value.ToString();
    }

    private enum AmphipodType
    {
        None = 0,
        Amber = 1,
        Bronze = 10,
        Copper = 100,
        Desert = 1000,
    }

    private static Burrow ParseInput(string input)
    {
        List<AmphipodType> burrowData = new();

        foreach (var ch in input)
        {
            switch (ch)
            {
                case '.':
                    burrowData.Add(AmphipodType.None);
                    break;
                case 'A':
                    burrowData.Add(AmphipodType.Amber);
                    break;
                case 'B':
                    burrowData.Add(AmphipodType.Bronze);
                    break;
                case 'C':
                    burrowData.Add(AmphipodType.Copper);
                    break;
                case 'D':
                    burrowData.Add(AmphipodType.Desert);
                    break;
                default:
                    break;
            }
        }

        return new Burrow(burrowData.ToArray());
    }

    private static Graph<DijkstraAlgorithm.IData<AmphipodType>> BuildEmptyBurrow(
        int roomSize,
        out Dictionary<AmphipodType, IReadOnlyList<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>> rooms,
        out Dictionary<(Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node, Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node), IReadOnlyCollection<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>> routes)
    {
        /*

####################################
#|00|01|02|03|04|05|06|07|08|09|10|#
#######|11|##|12|##|13|##|14|#######
      #|15|##|16|##|17|##|18|#
      # etc   etc   etc   etc#
      ########################

         */

        var graph = new Graph<DijkstraAlgorithm.IData<AmphipodType>>();
        Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node[] allLocationNodes = new Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node[11 + 4 * (roomSize)];

        for (int i = 0; i < allLocationNodes.Length; i++)
        {
            allLocationNodes[i] = graph.AddNode(new DijkstraAlgorithm.Data<AmphipodType>(AmphipodType.None));
        }

        for (int i = 0; i < 10; i++)
        {
            allLocationNodes[i].AddEdgesBetween(allLocationNodes[i + 1], 1);
        }

        allLocationNodes[2].AddEdgesBetween(allLocationNodes[11], 1);
        allLocationNodes[4].AddEdgesBetween(allLocationNodes[12], 1);
        allLocationNodes[6].AddEdgesBetween(allLocationNodes[13], 1);
        allLocationNodes[8].AddEdgesBetween(allLocationNodes[14], 1);

        for (int i = 11; i < allLocationNodes.Length - 4; i++)
        {
            allLocationNodes[i].AddEdgesBetween(allLocationNodes[i + 4], 1);
        }

        var amberRoom = new List<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>();
        var bronzeRoom = new List<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>();
        var copperRoom = new List<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>();
        var desertRoom = new List<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>();

        for (int i = 11; i < allLocationNodes.Length;)
        {
            amberRoom.Add(allLocationNodes[i++]);
            bronzeRoom.Add(allLocationNodes[i++]);
            copperRoom.Add(allLocationNodes[i++]);
            desertRoom.Add(allLocationNodes[i++]);
        }

        // only consider those nodes in the hall tht we're allowed to stop on
        var hallway = new List<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>()
        {
            graph.Nodes[0],
            graph.Nodes[1],
            graph.Nodes[3],
            graph.Nodes[5],
            graph.Nodes[7],
            graph.Nodes[9],
            graph.Nodes[10],
        };

        rooms = new()
        {
            { AmphipodType.Amber, amberRoom },
            { AmphipodType.Bronze, bronzeRoom },
            { AmphipodType.Copper, copperRoom },
            { AmphipodType.Desert, desertRoom },
            { AmphipodType.None, hallway }
        };

        routes = new();

        foreach (var hallspace in hallway)
        {
            DijkstraAlgorithm.FindShortestPathsFrom(graph, hallspace, e => true);

            foreach (var roomspace in amberRoom.Concat(bronzeRoom).Concat(copperRoom).Concat(desertRoom))
            {
                var route = DijkstraAlgorithm.ReportShortestPathTo(roomspace).Route.Select(r => r.End).ToList();
                route.Insert(0, hallspace);
                routes.Add((hallspace, roomspace), route.ToArray());
                route.Reverse();
                routes.Add((roomspace, hallspace), route);
            }
        }

        return graph;
    }

    private static IEnumerable<(Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node Start, Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node End)> ValidJourneys(
        Dictionary<AmphipodType, IReadOnlyList<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node>> rooms)
    {
        foreach (var node in rooms[AmphipodType.None])  //i.e the hall
        {
            switch (node.Data.NodeData)
            {
                case AmphipodType.Amber:
                case AmphipodType.Bronze:
                case AmphipodType.Copper:
                case AmphipodType.Desert:
                    {
                        if (TryGetRoomTarget(node.Data.NodeData, rooms[node.Data.NodeData], out var destination))
                        {
                            yield return (node, destination);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        var hallwayDestinations = rooms[AmphipodType.None]
            .Where(d => d.Data.NodeData == AmphipodType.None).ToArray();

        foreach ((var correctOccupancy, var room) in rooms.Where(r => r.Key != AmphipodType.None))
        {
            if (TryGetRoomDeparture(correctOccupancy, room, out var departure))
            {
                foreach (var destination in hallwayDestinations)
                {
                    yield return (departure, destination);
                }
            }
        }

        static bool TryGetRoomTarget(
            AmphipodType targetType,
            IReadOnlyList<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node> room,
            [NotNullWhen(true)] out Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node? target)
        {
            if (room.Any(r => r.Data.NodeData != AmphipodType.None && r.Data.NodeData != targetType))
            {
                target = null;
                return false;
            }

            target = room.Last(r => r.Data.NodeData == AmphipodType.None);
            return true;
        }

        static bool TryGetRoomDeparture(
            AmphipodType correctOccupancy,
            IReadOnlyList<Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node> room,
            [NotNullWhen(true)] out Graph<DijkstraAlgorithm.IData<AmphipodType>>.Node? departure)
        {
            for (int i = 0; i < room.Count; i++)
            {
                AmphipodType currentOccupant = room[i].Data.NodeData;

                if (currentOccupant == AmphipodType.None)
                {
                    continue;
                }

                if (currentOccupant == correctOccupancy && room.Skip(i + 1).All(r => r.Data.NodeData == correctOccupancy))
                {
                    break;
                }

                departure = room[i];
                return true;
            }

            departure = null;
            return false;
        }
    }

    [Equatable]
    private partial class Burrow : IEquatable<Burrow>
    {
        [OrderedEquality]
        private AmphipodType[] occupations { get; }

        public void UpdateGraph(Graph<DijkstraAlgorithm.IData<AmphipodType>> graph)
        {
            for (int i = 0; i < occupations.Length; i++)
            {
                graph.Nodes[i].Data.NodeData = occupations[i];
            }
        }

        public int RoomSize => (occupations.Length - 11) / 4;

        public Burrow BuildEndBurrow()
        {
            List<AmphipodType> result = new(this.occupations.Length);

            result.AddRange(Enumerable.Repeat(AmphipodType.None, 11));

            for (int i = 11; i < this.occupations.Length; i += 4)
            {
                result.AddRange(new[] { AmphipodType.Amber, AmphipodType.Bronze, AmphipodType.Copper, AmphipodType.Desert });
            }

            return new Burrow(result.ToArray());
        }

        public Burrow(AmphipodType[] occupations)
        {
            this.occupations = occupations;
        }

        public Burrow(Graph<DijkstraAlgorithm.IData<AmphipodType>> graph)
        {
            this.occupations = graph.Nodes.Select(n => n.Data.NodeData).ToArray();
        }
    }
}
