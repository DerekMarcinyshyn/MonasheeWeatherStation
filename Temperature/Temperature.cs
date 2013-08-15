using System;
using System.Threading;
using ThreelnDotOrg.NETMF.Hardware;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace MonasheeWeatherStation
{
    public class Temperature
    {
        private Timer _sensorTimer;
        private float _celsius;
        DS18B20 t = new DS18B20(Pins.GPIO_PIN_D4);
                
        public Temperature()
        {
            // take initial measurement
            TakeMeasurements();

            // take new measurement every 10 seconds
            _sensorTimer = new Timer(TakeMeasurements, null, 200, 10000);
        }

        private void TakeMeasurements()
        {
            TakeMeasurements(null);
        }

        private void TakeMeasurements(object state)
        {
            float temp = t.ConvertAndReadTemperature();
            _celsius = temp;
        }

        public float Celsius
        {
            get { return _celsius; }
        }
    }
}
