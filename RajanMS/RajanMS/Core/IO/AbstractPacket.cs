using System;
using System.IO;

namespace RajanMS.IO
{
    public abstract class AbstractPacket : IDisposable
    {
        protected MemoryStream m_memoryStream;

        public long Position
        {
            get
            {
                return m_memoryStream.Position;
            }
            set
            {
                m_memoryStream.Position = value;
            }
        }
        public long Length
        {
            get
            {
                return m_memoryStream.Length;
            }
        }

        public void Skip(int count)
        {
            Position += count;
        }

        public byte[] ToArray()
        {
            return m_memoryStream.ToArray();
        }
        public override string ToString()
        {
            return BitConverter.ToString(ToArray());
        }

        protected virtual void CustomDispose()
        {
        }
        public void Dispose()
        {
            CustomDispose();
            m_memoryStream.Dispose();
        }
    }
}
