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
        Dictionary<int, int> previouslySpoken = new(gameLength);

        for (int i = 0; i < startingNumbers.Length - 1; i++)
        {
            previouslySpoken.Add(startingNumbers[i], i);
        }

        int lastSpoken = startingNumbers[^1];

        for (int i = startingNumbers.Length; i < gameLength; i++)
        {
            int nextSpoken;
            if (previouslySpoken.TryGetValue(lastSpoken, out var spokenInRound))
            {
                nextSpoken = i - spokenInRound - 1;
            }
            else
            {
                nextSpoken = 0;
            }

            previouslySpoken[lastSpoken] = i - 1;
            lastSpoken = nextSpoken;
        }

        return lastSpoken;
    }
}