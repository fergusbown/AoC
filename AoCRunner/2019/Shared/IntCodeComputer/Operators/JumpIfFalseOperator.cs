namespace AoCRunner
{
    internal class JumpIfFalseOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input };

        public Task Execute(long[] operands, IntCodeComputer.ProgramState state)
        {
            if (operands[0] == 0)
            {
                state.Index = operands[1];
            }

            return Task.CompletedTask;
        }
    }

}
