namespace AoC2021Runner;

internal class Day_2021_07 : IDayChallenge
{
    private readonly int[] positions;

    public Day_2021_07(string inputData)
    {
        this.positions = inputData.Split(',').Select(p => int.Parse(p)).ToArray();
    }

    public string Part1()
    {
        return BruteForceIt(positions, d => d).ToString();
    }

    public string Part2()
    {
        return BruteForceIt(positions, d => ((d + 1) * d) / 2).ToString();
    }

    private static int BruteForceIt(int[] positions, Func<int, int> costToMoveDistance)
    {
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