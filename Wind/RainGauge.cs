using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace MonasheeWeatherStation
{
    class RainGauge
    {
        // interrupt port bind
        private InterruptPort inPort;

        // interval for debouncing
        private const int DEBOUNCING_INTERVAL = 200;

        // last raingauge interrupt
        private static DateTime raingauge_interruptTime = DateTime.Now;

        // reference rainfall per click
        private const float REFERENCE_RAINFALL = 0.2791f;

        // rainfall counter
        private int rainfallCount;

        /// <summary>
        /// Rain Fall
        /// </summary>
        public double RainFall { get; private set; }

        /// <summary>
        /// Rain Gauge for reading tipping bucket
        /// </summary>
        /// <param name="inPin">Digital Pin the Rain Gauge is connected to</param>
        public RainGauge(Cpu.Pin inPin)
        {
            this.inPort = new InterruptPort(inPin, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeHigh);
            this.inPort.OnInterrupt += new NativeEventHandler(inPort_OnInterrupt);
        }

        void inPort_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (raingauge_interruptTime.AddMilliseconds(DEBOUNCING_INTERVAL) < time)
            {
                // set last interupt time
                raingauge_interruptTime = time;

                // maybe here to post to database that a rain event happened?

                rainfallCount++;
                this.RainFall = rainfallCount * REFERENCE_RAINFALL;                
            }
        }
    }
}
