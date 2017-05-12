using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using SecretLabs.NETMF.Hardware;
using System.IO;

namespace MonasheeWeatherStation
{
    public class WebServer : IDisposable
    {
        /// <summary>
        /// Socket to listen to web requests
        /// </summary>
        private Socket _socket = null;

        // add led to show requests

        // add collector class to gather sensor data
        private DataCollector Collector { get; set; }

        // server config
        private const Int32 SERVER_PORT = 80;
        private string IP_ADDRESS = "192.168.1.50";
        private string IP_SUBNET_MASK = "255.255.255.0";
        private string IP_GATEWAY = "192.168.1.1";

        /// <summary>
        /// Creates a webserver listening on SERVER_PORT
        /// </summary>
        public WebServer(DataCollector collector)
        {
            // keep reference to the collector
            Collector = collector;

            // Set the Static IP Address
            var NetworkInterface = Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0];
            NetworkInterface.EnableStaticIP(IP_ADDRESS, IP_SUBNET_MASK, IP_GATEWAY);

            // Initialize Socket Class
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Request and bind to an IP from the router
            _socket.Bind(new IPEndPoint(IPAddress.Any, SERVER_PORT));

            // Debug print out IP address
            Debug.Print(Microsoft.SPOT.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()[0].IPAddress);

            //start listening for web requests
            _socket.Listen(3);            
            AcceptRequests();
        }

        /// <summary>
        /// Continuously accepts requests and servers them
        /// </summary>
        public void AcceptRequests()
        {
            while (true)
            {
                try
                {
                    using (Socket connectionSocket = _socket.Accept())
                    {
                        // get clients IP
                        IPEndPoint ClientIP = connectionSocket.RemoteEndPoint as IPEndPoint;
                        EndPoint ClientEndPoint = connectionSocket.RemoteEndPoint;
                        int BytesReceived = connectionSocket.Available;

                        if (BytesReceived > 0)
                        {
                            // Get request
                            byte[] Buffer = new byte[BytesReceived];
                            int ByteCount = connectionSocket.Receive(Buffer, BytesReceived, SocketFlags.None);
                            string Request = new string(Encoding.UTF8.GetChars(Buffer));
                            Debug.Print(Request);

                            // Server the request -- basic routing
                            if (Request.IndexOf("GET / HTTP/1.1") == 0)
                            {
                                //Debug.Print("show simple page");
                                String response = GetJsonResponse();
                                Serve(response, connectionSocket);
                            }
                            else if (Request.IndexOf("GET /favicon.ico HTTP/1.1") == 0) // favicon requested?
                            {
                                //Debug.Print("show 404");
                                String response = "";
                                ServeWith404(response, connectionSocket);
                            }
                            else // do not know what to serve so 404
                            {
                                //Debug.Print("show do not know 404");
                                String response = "<!DOCTYPE html><html><head><title>Unknown request 404 Error</title></head>" +
                                    "<body><h1>Unknown request 404 ERROR</h1></body></html>";
                                ServeWith404(response, connectionSocket);
                            }
                        }
                    }
                }
                catch
                {
                    PowerState.RebootDevice(true);   
                }   
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetJsonResponse()
        {         
            if (Collector.Data != null) 
            {
                String jsonData = @"";

                foreach (var item in Collector.Data)
                    jsonData += item;

                jsonData += @"";

                return jsonData;
            }
            else
            {
                return @"{""temp"":""error""}";
            }            
        }
                
        /// <summary>
        /// Serves the response to the client with a status code of 200 OK
        /// </summary>
        /// <param name="response">The response to send</param>
        /// <param name="socket">The socket to send the response with</param>
        private void Serve(string response, Socket socket)
        {
            string header = "HTTP/1.1 200 OK\r\n"
                + "Cache-Control: no-cache, must-revalidate\r\n"
                + "Connection: close\r\n"                
                + "Content-Length: " + response.Length.ToString() + "\r\n"                
                + "Content-Type: application/json\r\n"
                + "Expires: Mon, 26 Jul 1997 05:00:00 GMT\r\n\r\n";
            SendResponse(response, socket, header);
        }

        /// <summary>
        /// Sends the header and response to the client using the given socket
        /// </summary>
        /// <param name="response"></param>
        /// <param name="socket"></param>
        /// <param name="header"></param>
        private void SendResponse(string response, Socket socket, string header)
        {
            socket.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
            var sendStream = new NetworkStream(socket, false);

            try
            {
                socket.Send(Encoding.UTF8.GetBytes(response), response.Length, SocketFlags.None);
            } catch {
                PowerState.RebootDevice(true);
            }
            // add blink here
        }

        /// <summary>
        /// Serves the response to the client with a status of 404 Not Found
        /// </summary>
        /// <param name="response"></param>
        /// <param name="socket"></param>
        private void ServeWith404(string response, Socket socket)
        {
            string header = "HTTP/1.0 404 Not Found\r\nContent-Type: text; charset=utf-8\r\nContent-Length: " 
                + response.Length.ToString() + "\r\nConnection: close\r\n\r\n";
            SendResponse(response, socket, header);
        }
        
        ~WebServer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (_socket != null)
                _socket.Close();
        }
    }
}
