using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajanMS.Game
{
    public enum InventoryType
    {
        //UNDEFINED = 0,
        EQUIP = 1,
        USE = 2,
        SETUP = 3,
        ETC = 4,
        CASH = 5,
        EQUIPPED = -1,
    }

    public class Inventory
    {
        private Dictionary<byte, string> m_items; //string should be equip!

        public byte SlotLimit { get; private set; }
        public InventoryType InventoryType { get; private set; }

        public Inventory(InventoryType invtype, byte slotLimit = 96)
        {
            m_items = new Dictionary<byte, string>();
            SlotLimit = slotLimit;
            InventoryType = invtype;
        }




    }
}
