using System.Collections;
using MoreLinq.Extensions;

namespace AoCRunner;

internal class Day_2022_05 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_05(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        (var stacks, var instructions) = Parse(inputData);

        foreach (var instruction in instructions)
        {
            Move(instruction.Move, stacks[instruction.From], stacks[instruction.To]);
        }

        return Message(stacks);
    }

    public string Part2()
    {
        (var stacks, var instructions) = Parse(inputData);
        Stack<char> temp = new();

        foreach (var instruction in instructions)
        {
            Move(instruction.Move, stacks[instruction.From], temp);
            Move(instruction.Move, temp, stacks[instruction.To]);
        }

        return Message(stacks);
    }

    public static void Move(int count, Stack<char> source, Stack<char> dest)
    {
        for (int i = 0; i < count; i++)
        {
            dest.Push(source.Pop());
        }
    }

    public string Message(Stack<char>[] stacks)
    {
        string result = "";

        foreach (var st in stacks)
        {
            result += st.Pop();
        }

        return result;
    }

    public static (Stack<char>[], (int Move, int From, int To)[]) Parse(string inputData)
    {
        var parts = inputData.Split($"{Environment.NewLine}{Environment.NewLine}");

        var stackDescriptions = parts[0].StringsForDay();

        Stack<char>[] stacks = new Stack<char>[9]; 
        for (int i = 0; i < 9; i++)
        {
            stacks[i] = new Stack<char>();
        }

        foreach (var line in stackDescriptions.Reverse().Skip(1))
        {
            int index = 0;
            for (int i = 0; i < line.Length; i += 4, index++)
            {
                char box = line[i+1];
                if (box != ' ')
                {
                    stacks[index].Push(box);
                }
            }
        }

        List<(int Move, int From, int To)> instructions = new();
        foreach(var instructionDescription in parts[1].StringsForDay())
        {
            var descriptionParts = instructionDescription.Split(' ');
            instructions.Add((int.Parse(descriptionParts[1]), int.Parse(descriptionParts[3]) - 1, int.Parse(descriptionParts[5]) - 1));
        }

        return (stacks, instructions.ToArray());
    }
}
