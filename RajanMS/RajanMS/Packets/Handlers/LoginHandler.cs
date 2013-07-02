using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.IO;
using RajanMS.Servers;

namespace RajanMS.Packets.Handlers
{
    static class LoginHandler
    {
        public static void HandleValidate(MapleClient c, InPacket p)
        {
            //14-00-08-6F-00-01-00

            byte locale = p.ReadByte();
            short major = p.ReadShort();
            string minor = p.ReadShort().ToString();

            if (locale != Constants.Locale || major != Constants.MajorVersion || minor != Constants.MinorVersion)
                c.Close();
        }

        public static void HandleLoginPassword(MapleClient c, InPacket p)
        {
            string user = p.ReadMapleString();
            string pass = p.ReadMapleString();
            p.Skip(6);
            int hwid1 = p.ReadInt();
            p.Skip(4);
            int hwid2 = p.ReadInt();

            if (user != "rajan")
            {
                c.WritePacket(PacketCreator.LoginFail(5));
            }
            else if (pass != "12345")
            {
                c.WritePacket(PacketCreator.LoginFail(4));
            }
            else
            {
                c.LoggedIn = true;
                c.AccountName = user;

                c.WritePacket(PacketCreator.LoginSuccess(c));
            }
        }

        public static void HandleServerlistRequest(MapleClient c, InPacket p)
        {
            if (!c.LoggedIn)
            {
                c.Close();
                return;
            }

            foreach (WorldServer ws in MasterServer.Instance.Worlds)
            {
                int[] loads = ws.GetChannelLoads();
                string name = Constants.WorldNames[ws.Id];
                c.WritePacket(PacketCreator.Serverlist(ws.Id, name, loads));
            }

            c.WritePacket(PacketCreator.EndOfServerlist());
        }

        public static void HandleServerStatusRequest(MapleClient c, InPacket p)
        {
            if (!c.LoggedIn)
            {
                c.Close();
                return;
            }

            int current = MasterServer.Instance.LoginServer.ClientLoad;
            int max = Constants.MaxPlayers;

            if (current >= max)
            {
                c.WritePacket(PacketCreator.ServerStatus(2));
            }
            else if (current * 2 >= max)
            {
                c.WritePacket(PacketCreator.ServerStatus(1));
            }
            else
            {
                c.WritePacket(PacketCreator.ServerStatus(0));
            }
        }

        public static void HandleCharlistRequest(MapleClient c, InPacket p)
        {
            if (!c.LoggedIn)
            {
                c.Close();
                return;
            }

            p.Skip(1);

            byte server = p.ReadByte();
            byte channel = p.ReadByte();

            WorldServer wrServer = null;

            foreach (WorldServer ws in MasterServer.Instance.Worlds)
            {
                if (ws.Id == server)
                {
                    wrServer = ws;
                    break;
                }
            }

            if (wrServer == null || channel > wrServer.Channels.Length) //not found
            {
                c.Close();
                return;
            }

            ChannelServer chServer = wrServer.Channels[channel];    

        }
    }
}
