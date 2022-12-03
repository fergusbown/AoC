using System.Diagnostics.CodeAnalysis;

namespace AoCRunner;

/// <summary>
/// Work out the regions in a graph
/// </summary>
internal static class StronglyConnectedRegion
{
    public static IReadOnlyCollection<IReadOnlyCollection<Graph<T>.Node>> StronglyConnectedRegions<T>(this Graph<T> graph)
    {
        HashSet<Graph<T>.Node> unvisited = new(graph.Nodes);
        List<IReadOnlyCollection<Graph<T>.Node>> result = new();

        while (unvisited.Count > 0)
        {
            HashSet<Graph<T>.Node> region = new();
            Stack<Graph<T>.Node> pending = new();
            pending.Push(unvisited.First());

            while(pending.TryPop(out var node))
            {
                if (region.Add(node))
                {
                    foreach (var edge in node.Edges)
                    {
                        pending.Push(edge.End);
                    }
                }
            }

            unvisited.ExceptWith(region);
            result.Add(region);
        }

        return result;
    }
}
