using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace RajanMS.Servers
{
    sealed class MasterServer
    {
        public static MasterServer Instance { get; set; }

        public LoginServer LoginServer { get; private set; }
        public WorldServer[] Worlds { get; private set; }
        public Database Database { get; private set; }

        public MasterServer(int worlds,short channels)
        {
            Database = new Database("mongodb://localhost","RajanMS");

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
        }
        public void Shutdown()
        {
            LoginServer.Shutdown();

            foreach (WorldServer ws in Worlds)
                ws.Shutdown();
        }
    }
}
