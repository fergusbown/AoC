namespace AoC2021Runner
{
    internal class Day07 : IDayChallenge
    {
        private readonly string inputData;

        public Day07(string inputData)
        {
            this.inputData = inputData;
        }

        public string Part1()
        {
            return BruteForceIt(inputData, d => d).ToString();
        }

        public string Part2()
        {
            return BruteForceIt(inputData, d => ((d + 1) * d) / 2).ToString();
        }

        private static int BruteForceIt(string input, Func<int, int> costToMoveDistance)
        {
            int[] positions = input.Split(',').Select(p => int.Parse(p)).ToArray();

            int cheapestYet = int.MaxValue;

            for (int testPosition = positions.Min(); testPosition < positions.Max(); testPosition++)
            {
                int cost = 0;

                foreach (var currentPosition in positions)
                {
                    cost += costToMoveDistance(Math.Abs(currentPosition - testPosition));
                }

                if (cost < cheapestYet)
                {
                    cheapestYet = cost;
                }
            }

            return cheapestYet;
        }
    }
}
