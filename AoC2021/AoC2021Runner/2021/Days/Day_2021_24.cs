namespace AoC2021Runner;

internal class Day_2021_24 : IDayChallenge
{
    private readonly IReadOnlyList<IAluDigit> alus;
    private string lowest = string.Empty;
    private string highest = string.Empty;

    public Day_2021_24(string inputData)
    {
        var inputLines = inputData.StringsForDay();
        var loadedAlus = new List<IAluDigit>();
        
        for (int startIndex = 0; startIndex < inputLines.Length; startIndex+=18)
        {
            bool truncate = inputLines[startIndex + 4] == "div z 26";
            int xModifier = int.Parse(inputLines[startIndex + 5][6..]);
            int yModifier = int.Parse(inputLines[startIndex + 15][6..]);

            if (truncate)
            {
                loadedAlus.Add(new AluDigitTruncating(xModifier, yModifier));
            }
            else
            {
                loadedAlus.Add(new AluDigitNonTruncating(xModifier, yModifier));
            }
        }

        this.alus = loadedAlus;
    }

    public string Part1()
    {
        Solve();
        return highest;
    }

    public string Part2()
        => lowest;

    private void Solve()
    {
        HashSet<long>[] zsForDigit = new HashSet<long>[14];
        zsForDigit[0] = new HashSet<long> { 0 };

        Console.WriteLine($"Working out all input states:");

        for (int digit = 0; digit < 13; digit++)
        {
            IAluDigit alu = alus[digit];
            HashSet<long> nextZs = new();
            zsForDigit[digit + 1] = nextZs;

            Console.WriteLine($"  Handling {zsForDigit[digit].Count} input states for digit {digit}");

            foreach (var z in zsForDigit[digit])
            {
                for (int input = 1; input <= 9; input++)
                {
                    nextZs.Add(alu.Calculate(z, input));
                }
            }
        }

        Console.WriteLine($"Working out valid output states:");

        HashSet<long>[] endZsForDigit = new HashSet<long>[14];
        endZsForDigit[13] = new HashSet<long> { 0 };

        for (int digit = 13; digit > 0; digit--)
        {
            IAluDigit alu = alus[digit];
            HashSet<long> previousZs = new();
            endZsForDigit[digit - 1] = previousZs;
            var endZs = endZsForDigit[digit];

            Console.WriteLine($"  Handling {zsForDigit[digit].Count} input states for digit {digit}");

            foreach (var z in zsForDigit[digit])
            {
                for (int input = 1; input <= 9; input++)
                {
                    if (endZs.Contains(alu.Calculate(z, input)))
                    {
                        previousZs.Add(z);
                    }
                }
            }
        }

        List<int> result = new();
        HashSet<long>[] validatedZsForDigit = new HashSet<long>[15];
        validatedZsForDigit[0] = new HashSet<long> { 0, };

        for (int digit = 0; digit < 14; digit++)
        {
            IAluDigit alu = alus[digit];
            HashSet<long> nextZs = new();
            validatedZsForDigit[digit + 1] = nextZs;
            var endZs = endZsForDigit[digit];

            for (int input = 9; input >= 1; input--)
            {
                foreach (var z in validatedZsForDigit[digit])
                {
                    var newZ = alu.Calculate(z, input);
                    if (endZs.Contains(newZ))
                    {
                        nextZs.Add(newZ);
                    }
                }

                if (nextZs.Count > 0)
                {
                    result.Add(input);
                    break;
                }
            }
        }

        this.highest = String.Join("", result);

        result = new();
        validatedZsForDigit = new HashSet<long>[15];
        validatedZsForDigit[0] = new HashSet<long> { 0, };

        for (int digit = 0; digit < 14; digit++)
        {
            IAluDigit alu = alus[digit];
            HashSet<long> nextZs = new();
            validatedZsForDigit[digit + 1] = nextZs;
            var endZs = endZsForDigit[digit];

            for (int input = 1; input <= 9; input++)
            {
                foreach (var z in validatedZsForDigit[digit])
                {
                    var newZ = alu.Calculate(z, input);
                    if (endZs.Contains(newZ))
                    {
                        nextZs.Add(newZ);
                    }
                }

                if (nextZs.Count > 0)
                {
                    result.Add(input);
                    break;
                }
            }
        }

        this.lowest = String.Join("", result);
    }


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
