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
using System.Timers;
using MeditationEnthusiasts.MeditationLogger.Api;

namespace MeditationEnthusiasts.MeditationLogger.Desktop
{
    public class Win32Timer : ITimer
    {
        // -------- Fields --------

        /// <summary>
        /// Whether or not to count up or count down.
        /// </summary>
        private bool countUp;

        /// <summary>
        /// The timer to fire events.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// The current time of the timer.
        /// </summary>
        private TimeSpan currentTime;

        /// <summary>
        /// How much to increment (or decrement) the timer.
        /// </summary>
        private static TimeSpan increment = new TimeSpan( 0, 0, 1 );

        /// <summary>
        /// Action that gets fired each time the timer ticks.
        /// The given string is the time remaining in an
        /// easy to read format.
        /// </summary>
        private Action<string> onUpdate;

        /// <summary>
        /// Fired when the timer is completed (reaches zero).
        /// </summary>
        public Action onComplete;

        //  -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public Win32Timer()
        {
            this.timer = new Timer();
            this.IsRunning = false;

            this.timer.Interval = 1000; // 1 second
            this.timer.Elapsed += Timer_Tick;
        }

        // -------- Properties --------

        /// <summary>
        /// Action that gets fired each time the timer ticks.
        /// The given string is the time remaining in an
        /// easy to read format.
        /// </summary>
        public Action<string> OnUpdate
        {
            get
            {
                return onUpdate;
            }
            set
            {
                if( value == null )
                {
                    throw new ArgumentNullException( "OnUpdate can not be null" );
                }
                this.onUpdate = value;
            }
        }

        /// <summary>
        /// Fired when the timer is completed (reaches zero).
        /// </summary>
        public Action OnComplete
        {
            get
            {
                return onComplete;
            }
            set
            {
                if( value == null )
                {
                    throw new ArgumentNullException( "OnComplete can not be null" );
                }
                this.onComplete = value;
            }
        }

        /// <summary>
        /// Whether or not the timer is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        // -------- Functions --------

        /// <summary>
        /// Starts the timer.
        /// OnComplete and OnUpdate must not be null.
        /// </summary>
        /// <param name="countDownTime">How long to time for.  Null for count up.</param>
        public void StartTimer( TimeSpan? countDownTime )
        {
            if( ( OnUpdate == null ) || ( OnComplete == null ) )
            {
                throw new InvalidOperationException(
                    "OnUpdate and OnComplete must contain values before starting the timer."
                );
            }

            if( this.IsRunning == false )
            {
                this.currentTime = countDownTime ?? TimeSpan.Zero;
                OnUpdate( this.currentTime.ToString( "c" ) );
                this.countUp = ( countDownTime.HasValue == false );
                this.timer.Start();
                this.IsRunning = true;
            }
        }

        /// <summary>
        /// Stops and resets the timer.
        /// </summary>
        public void StopAndResetTimer()
        {
            if( this.IsRunning )
            {
                this.timer.Stop();
                this.currentTime = new TimeSpan( 0, 0, 0 );
                OnUpdate( this.currentTime.ToString( "c" ) );
                this.IsRunning = false;
            }
        }

        /// <summary>
        /// Launches every second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick( object sender, EventArgs e )
        {
            if( countUp )
            {
                this.currentTime = this.currentTime.Add( increment );
            }
            else
            {
                this.currentTime = this.currentTime.Subtract( increment );
            }

            OnUpdate( this.currentTime.ToString( "c" ) );

            if( this.currentTime <= TimeSpan.Zero )
            {
                OnComplete();
            }
        }
    }
}