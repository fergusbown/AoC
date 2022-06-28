using MoreLinq;

namespace AoC2021Runner;

internal partial class Day_2019_07 : IAsyncDayChallenge
{
    private readonly string inputData;

    public Day_2019_07(string inputData)
    {
        this.inputData = inputData;
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

        static async Task<int> GetThrust(IList<int> phases, string inputData)
        {
            var computers = phases
                .Select(p => new IntCodeComputer(p))
                .ToArray();

            for (int i = 0; i < computers.Length - 1; i++)
            {
                computers[i].PipeOutputTo(computers[i + 1]);
            }

            computers[^1].PipeOutputTo(computers[0]);

            computers[0].AddInput(0);
            int[] results = await Task.WhenAll(computers.Select(c => c.Run(c.GetProgram(inputData))));

            return results[^1];
        }
    }
}
