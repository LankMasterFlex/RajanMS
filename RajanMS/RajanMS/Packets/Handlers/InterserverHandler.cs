using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RajanMS.IO;
using RajanMS.Servers;

namespace RajanMS.Packets.Handlers
{
    class InterserverHandler
    {
        public static void HandleMigrate(MapleClient c, InPacket p)
        {
            int charId = p.ReadInt();
            p.Skip(18);
            long sessionId = p.ReadLong();

            if (MasterServer.Instance.Worlds[c.World].EligableMigration(charId,sessionId))
            {
                c.SessionId = sessionId;
                c.Character = MasterServer.Instance.Database.GetCharacter(charId);
                c.Account = MasterServer.Instance.Database.GetAccount(c.Character.AccountId);

                if (c.Account.LoggedIn) //i dont know what to do, should never happpen though
                {
                    c.Close(); return;
                }

                c.Account.LoggedIn = true;

                MainForm.Instance.Log("[{0}] Migration success", c.Label); //debugging
                //c.WritePacket(FieldPacketCreator.LoadInitialStage(c, c.Character.MapId, 0));
            }
            else
            {
                MainForm.Instance.Log("[{0}] Migration failed",c.Label);
                c.Close(); //nop nop nop
            }
        }
    }
}
