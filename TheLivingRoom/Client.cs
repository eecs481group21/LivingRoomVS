using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom
{
    public class Client
    {
        private HttpWebRequest _request;

        public async Task<object> HttpGetAsync(string url)
        {
            _request = (HttpWebRequest)WebRequest.Create(url);
            _request.Proxy = null;
            _request.Method = "GET";
            // Set the ContentType property of the WebRequest
            _request.ContentType = "application/x-www-form-urlencoded";

            using (WebResponse response = await _request.GetResponseAsync())
                return response;
        }
    }
}
