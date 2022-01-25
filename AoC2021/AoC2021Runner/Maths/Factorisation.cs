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

        public static IReadOnlyCollection<(int factor, int factorCount)> GetPrimeFactors(int number)
            => GetPrimeFactors(CalculatePrimes(number), number);

        public static IReadOnlyCollection<(int factor, int factorCount)> GetPrimeFactors(IReadOnlyCollection<int> primes, int number)
        {
            var factors = new List<(int, int)>();

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
                    factors.Add((prime, count));
                }
            }

            return factors;
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

    }
}
