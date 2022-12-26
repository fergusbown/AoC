namespace AoCRunner;

internal class Day_2022_21 : IDayChallenge
{
    private readonly string inputData;
    private const string Root = "root";
    private const string Human = "humn";

    public Day_2022_21(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (Dictionary<string, long> solvedMonkeys, Dictionary<string, MathsMonkey> mathsMonkeys) = Parse(inputData);
        TrySolve(solvedMonkeys, mathsMonkeys, Root);

        return solvedMonkeys[Root].ToString();
    }

    public string Part2()
    {
        (Dictionary<string, long> solvedMonkeys, Dictionary<string, MathsMonkey> mathsMonkeys) = Parse(inputData);
        solvedMonkeys.Remove(Human);

        var root = mathsMonkeys[Root];

        TrySolve(solvedMonkeys, mathsMonkeys, root.Input1);
        TrySolve(solvedMonkeys, mathsMonkeys, root.Input2);

        (string target, long targetVaue) = solvedMonkeys.TryGetValue(root.Input1, out long solved)
            ? (root.Input2, solved)
            : (root.Input1, solvedMonkeys[root.Input2]);

        return SolveMissingInput(solvedMonkeys, mathsMonkeys, target, targetVaue);
    }

    private static (Dictionary<string, long> NumberMonkeys, Dictionary<string, MathsMonkey> MathsMonkeys) Parse(string inputData)
    {
        Dictionary<string, long> numberMonkeys = new();
        Dictionary<string, MathsMonkey> mathsMonkeys = new();

        foreach(string[] monkey in inputData.Replace(":", "").StringsForDay().Select(s => s.Split(' ')))
        {
            if (monkey.Length == 2)
            {
                numberMonkeys.Add(monkey[0], int.Parse(monkey[1]));
            }
            else
            {
                mathsMonkeys.Add(monkey[0], new MathsMonkey(
                    monkey[0],
                    monkey[1],
                    (MathsOperation)monkey[2][0],
                    monkey[3]));
            }
        }

        return (numberMonkeys, mathsMonkeys);
    }

    private static void TrySolve(
        Dictionary<string, long> solvedMonkeys,
        Dictionary<string, MathsMonkey> mathsMonkeys,
        string targetMonkey)
    {
        if (!mathsMonkeys.TryGetValue(targetMonkey, out var target))
        {
            return;
        }

        Stack<MathsMonkey> pending = new Stack<MathsMonkey>();
        pending.Push(target);
        HashSet<string> seen = new HashSet<string>();

        while (pending.TryPeek(out var pendingMonkey))
        {
            if (solvedMonkeys.TryGetValue(pendingMonkey.Input1, out long input1)
                && solvedMonkeys.TryGetValue(pendingMonkey.Input2, out long input2))
            {
                pending.Pop();

                if (!solvedMonkeys.ContainsKey(pendingMonkey.Name))
                {
                    solvedMonkeys[pendingMonkey.Name] = pendingMonkey.Calculate(input1, input2);
                }

                continue;
            }

            if (InputSoluble(pendingMonkey.Input1) && InputSoluble(pendingMonkey.Input2))
            {
                continue;
            }

            pending.Pop();
        }

        bool InputSoluble(string input)
        {
            if (solvedMonkeys.ContainsKey(input))
            {
                return true;
            }

            if (mathsMonkeys.TryGetValue(input, out var monkey))
            {
                if (seen.Add(input))
                {
                    pending.Push(monkey);
                    return true;
                }
            }

            return false;
        }
    }

    private static string SolveMissingInput(
        Dictionary<string, long> solvedMonkeys,
        Dictionary<string, MathsMonkey> mathsMonkeys,
        string targetMonkey,
        long targetValue)
    {
        solvedMonkeys[targetMonkey] = targetValue;
        var currentSolveFor = targetValue;
        var currentMonkey = mathsMonkeys[targetMonkey];

        while (true)
        {
            string? nextMonkey;

            if (solvedMonkeys.TryGetValue(currentMonkey.Input1, out long input1))
            {
                solvedMonkeys[currentMonkey.Input2] = currentSolveFor = currentMonkey.Input2FromInput1(input1, currentSolveFor);
                nextMonkey = currentMonkey.Input2;
            }
            else if (solvedMonkeys.TryGetValue(currentMonkey.Input2, out long input2))
            {
                solvedMonkeys[currentMonkey.Input1] = currentSolveFor = currentMonkey.Input1FromInput2(input2, currentSolveFor);
                nextMonkey = currentMonkey.Input1;
            }
            else
            {
                TrySolve(solvedMonkeys, mathsMonkeys, currentMonkey.Input1);
                TrySolve(solvedMonkeys, mathsMonkeys, currentMonkey.Input2);
                continue;
            }

            if (!mathsMonkeys.TryGetValue(nextMonkey, out currentMonkey))
            {
                return currentSolveFor.ToString();
            }
        }
    }

    private enum MathsOperation
    {
        Addition = '+',
        Subtraction = '-',
        Multiplication = '*',
        Division = '/',
    }

    private class MathsMonkey
    {
        public MathsMonkey(string name, string input1, MathsOperation operation, string input2)
        {
            Name = name;
            Input1 = input1;
            Input2 = input2;
            Operation = operation;
        }

        public string Name { get; }

        public string Input1 { get; }

        public string Input2 { get; }

        public MathsOperation Operation { get; }

        public long Calculate(long input1, long input2) 
        {
            return Operation switch
            {
                MathsOperation.Addition => input1 + input2,
                MathsOperation.Subtraction => input1 - input2,
                MathsOperation.Multiplication => input1 * input2,
                MathsOperation.Division => input1 / input2,
                _ => throw new InvalidOperationException(),
            };
        }

        public long Input2FromInput1(long input1, long answer)
        {
            return Operation switch
            {
                MathsOperation.Addition => answer - input1,
                MathsOperation.Subtraction => input1 - answer,
                MathsOperation.Multiplication => answer / input1,
                MathsOperation.Division => input1 / answer,
                _ => throw new InvalidOperationException(),
            };
        }

        public long Input1FromInput2(long input2, long answer)
        {
            return Operation switch
            {
                MathsOperation.Addition => answer - input2,
                MathsOperation.Subtraction => answer + input2,
                MathsOperation.Multiplication => answer / input2,
                MathsOperation.Division => answer * input2,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
