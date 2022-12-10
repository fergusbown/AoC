using CommunityToolkit.HighPerformance;

namespace AoCRunner;

internal class Day_2022_08 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_08(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        Span2D<int> grid = this.inputData.GridForDay(c => c - '0');
        bool[] visibleBacking = new bool[grid.Width * grid.Height];
        Span2D<bool> visible = new Span2D<bool>(visibleBacking, grid.Height, grid.Width);

        for (int column = 0; column < grid.Width; column++)
        {
            DetermineVisibilityOnLine(grid, visible, 0, column, 1, 0);
            DetermineVisibilityOnLine(grid, visible, grid.Height - 1, column, -1, 0);
        }

        for (int row = 0; row < grid.Height; row++)
        {
            DetermineVisibilityOnLine(grid, visible, row, 0, 0, 1);
            DetermineVisibilityOnLine(grid, visible, row, grid.Width - 1, 0, -1);
        }

        return visibleBacking.Count(b => b).ToString();

        static void DetermineVisibilityOnLine(Span2D<int> grid, Span2D<bool> visible, int row, int column, int rowDelta, int columnDelta)
        {
            int maxHeight = -1;

            while (row >= 0 && row < grid.Height && column >= 0 && column < grid.Width)
            {
                if (grid[row, column] > maxHeight)
                {
                    visible[row, column] = true;
                    maxHeight = grid[row, column];
                }

                row += rowDelta;
                column += columnDelta;
            }
        }
    }

    public string Part2()
    {
        Span2D<int> grid = this.inputData.GridForDay(c => c - '0');
        int maxViewingDistance = 0;

        for (int row = 0; row < grid.Height; row++)
        {
            for (int column = 0; column < grid.Width; column++)
            {
                int viewingDistance =
                    ScenicDistance(grid, row, column, 1, 0) *
                    ScenicDistance(grid, row, column, -1, 0) *
                    ScenicDistance(grid, row, column, 0, 1) *
                    ScenicDistance(grid, row, column, 0, -1);
                maxViewingDistance = Math.Max(viewingDistance, maxViewingDistance);
            }
        }

        return maxViewingDistance.ToString();

        static int ScenicDistance(Span2D<int> grid, int row, int column, int rowDelta, int columnDelta)
        {
            int height = grid[row, column];

            int viewingDistance = 0;

            int testRow = row + rowDelta;
            int testColumn = column + columnDelta;

            while (testRow >= 0 && testRow < grid.Height && testColumn >= 0 && testColumn < grid.Width)
            {
                viewingDistance++;
                
                if (grid[testRow, testColumn] >= height)
                {
                    break;
                }

                testRow += rowDelta;
                testColumn += columnDelta;
            }

            return viewingDistance;
        }
    }
}
