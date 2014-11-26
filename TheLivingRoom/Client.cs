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
        private static Client _instance;

        private Client()
        {
        }

        public static Client GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            _instance = new Client();
            return _instance;
        }

        public async Task<string> HttpGetAsync(string uri)
        {
            _request = (HttpWebRequest)WebRequest.Create("http://localhost:8080" + uri);
            _request.Proxy = null;
            _request.Method = "GET";
            // Set the ContentType property of the WebRequest
            _request.ContentType = "application/x-www-form-urlencoded";

            using (WebResponse response = await _request.GetResponseAsync())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                return reader.ReadToEnd();
        }
    }
}
