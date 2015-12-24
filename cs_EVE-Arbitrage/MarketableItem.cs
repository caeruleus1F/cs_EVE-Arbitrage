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
        float _sellorderlowest = 0F;
        float _buyorderhighest = 0F;
        string _sellorderstation = null;
        string _buyorderstation = null;

        public MarketableItem()
        {

        }

        public MarketableItem(string typeid, string typename, float volume)
        {
            _typeid = typeid;
            _typename = typename;
            _volume = volume;
        }

        string TypeID
        {
            get { return _typeid; }
            set { _typeid = value; }
        }

        string TypeName
        {
            get { return _typename; }
            set { _typename = value; }
        }

        float Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        float SellOrderLowest
        {
            get { return _sellorderlowest; }
            set { _sellorderlowest = value; }
        }

        float BuyOrderHighest
        {
            get { return _buyorderhighest; }
            set { _buyorderhighest = value; }
        }

        string SellOrderStation
        {
            get { return _sellorderstation; }
            set { _sellorderstation = value; }
        }

        string BuyOrderStation
        {
            get { return _buyorderstation; }
            set { _buyorderstation = value; }
        }

    }
}
