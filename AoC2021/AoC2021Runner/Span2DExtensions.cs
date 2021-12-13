using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Enumerables;

namespace AoC2021Runner
{
    public static class Span2DExtensions
    {
        public static void TransposeColumns<T>(this Span2D<T> span)
        {
            int firstColumnIndex = span.Width - 1;
            Span<T> temp = new Span<T>(new T[span.Height]);
            for (int secondColumnIndex = 0; secondColumnIndex < span.Width / 2; secondColumnIndex++, firstColumnIndex--)
            {
                RefEnumerable<T> swapping1 = span.GetColumn(firstColumnIndex);
                RefEnumerable<T> swapping2 = span.GetColumn(secondColumnIndex);

                swapping1.CopyTo(temp);
                swapping2.CopyTo(swapping1);
                temp.CopyTo(swapping2);
            }
        }

        public static void TransposeRows<T>(this Span2D<T> span)
        {
            int firstRowIndex = span.Height - 1;
            Span<T> temp = new Span<T>(new T[span.Width]);
            for (int secondRowIndex = 0; secondRowIndex < span.Height / 2; secondRowIndex++, firstRowIndex--)
            {
                Span<T> swapping1 = span.GetRowSpan(firstRowIndex);
                Span<T> swapping2 = span.GetRowSpan(secondRowIndex);
                swapping1.CopyTo(temp);
                swapping2.CopyTo(swapping1);
                temp.CopyTo(swapping2);
            }
        }
    }
}
