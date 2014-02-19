namespace RajanMS.Packets
{
    public static class RecvOps
    {
        public const byte

            //WvsLogin
            //CClientSocket::ProcessPacket
            CheckPassword = 1,
            SelectWorld = 2,
            CheckUserLimit = 3,
            SelectCharacter = 4,
            MigrateIn = 5,
            CheckDuplicatedID = 6,
            CreateNewCharacter = 7,
            DeleteCharacter = 8;
            
    }
    public static class SendOps
    {
        public const byte

            //v40b
            //CLogin::OnPacket
            CheckPasswordResult = 1,
            CheckUserLimitResult = 2,
            WorldInformation = 3,
            SelectWorldResult = 4,
            SelectCharacterResult = 5,
            CheckDuplicatedIDResult = 6,
            CreateNewCharacterResult = 7,
            DeleteCharacterResult = 8,

            //CWvsContext::OnPacket
            InventoryOperation = 0x12,
            InventoryGrow = 0x13,
            StatChanged = 0x14,
            OnForcedStatSet = 0x15,
            BroadcastMsg = 0x23,

            //CStage::OnPacket
            SetField = 0x26,
            SetCashShop = 0x27;
    }
}
