using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajanMS.Servers
{
    class MasterServer
    {
        private LoginServer m_login;
        private List<WorldServer> m_worlds;

        public MasterServer(int worlds,short channels)
        {
            if (channels > 20)
                throw new Exception("More than 20 channels");


            m_login = new LoginServer(8484);
            m_worlds = new List<WorldServer>();

            short port = 8485;

            for (int i = 0; i < worlds; i++)
            {
                WorldServer ws = new WorldServer(port, channels);
                port += channels;

                m_worlds.Add(ws);
            }
        }

        public void Run()
        {
            m_login.Run();

            foreach (WorldServer ws in m_worlds)
                ws.Run();
        }
        public void Shutdown()
        {
            m_login.Shutdown();

            foreach (WorldServer ws in m_worlds)
                ws.Shutdown();

            m_worlds.Clear();
        }
    }
}
