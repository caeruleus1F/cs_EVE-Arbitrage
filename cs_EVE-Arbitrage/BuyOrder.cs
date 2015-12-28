using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_EVE_Arbitrage
{
    public class BuyOrder : MarketOrder
    {
        bool _ishigherthansellorder = false;

        public BuyOrder(string stationname, decimal price, int remainingvolume, int minvolume = 1)
            : base(stationname, price, remainingvolume, minvolume)
        {

        }

        public bool IsHigherThanSellOrder
        {
            get { return _ishigherthansellorder; }
            set { _ishigherthansellorder = value; }
        }
    }
}
