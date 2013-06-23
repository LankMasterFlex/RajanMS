using System;
using Common;

namespace WvsLogin
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.InitConsole("WvsLogin v111");
            LoginServer.Instance.Connect(6969);
            LoginServer.Instance.Listen(8484);
            Console.ReadLine();
            LoginServer.Instance.Stop();
        }
    }
}
