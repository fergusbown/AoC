using System.Diagnostics;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner;

internal static class InputData
{
    public static string? InputForDay(Type implementation)
    {
        string? resourceName = implementation.Assembly.GetManifestResourceNames().SingleOrDefault(n => n.EndsWith($"{implementation.Name}.txt"));

        if (resourceName is null)
        {
            return null;
        }
        else
        {
            using Stream stream = implementation.Assembly.GetManifestResourceStream(resourceName)!;
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }

    public static string[] StringsForDay(this string inputData)
        => inputData.Split(Environment.NewLine);

    public static int[] IntsForDay(this string inputData)
        => inputData.StringsForDay().Select(s => int.Parse(s)).ToArray();

    public static (T, int)[] InstructionsForDay<T>(this string inputData)
        where T : struct
    {
        return inputData
            .StringsForDay()
            .Select(s => s.Split(' '))
            .Select(s => (Enum.Parse<T>(s[0], true), int.Parse(s[1])))
            .ToArray();
    }

    public static int ToIntFromBinaryString(this string binaryString)
        => Convert.ToInt32(binaryString, 2);

    public static Span2D<T> GridForDay<T>(
        this string inputData,
        Func<char, T> parse,
        int extraBorder = 0,
        Func<T>? borderValue = null)
    {
        if (extraBorder > 0 && borderValue is null)
        {
            throw new ArgumentOutOfRangeException(nameof(borderValue));
        }

        var lines = inputData.StringsForDay();
        var width = lines[0].Length + (extraBorder * 2);
        var height = lines.Length + (extraBorder * 2);

        T[] backingArray = new T[width * height];

        Span2D<T> grid = new(backingArray, height, width);

        for (int i = 0; i < extraBorder; i++)
        {
            Debug.Assert(borderValue is not null);

            for (int columnIndex = 0; columnIndex < grid.Width; columnIndex++)
            {
                grid[i, columnIndex] = borderValue();
                grid[grid.Height - i - 1, columnIndex] = borderValue();
            }

            for (int rowIndex = 0; rowIndex < grid.Height; rowIndex++)
            {
                grid[rowIndex, i] = borderValue();
                grid[rowIndex, grid.Width - i - 1] = borderValue();
            }
        }

        for (int rowIndex = 0; rowIndex < lines.Length; rowIndex++)
        {
            var row = lines[rowIndex];

            for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                grid[rowIndex + extraBorder, columnIndex + extraBorder] = parse(row[columnIndex]);
            }
        }

        return grid;
    }
}