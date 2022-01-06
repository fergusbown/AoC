using System.Text;

namespace AoC2021Runner;

internal class Day_2021_03 : IDayChallenge
{
    private readonly (HashSet<string> Ones, HashSet<string> Zeros)[] buckets;

    public Day_2021_03(string inputData)
    {
        this.buckets = BucketInput(inputData);
    }

    public string Part1()
    {
        StringBuilder gammaBuilder = new();
        StringBuilder epsilonBuilder = new();

        for (int i = 0; i < buckets.Length; i++)
        {
            int ones = buckets[i].Ones.Count;
            int zeros = buckets[i].Zeros.Count;

            if (ones > zeros)
            {
                gammaBuilder.Append('1');
                epsilonBuilder.Append('0');
            }
            else
            {
                gammaBuilder.Append('0');
                epsilonBuilder.Append('1');
            }
        }

        return $"{gammaBuilder.ToString().ToIntFromBinaryString() * epsilonBuilder.ToString().ToIntFromBinaryString()}";
    }

    public string Part2()
    {
        string oxygen = Filter(buckets, FilterOxygen);
        string co2 = Filter(buckets, FilterCO2);

        return $"{oxygen.ToIntFromBinaryString() * co2.ToIntFromBinaryString()}";
    }

    private static string Filter((HashSet<string> Ones, HashSet<string> Zeros)[] buckets, Func<HashSet<string>, HashSet<string>, HashSet<string>> filter)
    {
        var remaining = new HashSet<string>(buckets[0].Ones);
        remaining.UnionWith(buckets[0].Zeros);

        for (int i = 0; i < buckets.Length; i++)
        {
            var remainingOnes = new HashSet<string>(remaining);
            remainingOnes.IntersectWith(buckets[i].Ones);
            var remainingZeros = new HashSet<string>(remaining);
            remainingZeros.IntersectWith(buckets[i].Zeros);

            remaining = filter(remainingOnes, remainingZeros);

            if (remaining.Count == 1)
            {
                break;
            }
        }

        return remaining.Single();
    }

    private HashSet<string> FilterOxygen(HashSet<string> remainingOnes, HashSet<string> remainingZeros)
    {
        if (remainingZeros.Count > remainingOnes.Count)
        {
            return remainingZeros;
        }
        else
        {
            return remainingOnes;
        }
    }

    private HashSet<string> FilterCO2(HashSet<string> remainingOnes, HashSet<string> remainingZeros)
    {
        if (remainingOnes.Count < remainingZeros.Count)
        {
            return remainingOnes;
        }
        else
        {
            return remainingZeros;
        }
    }

    private static (HashSet<string> Ones, HashSet<string> Zeros)[] BucketInput(string data)
    {
        string[] lines = data.StringsForDay();
        (HashSet<string> Ones, HashSet<string> Zeros)[] entryAtIndex = new (HashSet<string> Ones, HashSet<string> Zeros)[lines[0].Length];

        for (int i = 0; i < entryAtIndex.Length; i++)
        {
            entryAtIndex[i] = (new HashSet<string>(), new HashSet<string>());
        }

        foreach (string line in lines)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '1')
                {
                    entryAtIndex[i].Ones.Add(line);
                }
                else
                {
                    entryAtIndex[i].Zeros.Add(line);
                }
            }
        }

        return entryAtIndex;
    }
}