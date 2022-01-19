using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronometer
{
    public class Chronometer : IChronometer
    {
        private Stopwatch stopWatch;

        private readonly List<string> laps;

        public Chronometer()
        {
            stopWatch = new Stopwatch();
            laps = new List<string>();
        }

        public string GetTime => stopWatch.Elapsed.ToString(@"mm\:ss\.ffff");

        public List<string> Laps => laps;

        public string Lap()
        {
            string time = GetTime;
            laps.Add(time);

            return time;
        }

        public void Reset()
        {
            stopWatch.Reset();
            laps.Clear();
        }

        public void Start()
        {
            stopWatch.Start();
        }

        public void Stop()
        {
            stopWatch.Stop();
        }
    }
}
