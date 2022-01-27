namespace AoC2021Runner;

internal class Day_2020_15 : IDayChallenge
{
    private readonly int[] inputData;

    public Day_2020_15(string inputData)
    {
        this.inputData = inputData
            .Split(',')
            .Select(c => int.Parse(c))
            .ToArray();
    }

    public string Part1()
    {
        return $"{PlayRounds(inputData, 2020)}";
    }

    public string Part2()
    {
        return $"{PlayRounds(inputData, 30000000)}";
    }

    private static int PlayRounds(int[] startingNumbers, int gameLength)
    {
        int[] previouslySpoken = new int[gameLength];

        for (int i = 0; i < startingNumbers.Length - 1; i++)
        {
            previouslySpoken[startingNumbers[i]] = i+1;
        }

        int lastSpoken = startingNumbers[^1];

        for (int i = startingNumbers.Length; i < gameLength; i++)
        {
            int nextSpoken;
            int spokenInRound = previouslySpoken[lastSpoken];
            if (spokenInRound > 0)
            {
                nextSpoken = i - spokenInRound;
            }
            else
            {
                nextSpoken = 0;
            }

            previouslySpoken[lastSpoken] = i;
            lastSpoken = nextSpoken;
        }

        return lastSpoken;
    }
}