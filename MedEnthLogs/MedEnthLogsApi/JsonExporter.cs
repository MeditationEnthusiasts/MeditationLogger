﻿// 
// Meditation Logger.
// Copyright (C) 2015  Seth Hendrick.
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
using System.IO;
using Newtonsoft.Json.Linq;
using SQLite.Net;

namespace MedEnthLogsApi
{
    public class JsonExporter
    {
        /// <summary>
        /// Exports the given logbook to json.
        /// </summary>
        /// <param name="outFile">Where to write the json to.</param>
        /// <param name="logBook">The logbook to convert to json.</param>
        public static void ExportJson( Stream outFile, LogBook logBook )
        {
            using ( StreamWriter writer = new StreamWriter( outFile ) )
            {
                JArray array = new JArray();
                foreach ( Log log in logBook.Logs )
                {
                    JObject o = new JObject();
                    o[Log.CreationTimeString] = log.CreationTime.ToString( "o" );
                    o[Log.EditTimeString] = log.EditTime.ToString( "o" );
                    o[Log.StartTimeString] = log.StartTime.ToString( "o" );
                    o[Log.EndTimeString] = log.EndTime.ToString( "o" );
                    o[Log.TechniqueString] = log.Technique;
                    o[Log.CommentsString] = log.Comments;
                    o[Log.LatitudeString] = log.Latitude.HasValue ? log.Latitude.ToString() : string.Empty;
                    o[Log.LongitudeString] = log.Longitude.HasValue ? log.Longitude.ToString() : string.Empty;

                    array.Add( o );
                }
                writer.WriteLine( array.ToString() );
            }
        }

        /// <summary>
        /// Imports logs from JSON to the database.
        /// This will not repopulate the logbook itself.  You must call PopulateLogbook() to do that.
        /// </summary>
        /// <param name="outFile">The stream to read from.</param>
        /// <param name="logBook">The logbook to import to.</param>
        /// <param name="sqlite">The sqlite connection to import the logs to.</param>
        public static void ImportFromJson( Stream outFile, LogBook logBook, SQLiteConnection sqlite )
        {
            List<Log> logs = new List<Log>();

            using ( StreamReader reader = new StreamReader( outFile ) )
            {
                string json = reader.ReadToEnd();
                JArray array = JArray.Parse( json );
                foreach ( JObject o in array.Children() )
                {
                    Log log = new Log();

                    JToken token;

                    // Get the start time.
                    if ( o.TryGetValue( Log.StartTimeString, out token ) )
                    {
                        // ToObject will create the DateTime object for us.
                        log.StartTime = token.ToObject<DateTime>();
                    }

                    // Get the End time.
                    if ( o.TryGetValue( Log.EndTimeString, out token ) )
                    {
                        // ToObject will create the DateTime object for us.
                        log.EndTime = token.ToObject<DateTime>();
                    }

                    // Get the technique
                    if ( o.TryGetValue( Log.TechniqueString, out token ) )
                    {
                        log.Technique = token.ToString();
                    }

                    // Get the comments
                    if ( o.TryGetValue( Log.CommentsString, out token ) )
                    {
                        log.Comments = token.ToString();
                    }

                    // Get the Latitude
                    if ( o.TryGetValue( Log.LatitudeString, out token ) )
                    {
                        // Try to parse the Latitude.  If fails, just make it empty.
                        decimal lat;
                        if ( decimal.TryParse( token.ToString(), out lat ) )
                        {
                            log.Latitude = lat;
                        }
                        else
                        {
                            log.Latitude = null;
                        }
                    }

                    // Get the Longitude
                    if ( o.TryGetValue( Log.LongitudeString, out token ) )
                    {
                        // Try to parse the Longitude.  If fails, just make it empty.
                        decimal lon;
                        if ( decimal.TryParse( token.ToString(), out lon ) )
                        {
                            log.Longitude = lon;
                        }
                        else
                        {
                            log.Longitude = null;
                        }
                    }

                    DateTime creationTime = DateTime.Now.ToUniversalTime();

                    // Keep looking until we have a unique creation date.
                    while ( logBook.LogExists( creationTime ) || ( logs.Find( i => i.CreationTime == creationTime ) != null ) )
                    {
                        creationTime = DateTime.Now.ToUniversalTime();
                    }
                    log.CreationTime = creationTime;
                    log.EditTime = creationTime;

                    log.Validate();
                    logs.Add( log );
                } // End foreach
            } // End using

            // Last thing to do is add the new logs to the database.
            if ( logs.Count != 0 )
            {
                foreach ( Log newLog in logs )
                {
                    sqlite.Insert( newLog );
                }

                sqlite.Commit();
            }
        }
    }
}