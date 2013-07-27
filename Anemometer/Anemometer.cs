using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace MonasheeWeatherStation
{
    class Anemometer
    {
        // default watching period for calculating wind speed
        public const int DEFAULT_CALCULATE_PERIOD = 5000;

        // interval for debouncing
        private const int DEBOUNCING_INTERVAL = 1;

        // reference wind speed
        private const float REFERENCE_WIND_SPEED = 12; 

        // reference pulse for seconds
        private const float REFERENCE_PULSE_FOR_SECOND = 5;

        // watching period for calculating wind speed
        private int calculatePeriod;

        // interrupt port bind to anemometer internal switch
        private InterruptPort inPort;

        // timer for calculating wind speed periodically
        private Timer timer;

        // previous pulse ticks
        private long prevPulseTicks;

        // anemometer pulse count
        private int pulseCount;

        private float referenceWindSpeed;
        private float referencePulseForSecond;

        private object lockObj = new Object();

        /// <summary>
        /// Wind Speed
        /// </summary>
        public float WindSpeed { get; private set; }
        
        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="inPin">Pin used for reading anemometer</param>
        /// <param name="calculatePeriod">Period for calculating wind speed</param>
        /// <param name="referenceWindSpeed">Reference wind speed</param>
        /// <param name="referencePulseForSecond">Reference pulse for second</param>
        public Anemometer(
            Cpu.Pin inPin,
            int calculatePeriod = DEFAULT_CALCULATE_PERIOD,
            float referenceWindSpeed = REFERENCE_WIND_SPEED,
            float referencePulseForSecond = REFERENCE_PULSE_FOR_SECOND)
        {
            this.calculatePeriod = calculatePeriod;
            this.referenceWindSpeed = referenceWindSpeed;
            this.referencePulseForSecond = referencePulseForSecond;
            
            this.inPort = new InterruptPort(inPin, false, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            this.inPort.OnInterrupt += new NativeEventHandler(intPort_OnInterrupt);

            this.timer = new Timer(CalculateWindSpeed, null, Timeout.Infinite, 0);
        }

        /// <summary>
        /// Start periodically wind speed calculation
        /// </summary>
        public void Start()
        {
            this.timer.Change(0, this.calculatePeriod);
        }

        /// <summary>
        /// Stop periodically wind speed calculation
        /// </summary>
        public void Stop()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        void intPort_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            lock (this.lockObj)
            {
                long ticks = time.Ticks;
                
                // if two consecutive interrupts are very closed (inside DEBOUNCING_INTERVAL)
                // we need to filter with a debouncing
                if (ticks - prevPulseTicks < DEBOUNCING_INTERVAL * TimeSpan.TicksPerMillisecond)
                    return;
                else
                {
                    prevPulseTicks = ticks;
                    pulseCount++;
                }
            }
        }

        void CalculateWindSpeed(object state)
        {
            lock (this.lockObj)
            {
                this.WindSpeed = (this.referenceWindSpeed * ((float)pulseCount / this.calculatePeriod) * (1000 / this.referencePulseForSecond));
                pulseCount = 0;
            }
        }
    }
}