using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using MedEnthLogsApi;

namespace MedEnthLogsDesktop
{
    public class Win32LocationDetector : ILocationDetector
    {
        // -------- Fields --------

        /// <summary>
        /// The class that watches the location.
        /// </summary>
        private GeoCoordinateWatcher geoWatcher;

        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public Win32LocationDetector()
        {
            this.geoWatcher = new GeoCoordinateWatcher();
            this.IsReady = false;
            this.Latitude = 0.0M;
            this.Longitude = 0.0M;
        }

        // -------- Properties --------

        /// <summary>
        /// Latitude Position.
        /// </summary>
        public decimal Latitude { get; private set; }

        /// <summary>
        /// Longitude Position.
        /// </summary>
        public decimal Longitude { get; private set; }

        /// <summary>
        /// True if we have a valid position, else false.
        /// </summary>
        public bool IsReady { get; private set; }

        // -------- Functions --------

        /// <summary>
        /// Refreshes the position information.
        /// </summary>
        /// <returns>True if we got a position.</returns>
        public bool RefreshPosition()
        {
            for ( int i = 0; ( i < 3 ) && ( IsReady == false ); ++i )
            {
                this.geoWatcher.TryStart( false, TimeSpan.FromMilliseconds( 1000 ) );
                GeoCoordinate coord = geoWatcher.Position.Location;

                if ( coord.IsUnknown )
                {
                    this.IsReady = false;
                }
                else
                {
                    this.Latitude = Convert.ToDecimal( coord.Latitude );
                    this.Longitude = Convert.ToDecimal( coord.Longitude );
                    this.IsReady = true;
                }
            }
            return this.IsReady;
        }
    }
}
