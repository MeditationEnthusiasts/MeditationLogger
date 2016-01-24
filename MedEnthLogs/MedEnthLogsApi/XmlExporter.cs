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

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SQLite.Net;

namespace MedEnthLogsApi
{
    public class XmlExporter
    {
        const string xmlNameSpace = "http://app.meditationenthusiasts.org/schemas/logs/2015/LogXmlSchema.xsd";

        /// <summary>
        /// Imports logs from XML to the database.
        /// This will not repopulate the logbook itself.  You must call PopulateLogbook() to do that.
        /// </summary>
        /// <param name="outFile">The stream to read the XML from.</param>
        /// <param name="logBook">The logbook to import to.</param>
        /// <param name="sqlite">The sqlite connection to import the logs to.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public static void ImportFromXml( Stream outFile, LogBook logBook, SQLiteConnection sqlite, Action<int, int> onStep = null )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( outFile );

            List<Log> logs = new List<Log>();

            XmlElement rootNode = doc.DocumentElement;
            if ( rootNode.Name != "logbook" )
            {
                throw new XmlException(
                    "Root node should be named \"logbook\".  Got: " + rootNode.Name
                );
            }

            for( int i = 0; i < rootNode.ChildNodes.Count; ++i )
            {
                XmlNode node = rootNode.ChildNodes[i];

                if ( node.Name != "log" )
                {
                    throw new XmlException(
                        "Element is not a log.  Got: " + node.Name
                    );
                }

                Log log = new Log();

                foreach ( XmlAttribute attr in node.Attributes )
                {
                    switch ( attr.Name )
                    {
                        case ( Log.StartTimeString ):
                            log.StartTime = DateTime.Parse( attr.Value );
                            break;

                        case ( Log.EndTimeString ):
                            log.EndTime = DateTime.Parse( attr.Value );
                            break;

                        case ( Log.TechniqueString ):
                            log.Technique = attr.Value;
                            break;

                        case ( Log.CommentsString ):
                            log.Comments = attr.Value;
                            break;

                        case ( Log.LatitudeString ):
                            // Try to parse the latitude.  If fails, just make it empty.
                            decimal lat;
                            if ( decimal.TryParse( attr.Value, out lat ) )
                            {
                                log.Latitude = lat;
                            }
                            else
                            {
                                log.Latitude = null;
                            }
                            break;

                        case ( Log.LongitudeString ):
                            // Try to parse the Longitude.  If fails, just make it empty.
                            decimal lon;
                            if ( decimal.TryParse( attr.Value, out lon ) )
                            {
                                log.Longitude = lon;
                            }
                            else
                            {
                                log.Longitude = null;
                            }
                            break;
                    }
                }

                // We ignore GUID and Edit time in the file,
                // and create them here.
                Guid guid = Guid.NewGuid();

                // Keep looking until we have a unique guid.
                while ( logBook.LogExists( guid ) || ( logs.Find( l => l.Guid == guid ) != null ) )
                {
                    guid = Guid.NewGuid();
                }
                log.Guid = guid;
                log.EditTime = DateTime.Now.ToUniversalTime();

                // Ensure the log is good.
                log.Validate();

                // Add to list.
                logs.Add( log );

                if ( onStep != null )
                {
                    onStep( i + 1, rootNode.ChildNodes.Count );
                }
            }

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

        /// <summary>
        /// Exports the loaded logbook to XML.
        /// </summary>
        /// <param name="outFile">The stream which outputs the file.</param>
        /// <param name="logbook">The logbook to export from.</param>
        /// <param name="onStep">
        /// Action to take on each step during the process. Parameter 1 is the current step
        /// we are on.  Parameter 2 is the total number of steps the function will take.
        /// Null for no-op.
        /// </param>
        public static void ExportToXml( Stream outFile, LogBook logbook, Action<int, int> onStep = null )
        {
            XmlDocument doc = new XmlDocument();

            // Create declaration.
            XmlDeclaration dec = doc.CreateXmlDeclaration( "1.0", "UTF-8", null );

            // Add declaration to document.
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore( dec, root );

            XmlNode logbookNode = doc.CreateNode( XmlNodeType.Element, "logbook", xmlNameSpace );

            for ( int i = 0; i < logbook.Logs.Count; ++i )
            {
                XmlNode logNode = doc.CreateNode( XmlNodeType.Element, "log", xmlNameSpace );

                // Reducing scope so I don't accidently add the wrong attribute.

                // Add Guid
                {
                    XmlAttribute guid = doc.CreateAttribute( Log.GuidString );
                    guid.Value = logbook.Logs[i].Guid.ToString();
                    logNode.Attributes.Append( guid );
                }

                // Add Edit Time.
                {
                    XmlAttribute editTime = doc.CreateAttribute( Log.EditTimeString );
                    editTime.Value = logbook.Logs[i].EditTime.ToString( "o" );
                    logNode.Attributes.Append( editTime );
                }

                // Add Start Time.
                {
                    XmlAttribute startTime = doc.CreateAttribute( Log.StartTimeString );
                    startTime.Value = logbook.Logs[i].StartTime.ToString( "o" );
                    logNode.Attributes.Append( startTime );
                }

                // Add End time.
                {
                    XmlAttribute endTime = doc.CreateAttribute( Log.EndTimeString );
                    endTime.Value = logbook.Logs[i].EndTime.ToString( "o" );
                    logNode.Attributes.Append( endTime );
                }

                // Add technique.
                {
                    XmlAttribute technique = doc.CreateAttribute( Log.TechniqueString );
                    technique.Value = logbook.Logs[i].Technique;
                    logNode.Attributes.Append( technique );
                }

                // Add Comments
                {
                    XmlAttribute comments = doc.CreateAttribute( Log.CommentsString );
                    comments.Value = logbook.Logs[i].Comments;
                    logNode.Attributes.Append( comments );
                }

                // Add Latitude
                {
                    XmlAttribute lat = doc.CreateAttribute( Log.LatitudeString );
                    lat.Value = logbook.Logs[i].Latitude.HasValue ? logbook.Logs[i].Latitude.ToString() : string.Empty;
                    logNode.Attributes.Append( lat );
                }

                // Add Longitude.
                {
                    XmlAttribute lon = doc.CreateAttribute( Log.LongitudeString );
                    lon.Value = logbook.Logs[i].Longitude.HasValue ? logbook.Logs[i].Longitude.ToString() : string.Empty;
                    logNode.Attributes.Append( lon );
                }

                logbookNode.AppendChild( logNode );

                if ( onStep != null )
                {
                    onStep( i + 1, logbook.Logs.Count );
                }
            }

            doc.InsertAfter( logbookNode, dec );
            doc.Save( outFile );
        }
    }
}
