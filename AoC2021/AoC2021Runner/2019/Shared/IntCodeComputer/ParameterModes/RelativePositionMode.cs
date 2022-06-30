namespace AoC2021Runner
{
    internal class RelativePositionMode : IIntCodeParameterMode
    {
        public long GetValue(long initialValue, IntCodeComputer.ProgramState state)
        {
            return state[initialValue + state.RelativeBase];
        }
    }
}
