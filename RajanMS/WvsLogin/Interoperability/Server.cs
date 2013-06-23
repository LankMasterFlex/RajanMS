using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Operations.GameServer;

namespace WvsLogin.Interoperability
{
    internal abstract class Server
    {
        public ServerType ServerType { get; set; }
        public int Dataload { get; set; }
    }
}
