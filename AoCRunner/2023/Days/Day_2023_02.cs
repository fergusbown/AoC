namespace AoCRunner;

internal class Day_2023_02 : IDayChallenge
{
    private readonly IReadOnlyCollection<Game> inputData;

    public Day_2023_02(string inputData)
    {
        this.inputData = Parse(inputData);
    }

    public string Part1()
    {
        return inputData
            .Where(g => g.Rounds.All(r => r.Red <= 12 && r.Green <= 13 && r.Blue <= 14))
            .Select(g => g.Id)
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        return inputData
            .Select(g => g.Rounds.Max(r => r.Red) * g.Rounds.Max(r => r.Green) * g.Rounds.Max(r => r.Blue))
            .Sum()
            .ToString();
    }

    private static IReadOnlyCollection<Game> Parse(string inputData)
    {
        List<Game> result = new();

        foreach (string gameLine in inputData.StringsForDay())
        {
            var parts = gameLine.Split(": ");
            int id = int.Parse(parts[0].Substring(4));
            List<Round> rounds = new();

            foreach(string roundString in parts[1].Split("; "))
            {
                int red = 0;
                int green = 0;
                int blue = 0;

                foreach(string cubeString in roundString.Split(", "))
                {
                    if (!ParseCube(cubeString, " red", ref red) && !ParseCube(cubeString, " green", ref green) && !ParseCube(cubeString, " blue", ref blue))
                    {
                        throw new InvalidOperationException("Couldn't parse");
                    }
                }

                rounds.Add(new Round(red, green, blue));
            }

            result.Add(new Game(id, rounds));
        }

        return result;

        static bool ParseCube(string cubeString, string match, ref int parsed)
        {
            if (cubeString.EndsWith(match))
            {
                parsed = int.Parse(cubeString.Replace(match, ""));
                return true;
            }

            return false;
        }
    }

    private record Round(
        int Red,
        int Green,
        int Blue);

    private record Game(
        int Id,
        IReadOnlyList<Round> Rounds);
}
