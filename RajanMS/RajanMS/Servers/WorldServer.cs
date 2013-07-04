using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RajanMS.Game;
using RajanMS.Core.Collections;

namespace RajanMS.Servers
{
    sealed class WorldServer
    {
        public byte Id { get; private set; }
        public ChannelServer[] Channels { get; private set; }

        private BlockingList<MigrateRequest> m_migrateReqs;

        public WorldServer(byte id,short port,int channels)
        {
            Id = id;
            Channels = new ChannelServer[channels];

            for (int i = 0; i < channels; i++)
            {
                Channels[i] = new ChannelServer((byte)i,id, port);
                port++;
            }

            m_migrateReqs = new BlockingList<MigrateRequest>();
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

        public int CurrentLoad
        {
            get
            {
                int final = 0;

                foreach (ChannelServer cs in Channels)
                    final += cs.Load;

                return final;
            }
        }

        public void AddMigrationRequest(MigrateRequest mr)
        {
            m_migrateReqs.Add(mr);
        }

        public bool EligableMigration(MigrateRequest mr)
        {
            bool found = false;

            m_migrateReqs.ForEach((itr) =>
                {
                    if ((DateTime.Now - mr.Expiry).Minutes > 1)
                    {
                        m_migrateReqs.EnqueRemove(itr);
                        return;
                    }

                    if (found) //still iterate times but if found no need to check legitimacy
                        return;

                    if (itr.CharacterId == mr.CharacterId &&
                        itr.SessionId == mr.SessionId)
                    {
                        m_migrateReqs.EnqueRemove(itr);
                        found = true;
                        return;
                    }

                });

            m_migrateReqs.DispatchRemoval();

            return found;
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
