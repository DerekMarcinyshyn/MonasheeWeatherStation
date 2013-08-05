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

namespace MonasheeWeatherStation
{
    public class Program
    {
        static RainGauge raingauge = new RainGauge(Pins.GPIO_PIN_D10);
        //static BMP085 barometer = new BMP085(0x77, BMP085.DeviceMode.UltraHighResolution);

        /// <summary>
        /// Main program
        /// </summary>
        public static void Main()
        {
            // check RAM usage
            //Debug.Print(Debug.GC(true) + " bytes available after garbage collection");

            // let the Netduino fire up first
            Thread.Sleep(5000);

            // start collecting data
            DataCollector collector = new DataCollector();

            Thread.Sleep(2000);

            // start the webserver
            WebServer webserver = new WebServer(collector);

            Thread.Sleep(Timeout.Infinite);
        }        
    }   
}