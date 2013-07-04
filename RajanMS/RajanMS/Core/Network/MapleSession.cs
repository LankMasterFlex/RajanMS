using System;
using System.Net;
using System.Net.Sockets;
using RajanMS.Cryptography;
using RajanMS.IO;

namespace RajanMS.Network
{
    public abstract class MapleSession
    {
        public string Label { get; private set; }

        private readonly Socket m_socket;

        private MapleCipher m_sendCipher;
        private MapleCipher m_recvCipher;

        private bool m_header;
        private int m_offset;
        private byte[] m_buffer;

        private object m_locker;
        private bool m_disposed;


        protected abstract void OnPacket(byte[] packet);
        protected abstract void OnDisconnected();

        public bool Connected
        {
            get
            {
                return !m_disposed;
            }
        }

        public MapleSession(Socket socket)
        {
            m_socket = socket;
            m_socket.NoDelay = true;
            m_socket.ReceiveBufferSize = 0xFFFF;
            m_socket.SendBufferSize = 0xFFFF;

            m_locker = new object();
            m_disposed = false;

            Label = ((IPEndPoint)(m_socket.RemoteEndPoint)).Address.ToString();

            m_sendCipher = new MapleCipher(Constants.MajorVersion, Constants.SIV, MapleCipher.TransformDirection.Encrypt);
            m_recvCipher = new MapleCipher(Constants.MajorVersion, Constants.RIV, MapleCipher.TransformDirection.Decrypt);

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

            if (received ==  0 ||error != SocketError.Success)
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
                int size = MapleCipher.GetPacketLength(m_buffer);

                if (size > m_socket.ReceiveBufferSize || !m_recvCipher.CheckServerPacket(m_buffer, 0))
                {
                    Close();
                    return;
                }

                WaitForData(false, size);
            }
            else
            {
                m_recvCipher.Transform(m_buffer);
                OnPacket(m_buffer);
                WaitForData(true, 4);
            }
        }

        public void WritePacket(params byte[][] packets)
        {
            if (m_disposed) { return; }

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

                WriteRawPacket(finalPacket); //send the giant crypted packet
            }
        }

        public void WriteRawPacket(byte[] packet)
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

                if (m_sendCipher != null)
                    m_sendCipher.Dispose();
                if (m_recvCipher != null)
                    m_recvCipher.Dispose();

                m_offset = 0;
                m_buffer = null;
                
                m_sendCipher = null;
                m_recvCipher = null;


                OnDisconnected();
            }
        }
    }
}
