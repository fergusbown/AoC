namespace AoC2021Runner;

internal class Day_2020_13 : IDayChallenge
{
    private readonly int earliestArrivalTime;
    private readonly IReadOnlyCollection<(int bus, int index)> buses;

    public Day_2020_13(string inputData)
    {
        var lines = inputData.StringsForDay();

        var timetable = lines[1].Split(',');

        List<(int bus, int index)> buses = new();

        for (int i = 0; i < timetable.Length; i++)
        {
            if (int.TryParse(timetable[i], out int bus))
            {
                buses.Add((bus, i));
            }
        }

        this.earliestArrivalTime = int.Parse(lines[0]);
        this.buses = buses;
    }

    public string Part1()
    {
        int minWaitTime = int.MaxValue;
        int nearestBus = 0;

        foreach ((int bus, _) in buses)
        {
            int waitTime = bus - (earliestArrivalTime % bus);

            if (waitTime < minWaitTime)
            {
                minWaitTime = waitTime;
                nearestBus = bus;
            }
        }

        return $"{minWaitTime * nearestBus}";
    }

    public string Part2()
    {
        // this broke my head, so see:
        // https://0xdf.gitlab.io/adventofcode2020/13
        // https://rosettacode.org/wiki/Chinese_remainder_theorem#C.23

        List<int> buses = new();
        List<int> offsetBuses = new();

        foreach ((var bus, var index) in this.buses)
        {
            buses.Add(bus);
            offsetBuses.Add(bus - index);
        }

        return $"{ChineseRemainderTheorem.Solve(buses, offsetBuses)}";
    }
}