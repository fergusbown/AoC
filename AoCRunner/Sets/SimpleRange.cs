using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AoCRunner
{
    /// <summary>
    /// A range from <paramref name="Start"/> (inclusive) to <paramref name="End"/> (exclusive)
    /// </summary>
    /// <param name="Start">The first element in the range (inclusive)</param>
    /// <param name="End">The last element in the range (exclsive)</param>
    internal sealed record SimpleRange<T>(T Start, T End)
        where T : INumber<T>
    {
        public bool TryMerge(SimpleRange<T> second, [NotNullWhen(true)] out SimpleRange<T>? merged)
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
                T.Min(this.Start, second.Start),
                T.Max(this.End, second.End));

            return true;
        }

        public IEnumerable<SimpleRange<T>> Except(SimpleRange<T> remove)
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

        public bool TryOverlap(SimpleRange<T> with, [NotNullWhen(true)] out SimpleRange<T>? overlap)
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
                T.Max(this.Start, with.Start),
                T.Min(this.End, with.End));

            return true;
        }

        public bool Overlaps(SimpleRange<T> with)
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

        public T Count => this.End - this.Start;

        public bool Contains(T value) => value >= this.Start && value < this.End;
    }
}
