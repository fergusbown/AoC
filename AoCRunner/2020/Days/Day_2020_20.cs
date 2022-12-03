using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2020_20 : IDayChallenge
{
    private readonly IReadOnlyCollection<JigsawPiece> jigsawPieces;

    public Day_2020_20(string inputData)
    {
        this.jigsawPieces = JigsawPiece.Build(inputData);
    }

    public string Part1()
    {
        IEnumerable<long> corners = jigsawPieces
            .Where(g => g.Adjacencies.Count == 2)
            .Select(g => g.Id);

        return $"{corners.Aggregate(1L, (x, y) => x * y)}";
    }

    public string Part2()
    {
        Span2D<bool> monster = @"                  # 
#    ##    ##    ###
 #  #  #  #  #  #   ".GridForDay(c => c == '#');

        Span2D<bool> sea = CompleteJigsaw(jigsawPieces);
        return CalculateRoughness(sea, monster).ToString();

        static Span2D<bool> CompleteJigsaw(IReadOnlyCollection<JigsawPiece> jigsawPieces)
        {
            var corner = jigsawPieces.Where(g => g.Adjacencies.Count == 2).First();

            int piecesPerEdge = (int)Math.Sqrt(jigsawPieces.Count);

            // find a valid permutation for the first corner
            Span2D<Orientation> result = new Span2D<Orientation>(new Orientation[piecesPerEdge, piecesPerEdge]);
            result[0, 0] = GetFirstCorner(corner);

            // process top row
            for (int columnIndex = 1; columnIndex < piecesPerEdge; columnIndex++)
            {
                var left = result[0, columnIndex - 1];
                var nextPiece = left.JigsawPiece.Adjacencies.SelectMany(a => a.Orientations).Single(p => p.Left == left.Right);
                result[0, columnIndex] = nextPiece;
            }

            // process left column
            for (int rowIndex = 1; rowIndex < piecesPerEdge; rowIndex++)
            {
                var above = result[rowIndex - 1, 0];
                var nextPiece = above.JigsawPiece.Adjacencies.SelectMany(a => a.Orientations).Single(p => p.Top == above.Bottom);
                result[rowIndex, 0] = nextPiece;
            }

            // fill in everything else
            for (int rowIndex = 1; rowIndex < piecesPerEdge; rowIndex++)
            {
                for (int columnIndex = 1; columnIndex < piecesPerEdge; columnIndex++)
                {
                    var above = result[rowIndex - 1, columnIndex];
                    var left = result[rowIndex, columnIndex - 1];
                    var adjacencies = above.JigsawPiece.Adjacencies.Intersect(left.JigsawPiece.Adjacencies);
                    var nextPiece = adjacencies.SelectMany(a => a.Orientations).Single(p => p.Top == above.Bottom && p.Left == left.Right);
                    result[rowIndex, columnIndex] = nextPiece;
                }
            }

            return GenerateFinalJigsaw(result);
        }

        static Orientation GetFirstCorner(JigsawPiece cornerPiece)
        {
            JigsawPiece adjacency1 = cornerPiece.Adjacencies.First();
            JigsawPiece adjacency2 = cornerPiece.Adjacencies.Last();

            return cornerPiece.Orientations.First(
                p => adjacency1.Orientations.Any(a =>
                    a.Left == p.Right) && adjacency2.Orientations.Any(a => a.Top == p.Bottom));
        }

        static Span2D<bool> GenerateFinalJigsaw(Span2D<Orientation> result)
        {
            int pieceSize = result[0, 0].JigsawPiece.Size;
            int jigsawSize = pieceSize * result.Width;
            Span2D<bool> jigsaw = new(new bool[jigsawSize, jigsawSize]);

            int resultRowIndex = 0;
            int resultRowEnd = pieceSize;
            for (int rowIndex = 0; rowIndex < result.Height; rowIndex++, resultRowIndex += pieceSize, resultRowEnd += pieceSize)
            {
                int resultColumnIndex = 0;
                int resultColumnEnd = pieceSize;
                for (int columnIndex = 0; columnIndex < result.Width; columnIndex++, resultColumnIndex += pieceSize, resultColumnEnd += pieceSize)
                {
                    if (result[rowIndex, columnIndex] is not null)
                    {
                        result[rowIndex, columnIndex].OrientedPiece().CopyTo(jigsaw[resultRowIndex..resultRowEnd, resultColumnIndex..resultColumnEnd]);
                    }
                }
            }

            return jigsaw;
        }

        static int CalculateRoughness(Span2D<bool> sea, Span2D<bool> monster)
        {
            for (int flip = 0; flip < 2; flip++)
            {
                for (int rotation = 0; rotation < 4; rotation++)
                {
                    if (RemoveSeaMonsters(sea, monster))
                    {
                        if (sea.TryGetSpan(out var span))
                        {
                            return span.Count(true);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    _ = sea.RotateRight();
                }

                _ = sea.FlipVertical();
            }

            return -2;
        }

        static bool RemoveSeaMonsters(Span2D<bool> sea, Span2D<bool> monster)
        {
            bool removedSeaMonster = false;
            int endColumnIndex = monster.Width;
            for (int columnIndex = 0; columnIndex < sea.Width - monster.Width; columnIndex++, endColumnIndex++)
            {
                int endRowIndex = monster.Height;
                for (int rowIndex = 0; rowIndex < sea.Height - monster.Height; rowIndex++, endRowIndex++)
                {
                    var testSpace = sea[rowIndex..endRowIndex, columnIndex..endColumnIndex];
                    removedSeaMonster |= RemoveSeaMonster(testSpace, monster);
                }
            }

            return removedSeaMonster;
        }

        static bool RemoveSeaMonster(Span2D<bool> testSpace, Span2D<bool> monster)
        {
            for (int columnIndex = 0; columnIndex < monster.Width; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < monster.Height; rowIndex++)
                {
                    if (monster[rowIndex, columnIndex] && !testSpace[rowIndex, columnIndex])
                    {
                        return false;
                    }
                }
            }

            for (int columnIndex = 0; columnIndex < monster.Width; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < monster.Height; rowIndex++)
                {
                    if (monster[rowIndex, columnIndex])
                    {
                        testSpace[rowIndex, columnIndex] = false;
                    }
                }
            }

            return true;
        }
    }

    private class JigsawPiece
    {
        private readonly bool[,] initialPiece;
        private readonly Dictionary<JigsawPiece, List<JigsawPiece>> adjacencies;

        private JigsawPiece(int id, bool[,] grid, Dictionary<JigsawPiece, List<JigsawPiece>> adjacencies)
        {
            this.Id = id;
            this.adjacencies = adjacencies;
            Span2D<bool> span = new(grid);
            this.initialPiece = span[1..^1, 1..^1].ToArray();

            // convert grid into an integer representing each edge
            bool[] topRow = span.GetRowSpan(0).ToArray();
            bool[] bottomRow = span.GetRowSpan(span.Height - 1).ToArray();
            bool[] leftColumn = span.GetColumn(0).ToArray();
            bool[] rightColumn = span.GetColumn(span.Width - 1).ToArray();

            var topBorder = ConvertToInt(topRow);
            var bottomBorder = ConvertToInt(bottomRow);
            var leftBorder = ConvertToInt(leftColumn);
            var rightBorder = ConvertToInt(rightColumn);

            var topBorderReversed = ConvertToInt(topRow.Reverse());
            var bottomBorderReversed = ConvertToInt(bottomRow.Reverse());
            var leftBorderReversed = ConvertToInt(leftColumn.Reverse());
            var rightBorderReversed = ConvertToInt(rightColumn.Reverse());

            List<Orientation> permutations = new List<Orientation>();
            OrientationFunc getOrientation = () => new Span2D<bool>((bool[,])this.initialPiece.Clone());
            // original + rotations
            permutations.AddRange(GetRotations(
                topBorder,
                bottomBorder,
                leftBorder,
                rightBorder,
                topBorderReversed,
                bottomBorderReversed,
                leftBorderReversed,
                rightBorderReversed,
                this,
                getOrientation));

            // flipped vertically + rotations
            permutations.AddRange(GetRotations(
                bottomBorder,
                topBorder,
                leftBorderReversed,
                rightBorderReversed,
                bottomBorderReversed,
                topBorderReversed,
                leftBorder,
                rightBorder,
                this,
                () => getOrientation().FlipVertical()));

            this.AllEdges = new int[]
            {
                topBorder,
                bottomBorder,
                leftBorder,
                rightBorder,
                topBorderReversed,
                bottomBorderReversed,
                leftBorderReversed,
                rightBorderReversed,
            };

            this.Orientations = permutations;

            static int ConvertToInt(IEnumerable<bool> border)
            {
                int result = 0;

                foreach (var val in border)
                {
                    result <<= 1;
                    result += val ? 1 : 0;
                }

                return result;
            }

            static IEnumerable<Orientation> GetRotations(
                int topBorder,
                int bottomBorder,
                int leftBorder,
                int rightBorder,
                int topBorderReversed,
                int bottomBorderReversed,
                int leftBorderReversed,
                int rightBorderReversed,
                JigsawPiece jigsawPiece,
                OrientationFunc initialOrientationFunc)
            {
                yield return new Orientation(topBorder, bottomBorder, leftBorder, rightBorder, jigsawPiece,
                    initialOrientationFunc);

                yield return new Orientation(leftBorderReversed, rightBorderReversed, bottomBorder, topBorder, jigsawPiece,
                    () => initialOrientationFunc().RotateRight());

                yield return new Orientation(bottomBorderReversed, topBorderReversed, rightBorderReversed, leftBorderReversed, jigsawPiece,
                    () => initialOrientationFunc().RotateRight().RotateRight());

                yield return new Orientation(rightBorder, leftBorder, topBorderReversed, bottomBorderReversed, jigsawPiece,
                    () => initialOrientationFunc().RotateRight().RotateRight().RotateRight());
            }
        }

        public long Id { get; }

        public int Size => this.initialPiece.GetLength(0);

        public IReadOnlyCollection<JigsawPiece> Adjacencies => this.adjacencies[this];

        public IReadOnlyList<Orientation> Orientations { get; }

        public IReadOnlyList<int> AllEdges { get; }

        public static IReadOnlyCollection<JigsawPiece> Build(string inputData)
        {
            var grids = inputData.Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries);
            var adjacencies = new Dictionary<JigsawPiece, List<JigsawPiece>>();

            var jigsawPieces = grids
                .Select(
                gridString =>
                {
                    int id = int.Parse(gridString.Substring(5, 4));

                    var grid = gridString[12..]
                        .GridForDay(c => c == '#')
                        .ToArray();

                    return new JigsawPiece(id, grid, adjacencies);
                })
                .ToArray();

            var adj = jigsawPieces.Select(jigsawPiece =>
                {
                    List<JigsawPiece> adjacencies = jigsawPieces
                        .Where(adj => adj != jigsawPiece)
                        .Where(adj => adj.AllEdges.Intersect(jigsawPiece.AllEdges).Any())
                        .ToList();
                    return (jigsawPiece, adjacencies);
                });

            foreach ((var grid, var adjs) in adj)
            {
                adjacencies.Add(grid, adjs);
            }

            return jigsawPieces;
        }
    }

    private class Orientation
    {
        private readonly OrientationFunc orientationFunc;

        public Orientation(int top, int bottom, int left, int right, JigsawPiece grid, OrientationFunc orientationFunc)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            JigsawPiece = grid;
            this.orientationFunc = orientationFunc;
        }

        public int Top { get; }

        public int Bottom { get; }

        public int Left { get; }

        public int Right { get; }

        public JigsawPiece JigsawPiece { get; }

        public Span2D<bool> OrientedPiece() => orientationFunc();
    }

    private delegate Span2D<bool> OrientationFunc();
}
