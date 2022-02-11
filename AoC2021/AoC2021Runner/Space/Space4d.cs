namespace AoC2021Runner;

public class Space4d<TElementType>
    where TElementType : new()
{
    private readonly TElementType[,,,] data;

    public Space4d(int width, int height, int depth, int metaDimensionSize)
    {
        this.data = CreateData(width, height, depth, metaDimensionSize);
    }

    public TElementType this[int x, int y, int z, int w]
    {
        get => data[x, y, z, w];
        set => data[x, y, z, w] = value;
    }

    public int Width => data.GetLength(0);

    public int Height => data.GetLength(1);

    public int Depth => data.GetLength(2);

    public int MetaDimensionSize => data.GetLength(3);

    public IEnumerable<TElementType> Adjacencies(int x, int y, int z, int w)
    {
        for (int adjX = Math.Max(0, x - 1); adjX < Math.Min(x + 2, Width); adjX++)
        {
            for (int adjY = Math.Max(0, y - 1); adjY < Math.Min(y + 2, Height); adjY++)
            {
                for (int adjZ = Math.Max(0, z - 1); adjZ < Math.Min(z + 2, Depth); adjZ++)
                {
                    for (int adjW = Math.Max(0, w - 1); adjW < Math.Min(w + 2, MetaDimensionSize); adjW++)
                    {
                        if (x != adjX || y != adjY || z != adjZ || w != adjW)
                        {
                            yield return data[adjX, adjY, adjZ, adjW];
                        }
                    }
                }
            }
        }
    }

    private static TElementType[,,,] CreateData(int width, int height, int depth, int metaDimensionSize)
    {
        TElementType[,,,] data = new TElementType[width, height, depth, metaDimensionSize];

        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int z = 0; z < data.GetLength(2); z++)
                {
                    for (int w = 0; w < data.GetLength(3); w++)
                    {
                        data[x, y, z, w] = new();
                    }
                }
            }
        }

        return data;
    }
}