namespace AoCRunner;

internal class Day_2020_01 : IDayChallenge
{
    private readonly int[] inputData;

    public Day_2020_01(string inputData)
    {
        this.inputData = inputData.IntsForDay();
    }

    public string Part1()
    {
        for (int i = 0; i < inputData.Length; i++)
        {
            for (int j = i+1; j < inputData.Length; j++)
            {
                if (inputData[i] + inputData[j] == 2020)
                {
                    return $"{inputData[i] * inputData[j]}";
                }
            }
        }

        throw new InvalidOperationException("Should have found an answer");
    }

    public string Part2()
    {
        for (int i = 0; i < inputData.Length; i++)
        {
            for (int j = i + 1; j < inputData.Length; j++)
            {
                for (int k = j+1; k < inputData.Length; k++)
                {
                    if (inputData[i] + inputData[j] + inputData[k] == 2020)
                    {
                        return $"{inputData[i] * inputData[j] * inputData[k]}";
                    }

                }
            }
        }

        throw new InvalidOperationException("Should have found an answer");
    }
}