using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.IO;
using RajanMS.Game;

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
            using (OutPacket p = new OutPacket(SendOps.Ping, 2))
            {
                return p.ToArray();
            }
        }

        public static byte[] LoginSuccess(Account acc)
        {
            using (OutPacket p = new OutPacket(SendOps.LoginResponse))
            {
                p.WriteZero(6); //byte byte int
                p.WriteInt(acc.AccountId);
                p.WriteByte(acc.Gender);
                p.WriteBool(acc.GM);
                p.WriteShort();
                p.WriteBool(false); //admin commands
                p.WriteMapleString(acc.Username);
                p.WriteByte(3);
                p.WriteByte(); //quiete ban
                p.WriteLong(); //quiete ban time
                p.WriteByte(1);
                p.WriteLong(); //create time
                p.WriteInt(4);
                p.WriteBool(true); //disable pin

                if (string.IsNullOrEmpty(acc.PIC)) //p.WriteByte(2); //pic 0 = set 1= use 2 = none
                {
                    p.WriteByte();
                }
                else
                {
                    p.WriteByte(1);
                }

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

        public static byte[] Worldlist(byte serverId, string serverName, int[] loads)
        {
            using (OutPacket p = new OutPacket(SendOps.Worldlist))
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
                    p.WriteInt(chLoad * 200);
                    p.WriteByte(serverId);
                    p.WriteShort((short)(id - 1));
                }

                /*
                var chatBubbles = new Dictionary<Point, string>();

                chatBubbles.Add(new Point(0, 265), "What the fuck is this shit?!");
                chatBubbles.Add(new Point(500, 370), "Stolen from Rice?!");
                
                p.WriteShort((short)chatBubbles.Count);

                foreach (KeyValuePair<Point, string> kvp in chatBubbles)
                {
                    p.WritePosition(kvp.Key);
                    p.WriteMapleString(kvp.Value);
                }
                */

                p.WriteZero(6);
                return p.ToArray();
            }
        }

        public static byte[] EndOfWorldlist()
        {
            using (OutPacket p = new OutPacket(SendOps.Worldlist, 3))
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
        public static byte[] UserLimit(byte status)
        {
            using (OutPacket p = new OutPacket(SendOps.ServerStatus, 4))
            {
                p.WriteShort(status);
                return p.ToArray();
            }
        }

        public static byte[] CharacterList(MapleClient c)
        {
            using (OutPacket p = new OutPacket(SendOps.Charlist))
            {
                p.WriteByte(); //idk

                IEnumerable<Character> chars = c.Characters.Where((chr) => chr.WorldId == c.World); //characters in same world

                p.WriteByte((byte)chars.Count());

                foreach (Character chr in chars)
                {
                    AddCharEntry(p, chr, chr.Level >= 30, false);
                }

                if (string.IsNullOrEmpty(c.Account.PIC)) //p.WriteByte(2); //pic 0 = set 1= use 2 = none
                {
                    p.WriteByte(); //set
                }
                else
                {
                    p.WriteByte(1); //use
                }

                p.WriteByte();
                p.WriteInt(3); //char slots
                p.WriteInt();
                return p.ToArray();
            }
        }

        public static void AddCharStats(OutPacket p, Character c)
        {
            p.WriteInt(c.CharId);
            p.WritePaddedString(c.Name, 13);
            p.WriteByte(c.Gender);
            p.WriteByte(c.SkinColor);
            p.WriteInt(c.FaceId);
            p.WriteInt(c.HairId);
            p.WriteZero(24);// pet unique id's
            p.WriteByte(c.Level);
            p.WriteShort(c.Job);
            p.WriteShort(c.Str);
            p.WriteShort(c.Dex);
            p.WriteShort(c.Int);
            p.WriteShort(c.Luk);
            p.WriteInt(c.HP);
            p.WriteInt(c.MaxHP);
            p.WriteInt(c.MP);
            p.WriteInt(c.MaxMP);
            p.WriteShort(c.AP);

            if (Constants.isSeparatedSp(c.Job))
            {
                byte length = 0;

                for (int i = 0; i < c.SP.Length; i++)
                {
                    if (c.SP[i] > 0)
                    {
                        length++;
                    }
                }

                p.WriteByte(length);

                for (int i = 0; i < c.SP.Length; i++)
                {
                    if (c.SP[i] > 0)
                    {
                        p.WriteByte((byte)(i + 1));
                        p.WriteByte(c.SP[i]);
                    }
                }
            }
            else
            {
                p.WriteShort(c.SP[0]);
            }

            p.WriteInt(c.EXP);
            p.WriteInt(c.Fame);
            p.WriteInt(); // gacha exp
            p.WriteInt(c.MapId);
            p.WriteByte(c.SpawnPoint);
            p.WriteInt(0);// online time in seconds


            if (Constants.isDualBlade(c.Job))
                p.WriteShort(1);
            else if (Constants.isCannon(c.Job))
                p.WriteShort(2);
            else
                p.WriteShort();

            if (Constants.isDemon(c.Job))
                p.WriteInt(c.DemonSlayerAccessory);

            p.WriteByte(c.Fatigue);

            p.WriteInt(0);

            p.WriteInt(c.Ambition);
            p.WriteInt(c.Insight);
            p.WriteInt(c.Willpower);
            p.WriteInt(c.Diligence);
            p.WriteInt(c.Empathy);
            p.WriteInt(c.Charm);
            p.WriteShort(c.AmbitionGained);
            p.WriteShort(c.InsightGained);
            p.WriteShort(c.WillpowerGained);
            p.WriteShort(c.DiligenceGained);
            p.WriteShort(c.EmpathyGained);
            p.WriteShort(c.CharmGained);

            p.WriteInt(c.BattleEXP);
            p.WriteByte(c.BattleRank);
            p.WriteInt(c.BattlePoints);
            p.WriteByte(5);
            p.WriteInt(0);

            p.WriteInt((int)(DateTime.Now.ToFileTime() >> 32)); // FileTime.dwHighDateTime
            p.WriteInt((int)(DateTime.Now.ToFileTime() << 32 >> 32)); // FileTime.dwLowDateTime
            //p.WriteBytes(new byte[] { 0xD5, 0x64, 0xFB, 0x95, 0x37, 0x01, 0x00, 0x00 });
            //p.WriteLong();
            //p.WriteInt(0x95FB64D5); // dwHighDateTime // D5 64 FB 95
            //p.WriteInt(0x137); // dwLowDateTime
        }

        public static void AddCharLook(OutPacket p, Character c, bool megaphone)
        {
            p.WriteByte(c.Gender);
            p.WriteByte(c.SkinColor);
            p.WriteInt(c.FaceId);
            p.WriteInt(c.Job);
            p.WriteByte(megaphone ? (byte)0 : (byte)1);
            p.WriteInt(c.HairId);

            //TO WRITE INVENTORY UGH
            p.WriteByte(255);
            p.WriteByte(255);

            p.WriteInt(); //cash weapon
            p.WriteBool(Constants.isMercedes(c.Job));
            p.WriteZero(12); //pet
            if (Constants.isDemon(c.Job)) // demon slayer
                p.WriteInt(c.DemonSlayerAccessory);
        }

        public static void AddCharEntry(OutPacket p, Character c, bool rank, bool viewAll)
        {
            AddCharStats(p, c);
            AddCharLook(p, c, true);

            if (!viewAll)
            {
                p.WriteByte();
            }

            p.WriteBool(rank);

            if (rank)
            {
                p.WriteInt(1); //rank
                p.WriteInt(1); //rank move
                p.WriteInt(1); //job rank
                p.WriteInt(1); //job rank move
            }
        }

        public static byte[] NameAvailable(string name, bool isTaken)
        {
            using (OutPacket p = new OutPacket(SendOps.NameAvailable))
            {
                p.WriteMapleString(name);
                p.WriteBool(isTaken);
                return p.ToArray();
            }
        }

        public static byte[] NewCharacter(Character chr)
        {
            using (OutPacket p = new OutPacket(SendOps.CreateCharacter))
            {
                p.WriteByte();
                AddCharEntry(p, chr, false, false);
                return p.ToArray();
            }
        }

        /// <summary>
        /// 0 = okay
        /// 0x12 = invalid birthday
        /// 0x14 = invalid pic
        /// </summary>
        public static byte[] DeleteCharacter(int cid, byte mode)
        {
            using (OutPacket p = new OutPacket(SendOps.DeleteCharacter, 8))
            {
                p.WriteInt(cid);
                p.WriteByte(mode);
                return p.ToArray();
            }
        }

        public static byte[] BadPic()
        {
            using (OutPacket p = new OutPacket(SendOps.CheckPIC,3))
            {
                p.WriteByte(0);
                return p.ToArray();
            }
        }
    }
}
