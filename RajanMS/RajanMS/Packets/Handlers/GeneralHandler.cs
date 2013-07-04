using RajanMS.IO;

namespace RajanMS.Packets.Handlers
{
    static class GeneralHandler
    {
        public static void HandleNothing(MapleClient c, InPacket p)
        {
        }

        public static void HandleClientException(MapleClient c, InPacket p)
        {
            string error = p.ReadMapleString();
            MainForm.Instance.Log("[{0}] Exception : {1}", c.Label, error);
        }
    }
}
