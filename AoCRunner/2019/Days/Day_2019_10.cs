using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal partial class Day_2019_10 : IDayChallenge
{
    private readonly bool[,] inputData;

    private HashSet<(int row, int column)> bestAsteroids = new();
    private (int row, int column) best = (0, 0);


    public Day_2019_10(string inputData)
    {
        this.inputData = inputData.GridForDay(c => c == '#').ToArray();
    }

    public string Part1()
    {
        Span2D<bool> asteroids = new(inputData);

        HashSet<(int x, int y)> linesOfSight = AllLinesOfSight(asteroids.Width, asteroids.Height);

        for (int row = 0; row < asteroids.Height; row++)
        {
            for (int column = 0; column < asteroids.Width; column++)
            {
                if (asteroids[row, column])
                {
                    HashSet<(int row, int column)> visibleAsteroids = new();
                    foreach ((int x, int y) in linesOfSight)
                    {
                        int testRow = row + y;
                        int testColumn = column + x;

                        while (testRow >=0 && testRow < asteroids.Height && testColumn >= 0 && testColumn < asteroids.Width)
                        {
                            if (asteroids[testRow, testColumn])
                            {
                                visibleAsteroids.Add((testRow, testColumn));
                                break;
                            }

                            testRow += y;
                            testColumn += x;
                        }
                    }

                    if (visibleAsteroids.Count > bestAsteroids.Count)
                    {
                        bestAsteroids = visibleAsteroids;
                        best = (row, column);
                    }
                }
            }
        }
        return $"Best is {best.column},{best.row} with {bestAsteroids.Count} other asteroids detected";
    }

    public string Part2()
    {
        var twoHundredth = bestAsteroids
            .Select(a => WithDegrees(best, a))
            .OrderBy(a => a.angle)
            .Skip(199)
            .First().asteroid;

        return $"{(twoHundredth.column * 100) + twoHundredth.row}";

        static ((int row, int column) asteroid, double angle) WithDegrees((int row, int column) station, (int row, int column) asteroid)
        {
            double angle = (Math.Atan2(asteroid.row - station.row, asteroid.column - station.column) * (180 / Math.PI)) + 90;

            if (angle < 0)
            {
                angle = 360 + angle;
            }

            return (asteroid, angle);
        }
    }

    private HashSet<(int x, int y)> AllLinesOfSight(int width, int height)
    {
        HashSet<(int x, int y)> result = new();
        var primes = Factorisation.CalculatePrimes(Math.Max(width, height));

        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++)
            {
                result.Add(Factorisation.ReduceFraction(x, y, primes));
            }
        }
        result.Add((0, 1));
        result.Add((1, 0));

        result.UnionWith(result.Select(i => (-i.x, i.y)).ToArray());
        result.UnionWith(result.Select(i => (i.x, -i.y)).ToArray());

        return result;
    }
}
