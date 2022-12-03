namespace AoCRunner;

internal class Day_2020_08 : IDayChallenge
{
    private readonly (string command, int argument)[] inputData;

    public Day_2020_08(string inputData)
    {
        this.inputData = inputData.StringsForDay()
            .Select(i =>
            {
                var command = i[..3];
                var argument = int.Parse(i[4..]);
                return (command, argument);
            })
            .ToArray();
    }

    public string Part1()
    {
        _ = TryRunProgram(inputData, out int result);
        return $"{result}";
    }

    public string Part2()
    {
        int result = 0;

        for (int i = 0; i < inputData.Length; i++)
        {
            if (TryRunModifiedProgram(inputData, i, out result))
            {
                break;
            }
        }
        return $"{result}";

        static bool TryRunModifiedProgram((string command, int argument)[] program, int indexToChange, out int result)
        {
            (var command, _) = program[indexToChange];
            string replacementCommand;
            
            switch (command)
            {
                case "jmp":
                    replacementCommand = "nop";
                    break;
                case "nop":
                    replacementCommand = "jmp";
                    break;
                default:
                    result = 0;
                    return false;
            }

            program[indexToChange].command = replacementCommand;
            try
            {
                return TryRunProgram(program, out result);
            }
            finally
            {
                program[indexToChange].command = command;
            }
        }
    }

    private static bool TryRunProgram((string command, int argument)[] program, out int result)
    {
        int index = 0;
        result = 0;
        HashSet<int> executed = new();
        while (index < program.Length && index >= 0)
        {
            if (executed.Add(index))
            {
                (var command, var argument) = program[index];

                switch (command)
                {
                    case "acc":
                        result += argument;
                        index++;
                        break;
                    case "jmp":
                        index += argument;
                        break;
                    case "nop":
                        index++;
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected command: {command}");
                }
            }
            else
            {
                return false;
            }
        }

        return executed.Contains(program.Length - 1);
    }
}