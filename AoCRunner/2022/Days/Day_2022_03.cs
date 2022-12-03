namespace AoCRunner;

internal class Day_2022_03 : IDayChallenge
{
    private int[][] rucksacks;

    public Day_2022_03(string inputData)
    {
        this.rucksacks = inputData
            .StringsForDay()
            .Select(s => s.Select(ch => Priority(ch)).ToArray())
            .ToArray();
    }

    public string Part1()
    {
        return rucksacks
            .Select(r =>
            {
                int compartmentCount = r.Length / 2;
                return r[0..compartmentCount].Intersect(r[compartmentCount..]).Single();
            })
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        int result = 0;
        for (int g = 0; g < rucksacks.Length; g += 3)
        {
            var group = rucksacks[g..(g + 3)];
            result += group[0].Intersect(group[1]).Intersect(group[2]).Single();
        }

        return result.ToString();
    }

    private static int Priority(char ch)
    {
        int result = ch - 'a' + 1;

        return result > 0
            ? result
            : ch - 'A' + 27;
    }
}
