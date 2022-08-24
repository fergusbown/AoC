using System.Collections.Immutable;
using System.Text;
using Generator.Equals;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner;

internal partial class Day_2019_18 : IDayChallenge
{
    private readonly string inputData;

    public Day_2019_18(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
        => FindAllKeys(this.inputData).ToString();

    public string Part2()
        => FindAllKeys(ReplaceEntrance(this.inputData)).ToString();

    private static long FindAllKeys(string vault)
    {
        Explorations? explorations = ParseInitialExploration(vault);

        PriorityQueue<Explorations, long> pending = new();
        long successfulSteps = long.MaxValue;

        Dictionary<Explorations, long> previous = new()
        {
            { explorations, 0 }
        };

        pending.Enqueue(explorations, 0);

        while (pending.TryDequeue(out explorations, out long steps))
        {
            if (steps > successfulSteps)
            {
                continue;
            }

            if (explorations.RemainingKeys.IsEmpty)
            {
                successfulSteps = steps;
            }

            if (previous[explorations] < steps)
            {
                continue;
            }

            foreach (Exploration exploration in explorations.Values)
            {
                ProcessExploration(explorations, steps, successfulSteps, exploration, pending, previous);
            }
        }

        return successfulSteps;

        static void ProcessExploration(
            Explorations explorations,
            long steps,
            long successfulSteps,
            Exploration exploration,
            PriorityQueue<Explorations, long> pending,
            Dictionary<Explorations, long> previous)
        {
            var otherExplorations = explorations.Values.Remove(exploration);

            foreach (var edge in exploration.Current.Edges.Except(explorations.EdgesSinceFoundKey, explorations.EdgesSinceFoundKey.KeyComparer))
            {
                long newSteps = steps + edge.Weight;

                if (newSteps >= successfulSteps)
                {
                    continue;
                }

                var cell = edge.End.Data;

                if (cell.IsDoor && explorations.RemainingKeys.Contains(cell.Value))
                {
                    continue;
                }

                Explorations newExplorations;

                if (edge.End.Data.IsKey && explorations.RemainingKeys.Contains(edge.End.Data.Value))
                {
                    if (exploration.RemainingKeys == 1)
                    {
                        newExplorations = new(
                            otherExplorations,
                            explorations.RemainingKeys.Remove(edge.End.Data.Value),
                            explorations.EdgesSinceFoundKey.Clear());
                    }
                    else
                    {
                        newExplorations = new(
                            otherExplorations.Add(new(edge.End, exploration.RemainingKeys - 1)),
                            explorations.RemainingKeys.Remove(edge.End.Data.Value),
                            explorations.EdgesSinceFoundKey.Clear());
                    }
                }
                else
                {
                    newExplorations = new(
                        otherExplorations.Add(new(edge.End, exploration.RemainingKeys)),
                        explorations.RemainingKeys,
                        explorations.EdgesSinceFoundKey.Add(edge));
                }

                if (previous.TryGetValue(newExplorations, out long previousSteps))
                {
                    if (previousSteps > newSteps)
                    {
                        previous[newExplorations] = newSteps;
                        pending.Enqueue(newExplorations, newSteps);
                    }
                }
                else
                {
                    previous.Add(newExplorations, newSteps);
                    pending.Enqueue(newExplorations, newSteps);
                }
            }
        }
    }

    private static Explorations ParseInitialExploration(string inputData)
    {
        Dictionary<(int Row, int Column), DijkstraAlgorithm.DijkstraGraph<VaultCell>.Node> vault = new();
        DijkstraAlgorithm.DijkstraGraph<VaultCell> vaultGraph = new();

        _ = inputData
            .GridForDay((c, row, column) =>
            {
                var result = new VaultCell(c, row, column);

                if (!result.IsWall)
                {
                    vault.Add((row, column), vaultGraph.AddNode(result));
                }

                return result;
            });

        foreach (((int Row, int Column), var node) in vault)
        {
            if (vault.TryGetValue((Row + 1, Column), out var linkedNode))
            {
                node.AddEdgeTo(linkedNode, 1);
            }
            if (vault.TryGetValue((Row - 1, Column), out linkedNode))
            {
                node.AddEdgeTo(linkedNode, 1);
            }
            if (vault.TryGetValue((Row, Column + 1), out linkedNode))
            {
                node.AddEdgeTo(linkedNode, 1);
            }
            if (vault.TryGetValue((Row, Column - 1), out linkedNode))
            {
                node.AddEdgeTo(linkedNode, 1);
            }
        }

        Graph<VaultCell> compressedVaultGraph = new();
        Dictionary<(int Row, int Column), Graph<VaultCell>.Node> compressedVault = vault
            .Where(kvp => !kvp.Value.Data.NodeData.IsEmpty)
            .ToDictionary(kvp => kvp.Key, kvp => compressedVaultGraph.AddNode(kvp.Value.Data.NodeData));

        foreach (var node in vault.Values.Where(v => !v.Data.NodeData.IsEmpty))
        {
            DijkstraAlgorithm.FindShortestPathsFrom(vaultGraph, node, e => e.Start.Data.NodeData.IsEmpty || e.Start == node);
            var sourceNode = compressedVault[(node.Data.NodeData.Row, node.Data.NodeData.Column)];

            foreach (var traversableNode in vault.Values.Where(n => n.Data.Cost is not null && !n.Data.NodeData.IsEmpty))
            {
                var targetNode = compressedVault[(traversableNode.Data.NodeData.Row, traversableNode.Data.NodeData.Column)];

                if (sourceNode != targetNode)
                {
                    sourceNode.AddEdgeTo(targetNode, traversableNode.Data.Cost!.Value);
                }
            }
        }

        var explorations = compressedVaultGraph
            .StronglyConnectedRegions()
            .Select(r => new Exploration(
                r.Single(n => n.Data.IsEntrance),
                r.Count(n => n.Data.IsKey)))
            .ToImmutableHashSet();

        return new(
            explorations,
            compressedVaultGraph.Nodes.Where(n => n.Data.IsKey).Select(n => n.Data.Value).ToImmutableHashSet(),
            ImmutableHashSet.Create<Graph<VaultCell>.Edge>(new Graph<VaultCell>.DirectionAgnosticEdgeComparer()));
    }

    private static string ReplaceEntrance(string inputData)
    {
        Span2D<char> vault = inputData.GridForDay(c => c);
        ReplaceEntrance(vault);

        StringBuilder sb = new();

        for (int rowIndex = 0; rowIndex < vault.Height; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < vault.Width; columnIndex++)
            {
                sb.Append(vault[rowIndex, columnIndex]);
            }

            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();

        static void ReplaceEntrance(Span2D<char> vault)
        {
            var replacementEntrance = "@#@\r\n###\r\n@#@".GridForDay(c => c);

            for (int rowIndex = 0; rowIndex < vault.Height; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < vault.Width; columnIndex++)
                {
                    if (vault[rowIndex, columnIndex] == '@')
                    {
                        replacementEntrance.CopyTo(vault.Slice(rowIndex - 1, columnIndex - 1, 3, 3));
                        return;
                    }
                }
            }
        }
    }

    [Equatable]
    private partial record Explorations(
        [property: UnorderedEquality] ImmutableHashSet<Exploration> Values,
        [property: UnorderedEquality] ImmutableHashSet<char> RemainingKeys,
        [property: IgnoreEquality] ImmutableHashSet<Graph<VaultCell>.Edge> EdgesSinceFoundKey);

    [Equatable]
    private partial record Exploration(
        [property: ReferenceEquality] Graph<VaultCell>.Node Current,
        int RemainingKeys);

    private class VaultCell
    {
        public VaultCell(char value, int row, int column)
        {
            IsEntrance = value == '@';
            IsWall = value == '#';
            IsKey = value >= 'a' && value <= 'z';
            IsDoor = value >= 'A' && value <= 'Z';
            IsEmpty = value == '.';

            Value = char.ToUpper(value);
            Row = row;
            Column = column;
        }

        public bool IsEntrance { get; }

        public bool IsWall { get; }

        public bool IsKey { get; }

        public bool IsDoor { get; }

        public bool IsEmpty { get; }

        public char Value { get; }

        public int Row { get; }

        public int Column { get; }
    }
}
