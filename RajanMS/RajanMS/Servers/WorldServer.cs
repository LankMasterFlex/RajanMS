using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajanMS.Servers
{
    class WorldServer
    {
        private List<ChannelServer> m_channels;

        public WorldServer(short port,int channels)
        {
            m_channels = new List<ChannelServer>();

            for (int i = 0; i < channels; i++)
            {
                ChannelServer cs = new ChannelServer(port);
                m_channels.Add(cs);
                port++;
            }
        }

        public void Run()
        {
            for (int i = 0; i < m_channels.Count; i++)
            {
                m_channels[i].Run();
            }
        }

        public void Shutdown()
        {
            foreach (ChannelServer cs in m_channels)
                cs.Shutdown();

            m_channels.Clear();
        }

    }
}
