namespace AoCRunner
{
    internal class AddOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public Task Execute(long[] operands, IntCodeComputer.ProgramState state)
        {
            state[operands[2]] = operands[0] + operands[1];
            return Task.CompletedTask;
        }
    }

}
