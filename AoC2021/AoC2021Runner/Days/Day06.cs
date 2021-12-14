using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner;

internal class Day06 : IDayChallenge
{
    private readonly IReadOnlyCollection<int> initialState;

    public Day06(string inputData)
    {
        this.initialState = inputData.Split(',').Select(v => int.Parse(v)).ToArray();
    }

    public string Part1()
    {
        return LanternFish.SimulateDay(initialState, 80).ToString();
    }

    public string Part2()
    {
        return LanternFish.SimulateDay(initialState, 256).ToString();
    }

    private class LanternFish
    {
        readonly LinkedList<long> countInState;

        public LanternFish(IReadOnlyCollection<int> initialState)
        {
            long[] initialStateCount = new long[9];

            foreach (var group in initialState.GroupBy(s => s))
            {
                initialStateCount[group.Key] = group.Count();
            }

            this.countInState = new LinkedList<long>(initialStateCount);
        }

        public long MoveForward(int days)
        {
            for (int day = 0; day < days; day++)
            {
                //spawn as many new fish as are at zero
                var spawningFish = this.countInState.First!.Value;

                //add that many new fish at max spawn duration (so the last)
                this.countInState.AddLast(spawningFish);

                // move all fish forward a day
                this.countInState.RemoveFirst();

                //add spawning fish back at stage 6;
                this.countInState.Last!.Previous!.Previous!.Value += spawningFish;
            }

            return this.countInState.Sum();
        }

        public static long SimulateDay(IReadOnlyCollection<int> input, int days)
        {
            var fish = new LanternFish(input);
            return fish.MoveForward(days);
        }
    }
}