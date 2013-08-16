using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace MonasheeWeatherStation
{
    public class Humidity
    {
        // inputs
        private static SecretLabs.NETMF.Hardware.AnalogInput humidity = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A0);

        // variables
        private float _relativeHumidity;

        public Humidity()
        {
            // get 3 humidity readings and average
            int h1 = humidity.Read();
            Thread.Sleep(1000);

            int h2 = humidity.Read();
            Thread.Sleep(1000);

            int h3 = humidity.Read();
            
            _relativeHumidity = (h1 + h2 + h3) / 3;
        }

        public float RelativeHumidity
        {
            get { return _relativeHumidity; }
        }
    }
}
