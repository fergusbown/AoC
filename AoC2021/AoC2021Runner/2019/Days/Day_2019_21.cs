namespace AoC2021Runner;

internal partial class Day_2019_21 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_21(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public Task<string> Part1()
    {
        string[] commands = new[]
        {
            "NOT B T", // T true if B a hole
            "NOT C J", // J true if C a hole
            "OR T J",  // J true if B or C a hole
            "AND D J", // J true if B or C a hole and D ground
            "NOT A T", // T true if A a hole (cos then we have to jump)
            "OR T J",  // J true if A a hole or (B or C a hole and D ground)
            "WALK"
        };

        return LaunchSpringDroid(this.inputData, commands);
    }

    public Task<string> Part2()
    {
        string[] commands = new[]
        {
            "NOT B T", // T true if B a hole
            "NOT C J", // J true if C a hole
            "OR T J",  // J true if B or C a hole
            "AND D J", // J true if B or C a hole and D ground
            "AND H J", // but only if H is also ground, so that early jumping doesn't trap us
            "NOT A T", // T true if A a hole (cos then we have to jump)
            "OR T J",  // J true if A a hole or (B or C a hole and D and H ground)
            "RUN"
        };

        return LaunchSpringDroid(this.inputData, commands);
    }

    private static async Task<string> LaunchSpringDroid(long[] program, string[] commands)
    {
        Queue<char> instructions = new Queue<char>();

        foreach (var command in commands)
        {
            foreach (var ch in command)
            {
                instructions.Enqueue(ch);
            }
            instructions.Enqueue('\n');
        }

        long result = 0;

        IntCodeComputer computer = IntCodeComputer
            .New()
            .PipeInputFrom(ProvideInput)
            .PipeOutputTo(HandleOutput);

        await computer.Run(program);

        return result.ToString();

        Task<long> ProvideInput()
        {
            return Task.FromResult((long)instructions.Dequeue());
        }

        void HandleOutput(long output)
        {
            result = output;
        }

    }
}
