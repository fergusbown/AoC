namespace AoC2021Runner;

internal class Day21 : IDayChallenge
{
    private readonly int player1Start;
    private readonly int player2Start;

    public Day21(string inputData)
    {
        var playerPositions = inputData.StringsForDay().Select(x => int.Parse(x[28..])).ToArray();
        player1Start = playerPositions[0];
        player2Start = playerPositions[1];
    }

    public string Part1()
    {
        IDice dice = new DeterministicDice();
        Dirac.Play(dice, new Player(1, player1Start, 1000), new Player(2, player2Start, 1000), out _, out var losingPlayer);

        return $"{losingPlayer.Score * dice.RollCount}";
    }

    private static IReadOnlyList<(int TotalRoll, int Count)> quantumRolls = QuantumRolls();

    public string Part2()
    {
        var (player1Wins, player2Wins) = Play(player1Start, 0, player2Start, 0, true);

        return Math.Max(player1Wins, player2Wins).ToString();

        static (long player1Wins, long player2Wins) Play(int player1Position, long player1Score, int player2Position, long player2Score, bool player1sGo)
        {
            long player1Wins = 0;
            long player2Wins = 0;
            foreach (var (roll, count) in quantumRolls)
            {
                var (newP1Wins, newP2Wins) = PlayRoll(player1Position, player1Score, player2Position, player2Score, player1sGo, roll);
                player1Wins += newP1Wins * count;
                player2Wins += newP2Wins * count;
            }

            return (player1Wins, player2Wins);
        }

        static (long player1Wins, long player2Wins) PlayRoll(int player1Position, long player1Score, int player2Position, long player2Score, bool player1sGo, int roll)
        {
            if (player1sGo)
            {
                if (PlayerWins(ref player1Position, ref player1Score, roll))
                {
                    return (1, 0);
                }
            }
            else
            {
                if (PlayerWins(ref player2Position, ref player2Score, roll))
                {
                    return (0, 1);
                }
            }

            return Play(player1Position, player1Score, player2Position, player2Score, !player1sGo);
        }

        static bool PlayerWins(ref int position, ref long score, int roll)
        {
            position += roll;

            if (position > 10)
            {
                position -= 10;
            }

            score += position;

            return score >= 21;
        }
    }

    static IReadOnlyList<(int TotalRoll, int Count)> QuantumRolls()
    {
        List<int> rolls = new();

        for (int i = 1; i <= 3; i++)
        {
            for (int j = 1; j <= 3; j++)
            {
                for (int k = 1; k <= 3; k++)
                {
                    rolls.Add(i + j + k);
                }
            }
        }

        return rolls.GroupBy(r => r).Select(r => (r.Key, r.Count())).ToList();
    }

    private interface IDice
    {
        int Roll();

        long RollCount { get; }
    }

    private static class Dirac
    {
        public static void Play(IDice dice, Player player1, Player player2, out Player winningPlayer, out Player losingPlayer)
        {
            while (!player1.Play(dice))
            {
                if (player2.Play(dice))
                {
                    winningPlayer = player2;
                    losingPlayer = player1;
                    return;
                }
            }

            winningPlayer = player1;
            losingPlayer = player2;
        }
    }

    private class Player
    {
        private int position = 0;
        private readonly int winningScore;

        public long Score { get; private set; }
        public int Id { get; }

        public Player(int id, int startingPosition, int winningScore)
        {
            position = startingPosition - 1;
            Id = id;
            this.winningScore = winningScore;
        }

        public bool Play(IDice dice)
        {
            int roll = dice.Roll() + dice.Roll() + dice.Roll();

            position = (position + roll) % 10;

            Score += position + 1;

            return Score >= winningScore;
        }
    }

    private class DeterministicDice : IDice
    {
        int roll = 0;

        public long RollCount { get; private set; }

        public int Roll()
        {
            if (roll == 100)
                roll = 0;

            RollCount++;

            return (++roll);
        }
    }
}
