// 
// Meditation Logger.
// Copyright (C) 2015-2017  Seth Hendrick.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace MeditationEnthusiasts.MeditationLogger.Api
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
