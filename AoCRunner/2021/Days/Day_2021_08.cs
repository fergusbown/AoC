namespace AoCRunner;

internal class Day_2021_08 : IDayChallenge
{
    private readonly IReadOnlyCollection<(string[] Input, string[] Output)> notes;

    public Day_2021_08(string inputData)
    {
        this.notes = GetInput(inputData);
    }

    public string Part1()
    {
        return notes
            .SelectMany(n => n.Output)
            .Where(n => n.Length == 2 || n.Length == 4 || n.Length == 3 || n.Length == 7)
            .Count()
            .ToString();
    }

    public string Part2()
    {
        return notes.Select(n => Decode(n.Input, n.Output)).Sum().ToString();
    }

    private static IReadOnlyCollection<(string[] Input, string[] Output)> GetInput(string input)
    {
        return input
            .StringsForDay()
            .Select(s =>
            {
                string[] inAndOut = s.Split(new string[] { " ", " | " }, StringSplitOptions.None);
                return (inAndOut.Take(10).ToArray(), inAndOut.Skip(10).ToArray());
            })
            .ToArray();
    }

    private static int Decode(string[] input, string[] output)
    {
        HashSet<char>[] inputHashSets = input.Select(i => new HashSet<char>(i)).ToArray();
        HashSet<char>[] outputHashSets = output.Select(i => new HashSet<char>(i)).ToArray();

        var one = inputHashSets.Where(h => h.Count == 2).Single();
        var four = inputHashSets.Where(h => h.Count == 4).Single();
        var seven = inputHashSets.Where(h => h.Count == 3).Single();
        var eight = inputHashSets.Where(h => h.Count == 7).Single();

        var zeroOrSixOrNine = inputHashSets.Where(h => h.Count == 6).ToList();
        var twoOrThreeOrFive = inputHashSets.Where(h => h.Count == 5).ToList();

        var three = twoOrThreeOrFive.Where(h => h.Intersect(one).Count() == 2).Single();
        twoOrThreeOrFive.Remove(three);

        var five = twoOrThreeOrFive.Where(h => h.Intersect(four.Except(one)).Count() == 2).Single();
        twoOrThreeOrFive.Remove(five);
        var two = twoOrThreeOrFive.Single();

        var six = zeroOrSixOrNine.Where(h => h.Intersect(one).Count() == 1).Single();
        zeroOrSixOrNine.Remove(six);

        var nine = zeroOrSixOrNine.Where(h => h.Intersect(four).Count() == 4).Single();
        zeroOrSixOrNine.Remove(nine);
        var zero = zeroOrSixOrNine.Single();

        HashSet<char>[] decoded = new HashSet<char>[] { zero, one, two, three, four, five, six, seven, eight, nine };

        int result = 0;
        foreach (var digit in outputHashSets)
        {
            for (int i = 0; i < decoded.Length; i++)
            {
                if (decoded[i].SetEquals(digit))
                {
                    result = (result * 10) + i;
                    break;
                }
            }
        }

        return result;
    }
}