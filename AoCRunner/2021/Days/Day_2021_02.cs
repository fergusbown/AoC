﻿namespace AoCRunner;

internal class Day_2021_02 : IDayChallenge
{
    private readonly (Instruction, int)[] instructions;

    public Day_2021_02(string inputData)
    {
        this.instructions = inputData.InstructionsForDay<Instruction>();
    }

    public string Part1()
        => CalculatePosition(new Position());

    public string Part2()
        => CalculatePosition(new AimPosition());

    private string CalculatePosition(IPosition position)
    {
        foreach ((Instruction instruction, int amount) in instructions)
        {
            position.Apply(instruction, amount);
        }

        return $"{position.HorizontalPosition * position.Depth}";
    }


    private enum Instruction
    {
        Down,
        Up,
        Forward,
    }

    private interface IPosition
    {
        long Depth { get; }
        long HorizontalPosition { get; }

        void Apply(Instruction instruction, int amount);
    }

    private class Position : IPosition
    {
        public long Depth { get; private set; }
        public long HorizontalPosition { get; private set; }

        public void Apply(Instruction instruction, int amount)
        {
            switch (instruction)
            {
                case Instruction.Down:
                    this.Depth += amount;
                    break;
                case Instruction.Up:
                    this.Depth -= amount;
                    break;
                case Instruction.Forward:
                    this.HorizontalPosition += amount;
                    break;
                default:
                    break;
            }
        }
    }

    private class AimPosition : IPosition
    {
        private long aim;

        public long Depth { get; private set; }

        public long HorizontalPosition { get; private set; }

        public void Apply(Instruction instruction, int amount)
        {
            switch (instruction)
            {
                case Instruction.Down:
                    this.aim += amount;
                    break;
                case Instruction.Up:
                    this.aim -= amount;
                    break;
                case Instruction.Forward:
                    this.HorizontalPosition += amount;
                    this.Depth += this.aim * amount;
                    break;
                default:
                    break;
            }
        }
    }
}