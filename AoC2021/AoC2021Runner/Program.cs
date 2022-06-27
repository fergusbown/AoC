using System.Diagnostics;
using System.Reflection;
using AoC2021Runner;
using CommandLine;

var options = Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
{
    var daySolutions = TimeOperation(() => GetSolutionImplementations(GetSolutionDefinitions(o)), (s, t) => $"Initialised{t}");

    foreach ((int year, int day, IDayChallenge solution) in daySolutions)
    {
        Console.WriteLine($"{year} day {day}...");
        _ = TimeOperation(solution.Part1, (s, t) => $"  Part 1{t}: {s}");
        _ = TimeOperation(solution.Part2, (s, t) => $"  Part 2{t}: {s}");
    }
});

static IEnumerable<(int Year, int Day, Type Type)> GetSolutionDefinitions(Options options)
{
    var result = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsAssignableTo(typeof(IDayChallenge)) && t.IsClass)
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

static IEnumerable<(int Year, int Day, IDayChallenge Solution)> GetSolutionImplementations(IEnumerable<(int Year, int Day, Type Type)> definitions)
{
    return definitions.Select(t =>
    {
        string? inputData = InputData.InputForDay(t.Type);

        var instance = inputData is null ? Activator.CreateInstance(t.Type) : Activator.CreateInstance(t.Type, inputData);

        (int, int, IDayChallenge) result = (t.Year, t.Day, (IDayChallenge)instance!);
        return result;
    })
    .ToList();
}

static T TimeOperation<T>(Func<T> operation, Func<T, string, string> logFunction)
{
    Stopwatch sw = Stopwatch.StartNew();
    T result = operation();
    
    Console.WriteLine(logFunction(result, $" ({sw.ElapsedMilliseconds}ms)"));

    return result;
}

public class Options
{
    [Option('v', "year", Required = false)]
    public int? Year { get; set; }

    [Option('l', "latest", Required = false)]
    public bool LatestOnly { get; set; }
}
