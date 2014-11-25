using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLivingRoom_Kinect
{
    class Server
    {
        private static HttpListener _listener;

        public static void CreateHttpServer()
        {
            string reqUrl = "http://localhost:8080/";

            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("HttpServer is not supported\n");
                return;
            }

            if (reqUrl == null)
                throw new ArgumentException("prefixes");

            // Initialize the listener
            _listener = new HttpListener();
            // Add the url
            _listener.Prefixes.Add(reqUrl);

            try
            {
                StartListening();
            }
            finally
            {
                CloseHttpServer();
            }
        }

        public static void StartListening() {

            _listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                var apiRequest = request.RawUrl;

                switch (apiRequest)
                {
                    case "/api/kinect/distance":
                        DispatchDistance(response);
                        break;
                    case "/api/kinect/change-ratio":
                        DispatchChangeRatio(response);
                        break;
                    case "/api/kinect/contact":
                        DispatchContact(response);
                        break;
                }

                // Throw away the request if not matching
            }
        }

        private static void DispatchDistance(HttpListenerResponse response)
        {
            // Construct a response
            string distanceRespString = Kinect.GetInstance().GetLastDistance().ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(distanceRespString);

            // Get a response stream and write to it
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // Close the output stream
            output.Close();
        }

        private static void DispatchChangeRatio(HttpListenerResponse response)
        {
            // Construct a response
            string distanceRespString = Kinect.GetInstance().GetDistChangeRatio().ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(distanceRespString);

            // Get a response stream and write to it
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // Close the output stream
            output.Close();
        }

        private static void DispatchContact(HttpListenerResponse response)
        {
            // Construct a response
            string distanceRespString = Kinect.GetInstance().GetPastHandContact().ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(distanceRespString);

            // Get a response stream and write to it
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // Close the output stream
            output.Close();
        }

        public static void CloseHttpServer()
        {
            _listener.Stop();
        }
    }
}
