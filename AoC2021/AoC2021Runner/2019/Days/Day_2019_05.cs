namespace AoC2021Runner;

internal partial class Day_2019_05 : IAsyncDayChallenge
{
    private readonly string inputData;

    public Day_2019_05(string inputData)
    {
        this.inputData = inputData;
    }

    public async Task<string> Part1()
    {
        var computer = new IntCodeComputer(1);

        int result = await computer.Run(computer.GetProgram(this.inputData)) ?? throw new InvalidOperationException();

        return result.ToString();
    }

    public async Task<string> Part2()
    {
        var computer = new IntCodeComputer(5);

        int result = await computer.Run(computer.GetProgram(this.inputData)) ?? throw new InvalidOperationException();

        return result.ToString();
    }
}