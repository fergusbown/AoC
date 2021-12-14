using System.Diagnostics;

namespace AoC2021Runner;

internal static class InputData
{
    public static string InputForDay(Type implementation)
    {
        string resourceName = implementation.Assembly.GetManifestResourceNames().Single(n => n.EndsWith($"{implementation.Name}.txt"));
        using Stream stream = implementation.Assembly.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
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

    public static string ElapsedString(this Stopwatch sw)
    {
        string result = $" ({sw.ElapsedMilliseconds}ms)";
        sw.Restart();
        return result;
    }
}