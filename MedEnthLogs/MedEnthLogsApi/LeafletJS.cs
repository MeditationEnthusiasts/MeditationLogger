// 
// Meditation Logger.
// Copyright (C) 2015-2016  Seth Hendrick.
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

using System.Globalization;

namespace MedEnthLogsApi
{
    /// <summary>
    /// This class is the CSS or JS for leaflet.JS.
    /// </summary>
    public static class LeafletJS
    {
        // -------- Functions --------

        /// <summary>
        /// Gets the html for all the leaflet.js's marker html.
        /// </summary>
        /// <returns>The HTML for all the leaflet.js's markers.  These should go in the head of the html page.</returns>
        public static string GetMarkerHtml( Api api )
        {
            string js = string.Empty;
            foreach ( ILog log in api.LogBook.Logs )
            {
                if ( ( log.Latitude == null ) || ( log.Longitude == null ) )
                {
                    continue;
                }

                // Replace new lines with spaces so the javascript doesn't get broken.
                string commentString = log.Comments.Replace( "\n", @"  " );

                js += @"
var markerHTML" + log.Id + @" = '<div class = ""left"" style=""overflow: auto; color: black; "">' + 
                                '<p><strong>" + log.StartTime.ToLocalTime().ToString( "MM-dd-yyyy HH:mm" ) + @"</strong></p>' + 
                                '<p><strong>Duration:</strong> " + log.Duration.TotalMinutes.ToString( "F", CultureInfo.InvariantCulture ) + @" minutes</p>' + 
                                '<p><strong>Technique:</strong> " + log.Technique + @"</p>' +
                                '<p><strong>Comments:</strong> " + commentString + @"</p>';

                var newPopup" + log.Id + @" = L.popup({maxwidth:500}).setContent(markerHTML" + log.Id + @");
var newMarker" + log.Id + @" = L.marker([" + log.Latitude + ", " + log.Longitude + @"]).setIcon(icon).addTo(map).bindPopup(newPopup" + log.Id + @");
";
            }

            return js;
        }
    }
}

