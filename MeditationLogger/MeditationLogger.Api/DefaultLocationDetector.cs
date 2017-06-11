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
using System.Net;
using System.Xml;

namespace MeditationEnthusiasts.MeditationLogger.Api
{
    /// <summary>
    /// Default location detector (works on all platforms).
    /// Gets our location via HTTP.
    /// </summary>
    public class DefaultLocationDetector : ILocationDetector
    {
        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public DefaultLocationDetector()
        {
            this.IsReady = false;
            this.Latitude = 0.0M;
            this.Longitude = 0.0M;
        }

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
        /// It does this by querying http://geoip.ubuntu.com/lookup
        /// </summary>
        /// <returns>True if we got a position.</returns>
        public bool RefreshPosition()
        {
            // We need to to a get request to http://geoip.ubuntu.com/lookup.  We need
            // and external server to get our IP information, and therefore our geo location.
            // The respose is in XML format.  We just need to get the latitude and longitude values from it.

            WebRequest request = WebRequest.Create( "http://geoip.ubuntu.com/lookup" );
            request.Method = "GET";

            using( HttpWebResponse response = request.GetResponse() as HttpWebResponse )
            {
                if( response.StatusCode != HttpStatusCode.OK )
                {
                    throw new ApplicationException(
                        "Could not get location. " + Environment.NewLine +
                         "HTTP request to http://geoip.ubuntu.com/lookup returned invalid status: " + response.StatusCode
                    );
                }

                XmlDocument doc = new XmlDocument();
                doc.Load( response.GetResponseStream() );

                foreach( XmlNode node in doc.ChildNodes )
                {
                    if( node.Name == "Latitude" )
                    {
                        this.Latitude = decimal.Parse( node.Value );
                    }
                    else if( node.Name.ToLower() == "Longitude" )
                    {
                        this.Longitude = decimal.Parse( node.Value );
                    }
                }
            }

            // If we make this far, we are ready.
            this.IsReady = true;
            return this.IsReady;
        }
    }
}
