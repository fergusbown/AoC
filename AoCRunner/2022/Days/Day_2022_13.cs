using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AoCRunner;

internal class Day_2022_13 : IDayChallenge
{
    private readonly string[] inputData;

    public Day_2022_13(string inputData)
    {
        this.inputData = inputData.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }

    public string Part1()
    {
        var packets = inputData
        .Select(p => ParsePacket(p))
        .ToArray();

        int pairIndex = 0;
        int correctPairsSum = 0;
        for (int i = 0; i < packets.Length; i+=2)
        {
            pairIndex++;

            int compare = PacketPartComparer.Instance.Compare(packets[i], packets[i + 1]);
            
            if (compare < 0)
            {
                correctPairsSum += pairIndex;
            }
        }

        return correctPairsSum.ToString();
    }

    public string Part2()
    {
        IPacketPart[] extraPackets = new[]
        {
            ParsePacket("[[2]]"),
            ParsePacket("[[6]]"),
        };

        var orderedPackets = inputData
            .Select(p => ParsePacket(p))
            .Concat(extraPackets)
            .Order(PacketPartComparer.Instance)
            .ToList();

        var firstIndex = orderedPackets.IndexOf(extraPackets[0]) + 1;
        var secondIndex = orderedPackets.IndexOf(extraPackets[1]) + 1;
        return $"{firstIndex * secondIndex}";
    }

    private static IPacketPart ParsePacket(string packet)
    {
        Stack<CollectionPart> pendingParts = new();
        CollectionPart processingCollection = new();
        int? processingNumber = null;

        foreach (char c in packet.Skip(1))
        {
            switch (c)
            {
                case '[':
                    pendingParts.Push(processingCollection);
                    processingCollection = new();
                    break;
                case ']':
                    if (processingNumber is not null)
                    {
                        processingCollection.Add(new NumberPart(processingNumber.Value));
                        processingNumber = null;
                    }

                    if (pendingParts.TryPop(out var previousCollection))
                    {
                        previousCollection.Add(processingCollection);
                        processingCollection = previousCollection;
                    }
                    break;
                case ',':
                    if (processingNumber is not null)
                    {
                        processingCollection.Add(new NumberPart(processingNumber.Value));
                        processingNumber = null;
                    }
                    break;
                default:
                    processingNumber ??= 0;
                    processingNumber *= 10;
                    processingNumber += c - '0';
                    break;
            }
        }

        return processingCollection;
    }

    public interface IPacketPart
    {
        bool IsNumberPart([NotNullWhen(true)] out NumberPart? numberPart);

        CollectionPart AsCollectionPart { get; }
    }

    public class NumberPart : IPacketPart
    {
        public int Value { get; }

        public CollectionPart AsCollectionPart => new CollectionPart().Add(this);

        public NumberPart(int value)
        {
            this.Value = value;
        }

        public bool IsNumberPart([NotNullWhen(true)] out NumberPart? numberPart)
        {
            numberPart = this;
            return true;
        }
    }

    public class CollectionPart : IPacketPart
    {
        private List<IPacketPart> parts = new();

        public IPacketPart this[int index] => parts[index];

        public int Count => parts.Count;

        public CollectionPart Add(IPacketPart part)
        {
            parts.Add(part);
            return this;
        }

        public CollectionPart AsCollectionPart => this;

        public bool IsNumberPart([NotNullWhen(true)] out NumberPart? numberPart)
        {
            numberPart = null;
            return false;
        }
    }

    public class PacketPartComparer : IComparer<IPacketPart>
    {
        public static IComparer<IPacketPart> Instance { get; } = new PacketPartComparer();

        private PacketPartComparer()
        {

        }
        
        public int Compare(IPacketPart? x, IPacketPart? y)
        {
            Debug.Assert(x is not null && y is not null);

            if (x.IsNumberPart(out NumberPart? xn) && y.IsNumberPart(out NumberPart? yn))
            {
                return xn.Value.CompareTo(yn.Value);
            }

            var xc = x.AsCollectionPart;
            var yc = y.AsCollectionPart;

            int matchedCount = Math.Min(xc.Count, yc.Count);

            for (int i = 0; i < matchedCount; i++)
            {
                int result = this.Compare(xc[i], yc[i]);

                if (result != 0)
                {
                    return result;
                }
            }

            if (xc.Count < yc.Count)
            {
                return -1;
            }

            if (xc.Count == yc.Count)
            {
                return 0;
            }

            return 1;
        }
    }
}
