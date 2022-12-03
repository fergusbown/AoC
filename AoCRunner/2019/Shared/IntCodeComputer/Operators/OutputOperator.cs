namespace AoCRunner
{
    internal class OutputOperator : IIntCodeOperator
    {
        public Action<long>? OutputAction { get; set; }

        public IReadOnlyList<OperandDirection> Operands => new[] { OperandDirection.Input };

        public long? Result { get; private set; }

        public Task Execute(long[] operands, IntCodeComputer.ProgramState state)
        {
            Result = operands[0];
            OutputAction?.Invoke(Result.Value);

            return Task.CompletedTask;
        }
    }

}
