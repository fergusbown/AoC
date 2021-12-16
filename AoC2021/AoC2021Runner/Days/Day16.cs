using System.Diagnostics.CodeAnalysis;

namespace AoC2021Runner;
internal class Day16 : IDayChallenge
{
    private readonly bool[] inputData;

    public Day16(string inputData)
    {
        this.inputData = GetInput(inputData);
    }

    public string Part1()
    {
        IPacket inputPacket = ParseInput(inputData);
        return inputPacket.VersionSum.ToString();
    }

    public string Part2()
    {
        IPacket inputPacket = ParseInput(inputData);
        return inputPacket.Value.ToString();
    }

    private IPacket ParseInput(Span<bool> input)
    {
        return ConsumePackets(input).Single();

        static IReadOnlyList<IPacket> ConsumePackets(Span<bool> input)
        {
            List<IPacket> result = new List<IPacket>();

            while(TryConsumePacket(ref input, out var packet))
            {
                result.Add(packet);
            }

            return result;
        }

        static bool TryConsumePacket(ref Span<bool> input, [NotNullWhen(true)] out IPacket? packet)
        {
            const int literalValue = 4;

            const long sum = 0;
            const long product = 1;
            const long minimum = 2;
            const long maximum = 3;
            const long greaterThan = 5;
            const long lessThan = 6;
            const long equals =7;

            if (TryConsumeInt(ref input, 3, out var version))
            {
                if (TryConsumeInt(ref input, 3, out var typeId))
                {
                    switch (typeId)
                    {
                        case literalValue:
                            if (TryConsumeLiteral(ref input, out var value))
                            {
                                packet = new Literal(version, value);
                                return true;
                            }
                            break;
                        default: //operator
                            {
                                if (TryConsumeOperator(ref input, out var subPackets))
                                {
                                    switch (typeId)
                                    {
                                        case sum:
                                            packet = new SumOperator(version, subPackets);
                                            return true;
                                        case product:
                                            packet = new ProductOperator(version, subPackets);
                                            return true;
                                        case minimum:
                                            packet = new MinimumOperator(version, subPackets);
                                            return true;
                                        case maximum:
                                            packet = new MaximumOperator(version, subPackets);
                                            return true;
                                        case greaterThan:
                                            packet = new GreaterThanOperator(version, subPackets);
                                            return true;
                                        case lessThan:
                                            packet = new LessThanOperator(version, subPackets);
                                            return true;
                                        case equals:
                                            packet = new EqualOperator(version, subPackets);
                                            return true;
                                        default:
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            packet = null;
            return false;
        }

        static bool TryConsumeLiteral(ref Span<bool> input, out long result)
        {
            result = 0;
            long moreGroups = 0;

            do
            {
                if (!TryConsumeInt(ref input, 1, out moreGroups))
                {
                    return false;
                }

                if (!TryConsumeInt(ref input, 4, out result, result))
                {
                    return false;
                }
            }
            while (moreGroups == 1);

            return true;
        }

        static bool TryConsumeOperator(ref Span<bool> input, out IReadOnlyList<IPacket> subPackets)
        {
            const long lengthOfSubpackets = 0;
            const long countOfSubpackets = 1;
            if (TryConsumeInt(ref input, 1, out var lengthType))
            {
                switch (lengthType)
                {
                    case lengthOfSubpackets:
                        {
                            if (TryConsumeInt(ref input, 15, out var subPacketsLength))
                            {
                                subPackets = ConsumePackets(input[..(int)subPacketsLength]);
                                input = input[(int)subPacketsLength..];
                                return true;
                            }
                        }
                        break;
                    case countOfSubpackets:
                        {
                            if (TryConsumeInt(ref input, 11, out var subPacketsCount))
                            {
                                List<IPacket> packets = new List<IPacket>((int)subPacketsCount);
                                for (int i = 0; i < subPacketsCount; i++)
                                {
                                    _ = TryConsumePacket(ref input, out var subPacket);
                                    packets.Add(subPacket!);
                                }
                                subPackets = packets;
                                return true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            subPackets = Array.Empty<IPacket>();
            return false;
        }

        static bool TryConsumeInt(ref Span<bool> input, int length, out long result, long startingValue = 0)
        {
            if (input.Length < length)
            {
                result = startingValue;
                input = input[..0];
                return false;
            }
            else
            {
                result = startingValue;
                for (int i = 0; i < length; i++)
                {
                    result *= 2;
                    if (input[i])
                    {
                        result += 1;
                    }
                }

                input = input[length..];
                return true;
            }
        }
    }

    private interface IPacket
    {
        long Version { get; }

        long Value { get; }

        long VersionSum { get; }
    }
    private class Literal: IPacket
    {
        public Literal(long version, long value)
        {
            Version = version;
            Value = value;
        }

        public long Version { get; }

        public long Value { get; }

        public long VersionSum => Version;
    }

    private abstract class Operator: IPacket
    {
        public Operator(long version, IReadOnlyList<IPacket> subPackets)
        {
            Version = version;
            SubPackets = subPackets;
        }

        public long Version { get; }

        public IReadOnlyList<IPacket> SubPackets { get; }

        public long VersionSum
        {
            get
            {
                return SubPackets.Select(p => p.VersionSum).Sum() + Version;
            }
        }

        public abstract long Value { get; }
    }

    private class SumOperator : Operator
    {
        public SumOperator(long version, IReadOnlyList<IPacket> subPackets) : base(version, subPackets)
        {
        }

        public override long Value => this.SubPackets.Select(p => p.Value).Sum();
    }

    private class ProductOperator : Operator
    {
        public ProductOperator(long version, IReadOnlyList<IPacket> subPackets) : base(version, subPackets)
        {
        }

        public override long Value => this.SubPackets.Select(p => p.Value).Aggregate((a, b) => a * b);
    }

    private class MinimumOperator : Operator
    {
        public MinimumOperator(long version, IReadOnlyList<IPacket> subPackets) : base(version, subPackets)
        {
        }

        public override long Value => this.SubPackets.Select(p => p.Value).Min();
    }

    private class MaximumOperator : Operator
    {
        public MaximumOperator(long version, IReadOnlyList<IPacket> subPackets) : base(version, subPackets)
        {
        }

        public override long Value => this.SubPackets.Select(p => p.Value).Max();
    }

    private class GreaterThanOperator : Operator
    {
        public GreaterThanOperator(long version, IReadOnlyList<IPacket> subPackets) : base(version, subPackets)
        {
        }

        public override long Value => this.SubPackets[0].Value > this.SubPackets[1].Value ? 1 : 0;
    }

    private class LessThanOperator : Operator
    {
        public LessThanOperator(long version, IReadOnlyList<IPacket> subPackets) : base(version, subPackets)
        {
        }

        public override long Value => this.SubPackets[0].Value < this.SubPackets[1].Value ? 1 : 0;
    }

    private class EqualOperator : Operator
    {
        public EqualOperator(long version, IReadOnlyList<IPacket> subPackets) : base(version, subPackets)
        {
        }

        public override long Value => this.SubPackets[0].Value == this.SubPackets[1].Value ? 1 : 0;
    }


    private static bool[] GetInput(string input)
    {
        bool[] result = new bool[input.Length * 4];
        int resultIndex = 0;
 
        foreach (char ch in input)
        {
            switch (ch)
            {
                case '0':
                    SetResult(0, 0, 0, 0);
                    break;
                case '1':
                    SetResult(0, 0, 0, 1);
                    break;
                case '2':
                    SetResult(0, 0, 1, 0);
                    break;
                case '3':
                    SetResult(0, 0, 1, 1);
                    break;
                case '4':
                    SetResult(0, 1, 0, 0);
                    break;
                case '5':
                    SetResult(0, 1, 0, 1);
                    break;
                case '6':
                    SetResult(0, 1, 1, 0);
                    break;
                case '7':
                    SetResult(0, 1, 1, 1);
                    break;
                case '8':
                    SetResult(1, 0, 0, 0);
                    break;
                case '9':
                    SetResult(1, 0, 0, 1);
                    break;
                case 'A':
                    SetResult(1, 0, 1, 0);
                    break;
                case 'B':
                    SetResult(1, 0, 1, 1);
                    break;
                case 'C':
                    SetResult(1, 1, 0, 0);
                    break;
                case 'D':
                    SetResult(1, 1, 0, 1);
                    break;
                case 'E':
                    SetResult(1, 1, 1, 0);
                    break;
                case 'F':
                    SetResult(1, 1, 1, 1);
                    break;
                default:
                    break;
            }
        }

        return result;

        void SetResult(int bit1, int bit2, int bit3, int bit4)
        {
            result[resultIndex++] = bit1 == 1;
            result[resultIndex++] = bit2 == 1;
            result[resultIndex++] = bit3 == 1;
            result[resultIndex++] = bit4 == 1;
        }
    }
}
