using System.Collections;

namespace AoCRunner;

internal class Day_2020_17 : IDayChallenge
{
    private readonly string inputData;

    public Day_2020_17(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
        => Solve(GetStartSpace(inputData, false, 6));

    public string Part2()
        => Solve(GetStartSpace(inputData, true, 6));

    private static Space4d<SpaceActive> GetStartSpace(string inputData, bool fourthDimension, int numberOfCycles)
    {
        var rows = inputData.StringsForDay();
        int height = rows.Length;
        int width = rows[0].Length;
        int depth = 1;
        int metaDimension = 1;

        int spaceForCycles = numberOfCycles * 2;
        int fourthIndex = fourthDimension ? 6 : 0;

        Space4d<SpaceActive> result = new(
            width + spaceForCycles,
            height + spaceForCycles, 
            depth + spaceForCycles, 
            fourthDimension 
                ? metaDimension + spaceForCycles 
                : metaDimension);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var element = result[x + 6, y + 6, 6, fourthIndex];
                element.Active = rows[y][x] == '#';
                element.Commit();
            }
        }

        return result;
    }

    private static string Solve(Space4d<SpaceActive> space)
    {
        int lit = 0;
        int numberOfCycles = space.Depth / 2;

        for (int offset = numberOfCycles - 1; offset >= 0; offset--)
        {
            List<SpaceActive> dirty = new();
            lit = 0;

            int metaOffset = space.MetaDimensionSize == 1 ? 0 : offset;

            for (int x = offset; x < space.Width - offset; x++)
            {
                for (int y = offset; y < space.Height - offset; y++)
                {
                    for (int z = offset; z < space.Depth - offset; z++)
                    {
                        for (int w = metaOffset; w < space.MetaDimensionSize - metaOffset; w++)
                        {
                            var element  = space[x, y, z, w];
                            var activeAdjacencies = space.Adjacencies(x, y, z, w).Count((point) => point.Active);

                            if (element.Active && activeAdjacencies != 2 && activeAdjacencies != 3)
                            {
                                element.Active = false;
                                dirty.Add(element);
                            }
                            else if (element.Active is false && activeAdjacencies == 3)
                            {
                                element.Active = true;
                                dirty.Add(element);
                                lit++;
                            }
                            else if (element.Active)
                            {
                                lit++;
                            }
                        }
                    }
                }
            }

            foreach (var item in dirty)
            {
                item.Commit();
            }
        }

        return $"{lit}";
    }

    private class SpaceActive
    {
        private bool active;
        private bool? pendingActive;

        public bool Active 
        {
            get => active;
            set => pendingActive = value; 
        }

        public void Commit()
        {
            if (pendingActive != null)
            {
                active = pendingActive.Value;
                pendingActive = null;
            }
        }
    }
}
