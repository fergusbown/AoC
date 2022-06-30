using System.Collections.Immutable;

namespace AoC2021Runner
{
    internal class IntCodeComputer
    {
        internal class ProgramState
        {
            private readonly Dictionary<long, long> addressSpace = new();

            public ProgramState(long[] addressSpace)
            {
                for (int i = 0; i < addressSpace.Length; i++)
                {
                    this.addressSpace[i] = addressSpace[i];
                }
            }

            public long Index { get; set; }

            public long RelativeBase { get; set; }

            public long this[long address]
            {
                get
                {
                    if (address < 0)
                    {
                        throw new IndexOutOfRangeException(nameof(address));
                    }

                    if (this.addressSpace.TryGetValue(address, out var result))
                    {
                        return result;
                    }
                    else
                    {
                        return 0;
                    }
                }

                set
                {
                    if (address < 0)
                    {
                        throw new IndexOutOfRangeException(nameof(address));
                    }

                    this.addressSpace[address] = value;
                }
            }
        }

        private readonly ImmutableDictionary<long, IIntCodeOperator> operators;
        private readonly ImmutableDictionary<(long, OperandDirection), IIntCodeParameterMode> parameterModes;

        private readonly long[] operands;
        private readonly InputOperator inputOperator;
        private readonly OutputOperator outputOperator;

        public static IntCodeComputer New() => new IntCodeComputer();

        public IntCodeComputer()
        {
            this.inputOperator = new InputOperator();
            this.outputOperator = new OutputOperator();

            this.operators =
                new Dictionary<long, IIntCodeOperator>()
                {
                    { 1, new AddOperator() },
                    { 2, new MultiplyOperator() },
                    { 3, inputOperator },
                    { 4, outputOperator },
                    { 5, new JumpIfTrueOperator()},
                    { 6, new JumpIfFalseOperator()},
                    { 7, new LessThanOperator()},
                    { 8, new EqualsOperator()},
                    { 9, new RelativeBaseOperator()},
                }.ToImmutableDictionary();

            this.parameterModes = new Dictionary<(long, OperandDirection), IIntCodeParameterMode>()
            {
                { (0, OperandDirection.Input), new PositionMode() },
                { (1, OperandDirection.Input), new ImmediateMode() },
                { (2, OperandDirection.Input), new RelativePositionMode() },

                { (0, OperandDirection.Output), new ImmediateMode() },
                { (1, OperandDirection.Output), new ImmediateMode() },
                { (2, OperandDirection.Output), new RelativeImmediateMode() },
            }.ToImmutableDictionary();


            this.operands = new long[operators.Select(o => o.Value.Operands.Count).Max()];
        }

        public IntCodeComputer AddInput(long value)
        {
            this.inputOperator.AddInput(value);
            return this;
        }

        public void PipeOutputTo(IntCodeComputer destination)
        {
            this.outputOperator.OutputAction = (v) => destination.AddInput(v);
        }

        public void PipeOutputTo(Action<long> destination)
        {
            this.outputOperator.OutputAction = destination;
        }

        public async Task<long> Run(long[] initialProgram)
        {
            ProgramState state = new(initialProgram);
            long opCodeAndMode = state[state.Index++];

            while (opCodeAndMode != 99)
            {
                long opCode = opCodeAndMode % 100;
                long parametersMode = opCodeAndMode / 100;

                if (this.operators.TryGetValue(opCode, out IIntCodeOperator? op))
                {
                    for (int i = 0; i < op.Operands.Count; i++)
                    {
                        long parameterMode = parametersMode % 10;
                        parametersMode /= 10;

                        operands[i] = state[state.Index++];

                        if (this.parameterModes.TryGetValue((parameterMode, op.Operands[i]), out var modeImplementation))
                        {
                            operands[i] = modeImplementation.GetValue(operands[i], state);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    await op.Execute(operands, state);
                    opCodeAndMode = state[state.Index++];
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }

            return outputOperator.Result ?? state[0];
        }

        public Task<long> Run(long[] program, long noun, long verb)
        {
            program[1] = noun;
            program[2] = verb;
            return Run(program);
        }

        public static long[] GetProgram(string input)
            => input.Split(',').Select(i => long.Parse(i)).ToArray();
    }

}
