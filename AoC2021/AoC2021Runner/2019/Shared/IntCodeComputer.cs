using System.Collections.Immutable;

namespace AoC2021Runner
{
    internal class IntCodeComputer
    {
        private readonly ImmutableDictionary<int, IIntCodeOperator> operators;
        private readonly int[] operands;

        public IntCodeComputer(int input = 0)
        {
            var builder = ImmutableDictionary.CreateBuilder<int, IIntCodeOperator>();
            builder.Add(1, new AddOperator());
            builder.Add(2, new MultiplyOperator());
            builder.Add(3, new InputOperator(input));
            builder.Add(4, new OutputOperator());
            builder.Add(5, new JumpIfTrueOperator());
            builder.Add(6, new JumpIfFalseOperator());
            builder.Add(7, new LessThanOperator());
            builder.Add(8, new EqualsOperator());

            this.operators = builder.ToImmutable();
            this.operands = new int[operators.Select(o => o.Value.Operands.Count).Max()];
        }

        public int? Run(int[] program)
        {
            try
            {
                Span<int> operandsSpan = this.operands;

                int address = 0;
                int opCodeAndMode = program[address++];

                while (opCodeAndMode != 99)
                {
                    int opCode = opCodeAndMode % 100;
                    int parametersMode = opCodeAndMode / 100;

                    if (this.operators.TryGetValue(opCode, out IIntCodeOperator? op))
                    {
                        Span<int> parameterOperands = operandsSpan[0..op.Operands.Count];

                        for (int i = 0; i < parameterOperands.Length; i++)
                        {
                            int parameterMode = parametersMode % 10;
                            parametersMode /= 10;

                            parameterOperands[i] = program[address++];

                            if (parameterMode == 0 && op.Operands[i] == OperandDirection.Input)
                            {
                                parameterOperands[i] = program[parameterOperands[i]];
                            }
                        }

                        op.Execute(parameterOperands, program, ref address);
                        opCodeAndMode = program[address++];
                    }
                    else
                    {
                        return null;
                    }
                }

                return program[0];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        public int? Run(int[] program, int noun, int verb)
        {
            program[1] = noun;
            program[2] = verb;
            return Run(program);
        }

        public int[] GetProgram(string input)
            => input.Split(',').Select(i => int.Parse(i)).ToArray();
    }

    internal enum OperandDirection
    {
        Input,
        Output,
    }

    internal interface IIntCodeOperator
    {
        IReadOnlyList<OperandDirection> Operands { get; }

        void Execute(ReadOnlySpan<int> operands, int[] program, ref int address);
    }

    internal class AddOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
            => program[operands[2]] = operands[0] + operands[1];
    }

    internal class MultiplyOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
            => program[operands[2]] = operands[0] * operands[1];
    }

    internal class InputOperator : IIntCodeOperator
    {
        private readonly int input;

        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Output };

        public InputOperator(int input)
        {
            this.input = input;
        }

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
            => program[operands[0]] = input;
    }

    internal class OutputOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands => new[] { OperandDirection.Input };

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
            => program[0] = operands[0];
    }

    internal class JumpIfTrueOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input };

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
        {
            if (operands[0] != 0)
            {
                address = operands[1];
            }
        }
    }

    internal class JumpIfFalseOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input };

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
        {
            if (operands[0] == 0)
            {
                address = operands[1];
            }
        }
    }

    internal class LessThanOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
        {
            if (operands[0] < operands[1])
            {
                program[operands[2]] = 1;
            }
            else
            {
                program[operands[2]] = 0;
            }
        }
    }

    internal class EqualsOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public void Execute(ReadOnlySpan<int> operands, int[] program, ref int address)
        {
            if (operands[0] == operands[1])
            {
                program[operands[2]] = 1;
            }
            else
            {
                program[operands[2]] = 0;
            }
        }
    }

}
