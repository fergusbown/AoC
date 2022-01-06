namespace AoC2021Runner;

internal class Day_2021_19 : IDayChallenge
{
    private readonly IReadOnlyList<Scanner> inputData;

    public Day_2021_19(string inputData)
    {
        this.inputData = ParseInput(inputData);
    }

    public string Part1()
    {
        return TranslateScannersToSameCoordinates().SelectMany(t => t.Beacons).Distinct().Count().ToString();
    }

    public string Part2()
    {
        var translated = TranslateScannersToSameCoordinates();

        int maxDistance = 0;

        for (int firstScannerIndex = 0; firstScannerIndex < translated.Count; firstScannerIndex++)
        {
            var firstScanner = translated[firstScannerIndex];
            for (int secondScannerIndex = firstScannerIndex + 1; secondScannerIndex < translated.Count; secondScannerIndex++)
            {
                var secondScanner = translated[secondScannerIndex];

                var distance = Math.Abs(firstScanner.Position.X - secondScanner.Position.X) +
                    Math.Abs(firstScanner.Position.Y - secondScanner.Position.Y) +
                    Math.Abs(firstScanner.Position.Z - secondScanner.Position.Z);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
            }
        }

        return maxDistance.ToString();
    }

    private IReadOnlyList<Scanner> TranslateScannersToSameCoordinates()
    {
        Stack<Scanner> check = new();
        check.Push(inputData[0]);
        HashSet<int> pending = new(inputData.Skip(1).Select(s => s.Id));
        List<Scanner> translated = new()
        {
            inputData[0]
        };

        while (check.TryPop(out var scanner))
        {
            foreach (var id in pending.ToArray())
            {
                Scanner compare = inputData[id];

                if (scanner.Overlaps(compare, out var compareTranslated))
                {
                    pending.Remove(compare.Id);
                    check.Push(compareTranslated);
                    translated.Add(compareTranslated);
                }
            }
        }

        return translated;
    }

    private static IReadOnlyList<Scanner> ParseInput(string input)
    {
        List<Scanner> result = new();

        int scannerId = 0;
        List<Beacon> beacons = new();

        foreach (var line in input.StringsForDay())
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                result.Add(new Scanner(scannerId, beacons));
                beacons = new();
            }
            else if (line.StartsWith("---"))
            {
                scannerId = int.Parse(line.Substring(12, 2).Trim());
            }
            else
            {
                var points = line.Split(',').Select(x => int.Parse(x)).ToArray();
                beacons.Add(new Beacon(points[0], points[1], points[2]));
            }
        }

        result.Add(new Scanner(scannerId, beacons));

        return result;
    }

    private class Scanner
    {
        private readonly Dictionary<long, (Beacon beacon1, Beacon beacon2, long X2, long Y2, long Z2)> distanceBetweenBeacons = new();

        public Scanner(int id, IReadOnlyList<Beacon> beacons, int X = 0, int Y = 0, int Z = 0)
        {
            Id = id;
            Beacons = beacons;
            Position = (X, Y, Z);

            for (int firstBeaconIndex = 0; firstBeaconIndex < beacons.Count; firstBeaconIndex++)
            {
                var firstBeacon = beacons[firstBeaconIndex];
                for (int otherBeaconIndex = firstBeaconIndex + 1; otherBeaconIndex < beacons.Count; otherBeaconIndex++)
                {
                    var otherBeacon = beacons[otherBeaconIndex];

                    var x2 = Square(firstBeacon.X - otherBeacon.X);
                    var y2 = Square(firstBeacon.Y - otherBeacon.Y);
                    var z2 = Square(firstBeacon.Z - otherBeacon.Z);

                    // note that this only works because all distances happen to be unique
                    distanceBetweenBeacons.Add(x2 + y2 + z2, (firstBeacon, otherBeacon, x2, y2, z2));
                }
            }

            static long Square(long number)
                => number * number;
        }

        public (int X, int Y, int Z) Position { get; private set; }

        public int Id { get; }

        public IReadOnlyList<Beacon> Beacons { get; }

        public bool Overlaps(Scanner other, out Scanner translatedOther)
        {
            HashSet<long> thisDistances = new(this.distanceBetweenBeacons.Keys);
            HashSet<long> commonDistances = new(other.distanceBetweenBeacons.Keys);

            commonDistances.IntersectWith(thisDistances);

            // 12 in common => 66 common distances

            if (commonDistances.Count < 66)
            {
                translatedOther = other;
                return false;
            }

            IReadOnlyList<Scanner> rotations = other.Rotations();

            var testDistance = commonDistances.First();
            (Beacon sourceBeacon, _, _, _, _) = this.distanceBetweenBeacons[testDistance];
            foreach (Scanner rotated in rotations)
            {
                (Beacon rotatedBeacon1, Beacon rotatedBeacon2, _, _, _) = rotated.distanceBetweenBeacons[testDistance];
                Scanner test = rotated.Offset(rotatedBeacon1, sourceBeacon);

                if (this.Beacons.Intersect(test.Beacons).Count() >= 12)
                {
                    translatedOther = test;
                    return true;
                }

                test = rotated.Offset(rotatedBeacon2, sourceBeacon);

                if (this.Beacons.Intersect(test.Beacons).Count() >= 12)
                {
                    translatedOther = test;
                    return true;
                }
            }

            translatedOther = other;
            return false;
        }

        private IReadOnlyList<Scanner> Rotations()
        {
            List<Scanner> result = new();

            var rotatedBeacons = this.Beacons.Select(b => b.Rotations()).ToList();

            for (int i = 0; i < rotatedBeacons[0].Count; i++)
            {
                result.Add(new Scanner(this.Id, rotatedBeacons.Select(r => r[i]).ToList()));
            }

            return result;
        }

        private Scanner Offset(Beacon currentPosition, Beacon newPosition)
        {
            int xAdd = newPosition.X - currentPosition.X;
            int yAdd = newPosition.Y - currentPosition.Y;
            int zAdd = newPosition.Z - currentPosition.Z;
            return new Scanner(
                this.Id,
                this.Beacons.Select(b => new Beacon(b.X + xAdd, b.Y + yAdd, b.Z + zAdd)).ToList(),
                xAdd,
                yAdd,
                zAdd);
        }
    }


    private class Beacon : IEquatable<Beacon>
    {
        public Beacon(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public bool Equals(Beacon? other)
        {
            if (other is null)
            {
                return false;
            }

            return other.X == X && other.Y == Y && other.Z == Z;
        }

        public override int GetHashCode()
        {
            return X + Y + Z;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Beacon);
        }

        public IReadOnlyList<Beacon> Rotations()
        {
            return new List<Beacon>()
            {
                new Beacon(X, Y, Z),
                new Beacon(-Y, X, Z),
                new Beacon(-X, -Y, Z),
                new Beacon(Y, -X, Z),
                new Beacon(Y, -Z, -X),
                new Beacon(Z, Y, -X),
                new Beacon(-Y, Z, -X),
                new Beacon(-Z, -Y, -X),
                new Beacon(-Z, X, -Y),
                new Beacon(-X, -Z, -Y),
                new Beacon(Z, -X, -Y),
                new Beacon(X, Z, -Y),
                new Beacon(Y, Z, X),
                new Beacon(-Z, Y, X),
                new Beacon(-Y, -Z, X),
                new Beacon(Z, -Y, X),
                new Beacon(-Y, -X, -Z),
                new Beacon(X, -Y, -Z),
                new Beacon(Y, X, -Z),
                new Beacon(-X, Y, -Z),
                new Beacon(-X, Z, Y),
                new Beacon(-Z, -X, Y),
                new Beacon(X, -Z, Y),
                new Beacon(Z, X, Y),
            };
        }

    }
}
