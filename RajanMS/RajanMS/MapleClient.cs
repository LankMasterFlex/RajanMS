using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common.IO;
using Common.Network;
using RajanMS.Packets;

namespace RajanMS
{
    public sealed class MapleClient : MapleSession
    {
        private PacketProcessor m_processor;
        private Func<MapleClient,bool> m_deathAction; //dont want to pass whole parent server obj

        public byte World { get; set; }
        public byte Channel { get; set; }

        public int AccountId { get; set; }
        public byte Gender { get; set; }
        public string AccountName { get; set; }
        public bool IsAdmin { get; set; }
        public bool LoggedIn { get; set; }

        public MapleClient(Socket client, PacketProcessor processor, Func<MapleClient, bool> death)
            : base(client)
        {
            m_processor = processor;
            m_deathAction = death;
        }

        protected override void OnPacket(byte[] packet)
        {
            using (InPacket p = new InPacket(packet))
            {
                short opcode = p.ReadShort();

                PacketHandler handler = m_processor[opcode];

                if (handler != null)
                {
                    handler(this, p);
                }
                else
                {
                    MainForm.Instance.Log("[{0}] Unhandled packet from {1}{2}{3}", m_processor.Label, Label, Environment.NewLine, p.ToString());
                }
            }
        }

        protected override void OnDisconnected()
        {
            MainForm.Instance.Log("[{0}] Client {1} disconnected", m_processor.Label, Label);
            m_deathAction(this); //remove from list!
        }
    }
}
