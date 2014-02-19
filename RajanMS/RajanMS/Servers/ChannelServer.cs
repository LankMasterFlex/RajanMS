using RajanMS.Packets;
using RajanMS.Packets.Handlers;
using RajanMS.Tools;
using System.Net.Sockets;

namespace RajanMS.Servers
{
    public sealed class ChannelServer : ServerBase
    {
        public byte Id { get; private set; }

        public int Load
        {
            get
            {
                return m_clients.Count;
            }
        }

        public ChannelServer(byte id, short port) : base(port)
        {
            Id = id;
        }

        protected override void SpawnHandlers()
        {
            m_processor = new PacketProcessor("Channel");

            m_processor.AppendHandler(RecvOps.MigrateIn, PacketHandlers.OnMigrateIn);
        }
        protected override void OnClientAccepted(Socket client)
        {
            MapleClient mc = new MapleClient(client, this, m_processor)
            {
                Channel = Id
            };

            mc.SendRaw(PacketCreator.Handshake());
            MainForm.Instance.Log("[Channel] Accepted client {0}", mc.Label);
        }

        public override void Run()
        {
            m_acceptor.Start();
            MainForm.Instance.Log("[{0}] ChannelServer listening on port {1}", Constants.WorldName, m_acceptor.Port);
        }
        public override void Shutdown()
        {
            m_acceptor.Stop();
            m_clients.ToArray().ForAll((mc) => mc.Close());
        }
    }
}
