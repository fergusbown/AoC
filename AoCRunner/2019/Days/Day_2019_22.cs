namespace AoCRunner;

internal class Day_2019_22 : IDayChallenge
{
    private readonly string inputData;

    public Day_2019_22(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var card = ShuffleForward(10007, 2019, this.inputData);

        return card.ToString();
    }

    public string Part2()
    {
        var card = ShuffleBackward(119315717514047, 2020, this.inputData);

        return card.ToString();
    }


    private static int ShuffleForward(int deckSize, int cardToShuffle, string inputData)
    {
        var steps = inputData.StringsForDay();

        return steps
            .Select(s => GetShuffleStep(s, deckSize))
            .Aggregate(cardToShuffle, (c, ss) => ss(c));

        static Func<int, int> GetShuffleStep(string step, int deckSize)
        {
            return Parse(step) switch
            {
                (Shuffle.NewStack, _) => i => deckSize - i - 1,
                (Shuffle.Cut, int number) => i =>(deckSize + i - number) % deckSize,
                (Shuffle.DealWithIncrement, int increment) => i => (increment * i) % deckSize,
                _ => throw new ArgumentOutOfRangeException(nameof(step)),
            };
        }
    }

    private static long ShuffleBackward(long deckSize, long cardToShuffle, string inputData)
    {
        var steps = inputData.StringsForDay();
        var shuffleSteps = steps
            .Select(s => GetShuffleStep(s, deckSize))
            .Reverse()
            .ToArray();

        var card = cardToShuffle;

        // work out how many shuffles get you back to where you started:
        for (long i = 0; i < 101741582076661; i++)
        {
            card = DoShuffle(shuffleSteps, card);

            if (card == cardToShuffle)
            {
                break;
            }
        }

        /* int remainingShufflesRequired = (int)(101741582076661 % shufflesToReset);

        for (int i = 0; i < remainingShufflesRequired; i++)
        {
            card = DoShuffle(shuffleSteps, card);
        }
        */

        return card;

        static long DoShuffle(IReadOnlyList<Func<long, long>> steps, long cardToShuffle)
        {
            var card = cardToShuffle;

            foreach (var shuffleStep in steps)
            {
                card = shuffleStep(card);
            }

            return card;
        }

        static Func<long, long> GetShuffleStep(string step, long deckSize)
        {
            return Parse(step) switch
            {
                (Shuffle.NewStack, _) => i => deckSize - i - 1,
                (Shuffle.Cut, int number) => i => (deckSize + i + number) % deckSize,
                (Shuffle.DealWithIncrement, int increment) => i => Factorisation.ModuloDivide(i, increment, deckSize)!.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(step)),
            };
        }
    }

    private static (Shuffle Shuffle, int Amount) Parse(string step)
    {
        if (step == "deal into new stack")
        {
            return (Shuffle.NewStack, 0);
        }
        else if (step.StartsWith("cut "))
        {
            return (Shuffle.Cut, int.Parse(step[4..]));
        }
        else if (step.StartsWith("deal with increment "))
        {
            return (Shuffle.DealWithIncrement, int.Parse(step[20..]));
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(step));
        }
    }


    private enum Shuffle
    {
        NewStack,
        Cut,
        DealWithIncrement
    }
}
