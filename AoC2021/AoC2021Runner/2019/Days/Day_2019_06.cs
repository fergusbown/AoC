using System.Diagnostics;

namespace AoC2021Runner;

internal partial class Day_2019_06 : IDayChallenge
{
    private readonly string inputData;

    public Day_2019_06(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var orbitalGraph = BuildObitalGraph(this.inputData, false);
        int orbits = 0;

        foreach(var celestialObject in orbitalGraph.Nodes)
        {
            var edge = celestialObject.Edges.SingleOrDefault();
            while (edge is not null)
            {
                orbits++;
                edge = edge.End.Edges.SingleOrDefault();
            }
        }

        return orbits.ToString();
    }

    public string Part2()
    {
        var orbitalGraph = BuildObitalGraph(this.inputData, true);
        var you = orbitalGraph.Nodes.Single(n => n.Data.NodeData == "YOU");
        var santa = orbitalGraph.Nodes.Single(n => n.Data.NodeData == "SAN");

        (var cost, _) = DijkstraAlgorithm.FindShortestPath(you, santa);

        Debug.Assert(cost is not null);

        return $"{cost - 2}"; // -2 since its the distance between the objects we're orbitting, not the objects themselves
    }

    private static DijkstraAlgorithm.DijkstraGraph<string> BuildObitalGraph(string inputData, bool bidirectional)
    {
        var orbits = inputData.Split(new string[] { Environment.NewLine, ")" }, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, DijkstraAlgorithm.DijkstraGraph<string>.Node> celestialObjects = new();
        var graph = new DijkstraAlgorithm.DijkstraGraph<string>();

        for (int i = 0; i < orbits.Length; i+=2)
        {
            var primary = GetNode(orbits[i]);
            var satellite = GetNode(orbits[i+1]);

            satellite.AddEdgeTo(primary, 1);

            if (bidirectional)
            {
                primary.AddEdgeTo(satellite, 1);
            }
        }

        return graph;

        DijkstraAlgorithm.DijkstraGraph<string>.Node GetNode(string name)
        {
            if (!celestialObjects.TryGetValue(name, out var node))
            {
                node = graph.AddNode(name);
                celestialObjects.Add(name, node);
            }

            return node;
        }
    }
}