using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Enumerables;

namespace AoC2021Runner
{
    internal class Day04 : IDayChallenge
    {
        private readonly string inputData;

        public Day04(string inputData)
        {
            this.inputData = inputData;
        }

        public string Part1()
        {
            (IReadOnlyCollection<int> calls, IList<Board> boards) = GetInput(inputData);

            foreach (int call in calls)
            {
                foreach (Board board in boards)
                {
                    int score = board.Play(call);

                    if (score > 0)
                    {
                        return score.ToString();
                    }
                }
            }

            return "Shit";
        }

        public string Part2()
        {
            (IReadOnlyCollection<int> calls, IList<Board> boards) = GetInput(inputData);

            foreach (int call in calls)
            {
                for (int i = boards.Count - 1; i >= 0; i--)
                {
                    var board = boards[i];
                    int score = board.Play(call);

                    if (score > 0)
                    {
                        if (boards.Count == 1)
                        {
                            return score.ToString();
                        }
                        else
                        {
                            boards.RemoveAt(i);
                        }
                    }

                }
            }

            return "Shit";
        }

        private static (IReadOnlyCollection<int> Calls, IList<Board> Boards) GetInput(string data)
        {
            var inputPieces = data.Split($"{Environment.NewLine}{Environment.NewLine}");
            var calls = inputPieces[0].Split(',').Select(x => int.Parse(x)).ToArray();
            List<Board> boards = new();

            foreach (var boardText in inputPieces.Skip(1))
            {
                boards.Add(new Board(boardText));
            }

            return (calls, boards);
        }

        private class Board
        {
            private readonly int[] boardValues;
            private readonly bool[] calledValues;


            public Board(string boardText)
            {
                this.boardValues = boardText.Split(new String[] { " ", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => int.Parse(v)).ToArray();
                this.calledValues = Enumerable.Repeat(false, this.boardValues.Length).ToArray();
            }

            public int Play(int call)
            {
                Span2D<int> board = new(this.boardValues, 5, 5);
                Span2D<bool> calls = new(this.calledValues, 5, 5);

                if (IsBingo(call, board, calls))
                {
                    return GetScore(call, board, calls);
                }
                else
                {
                    return 0;
                }
            }

            private static bool IsBingo(int call, Span2D<int> board, Span2D<bool> calls)
            {
                for (int rowIndex = 0; rowIndex < board.Height; rowIndex++)
                {
                    for (int columnindex = 0; columnindex < board.Width; columnindex++)
                    {
                        if (board[rowIndex, columnindex] == call)
                        {
                            calls[rowIndex, columnindex] = true;
                            return IsBingo(calls.GetRow(rowIndex)) || IsBingo(calls.GetColumn(columnindex));
                        }
                    }
                }

                return false;

                static bool IsBingo(RefEnumerable<bool> values)
                {
                    foreach (bool called in values)
                    {
                        if (!called)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            private static int GetScore(int call, Span2D<int> board, Span2D<bool> calls)
            {
                int uncalled = 0;
                for (int rowIndex = 0; rowIndex < calls.Height; rowIndex++)
                {
                    for (int columnindex = 0; columnindex < calls.Width; columnindex++)
                    {
                        if (!calls[rowIndex, columnindex])
                        {
                            uncalled += board[rowIndex, columnindex];
                        }
                    }
                }

                return uncalled * call;
            }
        }
    }
}
