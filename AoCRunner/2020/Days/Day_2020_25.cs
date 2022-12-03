namespace AoCRunner;

internal partial class Day_2020_25 : IDayChallenge
{
    private readonly int publicKey1;
    private readonly int publicKey2;

    public Day_2020_25()
    {
        this.publicKey1 = 11562782;
        this.publicKey2 = 18108497;
    }

    public string Part1()
    {
        return CalculateEncryptionKey(publicKey1, publicKey2).ToString();
    }

    public string Part2()
    {
        return "Happy Christmas";
    }

    private static long CalculateEncryptionKey(int publicKey1, int publicKey2)
    {
        long value = 1;
        int loopSize = 0;
        do
        {
            Transform(ref value, 7);
            loopSize++;
        }
        while (value != publicKey1);

        value = 1;
        for (int i = 0; i < loopSize; i++)
        {
            Transform(ref value, publicKey2);
        }

        return value;

        static void Transform(ref long value, int subjectNumber)
        {
            value *= subjectNumber;
            value %= 20201227;
        }
    }
}
