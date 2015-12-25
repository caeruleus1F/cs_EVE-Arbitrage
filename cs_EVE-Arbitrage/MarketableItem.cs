using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_EVE_Arbitrage
{
    class MarketableItem
    {
        string _typeid = null;
        string _typename = null;
        float _volume = 0F;
        decimal _sellorderlowest = 0M;
        decimal _buyorderhighest = 0M;
        string _sellorderstation = null;
        string _buyorderstation = null;
        decimal _profitperm3 = 0M;

        public MarketableItem()
        {

        }

        public MarketableItem(string typeid, string typename, float volume)
        {
            _typeid = typeid;
            _typename = typename;
            _volume = volume;
        }

        public string TypeID
        {
            get { return _typeid; }
            set { _typeid = value; }
        }

        public string TypeName
        {
            get { return _typename; }
            set { _typename = value; }
        }

        public float Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        public decimal SellOrderLowest
        {
            get { return _sellorderlowest; }
            set { _sellorderlowest = value; }
        }

        public decimal BuyOrderHighest
        {
            get { return _buyorderhighest; }
            set { _buyorderhighest = value; }
        }

        public string SellOrderStation
        {
            get { return _sellorderstation; }
            set { _sellorderstation = value; }
        }

        public string BuyOrderStation
        {
            get { return _buyorderstation; }
            set { _buyorderstation = value; }
        }

        public decimal ProfitPerM3
        {
            get { return _profitperm3; }
            set { _profitperm3 = value; }
        }

    }
}
