// See https://aka.ms/new-console-template for more information


using System.Reflection;
using AoC2021Runner;

IEnumerable<(int Day, IDayChallenge Solution)> daySolutions = Assembly
    .GetExecutingAssembly()
    .GetTypes()
    .Where(t => t.IsAssignableTo(typeof(IDayChallenge)) && t.IsClass)
    .Select(t =>
    {
        string inputData = InputData.InputForDay(t);
        return (int.Parse(t.Name.Replace("Day", "")), (IDayChallenge)Activator.CreateInstance(t, inputData)!);
    })
    .OrderBy(t => t.Item1);

foreach ((int day, IDayChallenge solution) in daySolutions)
{
    Console.WriteLine($"Day {day}...");
    Console.WriteLine($"  Part 1: {solution.Part1()}");
    Console.WriteLine($"  Part 2: {solution.Part2()}");
}
