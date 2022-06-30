namespace AoC2021Runner
{
    internal interface IIntCodeParameterMode
    {
        long GetValue(long initialValue, IntCodeComputer.ProgramState state);
    }

}
