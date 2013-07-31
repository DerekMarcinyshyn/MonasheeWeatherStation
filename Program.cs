using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using CW.NETMF; // temp/humidity drivers
using Toolbox.NETMF.Hardware;

namespace MonasheeWeatherStation
{
    public class Program
    {
        /// <summary>
        /// Main program
        /// </summary>
        public static void Main()
        {
            // check RAM usage
            Debug.Print(Debug.GC(true) + " bytes available after garbage collection");

            // let the Netduino fire up first
            Thread.Sleep(500);

            // start collecting data
            var collector = new DataCollector();

            // start the webserver
            WebServer webserver = new WebServer(collector);

            Thread.Sleep(Timeout.Infinite);
        }        
    }   
}