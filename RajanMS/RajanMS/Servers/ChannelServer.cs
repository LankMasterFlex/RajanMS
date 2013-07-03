using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Network;

namespace RajanMS.Servers
{
    sealed class ChannelServer
    {
        public byte Id { get; private set; }
        public byte WorldId { get; private set; }
        
        private Acceptor m_acceptor;
        private List<MapleClient> m_clients;

        public ChannelServer(byte id,byte worldId,short port)
        {
            Id = id;
            WorldId = worldId;

            m_acceptor = new Acceptor(port);
            m_acceptor.OnClientAccepted = OnClientAccepted;

            m_clients = new List<MapleClient>();
        }

        public int Load
        {
            get
            {
                return m_clients.Count;
            }
        }

        private void OnClientAccepted(Socket client)
        {
        }

        public void Run()
        {
            m_acceptor.Start();
            MainForm.Instance.Log("[{0}] ChannelServer listening on port {1}",Constants.WorldNames[WorldId], m_acceptor.Port);
        }

        public void Shutdown()
        {
            m_acceptor.Stop();

            for (int i = 0; i < m_clients.Count; i++)
                m_clients[i].Close();

            m_clients.Clear();
        }
    }
}
