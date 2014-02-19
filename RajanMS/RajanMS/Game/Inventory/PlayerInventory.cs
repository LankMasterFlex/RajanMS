using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RajanMS.Game.Inventory
{
    public class PlayerInventory
    {
        public ObjectId Id { get; set; }

        public Dictionary<sbyte, Item> Items
        {
            get;
            private set;
        }

        public byte SlotLimit
        {
            get;
            private set;
        }

        public PlayerInventory() : this(96)
        {

        }

        public PlayerInventory(byte slotLimit)
        {
            Items = new Dictionary<sbyte, Item>(slotLimit);
            SlotLimit = slotLimit;
        }
    }
}
