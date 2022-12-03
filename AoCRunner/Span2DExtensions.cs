using Microsoft.Toolkit.HighPerformance;
using Microsoft.Toolkit.HighPerformance.Enumerables;
using System.Text;

namespace AoCRunner;

public static class Span2DExtensions
{
    public static void TransposeColumns<T>(this Span2D<T> span)
    {
        int firstColumnIndex = span.Width - 1;
        Span<T> temp = new(new T[span.Height]);
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
        Span<T> temp = new(new T[span.Width]);
        for (int secondRowIndex = 0; secondRowIndex < span.Height / 2; secondRowIndex++, firstRowIndex--)
        {
            Span<T> swapping1 = span.GetRowSpan(firstRowIndex);
            Span<T> swapping2 = span.GetRowSpan(secondRowIndex);
            swapping1.CopyTo(temp);
            swapping2.CopyTo(swapping1);
            temp.CopyTo(swapping2);
        }
    }

    public static Span2D<T> FlipVertical<T>(this Span2D<T> span)
    {
        for (int column = 0; column < span.Width; column++)
        {
            int endRow = span.Height - 1;
            for (int row = 0; row < span.Height / 2; row++, endRow--)
            {
                var top = span[row, column];
                var bottom = span[endRow, column];

                span[row, column] = bottom;
                span[endRow, column] = top;
            }
        }

        return span;
    }

    public static Span2D<T> RotateRight<T>(this Span2D<T> span)
    {
        if (span.Width != span.Height)
        {
            throw new NotImplementedException("span must be square to rotate in place");
        }

        int circuitEndIndex = span.Width - 1;
        for (int circuitStartIndex = 0; circuitStartIndex < span.Width / 2; circuitStartIndex++, circuitEndIndex--)
        {
            for (int circuitOffset = 0; circuitOffset < circuitEndIndex - circuitStartIndex; circuitOffset++)
            {
                var first = span[circuitStartIndex, circuitStartIndex + circuitOffset];
                var second = span[circuitStartIndex + circuitOffset, circuitEndIndex];
                var third = span[circuitEndIndex, circuitEndIndex - circuitOffset];
                var fourth = span[circuitEndIndex - circuitOffset, circuitStartIndex];

                span[circuitStartIndex, circuitStartIndex + circuitOffset] = fourth;
                span[circuitStartIndex + circuitOffset, circuitEndIndex] = first;
                span[circuitEndIndex, circuitEndIndex - circuitOffset] = second;
                span[circuitEndIndex - circuitOffset, circuitStartIndex] = third;
            }
        }

        return span;
    }

    public static void AddToStringBuilder<T, TOut>(this Span2D<T> span, StringBuilder sb, Func<T, TOut> converter)
    {
        for (int rowIndex = 0; rowIndex < span.Height; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < span.Width; columnIndex++)
            {
                sb.Append(converter(span[rowIndex, columnIndex]));
            }

            sb.AppendLine();
        }
    }
    public static string ToString<T, TOut>(this Span2D<T> span, Func<T, TOut> converter)
    {
        StringBuilder sb = new();

        span.AddToStringBuilder(sb, converter);

        return sb.ToString();
    }
}