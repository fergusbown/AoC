namespace AoCRunner;

internal partial class Day_2020_24 : IDayChallenge
{
    private readonly HexFloor inputData;

    public Day_2020_24(string inputData)
    {
        this.inputData = new HexFloor(inputData);
    }

    public string Part1()
    {
        return this.inputData.Run(0).ToString();
    }

    public string Part2()
    {
        return this.inputData.Run(100).ToString();
    }

    private enum Direction
    {
        NorthEast,
        NorthWest,
        West,
        SouthWest,
        SouthEast,
        East,
    }

    private class HexFloor
    {
        private readonly IEnumerable<IEnumerable<Direction>> instructionsForDay;

        public HexFloor(string inputData)
        {
            string[] inputLines = inputData.StringsForDay();
            List<IEnumerable<Direction>> instructions = new(inputLines.Length);

            foreach (var inputLine in inputLines)
            {
                int index = 0;
                List<Direction> lineInstructions = new(inputLine.Length);

                while (index < inputLine.Length)
                {
                    switch (inputLine[index++])
                    {
                        case 'w':
                            lineInstructions.Add(Direction.West);
                            break;
                        case 'e':
                            lineInstructions.Add(Direction.East);
                            break;
                        case 'n':
                            {
                                switch (inputLine[index++])
                                {
                                    case 'w':
                                        lineInstructions.Add(Direction.NorthWest);
                                        break;
                                    case 'e':
                                        lineInstructions.Add(Direction.NorthEast);
                                        break;
                                }
                                break;
                            }
                        case 's':
                            {
                                switch (inputLine[index++])
                                {
                                    case 'w':
                                        lineInstructions.Add(Direction.SouthWest);
                                        break;
                                    case 'e':
                                        lineInstructions.Add(Direction.SouthEast);
                                        break;
                                }
                                break;
                            }
                    }
                }

                instructions.Add(lineInstructions);
            }

            instructionsForDay = instructions;
        }
        public int Run(int days)
        {
            HashSet<HexPoint> blackPieces = new();

            foreach (var instructions in instructionsForDay)
            {
                HexPoint position = new(0, 0, 0);

                foreach (var instruction in instructions)
                {
                    position = instruction switch
                    {
                        Direction.SouthEast => position.SouthEast,
                        Direction.SouthWest => position.SouthWest,
                        Direction.NorthEast => position.NorthEast,
                        Direction.NorthWest => position.NorthWest,
                        Direction.West => position.West,
                        _ => position.East,
                    };
                }

                if (!blackPieces.Add(position))
                {
                    blackPieces.Remove(position);
                }
            }

            for (int i = 0; i < days; i++)
            {
                HashSet<HexPoint> whitePieces = blackPieces
                    .SelectMany(b => b.Adjacencies)
                    .ToHashSet();

                whitePieces.ExceptWith(blackPieces);

                var blackPiecesToFlip = blackPieces
                    .Where(b =>
                    {
                        int blackAdjacents = b.Adjacencies.Intersect(blackPieces).Count();
                        return blackAdjacents == 0 || blackAdjacents > 2;
                    })
                    .ToHashSet();

                var whitePiecesToFlip = whitePieces
                    .Where(w => w.Adjacencies.Intersect(blackPieces).Count() == 2)
                    .ToHashSet();

                blackPieces.ExceptWith(blackPiecesToFlip);
                blackPieces.UnionWith(whitePiecesToFlip);
            }

            return blackPieces.Count;
        }
    }

    /// <summary>
    /// See Cube coordinates from https://www.redblobgames.com/grids/hexagons/
    /// </summary>
    /// <param name="Q">Q Coordinate</param>
    /// <param name="R">R Coordinate</param>
    /// <param name="S">S Coordinate</param>
    private partial record HexPoint(int Q, int R, int S)
    {
        public HexPoint NorthEast => new HexPoint(Q + 1, R - 1, S);

        public HexPoint NorthWest => new HexPoint(Q, R - 1, S + 1);

        public HexPoint West => new HexPoint(Q - 1, R, S + 1);

        public HexPoint SouthWest => new HexPoint(Q - 1, R + 1, S);

        public HexPoint SouthEast => new HexPoint(Q, R + 1, S - 1);

        public HexPoint East => new HexPoint(Q + 1, R, S - 1);

        private HexPoint[]? adjacencies = null;

        public IEnumerable<HexPoint> Adjacencies
        {
            get
            {
                if (adjacencies is null)
                {
                    adjacencies = new[]
                    {
                        NorthEast,
                        NorthWest,
                        West,
                        SouthWest,
                        SouthEast,
                        East,
                    };
                }

                return adjacencies;
            }
        }
    }
}
