namespace AoCRunner;

internal class Day_2021_17 : IDayChallenge
{
    private readonly (int start, int end) xRange;
    private readonly (int start, int end) yRange;

    public Day_2021_17(string inputData)
    {
        var targetPoints = inputData
            .Replace("target area: x=", "")
            .Replace(", y=", "..")
            .Split("..")
            .Select(s => int.Parse(s))
            .ToArray();

        xRange = (targetPoints[0], targetPoints[1]);
        yRange = (targetPoints[2], targetPoints[3]);
    }

    public string Part1()
        => GetSuccessfulYVelocities().OrderByDescending(x => x.MaxHeight).First().MaxHeight.ToString();

    public string Part2()
        => GetSuccessfulVelocites().Count.ToString();

    private IReadOnlyCollection<(int X, int Y, int MaxY)> GetSuccessfulVelocites()
    {
        var yVelocitiesByStepCount = GetSuccessfulYVelocities()
            .GroupBy(y => y.Steps)
            .ToDictionary(
                x => x.Key,
                x => x.ToArray());

        var xVelocitiesByStepCount = GetSuccessfulXVelocities(yVelocitiesByStepCount.Select(y => y.Key).Max())
            .GroupBy(x => x.Steps)
            .ToDictionary(
                x => x.Key,
                x => x.Select(x => x.Velocity).ToArray());

        HashSet<(int X, int Y, int MaxY)> velocities = new();

        foreach (var (stepCount, yVelocites) in yVelocitiesByStepCount)
        {
            if (xVelocitiesByStepCount.TryGetValue(stepCount, out var xVelocities))
            {
                foreach (var (y, _, maxY) in yVelocites)
                {
                    foreach (var x in xVelocities)
                    {
                        velocities.Add((x, y, maxY));
                    }
                }
            }
        }

        return velocities;
    }

    private IEnumerable<(int Velocity, int Steps)> GetSuccessfulXVelocities(int maxSteps)
    {
        int maxVelocity = xRange.end + 1; //otherwise you instantly overshot

        for (int startVelocity = 1; startVelocity <= maxVelocity; startVelocity++)
        {
            int xPosition = 0;
            int velocity = startVelocity;
            int steps = 0;

            while (xPosition < xRange.end && steps <= maxSteps)
            {
                xPosition += velocity;
                if (velocity > 0)
                {
                    velocity--;
                }

                if (xPosition >= xRange.start && xPosition <= xRange.end)
                {
                    yield return (startVelocity, steps);
                }

                steps++;
            }
        }
    }

    private IEnumerable<(int Velocity, int Steps, int MaxHeight)> GetSuccessfulYVelocities()
    {
        int startYVelocity = yRange.start;

        // if you shoot up at velocity y, after 1 step you'll be back to zero
        // after 1 step you'll be at depth y - 1
        // so if y - 1 is below the trench, you've overshot and always will:
        int maxYVelocity = (yRange.start - 2) * -1;

        while (startYVelocity <= maxYVelocity)
        {
            int yVelocity = startYVelocity;
            int yPosition = 0;
            int maxYPosition = 0;
            int steps = 0;

            while (yPosition > yRange.start)
            {
                yPosition += yVelocity;
                yVelocity--;

                if (yPosition > maxYPosition)
                {
                    maxYPosition = yPosition;
                }

                if (yPosition >= yRange.start && yPosition <= yRange.end)
                {
                    yield return (startYVelocity, steps, maxYPosition);
                }

                steps++;
            }
            startYVelocity++;
        }
    }
}
