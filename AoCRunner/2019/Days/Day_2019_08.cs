using System.Text;
using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal partial class Day_2019_08 : IDayChallenge
{
    private readonly int[][] layers;
    private int width;
    private readonly int height;

    public Day_2019_08(string inputData)
    {
        this.width = 25;
        this.height = 6;
        this.layers = GetLayers(inputData.Select(c => c - '0').ToArray(), width, height).ToArray();

        static IEnumerable<int[]> GetLayers(int[] inputData, int width, int height)
        {
            int layerSize = width * height;

            int endIndex = layerSize;
            for (int startIndex = 0; startIndex < inputData.Length; startIndex += layerSize, endIndex += layerSize)
            {
                yield return inputData[startIndex..endIndex];
            }
        }
    }

    public string Part1()
    {
        int lowestZeros = int.MaxValue;
        int answer = 0;
        foreach (int[] layer in layers)
        {
            int zeroCount = layer.Count(0);
            if (zeroCount < lowestZeros)
            {
                answer = layer.Count(1) * layer.Count(2);
                lowestZeros = zeroCount;
            }
        }
        return answer.ToString();
    }

    public string Part2()
    {
        var image = Enumerable.Repeat(2, width * height).ToArray();

        foreach (int[] layer in layers)
        {
            for (int i = 0; i < image.Length; i++)
            {
                if (image[i] == 2)
                {
                    image[i] = layer[i];
                }
            }
        }

        StringBuilder result = new();

        int index = 0;
        for (int rowIndex = 0; rowIndex < 6; rowIndex++)
        {
            result.AppendLine();
            for (int columnIndex = 0; columnIndex < 25; columnIndex++)
            {
                result.Append(image[index++] == 1 ? '#' : ' ');
            }
        }

        return result.ToString();
    }
}
