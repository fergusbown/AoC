namespace AoC2021Runner
{
    internal class PositionMode : IIntCodeParameterMode
    {
        public long GetValue(long initialValue, IntCodeComputer.ProgramState state)
        {
            return state[initialValue];
        }
    }
}
