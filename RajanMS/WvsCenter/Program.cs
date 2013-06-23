using System;
using Common;

namespace WvsCenter
{
    class Program
    {
        public static int SpecialId = 0;

        static void Main(string[] args)
        {
            Logger.InitConsole("WvsCenter v111");

            MasterServer.Instance.Start(6969);

            Console.ReadLine();

            MasterServer.Instance.Stop();
        }
    }
}
