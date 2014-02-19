using System;
using System.Net.Sockets;
using RajanMS.Core.IO;
using RajanMS.Tools;
using RajanMS.IO;

namespace RajanMS.Network
{
    public abstract class Session
    {
        public const byte HeaderSize = 4;

        private readonly Socket m_socket;
        private readonly MapleCipher m_sendCipher;
        private readonly MapleCipher m_recvCipher;

        private int m_offset;

        private byte[] m_buffer;
        private byte[] m_packet;

        private object m_locker;
        private bool m_connected;

        protected abstract void OnPacket(byte[] packet);
        protected abstract void OnDisconnected();

        public bool Connected
        {
            get
            {
                return m_connected;
            }
        }

        public Session(Socket socket)
        {
            m_socket = socket;

            m_buffer = BufferPool.Get();
            m_packet = BufferPool.Get();

            m_locker = new object();
            m_connected = true;

            m_sendCipher = new MapleCipher(Constants.MajorVersion, Constants.SIV, MapleCipher.TransformDirection.Encrypt);
            m_recvCipher = new MapleCipher(Constants.MajorVersion, Constants.RIV, MapleCipher.TransformDirection.Decrypt);

            BeginRead();
        }

        private void BeginRead()
        {
            if (!m_connected) { return; }

            SocketError outError = SocketError.Success;

            m_socket.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, out outError, ReadCallback, null);

            if (outError != SocketError.Success)
            {
                Close();
            }
        }

        private void ReadCallback(IAsyncResult iar)
        {
            if (!m_connected) { return; }

            SocketError error;
            int received = m_socket.EndReceive(iar, out error);

            if (received == 0 || error != SocketError.Success)
            {
                Close();
                return;
            }

            Append(received);
            ManipulateBuffer();
            BeginRead();
        }
        private void Append(int length)
        {
            if (m_packet.Length - m_offset < length)
            {
                int newSize = m_packet.Length * 2;

                while (newSize < m_offset + length)
                    newSize *= 2;

                Array.Resize<byte>(ref m_packet, newSize);
            }

            Buffer.BlockCopy(m_buffer, 0, m_packet, m_offset, length);

            m_offset += length;
        }
        private void ManipulateBuffer()
        {
            while (m_offset > HeaderSize) //header room
            {
                int packetSize = MapleCipher.GetPacketLength(m_packet);

                if (m_offset < packetSize + HeaderSize) //header + packet room
                    break;

                byte[] packetBuffer = new byte[packetSize];
                Buffer.BlockCopy(m_packet, 4, packetBuffer, 0, packetSize); //copy packet

                m_recvCipher.Transform(packetBuffer); //decrypt

                m_offset -= packetSize + HeaderSize; //fix len

                if (m_offset > 0) //move reamining bytes
                    Buffer.BlockCopy(m_packet, packetSize + HeaderSize, m_packet, 0, m_offset);

                OnPacket(packetBuffer);
            }
        }

        public void Send(OutPacket packet)
        {
            Send(packet.ToArray());
        }
        public void Send(params byte[][] packets)
        {
            if (!m_connected) { return; }

            lock (m_locker)
            {
                int length = 0;
                int offset = 0;

                foreach (byte[] buffer in packets)
                {
                    length += 4;                //header length
                    length += buffer.Length;    //packet length
                }

                byte[] finalPacket = new byte[length];

                foreach (byte[] buffer in packets)
                {
                    m_sendCipher.GetHeaderToClient(finalPacket, offset, buffer.Length);

                    offset += 4; //header space

                    m_sendCipher.Transform(buffer);
                    Buffer.BlockCopy(buffer, 0, finalPacket, offset, buffer.Length);

                    offset += buffer.Length; //packet space
                }

                SendRaw(finalPacket); //send the giant crypted packet
            }
        }
        public void SendRaw(byte[] packet)
        {
            if (!m_connected) { return; }

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
            if (m_connected)
            {
                m_connected = false;

                m_socket.Shutdown(SocketShutdown.Both);
                m_socket.Close();

                m_offset = 0;

                BufferPool.Put(m_buffer);

                if (m_packet.Length == BufferPool.BufferSize)
                    BufferPool.Put(m_packet);
                else
                    m_packet = null;

                OnDisconnected();
            }
        }
    }
}
