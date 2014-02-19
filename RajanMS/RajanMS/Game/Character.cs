using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RajanMS.Game.Inventory;

namespace RajanMS.Game
{
    public sealed class Character
    {
        public ObjectId Id { get; set; }

        public Character()
        {

        }

        public Character(string name)
        {
            Name = name;

            Inventory = new PlayerInventory[6];

            for (int i = 0; i < Inventory.Length; i++)
                Inventory[i] = new PlayerInventory();

            TeleportRock = new int[5];

            for (int i = 0; i < TeleportRock.Length; i++)
                TeleportRock[i] = 999999999;
        }

        public int[] TeleportRock {get; set;}

        public PlayerInventory[] Inventory { get; private set; }

        public int AccountId { get; set; }

        public int CharId { get; set; }

        public int Meso { get; set; }

        public string Name { get; set; }
        public byte Gender { get; set; }
        public byte SkinColor { get; set; }
        public int Face { get; set; }
        public int Hair { get; set; }

        public byte Level { get; set; }
        public short Job { get; set; }
        public short Str { get; set; }
        public short Dex { get; set; }
        public short Int { get; set; }
        public short Luk { get; set; }
        public short HP { get; set; }
        public short MaxHP { get; set; }
        public short MP { get; set; }
        public short MaxMP { get; set; }
        public short AP { get; set; }
        public short SP { get; set; }
        public int EXP { get; set; }
        public short Fame { get; set; }
        public int MapId { get; set; }
        public byte SpawnPoint { get; set; }
    }
}
