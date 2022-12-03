namespace AoCRunner;

internal class Day_2020_09 : IDayChallenge
{
    private readonly long[] inputData;

    public Day_2020_09(string inputData)
    {
        this.inputData = inputData
            .StringsForDay()
            .Select(x => long.Parse(x))
            .ToArray();
    }

    public string Part1()
    {
        return $"{FindFirstWrongNumber(inputData, 25)}";
    }

    public string Part2()
    {
        long wrongNumber = FindFirstWrongNumber(inputData, 25);

        long sum = 0;
        int startIndex = 0;
        int endIndex = 0;

        while (sum != wrongNumber)
        {
            // roll forward until we can add the next number successfully
            while (sum + inputData[endIndex] > wrongNumber)
            {
                sum -= inputData[startIndex++];
            }

            // keep adding the next numbers until we would be too big
            while (sum < wrongNumber)
            {
                sum += inputData[endIndex++];
            }
        }

        long[] range = inputData[startIndex..endIndex];

        long min = range.Min();
        long max = range.Max();

        return $"{min + max}";
    }

    private static long FindFirstWrongNumber(long[] data, int preamblelength)
    {
        int validStartIndex = 0;
        ReadOnlySpan<long> validForSum = new(data, validStartIndex, preamblelength);

        for (int i = preamblelength; i < data.Length; i++)
        {
            if (IsValid(data[i], validForSum))
            {
                validForSum = new Span<long>(data, ++validStartIndex, preamblelength);
            }
            else
            {
                return data[i];
            }
        }

        throw new InvalidOperationException("All numbers are OK");

        static bool IsValid(long value, ReadOnlySpan<long> inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    if (inputs[i] + inputs[j] == value && inputs[i] != inputs[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}