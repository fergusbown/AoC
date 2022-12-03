namespace AoCRunner
{
    internal static class ChineseRemainderTheorem
    {
        public static long Solve(IReadOnlyList<int> n, IReadOnlyList<int> a)
        {
            long prod = n.Select(n => (long)n).Aggregate(1L, (i, j) => i * j);
            long p;
            long sm = 0;
            for (int i = 0; i < n.Count; i++)
            {
                p = prod / n[i];
                sm += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
            }
            return sm % prod;
        }

        private static int ModularMultiplicativeInverse(long a, int mod)
        {
            int b = (int)(a % mod);
            for (int x = 1; x < mod; x++)
            {
                if ((b * x) % mod == 1)
                {
                    return x;
                }
            }
            return 1;
        }
    }
}
