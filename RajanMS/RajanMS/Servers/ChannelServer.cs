using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RajanMS;
using RajanMS.Network;
using RajanMS.Packets;
using RajanMS.Packets.Handlers;

namespace RajanMS.Servers
{
    sealed class ChannelServer
    {
        public byte Id { get; private set; }
        public byte WorldId { get; private set; }
        public short Port { get; private set; }

        private PacketProcessor m_processor;
        private Acceptor m_acceptor;
        private List<MapleClient> m_clients;

        public ChannelServer(byte id,byte worldId,short port)
        {
            Id = id;
            WorldId = worldId;
            Port = port;

            m_acceptor = new Acceptor(port);
            m_acceptor.OnClientAccepted = OnClientAccepted;

            m_clients = new List<MapleClient>();

            SpawnHandlers();
        }

        private void SpawnHandlers()
        {
            m_processor = new PacketProcessor("Channel");

            m_processor.AppendHandler(RecvOps.Migrate, InterserverHandler.HandleMigrate);
        }

        public bool Contains(MapleClient c)
        {
            return m_clients.Contains(c);
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
            MapleClient mc = new MapleClient(client, m_processor, m_clients.Remove)
            {
                World = WorldId,
                Channel = Id
            };
            
            m_clients.Add(mc);

            mc.WriteRawPacket(PacketCreator.Handshake());
            MainForm.Instance.Log("[Channel] Accepted client {0}", mc.Label);
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
