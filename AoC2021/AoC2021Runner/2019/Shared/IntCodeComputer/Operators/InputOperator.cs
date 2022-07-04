using System.Collections.Concurrent;

namespace AoC2021Runner
{
    internal class InputOperator : IIntCodeOperator
    {
        private readonly ConcurrentQueue<long> inputs = new();

        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Output };

        public async Task Execute(long[] operands, IntCodeComputer.ProgramState state)
        {
            long input;

            if (InputProvider is null)
            {
                while (!inputs.TryDequeue(out input))
                {
                    await Task.Delay(1);
                }
            }
            else
            {
                input = await InputProvider();
            }

            state[operands[0]] = input;
        }

        public void AddInput(long value)
        {
            this.inputs.Enqueue(value);
        }

        public Func<Task<long>>? InputProvider { get; set; }
    }

}
