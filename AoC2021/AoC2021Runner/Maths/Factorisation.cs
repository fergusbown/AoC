using System.Collections.Immutable;

namespace AoC2021Runner
{
    internal static class Factorisation
    {
        public static IReadOnlyCollection<int> CalculatePrimes(int max)
        {
            List<int> primes = new();

            for (int i = 3; i <= max; i += 2)
            {
                bool isPrime = true;
                foreach (var prime in primes)
                {
                    if (i % prime == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                {
                    primes.Add(i);
                }
            }

            primes.Insert(0, 2);
            return primes;
        }

        public static ImmutableDictionary<int, int> GetPrimeFactors(int number)
            => GetPrimeFactors(CalculatePrimes(number), number);

        public static ImmutableDictionary<int, int> GetPrimeFactors(IReadOnlyCollection<int> primes, int number)
        {
            var factors = new Dictionary<int, int>();

            if (number == 0)
            {
                return factors.ToImmutableDictionary();
            }

            foreach (var prime in primes)
            {
                if (number == 1)
                {
                    break;
                }

                int count = 0;

                while (number % prime == 0)
                {
                    number /= prime;
                    count++;
                }

                if (count > 0)
                {
                    factors.Add(prime, count);
                }
            }

            return factors.ToImmutableDictionary();
        }

        public static long GetLowestCommonMultiple(IReadOnlyCollection<int> numbers)
            => GetLowestCommonMultiple(CalculatePrimes(numbers.Max()), numbers);

        public static long GetLowestCommonMultiple(IReadOnlyCollection<int> primes, IReadOnlyCollection<int> numbers)
        {
            var primeFactors = numbers
                .Select(n => GetPrimeFactors(primes, n))
                .ToArray();

            Dictionary<int, int> maxPrimeFactors = new();

            foreach (var factors in primeFactors)
            {
                foreach (var (factor, count) in factors)
                {
                    if (!maxPrimeFactors.TryGetValue(factor, out var existingCount) || count > existingCount)
                    {
                        maxPrimeFactors[factor] = count;
                    }
                }
            }

            long result = 1;
            foreach (var (factor, count) in maxPrimeFactors)
            {
                result *= factor * count;
            }

            return result;
        }

        public static (int Top, int Bottom) ReduceFraction(int top, int bottom, IReadOnlyCollection<int> primes)
        {
            var topFactors = GetPrimeFactors(primes, top);
            var bottomFactors = GetPrimeFactors(primes, bottom);

            var commonFactors = topFactors.Keys.Intersect(bottomFactors.Keys);
            int largestCommonFactor = 1;

            foreach (var factor in commonFactors)
            {
                int numberInCommon = Math.Min(topFactors[factor], bottomFactors[factor]);

                for (int i = 0; i < numberInCommon; i++)
                {
                    largestCommonFactor *= factor;
                }
            }

            return (top / largestCommonFactor, bottom / largestCommonFactor);
        }
    }
}
