using CommunityToolkit.HighPerformance;

namespace AoCRunner
{
    internal class Cube<T>
    {
        private int populateIndex = 1;
        private (int, T[,]) frontFace;
        private (int, T[,]) backFace;
        private (int, T[,]) topFace;
        private (int, T[,]) bottomFace;
        private (int, T[,]) leftFace;
        private (int, T[,]) rightFace;

        public Cube(int size)
        {
            frontFace = (0, new T[size, size]);
            backFace = (0, new T[size, size]);
            topFace = (0, new T[size, size]);
            bottomFace = (0, new T[size, size]);
            leftFace = (0, new T[size, size]);
            rightFace = (0, new T[size, size]);
        }

        public void RotateLeft()
        {
            frontFace.Item2.AsSpan2D().TransposeColumns();
            backFace.Item2.AsSpan2D().TransposeColumns();

            for (int i = 0; i < 3; i++)
            {
                topFace.Item2.AsSpan2D().RotateRight();
                bottomFace.Item2.AsSpan2D().RotateRight();
            }

            (frontFace, leftFace, backFace, rightFace) = (rightFace, frontFace, leftFace, backFace);
        }

        public void RotateRight()
        {
            leftFace.Item2.AsSpan2D().TransposeColumns();
            rightFace.Item2.AsSpan2D().TransposeColumns();
            topFace.Item2.AsSpan2D().RotateRight();
            bottomFace.Item2.AsSpan2D().RotateRight();

            (frontFace, leftFace, backFace, rightFace) = (leftFace, backFace, rightFace, frontFace);
        }

        public void RotateUp()
        {
            leftFace.Item2.AsSpan2D().RotateRight();
            rightFace.Item2.AsSpan2D().RotateRight();

            frontFace.Item2.AsSpan2D().TransposeRows();
            backFace.Item2.AsSpan2D().TransposeRows();

            (frontFace, topFace, backFace, bottomFace) = (bottomFace, frontFace, topFace, backFace);
        }

        public void PopulateFront(Span2D<T> values)
        {
            frontFace.Item1 = populateIndex++;
            values.CopyTo(frontFace.Item2);
        }

        public ReadOnlySpan2D<T> Front => this.frontFace.Item2.AsSpan2D();

        public ReadOnlySpan2D<T> Left => this.leftFace.Item2.AsSpan2D();

        public ReadOnlySpan2D<T> Right => this.rightFace.Item2.AsSpan2D();

        public ReadOnlySpan2D<T> Top => this.topFace.Item2.AsSpan2D();

        public ReadOnlySpan2D<T> Bottom => this.bottomFace.Item2.AsSpan2D();

        public ReadOnlySpan2D<T> Back => this.backFace.Item2.AsSpan2D();
    }
}
