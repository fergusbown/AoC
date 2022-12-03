namespace AoCRunner;

internal class Day_2020_12 : IDayChallenge
{
    private readonly (char command, int amount)[] inputData;

    public Day_2020_12(string inputData)
    {
        this.inputData = inputData
            .StringsForDay()
            .Select(x =>
            {
                var command = x[0];
                var amount = int.Parse(x[1..]);
                return (command, amount);
            })
            .ToArray();
    }

    public string Part1()
    {
        return $"{new ShipPosition().Apply(inputData)}";
    }

    public string Part2()
    {
        return $"{new WaypointPosition().Apply(inputData)}";
    }

    private class Position
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int ManhattanDistance => Math.Abs(this.X) + Math.Abs(this.Y);

        public void MoveDirection(int degrees, int amount)
        {
            switch (degrees)
            {
                case 0: // North
                    Y += amount;
                    return;
                case 90: // East
                    X += amount;
                    return;
                case 180: // South
                    Y -= amount;
                    return;
                case 270: // West
                    X -= amount;
                    return;
                default:
                    break;
            }
        }

        public void MoveDirection(char command, int amount)
        {
            switch (command)
            {
                case 'N':
                    MoveDirection(0, amount);
                    break;
                case 'S':
                    MoveDirection(180, amount);
                    break;
                case 'E':
                    MoveDirection(90, amount);
                    break;
                case 'W':
                    MoveDirection(270, amount);
                    break;
            }
        }

        public void Rotate(int degrees)
        {
            var (x, y) = (X, Y);

            switch (degrees)
            {
                case 90:
                case -270:
                    Y = -x;
                    X = y;
                    break;
                case 180:
                case -180:
                    Y = -y;
                    X = -x;
                    break;
                case 270:
                case -90:
                    X = -y;
                    Y = x;
                    break;
                default:
                    break;
            }
        }
    }

    private class ShipPosition
    {
        private int degrees = 90;

        private readonly Position position = new();

        public int Apply((char command, int amount)[] instructions)
        {
            foreach ((var command, var amount) in instructions)
            {
                switch (command)
                {
                    case 'L':
                        Turn(-amount);
                        break;
                    case 'R':
                        Turn(amount);
                        break;
                    case 'F':
                        position.MoveDirection(this.degrees, amount);
                        break;
                    default:
                        position.MoveDirection(command, amount);
                        break;
                }
            }

            return position.ManhattanDistance;

            void Turn(int degrees)
            {
                this.degrees += degrees;

                if (this.degrees < 0)
                {
                    this.degrees += 360;
                }

                this.degrees %= 360;
            }
        }
    }
    private class WaypointPosition
    {
        private readonly Position position = new();

        private readonly Position waypoint = new()
        {
            X = 10,
            Y = 1,
        };

        public int Apply((char command, int amount)[] instructions)
        {
            foreach ((var command, var amount) in instructions)
            {
                switch (command)
                {
                    case 'L':
                        waypoint.Rotate(-amount);
                        break;
                    case 'R':
                        waypoint.Rotate(amount);
                        break;
                    case 'F':
                        position.X += waypoint.X * amount;
                        position.Y += waypoint.Y * amount;
                        break;
                    default:
                        waypoint.MoveDirection(command, amount);
                        break;
                }
            }

            return position.ManhattanDistance;
        }
    }
}