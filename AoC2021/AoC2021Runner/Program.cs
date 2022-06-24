﻿using System.Diagnostics;
using System.Reflection;
using AoC2021Runner;

var daySolutions = TimeOperation(() => GetSolutions(args), (s, t) => $"Initialised{t}");

foreach ((int year, int day, IDayChallenge solution) in daySolutions)
{
    Console.WriteLine($"{year} day {day}...");
    _ = TimeOperation(solution.Part1, (s, t) => $"  Part 1{t}: {s}");
    _ = TimeOperation(solution.Part2, (s, t) => $"  Part 2{t}: {s}");
}

static IEnumerable<(int year, int Day, IDayChallenge Solution)> GetSolutions(string[] args)
{
    return Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.IsAssignableTo(typeof(IDayChallenge)) && t.IsClass)
        .Select(t =>
        {
            (int year, int day, Type type) result = (int.Parse(t.Name[4..8]), int.Parse(t.Name[9..]), t);
            return result;
        })
        .Where(t => t.year == 2020)
        .OrderBy(t => t.year)
        .ThenBy(t => t.day)
        .TakeLast(1)
        .Select(t =>
        {
            string? inputData = InputData.InputForDay(t.type);

            var instance = inputData is null ? Activator.CreateInstance(t.type) : Activator.CreateInstance(t.type, inputData);

            (int year, int day, IDayChallenge solution) result = (t.year, t.day, (IDayChallenge)instance!);
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
