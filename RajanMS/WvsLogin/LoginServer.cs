using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.Collections;
using Common;
using Common.Network;
using Common.IO;
using Common.Operations.GameServer;

namespace WvsLogin
{
    class LoginServer
    {
        private static LoginServer theOnly = new LoginServer();
        public static LoginServer Instance
        {
            get
            {
                return theOnly;
            }
        }

        private SafeList<Client> m_clients;
        private Acceptor m_acceptor;
        private InteroperabilitySession m_session;

        private bool m_closing;

        private LoginServer()
        {
            m_clients = new SafeList<Client>();
            m_closing = false;
        }

        public void Connect(short port)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(IPAddress.Loopback, port);

            Logger.Write(LogLevel.Connection, "Connected to WvsCenter at {0}", client.RemoteEndPoint);

            m_session = new InteroperabilitySession(client);
            m_session.OnPacket = HandlePacket;
            m_session.OnDisconnected = HandleDisconnection;

            m_session.StartClient();

            using (OutPacket p = new OutPacket((short)OpCodes.Identify))
            {
                p.WriteByte((byte)ServerType.WvsLogin);
                m_session.WritePacket(p.ToArray());
            }
        }
        private void HandlePacket(byte[] packet)
        {
            Logger.Write(LogLevel.DataLoad, BitConverter.ToString(packet));
        }
        private void HandleDisconnection()
        {
            Logger.Write(LogLevel.Warning, "DISCONNECTED FROM THE MASTER SERVER");
        }

        public void Listen(short port)
        {
            m_acceptor = new Acceptor(IPAddress.Any, port);
            m_acceptor.OnClientAccepted = OnClientAccepted;
            m_acceptor.Start();

            Logger.Write(LogLevel.Info, "Listening on port {0}", port);
        }

        public void Stop()
        {
            m_closing = true;
            m_acceptor.Dispose();
            m_clients.ForAll((c) => c.Close());
        }

        private void OnClientAccepted(Socket socket)
        {
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
    }
}
