using System;

namespace RajanMS.Tools
{
    public static class Randomizer
    {
        private static Random sRandom = new Random();

        public static void NextBytes(byte[] bytes)
        {
            sRandom.NextBytes(bytes);
        }

        public static int Generate()
        {
            return sRandom.Next();
        }
        public static int Generate(int high)
        {
            return sRandom.Next(high);
        }
        public static int Generate(int low, int high)
        {
            return sRandom.Next(low, high);
        }
    }
}
