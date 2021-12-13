// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Reflection;
using AoC2021Runner;

Stopwatch sw = Stopwatch.StartNew();

Console.WriteLine("Initialising...");

IEnumerable<(int Day, IDayChallenge Solution)> daySolutions = Assembly
    .GetExecutingAssembly()
    .GetTypes()
    .Where(t => t.Name == "Day12")
    .Where(t => t.IsAssignableTo(typeof(IDayChallenge)) && t.IsClass)
    .Select(t =>
    {
        string inputData = InputData.InputForDay(t);
        return (int.Parse(t.Name.Replace("Day", "")), (IDayChallenge)Activator.CreateInstance(t, inputData)!);
    })
    .OrderBy(t => t.Item1);

Console.WriteLine($"Initialised{sw.ElapsedString()}");

foreach ((int day, IDayChallenge solution) in daySolutions)
{
    Console.WriteLine($"Day {day}...");
    Console.WriteLine($"  Part 1: {solution.Part1()}{sw.ElapsedString()}");
    Console.WriteLine($"  Part 2: {solution.Part2()}{sw.ElapsedString()}");
}
