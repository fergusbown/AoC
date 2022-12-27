using System.Diagnostics;
using CommunityToolkit.HighPerformance;

namespace AoCRunner;

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
        ParseGridDelegate<T> parse,
        int extraBorder = 0,
        DefaultValueDelegate<T>? defaultValue = null)
    {
        if (extraBorder > 0 && defaultValue is null)
        {
            throw new ArgumentOutOfRangeException(nameof(defaultValue));
        }

        var lines = inputData.StringsForDay();
        var width = lines.Select(l => l.Length).Max() + (extraBorder * 2);
        var height = lines.Length + (extraBorder * 2);

        T[] backingArray = new T[width * height];

        Span2D<T> grid = new(backingArray, height, width);

        if (defaultValue is not null)
        {
            for (int row = 0; row < grid.Height; row++)
            {
                for (int column = 0; column < grid.Width; column++)
                {
                    grid[row, column] = defaultValue(row, column);
                }
            }
        }

        for (int rowIndex = 0; rowIndex < lines.Length; rowIndex++)
        {
            var row = lines[rowIndex];

            for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                grid[rowIndex + extraBorder, columnIndex + extraBorder] = parse(row[columnIndex], rowIndex, columnIndex);
            }
        }

        return grid;

    }

    public static Span2D<T> GridForDay<T>(
        this string inputData,
        Func<char, T> parse,
        int extraBorder = 0,
        Func<T>? defaultValue = null)
        => inputData.GridForDay((c, _, _) => parse(c), extraBorder, defaultValue is null ? null : (_, _) => defaultValue());
}

public delegate T ParseGridDelegate<T>(char value, int row, int column);
public delegate T DefaultValueDelegate<T>(int row, int column);