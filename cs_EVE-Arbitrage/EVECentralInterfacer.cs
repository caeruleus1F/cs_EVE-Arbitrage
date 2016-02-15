using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace cs_EVE_Arbitrage
{
    class EVECentralInterfacer : WebClient
    {

        public EVECentralInterfacer()
        {
            this.Proxy = null;
            this.Headers.Add("Program", "Arbitrage Trader");
            this.Headers.Add("Contact", "garrett.bates@outlook.com");
            this.Headers.Add("IGN", "Thirtyone Organism");
            this.Headers.Add("Accept-Encoding", "gzip");
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.GZip
                     | DecompressionMethods.Deflate;
            return request;
        }
    }
}
