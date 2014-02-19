using RajanMS.Game;
using RajanMS.Game.Inventory;
using RajanMS.IO;
using RajanMS.Tools;
using System;

namespace RajanMS.Packets
{
    static class PacketCreator
    {
        public static byte[] Handshake()
        {
            using (OutPacket p = new OutPacket())
            {
                p.WriteShort(14);
                p.WriteShort(Constants.MajorVersion);
                p.WriteMapleString(Constants.MinorVersion);
                p.WriteBytes(Constants.RIV);
                p.WriteBytes(Constants.SIV);
                p.WriteByte(Constants.Locale);
                return p.ToArray();
            }
        }

        public static class Helper
        {
            public static void AddCharacterData(OutPacket p, Character c)
            {
                p.WriteInt(c.CharId);
                p.WritePaddedString(c.Name, 13);
                p.WriteByte(c.Gender);
                p.WriteByte(c.SkinColor);
                p.WriteInt(c.Face);
                p.WriteInt(c.Hair);
                p.WriteLong(); //pet
                p.WriteByte(c.Level);
                p.WriteShort(c.Job);
                p.WriteShort(c.Str);
                p.WriteShort(c.Dex); 
                p.WriteShort(c.Int); 
                p.WriteShort(c.Luk); 
                p.WriteShort(c.HP); 
                p.WriteShort(c.MaxHP); 
                p.WriteShort(c.MP); 
                p.WriteShort(c.MaxMP); 
                p.WriteShort(c.AP);
                p.WriteShort(c.SP);
                p.WriteInt(c.EXP); 
                p.WriteShort(c.Fame); 
                p.WriteInt(c.MapId);
                p.WriteByte(c.SpawnPoint);
                p.WriteZero(16);
            }

            public static void AddCharaterLook(OutPacket p,Character c)
            {
                foreach(var kvp in c.Inventory[InventorySlot.Equipped].Items)
                {
                    if (kvp.Key < -100)
                        continue;

                    p.WriteByte((byte)Math.Abs(kvp.Key));
                    p.WriteInt(kvp.Value.ItemId);
                }

                p.WriteByte();

                foreach (var kvp in c.Inventory[InventorySlot.Equipped].Items)
                {
                    if (kvp.Key > -100)
                        continue;

                    p.WriteByte((byte)Math.Abs(kvp.Key));
                    p.WriteInt(kvp.Value.ItemId);
                }

                p.WriteByte();
            }

            public static void AddInventory(OutPacket p,Character c)
            {
                p.WriteInt(c.Meso);
                //cash equips
                p.WriteByte(0);
                //reg equips
                p.WriteByte(0);

                for (int i = InventorySlot.Equip; i < InventorySlot.Equipped; i++)
                {
                    p.WriteByte(c.Inventory[i].SlotLimit);
                    //invetory items
                    p.WriteByte(0);
                }
            }
        }
    }
}