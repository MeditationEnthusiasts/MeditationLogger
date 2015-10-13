using System;
using System.Collections.Generic;
using System.Text;

namespace MedEnthLogsApi
{
    /// <summary>
    /// A countdown or countup timer interface.
    /// </summary>
    public interface ITimer
    {
        // -------- Properties --------

        /// <summary>
        /// Action that gets fired each time the timer ticks.
        /// The given string is the time remaining in an
        /// easy to read format.
        /// </summary>
        Action<string> OnUpdate { get; set; }

        /// <summary>
        /// Fired when the timer is completed (reaches zero).
        /// </summary>
        Action OnComplete { get; set; }

        /// <summary>
        /// Whether or not the timer is currently running.
        /// </summary>
        bool IsRunning { get; }

        // -------- Functions --------

        /// <summary>
        /// Starts the timer.
        /// No-op if started.
        /// </summary>
        /// <param name="countDownTime">How long to time for.  Null for count up.</param>
        void StartTimer( TimeSpan? countDownTime );

        /// <summary>
        /// Stops and resets the timer.
        /// No-op if not started.
        /// </summary>
        void StopAndResetTimer();
    }
}
