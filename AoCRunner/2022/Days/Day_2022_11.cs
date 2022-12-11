namespace AoCRunner;

internal class Day_2022_11 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_11(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
        => Play(inputData, 20, true);

    public string Part2()
        => Play(inputData, 10_000, false);

    private static string Play(string inputData, int rounds, bool divideByThree)
    {
        var monkeys = Parse(inputData, divideByThree);

        for (int i = 0; i < rounds; i++)
        {
            foreach (var monkey in monkeys)
            {
                monkey.PlayWith(monkeys);
            }
        }

        var ringleaders = monkeys
            .OrderByDescending(m => m.Inspections)
            .Take(2)
            .Select(m => m.Inspections)
            .ToArray();

        return $"{ringleaders[0] * ringleaders[1]}";
    }

    private static IReadOnlyList<Monkey> Parse(string inputData, bool divideByThree)
    {
        var monkeyStrings = inputData
            .Split($"{Environment.NewLine}{Environment.NewLine}");

        List<Monkey> result = new(monkeyStrings.Length);

        int multipleOfDivisors = 1;
        foreach (var monkeyString in monkeyStrings)
        {
            var monkeyParts = monkeyString.StringsForDay()
                .Skip(1)
                .Select(m => m[(m.IndexOf(':') + 1)..].Trim())
                .ToArray();

            Monkey monkey = new Monkey(
                ParseItems(monkeyParts[0]),
                ParseOperation(monkeyParts[1]),
                ParseTest(monkeyParts[2]),
                ParseThrow(monkeyParts[3]),
                ParseThrow(monkeyParts[4]));

            multipleOfDivisors *= monkey.DivisibleByTest;
            result.Add(monkey);
        }

        Func<long, long> decorator = divideByThree
            ? x => x / 3
            : x => x % multipleOfDivisors;

        foreach(var monkey in result)
        {
            monkey.Decorate(decorator);
        }

        return result;

        static IEnumerable<long> ParseItems(string value)
            => value.Split(", ").Select(i => long.Parse(i));

        static Func<long, long> ParseOperation(string value)
        {
            var parts = value.Split(' ');

            var arg1 = parts[^3];
            var operand = parts[^2];
            var arg2 = parts[^1];

            long.TryParse(arg1, out long arg1Value);
            long.TryParse(arg2, out long arg2Value);

            if (operand == "*")
            {
                return (arg1, arg2) switch
                {
                    ("old", "old") => x => x * x,
                    ("old", _) => x => x * arg2Value,
                    (_, "old") => x => arg1Value * x,
                    (_, _) => throw new ArgumentOutOfRangeException(nameof(value)),
                };
            }
            else if (operand == "+")
            {
                return (arg1, arg2) switch
                {
                    ("old", "old") => x => x + x,
                    ("old", _) => x => x + arg2Value,
                    (_, "old") => x => arg1Value + x,
                    (_, _) => throw new ArgumentOutOfRangeException(nameof(value)),
                };
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        static int ParseTest(string value)
            => int.Parse(value.Split(' ').Last());

        static int ParseThrow(string value)
            => int.Parse(value[^2..]);
    }

    private class Monkey
    {
        private Func<long, long> operation;

        private int trueMonkey;

        private int falseMonkey;

        public Monkey(
            IEnumerable<long> items,
            Func<long, long> operation,
            int divisibleByTest,
            int trueMonkey,
            int falseMonkey)
        {
            this.Items = new(items);
            this.operation = operation;
            this.DivisibleByTest = divisibleByTest;
            this.trueMonkey = trueMonkey;
            this.falseMonkey = falseMonkey;
        }

        public Queue<long> Items { get; }

        public long Inspections { get; private set; } = 0;

        public int DivisibleByTest { get; }

        public void Decorate(Func<long, long> decorator)
        {
            var orig = operation;
            operation = x => decorator(orig(x));
        }

        public void PlayWith(IReadOnlyList<Monkey> monkeys)
        {
            while (this.Items.TryDequeue(out long item))
            {
                Inspections++;

                item = operation(item);

                bool test = (item % DivisibleByTest) == 0;
                monkeys[test ? trueMonkey : falseMonkey].Items.Enqueue(item);
            }
        }
    }
}
