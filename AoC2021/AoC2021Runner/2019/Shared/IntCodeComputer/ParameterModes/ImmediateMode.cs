﻿namespace AoC2021Runner
{
    internal class ImmediateMode : IIntCodeParameterMode
    {
        public long GetValue(long initialValue, IntCodeComputer.ProgramState state)
        {
            return initialValue;
        }
    }
}
