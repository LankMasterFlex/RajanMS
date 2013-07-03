namespace RajanMS.Packets
{
    static class RecvOps
    {
        public static readonly short Validate = 0x0014;
        public static readonly short LoginPassword = 0x0015;
        public static readonly short WorldSelect = 0x0019;
        public static readonly short Unk1 = 0x0018;
        public static readonly short CheckUserLimit = 0x001A;
        public static readonly short WorldInfoRequest = 0x001F;
        public static readonly short WorldInfoRequest2 = 0x0020;
        public static readonly short CheckDuplicateName = 0x0029;
        public static readonly short CreateCharacter = 0x002A;
        public static readonly short DeleteCharacter = 0x002D;
        public static readonly short Pong = 0x002E;
        public static readonly short ClientException = 0x002F;
        public static readonly short SelectCharacterSetPIC = 0x0032;
        public static readonly short SelectCharacter = 0x0033;
        public static readonly short StartHackshield = 0x0038;
    }

    static class SendOps
    {
        public static readonly short LoginResponse = 0x0000;
        public static readonly short ServerStatus = 0x0003;
        public static readonly short Worldlist = 0x000A;
        public static readonly short Charlist = 0x000B;
        public static readonly short NameAvailable = 0x000D;
        public static readonly short CreateCharacter = 0x000E;
        public static readonly short DeleteCharacter = 0x000F;
        public static readonly short Ping = 0x0011;
        public static readonly short CheckPIC = 0x1F;
    }
}
