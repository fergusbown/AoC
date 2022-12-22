using System.Collections.Immutable;
using System.Linq;

namespace AoCRunner;

internal class Day_2022_16 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_16(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (var costs, var flows, var start) = ParseInput(this.inputData);

        Journey startingJourney = new(1, start, 0, 0);
        Journey bestJourney = startingJourney;

        Stack<Journey> pending = new();
        pending.Push(startingJourney);

        while (pending.TryPop(out var currentJourney))
        {
            if (currentJourney.Value > bestJourney.Value)
            {
                bestJourney = currentJourney;
            }

            foreach(var journey in GetNewJourneys(costs, flows, currentJourney, 30))
            {
                pending.Push(journey);
            }
        }

        return bestJourney.Value.ToString();
    }

    public string Part2()
    {
        (var costs, var flows, var start) = ParseInput(this.inputData);

        Journey startingJourney = new(1, start, 0, 0);
        Dictionary<ulong, int> bestJourneys = new();

        Stack<(Journey, Journey)> pending = new();
        pending.Push((startingJourney, startingJourney));

        while (pending.TryPop(out var current))
        {
            (var me, var elephant) = current;

            if (bestJourneys.TryGetValue(me.Open, out int best))
            {
                if (best > me.Value + elephant.Value)
                {
                    continue;
                }
            }

            bestJourneys[me.Open] = me.Value + elephant.Value;

            foreach (var myNewJourney in GetNewJourneys(costs, flows, me, 26))
            {
                pending.Push((myNewJourney, elephant with { Open = myNewJourney.Open }));
            }

            foreach (var elephantsNewJourney in GetNewJourneys(costs, flows, elephant, 26))
            {
                pending.Push((me with { Open = elephantsNewJourney.Open}, elephantsNewJourney));
            }
        }

        return $"{bestJourneys.Values.Max()}";
    }

    private static IEnumerable<Journey> GetNewJourneys(
        IImmutableDictionary<ulong, IImmutableDictionary<ulong, int>> costs,
        IImmutableDictionary<ulong, int> flows,
        Journey moving,
        int timeLimit)
    {
        foreach ((var node, var cost) in costs[moving.Location])
        {
            if ((moving.Open & node) == 0)
            {
                var minutesConsumed = cost + 1;
                var minutesWillBeOpen = timeLimit + 1 - moving.Minute - minutesConsumed;
                var value = minutesWillBeOpen * flows[node];

                if (value > 0)
                {
                    Journey moved = new Journey(
                        moving.Minute + minutesConsumed,
                        node,
                        moving.Open | node,
                        moving.Value + value);

                    yield return moved;
                }
            }
        }
    }

    private static (IImmutableDictionary<ulong, IImmutableDictionary<ulong, int>> Costs, IImmutableDictionary<ulong, int> Flows, ulong Start) ParseInput(string input)
    {
        var lines = input
            .Replace("has flow rate=", "")
            .Replace("Valve ", "")
            .Replace("; tunnels lead to valves", "")
            .Replace("; tunnel leads to valve", "")
            .Replace(",", "")
            .StringsForDay()
            .Select(x => x.Split(' '))
            .ToArray();

        ulong startingValve = 0;
        ulong nextValveIdentifier = 1;
        Dictionary<DijkstraAlgorithm.DijkstraGraph<int>.Node, ulong> nodeIdentifiers = new();
        Dictionary<string, DijkstraAlgorithm.DijkstraGraph<int>.Node> nodes = new();
        DijkstraAlgorithm.DijkstraGraph<int> result = new();

        foreach (var line in lines)
        {
            string valveName = line[0];
            int valveFlow = int.Parse(line[1]);
            var node = result.AddNode(valveFlow);
            nodes.Add(valveName, node);
            nodeIdentifiers.Add(node, nextValveIdentifier);

            if (valveName == "AA")
            {
                startingValve = nextValveIdentifier;
            }

            nextValveIdentifier *= 2;
        }

        foreach (var line in lines)
        {
            var source = nodes[line[0]];

            foreach (var tunnel in line[2..])
            {
                var destination = nodes[tunnel];
                source.AddEdgeTo(destination, 1);
            }
        }

        Dictionary<ulong, IImmutableDictionary<ulong, int>> costs = new();
        Dictionary<ulong, int> flows = new();

        foreach (var node in result.Nodes)
        {
            DijkstraAlgorithm.FindShortestPathsFrom(result, node, (_) => true);

            var nodeCosts = result.Nodes
                .Where(n => n.Data.Cost.HasValue && n.Data.Cost.Value > 0 && n.Data.NodeData > 0)
                .ToImmutableDictionary(n => nodeIdentifiers[n], k => (int)k.Data.Cost!.Value);

            flows.Add(nodeIdentifiers[node], node.Data.NodeData);
            costs.Add(nodeIdentifiers[node], nodeCosts);
        }

        return (costs.ToImmutableDictionary(), flows.ToImmutableDictionary(), startingValve);
    }

    private record Journey(int Minute, ulong Location, ulong Open, int Value);
}
