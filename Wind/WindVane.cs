using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace MonasheeWeatherStation
{
    class WindVane
    {
        // wind vane on analog 2
        private static SecretLabs.NETMF.Hardware.AnalogInput windvane = new SecretLabs.NETMF.Hardware.AnalogInput(Pins.GPIO_PIN_A2);

        // wind direction
        private string _windDirection;
        private string _windRaw;

        private const int MaximumValue = 1024;
        private const float AnalogReference = 3.30f;
        private const float VoltsPerCount = AnalogReference / MaximumValue;

        public WindVane()
        {
            // get the digital reading and convert to analog volts
            float volts = windvane.Read() * VoltsPerCount;
            _windRaw = volts.ToString();

            _windDirection = "N/A";           
            
            /**
             * N    1.36
             * NW   1.90
             * W    2.36
             * SW   .84
             * S    .26
             * SE   .15
             * E    .06
             * NE   .50 
             */

            if (volts > 1.20 && volts < 1.60)
                _windDirection = "N";

            if (volts > 1.80 && volts < 2.00)
                _windDirection = "NW";

            if (volts > 2.0 && volts < 2.4)
                _windDirection = "W";

            if (volts > .7 && volts < 1.0)
                _windDirection = "SW";

            if (volts > .20 && volts < .6)
                _windDirection = "S";

            if (volts > .1 && volts < .19)
                _windDirection = "SE";

            if (volts > .01 && volts < .09)
                _windDirection = "E";

            if (volts > .3 && volts < .6)
                _windDirection = "NE";           
        }

        public string WindDirection
        {
            get { return _windDirection; }
        }

        public string WindRaw
        {
            get { return _windRaw; }
        }
    }

}
