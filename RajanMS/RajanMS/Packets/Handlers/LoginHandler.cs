using System.Collections;
using System.Collections.Generic;
using RajanMS;
using System.Linq;
using RajanMS.IO;
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

            byte loginResult = MasterServer.Instance.Database.Login(c, user, pass);

            if (loginResult == 0)
            {
                c.Account.LoggedIn = true;
                c.WritePacket(LoginPacketCreator.LoginSuccess(c.Account,c.SessionId));

                MasterServer.Instance.Database.SaveAccount(c.Account); //update logged in status
            }
            else
            {
                c.WritePacket(LoginPacketCreator.LoginFail(loginResult));
            }
        }

        public static void HandleWorldInfoRequest(MapleClient c, InPacket p)
        {
            foreach (WorldServer ws in MasterServer.Instance.Worlds)
            {
                var loads = ws.GetChannelLoads();
                string name = Constants.WorldNames[ws.Id];
                c.WritePacket(LoginPacketCreator.Worldlist(ws.Id, name, loads));
            }

            c.WritePacket(LoginPacketCreator.EndOfWorldlist());
        }

        public static void HandleCheckUserLimit(MapleClient c, InPacket p)
        {
            byte world = p.ReadByte();

            if (world < 0 || world > MasterServer.Instance.Worlds.Length)
            {
                return;
            }

            int current = MasterServer.Instance.Worlds[world].CurrentLoad;
            int max = Constants.MaxPlayers;

            if (current >= max) //full
            {
                c.WritePacket(LoginPacketCreator.CheckUserLimit(2));
            }
            else if (current * 2 >= max) //half full
            {
                c.WritePacket(LoginPacketCreator.CheckUserLimit(1));
            }
            else //under half full
            {
                c.WritePacket(LoginPacketCreator.CheckUserLimit(0));
            }
        }

        public static void HandleViewAllCharacters(MapleClient c, InPacket p)
        {
            c.WritePacket(LoginPacketCreator.ShowAllCharacter(c.Characters.Count));

            foreach (WorldServer ws in MasterServer.Instance.Worlds)
            {
                var chars = c.Characters.Where((chr) => chr.WorldId == ws.Id); //characters in same world

                c.WritePacket(LoginPacketCreator.ShowAllCharacterInfo(ws.Id, chars, c.Account.PIC));
            }
        }

        public static void HandleViewWorldInfo(MapleClient c, InPacket p)
        {
            if (p.ReadByte() == 0)
                HandleWorldInfoRequest(c, null);
        }

        public static void HandleWorldSelect(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; }

            p.Skip(1);

            c.World = p.ReadByte();
            c.Channel = p.ReadByte();

            if (MasterServer.Instance.Worlds.InRange(c.World) == false ||
                MasterServer.Instance.Worlds[c.World].Channels.InRange(c.Channel) == false)
            {
                c.Close();
                return;
            }

            c.WritePacket(LoginPacketCreator.SelectWorldResult(c));
        }

        public static void HandleCheckDuplicateName(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; } //dont fkin query for hackers

            string name = p.ReadMapleString();

            bool taken = MasterServer.Instance.Database.NameAvailable(name);

            c.WritePacket(LoginPacketCreator.CheckDuplicatedID(name, taken));
        }

        public static void HandleCreateCharacter(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; }

            string charName = p.ReadMapleString();
            int jobType = p.ReadInt();
            short specialjobtype = p.ReadShort();
            byte gender = p.ReadByte();
            byte skin = p.ReadByte();
            p.Skip(1); //unk
            int face = p.ReadInt();
            int hair = p.ReadInt();

            int haircolor = 0;

            if (jobType != 5 && jobType != 6) //not a mercedes or demon slayer
            {
                haircolor = p.ReadInt();
                skin = (byte)p.ReadInt();
            }

            int demonMark = 0;

            if (jobType == 6) //ds
            {
                demonMark = p.ReadInt();
            }

            int top = p.ReadInt();
            int bottom = 0;
            
            if (jobType < 5)
                bottom = p.ReadInt();

            int shoes = p.ReadInt();
            int weapon = p.ReadInt();
            int shield = 0;

            if (jobType == 6) //ds
                shield = p.ReadInt();
            else if (jobType == 5)
                shield = 1352000; //magic arrows

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
            newChr.DemonSlayerAccessory = demonMark;

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
            c.WritePacket(LoginPacketCreator.CreateNewCharacter(newChr));
        }

        public static void HandleDeleteCharacter(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; }

            string pic = p.ReadMapleString();
            int cid = p.ReadInt();

            if (pic != c.Account.PIC)
            {
                c.WritePacket(LoginPacketCreator.DeleteCharacter(cid, 0x14));
            }
            else
            {
                Character target = default(Character);

                foreach (Character chr in c.Characters)
                {
                    if (chr.CharId == cid)
                        target = chr;
                }

                if (target != default(Character))
                {
                    MasterServer.Instance.Database.DeleteCharacter(target);
                    c.Characters.Remove(target); //no charlist reappear!
                    c.WritePacket(LoginPacketCreator.DeleteCharacter(cid, 0));
                }
                else
                {
                    c.Close(); //hackers mang
                }
            }
        }

        public static void HandleSelectCharacterSetPIC(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; }

            p.Skip(2); // 01 01
            int charId = p.ReadInt();
            string macs = p.ReadMapleString();
            string hwid = p.ReadMapleString();
            string pic = p.ReadMapleString();

            MainForm.Instance.Log(p.ToString());

            c.Account.PIC = pic;
            //MasterServer.Instance.Database.SaveAccount(c.Account); //set pic!

            if (pic != c.Account.PIC)
            {
                c.WritePacket(LoginPacketCreator.BadPic());
            }
            else
            {
                MigrateClient(charId, c);
            }
        }

        public static void HandleSelectCharacter(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; }

            string pic = p.ReadMapleString();
            int charId = p.ReadInt();
            //string macs = p.ReadMapleString();
            //string hwid = p.ReadMapleString();

            if (pic != c.Account.PIC)
            {
                c.WritePacket(LoginPacketCreator.BadPic());
            }
            else
            {
                MigrateClient(charId, c);
            }
        }

        private static void MigrateClient(int charId, MapleClient c)
        {
            byte[] ip = new byte[] { 8, 31, 98, 52 }; //loop me back plz =)

            short port = MasterServer.Instance.Worlds[c.World].Channels[c.Channel].Port;

            MigrateRequest mr = new MigrateRequest(charId, c.SessionId);
            MasterServer.Instance.Worlds[c.World].AddMigrationRequest(mr);
            
            c.WritePacket(LoginPacketCreator.ServerIP(ip, port, charId));
        }
    }
}
