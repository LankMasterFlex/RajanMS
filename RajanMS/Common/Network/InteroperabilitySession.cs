using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public sealed class InteroperabilitySession
    {
        private readonly Socket m_socket;

        private bool m_header;
        private int m_offset;
        private byte[] m_buffer;

        private object m_locker;
        private bool m_disposed;

        public Action<byte[]> OnPacket;
        public Action OnDisconnected;

        public bool Alive
        {
            get
            {
                return !m_disposed;
            }
        }

        public InteroperabilitySession(Socket socket)
        {
            m_socket = socket;
            m_socket.NoDelay = true;
            m_socket.ReceiveBufferSize = 0xFFFF;
            m_socket.SendBufferSize = 0xFFFF;

            m_locker = new object();
            m_disposed = false;
        }

        public void StartClient()
        {
            WaitForData(true, 4);
        }

        private void WaitForData(bool header, int size)
        {
            if (m_disposed) { return; }

            m_header = header;
            m_offset = 0;
            m_buffer = new byte[size];

            BeginRead(m_buffer.Length);
        }

        private void BeginRead(int size)
        {
            SocketError outError = SocketError.Success;

            m_socket.BeginReceive(m_buffer, m_offset, size, SocketFlags.None, out outError, ReadCallback, null);

            if (outError != SocketError.Success)
            {
                Close();
            }
        }

        private void ReadCallback(IAsyncResult iar)
        {
            if (m_disposed) { return; }

            SocketError error;
            int received = m_socket.EndReceive(iar, out error);

            if (received == 0 || error != SocketError.Success)
            {
                Close();
                return;
            }

            m_offset += received;

            if (m_offset == m_buffer.Length)
            {
                HandleStream();
            }
            else
            {
                BeginRead(m_buffer.Length - m_offset);
            }
        }

        private void HandleStream()
        {
            if (m_header)
            {
                int size = BitConverter.ToInt32(m_buffer,0);
                WaitForData(false, size);
            }
            else
            {
                if (OnPacket != null)
                    OnPacket(m_buffer);

                WaitForData(true, 4);
            }
        }

        public void WritePacket(byte[] packet)
        {
            if (m_disposed) { return; }

            lock (m_locker)
            {
                byte[] final = new byte[packet.Length + 4];
                byte[] header = BitConverter.GetBytes(packet.Length);

                Buffer.BlockCopy(header,0,final,0,4);
                Buffer.BlockCopy(packet,0,final,4,packet.Length);

                WriteRawPacket(final); //send the giant crypted packet
            }
        }

        private void WriteRawPacket(byte[] packet)
        {
            if (m_disposed) { return; }

            int offset = 0;

            while (offset < packet.Length)
            {
                SocketError outError = SocketError.Success;
                int sent = m_socket.Send(packet, offset, packet.Length - offset, SocketFlags.None, out outError);

                if (sent == 0 || outError != SocketError.Success)
                {
                    Close();
                    return;
                }

                offset += sent;
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;

                m_socket.Shutdown(SocketShutdown.Both);
                m_socket.Close();

                m_offset = 0;
                m_buffer = null;

                if (OnDisconnected != null)
                    OnDisconnected();
            }
        }
    }
}
