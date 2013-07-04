using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RajanMS;
using RajanMS.Tools;

namespace RajanMS.Servers
{
    sealed class MasterServer
    {
        public static MasterServer Instance { get; set; }

        public bool Running { get; private set; }

        public LoginServer LoginServer { get; private set; }
        public WorldServer[] Worlds { get; private set; }
        public Database Database { get; private set; }

        public ConfigReader Config { get; private set; }

        public MasterServer()
        {
            Config = new ConfigReader(Constants.ConfigName);

            Database = new Database(Config["Database", "Host"], Config["Database", "Name"]);

            int worlds = Config["Server", "Worlds"].ToInt32();
            byte channels = (byte)Config["Server", "Channels"].ToInt32();

            if (channels > 20)
                throw new Exception("More than 20 channels");

            if (worlds > Constants.WorldNames.Length)
                throw new Exception("More than supported worlds");

            LoginServer = new LoginServer(8484);
            Worlds = new WorldServer[worlds];

            short port = 8485;

            for (int i = 0; i < worlds; i++)
            {
                Worlds[i] = new WorldServer((byte)i, port, channels);
                port += channels;
            }
        }

        public void Run()
        {
            LoginServer.Run();

            foreach (WorldServer ws in Worlds)
                ws.Run();

            Running = true;

            MainForm.Instance.Log("RajanMS is online");
        }
        public void Shutdown()
        {
            MainForm.Instance.Log("Shutting down MasterServer");

            LoginServer.Shutdown();

            foreach (WorldServer ws in Worlds)
                ws.Shutdown();

            Database.SetAllOffline();

            Running = false;

            MainForm.Instance.Log("RajanMS is offline");
        }
    }
}
