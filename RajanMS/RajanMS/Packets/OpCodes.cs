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
        public static readonly short ViewAllCharacters = 0x0021;
        public static readonly short ViewWorldInfo = 0x0023;
        public static readonly short Migrate = 0x0028;
        public static readonly short CheckDuplicateName = 0x0029;
        public static readonly short CreateCharacter = 0x002A;
        public static readonly short DeleteCharacter = 0x002D;
        public static readonly short Pong = 0x002E;
        public static readonly short ClientException = 0x002F;
        public static readonly short SelectCharacterSetPIC = 0x0032;
        public static readonly short SelectCharacter = 0x0033;
        public static readonly short ViewAllSelectCharacter = 0x0035;
        public static readonly short StartHackshield = 0x0038;
    }

    static class SendOps
    {
        public static readonly short CheckPassword = 0x0000;
        public static readonly short CheckUserLimit = 0x0003;
        public static readonly short ViewAllChar = 0x0008;
        public static readonly short WorldInformation = 0x000A;
        public static readonly short SelectWorldResult = 0x000B;
        public static readonly short ServerIP = 0x000C;
        public static readonly short CheckDuplicatedID = 0x000D;
        public static readonly short CreateNewCharacter = 0x000E;
        public static readonly short DeleteCharacter = 0x000F;
        public static readonly short Ping = 0x0011;
        public static readonly short CheckSPW = 0x1F;
        public static readonly short LoadStage = 0x00B9;
    }
}
