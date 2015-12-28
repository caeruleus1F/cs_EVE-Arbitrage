using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_EVE_Arbitrage
{
    public class SellOrder : MarketOrder
    {
        bool _islowerthanbuyorder = false;

        public SellOrder(string stationname, decimal price, int remainingvolume, int minvolume = 1)
            : base(stationname, price, remainingvolume, minvolume)
        {

        }

        public bool IsLowerThanBuyOrder
        {
            get { return _islowerthanbuyorder; }
            set { _islowerthanbuyorder = value; }
        }
    }
}
