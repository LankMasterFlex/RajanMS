using RajanMS.Game;
using RajanMS.IO;
using RajanMS.Network;
using RajanMS.Packets;
using RajanMS.Servers;
using RajanMS.Tools;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

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

        public bool LoggedIn { get; set; }

        public byte Channel { get; set; }

        public MapleClient(Socket client, ServerBase parent, PacketProcessor processor)
            : base(client)
        {
            Label = client.RemoteEndPoint.ToString();
            m_processor = processor;
            m_parentServer = parent;
            m_parentServer.AddClient(this);
        }

        protected override void OnPacket(byte[] packet)
        {
            var p = new InPacket(packet);
            PacketHandler handler = m_processor[p.ReadByte()];

            if (handler != null)
                handler(this, p);
            else
                MainForm.Instance.Log(BitConverter.ToString(packet).Replace('-', ' '));
        }

        protected override void OnDisconnected()
        {
            if (Characters != null)
                Characters.Clear();

            if (Character != null)
                Database.Instance.Save<Character>(Database.Characters, Character);

            if (LoggedIn)
                MasterServer.Instance.RemoveClient(Account.Username);

            MainForm.Instance.Log("[{0}] Client {1} disconnected", m_processor.Label, Label);
            m_parentServer.RemoveClient(this);
        }
    }
}
