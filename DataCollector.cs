using System;
using System.Text;
using System.Threading;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Toolbox.NETMF;
using CW.NETMF; // temp-humdity drivers

namespace MonasheeWeatherStation
{
    /// <summary>
    /// A collector for data
    /// </summary>
    public class DataCollector
    {
        /// <summary>
        /// The current collected data which is a list of ISample objects
        /// </summary>
        public ArrayList Data { get; private set; }

        /// <summary>
        /// Creates a new data collector
        /// Immediately starts to collect data on all its registered sensors in the background
        /// Most recent data is available through the Data property
        /// </summary>
        internal DataCollector()
        {
            Start();
        }

        /// <summary>
        /// Start collecting data
        /// </summary>
        public void Start()
        {
            ThreadStart collectMethod = new ThreadStart(Collect);
            Thread collectorThread = new Thread(collectMethod);
            collectorThread.Start();
        }

        /// <summary>
        /// Synchronously collects new data in a loop.
        /// This methods runs on a background thread
        /// </summary>
        public void Collect()
        {
 
            // Sensor variables
            Dht22Sensor temphumidity = new Dht22Sensor(Pins.GPIO_PIN_D0, Pins.GPIO_PIN_D1, PullUpResistor.Internal);
            //try
            //{
                //BMP085 baro = new BMP085(0x77, BMP085.DeviceMode.Standard);
            //}
            //catch (Exception ex)
            //{
                //String breadcrumbs = @"{""tempbmp"":"""+ex.ToString()+"\"" + "}";
                //ArrayList bc = new ArrayList();
                //bc.Add(breadcrumbs);
                //Data = bc;
            //}


            BMP085 barometer = new BMP085(0x77, BMP085.DeviceMode.Standard);
            Anemometer anemometer = new Anemometer(Pins.GPIO_PIN_D12);

            while (true)
            {
                // read DHT22
                if (temphumidity.Read())
                {
                    // DHT22 sensor
                    String temp = temphumidity.Temperature.ToString("F1");
                    String humidity = temphumidity.Humidity.ToString("F1");

                    // compensate for elevation in meters
                    int altitude = 500;
                    double altimeter = (float)101325 * System.Math.Pow(((288 - 0.0065 * altitude) / 288), 5.256);
                    double pressureASL = ((101325 + barometer.Pascal) - altimeter) / 1000;

                    anemometer.Start();
                    WindVane windvane = new WindVane();

                    // create the json data array
                    ArrayList data = new ArrayList();
                    data.Add(@"{""tempdht"":""" + temp + "\"" + ",");
                    data.Add(@"""humiditydht"":""" + humidity + "\"" + ",");
                    data.Add(@"""tempbmp"":""" + barometer.Celsius.ToString("F1") + "\"" + ",");
                    data.Add(@"""pressurebmp"":""" + pressureASL.ToString("F1") + "\"" + ",");
                    data.Add(@"""direction"":""" + windvane.WindDirection + "\"" + ",");
                    data.Add(@"""speed"":""" + anemometer.WindSpeed.ToString("F1") + "\"" + "}");

                    Data = data;
                }
                else
                {
                    // compensate for elevation in meters
                    int altitude = 500;
                    double altimeter = (float)101325 * System.Math.Pow(((288 - 0.0065 * altitude) / 288), 5.256);
                    double pressureASL = ((101325 + barometer.Pascal) - altimeter) / 1000;

                    anemometer.Start();
                    WindVane windvane = new WindVane();

                    // create the json data array
                    ArrayList data = new ArrayList();
                    String na = "N/A";
                    data.Add(@"{""tempdht"":""" + na + "\"" + ",");
                    data.Add(@"""humiditydht"":""" + na + "\"" + ",");
                    data.Add(@"""tempbmp"":""" + barometer.Celsius.ToString("F1") + "\"" + ",");
                    data.Add(@"""pressurebmp"":""" + pressureASL.ToString("F1") + "\"" + ",");
                    data.Add(@"""direction"":""" + windvane.WindDirection + "\"" + ",");
                    data.Add(@"""speed"":""" + anemometer.WindSpeed.ToString("F1") + "\"" + "}");

                    Data = data;
                }

                // collect every 10 seconds
                Thread.Sleep(10000);
            }           
        }
    }
}
