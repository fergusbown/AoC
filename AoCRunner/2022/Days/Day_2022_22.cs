using CommunityToolkit.HighPerformance;
using Generator.Equals;

namespace AoCRunner;

internal partial class Day_2022_22 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_22(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var startingTile = Parse2D(inputData);
        var instructions = ParseInstructions(inputData);

        return GetPassword(startingTile, Facing.Right, instructions).ToString();
    }

    public string Part2()
    {
        var startingTile = Parse3D(inputData);
        var instructions = ParseInstructions(inputData);

        return GetPassword(startingTile, Facing.Right, instructions).ToString();
    }

    private static IReadOnlyCollection<Instruction> ParseInstructions(string inputData)
    {
        var endOfGrid = inputData.IndexOf($"{Environment.NewLine}{Environment.NewLine}");

        var instructionsText = inputData[(endOfGrid + 4)..];
        List<Instruction> instructions = new();

        int currentSteps = 0;

        foreach (char ch in instructionsText)
        {
            switch (ch)
            {
                case 'L':
                case 'R':
                    instructions.Add(new Instruction(currentSteps, ch));
                    currentSteps = 0;
                    break;
                default:
                    currentSteps *= 10;
                    currentSteps += ch - '0';
                    break;
            }
        }

        instructions.Add(new Instruction(currentSteps, ' '));

        return instructions;
    }

    private static Tile Parse2D(string inputData)
    {
        var endOfGrid = inputData.IndexOf($"{Environment.NewLine}{Environment.NewLine}");
        var grid = inputData[0..endOfGrid].GridForDay(c => c, extraBorder: 1, defaultValue: () => ' ');
        HashSet<Tile> passableTiles = new();
        int edgeLength = (int)Math.Sqrt((grid.Width * grid.Height) / 12);

        for (int row = 1; row < grid.Height - 1; row++)
        {
            for (int column = 1; column < grid.Width - 1; column++)
            {
                if (grid[row, column] == '.')
                {
                    Tile tile = GetTile(row, column);
                    tile.Up = GetAdjacent(grid, tile, -1, 0, Facing.Up);
                    tile.Down = GetAdjacent(grid, tile, 1, 0, Facing.Down);
                    tile.Left = GetAdjacent(grid, tile, 0, -1, Facing.Left);
                    tile.Right = GetAdjacent(grid, tile, 0, 1, Facing.Right);
                }
            }
        }

        for (int column = 0; column < grid.Width; column++)
        {
            if (grid[1, column] == '.')
            {
                return GetTile(1, column);
            }
        }

        throw new Exception("Doh");

        Tile GetTile(int row, int column)
        {
            Tile tile = new('.', row, column);

            if (!passableTiles.Add(tile))
            {
                _ = passableTiles.TryGetValue(tile, out var actualTile);
                tile = actualTile!;
            }

            return tile;
        }

        (Tile, Facing)? GetAdjacent(Span2D<char> grid, Tile tile, int rowDelta, int columnDelta, Facing facing)
        {
            var contents = grid[tile.Row + rowDelta, tile.Column + columnDelta];

            if (contents == '.')
            {
                return (GetTile(tile.Row + rowDelta, tile.Column + columnDelta), facing);
            }
            else if (contents == '#')
            {
                return null;
            }
            else
            {
                int testRow = tile.Row;
                int testColumn = tile.Column;

                while (grid[testRow, testColumn] != ' ')
                {
                    testRow -= rowDelta;
                    testColumn -= columnDelta;
                }

                return GetAdjacent(grid, new Tile(grid[testRow, testColumn], testRow, testColumn), rowDelta, columnDelta, facing);
            }
        }
    }

    private static Tile Parse3D(string inputData)
    {
        ParseGridDelegate<Tile> parse = (c, row, column) => new Tile(c, row + 1, column + 1);

        var endOfGrid = inputData.IndexOf($"{Environment.NewLine}{Environment.NewLine}");
        var grid = inputData[0..endOfGrid].GridForDay(parse, defaultValue: (row, column) => parse(' ', row, column));

        int edgeLength = (int)Math.Sqrt((grid.Width * grid.Height) / 12);
        Cube<Tile> cube = new(edgeLength);

        HashSet<Tile> passableTiles = new();

        int row = 0;
        int column = 0;

        while (grid[row, column].Value == ' ')
        {
            column += edgeLength;
        }

        while (row < grid.Height)
        {
            while (column > 0 && grid[row, column - edgeLength].Value != ' ')
            {
                cube.RotateRight();
                column -= edgeLength;
            }

            while (column < grid.Width && grid[row, column].Value != ' ')
            {
                var face = grid.Slice(row, column, edgeLength, edgeLength);
                PopulateSimpleAdjacents(face);

                cube.PopulateFront(face);
                cube.RotateLeft();
                column += edgeLength;
            }

            row += edgeLength;

            if (row < grid.Height)
            {
                while (column >= grid.Width || grid[row, column].Value == ' ' || grid[row - edgeLength, column].Value == ' ')
                {
                    cube.RotateRight();
                    column -= edgeLength;
                }

                cube.RotateUp();
            }
        }

        for (int outerHorizontal = 0; outerHorizontal < 4; outerHorizontal++)
        {
            for (int vertical = 0; vertical < 4; vertical++)
            {
                for (int horizontal = 0; horizontal < 4; horizontal++)
                {
                    var frontFace = cube.Front;
                    var leftFace = cube.Left;
                    var rightFace = cube.Right;

                    Facing leftFacing = GetFacing(frontFace[0, 1], frontFace[0, 0]);
                    Facing rightFacing = GetFacing(frontFace[0, ^2], frontFace[0, ^1]);

                    Facing leftDestinationFacing = GetFacing(leftFace[0, 0], leftFace[0, 1]);
                    Facing rightDestinationFacing = GetFacing(rightFace[0, 0], rightFace[0, 1]);

                    for (int r = 0; r < frontFace.Height; r++)
                    {
                        var location = frontFace[r, 0];
                        var adj = GetCubeAdjcent(leftFace[r, 0], leftDestinationFacing);
                        switch (leftFacing)
                        {
                            case Facing.Right:
                                location.Right = adj;
                                break;
                            case Facing.Down:
                                location.Down = adj;
                                break;
                            case Facing.Left:
                                location.Left = adj;
                                break;
                            case Facing.Up:
                                location.Up = adj;
                                break;
                            default:
                                break;
                        }

                        location = frontFace[r, ^1];
                        adj = GetCubeAdjcent(rightFace[r, 0], rightDestinationFacing);

                        switch (rightFacing)
                        {
                            case Facing.Right:
                                location.Right = adj;
                                break;
                            case Facing.Down:
                                location.Down = adj;
                                break;
                            case Facing.Left:
                                location.Left = adj;
                                break;
                            case Facing.Up:
                                location.Up = adj;
                                break;
                            default:
                                break;
                        }
                    }

                    cube.RotateRight();
                }

                cube.RotateUp();
            }

            cube.RotateRight();
        }

        foreach(var tile in grid.GetRow(0))
        {
            if (tile.Value == '.')
            {
                return tile;
            }
        }

        throw new Exception("Doh");

        static void PopulateSimpleAdjacents(Span2D<Tile> face)
        {
            for (int faceRow = 0; faceRow < face.Height; faceRow++)
            {
                for (int faceColumn = 0; faceColumn < face.Width; faceColumn++)
                {
                    Tile tile = face[faceRow, faceColumn];
                    tile.Up = GetAdjacent(face, faceRow - 1, faceColumn, Facing.Up);
                    tile.Down = GetAdjacent(face, faceRow + 1, faceColumn, Facing.Down);
                    tile.Left = GetAdjacent(face, faceRow, faceColumn - 1, Facing.Left);
                    tile.Right = GetAdjacent(face, faceRow, faceColumn + 1, Facing.Right);
                }
            }
        }

        static (Tile, Facing)? GetAdjacent(Span2D<Tile> face, int row, int column, Facing facing)
        {
            if (row >= 0 && row < face.Height && column >= 0 && column < face.Width)
            {
                Tile adj = face[row, column];

                if (adj.Value == '.')
                {
                    return (face[row, column], facing);
                }
            }

            return null;
        }

        static Facing GetFacing(Tile first, Tile second)
        {
            int rowDelta = second.Row - first.Row;
            int columnDelta = second.Column - first.Column;

            return (rowDelta, columnDelta) switch
            {
                (1, 0) => Facing.Down,
                (-1, 0) => Facing.Up,
                (0, 1) => Facing.Right,
                (0, -1) => Facing.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(first)),
            };
        }

        static (Tile, Facing)? GetCubeAdjcent(Tile adjacent, Facing facing)
        {
            if (adjacent.Value == '#')
            {
                return null;
            }

            return (adjacent, facing);
        }
    }

    private int GetPassword(
        Tile location,
        Facing facing,
        IReadOnlyCollection<Instruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            for (int i = 0; i < instruction.Steps; i++)
            {
                (Tile nextTile, Facing nextFacing)? next = location.TileFacing(facing);

                if (next is null)
                {
                    break;
                }

                location = next.Value.nextTile;
                facing = next.Value.nextFacing;
            }

            facing = instruction.PerformTurn(facing);
        }

        return (1000 * location.Row) + (4 * location.Column) + (int)facing;
    }

    private record Instruction(int Steps, char Turn)
    {
        public Facing PerformTurn(Facing current)
        {
            if (Turn == 'L')
            {
                return current switch
                {
                    Facing.Right => Facing.Up,
                    Facing.Up => Facing.Left,
                    Facing.Left => Facing.Down,
                    _ => Facing.Right
                };
            }
            else if (Turn == 'R')
            {
                return current switch
                {
                    Facing.Right => Facing.Down,
                    Facing.Up => Facing.Right,
                    Facing.Left => Facing.Up,
                    _ => Facing.Left
                };
            }
            else
            {
                return current;
            }

        }
    }

    private enum Facing
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3,
    }

    [Equatable]
    private partial class Tile
    {
        [IgnoreEquality]
        public char Value { get; }

        public int Row { get; }

        public int Column { get; }

        [IgnoreEquality]
        public (Tile, Facing)? Up { get; set; }

        [IgnoreEquality]
        public (Tile, Facing)? Down { get; set; }

        [IgnoreEquality]
        public (Tile, Facing)? Left { get; set; }

        [IgnoreEquality]
        public (Tile, Facing)? Right { get; set; }


        public (Tile, Facing)? TileFacing(Facing facing)
        {
            return facing switch
            {
                Facing.Up => Up,
                Facing.Down => Down,
                Facing.Right => Right,
                _ => Left,
            };
        }

        public Tile(char value, int row, int column)
        {
            Value = value;
            Row = row;
            Column = column;
        }
    }
}
