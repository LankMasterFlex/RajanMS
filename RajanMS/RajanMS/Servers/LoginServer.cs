using RajanMS.Packets;
using RajanMS.Packets.Handlers;
using System.Net.Sockets;

namespace RajanMS.Servers
{
    public sealed class LoginServer : ServerBase
    {
        public LoginServer(short port) : base(port) { }

        protected override void SpawnHandlers()
        {
            m_processor = new PacketProcessor("Login");
            m_processor.AppendHandler(RecvOps.CheckPassword, PacketHandlers.OnCheckPassword);
            m_processor.AppendHandler(RecvOps.SelectWorld, PacketHandlers.OnSelectWorld);
            m_processor.AppendHandler(RecvOps.CheckUserLimit, PacketHandlers.OnCheckUserLimit);
            m_processor.AppendHandler(RecvOps.SelectCharacter, PacketHandlers.OnSelectCharacter);
            m_processor.AppendHandler(RecvOps.CheckDuplicatedID, PacketHandlers.OnCheckDuplicatedID);
            m_processor.AppendHandler(RecvOps.CreateNewCharacter, PacketHandlers.OnCreateNewCharacter);
            m_processor.AppendHandler(RecvOps.DeleteCharacter, PacketHandlers.OnDeleteCharacter);
        }
        protected override void OnClientAccepted(Socket client)
        {
            MapleClient mc = new MapleClient(client, this, m_processor);
            mc.SendRaw(PacketCreator.Handshake());
            MainForm.Instance.Log("[Login] Accepted client {0}", mc.Label);
        }

        public override void Run()
        {
            m_acceptor.Start();
            MainForm.Instance.Log("LoginServer listening on port {0}", m_acceptor.Port);
        }
        public override void Shutdown()
        {
            m_acceptor.Stop();
            m_clients.ToArray().ForAll((mc) => mc.Close());
        }
    }
}
