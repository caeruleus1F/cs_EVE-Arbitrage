using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Net;
using System.Xml;

namespace cs_EVE_Arbitrage
{
    class Backend
    {
        string _sourceid = null;
        string _destinationid = null;
        string _locationid = null;
        volatile List<MarketableItem> _marketableitems = null;
        Form1 _f = null;

        string _basemarketstaturl = "http://api.eve-central.com/api/marketstat?";
        string _basequicklookurl = "http://api.eve-central.com/api/quicklook?";

        int _requests = 0;
        int _serverresponses = 0;

        int _marketstatsellresponses = 0;
        bool _marketstatsellcomplete = false;
        int _marketstatbuyresponses = 0;
        bool _marketstatbuycomplete = false;
        int _quicklooksellresponses = 0;
        bool _quicklooksellcomplete = false;
        int _quicklookbuyresponses = 0;
        bool _quicklookbuycomplete = false;

        public delegate void DisplayDelegate(List<MarketableItem> marketableitems);
        public delegate void StatusDelegate(string text);

        bool _resolvestationnames = false;
        int _waitmilli = 100;
        int _maxurilength = 1500;

        public Backend(Form1 f, string source, string destination, List<MarketableItem> marketableitems)
        {
            _f = f;
            _sourceid = source;
            _destinationid = destination;
            _marketableitems = marketableitems;
        }

        public void Begin()
        {
            //TestTypeIDs();

            // get marketstat sellmin from source
            GetMarketstatSellMinData();

            // get marketstat buymax from destination
            GetMarketstatBuyMaxData();

            // discard marketables with no arbitrage opportunities
            DiscardUnprofitableTrades();

            // calculate profit per volume
            CalcProfitPerVolume();

            if (_f.ResolveStationNames())
            {
                // get quicklook buymax from marketables
                GetQuicklookBuyData();

                // get quicklook sellmin from marketables
                GetQuicklookSellData();

                TrimMarketOrders();
            }

            // display the results
            OrganizeByDescendingPPV();
            DisplayResults();
        }

        private void GetQuicklookSellData()
        {
            List<string> URIs = null;

            lock (new object())
            {
                _quicklooksellresponses = 0;
                _locationid = _sourceid;
                URIs = AssembleQuicklookURIs();
                _requests = URIs.Count;
            }

            try
            {
                WebClient[] w = new WebClient[_requests];
                for (int i = 0; i < _requests; ++i)
                {
                    w[i] = new WebClient();
                    w[i].DownloadStringCompleted += w_QuicklookSellData;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                }

                while (_quicklooksellresponses < _requests - 1)
                {
                    Thread.Sleep(1);
                }

                UpdateStatus("Source station data retrieved.\n");
            }
            catch (Exception ex)
            {
            }
        }

        private void w_QuicklookSellData(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(e.Result);
                string typeid = xmldoc.SelectSingleNode("/evec_api/quicklook/item").InnerText;
                bool match = false;

                for (int i = 0; i < _marketableitems.Count && !match; ++i)
                {
                    if (_marketableitems[i].TypeID.Equals(typeid))
                    {
                        match = true;
                        MarketableItem m = _marketableitems[i];

                        foreach (XmlNode n in xmldoc.SelectNodes("/evec_api/quicklook/sell_orders/order"))
                        {
                            string stationname = n.SelectSingleNode("station_name").InnerText;
                            decimal price = Convert.ToDecimal(n.SelectSingleNode("price").InnerText);
                            int remainingvolume = Convert.ToInt32(n.SelectSingleNode("vol_remain").InnerText);
                            int minvolume = Convert.ToInt32(n.SelectSingleNode("min_volume").InnerText);
                            m.SellOrders.Add(new SellOrder(stationname, price, remainingvolume, minvolume));
                        }
                    }
                }

                lock (new object())
                {
                    ++_quicklooksellresponses;
                    UpdateStatus("Source Station Data Retrieved (" + _quicklooksellresponses + " of " + _requests + ")");
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void GetQuicklookBuyData()
        {
            List<string> URIs = null;

            lock (new object())
            {
                _quicklookbuyresponses = 0;
                _locationid = _destinationid;
                URIs = AssembleQuicklookURIs();
                _requests = URIs.Count;
            }

            try
            {
                EVECentralInterfacer[] w = new EVECentralInterfacer[_requests];
                for (int i = 0; i < _requests; ++i)
                {
                    w[i] = new EVECentralInterfacer();
                    w[i].DownloadStringCompleted += w_QuicklookBuyData;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                }

                while (_quicklookbuyresponses < _requests - 1)
                {
                    Thread.Sleep(1);
                }

                UpdateStatus("Destination station data retrieved.\n");
            }
            catch (Exception ex)
            {
            }
        }

        private void w_QuicklookBuyData(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(e.Result);
                string typeid = xmldoc.SelectSingleNode("/evec_api/quicklook/item").InnerText;
                bool match = false;

                for (int i = 0; i < _marketableitems.Count && !match; ++i)
                {
                    if (_marketableitems[i].TypeID.Equals(typeid))
                    {
                        match = true;
                        MarketableItem m = _marketableitems[i];

                        foreach (XmlNode n in xmldoc.SelectNodes("/evec_api/quicklook/buy_orders/order"))
                        {
                            string stationname = n.SelectSingleNode("station_name").InnerText;
                            decimal price = Convert.ToDecimal(n.SelectSingleNode("price").InnerText);
                            int remainingvolume = Convert.ToInt32(n.SelectSingleNode("vol_remain").InnerText);
                            int minvolume = Convert.ToInt32(n.SelectSingleNode("min_volume").InnerText);
                            m.BuyOrders.Add(new BuyOrder(stationname, price, remainingvolume, minvolume));
                        }
                    }
                }

                lock (new object())
                {
                    ++_quicklookbuyresponses;
                    UpdateStatus("Destination Station Data Retrieved (" + _quicklookbuyresponses + " of " + _requests + ")");
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void GetMarketstatSellMinData()
        {
            List<string> URIs = null;

            lock (new object())
            {
                _marketstatsellresponses = 0;
                _marketstatsellcomplete = false;
                _locationid = _sourceid;
                URIs = AssembleMarketstatURIs();
                _requests = URIs.Count;
            }

            try
            {
                EVECentralInterfacer[] w = new EVECentralInterfacer[_requests];
                for (int i = 0; i < _requests; ++i)
                {
                    w[i] = new EVECentralInterfacer();
                    w[i].DownloadStringCompleted += w_MarketstatSellMin;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                }

                while (!_marketstatsellcomplete)
                {
                    Thread.Sleep(1);
                }

                // for whatever reason, not all items with no sell orders
                // are removed from the list of marketables.
                for (int i = _marketableitems.Count - 1; i >= 0; --i)
                {
                    if (_marketableitems[i].SellOrderLowest == 0M)
                    {
                        _marketableitems.RemoveAt(i);
                    }
                }

                UpdateStatus("Source market sell min data complete.\n");
            }
            catch (Exception ex)
            {
            }
        }

        private void w_MarketstatSellMin(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(e.Result);

                foreach (XmlNode n in xmldoc.SelectNodes("/evec_api/marketstat/type"))
                {
                    string typeid = n.Attributes[0].Value;
                    decimal sellmin = Convert.ToDecimal(n.SelectSingleNode("sell/min").InnerText);

                    lock (new object())
                    {
                        _marketableitems[_marketableitems.IndexOf(FindMarketableByTypeID(typeid))].SellOrderLowest = sellmin;
                    }
                }

                lock (new object())
                {
                    ++_marketstatsellresponses;
                    UpdateStatus("Sellmin Data Retrieved (" + _marketstatsellresponses + " of " + _requests + ")");

                    if (_marketstatsellresponses == _requests - 1)
                    {
                        _marketstatsellcomplete = true;
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void GetMarketstatBuyMaxData()
        {
            List<string> URIs = null;

            lock (new object())
            {
                _marketstatbuyresponses = 0;
                _marketstatbuycomplete = false;
                _locationid = _destinationid;
                URIs = AssembleMarketstatURIs();
                _requests = URIs.Count;
            }

            try
            {
                EVECentralInterfacer[] w = new EVECentralInterfacer[_requests];
                for (int i = 0; i < _requests; ++i)
                {
                    w[i] = new EVECentralInterfacer();
                    w[i].DownloadStringCompleted += w_MarketstatBuyMax;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                }

                while (!_marketstatbuycomplete)
                {
                    Thread.Sleep(1);
                }

                // for whatever reason, not all items with no sell orders
                // are removed from the list of marketables.
                for (int i = _marketableitems.Count - 1; i >= 0; --i)
                {
                    if (_marketableitems[i].BuyOrderHighest == 0M)
                    {
                        _marketableitems.RemoveAt(i);
                    }
                }

                UpdateStatus("Destination market buy max data complete...\n");
            }
            catch (Exception ex)
            {
            }
        }

        private void w_MarketstatBuyMax(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(e.Result);

                foreach (XmlNode n in xmldoc.SelectNodes("/evec_api/marketstat/type"))
                {
                    string typeid = n.Attributes[0].Value;
                    decimal buymax = Convert.ToDecimal(n.SelectSingleNode("buy/max").InnerText);

                    lock (new object())
                    {
                        _marketableitems[_marketableitems.IndexOf(FindMarketableByTypeID(typeid))].BuyOrderHighest = buymax;
                    }
                }

                lock (new object())
                {
                    ++_marketstatbuyresponses;
                    UpdateStatus("Buymax Data Retrieved (" + _marketstatbuyresponses + " of " + _requests + ")");

                    if (_marketstatbuyresponses == _requests - 1)
                    {
                        _marketstatbuycomplete = true;
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        /***********************************************************************
         * 
         * SUPPORTING FUNCTIONS
         * 
         ***********************************************************************/

        private void TrimMarketOrders()
        {
            // identify market orders that are profitable
            for (int i = 0; i < _marketableitems.Count; ++i)
            {
                MarketableItem m = _marketableitems[i];

                foreach (BuyOrder b in m.BuyOrders)
                {
                    foreach (SellOrder s in m.SellOrders)
                    {
                        if (b.Price * .98M > s.Price)
                        {
                            b.IsHigherThanSellOrder = true;
                            s.IsLowerThanBuyOrder = true;
                        }
                    }
                }
            }

            // trim sell orders that are too high
            for (int i = 0; i < _marketableitems.Count; ++i)
            {
                MarketableItem m = _marketableitems[i];

                for (int j = m.SellOrders.Count - 1; j >= 0; --j)
                {
                    if (m.SellOrders[j].IsLowerThanBuyOrder == false)
                    {
                        m.SellOrders.RemoveAt(j);
                    }
                }
            }

            // trim buy orders that are too low
            for (int i = 0; i < _marketableitems.Count; ++i)
            {
                MarketableItem m = _marketableitems[i];

                for (int j = m.BuyOrders.Count - 1; j >= 0; --j)
                {
                    if (m.BuyOrders[j].IsHigherThanSellOrder == false)
                    {
                        m.BuyOrders.RemoveAt(j);
                    }
                }
            }

            // if an item has no remaining buy or sell orders, remove
            for (int i = _marketableitems.Count - 1; i >= 0; --i)
            {
                MarketableItem m = _marketableitems[i];

                if (m.BuyOrders.Count == 0 || m.SellOrders.Count == 0)
                {
                    _marketableitems.Remove(m);
                }
            }

            // bubblesort sell orders by descending
            foreach (MarketableItem m in _marketableitems)
            {
                for (int i = 0; i < m.SellOrders.Count - 1; ++i)
                {
                    for (int j = 1; j < m.SellOrders.Count - i; ++j)
                    {

                        if (m.SellOrders[j - 1].Price < m.SellOrders[j].Price)
                        {
                            SellOrder temp = m.SellOrders[j - 1];
                            m.SellOrders[j - 1] = m.SellOrders[j];
                            m.SellOrders[j] = temp;
                        }
                    }
                }
            }

            // bubblesort buy orders by descending
            foreach (MarketableItem m in _marketableitems)
            {
                for (int i = 0; i < m.BuyOrders.Count - 1; ++i)
                {
                    for (int j = 1; j < m.BuyOrders.Count - i; ++j)
                    {

                        if (m.BuyOrders[j - 1].Price < m.BuyOrders[j].Price)
                        {
                            BuyOrder temp = m.BuyOrders[j - 1];
                            m.BuyOrders[j - 1] = m.BuyOrders[j];
                            m.BuyOrders[j] = temp;
                        }
                    }
                }
            }


        }

        private void TestTypeIDs()
        {
            WebClient w = new WebClient();
            w.Proxy = null;
            XmlDocument xmldoc = new XmlDocument();
            _locationid = "30000142";
            int count = 0;
            List<string> URIs = null;

            URIs = AssembleMarketstatURIs();
            _requests = URIs.Count;

            foreach (string uri in URIs)
            {
                FindBadTypeIDs(uri);
            }
        }

        private void FindBadTypeIDs(string uri)
        {
            WebClient web = new WebClient();
            web.Proxy = null;
            XmlDocument xmldoc = new XmlDocument();
            string[] typeids = uri.Remove(0, uri.LastIndexOf('=') + 1).Split(',');
            int length = typeids.Length;

            try
            {
                xmldoc.LoadXml(web.DownloadString(uri));
            }
            catch (Exception ex)
            {
                if (length > 1)
                {
                    StringBuilder sb = CreateBaseMarketstatURL();

                    for (int i = 0; i < length / 2; ++i)
                    {
                        sb.Append(typeids[i]);

                        if (i < (length / 2) - 1)
                        {
                            sb.Append(',');
                        }
                    }

                    FindBadTypeIDs(sb.ToString());
                    sb.Clear();

                    sb = CreateBaseMarketstatURL();

                    for (int i = length / 2; i < length; ++i)
                    {
                        sb.Append(typeids[i]);

                        if (i < length - 1)
                        {
                            sb.Append(',');
                        }
                    }

                    FindBadTypeIDs(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    using (System.IO.StreamWriter wr = new System.IO.StreamWriter("badtypeids.txt", true))
                    {
                        wr.WriteLine(typeids[0]);
                        wr.Close();
                    }
                }
            }
        }

        private List<string> AssembleQuicklookURIs()
        {
            List<string> uris = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (MarketableItem m in _marketableitems)
            {
                sb = CreateBaseQuicklookURL();
                sb.Append(m.TypeID);
                uris.Add(sb.ToString());
                sb.Clear();
            }

            return uris;
        }

        private List<string> AssembleMarketstatURIs()
        {
            List<string> uris = new List<string>();
            StringBuilder sb = new StringBuilder();
            int typeidlength = 0;

            sb = CreateBaseMarketstatURL();

            for (int i = 0; i < _marketableitems.Count; ++i)
            {
                typeidlength = _marketableitems[i].TypeID.Length;
            
                if (typeidlength + sb.Length + 1 < _maxurilength)
                {
                    sb.Append(_marketableitems[i].TypeID);
            
                    if (i < _marketableitems.Count - 1)
                    {
                        sb.Append(',');
                    }
                }
                else
                {
                    if (sb[sb.Length - 1].Equals(','))
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
            
                    uris.Add(sb.ToString());
                    sb.Clear();

                    sb = CreateBaseMarketstatURL();

                    sb.Append(_marketableitems[i].TypeID);

                    if (i < _marketableitems.Count - 1)
                    {
                        sb.Append(',');
                    }
                }
            }
            
            if (sb.Length > 68)
            {
                uris.Add(sb.ToString());
            }

            return uris;
        }

        private StringBuilder CreateBaseQuicklookURL()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_basequicklookurl);

            if (_locationid[0].Equals('1'))
            {
                sb.Append("regionlimit=");
            }
            else if (_locationid[0].Equals('3'))
            {
                sb.Append("usesystem=");
            }

            sb.Append(_locationid).Append("&typeid=");

            return sb;
        }

        private void DiscardUnprofitableTrades()
        {
            float buyprice = 0F;
            float sellprice = 0F;
            for (int i = _marketableitems.Count - 1; i >= 0; --i)
            {
                buyprice = Convert.ToSingle(_marketableitems[i].BuyOrderHighest) * 0.98F;
                sellprice = Convert.ToSingle(_marketableitems[i].SellOrderLowest);

                if (sellprice > buyprice)
                {
                    _marketableitems.RemoveAt(i);
                }
            }

            UpdateStatus("Unprofitable trades discarded...\n");
        }

        private void CalcProfitPerVolume()
        {
            float buy = 0F;
            float sell = 0F;
            float difference = 0F;

            for (int i = 0; i < _marketableitems.Count; ++i)
            {
                buy = Convert.ToSingle(_marketableitems[i].BuyOrderHighest) * .98F;
                sell = Convert.ToSingle(_marketableitems[i].SellOrderLowest);
                difference = buy - sell;
                _marketableitems[i].ProfitPerM3 = Convert.ToDecimal(difference / _marketableitems[i].Volume);
            }

            UpdateStatus("Profit per volume calculated...\n");
        }

        private void OrganizeByDescendingPPV()
        {
            // bubble sort
            for(int i = 0; i < _marketableitems.Count - 1; ++i)
            {
                for(int j = 1; j < _marketableitems.Count - i; ++j)
                {
                    if(_marketableitems[j-1].ProfitPerM3 < _marketableitems[j].ProfitPerM3){
                        MarketableItem temp = _marketableitems[j-1];
                        _marketableitems[j-1] = _marketableitems[j];
                        _marketableitems[j] = temp;
                    }
                }
            }
        }

        private void UpdateStatus(string text)
        {
            if (_f.rtbDisplay.InvokeRequired)
            {
                _f.rtbDisplay.Invoke(new StatusDelegate(_f.UpdateStatus), text);
            }
        }

        private void ResolveStationNames()
        {
            _resolvestationnames = _f.ResolveStationNames();
        }

        private void DisplayResults()
        {
            if (_f.rtbDisplay.InvokeRequired)
            {
                _f.rtbDisplay.Invoke(new DisplayDelegate(_f.Display), _marketableitems);
            }
        }

        private StringBuilder CreateBaseMarketstatURL()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_basemarketstaturl);

            if (_locationid[0].Equals('1'))
            {
                sb.Append("regionlimit=");
            }
            else if (_locationid[0].Equals('3'))
            {
                sb.Append("usesystem=");
            }

            sb.Append(_locationid).Append("&typeid=");

            return sb;
        }

        private MarketableItem FindMarketableByTypeID(string typeid)
        {
            MarketableItem m = null;
            int length = _marketableitems.Count;
            bool match = false;

            for (int i = 0; i < length && !match; ++i)
            {
                if (typeid.Equals(_marketableitems[i].TypeID))
                {
                    match = true;
                    m = _marketableitems[i];
                }
            }

            return m;
        }

    }
}
