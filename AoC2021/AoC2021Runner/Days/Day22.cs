using System.Diagnostics.CodeAnalysis;

namespace AoC2021Runner;

internal class Day22 : IDayChallenge
{
    private readonly IReadOnlyList<Instruction> instructions;

    public Day22(string inputData)
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
            var parts = text.Split(new String[] { " x=", "..", ",y=", ",z=" }, StringSplitOptions.None);
            TurnOn = parts[0] == "on";
            var start = (int.Parse(parts[1]), int.Parse(parts[3]), int.Parse(parts[5]));
            var end = (int.Parse(parts[2]), int.Parse(parts[4]), int.Parse(parts[6]));
            this.Region = new Region(start, end);

            this.IsInitialisationSequence =
                Math.Abs(Region.Start.X) <= 50 &&
                Math.Abs(Region.Start.Y) <= 50 &&
                Math.Abs(Region.Start.Z) <= 50 &&
                Math.Abs(Region.End.X) <= 50 &&
                Math.Abs(Region.End.Y) <= 50 &&
                Math.Abs(Region.End.Z) <= 50;
        }

        public bool IsInitialisationSequence { get; }
        public bool TurnOn { get; }

        public Region Region { get; }
    }

    private class Region : IEquatable<Region>
    {
        public Region((int X, int Y, int Z) start, (int X, int Y, int Z) end)
        {
            Start = start;
            End = end;
        }

        public (int X, int Y, int Z) Start { get; }

        public (int X, int Y, int Z) End { get; }

        public long Volume => 1L * (End.X - Start.X + 1) * (End.Y - Start.Y + 1) * (End.Z - Start.Z + 1);

        public bool Equals(Region? other)
        {
            if (other is null)
            {
                return false;
            }

            return other.Start == this.Start && other.End == this.End;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj as Region);
        }

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
                (int remainingStartX, int remainingStartY, int remainingStartZ) = Start;
                (int remainingEndX, int remainingEndY, int remainingEndZ) = End;

                if (overlap.Start.X > remainingStartX)
                {
                    yield return new Region((remainingStartX, remainingStartY, remainingStartZ), (overlap.Start.X - 1, remainingEndY, remainingEndZ));
                    remainingStartX = overlap.Start.X;
                }

                if (overlap.End.X < remainingEndX)
                {
                    yield return new Region((overlap.End.X + 1, remainingStartY, remainingStartZ), (remainingEndX, remainingEndY, remainingEndZ));
                    remainingEndX = overlap.End.X;
                }

                if (overlap.Start.Y > remainingStartY)
                {
                    yield return new Region((remainingStartX, remainingStartY, remainingStartZ), (remainingEndX, overlap.Start.Y - 1, remainingEndZ));
                    remainingStartY = overlap.Start.Y;
                }

                if (overlap.End.Y < remainingEndY)
                {
                    yield return new Region((remainingStartX, overlap.End.Y + 1, remainingStartZ), (remainingEndX, remainingEndY, remainingEndZ));
                    remainingEndY = overlap.End.Y;
                }

                if (overlap.Start.Z > remainingStartZ)
                {
                    yield return new Region((remainingStartX, remainingStartY, remainingStartZ), (remainingEndX, remainingEndY, overlap.Start.Z - 1));
                }

                if (overlap.End.Z < remainingEndZ)
                {
                    yield return new Region((remainingStartX, remainingStartY, overlap.End.Z + 1), (remainingEndX, remainingEndY, remainingEndZ));
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
