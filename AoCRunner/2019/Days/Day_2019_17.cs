using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Generator.Equals;

namespace AoCRunner;

internal partial class Day_2019_17 : IAsyncDayChallenge
{
    private readonly long[] inputData;
    private static readonly bool liveFeed = false;

    public Day_2019_17(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        var explorer = new Ascii(inputData);
        (var scaffolding, _) = await explorer.Map();

        return scaffolding
            .Where(s => scaffolding.Contains((s.x + 1, s.y))
                && scaffolding.Contains((s.x - 1, s.y))
                && scaffolding.Contains((s.x, s.y + 1))
                && scaffolding.Contains((s.x, s.y - 1)))
            .Select(s => s.x * s.y)
            .Sum()
            .ToString();
    }

    public async Task<string> Part2()
    {
        var explorer = new Ascii(inputData);
        (var scaffolding, Vacuum currentLocation) = await explorer.Map();

        IReadOnlyList<Command> commands = FindRoute(scaffolding, currentLocation);
        var route = CompressRoute(commands);
        var result = await explorer.Clean(route.ToInputs());
        return result.ToString();

        static IReadOnlyList<Command> FindRoute(ImmutableHashSet<(int x, int y)> scaffolding, Vacuum currentLocation)
        {
            List<Command> commands = new();
            while (true)
            {
                char turn;
                if (currentLocation.TryTurnLeft(scaffolding, out Vacuum newLocation))
                {
                    turn = 'L';
                }
                else if (currentLocation.TryTurnRight(scaffolding, out newLocation))
                {
                    turn = 'R';
                }
                else
                {
                    break;
                }

                currentLocation = newLocation;

                int moves = 0;
                while (currentLocation.TryMove(scaffolding, out newLocation))
                {
                    currentLocation = newLocation;
                    moves++;
                }

                commands.Add(new(turn, moves));
            }

            return commands;
        }

        static CompressionState CompressRoute(IReadOnlyList<Command> commands)
        {
            // in order to compress enough, but not blow our memory limits, we must have 3 or 4 commands in each instruction
            // clearly we must start with an instruction so take the first 3 commands and go from there
            // try all the permutations o useful until we find one that consumes everything with at most 3 instructions

            Stack<CompressionState> compressions = new();

            compressions.Push(new(commands, Array.Empty<Instruction>(), Array.Empty<int>()));

            CompressionState compressionState;

            while (true)
            {
                compressionState = compressions.Pop();

                if (compressionState.Instructions.Count > 3 || compressionState.Main.Count > 10)
                {
                    continue;
                }

                if (compressionState.Remaining.Count == 0)
                {
                    return compressionState;
                }

                var instruction1 = new Instruction(compressionState.Remaining.Take(3).ToArray());
                var instruction2 = new Instruction(compressionState.Remaining.Take(4).ToArray());

                compressions.Push(GetNextState(compressionState, instruction1));
                compressions.Push(GetNextState(compressionState, instruction2));
            }
        }

        static CompressionState GetNextState(CompressionState compressionState, Instruction nextInstruction)
        {
            (var remaining, var instructions, var main) = compressionState;

            if (!compressionState.ContainsInstruction(nextInstruction, out int index))
            {
                index = instructions.Count;
                instructions = instructions.Append(nextInstruction).ToArray();
            }

            CompressionState newCompressionState = new(
                remaining.Skip(nextInstruction.Commands.Count).ToArray(),
                instructions,
                main.Append(index).ToArray());

            return newCompressionState;
        }
    }

    private record CompressionState(IReadOnlyList<Command> Remaining, IReadOnlyList<Instruction> Instructions, IReadOnlyList<int> Main)
    {
        public IReadOnlyList<char> ToInputs()
        {
            List<char> inputs = new();

            // main method
            foreach (var instructionCall in Main)
            {
                inputs.Add((char)('A' + instructionCall));
                inputs.Add(',');
            }

            inputs[^1] = '\n';

            foreach (var instruction in Instructions)
            {
                foreach (var command in instruction.Commands)
                {
                    inputs.Add(command.Turn);
                    inputs.Add(',');

                    foreach(char digit in command.Moves.ToString())
                    {
                        inputs.Add(digit);
                    }
                    inputs.Add(',');
                }

                inputs[^1] = '\n';
            }

            inputs.Add(liveFeed ? 'y' : 'n');
            inputs.Add('\n');

            return inputs;
        }

        public bool ContainsInstruction(Instruction instruction, out int index)
        {
            for (index = 0; index < this.Instructions.Count; index++)
            {
                if (this.Instructions[index] == instruction)
                {
                    return true;
                }
            }

            return false;
        }
    }

    private record Command(char Turn, int Moves);

    [Equatable]
    private partial record Instruction([property: OrderedEquality] IReadOnlyList<Command> Commands);

    private enum Direction
    {
        North = '^',
        East = '>',
        West = '<',
        South = 'v',
    }

    private record Vacuum(Direction Direction, int x, int y)
    {
        public bool TryMove(ImmutableHashSet<(int x, int y)> scaffolding, out Vacuum newLocation)
        {
            int xDelta = 0;
            int yDelta = 0;

            switch (Direction)
            {
                case Direction.North:
                    yDelta = -1;
                    break;
                case Direction.East:
                    xDelta = 1;
                    break;
                case Direction.West:
                    xDelta = -1;
                    break;
                case Direction.South:
                    yDelta = 1;
                    break;
                default:
                    break;
            }

            newLocation = this with
            {
                x = x + xDelta,
                y = y + yDelta,
            };

            return scaffolding.Contains((newLocation.x, newLocation.y));
        }

        public bool TryTurnLeft(ImmutableHashSet<(int x, int y)> scaffolding, out Vacuum newLocation)
        {
            var newDirection = Direction switch
            {
                Direction.North => Direction.West,
                Direction.East => Direction.North,
                Direction.West => Direction.South,
                _ => Direction.East,
            };

            newLocation = this with
            {
                Direction = newDirection,
            };

            return newLocation.TryMove(scaffolding, out _);
        }

        public bool TryTurnRight(ImmutableHashSet<(int x, int y)> scaffolding, out Vacuum newLocation)
        {
            var newDirection = Direction switch
            {
                Direction.North => Direction.East,
                Direction.East => Direction.South,
                Direction.West => Direction.North,
                _ => Direction.West,
            };

            newLocation = this with
            {
                Direction = newDirection,
            };

            return newLocation.TryMove(scaffolding, out _);
        }
    }

    private class Ascii
    {
        private readonly long[] program;

        public Ascii(long[] program)
        {
            this.program = program;
        }

        public async Task<(ImmutableHashSet<(int x, int y)> Scaffolding, Vacuum Vacuum)> Map()
        {
            this.program[0] = 1;

            HashSet<(int x, int y)> scaffolding = new();
            (int x, int y) currentLocation = (0, 0);
            Vacuum vacuum = null!;
            
            var computer = IntCodeComputer.New();

            computer
                .PipeOutputTo(HandleOutput);

            await computer.Run(this.program);

            return (scaffolding.ToImmutableHashSet(), vacuum);

            void HandleOutput(long output)
            {
                char character = (char)output;

                if (liveFeed)
                {
                    Console.Write(character);
                }

                switch (character)
                {
                    case '\n':
                        currentLocation = (0, currentLocation.y + 1);
                        return;
                    case '#':
                        scaffolding.Add(currentLocation);
                        break;
                    case '^':
                    case '>':
                    case 'v':
                    case '<':
                        scaffolding.Add(currentLocation);
                        vacuum = new((Direction)character, currentLocation.x, currentLocation.y);
                        break;
                    default:
                        break;
                }

                currentLocation = (currentLocation.x + 1, currentLocation.y);
            }

        }
        public async Task<long> Clean(IReadOnlyList<char> instructions)
        {
            this.program[0] = 2;

            var computer = IntCodeComputer.New();
            var inputs = new Queue<char>(instructions);
            long result = 0;

            computer
                .PipeInputFrom(ProvideInput)
                .PipeOutputTo(HandleOutput);

            await computer.Run(this.program);

            return result;

            Task<long> ProvideInput()
            {
                return Task.FromResult((long)inputs.Dequeue());
            }

            void HandleOutput(long output)
            {
                result = output;

                if (output < char.MaxValue && instructions[^2] == 'y')
                {
                    Console.Write((char)output);
                }
            }
        }
    }
}
