namespace AoC2021Runner;

internal class Day_2021_18 : IDayChallenge
{
    private readonly string inputData;

    public Day_2021_18(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var numbers = GetInput(inputData);

        var result = numbers[0];

        for (int i = 1; i < numbers.Count; i++)
        {
            result = result.Add(numbers[i]);
        }
        return $"{result.Magnitude}";
    }

    public string Part2()
    {
        var numberStrings = inputData.StringsForDay();

        long maxMagnitude = 0;
        for (int i = 0; i < numberStrings.Length; i++)
        {
            for (int j = 0; j < numberStrings.Length; j++)
            {
                if (i != j)
                {
                    var magnitude = GetNumber(numberStrings[i]).Add(GetNumber(numberStrings[j])).Magnitude;

                    if (magnitude > maxMagnitude)
                    {
                        maxMagnitude = magnitude;
                    }
                }
            }
        }

        return maxMagnitude.ToString();
    }

    private static IReadOnlyList<SnailfishNumber> GetInput(string input)
    {
        string[] numberStrings = input.StringsForDay();
        List<SnailfishNumber> numbers = new(numberStrings.Length);

        foreach (var numberString in numberStrings)
        {
            numbers.Add(GetNumber(numberString));
        }
        return numbers;
    }

    private static SnailfishNumber GetNumber(string input)
    {
        Stack<INumber> processing = new();
        foreach (var ch in input)
        {
            switch (ch)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    processing.Push(new RegularNumber(ch - '0'));
                    break;
                case ']':
                    var right = processing.Pop();
                    var left = processing.Pop();
                    var number = new SnailfishNumber(left, right);
                    left.Parent = number;
                    right.Parent = number;
                    processing.Push(number);
                    break;
                default:
                    break;
            }
        }

        return (SnailfishNumber)processing.Pop();
    }

    private interface INumber
    {
        long Magnitude { get; }

        SnailfishNumber? Parent { get; set; }

        RegularNumber? FirstNumberToTheLeft { get; }

        RegularNumber? FirstNumberToTheRight { get; }

        int Depth { get; }
    }

    private abstract class BaseNumber : INumber
    {
        public SnailfishNumber? Parent { get; set; }

        public int Depth
        {
            get
            {
                var parent = Parent;
                int result = 0;

                while (parent is not null)
                {
                    result++;
                    parent = parent.Parent;
                }

                return result;
            }

        }

        public RegularNumber? FirstNumberToTheLeft
        {
            get
            {
                SnailfishNumber? parent = Parent;
                INumber child = this;

                while (parent is not null && parent.Left == child)
                {
                    child = parent;
                    parent = parent.Parent;
                }

                if (parent is null)
                {
                    return null;
                }

                Stack<INumber> search = new();

                search.Push(parent.Left);

                while (search.TryPop(out var test))
                {
                    if (test is RegularNumber regularNumber)
                    {
                        return regularNumber;
                    }
                    else if (test is SnailfishNumber snailfishNumber)
                    {
                        search.Push(snailfishNumber.Left);
                        search.Push(snailfishNumber.Right);
                    }
                }

                return null;
            }
        }

        public RegularNumber? FirstNumberToTheRight
        {
            get
            {
                SnailfishNumber? parent = Parent;
                INumber child = this;

                while (parent is not null && parent.Right == child)
                {
                    child = parent;
                    parent = parent.Parent;
                }

                if (parent is null)
                {
                    return null;
                }

                Stack<INumber> search = new();

                search.Push(parent.Right);

                while (search.TryPop(out var test))
                {
                    if (test is RegularNumber regularNumber)
                    {
                        return regularNumber;
                    }
                    else if (test is SnailfishNumber snailfishNumber)
                    {
                        search.Push(snailfishNumber.Right);
                        search.Push(snailfishNumber.Left);
                    }
                }

                return null;
            }
        }

        public abstract long Magnitude { get; }
    }

    private class SnailfishNumber : BaseNumber
    {
        public SnailfishNumber(INumber left, INumber right)
        {
            Left = left;
            Right = right;
        }

        public override long Magnitude => 3 * Left.Magnitude + 2 * Right.Magnitude;

        public INumber Left { get; set; }

        public INumber Right { get; set; }

        public bool Explode()
        {
            Stack<SnailfishNumber> search = new();

            search.Push(this);

            while (search.TryPop(out var test))
            {
                if (test.Depth == 4)
                {
                    var left = (RegularNumber)test.Left;
                    var right = (RegularNumber)test.Right;
                    var firstNumberToTheLeft = test.FirstNumberToTheLeft;
                    var firstNumberToTheRight = test.FirstNumberToTheRight;

                    if (firstNumberToTheLeft is not null)
                    {
                        firstNumberToTheLeft.Add(left.Magnitude);
                    }

                    if (firstNumberToTheRight is not null)
                    {
                        firstNumberToTheRight.Add(right.Magnitude);
                    }

                    var replacement = new RegularNumber(0)
                    {
                        Parent = test.Parent
                    };

                    if (test.Parent!.Left == test)
                    {
                        test.Parent!.Left = replacement;
                    }
                    else
                    {
                        test.Parent!.Right = replacement;
                    }

                    return true;
                }
                else if (test is SnailfishNumber snailfishNumber)
                {
                    if (snailfishNumber.Right is SnailfishNumber rightSnail)
                    {
                        search.Push(rightSnail);
                    }
                    if (snailfishNumber.Left is SnailfishNumber leftSnail)
                    {
                        search.Push(leftSnail);
                    }
                }
            }

            return false;
        }

        public bool Split()
        {
            Stack<INumber> search = new();

            search.Push(this.Right);
            search.Push(this.Left);

            while (search.TryPop(out var test))
            {
                if (test is RegularNumber regularNumber && regularNumber.Magnitude >= 10)
                {
                    var left = new RegularNumber(regularNumber.Magnitude / 2);
                    var right = new RegularNumber(regularNumber.Magnitude - left.Magnitude);
                    var replacement = new SnailfishNumber(left, right);
                    left.Parent = replacement;
                    right.Parent = replacement;
                    replacement.Parent = test.Parent;

                    if (test.Parent!.Left == test)
                    {
                        test.Parent!.Left = replacement;
                    }
                    else
                    {
                        test.Parent!.Right = replacement;
                    }

                    return true;
                }
                else if (test is SnailfishNumber snailfish)
                {
                    search.Push(snailfish.Right);
                    search.Push(snailfish.Left);
                }
            }

            return false;
        }

        public SnailfishNumber Add(INumber number)
        {
            SnailfishNumber result = new(this, number);
            this.Parent = result;
            number.Parent = result;
            bool finished = false;
            while (!finished)
            {
                if (!result.Explode())
                {
                    finished = !result.Split();
                }
            }

            return result;
        }

        public override string ToString()
            => $"[{Left},{Right}]";
    }

    private class RegularNumber : BaseNumber
    {
        private long magnitude;

        public RegularNumber(long magnitude)
        {
            this.magnitude = magnitude;
        }

        public override long Magnitude => magnitude;

        public void Add(long magnitude)
        {
            this.magnitude += magnitude;
        }

        public override string ToString()
            => Magnitude.ToString();
    }
}
