using System.Collections;
using System.Collections.Generic;
using RajanMS;
using System.Linq;
using RajanMS.IO;
using RajanMS.Game;
using RajanMS.Servers;
using RajanMS.Tools;


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

            //TODO: Write banning

            if (loginResult == 0)
            {
                c.Account.LoggedIn = true;
                c.Send(LoginPacketCreator.LoginSuccess(c.Account,c.SessionId));

                MasterServer.Instance.Database.Save<Account>(Database.Accounts,c.Account); //update logged in status
            }
            else
            {
                c.Send(LoginPacketCreator.LoginFail(loginResult));
            }
        }

        public static void HandleWorldInfoRequest(MapleClient c, InPacket p)
        {
            foreach (WorldServer ws in MasterServer.Instance.Worlds)
            {
                var loads = ws.GetChannelLoads();
                string name = Constants.WorldNames[ws.Id];
                c.Send(LoginPacketCreator.Worldlist(ws.Id, name, loads));
            }

            c.Send(LoginPacketCreator.EndOfWorldlist());
        }

        public static void HandleCheckUserLimit(MapleClient c, InPacket p)
        {
            byte world = p.ReadByte();

            if (!MasterServer.Instance.Worlds.InRange(world))
            {
                return;
            }

            int current = MasterServer.Instance.Worlds[world].CurrentLoad;

            if (current >= Constants.MaxPlayers) //full
            {
                c.Send(LoginPacketCreator.CheckUserLimit(2));
            }
            else if (current * 2 >= Constants.MaxPlayers) //half full
            {
                c.Send(LoginPacketCreator.CheckUserLimit(1));
            }
            else //under half full
            {
                c.Send(LoginPacketCreator.CheckUserLimit(0));
            }
        }

        public static void HandleViewAllCharacters(MapleClient c, InPacket p)
        {
            c.Send(LoginPacketCreator.ShowAllCharacter(c.Characters.Count));

            foreach (WorldServer ws in MasterServer.Instance.Worlds)
            {
                var chars = c.Characters.Where((chr) => chr.WorldId == ws.Id); //characters in same world

                c.Send(LoginPacketCreator.ShowAllCharacterInfo(ws.Id, chars, c.Account.PIC));
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
                return;
            }

            c.Send(LoginPacketCreator.SelectWorldResult(c));
        }

        public static void HandleCheckDuplicateName(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; } //dont fkin query for hackers

            string name = p.ReadMapleString();

            bool taken = MasterServer.Instance.Database.NameAvailable(name);

            c.Send(LoginPacketCreator.CheckDuplicatedID(name, taken));
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
            //newChr.Job = -1;
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

            /*
            if(newChr.Job == -1)
            {
                c.Close(); //hax
                return;
            }

            string jobStr = newChr.Job.ToString();

            if (jobStr == "0") //pad for beginner node
                jobStr = "000";

            string genderStr = gender == 0 ? "male" : "female";

            NXFile etcNX = MasterServer.Instance.Provider.EtcNX;
            NXNode reqNode = etcNX.BaseNode["MakeCharInfo.img"][jobStr][genderStr];

            bool isEligable = true;

            foreach (NXNode node in reqNode)
            {
                switch (node.Name)
                {
                    case "0":
                        isEligable = TestRequireIdNode(node, face);
                        break;
                    case "1":
                        isEligable = TestRequireIdNode(node, hair);
                        break;
                    case "2":
                        isEligable = TestRequireIdNode(node, haircolor);
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "6":
                        break;
                    case "7":
                        break;
                }

                if (isEligable == false)
                    break;
            }

            */


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
            MasterServer.Instance.Database.Save<Character>(Database.Characters, newChr);
            c.Send(LoginPacketCreator.CreateNewCharacter(newChr));
        }

        public static void HandleDeleteCharacter(MapleClient c, InPacket p)
        {
            if (!c.Account.LoggedIn) { return; }

            string pic = p.ReadMapleString();
            int cid = p.ReadInt();

            if (pic != c.Account.PIC)
            {
                c.Send(LoginPacketCreator.DeleteCharacter(cid, 0x14));
            }
            else
            {
                Character target = c.Characters.FindOne((chr) => chr.CharId == cid);

                if (target != default(Character)) //findone return
                {
                    MasterServer.Instance.Database.DeleteCharacter(target);
                    c.Characters.Remove(target); //no charlist reappear!
                    c.Send(LoginPacketCreator.DeleteCharacter(cid, 0));
                }
                else
                {
                    c.Close(); //hackers mang smh
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
                c.Send(LoginPacketCreator.BadPic());
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
                c.Send(LoginPacketCreator.BadPic());
            }
            else
            {
                MigrateClient(charId, c);
            }
        }

        private static void MigrateClient(int charId, MapleClient c)
        {
            int count = c.Characters.Count((chr) => chr.CharId == charId);

            if (count != 1) //does not have character
            {
                c.Close();
                return;
            }

            byte[] ip = new byte[] { 8, 31, 98, 52 }; //loop me back plz =)

            short port = MasterServer.Instance.Worlds[c.World].Channels[c.Channel].Port;

            MasterServer.Instance.Worlds[c.World].AddMigrationRequest(charId, c.SessionId);
            
            c.Send(LoginPacketCreator.ServerIP(ip, port, charId));
        }
    }
}
