namespace AoCRunner;

internal partial class Day_2019_05 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_05(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        long result = await IntCodeComputer.New().AddInput(1).Run(this.inputData);
        return result.ToString();
    }

    public async Task<string> Part2()
    {
        long result = await IntCodeComputer.New().AddInput(5).Run(this.inputData);
        return result.ToString();
    }
}