namespace AoCRunner
{
    internal class PositionMode : IIntCodeParameterMode
    {
        public long GetValue(long initialValue, IntCodeComputer.ProgramState state)
        {
            return state[initialValue];
        }
    }
}
