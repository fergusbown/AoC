﻿namespace AoCRunner
{
    internal enum OperandDirection
    {
        Input,
        Output,
    }

    internal interface IIntCodeOperator
    {
        IReadOnlyList<OperandDirection> Operands { get; }

        Task Execute(long[] operands, IntCodeComputer.ProgramState state);
    }

}
