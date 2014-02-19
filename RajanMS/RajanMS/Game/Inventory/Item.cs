using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RajanMS.Game.Inventory
{
    public class Item
    {
        public ObjectId Id { get; set; }

        public int ItemId { get; set; }

        public Item()
        {

        }

        public Item(int id)
        {
            ItemId = id;
        }
    }
}
