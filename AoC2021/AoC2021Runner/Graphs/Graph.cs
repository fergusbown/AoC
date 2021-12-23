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

        public void AddEdgeTo(Node end, int weight)
        {
            this.edges.Add(new Edge(this, end, weight));
        }

        public void AddEdgesBetween(Node other, int weight)
        {
            this.AddEdgeTo(other, weight);
            other.AddEdgeTo(this, weight);
        }
    }

    internal class Edge
    {
        public Edge(Node start, Node end, int weight)
        {
            Start = start;
            End = end;
            Weight = weight;
        }

        public int Weight { get; }

        public Node Start { get; }

        public Node End { get; }
    }
}
