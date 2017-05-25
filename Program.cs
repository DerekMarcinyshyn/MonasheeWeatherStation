using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace MonasheeWeatherStation
{
    public class Program
    {
        // Wind vane Analog Pin 2
        // Anemometer Digital Pin 12
        // Humidity Analog Pin 1
        // static RainGauge raingauge = new RainGauge(Pins.GPIO_PIN_D10);    

        /// <summary>
        /// Main program
        /// </summary>
        public static void Main()
        {
            // check RAM usage
            Debug.Print(Debug.GC(true) + " bytes available after garbage collection");
            
            // let the Netduino fire up first
            Thread.Sleep(3000);

            try
            {
                // start collecting data
                DataCollector collector = new DataCollector();

                // start the webserver
                WebServer webserver = new WebServer(collector);

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                PowerState.RebootDevice(false);
            }
        }        
    }   
}