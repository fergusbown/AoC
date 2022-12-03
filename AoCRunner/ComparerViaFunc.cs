namespace AoCRunner
{
    internal static class ComparerViaFunc
    {
        public static IComparer<T> Create<T>(Func<T, T, int> comparer)
            => new ComparerImplentation<T>(comparer);

        private class ComparerImplentation<T> : IComparer<T>
        {
            private readonly Func<T, T, int> comparer;

            public ComparerImplentation(Func<T, T, int> comparer)
            {
                this.comparer = comparer;
            }

            public int Compare(T? x, T? y)
            {
                if (x is null)
                {
                    if (y is null)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }

                if (y is null)
                {
                    return 1;
                }

                return comparer(x, y);
            }
        }
    }
}
