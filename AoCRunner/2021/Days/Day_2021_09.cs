using Microsoft.Toolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2021_09 : IDayChallenge
{
    private readonly int[,] inputArray;

    public Day_2021_09(string inputData)
    {
        this.inputArray = GetInputData(inputData).ToArray();
    }

    public string Part1()
    {
        Span2D<int> input = new(inputArray);

        int sumOfRisk = 0;
        for (int rowIndex = 0; rowIndex < input.Height; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < input.Width; columnIndex++)
            {
                sumOfRisk += GetRiskLevel(input, rowIndex, columnIndex);
            }
        }

        return sumOfRisk.ToString();
    }

    public string Part2()
    {
        Span2D<int> input = new(inputArray);

        List<HashSet<(int Row, int Column)>> basins = new();
        Stack<(int Row, int Column)> pending = new();

        HashSet<(int Row, int Column)> remaining = new();

        for (int rowIndex = 0; rowIndex < input.Height; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < input.Width; columnIndex++)
            {
                if (input[rowIndex, columnIndex] != 9)
                {
                    remaining.Add((rowIndex, columnIndex));
                }
            }
        }

        while (remaining.Count > 0)
        {
            HashSet<(int Row, int Column)> currentBasin = new();
            basins.Add(currentBasin);

            var item = remaining.First();
            remaining.Remove(item);
            pending.Push(item);

            while (pending.Count > 0)
            {
                item = pending.Pop();
                currentBasin.Add(item);

                foreach (var (row, column, height) in GetAdjacencies(input, item.Row, item.Column))
                {
                    if (remaining.Remove((row, column)))
                    {
                        pending.Push((row, column));
                    }
                }
            }
        }

        return basins
            .Select(b => b.Count)
            .OrderByDescending(b => b)
            .Take(3)
            .Aggregate((a, b) => a * b).ToString();
    }

    private static Span2D<int> GetInputData(string input)
    {
        List<int> digits = new(input.Length);
        int width = input.IndexOf(Environment.NewLine);

        foreach (char c in input)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    digits.Add(c - '0');
                    break;
                default:
                    break;
            }
        }

        return new Span2D<int>(digits.ToArray(), digits.Count / width, width);
    }

    private static IEnumerable<(int Row, int Column, int Height)> GetAdjacencies(Span2D<int> input, int row, int column)
    {
        List<(int Row, int Rolumn, int Value)> result = new();

        if (row > 0)
            result.Add((row - 1, column, input[row - 1, column]));

        if (row < input.Height - 1)
            result.Add((row + 1, column, input[row + 1, column]));

        if (column > 0)
            result.Add((row, column - 1, input[row, column - 1]));

        if (column < input.Width - 1)
            result.Add((row, column + 1, input[row, column + 1]));

        return result;
    }

    private static int GetRiskLevel(Span2D<int> input, int row, int column)
    {
        int height = input[row, column];
        if (GetAdjacencies(input, row, column).All(adj => adj.Height > height))
        {
            return height + 1;
        }
        else
        {
            return 0;
        }
    }
}