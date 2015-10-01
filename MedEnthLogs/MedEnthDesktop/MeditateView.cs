using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedEnthDesktop
{
    public partial class MeditateView : UserControl
    {
        // -------- Fields --------

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
        /// Fires when the timer expires.
        /// </summary>
        private Action onExpire;

        /// <summary>
        /// Whether or not to count up or count down.
        /// </summary>
        private bool countUp;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onExpire">The action to take when the timer expires.</param>
        public MeditateView( Action onExpire )
        {
            InitializeComponent();

            if ( onExpire == null )
            {
                throw new ArgumentNullException( "onExpire" );
            }
            this.onExpire = onExpire;

            this.timer = new Timer();
            StopAndResetTimer();

            this.timer.Interval = 1000;
            this.timer.Tick += Timer_Tick;
        }

        // -------- Functions --------

        /// <summary>
        /// Launches every second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick( object sender, EventArgs e )
        {
            if ( countUp )
            {
                this.currentTime = this.currentTime.Add( increment );
            }
            else
            {
                this.currentTime = this.currentTime.Subtract( increment );
            }
            this.TimerLabel.Text = this.currentTime.ToString( "c" );

            if ( this.currentTime <= TimeSpan.Zero )
            {
                onExpire();
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="countDownTime">How long to time for.  Null for count up.</param>
        public void StartTimer( TimeSpan? countDownTime )
        {
            this.currentTime = countDownTime ?? TimeSpan.Zero;
            this.TimerLabel.Text = this.currentTime.ToString( "c" );
            this.countUp = ( countDownTime.HasValue == false );
            this.timer.Start();
        }

        /// <summary>
        /// Stops and resets the timer.
        /// </summary>
        public void StopAndResetTimer()
        {
            this.timer.Stop();
            this.currentTime = new TimeSpan( 0, 0, 0 );
            this.TimerLabel.Text = this.currentTime.ToString( "c" );
        }
    }
}
