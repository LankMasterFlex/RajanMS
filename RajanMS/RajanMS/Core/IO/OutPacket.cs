using System.Drawing;
using System.IO;
using System.Text;

namespace RajanMS.IO
{
    public sealed class OutPacket : AbstractPacket
    {
        private BinaryWriter m_binaryWriter;

        public OutPacket(short opcode, int size = 64)
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
        public void WritePaddedString(string value,int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (i < value.Length)
                {
                    m_binaryWriter.Write(value[i]);
                }
                else
                {
                    WriteByte();
                }
            }
        }
        public void WriteMapleString(string fmt,params object[] args)
        {
            string final = string.Format(fmt, args);

            WriteShort((short)final.Length);
            WriteString(final);
        }
        public void WriteZero(int count)
        {
            for (int i = 0; i < count; i++)
                WriteByte();
        }
        public void WritePosition(Point value)
        {
            WriteShort((short)value.X);
            WriteShort((short)value.Y);
        }

        protected override void CustomDispose()
        {
            m_binaryWriter.Dispose();
        }
    }
}
