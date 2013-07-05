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

        private List<MigrateRequest> m_migrateReqs;

        public WorldServer(byte id,short port,int channels)
        {
            Id = id;
            Channels = new ChannelServer[channels];

            for (int i = 0; i < channels; i++)
            {
                Channels[i] = new ChannelServer((byte)i,id, port);
                port++;
            }

            m_migrateReqs = new List<MigrateRequest>();
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

        public void AddMigrationRequest(int charId,long sessionId)
        {
            lock (m_migrateReqs)
            {
                m_migrateReqs.Add(new MigrateRequest(charId, sessionId));
            }
        }

        public bool EligableMigration(int charId,long sessionId)
        {
            lock (m_migrateReqs)
            {
                //iterate backwards so when removing item, indexes dont change
                for (int i = m_migrateReqs.Count; i-- > 0; )
                {
                    MigrateRequest itr = m_migrateReqs[i];

                    if ((DateTime.Now - itr.Expiry).Seconds > 30) //30 second migration time
                    {
                        m_migrateReqs.Remove(itr);
                        continue; //skip itr
                    }

                    if (itr.CharacterId == charId && itr.SessionId == sessionId)
                    {
                        m_migrateReqs.Remove(itr);
                        return true;
                    }

                }
            }

            return false;
        }

        public int[] GetChannelLoads()
        {
            var final = new int[Channels.Length];

            for (int i = 0; i < final.Length; i++)
            {
                final[i] = Channels[i].Load;
            }

            return final;
        }
    }
}
