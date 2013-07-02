using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.IO;

namespace RajanMS.Packets.Handlers
{
    static class LoginHandlers
    {
        public static void HandleValidate(MapleClient c, InPacket p)
        {
            //14-00-08-6F-00-01-00

            byte locale = p.ReadByte();
            short major = p.ReadShort();
            string minor = p.ReadShort().ToString();

            if (locale != Constants.Locale || major != Constants.MajorVersion || minor != Constants.MinorVersion)
                c.Close();
        }
        public static void HandleLoginPassword(MapleClient c, InPacket p)
        {
        }
    }
}
