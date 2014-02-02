using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using RajanMS.IO;
using RajanMS.Network;
using RajanMS.Game;
using RajanMS.Packets;
using RajanMS.Servers;
using RajanMS.Tools;

namespace RajanMS
{
    public sealed class MapleClient : Session
    {
        public string Label { get; private set; }

        private PacketProcessor m_processor;
        private ServerBase m_parentServer;

        public Character Character { get; set; }
        public List<Character> Characters { get; set; }
        public Account Account { get; set; }

        public long SessionId { get; set; }

        public byte World { get; set; }
        public byte Channel { get; set; }

        public MapleClient(Socket client,ServerBase parent, PacketProcessor processor) : base(client)
        {
            Label = client.RemoteEndPoint.ToString();
            m_processor = processor;
            m_parentServer = parent;
            m_parentServer.AddClient(this);
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
                    MainForm.Instance.Log("[{0}] Unhandled packet from {1}: {2}", m_processor.Label, Label, p.ToString());
                }
            }
        }

        protected override void OnDisconnected()
        {
            if (Account != null)
            {
                Account.LoggedIn = false;
                MasterServer.Instance.Database.Save<Account>(Database.Accounts,Account);
            }

            if (Characters != null)
                Characters.Clear();

            if (Character != null)
                MasterServer.Instance.Database.Save<Character>(Database.Characters, Character);
            
            MainForm.Instance.Log("[{0}] Client {1} disconnected", m_processor.Label, Label);
            m_parentServer.RemoveClient(this);
        }
    }
}
