namespace AoC2021Runner
{
    internal class RelativeBaseOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands => new[] { OperandDirection.Input };

        public Task Execute(long[] operands, IntCodeComputer.ProgramState state)
        {
            state.RelativeBase += operands[0];
            return Task.CompletedTask;
        }
    }

}
