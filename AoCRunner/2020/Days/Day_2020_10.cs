using System.Diagnostics;

namespace AoCRunner;

internal class Day_2020_10 : IDayChallenge
{
    private readonly IReadOnlyList<int> inputData;

    public Day_2020_10(string inputData)
    {
        this.inputData = ParseInput(inputData);
    }

    public string Part1()
    {
        (var differencesOf1, var differencesOf3, _) = AnalyseAdapters(this.inputData);
        return $"{differencesOf1 * differencesOf3}";
    }

    public string Part2()
    {
        (_, _, var permutations) = AnalyseAdapters(this.inputData);
        return $"{permutations}";
    }

    private static IReadOnlyList<int> ParseInput(string input)
    {
        var adapters = input
            .StringsForDay()
            .Select(x => int.Parse(x))
            .OrderBy(x => x)
            .ToList();

        adapters.Insert(0, 0);
        adapters.Add(adapters[^1] + 3);

        return adapters;
    }

    private static (int differencesOf1Count, int differencesOf3Count, long validPermutations) AnalyseAdapters(IReadOnlyList<int> adapters)
    {
        int differencesOf1 = 0;
        int differencesOf3 = 0;

        Dictionary<int, int> runsOfDifferenceOfOne = new()
        {
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
        };

        int runOfDifferenceOfOne = 0;

        for (int i = 1; i < adapters.Count; i++)
        {
            var difference = adapters[i] - adapters[i - 1];
            switch (difference)
            {
                case 1:
                    differencesOf1++;
                    runOfDifferenceOfOne++;
                    break;
                case 3:
                    differencesOf3++;

                    if (!runsOfDifferenceOfOne.TryAdd(runOfDifferenceOfOne, 1))
                    {
                        runsOfDifferenceOfOne[runOfDifferenceOfOne]++;
                    }

                    runOfDifferenceOfOne = 0;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected difference of {difference}");
            }
        }

        Debug.Assert(runsOfDifferenceOfOne.Keys.Max() == 4);

        // 1 1s => no permutations (has to be chosen for the gapo at each end to be bridged)
        // 2 1s => no permutations (has to be chosen for the gapo at each end to be bridged)
        // 3 1s => 2 permutations (choose or don't choose the middle one)
        // 4 1s => 4 permutations (choose or don't choose either of the middle two)
        // 5 1s => 7 permutations (choose or don't choose any of the middle three, but you must choose at least one)

        //x 1s => runsOfDifferenceOfOne[x-1] since thats the number of gaps;

        long permutations = 1;
        permutations <<= runsOfDifferenceOfOne[2];
        permutations <<= runsOfDifferenceOfOne[3] * 2;

        for (int i = 0; i < runsOfDifferenceOfOne[4]; i++)
        {
            permutations *= 7;
        }

        return (differencesOf1, differencesOf3, permutations);
    }
}