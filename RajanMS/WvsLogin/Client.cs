using System.Net.Sockets;
using Common.Network;

namespace WvsLogin
{
    internal sealed class Client
    {
        private MapleSession m_session;

        public Client(Socket socket)
        {
            m_session = new MapleSession(socket);
            m_session.OnPacket = HandlePacket;
            m_session.OnDisconnected = HandleDisconnection;

            m_session.StartClient();

            LoginServer.Instance.Add(this);
        }

        private void HandlePacket(byte[] buffer)
        {

        }

        private void HandleDisconnection()
        {
            LoginServer.Instance.Remove(this);
        }

        public void WritePacket(params byte[][] packets)
        {
            if (m_session.Alive)
                m_session.WritePacket(packets);
        }

        public void Close()
        {
            m_session.Close(); //calls handle disconnection (lol)
        }
    }
}
