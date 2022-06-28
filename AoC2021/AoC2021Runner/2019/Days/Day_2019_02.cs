﻿namespace AoC2021Runner;

internal class Day_2019_02 : IAsyncDayChallenge
{
    private readonly string inputData;

    public Day_2019_02(string inputData)
    {
        this.inputData = inputData;
    }

    public async Task<string> Part1()
    {
        var computer = new IntCodeComputer();
        int[] program = computer.GetProgram(inputData);

        int result = await computer.Run(program, 12, 2);

        return result.ToString();
    }

    public async Task<string> Part2()
    {
        var computer = new IntCodeComputer();
        int[] startingProgram = computer.GetProgram(inputData);
        int[] currentProgram = new int[startingProgram.Length];

        for (int noun = 0; noun < 100; noun++)
        {
            for (int verb = 0; verb < 100; verb++)
            {
                startingProgram.CopyTo(currentProgram, 0);
                try
                {
                    if (await computer.Run(currentProgram, noun, verb) == 19690720)
                    {
                        return $"{100 * noun + verb}";
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }
            }
        }

        return "Oops";
    }
}