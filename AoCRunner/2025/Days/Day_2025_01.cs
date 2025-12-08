using System.Collections.Immutable;
using AoCRunner;

internal class Day_2025_01 : IDayChallenge
{
    private readonly ImmutableArray<int> inputData;

    public Day_2025_01(string inputData)
    {
        this.inputData = [.. Parse(inputData)];
    }

    public string Part1()
    {
        int position = 50;
        int zeros = 0;

        foreach (int move in inputData)
        {
            position = (position + move) % 100;

            if (position == 0)
            {
                zeros++;
            }
        }

        return zeros.ToString();
    }

    public string Part2()
    {
        int position = 50;
        int zeros = 0;

        foreach (int move in inputData)
        {
            // add in full rotations
            zeros += Math.Abs(move / 100);

            (int lastPosition, position) = (position, (position + move) % 100);

            if (position < 0)
            {
                position += 100;
            }

            if (lastPosition != 0)
            {
                if (position == 0)
                {
                    zeros++;
                }
                else if (move > 0 && position < lastPosition)
                {
                    zeros++;
                }
                else if (move < 0 && position > lastPosition)
                {
                    zeros++;
                }
            }
        }

        return zeros.ToString();
    }

    private static IEnumerable<int> Parse(string inputData)
    {
        return inputData
            .StringsForDay()
            .Select(i =>
            {
                int modifier = i[0] == 'L' ? -1 : 1;
                return modifier * int.Parse(i[1..]);
            });
    }
}