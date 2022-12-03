namespace AoCRunner;

internal partial class Day_2019_09 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_09(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        long result = await IntCodeComputer
            .New()
            .AddInput(1)
            .Run(inputData);

        return result.ToString();
    }

    public async Task<string> Part2()
    {
        long result = await IntCodeComputer
            .New()
            .AddInput(2)
            .Run(inputData);

        return result.ToString();
    }
}
