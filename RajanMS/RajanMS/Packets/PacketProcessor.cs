using System;
using System.Collections.Generic;
using RajanMS.IO;

namespace RajanMS.Packets
{
    public delegate void PacketHandler(MapleClient c,InPacket p);

    public sealed class PacketProcessor
    {
        public string Label { get; private set; }

        private PacketHandler[] m_handlers;
        private int m_count;

        public int Count
        {
            get
            {
                return m_count;
            }
        }

        public PacketProcessor(string label)
        {
            Label = label;
            m_handlers = new PacketHandler[0xFFFF + 1]; //YOLO
        }

        public void AppendHandler(short opcode, PacketHandler handler)
        {
            m_handlers[opcode] = handler;
            m_count++;
        }

        public PacketHandler this[short opcode]
        {
            get
            {
                return m_handlers[opcode];
            }
        }
    }
}
