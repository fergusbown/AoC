using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AoCRunner
{
    /// <summary>
    /// A range from <paramref name="Start"/> (inclusive) to <paramref name="End"/> (exclusive)
    /// </summary>
    /// <param name="Start">The first element in the range (inclusive)</param>
    /// <param name="End">The last element in the range (exclsive)</param>
    internal sealed record SimpleRange(int Start, int End)
    {
        public bool TryMerge(SimpleRange second, [NotNullWhen(true)] out SimpleRange? merged)
        {
            if (this.End < second.Start)
            {
                merged = null;
                return false;
            }

            if (this.Start > second.End)
            {
                merged = null;
                return false;
            }

            merged = new(
                Math.Min(this.Start, second.Start),
                Math.Max(this.End, second.End));

            return true;
        }

        public IEnumerable<SimpleRange> Except(SimpleRange remove)
        {
            if (this.End < remove.Start)
            {
                yield return this;
                yield break;
            }

            if (this.Start > remove.End)
            {
                yield return this;
                yield break;
            }

            if (this.Start >= remove.Start && this.End <= remove.End)
            {
                yield break;
            }

            if (remove.Start > this.Start)
            {
                yield return new(this.Start, remove.Start);
            }

            if (remove.End < this.End)
            {
                yield return new(remove.End, this.End);
            }
        }

        public bool TryOverlap(SimpleRange with, [NotNullWhen(true)] out SimpleRange? overlap)
        {
            overlap = null;

            if (this.End < with.Start)
            {
                return false;
            }

            if (this.Start > with.End)
            {
                return false;
            }

            if (this.Start >= with.Start && this.End <= with.End)
            {
                overlap = this;
                return true;
            }

            if (this.Start <= with.Start && this.End >= with.End)
            {
                overlap = with;
                return true;
            }

            overlap = new(
                Math.Max(this.Start, with.Start),
                Math.Min(this.End, with.End));

            return true;
        }

        public bool Overlaps(SimpleRange with)
        {
            if (this.End < with.Start)
            {
                return false;
            }

            if (this.Start > with.End)
            {
                return false;
            }

            return true;
        }
    }
}
