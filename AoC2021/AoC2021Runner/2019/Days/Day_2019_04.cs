namespace AoC2021Runner;

internal partial class Day_2019_04 : IDayChallenge
{
    private readonly int[] min;
    private readonly int[] max;

    public Day_2019_04()
    {
        this.min = new[] { 2, 3, 1, 8, 3, 2 }; // supplied
        this.min = new[] { 2, 3, 3, 3, 3, 3 }; // implied

        this.max = new[] { 7, 6, 7, 3, 4, 6 }; //supplied
        this.max = new[] { 6, 9, 9, 9, 9, 9 }; //implied
    }

    public string Part1()
    {
        return GetValidPasswordCount(HasPair).ToString();

        bool HasPair(int[] password)
        {
            int previous = password[0];
            for (int i = 1; i < password.Length; i++)
            {
                int next = password[i];
                if (previous == next)
                {
                    return true;
                }

                previous = next;
            }

            return false;
        }
    }

    public string Part2()
    {
        return GetValidPasswordCount(HasExactPair).ToString();

        bool HasExactPair(int[] password)
        {
            int previous = password[0];
            int matchingCount = 0;
            for (int i = 1; i < password.Length; i++)
            {
                int next = password[i];

                if (previous == next)
                {
                    matchingCount++;
                }
                else
                {
                    if (matchingCount == 1)
                    {
                        return true;
                    }

                    matchingCount = 0;
                }

                previous = next;
            }

            return matchingCount == 1;
        }
    }

    private int GetValidPasswordCount(Func<int[], bool> isValidSequence)
    {
        int validPasswords = 0;
        int[] password = new int[6];

        for (int digit0 = 2; digit0 <= 6; digit0++)
        {
            password[0] = digit0;
            for (int digit1 = Math.Max(digit0, 3); digit1 <= 9; digit1++)
            {
                password[1] = digit1;
                for (int digit2 = digit1; digit2 <= 9; digit2++)
                {
                    password[2] = digit2;
                    for (int digit3 = digit2; digit3 <= 9; digit3++)
                    {
                        password[3] = digit3;
                        for (int digit4 = digit3; digit4 <= 9; digit4++)
                        {
                            password[4] = digit4;
                            for (int digit5 = digit4; digit5 <= 9; digit5++)
                            {
                                password[5] = digit5;
                                if (isValidSequence(password))
                                {
                                    validPasswords++;
                                }
                            }
                        }
                    }
                }
            }
        }

        return validPasswords;
    }
}