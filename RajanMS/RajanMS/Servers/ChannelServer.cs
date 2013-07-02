using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Network;

namespace RajanMS.Servers
{
    class ChannelServer
    {
        private Acceptor m_acceptor;
        private List<MapleClient> m_clients;

        public ChannelServer(short port)
        {
            m_acceptor = new Acceptor(port);
            m_acceptor.OnClientAccepted = OnClientAccepted;

            m_clients = new List<MapleClient>();
        }

        private void OnClientAccepted(Socket client)
        {
        }

        public void Run()
        {
            m_acceptor.Start();
            MainForm.Instance.Log("ChannelServer listening on port {0}", m_acceptor.Port);
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
