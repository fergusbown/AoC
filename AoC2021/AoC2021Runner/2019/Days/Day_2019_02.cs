namespace AoC2021Runner;

internal class Day_2019_02 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_02(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        long result = await IntCodeComputer.New().Run(inputData, 12, 2);
        return result.ToString();
    }

    public async Task<string> Part2()
    {
        var computer = new IntCodeComputer();

        for (int noun = 0; noun < 100; noun++)
        {
            for (int verb = 0; verb < 100; verb++)
            {
                try
                {
                    if (await computer.Run(inputData, noun, verb) == 19690720)
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