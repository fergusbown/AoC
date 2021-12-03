using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoC2021Runner
{
    internal static class InputData
    {
        public static string[] StringsForDay(this string inputData)
            => inputData.Split(Environment.NewLine);

        public static int[] IntsForDay(this string inputData)
            => inputData.StringsForDay().Select(s => int.Parse(s)).ToArray();

        public static (T, int)[] InstructionsForDay<T>(this string inputData)
            where T : struct
        {
            return inputData
                .StringsForDay()
                .Select(s => s.Split(' '))
                .Select(s => (Enum.Parse<T>(s[0], true), int.Parse(s[1])))
                .ToArray();
        }

        public static int ToIntFromBinaryString(this string binaryString)
            => Convert.ToInt32(binaryString, 2);
    }
}
