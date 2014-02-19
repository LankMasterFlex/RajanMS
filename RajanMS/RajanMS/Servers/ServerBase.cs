using RajanMS.Network;
using RajanMS.Packets;
using System.Collections.Generic;
using System.Net.Sockets;

namespace RajanMS.Servers
{
    public abstract class ServerBase
    {
        protected Acceptor m_acceptor;
        protected List<MapleClient> m_clients;
        protected PacketProcessor m_processor;

        public short Port
        {
            get
            {
                return m_acceptor.Port;
            }
        }

        public ServerBase(short port)
        {
            m_acceptor = new Acceptor(port);
            m_acceptor.OnClientAccepted = OnClientAccepted;

            m_clients = new List<MapleClient>();

            SpawnHandlers();
        }

        protected abstract void SpawnHandlers();
        protected abstract void OnClientAccepted(Socket client);

        public void AddClient(MapleClient client)
        {
            m_clients.Add(client);
        }
        public void RemoveClient(MapleClient client)
        {
            m_clients.Remove(client);
        }

        public abstract void Run();
        public abstract void Shutdown();
    }
}
