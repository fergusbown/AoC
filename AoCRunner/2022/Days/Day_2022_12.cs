using HeightGraph = AoCRunner.DijkstraAlgorithm.DijkstraGraph<char>;

namespace AoCRunner;

internal class Day_2022_12 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_12(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (_, HeightGraph.Node start, HeightGraph.Node end) = Parse(inputData);

        (var cost, _ ) = DijkstraAlgorithm.FindShortestPath(end, start);

        return cost!.Value.ToString();
    }

    public string Part2()
    {
        (HeightGraph graph, _, HeightGraph.Node end) = Parse(inputData);

        DijkstraAlgorithm.FindShortestPathsFrom(graph, end, e => true);

        return graph.Nodes
            .Where(n => n.Data.NodeData == 'a' && n.Data.Cost.HasValue)
            .Select(n => n.Data.Cost!.Value)
            .Min()
            .ToString();
    }

    private static (HeightGraph Graph, HeightGraph.Node Start, HeightGraph.Node End) Parse(string inputData)
    {
        HeightGraph.Node start = null!;
        HeightGraph.Node end = null!;
        HeightGraph graph = new();

        Dictionary<(int row, int column), HeightGraph.Node> nodes = new();
        _ = inputData.GridForDay((value, row, column) =>
        {
            char nodeValue = value switch
            {
                'S' => 'a',
                'E' => 'z',
                _ => value
            };

            var node = graph.AddNode(nodeValue);

            nodes.Add((row, column), node);

            if (value == 'S')
            {
                start = node;
            }
            else if (value == 'E')
            {
                end = node;
            }

            return node;
        });

        foreach ((var location, var node) in nodes)
        {
            var above = location with { row = location.row - 1 };
            var below = location with { row = location.row + 1 };
            var left = location with { column = location.column - 1 };
            var right = location with { column = location.column + 1 };

            foreach(var adjacent in new[] { above, below, left, right })
            {
                if (nodes.TryGetValue(adjacent, out var adjacentNode))
                {
                    int heightDifference = adjacentNode.Data.NodeData - node.Data.NodeData;
                    if (heightDifference <= 1)
                    {
                        adjacentNode.AddEdgeTo(node, 1);
                    }
                }
            }
        }

        return (graph, start, end);
    }
}
