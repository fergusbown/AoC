using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace AoC2021Runner
{
    internal class IntCodeComputer
    {
        private readonly ImmutableDictionary<int, IIntCodeOperator> operators;
        private readonly int[] operands;
        private readonly InputOperator inputOperator;
        private readonly OutputOperator outputOperator;


        public IntCodeComputer(params int[] input)
        {
            this.inputOperator = new InputOperator(input);
            this.outputOperator = new OutputOperator();
            
            var builder = ImmutableDictionary.CreateBuilder<int, IIntCodeOperator>();
            builder.Add(1, new AddOperator());
            builder.Add(2, new MultiplyOperator());
            builder.Add(3, inputOperator);
            builder.Add(4, outputOperator);
            builder.Add(5, new JumpIfTrueOperator());
            builder.Add(6, new JumpIfFalseOperator());
            builder.Add(7, new LessThanOperator());
            builder.Add(8, new EqualsOperator());

            this.operators = builder.ToImmutable();
            this.operands = new int[operators.Select(o => o.Value.Operands.Count).Max()];
        }

        public void AddInput(int value)
        {
            this.inputOperator.AddInput(value);
        }

        public void PipeOutputTo(IntCodeComputer destination)
        {
            this.outputOperator.OutputAction = (v) => destination.AddInput(v);
        }

        public async Task<int> Run(int[] program)
        {
            int address = 0;
            int opCodeAndMode = program[address++];

            while (opCodeAndMode != 99)
            {
                int opCode = opCodeAndMode % 100;
                int parametersMode = opCodeAndMode / 100;

                if (this.operators.TryGetValue(opCode, out IIntCodeOperator? op))
                {
                    for (int i = 0; i < op.Operands.Count; i++)
                    {
                        int parameterMode = parametersMode % 10;
                        parametersMode /= 10;

                        operands[i] = program[address++];

                        if (parameterMode == 0 && op.Operands[i] == OperandDirection.Input)
                        {
                            operands[i] = program[operands[i]];
                        }
                    }

                    address = await op.Execute(operands, program) ?? address;
                    opCodeAndMode = program[address++];
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return program[0];
        }

        public Task<int> Run(int[] program, int noun, int verb)
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

        Task<int?> Execute(int[] operands, int[] program);
    }

    internal class AddOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public Task<int?> Execute(int[] operands, int[] program)
        {
            program[operands[2]] = operands[0] + operands[1];
            return Task.FromResult<int?>(null);
        }
    }

    internal class MultiplyOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public Task<int?> Execute(int[] operands, int[] program)
        {
            program[operands[2]] = operands[0] * operands[1];
            return Task.FromResult<int?>(null);
        }
    }

    internal class InputOperator : IIntCodeOperator
    {
        private readonly ConcurrentQueue<int> inputs;

        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Output };

        public InputOperator(int[] inputs)
        {
            this.inputs = new(inputs);
        }

        public async Task<int?> Execute(int[] operands, int[] program)
        {
            int input;
            while(!inputs.TryDequeue(out input))
            {
                await Task.Delay(1);
            }

            program[operands[0]] = input;

            return null;
        }

        public void AddInput(int value)
        {
            this.inputs.Enqueue(value);
        }
    }

    internal class OutputOperator : IIntCodeOperator
    {
        public  Action<int>? OutputAction { get; set; }

        public IReadOnlyList<OperandDirection> Operands => new[] { OperandDirection.Input };

        public Task<int?> Execute(int[] operands, int[] program)
        {
            program[0] = operands[0];
            OutputAction?.Invoke(operands[0]);
            return Task.FromResult<int?>(null);
        }
    }

    internal class JumpIfTrueOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input };

        public Task<int?> Execute(int[] operands, int[] program)
        {
            if (operands[0] != 0)
            {
                return Task.FromResult<int?>(operands[1]);
            }

            return Task.FromResult<int?>(null);
        }
    }

    internal class JumpIfFalseOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input };

        public Task<int?> Execute(int[] operands, int[] program)
        {
            if (operands[0] == 0)
            {
                return Task.FromResult<int?>(operands[1]);
            }

            return Task.FromResult<int?>(null);
        }
    }

    internal class LessThanOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public Task<int?> Execute(int[] operands, int[] program)
        {
            if (operands[0] < operands[1])
            {
                program[operands[2]] = 1;
            }
            else
            {
                program[operands[2]] = 0;
            }

            return Task.FromResult<int?>(null);
        }
    }

    internal class EqualsOperator : IIntCodeOperator
    {
        public IReadOnlyList<OperandDirection> Operands { get; } = new[] { OperandDirection.Input, OperandDirection.Input, OperandDirection.Output };

        public Task<int?> Execute(int[] operands, int[] program)
        {
            if (operands[0] == operands[1])
            {
                program[operands[2]] = 1;
            }
            else
            {
                program[operands[2]] = 0;
            }

            return Task.FromResult<int?>(null);
        }
    }

}
