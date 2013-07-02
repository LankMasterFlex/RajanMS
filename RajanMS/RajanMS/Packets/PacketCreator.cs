using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.IO;

namespace RajanMS.Packets
{
    static class PacketCreator
    {
        public static byte[] Handshake()
        {
            //lol this should just be a static byte[]
            using (OutPacket p = new OutPacket(14, 16))
            {
                p.WriteShort(Constants.MajorVersion);
                p.WriteMapleString(Constants.MinorVersion);
                p.WriteBytes(Constants.RIV);
                p.WriteBytes(Constants.SIV);
                p.WriteByte(Constants.Locale);
                return p.ToArray();
            }
        }

    }
}
