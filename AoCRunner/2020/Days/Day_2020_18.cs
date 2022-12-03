namespace AoCRunner;

internal class Day_2020_18 : IDayChallenge
{
    private readonly string[] inputData;

    public Day_2020_18(string inputData)
    {
        this.inputData = inputData.StringsForDay();
    }

    public string Part1()
        => $"{this.inputData.Select(l => Solve(l, "+*")).Sum()}";

    public string Part2()
        => $"{this.inputData.Select(l => Solve(l, "+", "*")).Sum()}";

    private static long Solve(string line, params string[] precedence)
    {
        return Solve(line.AsSpan(), precedence);

        static long Solve(ReadOnlySpan<char> tokens, string[] operatorPrecedence)
        {
            // get all of the numbers and operators in order, recursively taking account of brackets
            // by the time things get in these queues, brackets are resolved and irrelevent
            Queue<long> numbers = new();
            Queue<char> operators = new();

            numbers.Enqueue(ReadNextNumber(ref tokens, operatorPrecedence));
            
            while (!tokens.IsEmpty)
            {
                operators.Enqueue(ReadNextOperator(ref tokens));
                numbers.Enqueue(ReadNextNumber(ref tokens, operatorPrecedence));
            }

            // apply the numbers and operators in order of precedence, or left to right
            // if operators have the same precedence
            // once pass for each precedence level
            foreach(string operatorsToApply in operatorPrecedence)
            {
                int operatorCount = operators.Count;

                if (operatorCount > 0)
                {
                    long result = numbers.Dequeue();

                    for (int i = 0; i < operatorCount; i++)
                    {
                        var right = numbers.Dequeue();
                        var op = operators.Dequeue();

                        if (operatorsToApply.Contains(op))
                        {
                            // We are ready for this operator, just run it
                            result = Apply(result, op, right);
                        }
                        else
                        {
                            // Not ready to run this operator yet
                            // put the current result and operator on the queue for the next pass
                            // and start a new calculation
                            numbers.Enqueue(result);
                            operators.Enqueue(op);
                            result = right;
                        }
                    }

                    // the current result is either the answer, or the right hand side of a pending operator
                    // either way, requeue it
                    numbers.Enqueue(result);
                }
            }

            return numbers.Dequeue();
        }

        static long ReadNextNumber(ref ReadOnlySpan<char> tokens, string[] operatorPrecedence)
        {
            int index = 0;

            while (true)
            {
                char token = tokens[index++];
                switch (token)
                {
                    case ' ':
                        break;
                    case '(':
                        int brackets = 1;
                        int startIndex = index;

                        while (brackets > 0)
                        {
                            brackets += tokens[index++] switch
                            {
                                '(' => 1,
                                ')' => -1,
                                _ => 0,
                            };
                        }

                        long result = Solve(tokens[startIndex..(index-1)], operatorPrecedence);
                        tokens = tokens[index..];
                        return result;
                    default:
                        tokens = tokens[index..];
                        return token - '0';
                }
            }
        }

        static char ReadNextOperator(ref ReadOnlySpan<char> tokens)
        {
            int index = 0;

            while (true)
            {
                char token = tokens[index++];
                switch (token)
                {
                    case '*':
                    case '+':
                        tokens = tokens[index..];
                        return token;
                    default:
                        break;
                }
            }
        }

        static long Apply(long left, char @operator, long right)
        {
            return @operator switch
            {
                '+' => left + right,
                '*' => left * right,
                _ => throw new ArgumentOutOfRangeException(nameof(@operator)),
            };
        }
    }

}
