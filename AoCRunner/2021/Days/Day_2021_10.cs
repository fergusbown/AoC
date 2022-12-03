namespace AoCRunner;

internal class Day_2021_10 : IDayChallenge
{
    private readonly string[] inputData;

    public Day_2021_10(string inputData)
    {
        this.inputData = inputData.StringsForDay();
    }

    public string Part1()
    {
        return inputData
            .Select(s => GetLineState(s))
            .Where(s => s.State == LineState.Corrupt)
            .Select(s => s.Score)
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        long[] scores = inputData
            .Select(s => GetLineState(s))
            .Where(s => s.State == LineState.Incomplete)
            .Select(s => s.Score)
            .OrderBy(s => s)
            .ToArray();

        return scores[scores.Length / 2].ToString();
    }

    private (LineState State, long Score) GetLineState(string input)
    {
        Stack<char> openings = new();

        foreach (char ch in input)
        {
            if (closingToOpening.TryGetValue(ch, out var opening))
            {
                if (openings.Count == 0 || openings.Pop() != opening)
                {
                    return (LineState.Corrupt, closingToScore[ch]);
                }
            }
            else
            {
                openings.Push(ch);
            }
        }

        if (openings.Count == 0)
        {
            return (LineState.Legal, 0);
        }
        else
        {
            long score = 0;
            while (openings.TryPop(out var opening))
            {
                score *= 5;
                score += openingToScore[opening];
            }

            return (LineState.Incomplete, score);
        }
    }

    private readonly Dictionary<char, char> closingToOpening = new()
    {
        { ')', '(' },
        { ']', '[' },
        { '}', '{' },
        { '>', '<' },
    };

    private readonly Dictionary<char, int> closingToScore = new()
    {
        { ')', 3 },
        { ']', 57 },
        { '}', 1197 },
        { '>', 25137 },
    };

    private readonly Dictionary<char, int> openingToScore = new()
    {
        { '(', 1 },
        { '[', 2 },
        { '{', 3 },
        { '<', 4 },
    };

    private enum LineState
    {
        Legal,
        Corrupt,
        Incomplete
    }
}