using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.HighPerformance;

namespace AoC2021Runner
{
    public static class Span2DExtensions
    {
        public static void FlipHorizontally<T>(this Span2D<T> span)
        {
            int swappingColumn = span.Width - 1;
            Span<T> temp = new Span<T>(new T[span.Height]);
            for (int foldingColumn = 0; foldingColumn < span.Width / 2; foldingColumn++, swappingColumn--)
            {
                var swapping1 = span.GetColumn(swappingColumn);
                var swapping2 = span.GetColumn(foldingColumn);

                swapping1.CopyTo(temp);
                swapping2.CopyTo(swapping1);
                temp.CopyTo(swapping2);
            }
        }

        public static void FlipVertically<T>(this Span2D<T> span)
        {
            int swappingRow = span.Height - 1;
            Span<T> temp = new Span<T>(new T[span.Width]);
            for (int foldingRow = 0; foldingRow < span.Height / 2; foldingRow++, swappingRow--)
            {
                var swapping1 = span.GetRowSpan(swappingRow);
                var swapping2 = span.GetRowSpan(foldingRow);
                swapping1.CopyTo(temp);
                swapping2.CopyTo(swapping1);
                temp.CopyTo(swapping2);
            }
        }
    }
}
