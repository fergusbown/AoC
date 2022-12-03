namespace AoCRunner;

internal class Day_2020_07 : IDayChallenge
{
    private readonly Graph<string> bagToContainerGraph;
    private readonly Graph<string> bagToContentsGraph;

    public Day_2020_07(string inputData)
    {
        this.bagToContainerGraph = BuildBagGraph(inputData, LinkDirection.BagToContainer);
        this.bagToContentsGraph = BuildBagGraph(inputData, LinkDirection.BagToContents);
    }

    public string Part1()
    {
        var shinyGoldBag = this.bagToContainerGraph.Nodes.Single(n => n.Data == "shiny gold");

        HashSet<Graph<string>.Node> visited = new();
        Stack<Graph<string>.Node> pending = new();
        pending.Push(shinyGoldBag);

        while (pending.TryPop(out var bag))
        {
            foreach (var edge in bag.Edges)
            {
                if (visited.Add(edge.End))
                {
                    pending.Push(edge.End);
                }
            }
        }

        return $"{visited.Count}";
    }

    public string Part2()
    {
        var shinyGoldBag = this.bagToContentsGraph.Nodes.Single(n => n.Data == "shiny gold");

        Stack<Graph<string>.Node> pending = new();
        pending.Push(shinyGoldBag);
        long bagsRequiredInside = 0;

        while (pending.TryPop(out var bag))
        {
            foreach (var edge in bag.Edges)
            {
                bagsRequiredInside += edge.Weight;

                for (int i = 0; i < edge.Weight; i++)
                {
                    pending.Push(edge.End);
                }
            }
        }

        return $"{bagsRequiredInside}";
    }

    private enum LinkDirection
    {
        BagToContents,
        BagToContainer
    }

    private static Graph<string> BuildBagGraph(string inputData, LinkDirection linkDirection)
    {
        Graph<string> rulesGraph = new();

        Dictionary<string, Graph<string>.Node> bagToNode = new();
        var rules = inputData
            .Replace(" contain no other bags", "")
            .Replace(".", "")
            .Replace(" contain", ",")
            .Replace(" bags", "")
            .Replace(" bag", "")
            .StringsForDay();

        foreach (var rule in rules)
        {
            var ruleParts = rule.Split(", ");

            var bag = GetNode(ruleParts[0]);

            foreach (var rulePart in ruleParts.Skip(1))
            {
                int firstSpaceIndex = rulePart.IndexOf(' ');
                int count = int.Parse(rulePart[..firstSpaceIndex]);
                var containsBag = GetNode(rulePart[(firstSpaceIndex + 1)..]);

                switch (linkDirection)
                {
                    case LinkDirection.BagToContents:
                        bag.AddEdgeTo(containsBag, count);
                        break;
                    case LinkDirection.BagToContainer:
                        containsBag.AddEdgeTo(bag, count);
                        break;
                    default:
                        break;
                }
            }
        }

        return rulesGraph;

        Graph<string>.Node GetNode(string bagString)
        {
            if (!bagToNode.TryGetValue(bagString, out var bag))
            {
                bag = rulesGraph.AddNode(bagString);
                bagToNode[bagString] = bag;
            }

            return bag;
        }

    }
}