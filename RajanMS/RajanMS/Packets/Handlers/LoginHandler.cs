using Common;
using Common.IO;
using RajanMS.Game;
using RajanMS.Servers;

namespace RajanMS.Packets.Handlers
{
    static class LoginHandler
    {
        public static void HandleValidate(MapleClient c, InPacket p)
        {
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

            int loginResult = MasterServer.Instance.Database.Login(c, user, pass);

            if (loginResult == 1)
            {
                c.Account.LoggedIn = true;
                c.WritePacket(PacketCreator.LoginSuccess(c.Account));

                MasterServer.Instance.Database.SaveAccount(c.Account); //update logged in status
            }
            else if (loginResult == 2)
            {
                c.WritePacket(PacketCreator.LoginFail(5));
            }
            else if (loginResult == 3)
            {
                c.WritePacket(PacketCreator.LoginFail(4));
            }
            else if (loginResult == 4)
            {
                c.WritePacket(PacketCreator.LoginFail(7));
            }
            else
            {
                c.WritePacket(PacketCreator.LoginFail(9));
            }
        }

        public static void HandleWorldInfoRequest(MapleClient c, InPacket p)
        {
            foreach (WorldServer ws in MasterServer.Instance.Worlds)
            {
                var loads = ws.GetChannelLoads();
                string name = Constants.WorldNames[ws.Id];
                c.WritePacket(PacketCreator.Worldlist(ws.Id, name, loads));
            }

            c.WritePacket(PacketCreator.EndOfWorldlist());
        }

        public static void HandleCheckUserLimit(MapleClient c, InPacket p)
        {
            byte world = p.ReadByte();

            int current = MasterServer.Instance.Worlds[world].CurrentLoad;
            int max = Constants.MaxPlayers;

            if (current >= max) //full
            {
                c.WritePacket(PacketCreator.UserLimit(2));
            }
            else if (current * 2 >= max) //half full
            {
                c.WritePacket(PacketCreator.UserLimit(1));
            }
            else //under half full
            {
                c.WritePacket(PacketCreator.UserLimit(0));
            }
        }

        public static void HandleWorldSelect(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn)
            {
                return;
            }

            p.Skip(1);

            c.World = p.ReadByte();
            c.Channel = p.ReadByte();

            c.WritePacket(PacketCreator.CharacterList(c));
        }

        public static void HandleCheckDuplicateName(MapleClient c, InPacket p)
        {
            string name = p.ReadMapleString();

            bool taken =  MasterServer.Instance.Database.NameAvailable(name);

            c.WritePacket(PacketCreator.NameAvailable(name, taken));
        }

        public static void HandleCreateCharacter(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn)
            {
                return;
            }

           string charName  = p.ReadMapleString();
            int jobType = p.ReadInt();
            short specialjobtype = p.ReadShort();
            byte gender = p.ReadByte();
            p.Skip(2);
            int face = p.ReadInt();
            int hair = p.ReadInt();
            int haircolor = p.ReadInt();
            int skin = p.ReadInt();
            int top = p.ReadInt();
            int bottom = 0;
            if (jobType < 5)
                bottom = p.ReadInt();
            int shoes = p.ReadInt();
            int weapon = p.ReadInt();
            int shield = 0;
            if (jobType == 6)
                shield = p.ReadInt();

            Character newChr = new Character();

            switch (jobType)
            {
                case 0:
                    newChr.Job = (short)Constants.Job.Citizen;
                    break;
                case 1:
                    newChr.Job = (short)Constants.Job.Beginner;
                    break;
                case 2:
                    newChr.Job = (short)Constants.Job.Noblesse;
                    break;
                case 3:
                    newChr.Job = (short)Constants.Job.Legend;
                    break;
                case 4:
                    newChr.Job = (short)Constants.Job.Evan1;
                    break;
                case 5:
                    newChr.Job = (short)Constants.Job.Mercedes;
                    break;
                case 6:
                    newChr.Job = (short)Constants.Job.DemonSlayer;
                    break;
            }

            newChr.AccountId = c.Account.AccountId;
            newChr.WorldId = c.World;
            newChr.CharId = MasterServer.Instance.Database.GetNewCharacterId();
            newChr.Name = charName;

            newChr.MapId = 100000000;
            newChr.HairId = hair + haircolor; //lol
            newChr.FaceId = face;
            newChr.SkinColor = (byte)skin;
            newChr.Gender = gender;

            newChr.Level = 1;
            newChr.HP = 50;
            newChr.MaxHP = 50;
            newChr.MP = 50;
            newChr.MaxMP = 50;
            newChr.Str = 4;
            newChr.Dex = 4;
            newChr.Int = 4;
            newChr.Luk = 4;

            c.Characters.Add(newChr);
            MasterServer.Instance.Database.SaveCharacter(newChr);
            c.WritePacket(PacketCreator.NewCharacter(newChr));
        }

        public static void HandleDeleteCharacter(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn)
            {
                return;
            }

            string pic = p.ReadMapleString();
            int cid = p.ReadInt();

            if (pic != c.Account.PIC)
            {
                c.WritePacket(PacketCreator.DeleteCharacter(cid, 0x14));
            }
            else
            {
                Character target = null;

                foreach (Character chr in c.Characters)
                {
                    if (chr.CharId == cid)
                        target = chr;
                }

                if (target != null)
                {
                    MasterServer.Instance.Database.DeleteCharacter(target);
                    c.WritePacket(PacketCreator.DeleteCharacter(cid, 0));
                }
                else
                {
                    c.Close(); //hackers mang
                }
            }
        }

        public static void HandleSelectCharacterSetPIC(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn)
            {
                return;
            }

            p.Skip(2); // 01 01
            int charId = p.ReadInt();
            string macs = p.ReadMapleString();
            string hwid = p.ReadMapleString();
            string pic = p.ReadMapleString();

            MainForm.Instance.Log(p.ToString());

            c.Account.PIC = pic;
            MasterServer.Instance.Database.SaveAccount(c.Account); //set pic!

            if (pic != c.Account.PIC)
            {
                c.WritePacket(PacketCreator.BadPic());
            }
            else
            {
                MigrateClient(charId,c);
            }
        }

        public static void HandleSelectCharacter(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn)
            {
                return;
            }

            string pic = p.ReadMapleString();
            int charId = p.ReadInt();
            //string macs = p.ReadMapleString();
            //string hwid = p.ReadMapleString();

            if (pic != c.Account.PIC)
            {
                c.WritePacket(PacketCreator.BadPic());
            }
            else
            {
                MigrateClient(charId, c);
            }
        }

        private static void MigrateClient(int charId,MapleClient c)
        {
            MainForm.Instance.Log("Write Migration!");
        }
    }
}
