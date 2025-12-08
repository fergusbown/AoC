using System.Collections.Immutable;
using AoCRunner;
using CommunityToolkit.HighPerformance;

internal class Day_2025_06 : IDayChallenge
{
    private readonly string inputData;

    public Day_2025_06(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        long total = 0;

        foreach(var calculation in Parse(inputData))
        {
            if (calculation.Operator == '+')
            {
                total += calculation.Values.Sum();
            }
            else if (calculation.Operator == '*')
            {
                total += calculation.Values.Aggregate(1L, (x, y) => x * y);
            }
        }

        return total.ToString();
    }

    public string Part2()
    {
        Span2D<char> data = this.inputData.GridForDay(c => c);

        long total = 0;

        var numbers = new List<long>();

        for (int column = data.Width - 1; column >= 0; column--)
        {
            char op = data[data.Height - 1, column];

            long number = 0;

            for (int row = 0; row < data.Height - 1; row++)
            {
                char n = data[row, column];

                if (n != ' ')
                {
                    number = (number * 10) + (n - '0');
                }
            }

            if (number > 0)
            {
                numbers.Add(number);
            }

            if (op == '+')
            {
                total += numbers.Sum();
                numbers.Clear();
            }
            else if (op == '*')
            {
                total += numbers.Aggregate(1L, (x, y) => x * y);
                numbers.Clear();
            }
        }

        return total.ToString();
    }

    private static ImmutableArray<Calculation> Parse(string inputData)
    {
        string[] lines = inputData
            .StringsForDay();

        var values = lines[..^1]
            .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(v => long.Parse(v)).ToArray())
            .ToArray();

        string[] operators = lines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var result = ImmutableArray.CreateBuilder<Calculation>();
        var valuesBuilder = ImmutableArray.CreateBuilder<long>(values.Length);

        for (int i = 0; i < operators.Length; i++)
        {
            valuesBuilder.Clear();

            for (int v = 0; v < values.Length; v++)
            {
                valuesBuilder.Add(values[v][i]);
            }
            var calculation = new Calculation(valuesBuilder.ToImmutable(), operators[i][0]);
            result.Add(calculation);
        }

        return result.ToImmutable();
    }

    private record Calculation(ImmutableArray<long> Values, char Operator);
}
