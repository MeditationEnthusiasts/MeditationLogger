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
using System.IO;
using Newtonsoft.Json.Linq;
using SQLite.Net;

namespace MeditationEnthusiasts.MeditationLogger.Api
{
    public class JsonExporter
    {
        /// <summary>
        /// Exports the given logbook to json.
        /// </summary>
        /// <param name="outFile">Where to write the json to.</param>
        /// <param name="logBook">The logbook to convert to json.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public static void ExportJson( Stream outFile, LogBook logBook, Action<int, int> onStep = null )
        {
            using( StreamWriter writer = new StreamWriter( outFile ) )
            {
                writer.WriteLine( ExportJsonToString( logBook, onStep ) );
            }
        }

        /// <summary>
        /// Exports the given logbook to json in the form of a string.
        /// </summary>
        /// <param name="logBook">The logbook to convert to json.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        /// <returns>Raw Json in the form of a string.</returns>
        public static string ExportJsonToString( LogBook logBook, Action<int, int> onStep = null )
        {
            JArray array = new JArray();
            for( int i = 0; i < logBook.Logs.Count; ++i )
            {
                JObject o = new JObject();
                o[Log.GuidString] = logBook.Logs[i].Guid.ToString();
                o[Log.EditTimeString] = logBook.Logs[i].EditTime.ToString( "o" );
                o[Log.StartTimeString] = logBook.Logs[i].StartTime.ToString( "o" );
                o[Log.EndTimeString] = logBook.Logs[i].EndTime.ToString( "o" );
                o[Log.TechniqueString] = logBook.Logs[i].Technique;
                o[Log.CommentsString] = logBook.Logs[i].Comments;
                o[Log.LatitudeString] = logBook.Logs[i].Latitude.HasValue ? logBook.Logs[i].Latitude.ToString() : string.Empty;
                o[Log.LongitudeString] = logBook.Logs[i].Longitude.HasValue ? logBook.Logs[i].Longitude.ToString() : string.Empty;

                array.Add( o );

                if( onStep != null )
                {
                    onStep( i + 1, logBook.Logs.Count );
                }
            }

            return array.ToString();
        }

        /// <summary>
        /// Imports logs from JSON to the database.
        /// This will not repopulate the logbook itself.  You must call PopulateLogbook() to do that.
        /// </summary>
        /// <param name="outFile">The stream to read from.</param>
        /// <param name="logBook">The logbook to import to.</param>
        /// <param name="sqlite">The sqlite connection to import the logs to.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public static void ImportFromJson( Stream outFile, LogBook logBook, SQLiteConnection sqlite, Action<int, int> onStep = null )
        {
            List<Log> logs = new List<Log>();

            using( StreamReader reader = new StreamReader( outFile ) )
            {
                string json = reader.ReadToEnd();
                JArray array = JArray.Parse( json );

                int size = array.Count;
                int step = 1;
                foreach( JObject o in array.Children() )
                {
                    Log log = new Log();

                    JToken token;

                    // Get the start time.
                    if( o.TryGetValue( Log.StartTimeString, out token ) )
                    {
                        // ToObject will create the DateTime object for us.
                        log.StartTime = token.ToObject<DateTime>();
                    }

                    // Get the End time.
                    if( o.TryGetValue( Log.EndTimeString, out token ) )
                    {
                        // ToObject will create the DateTime object for us.
                        log.EndTime = token.ToObject<DateTime>();
                    }

                    // Get the technique
                    if( o.TryGetValue( Log.TechniqueString, out token ) )
                    {
                        log.Technique = token.ToString();
                    }

                    // Get the comments
                    if( o.TryGetValue( Log.CommentsString, out token ) )
                    {
                        log.Comments = token.ToString();
                    }

                    // Get the Latitude
                    if( o.TryGetValue( Log.LatitudeString, out token ) )
                    {
                        // Try to parse the Latitude.  If fails, just make it empty.
                        decimal lat;
                        if( decimal.TryParse( token.ToString(), out lat ) )
                        {
                            log.Latitude = lat;
                        }
                        else
                        {
                            log.Latitude = null;
                        }
                    }

                    // Get the Longitude
                    if( o.TryGetValue( Log.LongitudeString, out token ) )
                    {
                        // Try to parse the Longitude.  If fails, just make it empty.
                        decimal lon;
                        if( decimal.TryParse( token.ToString(), out lon ) )
                        {
                            log.Longitude = lon;
                        }
                        else
                        {
                            log.Longitude = null;
                        }
                    }

                    // We ignore GUID and Edit time in the file,
                    // and create them here.
                    Guid guid = Guid.NewGuid();

                    // Keep looking until we have a unique guid.
                    while( logBook.LogExists( guid ) || ( logs.Find( i => i.Guid == guid ) != null ) )
                    {
                        guid = Guid.NewGuid();
                    }
                    log.Guid = guid;
                    log.EditTime = DateTime.Now;

                    log.Validate();
                    logs.Add( log );

                    if( onStep != null )
                    {
                        onStep( step++, size );
                    }
                } // End foreach
            } // End using

            // Last thing to do is add the new logs to the database.
            if( logs.Count != 0 )
            {
                foreach( Log newLog in logs )
                {
                    sqlite.Insert( newLog );
                }

                sqlite.Commit();
            }
        }
    }
}