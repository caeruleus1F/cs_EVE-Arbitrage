using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace cs_EVE_Arbitrage
{
    public class MarketableItem
    {
        string _typeid = null;
        string _typename = null;
        float _volume = 0F;
        decimal _sellorderlowest = 0M;
        decimal _buyorderhighest = 0M;
        decimal _profitperm3 = 0M;
        List<BuyOrder> _buyorders = new List<BuyOrder>();
        List<SellOrder> _sellorders = new List<SellOrder>();

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

        public decimal ProfitPerM3
        {
            get { return _profitperm3; }
            set { _profitperm3 = value; }
        }

        public List<BuyOrder> BuyOrders
        {
            get { return _buyorders; }
            set { _buyorders = value; }
        }

        public List<SellOrder> SellOrders
        {
            get { return _sellorders; }
            set { _sellorders = value; }
        }
    }
}
