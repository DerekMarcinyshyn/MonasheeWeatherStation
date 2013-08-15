using System;
using System.Text;
using System.Threading;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

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
            //Anemometer anemometer = new Anemometer(Pins.GPIO_PIN_D12);

            Thread.Sleep(500);

            while (true)
            {
                //anemometer.Start();
                //WindVane windvane = new WindVane();

                // create the json data array
                ArrayList data = new ArrayList();
                String na = "N/A";
                String time = DateTime.Now.ToString();
                data.Add(@"{""temp"":""" + na + "\"" + ",");
                data.Add(@"""humidity"":""" + time + "\"" + ",");
                //data.Add(@"""direction"":""" + windvane.WindDirection + "\"" + ",");
                //data.Add(@"""speed"":""" + anemometer.WindSpeed.ToString("F1") + "\"" + "}");

                Data = data;

                // collect every 10 seconds
                Thread.Sleep(10000);
            }           
        }
    }
}
