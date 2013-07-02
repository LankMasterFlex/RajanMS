using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.IO;

namespace RajanMS.Packets
{
    static class PacketCreator
    {
        public static byte[] Handshake()
        {
            //lol this should just be a static byte[]
            using (OutPacket p = new OutPacket(14, 16))
            {
                p.WriteShort(Constants.MajorVersion);
                p.WriteMapleString(Constants.MinorVersion);
                p.WriteBytes(Constants.RIV);
                p.WriteBytes(Constants.SIV);
                p.WriteByte(Constants.Locale);
                return p.ToArray();
            }
        }

        public static byte[] Ping()
        {
            using (OutPacket p = new OutPacket(SendOps.Ping,2))
            {
                return p.ToArray();
            }
        }

        public static byte[] LoginSuccess(MapleClient c)
        {
            using (OutPacket p = new OutPacket(SendOps.LoginResponse))
            {
                p.WriteZero(6); //byte byte int
                p.WriteInt(c.AccountId);
                p.WriteByte(c.Gender);
                p.WriteBool(c.IsAdmin);
                p.WriteShort();
                p.WriteBool(false); //admin commands
                p.WriteMapleString(c.AccountName);
                p.WriteByte(3);
                p.WriteByte(); //quiete ban
                p.WriteLong(); //quiete ban time
                p.WriteByte(1);
                p.WriteLong(); //create time
                p.WriteInt(4);
                p.WriteBool(true); //disable pin
                p.WriteByte(2); //pic 0 = set 1= use 2 = none
                p.WriteLong(); //session?
                return p.ToArray();
            }
        }

        /// <summary>
        /// -1/6/8/9 : Trouble logging in
        /// 2/3 : Id deleted or blocked
        /// 4: Incorrect password
        /// 5: Not a registered ID
        /// 7: Logged in    
        /// 10: Too many requests
        /// 11: 20 years older can use
        /// 13: Unable to log on as master at IP
        /// 14/15: Redirect to nexon + buttons    
        /// 16/21: Verify account
        /// 17: Selected the wrong gateway
        /// 25: Logging in outside service region
        /// 23: License agreement
        /// 27: Download full client
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static byte[] LoginFail(byte reason)
        {
            using (OutPacket p = new OutPacket(SendOps.LoginResponse))
            {
                p.WriteByte(reason);
                p.WriteZero(6);
                return p.ToArray();
            }
        }

        public static byte[] Serverlist(byte serverId,string serverName,int[] loads)
        {
            using (OutPacket p = new OutPacket(SendOps.Serverlist))
            {
                p.WriteByte(serverId);
                p.WriteMapleString(serverName);
                p.WriteByte(); // Ribbon: 1 = E; 2 = N; 3 = H
                p.WriteMapleString(Constants.EventMessage);
                p.WriteShort(100);
                p.WriteShort(100);
                p.WriteByte();
                p.WriteByte((byte)loads.Length);

                int id = 1;

                foreach (int chLoad in loads)
                {
                    p.WriteMapleString("{0}-{1}", serverName, id++);
                    p.WriteInt(chLoad);
                    p.WriteByte(serverId);
                    p.WriteShort((short)(id - 1));
                }

                p.WriteZero(6);
                return p.ToArray();
            }
        }

        public static byte[] EndOfServerlist()
        {
            using (OutPacket p = new OutPacket(SendOps.Serverlist,3))
            {
                p.WriteShort(0xFF); //113+ controversy? nothing wrong with some padding :)
                return p.ToArray();
            }
        }

        /// <summary>
        ///0 - Normal
        ///1 - Highly populated
        ///2 - Full
        /// </summary>
        /// <returns></returns>
        public static byte[] ServerStatus(byte status)
        {
            using (OutPacket p = new OutPacket(SendOps.ServerStatus, 4))
            {
                p.WriteShort(status);
                return p.ToArray();
            }
        }
    }
}
