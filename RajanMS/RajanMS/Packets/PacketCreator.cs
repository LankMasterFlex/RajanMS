using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RajanMS.IO;
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
                return p.ToArray();
        }
    }

    static class PacketCreatorHelper
    {
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
                byte length = (byte)c.SP.Count((b) => b > 0);

                p.WriteByte(length);

                c.SP.ForAll((i) => //linq where caused issue with indexs O_O no idea how or why
                    {
                        if (i > 0)
                        {
                            p.WriteByte((byte)(i + 1));
                            p.WriteByte(c.SP[i]);
                        }
                    });
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
            p.WriteInt(c.Ambition); //its my amibiton nigga *ross grunt*
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

            p.WriteBytes(new byte[] { 0xD5, 0x64, 0xFB, 0x95, 0x37, 0x01, 0x00, 0x00 }); //p.WriteInt(0x95FB64D5); // dwHighDateTime // D5 64 FB 95 | p.WriteInt(0x137); // dwLowDateTime
        }

        public static void AddCharLook(OutPacket p, Character c, bool megaphone)
        {
            p.WriteByte(c.Gender);
            p.WriteByte(c.SkinColor);
            p.WriteInt(c.FaceId);
            p.WriteInt(c.Job);
            p.WriteByte(megaphone ? (byte)0 : (byte)1);
            p.WriteInt(c.HairId);

        //    c.Inventory[InventoryType.EQUIP]

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

        public static void AddCharInfo(OutPacket p, Character c)
        {
            p.WriteInt(-1);
            p.WriteInt(-3);
            p.WriteZero(7);//5 bytes v99 [byte] [byte] [int] [byte]
            AddCharStats(p, c);
            p.WriteByte(); //buddylist capacity

            //if true maplestring trail
            p.WriteBool(false);//fairy blessing link
            p.WriteBool(false); //empress blessing link
            p.WriteBool(false); //ult explorer

            //addInventoryInfo(mplew, chr);
            //addSkillInfo(mplew, chr); // 0x100
            //addCoolDownInfo(mplew, chr); // 0x8000
            //addQuestInfo(mplew, chr);
            //addRingInfo(mplew, chr);
            //addRocksInfo(mplew, chr); // 0x1000
            //addMonsterBookInfo(mplew, chr);

            p.WriteShort();
            p.WriteShort();// New year gift card size // 0x40000

        }
    }

    static class LoginPacketCreator
    {
        public static byte[] LoginSuccess(Account acc, long sessionId)
        {
            using (OutPacket p = new OutPacket(SendOps.CheckPassword))
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
                p.WriteBool(true); //disable pin OR use session (maybe both)

                if (string.IsNullOrEmpty(acc.PIC)) 
                {
                    p.WriteByte();
                }
                else
                {
                    p.WriteByte(1);
                }

                p.WriteLong(sessionId); //jajajaja
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
            using (OutPacket p = new OutPacket(SendOps.CheckPassword, 9))
            {
                p.WriteByte(reason);
                p.WriteZero(6);
                return p.ToArray();
            }
        }

        public static byte[] Worldlist(byte serverId, string serverName, int[] loads, params Tuple<Point, string>[] balloons)
        {
            using (OutPacket p = new OutPacket(SendOps.WorldInformation))
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
                //

                p.WriteShort((short)balloons.Length); //ZArray_CLogin::BALLOON_::RemoveAll((char *)v2 + 560); if 0

                foreach (Tuple<Point, string> balloon in balloons)
                {
                    p.WritePosition(balloon.Item1);
                    p.WriteMapleString(balloon.Item2);
                }


                p.WriteInt();
                return p.ToArray();
            }
        }

        public static byte[] EndOfWorldlist()
        {
            using (OutPacket p = new OutPacket(SendOps.WorldInformation, 3))
            {
                p.WriteShort(0xFF); //113+ controversy? nothing wrong with some padding
                return p.ToArray();
            }
        }

        /// <summary>
        ///0 - Normal
        ///1 - Highly populated
        ///2 - Full
        /// </summary>
        /// <returns></returns>
        public static byte[] CheckUserLimit(byte status)
        {
            using (OutPacket p = new OutPacket(SendOps.CheckUserLimit, 4))
            {
                p.WriteShort(status);
                return p.ToArray();
            }
        }

        public static byte[] SelectWorldResult(MapleClient c)
        {
            using (OutPacket p = new OutPacket(SendOps.SelectWorldResult))
            {
                p.WriteByte(); //idk

                var chars = c.Characters.Where((chr) => chr.WorldId == c.World); //characters in same world

                p.WriteByte((byte)chars.Count());

                foreach (Character chr in chars)
                {
                    PacketCreatorHelper.AddCharEntry(p, chr, chr.Level >= 30, false);
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



        public static byte[] CheckDuplicatedID(string name, bool isTaken)
        {
            using (OutPacket p = new OutPacket(SendOps.CheckDuplicatedID, 3 + name.Length))
            {
                p.WriteMapleString(name);
                p.WriteBool(isTaken);
                return p.ToArray();
            }
        }

        public static byte[] CreateNewCharacter(Character chr)
        {
            using (OutPacket p = new OutPacket(SendOps.CreateNewCharacter))
            {
                p.WriteByte();
                PacketCreatorHelper.AddCharEntry(p, chr, false, false);
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
            using (OutPacket p = new OutPacket(SendOps.CheckSPW, 3))
            {
                p.WriteByte(0);
                return p.ToArray();
            }
        }

        public static byte[] ShowAllCharacter(int chars)
        {
            using (OutPacket p = new OutPacket(SendOps.ViewAllChar))
            {
                p.WriteByte(1);
                p.WriteInt(chars);
                p.WriteInt(chars + (3 - chars % 3)); //rowsize
                return p.ToArray();
            }
        }

        public static byte[] ShowAllCharacterInfo(byte worldId, IEnumerable<Character> chars, string pic)
        {
            using (OutPacket p = new OutPacket(SendOps.ViewAllChar))
            {
                p.WriteByte(chars.Count() == 0 ? (byte)5 : (byte)0);//5 = cannot find any
                p.WriteByte(worldId);
                p.WriteByte((byte)chars.Count());

                foreach (Character chr in chars)
                {
                    PacketCreatorHelper.AddCharEntry(p, chr, true, true);
                }

                if (string.IsNullOrEmpty(pic)) //p.WriteByte(2); //pic 0 = set 1= use 2 = none
                {
                    p.WriteByte(); //set
                }
                else
                {
                    p.WriteByte(1); //use
                }

                return p.ToArray();
            }
        }

        public static byte[] ServerIP(byte[] ip, short port, int charId)
        {
            using (OutPacket p = new OutPacket(SendOps.ServerIP))
            {
                p.WriteShort();
                p.WriteBytes(ip);
                p.WriteShort(port);
                p.WriteInt(charId);
                p.WriteZero(10);
                return p.ToArray();
            }
        }


    }

    static class FieldPacketCreator
    {
        public static byte[] LoadStage(MapleClient c, int id, byte spawnpoint)
        {
            using (OutPacket p = new OutPacket(SendOps.LoadStage))
            {
                p.WriteShort(2);
                p.WriteLong(1);
                p.WriteLong(2);
                p.WriteLong(c.Channel);
                p.WriteByte();
                p.WriteLong(2);
                p.WriteByte();
                p.WriteInt(id);
                p.WriteByte(spawnpoint);
                p.WriteInt(c.Character.HP);
                p.WriteByte();
                p.WriteLong(); //miliseconds
                p.WriteInt(100);
                p.WriteByte();
                p.WriteByte(Constants.isResist(c.Character.Job) ? (byte)0 : (byte)1);
                return p.ToArray();
            }
        }

        public static byte[] LoadInitialStage(MapleClient c, int id, byte spawnpoint)
        {
            using (OutPacket p = new OutPacket(SendOps.LoadStage))
            {
                p.WriteShort(2);
                p.WriteLong(1);
                p.WriteLong(2);
                p.WriteLong(c.Channel);
                p.WriteByte();
                p.WriteByte(1);
                p.WriteInt();
                p.WriteByte(1);
                p.WriteShort();
                p.WriteZero(3 * 4);
                PacketCreatorHelper.AddCharInfo(p, c.Character);
                p.WriteZero(16);
                p.WriteLong();
                p.WriteInt(100);
                p.WriteByte();
                p.WriteByte(1);

                return p.ToArray();
            }
        }
    }
}
