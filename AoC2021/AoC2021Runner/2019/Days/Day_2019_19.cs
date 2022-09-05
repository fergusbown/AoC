namespace AoC2021Runner;

internal partial class Day_2019_19 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_19(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        IntCodeComputer computer = new();
        int result = 0;
        computer.PipeOutputTo(HandleOutput);

        for (int y = 0; y < 50; y++)
        {
            for (int x = 0; x < 50; x++)
            {
                computer.AddInput(x);
                computer.AddInput(y);
                await computer.Run(this.inputData);
            }
        }

        return result.ToString();

        void HandleOutput(long value)
            => result += (int)value;
    }

    public async Task<string> Part2()
    {
        long y = 100;
        (long min, long max) = await GetBounds(y, this.inputData);

        List<(long min, long max)> ranges = new(101)
        {
            (min, max),
        };

        y++;

        IntCodeComputer computer = new();
        bool inBeam = false;
        computer.PipeOutputTo(v => inBeam = v == 1);

        while ((ranges[0].max - ranges[^1].min) < 100)
        {
            do
            {
                computer.AddInput(min++);
                computer.AddInput(y);
                await computer.Run(this.inputData);
            }
            while (!inBeam);

            min -= 1;
            max = Math.Max(min, max);

            do
            {
                computer.AddInput(max++);
                computer.AddInput(y);
                await computer.Run(this.inputData);
            }
            while (inBeam);

            max -= 1;

            ranges.Add((min, max));
            y += 1;

            if (ranges.Count > 100)
            {
                ranges.RemoveAt(0);
            }
        }

        long x = ranges[^1].min;

        return $"{x * 10_000 + y - 100}";

        static async Task<(long min, long max)> GetBounds(long y, long[] program)
        {
            IntCodeComputer computer = new();

            long min = long.MaxValue;
            long max = 0;

            long x = 0;

            computer.PipeOutputTo(v =>
            {
                if (v == 1)
                {
                    min = Math.Min(min, x);
                    max = Math.Max(max, x);
                }
            });

            for (x = 0; x <= y; x++)
            {
                computer.AddInput(x);
                computer.AddInput(y);
                await computer.Run(program);
            }

            return (min, max);
        }
    }
}
