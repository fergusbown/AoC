namespace AoCRunner
{
    internal class RelativeImmediateMode : IIntCodeParameterMode
    {
        public long GetValue(long initialValue, IntCodeComputer.ProgramState state)
        {
            return initialValue + state.RelativeBase;
        }
    }
}
