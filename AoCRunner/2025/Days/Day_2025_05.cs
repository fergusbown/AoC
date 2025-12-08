using System.Collections.Immutable;
using AoCRunner;

internal class Day_2025_05 : IDayChallenge
{
    private readonly Input inputData;

    public Day_2025_05(string inputData)
    {
        this.inputData = Parse(inputData);
    }

    public string Part1()
    {
        int fresh = 0;
        foreach (var supply in inputData.Supplies)
        {
            if (inputData.Ranges.Any(r => r.Contains(supply)))
            {
                fresh++;
            }
        }

        return fresh.ToString();
    }

    public string Part2()
    {
        List<SimpleRange<long>> merged = [];

        foreach (SimpleRange<long> range in this.inputData.Ranges)
        {
            SimpleRange<long>[] remaining = [range];

            foreach (var existing in merged)
            {
                remaining = remaining
                    .SelectMany(r => r.Except(existing))
                    .ToArray();
            }

            merged.AddRange(remaining);
        }

        return merged.Select(m => m.Count).Sum().ToString();
    }


    private static Input Parse(string inputData)
    {
        var parts = inputData.Split($"{Environment.NewLine}{Environment.NewLine}");

        var ranges = parts[0].StringsForDay()
            .Select(line =>
            {
                var parts = line.Split('-', StringSplitOptions.TrimEntries);
                return new SimpleRange<long>(long.Parse(parts[0]), long.Parse(parts[1]) + 1);
            })
            .ToImmutableArray();

        var supplies = parts[1]
            .StringsForDay()
            .Select(long.Parse)
            .ToImmutableArray();

        return new Input(ranges, supplies);
    }

    private record Input(ImmutableArray<SimpleRange<long>> Ranges, ImmutableArray<long> Supplies);

}
