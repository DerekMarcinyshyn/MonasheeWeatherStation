using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Socket = System.Net.Sockets.Socket;

namespace MonasheeWeatherStation
{
    class RainGauge
    {
        // server settings
        const string SERVER_HOST_POST = "192.168.1.34";
        const Int32 SERVER_PORT = 80;  

        // interrupt port bind
        private InterruptPort inPort;

        // interval for debouncing
        private const int DEBOUNCING_INTERVAL = 200;

        // last raingauge interrupt
        private static DateTime raingauge_interruptTime = DateTime.Now;

        // reference rainfall per click
        private const float REFERENCE_RAINFALL = 0.2791f;

        // rainfall counter
        private int rainfallCount;

        static Random random = new Random();

        /// <summary>
        /// Rain Fall
        /// </summary>
        public double RainFall { get; private set; }

        /// <summary>
        /// Rain Gauge for reading tipping bucket
        /// </summary>
        /// <param name="inPin">Digital Pin the Rain Gauge is connected to</param>
        public RainGauge(Cpu.Pin inPin)
        {
            this.inPort = new InterruptPort(inPin, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
            this.inPort.OnInterrupt += new NativeEventHandler(inPort_OnInterrupt);
        }

        void inPort_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (raingauge_interruptTime.AddMilliseconds(DEBOUNCING_INTERVAL) < time)
            {
                // set last interupt time
                raingauge_interruptTime = time;                

                rainfallCount++;
                //this.RainFall = rainfallCount * REFERENCE_RAINFALL;

                //Debug.Print("it tipped: " + rainfallCount);

                SendPost();
            }
        }

        /// <summary>
        /// Builds the string to send to the server
        /// </summary>
        private void SendPost()
        {                     
            // Build the Request string
            String _args = "shared=9v5s44s7E284Nr2e2813z3cp107Fz2";

            String _request = "POST /weather-station/json.php HTTP/1.0\n";
            _request += "Content-Type: application/x-www-form-urlencoded\n";
            _request += "Content-Length: " + _args.Length + "\n\n";
            _request += _args;

            //Debug.Print("sendPost: \n" + _request);

            SendPostRequest(SERVER_HOST_POST, SERVER_PORT, _request);
        }

        /// <summary>
        ///  Sends the request to the server via a socket
        /// </summary>
        /// <param name="server">The IP/URL of the server</param>
        /// <param name="port">The port of the server</param>
        /// <param name="request">The request string including HTTP headers</param>
        /// <returns></returns>
        private static String SendPostRequest(String server, Int32 port, String request)
        {
            const Int32 c_microsecondsPerSecond = 1000000;

            // Create a socket connection to the specified server and port
            using (Socket ServerPostSocket = ConnectPostSocket(server, port))
            {
                // Send request to the server.
                Byte[] bytesToSend = Encoding.UTF8.GetBytes(request);
                ServerPostSocket.Send(bytesToSend, bytesToSend.Length, 0);

                // Reusable buffer for receiving chunks of the document.
                Byte[] postbuffer = new Byte[1024];

                // Accumulates the received page as it is built from the buffer.
                String postpage = String.Empty;

                // Wait up to 30 seconds for initial data to be available.  Throws an exception if the connection is closed with no data sent.
                DateTime timeoutAt = DateTime.Now.AddSeconds(30);
                while (ServerPostSocket.Available == 0 && DateTime.Now < timeoutAt)
                {
                    System.Threading.Thread.Sleep(100);
                }

                // Poll for data until 30-second timeout.  Returns true for data and connection closed.
                while (ServerPostSocket.Poll(30 * c_microsecondsPerSecond, SelectMode.SelectRead))
                {
                    // If there are 0 bytes in the buffer, then the connection is closed, or we have timed out.
                    if (ServerPostSocket.Available == 0) break;

                    // Zero all bytes in the re-usable buffer.
                    Array.Clear(postbuffer, 0, postbuffer.Length);

                    // Read a buffer-sized HTML chunk.
                    Int32 PostBytesRead = ServerPostSocket.Receive(postbuffer);

                    // Append the chunk to the string.
                    postpage = postpage + new String(Encoding.UTF8.GetChars(postbuffer));
                }

                // Return the request string
                return postpage;
            }
        }

        /// <summary>
        /// Creates a socket and uses the socket to connect to the server's IP address and port. (From the .NET Micro Framework SDK example)
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private static Socket ConnectPostSocket(String server, Int32 port)
        {
            // Get server's IP address.
            IPHostEntry HostEntry = Dns.GetHostEntry(server);

            // Create socket and connect to the server's IP address and port
            Socket PostSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            PostSocket.Connect(new IPEndPoint(HostEntry.AddressList[0], port));

            return PostSocket;
        }
    }
}
