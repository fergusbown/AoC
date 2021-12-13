namespace AoC2021Runner
{
    internal class Day01 : IDayChallenge
    {
        private readonly int[] inputData;

        public Day01(string inputData)
        {
            this.inputData = inputData.IntsForDay();
        }

        public string Part1()
            => IncreasingCount(inputData).ToString();

        public string Part2()
            => IncreasingCount(GetWindowSums(inputData, 3)).ToString();

        private static int[] GetWindowSums(int[] input, int windowSize)
        {
            int[] windows = new int[input.Length - windowSize + 1];

            for (int i = 0; i < windows.Length; i++)
            {
                windows[i] = input[i];

                for (int j = 1; j < windowSize; j++)
                {
                    windows[i] += input[i + j];
                }
            }

            return windows;
        }

        private static int IncreasingCount(int[] input)
        {
            int previous = input[0];
            int increasingCount = 0;

            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] > previous)
                {
                    increasingCount++;
                }

                previous = input[i];
            }

            return increasingCount;
        }
    }
}
