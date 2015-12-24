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
            int typeidsperrequest = 460;
            int fullrequests = _marketableitems.Count / typeidsperrequest;
            int partiallistlength = _marketableitems.Count % typeidsperrequest;
            int requests = partiallistlength != 0 ? fullrequests + 1 : fullrequests;

            StringBuilder[] sb = new StringBuilder[requests];
            
            int min = 0;
            int max = typeidsperrequest;
            for (int i = 0; i < requests; ++i, min += typeidsperrequest, max += typeidsperrequest)
            {
                sb[i] = AssembleSB(min, max); // REDO THIS SO THE REQUEST LENGTH DOESN'T EXCEED SERVER MAX
            }

            try
            {
                WebClient[] w = new WebClient[requests];
                for (int i = 0; i < requests; ++i)
                {
                    w[i] = new WebClient();
                    w[i].Proxy = null;
                    w[i].DownloadStringCompleted += w_MarketstatSellMin;
                    w[i].DownloadStringAsync(new Uri(sb[i].ToString()));
                    Thread.Sleep(1100);
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

            while (responses != requests)
            {
                Thread.Sleep(10);
            }
        }

        private StringBuilder AssembleSB(int min, int max)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_basemarketstaturl).Append("usesystem=").Append(_sourceid).Append("&typeid=");

            for (int i = min; i < max && i < _marketableitems.Count; ++i)
            {
                sb.Append(_marketableitems[i].TypeID);

                if (i < max - 1 && i < _marketableitems.Count - 1)
                {
                    sb.Append(',');
                }
            }

            return sb;
        }

        int responses = 1;
        private void w_MarketstatSellMin(object sender, DownloadStringCompletedEventArgs e)
        {
            if (responses == 2)
            {
                responses = 2;
            }
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
            ++responses;
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
