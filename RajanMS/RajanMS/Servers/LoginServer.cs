using System.Net.Sockets;
using RajanMS.Packets;
using RajanMS.Packets.Handlers;

namespace RajanMS.Servers
{
    public sealed class LoginServer : ServerBase
    {
        public LoginServer(short port) : base(port) { }

        protected override void SpawnHandlers()
        {
            m_processor = new PacketProcessor("Login");

            m_processor.AppendHandler(RecvOps.Validate, LoginHandler.HandleValidate);
            m_processor.AppendHandler(RecvOps.LoginPassword, LoginHandler.HandleLoginPassword);
            m_processor.AppendHandler(RecvOps.WorldInfoRequest, LoginHandler.HandleWorldInfoRequest);
            m_processor.AppendHandler(RecvOps.WorldInfoRequest2, LoginHandler.HandleWorldInfoRequest);
            m_processor.AppendHandler(RecvOps.CheckUserLimit, LoginHandler.HandleCheckUserLimit);
            m_processor.AppendHandler(RecvOps.WorldSelect, LoginHandler.HandleWorldSelect);
            m_processor.AppendHandler(RecvOps.CheckDuplicateName, LoginHandler.HandleCheckDuplicateName);
            m_processor.AppendHandler(RecvOps.CreateCharacter, LoginHandler.HandleCreateCharacter);
            m_processor.AppendHandler(RecvOps.DeleteCharacter, LoginHandler.HandleDeleteCharacter);
            m_processor.AppendHandler(RecvOps.SelectCharacter, LoginHandler.HandleSelectCharacter);
            m_processor.AppendHandler(RecvOps.ViewAllSelectCharacter, LoginHandler.HandleSelectCharacter);
            m_processor.AppendHandler(RecvOps.SelectCharacterSetPIC, LoginHandler.HandleSelectCharacterSetPIC);
            m_processor.AppendHandler(RecvOps.ViewAllCharacters, LoginHandler.HandleViewAllCharacters);
            m_processor.AppendHandler(RecvOps.ViewWorldInfo, LoginHandler.HandleViewWorldInfo);

            m_processor.AppendHandler(RecvOps.StartHackshield, GeneralHandler.HandleNothing);
            m_processor.AppendHandler(RecvOps.Pong, GeneralHandler.HandleNothing);
            m_processor.AppendHandler(RecvOps.Unk1, GeneralHandler.HandleNothing);
            m_processor.AppendHandler(RecvOps.ClientException, GeneralHandler.HandleClientException);
        }

        protected override void OnClientAccepted(Socket client)
        {
            MapleClient mc = new MapleClient(client,this, m_processor)
            {
                SessionId = Tools.Randomizer.Generate(),
            };

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
