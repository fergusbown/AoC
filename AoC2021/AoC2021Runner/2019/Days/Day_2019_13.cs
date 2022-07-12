namespace AoC2021Runner;

internal partial class Day_2019_13 : IAsyncDayChallenge
{
    private readonly long[] inputData;

    public Day_2019_13(string inputData)
    {
        this.inputData = IntCodeComputer.GetProgram(inputData);
    }

    public async Task<string> Part1()
    {
        ArcadeGame game = new(inputData);
        int blockTiles = await game.Run();
        return blockTiles.ToString(); ;
    }

    public async Task<string> Part2()
    {
        inputData[0] = 2;
        ArcadeGame game = new(inputData);
        int blockTiles = await game.Run();
        return game.Score.ToString();
    }

    private class ArcadeGame
    {
        private readonly long[] program;

        private int outputIndex;

        private long x;
        private long y;

        private long xBall;
        private long xPaddle;

        private int blocksPainted = 0;

        public ArcadeGame(long[] program)
        {
            this.program = program;
        }
        public long Score { get; private set; }

        public async Task<int> Run()
        {
            var computer = IntCodeComputer.New();

            await computer
                .PipeInputFrom(HandleInput)
                .PipeOutputTo(HandleOutput)
                .Run(program);

            return blocksPainted;
        }

        private Task<long> HandleInput()
        {
            // move the paddle towards the ball
            long diff = xBall - xPaddle;

            return diff switch
            {
                0 => Task.FromResult(0L),
                < 0 => Task.FromResult(-1L),
                _ => Task.FromResult(1L),
            };
        }

        private void HandleOutput(long output)
        {
            switch (outputIndex)
            {
                case 0:
                    x = output;
                    break;
                case 1:
                    y = output;
                    break;
                default:
                    if ((x, y) == (-1, 0))
                    {
                        Score = output;
                    }
                    else
                    {
                        switch (output)
                        {
                            case 2:
                                blocksPainted++;
                                break;
                            case 3:
                                xPaddle = x;
                                break;
                            case 4:
                                xBall = x;
                                break;
                        }
                    }
                    break;
            }

            outputIndex = (outputIndex + 1) % 3;
        }
    }
}
