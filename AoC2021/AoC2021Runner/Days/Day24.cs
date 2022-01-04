namespace AoC2021Runner;

internal class Day24 : IDayChallenge
{
    private readonly IReadOnlyList<IAluDigit> alus;
    public Day24(string inputData)
    {
        this.alus = new List<IAluDigit>()
        {
            new AluDigitNonTruncating(11, 6),
            new AluDigitNonTruncating(11, 14),
            new AluDigitNonTruncating(15, 13),
            new AluDigitTruncating(-14, 1),
            new AluDigitNonTruncating(10, 6),
            new AluDigitTruncating(0, 13),
            new AluDigitTruncating(-6, 6),
            new AluDigitNonTruncating(13, 3),
            new AluDigitTruncating(-3, 8),
            new AluDigitNonTruncating(13, 3),
            new AluDigitNonTruncating(15, 4),
            new AluDigitTruncating(-2, 7),
            new AluDigitTruncating(-9, 15),
            new AluDigitTruncating(-2, 1),
        };
    }

    public string Part1()
    {
        Dictionary<int, HashSet<long>> zsForDigit = new();
        zsForDigit.Add(0, new HashSet<long> { 0, });

        for (int digit = 0; digit < 14; digit++)
        {
            IAluDigit alu = alus[digit];
            HashSet<long> nextZs = new();
            zsForDigit[digit + 1] = nextZs;

            foreach(var z in zsForDigit[digit])
            {
                for (int input = 1; input <= 9; input++)
                {
                    nextZs.Add(alu.Calculate(z, input));
                }
            }
        }
        return "";
    }

    public string Part2()
        => "";

    private interface IAluDigit
    {
        long Calculate(long z, int input);
    }


    private class AluDigitNonTruncating : IAluDigit
    {
        private readonly int xModifier;
        private readonly int yModifier;

        public AluDigitNonTruncating(int xModifier, int yModifier)
        {
            this.xModifier = xModifier;
            this.yModifier = yModifier;
        }

        public long Calculate(long z, int input)
        {
            var x = (z % 26) + xModifier;

            if (x == input)
            {
                return z;
            }
            else
            {
                return (z * 26) + input + yModifier;
            }
        }
    }

    private class AluDigitTruncating : IAluDigit
    {
        private readonly int xModifier;
        private readonly int yModifier;

        public AluDigitTruncating(int xModifier, int yModifier)
        {
            this.xModifier = xModifier;
            this.yModifier = yModifier;
        }

        public long Calculate(long z, int input)
        {
            var x = (z % 26) + xModifier;

            if (x == input)
            {
                return z / 26;
            }
            else
            {
                return z + input + yModifier;
            }
        }
    }
}
