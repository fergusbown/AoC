namespace AoCRunner;
internal class Day_2021_14 : IDayChallenge
{
    private readonly Polymer startingPolymer;
    public Day_2021_14(string inputData)
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

        public long Strength => ElementCounts.Values.Max() - ElementCounts.Values.Where(c => c > 0).Min();

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
                    // parse from "AB -> C"
                    ((char, char) SourcePair, char InsertionCharacter) tarnsitionRule = ((x[0], x[1]), x[6]);

                    // ensure any element we may see is pre-added for simplicity
                    _ = elementCounts.TryAdd(tarnsitionRule.SourcePair.Item1, 0);
                    _ = elementCounts.TryAdd(tarnsitionRule.SourcePair.Item2, 0);
                    _ = elementCounts.TryAdd(tarnsitionRule.InsertionCharacter, 0);

                    // ensure any pair we may see is pre-added for simplicity
                    _ = pairCounts.TryAdd(tarnsitionRule.SourcePair, 0);
                    _ = pairCounts.TryAdd((tarnsitionRule.SourcePair.Item1, tarnsitionRule.InsertionCharacter), 0);
                    _ = pairCounts.TryAdd((tarnsitionRule.InsertionCharacter, tarnsitionRule.SourcePair.Item2), 0);

                    return tarnsitionRule;
                })
                .ToDictionary(r => r.SourcePair, r => r.InsertionCharacter);

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
