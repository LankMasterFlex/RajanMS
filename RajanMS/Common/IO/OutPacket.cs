using System.IO;
using System.Text;

namespace Common.IO
{
    public sealed class OutPacket : AbstractPacket
    {
        private BinaryWriter m_binaryWriter;

        public OutPacket(short opcode, short size = 32)
        {
            m_memoryStream = new MemoryStream(size); //Default Size
            m_binaryWriter = new BinaryWriter(m_memoryStream, Encoding.ASCII);
            WriteShort(opcode);
        }

        public void WriteBytes(byte[] value)
        {
            m_binaryWriter.Write(value);
        }
        public void WriteByte(byte value = 0)
        {
            m_binaryWriter.Write(value);
        }
        public void WriteBool(bool value = false)
        {
            m_binaryWriter.Write(value);
        }
        public void WriteShort(short value = 0)
        {
            m_binaryWriter.Write(value);
        }
        public void WriteInt(int value = 0)
        {
            m_binaryWriter.Write(value);
        }
        public void WriteLong(long value = 0)
        {
            m_binaryWriter.Write(value);
        }
        public void WriteString(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                m_binaryWriter.Write(value[i]);
            }
        }
        public void WriteMapleString(string value)
        {
            WriteShort((short)value.Length);
            WriteString(value);
        }
        public void WriteZero(int count)
        {
            for (int i = 0; i < count; i++)
                WriteByte();
        }

        protected override void CustomDispose()
        {
            m_binaryWriter.Dispose();
        }
    }
}
