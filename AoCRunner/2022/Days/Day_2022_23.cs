using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace AoCRunner;

internal class Day_2022_23 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_23(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        IReadOnlyCollection<Elf> elves = Parse(inputData);
        ElfMover elfMover = new();

        for (int i = 0; i < 10; i++)
        {
            elves = elfMover.MoveAll(elves, out _);
        }

        int minColumn = elves.Select(e => e.Column).Min();
        int maxColumn = elves.Select(e => e.Column).Max();
        int minRow = elves.Select(e => e.Row).Min();
        int maxRow = elves.Select(e => e.Row).Max();

        int cellCount = Math.Abs((maxColumn - minColumn + 1) * (maxRow - minRow + 1));

        return $"{cellCount - elves.Count}";
    }

    public string Part2()
    {
        IReadOnlyCollection<Elf> elves = Parse(inputData);
        ElfMover elfMover = new();
        int movedCount;
        int roundCount = 0;

        do
        {
            elves = elfMover.MoveAll(elves, out movedCount);
            roundCount++;
        }
        while (movedCount > 0);

        return roundCount.ToString();
    }

    private static IReadOnlyCollection<Elf> Parse(string inputData)
    {
        List<Elf> result = new();
        int elfIndex = 0;

        _ = inputData.GridForDay((ch, row, column) =>
        {
            if (ch == '#')
            {
                result.Add(new Elf(elfIndex++, row, column));
            }

            return ch;
        });

        return result;
    }

    private readonly record struct Elf(int Identifier, int Row, int Column);

    private class ElfIdComparer : IEqualityComparer<Elf>
    {
        private ElfIdComparer()
        {
            
        }

        public bool Equals(Elf x, Elf y)
            => x.Identifier.Equals(y.Identifier);

        public int GetHashCode([DisallowNull] Elf obj)
            => obj.Identifier;

        public static IEqualityComparer<Elf> Instance { get; } = new ElfIdComparer();
    }

    private class ElfLocationComparer : IEqualityComparer<Elf>
    {
        private ElfLocationComparer()
        {

        }

        public bool Equals(Elf x, Elf y)
            => x.Row.Equals(y.Row) && x.Column.Equals(y.Column);

        public int GetHashCode([DisallowNull] Elf obj)
            => HashCode.Combine(obj.Row, obj.Column);

        public static IEqualityComparer<Elf> Instance { get; } = new ElfLocationComparer();
    }

    private delegate bool CanProposeMove(Elf elf, HashSet<Elf> locations, out Elf proposal);

    private class ElfMover
    {
        private readonly CanProposeMove[] rules;
        private int ruleIndex = 0;

        public ElfMover()
        {
            rules = new CanProposeMove[]
            {
                CanMoveNorth,
                CanMoveSouth,
                CanMoveWest,
                CanMoveEast,
            };
        }

        public IReadOnlyCollection<Elf> MoveAll(IReadOnlyCollection<Elf> elves, out int numberMoved)
        {
            HashSet<Elf> currentLocations = new HashSet<Elf>(elves, ElfLocationComparer.Instance);
            HashSet<Elf> moved = new HashSet<Elf>(ElfLocationComparer.Instance);
            HashSet<Elf> invalidMoves = new HashSet<Elf>(ElfLocationComparer.Instance);

            foreach (Elf elf in elves)
            {
                if (CanProposeMove(elf, currentLocations, out var proposal))
                {
                    if (!moved.Add(proposal))
                    {
                        moved.TryGetValue(proposal, out var invalidMove);
                        invalidMoves.Add(invalidMove);
                    }
                }
            }

            moved.ExceptWith(invalidMoves);
            numberMoved = moved.Count;

            HashSet<Elf> finalLocations = new HashSet<Elf>(moved, ElfIdComparer.Instance);
            finalLocations.UnionWith(elves);

            ruleIndex++;
            ruleIndex %= rules.Length;

            return finalLocations;
        }

        private bool CanProposeMove(Elf elf, HashSet<Elf> locations, out Elf proposal)
        {
            Elf? target = null;
            bool foundNeighbour = false;

            foreach (var rule in rules.Skip(ruleIndex).Concat(rules.Take(ruleIndex)))
            {
                if (rule(elf, locations, out proposal))
                {
                    target ??= proposal;
                }
                else
                {
                    foundNeighbour = true;
                }
            }

            if (target is null || foundNeighbour is false)
            {
                proposal = default;
                return false;
            }

            proposal = target.Value;
            return true;
        }

        private static bool CanMoveNorth(Elf elf, HashSet<Elf> locations, out Elf proposal)
            => CanMove(
                locations,
                elf with { Row = elf.Row - 1 },
                elf with { Row = elf.Row - 1, Column = elf.Column - 1 },
                elf with { Row = elf.Row - 1, Column = elf.Column + 1 }, out proposal);

        private static bool CanMoveSouth(Elf elf, HashSet<Elf> locations, out Elf proposal)
            => CanMove(
                locations,
                elf with { Row = elf.Row + 1 },
                elf with { Row = elf.Row + 1, Column = elf.Column - 1 },
                elf with { Row = elf.Row + 1, Column = elf.Column + 1 }, out proposal);

        private static bool CanMoveWest(Elf elf, HashSet<Elf> locations, out Elf proposal)
            => CanMove(
                locations,
                elf with { Column = elf.Column - 1 },
                elf with { Column = elf.Column - 1, Row = elf.Row - 1 },
                elf with { Column = elf.Column - 1, Row = elf.Row + 1 }, out proposal);

        private static bool CanMoveEast(Elf elf, HashSet<Elf> locations, out Elf proposal)
            => CanMove(
                locations,
                elf with { Column = elf.Column + 1 },
                elf with { Column = elf.Column + 1, Row = elf.Row - 1 },
                elf with { Column = elf.Column + 1, Row = elf.Row + 1 }, out proposal);

        private static bool CanMove(HashSet<Elf> locations, Elf destination, Elf mustBeEmpty, Elf mustBeEmptyTwo, out Elf proposal)
        {
            if (locations.Contains(destination) || locations.Contains(mustBeEmpty) || locations.Contains(mustBeEmptyTwo))
            {
                proposal = default;
                return false;
            }

            proposal = destination;
            return true;
        }
    }
}
