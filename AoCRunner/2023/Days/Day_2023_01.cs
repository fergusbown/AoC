namespace AoCRunner;

internal class Day_2023_01 : IDayChallenge
{
    private readonly string inputData;
    private static string[] digitStrings = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", };

    public Day_2023_01(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
        => Solve(GetDigit);

    public string Part2()
        => Solve((s, i) => GetDigit(s, i) ?? GetDigitString(s, i));

    static int? GetDigit(string source, int index)
    {
        if (source[index] >= '0' && source[index] <= '9')
        {
            return source[index] - '0';
        }

        return null;
    }

    static int? GetDigitString(string source, int index)
    {
        for (int i = 0; i < digitStrings.Length; i++)
        {
            if (source.IndexOf(digitStrings[i], index) == index)
            {
                return i + 1;
            }
        }

        return null;
    }

    private string Solve(Func<string, int, int?> parse)
    {
        int result = 0;
        foreach (string line in this.inputData.StringsForDay())
        {
            int? first = null;
            int? last = null;
            for (int i = 0; i < line.Length; i++)
            {
                int? parsed = parse(line, i);
                first ??= parsed;
                last = parsed ?? last;
            }

            result += (first!.Value * 10) + last!.Value;
        }

        return result.ToString();
    }
}
