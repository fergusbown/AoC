namespace AoC2021Runner;
internal class Day14 : IDayChallenge
{
    private readonly Polymer startingPolymer;
    public Day14(string inputData)
    {
        this.startingPolymer = new Polymer(inputData);
    }

    public string Part1()
    {
        return startingPolymer.Step(10).Strength.ToString();
    }

    public string Part2()
    {
        return startingPolymer.Step(40).Strength.ToString();
    }

    private class Polymer
    {
        public IReadOnlyDictionary<char, long> ElementCounts { get; }
        public IReadOnlyDictionary<(char, char), long> PairCounts { get; }
        public IReadOnlyDictionary<(char, char), char> PairTransitions { get; }

        public long Strength => ElementCounts.Values.Max() - ElementCounts.Values.Min();

        public Polymer(string input)
        {
            var lines = input.StringsForDay();
            var startingPolymer = lines[0];
            Dictionary<char, long> elementCounts = new();

            foreach (var element in startingPolymer)
            {
                if (!elementCounts.TryAdd(element, 1))
                {
                    elementCounts[element] += 1;
                }
            }

            Dictionary<(char, char), long> pairCounts = new();
            for (int i = 0; i < startingPolymer.Length - 1; i++)
            {
                var pair = (startingPolymer[i], startingPolymer[i + 1]);

                if (!pairCounts.TryAdd(pair, 1))
                {
                    pairCounts[pair] += 1;
                }

            }

            this.PairTransitions = lines
                .Skip(2)
                .Select(x =>
                {
                    var rule = x.Split(" -> ");
                    var result = ((rule[0][0], rule[0][1]), rule[1][0]);

                        //ensure any element we may see is pre-added for simplicity
                        _ = elementCounts.TryAdd(result.Item1.Item1, 0);
                    _ = elementCounts.TryAdd(result.Item1.Item2, 0);
                    _ = elementCounts.TryAdd(result.Item2, 0);

                        //ensure any pair we may see is pre-added for simplicity
                        _ = pairCounts.TryAdd(result.Item1, 0);
                    _ = pairCounts.TryAdd((result.Item1.Item1, result.Item2), 0);
                    _ = pairCounts.TryAdd((result.Item2, result.Item1.Item2), 0);

                    return result;
                })
                .ToDictionary(r => r.Item1, r => r.Item2);

            this.ElementCounts = elementCounts;
            this.PairCounts = pairCounts;
        }

        private Polymer(Polymer previous)
        {
            this.PairTransitions = previous.PairTransitions;

            Dictionary<char, long> elementCounts = new(previous.ElementCounts);
            Dictionary<(char, char), long> pairCounts = new(previous.PairCounts);

            foreach ((var pair, var inserted) in previous.PairTransitions)
            {
                long pairCount = previous.PairCounts[pair];
                elementCounts[inserted] += pairCount;
                pairCounts[pair] -= pairCount;
                pairCounts[(pair.Item1, inserted)] += pairCount;
                pairCounts[(inserted, pair.Item2)] += pairCount;
            }

            this.ElementCounts = elementCounts;
            this.PairCounts = pairCounts;
        }

        public Polymer Step(int count)
        {
            Polymer result = this;
            for (int i = 0; i < count; i++)
            {
                result = new Polymer(result);
            }

            return result;
        }
    }

}
