using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace cs_EVE_Arbitrage
{
    class EVECentralInterfacer
    {
        string _sourceid = null;
        string _destinationid = null;
        List<MarketableItem> _marketableitems = null;

        public EVECentralInterfacer(string source, string destination, List<MarketableItem> marketableitems)
        {
            _sourceid = source;
            _destinationid = destination;
            _marketableitems = marketableitems;
        }

        public void Begin()
        {
            
        }
    }
}
