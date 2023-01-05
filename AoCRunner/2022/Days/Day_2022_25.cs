using System.Text;

namespace AoCRunner;

internal class Day_2022_25 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_25(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        return ToSnafuNumber(inputData
            .StringsForDay()
            .Select(FromSnafuNumber)
            .Sum());
    }

    public string Part2()
    {
        return "Happy Christmas!";
    }

    private static long FromSnafuNumber(string snafu)
    {
        long result = 0;
        foreach (char ch in snafu)
        {
            result *= 5;

            switch (ch)
            {
                case '-':
                    result -= 1;
                    break;
                case '=':
                    result -= 2;
                    break;
                default:
                    result += ch - '0';
                    break;
            }
        }

        return result;
    }

    private static string ToSnafuNumber(long regular)
    {
        StringBuilder sb = new();
        long remaining = regular;

        while (remaining > 0)
        {
            long digit = remaining % 5;
            remaining /= 5;

            switch (digit)
            {
                case 0:
                case 1:
                case 2:
                    sb.Insert(0, digit);
                    break;
                case 3:
                    remaining += 1;
                    sb.Insert(0, '=');
                    break;
                case 4:
                    remaining += 1;
                    sb.Insert(0, '-');
                    break;
                default:
                    throw new InvalidOperationException();
            }

        }

        return sb.ToString();
    }
}
