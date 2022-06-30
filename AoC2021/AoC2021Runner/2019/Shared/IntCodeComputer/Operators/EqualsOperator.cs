namespace AoC2021Runner
{
    internal class EqualsOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public Task Execute(long[] operands, IntCodeComputer.ProgramState state)
        {
            if (operands[0] == operands[1])
            {
                state[operands[2]] = 1;
            }
            else
            {
                state[operands[2]] = 0;
            }

            return Task.CompletedTask;
        }
    }

}
