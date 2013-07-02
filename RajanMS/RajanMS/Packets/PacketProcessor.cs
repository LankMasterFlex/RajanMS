using System;
using System.Collections.Generic;
using Common.IO;

namespace RajanMS.Packets
{
    public delegate void PacketHandler(MapleClient c,InPacket p);

    public sealed class PacketProcessor
    {
        public string Label { get; private set; }

        private Dictionary<short, PacketHandler> m_handlers;

        public int Count
        {
            get
            {
                return m_handlers.Count;
            }
        }

        public PacketProcessor(string label)
        {
            Label = label;
            m_handlers = new Dictionary<short, PacketHandler>();
        }

        public void AppendHandler(short opcode, PacketHandler handler)
        {
            m_handlers.Add(opcode, handler);
        }

        public PacketHandler this[short opcode]
        {
            get
            {
                PacketHandler handler;
                m_handlers.TryGetValue(opcode, out handler);
                return handler;
            }
        }
    }
}
