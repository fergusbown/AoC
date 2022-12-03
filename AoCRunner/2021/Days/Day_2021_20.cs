using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2021_20 : IDayChallenge
{
    private readonly (bool[] Algorithm, bool[,] InputGrid) inputData;

    public Day_2021_20(string inputData)
    {
        this.inputData = ParseInput(inputData);
    }

    public string Part1()
        => Enhance(this.inputData.InputGrid, 2);

    public string Part2()
        => Enhance(this.inputData.InputGrid, 50);

    private static (bool[] Algorithm, bool[,] InputGrid) ParseInput(string input)
    {
        var inputLines = input.StringsForDay();

        int width = inputLines[2].Length;
        int height = inputLines.Length - 2;

        var algorithm = inputLines[0].Select(x => x == '#').ToArray();

        // inflate the frid so that we don't have to worry about bits that fall off the edge
        bool[,] inputGrid = new bool[height + 4, width + 4];

        Span2D<bool> inputSpan = new Span2D<bool>(inputGrid).Slice(2, 2, height, width);

        for (int row = 0; row < height; row++)
        {
            string rowString = inputLines[row + 2];
            for (int column = 0; column < width; column++)
            {
                inputSpan[row, column] = rowString[column] == '#';
            }
        }

        return (algorithm, inputGrid);
    }

    private string Enhance(bool[,] input, int count)
    {
        var current = input;
        int litCount = 0;
        for (int i = 0; i < count; i++)
        {
            current = ApplyAlgorithm(current, out litCount);
        }

        return litCount.ToString();
    }

    private bool[,] ApplyAlgorithm(bool[,] input, out int litCount)
    {
        bool[,] result = new bool[input.GetLength(0) + 2, input.GetLength(1) + 2];
        litCount = 0;

        Span2D<bool> inputSpan = new(input);
        Span2D<bool> resultSpan = new(result);

        //take account of the fact that the infinitae space might toggle in value
        bool currentInfiniteSpace = input[0, 0];
        bool newInfiniteSpace = currentInfiniteSpace ? this.inputData.Algorithm[^1] : this.inputData.Algorithm[0];

        // set the new two rows/columns to the new infinite space value - cheat because our data is square
        for (int i = 0; i < resultSpan.Width; i++)
        {
            resultSpan[0, i] = newInfiniteSpace;
            resultSpan[1, i] = newInfiniteSpace;
            resultSpan[i, 0] = newInfiniteSpace;
            resultSpan[i, 1] = newInfiniteSpace;
            resultSpan[resultSpan.Height - 1, i] = newInfiniteSpace;
            resultSpan[i, resultSpan.Width - 1] = newInfiniteSpace;
            resultSpan[resultSpan.Height - 2, i] = newInfiniteSpace;
            resultSpan[i, resultSpan.Width - 2] = newInfiniteSpace;
        }

        resultSpan = resultSpan.Slice(1, 1, inputSpan.Height, inputSpan.Width);

        for (int row = 1; row < inputSpan.Height - 1; row++)
        {
            for (int column = 1; column < inputSpan.Width - 1; column++)
            {
                Span2D<bool> pointSpace = inputSpan.Slice(row - 1, column - 1, 3, 3);

                if (GetEnhanced(pointSpace, this.inputData.Algorithm))
                {
                    resultSpan[row, column] = true;
                    litCount++;
                }
            }
        }

        return result;

        static bool GetEnhanced(Span2D<bool> pointSpace, bool[] algorithm)
        {
            int index = 0;
            for (int row = 0; row < pointSpace.Height; row++)
            {
                for (int column = 0; column < pointSpace.Width; column++)
                {
                    index *= 2;
                    if (pointSpace[row, column])
                    {
                        index++;
                    }
                }
            }

            return algorithm[index];
        }
    }
}
