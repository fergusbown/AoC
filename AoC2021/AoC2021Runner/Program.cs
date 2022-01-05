using System.Diagnostics;
using System.Reflection;
using AoC2021Runner;

var daySolutions = TimeOperation(GetSolutions, (s, t) => $"Initialised{t}");

foreach ((int day, IDayChallenge solution) in daySolutions)
{
    Console.WriteLine($"Day {day}...");
    _ = TimeOperation(solution.Part1, (s, t) => $"  Part 1{t}: {s}");
    _ = TimeOperation(solution.Part2, (s, t) => $"  Part 2{t}: {s}");
}

static IEnumerable<(int Day, IDayChallenge Solution)> GetSolutions()
{
    return Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsAssignableTo(typeof(IDayChallenge)) && t.IsClass)
        .Select(t =>
        {
            string inputData = InputData.InputForDay(t);
            return (int.Parse(t.Name.Replace("Day", "")), (IDayChallenge)Activator.CreateInstance(t, inputData)!);
        })
        .OrderBy(t => t.Item1)
        .ToList();
}

static T TimeOperation<T>(Func<T> operation, Func<T, string, string> logFunction)
{
    Stopwatch sw = Stopwatch.StartNew();
    T result = operation();
    
    Console.WriteLine(logFunction(result, $" ({sw.ElapsedMilliseconds}ms)"));

    return result;
}
