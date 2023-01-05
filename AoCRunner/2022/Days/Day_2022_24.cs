using System.Data;

namespace AoCRunner;

internal class Day_2022_24 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_24(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (var blizzards, var entrance, var exit) = Parse(inputData);
        return TimeToTraverse(blizzards, 0, entrance, exit).ToString();
    }

    public string Part2()
    {
        (var blizzards, var entrance, var exit) = Parse(inputData);
        int timeToExit = TimeToTraverse(blizzards, 0, entrance, exit);
        int timeBackToStart = TimeToTraverse(blizzards, timeToExit, exit, entrance);
        return TimeToTraverse(blizzards, timeBackToStart, entrance, exit).ToString();
    }

    private static int TimeToTraverse(IReadOnlyCollection<Blizzard> blizzards, int startTime, Location entrance, Location exit)
    {
        int height = Math.Max(entrance.Row, exit.Row);
        int width = Math.Max(entrance.Column, exit.Column) + 1;

        Expedition start = new(entrance, startTime);
        Queue<Expedition> valleyStates = new();
        valleyStates.Enqueue(start);

        Location exitAdjacent = exit.Row < 0 ? exit with { Row = 0 } : exit with { Row = exit.Row - 1 };

        HashSet<Location> blizzardLocations = new();
        HashSet<Expedition> seen = new();
        HashSet<Location> seenLocations = new();

        int lastMinute = startTime;

        while (valleyStates.TryDequeue(out Expedition working))
        {
            int minute = working.Minute + 1;

            if (working.Location.Equals(exitAdjacent))
            {
                return minute;
            }

            if (minute != lastMinute)
            {
                blizzardLocations = new HashSet<Location>(blizzards.Select(b => b.Location(minute, width, height)));
                lastMinute = minute;
            }

            foreach (var nextLocation in working.Location.AccessableLocations(width, height))
            {
                seenLocations.Add(nextLocation);
                if (!blizzardLocations.Contains(nextLocation))
                {
                    var expedition = new Expedition(nextLocation, minute);

                    if (seen.Add(expedition))
                    {
                        valleyStates.Enqueue(expedition);
                    }
                }
            }
        }

        throw new InvalidOperationException();
    }

    private static (IReadOnlyCollection<Blizzard> Blizzards, Location Entrance, Location Exit) Parse(string inputData)
    {
        List<Blizzard> blizzards = new();
        var grid = inputData.GridForDay((ch, row, column) =>
        {
            Location location = new Location(row - 1, column - 1);
            switch (ch)
            {
                case '>':
                    blizzards.Add(new Blizzard(location, BlizzardDirection.Right));
                    break;
                case '<':
                    blizzards.Add(new Blizzard(location, BlizzardDirection.Left));
                    break;
                case '^':
                    blizzards.Add(new Blizzard(location, BlizzardDirection.Up));
                    break;
                case 'v':
                    blizzards.Add(new Blizzard(location, BlizzardDirection.Down));
                    break;
            }
            return ch;
        });

        return (blizzards, new Location(-1, 0), new Location(grid.Height - 2, grid.Width - 3));
    }

    private readonly record struct Expedition(Location Location, int Minute);

    private readonly record struct Location(int Row, int Column)
    {
        public int ManhattanDistanceTo(Location other)
            => Math.Abs(Row - other.Row) + Math.Abs(Column - other.Column);

        public IEnumerable<Location> AccessableLocations(int valleyWidth, int valleyHeight)
        {
            int maxRow = valleyHeight - 1;
            int maxColumn = valleyWidth - 1;

            if (Row > 0)
            {
                yield return this with { Row = Row - 1 };
            }

            if (Row < maxRow)
            {
                yield return this with { Row = Row + 1 };
            }

            if (Row >= 0 && Row <= maxRow)
            {
                if (Column > 0)
                {
                    yield return this with { Column = Column - 1 };
                }

                if (Column < maxColumn)
                {
                    yield return this with { Column = Column + 1 };
                }
            }

            yield return this;
        }
    }

    private readonly record struct Blizzard(Location Start, BlizzardDirection Direction)
    {
        public Location Location(int minute, int valleyWidth, int valleyHeight)
        {
            Location result = Direction switch
            {
                BlizzardDirection.Up => Start with { Row = Start.Row - minute },
                BlizzardDirection.Down => Start with { Row = Start.Row + minute },
                BlizzardDirection.Left => Start with { Column = Start.Column - minute },
                BlizzardDirection.Right => Start with { Column = Start.Column + minute },
                _ => throw new InvalidOperationException(),
            };

            result = result with
            {
                Row = result.Row % valleyHeight,
                Column = result.Column % valleyWidth,
            };

            if (result.Row < 0)
            {
                result = result with { Row = result.Row + valleyHeight };
            }
            if (result.Column < 0)
            {
                result = result with { Column = result.Column + valleyWidth };
            }

            return result;
        }
    }

    private enum BlizzardDirection
    {
        Up = '^',
        Down = 'v',
        Left = '<',
        Right = '>',
    }
}
