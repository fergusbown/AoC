namespace AoCRunner;

internal class Day_2020_05 : IDayChallenge
{
    private readonly string[] inputData;

    public Day_2020_05(string inputData)
    {
        this.inputData = inputData.StringsForDay();
    }

    public string Part1()
    {
        return inputData
            .Select(s => GetSeatNumber(s))
            .Max()
            .ToString();
    }

    public string Part2()
    {
        HashSet<int> seatNumbers = new(inputData.Select(s => GetSeatNumber(s)));

        var missingSeat = seatNumbers
            .Where(sn => !seatNumbers.Contains(sn + 1) && seatNumbers.Contains(sn + 2))
            .Single() + 1;
        return $"{missingSeat}";
    }

    private static int GetSeatNumber(string details)
    {
        return Resolve(details[0..7], 127) * 8 + Resolve(details[7..], 8);

        static int Resolve(string details, int max)
        {
            int lowerBound = 0;
            int upperBound = max;
            int keepAmount = max + 1;

            foreach (var ch in details)
            {
                keepAmount /= 2;

                switch (ch)
                {
                    case 'F':
                    case 'L':
                        upperBound -= keepAmount;
                        break;
                    case 'B':
                    case 'R':
                        lowerBound += keepAmount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(details));
                }
            }

            return lowerBound;
        }
    }
}