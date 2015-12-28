using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cs_EVE_Arbitrage
{
    public abstract class MarketOrder
    {
        string _stationname = null;
        decimal _price = 0M;
        int _remainingvolume = 0;
        int _minvolume = 0;

        public MarketOrder(string stationname, decimal price, int remainingvolume, int minvolume = 1)
        {
            _stationname = stationname;
            _price = price;
            _remainingvolume = remainingvolume;
            _minvolume = minvolume;
        }

        public string StationName
        {
            get { return _stationname; }
            set { _stationname = value; }
        }

        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public int RemainingVolume
        {
            get { return _remainingvolume; }
            set { _remainingvolume = value; }
        }

        public int MinVolume
        {
            get { return _minvolume; }
            set { _minvolume = value; }
        }
    }
}
