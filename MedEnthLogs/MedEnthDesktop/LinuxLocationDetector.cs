using MedEnthLogsApi;
using System.Net;
using System;
using System.Xml;

namespace MedEnthLogsDesktop
{
    public class LinuxLocationDetector : ILocationDetector
    {
        // -------- Constructor --------

        /// <summary>
        /// Constructor
        /// </summary>
        public LinuxLocationDetector()
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

            using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse )
            {
                if ( response.StatusCode != HttpStatusCode.OK )
                {
                    throw new ApplicationException(
                        "Could not get location. " + Environment.NewLine + 
                         "HTTP request to http://geoip.ubuntu.com/lookup returned invalid status: " + response.StatusCode
                    );
                }

                XmlDocument doc = new XmlDocument ();
                doc.Load ( response.GetResponseStream() );

                foreach ( XmlNode node in doc.ChildNodes )
                {
                    if ( node.Name == "Latitude" )
                    {
                        this.Latitude = decimal.Parse( node.Value );
                    }
                    else if ( node.Name.ToLower() == "Longitude" )
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
