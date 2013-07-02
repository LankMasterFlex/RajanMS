using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajanMS.Servers
{
    class WorldServer
    {
        public byte Id { get; private set; }
        public ChannelServer[] Channels { get; private set; }

        public WorldServer(byte id,short port,int channels)
        {
            Id = id;
            Channels = new ChannelServer[channels];

            for (int i = 0; i < channels; i++)
            {
                Channels[i] = new ChannelServer(id, port);
                port++;
            }
        }

        public void Run()
        {
            foreach (ChannelServer cs in Channels)
                cs.Run();
        }

        public void Shutdown()
        {
            foreach (ChannelServer cs in Channels)
                cs.Shutdown();
        }

        public int[] GetChannelLoads()
        {
            int[] final = new int[Channels.Length];

            for (int i = 0; i < final.Length; i++)
            {
                final[i] = Channels[i].Load;
            }

            return final;
        }
    }
}
