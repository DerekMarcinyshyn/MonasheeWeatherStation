using System;
using Microsoft.SPOT;

namespace MonasheeWeatherStation
{
    public class TimeCounter
    {
        TimeSpan elapsed = TimeSpan.Zero;
        DateTime timeStart, timeEnd;

        public TimeCounter()
        {
            timeStart = DateTime.Now;
            timeEnd = DateTime.Now;
        }

        public void Start()
        {
            timeStart = DateTime.Now;
        }

        public void Stop()
        {
            timeEnd = DateTime.Now;
            elapsed = timeEnd - timeStart;
        }

        public TimeSpan Elapsed
        {
            get
            {
                return elapsed;
            }
        }
    }
}