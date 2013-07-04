using System.IO;
using System.Text;

namespace RajanMS.IO
{
    public sealed class InPacket : AbstractPacket
    {
        private BinaryReader m_binaryReader;

        public InPacket(byte[] packet)
        {
            m_memoryStream = new MemoryStream(packet, false);
            m_binaryReader = new BinaryReader(m_memoryStream, Encoding.ASCII);
        }
        public InPacket(byte[] packet, int index, int count)
        {
            m_memoryStream = new MemoryStream(packet, index, count, false, false);
            m_binaryReader = new BinaryReader(m_memoryStream, Encoding.ASCII);
        }

        public byte[] ReadBytes(int count)
        {
            return m_binaryReader.ReadBytes(count);
        }
        public byte ReadByte()
        {
            return m_binaryReader.ReadByte();
        }
        public bool ReadBool()
        {
            return m_binaryReader.ReadBoolean();
        }
        public short ReadShort()
        {
            return m_binaryReader.ReadInt16();
        }
        public int ReadInt()
        {
            return m_binaryReader.ReadInt32();
        }
        public long ReadLong()
        {
            return m_binaryReader.ReadInt64();
        }
        public string ReadString(int length)
        {
            return new string(m_binaryReader.ReadChars(length));
        }
        public string ReadMapleString()
        {
            return ReadString(ReadShort());
        }

        protected override void CustomDispose()
        {
            m_binaryReader.Dispose();
        }
    }
}
