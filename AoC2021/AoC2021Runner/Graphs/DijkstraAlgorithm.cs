using System.Diagnostics.CodeAnalysis;

namespace AoC2021Runner;
internal static class DijkstraAlgorithm
{
    public static (int Cost, IReadOnlyCollection<Graph<TNodeData>.Edge>) FindShortestPath<TNodeData>(Graph<TNodeData>.Node start, Graph<TNodeData>.Node end)
    {
        Dictionary<Graph<TNodeData>.Node, double> unvisitedNodeCosts = new();
        Dictionary<Graph<TNodeData>.Node, Graph<TNodeData>.Node> nodeParents = new();
        Dictionary<Graph<TNodeData>.Node, double> visitedNodeCosts = new();

        unvisitedNodeCosts.Add(start, 0);

        while(TryGetCheapestNode(out var node, out double cost))
        {
            foreach (var edge in node.Edges.Where(e => !visitedNodeCosts.ContainsKey(e.End)))
            {
                var costToEnd = cost + edge.Weight;
                UpdateNodeCost(edge.End, costToEnd, node);
            }

            unvisitedNodeCosts.Remove(node);
            visitedNodeCosts.Add(node, cost);
        }

        return ((int)visitedNodeCosts[end], GetPath());

        bool TryGetCheapestNode([NotNullWhen(true)] out Graph<TNodeData>.Node? node, out double cost)
        {
            if (unvisitedNodeCosts.Count == 0)
            {
                node = null;
                cost = double.PositiveInfinity;
                return false;
            }

            KeyValuePair<Graph<TNodeData>.Node, double> cheapest = unvisitedNodeCosts.OrderBy(kvp => kvp.Value).FirstOrDefault();

            node = cheapest.Key;
            cost = cheapest.Value;
            return true;
        }

        void UpdateNodeCost(Graph<TNodeData>.Node node, double cost, Graph<TNodeData>.Node parent)
        {
            if (visitedNodeCosts.TryGetValue(node, out double currentCost) && currentCost > cost)
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
