// 
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
        /// <param name="outFile">The stream the outputs the file.</param>
        /// <param name="logBook">The logbook to import to.</param>
        /// <param name="sqlite">The sqlite connection to import the logs to.</param>
        public static void ImportFromXml( Stream outFile, LogBook logBook, SQLiteConnection sqlite )
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

            foreach ( XmlNode node in rootNode.ChildNodes )
            {
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
                        case ( "StartTime" ):
                            log.StartTime = DateTime.Parse( attr.Value );
                            break;

                        case ( "EndTime" ):
                            log.EndTime = DateTime.Parse( attr.Value );
                            break;

                        case ( "Technique" ):
                            log.Technique = attr.Value;
                            break;

                        case ( "Comments" ):
                            log.Comments = attr.Value;
                            break;

                        case ( "Latitude" ):
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

                        case ( "Longitude" ):
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

                DateTime creationTime = DateTime.Now.ToUniversalTime();

                // Keep looking until we have a unique creation date.
                while ( logBook.LogExists( creationTime ) || ( logs.Find( i => i.CreateTime == creationTime ) != null ) )
                {
                    creationTime = DateTime.Now.ToUniversalTime();
                }
                log.CreateTime = creationTime;
                log.EditTime = creationTime;

                // Ensure the log is good.
                log.Validate();

                // Add to list.
                logs.Add( log );
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
        public static void ExportToXml( Stream outFile, LogBook logbook )
        {
            XmlDocument doc = new XmlDocument();

            // Create declaration.
            XmlDeclaration dec = doc.CreateXmlDeclaration( "1.0", "UTF-8", null );

            // Add declaration to document.
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore( dec, root );

            XmlNode logbookNode = doc.CreateNode( XmlNodeType.Element, "logbook", xmlNameSpace );

            foreach ( Log log in logbook.Logs )
            {
                XmlNode logNode = doc.CreateNode( XmlNodeType.Element, "log", xmlNameSpace );

                // Reducing scope so I don't accidently add the wrong attribute.
                {
                    XmlAttribute creationTime = doc.CreateAttribute( "CreationTime" );
                    creationTime.Value = log.CreateTime.ToString( "o" );
                    logNode.Attributes.Append( creationTime );
                }
                {
                    XmlAttribute editTime = doc.CreateAttribute( "EditTime" );
                    editTime.Value = log.EditTime.ToString( "o" );
                    logNode.Attributes.Append( editTime );
                }
                {
                    XmlAttribute startTime = doc.CreateAttribute( "StartTime" );
                    startTime.Value = log.StartTime.ToString( "o" );
                    logNode.Attributes.Append( startTime );
                }
                {
                    XmlAttribute endTime = doc.CreateAttribute( "EndTime" );
                    endTime.Value = log.EndTime.ToString( "o" );
                    logNode.Attributes.Append( endTime );
                }
                {
                    XmlAttribute technique = doc.CreateAttribute( "Technique" );
                    technique.Value = log.Technique;
                    logNode.Attributes.Append( technique );
                }
                {
                    XmlAttribute comments = doc.CreateAttribute( "Comments" );
                    comments.Value = log.Comments;
                    logNode.Attributes.Append( comments );
                }
                {
                    XmlAttribute lat = doc.CreateAttribute( "Latitude" );
                    lat.Value = log.Latitude.HasValue ? log.Latitude.ToString() : string.Empty;
                    logNode.Attributes.Append( lat );
                }
                {
                    XmlAttribute lon = doc.CreateAttribute( "Longitude" );
                    lon.Value = log.Longitude.HasValue ? log.Longitude.ToString() : string.Empty;
                    logNode.Attributes.Append( lon );
                }

                logbookNode.AppendChild( logNode );
            }

            doc.InsertAfter( logbookNode, dec );
            doc.Save( outFile );
        }
    }
}
