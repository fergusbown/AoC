using System.Collections.Immutable;

namespace AoC2021Runner;

internal partial class Day_2020_23 : IDayChallenge
{
    private readonly int[] inputData;

    public Day_2020_23()
    {
        this.inputData = "974618352".Select(c => c - '0').ToArray();
    }

    public string Part1()
    {
        Cups cups = new Cups(this.inputData);
        cups.Play(100);

        Cups.Cup one = cups[1];
        Cups.Cup current = one.ClockwiseCup;

        int result = 0;
        while (current != one)
        {
            result = result * 10 + current.Value;
            current = current.ClockwiseCup;
        }

        return result.ToString();
    }

    public string Part2()
    {
        Cups cups = new Cups(inputData.Concat(Enumerable.Range(10, 999_991)));
        cups.Play(10_000_000);

        var firstCup = cups[1].ClockwiseCup;
        var secondCup = firstCup.ClockwiseCup;
        long result = (long)firstCup.Value * (long)secondCup.Value;

        return result.ToString();
    }

    private class Cups
    {
        private readonly ImmutableDictionary<int, Cup> itemsDictionary;
        private readonly Cup biggestCup;
        private readonly Cup smallestCup;
        private Cup currentCup;

        public Cups(IEnumerable<int> values)
        {
            var items = values
                .Select(v => new Cup(v))
                .ToArray();

            currentCup = items[0];

            for (int i = 0; i < items.Length - 1; i++)
            {
                items[i].SetClockwiseCup(items[i + 1]);
            }

            items[^1].SetClockwiseCup(items[0]);

            this.itemsDictionary = items.ToImmutableDictionary(k => k.Value, v => v);
            biggestCup = this.itemsDictionary[values.Max()];
            smallestCup = this.itemsDictionary[values.Min()];
        }

        public Cup this[int value] => this.itemsDictionary[value];

        public void Play(int rounds)
        {
            for (int i = 0; i < rounds; i++)
            {
                Play();
            }

            void Play()
            {
                var extractedFirst = currentCup.ClockwiseCup;
                var extractedSecond = extractedFirst.ClockwiseCup;
                var extractedThird = extractedSecond.ClockwiseCup;

                currentCup.SetClockwiseCup(extractedThird.ClockwiseCup);

                var destinationCup = currentCup;

                do
                {
                    destinationCup = destinationCup == smallestCup ? biggestCup : this[destinationCup.Value - 1];
                }
                while (destinationCup == extractedFirst || destinationCup == extractedSecond || destinationCup == extractedThird);

                var clockwiseOfDestination = destinationCup.ClockwiseCup;

                destinationCup.SetClockwiseCup(extractedFirst);
                extractedThird.SetClockwiseCup(clockwiseOfDestination);

                currentCup = currentCup.ClockwiseCup;
            }
        }

        public class Cup
        {
            public Cup(int value)
            {
                Value = value;
            }

            public Cup AntiClockwiseCup { get; private set; } = null!;

            public Cup ClockwiseCup { get; private set; } = null!;

            public void SetClockwiseCup(Cup cup)
            {
                this.ClockwiseCup = cup;
                cup.AntiClockwiseCup = this;
            }

            public int Value { get; }
        }
    }
}
