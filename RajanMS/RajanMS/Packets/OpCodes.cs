using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajanMS.Packets
{
    static class RecvOps
    {
        public static readonly short Validate = 0x0014;
        public static readonly short LoginPassword = 0x0015;
        public static readonly short ServerlistRequest = 0x001F;
        public static readonly short StartHackshield = 0x0038;
        public static readonly short ClientException = 0x002F;
        public static readonly short RequestCharlist = 0x0019;
        public static readonly short ServerStatusRequest = 0x001A;
        public static readonly short Pong = 0x002E;
       
    }

    static class SendOps
    {
        public static readonly short LoginResponse = 0x0000;
        public static readonly short ServerStatus = 0x0003;
        public static readonly short Serverlist = 0x000A;
        public static readonly short Charlist = 0x000B;
        public static readonly short Ping = 0x0011;
    }
}
