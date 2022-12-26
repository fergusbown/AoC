namespace AoCRunner;

internal class Day_2022_20 : IDayChallenge
{
    private readonly string inputData;

    public Day_2022_20(string inputData)
    {
        this.inputData = inputData;
    }

    public string Part1()
    {
        var input = this.inputData
            .IntsForDay()
            .Select(x => (long)x)
            .ToArray();

        return Mix(input, 1).ToString();
    }

    public string Part2()
    {
        var input = this.inputData
            .IntsForDay()
            .Select(x => x * 811589153L)
            .ToArray();

        return Mix(input, 10).ToString();
    }

    private static long Mix(long[] initialNumbers, int mixCount)
    {
        var list = new LinkedList<long>(initialNumbers);
        var originalOrder = new LinkedListNode<long>[list.Count];

        var current = list.First;
        int index = 0;

        while (current is not null)
        {
            originalOrder[index++] = current!;
            current = current.Next;
        }

        Func<LinkedListNode<long>, LinkedListNode<long>> getBefore
            = c => c.Previous ?? list.Last ?? throw new InvalidOperationException("there should be stuff");
        Func<LinkedListNode<long>, LinkedListNode<long>> getAfter
            = c => c.Next ?? list.First ?? throw new InvalidOperationException("there should be stuff");

        Action<LinkedListNode<long>, LinkedListNode<long>> addBefore = list.AddBefore;
        Action<LinkedListNode<long>, LinkedListNode<long>> addAfter = list.AddAfter;

        for (int m = 0; m < mixCount; m++)
        {
            foreach (var item in originalOrder)
            {
                long shiftsRequired = item.Value % (originalOrder.Length - 1);

                if (shiftsRequired != 0)
                {
                    (var getTarget, var placeTarget) = shiftsRequired < 0 
                        ? (getBefore, addBefore) 
                        : (getAfter, addAfter);

                    var target = getTarget(item);
                    list.Remove(item);

                    for (long i = 1; i < Math.Abs(shiftsRequired); i++)
                    {
                        target = getTarget(target);
                    }

                    placeTarget(target, item);
                }
            }
        }

        var location = list.Find(0) ?? throw new InvalidOperationException("there should be a 0 entry");

        long groveCoord = 0;

        for (int c = 0; c < 3; c++)
        {
            for (int i = 0; i < 1000; i++)
            {
                location = getAfter(location);
            }

            groveCoord += location.Value;
        }

        return groveCoord;
    }

}
