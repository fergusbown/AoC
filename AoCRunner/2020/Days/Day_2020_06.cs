namespace AoCRunner;

internal class Day_2020_06 : IDayChallenge
{
    private readonly string[][] groups;

    public Day_2020_06(string inputData)
    {
        this.groups = inputData
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(g => g.StringsForDay())
            .ToArray();
    }

    public string Part1()
    {
        return groups
            .Select(g =>
            {
                HashSet<char> allAnswers = new();

                foreach (var person in g)
                {
                    allAnswers.UnionWith(person);
                }

                return allAnswers.Count;
            })
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        return groups
            .Select(g =>
            {
                HashSet<char> universalAnswers = new(g[0]);

                foreach (var person in g.Skip(1))
                {
                    universalAnswers.IntersectWith(person);
                }

                return universalAnswers.Count;
            })
            .Sum()
            .ToString();
    }
}