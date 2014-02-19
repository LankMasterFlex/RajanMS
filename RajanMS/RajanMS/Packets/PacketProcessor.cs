using RajanMS.IO;
using System;

namespace RajanMS.Packets
{
    public delegate void PacketHandler(MapleClient c,InPacket p);

    public sealed class PacketProcessor
    {
        public string Label { get; private set; }

        private PacketHandler[] m_handlers;

        public PacketProcessor(string label)
        {
            Label = label;
            m_handlers = new PacketHandler[256];
        }

        public void AppendHandler(byte opcode, PacketHandler handler)
        {
            if (m_handlers[opcode] != null)
                throw new InvalidOperationException("Handler already set");

            m_handlers[opcode] = handler;
        }

        public PacketHandler this[byte opcode]
        {
            get
            {
                return m_handlers[opcode];
            }
        }
    }
}
