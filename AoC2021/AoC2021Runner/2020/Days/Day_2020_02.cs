namespace AoC2021Runner;

internal class Day_2020_02 : IDayChallenge
{
    private readonly (int MinOccurences, int MaxOccurences, char Character, string Password)[] inputData;

    public Day_2020_02(string inputData)
    {
        this.inputData = inputData
            .StringsForDay()
            .Select(s => s.Split(new char[] { ' ', '-', ':' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(s =>
            {
                (int MinOccurences, int MaxOccurences, char Character, string Password) result = (int.Parse(s[0]), int.Parse(s[1]), s[2][0], s[3]);
                return result;
            })
            .ToArray();
    }

    public string Part1()
    {
        return inputData.Where(d =>
        {
            var characterCount = d.Password.Count(c => c == d.Character);
            return characterCount >= d.MinOccurences && characterCount <= d.MaxOccurences;
        })
        .Count()
        .ToString();
    }

    public string Part2()
    {
        return inputData.Where(d =>
        {
            if (d.Password.Length >= d.MaxOccurences)
            {
                bool firstCharacterMaches = d.Password[d.MinOccurences - 1] == d.Character;
                bool secondCharacterMatches = d.Password[d.MaxOccurences - 1] == d.Character;

                return (firstCharacterMaches && !secondCharacterMatches) || (secondCharacterMatches && !firstCharacterMaches);
            }

            return false;
        })
        .Count()
        .ToString();

    }
}