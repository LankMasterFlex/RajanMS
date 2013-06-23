namespace Common.Operations
{
    namespace GameServer
    {
        public enum ServerType : byte
        {
            None,
            WvsLogin,
            WvsGame,
            WvsCenter
        }
        public enum OpCodes : short
        {
            Identify,
            Dataload,
        }
    }
}
