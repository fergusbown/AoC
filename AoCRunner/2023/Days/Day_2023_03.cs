using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2023_03 : IDayChallenge
{
    private readonly char[,] inputData;


    public Day_2023_03(string inputData)
    {
        this.inputData = inputData
            .GridForDay(c => c)
            .ToArray();
    }

    public string Part1()
    {
        (var parts, var symbols) = Parse();

        return parts
            .Where(p => symbols.Any(p.IsAdjacent))
            .Select(p => p.Number)
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        (var parts, var symbols) = Parse();

        return symbols
            .Where(s => s.IsGear)
            .Select(s => parts.Where(p => p.IsAdjacent(s)).ToArray())
            .Where(g => g.Length == 2)
            .Select(g => g[0].Number * g[1].Number)
            .Sum()
            .ToString();
    }

    private (IReadOnlyCollection<Part> Numbers, IReadOnlyCollection<Symbol> Symbols) Parse()
    {
        Span2D<char> grid = new(this.inputData);
        List<Part> parts = new();
        List<Symbol> symbols = new();

        int number = 0;
        int? numberStart = null;
        int numberEnd = 0;

        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                char ch = grid[y, x];

                if (char.IsDigit(ch))
                {
                    number = (number * 10) + (ch - '0');
                    numberStart ??= x;
                    numberEnd = x;
                }
                else
                {
                    HandleNumber(y);

                    if (IsSymbol(ch))
                    {
                        symbols.Add(new Symbol(ch, y, x));
                    }
                }
            }

            HandleNumber(y);
        }

        HandleNumber(grid.Height - 1);

        return (parts, symbols);

        static bool IsSymbol(char ch) => !Char.IsDigit(ch) && ch != '.';

        void HandleNumber(int row)
        {
            if (numberStart is not null)
            {
                parts.Add(new Part(number, row, numberStart.Value, numberEnd));
                number = 0;
                numberStart = null;
            }
        }

    }

    private record Part(int Number, int Row, int Start, int End)
    {
        public bool IsAdjacent(Symbol symbol)
        {
            return symbol.Row >= Row - 1 && symbol.Row <= Row + 1 && symbol.Column >= Start - 1 && symbol.Column <= End + 1;
        }
    }

    private record Symbol(char Character, int Row, int Column)
    {
        public bool IsGear => Character == '*';
    }
}
