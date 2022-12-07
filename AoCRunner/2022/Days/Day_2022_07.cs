namespace AoCRunner;

internal class Day_2022_07 : IDayChallenge
{
    private readonly Directory inputData;

    public Day_2022_07(string inputData)
    {
        this.inputData = Directory.Read(inputData);
    }

    public string Part1()
    {
        return inputData
            .AllSubdirectories
            .Select(d => d.TotalSize)
            .Where(d => d <= 100000)
            .Sum()
            .ToString();
    }

    public string Part2()
    {
        int spaceFree = 70000000 - inputData.TotalSize;
        int spaceToFree = 30000000 - spaceFree;

        return inputData
            .AllSubdirectories
            .Select(d => d.TotalSize)
            .Where(d => d >= spaceToFree)
            .OrderBy(d => d)
            .First()
            .ToString();
    }

    private class Directory
    {
        private readonly Dictionary<string, Directory> subdirectories = new();
        private int size = 0;

        private Directory()
        {

        }

        public string Name { get; init; } = string.Empty;

        public Directory? Parent { get; init; } = null;

        public Directory this[string subdrectory] => subdrectory switch
        {
            ".." => Parent!,
            _ => subdirectories[subdrectory],
        };

        public int TotalSize => size + subdirectories.Values.Select(d => d.TotalSize).Sum();

        public IEnumerable<Directory> AllSubdirectories
            => this.subdirectories.Values.Concat(this.subdirectories.Values.SelectMany(d => d.AllSubdirectories));

        public static Directory Read(string inputData)
        {
            string[] output = inputData.StringsForDay();

            Directory result = new Directory();
            Directory current = result;

            foreach (string s in output.Skip(1))
            {
                string[] parts = s.Split(' ');
                if (parts[0] == "$")
                {
                    if (parts[1] == "cd")
                    {
                        current = current[parts[2]];
                    }
                }
                else
                {
                    if (parts[0] == "dir")
                    {
                        current.AddDirectory(parts[1]);
                    }
                    else
                    {
                        current.AddFile(int.Parse(parts[0]));
                    }
                }
            }

            return result;
        }

        private void AddDirectory(string name)
        {
            subdirectories.Add(name, new Directory { Name = name, Parent = this });
        }

        private void AddFile(int size)
        {
            this.size += size;
        }
    }
}
