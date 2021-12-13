// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Reflection;
using AoC2021Runner;

Stopwatch sw = Stopwatch.StartNew();

Console.WriteLine("Initialising...");

IEnumerable<(int Day, IDayChallenge Solution)> daySolutions = Assembly
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

Console.WriteLine($"Initialised{sw.ElapsedString()}");

foreach ((int day, IDayChallenge solution) in daySolutions)
{
    Console.WriteLine($"Day {day}...");
    string result = solution.Part1();
    Console.WriteLine($"  Part 1{sw.ElapsedString()}: {result}");
    result = solution.Part2();
    Console.WriteLine($"  Part 2{sw.ElapsedString()}: {result}");
}
