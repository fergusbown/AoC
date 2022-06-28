namespace AoC2021Runner;

internal interface IDayChallenge
{
    string Part1();

    string Part2();
}

internal class DayChallengeAdapter : IAsyncDayChallenge
{
    private readonly IDayChallenge instance;

    public DayChallengeAdapter(IDayChallenge instance)
    {
        this.instance = instance;
    }

    public Task<string> Part1()
    {
        return Task.FromResult(instance.Part1());
    }

    public Task<string> Part2()
    {
        return Task.FromResult(instance.Part2());
    }
}
