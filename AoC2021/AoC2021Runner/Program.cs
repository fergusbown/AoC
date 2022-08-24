using System.Diagnostics;
using System.Reflection;
using AoC2021Runner;
using CommandLine;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<Options>(args).MapResult(
            async o =>
            {
                var daySolutions = GetSolutionImplementations(GetSolutionDefinitions(o));

                foreach ((int year, int day, IAsyncDayChallenge solution) in daySolutions)
                {
                    Console.WriteLine($"{year} day {day}...");
                    _ = await TimeOperation(solution.Part1, (s, t) => $"  Part 1{t}: {s}");
                    _ = await TimeOperation(solution.Part2, (s, t) => $"  Part 2{t}: {s}");
                }

                return true;
            },
            errors => Task.FromResult(false));

        static IEnumerable<(int Year, int Day, Type Type)> GetSolutionDefinitions(Options options)
        {
            var result = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => (t.IsAssignableTo(typeof(IDayChallenge)) || t.IsAssignableTo(typeof(IAsyncDayChallenge))) && t.IsClass && t != typeof(DayChallengeAdapter))
                .Select(t =>
                {
                    (int year, int day, Type type) result = (int.Parse(t.Name[4..8]), int.Parse(t.Name[9..]), t);
                    return result;
                })
                .Where(t => options.Year is null || t.year == options.Year)
                .OrderBy(t => t.year)
                .ThenBy(t => t.day);

            return options.LatestOnly ? result.TakeLast(1) : result;
        }

        static IEnumerable<(int Year, int Day, IAsyncDayChallenge Solution)> GetSolutionImplementations(IEnumerable<(int Year, int Day, Type Type)> definitions)
        {
            return definitions.Select(t =>
            {
                string? inputData = InputData.InputForDay(t.Type);

                var instance = inputData is null ? Activator.CreateInstance(t.Type) : Activator.CreateInstance(t.Type, inputData);

                if (instance is IDayChallenge dayChallenge)
                {
                    instance = new DayChallengeAdapter(dayChallenge);
                }

                (int, int, IAsyncDayChallenge) result = (t.Year, t.Day, (IAsyncDayChallenge)instance!);
                return result;
            })
            .ToList();
        }

        static async Task<T> TimeOperation<T>(Func<Task<T>> operation, Func<T, string, string> logFunction)
        {
            Stopwatch sw = Stopwatch.StartNew();
            T result = await operation();

            Console.WriteLine(logFunction(result, $" ({sw.Elapsed:g})"));

            return result;
        }
    }
}

public class Options
{
    [Option('v', "year", Required = false)]
    public int? Year { get; set; }

    [Option('l', "latest", Required = false)]
    public bool LatestOnly { get; set; }
}
