namespace AoC2021Runner;

internal class Graph<TNodeData>
{
    private readonly List<Node> nodes = new();

    public IReadOnlyList<Node> Nodes => nodes;

    public Node AddNode(TNodeData nodeData)
    {
        var result = new Node(nodeData);
        nodes.Add(result);
        return result;
    }

    internal class Node
    {
        private readonly List<Edge> edges = new();

        public Node(TNodeData data)
        {
            Data = data;
        }

        public TNodeData Data { get; }

        public IReadOnlyCollection<Edge> Edges => edges;

        public void AddEdgeTo(Node end, long weight)
        {
            this.edges.Add(new Edge(this, end, weight));
        }

        public void AddEdgesBetween(Node other, long weight)
        {
            this.AddEdgeTo(other, weight);
            other.AddEdgeTo(this, weight);
        }
    }

    internal class Edge
    {
        public Edge(Node start, Node end, long weight)
        {
            Start = start;
            End = end;
            Weight = weight;
        }

        public long Weight { get; }

        public Node Start { get; }

        public Node End { get; }
    }

    public class DirectionAgnosticEdgeComparer : IEqualityComparer<Edge>
    {
        bool IEqualityComparer<Graph<TNodeData>.Edge>.Equals(Graph<TNodeData>.Edge? x, Graph<TNodeData>.Edge? y)
        {
            if (x is null)
            {
                return y is null;
            }

            if (y is null)
            {
                return false;
            }

            if (x.Weight != y.Weight)
            {
                return false;
            }

            return (x.Start == y.Start && x.End == y.End) || (x.Start == y.End && x.End == y.Start);
        }

        int IEqualityComparer<Graph<TNodeData>.Edge>.GetHashCode(Graph<TNodeData>.Edge obj)
        {
            return obj.Start.GetHashCode() + obj.End.GetHashCode();
        }
    }
}
