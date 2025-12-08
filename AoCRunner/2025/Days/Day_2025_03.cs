using System.Collections.Immutable;
using AoCRunner;

internal class Day_2025_03 : IDayChallenge
{
    private readonly ImmutableArray<string> inputData;

    public Day_2025_03(string inputData)
    {
        this.inputData = Parse(inputData);
    }

    public string Part1()
        => Solve(batteryCount: 2);

    public string Part2()
        => Solve(batteryCount: 12);

    private static ImmutableArray<string> Parse(string inputData)
        => [.. inputData.StringsForDay()];

    private string Solve(int batteryCount)
    {
        long total = 0;

        foreach (string bank in inputData)
        {
            long bankTotal = 0;

            int startIndex = 0;
            int endIndex = bank.Length - batteryCount;

            for (int j = 0; j < batteryCount; j++)
            {
                char max = '0';
                int maxIndex = -1;

                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (bank[i] > max)
                    {
                        max = bank[i];
                        maxIndex = i;
                    }
                }

                bankTotal = (bankTotal * 10) + (max - '0');
                startIndex = maxIndex + 1;
                endIndex++;
            }

            total += bankTotal;
        }

        return total.ToString();
    }
}