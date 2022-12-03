using MoreLinq;

namespace AoCRunner;

internal partial class Day_2019_07 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_07(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public Task<string> Part1()
        => Solve(Enumerable.Range(0, 5));

    public Task<string> Part2()
        => Solve(Enumerable.Range(5, 5));

    private async Task<string> Solve(IEnumerable<int> phases)
    {
        var thrusts = await Task.WhenAll(phases
            .Permutations()
            .Select(p => GetThrust(p, this.inputData)));

        return thrusts.Max().ToString();

        static async Task<long> GetThrust(IList<int> phases, long[] inputData)
        {
            var computers = phases
                .Select(p => IntCodeComputer.New().AddInput(p))
                .ToArray();

            for (int i = 0; i < computers.Length - 1; i++)
            {
                computers[i].PipeOutputTo(computers[i + 1]);
            }

            computers[^1].PipeOutputTo(computers[0]);

            computers[0].AddInput(0);
            long[] results = await Task.WhenAll(computers.Select(c => c.Run(inputData)));

            return results[^1];
        }
    }
}
