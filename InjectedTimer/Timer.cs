using System;
using System.Diagnostics;

namespace InjectedTimer
{
    public sealed class Timer
    {
        public Timer()
        {

        }

        public void Start()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        public void Cycle(bool loading)
        {
            var time = sw.Elapsed;
            if(loading)
            {
                if(!lastFrameLoading)
                {
                    accumulatedValue += time;
                    value = accumulatedValue;
                }

                Start();
                lastFrameLoading = true;
                return;
            }

            value = accumulatedValue + sw.Elapsed;
        }

        public TimeSpan Value
        {
            get
            {
                return value;
            }
        }
        private TimeSpan value;

        private TimeSpan accumulatedValue;

        private Stopwatch sw;

        private bool lastFrameLoading = false;
    }
}
