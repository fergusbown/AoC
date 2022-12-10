using System.Text;

namespace AoCRunner;

internal class Day_2022_10 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_10(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        VideoSystem system = new VideoSystem(inputData);

        int[] cycles = system.Run().ToArray();

        int result = 0;

        for (int i = 19; i <= 219; i += 40)
        {
            result += (i + 1) * cycles[i];
        }

        return result.ToString();
    }

    public string Part2()
    {
        VideoSystem system = new VideoSystem(inputData);

        StringBuilder sb = new();

        int drawingIndex = 0;

        foreach(int x in system.Run())
        {
            if (drawingIndex == 0)
            {
                sb.AppendLine();
            }

            if ((x >= drawingIndex - 1) && (x <= drawingIndex + 1))
            {
                sb.Append('#');
            }
            else
            {
                sb.Append(' ');
            }

            drawingIndex = (drawingIndex + 1) % 40;
        }

        return sb.ToString();
    }

    private interface IOperation
    {
        int Duration { get; }

        int Execute(int register);
    }

    private class Noop : IOperation
    {
        public int Duration => 1;

        public int Execute(int register)
            => register;

        public static IOperation Instance { get; } = new Noop();
    }

    private class AddX : IOperation
    {
        private readonly int increment;

        public AddX(int increment)
        {
            this.increment = increment;
        }

        public int Duration => 2;

        public int Execute(int register) => register + increment;
    }

    private class VideoSystem
    {
        private readonly IReadOnlyList<IOperation> operations;

        private int x = 1;

        public VideoSystem(string inputData)
        {
            operations = inputData
                .StringsForDay()
                .Select(s => s == "noop" ? Noop.Instance : new AddX(int.Parse(s.Split(' ').Last())))
                .ToArray();
        }

        public IEnumerable<int> Run()
        {
            x = 1;

            foreach (var operation in operations)
            {
                for (int i = 0; i < operation.Duration; i++)
                {
                    yield return x;
                }

                x = operation.Execute(x);
            }
        }
    }
}
