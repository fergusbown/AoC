using System.Text;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner;
internal class Day15 : IDayChallenge
{
    private readonly int[,] inputArray;

    public Day15(string inputData)
    {
        this.inputArray = GetInputData(inputData).ToArray();
    }

    public string Part1()
        => GetOutput(1);

    public string Part2()
        => GetOutput(5);

    private string GetOutput(int inflateBy)
    {
        Span2D<int> input = new(inputArray);
        var inputGraph = BuildInputGraph(input, inflateBy);
        (var cost, _) = DijkstraAlgorithm.FindShortestPath(inputGraph.Start, inputGraph.End);
        return cost.ToString();
    }

    private static Span2D<int> GetInputData(string input)
    {
        List<int> digits = new(input.Length);
        int width = input.IndexOf(Environment.NewLine);

        foreach (char c in input)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    digits.Add(c - '0');
                    break;
                default:
                    break;
            }
        }

        return new Span2D<int>(digits.ToArray(), digits.Count / width, width);
    }

    private static (Graph<int> Graph, Graph<int>.Node Start, Graph<int>.Node End) BuildInputGraph(Span2D<int> matrix, int inflateBy)
    {
        Dictionary<(int Column, int Row), Graph<int>.Node> nodes = new();
        Graph<int> result = new();
        int height = matrix.Height * inflateBy;
        int width = matrix.Height * inflateBy;

        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                nodes[(column, row)] = result.AddNode(AdjustedWeight(row, column, matrix));
            }
        }

        for (int column = 0; column < width; column++)
        {
            for (int row = 0; row < height; row++)
            {
                var sourceNode = nodes[(column, row)];
                AddAdjacencies(row, column, height, width, sourceNode, nodes);
            }
        }

        return (result, nodes[(0, 0)], nodes[(width - 1, height - 1)]);

        static int AdjustedWeight(int row, int column, Span2D<int> source)
        {
            int weight = source[row % source.Height, column % source.Width] + (row / source.Height) + (column / source.Width);
            int remainder = weight % 9;
            return remainder == 0 ? 9 : remainder;
        }

        static void AddAdjacencies(int row, int column, int height, int width, Graph<int>.Node sourceNode, Dictionary<(int Column, int Row), Graph<int>.Node> nodes)
        {
            if (row > 0)
                AddEdgeBetween(sourceNode, nodes[(column, row - 1)]);

            if (row < height - 1)
                AddEdgeBetween(sourceNode, nodes[(column, row + 1)]);

            if (column > 0)
                AddEdgeBetween(sourceNode, nodes[(column - 1, row)]);

            if (column < width - 1)
                AddEdgeBetween(sourceNode, nodes[(column + 1, row)]);

            static void AddEdgeBetween(Graph<int>.Node sourceNode, Graph<int>.Node targetNode)
                => sourceNode.AddEdgeTo(targetNode, targetNode.Data);
        }
    }
}
