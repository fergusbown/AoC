using System.Linq;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner;

internal class Day_2020_11 : IDayChallenge
{
    private readonly SpaceState[,] inputGrid;

    public Day_2020_11(string inputData)
    {
        var inputGrid = inputData.GridForDay(ch => ch switch
        {
            '.' => SpaceState.Floor,
            _ => SpaceState.EmptySeat,
        },
        extraBorder: 1,
        () => SpaceState.Floor);

        this.inputGrid = inputGrid.ToArray();
    }

    public string Part1()
    {
        return $"{GetFinalOccupationCount(inputGrid, 4, GetVisibleOccupiedSeatCount)}";

        static int GetVisibleOccupiedSeatCount(Span2D<SpaceState> grid, int rowIndex, int columnIndex)
        {
            var square = grid.Slice(rowIndex - 1, columnIndex - 1, 3, 3);

            int occupiedCount = 0;

            foreach (SpaceState state in square)
            {
                if (state == SpaceState.OccupiedSeat)
                {
                    occupiedCount++;
                }
            }

            if (grid[rowIndex, columnIndex] == SpaceState.OccupiedSeat)
            {
                occupiedCount--;
            }

            return occupiedCount;
        }
    }

    public string Part2()
    {
        return $"{GetFinalOccupationCount(inputGrid, 5, GetVisibleOccupiedSeatCount)}";

        static int GetVisibleOccupiedSeatCount(Span2D<SpaceState> grid, int rowIndex, int columnIndex)
        {
            int occupiedCount = 0;
            for (int rowDelta = -1; rowDelta <= 1; rowDelta++)
            {
                for (int columnDelta = -1; columnDelta <= 1; columnDelta++)
                {
                    if (rowDelta == 0 && columnDelta == 0)
                    {
                        continue;
                    }

                    if (IsOccupied(grid, rowIndex, columnIndex, rowDelta, columnDelta))
                    {
                        occupiedCount++;
                    }
                }
            }

            return occupiedCount;
        }

        static bool IsOccupied(Span2D<SpaceState> grid, int rowIndex, int columnIndex, int rowDelta, int columnDelta)
        {
            int checkingRow = rowIndex + rowDelta;
            int checkingColumn = columnIndex + columnDelta;

            while (checkingRow >= 0 && checkingRow < grid.Height && checkingColumn >= 0 && checkingColumn < grid.Width)
            {
                switch (grid[checkingRow, checkingColumn])
                {
                    case SpaceState.EmptySeat:
                        return false;
                    case SpaceState.OccupiedSeat:
                        return true;
                    default:
                        break;
                }

                checkingRow += rowDelta;
                checkingColumn += columnDelta;
            }

            return false;
        }
    }

    private static int GetFinalOccupationCount(SpaceState[,] inputGrid, int maxVisibleOccupations, GetVisibleOccupiedSeatCount getVisibleOccupiedSeatCount)
    {
        var sourceGrid = new Span2D<SpaceState>((SpaceState[,])inputGrid.Clone());
        var targetGrid = new Span2D<SpaceState>((SpaceState[,])inputGrid.Clone());

        int stateChanges;
        int occupiedCount;
        do
        {
            stateChanges = 0;
            occupiedCount = 0;
#pragma warning disable IDE0180 // Use tuple to swap values
            var temp = sourceGrid;
#pragma warning restore IDE0180 // Use tuple to swap values
            sourceGrid = targetGrid;
            targetGrid = temp;

            for (int columnIndex = 1; columnIndex < sourceGrid.Width - 1; columnIndex++)
            {
                for (int rowIndex = 1; rowIndex < sourceGrid.Height - 1; rowIndex++)
                {
                    targetGrid[rowIndex, columnIndex] = sourceGrid[rowIndex, columnIndex];

                    switch (sourceGrid[rowIndex, columnIndex])
                    {
                        case SpaceState.EmptySeat:
                            if (getVisibleOccupiedSeatCount(sourceGrid, rowIndex, columnIndex) == 0)
                            {
                                targetGrid[rowIndex, columnIndex] = SpaceState.OccupiedSeat;
                                stateChanges++;
                                occupiedCount++;
                            }
                            break;
                        case SpaceState.OccupiedSeat:
                            if (getVisibleOccupiedSeatCount(sourceGrid, rowIndex, columnIndex) >= maxVisibleOccupations)
                            {
                                targetGrid[rowIndex, columnIndex] = SpaceState.EmptySeat;
                                stateChanges++;
                            }
                            else
                            {
                                occupiedCount++;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        while (stateChanges > 0);

        return occupiedCount;
    }

    private delegate int GetVisibleOccupiedSeatCount(Span2D<SpaceState> grid, int rowIndex, int columnIndex);

    private enum SpaceState
    {
        Floor,
        EmptySeat,
        OccupiedSeat,
    }
}