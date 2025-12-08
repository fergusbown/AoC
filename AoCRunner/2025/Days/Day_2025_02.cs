using AoCRunner;

internal class Day_2025_02 : IDayChallenge
{
    private readonly (long Start, long End)[] inputData;

    public Day_2025_02(string inputData)
    {
        this.inputData = Parse(inputData);
    }

    public string Part1()
    {
        long answer = 0;

        foreach ((var start, var end) in inputData)
        {
            for (long id = start; id <= end; id++)
            {
                var str = id.ToString();

                bool valid = str.Length % 2 == 1;

                if (!valid)
                {
                    int half = str.Length / 2;
                    for (int i = 0; i < half; i++)
                    {
                        if (str[i] != str[i + half])
                        {
                            valid = true;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    answer += id;
                }
            }
        }

        return answer.ToString();
    }

    public string Part2()
    {
        long answer = 0;

        foreach ((var start, var end) in inputData)
        {
            for (long id = start; id <= end; id++)
            {
                if (IsInvalid(id))
                {
                    answer += id;
                }
            }
        }

        return answer.ToString();

        static bool IsInvalid(long id)
        {
            var str = id.ToString();

            for (int length = 1; length <= str.Length / 2; length++)
            {
                if (str.Length % length != 0)
                {
                    continue;
                }

                bool valid = false;
                for (int i = length; i < str.Length; i++)
                {
                    if (str[i] != str[i % length])
                    {
                        valid = true;
                        break;
                    }
                }

                if (!valid)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private static (long Start, long End)[] Parse(string inputData)
    {
        return inputData
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Split('-', StringSplitOptions.RemoveEmptyEntries))
            .Select(r => (long.Parse(r[0]), long.Parse(r[1])))
            .ToArray();
    }
}