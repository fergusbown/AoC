namespace AoCRunner;

internal partial class Day_2019_16 : IDayChallenge
{
    private readonly int[] inputData;

    public Day_2019_16(string inputData)
    {
        this.inputData = inputData.Select(d => d - '0').ToArray();
    }

    public string Part1()
    {
        int[] values = CopyInputData(this.inputData, 1);
        int[] buffer = new int[values.Length];

        for (int phase = 0; phase < 100; phase++)
        {
            for (int rowIndex = 0; rowIndex < values.Length; rowIndex++)
            {
                int result = 0;

                for (int columnIndex = 0; columnIndex < values.Length; columnIndex++)
                {
                    result += values[columnIndex] * Cofficient(rowIndex, columnIndex);
                }

                buffer[rowIndex] = Math.Abs(result) % 10;
            }

            (buffer, values) = (values, buffer);
        }

        return String.Join("", values.Take(8));

        static int Cofficient(int rowIndex, int columnIndex)
        {
            return (((columnIndex + 1) / (rowIndex + 1)) % 4) switch
            {
                0 or 2 => 0,
                1 => 1,
                _ => -1,
            };
        }
    }

    public string Part2()
    {
        // something something the coefficients cancel out for the latter half of the data
        // not entirely sure i understand why but I've lost interest now

        int[] values = CopyInputData(this.inputData, 10_000);
        int[] buffer = new int[values.Length];
        int offset = int.Parse(String.Join("", values.Take(7)));

        for (int phase = 0; phase < 100; phase++)
        {
            int sum = values.Skip(offset).Sum();

            for (int k = offset; k < values.Length; k++)
            {
                buffer[k] = sum % 10;
                sum -= values[k];
            }

            (buffer, values) = (values, buffer);
        }

        return String.Join("", values.Skip(offset).Take(8));
    }

    private static int[] CopyInputData(int[] original, int repeats)
    {
        int[] result = new int[original.Length * repeats];

        for (int i = 0; i < repeats; i++)
        {
            Array.Copy(original, 0, result, original.Length * i, original.Length);
        }

        return result;
    }
}
