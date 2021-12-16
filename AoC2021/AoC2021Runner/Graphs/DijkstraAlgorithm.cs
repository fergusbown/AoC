using System.Diagnostics.CodeAnalysis;

namespace AoC2021Runner;

/// <summary>
/// Calculate the shortest path between two points of a weighted graph
/// </summary>
internal static class DijkstraAlgorithm
{
    public class DijkstraData<TNodeData>
    {
        public DijkstraData(TNodeData data)
        {
            Data = data;
        }

        public TNodeData Data { get; }

        public bool Visited { get; set; }

        public Graph<DijkstraData<TNodeData>>.Node? Parent { get; private set; }

        public long? Cost { get; private set; }

        public bool UpdateCost(long newCost, Graph<DijkstraData<TNodeData>>.Node? parent)
        {
            if (!Cost.HasValue || Cost > newCost)
            {
                Cost = newCost;
                Parent = parent;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            Parent = null;
            Visited = false;
            Cost = null;
        }
    }

    /// <summary>
    /// Find the shortest route through a graph whose data is specialised to support the algorithm
    /// </summary>
    public static (long? Cost, IReadOnlyCollection<Graph<DijkstraData<TNodeData>>.Edge>) FindShortestPath<TNodeData>(
        Graph<DijkstraData<TNodeData>> graph,
        Graph<DijkstraData<TNodeData>>.Node start, 
        Graph<DijkstraData<TNodeData>>.Node end)
    {
        foreach(var node in graph.Nodes)
        {
            node.Data.Reset();
        }

        PriorityQueue<Graph<DijkstraData<TNodeData>>.Node, long> nodeCosts = new();

        start.Data.UpdateCost(0, null);
        nodeCosts.Enqueue(start, 0);

        while(nodeCosts.TryDequeue(out var node, out _))
        {
            if (node.Data.Visited)
            {
                continue;
            }

            foreach (var edge in node.Edges.Where(e => !e.End.Data.Visited))
            {
                var endData = edge.End.Data;
                var costToEnd = node.Data.Cost!.Value + edge.Weight;

                if (endData.UpdateCost(costToEnd, node) && !endData.Visited)
                {
                    nodeCosts.Enqueue(edge.End, costToEnd);
                }
            }
        }

        return (end.Data.Cost, GetPath(end));

        static IReadOnlyCollection<Graph<DijkstraData<TNodeData>>.Edge> GetPath(Graph<DijkstraData<TNodeData>>.Node end)
        {
            if (end.Data.Cost is null)
            {
                return Array.Empty<Graph<DijkstraData<TNodeData>>.Edge>();
            }

            List<Graph<DijkstraData<TNodeData>>.Node> nodePath = new()
            {
                end,
            };


            Graph<DijkstraData<TNodeData>>.Node? current = end.Data.Parent;

            while (current is not null)
            {
                nodePath.Add(current);
                current = current.Data.Parent;
            }

            nodePath.Reverse();

            List<Graph<DijkstraData<TNodeData>>.Edge> result = new(nodePath.Count - 1);

            for (int i = 0; i < nodePath.Count - 1; i++)
            {
                var edgeStart = nodePath[i];
                var edgeEnd = nodePath[i + 1];

                result.Add(edgeStart.Edges.Where(e => e.End == edgeEnd).First());
            }

            return result;
        }
    }

    /// <summary>
    /// Find the shortest route through a graph whose data is not specialised for the algorithm (lower performance)
    /// </summary>
    public static (long? Cost, IReadOnlyCollection<Graph<TNodeData>.Edge>) FindShortestPath<TNodeData>(Graph<TNodeData>.Node start, Graph<TNodeData>.Node end)
    {
        Dictionary<Graph<TNodeData>.Node, long> unvisitedNodeCosts = new();
        Dictionary<Graph<TNodeData>.Node, Graph<TNodeData>.Node> nodeParents = new();
        Dictionary<Graph<TNodeData>.Node, long> visitedNodeCosts = new();

        unvisitedNodeCosts.Add(start, 0);

        while(TryGetCheapestNode(out var node, out var cost))
        {
            foreach (var edge in node.Edges.Where(e => !visitedNodeCosts.ContainsKey(e.End)))
            {
                long costToEnd = cost.Value + edge.Weight;
                UpdateNodeCost(edge.End, costToEnd, node);
            }

            unvisitedNodeCosts.Remove(node);
            visitedNodeCosts.Add(node, cost.Value);
        }

        return ((int)visitedNodeCosts[end], GetPath());

        bool TryGetCheapestNode([NotNullWhen(true)] out Graph<TNodeData>.Node? node, [NotNullWhen(true)] out long? cost)
        {
            if (unvisitedNodeCosts.Count == 0)
            {
                node = null;
                cost = null;
                return false;
            }

            KeyValuePair<Graph<TNodeData>.Node, long> cheapest = unvisitedNodeCosts.OrderBy(kvp => kvp.Value).FirstOrDefault();

            node = cheapest.Key;
            cost = cheapest.Value;
            return true;
        }

        void UpdateNodeCost(Graph<TNodeData>.Node node, long cost, Graph<TNodeData>.Node parent)
        {
            if (visitedNodeCosts.TryGetValue(node, out long currentCost) && currentCost > cost)
            {
                visitedNodeCosts[node] = cost;
                nodeParents[node] = parent;
            }
            else if (!unvisitedNodeCosts.TryGetValue(node, out currentCost) || currentCost > cost)
            {
                unvisitedNodeCosts[node] = cost;
                nodeParents[node] = parent;
            }
        }

        IReadOnlyCollection<Graph<TNodeData>.Edge> GetPath()
        {
            if (!visitedNodeCosts.ContainsKey(end))
            {
                return Array.Empty<Graph<TNodeData>.Edge>();
            }
            
            List<Graph<TNodeData>.Node> nodePath = new()
            {
                end,
            };


            Graph<TNodeData>.Node current = end;

            while(nodeParents.TryGetValue(current, out var parent))
            {
                nodePath.Add(parent);
                current = parent;
            }

            nodePath.Reverse();

            List<Graph<TNodeData>.Edge> result = new(nodePath.Count - 1);

            for (int i = 0; i < nodePath.Count -1; i++)
            {
                var start = nodePath[i];
                var end = nodePath[i + 1];

                result.Add(start.Edges.Where(e => e.End == end).First());
            }

            return result;
        }
    }
}
