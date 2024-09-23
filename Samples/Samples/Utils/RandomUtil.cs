using System;

namespace Samples.Utils
{
    static class RandomUtil
    {
        private static Random _random = new Random(DateTime.Now.Millisecond);

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }
    }
}
