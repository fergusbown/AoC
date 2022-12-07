namespace AoCRunner;

internal class Day_2022_06 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_06(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        return FindStart(this.inputData, 4).ToString();
    }

    public string Part2()
    {
        return FindStart(this.inputData, 14).ToString();
    }

    static int FindStart(string input,int length)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input.Skip(i).Take(length).Distinct().Count() == length)
            {
                return i + length;
            }
        }

        return -1;
    }
}
