using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Net;
using System.Xml;

namespace cs_EVE_Arbitrage
{
    class EVECentralInterfacer
    {
        string _sourceid = null;
        string _destinationid = null;
        string _locationid = null;
        volatile List<MarketableItem> _marketableitems = null;
        Form1 _f = null;

        string _basemarketstaturl = "http://api.eve-central.com/api/marketstat?";
        //string _basequicklookurl = "http://api.eve-central.com/api/quicklook?";

        int _requests = 0;
        int _serverresponses = 0;

        public delegate void DisplayDelegate(List<MarketableItem> marketableitems);
        public delegate void StatusDelegate(string text);

        public EVECentralInterfacer(Form1 f, string source, string destination, List<MarketableItem> marketableitems)
        {
            _f = f;
            _sourceid = source;
            _destinationid = destination;
            _marketableitems = marketableitems;
        }

        public void Begin()
        {
            // get marketstat sellmin from source
            int maxurilength = 2000;
            GetMarketstatSellMinData(maxurilength);

            // get marketstat buymax from destination
            GetMarketstatBuyMaxData(maxurilength);

            // discard marketables with no arbitrage opportunities
            DiscardUnprofitableTrades();

            // calculate profit per volume
            CalcProfitPerVolume();

            // get quicklook sellmin from marketables


            // get quicklook buymax from marketables


            // display the results
            OrganizeByDescendingPPV();
            DisplayResults();
            Thread.CurrentThread.Abort();
            Thread.CurrentThread.Join();
        }

        private void GetMarketstatSellMinData(int maxlength)
        {
            _serverresponses = 0;
            _locationid = _sourceid;
            List<string> URIs = AssembleMarketstatURIs(maxlength);
            _requests = URIs.Count;

            try
            {
                WebClient[] w = new WebClient[_requests];
                for (int i = 0; i < _requests; ++i)
                {
                    w[i] = new WebClient();
                    w[i].Proxy = null;
                    w[i].Headers.Add("Contact", "gbates31@gmail.com");
                    w[i].Headers.Add("IGN", "Thirtyone Organism");
                    w[i].DownloadStringCompleted += w_MarketstatSellMin;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                    Thread.Sleep(1000);
                }

                while (_serverresponses < _requests - 1)
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

                UpdateStatus("Source market sell min data complete...\n");
            }
            catch (Exception ex)
            {
            }
        }

        private List<string> AssembleMarketstatURIs(int maxlength)
        {
            List<string> uris = new List<string>();
            StringBuilder sb = new StringBuilder();

            sb = CreateBaseMarketstatURL();

            int typeidlength = 0;

            for (int i = 0; i < _marketableitems.Count; ++i)
            {
                typeidlength = _marketableitems[i].TypeID.Length;

                if (typeidlength + sb.Length < maxlength)
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
                }
            }

            if (sb.Length > 68)
            {
                uris.Add(sb.ToString());
            }

            return uris;
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

                    if (sellmin == 0M)
                    {
                        lock (new object())
                        {
                            _marketableitems.Remove(FindMarketableByTypeID(typeid));
                        }
                    }
                    else
                    {
                        lock (new object())
                        {
                            _marketableitems[_marketableitems.IndexOf(FindMarketableByTypeID(typeid))].SellOrderLowest = sellmin;
                        }
                    }
                }
                ++_serverresponses;
                UpdateStatus("Sellmin Data Retrieved (" + _serverresponses + " of " + _requests + ")");
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdateStatus(string text)
        {
            if (_f.rtbDisplay.InvokeRequired)
            {
                _f.rtbDisplay.Invoke(new StatusDelegate(_f.UpdateStatus), text);
            }
        }

        private void GetMarketstatBuyMaxData(int maxurilength)
        {
            _serverresponses = 0;
            _locationid = _destinationid;
            List<string> URIs = AssembleMarketstatURIs(maxurilength);
            _requests = URIs.Count;

            try
            {
                WebClient[] w = new WebClient[_requests];
                for (int i = 0; i < _requests; ++i)
                {
                    w[i] = new WebClient();
                    w[i].Proxy = null;
                    w[i].DownloadStringCompleted += w_MarketstatBuyMax;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                    Thread.Sleep(1000);
                }

                while (_serverresponses < _requests - 1)
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

                    if (buymax == 0M)
                    {
                        lock (new object())
                        {
                            _marketableitems.Remove(FindMarketableByTypeID(typeid));
                        }
                    }
                    else
                    {
                        lock (new object())
                        {
                            _marketableitems[_marketableitems.IndexOf(FindMarketableByTypeID(typeid))].BuyOrderHighest = buymax;
                        }
                    }
                }
                ++_serverresponses;
                UpdateStatus("Buymax Data Retrieved (" + _serverresponses + " of " + _requests + ")");
            }
            catch (Exception ex)
            {
            }
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
            // bubble sortint temp;
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
