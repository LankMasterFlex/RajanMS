using System;
using System.Runtime.InteropServices;

namespace RajanMS.IO
{
    public class InPacket
    {
        private byte[] m_buffer;
        private int m_index;

        public int Position
        {
            get
            {
                return m_index;
            }
        }
        public int Available
        {
            get
            {
                return m_buffer.Length - m_index;
            }
        }

        public InPacket(byte[] packet)
        {
            m_buffer = packet;
            m_index = 0;
        }

        private void CheckLength(int length)
        {
            if (m_index + length > m_buffer.Length || length < 0)
                throw new IndexOutOfRangeException();
        }

        public bool ReadBool()
        {
            return m_buffer[m_index++] != 0;
        }
        public byte ReadByte()
        {
            return m_buffer[m_index++];
        }
        public byte[] ReadBytes(int count)
        {
            CheckLength(count);
            var temp = new byte[count];
            Buffer.BlockCopy(m_buffer, m_index, temp, 0, count);
            m_index += count;
            return temp;
        }
        public unsafe short ReadShort()
        {
            CheckLength(2);

            short value;

            fixed (byte* ptr = m_buffer)
            {
                value = *(short*)(ptr + m_index);
            }

            m_index += 2;

            return value;
        }
        public unsafe int ReadInt()
        {
            CheckLength(4);

            int value;

            fixed (byte* ptr = m_buffer)
            {
                value = *(int*)(ptr + m_index);
            }

            m_index += 4;

            return value;
        }
        public unsafe long ReadLong()
        {
            CheckLength(8);

            long value;

            fixed (byte* ptr = m_buffer)
            {
                value = *(long*)(ptr + m_index);
            }

            m_index += 8;

            return value;
        }

        public string ReadString(int count)
        {
            CheckLength(count);

            char[] final = new char[count];

            for (int i = 0; i < count; i++)
            {
                final[i] = (char)ReadByte();
            }

            return new string(final);
        }
        public string ReadMapleString()
        {
            short count = ReadShort();
            return ReadString(count);
        }

        public void Skip(int count)
        {
            CheckLength(count);
            m_index += count;
        }

        public byte[] ToArray()
        {
            var final = new byte[m_buffer.Length];
            Buffer.BlockCopy(m_buffer, 0, final, 0, m_buffer.Length);
            return final;
        }
    }
}
