using System.Net.Sockets;
using RajanMS.Packets;
using RajanMS.Packets.Handlers;

namespace RajanMS.Servers
{
    public sealed class ChannelServer : ServerBase
    {
        public byte Id { get; private set; }
        public byte WorldId { get; private set; }
        public short Port
        {
            get
            {
                return m_acceptor.Port;
            }
        }
        public int Load
        {
            get
            {
                return m_clients.Count;
            }
        }

        public ChannelServer(byte id, byte worldId, short port) : base(port)
        {
            Id = id;
            WorldId = worldId;
        }

        public bool Contains(MapleClient c)
        {
            return m_clients.Contains(c);
        }

        protected override void SpawnHandlers()
        {
            m_processor = new PacketProcessor("Channel");

            m_processor.AppendHandler(RecvOps.Migrate, InterserverHandler.HandleMigrate);
        }

        protected override void OnClientAccepted(Socket client)
        {
            MapleClient mc = new MapleClient(client, this, m_processor)
            {
                World = WorldId,
                Channel = Id
            };


            mc.SendRaw(PacketCreator.Handshake());
            MainForm.Instance.Log("[Channel] Accepted client {0}", mc.Label);
        }
        public override void Run()
        {
            m_acceptor.Start();
            MainForm.Instance.Log("[{0}] ChannelServer listening on port {1}", Constants.WorldNames[WorldId], m_acceptor.Port);
        }
        public override void Shutdown()
        {
            m_acceptor.Stop();
            m_clients.ToArray().ForAll((mc) => mc.Close());
        }
    }
}
