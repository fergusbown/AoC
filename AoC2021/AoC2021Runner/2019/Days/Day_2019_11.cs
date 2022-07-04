using System.Collections.Immutable;
using System.Text;

namespace AoC2021Runner;

internal partial class Day_2019_11 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_11(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        var robot = new HullPaintingRobot(this.inputData, false);
        int result = (await robot.Run()).Count;

        return result.ToString();
    }

    public async Task<string> Part2()
    {
        var robot = new HullPaintingRobot(this.inputData, true);
        await robot.Run();
        return robot.ToString();
    }

    private class HullPaintingRobot
    {
        private readonly long[] program;

        private Dictionary<(long X, long Y), long> painted = new();

        private (long X, long Y) position = (0, 0);
        private (long X, long Y) move = (0, -1);
        private bool nextOutputShouldPaint = true;

        public HullPaintingRobot(long[] program, bool startOnWhite)
        {
            this.program = program;

            if (startOnWhite)
            {
                painted[position] = 1;
            }
        }

        public async Task<ImmutableDictionary<(long X, long Y), long>> Run()
        {
            var computer = IntCodeComputer.New();

            await computer
                .PipeInputFrom(ProvideInput)
                .PipeOutputTo(HandleOutput)
                .Run(program);

            return painted.ToImmutableDictionary();
        }

        private Task<long> ProvideInput()
            => Task.FromResult(painted.TryGetValue(position, out var colour) ? colour : 0L);

        private void HandleOutput(long output)
        {
            if (nextOutputShouldPaint)
            {
                painted[position] = output;
            }
            else
            {
                if (output == 0) // turn left
                {
                    move = (move.Y, -move.X);
                }
                else // turn right
                {
                    move = (-move.Y, move.X);
                }

                position = (position.X + move.X, position.Y + move.Y);
            }

            nextOutputShouldPaint = !nextOutputShouldPaint;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            (long x, long y) min = (painted.Keys.Select(k => k.X).Min(), painted.Keys.Select(k => k.Y).Min());
            (long x, long y) max = (painted.Keys.Select(k => k.X).Max(), painted.Keys.Select(k => k.Y).Max());

            for (long y = min.y; y <= max.y; y++)
            {
                sb.AppendLine();
                for (long x = min.x; x <= max.x; x++)
                {
                    if (painted.TryGetValue((x, y), out var colour) && colour == 1)
                    {
                        sb.Append('#');
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }
            }
            return sb.ToString();
        }
    }
}
