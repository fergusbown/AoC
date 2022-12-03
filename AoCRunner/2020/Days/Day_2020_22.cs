using Generator.Equals;
using System.Diagnostics.CodeAnalysis;

namespace AoCRunner;

internal partial class Day_2020_22 : IDayChallenge
{
    private (int[] Player1, int[] Player2) players;

    public Day_2020_22(string inputData)
    {
        this.players = Parse(inputData);
        static (int[] Player1, int[] Player2) Parse(string inputData)
        {
            var player = inputData.Split($"{Environment.NewLine}{Environment.NewLine}");
            var player1 = player[0].StringsForDay().Skip(1).Select(x => int.Parse(x)).ToArray();
            var player2 = player[1].StringsForDay().Skip(1).Select(x => int.Parse(x)).ToArray();

            return (player1, player2);
        }
    }

    public string Part1()
    {
        return Play(this.players.Player1, this.players.Player2).ToString();
    }

    public string Part2()
    {
        return PlayRecursive(this.players.Player1, this.players.Player2).ToString(); ;
    }

    static long Play(int[] player1, int[] player2)
    {
        GameState state = new(player1, player2);

        while(state.Draw(out int player1Card, out int player2Card, out _))
        {
            state.EndRound(player1Card, player2Card);
        }
        
        return state.WinningScore();
    }

    static long PlayRecursive(int[] player1, int[] player2)
    {
        GameState state = new(player1, player2);
        _ = Play(state);

        return state.WinningScore();

        static bool Play(GameState state)
        {
            HashSet<GameState> previousRounds = new();

            while (true)
            {
                if (previousRounds.Contains(state))
                {
                    return true;
                }

                previousRounds.Add(state.Clone());

                if (!state.Draw(out int player1Card, out int player2Card, out bool player1Wins))
                {
                    return player1Wins;
                }

                if (state.CanRecurse(player1Card, player2Card, out GameState? subGameState))
                {
                    state.EndRound(player1Card, player2Card, Play(subGameState));
                }
                else
                {
                    state.EndRound(player1Card, player2Card);
                }
            }
        }
    }

    [Equatable]
    private partial class GameState
    {
        [OrderedEquality]
        private Queue<int> player1Deck { get; }

        [OrderedEquality]
        private Queue<int> player2Deck { get; }

        public GameState(IEnumerable<int> player1Deck, IEnumerable<int> player2Deck)
        {
            this.player1Deck = new(player1Deck);
            this.player2Deck = new(player2Deck);
        }

        public bool Draw(out int player1Card, out int player2Card, out bool player1Wins)
        {
            if (player1Deck.Count == 0 || player2Deck.Count == 0)
            {
                player1Card = player2Card = 0;
                player1Wins = player1Deck.Count > 0;
                return false;
            }

            player1Card = player1Deck.Dequeue();
            player2Card = player2Deck.Dequeue();
            player1Wins = false;
            return true;
        }

        public long WinningScore()
        {
            var winningDeck = player1Deck.Count == 0 ? player2Deck : player1Deck;
            var multiplier = winningDeck.Count;
            long winningScore = 0;

            while (winningDeck.TryDequeue(out var card))
            {
                winningScore += card * multiplier--;
            }

            return winningScore;
        }

        public bool CanRecurse(int player1Card, int player2Card, [NotNullWhen(true)] out GameState? subGameState)
        {
            if (player1Deck.Count >= player1Card && player2Deck.Count >= player2Card)
            {
                subGameState = new(player1Deck.Take(player1Card), player2Deck.Take(player2Card));
                return true;
            }
            else
            {
                subGameState = null;
                return false;
            }
        }

        public GameState Clone()
        {
            return new(player1Deck, player2Deck);
        }

        public void EndRound(int player1Card, int player2Card, bool? player1Wins = null)
        {
            if (player1Wins ?? player1Card > player2Card)
            {
                player1Deck.Enqueue(player1Card);
                player1Deck.Enqueue(player2Card);
            }
            else
            {
                player2Deck.Enqueue(player2Card);
                player2Deck.Enqueue(player1Card);
            }
        }
    }
}
