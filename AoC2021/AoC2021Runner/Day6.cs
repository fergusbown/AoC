using System.Drawing;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner
{
    internal class Day6 : IDayChallenge
    {
        public string Part1()
        {
            return LanternFish.SimulateDay(inputData, 80).ToString();
        }

        public string Part2()
        {
            return LanternFish.SimulateDay(inputData, 256).ToString();
        }

        private const string exampleData = @"3,4,3,1,2";

        private const string inputData = @"2,5,5,3,2,2,5,1,4,5,2,1,5,5,1,2,3,3,4,1,4,1,4,4,2,1,5,5,3,5,4,3,4,1,5,4,1,5,5,5,4,3,1,2,1,5,1,4,4,1,4,1,3,1,1,1,3,1,1,2,1,3,1,1,1,2,3,5,5,3,2,3,3,2,2,1,3,1,3,1,5,5,1,2,3,2,1,1,2,1,2,1,2,2,1,3,5,4,3,3,2,2,3,1,4,2,2,1,3,4,5,4,2,5,4,1,2,1,3,5,3,3,5,4,1,1,5,2,4,4,1,2,2,5,5,3,1,2,4,3,3,1,4,2,5,1,5,1,2,1,1,1,1,3,5,5,1,5,5,1,2,2,1,2,1,2,1,2,1,4,5,1,2,4,3,3,3,1,5,3,2,2,1,4,2,4,2,3,2,5,1,5,1,1,1,3,1,1,3,5,4,2,5,3,2,2,1,4,5,1,3,2,5,1,2,1,4,1,5,5,1,2,2,1,2,4,5,3,3,1,4,4,3,1,4,2,4,4,3,4,1,4,5,3,1,4,2,2,3,4,4,4,1,4,3,1,3,4,5,1,5,4,4,4,5,5,5,2,1,3,4,3,2,5,3,1,3,2,2,3,1,4,5,3,5,5,3,2,3,1,2,5,2,1,3,1,1,1,5,1";

        private class LanternFish
        {
            LinkedList<long> countInState;

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

            public static long SimulateDay(string input, int days)
            {
                var fish = new LanternFish(input.Split(',').Select(v => int.Parse(v)).ToArray());

                return fish.MoveForward(days);
            }
        }
    }
}
