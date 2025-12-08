using System.Collections;
using System.Collections.Immutable;

namespace AoCRunner
{
    internal class RangesSet : IImmutableSet<int>
    {
        private readonly IReadOnlyCollection<SimpleRange<int>> ranges;

        public static IImmutableSet<int> Empty { get; } = new RangesSet();

        public RangesSet(int start, int end)
        {
            this.ranges = new[] { new SimpleRange<int>(start, end) };
        }

        public RangesSet(params SimpleRange<int>[] ranges)
        {
            bool haveOverlap = false;
            for (int i = 0; i < ranges.Length; i++)
            {
                var a = ranges[i];
                for (int j = i + 1; j < ranges.Length; j++)
                {
                    var b = ranges[j];

                    if (a.Overlaps(b))
                    {
                        haveOverlap = true;
                    }
                }
            }

            if (haveOverlap)
            {
                this.ranges = Merge(ranges);
            }
            else
            {
                this.ranges = ranges;
            }
        }

        private RangesSet(IReadOnlyCollection<SimpleRange<int>> ranges)
        {
            this.ranges = ranges;
        }

        public int Count
        {
            get
            {
                int result = 0;

                foreach (var range in this.ranges)
                {
                    result += range.End - range.Start;
                }

                return result;
            }
        }

        public IImmutableSet<int> Add(int value)
        {
            if (Contains(value))
            {
                return this;
            }

            IReadOnlyCollection<SimpleRange<int>> newRanges = this.ranges
                .Append(new SimpleRange<int>(value, value + 1))
                .ToArray();

            return new RangesSet(newRanges);
        }

        public IImmutableSet<int> Remove(int value)
        {
            if (!Contains(value))
            {
                return this;
            }

            SimpleRange<int> toRemove = new(value, value + 1);

            IReadOnlyCollection<SimpleRange<int>> newRanges = this.ranges
                .SelectMany(r => r.Except(toRemove))
                .ToArray();

            return new RangesSet(newRanges);
        }

        public IImmutableSet<int> Clear() => Empty;

        public bool Contains(int item)
        {
            foreach (var range in this.ranges)
            {
                if (item >= range.Start && item < range.End)
                {
                    return true;
                }
            }

            return false;
        }

        public IImmutableSet<int> Except(IEnumerable<int> other)
        {
            if (other is RangesSet otherRanges)
            {
                IReadOnlyCollection<SimpleRange<int>> remaining = this.ranges;

                foreach (var toRemove in otherRanges.ranges)
                {
                    remaining = remaining.SelectMany(r => r.Except(toRemove)).ToArray();
                }

                return new RangesSet(remaining);
            }

            return this.ToImmutableHashSet().Except(other);
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (SimpleRange<int> range in this.ranges)
            {
                for (int i = range.Start; i < range.End; i++)
                {
                    yield return i;
                }
            }
        }

        public IImmutableSet<int> Intersect(IEnumerable<int> other)
        {
            if (other is RangesSet otherRanges)
            {
                if (otherRanges.ranges.Count == 0)
                {
                    return otherRanges;
                }

                List<SimpleRange<int>>? intersection = null;

                foreach (var range in this.ranges)
                {
                    foreach (var otherRange in otherRanges.ranges)
                    {
                        if (range.TryOverlap(otherRange, out var overlap))
                        {
                            intersection ??= new List<SimpleRange<int>>(1);
                            intersection.Add(overlap);
                        }
                    }
                }

                return intersection is null ? Empty : new RangesSet(intersection);
            }

            return this.ToImmutableHashSet().Intersect(other);
        }

        public bool IsProperSubsetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<int> other)
        {
            if (other is RangesSet otherRanges)
            {
                foreach (var range in this.ranges)
                {
                    foreach (var otherRange in otherRanges.ranges)
                    {
                        if (range.TryOverlap(otherRange, out var overlap))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (var item in other)
                {
                    if (Contains(item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool SetEquals(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public IImmutableSet<int> SymmetricExcept(IEnumerable<int> other)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(int equalValue, out int actualValue)
        {
            throw new NotImplementedException();
        }

        public IImmutableSet<int> Union(IEnumerable<int> other)
        {
            if (other is RangesSet otherRanges)
            {
                return new RangesSet(Merge(this.ranges.Concat(otherRanges.ranges)));
            }

            return ImmutableHashSet.CreateRange(this).Union(other);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static IReadOnlyCollection<SimpleRange<int>> Merge(IEnumerable<SimpleRange<int>> ranges)
        {
            List<SimpleRange<int>> pending = new(ranges);
            List<SimpleRange<int>> mergedSet = new(pending.Count);

            while (pending.Count > 0)
            {
                var merging = pending[0];
                pending.RemoveAt(0);

                for (int i = pending.Count - 1; i >= 0; i--)
                {
                    var testing = pending[i];

                    if (merging.TryMerge(testing, out var merged))
                    {
                        pending.RemoveAt(i);
                        merging = merged;
                    }
                    else
                    {
                        continue;
                    }
                }

                mergedSet.Add(merging);
            }

            return mergedSet;
        }
    }
}
