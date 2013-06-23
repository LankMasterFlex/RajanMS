using System.Net.Sockets;
using Common;
using Common.IO;
using Common.Network;
using Common.Operations.GameServer;

namespace WvsCenter
{
    internal sealed class Client
    {
        public int SpecialId { get; private set; }

        public ServerType ServerType { get; private set; }
        public int Dataload { get; private set; }
        
        private InteroperabilitySession m_session;

        public Client(Socket socket)
        {
            SpecialId = ++Program.SpecialId;

            ServerType = ServerType.None;
            Dataload = 0;

            m_session = new InteroperabilitySession(socket);
            m_session.OnPacket = HandlePacket;
            m_session.OnDisconnected = HandleDisconnection;

            m_session.StartClient();

            MasterServer.Instance.Add(this);
        }

        private void HandlePacket(byte[] buffer)
        {
            InPacket ip = new InPacket(buffer);
            OpCodes opcode = (OpCodes)ip.ReadShort();

            switch (opcode)
            {
                case OpCodes.Identify:
                    ServerType = (ServerType)ip.ReadByte();
                    Logger.Write(LogLevel.Server, "Indentified {0} as {1}", SpecialId, ServerType);
                    MasterServer.Instance.DispatchTypes(this);
                    break;
                case OpCodes.Dataload:
                    Dataload = ip.ReadInt();
                    MasterServer.Instance.DispatchLoads(this);
                    break;
            }

            ip.Dispose();
        }

        private void HandleDisconnection()
        {
            MasterServer.Instance.Remove(this);
            Logger.Write(LogLevel.Connection, "{0} server disconnected",ServerType);
        }

        public void WritePacket(byte[] packet)
        {
            if (m_session.Alive)
                m_session.WritePacket(packet);
        }
        
        public void Close()
        {
            m_session.Close();
        }
    }
}
