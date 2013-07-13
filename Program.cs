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
        static Dht22Sensor temphumidity = new Dht22Sensor(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, PullUpResistor.Internal);
        static BMP085 barometer = new BMP085(0x77, BMP085.DeviceMode.UltraHighResolution);

        /// <summary>
        /// Main program
        /// </summary>
        public static void Main()
        {
            // let the Netduino fire up first
            Thread.Sleep(500);

            /**** MAIN LOOP ****/
            while (true)
            {

                barometerTemperature();

                //temperatureHumidity();
                
                // Loop every X seconds?
                Thread.Sleep(10000);
                
            }
            /**** END MAIN LOOP ****/
        }

        /// <summary>
        /// Barometer and Temperture
        /// </summary>
        private static void barometerTemperature()
        {
            // compensate for elevation
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
