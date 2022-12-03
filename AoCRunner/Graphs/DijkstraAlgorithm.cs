using System.Diagnostics.CodeAnalysis;

namespace AoCRunner;

/// <summary>
/// Calculate the shortest path between two points of a weighted graph
/// </summary>
internal static class DijkstraAlgorithm
{
    public class DijkstraGraph<TNodeData> : Graph<IData<TNodeData>>
    {
        public Node AddNode(TNodeData data)
            => AddNode(new Data<TNodeData>(data));
    }

    public interface IData<TNodeData>
    {
        TNodeData NodeData { get; set; }

        bool Visited { get; set; }

        Graph<IData<TNodeData>>.Node? Parent { get; set; }

        long? Cost { get; set; }
    }

    public class Data<TNodeData> : IData<TNodeData>
    {
        public Data(TNodeData data)
        {
            NodeData = data;
        }

        public TNodeData NodeData { get; set; }

        public bool Visited { get; set; }

        public Graph<IData<TNodeData>>.Node? Parent { get; set; }

        public long? Cost { get; set; }
    }

    public static (long? Cost, IReadOnlyCollection<Graph<IData<TNodeData>>.Edge> Route) FindShortestPath<TNodeData>(
        Graph<IData<TNodeData>> graph,
        Graph<IData<TNodeData>>.Node start,
        Graph<IData<TNodeData>>.Node end,
        Func<Graph<IData<TNodeData>>.Edge, bool> canTraverse)
    {
        FindShortestPathsFrom(graph, start, canTraverse);
        return ReportShortestPathTo(end);
    }

    /// <summary>
    /// Find the shortest route through a graph whose data is specialised to support the algorithm
    /// </summary>
    public static void FindShortestPathsFrom<TNodeData>(
        Graph<IData<TNodeData>> graph,
        Graph<IData<TNodeData>>.Node start,
        Func<Graph<IData<TNodeData>>.Edge, bool> canTraverse)
    {
        foreach (var node in graph.Nodes)
        {
            Reset(node);
        }

        PriorityQueue<Graph<IData<TNodeData>>.Node, long> nodeCosts = new();

        UpdateCost(0, start, null);
        nodeCosts.Enqueue(start, 0);

        while (nodeCosts.TryDequeue(out var node, out _))
        {
            if (node.Data.Visited)
            {
                continue;
            }

            foreach (var edge in node.Edges.Where(e => !e.End.Data.Visited && canTraverse(e)))
            {
                var travelTo = edge.End;
                var costToEnd = node.Data.Cost!.Value + edge.Weight;

                if (UpdateCost(costToEnd, travelTo, node) && !travelTo.Data.Visited)
                {
                    nodeCosts.Enqueue(edge.End, costToEnd);
                }
            }
        }

        static bool UpdateCost(long newCost, Graph<IData<TNodeData>>.Node node, Graph<IData<TNodeData>>.Node? parent)
        {
            if (!node.Data.Cost.HasValue || node.Data.Cost > newCost)
            {
                node.Data.Cost = newCost;
                node.Data.Parent = parent;
                return true;
            }
            else
            {
                return false;
            }
        }

        static void Reset(Graph<IData<TNodeData>>.Node node)
        {
            node.Data.Parent = null;
            node.Data.Visited = false;
            node.Data.Cost = null;
        }
    }

    /// <summary>
    /// Find the shortest route through a graph whose data is specialised to support the algorithm
    /// </summary>
    public static (long? Cost, IReadOnlyCollection<Graph<IData<TNodeData>>.Edge> Route) ReportShortestPathTo<TNodeData>(
        Graph<IData<TNodeData>>.Node end)
    {
        return (end.Data.Cost, GetPath(end));

        static IReadOnlyCollection<Graph<IData<TNodeData>>.Edge> GetPath(Graph<IData<TNodeData>>.Node end)
        {
            if (end.Data.Cost is null)
            {
                return Array.Empty<Graph<IData<TNodeData>>.Edge>();
            }

            List<Graph<IData<TNodeData>>.Node> nodePath = new()
            {
                end,
            };


            Graph<IData<TNodeData>>.Node? current = end.Data.Parent;

            while (current is not null)
            {
                nodePath.Add(current);
                current = current.Data.Parent;
            }

            nodePath.Reverse();

            List<Graph<IData<TNodeData>>.Edge> result = new(nodePath.Count - 1);

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
