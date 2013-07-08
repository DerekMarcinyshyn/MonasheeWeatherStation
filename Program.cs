using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using CW.NETMF;

namespace MonasheeWeatherStation
{
    public class Program
    {
        static Dht22Sensor temphumidity = new Dht22Sensor(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, PullUpResistor.Internal);

        /**
         * Main program
         */
        public static void Main()
        {
            // let the Netduino fire up first
            Thread.Sleep(3000);

            /**** MAIN LOOP ****/
            while (true)
            {

                temperatureHumidity();
                
                // Loop every X seconds?
                Thread.Sleep(3000);
                
            }
            /**** END MAIN LOOP ****/
        }

        /**
         * Temperature and Humidity
         */
        private static void temperatureHumidity()
        {
            if (temphumidity.Read())
            {
                var temp = temphumidity.Temperature;
                var humidity = temphumidity.Humidity;

                Debug.Print("RH = " + humidity.ToString("F1") + "%, temp = " + temp.ToString("F1") + "*C");
            }
        }

    }
}
