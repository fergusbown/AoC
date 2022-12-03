namespace AoCRunner;

internal class Day_2022_01 : IDayChallenge
{
    private readonly int[][] inputData;

    public Day_2022_01(string inputData)
    {
        this.inputData = inputData
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(e => e.IntsForDay())
            .ToArray();
    }

    public string Part1()
        => inputData.Select(e => e.Sum()).Max().ToString();

    public string Part2()
        => inputData.Select(e => e.Sum()).OrderByDescending(k => k).Take(3).Sum().ToString();
}
