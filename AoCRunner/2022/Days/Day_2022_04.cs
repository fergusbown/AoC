namespace AoCRunner;

internal class Day_2022_04 : IDayChallenge
{
    private readonly (Assignment, Assignment)[] inputData;

    public Day_2022_04(string inputData)
    {
        this.inputData = inputData
            .StringsForDay()
            .Select(s => s.Split('-', ','))
            .Select(p => (new Assignment(int.Parse(p[0]), int.Parse(p[1])), new Assignment(int.Parse(p[2]), int.Parse(p[3]))))
            .ToArray();
    }

    public string Part1()
    {
        return this.inputData
            .Count(i => i.Item1.Contains(i.Item2) || i.Item2.Contains(i.Item1))
            .ToString();
    }

    public string Part2()
    {
        return this.inputData
            .Count(i => i.Item1.Overlaps(i.Item2))
            .ToString();
    }

    public record Assignment(int Start, int End)
    {
        public bool Contains(Assignment other)
        {
            return other.Start >= this.Start && other.End <= this.End;
        }

        public bool Overlaps(Assignment other)
        {
            return Overlaps(other.Start) || Overlaps(other.End) || other.Contains(this);

            bool Overlaps(int location) => location >= this.Start && location <= this.End;
        }
    }
}
