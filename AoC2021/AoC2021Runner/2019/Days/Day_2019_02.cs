namespace AoC2021Runner;

internal class Day_2019_02 : IDayChallenge
{
    private readonly string inputData;

    public Day_2019_02(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        int[] program = GetStartingProgram(inputData);

        return (RunProgram(program, 12, 2) ?? throw new InvalidOperationException()).ToString();
    }

    public string Part2()
    {
        int[] startingProgram = GetStartingProgram(inputData);
        int[] currentProgram = new int[startingProgram.Length];

        for (int noun = 0; noun < 100; noun++)
        {
            for (int verb = 0; verb < 100; verb++)
            {
                startingProgram.CopyTo(currentProgram, 0);
                if (RunProgram(currentProgram, noun, verb) == 19690720)
                {
                    return $"{100 * noun + verb}";
                }
            }
        }

        return "Oops";
    }

    private static int? RunProgram(int[] program, int noun, int verb)
    {
        try
        {
            program[1] = noun;
            program[2] = verb;

            int index = 0;
            int opCode = program[index++];

            while (opCode != 99)
            {
                int operand1 = program[program[index++]];
                int operand2 = program[program[index++]];
                int outputLocation = program[index++];

                int result = opCode == 1 ? operand1 + operand2 : operand1 * operand2;

                program[outputLocation] = result;

                opCode = program[index++];
            }

            return program[0];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    private static int[] GetStartingProgram(string inputData) 
        => inputData.Split(',').Select(i => int.Parse(i)).ToArray();
}