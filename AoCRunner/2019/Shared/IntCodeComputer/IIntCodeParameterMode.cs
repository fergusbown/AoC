namespace AoCRunner
{
    internal interface IIntCodeParameterMode
    {
        long GetValue(long initialValue, IntCodeComputer.ProgramState state);
    }

}
