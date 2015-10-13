using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedEnthLogsApi;

namespace Test.TestFiles
{
    public class MockTimer : ITimer
    {
        // -------- Properties --------

        /// <summary>
        /// Action that gets fired each time the timer ticks.
        /// The given string is the time remaining in an
        /// easy to read format.
        /// </summary>
        public Action<string> OnUpdate { get; set; }

        /// <summary>
        /// Fired when the timer is completed (reaches zero).
        /// </summary>
        public Action OnComplete { get; set; }

        /// <summary>
        /// Whether or not the timer is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MockTimer()
        {
            this.IsRunning = false;
        }

        // -------- Functions --------

        /// <summary>
        /// Starts the timer.
        /// No-op if started.
        /// </summary>
        /// <param name="countDownTime">How long to time for.  Null for count up.</param>
        public void StartTimer( TimeSpan? countDownTime )
        {
            this.IsRunning = true;
        }

        /// <summary>
        /// Stops and resets the timer.
        /// No-op if not started.
        /// </summary>
        public void StopAndResetTimer()
        {
            this.IsRunning = false;
        }
    }
}
