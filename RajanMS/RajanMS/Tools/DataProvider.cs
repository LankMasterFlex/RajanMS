using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using reNX;
using reNX.NXProperties;

namespace RajanMS.Tools
{
    sealed class DataProvider
    {
        public NXFile EtcNX { get; private set; }

        public void Cache()
        {
            EtcNX = new NXFile("NX\\etc.nx");
        }

    }
}
