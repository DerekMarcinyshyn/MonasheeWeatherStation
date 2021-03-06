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

        public Thread collectorThread = null;

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
            Anemometer anemometer = new Anemometer(Pins.GPIO_PIN_D12);           

            Thread.Sleep(3000);

            while (true)
            {
                try
                {
                    anemometer.Start();
                    WindVane windvane = new WindVane();
                    Humidity humidity = new Humidity();

                    // create the json data array
                    ArrayList data = new ArrayList();

                    data.Add(@"{""humidity"":""" + humidity.RelativeHumidity.ToString() + "\"" + ",");
                    data.Add(@"""direction"":""" + windvane.WindDirection + "\"" + ",");
                    data.Add(@"""speed"":""" + anemometer.WindSpeed.ToString("F1") + "\"" + "}");

                    Data = data;

                    Debug.Print(anemometer.WindSpeed.ToString("F1"));
                    //Debug.Print(windvane.WindDirection);
     
                    // collect every second
                    Thread.Sleep(2500);
                    Debug.GC(true);
                }
                catch (Exception e)
                {
                    Debug.Print(e.ToString());
                    PowerState.RebootDevice(false); 
                }
            }          
        }

        ~DataCollector()
        {
            Dispose();
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                collectorThread = null;
            }
        }
    }
}
