using Microsoft.Toolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2020_03 : IDayChallenge
{
    private readonly bool[,] input;

    public Day_2020_03(string inputData)
    {
        var lines = inputData.StringsForDay();
        input = new bool[lines.Length, lines[0].Length];

        for (int row = 0; row < lines.Length; row++)
        {
            string rowText = lines[row];
            for (int column = 0; column < rowText.Length; column++)
            {
                input[row, column] = rowText[column] switch
                {
                    '#' => true,
                    _ => false,
                };
            }
        }
    }

    public string Part1()
        => TreesHit(3, 1).ToString();

    public string Part2()
    {
        long result =
            TreesHit(1, 1) *
            TreesHit(3, 1) *
            TreesHit(5, 1) *
            TreesHit(7, 1) *
            TreesHit(1, 2);

        return $"{result}";
    }

    private long TreesHit(int slopeRight, int slopeDown)
    {
        Span2D<bool> data = new(input);

        long trees = 0;
        int column = 0;
        for (int row = 0; row < data.Height; row += slopeDown, column = (column + slopeRight) % data.Width)
        {
            if (data[row, column])
            {
                trees++;
            }
        }

        return trees;

    }
}