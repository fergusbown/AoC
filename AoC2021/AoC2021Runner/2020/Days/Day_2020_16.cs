namespace AoC2021Runner;

internal class Day_2020_16 : IDayChallenge
{
    private readonly (IReadOnlyDictionary<string, Func<int, bool>> rules, IReadOnlyList<int> yourTicket, IReadOnlyList<IReadOnlyList<int>> otherTickets) inputData;

    public Day_2020_16(string inputData)
    {
        this.inputData = ParseInput(inputData);
    }

    public string Part1()
    {
        (var rules, _, var otherTickets) = inputData;

        var errorRate = otherTickets
            .SelectMany(o => o)
            .Where(v => rules.Values.All(r => r(v) is false))
            .Sum();

        return $"{errorRate}";
    }

    public string Part2()
    {
        (var rules, var ticket, var otherTickets) = inputData;

        var validTickets = otherTickets
            .Where(t => !t.Any(v => rules.Values.All(r => r(v) is false)))
            .ToList();

        validTickets.Add(ticket);

        var possibleFieldMappings = Enumerable
            .Range(0, ticket.Count)
            .Select(index =>
            {
                var columnValues = validTickets.Select(t => t[index]).ToArray();
                return (index, columnValues);
            })
            .Select(column =>
            {
                var fields = rules.Where(r => column.columnValues.All(v => r.Value(v))).Select(r => r.Key).ToList();
                return (column.index, fields);
            })
            .ToArray();

        Dictionary<string, int> finalfieldMappings = new();
        
        while(possibleFieldMappings.Length > 0)
        {
            var resolved = possibleFieldMappings.Where(m => m.fields.Count == 1).ToArray();
            possibleFieldMappings = possibleFieldMappings.Except(resolved).ToArray();

            foreach((var index, var fields) in resolved)
            {
                var mappedTo = fields[0];
                finalfieldMappings.Add(mappedTo, index);

                foreach(var remaining in possibleFieldMappings)
                {
                    remaining.fields.Remove(mappedTo);
                }
            }
        }

        var departureProducts = finalfieldMappings
            .Where(m => m.Key.StartsWith("departure"))
            .Select(v => ticket[v.Value])
            .Aggregate(1L, (x, y) => x * y);

        return $"{departureProducts}";
    }

    private static (IReadOnlyDictionary<string, Func<int, bool>> rules, IReadOnlyList<int> yourTicket, IReadOnlyList<IReadOnlyList<int>> otherTickets) ParseInput(string inputData)
    {
        var inputParts = inputData.Split($"{Environment.NewLine}{Environment.NewLine}");
        var rulesPart = inputParts[0].StringsForDay();
        var yourTicket = inputParts[1].StringsForDay().Skip(1);
        var otherTickets = inputParts[2].StringsForDay().Skip(1);

        var rules = new Dictionary<string, Func<int, bool>>();

        foreach (var rule in rulesPart)
        {
            var ruleParts = rule.Split(new[] { ": ", "-", " or " }, StringSplitOptions.None);
            int rulePart = 0;
            var field = ruleParts[rulePart++];
            var range1Start = int.Parse(ruleParts[rulePart++]);
            var range1End = int.Parse(ruleParts[rulePart++]);
            var range2Start = int.Parse(ruleParts[rulePart++]);
            var range2End = int.Parse(ruleParts[rulePart++]);

            rules.Add(field, (value) => (value >= range1Start && value <= range1End) || (value >= range2Start && value <= range2End));
        }

        var ticket = yourTicket.Select(t => ToTicket(t)).Single();
        var tickets = otherTickets.Select(t => ToTicket(t)).ToArray();

        return (rules, ticket, tickets);

        static IReadOnlyList<int> ToTicket(string s)
            => s.Split(',').Select(v => int.Parse(v)).ToArray();
    }
}