namespace AoC2021Runner;

internal class Day_2022_02 : IDayChallenge
{
    private readonly (char, char)[] inputData;

    private static readonly Dictionary<Implement, (Implement Beats, Implement LosesTo)> rules = new()
    {
        { Implement.Rock, (Implement.Scissors, Implement.Paper) },
        { Implement.Paper, (Implement.Rock, Implement.Scissors) },
        { Implement.Scissors, (Implement.Paper, Implement.Rock) },
    };

    public Day_2022_02(string inputData)
    {
        this.inputData = inputData
            .StringsForDay()
            .Select(s => (s[0], s[2]))
            .ToArray();
    }

    public string Part1()
        => this.inputData.Select(d => IncorrectStrategyToScore(d.Item1, d.Item2)).Sum().ToString();

    public string Part2()
        => this.inputData.Select(d => CorrectStrategyToScore(d.Item1, d.Item2)).Sum().ToString();

    private static Implement CharToImplement(char ch)
    {
        return ch switch
        {
            'A' or 'X' => Implement.Rock,
            'B' or 'Y' => Implement.Paper,
            _ => Implement.Scissors,
        };
    }

    private static Outcome CharToOutcome(char ch)
    {
        return ch switch
        {
            'X' => Outcome.Loss,
            'Y' => Outcome.Draw,
            _ => Outcome.Win,
        };

    }

    private static int IncorrectStrategyToScore(char opponent, char you)
    {
        Implement opponentPlay = CharToImplement(opponent);
        Implement yourPlay = CharToImplement(you);

        Outcome outcome;

        if (rules[yourPlay].Beats == opponentPlay)
        {
            outcome = Outcome.Win;
        }
        else if (rules[yourPlay].LosesTo == opponentPlay)
        {
            outcome = Outcome.Loss;
        }
        else
        {
            outcome = Outcome.Draw;
        }

        return (int)yourPlay + (int)outcome;
    }

    private static int CorrectStrategyToScore(char opponent, char outcome)
    {
        Implement opponentPlay = CharToImplement(opponent);
        Outcome requiredOutcome = CharToOutcome(outcome);

        Implement yourPlay;
        if (requiredOutcome == Outcome.Win)
        {
            yourPlay = rules[opponentPlay].LosesTo;
        }
        else if (requiredOutcome == Outcome.Loss)
        {
            yourPlay = rules[opponentPlay].Beats;
        }
        else
        {
            yourPlay = opponentPlay;
        }

        return (int)yourPlay + (int)requiredOutcome;
    }

    private enum Outcome
    {
        Loss = 0,
        Draw = 3,
        Win = 6,
    }

    private enum Implement
    {
        Rock = 1,
        Paper = 2,
        Scissors = 3,
    }
}
