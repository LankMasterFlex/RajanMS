using System.Net;
using System.Net.Sockets;
using Common;
using Common.Collections;
using Common.IO;
using Common.Network;
using Common.Operations.GameServer;

namespace WvsCenter
{
    internal sealed class MasterServer
    {
        private static MasterServer theOnly = new MasterServer();
        public static MasterServer Instance
        {
            get
            {
                return theOnly;
            }
        }

        private SafeList<Client> m_clients;
        private Acceptor m_acceptor;

        private bool m_closing;

        private MasterServer()
        {
            m_clients = new SafeList<Client>();
            m_closing = false;
        }

        public void Start(short port)
        {
            m_acceptor = new Acceptor(IPAddress.Loopback, port);
            m_acceptor.OnClientAccepted = OnClientAccepted;
            m_acceptor.Start();

            Logger.Write(LogLevel.Info, "Listening on port {0}", port);
        }
        public void Stop()
        {
            m_closing = true;
            m_acceptor.Dispose();
            m_clients.ForAll((c) => c.Close()); //will clear it lol
        }

        private void OnClientAccepted(Socket socket)
        {
            new Client(socket);
            Logger.Write(LogLevel.Connection, "Accepted client from {0}", socket.RemoteEndPoint);
        }

        public void Add(Client client)
        {
            if(!m_closing)
                m_clients.Add(client);
        }
        public void Remove(Client client)
        {
            if (!m_closing)
                m_clients.Remove(client);
            
        }

        public void DispatchTypes(Client client)
        {
            m_clients.ForAll((c) =>
            {
                using (OutPacket op = new OutPacket((short)OpCodes.Identify))
                {
                    op.WriteInt(c.SpecialId);
                    op.WriteByte((byte)c.ServerType);
                    client.WritePacket(op.ToArray());
                }
            });
        }
        public void DispatchLoads(Client client)
        {
            m_clients.ForAll((c) =>
            {
                using (OutPacket op = new OutPacket((short)OpCodes.Dataload))
                {
                    op.WriteInt(c.SpecialId);
                    op.WriteInt(c.Dataload);
                    client.WritePacket(op.ToArray());
                }
            });
        }
    }
}
