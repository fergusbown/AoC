using Generator.Equals;

namespace AoC2021Runner;

internal partial class Day_2021_22 : IDayChallenge
{
    private readonly IReadOnlyList<Instruction> instructions;

    public Day_2021_22(string inputData)
    {
        instructions = ParseInput(inputData);
    }

    public string Part1()
        => ApplyInstructions(instructions.Where(i => i.IsInitialisationSequence));

    public string Part2()
        => ApplyInstructions(instructions);

    private static IReadOnlyList<Instruction> ParseInput(string input)
        => input.StringsForDay().Select(s => new Instruction(s)).ToArray();

    private static string ApplyInstructions(IEnumerable<Instruction> toApply)
    {
        List<Region> litRegions = new();

        foreach (var instruction in toApply)
        {
            List<Region> newLitRegions = new();
            if (instruction.TurnOn)
            {
                newLitRegions.Add(instruction.Region);
            }

            foreach (var litRegion in litRegions)
            {
                newLitRegions.AddRange(litRegion.Except(instruction.Region));
            }

            litRegions = newLitRegions;
        }

        return litRegions.Select(r => r.Volume).Sum().ToString();
    }

    private class Instruction
    {
        public Instruction(string text)
        {
            var parts = text.Split(new string[] { " x=", "..", ",y=", ",z=" }, StringSplitOptions.None);
            TurnOn = parts[0] == "on";
            var numericParts = parts.Skip(1).Select(x => int.Parse(x)).ToArray();

            var start = (numericParts[0], numericParts[2], numericParts[4]);
            var end = (numericParts[1], numericParts[3], numericParts[5]);
            this.Region = new Region(start, end);

            this.IsInitialisationSequence = numericParts.All(v => Math.Abs(v) <= 50);
        }

        public bool IsInitialisationSequence { get; }
        public bool TurnOn { get; }

        public Region Region { get; }
    }

    [Equatable]
    private partial class Region
    {
        public Region((int X, int Y, int Z) start, (int X, int Y, int Z) end)
        {
            Start = start;
            End = end;
        }

        public (int X, int Y, int Z) Start { get; }

        public (int X, int Y, int Z) End { get; }

        public long Volume => 1L * (End.X - Start.X + 1) * (End.Y - Start.Y + 1) * (End.Z - Start.Z + 1);

        public override string ToString()
        {
            return $"({Start.X},{Start.Y},{Start.Z}) => ({End.X},{End.Y},{End.Z})";
        }

        /// <summary>
        /// Get the set of regions (if any) that remain after removing the provided region
        /// Will be the original region if no overlap, empty if totally overlapped,
        /// multiple (1 - 6) regions if partially overlapped
        /// </summary>
        /// <param name="other">The region to remove</param>
        /// <returns>The remaining regions</returns>
        public IEnumerable<Region> Except(Region other)
        {
            var overlap = Intersect(other);
            if (overlap is null)
            {
                yield return this;
            }
            else if (overlap.Equals(this))
            {
                yield break;
            }
            else
            {
                (int startX, int startY, int startZ) = Start;
                (int endX, int endY, int endZ) = End;

                if (overlap.Start.X > startX)
                {
                    yield return new Region((startX, startY, startZ), (overlap.Start.X - 1, endY, endZ));
                    startX = overlap.Start.X;
                }

                if (overlap.End.X < endX)
                {
                    yield return new Region((overlap.End.X + 1, startY, startZ), (endX, endY, endZ));
                    endX = overlap.End.X;
                }

                if (overlap.Start.Y > startY)
                {
                    yield return new Region((startX, startY, startZ), (endX, overlap.Start.Y - 1, endZ));
                    startY = overlap.Start.Y;
                }

                if (overlap.End.Y < endY)
                {
                    yield return new Region((startX, overlap.End.Y + 1, startZ), (endX, endY, endZ));
                    endY = overlap.End.Y;
                }

                if (overlap.Start.Z > startZ)
                {
                    yield return new Region((startX, startY, startZ), (endX, endY, overlap.Start.Z - 1));
                }

                if (overlap.End.Z < endZ)
                {
                    yield return new Region((startX, startY, overlap.End.Z + 1), (endX, endY, endZ));
                }
            }
        }

        public Region? Intersect(Region other)
        {
            // no overlap if shifted entirely in at least one dimension
            if (other.End.X < this.Start.X
                || other.End.Y < this.Start.Y
                || other.End.Z < this.Start.Z
                || other.Start.X > this.End.X
                || other.Start.Y > this.End.Y
                || other.Start.Z > this.End.Z)
            {
                return null;
            }

            (int X, int Y, int Z) overlapStart = (Math.Max(this.Start.X, other.Start.X), Math.Max(this.Start.Y, other.Start.Y), Math.Max(this.Start.Z, other.Start.Z));
            (int X, int Y, int Z) overlapEnd = (Math.Min(this.End.X, other.End.X), Math.Min(this.End.Y, other.End.Y), Math.Min(this.End.Z, other.End.Z));

            return new Region(overlapStart, overlapEnd);
        }
    }
}
