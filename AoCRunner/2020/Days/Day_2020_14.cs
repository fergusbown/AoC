namespace AoCRunner;

internal class Day_2020_14 : IDayChallenge
{
    private readonly string[] inputData;

    public Day_2020_14(string inputData)
    {
        this.inputData = inputData.StringsForDay();
    }

    public string Part1()
    {
        return $"{DockingProgram.Run(inputData, new DockingExecutorV1())}";
    }

    public string Part2()
    {
        return $"{DockingProgram.Run(inputData, new DockingExecutorV2())}";
    }

    private interface IDockingExecutor
    {
        void SetMask(string mask);

        void SetMemory(long slot, long value, Dictionary<long, long> storage);
    }

    private class DockingExecutorV1 : IDockingExecutor
    {
        private long setMask = 0;
        private long unsetMask = 0;

        public void SetMask(string mask)
        {
            setMask = 0;
            unsetMask = 0;
            foreach (var ch in mask)
            {
                setMask <<= 1;
                unsetMask <<= 1;
                switch (ch)
                {
                    case '0':
                        unsetMask += 1;
                        break;
                    case '1':
                        setMask += 1;
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetMemory(long slot, long value, Dictionary<long, long> storage)
        {
            long result = (value | setMask) & (~unsetMask);

            if (result == 0)
            {
                storage.Remove(slot);
            }
            else
            {
                storage[slot] = result;
            }
        }
    }

    private class DockingExecutorV2 : IDockingExecutor
    {
        private IEnumerable<(long setMask, long unsetMask)> masks = Array.Empty<(long setMask, long unsetMask)>();

        public void SetMask(string mask)
        {
            List<(long setMask, long unsetMask)> pendingMasks = new()
            {
                (0, 0),
            };

            foreach (var ch in mask)
            {
                List<(long setMask, long unsetMask)> nextMasks = new();
                foreach (var masks in pendingMasks)
                {
                    (var setMask, var unsetMask) = masks;

                    setMask <<= 1;
                    unsetMask <<= 1;
                    switch (ch)
                    {
                        case 'X':
                            nextMasks.Add((setMask, unsetMask + 1));
                            nextMasks.Add((setMask + 1, unsetMask));
                            break;
                        case '1':
                            nextMasks.Add((setMask + 1, unsetMask));
                            break;
                        default:
                            nextMasks.Add((setMask, unsetMask));
                            break;
                    }
                }

                pendingMasks = nextMasks;
            }

            masks = pendingMasks;
        }

        public void SetMemory(long slot, long value, Dictionary<long, long> storage)
        {
            foreach ((var setMask, var unsetMask) in masks)
            {
                long targetSlot = (slot | setMask) & (~unsetMask);
                storage[targetSlot] = value;
            }
        }
    }


    private static class DockingProgram
    {
        public static long Run(string[] program, IDockingExecutor executor)
        {
            Dictionary<long, long> storage = new();

            foreach (var command in program)
            {
                var commandParts = command
                    .Split(new string[] { "[", "] = ", " = " }, StringSplitOptions.None);

                switch (commandParts[0])
                {
                    case "mask":
                        executor.SetMask(commandParts[1]);
                        break;
                    case "mem":
                        executor.SetMemory(long.Parse(commandParts[1]), long.Parse(commandParts[2]), storage);
                        break;
                    default:
                        break;
                }
            }

            return storage.Values.Sum();
        }
    }
}