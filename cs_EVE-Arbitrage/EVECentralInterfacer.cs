using System;
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
        List<MarketableItem> _marketableitems = null;

        string _basemarketstaturl = "http://api.eve-central.com/api/marketstat?";
        string _basequicklookurl = "http://api.eve-central.com/api/quicklook?";

        public EVECentralInterfacer(string source, string destination, List<MarketableItem> marketableitems)
        {
            _sourceid = source;
            _destinationid = destination;
            _marketableitems = marketableitems;
        }

        public void Begin()
        {
            // get marketstat sellmin from source
            int maxurilength = 2048;
            List<string> URIs = AssembleMarketstatSellMinURIs(maxurilength);
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
            }
            catch (Exception ex)
            {

            }


            // get marketstat buymax from destination


            // discard marketables with no arbitrage opportunities


            // get quicklook sellmin from marketables


            // get quicklook buymax from marketables


            // display the results

        }

        private List<string> AssembleMarketstatSellMinURIs(int maxlength)
        {
            List<string> uris = new List<string>();
            StringBuilder sb = new StringBuilder();

            sb.Append(_basemarketstaturl).Append("usesystem=").Append(_sourceid).Append("&typeid=");

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
                    sb.Append(_basemarketstaturl).Append("usesystem=").Append(_sourceid).Append("&typeid=");
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
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(e.Result);
                xmldoc.Save(DateTime.Now.Millisecond.ToString() + ".xml");

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
            }
            catch (Exception ex)
            {

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
