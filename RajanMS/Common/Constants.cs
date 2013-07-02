using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Constants
    {
        public static readonly short MajorVersion = 111;
        public static readonly string MinorVersion = "1";
        public static readonly byte Locale = 8; //GMS

        public static readonly string ConfigName = "Server.ini";

        public static readonly byte[] RIV = new byte[] { 0x52, 0x61, 0x6A, 0x61 }; //Raja
        public static readonly byte[] SIV = new byte[] { 0x6E, 0x52, 0x30, 0x58 }; //nR0X
    }
}
