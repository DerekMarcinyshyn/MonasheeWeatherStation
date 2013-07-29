using System;
using System.Net;
using System.Net.Sockets;
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
        //static Dht22Sensor temphumidity = new Dht22Sensor(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, PullUpResistor.Internal);
        //static BMP085 barometer = new BMP085(0x77, BMP085.DeviceMode.UltraHighResolution);
        //static Anemometer anemometer = new Anemometer(Pins.GPIO_PIN_D12); 
        static RainGauge raingauge = new RainGauge(Pins.GPIO_PIN_D10);

        /// <summary>
        /// Main program
        /// </summary>
        public static void Main()
        {            
            // check RAM usage
            Debug.Print(Debug.GC(true) + " bytes available after garbage collection");            

            // let the Netduino fire up first
            Thread.Sleep(500);

            //anemometer.Start();

            /**** MAIN LOOP ****/
            while (true)
            {
                //BarometerTemperature();

                //TemperatureHumidity();

                //WindDirection();

                Rainfall();

                //Debug.Print("windspeed: " + System.Math.Round(anemometer.WindSpeed));                

                // Loop every X seconds?
                Thread.Sleep(2000);

                // call Garbage Collector so it can run forever
                Debug.GC(true);                
            }
            /**** END MAIN LOOP ****/

 
        }

        private static void Rainfall()
        {
            // send post to database to record that a rainfall gauge event happened
            Debug.Print("rainfall: " + raingauge.RainFall.ToString());
        }               

        /// <summary>
        /// Wind Direction
        /// </summary>
        private static void WindDirection()
        {
            var windvane = new WindVane();
            //Debug.Print("wind raw: " + windvane.WindRaw);
            //Debug.Print("wind direction: " + windvane.WindDirection);
        }

        /**
        /// <summary>
        /// Barometer and Temperture
        /// </summary>
        private static void BarometerTemperature()
        {
            // compensate for elevation in meters
            int altitude = 500;
            double altimeter = (float)101325 * System.Math.Pow(((288 - 0.0065 * altitude) / 288), 5.256);
            double pressureASL = (101325 + barometer.Pascal) - altimeter;

            Debug.Print("Pascal: " + barometer.Pascal);
            Debug.Print("kPa: " + pressureASL);
            Debug.Print("Mg: " + barometer.InchesMercury.ToString("F2"));
            Debug.Print("Temp: " + barometer.Celsius.ToString("F2"));
        }

        /// <summary>
        /// Temperature and Humidity
        /// </summary>
        private static void TemperatureHumidity()
        {
            if (temphumidity.Read())
            {
                var temp = temphumidity.Temperature;
                var humidity = temphumidity.Humidity;

                Debug.Print("RH = " + humidity.ToString("F1") + "%, temp = " + temp.ToString("F1") + "*C");
            }
        }
        */
    }
}
