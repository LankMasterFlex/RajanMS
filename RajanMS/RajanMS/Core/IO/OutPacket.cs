using System;
using System.IO;

namespace RajanMS.IO
{
    public class OutPacket : IDisposable
    {
        public const int DefaultBufferSize = 32;

        private MemoryStream m_stream;
        private bool m_disposed;

        public int Position
        {
            get
            {
                return (int)m_stream.Position;
            }
            set
            {
                if (value <= 0)
                    throw new IndexOutOfRangeException();

                m_stream.Position = value;
            }
        }
        public bool Disposed
        {
            get
            {
                return m_disposed;
            }
        }

        public OutPacket()
        {
            m_stream = new MemoryStream(DefaultBufferSize);
            m_disposed = false;
        }
        public OutPacket(byte opcode, int size = DefaultBufferSize)
        {
            m_stream = new MemoryStream(size);
            m_disposed = false;
            WriteByte(opcode);
        }

        //From LittleEndianByteConverter by Shoftee
        private void Append(long value, int byteCount)
        {
            for (int i = 0; i < byteCount; i++)
            {
                m_stream.WriteByte((byte)value);
                value >>= 8;
            }
        }

        public void WriteBool(bool value)
        {
            ThrowIfDisposed();
            WriteByte(value ? (byte)1 : (byte)0);
        }
        public void WriteByte(byte value = 0)
        {
            ThrowIfDisposed();
            m_stream.WriteByte(value);
        }
        public void WriteBytes(params byte[] value)
        {
            ThrowIfDisposed();
            m_stream.Write(value, 0, value.Length);
        }
        public void WriteShort(short value = 0)
        {
            ThrowIfDisposed();
            Append(value, 2);
        }
        public void WriteInt(int value = 0)
        {
            ThrowIfDisposed();
            Append(value, 4);
        }
        public void WriteLong(long value = 0)
        {
            ThrowIfDisposed();
            Append(value, 8);
        }
        public void WriteString(string value)
        {
            ThrowIfDisposed();

            foreach (char c in value)
                WriteByte((byte)c);
        }
        public void WritePaddedString(string value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < value.Length)
                {
                    WriteByte((byte)value[i]);
                }
                else
                {
                    WriteByte();
                }
            }
        }
        public void WriteMapleString(string value)
        {
            ThrowIfDisposed();

            WriteShort((short)value.Length);
            WriteString(value);
        }
        public void WriteMapleString(string format,params object[] args)
        {
            WriteMapleString(string.Format(format, args));
        }

        public void WriteZero(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            for (int i = 0; i < count; i++)
                WriteByte();
        }

        public byte[] ToArray()
        {
            ThrowIfDisposed();
            return m_stream.ToArray();
        }

        private void ThrowIfDisposed()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            m_disposed = true;

            if (m_stream != null)
                m_stream.Dispose();

            m_stream = null;
        }
    }
}
