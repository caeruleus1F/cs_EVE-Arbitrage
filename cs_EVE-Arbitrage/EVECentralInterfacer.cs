﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Xml;

namespace cs_EVE_Arbitrage
{
    class EVECentralInterfacer
    {
        string _sourceid = null;
        string _destinationid = null;
        string _locationid = null;
        List<MarketableItem> _marketableitems = null;

        string _basemarketstaturl = "http://api.eve-central.com/api/marketstat?";
        string _basequicklookurl = "http://api.eve-central.com/api/quicklook?";

        int _serverresponses = 0;

        public EVECentralInterfacer(string source, string destination, List<MarketableItem> marketableitems)
        {
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
        }

        private void GetMarketstatBuyMaxData(int maxurilength)
        {
            _serverresponses = 0;
            _locationid = _destinationid;
            List<string> URIs = AssembleMarketstatURIs(maxurilength);
            int requests = URIs.Count;

            try
            {
                WebClient[] w = new WebClient[requests];
                for (int i = 0; i < requests; ++i)
                {
                    w[i] = new WebClient();
                    w[i].Proxy = null;
                    w[i].DownloadStringCompleted += w_MarketstatBuyMax;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                    Thread.Sleep(1000);
                }

                while (_serverresponses < requests - 1)
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

        private void GetMarketstatSellMinData(int maxlength)
        {
            _serverresponses = 0;
            _locationid = _sourceid;
            List<string> URIs = AssembleMarketstatURIs(maxlength);
            int requests = URIs.Count;

            try
            {
                WebClient[] w = new WebClient[requests];
                for (int i = 0; i < requests; ++i)
                {
                    w[i] = new WebClient();
                    w[i].Proxy = null;
                    w[i].DownloadStringCompleted += w_MarketstatSellMin;
                    w[i].DownloadStringAsync(new Uri(URIs[i]));
                    Thread.Sleep(1000);
                }

                while (_serverresponses < requests - 1)
                {
                    Thread.Sleep(1);
                }

                // for whatever reason, not all items with no sell orders
                // are removed from the list of marketables.
                for (int i = _marketableitems.Count - 1; i >= 0 ; --i)
                {
                    if (_marketableitems[i].SellOrderLowest == 0M)
                    {
                        _marketableitems.RemoveAt(i);
                    }
                }
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
                xmldoc.Save(DateTime.Now.Second + "_" + DateTime.Now.Millisecond.ToString() + ".xml");

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
            }
            catch (Exception ex)
            {
                xmldoc.Save("problem_response.xml");
            }
        }

        private void w_MarketstatSellMin(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(e.Result);
                xmldoc.Save(DateTime.Now.Second + "_" + DateTime.Now.Millisecond.ToString() + ".xml");

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
            }
            catch (Exception ex)
            {
                xmldoc.Save("problem_response.xml");
            }
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
