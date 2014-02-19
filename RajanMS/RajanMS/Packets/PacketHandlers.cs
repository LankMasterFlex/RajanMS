using RajanMS.Game;
using RajanMS.Game.Inventory;
using RajanMS.IO;
using RajanMS.Servers;
using RajanMS.Tools;
using System.Linq;

namespace RajanMS.Packets.Handlers
{
    static class PacketHandlers
    {
        public static void OnCheckPassword(MapleClient c, InPacket p)
        {
            string user = p.ReadMapleString().Trim();
            string pass = p.ReadMapleString();

            //byte[] machineId = p.ReadBytes(0x10);
            //int gameRoomCode = p.ReadInt();
            //byte gameStartMode = p.ReadByte();
            //bool adminClient = p.ReadBool();

            byte loginResult = MasterServer.Instance.LoginClient(c, user, pass);

            using (OutPacket packet = new OutPacket(SendOps.CheckPasswordResult))
            {
                packet.WriteByte(loginResult);
                packet.WriteZero(5); //v11 (1) + v12 (4)

                if (loginResult == 0)
                {
                    packet.WriteInt(c.Account.AccountId);
                    packet.WriteByte();
                    packet.WriteBool(c.Account.GM);
                    packet.WriteByte(1);
                    packet.WriteMapleString(c.Account.Username);
                    packet.WriteZero(12);
                }
                else if (loginResult == 2) //TODO : Add packet for this
                {
                    c.Close();
                    return;
                }

                packet.WriteZero(24); //padding
                c.Send(packet);
            }

            if (loginResult != 0)
                return;

            using (OutPacket packet = new OutPacket(SendOps.CheckUserLimitResult,3))
            {
                packet.WriteByte(4);
                c.Send(packet);
            }

            using (OutPacket packet = new OutPacket(SendOps.WorldInformation))
            {
                byte worldId = 0;
                string name = Constants.WorldName;

                packet.WriteByte(worldId);
                packet.WriteMapleString(name);

                packet.WriteByte((byte)MasterServer.Instance.Channels.Length);

                int id = 1;

                foreach (var ch in MasterServer.Instance.Channels)
                {
                    packet.WriteMapleString("{0}-{1}", name, id++);
                    packet.WriteInt(ch.Load);
                    packet.WriteByte(worldId);
                    packet.WriteByte((byte)(id - 1));
                    packet.WriteByte(0x00);
                }

                c.Send(packet);
            }

            using (OutPacket packet = new OutPacket(SendOps.WorldInformation, 3))
            {
                packet.WriteByte(0xff);
                c.Send(packet);
            }
        }
        public static void OnSelectWorld(MapleClient c, InPacket p)
        {
            p.Skip(1); // world id
            c.Channel = p.ReadByte();

            using (OutPacket packet = new OutPacket(SendOps.SelectWorldResult))
            {
                packet.WriteByte(); //some error

                packet.WriteByte((byte)c.Characters.Count);

                foreach (var character in c.Characters)
                {
                    PacketCreator.Helper.AddCharacterData(packet, character);
                    PacketCreator.Helper.AddCharaterLook(packet, character);
                    packet.WriteBool(false); //rank
                }

                packet.WriteZero(64); //padding
                c.Send(packet);
            }
        }
        public static void OnCheckUserLimit(MapleClient c, InPacket p)
        {
            int current = MasterServer.Instance.Channels.Sum((ch) => ch.Load);

            using (OutPacket packet = new OutPacket(SendOps.CheckUserLimitResult, 2))
            {
                if (current >= Constants.MaxPlayers) //full
                {
                    packet.WriteByte(2);
                }
                else if (current * 2 >= Constants.MaxPlayers) //half full
                {
                    packet.WriteByte(1);
                }
                else //under half full
                {
                    packet.WriteByte();
                }

                c.Send(packet);
            }
        }
        public static void OnSelectCharacter(MapleClient c, InPacket p)
        {
            int charId = p.ReadInt();

            Character target = c.Characters.FindOne((chr) => chr.CharId == charId);

            if (target != default(Character))
            {
                using(OutPacket packet = new OutPacket(SendOps.SelectCharacterResult))
                {
                    packet.WriteShort();
                    packet.WriteBytes(127, 0, 0, 1); //TODO : non const ip
                    packet.WriteShort(MasterServer.Instance.Channels[c.Channel].Port);
                    packet.WriteInt(charId);
                    packet.WriteByte();

                    c.Send(packet);
                }
            }
            else
            {
                c.Close();
            }
        }
        public static void OnMigrateIn(MapleClient c,InPacket p)
        {
            int charId = p.ReadInt();
            c.Character = Database.Instance.GetCharacter(charId);
            c.Account = Database.Instance.GetAccount(c.Character.AccountId);
            c.LoggedIn = true;

            using(OutPacket packet = new OutPacket(SendOps.SetField))
            {
                packet.WriteInt(c.Channel);
                packet.WriteByte(); //pcount
                packet.WriteBool(true); //new channel

                for (int i = 0; i < 3; i++) //rand seeds
                    packet.WriteInt(Randomizer.Generate());

                packet.WriteInt();
                packet.WriteShort(-1); // Flags (contains everything: 0xFFFF)

                PacketCreator.Helper.AddCharacterData(packet, c.Character);

                packet.WriteByte(20); //Buddylist

                PacketCreator.Helper.AddInventory(packet, c.Character);

                packet.WriteShort(); //skills
                packet.WriteShort(); //quests
                packet.WriteShort(); //rps
                packet.WriteShort(); //rings

                foreach(int rock in c.Character.TeleportRock)
                    packet.WriteInt(rock);

                packet.WriteLong(64); //padding

                c.Send(packet);
            }

        }
        public static void OnCheckDuplicatedID(MapleClient c, InPacket p)
        {
            string name = p.ReadMapleString();
            bool taken = !Database.Instance.NameAvailable(name);

            using (OutPacket packet = new OutPacket(SendOps.CheckDuplicatedIDResult))
            {
                packet.WriteMapleString(name);
                packet.WriteBool(taken);
                c.Send(packet);
            }
        }
        public static void OnCreateNewCharacter(MapleClient c, InPacket p)
        {
            string name = p.ReadMapleString();

            if(Database.Instance.NameAvailable(name))
            {
                Character character = new Character(name);
                character.AccountId = c.Account.AccountId;
                character.CharId = Database.Instance.GetNewCharacterId();
                character.Level = 1;

                character.HP = 50;
                character.MaxHP = 50;
                character.MP = 5;
                character.MaxMP = 5;

                character.Face = p.ReadInt();
                character.Hair = p.ReadInt() + p.ReadInt();
                character.SkinColor = (byte)p.ReadInt();
                
                int top = p.ReadInt();
                int bottom = p.ReadInt();
                int shoes = p.ReadInt();
                int weapon = p.ReadInt();

                character.Inventory[InventorySlot.Equipped].Items.Add(-5, new Item(top));
                character.Inventory[InventorySlot.Equipped].Items.Add(-6, new Item(bottom));
                character.Inventory[InventorySlot.Equipped].Items.Add(-7, new Item(shoes));
                character.Inventory[InventorySlot.Equipped].Items.Add(-11, new Item(weapon));

                character.Str = p.ReadByte();
                character.Dex = p.ReadByte();
                character.Int = p.ReadByte();
                character.Luk = p.ReadByte();

                c.Characters.Add(character);
                Database.Instance.Save<Character>(Database.Characters, character);

                using(OutPacket packet = new OutPacket(SendOps.CreateNewCharacterResult))
                {
                    packet.WriteByte();
                    
                    PacketCreator.Helper.AddCharacterData(packet, character);
                    PacketCreator.Helper.AddCharaterLook(packet, character);
                    packet.WriteBool(false); //rank

                    packet.WriteZero(24);
                    c.Send(packet);
                }
            }
            else
            {
                using (OutPacket packet = new OutPacket(SendOps.CheckDuplicatedIDResult))
                {
                    packet.WriteMapleString(name);
                    packet.WriteBool(true);
                    c.Send(packet);
                }
            }
        }
        public static void OnDeleteCharacter(MapleClient c,InPacket p)
        {
            p.Skip(4);
            int charId = p.ReadInt();

            Character target = c.Characters.FindOne((chr) => chr.CharId == charId);

            if (target != default(Character))
            {
                Database.Instance.DeleteCharacter(target);
                c.Characters.Remove(target);

                  using(OutPacket packet = new OutPacket(SendOps.DeleteCharacterResult))
                  {
                      packet.WriteInt(charId);
                      packet.WriteByte();
                      c.Send(packet);
                  }
            }
            else
            {
                c.Close();
            }
        }
    }
}
