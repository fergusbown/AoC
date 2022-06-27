namespace AoC2021Runner;

internal class Day_2019_01 : IDayChallenge
{
    private readonly int[] inputData;

    public Day_2019_01(string inputData)
    {
        this.inputData = inputData.IntsForDay();
    }

    public string Part1()
    {
        return this.inputData.Select(FuelRequired).Sum().ToString();
    }

    public string Part2()
    {
        return this.inputData.Select(TotalFuelRequired).Sum().ToString();
    }

    private static int FuelRequired(int moduleWeight)
        => (moduleWeight / 3) - 2;

    private static int TotalFuelRequired(int moduleWeight)
    {
        int lastFuel = FuelRequired(moduleWeight);
        int totalFuel = 0;

        while (lastFuel > 0)
        {
            totalFuel += lastFuel;
            lastFuel = FuelRequired(lastFuel);
        }

        return totalFuel;
    }
}