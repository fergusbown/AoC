namespace AoCRunner;

internal class Day_2023_04 : IDayChallenge
{
    private readonly IReadOnlyCollection<(IReadOnlyCollection<int> Winning, IReadOnlyCollection<int> Have)> inputData;

    public Day_2023_04(string inputData)
    {
        this.inputData = Parse(inputData);
    }

    public string Part1()
    {
        return inputData
            .Select(i => i.Winning.Intersect(i.Have).Distinct().Count())
            .Select(w => w == 0 ? 0 : (int)Math.Pow(2, w - 1))
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        return "";
    }

    private static IReadOnlyCollection<(IReadOnlyCollection<int> Winning, IReadOnlyCollection<int> Have)> Parse(string inputData)
    {
        return inputData
            .StringsForDay()
            .Select(s => s.Substring(10))
            .Select(s => s.Split(" | "))
            .Select(p => (GetNumbers(p[0]), GetNumbers(p[1])))
            .ToArray();

        static IReadOnlyCollection<int> GetNumbers(string str)
        {
            return str.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
        }
    }
}
