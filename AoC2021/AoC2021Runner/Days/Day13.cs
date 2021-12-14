using System.Text;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner;

internal class Day13 : IDayChallenge
{
    private readonly bool[,] inputPaper;
    private readonly IReadOnlyCollection<(char Axis, int Position)> folds;

    public Day13(string inputData)
    {
        this.inputPaper = GetPaper(inputData, out folds).ToArray();
    }

    public string Part1()
    {
        var paper = GetPaper();

        (char axis, int position) = folds.First();
        paper = Fold(paper, axis, position);

        return DotCount(paper).ToString();
    }

    public string Part2()
    {
        var paper = GetPaper();

        foreach ((char axis, int position) in folds)
        {
            paper = Fold(paper, axis, position);
        }

        return Draw(paper);
    }

    private Span2D<bool> GetPaper()
        => new Span2D<bool>((bool[,])inputPaper.Clone());

    private static Span2D<bool> GetPaper(string input, out IReadOnlyCollection<(char Axis, int Position)> folds)
    {
        var paperAndFolds = input.Split($"{Environment.NewLine}{Environment.NewLine}");
        folds = paperAndFolds[1]
            .Split(Environment.NewLine)
            .Select(f =>
            {
                var instruction = f.Split('=');
                return (instruction[0][11], int.Parse(instruction[1]));
            })
            .ToArray();

        int maxX = 0;
        int maxY = 0;

        IReadOnlyCollection<(int X, int Y)> dots = paperAndFolds[0]
            .Split(Environment.NewLine)
            .Select(d =>
            {
                var dotParts = d.Split(',');
                (int X, int Y) dot = (int.Parse(dotParts[0]), int.Parse(dotParts[1]));

                if (dot.X > maxX)
                    maxX = dot.X;

                if (dot.Y > maxY)
                    maxY = dot.Y;

                return dot;
            })
            .ToArray();

        int width = maxX + 1;
        int height = maxY + 1;
        Span2D<bool> result = new(new bool[width * height], height, width);

        foreach ((var x, var y) in dots)
        {
            result[y, x] = true;
        }

        return result;
    }

    private static int DotCount(Span2D<bool> paper)
    {
        int dotCount = 0;

        foreach (var dot in paper)
        {
            if (dot)
            {
                dotCount++;
            }
        }

        return dotCount;
    }

    private static string Draw(Span2D<bool> paper)
    {
        StringBuilder debug = new();

        debug.AppendLine();

        for (int row = 0; row < paper.Height; row++)
        {
            for (int column = 0; column < paper.Width; column++)
            {
                debug.Append(paper[row, column] ? '#' : ' ');
            }
            debug.AppendLine();
        }

        debug.AppendLine();

        return debug.ToString();
    }

    private static Span2D<bool> Fold(Span2D<bool> paper, char axis, int position)
    {
        Span2D<bool> stationary;
        Span2D<bool> folded;

        if (axis == 'x') //folding along the vertical
        {
            stationary = paper.Slice(0, 0, paper.Height, position);
            folded = paper.Slice(0, position + 1, paper.Height, paper.Width - position - 1);

            folded.TransposeColumns();
        }
        else //folding along the horizontal
        {
            stationary = paper.Slice(0, 0, position, paper.Width);
            folded = paper.Slice(position + 1, 0, paper.Height - position - 1, paper.Width);

            folded.TransposeRows();
        }

        Span2D<bool> larger = stationary.Length > folded.Length ? stationary : folded;
        Span2D<bool> smaller = stationary.Length > folded.Length ? folded : stationary;

        //copy the smaller to the bottom right of the larger
        Span2D<bool> overlap = larger.Slice(larger.Height - smaller.Height, larger.Width - smaller.Width, smaller.Height, smaller.Width);

        for (int row = 0; row < overlap.Height; row++)
        {
            for (int column = 0; column < overlap.Width; column++)
            {
                overlap[row, column] |= smaller[row, column];
            }
        }

        return larger;
    }
}